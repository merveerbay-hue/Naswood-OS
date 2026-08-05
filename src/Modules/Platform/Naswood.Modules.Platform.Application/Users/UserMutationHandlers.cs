using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Audit;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Contracts.Users;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.Application.Users;

public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUserManagementRepository _users;
    private readonly IOrganizationReferenceRepository _organization;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IUserHistoryRepository _history;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public UpdateUserCommandHandler(
        IUserManagementRepository users,
        IOrganizationReferenceRepository organization,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IUserHistoryRepository history,
        IAuthRequestContext context,
        IClock clock)
    {
        _users = users;
        _organization = organization;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _history = history;
        _context = context;
        _clock = clock;
    }

    public async Task<Result<UserDto>> HandleAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound());
        }

        if (!UserEmailValidator.IsValid(command.Email))
        {
            return Result.Failure<UserDto>(UserErrors.InvalidEmail());
        }

        if (await _users.EmailExistsAsync(command.Email.Trim(), user.Id, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<UserDto>(UserErrors.EmailTaken());
        }

        var companyIds = command.CompanyIds?.ToArray() ?? user.CompanyIds.ToArray();
        var plantIds = command.PlantIds?.ToArray() ?? user.PlantIds.ToArray();
        var department = command.DepartmentCode ?? user.DepartmentCode;
        var position = command.PositionCode ?? user.PositionCode;

        var org = await OrganizationValidator.ValidateAssignmentsAsync(
                _organization,
                companyIds,
                plantIds,
                department,
                position,
                cancellationToken)
            .ConfigureAwait(false);
        if (org.IsFailure)
        {
            return Result.Failure<UserDto>(org.Error!);
        }

        var profile = user.UpdateProfile(
            command.FirstName,
            command.LastName,
            command.Email,
            command.Phone,
            command.MobilePhone,
            command.CostCenter,
            command.HireDate,
            command.EmploymentType,
            command.EmployeeCategory,
            command.Language,
            command.TimeZone,
            command.DateFormat,
            command.NumberFormat,
            command.Currency,
            command.Theme,
            _context.UserId,
            _clock.UtcNow);
        if (profile.IsFailure)
        {
            return Result.Failure<UserDto>(profile.Error!);
        }

        var organization = user.AssignOrganization(
            companyIds,
            plantIds,
            department,
            position,
            command.ManagerUserId,
            _context.UserId,
            _clock.UtcNow);
        if (organization.IsFailure)
        {
            return Result.Failure<UserDto>(organization.Error!);
        }

        await PersistAsync(user, "UserUpdated", cancellationToken).ConfigureAwait(false);
        return Result.Success(UserDtoMapper.ToDto(user));
    }

    private async Task PersistAsync(AuthUser user, string action, CancellationToken cancellationToken)
    {
        await _history.AddAsync(
                new UserHistoryEntry
                {
                    UserId = user.Id,
                    ActorUserId = _context.UserId,
                    Action = action,
                    CorrelationId = _context.CorrelationId,
                    OccurredAt = _clock.UtcNow
                },
                cancellationToken)
            .ConfigureAwait(false);

        foreach (var domainEvent in user.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    user.Id,
                    _context.CorrelationId,
                    _clock.UtcNow,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        user.ClearDomainEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result>
{
    private readonly IUserManagementRepository _users;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IUserHistoryRepository _history;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public DeleteUserCommandHandler(
        IUserManagementRepository users,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IUserHistoryRepository history,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _history = history;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        var deleted = user.SoftDelete(_context.UserId, command.Reason, _clock.UtcNow);
        if (deleted.IsFailure)
        {
            return deleted;
        }

        await UserMutationSupport.PersistAsync(
                user,
                "UserDeleted",
                _history,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateUser(user.Id);
        return Result.Success();
    }
}

public sealed class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand, Result<UserDto>>
{
    private readonly UserLifecycleService _lifecycle;

    public ActivateUserCommandHandler(UserLifecycleService lifecycle) => _lifecycle = lifecycle;

    public Task<Result<UserDto>> HandleAsync(
        ActivateUserCommand command,
        CancellationToken cancellationToken = default) =>
        _lifecycle.ActivateAsync(command.UserId, cancellationToken);
}

public sealed class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand, Result<UserDto>>
{
    private readonly UserLifecycleService _lifecycle;

    public DeactivateUserCommandHandler(UserLifecycleService lifecycle) => _lifecycle = lifecycle;

    public Task<Result<UserDto>> HandleAsync(
        DeactivateUserCommand command,
        CancellationToken cancellationToken = default) =>
        _lifecycle.DeactivateAsync(command.UserId, cancellationToken);
}

public sealed class LockUserCommandHandler : ICommandHandler<LockUserCommand, Result<UserDto>>
{
    private readonly UserLifecycleService _lifecycle;

    public LockUserCommandHandler(UserLifecycleService lifecycle) => _lifecycle = lifecycle;

    public Task<Result<UserDto>> HandleAsync(
        LockUserCommand command,
        CancellationToken cancellationToken = default) =>
        _lifecycle.LockAsync(command.UserId, command.Reason, cancellationToken);
}

public sealed class UnlockUserCommandHandler : ICommandHandler<UnlockUserCommand, Result<UserDto>>
{
    private readonly UserLifecycleService _lifecycle;

    public UnlockUserCommandHandler(UserLifecycleService lifecycle) => _lifecycle = lifecycle;

    public Task<Result<UserDto>> HandleAsync(
        UnlockUserCommand command,
        CancellationToken cancellationToken = default) =>
        _lifecycle.UnlockAsync(command.UserId, cancellationToken);
}

public sealed class ResetUserPasswordCommandHandler : ICommandHandler<ResetUserPasswordCommand, Result>
{
    private readonly IUserManagementRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IUserHistoryRepository _history;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public ResetUserPasswordCommandHandler(
        IUserManagementRepository users,
        IPasswordHasher passwordHasher,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IUserHistoryRepository history,
        IAuthRequestContext context,
        IClock clock)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _history = history;
        _context = context;
        _clock = clock;
    }

    public async Task<Result> HandleAsync(
        ResetUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!UserPasswordPolicy.IsValid(command.NewPassword))
        {
            return Result.Failure(UserErrors.WeakPassword());
        }

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        var reset = user.ResetPassword(
            _passwordHasher.Hash(command.NewPassword),
            _context.UserId,
            _clock.UtcNow);
        if (reset.IsFailure)
        {
            return reset;
        }

        await UserMutationSupport.PersistAsync(
                user,
                "PasswordReset",
                _history,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken)
            .ConfigureAwait(false);
        return Result.Success();
    }
}

public sealed class AssignUserRolesCommandHandler : ICommandHandler<AssignUserRolesCommand, Result<UserDto>>
{
    private readonly IUserManagementRepository _users;
    private readonly IRoleCatalogRepository _roles;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IUserHistoryRepository _history;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public AssignUserRolesCommandHandler(
        IUserManagementRepository users,
        IRoleCatalogRepository roles,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IUserHistoryRepository history,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _users = users;
        _roles = roles;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _history = history;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<UserDto>> HandleAsync(
        AssignUserRolesCommand command,
        CancellationToken cancellationToken = default)
    {
        var roleCheck = await OrganizationValidator.ValidateRolesAsync(_roles, command.Roles, cancellationToken)
            .ConfigureAwait(false);
        if (roleCheck.IsFailure)
        {
            return Result.Failure<UserDto>(roleCheck.Error!);
        }

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound());
        }

        var assigned = user.AssignRoles(command.Roles, _context.UserId, _clock.UtcNow);
        if (assigned.IsFailure)
        {
            return Result.Failure<UserDto>(assigned.Error!);
        }

        await UserMutationSupport.PersistAsync(
                user,
                "RoleAssigned",
                _history,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateUser(user.Id);
        return Result.Success(UserDtoMapper.ToDto(user));
    }
}

public sealed class AssignUserPlantsCommandHandler : ICommandHandler<AssignUserPlantsCommand, Result<UserDto>>
{
    private readonly IUserManagementRepository _users;
    private readonly IOrganizationReferenceRepository _organization;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IUserHistoryRepository _history;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public AssignUserPlantsCommandHandler(
        IUserManagementRepository users,
        IOrganizationReferenceRepository organization,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IUserHistoryRepository history,
        IAuthRequestContext context,
        IClock clock)
    {
        _users = users;
        _organization = organization;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _history = history;
        _context = context;
        _clock = clock;
    }

    public async Task<Result<UserDto>> HandleAsync(
        AssignUserPlantsCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound());
        }

        var org = await OrganizationValidator.ValidateAssignmentsAsync(
                _organization,
                user.CompanyIds.ToArray(),
                command.PlantIds,
                user.DepartmentCode,
                user.PositionCode,
                cancellationToken)
            .ConfigureAwait(false);
        if (org.IsFailure)
        {
            return Result.Failure<UserDto>(org.Error!);
        }

        var assigned = user.AssignPlants(command.PlantIds, _context.UserId, _clock.UtcNow);
        if (assigned.IsFailure)
        {
            return Result.Failure<UserDto>(assigned.Error!);
        }

        await UserMutationSupport.PersistAsync(
                user,
                "PlantAssigned",
                _history,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken)
            .ConfigureAwait(false);
        return Result.Success(UserDtoMapper.ToDto(user));
    }
}

public sealed class ImportUsersCommandHandler : ICommandHandler<ImportUsersCommand, Result<UserImportResultDto>>
{
    private readonly IDispatcher _dispatcher;

    public ImportUsersCommandHandler(IDispatcher dispatcher) => _dispatcher = dispatcher;

    public async Task<Result<UserImportResultDto>> HandleAsync(
        ImportUsersCommand command,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<UserCsvRow> rows;
        try
        {
            rows = UserCsvMapper.Parse(command.CsvContent);
        }
        catch (FormatException ex)
        {
            return Result.Failure<UserImportResultDto>(UserErrors.Validation(ex.Message));
        }

        var created = 0;
        var errors = new List<string>();

        foreach (var row in rows)
        {
            var companies = row.Company.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var plants = row.Plant.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var roles = row.Roles.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var result = await _dispatcher.SendAsync(
                    new CreateUserCommand(
                        row.EmployeeNumber,
                        row.Username,
                        row.FirstName,
                        row.LastName,
                        row.Email,
                        row.Password,
                        companies,
                        plants,
                        roles,
                        row.Department,
                        row.Position,
                        null,
                        null),
                    cancellationToken)
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                created++;
            }
            else
            {
                errors.Add($"Line {row.LineNumber}: {result.Error!.Message}");
            }
        }

        return Result.Success(new UserImportResultDto
        {
            CreatedCount = created,
            FailedCount = errors.Count,
            Errors = errors
        });
    }
}

public sealed class UserLifecycleService
{
    private readonly IUserManagementRepository _users;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IUserHistoryRepository _history;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;
    private readonly IAuditWriter _audit;

    public UserLifecycleService(
        IUserManagementRepository users,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IUserHistoryRepository history,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache,
        IAuditWriter audit)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _history = history;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
        _audit = audit;
    }

    public Task<Result<UserDto>> ActivateAsync(Guid userId, CancellationToken cancellationToken) =>
        MutateAsync(userId, "UserActivated", u => u.Activate(_context.UserId, _clock.UtcNow), cancellationToken);

    public Task<Result<UserDto>> DeactivateAsync(Guid userId, CancellationToken cancellationToken) =>
        MutateAsync(userId, "UserDeactivated", u => u.Deactivate(_context.UserId, _clock.UtcNow), cancellationToken);

    public Task<Result<UserDto>> LockAsync(Guid userId, string reason, CancellationToken cancellationToken) =>
        MutateAsync(userId, "UserLocked", u => u.Lock(reason, _context.UserId, _clock.UtcNow), cancellationToken);

    public Task<Result<UserDto>> UnlockAsync(Guid userId, CancellationToken cancellationToken) =>
        MutateAsync(userId, "UserUnlocked", u => u.Unlock(_context.UserId, _clock.UtcNow), cancellationToken);

    private async Task<Result<UserDto>> MutateAsync(
        Guid userId,
        string action,
        Func<AuthUser, Result> mutate,
        CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound());
        }

        var result = mutate(user);
        if (result.IsFailure)
        {
            return Result.Failure<UserDto>(result.Error!);
        }

        await UserMutationSupport.PersistAsync(
                user,
                action,
                _history,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken,
                _audit)
            .ConfigureAwait(false);
        _permissionCache.InvalidateUser(user.Id);
        return Result.Success(UserDtoMapper.ToDto(user));
    }
}

internal static class UserMutationSupport
{
    public static async Task PersistAsync(
        AuthUser user,
        string action,
        IUserHistoryRepository history,
        IOutboxWriter outbox,
        IPlatformUnitOfWork unitOfWork,
        IAuthRequestContext context,
        IClock clock,
        CancellationToken cancellationToken,
        IAuditWriter? audit = null)
    {
        await history.AddAsync(
                new UserHistoryEntry
                {
                    UserId = user.Id,
                    ActorUserId = context.UserId,
                    Action = action,
                    CorrelationId = context.CorrelationId,
                    OccurredAt = clock.UtcNow
                },
                cancellationToken)
            .ConfigureAwait(false);

        if (audit is not null)
        {
            await audit.WriteAsync(
                    new Domain.Audit.AuditWriteModel
                    {
                        OccurredAt = clock.UtcNow,
                        UserId = context.UserId,
                        Module = "Administration",
                        Entity = "User",
                        EntityId = user.Id.ToString("D"),
                        Action = action,
                        CorrelationId = context.CorrelationId,
                        CompanyId = context.CompanyId,
                        PlantId = context.PlantId,
                        IpAddress = context.IpAddress,
                        SessionId = context.SessionId
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }

        foreach (var domainEvent in user.DomainEvents)
        {
            await outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    user.Id,
                    context.CorrelationId,
                    clock.UtcNow,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        user.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

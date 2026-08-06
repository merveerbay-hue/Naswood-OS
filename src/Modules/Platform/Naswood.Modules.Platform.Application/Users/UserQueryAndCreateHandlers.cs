using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Contracts.Users;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.Application.Users;

public sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, Result<PagedUsersDto>>
{
    private readonly IUserManagementRepository _users;

    public GetUsersQueryHandler(IUserManagementRepository users) => _users = users;

    public async Task<Result<PagedUsersDto>> HandleAsync(
        GetUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!UserStatusParser.TryParse(query.Status, out var status))
        {
            return Result.Failure<PagedUsersDto>(UserErrors.Validation("Invalid status filter."));
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var (items, total) = await _users.SearchAsync(
                new UserSearchCriteria(
                    query.EmployeeNumber,
                    query.Username,
                    query.Name,
                    query.Email,
                    query.DepartmentCode,
                    query.CompanyId,
                    query.PlantId,
                    status,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        var dto = new PagedUsersDto
        {
            Items = items.Select(u => new UserListItemDto
            {
                Id = u.Id,
                EmployeeNumber = u.EmployeeNumber,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Email = u.Email,
                DepartmentCode = u.DepartmentCode,
                CompanyIds = u.CompanyIds.ToArray(),
                PlantIds = u.PlantIds.ToArray(),
                Status = u.Status.ToString(),
                IsLocked = u.IsLocked
            }).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        };

        return Result.Success(dto);
    }
}

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserManagementRepository _users;

    public GetUserByIdQueryHandler(IUserManagementRepository users) => _users = users;

    public async Task<Result<UserDto>> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(query.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound());
        }

        return Result.Success(UserDtoMapper.ToDto(user));
    }
}

public sealed class ExportUsersQueryHandler : IQueryHandler<ExportUsersQuery, Result<string>>
{
    private readonly IUserManagementRepository _users;

    public ExportUsersQueryHandler(IUserManagementRepository users) => _users = users;

    public async Task<Result<string>> HandleAsync(
        ExportUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        var users = await _users.ListActiveForExportAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(UserCsvMapper.BuildExport(users));
    }
}

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserManagementRepository _users;
    private readonly IOrganizationReferenceRepository _organization;
    private readonly IRoleCatalogRepository _roles;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IUserHistoryRepository _history;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public CreateUserCommandHandler(
        IUserManagementRepository users,
        IOrganizationReferenceRepository organization,
        IRoleCatalogRepository roles,
        IPasswordHasher passwordHasher,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IUserHistoryRepository history,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _users = users;
        _organization = organization;
        _roles = roles;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _history = history;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<UserDto>> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateNewUserAsync(command, cancellationToken).ConfigureAwait(false);
        if (validation.IsFailure)
        {
            return Result.Failure<UserDto>(validation.Error!);
        }

        if (!UserPasswordPolicy.IsValid(command.Password))
        {
            return Result.Failure<UserDto>(UserErrors.WeakPassword());
        }

        var user = AuthUser.Register(
            command.Username,
            command.EmployeeNumber,
            command.FirstName,
            command.LastName,
            command.Email,
            _passwordHasher.Hash(command.Password),
            command.CompanyIds,
            command.PlantIds,
            command.Roles,
            command.DepartmentCode,
            command.PositionCode,
            _context.UserId);

        if (!string.IsNullOrWhiteSpace(command.Phone) || !string.IsNullOrWhiteSpace(command.MobilePhone))
        {
            var profile = user.UpdateProfile(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Phone,
                command.MobilePhone,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                _context.UserId,
                _clock.UtcNow);
            if (profile.IsFailure)
            {
                return Result.Failure<UserDto>(profile.Error!);
            }
        }

        await _users.AddAsync(user, cancellationToken).ConfigureAwait(false);
        await WriteAuditAsync(user.Id, "UserCreated", null, cancellationToken).ConfigureAwait(false);
        await EnqueueEventsAsync(user, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _permissionCache.InvalidateUser(user.Id);
        return Result.Success(UserDtoMapper.ToDto(user));
    }

    private async Task<Result> ValidateNewUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Username) ||
            string.IsNullOrWhiteSpace(command.EmployeeNumber) ||
            string.IsNullOrWhiteSpace(command.FirstName) ||
            string.IsNullOrWhiteSpace(command.LastName) ||
            string.IsNullOrWhiteSpace(command.Email))
        {
            return Result.Failure(UserErrors.Validation("Mandatory fields are incomplete."));
        }

        if (!UserEmailValidator.IsValid(command.Email))
        {
            return Result.Failure(UserErrors.InvalidEmail());
        }

        if (await _users.UsernameExistsAsync(command.Username.Trim(), null, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure(UserErrors.UsernameTaken());
        }

        if (await _users.EmailExistsAsync(command.Email.Trim(), null, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure(UserErrors.EmailTaken());
        }

        if (await _users.EmployeeNumberExistsAsync(command.EmployeeNumber.Trim(), null, cancellationToken)
                .ConfigureAwait(false))
        {
            return Result.Failure(UserErrors.EmployeeNumberTaken());
        }

        var org = await OrganizationValidator.ValidateAssignmentsAsync(
                _organization,
                command.CompanyIds,
                command.PlantIds,
                command.DepartmentCode,
                command.PositionCode,
                cancellationToken)
            .ConfigureAwait(false);
        if (org.IsFailure)
        {
            return org;
        }

        return await OrganizationValidator.ValidateRolesAsync(_roles, command.Roles, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task WriteAuditAsync(
        Guid userId,
        string action,
        string? details,
        CancellationToken cancellationToken)
    {
        await _history.AddAsync(
                new UserHistoryEntry
                {
                    UserId = userId,
                    ActorUserId = _context.UserId,
                    Action = action,
                    Details = details,
                    CorrelationId = _context.CorrelationId,
                    OccurredAt = _clock.UtcNow
                },
                cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task EnqueueEventsAsync(AuthUser user, CancellationToken cancellationToken)
    {
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
    }
}

internal static class OrganizationValidator
{
    public static async Task<Result> ValidateAssignmentsAsync(
        IOrganizationReferenceRepository organization,
        IReadOnlyList<string> companyIds,
        IReadOnlyList<string> plantIds,
        string? departmentCode,
        string? positionCode,
        CancellationToken cancellationToken)
    {
        if (companyIds.Count == 0 || plantIds.Count == 0)
        {
            return Result.Failure(UserErrors.Validation("Company and plant are required."));
        }

        foreach (var companyId in companyIds)
        {
            var company = await organization.GetCompanyByCodeAsync(companyId, cancellationToken).ConfigureAwait(false);
            if (company is null || !company.IsActive)
            {
                return Result.Failure(UserErrors.CompanyNotFound(companyId));
            }
        }

        foreach (var plantId in plantIds)
        {
            var plant = await organization.GetPlantByCodeAsync(plantId, cancellationToken).ConfigureAwait(false);
            if (plant is null || !plant.IsActive)
            {
                return Result.Failure(UserErrors.PlantNotFound(plantId));
            }

            if (!companyIds.Contains(plant.CompanyCode, StringComparer.OrdinalIgnoreCase) &&
                companyIds.Count > 0)
            {
                // Plant must belong to one of the assigned companies.
                var matched = false;
                foreach (var companyId in companyIds)
                {
                    if (string.Equals(plant.CompanyCode, companyId.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    return Result.Failure(UserErrors.PlantCompanyMismatch(plantId, plant.CompanyCode));
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(departmentCode))
        {
            var department = await organization.GetDepartmentByCodeAsync(departmentCode, cancellationToken)
                .ConfigureAwait(false);
            if (department is null || !department.IsActive)
            {
                return Result.Failure(UserErrors.DepartmentNotFound(departmentCode));
            }
        }

        if (!string.IsNullOrWhiteSpace(positionCode))
        {
            var position = await organization.GetPositionByCodeAsync(positionCode, cancellationToken)
                .ConfigureAwait(false);
            if (position is null || !position.IsActive)
            {
                return Result.Failure(UserErrors.PositionNotFound(positionCode));
            }
        }

        return Result.Success();
    }

    public static async Task<Result> ValidateRolesAsync(
        IRoleCatalogRepository roles,
        IReadOnlyList<string> roleCodes,
        CancellationToken cancellationToken)
    {
        if (roleCodes.Count == 0)
        {
            return Result.Failure(UserErrors.RoleRequired());
        }

        var found = await roles.GetByCodesAsync(roleCodes, cancellationToken).ConfigureAwait(false);
        foreach (var code in roleCodes)
        {
            if (!found.Any(r => string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase) && r.IsActive))
            {
                return Result.Failure(UserErrors.RoleNotFound(code));
            }
        }

        return Result.Success();
    }
}

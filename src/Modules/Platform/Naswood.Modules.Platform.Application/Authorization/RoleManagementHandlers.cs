using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Users;
using Naswood.Modules.Platform.Contracts.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.Application.Authorization;

public sealed class SearchRolesQueryHandler : IQueryHandler<SearchRolesQuery, Result<PagedRolesDto>>
{
    private readonly IRoleManagementRepository _roles;

    public SearchRolesQueryHandler(IRoleManagementRepository roles) => _roles = roles;

    public async Task<Result<PagedRolesDto>> HandleAsync(
        SearchRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _roles.SearchAsync(
                new RoleSearchCriteria(
                    query.Code,
                    query.Name,
                    query.CompanyCode,
                    query.PlantCode,
                    query.IsActive,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new PagedRolesDto
        {
            Items = items.Select(RoleDtoMapper.ToDetail).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, Result<RoleDetailDto>>
{
    private readonly IRoleManagementRepository _roles;

    public GetRoleByIdQueryHandler(IRoleManagementRepository roles) => _roles = roles;

    public async Task<Result<RoleDetailDto>> HandleAsync(
        GetRoleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(query.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NotFound());
        }

        return Result.Success(RoleDtoMapper.ToDetail(role));
    }
}

public sealed class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Result<RoleDetailDto>>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IPermissionCatalogRepository _permissions;
    private readonly IOrganizationReferenceRepository _organization;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public CreateRoleCommandHandler(
        IRoleManagementRepository roles,
        IPermissionCatalogRepository permissions,
        IOrganizationReferenceRepository organization,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _permissions = permissions;
        _organization = organization;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<RoleDetailDto>> HandleAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var validated = await RoleMutationGuard.ValidateNewAsync(
                command.Code,
                command.Name,
                command.CompanyCode,
                command.Permissions,
                _roles,
                _permissions,
                _organization,
                excludingRoleId: null,
                cancellationToken)
            .ConfigureAwait(false);
        if (validated.IsFailure)
        {
            return Result.Failure<RoleDetailDto>(validated.Error!);
        }

        var role = RoleDefinition.CreateManaged(
            command.Code,
            command.Name,
            command.Description,
            command.CompanyCode,
            command.PlantCode,
            command.DepartmentCode,
            command.Category,
            command.Permissions,
            _context.UserId);

        await _roles.AddAsync(role, cancellationToken).ConfigureAwait(false);
        await RoleMutationGuard.PersistAsync(role, _outbox, _unitOfWork, _context, _clock, cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success(RoleDtoMapper.ToDetail(role));
    }
}

public sealed class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, Result<RoleDetailDto>>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IPermissionCatalogRepository _permissions;
    private readonly IOrganizationReferenceRepository _organization;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public UpdateRoleCommandHandler(
        IRoleManagementRepository roles,
        IPermissionCatalogRepository permissions,
        IOrganizationReferenceRepository organization,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _permissions = permissions;
        _organization = organization;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<RoleDetailDto>> HandleAsync(
        UpdateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(command.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NotFound());
        }

        if (await _roles.NameExistsInCompanyAsync(
                    command.Name,
                    command.CompanyCode ?? role.CompanyCode,
                    role.Id,
                    cancellationToken)
                .ConfigureAwait(false))
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NameTaken());
        }

        if (!string.IsNullOrWhiteSpace(command.CompanyCode))
        {
            var company = await _organization.GetCompanyByCodeAsync(command.CompanyCode, cancellationToken)
                .ConfigureAwait(false);
            if (company is null)
            {
                return Result.Failure<RoleDetailDto>(RoleErrors.CompanyNotFound(command.CompanyCode));
            }
        }

        if (command.Permissions is not null)
        {
            var perms = await RoleMutationGuard.ValidatePermissionsAsync(
                    command.Permissions,
                    _permissions,
                    cancellationToken)
                .ConfigureAwait(false);
            if (perms.IsFailure)
            {
                return Result.Failure<RoleDetailDto>(perms.Error!);
            }
        }

        var updated = role.Update(
            command.Name,
            command.Description,
            command.CompanyCode,
            command.PlantCode,
            command.DepartmentCode,
            command.Category,
            command.Permissions,
            _context.UserId,
            _clock.UtcNow);
        if (updated.IsFailure)
        {
            return Result.Failure<RoleDetailDto>(updated.Error!);
        }

        await RoleMutationGuard.PersistAsync(role, _outbox, _unitOfWork, _context, _clock, cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success(RoleDtoMapper.ToDetail(role));
    }
}

public sealed class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand, Result>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public DeleteRoleCommandHandler(
        IRoleManagementRepository roles,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(command.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        var deleted = role.SoftDelete(_context.UserId, _clock.UtcNow);
        if (deleted.IsFailure)
        {
            return deleted;
        }

        await RoleMutationGuard.PersistAsync(role, _outbox, _unitOfWork, _context, _clock, cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success();
    }
}

public sealed class CloneRoleCommandHandler : ICommandHandler<CloneRoleCommand, Result<RoleDetailDto>>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public CloneRoleCommandHandler(
        IRoleManagementRepository roles,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock)
    {
        _roles = roles;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
    }

    public async Task<Result<RoleDetailDto>> HandleAsync(
        CloneRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var source = await _roles.GetByIdAsync(command.RoleId, cancellationToken).ConfigureAwait(false);
        if (source is null || source.IsDeleted)
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NotFound());
        }

        if (await _roles.CodeExistsAsync(command.Code, null, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.CodeTaken());
        }

        if (await _roles.NameExistsInCompanyAsync(command.Name, source.CompanyCode, null, cancellationToken)
                .ConfigureAwait(false))
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NameTaken());
        }

        var clone = source.Clone(command.Code, command.Name, _context.UserId);
        await _roles.AddAsync(clone, cancellationToken).ConfigureAwait(false);
        await RoleMutationGuard.PersistAsync(clone, _outbox, _unitOfWork, _context, _clock, cancellationToken)
            .ConfigureAwait(false);
        return Result.Success(RoleDtoMapper.ToDetail(clone));
    }
}

public sealed class ActivateRoleCommandHandler : ICommandHandler<ActivateRoleCommand, Result<RoleDetailDto>>
{
    private readonly RoleLifecycleService _lifecycle;

    public ActivateRoleCommandHandler(RoleLifecycleService lifecycle) => _lifecycle = lifecycle;

    public Task<Result<RoleDetailDto>> HandleAsync(
        ActivateRoleCommand command,
        CancellationToken cancellationToken = default) =>
        _lifecycle.ActivateAsync(command.RoleId, cancellationToken);
}

public sealed class DeactivateRoleCommandHandler : ICommandHandler<DeactivateRoleCommand, Result<RoleDetailDto>>
{
    private readonly RoleLifecycleService _lifecycle;

    public DeactivateRoleCommandHandler(RoleLifecycleService lifecycle) => _lifecycle = lifecycle;

    public Task<Result<RoleDetailDto>> HandleAsync(
        DeactivateRoleCommand command,
        CancellationToken cancellationToken = default) =>
        _lifecycle.DeactivateAsync(command.RoleId, cancellationToken);
}

public sealed class AssignRolePermissionsCommandHandler
    : ICommandHandler<AssignRolePermissionsCommand, Result<RoleDetailDto>>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IPermissionCatalogRepository _permissions;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public AssignRolePermissionsCommandHandler(
        IRoleManagementRepository roles,
        IPermissionCatalogRepository permissions,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _permissions = permissions;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<RoleDetailDto>> HandleAsync(
        AssignRolePermissionsCommand command,
        CancellationToken cancellationToken = default)
    {
        var perms = await RoleMutationGuard.ValidatePermissionsAsync(
                command.Permissions,
                _permissions,
                cancellationToken)
            .ConfigureAwait(false);
        if (perms.IsFailure)
        {
            return Result.Failure<RoleDetailDto>(perms.Error!);
        }

        var role = await _roles.GetByIdAsync(command.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NotFound());
        }

        var assigned = role.AssignPermissions(command.Permissions, _context.UserId, _clock.UtcNow);
        if (assigned.IsFailure)
        {
            return Result.Failure<RoleDetailDto>(assigned.Error!);
        }

        await RoleMutationGuard.PersistAsync(role, _outbox, _unitOfWork, _context, _clock, cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success(RoleDtoMapper.ToDetail(role));
    }
}

public sealed class RemoveRolePermissionsCommandHandler
    : ICommandHandler<RemoveRolePermissionsCommand, Result<RoleDetailDto>>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public RemoveRolePermissionsCommandHandler(
        IRoleManagementRepository roles,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<RoleDetailDto>> HandleAsync(
        RemoveRolePermissionsCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(command.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NotFound());
        }

        var removed = role.RemovePermissions(command.Permissions, _context.UserId, _clock.UtcNow);
        if (removed.IsFailure)
        {
            return Result.Failure<RoleDetailDto>(removed.Error!);
        }

        await RoleMutationGuard.PersistAsync(role, _outbox, _unitOfWork, _context, _clock, cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success(RoleDtoMapper.ToDetail(role));
    }
}

public sealed class AssignRoleToUserCommandHandler : ICommandHandler<AssignRoleToUserCommand, Result>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IUserManagementRepository _users;
    private readonly IUserHistoryRepository _history;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public AssignRoleToUserCommandHandler(
        IRoleManagementRepository roles,
        IUserManagementRepository users,
        IUserHistoryRepository history,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _users = users;
        _history = history;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result> HandleAsync(
        AssignRoleToUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(command.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        if (!role.IsActive)
        {
            return Result.Failure(RoleErrors.InactiveCannotAssign());
        }

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        var roles = user.Roles.Append(role.Code).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var assigned = user.AssignRoles(roles, _context.UserId, _clock.UtcNow);
        if (assigned.IsFailure)
        {
            return assigned;
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
        return Result.Success();
    }
}

public sealed class RemoveRoleFromUserCommandHandler : ICommandHandler<RemoveRoleFromUserCommand, Result>
{
    private readonly IRoleManagementRepository _roles;
    private readonly IUserManagementRepository _users;
    private readonly IUserHistoryRepository _history;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public RemoveRoleFromUserCommandHandler(
        IRoleManagementRepository roles,
        IUserManagementRepository users,
        IUserHistoryRepository history,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _users = users;
        _history = history;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result> HandleAsync(
        RemoveRoleFromUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(command.RoleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        var roles = user.Roles
            .Where(r => !string.Equals(r, role.Code, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (roles.Length == 0)
        {
            return Result.Failure(UserErrors.RoleRequired());
        }

        var assigned = user.AssignRoles(roles, _context.UserId, _clock.UtcNow);
        if (assigned.IsFailure)
        {
            return assigned;
        }

        await UserMutationSupport.PersistAsync(
                user,
                "RoleRemoved",
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

public sealed class RoleLifecycleService
{
    private readonly IRoleManagementRepository _roles;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public RoleLifecycleService(
        IRoleManagementRepository roles,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _roles = roles;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public Task<Result<RoleDetailDto>> ActivateAsync(Guid roleId, CancellationToken cancellationToken) =>
        MutateAsync(roleId, r => r.Activate(_context.UserId, _clock.UtcNow), cancellationToken);

    public Task<Result<RoleDetailDto>> DeactivateAsync(Guid roleId, CancellationToken cancellationToken) =>
        MutateAsync(roleId, r => r.Deactivate(_context.UserId, _clock.UtcNow), cancellationToken);

    private async Task<Result<RoleDetailDto>> MutateAsync(
        Guid roleId,
        Func<RoleDefinition, Result> mutate,
        CancellationToken cancellationToken)
    {
        var role = await _roles.GetByIdAsync(roleId, cancellationToken).ConfigureAwait(false);
        if (role is null || role.IsDeleted)
        {
            return Result.Failure<RoleDetailDto>(RoleErrors.NotFound());
        }

        var result = mutate(role);
        if (result.IsFailure)
        {
            return Result.Failure<RoleDetailDto>(result.Error!);
        }

        await RoleMutationGuard.PersistAsync(role, _outbox, _unitOfWork, _context, _clock, cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success(RoleDtoMapper.ToDetail(role));
    }
}

internal static class RoleMutationGuard
{
    public static async Task<Result> ValidateNewAsync(
        string code,
        string name,
        string? companyCode,
        IReadOnlyList<string> permissions,
        IRoleManagementRepository roles,
        IPermissionCatalogRepository permissionCatalog,
        IOrganizationReferenceRepository organization,
        Guid? excludingRoleId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(RoleErrors.Validation("Role code and name are required."));
        }

        if (await roles.CodeExistsAsync(code, excludingRoleId, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure(RoleErrors.CodeTaken());
        }

        if (await roles.NameExistsInCompanyAsync(name, companyCode, excludingRoleId, cancellationToken)
                .ConfigureAwait(false))
        {
            return Result.Failure(RoleErrors.NameTaken());
        }

        if (!string.IsNullOrWhiteSpace(companyCode))
        {
            var company = await organization.GetCompanyByCodeAsync(companyCode, cancellationToken)
                .ConfigureAwait(false);
            if (company is null)
            {
                return Result.Failure(RoleErrors.CompanyNotFound(companyCode));
            }
        }

        return await ValidatePermissionsAsync(permissions, permissionCatalog, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task<Result> ValidatePermissionsAsync(
        IReadOnlyList<string> permissions,
        IPermissionCatalogRepository permissionCatalog,
        CancellationToken cancellationToken)
    {
        if (permissions.Count == 0)
        {
            return Result.Success();
        }

        var catalog = await permissionCatalog.GetAllActiveAsync(cancellationToken).ConfigureAwait(false);
        foreach (var code in permissions)
        {
            if (!catalog.Any(p => string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase)))
            {
                return Result.Failure(RoleErrors.PermissionNotFound(code));
            }
        }

        return Result.Success();
    }

    public static async Task PersistAsync(
        RoleDefinition role,
        IOutboxWriter outbox,
        IPlatformUnitOfWork unitOfWork,
        IAuthRequestContext context,
        IClock clock,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in role.DomainEvents)
        {
            await outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    context.UserId,
                    context.CorrelationId,
                    clock.UtcNow,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        role.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

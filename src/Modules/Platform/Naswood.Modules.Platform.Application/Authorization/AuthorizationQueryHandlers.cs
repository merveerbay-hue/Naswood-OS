using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Contracts.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public sealed class GetPermissionsQueryHandler
    : IQueryHandler<GetPermissionsQuery, Result<IReadOnlyList<PermissionDto>>>
{
    private readonly IPermissionCatalogRepository _permissions;

    public GetPermissionsQueryHandler(IPermissionCatalogRepository permissions) => _permissions = permissions;

    public async Task<Result<IReadOnlyList<PermissionDto>>> HandleAsync(
        GetPermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var items = await _permissions.GetAllActiveAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyList<PermissionDto> dto = items.Select(Map).ToArray();
        return Result.Success(dto);
    }

    internal static PermissionDto Map(PermissionDefinition permission) =>
        new()
        {
            Id = permission.Id,
            Code = permission.Code,
            Module = permission.Module,
            Entity = permission.Entity,
            Action = permission.Action,
            Field = permission.Field,
            DisplayName = permission.DisplayName,
            IsActive = permission.IsActive
        };
}

public sealed class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    private readonly IRoleCatalogRepository _roles;

    public GetRolesQueryHandler(IRoleCatalogRepository roles) => _roles = roles;

    public async Task<Result<IReadOnlyList<RoleDto>>> HandleAsync(
        GetRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var items = await _roles.GetAllActiveAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyList<RoleDto> dto = items.Select(role => new RoleDto
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            IsActive = role.IsActive,
            Permissions = role.PermissionCodes.ToArray()
        }).ToArray();

        return Result.Success(dto);
    }
}

public sealed class GetMyPermissionsQueryHandler
    : IQueryHandler<GetMyPermissionsQuery, Result<MyPermissionsDto>>
{
    private readonly IEffectivePermissionService _effectivePermissions;

    public GetMyPermissionsQueryHandler(IEffectivePermissionService effectivePermissions) =>
        _effectivePermissions = effectivePermissions;

    public async Task<Result<MyPermissionsDto>> HandleAsync(
        GetMyPermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _effectivePermissions.GetForCurrentUserAsync(cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            return Result.Failure<MyPermissionsDto>(result.Error!);
        }

        var (user, permissions) = result.Value;
        return Result.Success(new MyPermissionsDto
        {
            UserId = user.Id.ToString("D"),
            Roles = user.Roles.ToArray(),
            Permissions = permissions.OrderBy(p => p).ToArray(),
            CompanyIds = user.CompanyIds.ToArray(),
            PlantIds = user.PlantIds.ToArray()
        });
    }
}

public sealed class CheckAuthorizationCommandHandler
    : ICommandHandler<CheckAuthorizationCommand, Result<AuthorizationCheckResponseDto>>
{
    private readonly IAuthorizationEngine _engine;
    private readonly IAuthUserRepository _users;
    private readonly IAuthRequestContext _context;

    public CheckAuthorizationCommandHandler(
        IAuthorizationEngine engine,
        IAuthUserRepository users,
        IAuthRequestContext context)
    {
        _engine = engine;
        _users = users;
        _context = context;
    }

    public async Task<Result<AuthorizationCheckResponseDto>> HandleAsync(
        CheckAuthorizationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (_context.UserId is null)
        {
            return Result.Failure<AuthorizationCheckResponseDto>(AuthorizationErrors.SessionInvalid());
        }

        var user = await _users.GetByIdAsync(_context.UserId.Value, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<AuthorizationCheckResponseDto>(AuthorizationErrors.SessionInvalid());
        }

        var decision = await _engine.EvaluateAsync(
                new AuthorizationEvaluationRequest(
                    user.Id,
                    user.Roles,
                    user.CompanyIds,
                    user.PlantIds,
                    command.Permission,
                    command.CompanyId ?? _context.CompanyId,
                    command.PlantId ?? _context.PlantId,
                    command.ResourceOwnerId,
                    command.Field,
                    RecordHistory: true),
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new AuthorizationCheckResponseDto
        {
            Allowed = decision.Allowed,
            Permission = decision.Permission,
            Reason = decision.Reason,
            DenialCode = decision.DenialCode
        });
    }
}

public sealed class GetAuthorizedModulesQueryHandler
    : IQueryHandler<GetAuthorizedModulesQuery, Result<IReadOnlyList<AuthorizedModuleDto>>>
{
    private readonly IEffectivePermissionService _effectivePermissions;

    public GetAuthorizedModulesQueryHandler(IEffectivePermissionService effectivePermissions) =>
        _effectivePermissions = effectivePermissions;

    public async Task<Result<IReadOnlyList<AuthorizedModuleDto>>> HandleAsync(
        GetAuthorizedModulesQuery query,
        CancellationToken cancellationToken = default)
    {
        var mine = await _effectivePermissions.GetForCurrentUserAsync(cancellationToken).ConfigureAwait(false);
        if (mine.IsFailure)
        {
            return Result.Failure<IReadOnlyList<AuthorizedModuleDto>>(mine.Error!);
        }

        var permissions = mine.Value.Permissions;
        IReadOnlyList<AuthorizedModuleDto> modules = permissions
            .Select(p => p.Contains('.') ? p.Split('.', 2)[0] : p)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(m => m)
            .Select(module => new AuthorizedModuleDto
            {
                Module = module,
                Permissions = permissions
                    .Where(p => p.StartsWith(module + ".", StringComparison.OrdinalIgnoreCase) ||
                                p.Equals(module, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(p => p)
                    .ToArray()
            })
            .ToArray();

        return Result.Success(modules);
    }
}

public sealed class GetAuthorizedMenuQueryHandler
    : IQueryHandler<GetAuthorizedMenuQuery, Result<IReadOnlyList<MenuItemDto>>>
{
    private static readonly MenuItemDto[] Catalog =
    [
        new() { Key = "dashboard", Title = "Dashboard", Module = "Platform", RequiredPermission = "Platform.Dashboard.View" },
        new() { Key = "inventory", Title = "Inventory", Module = "Inventory", RequiredPermission = "Inventory.View" },
        new() { Key = "purchasing", Title = "Purchasing", Module = "Purchasing", RequiredPermission = "Purchasing.View" },
        new() { Key = "sales", Title = "Sales", Module = "Sales", RequiredPermission = "Sales.View" },
        new() { Key = "production", Title = "Production", Module = "Production", RequiredPermission = "Production.View" },
        new() { Key = "quality", Title = "Quality", Module = "Quality", RequiredPermission = "Quality.View" },
        new() { Key = "maintenance", Title = "Maintenance", Module = "Maintenance", RequiredPermission = "Maintenance.View" },
        new() { Key = "finance", Title = "Finance", Module = "Finance", RequiredPermission = "Finance.View" },
        new() { Key = "administration", Title = "Administration", Module = "Administration", RequiredPermission = "Administration.Manage" }
    ];

    private readonly IEffectivePermissionService _effectivePermissions;

    public GetAuthorizedMenuQueryHandler(IEffectivePermissionService effectivePermissions) =>
        _effectivePermissions = effectivePermissions;

    public async Task<Result<IReadOnlyList<MenuItemDto>>> HandleAsync(
        GetAuthorizedMenuQuery query,
        CancellationToken cancellationToken = default)
    {
        var mine = await _effectivePermissions.GetForCurrentUserAsync(cancellationToken).ConfigureAwait(false);
        if (mine.IsFailure)
        {
            return Result.Failure<IReadOnlyList<MenuItemDto>>(mine.Error!);
        }

        var granted = mine.Value.Permissions;
        IReadOnlyList<MenuItemDto> menu = Catalog
            .Where(item => granted.Contains(item.RequiredPermission))
            .ToArray();

        return Result.Success(menu);
    }
}

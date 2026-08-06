using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public sealed record GetPermissionsQuery : IQuery<Result<IReadOnlyList<PermissionDto>>>;

public sealed record GetRolesQuery : IQuery<Result<IReadOnlyList<RoleDto>>>;

public sealed record GetMyPermissionsQuery : IQuery<Result<MyPermissionsDto>>;

public sealed record CheckAuthorizationCommand(
    string Permission,
    string? CompanyId,
    string? PlantId,
    string? ResourceOwnerId,
    string? Field) : ICommand<Result<AuthorizationCheckResponseDto>>;

public sealed record GetAuthorizedModulesQuery : IQuery<Result<IReadOnlyList<AuthorizedModuleDto>>>;

public sealed record GetAuthorizedMenuQuery : IQuery<Result<IReadOnlyList<MenuItemDto>>>;

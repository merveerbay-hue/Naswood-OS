namespace Naswood.Modules.Platform.Contracts.Authorization;

public sealed class PermissionDto
{
    public required Guid Id { get; init; }

    public required string Code { get; init; }

    public required string Module { get; init; }

    public string? Entity { get; init; }

    public required string Action { get; init; }

    public string? Field { get; init; }

    public required string DisplayName { get; init; }

    public required bool IsActive { get; init; }
}

public sealed class RoleDto
{
    public required Guid Id { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public required bool IsActive { get; init; }

    public required IReadOnlyList<string> Permissions { get; init; }
}

public sealed class MyPermissionsDto
{
    public required string UserId { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }

    public required IReadOnlyList<string> Permissions { get; init; }

    public required IReadOnlyList<string> CompanyIds { get; init; }

    public required IReadOnlyList<string> PlantIds { get; init; }
}

public sealed class AuthorizationCheckRequestDto
{
    public required string Permission { get; init; }

    public string? CompanyId { get; init; }

    public string? PlantId { get; init; }

    public string? ResourceOwnerId { get; init; }

    public string? Field { get; init; }
}

public sealed class AuthorizationCheckResponseDto
{
    public required bool Allowed { get; init; }

    public required string Permission { get; init; }

    public string? Reason { get; init; }

    public string? DenialCode { get; init; }
}

public sealed class AuthorizedModuleDto
{
    public required string Module { get; init; }

    public required IReadOnlyList<string> Permissions { get; init; }
}

public sealed class MenuItemDto
{
    public required string Key { get; init; }

    public required string Title { get; init; }

    public required string Module { get; init; }

    public required string RequiredPermission { get; init; }
}

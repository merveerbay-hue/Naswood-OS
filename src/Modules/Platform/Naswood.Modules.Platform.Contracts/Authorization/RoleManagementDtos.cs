namespace Naswood.Modules.Platform.Contracts.Authorization;

public sealed class RoleDetailDto
{
    public required Guid Id { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public string? CompanyCode { get; init; }

    public string? PlantCode { get; init; }

    public string? DepartmentCode { get; init; }

    public string? Category { get; init; }

    public required string Status { get; init; }

    public required bool IsActive { get; init; }

    public required bool IsSystem { get; init; }

    public required int Version { get; init; }

    public required IReadOnlyList<string> Permissions { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset UpdatedAt { get; init; }
}

public sealed class PagedRolesDto
{
    public required IReadOnlyList<RoleDetailDto> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required int TotalPages { get; init; }
}

public sealed class CreateRoleRequestDto
{
    public required string Code { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public string? Company { get; init; }

    public string? CompanyCode { get; init; }

    public string? PlantCode { get; init; }

    public string? DepartmentCode { get; init; }

    public string? Category { get; init; }

    public IReadOnlyList<string> Permissions { get; init; } = [];
}

public sealed class UpdateRoleRequestDto
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public string? CompanyCode { get; init; }

    public string? PlantCode { get; init; }

    public string? DepartmentCode { get; init; }

    public string? Category { get; init; }

    public IReadOnlyList<string>? Permissions { get; init; }
}

public sealed class CloneRoleRequestDto
{
    public required string Code { get; init; }

    public required string Name { get; init; }
}

public sealed class RolePermissionCodesRequestDto
{
    public required IReadOnlyList<string> Permissions { get; init; }
}

public sealed class RoleUserAssignmentRequestDto
{
    public required Guid UserId { get; init; }
}

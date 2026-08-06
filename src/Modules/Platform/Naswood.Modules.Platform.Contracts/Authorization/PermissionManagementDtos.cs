namespace Naswood.Modules.Platform.Contracts.Authorization;

public sealed class PermissionDetailDto
{
    public required Guid Id { get; init; }

    public required string Code { get; init; }

    public required string Module { get; init; }

    public string? Feature { get; init; }

    public required string Action { get; init; }

    public string? Field { get; init; }

    public required string DisplayName { get; init; }

    public string? Category { get; init; }

    public string? Description { get; init; }

    public required bool IsActive { get; init; }

    public required bool IsReserved { get; init; }

    public required IReadOnlyList<string> DependsOn { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset UpdatedAt { get; init; }
}

public sealed class PagedPermissionsDto
{
    public required IReadOnlyList<PermissionDetailDto> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required int TotalPages { get; init; }
}

public sealed class CreatePermissionRequestDto
{
    public required string Code { get; init; }

    public required string Module { get; init; }

    public string? Feature { get; init; }

    public required string Action { get; init; }

    public string? Field { get; init; }

    public string? DisplayName { get; init; }

    public string? Category { get; init; }

    public string? Description { get; init; }

    public IReadOnlyList<string>? DependsOn { get; init; }
}

public sealed class UpdatePermissionRequestDto
{
    public required string DisplayName { get; init; }

    public string? Category { get; init; }

    public string? Description { get; init; }

    public IReadOnlyList<string>? DependsOn { get; init; }

    public bool? IsActive { get; init; }
}

public sealed class ValidatePermissionRequestDto
{
    public required string Code { get; init; }

    public required string Module { get; init; }

    public string? Feature { get; init; }

    public required string Action { get; init; }

    public IReadOnlyList<string>? DependsOn { get; init; }
}

public sealed class PermissionValidationResultDto
{
    public required bool IsValid { get; init; }

    public required IReadOnlyList<string> Errors { get; init; }
}

public sealed class PermissionTemplateDto
{
    public required string Key { get; init; }

    public required string Name { get; init; }

    public required string Module { get; init; }

    public string? Feature { get; init; }

    public required IReadOnlyList<string> Actions { get; init; }

    public required IReadOnlyList<string> GeneratedCodes { get; init; }
}

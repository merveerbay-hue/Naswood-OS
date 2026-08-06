namespace Naswood.Modules.Platform.Contracts.Users;

public sealed class UserDto
{
    public required Guid Id { get; init; }

    public required string Username { get; init; }

    public string? EmployeeNumber { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public required string DisplayName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? MobilePhone { get; init; }

    public string? AvatarUrl { get; init; }

    public required string Status { get; init; }

    public required bool IsActive { get; init; }

    public required bool IsLocked { get; init; }

    public string? LockReason { get; init; }

    public required IReadOnlyList<string> CompanyIds { get; init; }

    public required IReadOnlyList<string> PlantIds { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }

    public string? DepartmentCode { get; init; }

    public string? PositionCode { get; init; }

    public Guid? ManagerUserId { get; init; }

    public string? CostCenter { get; init; }

    public DateOnly? HireDate { get; init; }

    public string? EmploymentType { get; init; }

    public string? EmployeeCategory { get; init; }

    public string? Language { get; init; }

    public string? TimeZone { get; init; }

    public string? DateFormat { get; init; }

    public string? NumberFormat { get; init; }

    public string? Currency { get; init; }

    public string? Theme { get; init; }

    public DateTimeOffset? LastLoginAt { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset UpdatedAt { get; init; }
}

public sealed class UserListItemDto
{
    public required Guid Id { get; init; }

    public string? EmployeeNumber { get; init; }

    public required string Username { get; init; }

    public required string DisplayName { get; init; }

    public string? Email { get; init; }

    public string? DepartmentCode { get; init; }

    public required IReadOnlyList<string> CompanyIds { get; init; }

    public required IReadOnlyList<string> PlantIds { get; init; }

    public required string Status { get; init; }

    public required bool IsLocked { get; init; }
}

public sealed class PagedUsersDto
{
    public required IReadOnlyList<UserListItemDto> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required int TotalPages { get; init; }
}

public sealed class CreateUserRequestDto
{
    public required string EmployeeNumber { get; init; }

    public required string Username { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public IReadOnlyList<string>? CompanyIds { get; init; }

    public IReadOnlyList<string>? PlantIds { get; init; }

    public IReadOnlyList<string> Roles { get; init; } = [];

    public string? DepartmentCode { get; init; }

    public string? PositionCode { get; init; }

    public string? Phone { get; init; }

    public string? MobilePhone { get; init; }

    /// <summary>Legacy single-company field from TASK example.</summary>
    public string? Company { get; init; }

    /// <summary>Legacy single-plant field from TASK example.</summary>
    public string? Plant { get; init; }
}

public sealed class UpdateUserRequestDto
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public string? Phone { get; init; }

    public string? MobilePhone { get; init; }

    public IReadOnlyList<string>? CompanyIds { get; init; }

    public IReadOnlyList<string>? PlantIds { get; init; }

    public string? DepartmentCode { get; init; }

    public string? PositionCode { get; init; }

    public Guid? ManagerUserId { get; init; }

    public string? CostCenter { get; init; }

    public DateOnly? HireDate { get; init; }

    public string? EmploymentType { get; init; }

    public string? EmployeeCategory { get; init; }

    public string? Language { get; init; }

    public string? TimeZone { get; init; }

    public string? DateFormat { get; init; }

    public string? NumberFormat { get; init; }

    public string? Currency { get; init; }

    public string? Theme { get; init; }
}

public sealed class LockUserRequestDto
{
    public required string Reason { get; init; }
}

public sealed class ResetPasswordRequestDto
{
    public required string NewPassword { get; init; }
}

public sealed class AssignRolesRequestDto
{
    public required IReadOnlyList<string> Roles { get; init; }
}

public sealed class AssignPlantsRequestDto
{
    public required IReadOnlyList<string> PlantIds { get; init; }
}

public sealed class UserImportResultDto
{
    public required int CreatedCount { get; init; }

    public required int FailedCount { get; init; }

    public required IReadOnlyList<string> Errors { get; init; }
}

public sealed class OrganizationReferenceDto
{
    public required string Code { get; init; }

    public required string Name { get; init; }
}

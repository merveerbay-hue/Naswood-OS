using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Users;

namespace Naswood.Modules.Platform.Application.Users;

public sealed record GetUsersQuery(
    string? EmployeeNumber,
    string? Username,
    string? Name,
    string? Email,
    string? DepartmentCode,
    string? CompanyId,
    string? PlantId,
    string? Status,
    int Page,
    int PageSize) : IQuery<Result<PagedUsersDto>>;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<Result<UserDto>>;

public sealed record CreateUserCommand(
    string EmployeeNumber,
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    IReadOnlyList<string> CompanyIds,
    IReadOnlyList<string> PlantIds,
    IReadOnlyList<string> Roles,
    string? DepartmentCode,
    string? PositionCode,
    string? Phone,
    string? MobilePhone) : ICommand<Result<UserDto>>;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? MobilePhone,
    IReadOnlyList<string>? CompanyIds,
    IReadOnlyList<string>? PlantIds,
    string? DepartmentCode,
    string? PositionCode,
    Guid? ManagerUserId,
    string? CostCenter,
    DateOnly? HireDate,
    string? EmploymentType,
    string? EmployeeCategory,
    string? Language,
    string? TimeZone,
    string? DateFormat,
    string? NumberFormat,
    string? Currency,
    string? Theme) : ICommand<Result<UserDto>>;

public sealed record DeleteUserCommand(Guid UserId, string? Reason) : ICommand<Result>;

public sealed record ActivateUserCommand(Guid UserId) : ICommand<Result<UserDto>>;

public sealed record DeactivateUserCommand(Guid UserId) : ICommand<Result<UserDto>>;

public sealed record LockUserCommand(Guid UserId, string Reason) : ICommand<Result<UserDto>>;

public sealed record UnlockUserCommand(Guid UserId) : ICommand<Result<UserDto>>;

public sealed record ResetUserPasswordCommand(Guid UserId, string NewPassword) : ICommand<Result>;

public sealed record AssignUserRolesCommand(Guid UserId, IReadOnlyList<string> Roles) : ICommand<Result<UserDto>>;

public sealed record AssignUserPlantsCommand(Guid UserId, IReadOnlyList<string> PlantIds) : ICommand<Result<UserDto>>;

public sealed record ImportUsersCommand(string CsvContent) : ICommand<Result<UserImportResultDto>>;

public sealed record ExportUsersQuery : IQuery<Result<string>>;

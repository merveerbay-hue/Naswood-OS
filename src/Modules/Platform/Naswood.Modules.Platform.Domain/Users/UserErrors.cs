using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Users;

public static class UserErrors
{
    public static Error NotFound() =>
        Error.NotFound("USER-001", "User was not found.");

    public static Error UsernameTaken() =>
        Error.Conflict("USER-002", "Username is already in use.");

    public static Error EmailTaken() =>
        Error.Conflict("USER-003", "Email is already in use.");

    public static Error EmployeeNumberTaken() =>
        Error.Conflict("USER-004", "Employee number is already in use.");

    public static Error CompanyNotFound(string code) =>
        Error.Validation("USER-005", $"Company '{code}' does not exist.");

    public static Error PlantNotFound(string code) =>
        Error.Validation("USER-006", $"Plant '{code}' does not exist.");

    public static Error DepartmentNotFound(string code) =>
        Error.Validation("USER-007", $"Department '{code}' does not exist.");

    public static Error PositionNotFound(string code) =>
        Error.Validation("USER-008", $"Position '{code}' does not exist.");

    public static Error RoleNotFound(string code) =>
        Error.Validation("USER-009", $"Role '{code}' does not exist.");

    public static Error RoleRequired() =>
        Error.Validation("USER-010", "At least one role is required.");

    public static Error InvalidEmail() =>
        Error.Validation("USER-011", "Email format is invalid.");

    public static Error Validation(string message) =>
        Error.Validation("USER-012", message);

    public static Error InvalidStatusTransition(string from, string to) =>
        Error.Conflict("USER-013", $"Cannot transition user status from {from} to {to}.");

    public static Error UsernameImmutable() =>
        Error.Validation("USER-014", "Username cannot be changed.");

    public static Error WeakPassword() =>
        Error.Validation(
            "USER-015",
            "Password must be at least 12 characters and include uppercase, lowercase, number and special character.");

    public static Error PlantCompanyMismatch(string plantCode, string companyCode) =>
        Error.Validation("USER-016", $"Plant '{plantCode}' does not belong to company '{companyCode}'.");
}

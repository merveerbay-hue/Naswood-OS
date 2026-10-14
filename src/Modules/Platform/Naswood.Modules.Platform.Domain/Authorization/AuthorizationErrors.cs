using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

public static class AuthorizationErrors
{
    public static Error AccessDenied(string? permission = null) =>
        Error.Forbidden(
            "AUTHZ-001",
            string.IsNullOrWhiteSpace(permission)
                ? "Access denied."
                : $"Access denied for permission '{permission}'.");

    public static Error PermissionRequired() =>
        Error.Forbidden("AUTHZ-002", "A permission is required for this operation.");

    public static Error CompanyAccessDenied() =>
        Error.Forbidden("AUTHZ-003", "Company access denied.");

    public static Error PlantAccessDenied() =>
        Error.Forbidden("AUTHZ-004", "Plant access denied.");

    public static Error RoleRequired() =>
        Error.Forbidden("AUTHZ-005", "At least one role is required.");

    public static Error SessionInvalid() =>
        Error.Unauthorized("AUTHZ-006", "Session is invalid.");

    public static Error Validation(string message) =>
        Error.Validation("AUTHZ-010", message);
}

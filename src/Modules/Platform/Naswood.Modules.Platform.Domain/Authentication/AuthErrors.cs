using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authentication;

public static class AuthErrors
{
    public static Error InvalidCredentials() =>
        Error.Unauthorized("AUTH-001", "Invalid username or password.");

    public static Error AccountDisabled() =>
        Error.Forbidden("AUTH-002", "Account is disabled.");

    public static Error AccountLocked() =>
        Error.Forbidden("AUTH-003", "Account is locked.");

    public static Error PasswordExpired() =>
        Error.Forbidden("AUTH-004", "Password has expired.");

    public static Error TokenExpired() =>
        Error.Unauthorized("AUTH-005", "Token has expired.");

    public static Error TokenInvalid() =>
        Error.Unauthorized("AUTH-006", "Token is invalid.");

    public static Error SessionExpired() =>
        Error.Unauthorized("AUTH-007", "Session has expired.");

    public static Error RefreshTokenInvalid() =>
        Error.Unauthorized("AUTH-008", "Refresh token is invalid.");

    public static Error CompanyOrPlantRequired() =>
        Error.Validation("AUTH-009", "Company and plant selection is required.");

    public static Error Validation(string message) =>
        Error.Validation("AUTH-010", message);
}

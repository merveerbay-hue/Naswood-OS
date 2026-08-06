namespace Naswood.BuildingBlocks.Domain;

/// <summary>
/// Structured error used by Result without throwing for expected failures.
/// </summary>
public sealed record Error(string Code, string Message, string Category = "Domain")
{
    public static Error Validation(string code, string message) =>
        new(code, message, "Validation");

    public static Error NotFound(string code, string message) =>
        new(code, message, "NotFound");

    public static Error Conflict(string code, string message) =>
        new(code, message, "Conflict");

    public static Error Unauthorized(string code, string message) =>
        new(code, message, "Unauthorized");

    public static Error Forbidden(string code, string message) =>
        new(code, message, "Forbidden");

    public static Error Failure(string code, string message) =>
        new(code, message, "Failure");
}

namespace Naswood.BuildingBlocks.Application.Contracts;

/// <summary>
/// Canonical API success envelope from Phase_0_Canonical_Contracts.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public T? Data { get; init; }

    public string? Message { get; init; }

    public object? Metadata { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null, object? metadata = null) =>
        new()
        {
            Success = true,
            Data = data,
            Message = message,
            Metadata = metadata ?? new { }
        };
}

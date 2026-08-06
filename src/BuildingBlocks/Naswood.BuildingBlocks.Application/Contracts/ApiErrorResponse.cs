namespace Naswood.BuildingBlocks.Application.Contracts;

/// <summary>
/// Canonical API error envelope from Phase_0_Canonical_Contracts.
/// </summary>
public sealed class ApiErrorResponse
{
    public bool Success { get; init; } = false;

    public object? Data { get; init; }

    public string? Message { get; init; }

    public IReadOnlyList<ApiErrorItem> Errors { get; init; } = [];

    public ApiErrorMetadata Metadata { get; init; } = new();
}

public sealed class ApiErrorItem
{
    public required string Code { get; init; }

    public required string Category { get; init; }

    public string? Field { get; init; }

    public required string Message { get; init; }

    public object? Details { get; init; }
}

public sealed class ApiErrorMetadata
{
    public string CorrelationId { get; init; } = Guid.NewGuid().ToString("N");

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

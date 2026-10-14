namespace Naswood.Modules.Platform.Contracts.Audit;

public sealed class AuditLogDto
{
    public required Guid Id { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public Guid? UserId { get; init; }

    public string? Username { get; init; }

    public required string Module { get; init; }

    public string? Entity { get; init; }

    public string? EntityId { get; init; }

    public required string Action { get; init; }

    public string? OldValuesJson { get; init; }

    public string? NewValuesJson { get; init; }

    public string? IpAddress { get; init; }

    public Guid? SessionId { get; init; }

    public required string CorrelationId { get; init; }

    public string? CompanyId { get; init; }

    public string? PlantId { get; init; }

    public required string Severity { get; init; }

    public required string Status { get; init; }
}

public sealed class PagedAuditLogsDto
{
    public required IReadOnlyList<AuditLogDto> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required int TotalPages { get; init; }
}

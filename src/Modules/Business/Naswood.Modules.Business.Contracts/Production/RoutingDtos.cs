namespace Naswood.Modules.Business.Contracts.Production;

public sealed class RoutingDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string MaterialCode { get; init; }
    public int Version { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertRoutingRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string MaterialCode { get; init; } = string.Empty;
    public int Version { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedRoutingDto
{
    public required IReadOnlyList<RoutingDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

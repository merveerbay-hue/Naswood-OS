namespace Naswood.Modules.Business.Contracts.Sales;

public sealed class OpportunityDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string CustomerCode { get; init; }
    public string? Title { get; init; }
    public decimal Amount { get; init; }
    public required string Stage { get; init; }
    public required string Status { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertOpportunityRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string CustomerCode { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Stage { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

public sealed class PagedOpportunityDto
{
    public required IReadOnlyList<OpportunityDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

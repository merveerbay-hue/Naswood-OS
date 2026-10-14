namespace Naswood.Modules.Business.Contracts.Purchasing;

public sealed class PurchaseRequestDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string Requester { get; init; }
    public DateOnly? NeededDate { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertPurchaseRequestRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string Requester { get; init; } = string.Empty;
    public DateOnly? NeededDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedPurchaseRequestDto
{
    public required IReadOnlyList<PurchaseRequestDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

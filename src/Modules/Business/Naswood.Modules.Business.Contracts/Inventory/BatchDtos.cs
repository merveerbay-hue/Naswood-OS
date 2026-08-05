namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class BatchDto
{
    public required Guid Id { get; init; }
    public required string? BatchNumber { get; init; }
    public required string MaterialCode { get; init; }
    public decimal Quantity { get; init; }
    public DateOnly? ExpiryDate { get; init; }
    public required string Status { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertBatchRequestDto
{
    public string BatchNumber { get; init; } = string.Empty;
    public string MaterialCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public DateOnly? ExpiryDate { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed class PagedBatchDto
{
    public required IReadOnlyList<BatchDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

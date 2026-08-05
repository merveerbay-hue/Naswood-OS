namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class GoodsReceiptDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string WarehouseCode { get; init; }
    public string? Reference { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertGoodsReceiptRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedGoodsReceiptDto
{
    public required IReadOnlyList<GoodsReceiptDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

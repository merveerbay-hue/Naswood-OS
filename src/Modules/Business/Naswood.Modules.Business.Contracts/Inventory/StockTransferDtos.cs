namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class StockTransferDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string FromWarehouseCode { get; init; }
    public required string ToWarehouseCode { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertStockTransferRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string FromWarehouseCode { get; init; } = string.Empty;
    public string ToWarehouseCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedStockTransferDto
{
    public required IReadOnlyList<StockTransferDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

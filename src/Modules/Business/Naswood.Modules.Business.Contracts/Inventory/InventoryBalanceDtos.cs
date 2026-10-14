namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class InventoryBalanceDto
{
    public required Guid Id { get; init; }
    public required string MaterialCode { get; init; }
    public required string WarehouseCode { get; init; }
    public required string? LocationCode { get; init; }
    public required string? BatchNumber { get; init; }
    public decimal QuantityOnHand { get; init; }
    public decimal QuantityReserved { get; init; }
    public required string Status { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertInventoryBalanceRequestDto
{
    public string MaterialCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string BatchNumber { get; init; } = string.Empty;
    public decimal QuantityOnHand { get; init; }
    public decimal QuantityReserved { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed class PagedInventoryBalanceDto
{
    public required IReadOnlyList<InventoryBalanceDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

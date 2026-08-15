namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class InventoryDashboardDto
{
    public decimal MaterialCount { get; init; }
    public decimal WarehouseCount { get; init; }
    public decimal LocationCount { get; init; }
    public decimal BalanceRows { get; init; }
    public decimal QuantityOnHand { get; init; }
    public decimal QuantityReserved { get; init; }
    public decimal QuantityAvailable { get; init; }
    public decimal OpenGoodsReceipts { get; init; }
    public decimal OpenGoodsIssues { get; init; }
    public decimal OpenTransfers { get; init; }
    public decimal OpenCounts { get; init; }
    public decimal NegativeBalanceRows { get; init; }
    public decimal HoldPackageCount { get; init; }
    public decimal AvailablePackageCount { get; init; }
    public decimal PostedMovementCount { get; init; }
    public IReadOnlyList<InventoryDockItemDto> DockItems { get; init; } = Array.Empty<InventoryDockItemDto>();
}

public sealed class InventoryDockItemDto
{
    public required string Gate { get; init; }
    public required string Truck { get; init; }
    public required string Supplier { get; init; }
    public required string Stage { get; init; }
    public required string DocumentNumber { get; init; }
    public required string Status { get; init; }
}

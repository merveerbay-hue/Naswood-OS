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
}

namespace Naswood.Modules.Business.Contracts.Purchasing;

public sealed class PurchasingDashboardDto
{
    public decimal OpenPurchaseOrders { get; init; }
    public decimal PendingApprovals { get; init; }
    public decimal OverdueReceipts { get; init; }
    public decimal SpendMtd { get; init; }
}

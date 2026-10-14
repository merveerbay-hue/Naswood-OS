namespace Naswood.Modules.Business.Domain.Purchasing;

public sealed class PurchasingDashboardSnapshot
{
    public decimal OpenPurchaseOrders { get; init; }
    public decimal PendingApprovals { get; init; }
    public decimal OverdueReceipts { get; init; }
    public decimal SpendMtd { get; init; }
}

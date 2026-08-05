namespace Naswood.Modules.Business.Domain.Sales;

public sealed class SalesDashboardSnapshot
{
    public decimal OpenOrders { get; init; }
    public decimal PipelineAmount { get; init; }
    public decimal OverdueDeliveries { get; init; }
    public decimal RevenueMtd { get; init; }
}

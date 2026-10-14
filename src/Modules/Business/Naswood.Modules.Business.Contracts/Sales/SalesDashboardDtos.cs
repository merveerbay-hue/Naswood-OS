namespace Naswood.Modules.Business.Contracts.Sales;

public sealed class SalesDashboardDto
{
    public decimal OpenOrders { get; init; }
    public decimal PipelineAmount { get; init; }
    public decimal OverdueDeliveries { get; init; }
    public decimal RevenueMtd { get; init; }
}

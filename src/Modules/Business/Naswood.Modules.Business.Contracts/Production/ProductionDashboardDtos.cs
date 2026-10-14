namespace Naswood.Modules.Business.Contracts.Production;

public sealed class ProductionDashboardDto
{
    public decimal OpenProductionOrders { get; init; }
    public decimal ActiveWorkOrders { get; init; }
    public decimal WipQuantity { get; init; }
    public decimal ScrapRate { get; init; }
}

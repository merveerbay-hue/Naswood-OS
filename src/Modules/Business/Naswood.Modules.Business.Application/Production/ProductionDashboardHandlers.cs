using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Contracts.Production;

namespace Naswood.Modules.Business.Application.Production;

public sealed record GetProductionDashboardQuery() : IQuery<Result<ProductionDashboardDto>>;

public sealed class GetProductionDashboardQueryHandler : IQueryHandler<GetProductionDashboardQuery, Result<ProductionDashboardDto>>
{
    private readonly IProductionOrderRepository _orders;
    private readonly IWorkOrderRepository _workOrders;
    private readonly IWipRepository _wips;
    private readonly IScrapRepository _scraps;

    public GetProductionDashboardQueryHandler(
        IProductionOrderRepository orders,
        IWorkOrderRepository workOrders,
        IWipRepository wips,
        IScrapRepository scraps)
    {
        _orders = orders;
        _workOrders = workOrders;
        _wips = wips;
        _scraps = scraps;
    }

    public async Task<Result<ProductionDashboardDto>> HandleAsync(
        GetProductionDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        var orders = await _orders.SearchAsync(null, 1, 200, cancellationToken).ConfigureAwait(false);
        var workOrders = await _workOrders.SearchAsync(null, 1, 200, cancellationToken).ConfigureAwait(false);
        var wips = await _wips.SearchAsync(null, 1, 200, cancellationToken).ConfigureAwait(false);
        var scraps = await _scraps.SearchAsync(null, 1, 200, cancellationToken).ConfigureAwait(false);

        static bool IsOpen(string status) =>
            !string.Equals(status, "Closed", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(status, "Posted", StringComparison.OrdinalIgnoreCase);

        var openOrders = orders.Items.Count(x => IsOpen(x.Status));
        var activeWos = workOrders.Items.Count(x => IsOpen(x.Status));
        var scrapRate = workOrders.Total == 0 ? 0 : Math.Round((decimal)scraps.Total / workOrders.Total * 100m, 2);

        return Result.Success(new ProductionDashboardDto
        {
            OpenProductionOrders = openOrders,
            ActiveWorkOrders = activeWos,
            WipQuantity = wips.Total,
            ScrapRate = scrapRate,
        });
    }
}

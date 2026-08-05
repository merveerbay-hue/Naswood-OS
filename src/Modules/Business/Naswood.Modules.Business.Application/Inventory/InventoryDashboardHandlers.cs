using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Contracts.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public sealed record GetInventoryDashboardQuery() : IQuery<Result<InventoryDashboardDto>>;

public sealed class GetInventoryDashboardQueryHandler : IQueryHandler<GetInventoryDashboardQuery, Result<InventoryDashboardDto>>
{
    private readonly IMaterialRepository _materials;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IGoodsReceiptRepository _goodsReceipts;
    private readonly IGoodsIssueRepository _goodsIssues;
    private readonly IStockTransferRepository _transfers;
    private readonly IInventoryCountRepository _counts;

    public GetInventoryDashboardQueryHandler(
        IMaterialRepository materials,
        IWarehouseRepository warehouses,
        ILocationRepository locations,
        IInventoryBalanceRepository balances,
        IGoodsReceiptRepository goodsReceipts,
        IGoodsIssueRepository goodsIssues,
        IStockTransferRepository transfers,
        IInventoryCountRepository counts)
    {
        _materials = materials;
        _warehouses = warehouses;
        _locations = locations;
        _balances = balances;
        _goodsReceipts = goodsReceipts;
        _goodsIssues = goodsIssues;
        _transfers = transfers;
        _counts = counts;
    }

    public async Task<Result<InventoryDashboardDto>> HandleAsync(
        GetInventoryDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        var materials = await _materials.SearchAsync(null, 1, 1, cancellationToken).ConfigureAwait(false);
        var warehouses = await _warehouses.SearchAsync(null, 1, 1, cancellationToken).ConfigureAwait(false);
        var locations = await _locations.SearchAsync(null, 1, 1, cancellationToken).ConfigureAwait(false);
        var balances = await _balances.SearchAsync(null, 1, 500, cancellationToken).ConfigureAwait(false);
        var receipts = await _goodsReceipts.SearchAsync(null, 1, 100, cancellationToken).ConfigureAwait(false);
        var issues = await _goodsIssues.SearchAsync(null, 1, 100, cancellationToken).ConfigureAwait(false);
        var transfers = await _transfers.SearchAsync(null, 1, 100, cancellationToken).ConfigureAwait(false);
        var counts = await _counts.SearchAsync(null, 1, 100, cancellationToken).ConfigureAwait(false);

        var onHand = balances.Items.Sum(x => x.QuantityOnHand);
        var reserved = balances.Items.Sum(x => x.QuantityReserved);

        static int OpenDocs<T>(IReadOnlyList<T> items, Func<T, string> status) =>
            items.Count(x =>
            {
                var s = status(x);
                return !string.Equals(s, "Posted", StringComparison.OrdinalIgnoreCase)
                       && !string.Equals(s, "Cancelled", StringComparison.OrdinalIgnoreCase)
                       && !string.Equals(s, "Closed", StringComparison.OrdinalIgnoreCase);
            });

        return Result.Success(new InventoryDashboardDto
        {
            MaterialCount = materials.Total,
            WarehouseCount = warehouses.Total,
            LocationCount = locations.Total,
            BalanceRows = balances.Total,
            QuantityOnHand = onHand,
            QuantityReserved = reserved,
            QuantityAvailable = onHand - reserved,
            OpenGoodsReceipts = OpenDocs(receipts.Items, x => x.Status),
            OpenGoodsIssues = OpenDocs(issues.Items, x => x.Status),
            OpenTransfers = OpenDocs(transfers.Items, x => x.Status),
            OpenCounts = OpenDocs(counts.Items, x => x.Status),
        });
    }
}

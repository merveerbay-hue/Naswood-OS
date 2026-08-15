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
    private readonly IInventoryPackageRepository _packages;
    private readonly IInventoryMovementRepository _movements;

    public GetInventoryDashboardQueryHandler(
        IMaterialRepository materials,
        IWarehouseRepository warehouses,
        ILocationRepository locations,
        IInventoryBalanceRepository balances,
        IGoodsReceiptRepository goodsReceipts,
        IGoodsIssueRepository goodsIssues,
        IStockTransferRepository transfers,
        IInventoryCountRepository counts,
        IInventoryPackageRepository packages,
        IInventoryMovementRepository movements)
    {
        _materials = materials;
        _warehouses = warehouses;
        _locations = locations;
        _balances = balances;
        _goodsReceipts = goodsReceipts;
        _goodsIssues = goodsIssues;
        _transfers = transfers;
        _counts = counts;
        _packages = packages;
        _movements = movements;
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
        var packages = await _packages.SearchAsync(null, 1, 200, cancellationToken).ConfigureAwait(false);
        var movements = await _movements.SearchAsync(null, 1, 1, cancellationToken).ConfigureAwait(false);

        var onHand = balances.Items.Sum(x => x.QuantityOnHand);
        var reserved = balances.Items.Sum(x => x.QuantityReserved);
        var negative = balances.Items.Count(x => x.QuantityOnHand < 0);
        var holdPackages = packages.Items.Count(x =>
            string.Equals(x.Status, "Hold", StringComparison.OrdinalIgnoreCase)
            || string.Equals(x.Status, "Quarantine", StringComparison.OrdinalIgnoreCase));
        var availablePackages = packages.Items.Count(x =>
            string.Equals(x.Status, "Available", StringComparison.OrdinalIgnoreCase));

        static int OpenDocs<T>(IReadOnlyList<T> items, Func<T, string> status) =>
            items.Count(x =>
            {
                var s = status(x);
                return !string.Equals(s, "Posted", StringComparison.OrdinalIgnoreCase)
                       && !string.Equals(s, "Cancelled", StringComparison.OrdinalIgnoreCase)
                       && !string.Equals(s, "Closed", StringComparison.OrdinalIgnoreCase);
            });

        // Dock board: open (non-posted) receipts first, then recent posted as operational trail.
        var dock = receipts.Items
            .OrderByDescending(x => x.CreatedAt)
            .Take(8)
            .Select(r =>
            {
                var notes = r.Notes ?? string.Empty;
                string Pick(string key)
                {
                    var token = notes.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault(p => p.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase));
                    return token is null ? "—" : token[(key.Length + 1)..];
                }

                return new InventoryDockItemDto
                {
                    Gate = Pick("gate"),
                    Truck = string.IsNullOrWhiteSpace(r.Reference) ? "—" : r.Reference,
                    Supplier = Pick("supplier"),
                    Stage = string.Equals(r.Status, "Posted", StringComparison.OrdinalIgnoreCase) ? "Posted" : "Open",
                    DocumentNumber = r.Number,
                    Status = r.Status
                };
            })
            .ToArray();

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
            NegativeBalanceRows = negative,
            HoldPackageCount = holdPackages,
            AvailablePackageCount = availablePackages,
            PostedMovementCount = movements.Total,
            DockItems = dock
        });
    }
}

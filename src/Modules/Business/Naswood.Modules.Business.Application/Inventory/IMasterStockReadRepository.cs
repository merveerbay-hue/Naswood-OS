using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public sealed record MasterStockQueryFilter(
    string PlantId,
    string? WarehouseCode,
    string? LocationCode,
    string? MaterialCode,
    string? LotNumber,
    string? StockStatus,
    string? Q);

public sealed record MasterStockPackageAgg(
    string PlantId,
    string WarehouseCode,
    string LocationCode,
    string MaterialCode,
    string LotNumber,
    int PackageCount,
    decimal QuantitySum);

public sealed record MasterStockPhysNote(
    string PlantId,
    string WarehouseCode,
    string LocationCode,
    string MaterialCode,
    string LotNumber,
    string? PackageNumber,
    string Notes,
    DateTimeOffset CreatedAt);

public interface IMasterStockReadRepository
{
    Task<(IReadOnlyList<InventoryBalance> Items, int Total)> SearchBalancesAsync(
        MasterStockQueryFilter filter,
        int page,
        int pageSize,
        string sortBy,
        bool desc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryBalance>> ListAllBalancesAsync(
        MasterStockQueryFilter filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MasterStockPackageAgg>> PackageAggregatesAsync(
        string plantId,
        string? warehouseCode,
        string? locationCode,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InventoryPackage> Items, int Total)> SearchPackagesAsync(
        MasterStockQueryFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryPackage>> ListAllPackagesAsync(
        MasterStockQueryFilter filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryPackage>> ListPackagesForBalanceAsync(
        string plantId,
        string warehouseCode,
        string locationCode,
        string materialCode,
        string lot,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Material>> ListMaterialsByCodesAsync(
        IReadOnlyList<string> codes,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> WarehouseNamesAsync(
        string plantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> LocationNamesAsync(
        string plantId,
        string? warehouseCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MasterStockPhysNote>> LatestPhysNotesAsync(
        string plantId,
        IReadOnlyList<string> materialCodes,
        CancellationToken cancellationToken = default);

    Task<int> CountPackagesAsync(MasterStockQueryFilter filter, CancellationToken cancellationToken = default);
}

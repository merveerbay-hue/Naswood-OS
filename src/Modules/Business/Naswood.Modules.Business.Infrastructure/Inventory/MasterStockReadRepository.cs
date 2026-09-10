using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class MasterStockReadRepository : IMasterStockReadRepository
{
    private readonly BusinessDbContext _db;
    public MasterStockReadRepository(BusinessDbContext db) => _db = db;

    public async Task<(IReadOnlyList<InventoryBalance> Items, int Total)> SearchBalancesAsync(
        MasterStockQueryFilter filter,
        int page,
        int pageSize,
        string sortBy,
        bool desc,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyBalanceFilter(_db.Set<InventoryBalance>().AsNoTracking(), filter);
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        query = ApplyBalanceSort(query, sortBy, desc);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    public async Task<IReadOnlyList<InventoryBalance>> ListAllBalancesAsync(
        MasterStockQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        return await ApplyBalanceSort(ApplyBalanceFilter(_db.Set<InventoryBalance>().AsNoTracking(), filter), "materialCode", false)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MasterStockPackageAgg>> PackageAggregatesAsync(
        string plantId,
        string? warehouseCode,
        string? locationCode,
        CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        var query = _db.Set<InventoryPackage>().AsNoTracking().Where(x => !x.IsDeleted && x.PlantId == plant);
        if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            var wh = warehouseCode.Trim();
            query = query.Where(x => x.WarehouseCode == wh);
        }
        if (!string.IsNullOrWhiteSpace(locationCode))
        {
            var loc = locationCode.Trim();
            query = query.Where(x => x.LocationCode == loc);
        }

        return await query
            .GroupBy(x => new { x.PlantId, x.WarehouseCode, x.LocationCode, x.MaterialCode, x.LotNumber })
            .Select(g => new MasterStockPackageAgg(
                g.Key.PlantId ?? plant,
                g.Key.WarehouseCode,
                g.Key.LocationCode,
                g.Key.MaterialCode,
                g.Key.LotNumber,
                g.Count(),
                g.Sum(x => x.Quantity)))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<(IReadOnlyList<InventoryPackage> Items, int Total)> SearchPackagesAsync(
        MasterStockQueryFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyPackageFilter(_db.Set<InventoryPackage>().AsNoTracking(), filter);
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderBy(x => x.MaterialCode).ThenBy(x => x.PackageNumber)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    public async Task<IReadOnlyList<InventoryPackage>> ListAllPackagesAsync(
        MasterStockQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        return await ApplyPackageFilter(_db.Set<InventoryPackage>().AsNoTracking(), filter)
            .OrderBy(x => x.MaterialCode).ThenBy(x => x.PackageNumber)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<InventoryPackage>> ListPackagesForBalanceAsync(
        string plantId,
        string warehouseCode,
        string locationCode,
        string materialCode,
        string lot,
        CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        var wh = warehouseCode.Trim();
        var loc = locationCode.Trim();
        var mat = materialCode.Trim();
        var lotKey = (lot ?? string.Empty).Trim();
        return await _db.Set<InventoryPackage>().AsNoTracking()
            .Where(x => !x.IsDeleted
                && x.PlantId == plant
                && x.WarehouseCode == wh
                && x.LocationCode == loc
                && x.MaterialCode == mat
                && x.LotNumber == lotKey)
            .OrderBy(x => x.PackageNumber)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Material>> ListMaterialsByCodesAsync(
        IReadOnlyList<string> codes,
        CancellationToken cancellationToken = default)
    {
        if (codes.Count == 0) return Array.Empty<Material>();
        var set = codes.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim()).Distinct().ToArray();
        return await _db.Set<Material>().AsNoTracking()
            .Where(x => !x.IsDeleted && set.Contains(x.Code))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyDictionary<string, string>> WarehouseNamesAsync(
        string plantId,
        CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        var rows = await _db.Set<Warehouse>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.PlantId == plant)
            .Select(x => new { x.Code, x.Name })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.ToDictionary(x => x.Code, x => string.IsNullOrWhiteSpace(x.Name) ? x.Code : x.Name, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyDictionary<string, string>> LocationNamesAsync(
        string plantId,
        string? warehouseCode,
        CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        var query = _db.Set<Location>().AsNoTracking().Where(x => !x.IsDeleted && x.PlantId == plant);
        if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            var wh = warehouseCode.Trim();
            query = query.Where(x => x.WarehouseCode == wh);
        }
        var rows = await query.Select(x => new { x.WarehouseCode, x.Code, x.Name }).ToListAsync(cancellationToken).ConfigureAwait(false);
        return rows
            .GroupBy(x => $"{x.WarehouseCode}|{x.Code}", StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => string.IsNullOrWhiteSpace(g.First().Name) ? g.First().Code : g.First().Name, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<MasterStockPhysNote>> LatestPhysNotesAsync(
        string plantId,
        IReadOnlyList<string> materialCodes,
        CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        var mats = materialCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim()).Distinct().ToArray();
        if (mats.Length == 0) return Array.Empty<MasterStockPhysNote>();

        var rows = await _db.Set<InventoryMovement>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.PlantId == plant && mats.Contains(x.MaterialCode) && x.Notes.Contains("phys="))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new MasterStockPhysNote(
                x.PlantId ?? plant,
                x.WarehouseCode,
                x.LocationCode,
                x.MaterialCode,
                x.LotNumber,
                x.PackageNumber,
                x.Notes,
                x.CreatedAt))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows;
    }

    public async Task<int> CountPackagesAsync(MasterStockQueryFilter filter, CancellationToken cancellationToken = default)
        => await ApplyPackageFilter(_db.Set<InventoryPackage>().AsNoTracking(), filter).CountAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<InventoryPackageContent>> ListContentsForPackagesAsync(
        IReadOnlyList<Guid> packageIds,
        CancellationToken cancellationToken = default)
    {
        if (packageIds.Count == 0) return Array.Empty<InventoryPackageContent>();
        var ids = packageIds.Distinct().ToArray();
        return await _db.Set<InventoryPackageContent>().AsNoTracking()
            .Where(x => !x.IsDeleted && ids.Contains(x.PackageId))
            .OrderBy(x => x.PackageId)
            .ThenBy(x => x.LineNo)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private static IQueryable<InventoryBalance> ApplyBalanceFilter(IQueryable<InventoryBalance> query, MasterStockQueryFilter filter)
    {
        var plant = filter.PlantId.Trim();
        query = query.Where(x => !x.IsDeleted && x.PlantId == plant);
        if (!string.IsNullOrWhiteSpace(filter.WarehouseCode))
        {
            var wh = filter.WarehouseCode.Trim();
            query = query.Where(x => x.WarehouseCode == wh);
        }
        if (!string.IsNullOrWhiteSpace(filter.LocationCode))
        {
            var loc = filter.LocationCode.Trim();
            query = query.Where(x => x.LocationCode == loc);
        }
        if (!string.IsNullOrWhiteSpace(filter.MaterialCode))
        {
            var mat = filter.MaterialCode.Trim();
            query = query.Where(x => x.MaterialCode == mat);
        }
        if (!string.IsNullOrWhiteSpace(filter.LotNumber))
        {
            var lot = filter.LotNumber.Trim();
            query = query.Where(x => x.BatchNumber == lot);
        }
        if (!string.IsNullOrWhiteSpace(filter.Q))
        {
            var q = filter.Q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.MaterialCode, "%" + q + "%")
                || EF.Functions.ILike(x.BatchNumber, "%" + q + "%"));
        }
        if (!string.IsNullOrWhiteSpace(filter.StockStatus)
            && !filter.StockStatus.Trim().Equals("Tümü", StringComparison.OrdinalIgnoreCase))
        {
            var st = filter.StockStatus.Trim();
            if (st.Equals("Available", StringComparison.OrdinalIgnoreCase)
                || st.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.Status == "Active" || x.Status == "Available");
            }
            else
            {
                query = query.Where(x => x.Status == st);
            }
        }
        return query;
    }

    private static IQueryable<InventoryBalance> ApplyBalanceSort(IQueryable<InventoryBalance> query, string sortBy, bool desc)
    {
        var key = (sortBy ?? "materialCode").Trim().ToLowerInvariant();
        IOrderedQueryable<InventoryBalance> ordered = key switch
        {
            "warehouse" or "warehousecode" => desc
                ? query.OrderByDescending(x => x.WarehouseCode)
                : query.OrderBy(x => x.WarehouseCode),
            "location" or "locationcode" => desc
                ? query.OrderByDescending(x => x.LocationCode)
                : query.OrderBy(x => x.LocationCode),
            "lot" or "batchnumber" => desc
                ? query.OrderByDescending(x => x.BatchNumber)
                : query.OrderBy(x => x.BatchNumber),
            "qty" or "stockquantity" or "quantityonhand" => desc
                ? query.OrderByDescending(x => x.QuantityOnHand)
                : query.OrderBy(x => x.QuantityOnHand),
            "status" or "stockstatus" => desc
                ? query.OrderByDescending(x => x.Status)
                : query.OrderBy(x => x.Status),
            _ => desc
                ? query.OrderByDescending(x => x.MaterialCode)
                : query.OrderBy(x => x.MaterialCode)
        };
        return ordered.ThenBy(x => x.WarehouseCode).ThenBy(x => x.LocationCode).ThenBy(x => x.BatchNumber);
    }

    private static IQueryable<InventoryPackage> ApplyPackageFilter(IQueryable<InventoryPackage> query, MasterStockQueryFilter filter)
    {
        var plant = filter.PlantId.Trim();
        query = query.Where(x => !x.IsDeleted && x.PlantId == plant);
        if (!string.IsNullOrWhiteSpace(filter.WarehouseCode))
        {
            var wh = filter.WarehouseCode.Trim();
            query = query.Where(x => x.WarehouseCode == wh);
        }
        if (!string.IsNullOrWhiteSpace(filter.LocationCode))
        {
            var loc = filter.LocationCode.Trim();
            query = query.Where(x => x.LocationCode == loc);
        }
        if (!string.IsNullOrWhiteSpace(filter.MaterialCode))
        {
            var mat = filter.MaterialCode.Trim();
            query = query.Where(x => x.MaterialCode == mat);
        }
        if (!string.IsNullOrWhiteSpace(filter.LotNumber))
        {
            var lot = filter.LotNumber.Trim();
            query = query.Where(x => x.LotNumber == lot);
        }
        if (!string.IsNullOrWhiteSpace(filter.StockStatus)
            && !filter.StockStatus.Trim().Equals("Tümü", StringComparison.OrdinalIgnoreCase)
            && !filter.StockStatus.Trim().Equals("Available", StringComparison.OrdinalIgnoreCase)
            && !filter.StockStatus.Trim().Equals("Active", StringComparison.OrdinalIgnoreCase))
        {
            var st = filter.StockStatus.Trim();
            query = query.Where(x => x.Status == st);
        }
        else if (!string.IsNullOrWhiteSpace(filter.StockStatus)
            && (filter.StockStatus.Trim().Equals("Available", StringComparison.OrdinalIgnoreCase)
                || filter.StockStatus.Trim().Equals("Active", StringComparison.OrdinalIgnoreCase)))
        {
            query = query.Where(x => x.Status == "Available" || x.Status == "Active");
        }
        if (!string.IsNullOrWhiteSpace(filter.Q))
        {
            var q = filter.Q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.PackageNumber, "%" + q + "%")
                || EF.Functions.ILike(x.MaterialCode, "%" + q + "%")
                || EF.Functions.ILike(x.LotNumber, "%" + q + "%"));
        }
        return query;
    }
}

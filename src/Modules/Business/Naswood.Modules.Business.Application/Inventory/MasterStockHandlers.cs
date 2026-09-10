using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public sealed record SearchMasterStockQuery(
    string PlantId,
    int Page,
    int PageSize,
    string? WarehouseCode,
    string? LocationCode,
    string? MaterialCode,
    string? LotNumber,
    string? StockStatus,
    string? Q,
    string? SortBy,
    string? SortDir,
    DateTimeOffset? AsOfDate,
    Guid? WarehouseId,
    Guid? LocationId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedMasterStockDto>>;

public sealed record SearchMasterStockPackagesQuery(
    string PlantId,
    int Page,
    int PageSize,
    string? WarehouseCode,
    string? LocationCode,
    string? MaterialCode,
    string? LotNumber,
    string? StockStatus,
    string? Q,
    Guid? WarehouseId,
    Guid? LocationId,
    DateTimeOffset? AsOfDate,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedMasterStockPackageDto>>;

public sealed record GetMasterStockBalancePackagesQuery(
    Guid BalanceId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<IReadOnlyList<MasterStockPackageRowDto>>>;

public sealed record ExportMasterStockQuery(
    string PlantId,
    string? WarehouseCode,
    string? LocationCode,
    string? MaterialCode,
    string? LotNumber,
    string? StockStatus,
    string? Q,
    Guid? WarehouseId,
    Guid? LocationId,
    DateTimeOffset? AsOfDate,
    string PreparedBy,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<MasterStockExportDto>>;

internal sealed record ResolvedMasterStockScope(
    string PlantId,
    string? WarehouseCode,
    string? LocationCode,
    string? MaterialCode,
    string? LotNumber,
    string? StockStatus,
    string? Q);

public sealed class SearchMasterStockQueryHandler : IQueryHandler<SearchMasterStockQuery, Result<PagedMasterStockDto>>
{
    private readonly IMasterStockReadRepository _read;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;

    public SearchMasterStockQueryHandler(
        IMasterStockReadRepository read,
        IWarehouseRepository warehouses,
        ILocationRepository locations)
    {
        _read = read;
        _warehouses = warehouses;
        _locations = locations;
    }

    public async Task<Result<PagedMasterStockDto>> HandleAsync(
        SearchMasterStockQuery query,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var tz = MasterStockProjection.ResolveTimeZone();
        if (MasterStockProjection.IsHistoricalAsOf(query.AsOfDate, now, tz))
            return Result.Failure<PagedMasterStockDto>(Error.Validation(
                "INV-MST-ASOF",
                "Geçmiş tarih stok raporu henüz desteklenmiyor. Güncel stok InventoryBalance üzerinden okunur."));

        var scope = await MasterStockScope.ResolveAsync(
            query.PlantId, query.WarehouseCode, query.LocationCode, query.MaterialCode, query.LotNumber,
            query.StockStatus, query.Q, query.WarehouseId, query.LocationId, query.AllowedPlantIds,
            _warehouses, _locations, cancellationToken).ConfigureAwait(false);
        if (!scope.Ok) return Result.Failure<PagedMasterStockDto>(scope.Error!);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var filter = scope.Filter!;
        var desc = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var (items, total) = await _read.SearchBalancesAsync(
            filter, page, pageSize, query.SortBy ?? "materialCode", desc, cancellationToken).ConfigureAwait(false);

        var rows = await MasterStockAssembler.AssembleRowsAsync(_read, filter.PlantId, items, cancellationToken)
            .ConfigureAwait(false);

        var allForTotals = await _read.ListAllBalancesAsync(filter, cancellationToken).ConfigureAwait(false);
        var totalRows = await MasterStockAssembler.AssembleRowsAsync(_read, filter.PlantId, allForTotals, cancellationToken)
            .ConfigureAwait(false);
        var pkgCount = await _read.CountPackagesAsync(filter, cancellationToken).ConfigureAwait(false);

        return Result.Success(new PagedMasterStockDto
        {
            Items = rows,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize),
            Totals = MasterStockProjection.Totals(totalRows, pkgCount),
            ExportGeneratedAt = now,
            AsOfDate = query.AsOfDate
        });
    }
}

public sealed class SearchMasterStockPackagesQueryHandler
    : IQueryHandler<SearchMasterStockPackagesQuery, Result<PagedMasterStockPackageDto>>
{
    private readonly IMasterStockReadRepository _read;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;

    public SearchMasterStockPackagesQueryHandler(
        IMasterStockReadRepository read,
        IWarehouseRepository warehouses,
        ILocationRepository locations)
    {
        _read = read;
        _warehouses = warehouses;
        _locations = locations;
    }

    public async Task<Result<PagedMasterStockPackageDto>> HandleAsync(
        SearchMasterStockPackagesQuery query,
        CancellationToken cancellationToken = default)
    {
        var tz = MasterStockProjection.ResolveTimeZone();
        if (MasterStockProjection.IsHistoricalAsOf(query.AsOfDate, DateTimeOffset.UtcNow, tz))
            return Result.Failure<PagedMasterStockPackageDto>(Error.Validation(
                "INV-MST-ASOF",
                "Geçmiş tarih stok raporu henüz desteklenmiyor. Güncel stok InventoryBalance üzerinden okunur."));

        var scope = await MasterStockScope.ResolveAsync(
            query.PlantId, query.WarehouseCode, query.LocationCode, query.MaterialCode, query.LotNumber,
            query.StockStatus, query.Q, query.WarehouseId, query.LocationId, query.AllowedPlantIds,
            _warehouses, _locations, cancellationToken).ConfigureAwait(false);
        if (!scope.Ok) return Result.Failure<PagedMasterStockPackageDto>(scope.Error!);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _read.SearchPackagesAsync(scope.Filter!, page, pageSize, cancellationToken)
            .ConfigureAwait(false);
        var rows = await MasterStockAssembler.AssemblePackagesAsync(_read, scope.Filter!.PlantId, items, cancellationToken)
            .ConfigureAwait(false);
        return Result.Success(new PagedMasterStockPackageDto
        {
            Items = rows,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetMasterStockBalancePackagesQueryHandler
    : IQueryHandler<GetMasterStockBalancePackagesQuery, Result<IReadOnlyList<MasterStockPackageRowDto>>>
{
    private readonly IInventoryBalanceRepository _balances;
    private readonly IMasterStockReadRepository _read;

    public GetMasterStockBalancePackagesQueryHandler(
        IInventoryBalanceRepository balances,
        IMasterStockReadRepository read)
    {
        _balances = balances;
        _read = read;
    }

    public async Task<Result<IReadOnlyList<MasterStockPackageRowDto>>> HandleAsync(
        GetMasterStockBalancePackagesQuery query,
        CancellationToken cancellationToken = default)
    {
        var balance = await _balances.GetByIdAsync(query.BalanceId, cancellationToken).ConfigureAwait(false);
        if (balance is null || balance.IsDeleted)
            return Result.Failure<IReadOnlyList<MasterStockPackageRowDto>>(
                Error.NotFound("BUS-001", "InventoryBalance was not found."));
        if (query.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(query.AllowedPlantIds, balance.PlantId))
        {
            return Result.Failure<IReadOnlyList<MasterStockPackageRowDto>>(Error.Forbidden(
                "INV-MST-403",
                "Bu tesisin stok bakiyelerini görüntüleme yetkiniz yok."));
        }

        var pkgs = await _read.ListPackagesForBalanceAsync(
            balance.PlantId ?? "",
            balance.WarehouseCode,
            balance.LocationCode,
            balance.MaterialCode,
            balance.BatchNumber,
            cancellationToken).ConfigureAwait(false);
        var rows = await MasterStockAssembler.AssemblePackagesAsync(
            _read, balance.PlantId ?? "", pkgs, cancellationToken, balance.Id).ConfigureAwait(false);
        return Result.Success(rows);
    }
}

public sealed class ExportMasterStockQueryHandler : IQueryHandler<ExportMasterStockQuery, Result<MasterStockExportDto>>
{
    private readonly IMasterStockReadRepository _read;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;

    public ExportMasterStockQueryHandler(
        IMasterStockReadRepository read,
        IWarehouseRepository warehouses,
        ILocationRepository locations)
    {
        _read = read;
        _warehouses = warehouses;
        _locations = locations;
    }

    public async Task<Result<MasterStockExportDto>> HandleAsync(
        ExportMasterStockQuery query,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var tz = MasterStockProjection.ResolveTimeZone();
        if (MasterStockProjection.IsHistoricalAsOf(query.AsOfDate, now, tz))
            return Result.Failure<MasterStockExportDto>(Error.Validation(
                "INV-MST-ASOF",
                "Geçmiş tarih stok raporu henüz desteklenmiyor. Güncel stok InventoryBalance üzerinden okunur."));

        var scope = await MasterStockScope.ResolveAsync(
            query.PlantId, query.WarehouseCode, query.LocationCode, query.MaterialCode, query.LotNumber,
            query.StockStatus, query.Q, query.WarehouseId, query.LocationId, query.AllowedPlantIds,
            _warehouses, _locations, cancellationToken).ConfigureAwait(false);
        if (!scope.Ok) return Result.Failure<MasterStockExportDto>(scope.Error!);

        var filter = scope.Filter!;
        var balances = await _read.ListAllBalancesAsync(filter, cancellationToken).ConfigureAwait(false);
        var stockRows = await MasterStockAssembler.AssembleRowsAsync(_read, filter.PlantId, balances, cancellationToken)
            .ConfigureAwait(false);
        var packages = await _read.ListAllPackagesAsync(filter, cancellationToken).ConfigureAwait(false);
        var packageRows = await MasterStockAssembler.AssemblePackagesAsync(_read, filter.PlantId, packages, cancellationToken)
            .ConfigureAwait(false);

        var (date, time) = MasterStockProjection.ReportDateTime(now, tz);
        var fileName = MasterStockProjection.FileName(now, tz);
        var totals = MasterStockProjection.Totals(stockRows, packageRows.Count);

        return Result.Success(new MasterStockExportDto
        {
            FileName = fileName,
            GeneratedAt = now,
            StockRows = stockRows,
            PackageRows = packageRows,
            Totals = totals,
            Report = new MasterStockReportInfoDto
            {
                GeneratedAt = now,
                ReportDate = date,
                ReportTime = time,
                TimeZone = MasterStockProjection.ReportTimeZoneId,
                Factory = filter.PlantId,
                WarehouseFilter = MasterStockProjection.FilterLabel(filter.WarehouseCode),
                LocationFilter = MasterStockProjection.FilterLabel(filter.LocationCode),
                StockStatusFilter = MasterStockProjection.FilterLabel(filter.StockStatus),
                MaterialFilter = MasterStockProjection.FilterLabel(filter.MaterialCode ?? filter.Q),
                LotFilter = MasterStockProjection.FilterLabel(filter.LotNumber),
                PreparedBy = string.IsNullOrWhiteSpace(query.PreparedBy) ? "unknown" : query.PreparedBy.Trim(),
                TotalStockRows = stockRows.Count,
                TotalPackages = packageRows.Count,
                ScopeNote = "Güncel stok (InventoryBalance). Geçmiş tarih (as-of) hesaplanmadı. Paket listesi bağımsız kaynaktır; miktar paketten türetilmez."
            }
        });
    }
}

internal static class MasterStockScope
{
    public sealed record ResultBox(bool Ok, MasterStockQueryFilter? Filter, Error? Error);

    public static async Task<ResultBox> ResolveAsync(
        string plantId,
        string? warehouseCode,
        string? locationCode,
        string? materialCode,
        string? lotNumber,
        string? stockStatus,
        string? q,
        Guid? warehouseId,
        Guid? locationId,
        IReadOnlyList<string>? allowedPlantIds,
        IWarehouseRepository warehouses,
        ILocationRepository locations,
        CancellationToken cancellationToken)
    {
        var plant = string.IsNullOrWhiteSpace(plantId) ? null : plantId.Trim();
        if (string.IsNullOrWhiteSpace(plant))
            return new ResultBox(false, null, Error.Validation("INV-BAL-004", "Plant / factory context is required."));

        if (allowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(allowedPlantIds, plant))
            return new ResultBox(false, null, Error.Forbidden("INV-MST-403", "Bu tesisin stok bakiyelerini görüntüleme yetkiniz yok."));

        var whCode = string.IsNullOrWhiteSpace(warehouseCode) ? null : warehouseCode.Trim();
        var locCode = string.IsNullOrWhiteSpace(locationCode) ? null : locationCode.Trim();

        if (warehouseId is Guid wid)
        {
            var whById = await warehouses.GetByIdAsync(wid, cancellationToken).ConfigureAwait(false);
            if (whById is null || whById.IsDeleted)
                return new ResultBox(false, null, Error.Validation("INV-BAL-017", "Warehouse was not found."));
            if (!string.IsNullOrWhiteSpace(whById.PlantId)
                && !string.Equals(whById.PlantId, plant, StringComparison.OrdinalIgnoreCase))
                return new ResultBox(false, null, Error.Forbidden("INV-MST-403", "Warehouse seçili fabrikaya ait değil."));
            if (allowedPlantIds is { Count: > 0 }
                && !string.IsNullOrWhiteSpace(whById.PlantId)
                && !PlantAccess.CanAccess(allowedPlantIds, whById.PlantId))
                return new ResultBox(false, null, Error.Forbidden("INV-MST-403", "Bu deponun stok bakiyelerini görüntüleme yetkiniz yok."));
            whCode = whById.Code;
        }
        else if (!string.IsNullOrWhiteSpace(whCode))
        {
            var wh = await warehouses.GetByCodeAndPlantAsync(whCode, plant, cancellationToken).ConfigureAwait(false);
            if (wh is null || wh.IsDeleted)
                return new ResultBox(false, null, Error.Validation("INV-BAL-017", $"Depo '{whCode}' bu tesiste ({plant}) tanımlı değil."));
            whCode = wh.Code;
        }

        if (locationId is Guid lid)
        {
            var locById = await locations.GetByIdAsync(lid, cancellationToken).ConfigureAwait(false);
            if (locById is null || locById.IsDeleted)
                return new ResultBox(false, null, Error.Validation("INV-BAL-019", "Location was not found."));
            if (!string.IsNullOrWhiteSpace(locById.PlantId)
                && !string.Equals(locById.PlantId, plant, StringComparison.OrdinalIgnoreCase))
                return new ResultBox(false, null, Error.Forbidden("INV-MST-403", "Location seçili fabrikaya ait değil."));
            if (!string.IsNullOrWhiteSpace(whCode)
                && !string.Equals(locById.WarehouseCode, whCode, StringComparison.OrdinalIgnoreCase))
                return new ResultBox(false, null, Error.Validation("INV-BAL-020", "Location seçilen depoya ait değil."));
            whCode ??= locById.WarehouseCode;
            locCode = locById.Code;
        }
        else if (!string.IsNullOrWhiteSpace(locCode))
        {
            if (string.IsNullOrWhiteSpace(whCode))
                return new ResultBox(false, null, Error.Validation("INV-BAL-018", "Location filtresi için Warehouse zorunludur."));
            var loc = await locations.FindByWarehouseAndCodeAsync(whCode, locCode, plant, cancellationToken).ConfigureAwait(false);
            if (loc is null || loc.IsDeleted)
                return new ResultBox(false, null, Error.Validation("INV-BAL-019", $"Lokasyon '{locCode}' depo '{whCode}' / tesis '{plant}' altında tanımlı değil."));
            locCode = loc.Code;
        }

        return new ResultBox(true, new MasterStockQueryFilter(
            plant,
            whCode,
            locCode,
            string.IsNullOrWhiteSpace(materialCode) ? null : materialCode.Trim(),
            string.IsNullOrWhiteSpace(lotNumber) ? null : lotNumber.Trim(),
            string.IsNullOrWhiteSpace(stockStatus) ? null : stockStatus.Trim(),
            string.IsNullOrWhiteSpace(q) ? null : q.Trim()), null);
    }
}

internal static class MasterStockAssembler
{
    public static async Task<IReadOnlyList<MasterStockRowDto>> AssembleRowsAsync(
        IMasterStockReadRepository read,
        string plantId,
        IReadOnlyList<InventoryBalance> balances,
        CancellationToken cancellationToken)
    {
        if (balances.Count == 0) return Array.Empty<MasterStockRowDto>();

        var codes = balances.Select(b => b.MaterialCode).Distinct().ToArray();
        var materials = (await read.ListMaterialsByCodesAsync(codes, cancellationToken).ConfigureAwait(false))
            .ToDictionary(m => m.Code, m => m, StringComparer.OrdinalIgnoreCase);
        var whNames = await read.WarehouseNamesAsync(plantId, cancellationToken).ConfigureAwait(false);
        var locNames = await read.LocationNamesAsync(plantId, null, cancellationToken).ConfigureAwait(false);
        var aggs = await read.PackageAggregatesAsync(plantId, null, null, cancellationToken).ConfigureAwait(false);
        var aggMap = aggs.ToDictionary(
            a => MasterStockProjection.PackageMatchKey(a.PlantId, a.WarehouseCode, a.LocationCode, a.MaterialCode, a.LotNumber),
            a => a,
            StringComparer.OrdinalIgnoreCase);
        var phys = await read.LatestPhysNotesAsync(plantId, codes, cancellationToken).ConfigureAwait(false);
        var physByKey = new Dictionary<string, MasterStockPhysNote>(StringComparer.OrdinalIgnoreCase);
        foreach (var n in phys)
        {
            var key = MasterStockProjection.PackageMatchKey(n.PlantId, n.WarehouseCode, n.LocationCode, n.MaterialCode, n.LotNumber);
            if (!physByKey.ContainsKey(key)) physByKey[key] = n;
        }

        var rows = new List<MasterStockRowDto>(balances.Count);
        foreach (var b in balances)
        {
            materials.TryGetValue(b.MaterialCode, out var mat);
            whNames.TryGetValue(b.WarehouseCode, out var whName);
            locNames.TryGetValue($"{b.WarehouseCode}|{b.LocationCode}", out var locName);
            var key = MasterStockProjection.PackageMatchKey(b.PlantId ?? "", b.WarehouseCode, b.LocationCode, b.MaterialCode, b.BatchNumber);
            aggMap.TryGetValue(key, out var agg);
            physByKey.TryGetValue(key, out var note);
            var (t, w, l, label) = MasterStockProjection.ParseActualDims(note?.Notes);
            rows.Add(MasterStockProjection.ToRow(
                b,
                mat,
                whName ?? b.WarehouseCode,
                locName ?? b.LocationCode,
                agg?.PackageCount ?? 0,
                agg?.QuantitySum ?? 0,
                label,
                t,
                w,
                l));
        }
        return rows;
    }

    public static async Task<IReadOnlyList<MasterStockPackageRowDto>> AssemblePackagesAsync(
        IMasterStockReadRepository read,
        string plantId,
        IReadOnlyList<InventoryPackage> packages,
        CancellationToken cancellationToken,
        Guid? balanceId = null)
    {
        if (packages.Count == 0) return Array.Empty<MasterStockPackageRowDto>();
        var codes = packages.Select(p => p.MaterialCode).Distinct().ToArray();
        var materials = (await read.ListMaterialsByCodesAsync(codes, cancellationToken).ConfigureAwait(false))
            .ToDictionary(m => m.Code, m => m, StringComparer.OrdinalIgnoreCase);
        var whNames = await read.WarehouseNamesAsync(plantId, cancellationToken).ConfigureAwait(false);
        var locNames = await read.LocationNamesAsync(plantId, null, cancellationToken).ConfigureAwait(false);
        var phys = await read.LatestPhysNotesAsync(plantId, codes, cancellationToken).ConfigureAwait(false);
        var physByPkg = new Dictionary<string, MasterStockPhysNote>(StringComparer.OrdinalIgnoreCase);
        var physByKey = new Dictionary<string, MasterStockPhysNote>(StringComparer.OrdinalIgnoreCase);
        foreach (var n in phys)
        {
            if (!string.IsNullOrWhiteSpace(n.PackageNumber) && !physByPkg.ContainsKey(n.PackageNumber))
                physByPkg[n.PackageNumber] = n;
            var key = MasterStockProjection.PackageMatchKey(n.PlantId, n.WarehouseCode, n.LocationCode, n.MaterialCode, n.LotNumber);
            if (!physByKey.ContainsKey(key)) physByKey[key] = n;
        }

        var contents = await read.ListContentsForPackagesAsync(packages.Select(p => p.Id).ToArray(), cancellationToken)
            .ConfigureAwait(false);
        var contentsByPkg = contents
            .GroupBy(c => c.PackageId)
            .ToDictionary(g => g.Key, g => g.Select(PackagePassportComposer.ToContent).ToArray());

        var rows = new List<MasterStockPackageRowDto>(packages.Count);
        foreach (var p in packages)
        {
            materials.TryGetValue(p.MaterialCode, out var mat);
            whNames.TryGetValue(p.WarehouseCode, out var whName);
            locNames.TryGetValue($"{p.WarehouseCode}|{p.LocationCode}", out var locName);
            physByPkg.TryGetValue(p.PackageNumber, out var note);
            if (note is null)
            {
                var key = MasterStockProjection.PackageMatchKey(p.PlantId ?? "", p.WarehouseCode, p.LocationCode, p.MaterialCode, p.LotNumber);
                physByKey.TryGetValue(key, out note);
            }
            var (t, w, l, label) = MasterStockProjection.ParseActualDims(note?.Notes);
            contentsByPkg.TryGetValue(p.Id, out var pkgContents);
            rows.Add(MasterStockProjection.ToPackageRow(
                p,
                balanceId,
                mat,
                whName ?? p.WarehouseCode,
                locName ?? p.LocationCode,
                label,
                t,
                w,
                l,
                physicalGroupLabel: p.PhysicalGroupLabel,
                contents: pkgContents));
        }
        return rows;
    }
}

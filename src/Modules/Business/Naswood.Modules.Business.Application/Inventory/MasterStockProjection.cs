using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

/// <summary>
/// Pure master-stock projection over existing ledger entities.
/// Does not write balances. Packages never become the quantity source.
/// </summary>
public static class MasterStockProjection
{
    public const string ReportTimeZoneId = "Europe/Istanbul";

    public static string BalanceGroupKey(
        string plantId,
        string warehouseCode,
        string locationCode,
        string materialCode,
        string lot,
        string stockStatus)
        => string.Join('|',
            (plantId ?? "").Trim().ToUpperInvariant(),
            (warehouseCode ?? "").Trim().ToUpperInvariant(),
            (locationCode ?? "").Trim().ToUpperInvariant(),
            (materialCode ?? "").Trim().ToUpperInvariant(),
            (lot ?? "").Trim().ToUpperInvariant(),
            NormalizeDisplayedStatus(stockStatus));

    public static string PackageMatchKey(
        string plantId,
        string warehouseCode,
        string locationCode,
        string materialCode,
        string lot)
        => string.Join('|',
            (plantId ?? "").Trim().ToUpperInvariant(),
            (warehouseCode ?? "").Trim().ToUpperInvariant(),
            (locationCode ?? "").Trim().ToUpperInvariant(),
            (materialCode ?? "").Trim().ToUpperInvariant(),
            (lot ?? "").Trim().ToUpperInvariant());

    public static string NormalizeDisplayedStatus(string? status)
    {
        var s = (status ?? "").Trim();
        if (s.Equals("Active", StringComparison.OrdinalIgnoreCase)) return "Available";
        return string.IsNullOrEmpty(s) ? "Available" : s;
    }

    public static bool StatusMatchesFilter(string displayedStatus, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter) || filter.Trim().Equals("Tümü", StringComparison.OrdinalIgnoreCase))
            return true;
        var f = filter.Trim();
        if (f.Equals("Available", StringComparison.OrdinalIgnoreCase)
            || f.Equals("Active", StringComparison.OrdinalIgnoreCase))
        {
            return displayedStatus.Equals("Available", StringComparison.OrdinalIgnoreCase)
                || displayedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase);
        }
        return displayedStatus.Equals(f, StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasPackageBalanceMismatch(int packageCount, decimal packageQtySum, decimal balanceQty)
    {
        if (packageCount <= 0) return false;
        return packageQtySum != balanceQty;
    }

    public static bool IsHistoricalAsOf(DateTimeOffset? asOf, DateTimeOffset nowUtc, TimeZoneInfo tz)
    {
        if (asOf is null) return false;
        var asOfLocal = TimeZoneInfo.ConvertTime(asOf.Value, tz).Date;
        var todayLocal = TimeZoneInfo.ConvertTime(nowUtc, tz).Date;
        return asOfLocal < todayLocal;
    }

    public static TimeZoneInfo ResolveTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(ReportTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.Utc;
            }
        }
    }

    public static string FileName(DateTimeOffset generatedAt, TimeZoneInfo tz)
    {
        var local = TimeZoneInfo.ConvertTime(generatedAt, tz);
        return $"NASWOOD_Stok_Listesi_{local:yyyy-MM-dd}_{local:HHmm}.xlsx";
    }

    public static (string Date, string Time) ReportDateTime(DateTimeOffset generatedAt, TimeZoneInfo tz)
    {
        var local = TimeZoneInfo.ConvertTime(generatedAt, tz);
        return (local.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            local.ToString("HH:mm", CultureInfo.InvariantCulture));
    }

    public static string FilterLabel(string? value)
        => string.IsNullOrWhiteSpace(value) ? "Tümü" : value.Trim();

    public static string WoodSpeciesFromDefinition(string? definitionJson, string? description)
    {
        var def = ParseObject(definitionJson);
        var fromJson = ReadString(def, "woodSpecies", "WoodSpecies", "species", "woodToken");
        if (!string.IsNullOrWhiteSpace(fromJson)) return fromJson.Trim();
        return string.Empty;
    }

    public static string StockUnitOf(Material? material)
    {
        if (material is null) return "PCS";
        var policy = InventoryCountMath.ResolvePolicy(material.UnitOfMeasure, material.Category, material.DefinitionJson);
        return policy.StockUnit;
    }

    public static decimal? PieceCountFor(string stockUnit, decimal stockQuantity)
        => InventoryCountMath.NormalizeUnit(stockUnit) == "PCS" ? stockQuantity : null;

    public static (decimal? T, decimal? W, decimal? L, string Label) ParseActualDims(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes)) return (null, null, null, string.Empty);
        var m = Regex.Match(notes, @"phys\s*=\s*([0-9]+(?:[.,][0-9]+)?)\s*[x×]\s*([0-9]+(?:[.,][0-9]+)?)\s*[x×]\s*([0-9]+(?:[.,][0-9]+)?)",
            RegexOptions.IgnoreCase);
        if (!m.Success) return (null, null, null, string.Empty);
        var t = ParseDec(m.Groups[1].Value);
        var w = ParseDec(m.Groups[2].Value);
        var l = ParseDec(m.Groups[3].Value);
        if (t is null || w is null || l is null) return (null, null, null, string.Empty);
        var label = $"{t.Value.ToString("0.####", CultureInfo.InvariantCulture)}×{w.Value.ToString("0.####", CultureInfo.InvariantCulture)}×{l.Value.ToString("0.####", CultureInfo.InvariantCulture)}";
        return (t, w, l, label);
    }

    public static MasterStockRowDto ToRow(
        InventoryBalance balance,
        Material? material,
        string warehouseName,
        string locationName,
        int packageCount,
        decimal packageQtySum,
        string actualMeasurement,
        decimal? t,
        decimal? w,
        decimal? l)
    {
        var unit = StockUnitOf(material);
        var qty = balance.QuantityOnHand;
        var status = NormalizeDisplayedStatus(balance.Status);
        return new MasterStockRowDto
        {
            BalanceId = balance.Id,
            PlantId = balance.PlantId ?? string.Empty,
            MaterialCode = balance.MaterialCode,
            MaterialName = material?.Name ?? string.Empty,
            WoodSpecies = WoodSpeciesFromDefinition(material?.DefinitionJson, material?.Description),
            ActualMeasurement = actualMeasurement,
            ActualThicknessMm = t,
            ActualWidthMm = w,
            ActualLengthMm = l,
            Lot = balance.BatchNumber ?? string.Empty,
            Factory = balance.PlantId ?? string.Empty,
            WarehouseCode = balance.WarehouseCode,
            Warehouse = string.IsNullOrWhiteSpace(warehouseName) ? balance.WarehouseCode : warehouseName,
            LocationCode = balance.LocationCode,
            Location = string.IsNullOrWhiteSpace(locationName) ? balance.LocationCode : locationName,
            PieceCount = PieceCountFor(unit, qty),
            PackageCount = packageCount,
            StockUnit = unit,
            StockQuantity = qty,
            QuantityReserved = balance.QuantityReserved,
            QuantityAvailable = Math.Max(0, qty - balance.QuantityReserved),
            StockStatus = status,
            PackageBalanceMismatch = HasPackageBalanceMismatch(packageCount, packageQtySum, qty),
            PackageQuantitySum = packageCount > 0 ? packageQtySum : null
        };
    }

    public static MasterStockPackageRowDto ToPackageRow(
        InventoryPackage pkg,
        Guid? balanceId,
        Material? material,
        string warehouseName,
        string locationName,
        string actualMeasurement,
        decimal? t,
        decimal? w,
        decimal? l,
        string physicalGroupLabel,
        IReadOnlyList<PackageContentDto>? contents = null)
    {
        var unit = string.IsNullOrWhiteSpace(pkg.UnitOfMeasure)
            ? StockUnitOf(material)
            : InventoryCountMath.NormalizeUnit(pkg.UnitOfMeasure);
        var rows = contents ?? Array.Empty<PackageContentDto>();
        var measure = rows.Count > 0
            ? string.Join(" · ", rows.Select(c => c.Measurement).Where(s => !string.IsNullOrWhiteSpace(s)))
            : actualMeasurement;
        var first = rows.Count > 0 ? rows[0] : null;
        var pieces = rows.Count > 0 ? rows.Sum(c => c.PieceCount ?? 0) : PieceCountFor(unit, pkg.Quantity);
        return new MasterStockPackageRowDto
        {
            Id = pkg.Id,
            BalanceId = balanceId,
            PackageNo = pkg.PackageNumber,
            PhysicalGroupLabel = physicalGroupLabel,
            MaterialCode = pkg.MaterialCode,
            MaterialName = material?.Name ?? string.Empty,
            ActualMeasurement = measure,
            ActualThicknessMm = first?.ThicknessMm ?? t,
            ActualWidthMm = first?.WidthMm ?? w,
            ActualLengthMm = first?.LengthMm ?? l,
            Lot = pkg.LotNumber,
            Factory = pkg.PlantId ?? string.Empty,
            WarehouseCode = pkg.WarehouseCode,
            Warehouse = string.IsNullOrWhiteSpace(warehouseName) ? pkg.WarehouseCode : warehouseName,
            LocationCode = pkg.LocationCode,
            Location = string.IsNullOrWhiteSpace(locationName) ? pkg.LocationCode : locationName,
            PieceCount = pieces,
            StockUnit = unit,
            StockQuantity = pkg.Quantity,
            Status = pkg.Status,
            Barcode = pkg.Barcode,
            MaterialIdentityNumber = pkg.MaterialIdentityNumber,
            Contents = rows
        };
    }

    public static MasterStockTotalsDto Totals(IReadOnlyList<MasterStockRowDto> rows, int packageCount)
    {
        var byUnit = rows
            .GroupBy(r => InventoryCountMath.NormalizeUnit(r.StockUnit), StringComparer.OrdinalIgnoreCase)
            .Select(g => new MasterStockUnitTotalDto
            {
                Unit = g.Key,
                Quantity = g.Sum(x => x.StockQuantity)
            })
            .OrderBy(x => x.Unit)
            .ToArray();
        return new MasterStockTotalsDto
        {
            MaterialRowCount = rows.Count,
            PackageCount = packageCount,
            ByUnit = byUnit
        };
    }

    private static decimal? ParseDec(string raw)
    {
        if (decimal.TryParse(raw.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var d) && d > 0)
            return d;
        return null;
    }

    private static Dictionary<string, JsonElement>? ParseObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;
            return doc.RootElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.Clone(), StringComparer.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? ReadString(Dictionary<string, JsonElement>? def, params string[] keys)
    {
        if (def is null) return null;
        foreach (var key in keys)
        {
            if (!def.TryGetValue(key, out var el)) continue;
            if (el.ValueKind == JsonValueKind.String) return el.GetString();
            if (el.ValueKind is JsonValueKind.Number) return el.GetRawText();
        }
        return null;
    }
}

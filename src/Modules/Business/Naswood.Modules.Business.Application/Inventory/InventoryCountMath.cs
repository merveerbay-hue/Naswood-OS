using System.Globalization;
using System.Text.Json;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public enum CountQtyMode
{
    Piece,
    CubicMeter,
    SquareMeter,
    MeasuredVolume
}

public sealed record MaterialCountPolicy(
    string StockUnit,
    string CountUnit,
    CountQtyMode Mode,
    bool DimsRequired);

public static class InventoryCountMath
{
    public const decimal Billion = 1_000_000_000m;
    public const decimal Million = 1_000_000m;

    public static decimal CubicMeters(decimal thicknessMm, decimal widthMm, decimal lengthMm, decimal pieceCount)
        => thicknessMm * widthMm * lengthMm * pieceCount / Billion;

    public static decimal SquareMeters(decimal widthMm, decimal lengthMm, decimal pieceCount)
        => widthMm * lengthMm * pieceCount / Million;

    public static decimal Difference(decimal countedQuantity, decimal systemQuantity)
        => countedQuantity - systemQuantity;

    public static string NormalizeUnit(string? unit)
    {
        var u = (unit ?? string.Empty).Trim().ToUpperInvariant()
            .Replace("M³", "M3", StringComparison.Ordinal)
            .Replace("M²", "M2", StringComparison.Ordinal);
        return u switch
        {
            "PIECE" or "PCS" or "ADET" or "EA" => "PCS",
            "M3" or "M³" or "CBM" => "M3",
            "M2" or "M²" or "SQM" => "M2",
            _ => string.IsNullOrWhiteSpace(u) ? "PCS" : u
        };
    }

    public static MaterialCountPolicy ResolvePolicy(string? unitOfMeasure, string? category, string? definitionJson)
    {
        var def = ParseObject(definitionJson);
        var stock = NormalizeUnit(ReadString(def, "stockUom", "StockUom") ?? unitOfMeasure);
        var count = NormalizeUnit(ReadString(def, "countUom", "CountUom") ?? "PCS");
        var main = (ReadString(def, "mainCategory", "MainCategory") ?? category ?? string.Empty).Trim().ToUpperInvariant();
        var type = (ReadString(def, "materialType", "materialTypeToken") ?? string.Empty).Trim().ToUpperInvariant();
        var volumeReq = ReadBool(def, "volumeCalcRequired", "VolumeCalcRequired");
        var cat = (category ?? string.Empty).Trim().ToUpperInvariant();

        var isLog = main is "LOG" or "TOMRUK" or "LG"
            || type.Contains("TOMRUK", StringComparison.Ordinal)
            || type.Contains("LOG", StringComparison.Ordinal)
            || cat.Contains("TOMRUK", StringComparison.Ordinal)
            || cat.Contains("LOG", StringComparison.Ordinal);

        var isHardware = main is "HW" or "EL" or "MK" or "HARDWARE"
            || cat.Contains("HIRDAVAT", StringComparison.Ordinal)
            || cat.Contains("HARDWARE", StringComparison.Ordinal)
            || cat.Contains("ELEKTRIK", StringComparison.Ordinal)
            || cat.Contains("MEKANIK", StringComparison.Ordinal);

        var isMasifPanel = main is "MP" or "PANEL"
            || cat.Contains("MASIF", StringComparison.Ordinal);

        if (isLog)
            return new MaterialCountPolicy("M3", count == "PCS" ? "PCS" : count, CountQtyMode.MeasuredVolume, DimsRequired: false);

        if (isHardware || (stock == "PCS" && volumeReq != true && !isMasifPanel))
            return new MaterialCountPolicy(stock, count, CountQtyMode.Piece, DimsRequired: false);

        // Masif panel stock UoM is m³ (T×W×L×adet). Thermowood / explicit M2 stay m².
        if (isMasifPanel)
            return new MaterialCountPolicy("M3", count, CountQtyMode.CubicMeter, DimsRequired: true);

        if (stock == "M2")
            return new MaterialCountPolicy("M2", count, CountQtyMode.SquareMeter, DimsRequired: true);

        if (stock == "M3" || volumeReq == true)
            return new MaterialCountPolicy("M3", count, CountQtyMode.CubicMeter, DimsRequired: true);

        return new MaterialCountPolicy(stock, count, CountQtyMode.Piece, DimsRequired: false);
    }

    public static (bool Ok, string? Error, decimal StockQty) CalculateStockQty(
        MaterialCountPolicy policy,
        decimal? thicknessMm,
        decimal? widthMm,
        decimal? lengthMm,
        decimal? pieceCount,
        decimal? measuredVolumeM3)
    {
        if (pieceCount is < 0)
            return (false, "Fiziksel adet negatif olamaz.", 0);

        switch (policy.Mode)
        {
            case CountQtyMode.Piece:
                if (pieceCount is null)
                    return (false, "Adet gerekli.", 0);
                return (true, null, pieceCount.Value);

            case CountQtyMode.CubicMeter:
                if (thicknessMm is null or <= 0 || widthMm is null or <= 0 || lengthMm is null or <= 0 || pieceCount is null)
                    return (false, "Kereste/lata/lamel sayımı için kalınlık, genişlik, boy ve adet gerekli.", 0);
                return (true, null, CubicMeters(thicknessMm.Value, widthMm.Value, lengthMm.Value, pieceCount.Value));

            case CountQtyMode.SquareMeter:
                if (widthMm is null or <= 0 || lengthMm is null or <= 0 || pieceCount is null)
                    return (false, "m² stok (ör. thermowood) için genişlik, boy ve adet gerekli.", 0);
                return (true, null, SquareMeters(widthMm.Value, lengthMm.Value, pieceCount.Value));

            case CountQtyMode.MeasuredVolume:
                if (measuredVolumeM3 is null or < 0)
                    return (false, "Tomruk sayımı için MeasuredVolumeM3 gerekli.", 0);
                if (pieceCount is null)
                    return (false, "Tomruk sayımı için adet gerekli.", 0);
                return (true, null, measuredVolumeM3.Value);

            default:
                return (false, "Bilinmeyen stok birimi kuralı.", 0);
        }
    }

    public static string PhysicalKey(
        string materialCode,
        string locationCode,
        string lot,
        decimal? thicknessMm,
        decimal? widthMm,
        decimal? lengthMm)
    {
        static string D(decimal? v) => v is null ? "" : v.Value.ToString("0.####", CultureInfo.InvariantCulture);
        return string.Join('|',
            (materialCode ?? string.Empty).Trim().ToUpperInvariant(),
            (locationCode ?? string.Empty).Trim().ToUpperInvariant(),
            NormalizeLot(lot),
            D(thicknessMm),
            D(widthMm),
            D(lengthMm));
    }

    public static string GroupKey(string materialCode, string locationCode, string lot) =>
        string.Join('|',
            (materialCode ?? string.Empty).Trim().ToUpperInvariant(),
            (locationCode ?? string.Empty).Trim().ToUpperInvariant(),
            NormalizeLot(lot));

    public static string NormalizeLot(string? lot)
    {
        var v = (lot ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(v)) return InventoryCountLots.Unknown;
        return v.ToUpperInvariant();
    }

    public static string LineStatus(decimal systemQty, decimal countedQty, bool hasPhysical)
    {
        if (!hasPhysical && systemQty > 0 && countedQty == 0) return "MISSING";
        if (systemQty == 0 && countedQty > 0) return "UNEXPECTED";
        var diff = Difference(countedQty, systemQty);
        if (diff == 0) return "MATCHED";
        return "VARIANCE";
    }

    public static bool IsWipLocationType(string? locationType)
    {
        var t = (locationType ?? string.Empty).Trim().ToUpperInvariant();
        return t is "WIP" or "WORKCENTER" or "WORK_CENTER";
    }

    public static bool IsActiveStatus(string? status)
    {
        var s = (status ?? "Active").Trim();
        return s.Equals("Active", StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, JsonElement>? ParseObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;
            var map = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in doc.RootElement.EnumerateObject())
                map[p.Name] = p.Value.Clone();
            return map;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? ReadString(Dictionary<string, JsonElement>? map, params string[] keys)
    {
        if (map is null) return null;
        foreach (var key in keys)
        {
            if (!map.TryGetValue(key, out var el)) continue;
            if (el.ValueKind == JsonValueKind.String) return el.GetString();
            if (el.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
                return el.ToString();
        }
        return null;
    }

    private static bool? ReadBool(Dictionary<string, JsonElement>? map, params string[] keys)
    {
        if (map is null) return null;
        foreach (var key in keys)
        {
            if (!map.TryGetValue(key, out var el)) continue;
            if (el.ValueKind == JsonValueKind.True) return true;
            if (el.ValueKind == JsonValueKind.False) return false;
            if (el.ValueKind == JsonValueKind.String
                && bool.TryParse(el.GetString(), out var b))
                return b;
        }
        return null;
    }
}

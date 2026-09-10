using System.Globalization;
using System.Text.RegularExpressions;

namespace Naswood.Modules.Business.Application.Inventory;

public static class OpeningInventoryCodes
{
    public const string SourceType = "OPENING_INVENTORY";
    public const string MovementType = "OPENING_INVENTORY";
    public const string LotPrefix = "LOT-OPEN-";
    public const string PackagePrefix = "PKG-";
    public const string BarcodePrefix = "NW-";

    public static string LotDateStamp(DateTimeOffset utc) => utc.UtcDateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

    public static string OpeningLot(DateTimeOffset utc, int ordinal)
        => $"{LotPrefix}{LotDateStamp(utc)}-{ordinal:000}";

    public static string PackageNo(int ordinal) => $"{PackagePrefix}{ordinal:000000}";

    public static string Barcode(string packageNo)
    {
        var pkg = (packageNo ?? string.Empty).Trim();
        if (pkg.StartsWith(BarcodePrefix, StringComparison.OrdinalIgnoreCase)) return pkg.ToUpperInvariant();
        return $"{BarcodePrefix}{pkg}";
    }

    public static int ParseLotOrdinal(string? lotNumber, string yyyymmdd)
    {
        var m = Regex.Match(lotNumber ?? "", $@"^LOT-OPEN-{Regex.Escape(yyyymmdd)}-(\d+)$", RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : 0;
    }

    public static int ParsePackageOrdinal(string? packageNo)
    {
        var m = Regex.Match(packageNo ?? "", @"^PKG-(\d+)$", RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : 0;
    }

    public static int NextOrdinal(IEnumerable<int> existing) =>
        existing.DefaultIfEmpty(0).Max() + 1;
}

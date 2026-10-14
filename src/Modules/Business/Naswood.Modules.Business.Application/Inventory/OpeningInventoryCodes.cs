using System.Globalization;
using System.Text.RegularExpressions;

namespace Naswood.Modules.Business.Application.Inventory;

public static class OpeningInventoryCodes
{
    public const string SourceType = "OPENING_INVENTORY";
    public const string MovementType = "OPENING_INVENTORY";
    public const string LotPrefix = "LOT-OPEN-";
    public const string PackagePrefix = "NW-PKG-";
    public const string LegacyPackagePrefix = "PKG-";
    public const string BarcodePrefix = "NWPKG-";

    public static string FactoryToken(string? plantId)
    {
        var p = (plantId ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(p)) return "F00";
        if (Regex.IsMatch(p, @"^F\d{1,3}$")) return p;
        var digits = Regex.Match(p, @"(\d+)$");
        if (digits.Success && int.TryParse(digits.Groups[1].Value, out var n))
            return "F" + n.ToString("00", CultureInfo.InvariantCulture);
        return p.Length <= 6 ? p : p[..6];
    }

    public static string LotDateStamp(DateTimeOffset utc)
        => utc.UtcDateTime.ToString("yyMMdd", CultureInfo.InvariantCulture);

    public static string YearStamp(DateTimeOffset utc)
        => utc.UtcDateTime.ToString("yy", CultureInfo.InvariantCulture);

    public static string OpeningLot(string? plantId, DateTimeOffset utc, int ordinal)
        => $"{LotPrefix}{FactoryToken(plantId)}-{LotDateStamp(utc)}-{ordinal:0000}";

    public static string PackageNo(string? plantId, DateTimeOffset utc, int ordinal)
        => $"{PackagePrefix}{FactoryToken(plantId)}-{YearStamp(utc)}-{ordinal:000000}";

    public static string Barcode(string packageNo)
    {
        var pkg = (packageNo ?? string.Empty).Trim().ToUpperInvariant();
        if (pkg.StartsWith(BarcodePrefix, StringComparison.Ordinal)) return pkg;
        if (pkg.StartsWith(PackagePrefix, StringComparison.Ordinal))
            return BarcodePrefix + pkg[PackagePrefix.Length..];
        if (pkg.StartsWith(LegacyPackagePrefix, StringComparison.Ordinal))
            return "NW-" + pkg;
        return BarcodePrefix + pkg;
    }

    public static string LotPrefixForDay(string? plantId, DateTimeOffset utc)
        => $"{LotPrefix}{FactoryToken(plantId)}-{LotDateStamp(utc)}-";

    public static string PackagePrefixForYear(string? plantId, DateTimeOffset utc)
        => $"{PackagePrefix}{FactoryToken(plantId)}-{YearStamp(utc)}-";

    public static int ParseLotOrdinal(string? lotNumber, string? plantId, DateTimeOffset utc)
    {
        var factory = FactoryToken(plantId);
        var day = LotDateStamp(utc);
        var m = Regex.Match(lotNumber ?? "", $@"^LOT-OPEN-{Regex.Escape(factory)}-{Regex.Escape(day)}-(\d+)$", RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : 0;
    }

    public static int ParsePackageOrdinal(string? packageNo, string? plantId, DateTimeOffset utc)
    {
        var factory = FactoryToken(plantId);
        var year = YearStamp(utc);
        var m = Regex.Match(
            packageNo ?? "",
            $@"^NW-PKG-{Regex.Escape(factory)}-{Regex.Escape(year)}-(\d+)$",
            RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : 0;
    }

    public static int NextOrdinal(IEnumerable<int> existing) =>
        existing.DefaultIfEmpty(0).Max() + 1;

    public static string QrPath(string publicId) => PackageIdentityService.QrPath(publicId);
}

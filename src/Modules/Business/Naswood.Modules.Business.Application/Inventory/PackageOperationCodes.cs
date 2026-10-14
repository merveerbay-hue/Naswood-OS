using System.Globalization;
using System.Text.RegularExpressions;

namespace Naswood.Modules.Business.Application.Inventory;

public static class PackageOperationCodes
{
    public const string Prefix = "PKOP-";

    public static string Number(string? plantId, DateTimeOffset utc, int ordinal)
        => $"{Prefix}{OpeningInventoryCodes.FactoryToken(plantId)}-{OpeningInventoryCodes.YearStamp(utc)}-{ordinal:000000}";

    public static int ParseOrdinal(string? number, string? plantId, DateTimeOffset utc)
    {
        var factory = OpeningInventoryCodes.FactoryToken(plantId);
        var year = OpeningInventoryCodes.YearStamp(utc);
        var m = Regex.Match(
            number ?? "",
            $@"^PKOP-{Regex.Escape(factory)}-{Regex.Escape(year)}-(\d+)$",
            RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : 0;
    }
}

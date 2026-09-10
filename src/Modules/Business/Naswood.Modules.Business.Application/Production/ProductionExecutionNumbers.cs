using System.Globalization;
using System.Text.RegularExpressions;
using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Modules.Business.Application.Production;

public static class ProductionExecutionNumbers
{
    public const string Prefix = "PEX-";

    public static string Number(string? plantId, DateTimeOffset utc, int ordinal)
        => $"{Prefix}{OpeningInventoryCodes.FactoryToken(plantId)}-{OpeningInventoryCodes.YearStamp(utc)}-{ordinal:000000}";

    public static int ParseOrdinal(string? number, string? plantId, DateTimeOffset utc)
    {
        var factory = OpeningInventoryCodes.FactoryToken(plantId);
        var year = OpeningInventoryCodes.YearStamp(utc);
        var m = Regex.Match(
            number ?? "",
            $@"^PEX-{Regex.Escape(factory)}-{Regex.Escape(year)}-(\d+)$",
            RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : 0;
    }
}

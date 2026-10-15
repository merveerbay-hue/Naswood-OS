using System.Globalization;
using System.Text.RegularExpressions;
using Naswood.Modules.Business.Application.Common;

namespace Naswood.Modules.Business.Application.Production;

public static class ProductionExecutionNumbers
{
    public const string Prefix = "PEX-";

    public static string Number(string? plantId, DateTimeOffset utc, int ordinal)
        => $"{Prefix}{DocumentNumberTokens.FactoryToken(plantId)}-{DocumentNumberTokens.YearStamp(utc)}-{ordinal:000000}";

    public static int ParseOrdinal(string? number, string? plantId, DateTimeOffset utc)
    {
        var factory = DocumentNumberTokens.FactoryToken(plantId);
        var year = DocumentNumberTokens.YearStamp(utc);
        var m = Regex.Match(
            number ?? "",
            $@"^PEX-{Regex.Escape(factory)}-{Regex.Escape(year)}-(\d+)$",
            RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : 0;
    }
}

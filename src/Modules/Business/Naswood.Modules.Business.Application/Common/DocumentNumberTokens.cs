using System.Globalization;
using System.Text.RegularExpressions;

namespace Naswood.Modules.Business.Application.Common;

/// <summary>
/// Plant and year tokens for document numbers (PEX-, PKOP-). Identity codes (PKG/LOT/…) do not use
/// these; they come from <see cref="Inventory.IIdentityCodeAllocator"/>.
/// </summary>
public static class DocumentNumberTokens
{
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

    public static string YearStamp(DateTimeOffset utc)
        => utc.UtcDateTime.ToString("yy", CultureInfo.InvariantCulture);

    public static int NextOrdinal(IEnumerable<int> existing) =>
        existing.DefaultIfEmpty(0).Max() + 1;
}

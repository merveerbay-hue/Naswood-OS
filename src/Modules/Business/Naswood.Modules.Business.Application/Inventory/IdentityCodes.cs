using System.Globalization;
using System.Text.RegularExpressions;

namespace Naswood.Modules.Business.Application.Inventory;

/// <summary>Identity code types shared with Naswood Operations (ADR-002 §6).</summary>
public enum IdentityType
{
    Pkg,
    Lot,
    Lam,
    Auto,
    Trf,
    Rest
}

/// <summary>A parsed identity code: <c>&lt;TYPE&gt;-&lt;SITE&gt;-&lt;SEQ&gt;</c>.</summary>
public sealed record IdentityCode(IdentityType Type, string SiteCode, long Sequence, bool HasSitePrefix)
{
    public bool IsLocal => string.Equals(SiteCode, IdentityCodes.LocalSiteCode, StringComparison.Ordinal);
}

/// <summary>
/// ADR-002 §6 identity contract. Format <c>PKG-BUC-000001</c>; the sequence has at least six digits.
/// The scanner rule is identical in both applications; a code without site prefix belongs to Antalya.
/// </summary>
public static partial class IdentityCodes
{
    public const string LocalSiteCode = "BUC";
    public const string AntalyaSiteCode = "ANT";

    public const string ErrorNoSiteCode = "INV-ID-001";
    public const string ErrorInvalidCode = "INV-ID-002";
    public const string ErrorForeignSite = "INV-ID-ANT";

    public const string ForeignSiteMessage = "Antalya kodu — salt okunur görünüm henüz bağlı değil";

    [GeneratedRegex(@"^(PKG|LOT|LAM|AUTO|TRF|REST)-(?:(ANT|BUC)-)?(\d{6,})$", RegexOptions.CultureInvariant)]
    private static partial Regex ScannerRule();

    public static string Prefix(IdentityType type) => type switch
    {
        IdentityType.Pkg => "PKG",
        IdentityType.Lot => "LOT",
        IdentityType.Lam => "LAM",
        IdentityType.Auto => "AUTO",
        IdentityType.Trf => "TRF",
        IdentityType.Rest => "REST",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static string Format(IdentityType type, string siteCode, long sequence)
    {
        if (sequence <= 0) throw new ArgumentOutOfRangeException(nameof(sequence), sequence, "Sequence must be positive.");
        var site = (siteCode ?? string.Empty).Trim().ToUpperInvariant();
        if (site.Length != 3) throw new ArgumentException("Site code must be three letters.", nameof(siteCode));
        return $"{Prefix(type)}-{site}-{sequence.ToString("000000", CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Applies the scanner rule <c>^(PKG|LOT|LAM|AUTO|TRF|REST)-(?:(ANT|BUC)-)?\d{6,}$</c>.
    /// Input is trimmed and upper-cased first (scanners may send lower case); nothing else is tolerated.
    /// </summary>
    public static bool TryParse(string? raw, out IdentityCode code)
    {
        code = null!;
        var text = (raw ?? string.Empty).Trim().ToUpperInvariant();
        var m = ScannerRule().Match(text);
        if (!m.Success) return false;
        if (!long.TryParse(m.Groups[3].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var seq) || seq <= 0)
            return false;
        var type = m.Groups[1].Value switch
        {
            "PKG" => IdentityType.Pkg,
            "LOT" => IdentityType.Lot,
            "LAM" => IdentityType.Lam,
            "AUTO" => IdentityType.Auto,
            "TRF" => IdentityType.Trf,
            _ => IdentityType.Rest
        };
        var hasSite = m.Groups[2].Success;
        code = new IdentityCode(type, hasSite ? m.Groups[2].Value : AntalyaSiteCode, seq, hasSite);
        return true;
    }

    /// <summary>True when <paramref name="raw"/> is a package code of this site (PKG-BUC-…).</summary>
    public static bool IsLocalPackageCode(string? raw)
        => TryParse(raw, out var code) && code.Type == IdentityType.Pkg && code.IsLocal;
}

/// <summary>
/// Mints identity codes from one PostgreSQL sequence per (site code, type).
/// Plant records sharing a site code (PLANT-001, BUCAK, F01 → BUC) share the sequence.
/// </summary>
public interface IIdentityCodeAllocator
{
    /// <summary>Site code of the plant record, or null when the plant cannot mint identity codes.</summary>
    Task<string?> ResolveSiteCodeAsync(string? plantId, CancellationToken cancellationToken = default);

    Task<string> NextAsync(string siteCode, IdentityType type, CancellationToken cancellationToken = default);
}

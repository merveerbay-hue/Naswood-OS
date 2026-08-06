namespace Naswood.Modules.Business.Application.Common;

/// <summary>
/// Temporary mint helper until Platform Numbering Service is wired.
/// Empty/blank codes from clients are replaced — manual Code fields are not required.
/// </summary>
public static class SystemIdentifier
{
    public static string Mint(string prefix)
    {
        var p = string.IsNullOrWhiteSpace(prefix) ? "ID" : prefix.Trim().ToUpperInvariant();
        var seq = DateTime.UtcNow.ToString("yyMMdd") + "-" + Random.Shared.Next(100000, 999999);
        return $"{p}-{seq}";
    }

    public static string Ensure(string? existing, string prefix)
        => string.IsNullOrWhiteSpace(existing) ? Mint(prefix) : existing.Trim();
}

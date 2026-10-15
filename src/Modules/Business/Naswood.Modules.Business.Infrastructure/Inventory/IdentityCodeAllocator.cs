using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;
using Naswood.Modules.Platform.Contracts.Organization;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

/// <summary>
/// One PostgreSQL sequence per (site code, identity type). The sequences are part of the EF model
/// (<see cref="BusinessDbContext"/>) and are created by migrations; a new site needs a migration.
/// </summary>
public static class IdentitySequences
{
    /// <summary>Sites that mint identity codes in NOS. Antalya (ANT) is minted only by Operations.</summary>
    public static readonly IReadOnlyList<string> Sites = [IdentityCodes.LocalSiteCode];

    public static IEnumerable<string> All() =>
        Sites.SelectMany(site => Enum.GetValues<IdentityType>().Select(type => Name(site, type)));

    public static string Name(string siteCode, IdentityType type) =>
        $"identity_{siteCode.Trim().ToLowerInvariant()}_{IdentityCodes.Prefix(type).ToLowerInvariant()}";

    public static bool IsKnownSite(string siteCode) =>
        Sites.Contains((siteCode ?? string.Empty).Trim().ToUpperInvariant(), StringComparer.Ordinal);
}

public sealed class IdentityCodeAllocator : IIdentityCodeAllocator
{
    private readonly BusinessDbContext _db;
    private readonly IPlantSiteCodeReader _sites;

    public IdentityCodeAllocator(BusinessDbContext db, IPlantSiteCodeReader sites)
    {
        _db = db;
        _sites = sites;
    }

    public async Task<string?> ResolveSiteCodeAsync(string? plantId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(plantId)) return null;
        var site = await _sites.GetSiteCodeAsync(plantId, cancellationToken).ConfigureAwait(false);
        return site is not null && IdentitySequences.IsKnownSite(site) ? site : null;
    }

    public async Task<string> NextAsync(string siteCode, IdentityType type, CancellationToken cancellationToken = default)
    {
        if (!IdentitySequences.IsKnownSite(siteCode))
            throw new InvalidOperationException($"Site '{siteCode}' has no identity sequences.");
        var sequence = $"{BusinessDbContext.Schema}.{IdentitySequences.Name(siteCode, type)}";
        var next = await _db.Database
            .SqlQuery<long>($"SELECT nextval({sequence}::regclass) AS \"Value\"")
            .SingleAsync(cancellationToken)
            .ConfigureAwait(false);
        return IdentityCodes.Format(type, siteCode, next);
    }
}

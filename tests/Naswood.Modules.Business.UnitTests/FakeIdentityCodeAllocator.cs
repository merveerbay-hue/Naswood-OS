using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Modules.Business.UnitTests;

/// <summary>In-memory stand-in for the PostgreSQL sequences: one counter per (site, type).</summary>
internal sealed class FakeIdentityCodeAllocator : IIdentityCodeAllocator
{
    private readonly Dictionary<string, string?> _sites;
    private readonly Dictionary<(string, IdentityType), long> _next = new();

    public FakeIdentityCodeAllocator(Dictionary<string, string?>? sites = null)
        => _sites = sites ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["PLANT-001"] = "BUC",
            ["BUCAK"] = "BUC",
            ["F01"] = "BUC"
        };

    public Task<string?> ResolveSiteCodeAsync(string? plantId, CancellationToken cancellationToken = default)
        => Task.FromResult(plantId is not null && _sites.TryGetValue(plantId, out var site) ? site : null);

    public Task<string> NextAsync(string siteCode, IdentityType type, CancellationToken cancellationToken = default)
    {
        var key = (siteCode, type);
        var next = _next.GetValueOrDefault(key) + 1;
        _next[key] = next;
        return Task.FromResult(IdentityCodes.Format(type, siteCode, next));
    }
}

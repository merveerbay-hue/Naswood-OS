using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Contracts.Organization;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Modules.Platform.Infrastructure.Users;

public sealed class PlantSiteCodeReader : IPlantSiteCodeReader
{
    private readonly PlatformDbContext _db;

    public PlantSiteCodeReader(PlatformDbContext db) => _db = db;

    public async Task<string?> GetSiteCodeAsync(string plantId, CancellationToken cancellationToken = default)
    {
        var code = (plantId ?? string.Empty).Trim().ToUpperInvariant();
        if (code.Length == 0) return null;
        return await _db.Plants
            .AsNoTracking()
            .Where(x => x.Code == code && x.IsActive)
            .Select(x => x.SiteCode)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

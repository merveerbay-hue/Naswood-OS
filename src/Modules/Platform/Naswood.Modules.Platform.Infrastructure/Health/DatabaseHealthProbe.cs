using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Application.Health;
using Naswood.Modules.Platform.Domain.Health;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Modules.Platform.Infrastructure.Health;

public sealed class DatabaseHealthProbe : IHealthComponentProbe
{
    private readonly PlatformDbContext _db;

    public DatabaseHealthProbe(PlatformDbContext db) => _db = db;

    public string Name => "Database";

    public async Task<HealthComponent> CheckAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();

            return canConnect
                ? new HealthComponent(Name, HealthStatus.Healthy, stopwatch.Elapsed)
                : new HealthComponent(Name, HealthStatus.Unhealthy, stopwatch.Elapsed, "Cannot connect");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new HealthComponent(Name, HealthStatus.Unhealthy, stopwatch.Elapsed, ex.GetType().Name);
        }
    }
}

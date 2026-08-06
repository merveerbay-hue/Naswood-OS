using System.Diagnostics;
using Naswood.Modules.Platform.Application.Health;
using Naswood.Modules.Platform.Domain.Health;

namespace Naswood.Modules.Platform.Infrastructure.Health;

/// <summary>
/// Application self-check. Database/cache/queue probes are added when those
/// adapters exist; inventing unavailable dependency health is forbidden.
/// </summary>
public sealed class ApplicationHealthProbe : IHealthComponentProbe
{
    public string Name => "Application";

    public Task<HealthComponent> CheckAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var process = Process.GetCurrentProcess();
        stopwatch.Stop();

        var component = new HealthComponent(
            Name,
            HealthStatus.Healthy,
            stopwatch.Elapsed,
            $"PID {process.Id}");

        return Task.FromResult(component);
    }
}

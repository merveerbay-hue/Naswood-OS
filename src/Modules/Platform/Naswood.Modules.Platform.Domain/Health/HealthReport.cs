namespace Naswood.Modules.Platform.Domain.Health;

/// <summary>
/// Aggregates component checks into an overall platform health status.
/// </summary>
public sealed class HealthReport
{
    public HealthStatus Status { get; }

    public string Version { get; }

    public TimeSpan Uptime { get; }

    public IReadOnlyList<HealthComponent> Components { get; }

    public HealthReport(
        string version,
        TimeSpan uptime,
        IReadOnlyList<HealthComponent> components)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentNullException.ThrowIfNull(components);

        Version = version;
        Uptime = uptime;
        Components = components;
        Status = AggregateStatus(components);
    }

    private static HealthStatus AggregateStatus(IReadOnlyList<HealthComponent> components)
    {
        if (components.Count == 0)
        {
            return HealthStatus.Unknown;
        }

        if (components.Any(c => c.Status == HealthStatus.Unhealthy))
        {
            return HealthStatus.Unhealthy;
        }

        if (components.Any(c => c.Status is HealthStatus.Degraded or HealthStatus.Unknown))
        {
            return HealthStatus.Degraded;
        }

        return HealthStatus.Healthy;
    }
}

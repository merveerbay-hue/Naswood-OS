namespace Naswood.Modules.Platform.Domain.Health;

/// <summary>
/// Platform health status values from Health_Check design.
/// </summary>
public enum HealthStatus
{
    Healthy = 0,
    Degraded = 1,
    Unhealthy = 2,
    Unknown = 3
}

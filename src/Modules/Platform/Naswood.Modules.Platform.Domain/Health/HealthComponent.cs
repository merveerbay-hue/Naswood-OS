using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Health;

public sealed class HealthComponent : ValueObject
{
    public string Name { get; }

    public HealthStatus Status { get; }

    public TimeSpan? ResponseTime { get; }

    public string? Detail { get; }

    public HealthComponent(
        string name,
        HealthStatus status,
        TimeSpan? responseTime = null,
        string? detail = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Component name is required.", nameof(name));
        }

        Name = name.Trim();
        Status = status;
        ResponseTime = responseTime;
        Detail = detail;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Status;
        yield return ResponseTime;
        yield return Detail;
    }
}

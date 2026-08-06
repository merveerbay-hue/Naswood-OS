namespace Naswood.Modules.Platform.Contracts.Health;

/// <summary>
/// Public health contract consumed by monitoring and orchestration.
/// Matches Sprint 00 TASK-015 endpoint semantics.
/// </summary>
public sealed class HealthReportDto
{
    public required string Status { get; init; }

    public required string Version { get; init; }

    public required string Uptime { get; init; }

    public required IReadOnlyList<HealthComponentDto> Components { get; init; }
}

public sealed class HealthComponentDto
{
    public required string Name { get; init; }

    public required string Status { get; init; }

    public string? ResponseTime { get; init; }

    public string? Detail { get; init; }
}

public sealed class LivenessDto
{
    public required string Status { get; init; }

    public required DateTimeOffset Timestamp { get; init; }
}

public sealed class ReadinessDto
{
    public required string Status { get; init; }

    public required IReadOnlyList<HealthComponentDto> Components { get; init; }
}

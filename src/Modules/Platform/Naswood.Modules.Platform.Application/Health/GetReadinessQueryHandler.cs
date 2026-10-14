using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Health;
using Naswood.Modules.Platform.Domain.Health;

namespace Naswood.Modules.Platform.Application.Health;

public sealed class GetReadinessQueryHandler
    : IQueryHandler<GetReadinessQuery, Result<ReadinessDto>>
{
    private readonly IEnumerable<IHealthComponentProbe> _probes;

    public GetReadinessQueryHandler(IEnumerable<IHealthComponentProbe> probes)
    {
        _probes = probes;
    }

    public async Task<Result<ReadinessDto>> HandleAsync(
        GetReadinessQuery query,
        CancellationToken cancellationToken = default)
    {
        var components = new List<HealthComponent>();

        foreach (var probe in _probes)
        {
            components.Add(await probe.CheckAsync(cancellationToken).ConfigureAwait(false));
        }

        var overall = components.Count == 0
            ? HealthStatus.Unknown
            : components.Any(c => c.Status == HealthStatus.Unhealthy)
                ? HealthStatus.Unhealthy
                : components.Any(c => c.Status is HealthStatus.Degraded or HealthStatus.Unknown)
                    ? HealthStatus.Degraded
                    : HealthStatus.Healthy;

        var dto = new ReadinessDto
        {
            Status = overall.ToString(),
            Components = components.Select(GetHealthReportQueryHandler.MapComponent).ToArray()
        };

        return Result.Success(dto);
    }
}

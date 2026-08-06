using System.Globalization;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Health;
using Naswood.Modules.Platform.Domain.Health;

namespace Naswood.Modules.Platform.Application.Health;

public sealed class GetHealthReportQueryHandler
    : IQueryHandler<GetHealthReportQuery, Result<HealthReportDto>>
{
    private readonly IEnumerable<IHealthComponentProbe> _probes;
    private readonly IPlatformRuntimeInfo _runtimeInfo;

    public GetHealthReportQueryHandler(
        IEnumerable<IHealthComponentProbe> probes,
        IPlatformRuntimeInfo runtimeInfo)
    {
        _probes = probes;
        _runtimeInfo = runtimeInfo;
    }

    public async Task<Result<HealthReportDto>> HandleAsync(
        GetHealthReportQuery query,
        CancellationToken cancellationToken = default)
    {
        var components = new List<HealthComponent>();

        foreach (var probe in _probes)
        {
            components.Add(await probe.CheckAsync(cancellationToken).ConfigureAwait(false));
        }

        var report = new HealthReport(
            _runtimeInfo.Version,
            _runtimeInfo.Uptime,
            components);

        return Result.Success(Map(report));
    }

    private static HealthReportDto Map(HealthReport report) =>
        new()
        {
            Status = report.Status.ToString(),
            Version = report.Version,
            Uptime = FormatUptime(report.Uptime),
            Components = report.Components.Select(MapComponent).ToArray()
        };

    internal static HealthComponentDto MapComponent(HealthComponent component) =>
        new()
        {
            Name = component.Name,
            Status = component.Status.ToString(),
            ResponseTime = component.ResponseTime?.TotalMilliseconds is { } ms
                ? string.Create(CultureInfo.InvariantCulture, $"{ms:0}ms")
                : null,
            Detail = component.Detail
        };

    private static string FormatUptime(TimeSpan uptime)
    {
        if (uptime.TotalDays >= 1)
        {
            return string.Create(
                CultureInfo.InvariantCulture,
                $"{(int)uptime.TotalDays}d {uptime.Hours}h");
        }

        if (uptime.TotalHours >= 1)
        {
            return string.Create(
                CultureInfo.InvariantCulture,
                $"{(int)uptime.TotalHours}h {uptime.Minutes}m");
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{(int)uptime.TotalMinutes}m {uptime.Seconds}s");
    }
}

using Naswood.Modules.Platform.Application.Health;
using Naswood.Modules.Platform.Domain.Health;

namespace Naswood.Modules.Platform.UnitTests;

public class HealthReportTests
{
    [Fact]
    public void Aggregate_is_unhealthy_when_any_component_is_unhealthy()
    {
        var report = new HealthReport(
            "1.0.0",
            TimeSpan.FromMinutes(5),
            [
                new HealthComponent("Application", HealthStatus.Healthy),
                new HealthComponent("Database", HealthStatus.Unhealthy, detail: "unreachable")
            ]);

        Assert.Equal(HealthStatus.Unhealthy, report.Status);
    }

    [Fact]
    public void Aggregate_is_degraded_when_component_is_degraded()
    {
        var report = new HealthReport(
            "1.0.0",
            TimeSpan.FromMinutes(5),
            [
                new HealthComponent("Application", HealthStatus.Healthy),
                new HealthComponent("Cache", HealthStatus.Degraded)
            ]);

        Assert.Equal(HealthStatus.Degraded, report.Status);
    }

    [Fact]
    public async Task GetHealthReportQueryHandler_returns_application_component()
    {
        var handler = new GetHealthReportQueryHandler(
            [new StubProbe()],
            new StubRuntimeInfo());

        var result = await handler.HandleAsync(new GetHealthReportQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal("Healthy", result.Value.Status);
        Assert.Contains(result.Value.Components, c => c.Name == "Application");
        Assert.Equal("1.2.3", result.Value.Version);
    }

    private sealed class StubProbe : IHealthComponentProbe
    {
        public string Name => "Application";

        public Task<HealthComponent> CheckAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new HealthComponent(Name, HealthStatus.Healthy, TimeSpan.FromMilliseconds(1)));
    }

    private sealed class StubRuntimeInfo : IPlatformRuntimeInfo
    {
        public string Version => "1.2.3";

        public TimeSpan Uptime => TimeSpan.FromMinutes(12);
    }
}

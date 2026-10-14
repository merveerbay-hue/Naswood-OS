using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Health;

namespace Naswood.Modules.Platform.Presentation.Health;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public HealthController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Detailed health report for administrators and monitoring platforms.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var result = await _dispatcher
            .QueryAsync(new GetHealthReportQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Minimal liveness probe for process aliveness.
    /// </summary>
    [HttpGet("live")]
    public async Task<IActionResult> GetLiveness(CancellationToken cancellationToken)
    {
        var result = await _dispatcher
            .QueryAsync(new GetLivenessQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Readiness probe for traffic admission.
    /// </summary>
    [HttpGet("ready")]
    public async Task<IActionResult> GetReadiness(CancellationToken cancellationToken)
    {
        var result = await _dispatcher
            .QueryAsync(new GetReadinessQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }
}

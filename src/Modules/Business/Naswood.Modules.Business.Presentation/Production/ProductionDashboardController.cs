using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Production;

[ApiController]
[Authorize]
public sealed class ProductionDashboardController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ProductionDashboardController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/production/dashboard")]
    [RequirePermission("Production.View")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetProductionDashboardQuery(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

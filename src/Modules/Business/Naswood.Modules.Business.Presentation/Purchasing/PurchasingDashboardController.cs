using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Purchasing;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Purchasing;

[ApiController]
[Authorize]
public sealed class PurchasingDashboardController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public PurchasingDashboardController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/purchasing/dashboard")]
    [RequirePermission("Purchasing.View")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPurchasingDashboardQuery(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

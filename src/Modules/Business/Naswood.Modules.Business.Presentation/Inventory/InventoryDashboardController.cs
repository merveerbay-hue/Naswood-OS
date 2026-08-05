using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Inventory;

[ApiController]
[Authorize]
public sealed class InventoryDashboardController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public InventoryDashboardController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/inventory/dashboard")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetInventoryDashboardQuery(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

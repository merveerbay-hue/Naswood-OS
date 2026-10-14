using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Sales;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Sales;

[ApiController]
[Authorize]
public sealed class SalesDashboardController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public SalesDashboardController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/sales/dashboard")]
    [RequirePermission("Sales.View")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSalesDashboardQuery(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

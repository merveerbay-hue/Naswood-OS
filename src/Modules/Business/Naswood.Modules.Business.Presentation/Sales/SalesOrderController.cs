using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Sales;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Sales;

[ApiController]
[Authorize]
public sealed class SalesOrderController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public SalesOrderController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/sales-orders")]
    [RequirePermission("SalesOrder.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchSalesOrderQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/sales-orders/{id:guid}")]
    [RequirePermission("SalesOrder.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSalesOrderByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/sales-orders")]
    [RequirePermission("SalesOrder.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertSalesOrderRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateSalesOrderCommand(request.Number, request.CustomerCode, request.OrderDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesOrder created.");
    }

    [HttpPut("api/v1/sales-orders/{id:guid}")]
    [RequirePermission("SalesOrder.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertSalesOrderRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateSalesOrderCommand(id, request.Number, request.CustomerCode, request.OrderDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesOrder updated.");
    }

    [HttpDelete("api/v1/sales-orders/{id:guid}")]
    [RequirePermission("SalesOrder.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteSalesOrderCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesOrder deleted.");
    }
}

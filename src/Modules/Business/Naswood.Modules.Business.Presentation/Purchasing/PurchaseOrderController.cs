using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Purchasing;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Purchasing;

[ApiController]
[Authorize]
public sealed class PurchaseOrderController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public PurchaseOrderController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/purchase-orders")]
    [RequirePermission("PurchaseOrder.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchPurchaseOrderQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/purchase-orders/{id:guid}")]
    [RequirePermission("PurchaseOrder.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPurchaseOrderByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/purchase-orders")]
    [RequirePermission("PurchaseOrder.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertPurchaseOrderRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreatePurchaseOrderCommand(request.Number, request.SupplierCode, request.OrderDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseOrder created.");
    }

    [HttpPut("api/v1/purchase-orders/{id:guid}")]
    [RequirePermission("PurchaseOrder.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertPurchaseOrderRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdatePurchaseOrderCommand(id, request.Number, request.SupplierCode, request.OrderDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseOrder updated.");
    }

    [HttpDelete("api/v1/purchase-orders/{id:guid}")]
    [RequirePermission("PurchaseOrder.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeletePurchaseOrderCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseOrder deleted.");
    }
}

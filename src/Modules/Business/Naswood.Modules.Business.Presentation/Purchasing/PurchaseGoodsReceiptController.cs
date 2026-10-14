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
public sealed class PurchaseGoodsReceiptController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public PurchaseGoodsReceiptController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/purchase-goods-receipts")]
    [RequirePermission("PurchaseGoodsReceipt.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchPurchaseGoodsReceiptQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/purchase-goods-receipts/{id:guid}")]
    [RequirePermission("PurchaseGoodsReceipt.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPurchaseGoodsReceiptByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/purchase-goods-receipts")]
    [RequirePermission("PurchaseGoodsReceipt.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertPurchaseGoodsReceiptRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreatePurchaseGoodsReceiptCommand(request.Number, request.PurchaseOrderNumber, request.WarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseGoodsReceipt created.");
    }

    [HttpPut("api/v1/purchase-goods-receipts/{id:guid}")]
    [RequirePermission("PurchaseGoodsReceipt.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertPurchaseGoodsReceiptRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdatePurchaseGoodsReceiptCommand(id, request.Number, request.PurchaseOrderNumber, request.WarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseGoodsReceipt updated.");
    }

    [HttpDelete("api/v1/purchase-goods-receipts/{id:guid}")]
    [RequirePermission("PurchaseGoodsReceipt.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeletePurchaseGoodsReceiptCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseGoodsReceipt deleted.");
    }
}

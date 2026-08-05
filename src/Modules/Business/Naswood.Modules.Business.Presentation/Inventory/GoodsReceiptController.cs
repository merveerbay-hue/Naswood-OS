using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Inventory;

[ApiController]
[Authorize]
public sealed class GoodsReceiptController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public GoodsReceiptController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/goods-receipts")]
    [RequirePermission("GoodsReceipt.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchGoodsReceiptQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/goods-receipts/{id:guid}")]
    [RequirePermission("GoodsReceipt.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetGoodsReceiptByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/goods-receipts")]
    [RequirePermission("GoodsReceipt.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertGoodsReceiptRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateGoodsReceiptCommand(request.Number, request.WarehouseCode, request.Reference, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "GoodsReceipt created.");
    }

    [HttpPut("api/v1/goods-receipts/{id:guid}")]
    [RequirePermission("GoodsReceipt.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertGoodsReceiptRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateGoodsReceiptCommand(id, request.Number, request.WarehouseCode, request.Reference, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "GoodsReceipt updated.");
    }

    [HttpDelete("api/v1/goods-receipts/{id:guid}")]
    [RequirePermission("GoodsReceipt.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteGoodsReceiptCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "GoodsReceipt deleted.");
    }
}

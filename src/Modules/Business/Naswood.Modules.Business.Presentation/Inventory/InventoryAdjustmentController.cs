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
public sealed class InventoryAdjustmentController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public InventoryAdjustmentController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/inventory-adjustments")]
    [RequirePermission("InventoryAdjustment.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchInventoryAdjustmentQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-adjustments/{id:guid}")]
    [RequirePermission("InventoryAdjustment.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetInventoryAdjustmentByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/inventory-adjustments")]
    [RequirePermission("InventoryAdjustment.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertInventoryAdjustmentRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateInventoryAdjustmentCommand(request.Number, request.WarehouseCode, request.Reason, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryAdjustment created.");
    }

    [HttpPut("api/v1/inventory-adjustments/{id:guid}")]
    [RequirePermission("InventoryAdjustment.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertInventoryAdjustmentRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateInventoryAdjustmentCommand(id, request.Number, request.WarehouseCode, request.Reason, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryAdjustment updated.");
    }

    [HttpDelete("api/v1/inventory-adjustments/{id:guid}")]
    [RequirePermission("InventoryAdjustment.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteInventoryAdjustmentCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryAdjustment deleted.");
    }
}

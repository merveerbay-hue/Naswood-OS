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
public sealed class InventoryCountController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public InventoryCountController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/inventory-counts")]
    [RequirePermission("InventoryCount.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchInventoryCountQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-counts/{id:guid}")]
    [RequirePermission("InventoryCount.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetInventoryCountByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/inventory-counts")]
    [RequirePermission("InventoryCount.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertInventoryCountRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateInventoryCountCommand(request.Number, request.WarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryCount created.");
    }

    [HttpPut("api/v1/inventory-counts/{id:guid}")]
    [RequirePermission("InventoryCount.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertInventoryCountRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateInventoryCountCommand(id, request.Number, request.WarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryCount updated.");
    }

    [HttpDelete("api/v1/inventory-counts/{id:guid}")]
    [RequirePermission("InventoryCount.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteInventoryCountCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryCount deleted.");
    }
}

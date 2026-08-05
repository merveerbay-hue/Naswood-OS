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
public sealed class InventoryBalanceController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public InventoryBalanceController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/inventory")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchInventoryBalanceQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory/{id:guid}")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetInventoryBalanceByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/inventory")]
    [RequirePermission("Inventory.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertInventoryBalanceRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateInventoryBalanceCommand(request.MaterialCode, request.WarehouseCode, request.LocationCode, request.BatchNumber, request.QuantityOnHand, request.QuantityReserved, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryBalance created.");
    }

    [HttpPut("api/v1/inventory/{id:guid}")]
    [RequirePermission("Inventory.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertInventoryBalanceRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateInventoryBalanceCommand(id, request.MaterialCode, request.WarehouseCode, request.LocationCode, request.BatchNumber, request.QuantityOnHand, request.QuantityReserved, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryBalance updated.");
    }

    [HttpDelete("api/v1/inventory/{id:guid}")]
    [RequirePermission("Inventory.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteInventoryBalanceCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryBalance deleted.");
    }
}

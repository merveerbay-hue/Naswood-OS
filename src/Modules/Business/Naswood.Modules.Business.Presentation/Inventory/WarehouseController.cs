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
public sealed class WarehouseController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public WarehouseController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/warehouses")]
    [RequirePermission("Warehouse.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchWarehouseQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/warehouses/{id:guid}")]
    [RequirePermission("Warehouse.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetWarehouseByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/warehouses")]
    [RequirePermission("Warehouse.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertWarehouseRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateWarehouseCommand(request.Code, request.Name, request.WarehouseType, request.Status, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Warehouse created.");

    [HttpPut("api/v1/warehouses/{id:guid}")]
    [RequirePermission("Warehouse.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertWarehouseRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateWarehouseCommand(id, request.Code, request.Name, request.WarehouseType, request.Status, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Warehouse updated.");

    [HttpDelete("api/v1/warehouses/{id:guid}")]
    [RequirePermission("Warehouse.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteWarehouseCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Warehouse deleted.");
}

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
public sealed class ShipmentController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ShipmentController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/shipments")]
    [RequirePermission("Shipment.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchShipmentQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/shipments/{id:guid}")]
    [RequirePermission("Shipment.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetShipmentByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/shipments")]
    [RequirePermission("Shipment.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertShipmentRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateShipmentCommand(request.Number, request.SalesOrderNumber, request.WarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Shipment created.");
    }

    [HttpPut("api/v1/shipments/{id:guid}")]
    [RequirePermission("Shipment.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertShipmentRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateShipmentCommand(id, request.Number, request.SalesOrderNumber, request.WarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Shipment updated.");
    }

    [HttpDelete("api/v1/shipments/{id:guid}")]
    [RequirePermission("Shipment.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteShipmentCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Shipment deleted.");
    }
}

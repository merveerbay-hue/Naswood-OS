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
public sealed class DeliveryController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public DeliveryController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/deliveries")]
    [RequirePermission("Delivery.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchDeliveryQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/deliveries/{id:guid}")]
    [RequirePermission("Delivery.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetDeliveryByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/deliveries")]
    [RequirePermission("Delivery.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertDeliveryRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateDeliveryCommand(request.Number, request.ShipmentNumber, request.CustomerCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Delivery created.");
    }

    [HttpPut("api/v1/deliveries/{id:guid}")]
    [RequirePermission("Delivery.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertDeliveryRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateDeliveryCommand(id, request.Number, request.ShipmentNumber, request.CustomerCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Delivery updated.");
    }

    [HttpDelete("api/v1/deliveries/{id:guid}")]
    [RequirePermission("Delivery.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteDeliveryCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Delivery deleted.");
    }
}

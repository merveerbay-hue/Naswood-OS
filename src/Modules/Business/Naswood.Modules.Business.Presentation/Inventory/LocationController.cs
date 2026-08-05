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
public sealed class LocationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public LocationController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/locations")]
    [RequirePermission("Location.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchLocationQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/locations/{id:guid}")]
    [RequirePermission("Location.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetLocationByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/locations")]
    [RequirePermission("Location.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertLocationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateLocationCommand(request.Code, request.Name, request.WarehouseCode, request.LocationType, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Location created.");
    }

    [HttpPut("api/v1/locations/{id:guid}")]
    [RequirePermission("Location.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertLocationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateLocationCommand(id, request.Code, request.Name, request.WarehouseCode, request.LocationType, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Location updated.");
    }

    [HttpDelete("api/v1/locations/{id:guid}")]
    [RequirePermission("Location.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteLocationCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Location deleted.");
    }
}

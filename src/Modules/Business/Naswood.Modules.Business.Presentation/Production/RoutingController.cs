using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Production;

[ApiController]
[Authorize]
public sealed class RoutingController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public RoutingController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/routings")]
    [RequirePermission("Routing.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchRoutingQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/routings/{id:guid}")]
    [RequirePermission("Routing.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetRoutingByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/routings")]
    [RequirePermission("Routing.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertRoutingRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateRoutingCommand(request.Number, request.MaterialCode, request.Version, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Routing created.");
    }

    [HttpPut("api/v1/routings/{id:guid}")]
    [RequirePermission("Routing.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertRoutingRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateRoutingCommand(id, request.Number, request.MaterialCode, request.Version, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Routing updated.");
    }

    [HttpDelete("api/v1/routings/{id:guid}")]
    [RequirePermission("Routing.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteRoutingCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Routing deleted.");
    }
}

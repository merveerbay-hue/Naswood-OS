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
public sealed class ReworkController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ReworkController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/reworks")]
    [RequirePermission("Rework.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchReworkQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/reworks/{id:guid}")]
    [RequirePermission("Rework.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetReworkByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/reworks")]
    [RequirePermission("Rework.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertReworkRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateReworkCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Rework created.");

    [HttpPut("api/v1/reworks/{id:guid}")]
    [RequirePermission("Rework.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertReworkRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateReworkCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Rework updated.");

    [HttpDelete("api/v1/reworks/{id:guid}")]
    [RequirePermission("Rework.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteReworkCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Rework deleted.");
}

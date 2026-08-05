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
public sealed class WipController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public WipController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/wips")]
    [RequirePermission("Wip.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchWipQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/wips/{id:guid}")]
    [RequirePermission("Wip.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetWipByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/wips")]
    [RequirePermission("Wip.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertWipRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateWipCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Wip created.");

    [HttpPut("api/v1/wips/{id:guid}")]
    [RequirePermission("Wip.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertWipRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateWipCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Wip updated.");

    [HttpDelete("api/v1/wips/{id:guid}")]
    [RequirePermission("Wip.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteWipCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Wip deleted.");
}

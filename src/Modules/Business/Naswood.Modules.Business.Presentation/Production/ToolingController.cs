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
public sealed class ToolingController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ToolingController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/toolings")]
    [RequirePermission("Tooling.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchToolingQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/toolings/{id:guid}")]
    [RequirePermission("Tooling.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetToolingByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/toolings")]
    [RequirePermission("Tooling.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertToolingRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateToolingCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Tooling created.");

    [HttpPut("api/v1/toolings/{id:guid}")]
    [RequirePermission("Tooling.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertToolingRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateToolingCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Tooling updated.");

    [HttpDelete("api/v1/toolings/{id:guid}")]
    [RequirePermission("Tooling.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteToolingCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Tooling deleted.");
}

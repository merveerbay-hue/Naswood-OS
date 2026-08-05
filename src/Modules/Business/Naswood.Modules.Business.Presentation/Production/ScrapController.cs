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
public sealed class ScrapController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ScrapController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/scraps")]
    [RequirePermission("Scrap.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchScrapQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/scraps/{id:guid}")]
    [RequirePermission("Scrap.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetScrapByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/scraps")]
    [RequirePermission("Scrap.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertScrapRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateScrapCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Scrap created.");

    [HttpPut("api/v1/scraps/{id:guid}")]
    [RequirePermission("Scrap.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertScrapRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateScrapCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Scrap updated.");

    [HttpDelete("api/v1/scraps/{id:guid}")]
    [RequirePermission("Scrap.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteScrapCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Scrap deleted.");
}

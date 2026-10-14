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
public sealed class BomController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public BomController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/boms")]
    [RequirePermission("Bom.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchBomQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/boms/{id:guid}")]
    [RequirePermission("Bom.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetBomByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/boms")]
    [RequirePermission("Bom.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertBomRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateBomCommand(request.Number, request.MaterialCode, request.Version, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Bom created.");
    }

    [HttpPut("api/v1/boms/{id:guid}")]
    [RequirePermission("Bom.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertBomRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateBomCommand(id, request.Number, request.MaterialCode, request.Version, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Bom updated.");
    }

    [HttpDelete("api/v1/boms/{id:guid}")]
    [RequirePermission("Bom.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteBomCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Bom deleted.");
    }
}

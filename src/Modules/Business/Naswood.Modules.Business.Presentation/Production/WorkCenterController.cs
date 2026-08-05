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
public sealed class WorkCenterController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public WorkCenterController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/work-centers")]
    [RequirePermission("WorkCenter.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchWorkCenterQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/work-centers/{id:guid}")]
    [RequirePermission("WorkCenter.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetWorkCenterByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/work-centers")]
    [RequirePermission("WorkCenter.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertWorkCenterRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateWorkCenterCommand(request.Code, request.Name, request.CapacityPerHour, request.Status, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "WorkCenter created.");

    [HttpPut("api/v1/work-centers/{id:guid}")]
    [RequirePermission("WorkCenter.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertWorkCenterRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateWorkCenterCommand(id, request.Code, request.Name, request.CapacityPerHour, request.Status, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "WorkCenter updated.");

    [HttpDelete("api/v1/work-centers/{id:guid}")]
    [RequirePermission("WorkCenter.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteWorkCenterCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "WorkCenter deleted.");
}

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
public sealed class WorkOrderController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public WorkOrderController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/work-orders")]
    [RequirePermission("WorkOrder.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchWorkOrderQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/work-orders/{id:guid}")]
    [RequirePermission("WorkOrder.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetWorkOrderByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/work-orders")]
    [RequirePermission("WorkOrder.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertWorkOrderRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateWorkOrderCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "WorkOrder created.");

    [HttpPut("api/v1/work-orders/{id:guid}")]
    [RequirePermission("WorkOrder.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertWorkOrderRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateWorkOrderCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "WorkOrder updated.");

    [HttpDelete("api/v1/work-orders/{id:guid}")]
    [RequirePermission("WorkOrder.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteWorkOrderCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "WorkOrder deleted.");
}

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
public sealed class OperationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public OperationController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/operations")]
    [RequirePermission("Operation.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchOperationQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/operations/{id:guid}")]
    [RequirePermission("Operation.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetOperationByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/operations")]
    [RequirePermission("Operation.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertOperationRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateOperationCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Operation created.");

    [HttpPut("api/v1/operations/{id:guid}")]
    [RequirePermission("Operation.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertOperationRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateOperationCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Operation updated.");

    [HttpDelete("api/v1/operations/{id:guid}")]
    [RequirePermission("Operation.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteOperationCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Operation deleted.");
}

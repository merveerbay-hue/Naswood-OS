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
public sealed class MachineController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public MachineController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/machines")]
    [RequirePermission("Machine.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchMachineQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/machines/{id:guid}")]
    [RequirePermission("Machine.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetMachineByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/machines")]
    [RequirePermission("Machine.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertMachineRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateMachineCommand(request.Code, request.Name, request.WorkCenterCode, request.Status, request.OeeTarget), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Machine created.");
    }

    [HttpPut("api/v1/machines/{id:guid}")]
    [RequirePermission("Machine.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertMachineRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateMachineCommand(id, request.Code, request.Name, request.WorkCenterCode, request.Status, request.OeeTarget), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Machine updated.");
    }

    [HttpDelete("api/v1/machines/{id:guid}")]
    [RequirePermission("Machine.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteMachineCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Machine deleted.");
    }
}

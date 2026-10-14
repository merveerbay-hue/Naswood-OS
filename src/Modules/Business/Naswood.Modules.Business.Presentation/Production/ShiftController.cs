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
public sealed class ShiftController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ShiftController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/shifts")]
    [RequirePermission("Shift.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchShiftQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/shifts/{id:guid}")]
    [RequirePermission("Shift.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetShiftByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/shifts")]
    [RequirePermission("Shift.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertShiftRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateShiftCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Shift created.");

    [HttpPut("api/v1/shifts/{id:guid}")]
    [RequirePermission("Shift.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertShiftRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateShiftCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Shift updated.");

    [HttpDelete("api/v1/shifts/{id:guid}")]
    [RequirePermission("Shift.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteShiftCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Shift deleted.");
}

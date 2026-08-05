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
public sealed class CalendarController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public CalendarController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/calendars")]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchCalendarQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/calendars/{id:guid}")]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetCalendarByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/calendars")]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertCalendarRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateCalendarCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Calendar created.");

    [HttpPut("api/v1/calendars/{id:guid}")]
    [RequirePermission("Calendar.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertCalendarRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateCalendarCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Calendar updated.");

    [HttpDelete("api/v1/calendars/{id:guid}")]
    [RequirePermission("Calendar.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteCalendarCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Calendar deleted.");
}

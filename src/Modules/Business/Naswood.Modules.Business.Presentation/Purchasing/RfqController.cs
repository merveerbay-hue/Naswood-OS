using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Purchasing;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Purchasing;

[ApiController]
[Authorize]
public sealed class RfqController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public RfqController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/rfqs")]
    [RequirePermission("Rfq.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchRfqQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/rfqs/{id:guid}")]
    [RequirePermission("Rfq.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetRfqByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/rfqs")]
    [RequirePermission("Rfq.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertRfqRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateRfqCommand(request.Number, request.Title, request.DueDate, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Rfq created.");
    }

    [HttpPut("api/v1/rfqs/{id:guid}")]
    [RequirePermission("Rfq.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertRfqRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateRfqCommand(id, request.Number, request.Title, request.DueDate, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Rfq updated.");
    }

    [HttpDelete("api/v1/rfqs/{id:guid}")]
    [RequirePermission("Rfq.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteRfqCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Rfq deleted.");
    }
}

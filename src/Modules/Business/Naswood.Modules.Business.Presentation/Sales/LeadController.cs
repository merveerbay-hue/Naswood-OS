using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Sales;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Sales;

[ApiController]
[Authorize]
public sealed class LeadController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public LeadController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/leads")]
    [RequirePermission("Lead.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchLeadQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/leads/{id:guid}")]
    [RequirePermission("Lead.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetLeadByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/leads")]
    [RequirePermission("Lead.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertLeadRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateLeadCommand(request.Code, request.Name, request.CompanyName, request.Email, request.Source, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Lead created.");
    }

    [HttpPut("api/v1/leads/{id:guid}")]
    [RequirePermission("Lead.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertLeadRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateLeadCommand(id, request.Code, request.Name, request.CompanyName, request.Email, request.Source, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Lead updated.");
    }

    [HttpDelete("api/v1/leads/{id:guid}")]
    [RequirePermission("Lead.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteLeadCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Lead deleted.");
    }
}

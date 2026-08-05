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
public sealed class OpportunityController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public OpportunityController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/opportunities")]
    [RequirePermission("Opportunity.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchOpportunityQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/opportunities/{id:guid}")]
    [RequirePermission("Opportunity.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetOpportunityByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/opportunities")]
    [RequirePermission("Opportunity.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertOpportunityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateOpportunityCommand(request.Number, request.CustomerCode, request.Title, request.Amount, request.Stage, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Opportunity created.");
    }

    [HttpPut("api/v1/opportunities/{id:guid}")]
    [RequirePermission("Opportunity.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertOpportunityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateOpportunityCommand(id, request.Number, request.CustomerCode, request.Title, request.Amount, request.Stage, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Opportunity updated.");
    }

    [HttpDelete("api/v1/opportunities/{id:guid}")]
    [RequirePermission("Opportunity.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteOpportunityCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Opportunity deleted.");
    }
}

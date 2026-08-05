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
public sealed class PurchaseRequestController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public PurchaseRequestController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/purchase-requests")]
    [RequirePermission("PurchaseRequest.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchPurchaseRequestQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/purchase-requests/{id:guid}")]
    [RequirePermission("PurchaseRequest.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPurchaseRequestByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/purchase-requests")]
    [RequirePermission("PurchaseRequest.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertPurchaseRequestRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreatePurchaseRequestCommand(request.Number, request.Requester, request.NeededDate, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseRequest created.");
    }

    [HttpPut("api/v1/purchase-requests/{id:guid}")]
    [RequirePermission("PurchaseRequest.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertPurchaseRequestRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdatePurchaseRequestCommand(id, request.Number, request.Requester, request.NeededDate, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseRequest updated.");
    }

    [HttpDelete("api/v1/purchase-requests/{id:guid}")]
    [RequirePermission("PurchaseRequest.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeletePurchaseRequestCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseRequest deleted.");
    }
}

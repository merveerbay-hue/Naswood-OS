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
public sealed class PurchaseReturnController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public PurchaseReturnController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/purchase-returns")]
    [RequirePermission("PurchaseReturn.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchPurchaseReturnQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/purchase-returns/{id:guid}")]
    [RequirePermission("PurchaseReturn.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPurchaseReturnByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/purchase-returns")]
    [RequirePermission("PurchaseReturn.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertPurchaseReturnRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreatePurchaseReturnCommand(request.Number, request.SupplierCode, request.PurchaseOrderNumber, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseReturn created.");
    }

    [HttpPut("api/v1/purchase-returns/{id:guid}")]
    [RequirePermission("PurchaseReturn.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertPurchaseReturnRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdatePurchaseReturnCommand(id, request.Number, request.SupplierCode, request.PurchaseOrderNumber, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseReturn updated.");
    }

    [HttpDelete("api/v1/purchase-returns/{id:guid}")]
    [RequirePermission("PurchaseReturn.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeletePurchaseReturnCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchaseReturn deleted.");
    }
}

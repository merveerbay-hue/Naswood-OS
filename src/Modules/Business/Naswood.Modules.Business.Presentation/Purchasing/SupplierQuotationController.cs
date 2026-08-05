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
public sealed class SupplierQuotationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public SupplierQuotationController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/supplier-quotations")]
    [RequirePermission("SupplierQuotation.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchSupplierQuotationQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/supplier-quotations/{id:guid}")]
    [RequirePermission("SupplierQuotation.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSupplierQuotationByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/supplier-quotations")]
    [RequirePermission("SupplierQuotation.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertSupplierQuotationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateSupplierQuotationCommand(request.Number, request.SupplierCode, request.RfqNumber, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SupplierQuotation created.");
    }

    [HttpPut("api/v1/supplier-quotations/{id:guid}")]
    [RequirePermission("SupplierQuotation.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertSupplierQuotationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateSupplierQuotationCommand(id, request.Number, request.SupplierCode, request.RfqNumber, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SupplierQuotation updated.");
    }

    [HttpDelete("api/v1/supplier-quotations/{id:guid}")]
    [RequirePermission("SupplierQuotation.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteSupplierQuotationCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SupplierQuotation deleted.");
    }
}

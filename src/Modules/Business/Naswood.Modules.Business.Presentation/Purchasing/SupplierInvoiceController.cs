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
public sealed class SupplierInvoiceController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public SupplierInvoiceController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/supplier-invoices")]
    [RequirePermission("SupplierInvoice.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchSupplierInvoiceQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/supplier-invoices/{id:guid}")]
    [RequirePermission("SupplierInvoice.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSupplierInvoiceByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/supplier-invoices")]
    [RequirePermission("SupplierInvoice.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertSupplierInvoiceRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateSupplierInvoiceCommand(request.Number, request.SupplierCode, request.InvoiceDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SupplierInvoice created.");
    }

    [HttpPut("api/v1/supplier-invoices/{id:guid}")]
    [RequirePermission("SupplierInvoice.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertSupplierInvoiceRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateSupplierInvoiceCommand(id, request.Number, request.SupplierCode, request.InvoiceDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SupplierInvoice updated.");
    }

    [HttpDelete("api/v1/supplier-invoices/{id:guid}")]
    [RequirePermission("SupplierInvoice.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteSupplierInvoiceCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SupplierInvoice deleted.");
    }
}

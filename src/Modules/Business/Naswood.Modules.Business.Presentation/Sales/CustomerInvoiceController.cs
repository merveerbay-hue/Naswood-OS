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
public sealed class CustomerInvoiceController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public CustomerInvoiceController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/customer-invoices")]
    [RequirePermission("CustomerInvoice.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchCustomerInvoiceQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/customer-invoices/{id:guid}")]
    [RequirePermission("CustomerInvoice.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetCustomerInvoiceByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/customer-invoices")]
    [RequirePermission("CustomerInvoice.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertCustomerInvoiceRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateCustomerInvoiceCommand(request.Number, request.CustomerCode, request.InvoiceDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "CustomerInvoice created.");
    }

    [HttpPut("api/v1/customer-invoices/{id:guid}")]
    [RequirePermission("CustomerInvoice.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertCustomerInvoiceRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateCustomerInvoiceCommand(id, request.Number, request.CustomerCode, request.InvoiceDate, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "CustomerInvoice updated.");
    }

    [HttpDelete("api/v1/customer-invoices/{id:guid}")]
    [RequirePermission("CustomerInvoice.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteCustomerInvoiceCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "CustomerInvoice deleted.");
    }
}

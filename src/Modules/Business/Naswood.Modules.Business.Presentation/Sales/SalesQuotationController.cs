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
public sealed class SalesQuotationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public SalesQuotationController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/quotations")]
    [RequirePermission("SalesQuotation.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchSalesQuotationQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/quotations/{id:guid}")]
    [RequirePermission("SalesQuotation.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSalesQuotationByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/quotations")]
    [RequirePermission("SalesQuotation.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertSalesQuotationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateSalesQuotationCommand(request.Number, request.CustomerCode, request.ValidUntil, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesQuotation created.");
    }

    [HttpPut("api/v1/quotations/{id:guid}")]
    [RequirePermission("SalesQuotation.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertSalesQuotationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateSalesQuotationCommand(id, request.Number, request.CustomerCode, request.ValidUntil, request.TotalAmount, request.Currency, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesQuotation updated.");
    }

    [HttpDelete("api/v1/quotations/{id:guid}")]
    [RequirePermission("SalesQuotation.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteSalesQuotationCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesQuotation deleted.");
    }
}

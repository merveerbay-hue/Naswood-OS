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
public sealed class SalesReportController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public SalesReportController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/sales/reports")]
    [RequirePermission("SalesReport.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchSalesReportQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/sales/reports/{id:guid}")]
    [RequirePermission("SalesReport.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSalesReportByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/sales/reports")]
    [RequirePermission("SalesReport.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertSalesReportRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateSalesReportCommand(request.ReportCode, request.Name, request.Category, request.Description), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesReport created.");
    }

    [HttpPut("api/v1/sales/reports/{id:guid}")]
    [RequirePermission("SalesReport.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertSalesReportRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateSalesReportCommand(id, request.ReportCode, request.Name, request.Category, request.Description), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesReport updated.");
    }

    [HttpDelete("api/v1/sales/reports/{id:guid}")]
    [RequirePermission("SalesReport.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteSalesReportCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "SalesReport deleted.");
    }
}

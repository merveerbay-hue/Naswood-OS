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
public sealed class PurchasingReportController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public PurchasingReportController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/purchasing/reports")]
    [RequirePermission("PurchasingReport.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchPurchasingReportQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/purchasing/reports/{id:guid}")]
    [RequirePermission("PurchasingReport.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPurchasingReportByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/purchasing/reports")]
    [RequirePermission("PurchasingReport.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertPurchasingReportRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreatePurchasingReportCommand(request.ReportCode, request.Name, request.Category, request.Description), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchasingReport created.");
    }

    [HttpPut("api/v1/purchasing/reports/{id:guid}")]
    [RequirePermission("PurchasingReport.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertPurchasingReportRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdatePurchasingReportCommand(id, request.ReportCode, request.Name, request.Category, request.Description), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchasingReport updated.");
    }

    [HttpDelete("api/v1/purchasing/reports/{id:guid}")]
    [RequirePermission("PurchasingReport.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeletePurchasingReportCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "PurchasingReport deleted.");
    }
}

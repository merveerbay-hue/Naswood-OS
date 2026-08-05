using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Inventory;

[ApiController]
[Authorize]
public sealed class GoodsIssueController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public GoodsIssueController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/goods-issues")]
    [RequirePermission("GoodsIssue.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchGoodsIssueQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/goods-issues/{id:guid}")]
    [RequirePermission("GoodsIssue.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetGoodsIssueByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/goods-issues")]
    [RequirePermission("GoodsIssue.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertGoodsIssueRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateGoodsIssueCommand(request.Number, request.WarehouseCode, request.Reference, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "GoodsIssue created.");
    }

    [HttpPut("api/v1/goods-issues/{id:guid}")]
    [RequirePermission("GoodsIssue.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertGoodsIssueRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateGoodsIssueCommand(id, request.Number, request.WarehouseCode, request.Reference, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "GoodsIssue updated.");
    }

    [HttpDelete("api/v1/goods-issues/{id:guid}")]
    [RequirePermission("GoodsIssue.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteGoodsIssueCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "GoodsIssue deleted.");
    }
}

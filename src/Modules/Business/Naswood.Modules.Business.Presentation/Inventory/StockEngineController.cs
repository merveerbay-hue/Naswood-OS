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
public sealed class StockEngineController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public StockEngineController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpPost("api/v1/goods-receipts/execute")]
    [RequirePermission("GoodsReceipt.Execute")]
    public async Task<IActionResult> ExecuteReceipt([FromBody] ExecuteGoodsReceiptRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ExecuteGoodsReceiptCommand(request.Number, request.WarehouseCode, request.Reference, request.Notes, request.Lines),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Goods receipt posted to stock.");
    }

    [HttpPost("api/v1/goods-issues/execute")]
    [RequirePermission("GoodsIssue.Execute")]
    public async Task<IActionResult> ExecuteIssue([FromBody] ExecuteGoodsIssueRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ExecuteGoodsIssueCommand(request.Number, request.WarehouseCode, request.Reference, request.Notes, request.Lines),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Goods issue posted to stock.");
    }

    [HttpGet("api/v1/packages")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> SearchPackages([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchInventoryPackageQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/material-identities")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> SearchIdentities([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchMaterialIdentityQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-movements")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> SearchMovements([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchInventoryMovementQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

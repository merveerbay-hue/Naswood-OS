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
public sealed class StockTransferController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public StockTransferController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/transfers")]
    [RequirePermission("StockTransfer.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchStockTransferQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/transfers/{id:guid}")]
    [RequirePermission("StockTransfer.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetStockTransferByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/transfers")]
    [RequirePermission("StockTransfer.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertStockTransferRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateStockTransferCommand(request.Number, request.FromWarehouseCode, request.ToWarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "StockTransfer created.");
    }

    [HttpPut("api/v1/transfers/{id:guid}")]
    [RequirePermission("StockTransfer.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertStockTransferRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateStockTransferCommand(id, request.Number, request.FromWarehouseCode, request.ToWarehouseCode, request.Status, request.Notes), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "StockTransfer updated.");
    }

    [HttpDelete("api/v1/transfers/{id:guid}")]
    [RequirePermission("StockTransfer.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteStockTransferCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "StockTransfer deleted.");
    }

    /// <summary>
    /// Intra-factory location transfer: posts source ApplyIssue + dest ApplyReceipt + TRANSFER_OUT/IN ledger pair.
    /// </summary>
    [HttpPost("api/v1/transfers/execute")]
    [RequirePermission("StockTransfer.Create")]
    public async Task<IActionResult> Execute([FromBody] ExecuteStockTransferRequestDto request, CancellationToken cancellationToken)
    {
        var (plantId, error) = PlantClaims.ResolveRequestedPlant(User, request.PlantId);
        if (error is not null)
            return BadRequest(new { success = false, message = error });

        var allowed = PlantClaims.AllowedPlantIds(User);
        var result = await _dispatcher.SendAsync(
            new ExecuteStockTransferCommand(
                request.Number,
                request.MaterialCode,
                request.LotNumber,
                request.FromWarehouseCode,
                request.FromLocationCode,
                request.ToWarehouseCode,
                request.ToLocationCode,
                request.Quantity,
                request.UnitOfMeasure,
                request.PackageNumber,
                request.MaterialIdentityNumber,
                request.Notes,
                plantId,
                request.ToPlantId,
                allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Stock location transfer posted.");
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        var (plantId, error) = PlantClaims.ResolveRequestedPlant(User, request.PlantId);
        if (error is not null || plantId is null)
            return ForbiddenPlant(error, "INV-POST-403");

        var allowed = PlantClaims.AllowedPlantIds(User);
        var result = await _dispatcher.SendAsync(
            new ExecuteGoodsReceiptCommand(
                request.Number,
                request.WarehouseCode,
                request.Reference,
                request.Notes,
                request.QuantityVerified,
                request.ExtractSource ?? string.Empty,
                request.Lines,
                plantId,
                allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Goods receipt posted to stock.");
    }

    [HttpPost("api/v1/goods-issues/execute")]
    [RequirePermission("GoodsIssue.Execute")]
    public async Task<IActionResult> ExecuteIssue([FromBody] ExecuteGoodsIssueRequestDto request, CancellationToken cancellationToken)
    {
        var (plantId, error) = PlantClaims.ResolveRequestedPlant(User, request.PlantId);
        if (error is not null || plantId is null)
            return ForbiddenPlant(error, "INV-POST-403");

        var allowed = PlantClaims.AllowedPlantIds(User);
        var result = await _dispatcher.SendAsync(
            new ExecuteGoodsIssueCommand(
                request.Number,
                request.WarehouseCode,
                request.Reference,
                request.Notes,
                request.Lines,
                plantId,
                allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Goods issue posted to stock.");
    }

    [HttpGet("api/v1/packages")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> SearchPackages(
        [FromQuery] string? q,
        [FromQuery] string? plantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var allowed = PlantClaims.AllowedPlantIds(User);
        var requested = string.IsNullOrWhiteSpace(plantId)
            ? PlantClaims.HomePlantId(User) ?? PlantClaims.WorkingPlantId(User)
            : plantId;
        var (resolved, error) = PlantClaims.ResolveRequestedPlant(User, requested);
        if (error is not null || resolved is null)
            return ForbiddenPlant(error, "INV-PKG-403");

        var result = await _dispatcher.QueryAsync(
            new SearchInventoryPackageQuery(q, page, pageSize, resolved, allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/packages/{id:guid}")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> GetPackageById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetInventoryPackageByIdQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/material-identities")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> SearchIdentities(
        [FromQuery] string? q,
        [FromQuery] string? plantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var allowed = PlantClaims.AllowedPlantIds(User);
        var requested = string.IsNullOrWhiteSpace(plantId)
            ? PlantClaims.HomePlantId(User) ?? PlantClaims.WorkingPlantId(User)
            : plantId;
        var (resolved, error) = PlantClaims.ResolveRequestedPlant(User, requested);
        if (error is not null || resolved is null)
            return ForbiddenPlant(error, "INV-BAL-403");

        var result = await _dispatcher.QueryAsync(
            new SearchMaterialIdentityQuery(q, page, pageSize, resolved, allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-movements")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> SearchMovements(
        [FromQuery] string? q,
        [FromQuery] string? plantId,
        [FromQuery] string? warehouseCode,
        [FromQuery] string? locationCode,
        [FromQuery] string? documentNumber,
        [FromQuery] string? materialCode,
        [FromQuery] string? lotNumber,
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? locationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var allowed = PlantClaims.AllowedPlantIds(User);
        var requested = string.IsNullOrWhiteSpace(plantId)
            ? PlantClaims.HomePlantId(User) ?? PlantClaims.WorkingPlantId(User)
            : plantId;
        var (resolved, error) = PlantClaims.ResolveRequestedPlant(User, requested);
        if (error is not null || resolved is null)
            return ForbiddenPlant(error, "INV-MOV-403");

        var result = await _dispatcher.QueryAsync(
            new SearchInventoryMovementQuery(
                q,
                page,
                pageSize,
                resolved,
                warehouseCode,
                locationCode,
                documentNumber,
                materialCode,
                lotNumber,
                warehouseId,
                locationId,
                allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-movements/{id:guid}")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> GetMovementById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetInventoryMovementByIdQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    private IActionResult ForbiddenPlant(string? message, string code) =>
        StatusCode(StatusCodes.Status403Forbidden, new
        {
            success = false,
            message = message ?? "Bu tesise erişim yetkiniz yok.",
            errors = new[]
            {
                new
                {
                    code,
                    category = "Forbidden",
                    message = message ?? "Bu tesise erişim yetkiniz yok.",
                    details = new { }
                }
            }
        });
}

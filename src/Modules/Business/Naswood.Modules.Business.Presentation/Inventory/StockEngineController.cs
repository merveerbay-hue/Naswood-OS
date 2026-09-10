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

    [HttpGet("api/v1/packages/{id:guid}/passport")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> GetPackagePassport(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetPackagePassportByIdQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/packages/by-barcode/{barcode}")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> GetPackageByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetPackagePassportByBarcodeQuery(barcode, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/packages/by-public/{publicId}")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> GetPackageByPublicId(string publicId, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetPackagePassportByPublicIdQuery(publicId, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/packages/{id:guid}/label-print")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> RecordLabelPrint(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new RecordPackageLabelPrintCommand(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Etiket yazdırma kaydedildi.");
    }

    public sealed class RelocatePackageRequestDto
    {
        public string WarehouseCode { get; init; } = string.Empty;
        public string LocationCode { get; init; } = string.Empty;
    }

    [HttpPost("api/v1/packages/{id:guid}/relocate")]
    [RequirePermission("Inventory.Package.Move")]
    public async Task<IActionResult> RelocatePackage(Guid id, [FromBody] RelocatePackageRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new RelocatePackageCommand(
                id,
                body.WarehouseCode,
                body.LocationCode,
                PlantClaims.AllowedPlantIds(User),
                User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Paket lokasyonu güncellendi.");
    }

    [HttpPost("api/v1/packages/{id:guid}/split")]
    [RequirePermission("Inventory.Package.Split")]
    public async Task<IActionResult> SplitPackage(Guid id, [FromBody] SplitPackageRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new SplitPackageCommand(id, body, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: result.IsSuccess ? result.Value.Message : null);
    }

    [HttpPost("api/v1/packages/merge")]
    [RequirePermission("Inventory.Package.Merge")]
    public async Task<IActionResult> MergePackages([FromBody] MergePackagesRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new MergePackagesCommand(body, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: result.IsSuccess ? result.Value.Message : null);
    }

    [HttpPost("api/v1/packages/{id:guid}/repack")]
    [RequirePermission("Inventory.Package.Repack")]
    public async Task<IActionResult> RepackPackage(Guid id, [FromBody] RepackPackageRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new RepackPackageCommand(id, body, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: result.IsSuccess ? result.Value.Message : null);
    }

    [HttpPost("api/v1/packages/{id:guid}/partial-move")]
    [RequirePermission("Inventory.Package.Move")]
    public async Task<IActionResult> PartialMovePackage(Guid id, [FromBody] PartialMovePackageRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new PartialMovePackageCommand(id, body, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: result.IsSuccess ? result.Value.Message : null);
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

    [HttpGet("api/v1/material-identities/{id:guid}")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> GetIdentityById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetMaterialIdentityByIdQuery(id, PlantClaims.AllowedPlantIds(User)),
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

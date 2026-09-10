using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Inventory;

/// <summary>
/// Master stock view + dated Excel export payload.
/// Read-only. Canonical qty = InventoryBalance. Not the cycle-count Excel template.
/// </summary>
[ApiController]
[Authorize]
public sealed class MasterStockController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public MasterStockController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/inventory-master-stock")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? plantId,
        [FromQuery] string? warehouseCode,
        [FromQuery] string? locationCode,
        [FromQuery] string? materialCode,
        [FromQuery] string? lotNumber,
        [FromQuery] string? stockStatus,
        [FromQuery] string? q,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] DateTimeOffset? asOfDate,
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? locationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (resolved, error) = ResolvePlant(plantId);
        if (error is not null || resolved is null) return ForbiddenPlant(error);

        var result = await _dispatcher.QueryAsync(
            new SearchMasterStockQuery(
                resolved,
                page,
                pageSize,
                warehouseCode,
                locationCode,
                materialCode,
                lotNumber,
                stockStatus,
                q,
                sortBy,
                sortDir,
                asOfDate,
                warehouseId,
                locationId,
                PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-master-stock/packages")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> SearchPackages(
        [FromQuery] string? plantId,
        [FromQuery] string? warehouseCode,
        [FromQuery] string? locationCode,
        [FromQuery] string? materialCode,
        [FromQuery] string? lotNumber,
        [FromQuery] string? stockStatus,
        [FromQuery] string? q,
        [FromQuery] DateTimeOffset? asOfDate,
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? locationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (resolved, error) = ResolvePlant(plantId);
        if (error is not null || resolved is null) return ForbiddenPlant(error);

        var result = await _dispatcher.QueryAsync(
            new SearchMasterStockPackagesQuery(
                resolved,
                page,
                pageSize,
                warehouseCode,
                locationCode,
                materialCode,
                lotNumber,
                stockStatus,
                q,
                warehouseId,
                locationId,
                asOfDate,
                PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-master-stock/{balanceId:guid}/packages")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> PackagesForBalance(Guid balanceId, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetMasterStockBalancePackagesQuery(balanceId, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Full filtered snapshot for Excel (all matching rows, not one page).
    /// Separate from inventory-count field template.
    /// </summary>
    [HttpGet("api/v1/inventory-master-stock/export")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> Export(
        [FromQuery] string? plantId,
        [FromQuery] string? warehouseCode,
        [FromQuery] string? locationCode,
        [FromQuery] string? materialCode,
        [FromQuery] string? lotNumber,
        [FromQuery] string? stockStatus,
        [FromQuery] string? q,
        [FromQuery] DateTimeOffset? asOfDate,
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? locationId,
        CancellationToken cancellationToken = default)
    {
        var (resolved, error) = ResolvePlant(plantId);
        if (error is not null || resolved is null) return ForbiddenPlant(error);

        var result = await _dispatcher.QueryAsync(
            new ExportMasterStockQuery(
                resolved,
                warehouseCode,
                locationCode,
                materialCode,
                lotNumber,
                stockStatus,
                q,
                warehouseId,
                locationId,
                asOfDate,
                Actor(),
                PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    private (string? PlantId, string? Error) ResolvePlant(string? plantId)
    {
        var requested = string.IsNullOrWhiteSpace(plantId)
            ? PlantClaims.HomePlantId(User) ?? PlantClaims.WorkingPlantId(User)
            : plantId;
        return PlantClaims.ResolveRequestedPlant(User, requested);
    }

    private string Actor() =>
        User.FindFirstValue("preferred_username")
        ?? User.Identity?.Name
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(ClaimTypes.Name)
        ?? "unknown";

    private IActionResult ForbiddenPlant(string? message) =>
        StatusCode(StatusCodes.Status403Forbidden, new
        {
            success = false,
            message = message ?? "Bu tesise erişim yetkiniz yok.",
            errors = new[]
            {
                new
                {
                    code = "INV-MST-403",
                    category = "Forbidden",
                    message = message ?? "Bu tesise erişim yetkiniz yok.",
                    details = new { }
                }
            }
        });
}

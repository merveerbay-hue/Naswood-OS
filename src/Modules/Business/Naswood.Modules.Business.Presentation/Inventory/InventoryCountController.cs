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
public sealed class InventoryCountController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public InventoryCountController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/inventory-counts")]
    [RequirePermission("InventoryCount.View")]
    public async Task<IActionResult> Search(
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
            return ForbiddenPlant(error);

        var result = await _dispatcher.QueryAsync(
            new SearchInventoryCountQuery(q, page, pageSize, resolved, allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/inventory-counts/{id:guid}")]
    [RequirePermission("InventoryCount.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetInventoryCountByIdQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/inventory-counts")]
    [RequirePermission("InventoryCount.Create")]
    public async Task<IActionResult> Create(
        [FromBody] UpsertInventoryCountRequestDto request,
        [FromQuery] string? plantId,
        CancellationToken cancellationToken)
    {
        var allowed = PlantClaims.AllowedPlantIds(User);
        var requested = string.IsNullOrWhiteSpace(plantId)
            ? PlantClaims.HomePlantId(User) ?? PlantClaims.WorkingPlantId(User)
            : plantId;
        var (resolved, error) = PlantClaims.ResolveRequestedPlant(User, requested);
        if (error is not null || resolved is null)
            return ForbiddenPlant(error);

        var result = await _dispatcher.SendAsync(
            new CreateInventoryCountCommand(
                request.Number,
                request.WarehouseCode,
                request.Status,
                request.Notes,
                resolved,
                allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryCount created.");
    }

    [HttpPut("api/v1/inventory-counts/{id:guid}")]
    [RequirePermission("InventoryCount.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertInventoryCountRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new UpdateInventoryCountCommand(
                id,
                request.Number,
                request.WarehouseCode,
                request.Status,
                request.Notes,
                PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryCount updated.");
    }

    [HttpDelete("api/v1/inventory-counts/{id:guid}")]
    [RequirePermission("InventoryCount.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DeleteInventoryCountCommand(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "InventoryCount deleted.");
    }

    private IActionResult ForbiddenPlant(string? message) =>
        StatusCode(StatusCodes.Status403Forbidden, new
        {
            success = false,
            message = message ?? "Bu tesise erişim yetkiniz yok.",
            errors = new[]
            {
                new
                {
                    code = "INV-CNT-403",
                    category = "Forbidden",
                    message = message ?? "Bu tesise erişim yetkiniz yok.",
                    details = new { }
                }
            }
        });
}

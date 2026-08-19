using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Inventory;

[ApiController]
[Authorize]
public sealed class InventoryDashboardController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public InventoryDashboardController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/inventory/dashboard")]
    [RequirePermission("Inventory.View")]
    public async Task<IActionResult> Get(
        [FromQuery] string? plantId,
        CancellationToken cancellationToken)
    {
        var allowed = PlantClaims.AllowedPlantIds(User);
        var requested = string.IsNullOrWhiteSpace(plantId)
            ? PlantClaims.HomePlantId(User) ?? PlantClaims.WorkingPlantId(User)
            : plantId;
        var (resolved, error) = PlantClaims.ResolveRequestedPlant(User, requested);
        if (error is not null || resolved is null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                success = false,
                message = error ?? "Bu tesise erişim yetkiniz yok.",
                errors = new[]
                {
                    new
                    {
                        code = "INV-BAL-403",
                        category = "Forbidden",
                        message = error ?? "Bu tesise erişim yetkiniz yok.",
                        details = new { }
                    }
                }
            });
        }

        var result = await _dispatcher.QueryAsync(
            new GetInventoryDashboardQuery(resolved, allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

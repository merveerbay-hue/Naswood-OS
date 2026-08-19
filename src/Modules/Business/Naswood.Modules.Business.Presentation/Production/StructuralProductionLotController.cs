using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Production;

[ApiController]
[Authorize]
public sealed class StructuralProductionLotController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public StructuralProductionLotController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/structural-production-lots")]
    [RequirePermission("StructuralProductionLot.View")]
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
            new SearchStructuralProductionLotQuery(q, page, pageSize, resolved, allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/structural-production-lots/{id:guid}")]
    [RequirePermission("StructuralProductionLot.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetStructuralProductionLotByIdQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/structural-production-lots")]
    [RequirePermission("StructuralProductionLot.Create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateStructuralProductionLotRequestDto request,
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
            new CreateStructuralProductionLotCommand(
                request.MaterialId,
                request.ActualGradingMethod,
                request.WorkCenterCode,
                request.ProductionDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                request.ShiftCode,
                request.Notes ?? string.Empty,
                request.Inputs,
                resolved,
                allowed,
                User.Identity?.Name),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "StructuralProductionLot created.");
    }

    [HttpPut("api/v1/structural-production-lots/{id:guid}/draft")]
    [RequirePermission("StructuralProductionLot.Update")]
    public async Task<IActionResult> UpdateDraft(
        Guid id,
        [FromBody] UpdateStructuralProductionLotDraftRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new UpdateStructuralProductionLotDraftCommand(
                id,
                request.WorkCenterCode,
                request.ProductionDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                request.ShiftCode,
                request.Notes ?? string.Empty,
                request.Inputs,
                PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "StructuralProductionLot draft updated.");
    }

    [HttpPost("api/v1/structural-production-lots/{id:guid}/status")]
    [RequirePermission("StructuralProductionLot.Update")]
    public async Task<IActionResult> TransitionStatus(
        Guid id,
        [FromBody] TransitionStructuralProductionLotStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new TransitionStructuralProductionLotStatusCommand(
                id,
                request.Status,
                PlantClaims.AllowedPlantIds(User),
                User.Identity?.Name),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "StructuralProductionLot status updated.");
    }

    [HttpPost("api/v1/structural-production-lots/{id:guid}/cancel")]
    [RequirePermission("StructuralProductionLot.Update")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelStructuralProductionLotRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new CancelStructuralProductionLotCommand(
                id,
                request.Reason,
                PlantClaims.AllowedPlantIds(User),
                User.Identity?.Name),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "StructuralProductionLot cancelled.");
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
                    code = "PRD-SPL-403",
                    category = "Forbidden",
                    message = message ?? "Bu tesise erişim yetkiniz yok.",
                    details = new { }
                }
            }
        });
}

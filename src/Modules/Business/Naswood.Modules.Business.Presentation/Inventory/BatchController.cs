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
public sealed class BatchController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public BatchController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/batches")]
    [RequirePermission("Batch.View")]
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
            new SearchBatchQuery(q, page, pageSize, resolved, allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/batches/{id:guid}")]
    [RequirePermission("Batch.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetBatchByIdQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/batches")]
    [RequirePermission("Batch.Create")]
    public async Task<IActionResult> Create(
        [FromBody] UpsertBatchRequestDto request,
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
            new CreateBatchCommand(
                request.BatchNumber,
                request.MaterialCode,
                request.Quantity,
                request.ExpiryDate,
                request.Status,
                resolved,
                allowed),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Batch created.");
    }

    [HttpPut("api/v1/batches/{id:guid}")]
    [RequirePermission("Batch.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertBatchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new UpdateBatchCommand(
                id,
                request.BatchNumber,
                request.MaterialCode,
                request.Quantity,
                request.ExpiryDate,
                request.Status,
                PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Batch updated.");
    }

    [HttpDelete("api/v1/batches/{id:guid}")]
    [RequirePermission("Batch.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DeleteBatchCommand(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Batch deleted.");
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
                    code = "INV-LOT-403",
                    category = "Forbidden",
                    message = message ?? "Bu tesise erişim yetkiniz yok.",
                    details = new { }
                }
            }
        });
}

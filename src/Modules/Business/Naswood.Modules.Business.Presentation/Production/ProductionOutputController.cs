using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Production;

[ApiController]
[Authorize]
public sealed class ProductionOutputController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ProductionOutputController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpPost("api/v1/production-outputs/preview")]
    [RequirePermission("ProductionOrder.View")]
    public async Task<IActionResult> Preview([FromBody] PreviewProductionOutputRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new PreviewProductionOutputQuery(body, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/production-outputs")]
    [RequirePermission("ProductionOrder.Update")]
    public async Task<IActionResult> Post([FromBody] PostProductionOutputRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new PostProductionOutputCommand(body, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Üretim çıkışı işlendi.");
    }

    [HttpGet("api/v1/production-lots/{id:guid}/passport")]
    [RequirePermission("ProductionOrder.View")]
    public async Task<IActionResult> LotPassport(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetProductionLotPassportQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/production-outputs/consume-by-barcode/{barcode}")]
    [RequirePermission("ProductionOrder.Update")]
    public async Task<IActionResult> ConsumeByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new ScanProductionConsumptionQuery(barcode, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/production-outputs/{id:guid}/reverse")]
    [RequirePermission("ProductionOrder.Update")]
    public async Task<IActionResult> Reverse(Guid id, [FromBody] ReverseProductionOutputRequestDto? body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ReverseProductionOutputCommand(
                id,
                body?.Reason ?? string.Empty,
                PlantClaims.AllowedPlantIds(User),
                User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Üretim çıkışı tersine çevrildi.");
    }

    [HttpPost("api/v1/production-outputs/{id:guid}/qc")]
    [RequirePermission("QualityInspection.Execute")]
    public async Task<IActionResult> DecideQc(Guid id, [FromBody] ProductionOutputQcRequestDto? body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new DecideProductionOutputQcCommand(
                id,
                body?.Decision ?? string.Empty,
                body?.InspectionReference ?? string.Empty,
                body?.Notes ?? string.Empty,
                PlantClaims.AllowedPlantIds(User),
                User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "QC kararı kaydedildi.");
    }
}

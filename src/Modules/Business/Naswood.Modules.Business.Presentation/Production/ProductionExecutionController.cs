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
public sealed class ProductionExecutionController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ProductionExecutionController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/production-execution/work-centers")]
    [RequirePermission("Production.Execution.View")]
    public async Task<IActionResult> WorkCenters(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new ListShopFloorWorkCentersQuery(PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/production-execution/work-centers/{id:guid}/queue")]
    [RequirePermission("Production.Execution.View")]
    public async Task<IActionResult> Queue(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetShopFloorQueueQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/production-execution/orders/{id:guid}")]
    [RequirePermission("Production.Execution.View")]
    public async Task<IActionResult> Order(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetProductionOrderProgressQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/production-execution/orders/{id:guid}/operations")]
    [RequirePermission("ProductionOrder.Update")]
    public async Task<IActionResult> UpsertOperations(Guid id, [FromBody] IReadOnlyList<UpsertProductionOperationRequestDto> body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new UpsertProductionOperationsCommand(id, body ?? [], PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Operasyon sırası kaydedildi.");
    }

    [HttpPost("api/v1/production-execution/operations/{id:guid}/start")]
    [RequirePermission("Production.Execution.Start")]
    public async Task<IActionResult> Start(Guid id, [FromBody] StartProductionExecutionRequestDto? body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new StartProductionExecutionCommand(id, body?.WorkCenterId, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Operasyon başlatıldı.");
    }

    [HttpGet("api/v1/production-execution/executions/{id:guid}")]
    [RequirePermission("Production.Execution.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new GetProductionExecutionQuery(id, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/pause")]
    [RequirePermission("Production.Execution.Pause")]
    public async Task<IActionResult> Pause(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new PauseProductionExecutionCommand(id, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Operasyon duraklatıldı.");
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/resume")]
    [RequirePermission("Production.Execution.Pause")]
    public async Task<IActionResult> Resume(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ResumeProductionExecutionCommand(id, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Operasyon sürdürüldü.");
    }

    [HttpGet("api/v1/production-execution/executions/{id:guid}/scan/{barcode}")]
    [RequirePermission("Production.Execution.Consume")]
    public async Task<IActionResult> Scan(Guid id, string barcode, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(
            new ScanProductionExecutionQuery(id, barcode, PlantClaims.AllowedPlantIds(User)),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/consume")]
    [RequirePermission("Production.Execution.Consume")]
    public async Task<IActionResult> Consume(Guid id, [FromBody] ConsumeProductionExecutionRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new ConsumeProductionExecutionCommand(id, body, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Tüketim işlendi.");
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/downtime/start")]
    [RequirePermission("Production.Execution.Pause")]
    public async Task<IActionResult> DowntimeStart(Guid id, [FromBody] DowntimeRequestDto? body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new StartDowntimeCommand(id, body ?? new DowntimeRequestDto(), PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Duruş başladı.");
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/downtime/end")]
    [RequirePermission("Production.Execution.Pause")]
    public async Task<IActionResult> DowntimeEnd(Guid id, [FromBody] DowntimeRequestDto? body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new EndDowntimeCommand(id, body ?? new DowntimeRequestDto(), PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Duruş kapandı.");
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/scrap")]
    [RequirePermission("Production.Execution.Scrap")]
    public async Task<IActionResult> Scrap(Guid id, [FromBody] ScrapRequestDto body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new AddExecutionScrapCommand(id, body, PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Fire kaydedildi.");
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/complete")]
    [RequirePermission("Production.Execution.Complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteProductionExecutionRequestDto? body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new CompleteProductionExecutionCommand(id, body ?? new CompleteProductionExecutionRequestDto(), PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Operasyon tamamlandı.");
    }

    [HttpPost("api/v1/production-execution/executions/{id:guid}/cancel")]
    [RequirePermission("Production.Execution.Cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelProductionExecutionRequestDto? body, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
            new CancelProductionExecutionCommand(id, body ?? new CancelProductionExecutionRequestDto(), PlantClaims.AllowedPlantIds(User), User.Identity?.Name ?? string.Empty),
            cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "İcra iptal edildi.");
    }
}

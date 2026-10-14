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
public sealed class ProductionConfirmationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ProductionConfirmationController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/production-confirmations")]
    [RequirePermission("ProductionConfirmation.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchProductionConfirmationQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/production-confirmations/{id:guid}")]
    [RequirePermission("ProductionConfirmation.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetProductionConfirmationByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/production-confirmations")]
    [RequirePermission("ProductionConfirmation.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertProductionConfirmationRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateProductionConfirmationCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionConfirmation created.");

    [HttpPut("api/v1/production-confirmations/{id:guid}")]
    [RequirePermission("ProductionConfirmation.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertProductionConfirmationRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateProductionConfirmationCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionConfirmation updated.");

    [HttpDelete("api/v1/production-confirmations/{id:guid}")]
    [RequirePermission("ProductionConfirmation.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteProductionConfirmationCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionConfirmation deleted.");
}

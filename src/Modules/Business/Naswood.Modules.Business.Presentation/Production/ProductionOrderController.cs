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
public sealed class ProductionOrderController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ProductionOrderController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/production-orders")]
    [RequirePermission("ProductionOrder.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchProductionOrderQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/production-orders/{id:guid}")]
    [RequirePermission("ProductionOrder.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetProductionOrderByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/production-orders")]
    [RequirePermission("ProductionOrder.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertProductionOrderRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateProductionOrderCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionOrder created.");

    [HttpPut("api/v1/production-orders/{id:guid}")]
    [RequirePermission("ProductionOrder.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertProductionOrderRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateProductionOrderCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionOrder updated.");

    [HttpDelete("api/v1/production-orders/{id:guid}")]
    [RequirePermission("ProductionOrder.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteProductionOrderCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionOrder deleted.");
}

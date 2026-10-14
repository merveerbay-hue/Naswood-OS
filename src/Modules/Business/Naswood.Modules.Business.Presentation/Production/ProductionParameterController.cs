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
public sealed class ProductionParameterController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ProductionParameterController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/production-parameters")]
    [RequirePermission("ProductionParameter.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchProductionParameterQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/production-parameters/{id:guid}")]
    [RequirePermission("ProductionParameter.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetProductionParameterByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/production-parameters")]
    [RequirePermission("ProductionParameter.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertProductionParameterRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateProductionParameterCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionParameter created.");

    [HttpPut("api/v1/production-parameters/{id:guid}")]
    [RequirePermission("ProductionParameter.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertProductionParameterRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateProductionParameterCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionParameter updated.");

    [HttpDelete("api/v1/production-parameters/{id:guid}")]
    [RequirePermission("ProductionParameter.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteProductionParameterCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionParameter deleted.");
}

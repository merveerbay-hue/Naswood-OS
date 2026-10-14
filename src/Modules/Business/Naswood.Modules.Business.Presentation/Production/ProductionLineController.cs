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
public sealed class ProductionLineController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ProductionLineController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/production-lines")]
    [RequirePermission("ProductionLine.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchProductionLineQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/production-lines/{id:guid}")]
    [RequirePermission("ProductionLine.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetProductionLineByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/production-lines")]
    [RequirePermission("ProductionLine.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertProductionLineRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateProductionLineCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionLine created.");

    [HttpPut("api/v1/production-lines/{id:guid}")]
    [RequirePermission("ProductionLine.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertProductionLineRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateProductionLineCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionLine updated.");

    [HttpDelete("api/v1/production-lines/{id:guid}")]
    [RequirePermission("ProductionLine.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteProductionLineCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "ProductionLine deleted.");
}

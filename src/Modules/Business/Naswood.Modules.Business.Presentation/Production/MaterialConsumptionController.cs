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
public sealed class MaterialConsumptionController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public MaterialConsumptionController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/material-consumptions")]
    [RequirePermission("MaterialConsumption.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchMaterialConsumptionQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/material-consumptions/{id:guid}")]
    [RequirePermission("MaterialConsumption.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetMaterialConsumptionByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/material-consumptions")]
    [RequirePermission("MaterialConsumption.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertMaterialConsumptionRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreateMaterialConsumptionCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "MaterialConsumption created.");

    [HttpPut("api/v1/material-consumptions/{id:guid}")]
    [RequirePermission("MaterialConsumption.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertMaterialConsumptionRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdateMaterialConsumptionCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "MaterialConsumption updated.");

    [HttpDelete("api/v1/material-consumptions/{id:guid}")]
    [RequirePermission("MaterialConsumption.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeleteMaterialConsumptionCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "MaterialConsumption deleted.");
}

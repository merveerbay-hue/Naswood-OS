using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Inventory;

[ApiController]
[Authorize]
public sealed class MaterialController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public MaterialController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/materials")]
    [RequirePermission("Material.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchMaterialQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/materials/{id:guid}")]
    [RequirePermission("Material.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetMaterialByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/materials")]
    [RequirePermission("Material.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertMaterialRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateMaterialCommand(request.Code, request.Name, request.Description, request.Category, request.UnitOfMeasure, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Material created.");
    }

    [HttpPut("api/v1/materials/{id:guid}")]
    [RequirePermission("Material.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertMaterialRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateMaterialCommand(id, request.Code, request.Name, request.Description, request.Category, request.UnitOfMeasure, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Material updated.");
    }

    [HttpDelete("api/v1/materials/{id:guid}")]
    [RequirePermission("Material.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteMaterialCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Material deleted.");
    }
}

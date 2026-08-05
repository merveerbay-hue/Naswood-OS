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
public sealed class PackagingController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public PackagingController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/packagings")]
    [RequirePermission("Packaging.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => (await _dispatcher.QueryAsync(new SearchPackagingQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpGet("api/v1/packagings/{id:guid}")]
    [RequirePermission("Packaging.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.QueryAsync(new GetPackagingByIdQuery(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this);

    [HttpPost("api/v1/packagings")]
    [RequirePermission("Packaging.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertPackagingRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new CreatePackagingCommand(request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Packaging created.");

    [HttpPut("api/v1/packagings/{id:guid}")]
    [RequirePermission("Packaging.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertPackagingRequestDto request, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new UpdatePackagingCommand(id, request.Code, request.Name, request.Status, request.Notes, request.PlantId), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Packaging updated.");

    [HttpDelete("api/v1/packagings/{id:guid}")]
    [RequirePermission("Packaging.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => (await _dispatcher.SendAsync(new DeletePackagingCommand(id), cancellationToken).ConfigureAwait(false)).ToActionResult(this, successMessage: "Packaging deleted.");
}

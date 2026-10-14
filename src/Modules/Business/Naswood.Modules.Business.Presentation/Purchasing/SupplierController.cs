using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Purchasing;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Purchasing;

[ApiController]
[Authorize]
public sealed class SupplierController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public SupplierController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/suppliers")]
    [RequirePermission("Supplier.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchSupplierQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/suppliers/{id:guid}")]
    [RequirePermission("Supplier.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSupplierByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/suppliers")]
    [RequirePermission("Supplier.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertSupplierRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateSupplierCommand(request.Code, request.Name, request.TaxNumber, request.Email, request.Phone, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Supplier created.");
    }

    [HttpPut("api/v1/suppliers/{id:guid}")]
    [RequirePermission("Supplier.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertSupplierRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateSupplierCommand(id, request.Code, request.Name, request.TaxNumber, request.Email, request.Phone, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Supplier updated.");
    }

    [HttpDelete("api/v1/suppliers/{id:guid}")]
    [RequirePermission("Supplier.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteSupplierCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Supplier deleted.");
    }
}

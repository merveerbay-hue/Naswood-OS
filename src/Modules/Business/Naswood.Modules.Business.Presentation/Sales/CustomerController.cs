using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Business.Application.Sales;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Presentation.Security;

namespace Naswood.Modules.Business.Presentation.Sales;

[ApiController]
[Authorize]
public sealed class CustomerController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public CustomerController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/customers")]
    [RequirePermission("Customer.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchCustomerQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/customers/{id:guid}")]
    [RequirePermission("Customer.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetCustomerByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/customers")]
    [RequirePermission("Customer.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertCustomerRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateCustomerCommand(request.Code, request.Name, request.TaxNumber, request.Email, request.Phone, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Customer created.");
    }

    [HttpPut("api/v1/customers/{id:guid}")]
    [RequirePermission("Customer.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertCustomerRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateCustomerCommand(id, request.Code, request.Name, request.TaxNumber, request.Email, request.Phone, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Customer updated.");
    }

    [HttpDelete("api/v1/customers/{id:guid}")]
    [RequirePermission("Customer.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteCustomerCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Customer deleted.");
    }
}

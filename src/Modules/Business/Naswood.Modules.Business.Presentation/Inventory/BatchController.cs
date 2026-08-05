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
public sealed class BatchController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public BatchController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/batches")]
    [RequirePermission("Batch.View")]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(new SearchBatchQuery(q, page, pageSize), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/batches/{id:guid}")]
    [RequirePermission("Batch.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetBatchByIdQuery(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/batches")]
    [RequirePermission("Batch.Create")]
    public async Task<IActionResult> Create([FromBody] UpsertBatchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new CreateBatchCommand(request.BatchNumber, request.MaterialCode, request.Quantity, request.ExpiryDate, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Batch created.");
    }

    [HttpPut("api/v1/batches/{id:guid}")]
    [RequirePermission("Batch.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertBatchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateBatchCommand(id, request.BatchNumber, request.MaterialCode, request.Quantity, request.ExpiryDate, request.Status), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Batch updated.");
    }

    [HttpDelete("api/v1/batches/{id:guid}")]
    [RequirePermission("Batch.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteBatchCommand(id), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Batch deleted.");
    }
}

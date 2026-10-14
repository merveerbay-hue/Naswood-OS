using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Audit;
using Naswood.Modules.Platform.Presentation.Authorization;

namespace Naswood.Modules.Platform.Presentation.Audit;

[ApiController]
[Authorize]
public sealed class AuditController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuditController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/audit")]
    [RequirePermission("Audit.View")]
    public Task<IActionResult> List(
        [FromQuery] string? module,
        [FromQuery] string? entity,
        [FromQuery] string? entityId,
        [FromQuery] string? action,
        [FromQuery] Guid? userId,
        [FromQuery] Guid? sessionId,
        [FromQuery] string? companyId,
        [FromQuery] string? plantId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        Search(module, entity, entityId, action, userId, sessionId, companyId, plantId, from, to, page, pageSize, cancellationToken);

    [HttpGet("api/v1/audit/search")]
    [RequirePermission("Audit.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? module,
        [FromQuery] string? entity,
        [FromQuery] string? entityId,
        [FromQuery] string? action,
        [FromQuery] Guid? userId,
        [FromQuery] Guid? sessionId,
        [FromQuery] string? companyId,
        [FromQuery] string? plantId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new SearchAuditLogsQuery(
                    module,
                    entity,
                    entityId,
                    action,
                    userId,
                    sessionId,
                    companyId,
                    plantId,
                    from,
                    to,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/audit/export")]
    [RequirePermission("Audit.Export")]
    public async Task<IActionResult> Export(
        [FromQuery] string? module,
        [FromQuery] string? entity,
        [FromQuery] string? entityId,
        [FromQuery] string? action,
        [FromQuery] Guid? userId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new ExportAuditLogsQuery(module, entity, entityId, action, userId, from, to),
                cancellationToken)
            .ConfigureAwait(false);
        if (result.IsFailure)
        {
            return result.ToActionResult(this);
        }

        return File(Encoding.UTF8.GetBytes(result.Value), "text/csv", "audit.csv");
    }

    [HttpGet("api/v1/audit/entity/{entityId}")]
    [RequirePermission("Audit.View")]
    public async Task<IActionResult> ByEntity(
        string entityId,
        [FromQuery] string? entity,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new GetAuditByEntityQuery(entityId, entity, page, pageSize),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/audit/{id:guid}")]
    [RequirePermission("Audit.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetAuditLogByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

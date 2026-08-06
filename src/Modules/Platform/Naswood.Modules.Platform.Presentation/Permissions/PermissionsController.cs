using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Contracts.Authorization;
using Naswood.Modules.Platform.Presentation.Authorization;

namespace Naswood.Modules.Platform.Presentation.Permissions;

[ApiController]
[Authorize]
public sealed class PermissionsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public PermissionsController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/permissions")]
    [RequirePermission("Permission.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? code,
        [FromQuery] string? module,
        [FromQuery] string? feature,
        [FromQuery] string? action,
        [FromQuery] string? category,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new SearchPermissionsQuery(code, module, feature, action, category, isActive, page, pageSize),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/permissions/templates")]
    [RequirePermission("Permission.View")]
    public async Task<IActionResult> Templates(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPermissionTemplatesQuery(), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/permissions/{id:guid}")]
    [RequirePermission("Permission.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPermissionByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/permissions")]
    [RequirePermission("Permission.Create")]
    public async Task<IActionResult> Create(
        [FromBody] CreatePermissionRequestDto request,
        CancellationToken cancellationToken)
    {
        var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? request.Code : request.DisplayName;
        var result = await _dispatcher.SendAsync(
                new CreatePermissionCommand(
                    request.Code,
                    request.Module,
                    request.Feature,
                    request.Action,
                    request.Field,
                    displayName,
                    request.Category,
                    request.Description,
                    request.DependsOn),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Permission created successfully.");
    }

    [HttpPut("api/v1/permissions/{id:guid}")]
    [RequirePermission("Permission.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePermissionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new UpdatePermissionCommand(
                    id,
                    request.DisplayName,
                    request.Category,
                    request.Description,
                    request.DependsOn,
                    request.IsActive),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Permission updated successfully.");
    }

    [HttpDelete("api/v1/permissions/{id:guid}")]
    [RequirePermission("Permission.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeletePermissionCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Permission deleted successfully.");
    }

    [HttpPost("api/v1/permissions/validate")]
    [RequirePermission("Permission.Configure")]
    public async Task<IActionResult> Validate(
        [FromBody] ValidatePermissionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new ValidatePermissionCommand(
                    request.Code,
                    request.Module,
                    request.Feature,
                    request.Action,
                    request.DependsOn),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Contracts.Authorization;
using Naswood.Modules.Platform.Presentation.Authorization;

namespace Naswood.Modules.Platform.Presentation.Roles;

[ApiController]
[Authorize]
public sealed class RolesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public RolesController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/roles")]
    [RequirePermission("Role.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? code,
        [FromQuery] string? name,
        [FromQuery] string? company,
        [FromQuery] string? plant,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new SearchRolesQuery(code, name, company, plant, isActive, page, pageSize),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/roles/{id:guid}")]
    [RequirePermission("Role.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetRoleByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/roles")]
    [RequirePermission("Role.Create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        var company = request.CompanyCode ?? request.Company;
        var result = await _dispatcher.SendAsync(
                new CreateRoleCommand(
                    request.Code,
                    request.Name,
                    request.Description,
                    company,
                    request.PlantCode,
                    request.DepartmentCode,
                    request.Category,
                    request.Permissions),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Role created successfully.");
    }

    [HttpPut("api/v1/roles/{id:guid}")]
    [RequirePermission("Role.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new UpdateRoleCommand(
                    id,
                    request.Name,
                    request.Description,
                    request.CompanyCode,
                    request.PlantCode,
                    request.DepartmentCode,
                    request.Category,
                    request.Permissions),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Role updated successfully.");
    }

    [HttpDelete("api/v1/roles/{id:guid}")]
    [RequirePermission("Role.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteRoleCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Role deleted successfully.");
    }

    [HttpPost("api/v1/roles/{id:guid}/clone")]
    [RequirePermission("Role.Clone")]
    public async Task<IActionResult> Clone(
        Guid id,
        [FromBody] CloneRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new CloneRoleCommand(id, request.Code, request.Name),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Role cloned successfully.");
    }

    [HttpPost("api/v1/roles/{id:guid}/activate")]
    [RequirePermission("Role.Update")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new ActivateRoleCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Role activated successfully.");
    }

    [HttpPost("api/v1/roles/{id:guid}/deactivate")]
    [RequirePermission("Role.Update")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeactivateRoleCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Role deactivated successfully.");
    }

    [HttpPost("api/v1/roles/{id:guid}/assign-permission")]
    [RequirePermission("Role.Configure")]
    public async Task<IActionResult> AssignPermission(
        Guid id,
        [FromBody] RolePermissionCodesRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new AssignRolePermissionsCommand(id, request.Permissions),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Permissions assigned successfully.");
    }

    [HttpPost("api/v1/roles/{id:guid}/remove-permission")]
    [RequirePermission("Role.Configure")]
    public async Task<IActionResult> RemovePermission(
        Guid id,
        [FromBody] RolePermissionCodesRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new RemoveRolePermissionsCommand(id, request.Permissions),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Permissions removed successfully.");
    }

    [HttpPost("api/v1/roles/{id:guid}/assign-user")]
    [RequirePermission("Role.Assign")]
    public async Task<IActionResult> AssignUser(
        Guid id,
        [FromBody] RoleUserAssignmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new AssignRoleToUserCommand(id, request.UserId),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User assigned to role successfully.");
    }

    [HttpPost("api/v1/roles/{id:guid}/remove-user")]
    [RequirePermission("Role.Assign")]
    public async Task<IActionResult> RemoveUser(
        Guid id,
        [FromBody] RoleUserAssignmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new RemoveRoleFromUserCommand(id, request.UserId),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User removed from role successfully.");
    }
}

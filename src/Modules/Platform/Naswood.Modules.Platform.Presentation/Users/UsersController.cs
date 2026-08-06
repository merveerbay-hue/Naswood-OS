using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Users;
using Naswood.Modules.Platform.Contracts.Users;
using Naswood.Modules.Platform.Presentation.Authorization;

namespace Naswood.Modules.Platform.Presentation.Users;

[ApiController]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public UsersController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/users")]
    [RequirePermission("User.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? employeeNumber,
        [FromQuery] string? username,
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] string? department,
        [FromQuery] string? company,
        [FromQuery] string? plant,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new GetUsersQuery(
                    employeeNumber,
                    username,
                    name,
                    email,
                    department,
                    company,
                    plant,
                    status,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/users/export")]
    [RequirePermission("User.Export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new ExportUsersQuery(), cancellationToken)
            .ConfigureAwait(false);
        if (result.IsFailure)
        {
            return result.ToActionResult(this);
        }

        var bytes = Encoding.UTF8.GetBytes(result.Value);
        return File(bytes, "text/csv", "users.csv");
    }

    [HttpGet("api/v1/users/{id:guid}")]
    [RequirePermission("User.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetUserByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/users")]
    [RequirePermission("User.Create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var companies = ResolveCodes(request.CompanyIds, request.Company);
        var plants = ResolveCodes(request.PlantIds, request.Plant);

        var result = await _dispatcher.SendAsync(
                new CreateUserCommand(
                    request.EmployeeNumber,
                    request.Username,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Password,
                    companies,
                    plants,
                    request.Roles ?? Array.Empty<string>(),
                    request.DepartmentCode,
                    request.PositionCode,
                    request.Phone,
                    request.MobilePhone),
                cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this, successMessage: "User created successfully.");
    }

    [HttpPut("api/v1/users/{id:guid}")]
    [RequirePermission("User.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new UpdateUserCommand(
                    id,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Phone,
                    request.MobilePhone,
                    request.CompanyIds,
                    request.PlantIds,
                    request.DepartmentCode,
                    request.PositionCode,
                    request.ManagerUserId,
                    request.CostCenter,
                    request.HireDate,
                    request.EmploymentType,
                    request.EmployeeCategory,
                    request.Language,
                    request.TimeZone,
                    request.DateFormat,
                    request.NumberFormat,
                    request.Currency,
                    request.Theme),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User updated successfully.");
    }

    [HttpDelete("api/v1/users/{id:guid}")]
    [RequirePermission("User.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteUserCommand(id, null), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User deleted successfully.");
    }

    [HttpPost("api/v1/users/{id:guid}/activate")]
    [RequirePermission("User.Update")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new ActivateUserCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User activated successfully.");
    }

    [HttpPost("api/v1/users/{id:guid}/deactivate")]
    [RequirePermission("User.Update")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeactivateUserCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User deactivated successfully.");
    }

    [HttpPost("api/v1/users/{id:guid}/lock")]
    [RequirePermission("User.Lock")]
    public async Task<IActionResult> Lock(
        Guid id,
        [FromBody] LockUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new LockUserCommand(id, request.Reason), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User locked successfully.");
    }

    [HttpPost("api/v1/users/{id:guid}/unlock")]
    [RequirePermission("User.Unlock")]
    public async Task<IActionResult> Unlock(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UnlockUserCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User unlocked successfully.");
    }

    [HttpPost("api/v1/users/{id:guid}/reset-password")]
    [RequirePermission("User.ResetPassword")]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        [FromBody] ResetPasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new ResetUserPasswordCommand(id, request.NewPassword),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Password reset successfully.");
    }

    [HttpPost("api/v1/users/{id:guid}/assign-role")]
    [RequirePermission("User.AssignRole")]
    public async Task<IActionResult> AssignRole(
        Guid id,
        [FromBody] AssignRolesRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new AssignUserRolesCommand(id, request.Roles),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Roles assigned successfully.");
    }

    [HttpPost("api/v1/users/{id:guid}/assign-plant")]
    [RequirePermission("User.Update")]
    public async Task<IActionResult> AssignPlant(
        Guid id,
        [FromBody] AssignPlantsRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new AssignUserPlantsCommand(id, request.PlantIds),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Plants assigned successfully.");
    }

    [HttpPost("api/v1/users/import")]
    [RequirePermission("User.Import")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Import(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "CSV file is required." });
        }

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var result = await _dispatcher.SendAsync(new ImportUsersCommand(content), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "User import completed.");
    }

    private static IReadOnlyList<string> ResolveCodes(IReadOnlyList<string>? codes, string? single)
    {
        if (codes is { Count: > 0 })
        {
            return codes;
        }

        return string.IsNullOrWhiteSpace(single) ? [] : [single];
    }
}

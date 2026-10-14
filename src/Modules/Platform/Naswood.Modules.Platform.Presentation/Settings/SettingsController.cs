using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Settings;
using Naswood.Modules.Platform.Contracts.Settings;
using Naswood.Modules.Platform.Presentation.Authorization;

namespace Naswood.Modules.Platform.Presentation.Settings;

[ApiController]
[Authorize]
public sealed class SettingsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public SettingsController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/settings")]
    [RequirePermission("Settings.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? category,
        [FromQuery] string? key,
        [FromQuery] string? scope,
        [FromQuery] string? companyId,
        [FromQuery] string? plantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new SearchSettingsQuery(category, key, scope, companyId, plantId, page, pageSize),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/settings/categories")]
    [RequirePermission("Settings.View")]
    public async Task<IActionResult> Categories(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSettingCategoriesQuery(), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/settings/export")]
    [RequirePermission("Settings.Export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new ExportSettingsQuery(), cancellationToken)
            .ConfigureAwait(false);
        if (result.IsFailure)
        {
            return result.ToActionResult(this);
        }

        return File(Encoding.UTF8.GetBytes(result.Value), "application/json", "settings.json");
    }

    [HttpGet("api/v1/settings/{id:guid}")]
    [RequirePermission("Settings.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetSettingByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/settings")]
    [RequirePermission("Settings.Create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateSettingRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new CreateSettingCommand(
                    request.Category,
                    request.Key,
                    request.Name,
                    request.Description,
                    request.Value,
                    request.DataType,
                    request.DefaultValue,
                    request.Scope,
                    request.CompanyId,
                    request.PlantId,
                    request.UserId,
                    request.ValidationRule,
                    request.IsRequired),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Setting created successfully.");
    }

    [HttpPut("api/v1/settings/{id:guid}")]
    [RequirePermission("Settings.Update")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSettingRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new UpdateSettingCommand(id, request.Value), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Setting updated successfully.");
    }

    [HttpPost("api/v1/settings/reset")]
    [RequirePermission("Settings.Restore")]
    public async Task<IActionResult> Reset(
        [FromBody] ResetSettingRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new ResetSettingCommand(request.Id, request.Key),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Setting reset to default.");
    }

    [HttpPost("api/v1/settings/import")]
    [RequirePermission("Settings.Import")]
    public async Task<IActionResult> Import(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "JSON file is required." });
        }

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var result = await _dispatcher.SendAsync(new ImportSettingsCommand(content), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Settings imported.");
    }
}

public sealed class ResetSettingRequestDto
{
    public Guid? Id { get; init; }

    public string? Key { get; init; }
}

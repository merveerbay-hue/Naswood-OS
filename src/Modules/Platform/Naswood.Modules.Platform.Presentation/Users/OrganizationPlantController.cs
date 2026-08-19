using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Users;
using Naswood.Modules.Platform.Presentation.Authorization;

namespace Naswood.Modules.Platform.Presentation.Users;

/// <summary>Sistem Yöneticisi — tesis (fabrika) tanımı.</summary>
[ApiController]
[Authorize]
public sealed class OrganizationPlantController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public OrganizationPlantController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/organization/plants")]
    [RequirePermission("Plant.Manage")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new ListOrganizationPlantsQuery(), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/organization/plants")]
    [RequirePermission("Plant.Manage")]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationPlantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new CreateOrganizationPlantCommand(request.Code, request.Name, request.CompanyCode),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "Plant created.");
    }
}

public sealed class CreateOrganizationPlantRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string CompanyCode { get; init; } = "COMP-001";
}

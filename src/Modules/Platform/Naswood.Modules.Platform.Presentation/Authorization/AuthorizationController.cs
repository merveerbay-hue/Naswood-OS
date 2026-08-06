using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Contracts.Authorization;

namespace Naswood.Modules.Platform.Presentation.Authorization;

[ApiController]
[Authorize]
public sealed class AuthorizationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuthorizationController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/permissions")]
    [RequirePermission("Authorization.View")]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetPermissionsQuery(), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/me/permissions")]
    public async Task<IActionResult> GetMyPermissions(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetMyPermissionsQuery(), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpPost("api/v1/authorization/check")]
    public async Task<IActionResult> Check(
        [FromBody] AuthorizationCheckRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new CheckAuthorizationCommand(
                    request.Permission,
                    request.CompanyId,
                    request.PlantId,
                    request.ResourceOwnerId,
                    request.Field),
                cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/authorization/modules")]
    public async Task<IActionResult> GetModules(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetAuthorizedModulesQuery(), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/authorization/menu")]
    public async Task<IActionResult> GetMenu(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetAuthorizedMenuQuery(), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }
}

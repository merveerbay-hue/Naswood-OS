using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Contracts.Authentication;

namespace Naswood.Modules.Platform.Presentation.Authentication;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuthenticationController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("auth-login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new LoginCommand(
                    request.Username,
                    request.Password,
                    request.RememberMe,
                    request.CompanyId,
                    request.PlantId,
                    request.DeviceId,
                    request.DeviceName,
                    request.Browser,
                    request.OperatingSystem),
                cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new LogoutCommand(), cancellationToken).ConfigureAwait(false);
        return result.ToActionResult(this, "Logged out.");
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher
            .SendAsync(new RefreshTokenCommand(request.RefreshToken), cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }

    [AllowAnonymous]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(
        [FromBody] RevokeTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher
            .SendAsync(new RevokeTokenCommand(request.RefreshToken), cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this, "Token revoked.");
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var result = await _dispatcher
            .QueryAsync(new GetCurrentUserQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("session")]
    public async Task<IActionResult> Session(CancellationToken cancellationToken)
    {
        var result = await _dispatcher
            .QueryAsync(new GetCurrentSessionQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.ToActionResult(this);
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Naswood.Modules.Platform.Application.Authentication;

namespace Naswood.Modules.Platform.Infrastructure.Authentication;

public sealed class HttpAuthRequestContext : IAuthRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAuthRequestContext(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    public string? IpAddress =>
        HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string CorrelationId =>
        HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");

    public Guid? UserId =>
        ParseGuid(
            HttpContext?.User.FindFirstValue("sub")
            ?? HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier));

    public Guid? SessionId =>
        ParseGuid(HttpContext?.User.FindFirstValue("session_id"));

    public string? CompanyId =>
        HttpContext?.User.FindFirstValue("company_id");

    public string? PlantId =>
        HttpContext?.User.FindFirstValue("plant_id");

    private static Guid? ParseGuid(string? value) =>
        Guid.TryParse(value, out var id) ? id : null;
}

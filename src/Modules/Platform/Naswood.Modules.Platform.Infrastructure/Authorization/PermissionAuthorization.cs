using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Authorization;

namespace Naswood.Modules.Platform.Infrastructure.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission) => Permission = permission;

    public string Permission { get; }
}

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory) =>
        _scopeFactory = scopeFactory;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        using var scope = _scopeFactory.CreateScope();
        var engine = scope.ServiceProvider.GetRequiredService<IAuthorizationEngine>();
        var users = scope.ServiceProvider.GetRequiredService<IAuthUserRepository>();
        var requestContext = scope.ServiceProvider.GetRequiredService<IAuthRequestContext>();

        if (requestContext.UserId is null)
        {
            return;
        }

        var user = await users.GetByIdAsync(requestContext.UserId.Value).ConfigureAwait(false);
        if (user is null)
        {
            return;
        }

        var decision = await engine.EvaluateAsync(
                new AuthorizationEvaluationRequest(
                    user.Id,
                    user.Roles,
                    user.CompanyIds,
                    user.PlantIds,
                    requirement.Permission,
                    requestContext.CompanyId,
                    requestContext.PlantId,
                    ResourceOwnerId: null,
                    Field: null,
                    RecordHistory: true))
            .ConfigureAwait(false);

        if (decision.Allowed)
        {
            context.Succeed(requirement);
        }
    }
}

public static class PermissionPolicyProvider
{
    public const string Prefix = "Permission:";
}

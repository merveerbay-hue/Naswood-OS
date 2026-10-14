using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Naswood.Modules.Platform.Infrastructure.Authorization;

public sealed class DynamicPermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public DynamicPermissionPolicyProvider(IOptions<AuthorizationOptions> options) =>
        _fallback = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PermissionPolicyProvider.Prefix, StringComparison.Ordinal))
        {
            var permission = policyName[PermissionPolicyProvider.Prefix.Length..];
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission))
                .RequireAuthenticatedUser()
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}

using Microsoft.AspNetCore.Authorization;

namespace Naswood.Modules.Platform.Presentation.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission:";

    public RequirePermissionAttribute(string permission)
        : base(policy: PolicyPrefix + permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}

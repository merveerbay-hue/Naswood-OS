using Microsoft.AspNetCore.Authorization;

namespace Naswood.Modules.Business.Presentation.Security;

public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}

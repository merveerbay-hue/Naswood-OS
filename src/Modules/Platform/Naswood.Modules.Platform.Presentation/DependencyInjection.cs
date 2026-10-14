using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Naswood.Modules.Platform.Presentation;

public static class DependencyInjection
{
    public static IMvcBuilder AddPlatformPresentation(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddApplicationPart(Assembly.GetExecutingAssembly());
        return mvcBuilder;
    }
}

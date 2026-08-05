using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Naswood.Modules.Business.Presentation;

public static class DependencyInjection
{
    public static IMvcBuilder AddBusinessPresentation(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddApplicationPart(Assembly.GetExecutingAssembly());
        return mvcBuilder;
    }
}

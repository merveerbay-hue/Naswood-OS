using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Platform.Application.Health;
using Naswood.Modules.Platform.Infrastructure.Health;

namespace Naswood.Modules.Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatformInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPlatformRuntimeInfo, PlatformRuntimeInfo>();
        services.AddSingleton<IHealthComponentProbe, ApplicationHealthProbe>();
        return services;
    }
}

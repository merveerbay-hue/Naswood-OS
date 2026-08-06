using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Platform.Application.Health;

namespace Naswood.Modules.Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatformApplication(this IServiceCollection services)
    {
        // Handlers are registered by assembly scanning in BuildingBlocks Infrastructure.
        // This method exists so the Host composes Platform Application explicitly.
        return services;
    }
}

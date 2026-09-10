using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Modules.Business.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessApplication(this IServiceCollection services)
    {
        services.AddScoped<PackagePassportLoader>();
        services.AddScoped<PackageLifecycleGateway>();
        services.AddScoped<Naswood.Modules.Business.Application.Production.ProductionOutputGateway>();
        services.AddScoped<Naswood.Modules.Business.Application.Production.ProductionExecutionGateway>();
        return services;
    }
}

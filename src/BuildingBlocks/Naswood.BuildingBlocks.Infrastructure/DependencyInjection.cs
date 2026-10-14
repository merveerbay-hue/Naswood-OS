using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Infrastructure.Dispatching;

namespace Naswood.BuildingBlocks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocksInfrastructure(
        this IServiceCollection services,
        params Assembly[] handlerAssemblies)
    {
        services.AddScoped<IDispatcher, Dispatcher>();

        foreach (var assembly in handlerAssemblies)
        {
            RegisterHandlers(services, assembly, typeof(ICommandHandler<>));
            RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));
            RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));
        }

        return services;
    }

    private static void RegisterHandlers(
        IServiceCollection services,
        Assembly assembly,
        Type openHandlerType)
    {
        var implementations = assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .SelectMany(type => type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openHandlerType)
                .Select(i => new { Service = i, Implementation = type }));

        foreach (var registration in implementations)
        {
            services.AddScoped(registration.Service, registration.Implementation);
        }
    }
}

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Naswood.BuildingBlocks.Application.Abstractions;

namespace Naswood.BuildingBlocks.Infrastructure.Dispatching;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync))
            ?? throw new InvalidOperationException($"HandleAsync was not found on {handlerType.Name}.");

        var task = (Task)method.Invoke(handler, [command, cancellationToken])!;
        await task.ConfigureAwait(false);
    }

    public async Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"HandleAsync was not found on {handlerType.Name}.");

        var task = (Task<TResponse>)method.Invoke(handler, [command, cancellationToken])!;
        return await task.ConfigureAwait(false);
    }

    public async Task<TResponse> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResponse>, TResponse>.HandleAsync))
            ?? throw new InvalidOperationException($"HandleAsync was not found on {handlerType.Name}.");

        var task = (Task<TResponse>)method.Invoke(handler, [query, cancellationToken])!;
        return await task.ConfigureAwait(false);
    }
}

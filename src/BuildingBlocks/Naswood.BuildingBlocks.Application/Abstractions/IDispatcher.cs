namespace Naswood.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Minimal dispatcher that routes commands and queries to their handlers.
/// Keeps Application independent of any specific mediator package.
/// </summary>
public interface IDispatcher
{
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

    Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default);

    Task<TResponse> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default);
}

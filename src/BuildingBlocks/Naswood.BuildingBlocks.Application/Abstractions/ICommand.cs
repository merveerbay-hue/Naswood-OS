namespace Naswood.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Marker for a CQRS command that changes system state.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// CQRS command that returns a typed result.
/// </summary>
public interface ICommand<TResponse> : ICommand
{
}

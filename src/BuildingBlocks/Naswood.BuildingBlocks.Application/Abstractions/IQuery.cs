namespace Naswood.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Marker for a CQRS query that reads system state without mutation.
/// </summary>
public interface IQuery<TResponse>
{
}

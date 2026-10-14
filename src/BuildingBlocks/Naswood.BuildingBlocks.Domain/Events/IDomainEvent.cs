namespace Naswood.BuildingBlocks.Domain;

/// <summary>
/// Domain event raised by an aggregate within a local transaction boundary.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }
}

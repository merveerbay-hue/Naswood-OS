namespace Naswood.BuildingBlocks.Domain;

public abstract record DomainEventBase : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}

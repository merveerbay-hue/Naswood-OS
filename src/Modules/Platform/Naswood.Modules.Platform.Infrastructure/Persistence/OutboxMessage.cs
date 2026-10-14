namespace Naswood.Modules.Platform.Infrastructure.Persistence;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public Guid? UserId { get; set; }

    public string CorrelationId { get; set; } = string.Empty;

    public string Source { get; set; } = "Platform";

    public DateTimeOffset OccurredAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ProcessedAt { get; set; }
}

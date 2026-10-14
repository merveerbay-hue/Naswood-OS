using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public static class ProductionExecutionEventTypes
{
    public const string Start = "START";
    public const string Pause = "PAUSE";
    public const string Resume = "RESUME";
    public const string Complete = "COMPLETE";
    public const string DowntimeStart = "DOWNTIME_START";
    public const string DowntimeEnd = "DOWNTIME_END";
    public const string Cancel = "CANCEL";
    public const string Reversal = "REVERSAL";
}

public sealed class ProductionExecutionEvent : BusinessEntity
{
    private ProductionExecutionEvent() { }

    private ProductionExecutionEvent(
        Guid id,
        Guid executionId,
        string eventType,
        DateTimeOffset occurredAt,
        string userId,
        string reasonCode,
        string note,
        string companyId,
        string? plantId)
        : base(id)
    {
        ExecutionId = executionId;
        EventType = eventType;
        OccurredAt = occurredAt;
        UserId = userId;
        ReasonCode = reasonCode;
        Note = note;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = occurredAt;
    }

    public Guid ExecutionId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string ReasonCode { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;

    public static ProductionExecutionEvent Create(
        Guid executionId,
        string eventType,
        DateTimeOffset occurredAt,
        string userId,
        string? plantId,
        string? reasonCode = null,
        string? note = null,
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            executionId,
            eventType,
            occurredAt,
            userId ?? string.Empty,
            (reasonCode ?? string.Empty).Trim(),
            (note ?? string.Empty).Trim(),
            companyId,
            plantId);
}

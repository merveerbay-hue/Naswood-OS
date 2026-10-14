using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public sealed class ProductionExecutionScrap : BusinessEntity
{
    private ProductionExecutionScrap() { }

    private ProductionExecutionScrap(
        Guid id,
        Guid executionId,
        decimal quantity,
        string unit,
        string reasonCode,
        string note,
        string userId,
        DateTimeOffset recordedAt,
        string companyId,
        string? plantId)
        : base(id)
    {
        ExecutionId = executionId;
        Quantity = quantity;
        Unit = unit;
        ReasonCode = reasonCode;
        Note = note;
        UserId = userId;
        RecordedAt = recordedAt;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = recordedAt;
    }

    public Guid ExecutionId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public string ReasonCode { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public DateTimeOffset RecordedAt { get; private set; }

    public static ProductionExecutionScrap Create(
        Guid executionId,
        decimal quantity,
        string unit,
        string reasonCode,
        string note,
        string userId,
        DateTimeOffset recordedAt,
        string? plantId,
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            executionId,
            quantity,
            unit ?? string.Empty,
            (reasonCode ?? string.Empty).Trim().ToUpperInvariant(),
            (note ?? string.Empty).Trim(),
            userId ?? string.Empty,
            recordedAt,
            companyId,
            plantId);
}

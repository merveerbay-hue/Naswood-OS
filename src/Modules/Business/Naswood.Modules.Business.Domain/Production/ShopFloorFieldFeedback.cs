using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public static class ShopFloorFeedbackTopics
{
    public static readonly string[] All = ["BARCODE_FAIL", "FIELD_UNNECESSARY", "TOO_SLOW", "WRONG_STOCK", "OTHER"];
}

/// <summary>Pilot field note. Not a stock or execution command.</summary>
public sealed class ShopFloorFieldFeedback : BusinessEntity
{
    private ShopFloorFieldFeedback() { }

    private ShopFloorFieldFeedback(
        Guid id,
        string topic,
        string note,
        string screen,
        string userId,
        Guid? workCenterId,
        string workCenterCode,
        Guid? executionId,
        string executionNumber,
        string productionOrderNumber,
        string companyId,
        string? plantId)
        : base(id)
    {
        Topic = topic;
        Note = note;
        Screen = screen;
        UserId = userId;
        WorkCenterId = workCenterId;
        WorkCenterCode = workCenterCode;
        ExecutionId = executionId;
        ExecutionNumber = executionNumber;
        ProductionOrderNumber = productionOrderNumber;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Topic { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public string Screen { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public Guid? WorkCenterId { get; private set; }
    public string WorkCenterCode { get; private set; } = string.Empty;
    public Guid? ExecutionId { get; private set; }
    public string ExecutionNumber { get; private set; } = string.Empty;
    public string ProductionOrderNumber { get; private set; } = string.Empty;

    public static ShopFloorFieldFeedback Create(
        string topic,
        string note,
        string screen,
        string userId,
        Guid? workCenterId,
        string workCenterCode,
        Guid? executionId,
        string executionNumber,
        string productionOrderNumber,
        string? plantId,
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            (topic ?? string.Empty).Trim().ToUpperInvariant(),
            (note ?? string.Empty).Trim(),
            (screen ?? string.Empty).Trim(),
            userId ?? string.Empty,
            workCenterId,
            workCenterCode ?? string.Empty,
            executionId,
            executionNumber ?? string.Empty,
            productionOrderNumber ?? string.Empty,
            companyId,
            plantId);
}

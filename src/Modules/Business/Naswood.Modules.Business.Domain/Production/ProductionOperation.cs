using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public static class ProductionOperationStatuses
{
    public const string Waiting = "WAITING";
    public const string Ready = "READY";
    public const string InProgress = "IN_PROGRESS";
    public const string Completed = "COMPLETED";
}

public static class ProductionOperationOutputTypes
{
    public const string None = "NONE";
    public const string Wip = "WIP";
    public const string FinalOutput = "FINAL_OUTPUT";

    public static string Normalize(string? value)
    {
        var v = (value ?? string.Empty).Trim().ToUpperInvariant();
        return v is None or Wip or FinalOutput ? v : None;
    }

    public static bool RequiresStockOutput(string? value)
    {
        var v = Normalize(value);
        return v is Wip or FinalOutput;
    }
}

/// <summary>Planned operation on a production order. Not shop-floor actuals.</summary>
public sealed class ProductionOperation : BusinessEntity
{
    private ProductionOperation() { }

    private ProductionOperation(
        Guid id,
        Guid productionOrderId,
        int sequence,
        Guid operationId,
        Guid workCenterId,
        Guid? expectedMaterialId,
        string outputType,
        string status,
        Guid? structuralProductionLotId,
        string companyId,
        string? plantId)
        : base(id)
    {
        ProductionOrderId = productionOrderId;
        Sequence = sequence;
        OperationId = operationId;
        WorkCenterId = workCenterId;
        ExpectedMaterialId = expectedMaterialId;
        OutputType = outputType;
        Status = status;
        StructuralProductionLotId = structuralProductionLotId;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid ProductionOrderId { get; private set; }
    public int Sequence { get; private set; }
    public Guid OperationId { get; private set; }
    public Guid WorkCenterId { get; private set; }
    public Guid? ExpectedMaterialId { get; private set; }
    public string OutputType { get; private set; } = ProductionOperationOutputTypes.None;
    public string Status { get; private set; } = ProductionOperationStatuses.Waiting;
    public Guid? StructuralProductionLotId { get; private set; }

    public static ProductionOperation Create(
        Guid productionOrderId,
        int sequence,
        Guid operationId,
        Guid workCenterId,
        Guid? expectedMaterialId,
        string outputType,
        string status,
        Guid? structuralProductionLotId,
        string? plantId,
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            productionOrderId,
            sequence,
            operationId,
            workCenterId,
            expectedMaterialId,
            ProductionOperationOutputTypes.Normalize(outputType),
            string.IsNullOrWhiteSpace(status) ? ProductionOperationStatuses.Waiting : status.Trim().ToUpperInvariant(),
            structuralProductionLotId,
            companyId,
            plantId);

    public void MarkReady()
    {
        if (Status == ProductionOperationStatuses.Completed) return;
        Status = ProductionOperationStatuses.Ready;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkInProgress()
    {
        Status = ProductionOperationStatuses.InProgress;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkCompleted()
    {
        Status = ProductionOperationStatuses.Completed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

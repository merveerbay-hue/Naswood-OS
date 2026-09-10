namespace Naswood.Modules.Business.Contracts.Production;

public sealed class UpsertProductionOperationRequestDto
{
    public int Sequence { get; init; }
    public Guid OperationId { get; init; }
    public Guid WorkCenterId { get; init; }
    public Guid? ExpectedMaterialId { get; init; }
    public string OutputType { get; init; } = "NONE";
    public Guid? StructuralProductionLotId { get; init; }
}

public sealed class StartProductionExecutionRequestDto
{
    public Guid? WorkCenterId { get; init; }
}

public sealed class ConsumeProductionExecutionRequestDto
{
    public Guid? PackageId { get; init; }
    public string Barcode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public Guid? PackageContentId { get; init; }
    public decimal? PieceCount { get; init; }
    public string IdempotencyKey { get; init; } = string.Empty;
}

public sealed class DowntimeRequestDto
{
    public string ReasonCode { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
}

public sealed class ScrapRequestDto
{
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string ReasonCode { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
}

public sealed class CompleteProductionExecutionRequestDto
{
    public Guid? OutputMaterialId { get; init; }
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public Guid? StructuralProductionLotId { get; init; }
    public IReadOnlyList<ProductionOutputLineRequestDto> Lines { get; init; } = [];
    public string IdempotencyKey { get; init; } = string.Empty;
}

public sealed class CancelProductionExecutionRequestDto
{
    public string CancelReason { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public string IdempotencyKey { get; init; } = string.Empty;
}

public sealed class ProductionOperationPlanDto
{
    public Guid Id { get; init; }
    public int Sequence { get; init; }
    public Guid OperationId { get; init; }
    public string OperationCode { get; init; } = string.Empty;
    public string OperationName { get; init; } = string.Empty;
    public Guid WorkCenterId { get; init; }
    public string WorkCenterCode { get; init; } = string.Empty;
    public string WorkCenterName { get; init; } = string.Empty;
    public Guid? ExpectedMaterialId { get; init; }
    public string ExpectedMaterialCode { get; init; } = string.Empty;
    public string OutputType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid? StructuralProductionLotId { get; init; }
    public Guid? ActiveExecutionId { get; init; }
    public bool CanStart { get; init; }
}

public sealed class ShopFloorWorkCenterCardDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int PendingCount { get; init; }
    public int RunningCount { get; init; }
    public int CompletedTodayCount { get; init; }
}

public sealed class ShopFloorQueueItemDto
{
    public Guid ProductionOrderId { get; init; }
    public string ProductionOrderNumber { get; init; } = string.Empty;
    public string ProductionOrderName { get; init; } = string.Empty;
    public Guid ProductionOperationId { get; init; }
    public int Sequence { get; init; }
    public string OperationName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid? ExecutionId { get; init; }
    public string ExecutionNumber { get; init; } = string.Empty;
    public decimal? PlannedQuantity { get; init; }
}

public sealed class ShopFloorQueueDto
{
    public Guid WorkCenterId { get; init; }
    public string WorkCenterCode { get; init; } = string.Empty;
    public string WorkCenterName { get; init; } = string.Empty;
    public IReadOnlyList<ShopFloorQueueItemDto> Pending { get; init; } = [];
    public IReadOnlyList<ShopFloorQueueItemDto> Running { get; init; } = [];
    public IReadOnlyList<ShopFloorQueueItemDto> CompletedToday { get; init; } = [];
}

public sealed class ProductionOrderProgressDto
{
    public Guid Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string PlantId { get; init; } = string.Empty;
    public int CompletedOperations { get; init; }
    public int TotalOperations { get; init; }
    public IReadOnlyList<ProductionOperationPlanDto> Operations { get; init; } = [];
}

public sealed class ProductionExecutionEventDto
{
    public string EventType { get; init; } = string.Empty;
    public DateTimeOffset OccurredAt { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string ReasonCode { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
}

public sealed class ProductionExecutionConsumptionDto
{
    public Guid Id { get; init; }
    public Guid SourcePackageId { get; init; }
    public string Barcode { get; init; } = string.Empty;
    public string PackageNumber { get; init; } = string.Empty;
    public string MaterialCode { get; init; } = string.Empty;
    public string LotNumber { get; init; } = string.Empty;
    public Guid SourceLotId { get; init; }
    public string PhysicalMeasure { get; init; } = string.Empty;
    public decimal ConsumedQuantity { get; init; }
    public decimal RemainingPackageQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public bool Posted { get; init; } = true;
}

public sealed class ProductionExecutionScrapDto
{
    public Guid Id { get; init; }
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string ReasonCode { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public DateTimeOffset RecordedAt { get; init; }
}

public sealed class ProductionExecutionScanDto
{
    public Guid PackageId { get; init; }
    public string PackageNo { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public string MaterialCode { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public Guid? SourceLotId { get; init; }
    public string SourceLotNumber { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public decimal AvailableQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? InactiveReason { get; init; }
    public bool CanConsume { get; init; }
    public Guid? ExistingConsumptionId { get; init; }
    public IReadOnlyList<ProductionExecutionContentScanDto> Contents { get; init; } = [];
}

public sealed class ProductionExecutionContentScanDto
{
    public Guid Id { get; init; }
    public string Measurement { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal? PieceCount { get; init; }
    public string Unit { get; init; } = string.Empty;
}

public sealed class ProductionExecutionPassportDto
{
    public Guid Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid ProductionOrderId { get; init; }
    public string ProductionOrderNumber { get; init; } = string.Empty;
    public string ProductionOrderName { get; init; } = string.Empty;
    public Guid ProductionOperationId { get; init; }
    public int Sequence { get; init; }
    public string OperationCode { get; init; } = string.Empty;
    public string OperationName { get; init; } = string.Empty;
    public Guid WorkCenterId { get; init; }
    public string WorkCenterCode { get; init; } = string.Empty;
    public string WorkCenterName { get; init; } = string.Empty;
    public string PlantId { get; init; } = string.Empty;
    public string StartedByUserId { get; init; } = string.Empty;
    public string CompletedByUserId { get; init; } = string.Empty;
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public decimal TotalRunMinutes { get; init; }
    public decimal TotalPauseMinutes { get; init; }
    public decimal TotalDowntimeMinutes { get; init; }
    public decimal InputQuantity { get; init; }
    public decimal OutputQuantity { get; init; }
    public decimal ScrapQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string OutputType { get; init; } = string.Empty;
    public Guid? StructuralProductionLotId { get; init; }
    public string StructuralProductionLotNumber { get; init; } = string.Empty;
    public Guid? ProductionOutputId { get; init; }
    public string ProductionOutputNumber { get; init; } = string.Empty;
    public Guid? ProductionLotId { get; init; }
    public string ProductionLotNumber { get; init; } = string.Empty;
    public string QcStatus { get; init; } = string.Empty;
    public Guid? ExpectedMaterialId { get; init; }
    public bool IdempotentReplay { get; init; }
    public IReadOnlyList<string> AllowedActions { get; init; } = [];
    public IReadOnlyList<ProductionExecutionEventDto> Events { get; init; } = [];
    public IReadOnlyList<ProductionExecutionConsumptionDto> Inputs { get; init; } = [];
    public IReadOnlyList<ProductionExecutionScrapDto> Scraps { get; init; } = [];
    public IReadOnlyList<ProductionOutputPackageCreatedDto> Packages { get; init; } = [];
    public IReadOnlyList<ProductionLotSourceDto> SourceLots { get; init; } = [];
}

public sealed class ShopFloorFeedbackRequestDto
{
    public string Topic { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public string Screen { get; init; } = string.Empty;
    public Guid? WorkCenterId { get; init; }
    public string WorkCenterCode { get; init; } = string.Empty;
    public Guid? ExecutionId { get; init; }
    public string ExecutionNumber { get; init; } = string.Empty;
    public string ProductionOrderNumber { get; init; } = string.Empty;
    /// <summary>Operator choice: BLOCKING or CAN_CONTINUE. Not a technical severity.</summary>
    public string Impact { get; init; } = "CAN_CONTINUE";
}

public sealed class ShopFloorFeedbackDto
{
    public Guid Id { get; init; }
    public string Topic { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public string Screen { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public Guid? WorkCenterId { get; init; }
    public string WorkCenterCode { get; init; } = string.Empty;
    public Guid? ExecutionId { get; init; }
    public string ExecutionNumber { get; init; } = string.Empty;
    public string ProductionOrderNumber { get; init; } = string.Empty;
    public string PlantId { get; init; } = string.Empty;
    public string Impact { get; init; } = string.Empty;
    public string AppVersion { get; init; } = string.Empty;
    public string GitSha { get; init; } = string.Empty;
    public DateTimeOffset OccurredAt { get; init; }
}

namespace Naswood.Modules.Business.Contracts.Production;

public sealed class ProductionOutputLineRequestDto
{
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public decimal? ThicknessMm { get; init; }
    public decimal? WidthMm { get; init; }
    public decimal? LengthMm { get; init; }
    public decimal? PieceCount { get; init; }
    public decimal? MeasuredVolumeM3 { get; init; }
}

public sealed class ProductionOutputSourceRequestDto
{
    public Guid SourceLotId { get; init; }
    public string SourceWarehouseCode { get; init; } = string.Empty;
    public string SourceLocationCode { get; init; } = string.Empty;
    public decimal ConsumedQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public Guid? SourcePackageId { get; init; }
}

public class PreviewProductionOutputRequestDto
{
    public Guid ProductionOrderId { get; init; }
    public Guid OutputMaterialId { get; init; }
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string WorkCenterCode { get; init; } = string.Empty;
    public string StockStatus { get; init; } = "Available";
    public bool AllowQcOverride { get; init; }
    public string? PlantId { get; init; }
    public IReadOnlyList<ProductionOutputLineRequestDto> Lines { get; init; } = [];
    public IReadOnlyList<ProductionOutputSourceRequestDto> Sources { get; init; } = [];
}

public sealed class PostProductionOutputRequestDto : PreviewProductionOutputRequestDto
{
    public string Number { get; init; } = string.Empty;
    public Guid? ProductionOperationExecutionId { get; init; }
}

public sealed class ProductionOutputPreviewPackageDto
{
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public int MeasurementCount { get; init; }
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal? PieceCount { get; init; }
    public IReadOnlyList<string> Measurements { get; init; } = [];
}

public sealed class ProductionOutputPreviewDto
{
    public string ProductionOrderNumber { get; init; } = string.Empty;
    public string OutputMaterialCode { get; init; } = string.Empty;
    public string OutputMaterialName { get; init; } = string.Empty;
    public string DestinationWarehouse { get; init; } = string.Empty;
    public string DestinationLocation { get; init; } = string.Empty;
    public string WorkCenterCode { get; init; } = string.Empty;
    public string LotHint { get; init; } = "Otomatik oluşturulacak";
    public int PackageCount { get; init; }
    public decimal OutputQuantity { get; init; }
    public decimal InputQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public int SourceLotCount { get; set; }
    public string ResolvedStockStatus { get; set; } = "Available";
    public bool QcHoldRequired { get; set; }
    public string QcPolicySource { get; set; } = "request";
    public bool QcOverrideRequired { get; set; }
    public IReadOnlyList<ProductionOutputPreviewPackageDto> Packages { get; init; } = [];
    public IReadOnlyList<ProductionLotSourceDto> Sources { get; set; } = [];
}

public sealed class ProductionOutputPackageCreatedDto
{
    public Guid PackageId { get; init; }
    public string PackageNo { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public string PublicId { get; init; } = string.Empty;
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
}

public sealed record ProductionOutputResultDto
{
    public Guid OutputId { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid ProductionLotId { get; init; }
    public string ProductionLotNumber { get; init; } = string.Empty;
    public string SourceType { get; init; } = string.Empty;
    public string OutputMaterialCode { get; init; } = string.Empty;
    public decimal OutputQuantity { get; init; }
    public decimal InputQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public int PackageCount { get; init; }
    public int SourceLotCount { get; init; }
    public bool IdempotentReplay { get; init; }
    public IReadOnlyList<ProductionOutputPackageCreatedDto> Packages { get; init; } = [];
    public IReadOnlyList<string> SourceLotNumbers { get; init; } = [];
    public string StockStatus { get; init; } = "Available";
    public bool Reversed { get; init; }
    public string CancelReason { get; init; } = string.Empty;
    public string QcDecision { get; init; } = string.Empty;
    public string QcDecidedBy { get; init; } = string.Empty;
    public DateTimeOffset? QcDecidedAt { get; init; }
    public string QcInspectionReference { get; init; } = string.Empty;
    public string QcNotes { get; init; } = string.Empty;
}

public sealed class ProductionOutputQcRequestDto
{
    public string Decision { get; init; } = string.Empty;
    public string InspectionReference { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class ReverseProductionOutputRequestDto
{
    public string Reason { get; init; } = string.Empty;
}

public sealed class ProductionConsumptionScanDto
{
    public Guid PackageId { get; init; }
    public string PackageNo { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public Guid SourceLotId { get; init; }
    public string SourceLotNumber { get; init; } = string.Empty;
    public string MaterialCode { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public decimal AvailableQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string PlantId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

public sealed class ProductionLotPassportDto
{
    public Guid ProductionLotId { get; init; }
    public string LotNumber { get; init; } = string.Empty;
    public string SourceType { get; init; } = string.Empty;
    public string OutputMaterialCode { get; init; } = string.Empty;
    public string OutputMaterialName { get; init; } = string.Empty;
    public Guid? ProductionOrderId { get; init; }
    public string ProductionOrderNumber { get; init; } = string.Empty;
    public string PlantId { get; init; } = string.Empty;
    public string WorkCenterCode { get; init; } = string.Empty;
    public decimal OutputQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public IReadOnlyList<ProductionOutputPackageCreatedDto> Packages { get; init; } = [];
    public IReadOnlyList<ProductionLotSourceDto> SourceLots { get; init; } = [];
    public Guid? ProductionOperationExecutionId { get; init; }
    public string ProductionExecutionNumber { get; init; } = string.Empty;
    public string OperationName { get; init; } = string.Empty;
}

public sealed class ProductionLotSourceDto
{
    public Guid SourceLotId { get; init; }
    public string SourceLotNumber { get; init; } = string.Empty;
    public string SourceMaterialCode { get; init; } = string.Empty;
    public decimal ConsumedQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public Guid? SourcePackageId { get; init; }
}

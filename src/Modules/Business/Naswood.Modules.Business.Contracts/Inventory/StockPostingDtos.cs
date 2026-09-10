namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class StockPostLineRequestDto
{
    public string MaterialCode { get; init; } = string.Empty;
    /// <summary>Optional material master Id — must match MaterialCode when provided.</summary>
    public string? MaterialId { get; init; }
    /// <summary>Optional per-line warehouse; falls back to receipt WarehouseCode.</summary>
    public string? WarehouseCode { get; init; }
    public string LocationCode { get; init; } = string.Empty;
    public string LotNumber { get; init; } = string.Empty;
    public string PackageNumber { get; init; } = string.Empty;
    public string MaterialIdentityNumber { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = "Piece";
    public string Barcode { get; init; } = string.Empty;
    /// <summary>Available (default) or Quarantine — conditional accept uses Quarantine.</summary>
    public string? StockStatus { get; init; }
    /// <summary>Actual physical thickness (mm) — not material nominal.</summary>
    public decimal? ActualThicknessMm { get; init; }
    public decimal? ActualWidthMm { get; init; }
    public decimal? ActualLengthMm { get; init; }
}

public sealed class ExecuteGoodsReceiptRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    /// <summary>Operator must confirm counted quantity before ledger post.</summary>
    public bool QuantityVerified { get; init; }
    /// <summary>
    /// Document extract provenance. <c>demo</c> is UI-only and must never post to stock.
    /// Allowed for post: <c>ocr</c>, <c>manual</c>, or empty when lines are operator-confirmed masters.
    /// </summary>
    public string ExtractSource { get; init; } = string.Empty;
    /// <summary>Factory / Plant. Defaults to session Ana Üs when omitted.</summary>
    public string? PlantId { get; init; }
    public IReadOnlyList<StockPostLineRequestDto> Lines { get; init; } = Array.Empty<StockPostLineRequestDto>();
}

public sealed class ExecuteGoodsIssueRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    /// <summary>Factory / Plant. Defaults to session Ana Üs when omitted.</summary>
    public string? PlantId { get; init; }
    public IReadOnlyList<StockPostLineRequestDto> Lines { get; init; } = Array.Empty<StockPostLineRequestDto>();
}

public sealed class StockPostLineResultDto
{
    public required string MaterialCode { get; init; }
    public required string MaterialIdentityNumber { get; init; }
    public required string PackageNumber { get; init; }
    public required string LotNumber { get; init; }
    public required string MovementNumber { get; init; }
    public required decimal Quantity { get; init; }
}

public sealed class ExecuteStockDocumentResultDto
{
    public required Guid DocumentId { get; init; }
    public required string DocumentNumber { get; init; }
    public required string Status { get; init; }
    public required IReadOnlyList<StockPostLineResultDto> Lines { get; init; }
    /// <summary>True when an existing Posted GR was returned without creating new movements.</summary>
    public bool IdempotentReplay { get; init; }
}

public sealed class InventoryPackageDto
{
    public required Guid Id { get; init; }
    public required string PackageNumber { get; init; }
    public required string MaterialIdentityNumber { get; init; }
    public required string MaterialCode { get; init; }
    public required string LotNumber { get; init; }
    public required string WarehouseCode { get; init; }
    public required string LocationCode { get; init; }
    public required decimal Quantity { get; init; }
    public required string UnitOfMeasure { get; init; }
    public required string Barcode { get; init; }
    public required string Status { get; init; }
    public string PublicId { get; init; } = string.Empty;
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public Guid? MaterialId { get; init; }
    public Guid? BatchId { get; init; }
    public Guid? WarehouseId { get; init; }
    public Guid? LocationId { get; init; }
    public string? PlantId { get; init; }
    public DateTimeOffset? LabelPrintedAt { get; init; }
    public int LabelPrintCount { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class PackageContentDto
{
    public Guid Id { get; init; }
    public int LineNo { get; init; }
    public decimal? ThicknessMm { get; init; }
    public decimal? WidthMm { get; init; }
    public decimal? LengthMm { get; init; }
    public decimal? PieceCount { get; init; }
    public decimal Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = string.Empty;
    public string Measurement { get; init; } = string.Empty;
}

public sealed class PackagePassportDto
{
    public required Guid Id { get; init; }
    public required string PackageNo { get; init; }
    public required string Barcode { get; init; }
    public required string PublicId { get; init; }
    public required string QrPath { get; init; }
    public required string MaterialCode { get; init; }
    public string MaterialName { get; init; } = string.Empty;
    public string MaterialGroup { get; init; } = string.Empty;
    public string MaterialType { get; init; } = string.Empty;
    public string WoodSpecies { get; init; } = string.Empty;
    public string Quality { get; init; } = string.Empty;
    public string StockUnit { get; init; } = string.Empty;
    public string CountUnit { get; init; } = string.Empty;
    public required string LotNumber { get; init; }
    public string SourceType { get; init; } = string.Empty;
    public string SourceReferenceNo { get; init; } = string.Empty;
    public string ProductionOrderNumber { get; init; } = string.Empty;
    public int SourceLotCount { get; init; }
    public IReadOnlyList<string> SourceLotNumbers { get; init; } = [];
    public required string Factory { get; init; }
    public required string WarehouseCode { get; init; }
    public required string LocationCode { get; init; }
    public required string Status { get; init; }
    public required decimal Quantity { get; init; }
    public required string UnitOfMeasure { get; init; }
    public decimal? TotalPieceCount { get; init; }
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? LastMovementAt { get; init; }
    public DateTimeOffset? LabelPrintedAt { get; init; }
    public int LabelPrintCount { get; init; }
    public bool PackageBalanceMismatch { get; init; }
    public IReadOnlyList<string> AllowedActions { get; init; } = [];
    public string? InactiveReason { get; init; }
    public string? LabelHint { get; init; }
    public IReadOnlyList<PackageRelationRowDto> Relations { get; init; } = [];
    public IReadOnlyList<PackageContentDto> Contents { get; init; } = [];
    public IReadOnlyList<PackageMovementRowDto> Movements { get; init; } = [];
}

public sealed class PackageMovementRowDto
{
    public DateTimeOffset At { get; init; }
    public Guid? PackageId { get; init; }
    public string Action { get; init; } = string.Empty;
    public string FromLocation { get; init; } = string.Empty;
    public string ToLocation { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Reference { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
}

public sealed class PagedInventoryPackageDto
{
    public required IReadOnlyList<InventoryPackageDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

public sealed class MaterialIdentityDto
{
    public required Guid Id { get; init; }
    public required string IdentityNumber { get; init; }
    public required string MaterialCode { get; init; }
    public required string LotNumber { get; init; }
    public required string WarehouseCode { get; init; }
    public required string LocationCode { get; init; }
    public required decimal Quantity { get; init; }
    public required string UnitOfMeasure { get; init; }
    public required string Status { get; init; }
    public required string RootGoodsReceiptNumber { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class PagedMaterialIdentityDto
{
    public required IReadOnlyList<MaterialIdentityDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

public sealed class InventoryMovementDto
{
    public required Guid Id { get; init; }
    public required string MovementNumber { get; init; }
    public required string MovementType { get; init; }
    public required string Direction { get; init; }
    public required string DocumentNumber { get; init; }
    public required string MaterialCode { get; init; }
    public required string LotNumber { get; init; }
    public required string PackageNumber { get; init; }
    public Guid? PackageId { get; init; }
    public required decimal Quantity { get; init; }
    public required string WarehouseCode { get; init; }
    public string? LocationCode { get; init; }
    public required string Status { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class PagedInventoryMovementDto
{
    public required IReadOnlyList<InventoryMovementDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

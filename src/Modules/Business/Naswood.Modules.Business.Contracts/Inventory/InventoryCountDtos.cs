namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class InventoryCountDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string WarehouseCode { get; init; }
    public string? LocationCode { get; init; }
    public required string CountType { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset? SnapshotAt { get; init; }
    public string? StartedBy { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public string? CountedBy { get; init; }
    public string? CompletedBy { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? ApprovedBy { get; init; }
    public DateTimeOffset? ApprovedAt { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public bool MidCountMovement { get; init; }
    public int MidCountMovementCount { get; init; }
    public IReadOnlyList<InventoryCountLineDto>? Lines { get; init; }
    public InventoryCountSummaryDto? Summary { get; init; }
}

public sealed class InventoryCountLineDto
{
    public required Guid Id { get; init; }
    public required int LineNo { get; init; }
    public required string Role { get; init; }
    public required string Source { get; init; }
    public Guid? MaterialId { get; init; }
    public required string MaterialCode { get; init; }
    public required string MaterialName { get; init; }
    public required string LocationCode { get; init; }
    public required string BatchNumber { get; init; }
    public required bool LotUnknown { get; init; }
    public string? PackageNumber { get; init; }
    public decimal? ThicknessMm { get; init; }
    public decimal? WidthMm { get; init; }
    public decimal? LengthMm { get; init; }
    public decimal? PieceCount { get; init; }
    public decimal? MeasuredVolumeM3 { get; init; }
    public required decimal SystemQuantityAtStart { get; init; }
    public required decimal CountedQuantity { get; init; }
    public required decimal Difference { get; init; }
    public required string StockUnit { get; init; }
    public required string CountUnit { get; init; }
    public required decimal CalculatedStockQty { get; init; }
    public required string LineStatus { get; init; }
    public required bool Duplicate { get; init; }
    public required bool KeepSeparate { get; init; }
    public required bool Approved { get; init; }
    public string? Notes { get; init; }
    public bool NominalDiffers { get; init; }
    public string? NominalMeasurement { get; init; }
}

public sealed class InventoryCountSummaryDto
{
    public required int TotalLines { get; init; }
    public required int Matched { get; init; }
    public required int Variance { get; init; }
    public required int Unexpected { get; init; }
    public required int Missing { get; init; }
    public required int Unmatched { get; init; }
    public required int Duplicates { get; init; }
    public required bool MidCountMovement { get; init; }
}

public sealed class UpsertInventoryCountRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string CountType { get; init; } = "Normal";
}

public sealed class UpsertInventoryCountLineRequestDto
{
    public string MaterialCode { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string BatchNumber { get; init; } = string.Empty;
    public bool LotUnknown { get; init; }
    public string PackageNumber { get; init; } = string.Empty;
    public decimal? ThicknessMm { get; init; }
    public decimal? WidthMm { get; init; }
    public decimal? LengthMm { get; init; }
    public decimal? PieceCount { get; init; }
    public decimal? MeasuredVolumeM3 { get; init; }
    public string Source { get; init; } = "MANUAL";
    public bool KeepSeparate { get; init; }
    public bool Approved { get; init; }
    public string Notes { get; init; } = string.Empty;
}

public sealed class ReplaceInventoryCountLinesRequestDto
{
    public IReadOnlyList<UpsertInventoryCountLineRequestDto> Lines { get; init; } = [];
}

public sealed class PostInventoryCountRequestDto
{
    public string Reason { get; init; } = "Stok sayım düzeltmesi";
    public bool ApproveAllVariances { get; init; } = true;
}

public sealed class InventoryCountPostResultDto
{
    public required Guid CountId { get; init; }
    public required string CountNumber { get; init; }
    public required string Status { get; init; }
    public required int AdjustmentCount { get; init; }
    public required IReadOnlyList<InventoryCountAdjustmentDto> Adjustments { get; init; }
}

public sealed class InventoryCountAdjustmentDto
{
    public required string MaterialCode { get; init; }
    public required string LocationCode { get; init; }
    public required string LotNumber { get; init; }
    public required decimal OldQuantity { get; init; }
    public required decimal CountedQuantity { get; init; }
    public required decimal Difference { get; init; }
    public required string Unit { get; init; }
    public required string Direction { get; init; }
    public required string MovementNumber { get; init; }
}

public sealed class PagedInventoryCountDto
{
    public required IReadOnlyList<InventoryCountDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

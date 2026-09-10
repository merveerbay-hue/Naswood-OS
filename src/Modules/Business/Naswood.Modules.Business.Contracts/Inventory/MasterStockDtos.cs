namespace Naswood.Modules.Business.Contracts.Inventory;

/// <summary>
/// Read-only master stock row. Quantity is always InventoryBalance.QuantityOnHand
/// (never summed from packages).
/// </summary>
public sealed class MasterStockRowDto
{
    public required Guid BalanceId { get; init; }
    public required string PlantId { get; init; }
    public required string MaterialCode { get; init; }
    public string MaterialName { get; init; } = string.Empty;
    public string WoodSpecies { get; init; } = string.Empty;
    public string ActualMeasurement { get; init; } = string.Empty;
    public decimal? ActualThicknessMm { get; init; }
    public decimal? ActualWidthMm { get; init; }
    public decimal? ActualLengthMm { get; init; }
    public required string Lot { get; init; }
    public required string Factory { get; init; }
    public required string WarehouseCode { get; init; }
    public required string Warehouse { get; init; }
    public required string LocationCode { get; init; }
    public required string Location { get; init; }
    public decimal? PieceCount { get; init; }
    public int PackageCount { get; init; }
    public required string StockUnit { get; init; }
    public required decimal StockQuantity { get; init; }
    public decimal QuantityReserved { get; init; }
    public decimal QuantityAvailable { get; init; }
    public required string StockStatus { get; init; }
    /// <summary>True when packages exist and their qty sum differs from the balance. Balance is not rewritten.</summary>
    public bool PackageBalanceMismatch { get; init; }
    public decimal? PackageQuantitySum { get; init; }
}

public sealed class MasterStockPackageRowDto
{
    public required Guid Id { get; init; }
    public required Guid? BalanceId { get; init; }
    public required string PackageNo { get; init; }
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public required string MaterialCode { get; init; }
    public string MaterialName { get; init; } = string.Empty;
    public string ActualMeasurement { get; init; } = string.Empty;
    public decimal? ActualThicknessMm { get; init; }
    public decimal? ActualWidthMm { get; init; }
    public decimal? ActualLengthMm { get; init; }
    public required string Lot { get; init; }
    public required string Factory { get; init; }
    public required string WarehouseCode { get; init; }
    public required string Warehouse { get; init; }
    public required string LocationCode { get; init; }
    public required string Location { get; init; }
    public decimal? PieceCount { get; init; }
    public required string StockUnit { get; init; }
    public required decimal StockQuantity { get; init; }
    public required string Status { get; init; }
    public string Barcode { get; init; } = string.Empty;
    public string MaterialIdentityNumber { get; init; } = string.Empty;
}

public sealed class MasterStockUnitTotalDto
{
    public required string Unit { get; init; }
    public required decimal Quantity { get; init; }
}

public sealed class MasterStockTotalsDto
{
    public int MaterialRowCount { get; init; }
    public int PackageCount { get; init; }
    public IReadOnlyList<MasterStockUnitTotalDto> ByUnit { get; init; } = Array.Empty<MasterStockUnitTotalDto>();
}

public sealed class PagedMasterStockDto
{
    public required IReadOnlyList<MasterStockRowDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
    public required MasterStockTotalsDto Totals { get; init; }
    public required DateTimeOffset ExportGeneratedAt { get; init; }
    public DateTimeOffset? AsOfDate { get; init; }
}

public sealed class PagedMasterStockPackageDto
{
    public required IReadOnlyList<MasterStockPackageRowDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

public sealed class MasterStockReportInfoDto
{
    public required DateTimeOffset GeneratedAt { get; init; }
    public required string ReportDate { get; init; }
    public required string ReportTime { get; init; }
    public required string TimeZone { get; init; }
    public required string Factory { get; init; }
    public string WarehouseFilter { get; init; } = "Tümü";
    public string LocationFilter { get; init; } = "Tümü";
    public string StockStatusFilter { get; init; } = "Tümü";
    public string MaterialFilter { get; init; } = "Tümü";
    public string LotFilter { get; init; } = "Tümü";
    public required string PreparedBy { get; init; }
    public int TotalStockRows { get; init; }
    public int TotalPackages { get; init; }
    public string ScopeNote { get; init; } = "Güncel stok (InventoryBalance). Geçmiş tarih (as-of) hesaplanmadı.";
}

public sealed class MasterStockExportDto
{
    public required string FileName { get; init; }
    public required DateTimeOffset GeneratedAt { get; init; }
    public required IReadOnlyList<MasterStockRowDto> StockRows { get; init; }
    public required IReadOnlyList<MasterStockPackageRowDto> PackageRows { get; init; }
    public required MasterStockReportInfoDto Report { get; init; }
    public required MasterStockTotalsDto Totals { get; init; }
}

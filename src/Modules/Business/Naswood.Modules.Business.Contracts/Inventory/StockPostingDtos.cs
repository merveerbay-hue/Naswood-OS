namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class StockPostLineRequestDto
{
    public string MaterialCode { get; init; } = string.Empty;
    /// <summary>Optional material master Id — must match MaterialCode when provided.</summary>
    public string? MaterialId { get; init; }
    public string LocationCode { get; init; } = string.Empty;
    public string LotNumber { get; init; } = string.Empty;
    public string PackageNumber { get; init; } = string.Empty;
    public string MaterialIdentityNumber { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = "Piece";
    public string Barcode { get; init; } = string.Empty;
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
    public IReadOnlyList<StockPostLineRequestDto> Lines { get; init; } = Array.Empty<StockPostLineRequestDto>();
}

public sealed class ExecuteGoodsIssueRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
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
    public required DateTimeOffset CreatedAt { get; init; }
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
    public required string PackageNumber { get; init; }
    public required decimal Quantity { get; init; }
    public required string WarehouseCode { get; init; }
    public required string Status { get; init; }
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

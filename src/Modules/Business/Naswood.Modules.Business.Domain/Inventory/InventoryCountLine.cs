using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class InventoryCountLine : BusinessEntity
{
    private InventoryCountLine() { }

    private InventoryCountLine(
        Guid id,
        Guid countId,
        int lineNo,
        string role,
        string source,
        Guid? materialId,
        string materialCode,
        string materialName,
        string locationCode,
        string batchNumber,
        bool lotUnknown,
        string packageNumber,
        decimal? thicknessMm,
        decimal? widthMm,
        decimal? lengthMm,
        decimal? pieceCount,
        decimal? measuredVolumeM3,
        decimal systemQuantityAtStart,
        decimal countedQuantity,
        string stockUnit,
        string countUnit,
        decimal calculatedStockQty,
        bool keepSeparate,
        bool approved,
        string notes,
        string companyId,
        string? plantId)
        : base(id)
    {
        CountId = countId;
        LineNo = lineNo;
        Role = role;
        Source = source;
        MaterialId = materialId;
        MaterialCode = materialCode;
        MaterialName = materialName;
        LocationCode = locationCode;
        BatchNumber = batchNumber;
        LotUnknown = lotUnknown;
        PackageNumber = packageNumber;
        ThicknessMm = thicknessMm;
        WidthMm = widthMm;
        LengthMm = lengthMm;
        PieceCount = pieceCount;
        MeasuredVolumeM3 = measuredVolumeM3;
        SystemQuantityAtStart = systemQuantityAtStart;
        CountedQuantity = countedQuantity;
        StockUnit = stockUnit;
        CountUnit = countUnit;
        CalculatedStockQty = calculatedStockQty;
        KeepSeparate = keepSeparate;
        Approved = approved;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid CountId { get; private set; }
    public int LineNo { get; private set; }
    public string Role { get; private set; } = "SNAPSHOT";
    public string Source { get; private set; } = "SYSTEM";
    public Guid? MaterialId { get; private set; }
    public string MaterialCode { get; private set; } = string.Empty;
    public string MaterialName { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string BatchNumber { get; private set; } = string.Empty;
    public bool LotUnknown { get; private set; }
    public string PackageNumber { get; private set; } = string.Empty;
    public decimal? ThicknessMm { get; private set; }
    public decimal? WidthMm { get; private set; }
    public decimal? LengthMm { get; private set; }
    public decimal? PieceCount { get; private set; }
    public decimal? MeasuredVolumeM3 { get; private set; }
    public decimal SystemQuantityAtStart { get; private set; }
    public decimal CountedQuantity { get; private set; }
    public string StockUnit { get; private set; } = string.Empty;
    public string CountUnit { get; private set; } = string.Empty;
    public decimal CalculatedStockQty { get; private set; }
    public bool KeepSeparate { get; private set; }
    public bool Approved { get; private set; }
    public string Notes { get; private set; } = string.Empty;

    public static InventoryCountLine CreateSnapshot(
        Guid countId,
        int lineNo,
        Guid? materialId,
        string materialCode,
        string materialName,
        string locationCode,
        string batchNumber,
        decimal systemQuantity,
        string stockUnit,
        string countUnit,
        string? plantId)
    {
        return new InventoryCountLine(
            UuidV7.NewGuid(),
            countId,
            lineNo,
            "SNAPSHOT",
            "SYSTEM",
            materialId,
            materialCode,
            materialName,
            locationCode,
            batchNumber,
            lotUnknown: string.IsNullOrWhiteSpace(batchNumber),
            packageNumber: string.Empty,
            thicknessMm: null,
            widthMm: null,
            lengthMm: null,
            pieceCount: null,
            measuredVolumeM3: null,
            systemQuantity,
            countedQuantity: 0,
            stockUnit,
            countUnit,
            calculatedStockQty: 0,
            keepSeparate: false,
            approved: false,
            notes: string.Empty,
            "COMP-001",
            plantId);
    }

    public static InventoryCountLine CreatePhysical(
        Guid countId,
        int lineNo,
        string source,
        Guid materialId,
        string materialCode,
        string materialName,
        string locationCode,
        string batchNumber,
        bool lotUnknown,
        string packageNumber,
        decimal? thicknessMm,
        decimal? widthMm,
        decimal? lengthMm,
        decimal? pieceCount,
        decimal? measuredVolumeM3,
        decimal countedStockQty,
        string stockUnit,
        string countUnit,
        bool keepSeparate,
        string notes,
        string? plantId)
    {
        return new InventoryCountLine(
            UuidV7.NewGuid(),
            countId,
            lineNo,
            "PHYSICAL",
            source,
            materialId,
            materialCode,
            materialName,
            locationCode,
            lotUnknown ? InventoryCountLots.Unknown : batchNumber,
            lotUnknown,
            packageNumber ?? string.Empty,
            thicknessMm,
            widthMm,
            lengthMm,
            pieceCount,
            measuredVolumeM3,
            systemQuantityAtStart: 0,
            countedStockQty,
            stockUnit,
            countUnit,
            countedStockQty,
            keepSeparate,
            approved: false,
            notes ?? string.Empty,
            "COMP-001",
            plantId);
    }

    public void SetLineNo(int lineNo) => LineNo = lineNo;

    public void ApplyPhysicalCount(decimal countedStockQty, decimal? pieceCount, decimal? measuredVolumeM3)
    {
        CountedQuantity = countedStockQty;
        CalculatedStockQty = countedStockQty;
        if (pieceCount is not null) PieceCount = pieceCount;
        if (measuredVolumeM3 is not null) MeasuredVolumeM3 = measuredVolumeM3;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetKeepSeparate(bool value)
    {
        KeepSeparate = value;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetApproved(bool value)
    {
        Approved = value;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

public static class InventoryCountLots
{
    public const string Unknown = "LOT-UNKNOWN";
}

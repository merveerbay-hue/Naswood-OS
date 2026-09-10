using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

/// <summary>Physical measurement row inside one package. Not a stock balance.</summary>
public sealed class InventoryPackageContent : BusinessEntity
{
    private InventoryPackageContent() { }

    private InventoryPackageContent(
        Guid id,
        Guid packageId,
        int lineNo,
        decimal? thicknessMm,
        decimal? widthMm,
        decimal? lengthMm,
        decimal? pieceCount,
        decimal quantity,
        string unitOfMeasure,
        string companyId,
        string? plantId)
        : base(id)
    {
        PackageId = packageId;
        LineNo = lineNo;
        ThicknessMm = thicknessMm;
        WidthMm = widthMm;
        LengthMm = lengthMm;
        PieceCount = pieceCount;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid PackageId { get; private set; }
    public int LineNo { get; private set; }
    public decimal? ThicknessMm { get; private set; }
    public decimal? WidthMm { get; private set; }
    public decimal? LengthMm { get; private set; }
    public decimal? PieceCount { get; private set; }
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;

    public static InventoryPackageContent Create(
        Guid packageId,
        int lineNo,
        decimal? thicknessMm,
        decimal? widthMm,
        decimal? lengthMm,
        decimal? pieceCount,
        decimal quantity,
        string unitOfMeasure,
        string? plantId,
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            packageId,
            lineNo,
            thicknessMm,
            widthMm,
            lengthMm,
            pieceCount,
            quantity,
            unitOfMeasure ?? string.Empty,
            companyId,
            plantId);

    public void Reduce(decimal quantity, decimal? pieceCount)
    {
        if (quantity <= 0) throw new InvalidOperationException("Content quantity must be positive.");
        if (Quantity < quantity) throw new InvalidOperationException("Insufficient package content.");
        if (pieceCount is decimal pcs)
        {
            if (pcs < 0) throw new InvalidOperationException("Piece count cannot be negative.");
            if (PieceCount is decimal have)
            {
                if (have < pcs)
                    throw new InvalidOperationException("Insufficient package pieces.");
                PieceCount = have - pcs;
            }
        }
        Quantity -= quantity;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool SameMeasurement(decimal? t, decimal? w, decimal? l)
        => ThicknessMm == t && WidthMm == w && LengthMm == l;
}

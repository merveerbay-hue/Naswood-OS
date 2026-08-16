using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class MaterialIdentity : BusinessEntity
{
    private MaterialIdentity() { }

    private MaterialIdentity(
        Guid id,
        string identityNumber,
        string materialCode,
        string lotNumber,
        string warehouseCode,
        string locationCode,
        decimal quantity,
        string unitOfMeasure,
        string status,
        string rootGoodsReceiptNumber,
        string companyId,
        string? plantId)
        : base(id)
    {
        IdentityNumber = identityNumber;
        MaterialCode = materialCode;
        LotNumber = lotNumber;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        Status = status;
        RootGoodsReceiptNumber = rootGoodsReceiptNumber;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string IdentityNumber { get; private set; } = string.Empty;
    public string MaterialCode { get; private set; } = string.Empty;
    public string LotNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string RootGoodsReceiptNumber { get; private set; } = string.Empty;

    public static MaterialIdentity CreateRoot(
        string identityNumber,
        string materialCode,
        string lotNumber,
        string warehouseCode,
        string locationCode,
        decimal quantity,
        string unitOfMeasure,
        string goodsReceiptNumber,
        string status = "Active",
        string companyId = "COMP-001",
        string? plantId = "PLANT-001")
    {
        var normalized = string.IsNullOrWhiteSpace(status) ? "Active" : status.Trim();
        return new MaterialIdentity(
            UuidV7.NewGuid(),
            identityNumber,
            materialCode,
            lotNumber,
            warehouseCode,
            locationCode,
            quantity,
            unitOfMeasure,
            normalized,
            goodsReceiptNumber,
            companyId,
            plantId);
    }

    public void Reduce(decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        if (Quantity < quantity) throw new InvalidOperationException("Insufficient material identity quantity.");
        Quantity -= quantity;
        if (Quantity == 0) Status = "Consumed";
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

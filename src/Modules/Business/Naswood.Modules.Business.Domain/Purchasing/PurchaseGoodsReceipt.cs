using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Purchasing;

public sealed class PurchaseGoodsReceipt : BusinessEntity
{
    private PurchaseGoodsReceipt() { }

    private PurchaseGoodsReceipt(Guid id, string number, string purchaseOrderNumber, string warehouseCode, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        PurchaseOrderNumber = purchaseOrderNumber;
        WarehouseCode = warehouseCode;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string PurchaseOrderNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static PurchaseGoodsReceipt Create(string number, string purchaseOrderNumber, string warehouseCode, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new PurchaseGoodsReceipt(UuidV7.NewGuid(), number, purchaseOrderNumber, warehouseCode, status, notes, companyId, plantId);
    }

    public void Update(string number, string purchaseOrderNumber, string warehouseCode, string status, string notes)
    {
        Number = number;
        PurchaseOrderNumber = purchaseOrderNumber;
        WarehouseCode = warehouseCode;
        Status = status;
        Notes = notes;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

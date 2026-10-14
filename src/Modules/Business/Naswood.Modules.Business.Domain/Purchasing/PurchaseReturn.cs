using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Purchasing;

public sealed class PurchaseReturn : BusinessEntity
{
    private PurchaseReturn() { }

    private PurchaseReturn(Guid id, string number, string supplierCode, string purchaseOrderNumber, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        SupplierCode = supplierCode;
        PurchaseOrderNumber = purchaseOrderNumber;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string SupplierCode { get; private set; } = string.Empty;
    public string PurchaseOrderNumber { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static PurchaseReturn Create(string number, string supplierCode, string purchaseOrderNumber, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new PurchaseReturn(UuidV7.NewGuid(), number, supplierCode, purchaseOrderNumber, status, notes, companyId, plantId);
    }

    public void Update(string number, string supplierCode, string purchaseOrderNumber, string status, string notes)
    {
        Number = number;
        SupplierCode = supplierCode;
        PurchaseOrderNumber = purchaseOrderNumber;
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

using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class GoodsReceipt : BusinessEntity
{
    private GoodsReceipt() { }

    private GoodsReceipt(Guid id, string number, string warehouseCode, string reference, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        WarehouseCode = warehouseCode;
        Reference = reference;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static GoodsReceipt Create(string number, string warehouseCode, string reference, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new GoodsReceipt(UuidV7.NewGuid(), number, warehouseCode, reference, status, notes, companyId, plantId);
    }

    public void Update(string number, string warehouseCode, string reference, string status, string notes)
    {
        Number = number;
        WarehouseCode = warehouseCode;
        Reference = reference;
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

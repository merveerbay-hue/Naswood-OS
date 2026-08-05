using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class InventoryAdjustment : BusinessEntity
{
    private InventoryAdjustment() { }

    private InventoryAdjustment(Guid id, string number, string warehouseCode, string reason, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        WarehouseCode = warehouseCode;
        Reason = reason;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static InventoryAdjustment Create(string number, string warehouseCode, string reason, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new InventoryAdjustment(UuidV7.NewGuid(), number, warehouseCode, reason, status, notes, companyId, plantId);
    }

    public void Update(string number, string warehouseCode, string reason, string status, string notes)
    {
        Number = number;
        WarehouseCode = warehouseCode;
        Reason = reason;
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

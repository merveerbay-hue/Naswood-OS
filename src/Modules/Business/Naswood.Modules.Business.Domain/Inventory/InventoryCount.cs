using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class InventoryCount : BusinessEntity
{
    private InventoryCount() { }

    private InventoryCount(Guid id, string number, string warehouseCode, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        WarehouseCode = warehouseCode;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static InventoryCount Create(string number, string warehouseCode, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new InventoryCount(UuidV7.NewGuid(), number, warehouseCode, status, notes, companyId, plantId);
    }

    public void Update(string number, string warehouseCode, string status, string notes)
    {
        Number = number;
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

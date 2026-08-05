using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class InventoryBalance : BusinessEntity
{
    private InventoryBalance() { }

    private InventoryBalance(Guid id, string materialCode, string warehouseCode, string locationCode, string batchNumber, decimal quantityOnHand, decimal quantityReserved, string status, string companyId, string? plantId)
        : base(id)
    {
        MaterialCode = materialCode;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        BatchNumber = batchNumber;
        QuantityOnHand = quantityOnHand;
        QuantityReserved = quantityReserved;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string MaterialCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string BatchNumber { get; private set; } = string.Empty;
    public decimal QuantityOnHand { get; private set; }
    public decimal QuantityReserved { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public static InventoryBalance Create(string materialCode, string warehouseCode, string locationCode, string batchNumber, decimal quantityOnHand, decimal quantityReserved, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new InventoryBalance(UuidV7.NewGuid(), materialCode, warehouseCode, locationCode, batchNumber, quantityOnHand, quantityReserved, status, companyId, plantId);
    }

    public void Update(string materialCode, string warehouseCode, string locationCode, string batchNumber, decimal quantityOnHand, decimal quantityReserved, string status)
    {
        MaterialCode = materialCode;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        BatchNumber = batchNumber;
        QuantityOnHand = quantityOnHand;
        QuantityReserved = quantityReserved;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

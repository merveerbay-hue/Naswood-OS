using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class InventoryMovement : BusinessEntity
{
    private InventoryMovement() { }

    private InventoryMovement(
        Guid id,
        string movementNumber,
        string movementType,
        string direction,
        string documentNumber,
        string materialCode,
        string materialIdentityNumber,
        string packageNumber,
        string warehouseCode,
        string locationCode,
        string lotNumber,
        decimal quantity,
        string unitOfMeasure,
        string status,
        string notes,
        string companyId,
        string? plantId)
        : base(id)
    {
        MovementNumber = movementNumber;
        MovementType = movementType;
        Direction = direction;
        DocumentNumber = documentNumber;
        MaterialCode = materialCode;
        MaterialIdentityNumber = materialIdentityNumber;
        PackageNumber = packageNumber;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        LotNumber = lotNumber;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string MovementNumber { get; private set; } = string.Empty;
    public string MovementType { get; private set; } = string.Empty;
    public string Direction { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string MaterialCode { get; private set; } = string.Empty;
    public string MaterialIdentityNumber { get; private set; } = string.Empty;
    public string PackageNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string LotNumber { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static InventoryMovement Post(
        string movementType,
        string direction,
        string documentNumber,
        string materialCode,
        string materialIdentityNumber,
        string packageNumber,
        string warehouseCode,
        string locationCode,
        string lotNumber,
        decimal quantity,
        string unitOfMeasure,
        string notes,
        string companyId = "COMP-001",
        string? plantId = "PLANT-001")
    {
        var prefix = direction.Equals("In", StringComparison.OrdinalIgnoreCase) ? "MV-IN" : "MV-OUT";
        return new InventoryMovement(
            UuidV7.NewGuid(),
            $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            movementType,
            direction,
            documentNumber,
            materialCode,
            materialIdentityNumber,
            packageNumber,
            warehouseCode,
            locationCode,
            lotNumber,
            quantity,
            unitOfMeasure,
            "Posted",
            notes,
            companyId,
            plantId);
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

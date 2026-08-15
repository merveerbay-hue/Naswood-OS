using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class InventoryPackage : BusinessEntity
{
    private InventoryPackage() { }

    private InventoryPackage(
        Guid id,
        string packageNumber,
        string materialIdentityNumber,
        string materialCode,
        string lotNumber,
        string warehouseCode,
        string locationCode,
        decimal quantity,
        string unitOfMeasure,
        string barcode,
        string status,
        string companyId,
        string? plantId)
        : base(id)
    {
        PackageNumber = packageNumber;
        MaterialIdentityNumber = materialIdentityNumber;
        MaterialCode = materialCode;
        LotNumber = lotNumber;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        Quantity = quantity;
        UnitOfMeasure = unitOfMeasure;
        Barcode = barcode;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string PackageNumber { get; private set; } = string.Empty;
    public string MaterialIdentityNumber { get; private set; } = string.Empty;
    public string MaterialCode { get; private set; } = string.Empty;
    public string LotNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public string Barcode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static InventoryPackage Create(
        string packageNumber,
        string materialIdentityNumber,
        string materialCode,
        string lotNumber,
        string warehouseCode,
        string locationCode,
        decimal quantity,
        string unitOfMeasure,
        string? barcode = null,
        string companyId = "COMP-001",
        string? plantId = "PLANT-001")
    {
        var code = string.IsNullOrWhiteSpace(barcode) ? packageNumber : barcode.Trim();
        return new InventoryPackage(
            UuidV7.NewGuid(),
            packageNumber,
            materialIdentityNumber,
            materialCode,
            lotNumber,
            warehouseCode,
            locationCode,
            quantity,
            unitOfMeasure,
            code,
            "Available",
            companyId,
            plantId);
    }

    public void Issue(decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        if (!string.Equals(Status, "Available", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Package is not available.");
        if (Quantity < quantity) throw new InvalidOperationException("Insufficient package quantity.");
        Quantity -= quantity;
        Status = Quantity == 0 ? "Issued" : "Available";
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

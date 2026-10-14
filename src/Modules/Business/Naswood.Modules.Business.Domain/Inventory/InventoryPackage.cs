using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public static class InventoryPackageStatuses
{
    public const string Available = "Available";
    public const string Quarantine = "Quarantine";
    public const string Consumed = "Consumed";
    public const string Empty = "Empty";
    public const string Issued = "Issued";
    public const string Cancelled = "Cancelled";
    public const string Closed = "Closed";
    public const string Rejected = "Rejected";
    public const string Merged = "Merged";
    public const string Repacked = "Repacked";

    public static bool IsConsumable(string? status)
        => string.Equals(status, Available, StringComparison.OrdinalIgnoreCase);

    public static bool IsRestorable(string? status)
        => string.Equals(status, Consumed, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Empty, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Issued, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Available, StringComparison.OrdinalIgnoreCase);

    public static bool IsClosed(string? status)
        => string.Equals(status, Cancelled, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Closed, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Rejected, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Merged, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Repacked, StringComparison.OrdinalIgnoreCase);

    public static bool IsPhysicalActive(string? status)
        => string.Equals(status, Available, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, Quarantine, StringComparison.OrdinalIgnoreCase);
}

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
        string publicId,
        string physicalGroupLabel,
        string sourcePlantId,
        Guid? materialId,
        Guid? batchId,
        Guid? warehouseId,
        Guid? locationId,
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
        PublicId = publicId;
        PhysicalGroupLabel = physicalGroupLabel;
        SourcePlantId = sourcePlantId;
        CurrentPlantId = plantId ?? string.Empty;
        MaterialId = materialId;
        BatchId = batchId;
        WarehouseId = warehouseId;
        LocationId = locationId;
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
    public string PublicId { get; private set; } = string.Empty;
    public string PhysicalGroupLabel { get; private set; } = string.Empty;
    public string SourcePlantId { get; private set; } = string.Empty;
    public string CurrentPlantId { get; private set; } = string.Empty;
    public Guid? MaterialId { get; private set; }
    public Guid? BatchId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? LocationId { get; private set; }
    public DateTimeOffset? LabelPrintedAt { get; private set; }
    public int LabelPrintCount { get; private set; }

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
        string status = "Available",
        string companyId = "COMP-001",
        string? plantId = "PLANT-001",
        string? publicId = null,
        string physicalGroupLabel = "",
        string? sourcePlantId = null,
        Guid? materialId = null,
        Guid? batchId = null,
        Guid? warehouseId = null,
        Guid? locationId = null)
    {
        if (materialId is null)
            throw new InvalidOperationException("Package.MaterialId is required.");
        if (batchId is null)
            throw new InvalidOperationException("Package.BatchId is required.");
        var code = string.IsNullOrWhiteSpace(barcode) ? packageNumber : barcode.Trim();
        var normalized = string.IsNullOrWhiteSpace(status) ? "Available" : status.Trim();
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
            normalized,
            string.IsNullOrWhiteSpace(publicId) ? Guid.NewGuid().ToString("N") : publicId.Trim(),
            physicalGroupLabel ?? string.Empty,
            string.IsNullOrWhiteSpace(sourcePlantId) ? plantId ?? string.Empty : sourcePlantId.Trim(),
            materialId,
            batchId,
            warehouseId,
            locationId,
            companyId,
            plantId);
    }

    public void RecordLabelPrint()
    {
        LabelPrintCount += 1;
        LabelPrintedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Issue(decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        if (!InventoryPackageStatuses.IsConsumable(Status))
            throw new InvalidOperationException("Package is not available.");
        if (Quantity < quantity) throw new InvalidOperationException("Insufficient package quantity.");
        Quantity -= quantity;
        Status = Quantity == 0 ? InventoryPackageStatuses.Issued : InventoryPackageStatuses.Available;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Consume(decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        if (!InventoryPackageStatuses.IsConsumable(Status))
            throw new InvalidOperationException("Package is not available.");
        if (Quantity < quantity) throw new InvalidOperationException("Insufficient package quantity.");
        Quantity -= quantity;
        Status = Quantity == 0 ? InventoryPackageStatuses.Consumed : InventoryPackageStatuses.Available;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Restore(decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        if (InventoryPackageStatuses.IsClosed(Status))
            throw new InvalidOperationException("Closed package cannot be restored.");
        if (!InventoryPackageStatuses.IsRestorable(Status))
            throw new InvalidOperationException("Package cannot return to stock.");
        Quantity += quantity;
        if (Quantity > 0)
            Status = InventoryPackageStatuses.Available;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkIssued()
    {
        Status = InventoryPackageStatuses.Issued;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Reversal of an output package: same identity, not consumable, history kept.</summary>
    public void MarkCancelled()
    {
        Status = InventoryPackageStatuses.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ReducePhysical(decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        if (!InventoryPackageStatuses.IsPhysicalActive(Status))
            throw new InvalidOperationException("Package is not physically active.");
        if (Quantity < quantity) throw new InvalidOperationException("Insufficient package quantity.");
        Quantity -= quantity;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkMerged()
    {
        Status = InventoryPackageStatuses.Merged;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkRepacked()
    {
        Status = InventoryPackageStatuses.Repacked;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ApplyStockStatus(string status)
    {
        if (InventoryPackageStatuses.IsClosed(Status) && !string.Equals(status, Status, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Closed package status cannot change.");
        if (string.IsNullOrWhiteSpace(status))
            throw new InvalidOperationException("Package status is required.");
        Status = status.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Intra-factory relocate — does not change material, lot, qty, or status.</summary>
    public void Relocate(string warehouseCode, string locationCode, Guid? warehouseId = null, Guid? locationId = null)
    {
        if (string.IsNullOrWhiteSpace(warehouseCode)) throw new InvalidOperationException("Warehouse is required.");
        if (string.IsNullOrWhiteSpace(locationCode)) throw new InvalidOperationException("Location is required.");
        WarehouseCode = warehouseCode.Trim();
        LocationCode = locationCode.Trim();
        if (warehouseId is not null) WarehouseId = warehouseId;
        if (locationId is not null) LocationId = locationId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

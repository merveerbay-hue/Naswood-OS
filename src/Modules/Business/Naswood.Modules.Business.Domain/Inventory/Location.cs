using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public static class StockZoneTypes
{
    public const string Normal = "NORMAL";
    public const string Quarantine = "QUARANTINE";
    public const string Rejected = "REJECTED";
    public const string Blocked = "BLOCKED";

    public static string Resolve(string? explicitZone, string? locationType)
    {
        var z = (explicitZone ?? string.Empty).Trim().ToUpperInvariant();
        if (z is "NORMAL" or "QUARANTINE" or "REJECTED" or "BLOCKED") return z;
        var t = (locationType ?? string.Empty).Trim().ToUpperInvariant();
        if (t is "QUARANTINE" or "QUARANTINE_AREA") return Quarantine;
        return Normal;
    }

    public static bool IsQuarantineCompatible(string? zone)
        => string.Equals(zone, Quarantine, StringComparison.OrdinalIgnoreCase);

    public static bool IsNormalStock(string? zone)
        => string.Equals(zone, Normal, StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(zone);
}

public sealed class Location : BusinessEntity
{
    private Location() { }

    private Location(
        Guid id,
        string code,
        string name,
        string warehouseCode,
        string locationType,
        string status,
        string description,
        string stockZoneType,
        string companyId,
        string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        WarehouseCode = warehouseCode;
        LocationType = locationType;
        Status = status;
        Description = description;
        StockZoneType = stockZoneType;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationType { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string StockZoneType { get; private set; } = string.Empty;

    public static Location Create(
        string code,
        string name,
        string warehouseCode,
        string locationType,
        string status,
        string description = "",
        string companyId = "COMP-001",
        string? plantId = "PLANT-001",
        string stockZoneType = "")
    {
        return new Location(
            UuidV7.NewGuid(),
            code,
            name,
            warehouseCode,
            locationType,
            status,
            description ?? string.Empty,
            StockZoneTypes.Resolve(stockZoneType, locationType),
            companyId,
            plantId);
    }

    public void Update(
        string code,
        string name,
        string warehouseCode,
        string locationType,
        string status,
        string? description = null,
        string? stockZoneType = null)
    {
        Code = code;
        Name = name;
        WarehouseCode = warehouseCode;
        LocationType = locationType;
        Status = status;
        if (description is not null) Description = description;
        if (stockZoneType is not null)
            StockZoneType = StockZoneTypes.Resolve(stockZoneType, locationType);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Status = "Inactive";
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

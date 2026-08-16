using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

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

    public static Location Create(
        string code,
        string name,
        string warehouseCode,
        string locationType,
        string status,
        string description = "",
        string companyId = "COMP-001",
        string? plantId = "PLANT-001")
    {
        return new Location(
            UuidV7.NewGuid(),
            code,
            name,
            warehouseCode,
            locationType,
            status,
            description ?? string.Empty,
            companyId,
            plantId);
    }

    public void Update(
        string code,
        string name,
        string warehouseCode,
        string locationType,
        string status,
        string? description = null)
    {
        Code = code;
        Name = name;
        WarehouseCode = warehouseCode;
        LocationType = locationType;
        Status = status;
        if (description is not null) Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Status = "Inactive";
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

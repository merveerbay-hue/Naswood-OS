using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class Location : BusinessEntity
{
    private Location() { }

    private Location(Guid id, string code, string name, string warehouseCode, string locationType, string status, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        WarehouseCode = warehouseCode;
        LocationType = locationType;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationType { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static Location Create(string code, string name, string warehouseCode, string locationType, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Location(UuidV7.NewGuid(), code, name, warehouseCode, locationType, status, companyId, plantId);
    }

    public void Update(string code, string name, string warehouseCode, string locationType, string status)
    {
        Code = code;
        Name = name;
        WarehouseCode = warehouseCode;
        LocationType = locationType;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

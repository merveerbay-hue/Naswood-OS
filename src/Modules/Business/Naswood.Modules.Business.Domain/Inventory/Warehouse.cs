using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class Warehouse : BusinessEntity
{
    private Warehouse() { }

    private Warehouse(Guid id, string code, string name, string warehouseType, string status, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        WarehouseType = warehouseType;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string WarehouseType { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static Warehouse Create(string code, string name, string warehouseType, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
        => new(UuidV7.NewGuid(), code, name, warehouseType, status, companyId, plantId);

    public void Update(string code, string name, string warehouseType, string status, string? plantId = null)
    {
        Code = code;
        Name = name;
        WarehouseType = warehouseType;
        Status = status;
        if (plantId is not null) PlantId = plantId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

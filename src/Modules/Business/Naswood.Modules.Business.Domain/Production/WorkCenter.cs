using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public sealed class WorkCenter : BusinessEntity
{
    private WorkCenter() { }

    private WorkCenter(Guid id, string code, string name, decimal capacityPerHour, string status, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        CapacityPerHour = capacityPerHour;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal CapacityPerHour { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public static WorkCenter Create(string code, string name, decimal capacityPerHour, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
        => new(UuidV7.NewGuid(), code, name, capacityPerHour, status, companyId, plantId);

    public void Update(string code, string name, decimal capacityPerHour, string status, string? plantId = null)
    {
        Code = code;
        Name = name;
        CapacityPerHour = capacityPerHour;
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

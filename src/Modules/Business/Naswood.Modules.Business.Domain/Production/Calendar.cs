using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public sealed class Calendar : BusinessEntity
{
    private Calendar() { }

    private Calendar(Guid id, string code, string name, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static Calendar Create(string code, string name, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
        => new(UuidV7.NewGuid(), code, name, status, notes, companyId, plantId);

    public void Update(string code, string name, string status, string notes, string? plantId = null)
    {
        Code = code;
        Name = name;
        Status = status;
        Notes = notes;
        if (plantId is not null) PlantId = plantId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

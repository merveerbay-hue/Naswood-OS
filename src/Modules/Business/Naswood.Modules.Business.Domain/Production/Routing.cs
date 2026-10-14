using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public sealed class Routing : BusinessEntity
{
    private Routing() { }

    private Routing(Guid id, string number, string materialCode, int version, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        MaterialCode = materialCode;
        Version = version;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string MaterialCode { get; private set; } = string.Empty;
    public int Version { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static Routing Create(string number, string materialCode, int version, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Routing(UuidV7.NewGuid(), number, materialCode, version, status, notes, companyId, plantId);
    }

    public void Update(string number, string materialCode, int version, string status, string notes)
    {
        Number = number;
        MaterialCode = materialCode;
        Version = version;
        Status = status;
        Notes = notes;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

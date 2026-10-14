using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public sealed class Machine : BusinessEntity
{
    private Machine() { }

    private Machine(Guid id, string code, string name, string workCenterCode, string status, decimal oeeTarget, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        WorkCenterCode = workCenterCode;
        Status = status;
        OeeTarget = oeeTarget;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string WorkCenterCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public decimal OeeTarget { get; private set; }

    public static Machine Create(string code, string name, string workCenterCode, string status, decimal oeeTarget, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Machine(UuidV7.NewGuid(), code, name, workCenterCode, status, oeeTarget, companyId, plantId);
    }

    public void Update(string code, string name, string workCenterCode, string status, decimal oeeTarget)
    {
        Code = code;
        Name = name;
        WorkCenterCode = workCenterCode;
        Status = status;
        OeeTarget = oeeTarget;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

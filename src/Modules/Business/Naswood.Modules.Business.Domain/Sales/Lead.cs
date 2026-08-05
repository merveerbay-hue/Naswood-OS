using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Sales;

public sealed class Lead : BusinessEntity
{
    private Lead() { }

    private Lead(Guid id, string code, string name, string companyName, string email, string source, string status, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        CompanyName = companyName;
        Email = email;
        Source = source;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static Lead Create(string code, string name, string companyName, string email, string source, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Lead(UuidV7.NewGuid(), code, name, companyName, email, source, status, companyId, plantId);
    }

    public void Update(string code, string name, string companyName, string email, string source, string status)
    {
        Code = code;
        Name = name;
        CompanyName = companyName;
        Email = email;
        Source = source;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

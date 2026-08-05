using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Sales;

public sealed class Customer : BusinessEntity
{
    private Customer() { }

    private Customer(Guid id, string code, string name, string taxNumber, string email, string phone, string status, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        TaxNumber = taxNumber;
        Email = email;
        Phone = phone;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string TaxNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static Customer Create(string code, string name, string taxNumber, string email, string phone, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Customer(UuidV7.NewGuid(), code, name, taxNumber, email, phone, status, companyId, plantId);
    }

    public void Update(string code, string name, string taxNumber, string email, string phone, string status)
    {
        Code = code;
        Name = name;
        TaxNumber = taxNumber;
        Email = email;
        Phone = phone;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Sales;

public sealed class Opportunity : BusinessEntity
{
    private Opportunity() { }

    private Opportunity(Guid id, string number, string customerCode, string title, decimal amount, string stage, string status, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        CustomerCode = customerCode;
        Title = title;
        Amount = amount;
        Stage = stage;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string CustomerCode { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Stage { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static Opportunity Create(string number, string customerCode, string title, decimal amount, string stage, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Opportunity(UuidV7.NewGuid(), number, customerCode, title, amount, stage, status, companyId, plantId);
    }

    public void Update(string number, string customerCode, string title, decimal amount, string stage, string status)
    {
        Number = number;
        CustomerCode = customerCode;
        Title = title;
        Amount = amount;
        Stage = stage;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

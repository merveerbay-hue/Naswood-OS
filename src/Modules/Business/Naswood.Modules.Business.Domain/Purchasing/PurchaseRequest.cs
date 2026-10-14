using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Purchasing;

public sealed class PurchaseRequest : BusinessEntity
{
    private PurchaseRequest() { }

    private PurchaseRequest(Guid id, string number, string requester, DateOnly? neededDate, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        Requester = requester;
        NeededDate = neededDate;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string Requester { get; private set; } = string.Empty;
    public DateOnly? NeededDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static PurchaseRequest Create(string number, string requester, DateOnly? neededDate, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new PurchaseRequest(UuidV7.NewGuid(), number, requester, neededDate, status, notes, companyId, plantId);
    }

    public void Update(string number, string requester, DateOnly? neededDate, string status, string notes)
    {
        Number = number;
        Requester = requester;
        NeededDate = neededDate;
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

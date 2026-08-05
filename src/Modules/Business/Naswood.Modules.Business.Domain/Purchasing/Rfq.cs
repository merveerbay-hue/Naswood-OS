using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Purchasing;

public sealed class Rfq : BusinessEntity
{
    private Rfq() { }

    private Rfq(Guid id, string number, string title, DateOnly? dueDate, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        Title = title;
        DueDate = dueDate;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public DateOnly? DueDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static Rfq Create(string number, string title, DateOnly? dueDate, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Rfq(UuidV7.NewGuid(), number, title, dueDate, status, notes, companyId, plantId);
    }

    public void Update(string number, string title, DateOnly? dueDate, string status, string notes)
    {
        Number = number;
        Title = title;
        DueDate = dueDate;
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

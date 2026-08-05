using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Sales;

public sealed class Delivery : BusinessEntity
{
    private Delivery() { }

    private Delivery(Guid id, string number, string shipmentNumber, string customerCode, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        ShipmentNumber = shipmentNumber;
        CustomerCode = customerCode;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string ShipmentNumber { get; private set; } = string.Empty;
    public string CustomerCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static Delivery Create(string number, string shipmentNumber, string customerCode, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Delivery(UuidV7.NewGuid(), number, shipmentNumber, customerCode, status, notes, companyId, plantId);
    }

    public void Update(string number, string shipmentNumber, string customerCode, string status, string notes)
    {
        Number = number;
        ShipmentNumber = shipmentNumber;
        CustomerCode = customerCode;
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

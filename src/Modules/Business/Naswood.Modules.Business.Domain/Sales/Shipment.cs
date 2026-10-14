using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Sales;

public sealed class Shipment : BusinessEntity
{
    private Shipment() { }

    private Shipment(Guid id, string number, string salesOrderNumber, string warehouseCode, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        SalesOrderNumber = salesOrderNumber;
        WarehouseCode = warehouseCode;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string SalesOrderNumber { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static Shipment Create(string number, string salesOrderNumber, string warehouseCode, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Shipment(UuidV7.NewGuid(), number, salesOrderNumber, warehouseCode, status, notes, companyId, plantId);
    }

    public void Update(string number, string salesOrderNumber, string warehouseCode, string status, string notes)
    {
        Number = number;
        SalesOrderNumber = salesOrderNumber;
        WarehouseCode = warehouseCode;
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

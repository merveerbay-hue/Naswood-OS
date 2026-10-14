using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Purchasing;

public sealed class PurchaseOrder : BusinessEntity
{
    private PurchaseOrder() { }

    private PurchaseOrder(Guid id, string number, string supplierCode, DateOnly? orderDate, decimal totalAmount, string currency, string status, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        SupplierCode = supplierCode;
        OrderDate = orderDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string SupplierCode { get; private set; } = string.Empty;
    public DateOnly? OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static PurchaseOrder Create(string number, string supplierCode, DateOnly? orderDate, decimal totalAmount, string currency, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new PurchaseOrder(UuidV7.NewGuid(), number, supplierCode, orderDate, totalAmount, currency, status, companyId, plantId);
    }

    public void Update(string number, string supplierCode, DateOnly? orderDate, decimal totalAmount, string currency, string status)
    {
        Number = number;
        SupplierCode = supplierCode;
        OrderDate = orderDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

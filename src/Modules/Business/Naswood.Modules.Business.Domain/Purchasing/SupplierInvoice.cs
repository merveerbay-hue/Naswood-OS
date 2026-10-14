using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Purchasing;

public sealed class SupplierInvoice : BusinessEntity
{
    private SupplierInvoice() { }

    private SupplierInvoice(Guid id, string number, string supplierCode, DateOnly? invoiceDate, decimal totalAmount, string currency, string status, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        SupplierCode = supplierCode;
        InvoiceDate = invoiceDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string SupplierCode { get; private set; } = string.Empty;
    public DateOnly? InvoiceDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static SupplierInvoice Create(string number, string supplierCode, DateOnly? invoiceDate, decimal totalAmount, string currency, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new SupplierInvoice(UuidV7.NewGuid(), number, supplierCode, invoiceDate, totalAmount, currency, status, companyId, plantId);
    }

    public void Update(string number, string supplierCode, DateOnly? invoiceDate, decimal totalAmount, string currency, string status)
    {
        Number = number;
        SupplierCode = supplierCode;
        InvoiceDate = invoiceDate;
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

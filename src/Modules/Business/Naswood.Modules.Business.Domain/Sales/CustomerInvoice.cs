using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Sales;

public sealed class CustomerInvoice : BusinessEntity
{
    private CustomerInvoice() { }

    private CustomerInvoice(Guid id, string number, string customerCode, DateOnly? invoiceDate, decimal totalAmount, string currency, string status, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        CustomerCode = customerCode;
        InvoiceDate = invoiceDate;
        TotalAmount = totalAmount;
        Currency = currency;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string CustomerCode { get; private set; } = string.Empty;
    public DateOnly? InvoiceDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static CustomerInvoice Create(string number, string customerCode, DateOnly? invoiceDate, decimal totalAmount, string currency, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new CustomerInvoice(UuidV7.NewGuid(), number, customerCode, invoiceDate, totalAmount, currency, status, companyId, plantId);
    }

    public void Update(string number, string customerCode, DateOnly? invoiceDate, decimal totalAmount, string currency, string status)
    {
        Number = number;
        CustomerCode = customerCode;
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

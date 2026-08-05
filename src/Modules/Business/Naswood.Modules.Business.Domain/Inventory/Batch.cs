using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class Batch : BusinessEntity
{
    private Batch() { }

    private Batch(Guid id, string batchNumber, string materialCode, decimal quantity, DateOnly? expiryDate, string status, string companyId, string? plantId)
        : base(id)
    {
        BatchNumber = batchNumber;
        MaterialCode = materialCode;
        Quantity = quantity;
        ExpiryDate = expiryDate;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string BatchNumber { get; private set; } = string.Empty;
    public string MaterialCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public DateOnly? ExpiryDate { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public static Batch Create(string batchNumber, string materialCode, decimal quantity, DateOnly? expiryDate, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Batch(UuidV7.NewGuid(), batchNumber, materialCode, quantity, expiryDate, status, companyId, plantId);
    }

    public void Update(string batchNumber, string materialCode, decimal quantity, DateOnly? expiryDate, string status)
    {
        BatchNumber = batchNumber;
        MaterialCode = materialCode;
        Quantity = quantity;
        ExpiryDate = expiryDate;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

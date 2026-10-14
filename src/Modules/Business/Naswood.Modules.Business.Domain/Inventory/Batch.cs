using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class Batch : BusinessEntity
{
    private Batch() { }

    private Batch(Guid id, string batchNumber, string materialCode, decimal quantity, DateOnly? expiryDate, string status, string sourceType, string sourceReferenceNo, string companyId, string? plantId)
        : base(id)
    {
        BatchNumber = batchNumber;
        MaterialCode = materialCode;
        Quantity = quantity;
        ExpiryDate = expiryDate;
        Status = status;
        SourceType = sourceType;
        SourceReferenceNo = sourceReferenceNo;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string BatchNumber { get; private set; } = string.Empty;
    public string MaterialCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public DateOnly? ExpiryDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    /// <summary>OPENING_INVENTORY | GOODS_RECEIPT | PRODUCTION | empty (legacy).</summary>
    public string SourceType { get; private set; } = string.Empty;
    public string SourceReferenceNo { get; private set; } = string.Empty;

    public static Batch Create(string batchNumber, string materialCode, decimal quantity, DateOnly? expiryDate, string status, string companyId = "COMP-001", string? plantId = "PLANT-001", string sourceType = "", string sourceReferenceNo = "")
    {
        return new Batch(UuidV7.NewGuid(), batchNumber, materialCode, quantity, expiryDate, status, sourceType ?? string.Empty, sourceReferenceNo ?? string.Empty, companyId, plantId);
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

    /** Accumulate received qty onto an existing operational lot (same BatchNumber + MaterialCode). */
    public void ApplyReceipt(decimal quantity, string? status = null)
    {
        if (quantity <= 0) return;
        Quantity += quantity;
        if (!string.IsNullOrWhiteSpace(status)) Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new InvalidOperationException("Lot status is required.");
        Status = status.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ApplyIssue(decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Issue quantity must be positive.");
        if (Quantity < quantity) throw new InvalidOperationException("Insufficient lot quantity.");
        Quantity -= quantity;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

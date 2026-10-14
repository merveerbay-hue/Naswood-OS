using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class InventoryCount : BusinessEntity
{
    private InventoryCount() { }

    private InventoryCount(
        Guid id,
        string number,
        string warehouseCode,
        string locationCode,
        string countType,
        string status,
        string notes,
        string startedBy,
        string companyId,
        string? plantId)
        : base(id)
    {
        Number = number;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        CountType = countType;
        Status = status;
        Notes = notes;
        StartedBy = startedBy;
        StartedAt = DateTimeOffset.UtcNow;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string CountType { get; private set; } = "Normal";
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public DateTimeOffset? SnapshotAt { get; private set; }
    public string StartedBy { get; private set; } = string.Empty;
    public DateTimeOffset StartedAt { get; private set; }
    public string CountedBy { get; private set; } = string.Empty;
    public string CompletedBy { get; private set; } = string.Empty;
    public DateTimeOffset? CompletedAt { get; private set; }
    public string ApprovedBy { get; private set; } = string.Empty;
    public DateTimeOffset? ApprovedAt { get; private set; }

    public static InventoryCount Create(
        string number,
        string warehouseCode,
        string status,
        string notes,
        string companyId = "COMP-001",
        string? plantId = "PLANT-001",
        string locationCode = "",
        string countType = "Normal",
        string startedBy = "")
    {
        return new InventoryCount(
            UuidV7.NewGuid(),
            number,
            warehouseCode,
            locationCode ?? string.Empty,
            string.IsNullOrWhiteSpace(countType) ? "Normal" : countType.Trim(),
            string.IsNullOrWhiteSpace(status) ? InventoryCountStatuses.Counting : status.Trim(),
            notes ?? string.Empty,
            startedBy ?? string.Empty,
            companyId,
            plantId);
    }

    public void Update(string number, string warehouseCode, string status, string notes)
    {
        if (IsTerminal())
            throw new InvalidOperationException("Posted or cancelled counts cannot be edited.");
        Number = number;
        WarehouseCode = warehouseCode;
        Status = status;
        Notes = notes;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void CaptureSnapshot(DateTimeOffset at)
    {
        SnapshotAt = at;
        if (Status == InventoryCountStatuses.Draft || string.IsNullOrWhiteSpace(Status))
            Status = InventoryCountStatuses.Counting;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkCounting()
    {
        EnsureNotTerminal();
        Status = InventoryCountStatuses.Counting;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Complete(string countedBy)
    {
        EnsureNotTerminal();
        if (Status is InventoryCountStatuses.Posted)
            throw new InvalidOperationException("Posted counts cannot be completed again.");
        CountedBy = countedBy ?? string.Empty;
        CompletedBy = countedBy ?? string.Empty;
        CompletedAt = DateTimeOffset.UtcNow;
        Status = InventoryCountStatuses.Review;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Approve(string approvedBy)
    {
        EnsureNotTerminal();
        ApprovedBy = approvedBy ?? string.Empty;
        ApprovedAt = DateTimeOffset.UtcNow;
        Status = InventoryCountStatuses.Approved;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkPosted()
    {
        if (Status == InventoryCountStatuses.Cancelled)
            throw new InvalidOperationException("Cancelled counts cannot be posted.");
        Status = InventoryCountStatuses.Posted;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InventoryCountStatuses.Posted)
            throw new InvalidOperationException("Posted counts cannot be cancelled.");
        Status = InventoryCountStatuses.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool IsTerminal() =>
        string.Equals(Status, InventoryCountStatuses.Posted, StringComparison.OrdinalIgnoreCase)
        || string.Equals(Status, InventoryCountStatuses.Cancelled, StringComparison.OrdinalIgnoreCase);

    private void EnsureNotTerminal()
    {
        if (IsTerminal())
            throw new InvalidOperationException($"Count {Number} is {Status} and cannot change.");
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

public static class InventoryCountStatuses
{
    public const string Draft = "DRAFT";
    public const string Counting = "COUNTING";
    public const string Review = "REVIEW";
    public const string Approved = "APPROVED";
    public const string Posted = "POSTED";
    public const string Cancelled = "CANCELLED";
}

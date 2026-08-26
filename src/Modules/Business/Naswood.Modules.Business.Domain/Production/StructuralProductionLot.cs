using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;
using Naswood.Modules.Business.Domain.Production.En14081;

namespace Naswood.Modules.Business.Domain.Production;

/// <summary>
/// EN 14081 structural timber production/classification run — not a Batch, Package, or stock balance.
/// Master aggregate layers: see <see cref="En14081.StructuralProductionLotSchema"/>.
/// </summary>
public sealed class StructuralProductionLot : BusinessEntity
{
    private readonly List<StructuralProductionLotInput> _inputs = new();

    private StructuralProductionLot() { }

    private StructuralProductionLot(
        Guid id,
        string productionLotNumber,
        Guid materialId,
        string materialCode,
        string actualGradingMethod,
        string? workCenterCode,
        DateOnly productionDate,
        string? shiftCode,
        string status,
        string notes,
        string? createdBy,
        string companyId,
        string plantId)
        : base(id)
    {
        ProductionLotNumber = productionLotNumber;
        MaterialId = materialId;
        MaterialCode = materialCode;
        ActualGradingMethod = actualGradingMethod;
        WorkCenterCode = workCenterCode;
        ProductionDate = productionDate;
        ShiftCode = shiftCode;
        Status = status;
        Notes = notes;
        CreatedBy = createdBy;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string ProductionLotNumber { get; private set; } = string.Empty;
    public Guid MaterialId { get; private set; }
    public string MaterialCode { get; private set; } = string.Empty;
    public string ActualGradingMethod { get; private set; } = string.Empty;
    public string? WorkCenterCode { get; private set; }
    public DateOnly ProductionDate { get; private set; }
    public string? ShiftCode { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public string Status { get; private set; } = "DRAFT";
    public string Notes { get; private set; } = string.Empty;
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? ReleasedAt { get; private set; }
    public string? ReleasedBy { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancelledBy { get; private set; }

    public IReadOnlyCollection<StructuralProductionLotInput> Inputs => _inputs;

    public static StructuralProductionLot Create(
        string productionLotNumber,
        Guid materialId,
        string materialCode,
        string actualGradingMethod,
        DateOnly productionDate,
        string? workCenterCode = null,
        string? shiftCode = null,
        string notes = "",
        string? createdBy = null,
        string companyId = "COMP-001",
        string plantId = "PLANT-001")
    {
        return new StructuralProductionLot(
            UuidV7.NewGuid(),
            productionLotNumber.Trim(),
            materialId,
            materialCode.Trim(),
            actualGradingMethod.Trim().ToUpperInvariant(),
            string.IsNullOrWhiteSpace(workCenterCode) ? null : workCenterCode.Trim(),
            productionDate,
            string.IsNullOrWhiteSpace(shiftCode) ? null : shiftCode.Trim(),
            "DRAFT",
            notes ?? string.Empty,
            createdBy,
            companyId,
            plantId);
    }

    public bool IsDraft => string.Equals(Status, "DRAFT", StringComparison.OrdinalIgnoreCase);

    public void ReplaceDraftDetails(
        string? workCenterCode,
        DateOnly productionDate,
        string? shiftCode,
        string notes)
    {
        EnsureDraft();
        WorkCenterCode = string.IsNullOrWhiteSpace(workCenterCode) ? null : workCenterCode.Trim();
        ProductionDate = productionDate;
        ShiftCode = string.IsNullOrWhiteSpace(shiftCode) ? null : shiftCode.Trim();
        Notes = notes ?? string.Empty;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ReplaceDraftInputs(IEnumerable<StructuralProductionLotInput> inputs)
    {
        EnsureDraft();
        _inputs.Clear();
        foreach (var input in inputs)
            _inputs.Add(input);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void TransitionStatus(string newStatus)
    {
        if (string.Equals(Status, StructuralProductionLotSchema.LotStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Cancelled production lot cannot change status.");

        var normalized = StructuralProductionLotSchema.LotStatus.Normalize(newStatus);
        var allowed = StructuralProductionLotSchema.LotStatus.Target.Any(s =>
            string.Equals(s, normalized, StringComparison.OrdinalIgnoreCase));
        if (!allowed)
            throw new InvalidOperationException($"Invalid production lot status: {newStatus}");

        if (string.Equals(Status, StructuralProductionLotSchema.LotStatus.Released, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(normalized, StructuralProductionLotSchema.LotStatus.Cancelled, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(normalized, StructuralProductionLotSchema.LotStatus.FpcPending, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Released lot can only move to FPC_PENDING (re-gate) or CANCELLED.");

        Status = normalized;
        if (string.Equals(Status, StructuralProductionLotSchema.LotStatus.InClassification, StringComparison.OrdinalIgnoreCase)
            && StartedAt is null)
            StartedAt = DateTimeOffset.UtcNow;
        if (string.Equals(Status, StructuralProductionLotSchema.LotStatus.Released, StringComparison.OrdinalIgnoreCase))
        {
            CompletedAt ??= DateTimeOffset.UtcNow;
            ReleasedAt ??= DateTimeOffset.UtcNow;
        }
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkReleased(string? releasedBy)
    {
        TransitionStatus(StructuralProductionLotSchema.LotStatus.Released);
        ReleasedBy = releasedBy;
        ReleasedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel(string reason, string? cancelledBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Cancellation reason is required.");
        if (string.Equals(Status, StructuralProductionLotSchema.LotStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            return;
        Status = StructuralProductionLotSchema.LotStatus.Cancelled;
        CancellationReason = reason.Trim();
        CancelledAt = DateTimeOffset.UtcNow;
        CancelledBy = cancelledBy;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        // Prefer Cancel for audit; SoftDelete only for admin cleanup of DRAFT empty shells.
        if (!IsDraft)
            throw new InvalidOperationException("Only DRAFT production lots may be soft-deleted; use Cancel instead.");
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void EnsureDraft()
    {
        if (!IsDraft)
            throw new InvalidOperationException("Critical fields can only be changed while Status is DRAFT.");
    }
}

public sealed class StructuralProductionLotInput
{
    private StructuralProductionLotInput() { }

    private StructuralProductionLotInput(
        Guid id,
        Guid productionLotId,
        Guid? sourceBatchId,
        Guid? sourceInventoryBalanceId,
        Guid? sourcePackageId,
        decimal quantity,
        string unit)
    {
        Id = id;
        ProductionLotId = productionLotId;
        SourceBatchId = sourceBatchId;
        SourceInventoryBalanceId = sourceInventoryBalanceId;
        SourcePackageId = sourcePackageId;
        Quantity = quantity;
        Unit = unit;
    }

    public Guid Id { get; private set; }
    public Guid ProductionLotId { get; private set; }
    public Guid? SourceBatchId { get; private set; }
    public Guid? SourceInventoryBalanceId { get; private set; }
    public Guid? SourcePackageId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = "PCS";

    public static StructuralProductionLotInput Create(
        Guid productionLotId,
        decimal quantity,
        string unit,
        Guid? sourceBatchId = null,
        Guid? sourceInventoryBalanceId = null,
        Guid? sourcePackageId = null)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (sourceBatchId is null && sourceInventoryBalanceId is null && sourcePackageId is null)
            throw new ArgumentException("At least one source reference is required.");
        return new StructuralProductionLotInput(
            UuidV7.NewGuid(),
            productionLotId,
            sourceBatchId,
            sourceInventoryBalanceId,
            sourcePackageId,
            quantity,
            string.IsNullOrWhiteSpace(unit) ? "PCS" : unit.Trim());
    }
}

using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public static class ProductionExecutionStatuses
{
    public const string NotStarted = "NOT_STARTED";
    public const string Running = "RUNNING";
    public const string Paused = "PAUSED";
    public const string Completed = "COMPLETED";
    public const string Cancelled = "CANCELLED";
}

/// <summary>Shop-floor actual for a planned ProductionOperation.</summary>
public sealed class ProductionOperationExecution : BusinessEntity
{
    private ProductionOperationExecution() { }

    private ProductionOperationExecution(
        Guid id,
        string number,
        Guid productionOrderId,
        Guid productionOperationId,
        Guid workCenterId,
        string status,
        string unit,
        Guid? destinationWarehouseId,
        Guid? destinationLocationId,
        Guid? structuralProductionLotId,
        string companyId,
        string? plantId)
        : base(id)
    {
        Number = number;
        ProductionOrderId = productionOrderId;
        ProductionOperationId = productionOperationId;
        WorkCenterId = workCenterId;
        Status = status;
        Unit = unit;
        DestinationWarehouseId = destinationWarehouseId;
        DestinationLocationId = destinationLocationId;
        StructuralProductionLotId = structuralProductionLotId;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public Guid ProductionOrderId { get; private set; }
    public Guid ProductionOperationId { get; private set; }
    public Guid WorkCenterId { get; private set; }
    public string Status { get; private set; } = ProductionExecutionStatuses.NotStarted;
    public DateTimeOffset? StartedAt { get; private set; }
    public string StartedByUserId { get; private set; } = string.Empty;
    public DateTimeOffset? PausedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public string CompletedByUserId { get; private set; } = string.Empty;
    public decimal TotalRunMinutes { get; private set; }
    public decimal TotalPauseMinutes { get; private set; }
    public decimal TotalDowntimeMinutes { get; private set; }
    public decimal InputQuantity { get; private set; }
    public decimal OutputQuantity { get; private set; }
    public decimal ScrapQuantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public Guid? DestinationWarehouseId { get; private set; }
    public Guid? DestinationLocationId { get; private set; }
    public Guid? StructuralProductionLotId { get; private set; }
    public Guid? ProductionOutputId { get; private set; }
    public DateTimeOffset? IntervalStartedAt { get; private set; }
    public string CancelReason { get; private set; } = string.Empty;
    public string CancelNote { get; private set; } = string.Empty;
    public string CancelledByUserId { get; private set; } = string.Empty;
    public string CompleteIdempotencyKey { get; private set; } = string.Empty;
    public string CompletePayloadHash { get; private set; } = string.Empty;
    public string CancelIdempotencyKey { get; private set; } = string.Empty;
    public string CancelPayloadHash { get; private set; } = string.Empty;

    public static ProductionOperationExecution Create(
        string number,
        Guid productionOrderId,
        Guid productionOperationId,
        Guid workCenterId,
        string? plantId,
        string unit = "",
        Guid? structuralProductionLotId = null,
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            number,
            productionOrderId,
            productionOperationId,
            workCenterId,
            ProductionExecutionStatuses.NotStarted,
            unit ?? string.Empty,
            null,
            null,
            structuralProductionLotId,
            companyId,
            plantId);

    public void Start(string actor, DateTimeOffset utc)
    {
        if (Status == ProductionExecutionStatuses.Completed)
            throw new InvalidOperationException("COMPLETED");
        if (Status == ProductionExecutionStatuses.Cancelled)
            throw new InvalidOperationException("CANCELLED");
        if (Status == ProductionExecutionStatuses.Running)
            return;
        Status = ProductionExecutionStatuses.Running;
        StartedAt ??= utc;
        if (string.IsNullOrWhiteSpace(StartedByUserId))
            StartedByUserId = actor ?? string.Empty;
        IntervalStartedAt = utc;
        PausedAt = null;
        UpdatedAt = utc;
    }

    public void Pause(DateTimeOffset utc)
    {
        if (Status != ProductionExecutionStatuses.Running)
            throw new InvalidOperationException("NOT_RUNNING");
        FlushRun(utc);
        Status = ProductionExecutionStatuses.Paused;
        PausedAt = utc;
        IntervalStartedAt = utc;
        UpdatedAt = utc;
    }

    public void Resume(DateTimeOffset utc)
    {
        if (Status != ProductionExecutionStatuses.Paused)
            throw new InvalidOperationException("NOT_PAUSED");
        FlushPause(utc);
        Status = ProductionExecutionStatuses.Running;
        PausedAt = null;
        IntervalStartedAt = utc;
        UpdatedAt = utc;
    }

    public void Complete(string actor, DateTimeOffset utc)
    {
        if (Status == ProductionExecutionStatuses.Completed)
            return;
        if (Status is not (ProductionExecutionStatuses.Running or ProductionExecutionStatuses.Paused))
            throw new InvalidOperationException("NOT_ACTIVE");
        if (Status == ProductionExecutionStatuses.Running)
            FlushRun(utc);
        else
            FlushPause(utc);
        Status = ProductionExecutionStatuses.Completed;
        CompletedAt = utc;
        CompletedByUserId = actor ?? string.Empty;
        IntervalStartedAt = null;
        UpdatedAt = utc;
    }

    public void Cancel(string actor, string reason, string note, DateTimeOffset utc)
    {
        if (Status == ProductionExecutionStatuses.Cancelled)
            return;
        Status = ProductionExecutionStatuses.Cancelled;
        CancelReason = (reason ?? string.Empty).Trim().ToUpperInvariant();
        CancelNote = (note ?? string.Empty).Trim();
        CancelledByUserId = actor ?? string.Empty;
        IntervalStartedAt = null;
        UpdatedAt = utc;
    }

    public void RememberCompleteIdempotency(string key, string hash)
    {
        if (string.IsNullOrWhiteSpace(CompleteIdempotencyKey))
        {
            CompleteIdempotencyKey = (key ?? string.Empty).Trim();
            CompletePayloadHash = hash ?? string.Empty;
        }
    }

    public void RememberCancelIdempotency(string key, string hash)
    {
        if (string.IsNullOrWhiteSpace(CancelIdempotencyKey))
        {
            CancelIdempotencyKey = (key ?? string.Empty).Trim();
            CancelPayloadHash = hash ?? string.Empty;
        }
    }

    public void AddInput(decimal qty, string unit)
    {
        if (qty < 0) throw new InvalidOperationException("NEG");
        InputQuantity += qty;
        if (!string.IsNullOrWhiteSpace(unit)) Unit = unit.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddScrap(decimal qty)
    {
        if (qty <= 0) throw new InvalidOperationException("SCRAP");
        ScrapQuantity += qty;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetOutput(decimal qty, Guid? warehouseId, Guid? locationId, Guid? outputId)
    {
        OutputQuantity = qty;
        DestinationWarehouseId = warehouseId;
        DestinationLocationId = locationId;
        ProductionOutputId = outputId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AttachPlot(Guid? plotId)
    {
        StructuralProductionLotId = plotId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddDowntimeMinutes(decimal minutes)
    {
        if (minutes < 0) return;
        TotalDowntimeMinutes += minutes;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public decimal LiveRunMinutes(DateTimeOffset utc)
    {
        var extra = Status == ProductionExecutionStatuses.Running && IntervalStartedAt is DateTimeOffset s
            ? Minutes(s, utc) : 0m;
        return TotalRunMinutes + extra;
    }

    public decimal LivePauseMinutes(DateTimeOffset utc)
    {
        var extra = Status == ProductionExecutionStatuses.Paused && IntervalStartedAt is DateTimeOffset s
            ? Minutes(s, utc) : 0m;
        return TotalPauseMinutes + extra;
    }

    private void FlushRun(DateTimeOffset utc)
    {
        if (IntervalStartedAt is DateTimeOffset s)
            TotalRunMinutes += Minutes(s, utc);
        IntervalStartedAt = null;
    }

    private void FlushPause(DateTimeOffset utc)
    {
        if (IntervalStartedAt is DateTimeOffset s)
            TotalPauseMinutes += Minutes(s, utc);
        IntervalStartedAt = null;
    }

    private static decimal Minutes(DateTimeOffset from, DateTimeOffset to)
        => Math.Round((decimal)(to - from).TotalMinutes, 4);
}

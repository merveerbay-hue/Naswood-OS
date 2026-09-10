using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

public static class ProductionOutputStatuses
{
    public const string Draft = "DRAFT";
    public const string Ready = "READY";
    public const string Posted = "POSTED";
    public const string Cancelled = "CANCELLED";
}

public static class ProductionQcDecisions
{
    public const string Released = "Released";
    public const string Rejected = "Rejected";

    public static string Normalize(string? decision)
    {
        var d = (decision ?? string.Empty).Trim();
        if (d.Equals(Released, StringComparison.OrdinalIgnoreCase)
            || d.Equals("Release", StringComparison.OrdinalIgnoreCase)
            || d.Equals("AVAILABLE", StringComparison.OrdinalIgnoreCase))
            return Released;
        if (d.Equals(Rejected, StringComparison.OrdinalIgnoreCase)
            || d.Equals("Reject", StringComparison.OrdinalIgnoreCase)
            || d.Equals("Blocked", StringComparison.OrdinalIgnoreCase))
            return Rejected;
        return string.Empty;
    }
}

/// <summary>Stockable production output document. Does not replace EN 14081 PLOT.</summary>
public sealed class ProductionOutput : BusinessEntity
{
    private ProductionOutput() { }

    private ProductionOutput(
        Guid id,
        string number,
        Guid productionOrderId,
        Guid outputMaterialId,
        string outputMaterialCode,
        string warehouseCode,
        string locationCode,
        Guid? warehouseId,
        Guid? locationId,
        string workCenterCode,
        string stockStatus,
        string status,
        Guid? outputBatchId,
        string outputLotNumber,
        decimal inputQuantity,
        decimal outputQuantity,
        decimal scrapQuantity,
        string unitOfMeasure,
        string postedBy,
        string companyId,
        string? plantId)
        : base(id)
    {
        Number = number;
        ProductionOrderId = productionOrderId;
        OutputMaterialId = outputMaterialId;
        OutputMaterialCode = outputMaterialCode;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        WarehouseId = warehouseId;
        LocationId = locationId;
        WorkCenterCode = workCenterCode;
        StockStatus = stockStatus;
        Status = status;
        OutputBatchId = outputBatchId;
        OutputLotNumber = outputLotNumber;
        InputQuantity = inputQuantity;
        OutputQuantity = outputQuantity;
        ScrapQuantity = scrapQuantity;
        UnitOfMeasure = unitOfMeasure;
        PostedBy = postedBy;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public Guid ProductionOrderId { get; private set; }
    public Guid OutputMaterialId { get; private set; }
    public string OutputMaterialCode { get; private set; } = string.Empty;
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public Guid? WarehouseId { get; private set; }
    public Guid? LocationId { get; private set; }
    public string WorkCenterCode { get; private set; } = string.Empty;
    public string StockStatus { get; private set; } = "Available";
    public string Status { get; private set; } = ProductionOutputStatuses.Draft;
    public Guid? OutputBatchId { get; private set; }
    public string OutputLotNumber { get; private set; } = string.Empty;
    public decimal InputQuantity { get; private set; }
    public decimal OutputQuantity { get; private set; }
    public decimal ScrapQuantity { get; private set; }
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public string PostedBy { get; private set; } = string.Empty;
    public DateTimeOffset? PostedAt { get; private set; }
    public string CancelledBy { get; private set; } = string.Empty;
    public string CancelReason { get; private set; } = string.Empty;
    public DateTimeOffset? CancelledAt { get; private set; }
    public string QcDecision { get; private set; } = string.Empty;
    public string QcDecidedBy { get; private set; } = string.Empty;
    public DateTimeOffset? QcDecidedAt { get; private set; }
    public string QcInspectionReference { get; private set; } = string.Empty;
    public string QcNotes { get; private set; } = string.Empty;

    public static ProductionOutput Create(
        string number,
        Guid productionOrderId,
        Guid outputMaterialId,
        string outputMaterialCode,
        string warehouseCode,
        string locationCode,
        Guid? warehouseId,
        Guid? locationId,
        string workCenterCode,
        string stockStatus,
        string? plantId,
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            number,
            productionOrderId,
            outputMaterialId,
            outputMaterialCode,
            warehouseCode,
            locationCode,
            warehouseId,
            locationId,
            workCenterCode ?? string.Empty,
            string.IsNullOrWhiteSpace(stockStatus) ? "Available" : stockStatus.Trim(),
            ProductionOutputStatuses.Ready,
            null,
            string.Empty,
            0, 0, 0,
            string.Empty,
            string.Empty,
            companyId,
            plantId);

    public void MarkPosted(
        Guid outputBatchId,
        string outputLotNumber,
        decimal inputQuantity,
        decimal outputQuantity,
        decimal scrapQuantity,
        string unitOfMeasure,
        string postedBy)
    {
        if (Status == ProductionOutputStatuses.Posted)
            return;
        if (Status == ProductionOutputStatuses.Cancelled)
            throw new InvalidOperationException("Cancelled production output cannot be posted.");
        OutputBatchId = outputBatchId;
        OutputLotNumber = outputLotNumber;
        InputQuantity = inputQuantity;
        OutputQuantity = outputQuantity;
        ScrapQuantity = scrapQuantity;
        UnitOfMeasure = unitOfMeasure;
        PostedBy = postedBy ?? string.Empty;
        PostedAt = DateTimeOffset.UtcNow;
        Status = ProductionOutputStatuses.Posted;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RecordQc(string decision, string decidedBy, string inspectionReference, string notes)
    {
        if (Status == ProductionOutputStatuses.Cancelled)
            throw new InvalidOperationException("Cancelled production output cannot be QC decided.");
        if (Status != ProductionOutputStatuses.Posted)
            throw new InvalidOperationException("Only posted production output can be QC decided.");
        var next = ProductionQcDecisions.Normalize(decision);
        if (next.Length == 0)
            throw new InvalidOperationException("QC decision must be Released or Rejected.");
        if (!string.IsNullOrWhiteSpace(QcDecision)
            && !string.Equals(QcDecision, next, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("QC decision is already recorded.");
        QcDecision = next;
        QcDecidedBy = decidedBy ?? string.Empty;
        QcDecidedAt = DateTimeOffset.UtcNow;
        QcInspectionReference = (inspectionReference ?? string.Empty).Trim();
        QcNotes = (notes ?? string.Empty).Trim();
        StockStatus = next == ProductionQcDecisions.Rejected ? "Rejected" : "Available";
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkCancelled(string cancelledBy, string reason)
    {
        if (Status == ProductionOutputStatuses.Cancelled)
            return;
        if (Status != ProductionOutputStatuses.Posted)
            throw new InvalidOperationException("Only posted production output can be reversed.");
        Status = ProductionOutputStatuses.Cancelled;
        CancelledBy = cancelledBy ?? string.Empty;
        CancelReason = (reason ?? string.Empty).Trim();
        CancelledAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

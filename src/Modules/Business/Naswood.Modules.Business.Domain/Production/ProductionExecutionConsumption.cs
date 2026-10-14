using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

/// <summary>Posted operation-level consumption. Genealogy companion to ProductionLotSource.</summary>
public sealed class ProductionExecutionConsumption : BusinessEntity
{
    private ProductionExecutionConsumption() { }

    private ProductionExecutionConsumption(
        Guid id,
        Guid executionId,
        Guid productionOrderId,
        Guid sourcePackageId,
        Guid sourceLotId,
        Guid sourceMaterialId,
        string sourceMaterialCode,
        string packageNumber,
        string barcode,
        string sourceWarehouseCode,
        string sourceLocationCode,
        Guid? packageContentId,
        string physicalMeasure,
        decimal consumedQuantity,
        string unit,
        decimal remainingPackageQuantity,
        string idempotencyKey,
        string payloadHash,
        decimal? consumedPieceCount,
        string companyId,
        string? plantId)
        : base(id)
    {
        ExecutionId = executionId;
        ProductionOrderId = productionOrderId;
        SourcePackageId = sourcePackageId;
        SourceLotId = sourceLotId;
        SourceMaterialId = sourceMaterialId;
        SourceMaterialCode = sourceMaterialCode;
        PackageNumber = packageNumber;
        Barcode = barcode;
        SourceWarehouseCode = sourceWarehouseCode;
        SourceLocationCode = sourceLocationCode;
        PackageContentId = packageContentId;
        PhysicalMeasure = physicalMeasure;
        ConsumedQuantity = consumedQuantity;
        Unit = unit;
        RemainingPackageQuantity = remainingPackageQuantity;
        IdempotencyKey = idempotencyKey;
        PayloadHash = payloadHash;
        ConsumedPieceCount = consumedPieceCount;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid ExecutionId { get; private set; }
    public Guid ProductionOrderId { get; private set; }
    public Guid SourcePackageId { get; private set; }
    public Guid SourceLotId { get; private set; }
    public Guid SourceMaterialId { get; private set; }
    public string SourceMaterialCode { get; private set; } = string.Empty;
    public string PackageNumber { get; private set; } = string.Empty;
    public string Barcode { get; private set; } = string.Empty;
    public string SourceWarehouseCode { get; private set; } = string.Empty;
    public string SourceLocationCode { get; private set; } = string.Empty;
    public Guid? PackageContentId { get; private set; }
    public string PhysicalMeasure { get; private set; } = string.Empty;
    public decimal ConsumedQuantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal RemainingPackageQuantity { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public string PayloadHash { get; private set; } = string.Empty;
    public decimal? ConsumedPieceCount { get; private set; }

    public static ProductionExecutionConsumption Create(
        Guid executionId,
        Guid productionOrderId,
        Guid sourcePackageId,
        Guid sourceLotId,
        Guid sourceMaterialId,
        string sourceMaterialCode,
        string packageNumber,
        string barcode,
        string sourceWarehouseCode,
        string sourceLocationCode,
        Guid? packageContentId,
        string physicalMeasure,
        decimal consumedQuantity,
        string unit,
        decimal remainingPackageQuantity,
        string idempotencyKey,
        string? plantId,
        string companyId = "COMP-001",
        string payloadHash = "",
        decimal? consumedPieceCount = null)
        => new(
            UuidV7.NewGuid(),
            executionId,
            productionOrderId,
            sourcePackageId,
            sourceLotId,
            sourceMaterialId,
            sourceMaterialCode ?? string.Empty,
            packageNumber ?? string.Empty,
            barcode ?? string.Empty,
            sourceWarehouseCode ?? string.Empty,
            sourceLocationCode ?? string.Empty,
            packageContentId,
            physicalMeasure ?? string.Empty,
            consumedQuantity,
            unit ?? string.Empty,
            remainingPackageQuantity,
            idempotencyKey ?? string.Empty,
            payloadHash ?? string.Empty,
            consumedPieceCount,
            companyId,
            plantId);
}

using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Production;

/// <summary>Output production lot (inventory Batch) → source lot genealogy. Not a JSON blob.</summary>
public sealed class ProductionLotSource : BusinessEntity
{
    private ProductionLotSource() { }

    private ProductionLotSource(
        Guid id,
        Guid productionLotId,
        Guid sourceLotId,
        Guid sourceMaterialId,
        string sourceMaterialCode,
        decimal consumedQuantity,
        string unit,
        Guid productionOrderId,
        Guid productionOutputId,
        Guid? sourcePackageId,
        Guid? productionOperationExecutionId,
        string companyId,
        string? plantId)
        : base(id)
    {
        ProductionLotId = productionLotId;
        SourceLotId = sourceLotId;
        SourceMaterialId = sourceMaterialId;
        SourceMaterialCode = sourceMaterialCode;
        ConsumedQuantity = consumedQuantity;
        Unit = unit;
        ProductionOrderId = productionOrderId;
        ProductionOutputId = productionOutputId;
        SourcePackageId = sourcePackageId;
        ProductionOperationExecutionId = productionOperationExecutionId;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid ProductionLotId { get; private set; }
    public Guid SourceLotId { get; private set; }
    public Guid SourceMaterialId { get; private set; }
    public string SourceMaterialCode { get; private set; } = string.Empty;
    public decimal ConsumedQuantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public Guid ProductionOrderId { get; private set; }
    public Guid ProductionOutputId { get; private set; }
    public Guid? SourcePackageId { get; private set; }
    public Guid? ProductionOperationExecutionId { get; private set; }

    public static ProductionLotSource Create(
        Guid productionLotId,
        Guid sourceLotId,
        Guid sourceMaterialId,
        string sourceMaterialCode,
        decimal consumedQuantity,
        string unit,
        Guid productionOrderId,
        Guid productionOutputId,
        Guid? sourcePackageId,
        string? plantId,
        string companyId = "COMP-001",
        Guid? productionOperationExecutionId = null)
        => new(
            UuidV7.NewGuid(),
            productionLotId,
            sourceLotId,
            sourceMaterialId,
            sourceMaterialCode,
            consumedQuantity,
            unit ?? string.Empty,
            productionOrderId,
            productionOutputId,
            sourcePackageId,
            productionOperationExecutionId,
            companyId,
            plantId);
}

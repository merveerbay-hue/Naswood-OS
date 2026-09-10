using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public static class PackageOperationTypes
{
    public const string Split = "SPLIT";
    public const string Merge = "MERGE";
    public const string Repack = "REPACK";
    public const string PartialMove = "PARTIAL_MOVE";
}

public static class PackageRelationTypes
{
    public const string Split = "SPLIT";
    public const string Merge = "MERGE";
    public const string Repack = "REPACK";
    public const string PartialMove = "PARTIAL_MOVE";
}

public sealed class PackageOperation : BusinessEntity
{
    private PackageOperation() { }

    private PackageOperation(
        Guid id,
        string number,
        string operationType,
        string status,
        string warehouseCode,
        string locationCode,
        string notes,
        string postedBy,
        string companyId,
        string? plantId)
        : base(id)
    {
        Number = number;
        OperationType = operationType;
        Status = status;
        WarehouseCode = warehouseCode;
        LocationCode = locationCode;
        Notes = notes;
        PostedBy = postedBy;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
        PostedAt = CreatedAt;
    }

    public string Number { get; private set; } = string.Empty;
    public string OperationType { get; private set; } = string.Empty;
    public string Status { get; private set; } = "POSTED";
    public string WarehouseCode { get; private set; } = string.Empty;
    public string LocationCode { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public string PostedBy { get; private set; } = string.Empty;
    public DateTimeOffset? PostedAt { get; private set; }

    public static PackageOperation Create(
        string number,
        string operationType,
        string warehouseCode,
        string locationCode,
        string postedBy,
        string? plantId,
        string notes = "",
        string companyId = "COMP-001")
        => new(
            UuidV7.NewGuid(),
            number,
            operationType,
            "POSTED",
            warehouseCode ?? "",
            locationCode ?? "",
            notes ?? "",
            postedBy ?? "",
            companyId,
            plantId);
}

public sealed class PackageRelation : BusinessEntity
{
    private PackageRelation() { }

    private PackageRelation(
        Guid id,
        Guid operationId,
        Guid sourcePackageId,
        Guid targetPackageId,
        string relationType,
        decimal quantity,
        string unit,
        string companyId,
        string? plantId)
        : base(id)
    {
        OperationId = operationId;
        SourcePackageId = sourcePackageId;
        TargetPackageId = targetPackageId;
        RelationType = relationType;
        Quantity = quantity;
        Unit = unit;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Guid OperationId { get; private set; }
    public Guid SourcePackageId { get; private set; }
    public Guid TargetPackageId { get; private set; }
    public string RelationType { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;

    public static PackageRelation Create(
        Guid operationId,
        Guid sourcePackageId,
        Guid targetPackageId,
        string relationType,
        decimal quantity,
        string unit,
        string? plantId,
        string companyId = "COMP-001")
    {
        if (sourcePackageId == targetPackageId)
            throw new InvalidOperationException("Package relation cannot point to itself.");
        return new(
            UuidV7.NewGuid(),
            operationId,
            sourcePackageId,
            targetPackageId,
            relationType,
            quantity,
            unit ?? "",
            companyId,
            plantId);
    }
}

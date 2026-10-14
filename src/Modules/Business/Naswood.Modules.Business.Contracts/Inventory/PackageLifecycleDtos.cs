namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class PackageContentSplitLineDto
{
    public Guid? SourceContentId { get; init; }
    public decimal? ThicknessMm { get; init; }
    public decimal? WidthMm { get; init; }
    public decimal? LengthMm { get; init; }
    public decimal Quantity { get; init; }
    public decimal? PieceCount { get; init; }
}

public sealed class SplitPackageRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public IReadOnlyList<PackageContentSplitLineDto> Lines { get; init; } = [];
}

public sealed class MergePackagesRequestDto
{
    public string Number { get; init; } = string.Empty;
    public IReadOnlyList<Guid> SourcePackageIds { get; init; } = [];
    public string PhysicalGroupLabel { get; init; } = string.Empty;
}

public sealed class RepackPackageRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string PhysicalGroupLabel { get; init; } = string.Empty;
}

public sealed class PartialMovePackageRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string PhysicalGroupLabel { get; init; } = string.Empty;
    public IReadOnlyList<PackageContentSplitLineDto> Lines { get; init; } = [];
}

public sealed class PackageOperationResultDto
{
    public Guid OperationId { get; init; }
    public string Number { get; init; } = string.Empty;
    public string OperationType { get; init; } = string.Empty;
    public bool IdempotentReplay { get; init; }
    public PackagePassportDto? Source { get; init; }
    public PackagePassportDto? Target { get; init; }
    public IReadOnlyList<PackagePassportDto> Sources { get; init; } = [];
    public string Message { get; init; } = string.Empty;
    public bool ReprintOriginal { get; init; }
    public bool PrintTarget { get; init; }
}

public sealed class PackageRelationRowDto
{
    public string RelationType { get; init; } = string.Empty;
    public Guid SourcePackageId { get; init; }
    public Guid TargetPackageId { get; init; }
    public string SourcePackageNo { get; init; } = string.Empty;
    public string TargetPackageNo { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string Direction { get; init; } = string.Empty;
}

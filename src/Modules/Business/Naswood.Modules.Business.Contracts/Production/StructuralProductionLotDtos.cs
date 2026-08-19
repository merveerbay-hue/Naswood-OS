namespace Naswood.Modules.Business.Contracts.Production;

public sealed class StructuralProductionLotInputDto
{
    public Guid Id { get; init; }
    public Guid? SourceBatchId { get; init; }
    public Guid? SourceInventoryBalanceId { get; init; }
    public Guid? SourcePackageId { get; init; }
    public decimal Quantity { get; init; }
    public required string Unit { get; init; }
}

public sealed class StructuralProductionLotDto
{
    public Guid Id { get; init; }
    public required string ProductionLotNumber { get; init; }
    public Guid MaterialId { get; init; }
    public required string MaterialCode { get; init; }
    public required string ActualGradingMethod { get; init; }
    public string? WorkCenterCode { get; init; }
    public DateOnly ProductionDate { get; init; }
    public string? ShiftCode { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public required string Status { get; init; }
    public required string Notes { get; init; }
    public string? PlantId { get; init; }
    public string? CreatedBy { get; init; }
    public DateTimeOffset? ReleasedAt { get; init; }
    public string? ReleasedBy { get; init; }
    public string? CancellationReason { get; init; }
    public DateTimeOffset? CancelledAt { get; init; }
    public string? CancelledBy { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required IReadOnlyList<StructuralProductionLotInputDto> Inputs { get; init; }
}

public sealed class PagedStructuralProductionLotDto
{
    public required IReadOnlyList<StructuralProductionLotDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

public sealed class StructuralProductionLotInputRequestDto
{
    public Guid? SourceBatchId { get; init; }
    public Guid? SourceInventoryBalanceId { get; init; }
    public Guid? SourcePackageId { get; init; }
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = "PCS";
}

public sealed class CreateStructuralProductionLotRequestDto
{
    public Guid MaterialId { get; init; }
    public required string ActualGradingMethod { get; init; }
    public string? WorkCenterCode { get; init; }
    public DateOnly? ProductionDate { get; init; }
    public string? ShiftCode { get; init; }
    public string Notes { get; init; } = string.Empty;
    public IReadOnlyList<StructuralProductionLotInputRequestDto>? Inputs { get; init; }
}

public sealed class UpdateStructuralProductionLotDraftRequestDto
{
    public string? WorkCenterCode { get; init; }
    public DateOnly? ProductionDate { get; init; }
    public string? ShiftCode { get; init; }
    public string Notes { get; init; } = string.Empty;
    public IReadOnlyList<StructuralProductionLotInputRequestDto>? Inputs { get; init; }
}

public sealed class TransitionStructuralProductionLotStatusRequestDto
{
    public required string Status { get; init; }
}

public sealed class CancelStructuralProductionLotRequestDto
{
    public required string Reason { get; init; }
}

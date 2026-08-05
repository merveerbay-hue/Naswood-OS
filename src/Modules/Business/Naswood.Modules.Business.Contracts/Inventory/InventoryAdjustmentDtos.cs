namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class InventoryAdjustmentDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string WarehouseCode { get; init; }
    public string? Reason { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertInventoryAdjustmentRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedInventoryAdjustmentDto
{
    public required IReadOnlyList<InventoryAdjustmentDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

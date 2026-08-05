namespace Naswood.Modules.Business.Contracts.Purchasing;

public sealed class PurchaseReturnDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string SupplierCode { get; init; }
    public required string PurchaseOrderNumber { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertPurchaseReturnRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string SupplierCode { get; init; } = string.Empty;
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedPurchaseReturnDto
{
    public required IReadOnlyList<PurchaseReturnDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

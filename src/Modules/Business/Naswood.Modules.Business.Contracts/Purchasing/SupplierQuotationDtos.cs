namespace Naswood.Modules.Business.Contracts.Purchasing;

public sealed class SupplierQuotationDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string SupplierCode { get; init; }
    public required string RfqNumber { get; init; }
    public decimal TotalAmount { get; init; }
    public required string Currency { get; init; }
    public required string Status { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertSupplierQuotationRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string SupplierCode { get; init; } = string.Empty;
    public string RfqNumber { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

public sealed class PagedSupplierQuotationDto
{
    public required IReadOnlyList<SupplierQuotationDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

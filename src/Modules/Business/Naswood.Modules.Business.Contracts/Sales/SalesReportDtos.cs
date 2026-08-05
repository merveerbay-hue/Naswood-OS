namespace Naswood.Modules.Business.Contracts.Sales;

public sealed class SalesReportDto
{
    public required Guid Id { get; init; }
    public required string ReportCode { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public string? Description { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertSalesReportRequestDto
{
    public string ReportCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public sealed class PagedSalesReportDto
{
    public required IReadOnlyList<SalesReportDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

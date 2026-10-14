namespace Naswood.Modules.Business.Contracts.Purchasing;

public sealed class RfqDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public string? Title { get; init; }
    public DateOnly? DueDate { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertRfqRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DateOnly? DueDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedRfqDto
{
    public required IReadOnlyList<RfqDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

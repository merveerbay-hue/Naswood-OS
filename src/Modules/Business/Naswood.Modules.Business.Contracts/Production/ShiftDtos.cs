namespace Naswood.Modules.Business.Contracts.Production;

public sealed class ShiftDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public required string Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertShiftRequestDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public string? PlantId { get; init; }
}

public sealed class PagedShiftDto
{
    public required IReadOnlyList<ShiftDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

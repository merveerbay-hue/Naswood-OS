namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class LocationDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string WarehouseCode { get; init; }
    public required string LocationType { get; init; }
    public required string Status { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertLocationRequestDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string WarehouseCode { get; init; } = string.Empty;
    public string LocationType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

public sealed class PagedLocationDto
{
    public required IReadOnlyList<LocationDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class MaterialDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Category { get; init; }
    public required string UnitOfMeasure { get; init; }
    public required string Status { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertMaterialRequestDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string UnitOfMeasure { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

public sealed class PagedMaterialDto
{
    public required IReadOnlyList<MaterialDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

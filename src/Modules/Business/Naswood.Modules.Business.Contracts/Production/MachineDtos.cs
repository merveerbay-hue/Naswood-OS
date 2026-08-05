namespace Naswood.Modules.Business.Contracts.Production;

public sealed class MachineDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string? WorkCenterCode { get; init; }
    public required string Status { get; init; }
    public decimal OeeTarget { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertMachineRequestDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string WorkCenterCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal OeeTarget { get; init; }
}

public sealed class PagedMachineDto
{
    public required IReadOnlyList<MachineDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

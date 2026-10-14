namespace Naswood.Modules.Business.Contracts.Sales;

public sealed class DeliveryDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string ShipmentNumber { get; init; }
    public required string CustomerCode { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertDeliveryRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string ShipmentNumber { get; init; } = string.Empty;
    public string CustomerCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedDeliveryDto
{
    public required IReadOnlyList<DeliveryDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

namespace Naswood.Modules.Business.Contracts.Inventory;

public sealed class StockTransferDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required string FromWarehouseCode { get; init; }
    public required string ToWarehouseCode { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public required string CompanyId { get; init; }
    public string? PlantId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class UpsertStockTransferRequestDto
{
    public string Number { get; init; } = string.Empty;
    public string FromWarehouseCode { get; init; } = string.Empty;
    public string ToWarehouseCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class PagedStockTransferDto
{
    public required IReadOnlyList<StockTransferDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}

/// <summary>Intra-factory location transfer — posts balances + TRANSFER_OUT/IN ledger pair.</summary>
public sealed class ExecuteStockTransferRequestDto
{
    public string Number { get; init; } = string.Empty;
    /// <summary>Working factory. Source and target must belong to this plant.</summary>
    public string? PlantId { get; init; }
    /// <summary>If set and differs from PlantId → inter-factory transfer (rejected).</summary>
    public string? ToPlantId { get; init; }
    public string MaterialCode { get; init; } = string.Empty;
    public string LotNumber { get; init; } = string.Empty;
    public string FromWarehouseCode { get; init; } = string.Empty;
    public string FromLocationCode { get; init; } = string.Empty;
    public string ToWarehouseCode { get; init; } = string.Empty;
    public string ToLocationCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = "Piece";
    public string PackageNumber { get; init; } = string.Empty;
    public string MaterialIdentityNumber { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class ExecuteStockTransferResultDto
{
    public required Guid DocumentId { get; init; }
    public required string DocumentNumber { get; init; }
    public required string Status { get; init; }
    public required string PlantId { get; init; }
    public required string MaterialCode { get; init; }
    public required string LotNumber { get; init; }
    public required string FromWarehouseCode { get; init; }
    public required string FromLocationCode { get; init; }
    public required string ToWarehouseCode { get; init; }
    public required string ToLocationCode { get; init; }
    public required decimal Quantity { get; init; }
    public required string OutMovementNumber { get; init; }
    public required string InMovementNumber { get; init; }
}

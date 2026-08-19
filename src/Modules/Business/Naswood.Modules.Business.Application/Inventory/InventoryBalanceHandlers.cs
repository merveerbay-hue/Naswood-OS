using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IInventoryBalanceRepository
{
    Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryBalance?> FindByKeyAsync(string materialCode, string warehouseCode, string locationCode, string batchNumber, string? plantId = null, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryBalance entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<InventoryBalance> Items, int Total)> SearchAsync(
        string? q,
        int page,
        int pageSize,
        string? plantId = null,
        string? warehouseCode = null,
        string? locationCode = null,
        CancellationToken cancellationToken = default);
}

public sealed record SearchInventoryBalanceQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    string? WarehouseCode,
    string? LocationCode,
    Guid? WarehouseId,
    Guid? LocationId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedInventoryBalanceDto>>;

public sealed record GetInventoryBalanceByIdQuery(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<InventoryBalanceDto>>;

public sealed record CreateInventoryBalanceCommand(string MaterialCode, string WarehouseCode, string LocationCode, string BatchNumber, decimal QuantityOnHand, decimal QuantityReserved, string Status) : ICommand<Result<InventoryBalanceDto>>;
public sealed record UpdateInventoryBalanceCommand(Guid Id, string MaterialCode, string WarehouseCode, string LocationCode, string BatchNumber, decimal QuantityOnHand, decimal QuantityReserved, string Status) : ICommand<Result<InventoryBalanceDto>>;
public sealed record DeleteInventoryBalanceCommand(Guid Id) : ICommand<Result>;

public static class InventoryBalanceMapper
{
    public static InventoryBalanceDto ToDto(InventoryBalance e) => new()
    {
        Id = e.Id,
        MaterialCode = e.MaterialCode,
        WarehouseCode = e.WarehouseCode,
        LocationCode = e.LocationCode,
        BatchNumber = e.BatchNumber,
        QuantityOnHand = e.QuantityOnHand,
        QuantityReserved = e.QuantityReserved,
        Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchInventoryBalanceQueryHandler : IQueryHandler<SearchInventoryBalanceQuery, Result<PagedInventoryBalanceDto>>
{
    private readonly IInventoryBalanceRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;

    public SearchInventoryBalanceQueryHandler(
        IInventoryBalanceRepository repo,
        IWarehouseRepository warehouses,
        ILocationRepository locations)
    {
        _repo = repo;
        _warehouses = warehouses;
        _locations = locations;
    }

    public async Task<Result<PagedInventoryBalanceDto>> HandleAsync(
        SearchInventoryBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(query.PlantId) ? null : query.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                "INV-BAL-004",
                "Plant / factory context is required."));

        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, plantId))
            return Result.Failure<PagedInventoryBalanceDto>(Error.Forbidden(
                "INV-BAL-403",
                "Bu tesisin stok bakiyelerini görüntüleme yetkiniz yok."));

        var warehouseCode = string.IsNullOrWhiteSpace(query.WarehouseCode) ? null : query.WarehouseCode.Trim();
        var locationCode = string.IsNullOrWhiteSpace(query.LocationCode) ? null : query.LocationCode.Trim();

        if (query.WarehouseId is Guid warehouseId)
        {
            var whById = await _warehouses.GetByIdAsync(warehouseId, cancellationToken).ConfigureAwait(false);
            if (whById is null || whById.IsDeleted)
                return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                    "INV-BAL-017",
                    "Warehouse was not found."));
            if (!string.IsNullOrWhiteSpace(whById.PlantId)
                && !string.Equals(whById.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Forbidden(
                    "INV-BAL-403",
                    "Warehouse seçili fabrikaya ait değil."));
            if (query.AllowedPlantIds is { Count: > 0 }
                && !string.IsNullOrWhiteSpace(whById.PlantId)
                && !PlantAccess.CanAccess(query.AllowedPlantIds, whById.PlantId))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Forbidden(
                    "INV-BAL-403",
                    "Bu deponun stok bakiyelerini görüntüleme yetkiniz yok."));
            warehouseCode = whById.Code;
        }
        else if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            var wh = await _warehouses.GetByCodeAndPlantAsync(warehouseCode, plantId, cancellationToken).ConfigureAwait(false);
            if (wh is null || wh.IsDeleted)
                return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                    "INV-BAL-017",
                    $"Depo '{warehouseCode}' bu tesiste ({plantId}) tanımlı değil."));
            if (!string.IsNullOrWhiteSpace(wh.PlantId)
                && !string.Equals(wh.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Forbidden(
                    "INV-BAL-403",
                    "Warehouse seçili fabrikaya ait değil."));
            warehouseCode = wh.Code;
        }

        if (query.LocationId is Guid locationId)
        {
            var locById = await _locations.GetByIdAsync(locationId, cancellationToken).ConfigureAwait(false);
            if (locById is null || locById.IsDeleted)
                return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                    "INV-BAL-019",
                    "Location was not found."));
            if (!string.IsNullOrWhiteSpace(locById.PlantId)
                && !string.Equals(locById.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Forbidden(
                    "INV-BAL-403",
                    "Location seçili fabrikaya ait değil."));
            if (query.AllowedPlantIds is { Count: > 0 }
                && !string.IsNullOrWhiteSpace(locById.PlantId)
                && !PlantAccess.CanAccess(query.AllowedPlantIds, locById.PlantId))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Forbidden(
                    "INV-BAL-403",
                    "Bu lokasyonun stok bakiyelerini görüntüleme yetkiniz yok."));
            if (!string.IsNullOrWhiteSpace(warehouseCode)
                && !string.Equals(locById.WarehouseCode, warehouseCode, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                    "INV-BAL-020",
                    "Location seçilen depoya ait değil."));
            warehouseCode ??= locById.WarehouseCode;
            locationCode = locById.Code;
        }
        else if (!string.IsNullOrWhiteSpace(locationCode))
        {
            if (string.IsNullOrWhiteSpace(warehouseCode))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                    "INV-BAL-018",
                    "Location filtresi için Warehouse zorunludur."));
            var loc = await _locations.FindByWarehouseAndCodeAsync(warehouseCode, locationCode, plantId, cancellationToken).ConfigureAwait(false);
            if (loc is null || loc.IsDeleted)
                return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                    "INV-BAL-019",
                    $"Lokasyon '{locationCode}' depo '{warehouseCode}' / tesis '{plantId}' altında tanımlı değil."));
            if (!string.Equals(loc.WarehouseCode, warehouseCode, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Validation(
                    "INV-BAL-020",
                    "Location seçilen depoya ait değil."));
            if (!string.IsNullOrWhiteSpace(loc.PlantId)
                && !string.Equals(loc.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<PagedInventoryBalanceDto>(Error.Forbidden(
                    "INV-BAL-403",
                    "Location seçili fabrikaya ait değil."));
            locationCode = loc.Code;
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(
            query.Q, page, pageSize, plantId, warehouseCode, locationCode, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedInventoryBalanceDto
        {
            Items = items.Select(InventoryBalanceMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetInventoryBalanceByIdQueryHandler : IQueryHandler<GetInventoryBalanceByIdQuery, Result<InventoryBalanceDto>>
{
    private readonly IInventoryBalanceRepository _repo;
    public GetInventoryBalanceByIdQueryHandler(IInventoryBalanceRepository repo) => _repo = repo;

    public async Task<Result<InventoryBalanceDto>> HandleAsync(
        GetInventoryBalanceByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<InventoryBalanceDto>(Error.NotFound("BUS-001", "InventoryBalance was not found."));

        if (query.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
        {
            return Result.Failure<InventoryBalanceDto>(Error.Forbidden(
                "INV-BAL-403",
                "Bu tesisin stok bakiyelerini görüntüleme yetkiniz yok."));
        }

        return Result.Success(InventoryBalanceMapper.ToDto(e));
    }
}

public sealed class CreateInventoryBalanceCommandHandler : ICommandHandler<CreateInventoryBalanceCommand, Result<InventoryBalanceDto>>
{
    public Task<Result<InventoryBalanceDto>> HandleAsync(CreateInventoryBalanceCommand command, CancellationToken cancellationToken = default)
    {
        _ = command;
        return Task.FromResult(Result.Failure<InventoryBalanceDto>(Error.Validation(
            "INV-BAL-001",
            "Inventory balances are ledger-owned. Post stock via goods-receipts/execute or goods-issues/execute.")));
    }
}

public sealed class UpdateInventoryBalanceCommandHandler : ICommandHandler<UpdateInventoryBalanceCommand, Result<InventoryBalanceDto>>
{
    public Task<Result<InventoryBalanceDto>> HandleAsync(UpdateInventoryBalanceCommand command, CancellationToken cancellationToken = default)
    {
        _ = command;
        return Task.FromResult(Result.Failure<InventoryBalanceDto>(Error.Validation(
            "INV-BAL-002",
            "Inventory balances cannot be edited directly. Use inventory transactions (execute) or adjustments workflow.")));
    }
}

public sealed class DeleteInventoryBalanceCommandHandler : ICommandHandler<DeleteInventoryBalanceCommand, Result>
{
    public Task<Result> HandleAsync(DeleteInventoryBalanceCommand command, CancellationToken cancellationToken = default)
    {
        _ = command;
        return Task.FromResult(Result.Failure(Error.Validation(
            "INV-BAL-003",
            "Inventory balances cannot be deleted directly. Reverse stock via inventory transactions.")));
    }
}

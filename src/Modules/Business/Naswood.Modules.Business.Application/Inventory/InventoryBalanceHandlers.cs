using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IInventoryBalanceRepository
{
    Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryBalance?> FindByKeyAsync(string materialCode, string warehouseCode, string locationCode, string batchNumber, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryBalance entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<InventoryBalance> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchInventoryBalanceQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedInventoryBalanceDto>>;
public sealed record GetInventoryBalanceByIdQuery(Guid Id) : IQuery<Result<InventoryBalanceDto>>;
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
    public SearchInventoryBalanceQueryHandler(IInventoryBalanceRepository repo) => _repo = repo;
    public async Task<Result<PagedInventoryBalanceDto>> HandleAsync(SearchInventoryBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedInventoryBalanceDto
        {
            Items = items.Select(InventoryBalanceMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetInventoryBalanceByIdQueryHandler : IQueryHandler<GetInventoryBalanceByIdQuery, Result<InventoryBalanceDto>>
{
    private readonly IInventoryBalanceRepository _repo;
    public GetInventoryBalanceByIdQueryHandler(IInventoryBalanceRepository repo) => _repo = repo;
    public async Task<Result<InventoryBalanceDto>> HandleAsync(GetInventoryBalanceByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<InventoryBalanceDto>(Error.NotFound("BUS-001", "InventoryBalance was not found."));
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

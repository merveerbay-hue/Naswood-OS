using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IStockTransferRepository
{
    Task<StockTransfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(StockTransfer entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<StockTransfer> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchStockTransferQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedStockTransferDto>>;
public sealed record GetStockTransferByIdQuery(Guid Id) : IQuery<Result<StockTransferDto>>;
public sealed record CreateStockTransferCommand(string Number, string FromWarehouseCode, string ToWarehouseCode, string Status, string Notes) : ICommand<Result<StockTransferDto>>;
public sealed record UpdateStockTransferCommand(Guid Id, string Number, string FromWarehouseCode, string ToWarehouseCode, string Status, string Notes) : ICommand<Result<StockTransferDto>>;
public sealed record DeleteStockTransferCommand(Guid Id) : ICommand<Result>;

public static class StockTransferMapper
{
    public static StockTransferDto ToDto(StockTransfer e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            FromWarehouseCode = e.FromWarehouseCode,
            ToWarehouseCode = e.ToWarehouseCode,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchStockTransferQueryHandler : IQueryHandler<SearchStockTransferQuery, Result<PagedStockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    public SearchStockTransferQueryHandler(IStockTransferRepository repo) => _repo = repo;
    public async Task<Result<PagedStockTransferDto>> HandleAsync(SearchStockTransferQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedStockTransferDto
        {
            Items = items.Select(StockTransferMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetStockTransferByIdQueryHandler : IQueryHandler<GetStockTransferByIdQuery, Result<StockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    public GetStockTransferByIdQueryHandler(IStockTransferRepository repo) => _repo = repo;
    public async Task<Result<StockTransferDto>> HandleAsync(GetStockTransferByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<StockTransferDto>(Error.NotFound("BUS-001", "StockTransfer was not found."));
        return Result.Success(StockTransferMapper.ToDto(e));
    }
}

public sealed class CreateStockTransferCommandHandler : ICommandHandler<CreateStockTransferCommand, Result<StockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateStockTransferCommandHandler(IStockTransferRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<StockTransferDto>> HandleAsync(CreateStockTransferCommand command, CancellationToken cancellationToken = default)
    {
        var e = StockTransfer.Create(command.Number, command.FromWarehouseCode, command.ToWarehouseCode, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StockTransferMapper.ToDto(e));
    }
}

public sealed class UpdateStockTransferCommandHandler : ICommandHandler<UpdateStockTransferCommand, Result<StockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateStockTransferCommandHandler(IStockTransferRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<StockTransferDto>> HandleAsync(UpdateStockTransferCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<StockTransferDto>(Error.NotFound("BUS-001", "StockTransfer was not found."));
        e.Update(command.Number, command.FromWarehouseCode, command.ToWarehouseCode, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StockTransferMapper.ToDto(e));
    }
}

public sealed class DeleteStockTransferCommandHandler : ICommandHandler<DeleteStockTransferCommand, Result>
{
    private readonly IStockTransferRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteStockTransferCommandHandler(IStockTransferRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteStockTransferCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "StockTransfer was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

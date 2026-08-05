using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IGoodsReceiptRepository
{
    Task<GoodsReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(GoodsReceipt entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<GoodsReceipt> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchGoodsReceiptQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedGoodsReceiptDto>>;
public sealed record GetGoodsReceiptByIdQuery(Guid Id) : IQuery<Result<GoodsReceiptDto>>;
public sealed record CreateGoodsReceiptCommand(string Number, string WarehouseCode, string Reference, string Status, string Notes) : ICommand<Result<GoodsReceiptDto>>;
public sealed record UpdateGoodsReceiptCommand(Guid Id, string Number, string WarehouseCode, string Reference, string Status, string Notes) : ICommand<Result<GoodsReceiptDto>>;
public sealed record DeleteGoodsReceiptCommand(Guid Id) : ICommand<Result>;

public static class GoodsReceiptMapper
{
    public static GoodsReceiptDto ToDto(GoodsReceipt e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            WarehouseCode = e.WarehouseCode,
            Reference = e.Reference,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchGoodsReceiptQueryHandler : IQueryHandler<SearchGoodsReceiptQuery, Result<PagedGoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    public SearchGoodsReceiptQueryHandler(IGoodsReceiptRepository repo) => _repo = repo;
    public async Task<Result<PagedGoodsReceiptDto>> HandleAsync(SearchGoodsReceiptQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedGoodsReceiptDto
        {
            Items = items.Select(GoodsReceiptMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetGoodsReceiptByIdQueryHandler : IQueryHandler<GetGoodsReceiptByIdQuery, Result<GoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    public GetGoodsReceiptByIdQueryHandler(IGoodsReceiptRepository repo) => _repo = repo;
    public async Task<Result<GoodsReceiptDto>> HandleAsync(GetGoodsReceiptByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<GoodsReceiptDto>(Error.NotFound("BUS-001", "GoodsReceipt was not found."));
        return Result.Success(GoodsReceiptMapper.ToDto(e));
    }
}

public sealed class CreateGoodsReceiptCommandHandler : ICommandHandler<CreateGoodsReceiptCommand, Result<GoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateGoodsReceiptCommandHandler(IGoodsReceiptRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<GoodsReceiptDto>> HandleAsync(CreateGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = GoodsReceipt.Create(command.Number, command.WarehouseCode, command.Reference, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsReceiptMapper.ToDto(e));
    }
}

public sealed class UpdateGoodsReceiptCommandHandler : ICommandHandler<UpdateGoodsReceiptCommand, Result<GoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateGoodsReceiptCommandHandler(IGoodsReceiptRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<GoodsReceiptDto>> HandleAsync(UpdateGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<GoodsReceiptDto>(Error.NotFound("BUS-001", "GoodsReceipt was not found."));
        e.Update(command.Number, command.WarehouseCode, command.Reference, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsReceiptMapper.ToDto(e));
    }
}

public sealed class DeleteGoodsReceiptCommandHandler : ICommandHandler<DeleteGoodsReceiptCommand, Result>
{
    private readonly IGoodsReceiptRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteGoodsReceiptCommandHandler(IGoodsReceiptRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "GoodsReceipt was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

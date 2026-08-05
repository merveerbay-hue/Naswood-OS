using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface IPurchaseGoodsReceiptRepository
{
    Task<PurchaseGoodsReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PurchaseGoodsReceipt entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<PurchaseGoodsReceipt> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchPurchaseGoodsReceiptQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedPurchaseGoodsReceiptDto>>;
public sealed record GetPurchaseGoodsReceiptByIdQuery(Guid Id) : IQuery<Result<PurchaseGoodsReceiptDto>>;
public sealed record CreatePurchaseGoodsReceiptCommand(string Number, string PurchaseOrderNumber, string WarehouseCode, string Status, string Notes) : ICommand<Result<PurchaseGoodsReceiptDto>>;
public sealed record UpdatePurchaseGoodsReceiptCommand(Guid Id, string Number, string PurchaseOrderNumber, string WarehouseCode, string Status, string Notes) : ICommand<Result<PurchaseGoodsReceiptDto>>;
public sealed record DeletePurchaseGoodsReceiptCommand(Guid Id) : ICommand<Result>;

public static class PurchaseGoodsReceiptMapper
{
    public static PurchaseGoodsReceiptDto ToDto(PurchaseGoodsReceipt e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            PurchaseOrderNumber = e.PurchaseOrderNumber,
            WarehouseCode = e.WarehouseCode,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchPurchaseGoodsReceiptQueryHandler : IQueryHandler<SearchPurchaseGoodsReceiptQuery, Result<PagedPurchaseGoodsReceiptDto>>
{
    private readonly IPurchaseGoodsReceiptRepository _repo;
    public SearchPurchaseGoodsReceiptQueryHandler(IPurchaseGoodsReceiptRepository repo) => _repo = repo;
    public async Task<Result<PagedPurchaseGoodsReceiptDto>> HandleAsync(SearchPurchaseGoodsReceiptQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedPurchaseGoodsReceiptDto
        {
            Items = items.Select(PurchaseGoodsReceiptMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetPurchaseGoodsReceiptByIdQueryHandler : IQueryHandler<GetPurchaseGoodsReceiptByIdQuery, Result<PurchaseGoodsReceiptDto>>
{
    private readonly IPurchaseGoodsReceiptRepository _repo;
    public GetPurchaseGoodsReceiptByIdQueryHandler(IPurchaseGoodsReceiptRepository repo) => _repo = repo;
    public async Task<Result<PurchaseGoodsReceiptDto>> HandleAsync(GetPurchaseGoodsReceiptByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseGoodsReceiptDto>(Error.NotFound("BUS-001", "PurchaseGoodsReceipt was not found."));
        return Result.Success(PurchaseGoodsReceiptMapper.ToDto(e));
    }
}

public sealed class CreatePurchaseGoodsReceiptCommandHandler : ICommandHandler<CreatePurchaseGoodsReceiptCommand, Result<PurchaseGoodsReceiptDto>>
{
    private readonly IPurchaseGoodsReceiptRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreatePurchaseGoodsReceiptCommandHandler(IPurchaseGoodsReceiptRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseGoodsReceiptDto>> HandleAsync(CreatePurchaseGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = PurchaseGoodsReceipt.Create(command.Number, command.PurchaseOrderNumber, command.WarehouseCode, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseGoodsReceiptMapper.ToDto(e));
    }
}

public sealed class UpdatePurchaseGoodsReceiptCommandHandler : ICommandHandler<UpdatePurchaseGoodsReceiptCommand, Result<PurchaseGoodsReceiptDto>>
{
    private readonly IPurchaseGoodsReceiptRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdatePurchaseGoodsReceiptCommandHandler(IPurchaseGoodsReceiptRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseGoodsReceiptDto>> HandleAsync(UpdatePurchaseGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseGoodsReceiptDto>(Error.NotFound("BUS-001", "PurchaseGoodsReceipt was not found."));
        e.Update(command.Number, command.PurchaseOrderNumber, command.WarehouseCode, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseGoodsReceiptMapper.ToDto(e));
    }
}

public sealed class DeletePurchaseGoodsReceiptCommandHandler : ICommandHandler<DeletePurchaseGoodsReceiptCommand, Result>
{
    private readonly IPurchaseGoodsReceiptRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeletePurchaseGoodsReceiptCommandHandler(IPurchaseGoodsReceiptRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeletePurchaseGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "PurchaseGoodsReceipt was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

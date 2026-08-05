using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<PurchaseOrder> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchPurchaseOrderQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedPurchaseOrderDto>>;
public sealed record GetPurchaseOrderByIdQuery(Guid Id) : IQuery<Result<PurchaseOrderDto>>;
public sealed record CreatePurchaseOrderCommand(string Number, string SupplierCode, DateOnly? OrderDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<PurchaseOrderDto>>;
public sealed record UpdatePurchaseOrderCommand(Guid Id, string Number, string SupplierCode, DateOnly? OrderDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<PurchaseOrderDto>>;
public sealed record DeletePurchaseOrderCommand(Guid Id) : ICommand<Result>;

public static class PurchaseOrderMapper
{
    public static PurchaseOrderDto ToDto(PurchaseOrder e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            SupplierCode = e.SupplierCode,
            OrderDate = e.OrderDate,
            TotalAmount = e.TotalAmount,
            Currency = e.Currency,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchPurchaseOrderQueryHandler : IQueryHandler<SearchPurchaseOrderQuery, Result<PagedPurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repo;
    public SearchPurchaseOrderQueryHandler(IPurchaseOrderRepository repo) => _repo = repo;
    public async Task<Result<PagedPurchaseOrderDto>> HandleAsync(SearchPurchaseOrderQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedPurchaseOrderDto
        {
            Items = items.Select(PurchaseOrderMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetPurchaseOrderByIdQueryHandler : IQueryHandler<GetPurchaseOrderByIdQuery, Result<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repo;
    public GetPurchaseOrderByIdQueryHandler(IPurchaseOrderRepository repo) => _repo = repo;
    public async Task<Result<PurchaseOrderDto>> HandleAsync(GetPurchaseOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseOrderDto>(Error.NotFound("BUS-001", "PurchaseOrder was not found."));
        return Result.Success(PurchaseOrderMapper.ToDto(e));
    }
}

public sealed class CreatePurchaseOrderCommandHandler : ICommandHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreatePurchaseOrderCommandHandler(IPurchaseOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseOrderDto>> HandleAsync(CreatePurchaseOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = PurchaseOrder.Create(command.Number, command.SupplierCode, command.OrderDate, command.TotalAmount, command.Currency, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseOrderMapper.ToDto(e));
    }
}

public sealed class UpdatePurchaseOrderCommandHandler : ICommandHandler<UpdatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdatePurchaseOrderCommandHandler(IPurchaseOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseOrderDto>> HandleAsync(UpdatePurchaseOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseOrderDto>(Error.NotFound("BUS-001", "PurchaseOrder was not found."));
        e.Update(command.Number, command.SupplierCode, command.OrderDate, command.TotalAmount, command.Currency, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseOrderMapper.ToDto(e));
    }
}

public sealed class DeletePurchaseOrderCommandHandler : ICommandHandler<DeletePurchaseOrderCommand, Result>
{
    private readonly IPurchaseOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeletePurchaseOrderCommandHandler(IPurchaseOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeletePurchaseOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "PurchaseOrder was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

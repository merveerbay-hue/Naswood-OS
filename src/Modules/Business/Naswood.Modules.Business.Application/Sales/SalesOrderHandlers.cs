using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(SalesOrder entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SalesOrder> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchSalesOrderQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedSalesOrderDto>>;
public sealed record GetSalesOrderByIdQuery(Guid Id) : IQuery<Result<SalesOrderDto>>;
public sealed record CreateSalesOrderCommand(string Number, string CustomerCode, DateOnly? OrderDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SalesOrderDto>>;
public sealed record UpdateSalesOrderCommand(Guid Id, string Number, string CustomerCode, DateOnly? OrderDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SalesOrderDto>>;
public sealed record DeleteSalesOrderCommand(Guid Id) : ICommand<Result>;

public static class SalesOrderMapper
{
    public static SalesOrderDto ToDto(SalesOrder e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            CustomerCode = e.CustomerCode,
            OrderDate = e.OrderDate,
            TotalAmount = e.TotalAmount,
            Currency = e.Currency,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchSalesOrderQueryHandler : IQueryHandler<SearchSalesOrderQuery, Result<PagedSalesOrderDto>>
{
    private readonly ISalesOrderRepository _repo;
    public SearchSalesOrderQueryHandler(ISalesOrderRepository repo) => _repo = repo;
    public async Task<Result<PagedSalesOrderDto>> HandleAsync(SearchSalesOrderQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedSalesOrderDto
        {
            Items = items.Select(SalesOrderMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetSalesOrderByIdQueryHandler : IQueryHandler<GetSalesOrderByIdQuery, Result<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _repo;
    public GetSalesOrderByIdQueryHandler(ISalesOrderRepository repo) => _repo = repo;
    public async Task<Result<SalesOrderDto>> HandleAsync(GetSalesOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SalesOrderDto>(Error.NotFound("BUS-001", "SalesOrder was not found."));
        return Result.Success(SalesOrderMapper.ToDto(e));
    }
}

public sealed class CreateSalesOrderCommandHandler : ICommandHandler<CreateSalesOrderCommand, Result<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateSalesOrderCommandHandler(ISalesOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SalesOrderDto>> HandleAsync(CreateSalesOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = SalesOrder.Create(command.Number, command.CustomerCode, command.OrderDate, command.TotalAmount, command.Currency, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SalesOrderMapper.ToDto(e));
    }
}

public sealed class UpdateSalesOrderCommandHandler : ICommandHandler<UpdateSalesOrderCommand, Result<SalesOrderDto>>
{
    private readonly ISalesOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateSalesOrderCommandHandler(ISalesOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SalesOrderDto>> HandleAsync(UpdateSalesOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SalesOrderDto>(Error.NotFound("BUS-001", "SalesOrder was not found."));
        e.Update(command.Number, command.CustomerCode, command.OrderDate, command.TotalAmount, command.Currency, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SalesOrderMapper.ToDto(e));
    }
}

public sealed class DeleteSalesOrderCommandHandler : ICommandHandler<DeleteSalesOrderCommand, Result>
{
    private readonly ISalesOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteSalesOrderCommandHandler(ISalesOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteSalesOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "SalesOrder was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

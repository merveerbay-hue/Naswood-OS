using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface ICustomerInvoiceRepository
{
    Task<CustomerInvoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(CustomerInvoice entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<CustomerInvoice> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchCustomerInvoiceQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedCustomerInvoiceDto>>;
public sealed record GetCustomerInvoiceByIdQuery(Guid Id) : IQuery<Result<CustomerInvoiceDto>>;
public sealed record CreateCustomerInvoiceCommand(string Number, string CustomerCode, DateOnly? InvoiceDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<CustomerInvoiceDto>>;
public sealed record UpdateCustomerInvoiceCommand(Guid Id, string Number, string CustomerCode, DateOnly? InvoiceDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<CustomerInvoiceDto>>;
public sealed record DeleteCustomerInvoiceCommand(Guid Id) : ICommand<Result>;

public static class CustomerInvoiceMapper
{
    public static CustomerInvoiceDto ToDto(CustomerInvoice e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            CustomerCode = e.CustomerCode,
            InvoiceDate = e.InvoiceDate,
            TotalAmount = e.TotalAmount,
            Currency = e.Currency,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchCustomerInvoiceQueryHandler : IQueryHandler<SearchCustomerInvoiceQuery, Result<PagedCustomerInvoiceDto>>
{
    private readonly ICustomerInvoiceRepository _repo;
    public SearchCustomerInvoiceQueryHandler(ICustomerInvoiceRepository repo) => _repo = repo;
    public async Task<Result<PagedCustomerInvoiceDto>> HandleAsync(SearchCustomerInvoiceQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedCustomerInvoiceDto
        {
            Items = items.Select(CustomerInvoiceMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetCustomerInvoiceByIdQueryHandler : IQueryHandler<GetCustomerInvoiceByIdQuery, Result<CustomerInvoiceDto>>
{
    private readonly ICustomerInvoiceRepository _repo;
    public GetCustomerInvoiceByIdQueryHandler(ICustomerInvoiceRepository repo) => _repo = repo;
    public async Task<Result<CustomerInvoiceDto>> HandleAsync(GetCustomerInvoiceByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<CustomerInvoiceDto>(Error.NotFound("BUS-001", "CustomerInvoice was not found."));
        return Result.Success(CustomerInvoiceMapper.ToDto(e));
    }
}

public sealed class CreateCustomerInvoiceCommandHandler : ICommandHandler<CreateCustomerInvoiceCommand, Result<CustomerInvoiceDto>>
{
    private readonly ICustomerInvoiceRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateCustomerInvoiceCommandHandler(ICustomerInvoiceRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<CustomerInvoiceDto>> HandleAsync(CreateCustomerInvoiceCommand command, CancellationToken cancellationToken = default)
    {
        var e = CustomerInvoice.Create(command.Number, command.CustomerCode, command.InvoiceDate, command.TotalAmount, command.Currency, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(CustomerInvoiceMapper.ToDto(e));
    }
}

public sealed class UpdateCustomerInvoiceCommandHandler : ICommandHandler<UpdateCustomerInvoiceCommand, Result<CustomerInvoiceDto>>
{
    private readonly ICustomerInvoiceRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateCustomerInvoiceCommandHandler(ICustomerInvoiceRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<CustomerInvoiceDto>> HandleAsync(UpdateCustomerInvoiceCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<CustomerInvoiceDto>(Error.NotFound("BUS-001", "CustomerInvoice was not found."));
        e.Update(command.Number, command.CustomerCode, command.InvoiceDate, command.TotalAmount, command.Currency, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(CustomerInvoiceMapper.ToDto(e));
    }
}

public sealed class DeleteCustomerInvoiceCommandHandler : ICommandHandler<DeleteCustomerInvoiceCommand, Result>
{
    private readonly ICustomerInvoiceRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteCustomerInvoiceCommandHandler(ICustomerInvoiceRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteCustomerInvoiceCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "CustomerInvoice was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

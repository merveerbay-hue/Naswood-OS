using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Customer entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Customer> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchCustomerQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedCustomerDto>>;
public sealed record GetCustomerByIdQuery(Guid Id) : IQuery<Result<CustomerDto>>;
public sealed record CreateCustomerCommand(string Code, string Name, string TaxNumber, string Email, string Phone, string Status) : ICommand<Result<CustomerDto>>;
public sealed record UpdateCustomerCommand(Guid Id, string Code, string Name, string TaxNumber, string Email, string Phone, string Status) : ICommand<Result<CustomerDto>>;
public sealed record DeleteCustomerCommand(Guid Id) : ICommand<Result>;

public static class CustomerMapper
{
    public static CustomerDto ToDto(Customer e) => new()
    {
        Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            TaxNumber = e.TaxNumber,
            Email = e.Email,
            Phone = e.Phone,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchCustomerQueryHandler : IQueryHandler<SearchCustomerQuery, Result<PagedCustomerDto>>
{
    private readonly ICustomerRepository _repo;
    public SearchCustomerQueryHandler(ICustomerRepository repo) => _repo = repo;
    public async Task<Result<PagedCustomerDto>> HandleAsync(SearchCustomerQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedCustomerDto
        {
            Items = items.Select(CustomerMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repo;
    public GetCustomerByIdQueryHandler(ICustomerRepository repo) => _repo = repo;
    public async Task<Result<CustomerDto>> HandleAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<CustomerDto>(Error.NotFound("BUS-001", "Customer was not found."));
        return Result.Success(CustomerMapper.ToDto(e));
    }
}

public sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateCustomerCommandHandler(ICustomerRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<CustomerDto>> HandleAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var e = Customer.Create(command.Code, command.Name, command.TaxNumber, command.Email, command.Phone, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(CustomerMapper.ToDto(e));
    }
}

public sealed class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateCustomerCommandHandler(ICustomerRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<CustomerDto>> HandleAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<CustomerDto>(Error.NotFound("BUS-001", "Customer was not found."));
        e.Update(command.Code, command.Name, command.TaxNumber, command.Email, command.Phone, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(CustomerMapper.ToDto(e));
    }
}

public sealed class DeleteCustomerCommandHandler : ICommandHandler<DeleteCustomerCommand, Result>
{
    private readonly ICustomerRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteCustomerCommandHandler(ICustomerRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Customer was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

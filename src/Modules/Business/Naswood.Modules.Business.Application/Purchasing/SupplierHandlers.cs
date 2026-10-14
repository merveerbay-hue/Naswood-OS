using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Supplier entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Supplier> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchSupplierQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedSupplierDto>>;
public sealed record GetSupplierByIdQuery(Guid Id) : IQuery<Result<SupplierDto>>;
public sealed record CreateSupplierCommand(string Code, string Name, string TaxNumber, string Email, string Phone, string Status) : ICommand<Result<SupplierDto>>;
public sealed record UpdateSupplierCommand(Guid Id, string Code, string Name, string TaxNumber, string Email, string Phone, string Status) : ICommand<Result<SupplierDto>>;
public sealed record DeleteSupplierCommand(Guid Id) : ICommand<Result>;

public static class SupplierMapper
{
    public static SupplierDto ToDto(Supplier e) => new()
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

public sealed class SearchSupplierQueryHandler : IQueryHandler<SearchSupplierQuery, Result<PagedSupplierDto>>
{
    private readonly ISupplierRepository _repo;
    public SearchSupplierQueryHandler(ISupplierRepository repo) => _repo = repo;
    public async Task<Result<PagedSupplierDto>> HandleAsync(SearchSupplierQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedSupplierDto
        {
            Items = items.Select(SupplierMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetSupplierByIdQueryHandler : IQueryHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    private readonly ISupplierRepository _repo;
    public GetSupplierByIdQueryHandler(ISupplierRepository repo) => _repo = repo;
    public async Task<Result<SupplierDto>> HandleAsync(GetSupplierByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SupplierDto>(Error.NotFound("BUS-001", "Supplier was not found."));
        return Result.Success(SupplierMapper.ToDto(e));
    }
}

public sealed class CreateSupplierCommandHandler : ICommandHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    private readonly ISupplierRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateSupplierCommandHandler(ISupplierRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SupplierDto>> HandleAsync(CreateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var e = Supplier.Create(command.Code, command.Name, command.TaxNumber, command.Email, command.Phone, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SupplierMapper.ToDto(e));
    }
}

public sealed class UpdateSupplierCommandHandler : ICommandHandler<UpdateSupplierCommand, Result<SupplierDto>>
{
    private readonly ISupplierRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateSupplierCommandHandler(ISupplierRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SupplierDto>> HandleAsync(UpdateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SupplierDto>(Error.NotFound("BUS-001", "Supplier was not found."));
        e.Update(command.Code, command.Name, command.TaxNumber, command.Email, command.Phone, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SupplierMapper.ToDto(e));
    }
}

public sealed class DeleteSupplierCommandHandler : ICommandHandler<DeleteSupplierCommand, Result>
{
    private readonly ISupplierRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteSupplierCommandHandler(ISupplierRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Supplier was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

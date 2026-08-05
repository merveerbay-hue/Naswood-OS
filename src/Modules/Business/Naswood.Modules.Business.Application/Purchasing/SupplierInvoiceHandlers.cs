using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface ISupplierInvoiceRepository
{
    Task<SupplierInvoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(SupplierInvoice entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SupplierInvoice> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchSupplierInvoiceQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedSupplierInvoiceDto>>;
public sealed record GetSupplierInvoiceByIdQuery(Guid Id) : IQuery<Result<SupplierInvoiceDto>>;
public sealed record CreateSupplierInvoiceCommand(string Number, string SupplierCode, DateOnly? InvoiceDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SupplierInvoiceDto>>;
public sealed record UpdateSupplierInvoiceCommand(Guid Id, string Number, string SupplierCode, DateOnly? InvoiceDate, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SupplierInvoiceDto>>;
public sealed record DeleteSupplierInvoiceCommand(Guid Id) : ICommand<Result>;

public static class SupplierInvoiceMapper
{
    public static SupplierInvoiceDto ToDto(SupplierInvoice e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            SupplierCode = e.SupplierCode,
            InvoiceDate = e.InvoiceDate,
            TotalAmount = e.TotalAmount,
            Currency = e.Currency,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchSupplierInvoiceQueryHandler : IQueryHandler<SearchSupplierInvoiceQuery, Result<PagedSupplierInvoiceDto>>
{
    private readonly ISupplierInvoiceRepository _repo;
    public SearchSupplierInvoiceQueryHandler(ISupplierInvoiceRepository repo) => _repo = repo;
    public async Task<Result<PagedSupplierInvoiceDto>> HandleAsync(SearchSupplierInvoiceQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedSupplierInvoiceDto
        {
            Items = items.Select(SupplierInvoiceMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetSupplierInvoiceByIdQueryHandler : IQueryHandler<GetSupplierInvoiceByIdQuery, Result<SupplierInvoiceDto>>
{
    private readonly ISupplierInvoiceRepository _repo;
    public GetSupplierInvoiceByIdQueryHandler(ISupplierInvoiceRepository repo) => _repo = repo;
    public async Task<Result<SupplierInvoiceDto>> HandleAsync(GetSupplierInvoiceByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SupplierInvoiceDto>(Error.NotFound("BUS-001", "SupplierInvoice was not found."));
        return Result.Success(SupplierInvoiceMapper.ToDto(e));
    }
}

public sealed class CreateSupplierInvoiceCommandHandler : ICommandHandler<CreateSupplierInvoiceCommand, Result<SupplierInvoiceDto>>
{
    private readonly ISupplierInvoiceRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateSupplierInvoiceCommandHandler(ISupplierInvoiceRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SupplierInvoiceDto>> HandleAsync(CreateSupplierInvoiceCommand command, CancellationToken cancellationToken = default)
    {
        var e = SupplierInvoice.Create(command.Number, command.SupplierCode, command.InvoiceDate, command.TotalAmount, command.Currency, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SupplierInvoiceMapper.ToDto(e));
    }
}

public sealed class UpdateSupplierInvoiceCommandHandler : ICommandHandler<UpdateSupplierInvoiceCommand, Result<SupplierInvoiceDto>>
{
    private readonly ISupplierInvoiceRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateSupplierInvoiceCommandHandler(ISupplierInvoiceRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SupplierInvoiceDto>> HandleAsync(UpdateSupplierInvoiceCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SupplierInvoiceDto>(Error.NotFound("BUS-001", "SupplierInvoice was not found."));
        e.Update(command.Number, command.SupplierCode, command.InvoiceDate, command.TotalAmount, command.Currency, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SupplierInvoiceMapper.ToDto(e));
    }
}

public sealed class DeleteSupplierInvoiceCommandHandler : ICommandHandler<DeleteSupplierInvoiceCommand, Result>
{
    private readonly ISupplierInvoiceRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteSupplierInvoiceCommandHandler(ISupplierInvoiceRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteSupplierInvoiceCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "SupplierInvoice was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

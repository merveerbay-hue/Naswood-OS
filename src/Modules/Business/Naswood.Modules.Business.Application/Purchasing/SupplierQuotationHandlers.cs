using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface ISupplierQuotationRepository
{
    Task<SupplierQuotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(SupplierQuotation entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SupplierQuotation> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchSupplierQuotationQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedSupplierQuotationDto>>;
public sealed record GetSupplierQuotationByIdQuery(Guid Id) : IQuery<Result<SupplierQuotationDto>>;
public sealed record CreateSupplierQuotationCommand(string Number, string SupplierCode, string RfqNumber, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SupplierQuotationDto>>;
public sealed record UpdateSupplierQuotationCommand(Guid Id, string Number, string SupplierCode, string RfqNumber, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SupplierQuotationDto>>;
public sealed record DeleteSupplierQuotationCommand(Guid Id) : ICommand<Result>;

public static class SupplierQuotationMapper
{
    public static SupplierQuotationDto ToDto(SupplierQuotation e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            SupplierCode = e.SupplierCode,
            RfqNumber = e.RfqNumber,
            TotalAmount = e.TotalAmount,
            Currency = e.Currency,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchSupplierQuotationQueryHandler : IQueryHandler<SearchSupplierQuotationQuery, Result<PagedSupplierQuotationDto>>
{
    private readonly ISupplierQuotationRepository _repo;
    public SearchSupplierQuotationQueryHandler(ISupplierQuotationRepository repo) => _repo = repo;
    public async Task<Result<PagedSupplierQuotationDto>> HandleAsync(SearchSupplierQuotationQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedSupplierQuotationDto
        {
            Items = items.Select(SupplierQuotationMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetSupplierQuotationByIdQueryHandler : IQueryHandler<GetSupplierQuotationByIdQuery, Result<SupplierQuotationDto>>
{
    private readonly ISupplierQuotationRepository _repo;
    public GetSupplierQuotationByIdQueryHandler(ISupplierQuotationRepository repo) => _repo = repo;
    public async Task<Result<SupplierQuotationDto>> HandleAsync(GetSupplierQuotationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SupplierQuotationDto>(Error.NotFound("BUS-001", "SupplierQuotation was not found."));
        return Result.Success(SupplierQuotationMapper.ToDto(e));
    }
}

public sealed class CreateSupplierQuotationCommandHandler : ICommandHandler<CreateSupplierQuotationCommand, Result<SupplierQuotationDto>>
{
    private readonly ISupplierQuotationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateSupplierQuotationCommandHandler(ISupplierQuotationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SupplierQuotationDto>> HandleAsync(CreateSupplierQuotationCommand command, CancellationToken cancellationToken = default)
    {
        var e = SupplierQuotation.Create(command.Number, command.SupplierCode, command.RfqNumber, command.TotalAmount, command.Currency, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SupplierQuotationMapper.ToDto(e));
    }
}

public sealed class UpdateSupplierQuotationCommandHandler : ICommandHandler<UpdateSupplierQuotationCommand, Result<SupplierQuotationDto>>
{
    private readonly ISupplierQuotationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateSupplierQuotationCommandHandler(ISupplierQuotationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SupplierQuotationDto>> HandleAsync(UpdateSupplierQuotationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SupplierQuotationDto>(Error.NotFound("BUS-001", "SupplierQuotation was not found."));
        e.Update(command.Number, command.SupplierCode, command.RfqNumber, command.TotalAmount, command.Currency, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SupplierQuotationMapper.ToDto(e));
    }
}

public sealed class DeleteSupplierQuotationCommandHandler : ICommandHandler<DeleteSupplierQuotationCommand, Result>
{
    private readonly ISupplierQuotationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteSupplierQuotationCommandHandler(ISupplierQuotationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteSupplierQuotationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "SupplierQuotation was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

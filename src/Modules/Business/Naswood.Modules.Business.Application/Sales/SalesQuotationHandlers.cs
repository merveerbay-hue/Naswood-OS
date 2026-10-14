using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface ISalesQuotationRepository
{
    Task<SalesQuotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(SalesQuotation entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SalesQuotation> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchSalesQuotationQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedSalesQuotationDto>>;
public sealed record GetSalesQuotationByIdQuery(Guid Id) : IQuery<Result<SalesQuotationDto>>;
public sealed record CreateSalesQuotationCommand(string Number, string CustomerCode, DateOnly? ValidUntil, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SalesQuotationDto>>;
public sealed record UpdateSalesQuotationCommand(Guid Id, string Number, string CustomerCode, DateOnly? ValidUntil, decimal TotalAmount, string Currency, string Status) : ICommand<Result<SalesQuotationDto>>;
public sealed record DeleteSalesQuotationCommand(Guid Id) : ICommand<Result>;

public static class SalesQuotationMapper
{
    public static SalesQuotationDto ToDto(SalesQuotation e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            CustomerCode = e.CustomerCode,
            ValidUntil = e.ValidUntil,
            TotalAmount = e.TotalAmount,
            Currency = e.Currency,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchSalesQuotationQueryHandler : IQueryHandler<SearchSalesQuotationQuery, Result<PagedSalesQuotationDto>>
{
    private readonly ISalesQuotationRepository _repo;
    public SearchSalesQuotationQueryHandler(ISalesQuotationRepository repo) => _repo = repo;
    public async Task<Result<PagedSalesQuotationDto>> HandleAsync(SearchSalesQuotationQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedSalesQuotationDto
        {
            Items = items.Select(SalesQuotationMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetSalesQuotationByIdQueryHandler : IQueryHandler<GetSalesQuotationByIdQuery, Result<SalesQuotationDto>>
{
    private readonly ISalesQuotationRepository _repo;
    public GetSalesQuotationByIdQueryHandler(ISalesQuotationRepository repo) => _repo = repo;
    public async Task<Result<SalesQuotationDto>> HandleAsync(GetSalesQuotationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SalesQuotationDto>(Error.NotFound("BUS-001", "SalesQuotation was not found."));
        return Result.Success(SalesQuotationMapper.ToDto(e));
    }
}

public sealed class CreateSalesQuotationCommandHandler : ICommandHandler<CreateSalesQuotationCommand, Result<SalesQuotationDto>>
{
    private readonly ISalesQuotationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateSalesQuotationCommandHandler(ISalesQuotationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SalesQuotationDto>> HandleAsync(CreateSalesQuotationCommand command, CancellationToken cancellationToken = default)
    {
        var e = SalesQuotation.Create(command.Number, command.CustomerCode, command.ValidUntil, command.TotalAmount, command.Currency, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SalesQuotationMapper.ToDto(e));
    }
}

public sealed class UpdateSalesQuotationCommandHandler : ICommandHandler<UpdateSalesQuotationCommand, Result<SalesQuotationDto>>
{
    private readonly ISalesQuotationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateSalesQuotationCommandHandler(ISalesQuotationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SalesQuotationDto>> HandleAsync(UpdateSalesQuotationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SalesQuotationDto>(Error.NotFound("BUS-001", "SalesQuotation was not found."));
        e.Update(command.Number, command.CustomerCode, command.ValidUntil, command.TotalAmount, command.Currency, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SalesQuotationMapper.ToDto(e));
    }
}

public sealed class DeleteSalesQuotationCommandHandler : ICommandHandler<DeleteSalesQuotationCommand, Result>
{
    private readonly ISalesQuotationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteSalesQuotationCommandHandler(ISalesQuotationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteSalesQuotationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "SalesQuotation was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

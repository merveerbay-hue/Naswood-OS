using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface IPurchaseRequestRepository
{
    Task<PurchaseRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PurchaseRequest entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<PurchaseRequest> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchPurchaseRequestQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedPurchaseRequestDto>>;
public sealed record GetPurchaseRequestByIdQuery(Guid Id) : IQuery<Result<PurchaseRequestDto>>;
public sealed record CreatePurchaseRequestCommand(string Number, string Requester, DateOnly? NeededDate, string Status, string Notes) : ICommand<Result<PurchaseRequestDto>>;
public sealed record UpdatePurchaseRequestCommand(Guid Id, string Number, string Requester, DateOnly? NeededDate, string Status, string Notes) : ICommand<Result<PurchaseRequestDto>>;
public sealed record DeletePurchaseRequestCommand(Guid Id) : ICommand<Result>;

public static class PurchaseRequestMapper
{
    public static PurchaseRequestDto ToDto(PurchaseRequest e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            Requester = e.Requester,
            NeededDate = e.NeededDate,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchPurchaseRequestQueryHandler : IQueryHandler<SearchPurchaseRequestQuery, Result<PagedPurchaseRequestDto>>
{
    private readonly IPurchaseRequestRepository _repo;
    public SearchPurchaseRequestQueryHandler(IPurchaseRequestRepository repo) => _repo = repo;
    public async Task<Result<PagedPurchaseRequestDto>> HandleAsync(SearchPurchaseRequestQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedPurchaseRequestDto
        {
            Items = items.Select(PurchaseRequestMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetPurchaseRequestByIdQueryHandler : IQueryHandler<GetPurchaseRequestByIdQuery, Result<PurchaseRequestDto>>
{
    private readonly IPurchaseRequestRepository _repo;
    public GetPurchaseRequestByIdQueryHandler(IPurchaseRequestRepository repo) => _repo = repo;
    public async Task<Result<PurchaseRequestDto>> HandleAsync(GetPurchaseRequestByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseRequestDto>(Error.NotFound("BUS-001", "PurchaseRequest was not found."));
        return Result.Success(PurchaseRequestMapper.ToDto(e));
    }
}

public sealed class CreatePurchaseRequestCommandHandler : ICommandHandler<CreatePurchaseRequestCommand, Result<PurchaseRequestDto>>
{
    private readonly IPurchaseRequestRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreatePurchaseRequestCommandHandler(IPurchaseRequestRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseRequestDto>> HandleAsync(CreatePurchaseRequestCommand command, CancellationToken cancellationToken = default)
    {
        var e = PurchaseRequest.Create(command.Number, command.Requester, command.NeededDate, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseRequestMapper.ToDto(e));
    }
}

public sealed class UpdatePurchaseRequestCommandHandler : ICommandHandler<UpdatePurchaseRequestCommand, Result<PurchaseRequestDto>>
{
    private readonly IPurchaseRequestRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdatePurchaseRequestCommandHandler(IPurchaseRequestRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseRequestDto>> HandleAsync(UpdatePurchaseRequestCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseRequestDto>(Error.NotFound("BUS-001", "PurchaseRequest was not found."));
        e.Update(command.Number, command.Requester, command.NeededDate, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseRequestMapper.ToDto(e));
    }
}

public sealed class DeletePurchaseRequestCommandHandler : ICommandHandler<DeletePurchaseRequestCommand, Result>
{
    private readonly IPurchaseRequestRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeletePurchaseRequestCommandHandler(IPurchaseRequestRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeletePurchaseRequestCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "PurchaseRequest was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

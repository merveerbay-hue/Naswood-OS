using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface IPurchaseReturnRepository
{
    Task<PurchaseReturn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PurchaseReturn entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<PurchaseReturn> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchPurchaseReturnQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedPurchaseReturnDto>>;
public sealed record GetPurchaseReturnByIdQuery(Guid Id) : IQuery<Result<PurchaseReturnDto>>;
public sealed record CreatePurchaseReturnCommand(string Number, string SupplierCode, string PurchaseOrderNumber, string Status, string Notes) : ICommand<Result<PurchaseReturnDto>>;
public sealed record UpdatePurchaseReturnCommand(Guid Id, string Number, string SupplierCode, string PurchaseOrderNumber, string Status, string Notes) : ICommand<Result<PurchaseReturnDto>>;
public sealed record DeletePurchaseReturnCommand(Guid Id) : ICommand<Result>;

public static class PurchaseReturnMapper
{
    public static PurchaseReturnDto ToDto(PurchaseReturn e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            SupplierCode = e.SupplierCode,
            PurchaseOrderNumber = e.PurchaseOrderNumber,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchPurchaseReturnQueryHandler : IQueryHandler<SearchPurchaseReturnQuery, Result<PagedPurchaseReturnDto>>
{
    private readonly IPurchaseReturnRepository _repo;
    public SearchPurchaseReturnQueryHandler(IPurchaseReturnRepository repo) => _repo = repo;
    public async Task<Result<PagedPurchaseReturnDto>> HandleAsync(SearchPurchaseReturnQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedPurchaseReturnDto
        {
            Items = items.Select(PurchaseReturnMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetPurchaseReturnByIdQueryHandler : IQueryHandler<GetPurchaseReturnByIdQuery, Result<PurchaseReturnDto>>
{
    private readonly IPurchaseReturnRepository _repo;
    public GetPurchaseReturnByIdQueryHandler(IPurchaseReturnRepository repo) => _repo = repo;
    public async Task<Result<PurchaseReturnDto>> HandleAsync(GetPurchaseReturnByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseReturnDto>(Error.NotFound("BUS-001", "PurchaseReturn was not found."));
        return Result.Success(PurchaseReturnMapper.ToDto(e));
    }
}

public sealed class CreatePurchaseReturnCommandHandler : ICommandHandler<CreatePurchaseReturnCommand, Result<PurchaseReturnDto>>
{
    private readonly IPurchaseReturnRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreatePurchaseReturnCommandHandler(IPurchaseReturnRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseReturnDto>> HandleAsync(CreatePurchaseReturnCommand command, CancellationToken cancellationToken = default)
    {
        var e = PurchaseReturn.Create(command.Number, command.SupplierCode, command.PurchaseOrderNumber, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseReturnMapper.ToDto(e));
    }
}

public sealed class UpdatePurchaseReturnCommandHandler : ICommandHandler<UpdatePurchaseReturnCommand, Result<PurchaseReturnDto>>
{
    private readonly IPurchaseReturnRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdatePurchaseReturnCommandHandler(IPurchaseReturnRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchaseReturnDto>> HandleAsync(UpdatePurchaseReturnCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchaseReturnDto>(Error.NotFound("BUS-001", "PurchaseReturn was not found."));
        e.Update(command.Number, command.SupplierCode, command.PurchaseOrderNumber, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchaseReturnMapper.ToDto(e));
    }
}

public sealed class DeletePurchaseReturnCommandHandler : ICommandHandler<DeletePurchaseReturnCommand, Result>
{
    private readonly IPurchaseReturnRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeletePurchaseReturnCommandHandler(IPurchaseReturnRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeletePurchaseReturnCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "PurchaseReturn was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

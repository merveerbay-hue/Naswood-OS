using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IGoodsIssueRepository
{
    Task<GoodsIssue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(GoodsIssue entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<GoodsIssue> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchGoodsIssueQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedGoodsIssueDto>>;
public sealed record GetGoodsIssueByIdQuery(Guid Id) : IQuery<Result<GoodsIssueDto>>;
public sealed record CreateGoodsIssueCommand(string Number, string WarehouseCode, string Reference, string Status, string Notes) : ICommand<Result<GoodsIssueDto>>;
public sealed record UpdateGoodsIssueCommand(Guid Id, string Number, string WarehouseCode, string Reference, string Status, string Notes) : ICommand<Result<GoodsIssueDto>>;
public sealed record DeleteGoodsIssueCommand(Guid Id) : ICommand<Result>;

public static class GoodsIssueMapper
{
    public static GoodsIssueDto ToDto(GoodsIssue e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            WarehouseCode = e.WarehouseCode,
            Reference = e.Reference,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchGoodsIssueQueryHandler : IQueryHandler<SearchGoodsIssueQuery, Result<PagedGoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    public SearchGoodsIssueQueryHandler(IGoodsIssueRepository repo) => _repo = repo;
    public async Task<Result<PagedGoodsIssueDto>> HandleAsync(SearchGoodsIssueQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedGoodsIssueDto
        {
            Items = items.Select(GoodsIssueMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetGoodsIssueByIdQueryHandler : IQueryHandler<GetGoodsIssueByIdQuery, Result<GoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    public GetGoodsIssueByIdQueryHandler(IGoodsIssueRepository repo) => _repo = repo;
    public async Task<Result<GoodsIssueDto>> HandleAsync(GetGoodsIssueByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<GoodsIssueDto>(Error.NotFound("BUS-001", "GoodsIssue was not found."));
        return Result.Success(GoodsIssueMapper.ToDto(e));
    }
}

public sealed class CreateGoodsIssueCommandHandler : ICommandHandler<CreateGoodsIssueCommand, Result<GoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateGoodsIssueCommandHandler(IGoodsIssueRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<GoodsIssueDto>> HandleAsync(CreateGoodsIssueCommand command, CancellationToken cancellationToken = default)
    {
        var e = GoodsIssue.Create(command.Number, command.WarehouseCode, command.Reference, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsIssueMapper.ToDto(e));
    }
}

public sealed class UpdateGoodsIssueCommandHandler : ICommandHandler<UpdateGoodsIssueCommand, Result<GoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateGoodsIssueCommandHandler(IGoodsIssueRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<GoodsIssueDto>> HandleAsync(UpdateGoodsIssueCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<GoodsIssueDto>(Error.NotFound("BUS-001", "GoodsIssue was not found."));
        e.Update(command.Number, command.WarehouseCode, command.Reference, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsIssueMapper.ToDto(e));
    }
}

public sealed class DeleteGoodsIssueCommandHandler : ICommandHandler<DeleteGoodsIssueCommand, Result>
{
    private readonly IGoodsIssueRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteGoodsIssueCommandHandler(IGoodsIssueRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteGoodsIssueCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "GoodsIssue was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

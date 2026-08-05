using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IScrapRepository
{
    Task<Scrap?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Scrap entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Scrap> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchScrapQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedScrapDto>>;
public sealed record GetScrapByIdQuery(Guid Id) : IQuery<Result<ScrapDto>>;
public sealed record CreateScrapCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ScrapDto>>;
public sealed record UpdateScrapCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ScrapDto>>;
public sealed record DeleteScrapCommand(Guid Id) : ICommand<Result>;

public static class ScrapMapper
{
    public static ScrapDto ToDto(Scrap e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Name = e.Name,
        Status = e.Status,
        Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchScrapQueryHandler : IQueryHandler<SearchScrapQuery, Result<PagedScrapDto>>
{
    private readonly IScrapRepository _repo;
    public SearchScrapQueryHandler(IScrapRepository repo) => _repo = repo;
    public async Task<Result<PagedScrapDto>> HandleAsync(SearchScrapQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedScrapDto
        {
            Items = items.Select(ScrapMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetScrapByIdQueryHandler : IQueryHandler<GetScrapByIdQuery, Result<ScrapDto>>
{
    private readonly IScrapRepository _repo;
    public GetScrapByIdQueryHandler(IScrapRepository repo) => _repo = repo;
    public async Task<Result<ScrapDto>> HandleAsync(GetScrapByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ScrapDto>(Error.NotFound("BUS-001", "Scrap was not found."));
        return Result.Success(ScrapMapper.ToDto(e));
    }
}

public sealed class CreateScrapCommandHandler : ICommandHandler<CreateScrapCommand, Result<ScrapDto>>
{
    private readonly IScrapRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateScrapCommandHandler(IScrapRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ScrapDto>> HandleAsync(CreateScrapCommand command, CancellationToken cancellationToken = default)
    {
        var e = Scrap.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ScrapMapper.ToDto(e));
    }
}

public sealed class UpdateScrapCommandHandler : ICommandHandler<UpdateScrapCommand, Result<ScrapDto>>
{
    private readonly IScrapRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateScrapCommandHandler(IScrapRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ScrapDto>> HandleAsync(UpdateScrapCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ScrapDto>(Error.NotFound("BUS-001", "Scrap was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ScrapMapper.ToDto(e));
    }
}

public sealed class DeleteScrapCommandHandler : ICommandHandler<DeleteScrapCommand, Result>
{
    private readonly IScrapRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteScrapCommandHandler(IScrapRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteScrapCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Scrap was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

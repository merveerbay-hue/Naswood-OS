using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IWipRepository
{
    Task<Wip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Wip entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Wip> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchWipQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedWipDto>>;
public sealed record GetWipByIdQuery(Guid Id) : IQuery<Result<WipDto>>;
public sealed record CreateWipCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<WipDto>>;
public sealed record UpdateWipCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<WipDto>>;
public sealed record DeleteWipCommand(Guid Id) : ICommand<Result>;

public static class WipMapper
{
    public static WipDto ToDto(Wip e) => new()
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

public sealed class SearchWipQueryHandler : IQueryHandler<SearchWipQuery, Result<PagedWipDto>>
{
    private readonly IWipRepository _repo;
    public SearchWipQueryHandler(IWipRepository repo) => _repo = repo;
    public async Task<Result<PagedWipDto>> HandleAsync(SearchWipQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedWipDto
        {
            Items = items.Select(WipMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetWipByIdQueryHandler : IQueryHandler<GetWipByIdQuery, Result<WipDto>>
{
    private readonly IWipRepository _repo;
    public GetWipByIdQueryHandler(IWipRepository repo) => _repo = repo;
    public async Task<Result<WipDto>> HandleAsync(GetWipByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WipDto>(Error.NotFound("BUS-001", "Wip was not found."));
        return Result.Success(WipMapper.ToDto(e));
    }
}

public sealed class CreateWipCommandHandler : ICommandHandler<CreateWipCommand, Result<WipDto>>
{
    private readonly IWipRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateWipCommandHandler(IWipRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WipDto>> HandleAsync(CreateWipCommand command, CancellationToken cancellationToken = default)
    {
        var e = Wip.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WipMapper.ToDto(e));
    }
}

public sealed class UpdateWipCommandHandler : ICommandHandler<UpdateWipCommand, Result<WipDto>>
{
    private readonly IWipRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateWipCommandHandler(IWipRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WipDto>> HandleAsync(UpdateWipCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WipDto>(Error.NotFound("BUS-001", "Wip was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WipMapper.ToDto(e));
    }
}

public sealed class DeleteWipCommandHandler : ICommandHandler<DeleteWipCommand, Result>
{
    private readonly IWipRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteWipCommandHandler(IWipRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteWipCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Wip was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

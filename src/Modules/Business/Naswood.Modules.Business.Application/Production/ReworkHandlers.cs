using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IReworkRepository
{
    Task<Rework?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Rework entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Rework> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchReworkQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedReworkDto>>;
public sealed record GetReworkByIdQuery(Guid Id) : IQuery<Result<ReworkDto>>;
public sealed record CreateReworkCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ReworkDto>>;
public sealed record UpdateReworkCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ReworkDto>>;
public sealed record DeleteReworkCommand(Guid Id) : ICommand<Result>;

public static class ReworkMapper
{
    public static ReworkDto ToDto(Rework e) => new()
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

public sealed class SearchReworkQueryHandler : IQueryHandler<SearchReworkQuery, Result<PagedReworkDto>>
{
    private readonly IReworkRepository _repo;
    public SearchReworkQueryHandler(IReworkRepository repo) => _repo = repo;
    public async Task<Result<PagedReworkDto>> HandleAsync(SearchReworkQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedReworkDto
        {
            Items = items.Select(ReworkMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetReworkByIdQueryHandler : IQueryHandler<GetReworkByIdQuery, Result<ReworkDto>>
{
    private readonly IReworkRepository _repo;
    public GetReworkByIdQueryHandler(IReworkRepository repo) => _repo = repo;
    public async Task<Result<ReworkDto>> HandleAsync(GetReworkByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ReworkDto>(Error.NotFound("BUS-001", "Rework was not found."));
        return Result.Success(ReworkMapper.ToDto(e));
    }
}

public sealed class CreateReworkCommandHandler : ICommandHandler<CreateReworkCommand, Result<ReworkDto>>
{
    private readonly IReworkRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateReworkCommandHandler(IReworkRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ReworkDto>> HandleAsync(CreateReworkCommand command, CancellationToken cancellationToken = default)
    {
        var e = Rework.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ReworkMapper.ToDto(e));
    }
}

public sealed class UpdateReworkCommandHandler : ICommandHandler<UpdateReworkCommand, Result<ReworkDto>>
{
    private readonly IReworkRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateReworkCommandHandler(IReworkRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ReworkDto>> HandleAsync(UpdateReworkCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ReworkDto>(Error.NotFound("BUS-001", "Rework was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ReworkMapper.ToDto(e));
    }
}

public sealed class DeleteReworkCommandHandler : ICommandHandler<DeleteReworkCommand, Result>
{
    private readonly IReworkRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteReworkCommandHandler(IReworkRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteReworkCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Rework was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IToolingRepository
{
    Task<Tooling?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Tooling entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Tooling> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchToolingQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedToolingDto>>;
public sealed record GetToolingByIdQuery(Guid Id) : IQuery<Result<ToolingDto>>;
public sealed record CreateToolingCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ToolingDto>>;
public sealed record UpdateToolingCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ToolingDto>>;
public sealed record DeleteToolingCommand(Guid Id) : ICommand<Result>;

public static class ToolingMapper
{
    public static ToolingDto ToDto(Tooling e) => new()
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

public sealed class SearchToolingQueryHandler : IQueryHandler<SearchToolingQuery, Result<PagedToolingDto>>
{
    private readonly IToolingRepository _repo;
    public SearchToolingQueryHandler(IToolingRepository repo) => _repo = repo;
    public async Task<Result<PagedToolingDto>> HandleAsync(SearchToolingQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedToolingDto
        {
            Items = items.Select(ToolingMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetToolingByIdQueryHandler : IQueryHandler<GetToolingByIdQuery, Result<ToolingDto>>
{
    private readonly IToolingRepository _repo;
    public GetToolingByIdQueryHandler(IToolingRepository repo) => _repo = repo;
    public async Task<Result<ToolingDto>> HandleAsync(GetToolingByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ToolingDto>(Error.NotFound("BUS-001", "Tooling was not found."));
        return Result.Success(ToolingMapper.ToDto(e));
    }
}

public sealed class CreateToolingCommandHandler : ICommandHandler<CreateToolingCommand, Result<ToolingDto>>
{
    private readonly IToolingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateToolingCommandHandler(IToolingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ToolingDto>> HandleAsync(CreateToolingCommand command, CancellationToken cancellationToken = default)
    {
        var e = Tooling.Create(SystemIdentifier.Ensure(command.Code, "TL"), command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ToolingMapper.ToDto(e));
    }
}

public sealed class UpdateToolingCommandHandler : ICommandHandler<UpdateToolingCommand, Result<ToolingDto>>
{
    private readonly IToolingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateToolingCommandHandler(IToolingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ToolingDto>> HandleAsync(UpdateToolingCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ToolingDto>(Error.NotFound("BUS-001", "Tooling was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ToolingMapper.ToDto(e));
    }
}

public sealed class DeleteToolingCommandHandler : ICommandHandler<DeleteToolingCommand, Result>
{
    private readonly IToolingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteToolingCommandHandler(IToolingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteToolingCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Tooling was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

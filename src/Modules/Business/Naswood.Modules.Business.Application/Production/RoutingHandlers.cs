using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IRoutingRepository
{
    Task<Routing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Routing entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Routing> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchRoutingQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedRoutingDto>>;
public sealed record GetRoutingByIdQuery(Guid Id) : IQuery<Result<RoutingDto>>;
public sealed record CreateRoutingCommand(string Number, string MaterialCode, int Version, string Status, string Notes) : ICommand<Result<RoutingDto>>;
public sealed record UpdateRoutingCommand(Guid Id, string Number, string MaterialCode, int Version, string Status, string Notes) : ICommand<Result<RoutingDto>>;
public sealed record DeleteRoutingCommand(Guid Id) : ICommand<Result>;

public static class RoutingMapper
{
    public static RoutingDto ToDto(Routing e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            MaterialCode = e.MaterialCode,
            Version = e.Version,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchRoutingQueryHandler : IQueryHandler<SearchRoutingQuery, Result<PagedRoutingDto>>
{
    private readonly IRoutingRepository _repo;
    public SearchRoutingQueryHandler(IRoutingRepository repo) => _repo = repo;
    public async Task<Result<PagedRoutingDto>> HandleAsync(SearchRoutingQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedRoutingDto
        {
            Items = items.Select(RoutingMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetRoutingByIdQueryHandler : IQueryHandler<GetRoutingByIdQuery, Result<RoutingDto>>
{
    private readonly IRoutingRepository _repo;
    public GetRoutingByIdQueryHandler(IRoutingRepository repo) => _repo = repo;
    public async Task<Result<RoutingDto>> HandleAsync(GetRoutingByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<RoutingDto>(Error.NotFound("BUS-001", "Routing was not found."));
        return Result.Success(RoutingMapper.ToDto(e));
    }
}

public sealed class CreateRoutingCommandHandler : ICommandHandler<CreateRoutingCommand, Result<RoutingDto>>
{
    private readonly IRoutingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateRoutingCommandHandler(IRoutingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<RoutingDto>> HandleAsync(CreateRoutingCommand command, CancellationToken cancellationToken = default)
    {
        var e = Routing.Create(command.Number, command.MaterialCode, command.Version, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(RoutingMapper.ToDto(e));
    }
}

public sealed class UpdateRoutingCommandHandler : ICommandHandler<UpdateRoutingCommand, Result<RoutingDto>>
{
    private readonly IRoutingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateRoutingCommandHandler(IRoutingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<RoutingDto>> HandleAsync(UpdateRoutingCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<RoutingDto>(Error.NotFound("BUS-001", "Routing was not found."));
        e.Update(command.Number, command.MaterialCode, command.Version, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(RoutingMapper.ToDto(e));
    }
}

public sealed class DeleteRoutingCommandHandler : ICommandHandler<DeleteRoutingCommand, Result>
{
    private readonly IRoutingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteRoutingCommandHandler(IRoutingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteRoutingCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Routing was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

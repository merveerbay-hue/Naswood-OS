using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Location entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Location> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchLocationQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedLocationDto>>;
public sealed record GetLocationByIdQuery(Guid Id) : IQuery<Result<LocationDto>>;
public sealed record CreateLocationCommand(string Code, string Name, string WarehouseCode, string LocationType, string Status) : ICommand<Result<LocationDto>>;
public sealed record UpdateLocationCommand(Guid Id, string Code, string Name, string WarehouseCode, string LocationType, string Status) : ICommand<Result<LocationDto>>;
public sealed record DeleteLocationCommand(Guid Id) : ICommand<Result>;

public static class LocationMapper
{
    public static LocationDto ToDto(Location e) => new()
    {
        Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            WarehouseCode = e.WarehouseCode,
            LocationType = e.LocationType,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchLocationQueryHandler : IQueryHandler<SearchLocationQuery, Result<PagedLocationDto>>
{
    private readonly ILocationRepository _repo;
    public SearchLocationQueryHandler(ILocationRepository repo) => _repo = repo;
    public async Task<Result<PagedLocationDto>> HandleAsync(SearchLocationQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedLocationDto
        {
            Items = items.Select(LocationMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetLocationByIdQueryHandler : IQueryHandler<GetLocationByIdQuery, Result<LocationDto>>
{
    private readonly ILocationRepository _repo;
    public GetLocationByIdQueryHandler(ILocationRepository repo) => _repo = repo;
    public async Task<Result<LocationDto>> HandleAsync(GetLocationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<LocationDto>(Error.NotFound("BUS-001", "Location was not found."));
        return Result.Success(LocationMapper.ToDto(e));
    }
}

public sealed class CreateLocationCommandHandler : ICommandHandler<CreateLocationCommand, Result<LocationDto>>
{
    private readonly ILocationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateLocationCommandHandler(ILocationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<LocationDto>> HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var e = Location.Create(SystemIdentifier.Ensure(command.Code, "LOC"), command.Name, command.WarehouseCode, command.LocationType, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(LocationMapper.ToDto(e));
    }
}

public sealed class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand, Result<LocationDto>>
{
    private readonly ILocationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateLocationCommandHandler(ILocationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<LocationDto>> HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<LocationDto>(Error.NotFound("BUS-001", "Location was not found."));
        e.Update(command.Code, command.Name, command.WarehouseCode, command.LocationType, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(LocationMapper.ToDto(e));
    }
}

public sealed class DeleteLocationCommandHandler : ICommandHandler<DeleteLocationCommand, Result>
{
    private readonly ILocationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteLocationCommandHandler(ILocationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteLocationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Location was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

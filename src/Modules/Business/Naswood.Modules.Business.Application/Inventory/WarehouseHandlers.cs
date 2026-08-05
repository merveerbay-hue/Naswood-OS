using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Warehouse> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchWarehouseQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedWarehouseDto>>;
public sealed record GetWarehouseByIdQuery(Guid Id) : IQuery<Result<WarehouseDto>>;
public sealed record CreateWarehouseCommand(string Code, string Name, string WarehouseType, string Status, string? PlantId) : ICommand<Result<WarehouseDto>>;
public sealed record UpdateWarehouseCommand(Guid Id, string Code, string Name, string WarehouseType, string Status, string? PlantId) : ICommand<Result<WarehouseDto>>;
public sealed record DeleteWarehouseCommand(Guid Id) : ICommand<Result>;

public static class WarehouseMapper
{
    public static WarehouseDto ToDto(Warehouse e) => new()
    {
        Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            WarehouseType = e.WarehouseType,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchWarehouseQueryHandler : IQueryHandler<SearchWarehouseQuery, Result<PagedWarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    public SearchWarehouseQueryHandler(IWarehouseRepository repo) => _repo = repo;
    public async Task<Result<PagedWarehouseDto>> HandleAsync(SearchWarehouseQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedWarehouseDto
        {
            Items = items.Select(WarehouseMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetWarehouseByIdQueryHandler : IQueryHandler<GetWarehouseByIdQuery, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    public GetWarehouseByIdQueryHandler(IWarehouseRepository repo) => _repo = repo;
    public async Task<Result<WarehouseDto>> HandleAsync(GetWarehouseByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WarehouseDto>(Error.NotFound("BUS-001", "Warehouse was not found."));
        return Result.Success(WarehouseMapper.ToDto(e));
    }
}

public sealed class CreateWarehouseCommandHandler : ICommandHandler<CreateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateWarehouseCommandHandler(IWarehouseRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WarehouseDto>> HandleAsync(CreateWarehouseCommand command, CancellationToken cancellationToken = default)
    {
        var e = Warehouse.Create(command.Code, command.Name, command.WarehouseType, command.Status, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WarehouseMapper.ToDto(e));
    }
}

public sealed class UpdateWarehouseCommandHandler : ICommandHandler<UpdateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateWarehouseCommandHandler(IWarehouseRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WarehouseDto>> HandleAsync(UpdateWarehouseCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WarehouseDto>(Error.NotFound("BUS-001", "Warehouse was not found."));
        e.Update(command.Code, command.Name, command.WarehouseType, command.Status, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WarehouseMapper.ToDto(e));
    }
}

public sealed class DeleteWarehouseCommandHandler : ICommandHandler<DeleteWarehouseCommand, Result>
{
    private readonly IWarehouseRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteWarehouseCommandHandler(IWarehouseRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteWarehouseCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Warehouse was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

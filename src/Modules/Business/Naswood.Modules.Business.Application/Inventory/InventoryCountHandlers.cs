using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IInventoryCountRepository
{
    Task<InventoryCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryCount entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<InventoryCount> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchInventoryCountQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedInventoryCountDto>>;
public sealed record GetInventoryCountByIdQuery(Guid Id) : IQuery<Result<InventoryCountDto>>;
public sealed record CreateInventoryCountCommand(string Number, string WarehouseCode, string Status, string Notes) : ICommand<Result<InventoryCountDto>>;
public sealed record UpdateInventoryCountCommand(Guid Id, string Number, string WarehouseCode, string Status, string Notes) : ICommand<Result<InventoryCountDto>>;
public sealed record DeleteInventoryCountCommand(Guid Id) : ICommand<Result>;

public static class InventoryCountMapper
{
    public static InventoryCountDto ToDto(InventoryCount e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            WarehouseCode = e.WarehouseCode,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchInventoryCountQueryHandler : IQueryHandler<SearchInventoryCountQuery, Result<PagedInventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    public SearchInventoryCountQueryHandler(IInventoryCountRepository repo) => _repo = repo;
    public async Task<Result<PagedInventoryCountDto>> HandleAsync(SearchInventoryCountQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedInventoryCountDto
        {
            Items = items.Select(InventoryCountMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetInventoryCountByIdQueryHandler : IQueryHandler<GetInventoryCountByIdQuery, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    public GetInventoryCountByIdQueryHandler(IInventoryCountRepository repo) => _repo = repo;
    public async Task<Result<InventoryCountDto>> HandleAsync(GetInventoryCountByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<InventoryCountDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));
        return Result.Success(InventoryCountMapper.ToDto(e));
    }
}

public sealed class CreateInventoryCountCommandHandler : ICommandHandler<CreateInventoryCountCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateInventoryCountCommandHandler(IInventoryCountRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<InventoryCountDto>> HandleAsync(CreateInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = InventoryCount.Create(command.Number, command.WarehouseCode, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e));
    }
}

public sealed class UpdateInventoryCountCommandHandler : ICommandHandler<UpdateInventoryCountCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateInventoryCountCommandHandler(IInventoryCountRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<InventoryCountDto>> HandleAsync(UpdateInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<InventoryCountDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));
        e.Update(command.Number, command.WarehouseCode, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e));
    }
}

public sealed class DeleteInventoryCountCommandHandler : ICommandHandler<DeleteInventoryCountCommand, Result>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteInventoryCountCommandHandler(IInventoryCountRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "InventoryCount was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IInventoryAdjustmentRepository
{
    Task<InventoryAdjustment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryAdjustment entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<InventoryAdjustment> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchInventoryAdjustmentQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedInventoryAdjustmentDto>>;
public sealed record GetInventoryAdjustmentByIdQuery(Guid Id) : IQuery<Result<InventoryAdjustmentDto>>;
public sealed record CreateInventoryAdjustmentCommand(string Number, string WarehouseCode, string Reason, string Status, string Notes) : ICommand<Result<InventoryAdjustmentDto>>;
public sealed record UpdateInventoryAdjustmentCommand(Guid Id, string Number, string WarehouseCode, string Reason, string Status, string Notes) : ICommand<Result<InventoryAdjustmentDto>>;
public sealed record DeleteInventoryAdjustmentCommand(Guid Id) : ICommand<Result>;

public static class InventoryAdjustmentMapper
{
    public static InventoryAdjustmentDto ToDto(InventoryAdjustment e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            WarehouseCode = e.WarehouseCode,
            Reason = e.Reason,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchInventoryAdjustmentQueryHandler : IQueryHandler<SearchInventoryAdjustmentQuery, Result<PagedInventoryAdjustmentDto>>
{
    private readonly IInventoryAdjustmentRepository _repo;
    public SearchInventoryAdjustmentQueryHandler(IInventoryAdjustmentRepository repo) => _repo = repo;
    public async Task<Result<PagedInventoryAdjustmentDto>> HandleAsync(SearchInventoryAdjustmentQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedInventoryAdjustmentDto
        {
            Items = items.Select(InventoryAdjustmentMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetInventoryAdjustmentByIdQueryHandler : IQueryHandler<GetInventoryAdjustmentByIdQuery, Result<InventoryAdjustmentDto>>
{
    private readonly IInventoryAdjustmentRepository _repo;
    public GetInventoryAdjustmentByIdQueryHandler(IInventoryAdjustmentRepository repo) => _repo = repo;
    public async Task<Result<InventoryAdjustmentDto>> HandleAsync(GetInventoryAdjustmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<InventoryAdjustmentDto>(Error.NotFound("BUS-001", "InventoryAdjustment was not found."));
        return Result.Success(InventoryAdjustmentMapper.ToDto(e));
    }
}

public sealed class CreateInventoryAdjustmentCommandHandler : ICommandHandler<CreateInventoryAdjustmentCommand, Result<InventoryAdjustmentDto>>
{
    private readonly IInventoryAdjustmentRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateInventoryAdjustmentCommandHandler(IInventoryAdjustmentRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<InventoryAdjustmentDto>> HandleAsync(CreateInventoryAdjustmentCommand command, CancellationToken cancellationToken = default)
    {
        var e = InventoryAdjustment.Create(command.Number, command.WarehouseCode, command.Reason, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryAdjustmentMapper.ToDto(e));
    }
}

public sealed class UpdateInventoryAdjustmentCommandHandler : ICommandHandler<UpdateInventoryAdjustmentCommand, Result<InventoryAdjustmentDto>>
{
    private readonly IInventoryAdjustmentRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateInventoryAdjustmentCommandHandler(IInventoryAdjustmentRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<InventoryAdjustmentDto>> HandleAsync(UpdateInventoryAdjustmentCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<InventoryAdjustmentDto>(Error.NotFound("BUS-001", "InventoryAdjustment was not found."));
        e.Update(command.Number, command.WarehouseCode, command.Reason, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryAdjustmentMapper.ToDto(e));
    }
}

public sealed class DeleteInventoryAdjustmentCommandHandler : ICommandHandler<DeleteInventoryAdjustmentCommand, Result>
{
    private readonly IInventoryAdjustmentRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteInventoryAdjustmentCommandHandler(IInventoryAdjustmentRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteInventoryAdjustmentCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "InventoryAdjustment was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

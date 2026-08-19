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
    Task<(IReadOnlyList<InventoryCount> Items, int Total)> SearchAsync(string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default);
}

public sealed record SearchInventoryCountQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedInventoryCountDto>>;

public sealed record GetInventoryCountByIdQuery(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<InventoryCountDto>>;

public sealed record CreateInventoryCountCommand(
    string Number,
    string WarehouseCode,
    string Status,
    string Notes,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<InventoryCountDto>>;

public sealed record UpdateInventoryCountCommand(
    Guid Id,
    string Number,
    string WarehouseCode,
    string Status,
    string Notes,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<InventoryCountDto>>;

public sealed record DeleteInventoryCountCommand(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result>;

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
        var plantId = string.IsNullOrWhiteSpace(query.PlantId) ? null : query.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<PagedInventoryCountDto>(Error.Validation(
                "INV-CNT-004",
                "Plant / factory context is required."));

        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, plantId))
            return Result.Failure<PagedInventoryCountDto>(Error.Forbidden(
                "INV-CNT-403",
                "Bu tesisin sayım belgelerini görüntüleme yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, plantId, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedInventoryCountDto
        {
            Items = items.Select(InventoryCountMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
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
        if (e is null || e.IsDeleted)
            return Result.Failure<InventoryCountDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));

        if (query.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<InventoryCountDto>(Error.Forbidden(
                "INV-CNT-403",
                "Bu sayım belgesini görüntüleme yetkiniz yok."));

        return Result.Success(InventoryCountMapper.ToDto(e));
    }
}

public sealed class CreateInventoryCountCommandHandler : ICommandHandler<CreateInventoryCountCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public CreateInventoryCountCommandHandler(
        IInventoryCountRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<InventoryCountDto>> HandleAsync(CreateInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(command.PlantId) ? null : command.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<InventoryCountDto>(Error.Validation(
                "INV-CNT-004",
                "Plant / factory context is required."));

        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<InventoryCountDto>(Error.Forbidden(
                "INV-CNT-403",
                "Bu tesise sayım belgesi oluşturma yetkiniz yok."));

        if (string.IsNullOrWhiteSpace(command.WarehouseCode))
            return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-015", "WarehouseCode is required."));

        var wh = await _warehouses.GetByCodeAndPlantAsync(command.WarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (wh is null)
            return Result.Failure<InventoryCountDto>(Error.Validation(
                "INV-CNT-015",
                "Warehouse must belong to the resolved plant."));

        var e = InventoryCount.Create(
            command.Number,
            wh.Code,
            command.Status,
            command.Notes,
            plantId: plantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e));
    }
}

public sealed class UpdateInventoryCountCommandHandler : ICommandHandler<UpdateInventoryCountCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public UpdateInventoryCountCommandHandler(
        IInventoryCountRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<InventoryCountDto>> HandleAsync(UpdateInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<InventoryCountDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<InventoryCountDto>(Error.Forbidden(
                "INV-CNT-403",
                "Bu sayım belgesini değiştirme yetkiniz yok."));

        var plantId = string.IsNullOrWhiteSpace(e.PlantId) ? null : e.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<InventoryCountDto>(Error.Validation(
                "INV-CNT-004",
                "Plant / factory context is required."));

        var wh = await _warehouses.GetByCodeAndPlantAsync(command.WarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (wh is null)
            return Result.Failure<InventoryCountDto>(Error.Validation(
                "INV-CNT-015",
                "Warehouse must belong to the resolved plant."));

        e.Update(command.Number, wh.Code, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e));
    }
}

public sealed class DeleteInventoryCountCommandHandler : ICommandHandler<DeleteInventoryCountCommand, Result>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public DeleteInventoryCountCommandHandler(IInventoryCountRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(DeleteInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure(Error.NotFound("BUS-001", "InventoryCount was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure(Error.Forbidden(
                "INV-CNT-403",
                "Bu sayım belgesini silme yetkiniz yok."));

        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

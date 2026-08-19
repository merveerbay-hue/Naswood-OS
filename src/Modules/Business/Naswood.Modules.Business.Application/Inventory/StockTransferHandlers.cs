using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IStockTransferRepository
{
    Task<StockTransfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(StockTransfer entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<StockTransfer> Items, int Total)> SearchAsync(string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default);
}

public sealed record SearchStockTransferQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedStockTransferDto>>;

public sealed record GetStockTransferByIdQuery(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<StockTransferDto>>;

public sealed record CreateStockTransferCommand(
    string Number,
    string FromWarehouseCode,
    string ToWarehouseCode,
    string Status,
    string Notes,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<StockTransferDto>>;

public sealed record UpdateStockTransferCommand(
    Guid Id,
    string Number,
    string FromWarehouseCode,
    string ToWarehouseCode,
    string Status,
    string Notes,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<StockTransferDto>>;

public sealed record DeleteStockTransferCommand(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result>;

public static class StockTransferMapper
{
    public static StockTransferDto ToDto(StockTransfer e) => new()
    {
        Id = e.Id,
        Number = e.Number,
        FromWarehouseCode = e.FromWarehouseCode,
        ToWarehouseCode = e.ToWarehouseCode,
        Status = e.Status,
        Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchStockTransferQueryHandler : IQueryHandler<SearchStockTransferQuery, Result<PagedStockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    public SearchStockTransferQueryHandler(IStockTransferRepository repo) => _repo = repo;

    public async Task<Result<PagedStockTransferDto>> HandleAsync(SearchStockTransferQuery query, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(query.PlantId) ? null : query.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<PagedStockTransferDto>(Error.Validation(
                "INV-TRF-004",
                "Plant / factory context is required."));

        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, plantId))
            return Result.Failure<PagedStockTransferDto>(Error.Forbidden(
                "INV-TRF-403",
                "Bu tesisin transfer belgelerini görüntüleme yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, plantId, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedStockTransferDto
        {
            Items = items.Select(StockTransferMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetStockTransferByIdQueryHandler : IQueryHandler<GetStockTransferByIdQuery, Result<StockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    public GetStockTransferByIdQueryHandler(IStockTransferRepository repo) => _repo = repo;

    public async Task<Result<StockTransferDto>> HandleAsync(GetStockTransferByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<StockTransferDto>(Error.NotFound("BUS-001", "StockTransfer was not found."));

        if (query.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<StockTransferDto>(Error.Forbidden(
                "INV-TRF-403",
                "Bu transfer belgesini görüntüleme yetkiniz yok."));

        return Result.Success(StockTransferMapper.ToDto(e));
    }
}

public sealed class CreateStockTransferCommandHandler : ICommandHandler<CreateStockTransferCommand, Result<StockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public CreateStockTransferCommandHandler(
        IStockTransferRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<StockTransferDto>> HandleAsync(CreateStockTransferCommand command, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(command.PlantId) ? null : command.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<StockTransferDto>(Error.Validation(
                "INV-TRF-004",
                "Plant / factory context is required."));

        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<StockTransferDto>(Error.Forbidden(
                "INV-TRF-403",
                "Bu tesise transfer belgesi oluşturma yetkiniz yok."));

        if (string.IsNullOrWhiteSpace(command.FromWarehouseCode) || string.IsNullOrWhiteSpace(command.ToWarehouseCode))
            return Result.Failure<StockTransferDto>(Error.Validation("INV-TRF-015", "From/To warehouse codes are required."));

        var fromWh = await _warehouses.GetByCodeAndPlantAsync(command.FromWarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (fromWh is null)
            return Result.Failure<StockTransferDto>(Error.Validation(
                "INV-TRF-015",
                "From warehouse must belong to the resolved plant."));

        var toWh = await _warehouses.GetByCodeAndPlantAsync(command.ToWarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (toWh is null)
            return Result.Failure<StockTransferDto>(Error.Validation(
                "INV-TRF-015",
                "To warehouse must belong to the resolved plant."));

        var e = StockTransfer.Create(
            command.Number,
            fromWh.Code,
            toWh.Code,
            command.Status,
            command.Notes,
            plantId: plantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StockTransferMapper.ToDto(e));
    }
}

public sealed class UpdateStockTransferCommandHandler : ICommandHandler<UpdateStockTransferCommand, Result<StockTransferDto>>
{
    private readonly IStockTransferRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public UpdateStockTransferCommandHandler(
        IStockTransferRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<StockTransferDto>> HandleAsync(UpdateStockTransferCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<StockTransferDto>(Error.NotFound("BUS-001", "StockTransfer was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<StockTransferDto>(Error.Forbidden(
                "INV-TRF-403",
                "Bu transfer belgesini değiştirme yetkiniz yok."));

        var plantId = string.IsNullOrWhiteSpace(e.PlantId) ? null : e.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<StockTransferDto>(Error.Validation(
                "INV-TRF-004",
                "Plant / factory context is required."));

        var fromWh = await _warehouses.GetByCodeAndPlantAsync(command.FromWarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (fromWh is null)
            return Result.Failure<StockTransferDto>(Error.Validation(
                "INV-TRF-015",
                "From warehouse must belong to the resolved plant."));

        var toWh = await _warehouses.GetByCodeAndPlantAsync(command.ToWarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (toWh is null)
            return Result.Failure<StockTransferDto>(Error.Validation(
                "INV-TRF-015",
                "To warehouse must belong to the resolved plant."));

        e.Update(command.Number, fromWh.Code, toWh.Code, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StockTransferMapper.ToDto(e));
    }
}

public sealed class DeleteStockTransferCommandHandler : ICommandHandler<DeleteStockTransferCommand, Result>
{
    private readonly IStockTransferRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public DeleteStockTransferCommandHandler(IStockTransferRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(DeleteStockTransferCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure(Error.NotFound("BUS-001", "StockTransfer was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure(Error.Forbidden(
                "INV-TRF-403",
                "Bu transfer belgesini silme yetkiniz yok."));

        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

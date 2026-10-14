using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IGoodsReceiptRepository
{
    Task<GoodsReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GoodsReceipt?> GetByNumberAsync(string number, string? plantId = null, CancellationToken cancellationToken = default);
    Task AddAsync(GoodsReceipt entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<GoodsReceipt> Items, int Total)> SearchAsync(string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default);
    Task<int> CountOpenAsync(string plantId, CancellationToken cancellationToken = default);
}

public sealed record SearchGoodsReceiptQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedGoodsReceiptDto>>;

public sealed record GetGoodsReceiptByIdQuery(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<GoodsReceiptDto>>;

public sealed record CreateGoodsReceiptCommand(
    string Number,
    string WarehouseCode,
    string Reference,
    string Status,
    string Notes,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<GoodsReceiptDto>>;

public sealed record UpdateGoodsReceiptCommand(
    Guid Id,
    string Number,
    string WarehouseCode,
    string Reference,
    string Status,
    string Notes,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<GoodsReceiptDto>>;

public sealed record DeleteGoodsReceiptCommand(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result>;

public static class GoodsReceiptMapper
{
    public static GoodsReceiptDto ToDto(GoodsReceipt e) => new()
    {
        Id = e.Id,
        Number = e.Number,
        WarehouseCode = e.WarehouseCode,
        Reference = e.Reference,
        Status = e.Status,
        Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchGoodsReceiptQueryHandler : IQueryHandler<SearchGoodsReceiptQuery, Result<PagedGoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    public SearchGoodsReceiptQueryHandler(IGoodsReceiptRepository repo) => _repo = repo;

    public async Task<Result<PagedGoodsReceiptDto>> HandleAsync(SearchGoodsReceiptQuery query, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(query.PlantId) ? null : query.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<PagedGoodsReceiptDto>(Error.Validation(
                "INV-GR-004",
                "Plant / factory context is required."));

        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, plantId))
            return Result.Failure<PagedGoodsReceiptDto>(Error.Forbidden(
                "INV-GR-403",
                "Bu tesisin mal kabul belgelerini görüntüleme yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        // SQL-level PlantId filter — never return a merged multi-plant list.
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, plantId, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedGoodsReceiptDto
        {
            Items = items.Select(GoodsReceiptMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetGoodsReceiptByIdQueryHandler : IQueryHandler<GetGoodsReceiptByIdQuery, Result<GoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    public GetGoodsReceiptByIdQueryHandler(IGoodsReceiptRepository repo) => _repo = repo;

    public async Task<Result<GoodsReceiptDto>> HandleAsync(GetGoodsReceiptByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<GoodsReceiptDto>(Error.NotFound("BUS-001", "GoodsReceipt was not found."));

        if (query.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<GoodsReceiptDto>(Error.Forbidden(
                "INV-GR-403",
                "Bu mal kabul belgesini görüntüleme yetkiniz yok."));

        return Result.Success(GoodsReceiptMapper.ToDto(e));
    }
}

public sealed class CreateGoodsReceiptCommandHandler : ICommandHandler<CreateGoodsReceiptCommand, Result<GoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public CreateGoodsReceiptCommandHandler(
        IGoodsReceiptRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<GoodsReceiptDto>> HandleAsync(CreateGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(command.PlantId) ? null : command.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<GoodsReceiptDto>(Error.Validation(
                "INV-GR-004",
                "Plant / factory context is required."));

        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<GoodsReceiptDto>(Error.Forbidden(
                "INV-GR-403",
                "Bu tesise mal kabul belgesi oluşturma yetkiniz yok."));

        var whCheck = await ValidateWarehouseUnderPlantAsync(command.WarehouseCode, plantId, cancellationToken).ConfigureAwait(false);
        if (whCheck is not null)
            return Result.Failure<GoodsReceiptDto>(whCheck);

        var e = GoodsReceipt.Create(
            SystemIdentifier.Ensure(command.Number, "GR"),
            command.WarehouseCode.Trim(),
            command.Reference,
            command.Status,
            command.Notes,
            plantId: plantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsReceiptMapper.ToDto(e));
    }

    private async Task<Error?> ValidateWarehouseUnderPlantAsync(
        string warehouseCode,
        string plantId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(warehouseCode))
            return Error.Validation("INV-GR-015", "WarehouseCode is required.");

        var wh = await _warehouses.GetByCodeAndPlantAsync(warehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (wh is null)
            return Error.Validation(
                "INV-GR-015",
                "Warehouse must belong to the resolved plant.");

        if (!string.Equals(wh.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Error.Validation("INV-GR-016", "Warehouse must be Active.");

        return null;
    }
}

public sealed class UpdateGoodsReceiptCommandHandler : ICommandHandler<UpdateGoodsReceiptCommand, Result<GoodsReceiptDto>>
{
    private readonly IGoodsReceiptRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public UpdateGoodsReceiptCommandHandler(
        IGoodsReceiptRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<GoodsReceiptDto>> HandleAsync(UpdateGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<GoodsReceiptDto>(Error.NotFound("BUS-001", "GoodsReceipt was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<GoodsReceiptDto>(Error.Forbidden(
                "INV-GR-403",
                "Bu mal kabul belgesini değiştirme yetkiniz yok."));

        var plantId = string.IsNullOrWhiteSpace(e.PlantId) ? null : e.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<GoodsReceiptDto>(Error.Validation(
                "INV-GR-004",
                "Plant / factory context is required."));

        var wh = await _warehouses.GetByCodeAndPlantAsync(command.WarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (wh is null)
            return Result.Failure<GoodsReceiptDto>(Error.Validation(
                "INV-GR-015",
                "Warehouse must belong to the resolved plant."));

        e.Update(command.Number, command.WarehouseCode.Trim(), command.Reference, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsReceiptMapper.ToDto(e));
    }
}

public sealed class DeleteGoodsReceiptCommandHandler : ICommandHandler<DeleteGoodsReceiptCommand, Result>
{
    private readonly IGoodsReceiptRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public DeleteGoodsReceiptCommandHandler(IGoodsReceiptRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(DeleteGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure(Error.NotFound("BUS-001", "GoodsReceipt was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure(Error.Forbidden(
                "INV-GR-403",
                "Bu mal kabul belgesini silme yetkiniz yok."));

        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

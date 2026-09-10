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
    Task<IReadOnlyList<InventoryCountLine>> ListLinesAsync(Guid countId, CancellationToken cancellationToken = default);
    Task AddLineAsync(InventoryCountLine line, CancellationToken cancellationToken = default);
    void RemoveLine(InventoryCountLine line);
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
    IReadOnlyList<string>? AllowedPlantIds,
    string LocationCode = "",
    string CountType = "Normal",
    string Actor = "") : ICommand<Result<InventoryCountDto>>;

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
    public static InventoryCountDto ToDto(
        InventoryCount e,
        IReadOnlyList<InventoryCountLineDto>? lines = null,
        bool midCountMovement = false,
        int midCountMovementCount = 0,
        InventoryCountSummaryDto? summary = null) => new()
    {
        Id = e.Id,
        Number = e.Number,
        WarehouseCode = e.WarehouseCode,
        LocationCode = string.IsNullOrWhiteSpace(e.LocationCode) ? null : e.LocationCode,
        CountType = string.IsNullOrWhiteSpace(e.CountType) ? "Normal" : e.CountType,
        Status = NormalizeStatus(e.Status),
        Notes = e.Notes,
        SnapshotAt = e.SnapshotAt,
        StartedBy = e.StartedBy,
        StartedAt = e.StartedAt == default ? e.CreatedAt : e.StartedAt,
        CountedBy = e.CountedBy,
        CompletedBy = e.CompletedBy,
        CompletedAt = e.CompletedAt,
        ApprovedBy = e.ApprovedBy,
        ApprovedAt = e.ApprovedAt,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt,
        MidCountMovement = midCountMovement,
        MidCountMovementCount = midCountMovementCount,
        Lines = lines,
        Summary = summary
    };

    public static string NormalizeStatus(string? status)
    {
        var s = (status ?? string.Empty).Trim();
        if (s.Equals("Draft", StringComparison.OrdinalIgnoreCase)) return InventoryCountStatuses.Draft;
        if (s.Equals("Open", StringComparison.OrdinalIgnoreCase)
            || s.Equals("In Progress", StringComparison.OrdinalIgnoreCase)
            || s.Equals("InProgress", StringComparison.OrdinalIgnoreCase)
            || s.Equals("Released", StringComparison.OrdinalIgnoreCase))
            return InventoryCountStatuses.Counting;
        if (s.Equals("Closed", StringComparison.OrdinalIgnoreCase)) return InventoryCountStatuses.Review;
        return string.IsNullOrEmpty(s) ? InventoryCountStatuses.Draft : s.ToUpperInvariant();
    }
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
            Items = items.Select(x => InventoryCountMapper.ToDto(x)).ToArray(),
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
    private readonly IInventoryMovementRepository _movements;

    public GetInventoryCountByIdQueryHandler(IInventoryCountRepository repo, IInventoryMovementRepository movements)
    {
        _repo = repo;
        _movements = movements;
    }

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

        var rawLines = await _repo.ListLinesAsync(e.Id, cancellationToken).ConfigureAwait(false);
        var lineDtos = InventoryCountSessionComposer.ToLineDtos(rawLines);
        var mid = 0;
        if (e.SnapshotAt is not null && !string.IsNullOrWhiteSpace(e.PlantId))
        {
            mid = await _movements.CountPostedAfterAsync(
                e.PlantId,
                e.WarehouseCode,
                string.IsNullOrWhiteSpace(e.LocationCode) ? null : e.LocationCode,
                e.SnapshotAt.Value,
                e.Number,
                cancellationToken).ConfigureAwait(false);
        }

        return Result.Success(InventoryCountMapper.ToDto(
            e,
            lineDtos,
            mid > 0,
            mid,
            InventoryCountSessionComposer.Summarize(lineDtos, mid > 0)));
    }
}

public sealed class CreateInventoryCountCommandHandler : ICommandHandler<CreateInventoryCountCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IMaterialRepository _materials;
    private readonly IBusinessUnitOfWork _uow;

    public CreateInventoryCountCommandHandler(
        IInventoryCountRepository repo,
        IWarehouseRepository warehouses,
        ILocationRepository locations,
        IInventoryBalanceRepository balances,
        IMaterialRepository materials,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _locations = locations;
        _balances = balances;
        _materials = materials;
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
        if (!InventoryCountMath.IsActiveStatus(wh.Status))
            return Result.Failure<InventoryCountDto>(Error.Validation(
                "INV-CNT-016",
                $"Pasif depo '{wh.Code}' için sayım başlatılamaz."));

        var locationCode = (command.LocationCode ?? string.Empty).Trim();
        if (string.Equals(locationCode, "ALL", StringComparison.OrdinalIgnoreCase)
            || string.Equals(locationCode, "TUMU", StringComparison.OrdinalIgnoreCase)
            || string.Equals(locationCode, "TÜMÜ", StringComparison.OrdinalIgnoreCase))
            locationCode = string.Empty;

        if (!string.IsNullOrWhiteSpace(locationCode))
        {
            var loc = await _locations.FindByWarehouseAndCodeAsync(wh.Code, locationCode, plantId, cancellationToken).ConfigureAwait(false);
            if (loc is null)
                return Result.Failure<InventoryCountDto>(Error.Forbidden(
                    "INV-CNT-403",
                    $"Lokasyon '{locationCode}' bu tesis/depo altında değil."));
            if (!string.IsNullOrWhiteSpace(loc.PlantId)
                && !string.Equals(loc.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<InventoryCountDto>(Error.Forbidden(
                    "INV-CNT-403",
                    "Başka fabrikanın lokasyonu sayıma alınamaz."));
            if (!InventoryCountMath.IsActiveStatus(loc.Status))
                return Result.Failure<InventoryCountDto>(Error.Validation(
                    "INV-CNT-017",
                    $"Pasif lokasyon '{loc.Code}' için sayım başlatılamaz."));
            if (InventoryCountMath.IsWipLocationType(loc.LocationType))
                return Result.Failure<InventoryCountDto>(Error.Validation(
                    "INV-CNT-038",
                    "Üretim noktası / WIP lokasyonu stok sayımı lokasyonu değildir."));
            locationCode = loc.Code;
        }

        var countType = string.Equals(command.CountType?.Trim(), "Blind", StringComparison.OrdinalIgnoreCase)
            || string.Equals(command.CountType?.Trim(), "Kör", StringComparison.OrdinalIgnoreCase)
            || string.Equals(command.CountType?.Trim(), "Kor", StringComparison.OrdinalIgnoreCase)
            ? "Blind"
            : "Normal";

        var number = string.IsNullOrWhiteSpace(command.Number)
            ? $"SC-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1, 99999):D5}"
            : command.Number.Trim();

        var e = InventoryCount.Create(
            number,
            wh.Code,
            InventoryCountStatuses.Counting,
            command.Notes ?? string.Empty,
            plantId: plantId,
            locationCode: locationCode,
            countType: countType,
            startedBy: command.Actor ?? string.Empty);

        var snapshotAt = DateTimeOffset.UtcNow;
        e.CaptureSnapshot(snapshotAt);

        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);

        var balances = await _balances.ListForCountSnapshotAsync(plantId, wh.Code, string.IsNullOrWhiteSpace(locationCode) ? null : locationCode, cancellationToken).ConfigureAwait(false);
        var lineNo = 0;
        foreach (var bal in balances)
        {
            Location? locRow = null;
            if (!string.IsNullOrWhiteSpace(bal.LocationCode))
                locRow = await _locations.FindByWarehouseAndCodeAsync(wh.Code, bal.LocationCode, plantId, cancellationToken).ConfigureAwait(false);
            if (locRow is not null && InventoryCountMath.IsWipLocationType(locRow.LocationType))
                continue;

            var material = await _materials.GetByCodeAsync(bal.MaterialCode, cancellationToken).ConfigureAwait(false);
            var policy = InventoryCountMath.ResolvePolicy(material?.UnitOfMeasure, material?.Category, material?.DefinitionJson);
            lineNo++;
            var line = InventoryCountLine.CreateSnapshot(
                e.Id,
                lineNo,
                material?.Id,
                bal.MaterialCode,
                material?.Name ?? bal.MaterialCode,
                bal.LocationCode,
                bal.BatchNumber,
                bal.QuantityOnHand,
                policy.StockUnit,
                policy.CountUnit,
                plantId);
            await _repo.AddLineAsync(line, cancellationToken).ConfigureAwait(false);
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var lines = await _repo.ListLinesAsync(e.Id, cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e, InventoryCountSessionComposer.ToLineDtos(lines)));
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

        var nextStatus = command.Status;
        if (string.Equals(nextStatus, "Posted", StringComparison.OrdinalIgnoreCase)
            || string.Equals(nextStatus, InventoryCountStatuses.Posted, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<InventoryCountDto>(Error.Validation(
                "INV-CNT-047",
                "Sayım stoğa yalnızca onay/post endpoint’i ile işlenir."));

        try
        {
            e.Update(command.Number, wh.Code, nextStatus, command.Notes);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-048", ex.Message));
        }
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

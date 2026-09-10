using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Location?> FindByWarehouseAndCodeAsync(string warehouseCode, string code, string? plantId, CancellationToken cancellationToken = default);
    Task AddAsync(Location entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Location> Items, int Total)> SearchAsync(
        string? q,
        string? plantId,
        string? warehouseCode,
        string? locationType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<bool> HasStockOrMovementAsync(string warehouseCode, string locationCode, string? plantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Physical stock area kinds inside a warehouse.
/// STAGING / WIP hold inventory as Location — they are NOT Work Centers.
/// </summary>
public static class LocationTypes
{
    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        "OPEN_AREA",
        "FIELD",
        "RACK",
        "BLOCK",
        "QUARANTINE_AREA",
        "QUARANTINE", // alias → normalizes to QUARANTINE_AREA
        "PACKAGE_AREA",
        "STAGING",
        "WIP",
        "OTHER",
    };

    public static string Normalize(string? type)
    {
        var t = (type ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(t)) return "OPEN_AREA";
        if (t is "QUARANTINE") return "QUARANTINE_AREA";
        return t;
    }

    public static bool IsKnown(string? type)
    {
        var n = Normalize(type);
        return All.Contains(n) || string.Equals(n, "QUARANTINE_AREA", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed record SearchLocationQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    string? WarehouseCode,
    string? LocationType,
    IReadOnlyList<string> AllowedPlantIds) : IQuery<Result<PagedLocationDto>>;

public sealed record GetLocationByIdQuery(Guid Id, IReadOnlyList<string> AllowedPlantIds) : IQuery<Result<LocationDto>>;

public sealed record CreateLocationCommand(
    string Code,
    string Name,
    string WarehouseCode,
    string LocationType,
    string Status,
    string Description,
    string StockZoneType,
    string PlantId,
    IReadOnlyList<string> AllowedPlantIds) : ICommand<Result<LocationDto>>;

public sealed record UpdateLocationCommand(
    Guid Id,
    string Code,
    string Name,
    string WarehouseCode,
    string LocationType,
    string Status,
    string Description,
    string StockZoneType,
    IReadOnlyList<string> AllowedPlantIds) : ICommand<Result<LocationDto>>;

public sealed record DeleteLocationCommand(Guid Id, IReadOnlyList<string> AllowedPlantIds) : ICommand<Result>;

public static class LocationMapper
{
    public static LocationDto ToDto(Location e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Name = e.Name,
        WarehouseCode = e.WarehouseCode,
        LocationType = e.LocationType,
        StockZoneType = StockZoneTypes.Resolve(e.StockZoneType, e.LocationType),
        Status = e.Status,
        Description = e.Description,
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
        if (!PlantAccess.CanAccess(query.AllowedPlantIds, query.PlantId))
            return Result.Failure<PagedLocationDto>(Error.Forbidden("BUS-LOC-403", "Bu tesise erişim yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(
            query.Q, query.PlantId, query.WarehouseCode, query.LocationType, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedLocationDto
        {
            Items = items.Select(LocationMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
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
        if (e is null || e.IsDeleted)
            return Result.Failure<LocationDto>(Error.NotFound("BUS-001", "Location was not found."));
        if (!PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<LocationDto>(Error.Forbidden("BUS-LOC-403", "Bu tesise erişim yetkiniz yok."));
        return Result.Success(LocationMapper.ToDto(e));
    }
}

public sealed class CreateLocationCommandHandler : ICommandHandler<CreateLocationCommand, Result<LocationDto>>
{
    private readonly ILocationRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public CreateLocationCommandHandler(ILocationRepository repo, IWarehouseRepository warehouses, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<LocationDto>> HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
    {
        if (!PlantAccess.CanAccess(command.AllowedPlantIds, command.PlantId))
            return Result.Failure<LocationDto>(Error.Forbidden("BUS-LOC-403", "Bu tesiste lokasyon oluşturma yetkiniz yok."));

        if (string.IsNullOrWhiteSpace(command.WarehouseCode))
            return Result.Failure<LocationDto>(Error.Validation("BUS-LOC-001", "Depo seçimi zorunludur."));

        var warehouseCode = command.WarehouseCode.Trim().ToUpperInvariant();
        var warehouse = await _warehouses.GetByCodeAndPlantAsync(warehouseCode, command.PlantId, cancellationToken).ConfigureAwait(false);
        if (warehouse is null || warehouse.IsDeleted)
            return Result.Failure<LocationDto>(Error.Validation("BUS-LOC-002", "Seçilen depo bu tesise ait değil veya bulunamadı."));
        if (!string.Equals(warehouse.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<LocationDto>(Error.Validation("BUS-LOC-003", "Pasif depoya lokasyon eklenemez."));

        var locationType = LocationTypes.Normalize(command.LocationType);
        if (!LocationTypes.IsKnown(locationType))
            return Result.Failure<LocationDto>(Error.Validation("BUS-LOC-004", $"Bilinmeyen lokasyon tipi '{command.LocationType}'."));

        var code = string.IsNullOrWhiteSpace(command.Code)
            ? SystemIdentifier.Ensure(null, "LOC")
            : command.Code.Trim().ToUpperInvariant();
        var name = string.IsNullOrWhiteSpace(command.Name) ? code : command.Name.Trim();
        var status = string.IsNullOrWhiteSpace(command.Status) ? "Active" : command.Status.Trim();
        if (!string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase))
            status = "Active";

        var duplicate = await _repo.FindByWarehouseAndCodeAsync(warehouseCode, code, command.PlantId, cancellationToken).ConfigureAwait(false);
        if (duplicate is not null && !duplicate.IsDeleted)
            return Result.Failure<LocationDto>(Error.Conflict("BUS-LOC-005", "Bu depoda aynı lokasyon kodu zaten var."));

        var e = Location.Create(
            code,
            name,
            warehouseCode,
            locationType,
            status,
            command.Description?.Trim() ?? string.Empty,
            plantId: command.PlantId,
            stockZoneType: command.StockZoneType);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(LocationMapper.ToDto(e));
    }
}

public sealed class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand, Result<LocationDto>>
{
    private readonly ILocationRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public UpdateLocationCommandHandler(ILocationRepository repo, IWarehouseRepository warehouses, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<LocationDto>> HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<LocationDto>(Error.NotFound("BUS-001", "Location was not found."));
        if (!PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<LocationDto>(Error.Forbidden("BUS-LOC-403", "Bu tesiste lokasyon değiştirme yetkiniz yok."));

        var warehouseCode = string.IsNullOrWhiteSpace(command.WarehouseCode)
            ? e.WarehouseCode
            : command.WarehouseCode.Trim().ToUpperInvariant();
        var warehouse = await _warehouses.GetByCodeAndPlantAsync(warehouseCode, e.PlantId, cancellationToken).ConfigureAwait(false);
        if (warehouse is null || warehouse.IsDeleted)
            return Result.Failure<LocationDto>(Error.Validation("BUS-LOC-002", "Seçilen depo bu tesise ait değil veya bulunamadı."));

        var locationType = LocationTypes.Normalize(command.LocationType);
        if (!LocationTypes.IsKnown(locationType))
            return Result.Failure<LocationDto>(Error.Validation("BUS-LOC-004", $"Bilinmeyen lokasyon tipi '{command.LocationType}'."));

        var code = string.IsNullOrWhiteSpace(command.Code) ? e.Code : command.Code.Trim().ToUpperInvariant();
        var name = string.IsNullOrWhiteSpace(command.Name) ? code : command.Name.Trim();
        var status = string.IsNullOrWhiteSpace(command.Status) ? e.Status : command.Status.Trim();

        var duplicate = await _repo.FindByWarehouseAndCodeAsync(warehouseCode, code, e.PlantId, cancellationToken).ConfigureAwait(false);
        if (duplicate is not null && !duplicate.IsDeleted && duplicate.Id != e.Id)
            return Result.Failure<LocationDto>(Error.Conflict("BUS-LOC-005", "Bu depoda aynı lokasyon kodu zaten var."));

        e.Update(code, name, warehouseCode, locationType, status, command.Description?.Trim() ?? e.Description, command.StockZoneType);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(LocationMapper.ToDto(e));
    }
}

public sealed class DeleteLocationCommandHandler : ICommandHandler<DeleteLocationCommand, Result>
{
    private readonly ILocationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public DeleteLocationCommandHandler(ILocationRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(DeleteLocationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure(Error.NotFound("BUS-001", "Location was not found."));
        if (!PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure(Error.Forbidden("BUS-LOC-403", "Bu tesiste lokasyon silme yetkiniz yok."));

        // Soft-delete / Inactive only — never hard delete. History with movements keeps Inactive.
        var hasHistory = await _repo.HasStockOrMovementAsync(e.WarehouseCode, e.Code, e.PlantId, cancellationToken).ConfigureAwait(false);
        if (hasHistory)
        {
            e.Update(e.Code, e.Name, e.WarehouseCode, e.LocationType, "Inactive", e.Description);
        }
        else
        {
            e.SoftDelete();
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

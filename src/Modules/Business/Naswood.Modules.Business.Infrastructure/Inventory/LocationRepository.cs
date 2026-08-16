using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class LocationRepository : ILocationRepository
{
    private readonly BusinessDbContext _db;
    public LocationRepository(BusinessDbContext db) => _db = db;

    public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<Location>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Location?> FindByWarehouseAndCodeAsync(
        string warehouseCode,
        string code,
        string? plantId,
        CancellationToken cancellationToken = default)
    {
        var wh = (warehouseCode ?? string.Empty).Trim().ToLowerInvariant();
        var loc = (code ?? string.Empty).Trim().ToLowerInvariant();
        var plant = (plantId ?? string.Empty).Trim();
        return _db.Set<Location>().FirstOrDefaultAsync(
            x => !x.IsDeleted
                && x.WarehouseCode.ToLower() == wh
                && x.Code.ToLower() == loc
                && (plant.Length == 0 || x.PlantId == plant),
            cancellationToken);
    }

    public async Task AddAsync(Location entity, CancellationToken cancellationToken = default) =>
        await _db.Set<Location>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<Location> Items, int Total)> SearchAsync(
        string? q,
        string? plantId,
        string? warehouseCode,
        string? locationType,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Location>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }

        if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            var wh = warehouseCode.Trim().ToLowerInvariant();
            query = query.Where(x => x.WarehouseCode.ToLower() == wh);
        }

        if (!string.IsNullOrWhiteSpace(locationType))
        {
            var typ = locationType.Trim().ToUpperInvariant();
            query = query.Where(x => x.LocationType.ToUpper() == typ);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, "%" + value + "%")
                || EF.Functions.ILike(x.Code, "%" + value + "%")
                || EF.Functions.ILike(x.WarehouseCode, "%" + value + "%")
                || EF.Functions.ILike(x.LocationType, "%" + value + "%")
                || EF.Functions.ILike(x.Description, "%" + value + "%"));
        }

        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderBy(x => x.WarehouseCode).ThenBy(x => x.Code)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    public async Task<bool> HasStockOrMovementAsync(
        string warehouseCode,
        string locationCode,
        string? plantId,
        CancellationToken cancellationToken = default)
    {
        var wh = warehouseCode.Trim().ToLowerInvariant();
        var loc = locationCode.Trim().ToLowerInvariant();
        var plant = (plantId ?? string.Empty).Trim();

        var hasBalance = await _db.Set<InventoryBalance>().AsNoTracking().AnyAsync(
            x => !x.IsDeleted
                && x.WarehouseCode.ToLower() == wh
                && x.LocationCode.ToLower() == loc
                && (plant.Length == 0 || x.PlantId == plant)
                && x.QuantityOnHand != 0,
            cancellationToken).ConfigureAwait(false);
        if (hasBalance) return true;

        // Any historical movement referencing this location (via balance key or package).
        var hasPkg = await _db.Set<InventoryPackage>().AsNoTracking().AnyAsync(
            x => !x.IsDeleted
                && x.WarehouseCode.ToLower() == wh
                && x.LocationCode.ToLower() == loc
                && (plant.Length == 0 || x.PlantId == plant),
            cancellationToken).ConfigureAwait(false);
        return hasPkg;
    }
}

using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class WarehouseRepository : IWarehouseRepository
{
    private readonly BusinessDbContext _db;
    public WarehouseRepository(BusinessDbContext db) => _db = db;

    public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<Warehouse>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var value = (code ?? string.Empty).Trim();
        if (value.Length == 0)
            return Task.FromResult<Warehouse?>(null);
        var lower = value.ToLowerInvariant();
        return _db.Set<Warehouse>().FirstOrDefaultAsync(
            x => !x.IsDeleted && x.Code.ToLower() == lower,
            cancellationToken);
    }

    public Task<Warehouse?> GetByCodeAndPlantAsync(string code, string? plantId, CancellationToken cancellationToken = default)
    {
        var value = (code ?? string.Empty).Trim();
        if (value.Length == 0)
            return Task.FromResult<Warehouse?>(null);
        var lower = value.ToLowerInvariant();
        var plant = (plantId ?? string.Empty).Trim();
        return _db.Set<Warehouse>().FirstOrDefaultAsync(
            x => !x.IsDeleted
                && x.Code.ToLower() == lower
                && (plant.Length == 0 || x.PlantId == plant),
            cancellationToken);
    }

    public async Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default) =>
        await _db.Set<Warehouse>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<Warehouse> Items, int Total)> SearchAsync(
        string? q,
        string? plantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Warehouse>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, "%" + value + "%")
                || EF.Functions.ILike(x.Code, "%" + value + "%")
                || EF.Functions.ILike(x.WarehouseType, "%" + value + "%")
                || EF.Functions.ILike(x.Description, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    public async Task<bool> HasLocationsOrStockAsync(string warehouseCode, string? plantId, CancellationToken cancellationToken = default)
    {
        var wh = warehouseCode.Trim().ToLowerInvariant();
        var plant = (plantId ?? string.Empty).Trim();

        var hasLoc = await _db.Set<Location>().AsNoTracking().AnyAsync(
            x => !x.IsDeleted
                && x.WarehouseCode.ToLower() == wh
                && (plant.Length == 0 || x.PlantId == plant),
            cancellationToken).ConfigureAwait(false);
        if (hasLoc) return true;

        var hasStock = await _db.Set<InventoryBalance>().AsNoTracking().AnyAsync(
            x => !x.IsDeleted
                && x.WarehouseCode.ToLower() == wh
                && (plant.Length == 0 || x.PlantId == plant)
                && x.QuantityOnHand != 0,
            cancellationToken).ConfigureAwait(false);
        return hasStock;
    }
}

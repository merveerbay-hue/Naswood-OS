using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class InventoryBalanceRepository : IInventoryBalanceRepository
{
    private readonly BusinessDbContext _db;
    public InventoryBalanceRepository(BusinessDbContext db) => _db = db;

    public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<InventoryBalance>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<InventoryBalance?> FindByKeyAsync(
        string materialCode,
        string warehouseCode,
        string locationCode,
        string batchNumber,
        string? plantId = null,
        CancellationToken cancellationToken = default)
    {
        var plant = (plantId ?? string.Empty).Trim();
        return _db.Set<InventoryBalance>().FirstOrDefaultAsync(
            x => !x.IsDeleted
                && x.MaterialCode == materialCode
                && x.WarehouseCode == warehouseCode
                && x.LocationCode == locationCode
                && x.BatchNumber == batchNumber
                && (plant.Length == 0 || x.PlantId == plant),
            cancellationToken);
    }

    public async Task AddAsync(InventoryBalance entity, CancellationToken cancellationToken = default) =>
        await _db.Set<InventoryBalance>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<InventoryBalance> Items, int Total)> SearchAsync(
        string? q,
        int page,
        int pageSize,
        string? plantId = null,
        string? warehouseCode = null,
        string? locationCode = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Set<InventoryBalance>().AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }

        if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            var wh = warehouseCode.Trim();
            query = query.Where(x => x.WarehouseCode == wh);
        }

        if (!string.IsNullOrWhiteSpace(locationCode))
        {
            var loc = locationCode.Trim();
            query = query.Where(x => x.LocationCode == loc);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x => EF.Functions.ILike(x.MaterialCode, "%" + value + "%"));
        }

        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    public async Task<IReadOnlyList<InventoryBalance>> ListForCountSnapshotAsync(
        string plantId,
        string warehouseCode,
        string? locationCode,
        CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        var wh = warehouseCode.Trim();
        var query = _db.Set<InventoryBalance>().Where(x => !x.IsDeleted && x.PlantId == plant && x.WarehouseCode == wh);
        if (!string.IsNullOrWhiteSpace(locationCode))
        {
            var loc = locationCode.Trim();
            query = query.Where(x => x.LocationCode == loc);
        }
        return await query.OrderBy(x => x.MaterialCode).ThenBy(x => x.LocationCode).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}

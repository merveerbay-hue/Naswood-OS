using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class InventoryCountRepository : IInventoryCountRepository
{
    private readonly BusinessDbContext _db;
    public InventoryCountRepository(BusinessDbContext db) => _db = db;

    public Task<InventoryCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<InventoryCount>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(InventoryCount entity, CancellationToken cancellationToken = default) =>
        await _db.Set<InventoryCount>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<InventoryCount> Items, int Total)> SearchAsync(string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<InventoryCount>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Number, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    public async Task<IReadOnlyList<InventoryCountLine>> ListLinesAsync(Guid countId, CancellationToken cancellationToken = default) =>
        await _db.Set<InventoryCountLine>()
            .Where(x => x.CountId == countId && !x.IsDeleted)
            .OrderBy(x => x.LineNo)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task AddLineAsync(InventoryCountLine line, CancellationToken cancellationToken = default) =>
        await _db.Set<InventoryCountLine>().AddAsync(line, cancellationToken).ConfigureAwait(false);

    public void RemoveLine(InventoryCountLine line) => line.SoftDelete();
}

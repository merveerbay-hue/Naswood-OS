using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class GoodsReceiptRepository : IGoodsReceiptRepository
{
    private readonly BusinessDbContext _db;
    public GoodsReceiptRepository(BusinessDbContext db) => _db = db;

    public Task<GoodsReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<GoodsReceipt>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<GoodsReceipt?> GetByNumberAsync(string number, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<GoodsReceipt>().Where(x => !x.IsDeleted && x.Number == number);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(GoodsReceipt entity, CancellationToken cancellationToken = default) =>
        await _db.Set<GoodsReceipt>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<GoodsReceipt> Items, int Total)> SearchAsync(string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<GoodsReceipt>().AsNoTracking().Where(x => !x.IsDeleted);
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

    public Task<int> CountOpenAsync(string plantId, CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        return _db.Set<GoodsReceipt>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.PlantId == plant)
            .Where(x =>
                x.Status.ToLower() != "posted"
                && x.Status.ToLower() != "cancelled"
                && x.Status.ToLower() != "closed")
            .CountAsync(cancellationToken);
    }
}

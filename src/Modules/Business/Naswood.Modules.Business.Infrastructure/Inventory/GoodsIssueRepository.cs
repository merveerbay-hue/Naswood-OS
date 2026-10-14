using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class GoodsIssueRepository : IGoodsIssueRepository
{
    private readonly BusinessDbContext _db;
    public GoodsIssueRepository(BusinessDbContext db) => _db = db;

    public Task<GoodsIssue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<GoodsIssue>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(GoodsIssue entity, CancellationToken cancellationToken = default) =>
        await _db.Set<GoodsIssue>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<GoodsIssue> Items, int Total)> SearchAsync(string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<GoodsIssue>().AsNoTracking().Where(x => !x.IsDeleted);
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
        return _db.Set<GoodsIssue>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.PlantId == plant)
            .Where(x =>
                x.Status.ToLower() != "posted"
                && x.Status.ToLower() != "cancelled"
                && x.Status.ToLower() != "closed")
            .CountAsync(cancellationToken);
    }
}

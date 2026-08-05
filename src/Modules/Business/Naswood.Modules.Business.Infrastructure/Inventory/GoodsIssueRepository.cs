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

    public async Task<(IReadOnlyList<GoodsIssue> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<GoodsIssue>().AsNoTracking().Where(x => !x.IsDeleted);
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
}

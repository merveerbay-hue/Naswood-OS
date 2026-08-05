using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class BatchRepository : IBatchRepository
{
    private readonly BusinessDbContext _db;
    public BatchRepository(BusinessDbContext db) => _db = db;

    public Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<Batch>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Batch entity, CancellationToken cancellationToken = default) =>
        await _db.Set<Batch>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<Batch> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Batch>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x => EF.Functions.ILike(x.BatchNumber, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }
}

using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Purchasing;

public sealed class RfqRepository : IRfqRepository
{
    private readonly BusinessDbContext _db;
    public RfqRepository(BusinessDbContext db) => _db = db;

    public Task<Rfq?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<Rfq>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Rfq entity, CancellationToken cancellationToken = default) =>
        await _db.Set<Rfq>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<Rfq> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Rfq>().AsNoTracking().Where(x => !x.IsDeleted);
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

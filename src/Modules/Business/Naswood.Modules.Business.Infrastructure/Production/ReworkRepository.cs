using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Production;

public sealed class ReworkRepository : IReworkRepository
{
    private readonly BusinessDbContext _db;
    public ReworkRepository(BusinessDbContext db) => _db = db;

    public Task<Rework?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<Rework>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Rework entity, CancellationToken cancellationToken = default) =>
        await _db.Set<Rework>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<Rework> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Rework>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Name, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }
}

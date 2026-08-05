using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Production;

public sealed class MaterialConsumptionRepository : IMaterialConsumptionRepository
{
    private readonly BusinessDbContext _db;
    public MaterialConsumptionRepository(BusinessDbContext db) => _db = db;

    public Task<MaterialConsumption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<MaterialConsumption>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(MaterialConsumption entity, CancellationToken cancellationToken = default) =>
        await _db.Set<MaterialConsumption>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<MaterialConsumption> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<MaterialConsumption>().AsNoTracking().Where(x => !x.IsDeleted);
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

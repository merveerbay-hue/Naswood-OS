using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Purchasing;

public sealed class SupplierQuotationRepository : ISupplierQuotationRepository
{
    private readonly BusinessDbContext _db;
    public SupplierQuotationRepository(BusinessDbContext db) => _db = db;

    public Task<SupplierQuotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<SupplierQuotation>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(SupplierQuotation entity, CancellationToken cancellationToken = default) =>
        await _db.Set<SupplierQuotation>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<SupplierQuotation> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<SupplierQuotation>().AsNoTracking().Where(x => !x.IsDeleted);
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

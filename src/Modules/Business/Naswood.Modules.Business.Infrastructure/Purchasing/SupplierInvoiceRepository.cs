using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Purchasing;

public sealed class SupplierInvoiceRepository : ISupplierInvoiceRepository
{
    private readonly BusinessDbContext _db;
    public SupplierInvoiceRepository(BusinessDbContext db) => _db = db;

    public Task<SupplierInvoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<SupplierInvoice>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(SupplierInvoice entity, CancellationToken cancellationToken = default) =>
        await _db.Set<SupplierInvoice>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<SupplierInvoice> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<SupplierInvoice>().AsNoTracking().Where(x => !x.IsDeleted);
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

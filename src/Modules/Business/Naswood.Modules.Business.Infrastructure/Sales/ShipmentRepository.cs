using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Sales;
using Naswood.Modules.Business.Domain.Sales;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Sales;

public sealed class ShipmentRepository : IShipmentRepository
{
    private readonly BusinessDbContext _db;
    public ShipmentRepository(BusinessDbContext db) => _db = db;

    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<Shipment>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Shipment entity, CancellationToken cancellationToken = default) =>
        await _db.Set<Shipment>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<Shipment> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Shipment>().AsNoTracking().Where(x => !x.IsDeleted);
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

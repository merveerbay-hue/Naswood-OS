using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class StockTransferRepository : IStockTransferRepository
{
    private readonly BusinessDbContext _db;
    public StockTransferRepository(BusinessDbContext db) => _db = db;

    public Task<StockTransfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<StockTransfer>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(StockTransfer entity, CancellationToken cancellationToken = default) =>
        await _db.Set<StockTransfer>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<StockTransfer> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<StockTransfer>().AsNoTracking().Where(x => !x.IsDeleted);
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

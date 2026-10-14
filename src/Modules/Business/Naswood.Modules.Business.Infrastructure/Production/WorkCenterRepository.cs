using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Production;

public sealed class WorkCenterRepository : IWorkCenterRepository
{
    private readonly BusinessDbContext _db;
    public WorkCenterRepository(BusinessDbContext db) => _db = db;

    public Task<WorkCenter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<WorkCenter>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<WorkCenter?> GetByCodeAndPlantAsync(string code, string plantId, CancellationToken cancellationToken = default)
    {
        var c = code.Trim();
        var p = plantId.Trim();
        return _db.Set<WorkCenter>().FirstOrDefaultAsync(
            x => !x.IsDeleted && x.Code == c && x.PlantId == p,
            cancellationToken);
    }

    public async Task AddAsync(WorkCenter entity, CancellationToken cancellationToken = default) =>
        await _db.Set<WorkCenter>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<WorkCenter> Items, int Total)> SearchAsync(
        string? q,
        string? plantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Set<WorkCenter>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, "%" + value + "%")
                || EF.Functions.ILike(x.Code, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }
}

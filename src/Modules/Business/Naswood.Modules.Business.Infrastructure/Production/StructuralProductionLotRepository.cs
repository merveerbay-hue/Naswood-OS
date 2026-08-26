using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Production;

public sealed class StructuralProductionLotRepository : IStructuralProductionLotRepository
{
    private readonly BusinessDbContext _db;
    public StructuralProductionLotRepository(BusinessDbContext db) => _db = db;

    public Task<StructuralProductionLot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<StructuralProductionLot>()
            .Include(x => x.Inputs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> NumberExistsAsync(string productionLotNumber, CancellationToken cancellationToken = default) =>
        _db.Set<StructuralProductionLot>()
            .AnyAsync(x => !x.IsDeleted && x.ProductionLotNumber == productionLotNumber, cancellationToken);

    public Task<int> CountByPlantAsync(string plantId, CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        return _db.Set<StructuralProductionLot>()
            .CountAsync(x => !x.IsDeleted && x.PlantId == plant, cancellationToken);
    }

    public async Task AddAsync(StructuralProductionLot entity, CancellationToken cancellationToken = default) =>
        await _db.Set<StructuralProductionLot>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<StructuralProductionLot> Items, int Total)> SearchAsync(
        string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<StructuralProductionLot>()
            .AsNoTracking()
            .Include(x => x.Inputs)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.ProductionLotNumber, "%" + value + "%")
                || EF.Functions.ILike(x.MaterialCode, "%" + value + "%")
                || EF.Functions.ILike(x.ActualGradingMethod, "%" + value + "%")
                || EF.Functions.ILike(x.Status, "%" + value + "%"));
        }

        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }
}

using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Production;

public sealed class ProductionOutputRepository : IProductionOutputRepository
{
    private readonly BusinessDbContext _db;
    public ProductionOutputRepository(BusinessDbContext db) => _db = db;

    public Task AddAsync(ProductionOutput entity, CancellationToken cancellationToken = default)
        => _db.Set<ProductionOutput>().AddAsync(entity, cancellationToken).AsTask();

    public Task<ProductionOutput?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionOutput>().Local.FirstOrDefault(x => x.Id == id);
        return local is not null
            ? Task.FromResult<ProductionOutput?>(local)
            : _db.Set<ProductionOutput>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<ProductionOutput?> GetByNumberAsync(string number, string? plantId, CancellationToken cancellationToken = default)
    {
        var key = (number ?? string.Empty).Trim();
        if (key.Length == 0) return Task.FromResult<ProductionOutput?>(null);
        var query = _db.Set<ProductionOutput>().Where(x => !x.IsDeleted && x.Number == key);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ProductionOutput?> GetByExecutionIdAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionOutput>().Local.FirstOrDefault(x => !x.IsDeleted && x.ProductionOperationExecutionId == executionId);
        return local is not null
            ? Task.FromResult<ProductionOutput?>(local)
            : _db.Set<ProductionOutput>().FirstOrDefaultAsync(
                x => !x.IsDeleted && x.ProductionOperationExecutionId == executionId, cancellationToken);
    }
}

public sealed class ProductionLotSourceRepository : IProductionLotSourceRepository
{
    private readonly BusinessDbContext _db;
    public ProductionLotSourceRepository(BusinessDbContext db) => _db = db;

    public Task AddRangeAsync(IReadOnlyList<ProductionLotSource> rows, CancellationToken cancellationToken = default)
        => _db.Set<ProductionLotSource>().AddRangeAsync(rows, cancellationToken);

    public async Task<IReadOnlyList<ProductionLotSource>> ListByProductionLotIdAsync(Guid productionLotId, CancellationToken cancellationToken = default)
        => await _db.Set<ProductionLotSource>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.ProductionLotId == productionLotId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<ProductionLotSource>> ListByOutputIdAsync(Guid productionOutputId, CancellationToken cancellationToken = default)
        => await _db.Set<ProductionLotSource>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.ProductionOutputId == productionOutputId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<ProductionLotSource>> ListBySourcePackageIdsAsync(IReadOnlyList<Guid> packageIds, CancellationToken cancellationToken = default)
    {
        var ids = packageIds.Distinct().ToArray();
        if (ids.Length == 0) return Array.Empty<ProductionLotSource>();
        return await _db.Set<ProductionLotSource>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.SourcePackageId != null && ids.Contains(x.SourcePackageId.Value))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

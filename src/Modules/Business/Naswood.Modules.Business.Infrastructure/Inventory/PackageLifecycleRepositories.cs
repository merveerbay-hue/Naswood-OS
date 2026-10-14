using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class PackageOperationRepository : IPackageOperationRepository
{
    private readonly BusinessDbContext _db;
    public PackageOperationRepository(BusinessDbContext db) => _db = db;

    public Task AddAsync(PackageOperation entity, CancellationToken cancellationToken = default)
        => _db.Set<PackageOperation>().AddAsync(entity, cancellationToken).AsTask();

    public Task<PackageOperation?> GetByNumberAsync(string number, string? plantId, CancellationToken cancellationToken = default)
    {
        var key = (number ?? "").Trim();
        if (key.Length == 0) return Task.FromResult<PackageOperation?>(null);
        var q = _db.Set<PackageOperation>().Where(x => !x.IsDeleted && x.Number == key);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            q = q.Where(x => x.PlantId == plant);
        }
        return q.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ListNumbersAsync(string? plantId, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<PackageOperation>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            q = q.Where(x => x.PlantId == plant);
        }
        return await q.Select(x => x.Number).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}

public sealed class PackageRelationRepository : IPackageRelationRepository
{
    private readonly BusinessDbContext _db;
    public PackageRelationRepository(BusinessDbContext db) => _db = db;

    public Task AddRangeAsync(IReadOnlyList<PackageRelation> rows, CancellationToken cancellationToken = default)
        => _db.Set<PackageRelation>().AddRangeAsync(rows, cancellationToken);

    public async Task<IReadOnlyList<PackageRelation>> ListByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default)
        => await _db.Set<PackageRelation>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.OperationId == operationId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<PackageRelation>> ListByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
        => await _db.Set<PackageRelation>().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.SourcePackageId == packageId || x.TargetPackageId == packageId))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<PackageRelation>> ListByPackageIdsAsync(IReadOnlyList<Guid> packageIds, CancellationToken cancellationToken = default)
    {
        var ids = packageIds.Distinct().ToArray();
        if (ids.Length == 0) return Array.Empty<PackageRelation>();
        return await _db.Set<PackageRelation>().AsNoTracking()
            .Where(x => !x.IsDeleted && (ids.Contains(x.SourcePackageId) || ids.Contains(x.TargetPackageId)))
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}

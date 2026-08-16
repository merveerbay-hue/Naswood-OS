using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Inventory;

public sealed class MaterialIdentityRepository : IMaterialIdentityRepository
{
    private readonly BusinessDbContext _db;
    public MaterialIdentityRepository(BusinessDbContext db) => _db = db;

    public async Task AddAsync(MaterialIdentity entity, CancellationToken cancellationToken = default) =>
        await _db.Set<MaterialIdentity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public Task<MaterialIdentity?> GetByNumberAsync(string identityNumber, CancellationToken cancellationToken = default) =>
        _db.Set<MaterialIdentity>().FirstOrDefaultAsync(x => !x.IsDeleted && x.IdentityNumber == identityNumber, cancellationToken);

    public async Task<(IReadOnlyList<MaterialIdentity> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<MaterialIdentity>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.IdentityNumber, "%" + value + "%")
                || EF.Functions.ILike(x.MaterialCode, "%" + value + "%")
                || EF.Functions.ILike(x.LotNumber, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }
}

public sealed class InventoryPackageRepository : IInventoryPackageRepository
{
    private readonly BusinessDbContext _db;
    public InventoryPackageRepository(BusinessDbContext db) => _db = db;

    public async Task AddAsync(InventoryPackage entity, CancellationToken cancellationToken = default) =>
        await _db.Set<InventoryPackage>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public Task<InventoryPackage?> GetByNumberAsync(string packageNumber, CancellationToken cancellationToken = default) =>
        _db.Set<InventoryPackage>().FirstOrDefaultAsync(
            x => !x.IsDeleted && (x.PackageNumber == packageNumber || x.Barcode == packageNumber),
            cancellationToken);

    public async Task<(IReadOnlyList<InventoryPackage> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<InventoryPackage>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.PackageNumber, "%" + value + "%")
                || EF.Functions.ILike(x.Barcode, "%" + value + "%")
                || EF.Functions.ILike(x.MaterialCode, "%" + value + "%")
                || EF.Functions.ILike(x.MaterialIdentityNumber, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }
}

public sealed class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly BusinessDbContext _db;
    public InventoryMovementRepository(BusinessDbContext db) => _db = db;

    public async Task AddAsync(InventoryMovement entity, CancellationToken cancellationToken = default) =>
        await _db.Set<InventoryMovement>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<InventoryMovement> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<InventoryMovement>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var value = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.MovementNumber, "%" + value + "%")
                || EF.Functions.ILike(x.DocumentNumber, "%" + value + "%")
                || EF.Functions.ILike(x.MaterialCode, "%" + value + "%")
                || EF.Functions.ILike(x.PackageNumber, "%" + value + "%"));
        }
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return (items, total);
    }

    public async Task<IReadOnlyList<InventoryMovement>> ListByDocumentAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        var value = documentNumber.Trim();
        return await _db.Set<InventoryMovement>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.DocumentNumber == value)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

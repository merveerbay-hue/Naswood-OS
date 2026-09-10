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

    public Task<MaterialIdentity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<MaterialIdentity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public Task<MaterialIdentity?> GetByNumberAsync(
        string identityNumber,
        string? plantId = null,
        CancellationToken cancellationToken = default)
    {
        var key = identityNumber.Trim();
        var query = _db.Set<MaterialIdentity>().Where(x => !x.IsDeleted && x.IdentityNumber == key);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<MaterialIdentity> Items, int Total)> SearchAsync(
        string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<MaterialIdentity>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
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

    public Task<InventoryPackage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<InventoryPackage>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public Task<InventoryPackage?> GetByNumberAsync(
        string packageNumber,
        string? plantId = null,
        CancellationToken cancellationToken = default)
    {
        var key = packageNumber.Trim();
        var query = _db.Set<InventoryPackage>().Where(x =>
            !x.IsDeleted && (x.PackageNumber == key || x.Barcode == key));
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<InventoryPackage> Items, int Total)> SearchAsync(
        string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<InventoryPackage>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
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

    public async Task<int> CountByStatusesAsync(
        string? plantId,
        IReadOnlyList<string> statuses,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Set<InventoryPackage>().AsNoTracking().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }

        var normalized = statuses
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLower())
            .Distinct()
            .ToArray();
        if (normalized.Length == 0)
            return 0;

        query = query.Where(x => normalized.Contains(x.Status.ToLower()));
        return await query.CountAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<string>> ListPackageNumbersAsync(CancellationToken cancellationToken = default)
        => await _db.Set<InventoryPackage>().AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => x.PackageNumber)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}

public sealed class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly BusinessDbContext _db;
    public InventoryMovementRepository(BusinessDbContext db) => _db = db;

    public async Task AddAsync(InventoryMovement entity, CancellationToken cancellationToken = default) =>
        await _db.Set<InventoryMovement>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Set<InventoryMovement>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<InventoryMovement> Items, int Total)> SearchAsync(
        string? q,
        int page,
        int pageSize,
        string? plantId = null,
        string? warehouseCode = null,
        string? locationCode = null,
        string? documentNumber = null,
        string? materialCode = null,
        string? lotNumber = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Set<InventoryMovement>().AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
        if (!string.IsNullOrWhiteSpace(warehouseCode))
        {
            var wh = warehouseCode.Trim();
            query = query.Where(x => x.WarehouseCode == wh);
        }
        if (!string.IsNullOrWhiteSpace(locationCode))
        {
            var loc = locationCode.Trim();
            query = query.Where(x => x.LocationCode == loc);
        }
        if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            var doc = documentNumber.Trim();
            query = query.Where(x => x.DocumentNumber == doc);
        }
        if (!string.IsNullOrWhiteSpace(materialCode))
        {
            var mat = materialCode.Trim();
            query = query.Where(x => x.MaterialCode == mat);
        }
        if (!string.IsNullOrWhiteSpace(lotNumber))
        {
            var lot = lotNumber.Trim();
            query = query.Where(x => x.LotNumber == lot);
        }
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

    public async Task<IReadOnlyList<InventoryMovement>> ListByDocumentAsync(
        string documentNumber, string? plantId = null, CancellationToken cancellationToken = default)
    {
        var value = documentNumber.Trim();
        var query = _db.Set<InventoryMovement>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.DocumentNumber == value);
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var plant = plantId.Trim();
            query = query.Where(x => x.PlantId == plant);
        }
        return await query.OrderBy(x => x.CreatedAt).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> CountPostedAfterAsync(
        string plantId,
        string warehouseCode,
        string? locationCode,
        DateTimeOffset after,
        string? excludeDocumentNumber,
        CancellationToken cancellationToken = default)
    {
        var plant = plantId.Trim();
        var wh = warehouseCode.Trim();
        var query = _db.Set<InventoryMovement>().AsNoTracking()
            .Where(x => !x.IsDeleted && x.PlantId == plant && x.WarehouseCode == wh && x.CreatedAt >= after);
        if (!string.IsNullOrWhiteSpace(locationCode))
        {
            var loc = locationCode.Trim();
            query = query.Where(x => x.LocationCode == loc);
        }
        if (!string.IsNullOrWhiteSpace(excludeDocumentNumber))
        {
            var doc = excludeDocumentNumber.Trim();
            query = query.Where(x => x.DocumentNumber != doc);
        }
        return await query.CountAsync(cancellationToken).ConfigureAwait(false);
    }
}

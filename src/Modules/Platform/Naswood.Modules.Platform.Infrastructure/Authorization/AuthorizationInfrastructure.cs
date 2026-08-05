using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Modules.Platform.Infrastructure.Authorization;

public sealed class PermissionCatalogRepository : IPermissionCatalogRepository, IPermissionManagementRepository
{
    private readonly PlatformDbContext _db;

    public PermissionCatalogRepository(PlatformDbContext db) => _db = db;

    public async Task<IReadOnlyList<PermissionDefinition>> GetAllActiveAsync(
        CancellationToken cancellationToken = default) =>
        await _db.Permissions.AsNoTracking()
            .Where(p => p.IsActive && !p.IsDeleted)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _db.Permissions.AnyAsync(cancellationToken);

    public async Task AddRangeAsync(
        IEnumerable<PermissionDefinition> permissions,
        CancellationToken cancellationToken = default) =>
        await _db.Permissions.AddRangeAsync(permissions, cancellationToken).ConfigureAwait(false);

    public Task<PermissionDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Permissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<PermissionDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        return _db.Permissions.FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        string code,
        Guid? excludingId,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        return _db.Permissions.AnyAsync(
            x => !x.IsDeleted &&
                 x.Code == normalized &&
                 (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);
    }

    public async Task AddAsync(PermissionDefinition permission, CancellationToken cancellationToken = default) =>
        await _db.Permissions.AddAsync(permission, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<PermissionDefinition> Items, int TotalCount)> SearchAsync(
        PermissionSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Permissions.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(criteria.Code))
        {
            var value = criteria.Code.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Code, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Module))
        {
            var value = criteria.Module.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Module, value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Feature))
        {
            var value = criteria.Feature.Trim();
            query = query.Where(x => x.Entity != null && EF.Functions.ILike(x.Entity, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Action))
        {
            var value = criteria.Action.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Action, value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Category))
        {
            var value = criteria.Category.Trim();
            query = query.Where(x => x.Category != null && EF.Functions.ILike(x.Category, $"%{value}%"));
        }

        if (criteria.IsActive is not null)
        {
            query = query.Where(x => x.IsActive == criteria.IsActive);
        }

        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(x => x.Code)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, total);
    }
}

public sealed class RoleCatalogRepository : IRoleCatalogRepository, IRoleManagementRepository
{
    private readonly PlatformDbContext _db;

    public RoleCatalogRepository(PlatformDbContext db) => _db = db;

    public async Task<IReadOnlyList<RoleDefinition>> GetAllActiveAsync(
        CancellationToken cancellationToken = default) =>
        await _db.Roles.AsNoTracking()
            .Where(r => r.IsActive && !r.IsDeleted)
            .OrderBy(r => r.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<RoleDefinition>> GetByCodesAsync(
        IEnumerable<string> codes,
        CancellationToken cancellationToken = default)
    {
        var codeList = codes.ToArray();
        return await _db.Roles
            .Where(r => !r.IsDeleted && codeList.Contains(r.Code))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _db.Roles.AnyAsync(cancellationToken);

    public async Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default) =>
        await _db.Roles.AddAsync(role, cancellationToken).ConfigureAwait(false);

    public Task<RoleDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<RoleDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        return _db.Roles.FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        string code,
        Guid? excludingRoleId,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        return _db.Roles.AnyAsync(
            x => !x.IsDeleted &&
                 x.Code == normalized &&
                 (!excludingRoleId.HasValue || x.Id != excludingRoleId.Value),
            cancellationToken);
    }

    public Task<bool> NameExistsInCompanyAsync(
        string name,
        string? companyCode,
        Guid? excludingRoleId,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        var normalizedCompany = string.IsNullOrWhiteSpace(companyCode)
            ? null
            : companyCode.Trim().ToUpperInvariant();

        return _db.Roles.AnyAsync(
            x => !x.IsDeleted &&
                 x.Name == normalizedName &&
                 x.CompanyCode == normalizedCompany &&
                 (!excludingRoleId.HasValue || x.Id != excludingRoleId.Value),
            cancellationToken);
    }

    public async Task<(IReadOnlyList<RoleDefinition> Items, int TotalCount)> SearchAsync(
        RoleSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Roles.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(criteria.Code))
        {
            var value = criteria.Code.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Code, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Name))
        {
            var value = criteria.Name.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.CompanyCode))
        {
            var value = criteria.CompanyCode.Trim().ToUpper();
            query = query.Where(x => x.CompanyCode == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.PlantCode))
        {
            var value = criteria.PlantCode.Trim().ToUpper();
            query = query.Where(x => x.PlantCode == value);
        }

        if (criteria.IsActive is not null)
        {
            query = query.Where(x => x.IsActive == criteria.IsActive);
        }

        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderBy(x => x.Code)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, total);
    }

    public async Task<IReadOnlyList<Guid>> FindUserIdsByRoleCodeAsync(
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        var users = await _db.AuthUsers.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return users
            .Where(u => u.Roles.Contains(roleCode, StringComparer.OrdinalIgnoreCase))
            .Select(u => u.Id)
            .ToArray();
    }
}

public sealed class AuthorizationHistoryRepository : IAuthorizationHistoryRepository
{
    private readonly PlatformDbContext _db;

    public AuthorizationHistoryRepository(PlatformDbContext db) => _db = db;

    public async Task AddAsync(
        AuthorizationHistoryEntry entry,
        CancellationToken cancellationToken = default) =>
        await _db.AuthorizationHistory.AddAsync(entry, cancellationToken).ConfigureAwait(false);
}

public sealed class MemoryPermissionCache : IPermissionCache
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, byte> _keys = new();
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public MemoryPermissionCache(IMemoryCache cache) => _cache = cache;

    public Task<IReadOnlySet<string>?> GetUserPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(Key(userId), out IReadOnlySet<string>? value);
        return Task.FromResult(value);
    }

    public Task SetUserPermissionsAsync(
        Guid userId,
        IReadOnlySet<string> permissions,
        CancellationToken cancellationToken = default)
    {
        var key = Key(userId);
        _keys[key] = 0;
        _cache.Set(key, permissions, Ttl);
        return Task.CompletedTask;
    }

    public void InvalidateUser(Guid userId)
    {
        var key = Key(userId);
        _cache.Remove(key);
        _keys.TryRemove(key, out _);
    }

    public void InvalidateAll()
    {
        foreach (var key in _keys.Keys)
        {
            _cache.Remove(key);
        }

        _keys.Clear();
    }

    private static string Key(Guid userId) => $"authz:user:{userId:D}";
}

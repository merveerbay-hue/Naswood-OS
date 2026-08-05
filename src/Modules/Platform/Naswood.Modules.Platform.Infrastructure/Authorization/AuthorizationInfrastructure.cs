using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Infrastructure.Authorization;

public sealed class PermissionCatalogRepository : IPermissionCatalogRepository
{
    private readonly Persistence.PlatformDbContext _db;

    public PermissionCatalogRepository(Persistence.PlatformDbContext db) => _db = db;

    public async Task<IReadOnlyList<PermissionDefinition>> GetAllActiveAsync(
        CancellationToken cancellationToken = default) =>
        await _db.Permissions.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _db.Permissions.AnyAsync(cancellationToken);

    public async Task AddRangeAsync(
        IEnumerable<PermissionDefinition> permissions,
        CancellationToken cancellationToken = default) =>
        await _db.Permissions.AddRangeAsync(permissions, cancellationToken).ConfigureAwait(false);
}

public sealed class RoleCatalogRepository : IRoleCatalogRepository
{
    private readonly Persistence.PlatformDbContext _db;

    public RoleCatalogRepository(Persistence.PlatformDbContext db) => _db = db;

    public async Task<IReadOnlyList<RoleDefinition>> GetAllActiveAsync(
        CancellationToken cancellationToken = default) =>
        await _db.Roles.AsNoTracking()
            .Where(r => r.IsActive)
            .OrderBy(r => r.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<RoleDefinition>> GetByCodesAsync(
        IEnumerable<string> codes,
        CancellationToken cancellationToken = default)
    {
        var codeList = codes.ToArray();
        return await _db.Roles
            .Where(r => codeList.Contains(r.Code))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _db.Roles.AnyAsync(cancellationToken);

    public async Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default) =>
        await _db.Roles.AddAsync(role, cancellationToken).ConfigureAwait(false);
}

public sealed class AuthorizationHistoryRepository : IAuthorizationHistoryRepository
{
    private readonly Persistence.PlatformDbContext _db;

    public AuthorizationHistoryRepository(Persistence.PlatformDbContext db) => _db = db;

    public async Task AddAsync(
        AuthorizationHistoryEntry entry,
        CancellationToken cancellationToken = default) =>
        await _db.AuthorizationHistory.AddAsync(entry, cancellationToken).ConfigureAwait(false);
}

public sealed class MemoryPermissionCache : IPermissionCache
{
    private readonly IMemoryCache _cache;
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
        _cache.Set(Key(userId), permissions, Ttl);
        return Task.CompletedTask;
    }

    public void InvalidateUser(Guid userId) => _cache.Remove(Key(userId));

    public void InvalidateAll()
    {
        // MemoryCache has no enumerate-all; short TTL bounds staleness until TASK-004 invalidates explicitly.
    }

    private static string Key(Guid userId) => $"authz:user:{userId:D}";
}

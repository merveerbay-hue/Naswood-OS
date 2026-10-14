using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public interface IEffectivePermissionService
{
    Task<Result<(AuthUser User, IReadOnlySet<string> Permissions)>> GetForCurrentUserAsync(
        CancellationToken cancellationToken = default);
}

public sealed class EffectivePermissionService : IEffectivePermissionService
{
    private readonly IAuthUserRepository _users;
    private readonly IAuthRequestContext _context;
    private readonly IRoleCatalogRepository _roles;
    private readonly IPermissionCache _cache;

    public EffectivePermissionService(
        IAuthUserRepository users,
        IAuthRequestContext context,
        IRoleCatalogRepository roles,
        IPermissionCache cache)
    {
        _users = users;
        _context = context;
        _roles = roles;
        _cache = cache;
    }

    public async Task<Result<(AuthUser User, IReadOnlySet<string> Permissions)>> GetForCurrentUserAsync(
        CancellationToken cancellationToken = default)
    {
        if (_context.UserId is null)
        {
            return Result.Failure<(AuthUser, IReadOnlySet<string>)>(AuthorizationErrors.SessionInvalid());
        }

        var user = await _users.GetByIdAsync(_context.UserId.Value, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<(AuthUser, IReadOnlySet<string>)>(AuthorizationErrors.SessionInvalid());
        }

        if (user.Roles.Count == 0)
        {
            return Result.Failure<(AuthUser, IReadOnlySet<string>)>(AuthorizationErrors.RoleRequired());
        }

        var cached = await _cache.GetUserPermissionsAsync(user.Id, cancellationToken).ConfigureAwait(false);
        if (cached is not null)
        {
            return Result.Success((user, cached));
        }

        var roles = await _roles.GetByCodesAsync(user.Roles, cancellationToken).ConfigureAwait(false);
        IReadOnlySet<string> permissions = roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.PermissionCodes)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await _cache.SetUserPermissionsAsync(user.Id, permissions, cancellationToken).ConfigureAwait(false);
        return Result.Success((user, permissions));
    }
}

using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public sealed class AuthorizationEngine : IAuthorizationEngine
{
    private readonly IRoleCatalogRepository _roles;
    private readonly IPermissionCache _cache;
    private readonly IAuthorizationHistoryRepository _history;
    private readonly IOutboxWriter _outbox;
    private readonly IClock _clock;
    private readonly IAuthRequestContext _requestContext;
    private readonly IPlatformUnitOfWork _unitOfWork;

    public AuthorizationEngine(
        IRoleCatalogRepository roles,
        IPermissionCache cache,
        IAuthorizationHistoryRepository history,
        IOutboxWriter outbox,
        IClock clock,
        IAuthRequestContext requestContext,
        IPlatformUnitOfWork unitOfWork)
    {
        _roles = roles;
        _cache = cache;
        _history = history;
        _outbox = outbox;
        _clock = clock;
        _requestContext = requestContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthorizationDecision> EvaluateAsync(
        AuthorizationEvaluationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Permission))
        {
            return AuthorizationDecision.Deny(string.Empty, AuthorizationErrors.PermissionRequired());
        }

        var permission = ResolvePermissionCode(request.Permission, request.Field);

        if (request.RoleCodes.Count == 0)
        {
            return await FinalizeAsync(
                    request,
                    permission,
                    AuthorizationDecision.Deny(permission, AuthorizationErrors.RoleRequired()),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        if (!string.IsNullOrWhiteSpace(request.RequestedCompanyId) &&
            !request.CompanyIds.Contains(request.RequestedCompanyId, StringComparer.OrdinalIgnoreCase))
        {
            return await FinalizeAsync(
                    request,
                    permission,
                    AuthorizationDecision.Deny(permission, AuthorizationErrors.CompanyAccessDenied()),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        if (!string.IsNullOrWhiteSpace(request.RequestedPlantId) &&
            !request.PlantIds.Contains(request.RequestedPlantId, StringComparer.OrdinalIgnoreCase))
        {
            return await FinalizeAsync(
                    request,
                    permission,
                    AuthorizationDecision.Deny(permission, AuthorizationErrors.PlantAccessDenied()),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        var allowed = await HasPermissionAsync(request, permission, cancellationToken).ConfigureAwait(false);

        // Document-level owner access: owner may use an explicit *.Own permission.
        if (!allowed &&
            !string.IsNullOrWhiteSpace(request.ResourceOwnerId) &&
            string.Equals(request.ResourceOwnerId, request.UserId.ToString("D"), StringComparison.OrdinalIgnoreCase))
        {
            allowed = await HasPermissionAsync(request, ToOwnerPermission(permission), cancellationToken)
                .ConfigureAwait(false);
        }

        var decision = allowed
            ? AuthorizationDecision.Allow(permission)
            : AuthorizationDecision.Deny(permission, AuthorizationErrors.AccessDenied(permission));

        return await FinalizeAsync(request, permission, decision, cancellationToken).ConfigureAwait(false);
    }

    private async Task<bool> HasPermissionAsync(
        AuthorizationEvaluationRequest request,
        string permission,
        CancellationToken cancellationToken)
    {
        var cached = await _cache.GetUserPermissionsAsync(request.UserId, cancellationToken).ConfigureAwait(false);
        if (cached is not null)
        {
            return cached.Contains(permission);
        }

        var roles = await _roles.GetByCodesAsync(request.RoleCodes, cancellationToken).ConfigureAwait(false);
        var effective = roles
            .Where(r => r.IsActive)
            .SelectMany(r => r.PermissionCodes)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await _cache.SetUserPermissionsAsync(request.UserId, effective, cancellationToken).ConfigureAwait(false);
        return effective.Contains(permission);
    }

    private async Task<AuthorizationDecision> FinalizeAsync(
        AuthorizationEvaluationRequest request,
        string permission,
        AuthorizationDecision decision,
        CancellationToken cancellationToken)
    {
        if (!request.RecordHistory)
        {
            return decision;
        }

        var now = _clock.UtcNow;
        await _history.AddAsync(
                AuthorizationHistoryEntry.Create(
                    request.UserId,
                    decision,
                    request.RequestedCompanyId,
                    request.RequestedPlantId,
                    request.ResourceOwnerId,
                    request.Field,
                    _requestContext.CorrelationId,
                    now),
                cancellationToken)
            .ConfigureAwait(false);

        if (!decision.Allowed)
        {
            await _outbox.EnqueueAsync(
                    nameof(AuthorizationDenied),
                    new AuthorizationDenied
                    {
                        UserId = request.UserId,
                        Permission = permission,
                        Reason = decision.DenialCode ?? "ACCESS_DENIED"
                    },
                    request.UserId,
                    _requestContext.CorrelationId,
                    now,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return decision;
    }

    private static string ResolvePermissionCode(string permission, string? field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return permission.Trim();
        }

        if (permission.Contains(field, StringComparison.OrdinalIgnoreCase))
        {
            return permission.Trim();
        }

        var parts = permission.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length >= 2)
        {
            var action = parts[^1];
            var prefix = string.Join('.', parts.Take(parts.Length - 1));
            return $"{prefix}.{field.Trim()}.{action}";
        }

        return $"{permission.Trim()}.{field.Trim()}";
    }

    private static string ToOwnerPermission(string permission) =>
        permission.EndsWith(".View", StringComparison.OrdinalIgnoreCase)
            ? permission[..^5] + ".Own"
            : permission + ".Own";
}

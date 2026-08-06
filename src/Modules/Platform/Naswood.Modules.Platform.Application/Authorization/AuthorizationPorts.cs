using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public interface IPermissionCatalogRepository
{
    Task<IReadOnlyList<PermissionDefinition>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<PermissionDefinition> permissions, CancellationToken cancellationToken = default);
}

public interface IRoleCatalogRepository
{
    Task<IReadOnlyList<RoleDefinition>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoleDefinition>> GetByCodesAsync(
        IEnumerable<string> codes,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default);
}

public interface IAuthorizationHistoryRepository
{
    Task AddAsync(AuthorizationHistoryEntry entry, CancellationToken cancellationToken = default);
}

public interface IPermissionCache
{
    Task<IReadOnlySet<string>?> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task SetUserPermissionsAsync(
        Guid userId,
        IReadOnlySet<string> permissions,
        CancellationToken cancellationToken = default);

    void InvalidateUser(Guid userId);

    void InvalidateAll();
}

public interface IAuthorizationEngine
{
    Task<AuthorizationDecision> EvaluateAsync(
        AuthorizationEvaluationRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record AuthorizationEvaluationRequest(
    Guid UserId,
    IReadOnlyCollection<string> RoleCodes,
    IReadOnlyCollection<string> CompanyIds,
    IReadOnlyCollection<string> PlantIds,
    string Permission,
    string? RequestedCompanyId,
    string? RequestedPlantId,
    string? ResourceOwnerId,
    string? Field,
    bool RecordHistory);

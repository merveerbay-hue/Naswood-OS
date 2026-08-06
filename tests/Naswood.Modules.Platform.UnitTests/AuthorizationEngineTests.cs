using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.UnitTests;

public class AuthorizationEngineTests
{
    [Fact]
    public async Task Evaluate_denies_when_company_not_assigned()
    {
        var role = RoleDefinition.Create("Administrator", "Administrator", ["Inventory.View"]);
        var engine = CreateEngine([role]);

        var decision = await engine.EvaluateAsync(new AuthorizationEvaluationRequest(
            Guid.NewGuid(),
            ["Administrator"],
            ["COMP-001"],
            ["PLANT-001"],
            "Inventory.View",
            "COMP-999",
            "PLANT-001",
            null,
            null,
            RecordHistory: false));

        Assert.False(decision.Allowed);
        Assert.Equal("AUTHZ-003", decision.DenialCode);
    }

    [Fact]
    public async Task Evaluate_allows_when_role_has_permission()
    {
        var role = RoleDefinition.Create("Administrator", "Administrator", ["Inventory.View"]);
        var engine = CreateEngine([role]);

        var decision = await engine.EvaluateAsync(new AuthorizationEvaluationRequest(
            Guid.NewGuid(),
            ["Administrator"],
            ["COMP-001"],
            ["PLANT-001"],
            "Inventory.View",
            "COMP-001",
            "PLANT-001",
            null,
            null,
            RecordHistory: false));

        Assert.True(decision.Allowed);
    }

    [Fact]
    public async Task Evaluate_supports_owner_permission()
    {
        var userId = Guid.NewGuid();
        var role = RoleDefinition.Create("Buyer", "Buyer", ["PurchaseOrder.Own"]);
        var engine = CreateEngine([role]);

        var decision = await engine.EvaluateAsync(new AuthorizationEvaluationRequest(
            userId,
            ["Buyer"],
            ["COMP-001"],
            ["PLANT-001"],
            "PurchaseOrder.View",
            "COMP-001",
            "PLANT-001",
            userId.ToString("D"),
            null,
            RecordHistory: false));

        Assert.True(decision.Allowed);
    }

    [Fact]
    public void Catalog_seed_includes_administrator_permissions()
    {
        var permissions = AuthorizationCatalogSeed.CreatePermissions();
        var admin = AuthorizationCatalogSeed.CreateAdministratorRole(permissions);

        Assert.Contains(permissions, p => p.Code == "Authorization.View");
        Assert.Contains(admin.PermissionCodes, p => p == "Inventory.View");
        Assert.True(admin.HasPermission("Administration.Manage"));
    }

    private static AuthorizationEngine CreateEngine(IReadOnlyList<RoleDefinition> roles)
    {
        return new AuthorizationEngine(
            new StubRoles(roles),
            new StubCache(),
            new StubHistory(),
            new StubOutbox(),
            new StubClock(),
            new StubContext(),
            new StubUnitOfWork());
    }

    private sealed class StubRoles : IRoleCatalogRepository
    {
        private readonly IReadOnlyList<RoleDefinition> _roles;

        public StubRoles(IReadOnlyList<RoleDefinition> roles) => _roles = roles;

        public Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_roles.Count > 0);

        public Task<IReadOnlyList<RoleDefinition>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_roles);

        public Task<IReadOnlyList<RoleDefinition>> GetByCodesAsync(
            IEnumerable<string> codes,
            CancellationToken cancellationToken = default)
        {
            var set = codes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            IReadOnlyList<RoleDefinition> result = _roles.Where(r => set.Contains(r.Code)).ToArray();
            return Task.FromResult(result);
        }
    }

    private sealed class StubCache : IPermissionCache
    {
        public Task<IReadOnlySet<string>?> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlySet<string>?>(null);

        public void InvalidateAll()
        {
        }

        public void InvalidateUser(Guid userId)
        {
        }

        public Task SetUserPermissionsAsync(
            Guid userId,
            IReadOnlySet<string> permissions,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class StubHistory : IAuthorizationHistoryRepository
    {
        public Task AddAsync(AuthorizationHistoryEntry entry, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class StubOutbox : Application.Authentication.IOutboxWriter
    {
        public Task EnqueueAsync(
            string eventType,
            object payload,
            Guid? userId,
            string correlationId,
            DateTimeOffset occurredAt,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class StubClock : Application.Authentication.IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }

    private sealed class StubContext : Application.Authentication.IAuthRequestContext
    {
        public string? IpAddress => "127.0.0.1";

        public string CorrelationId => "test";

        public Guid? UserId => null;

        public Guid? SessionId => null;

        public string? CompanyId => null;

        public string? PlantId => null;
    }

    private sealed class StubUnitOfWork : Application.Authentication.IPlatformUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}

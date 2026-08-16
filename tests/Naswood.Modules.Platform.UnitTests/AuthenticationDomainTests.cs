using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.UnitTests;

public class AuthUserTests
{
    [Fact]
    public void RegisterFailedLogin_locks_after_five_attempts()
    {
        var user = AuthUser.Create(
            "operator",
            "Operator",
            "op@naswood.local",
            "hash",
            ["COMP-001"],
            ["PLANT-001"],
            ["Operator"]);

        var now = DateTimeOffset.UtcNow;
        for (var i = 0; i < 4; i++)
        {
            user.RegisterFailedLogin(now);
            Assert.False(user.IsLocked);
        }

        user.RegisterFailedLogin(now);
        Assert.True(user.IsLocked);
        Assert.Equal(5, user.FailedLoginCount);
        Assert.Contains(user.DomainEvents, e => e is AuthAccountLocked);
    }

    [Fact]
    public void EnsureCanAuthenticate_rejects_locked_and_disabled()
    {
        var user = AuthUser.Create(
            "operator",
            "Operator",
            null,
            "hash",
            ["COMP-001"],
            ["PLANT-001"],
            ["Operator"]);

        var now = DateTimeOffset.UtcNow;
        for (var i = 0; i < 5; i++)
        {
            user.RegisterFailedLogin(now);
        }

        var locked = user.EnsureCanAuthenticate(now);
        Assert.True(locked.IsFailure);
        Assert.Equal("AUTH-003", locked.Error!.Code);
    }

    [Fact]
    public void ResolveCompanyAndPlant_uses_home_plant_when_plant_not_requested()
    {
        var user = AuthUser.Create(
            "operator",
            "Operator",
            null,
            "hash",
            ["COMP-001", "COMP-002"],
            ["PLANT-001", "PLANT-002"],
            ["Operator"]);

        // Company still required when multiple.
        var missingCompany = user.ResolveCompanyAndPlant(null, null);
        Assert.True(missingCompany.IsFailure);
        Assert.Equal("AUTH-009", missingCompany.Error!.Code);

        // HomeFactory = first PlantId (Ana Üs) when plant not specified.
        var home = user.ResolveCompanyAndPlant("COMP-001", null);
        Assert.True(home.IsSuccess);
        Assert.Equal("PLANT-001", home.Value.PlantId);
        Assert.Equal("PLANT-001", user.HomePlantId);

        var resolved = user.ResolveCompanyAndPlant("COMP-002", "PLANT-002");
        Assert.True(resolved.IsSuccess);
        Assert.Equal("COMP-002", resolved.Value.CompanyId);
        Assert.Equal("PLANT-002", resolved.Value.PlantId);
        // Home does not change when resolving another plant for a session.
        Assert.Equal("PLANT-001", user.HomePlantId);
    }
}

public class AuthSessionTests
{
    [Fact]
    public void RotateTokens_updates_refresh_hash_and_raises_event()
    {
        var session = AuthSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "hash-1",
            "COMP-001",
            "PLANT-001",
            new DeviceInfo("d1", "test", "Chrome", "Linux", "127.0.0.1", null),
            rememberMe: false,
            DateTimeOffset.UtcNow,
            TimeSpan.FromHours(12),
            TimeSpan.FromDays(30));

        session.ClearDomainEvents();
        var newAccessId = Guid.NewGuid();
        session.RotateTokens(newAccessId, "hash-2", DateTimeOffset.UtcNow, TimeSpan.FromDays(30));

        Assert.Equal(newAccessId, session.AccessTokenId);
        Assert.Equal("hash-2", session.RefreshTokenHash);
        Assert.Equal(AuthSessionStatus.Refreshed, session.Status);
        Assert.Contains(session.DomainEvents, e => e is AuthTokenRefreshed);
    }

    [Fact]
    public void Revoke_on_logout_closes_session()
    {
        var session = AuthSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "hash-1",
            "COMP-001",
            "PLANT-001",
            new DeviceInfo(null, null, null, null, null, null),
            rememberMe: false,
            DateTimeOffset.UtcNow,
            TimeSpan.FromHours(12),
            TimeSpan.FromDays(30));

        session.ClearDomainEvents();
        session.Revoke(DateTimeOffset.UtcNow, logout: true);

        Assert.Equal(AuthSessionStatus.Closed, session.Status);
        Assert.Contains(session.DomainEvents, e => e is AuthUserLoggedOut);
    }
}

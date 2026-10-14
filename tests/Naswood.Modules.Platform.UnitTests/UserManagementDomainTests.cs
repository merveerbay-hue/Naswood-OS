using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.UnitTests;

public class UserManagementDomainTests
{
    [Fact]
    public void Register_starts_pending_activation_and_blocks_login()
    {
        var user = AuthUser.Register(
            "jdoe",
            "EMP001",
            "John",
            "Doe",
            "john@naswood.com",
            "hash",
            ["COMP-001"],
            ["PLANT-001"],
            ["ReadOnly"],
            "PURCHASING",
            "BUYER",
            createdBy: null);

        Assert.Equal(UserAccountStatus.PendingActivation, user.Status);
        Assert.False(user.IsActive);
        Assert.True(user.EnsureCanAuthenticate(DateTimeOffset.UtcNow).IsFailure);
        Assert.Contains(user.DomainEvents, e => e is UserCreated);
    }

    [Fact]
    public void Activate_then_deactivate_controls_authentication()
    {
        var user = AuthUser.Register(
            "jdoe",
            "EMP001",
            "John",
            "Doe",
            "john@naswood.com",
            "hash",
            ["COMP-001"],
            ["PLANT-001"],
            ["ReadOnly"],
            null,
            null,
            null);

        Assert.True(user.Activate(null, DateTimeOffset.UtcNow).IsSuccess);
        Assert.True(user.EnsureCanAuthenticate(DateTimeOffset.UtcNow).IsSuccess);

        Assert.True(user.Deactivate(null, DateTimeOffset.UtcNow).IsSuccess);
        Assert.Equal(UserAccountStatus.Inactive, user.Status);
        Assert.True(user.EnsureCanAuthenticate(DateTimeOffset.UtcNow).IsFailure);
    }

    [Fact]
    public void SoftDelete_archives_user()
    {
        var user = AuthUser.Create(
            "op",
            "Operator",
            null,
            "hash",
            ["COMP-001"],
            ["PLANT-001"],
            ["Operator"]);

        Assert.True(user.SoftDelete(null, "cleanup", DateTimeOffset.UtcNow).IsSuccess);
        Assert.True(user.IsDeleted);
        Assert.Equal(UserAccountStatus.Archived, user.Status);
        Assert.Contains(user.DomainEvents, e => e is UserSoftDeleted);
    }
}

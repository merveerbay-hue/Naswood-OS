using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.UnitTests;

public class PermissionManagementDomainTests
{
    [Fact]
    public void SoftDelete_protects_reserved_seed_permissions()
    {
        var permission = PermissionDefinition.Create(
            "Inventory.View",
            "Inventory",
            "View",
            "View Inventory");

        Assert.True(permission.IsReserved);
        Assert.True(permission.SoftDelete(null, DateTimeOffset.UtcNow).IsFailure);
        Assert.Equal("PERM-003", permission.SoftDelete(null, DateTimeOffset.UtcNow).Error!.Code);
    }

    [Fact]
    public void Managed_permission_can_be_soft_deleted()
    {
        var permission = PermissionDefinition.CreateManaged(
            "Custom.Thing.View",
            "Inventory",
            "View",
            "View Custom Thing",
            "Thing",
            null,
            "Transaction",
            null,
            null,
            null);

        Assert.False(permission.IsReserved);
        Assert.True(permission.SoftDelete(null, DateTimeOffset.UtcNow).IsSuccess);
        Assert.True(permission.IsDeleted);
        Assert.False(permission.IsActive);
    }
}

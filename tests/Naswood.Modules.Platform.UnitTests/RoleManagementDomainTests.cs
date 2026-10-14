using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.UnitTests;

public class RoleManagementDomainTests
{
    [Fact]
    public void SoftDelete_protects_system_roles()
    {
        var admin = RoleDefinition.Create("Administrator", "Administrator", ["Role.View"]);
        Assert.True(admin.IsSystem);
        Assert.True(admin.SoftDelete(null, DateTimeOffset.UtcNow).IsFailure);
        Assert.Equal("ROLE-004", admin.SoftDelete(null, DateTimeOffset.UtcNow).Error!.Code);
    }

    [Fact]
    public void Clone_copies_permissions()
    {
        var source = RoleDefinition.CreateManaged(
            "BUYER",
            "Buyer",
            "Buys things",
            "COMP-001",
            null,
            null,
            "Purchasing",
            ["Purchasing.View", "PurchaseOrder.Create"],
            null);

        var clone = source.Clone("BUYER_CLONE", "Buyer Clone", null);
        Assert.Equal(2, clone.PermissionCodes.Count);
        Assert.Contains(clone.DomainEvents, e => e is RoleCloned);
    }

    [Fact]
    public void Deactivate_blocks_system_roles()
    {
        var readOnly = RoleDefinition.Create("ReadOnly", "Read Only", ["Inventory.View"]);
        Assert.True(readOnly.Deactivate(null, DateTimeOffset.UtcNow).IsFailure);
    }
}

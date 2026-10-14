using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public class PackageLifecyclePolicyTests
{
    [Fact]
    public void Rejected_cannot_split_merge_repack_move_or_consume()
    {
        var pkg = Pkg(InventoryPackageStatuses.Rejected, 100);
        Assert.False(PackageStatePolicy.CanSplit(pkg));
        Assert.False(PackageStatePolicy.CanMerge(pkg));
        Assert.False(PackageStatePolicy.CanRepack(pkg));
        Assert.False(PackageStatePolicy.CanMove(pkg));
        Assert.False(PackageStatePolicy.CanConsume(pkg));
        Assert.Equal("PKG-QC-002", PackageStatePolicy.GuardRejected(pkg, "SPLIT").Error!.Code);
    }

    [Fact]
    public void Quarantine_can_split_and_move_but_not_consume()
    {
        var pkg = Pkg(InventoryPackageStatuses.Quarantine, 100);
        Assert.True(PackageStatePolicy.CanSplit(pkg));
        Assert.True(PackageStatePolicy.CanMove(pkg));
        Assert.False(PackageStatePolicy.CanConsume(pkg));
        Assert.Contains(PackageActions.Split, PackageStatePolicy.AllowedActions(pkg));
    }

    [Fact]
    public void Consumed_restore_requires_reversal_reason()
    {
        var fail = PackageStatePolicy.GuardTransition(InventoryPackageStatuses.Consumed, InventoryPackageStatuses.Available, "OTHER");
        Assert.True(fail.IsFailure);
        Assert.True(PackageStatePolicy.GuardTransition(InventoryPackageStatuses.Consumed, InventoryPackageStatuses.Available, "REVERSAL").IsSuccess);
    }

    [Fact]
    public void Rejected_cannot_become_available()
    {
        var fail = PackageStatePolicy.GuardTransition(InventoryPackageStatuses.Rejected, InventoryPackageStatuses.Available, "QC");
        Assert.Equal("PKG-QC-002", fail.Error!.Code);
    }

    [Fact]
    public void Genealogy_detects_cycle()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = Guid.NewGuid();
        var op = Guid.NewGuid();
        var rels = new[]
        {
            PackageRelation.Create(op, a, b, PackageRelationTypes.Split, 1, "PCS", "PLANT-001"),
            PackageRelation.Create(op, b, c, PackageRelationTypes.Split, 1, "PCS", "PLANT-001")
        };
        Assert.True(PackageGenealogy.WouldCycle(c, a, rels));
        Assert.False(PackageGenealogy.WouldCycle(a, Guid.NewGuid(), rels));
    }

    [Fact]
    public void Relation_cannot_point_to_self()
    {
        var id = Guid.NewGuid();
        Assert.Throws<InvalidOperationException>(() =>
            PackageRelation.Create(Guid.NewGuid(), id, id, PackageRelationTypes.Split, 1, "PCS", "PLANT-001"));
    }

    private static InventoryPackage Pkg(string status, decimal qty)
    {
        var mat = Guid.NewGuid();
        var lot = Guid.NewGuid();
        return InventoryPackage.Create(
            "NW-PKG-F01-26-000001", "MI-1", "MAT-1", "LOT-1", "WH-1", "A-01",
            qty, "PCS", "NWPKG-F01-26-000001", status, plantId: "PLANT-001",
            materialId: mat, batchId: lot);
    }
}

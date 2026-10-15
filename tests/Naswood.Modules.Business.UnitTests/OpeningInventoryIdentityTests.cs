using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class OpeningInventoryIdentityTests
{
    [Fact]
    public void Stack_key_includes_count_material_lot_location()
    {
        var count = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var matA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var matB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var lot = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var loc = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var line = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var a = PackageIdentityService.StackKey(count, matA, lot, loc, "İstif 1", line);
        var b = PackageIdentityService.StackKey(count, matB, lot, loc, "İstif 1", line);
        Assert.NotEqual(a, b);
        Assert.Equal(a, PackageIdentityService.StackKey(count, matA, lot, loc, "istif 1", Guid.NewGuid()));
    }

    [Fact]
    public void Empty_group_stays_line_scoped()
    {
        var count = Guid.NewGuid();
        var mat = Guid.NewGuid();
        var lot = Guid.NewGuid();
        var loc = Guid.NewGuid();
        var line1 = Guid.NewGuid();
        var line2 = Guid.NewGuid();
        Assert.NotEqual(
            PackageIdentityService.StackKey(count, mat, lot, loc, "  ", line1),
            PackageIdentityService.StackKey(count, mat, lot, loc, "", line2));
    }

    [Fact]
    public void Package_create_requires_material_and_batch()
    {
        Assert.Throws<InvalidOperationException>(() =>
            InventoryPackage.Create("PKG-BUC-000001", "MI-1", "YM-PR-AYO-001", "LOT-BUC-000001", "WH", "A-01", 1, "M3", plantId: "F01"));
        var pkg = InventoryPackage.Create(
            "PKG-BUC-000001", "MI-1", "YM-PR-AYO-001", "LOT-BUC-000001", "WH", "A-01", 1, "M3",
            plantId: "F01",
            materialId: Guid.NewGuid(),
            batchId: Guid.NewGuid());
        Assert.False(string.IsNullOrWhiteSpace(pkg.PublicId));
        Assert.Equal("PKG-BUC-000001", pkg.Barcode);
    }

    [Fact]
    public void Opening_post_does_not_take_unit_of_work()
    {
        var method = typeof(OpeningInventoryPost).GetMethod("ExecuteAsync");
        Assert.NotNull(method);
        Assert.DoesNotContain(method!.GetParameters(), p => p.ParameterType.Name.Contains("UnitOfWork"));
    }
}

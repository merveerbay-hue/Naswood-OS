using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class OpeningInventoryCodesTests
{
    [Fact]
    public void Material_change_splits_lots()
    {
        var day = new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        var tw = OpeningInventoryCodes.OpeningLot(day, 1);
        var lata = OpeningInventoryCodes.OpeningLot(day, 2);
        Assert.Equal("LOT-OPEN-20260910-001", tw);
        Assert.Equal("LOT-OPEN-20260910-002", lata);
        Assert.NotEqual(tw, lata);
    }

    [Fact]
    public void Same_material_stacks_share_lot_split_packages()
    {
        var lot = OpeningInventoryCodes.OpeningLot(new DateTimeOffset(2026, 9, 10, 0, 0, 0, TimeSpan.Zero), 2);
        Assert.Equal("LOT-OPEN-20260910-002", lot);
        Assert.Equal("PKG-000003", OpeningInventoryCodes.PackageNo(3));
        Assert.Equal("PKG-000004", OpeningInventoryCodes.PackageNo(4));
        Assert.Equal("PKG-000005", OpeningInventoryCodes.PackageNo(5));
        Assert.Equal("NW-PKG-000004", OpeningInventoryCodes.Barcode("PKG-000004"));
    }

    [Fact]
    public void Opening_may_mint_periodic_must_not()
    {
        Assert.True(InventoryCountKinds.MayMintLotPackageBarcode("Opening"));
        Assert.True(InventoryCountKinds.MayMintLotPackageBarcode("Açılış"));
        Assert.False(InventoryCountKinds.MayMintLotPackageBarcode("Periodic"));
        Assert.False(InventoryCountKinds.MayMintLotPackageBarcode("Normal"));
        Assert.False(InventoryCountKinds.MayMintLotPackageBarcode("Blind"));
        Assert.Equal(InventoryCountKinds.Periodic, InventoryCountKinds.Normalize("Normal"));
        Assert.Equal(InventoryCountKinds.Opening, InventoryCountKinds.Normalize("Initialization"));
    }

    [Fact]
    public void Source_type_is_opening_inventory()
    {
        Assert.Equal("OPENING_INVENTORY", OpeningInventoryCodes.SourceType);
        Assert.Equal("OPENING_INVENTORY", OpeningInventoryCodes.MovementType);
    }

    [Fact]
    public void Next_ordinal_continues()
    {
        Assert.Equal(1, OpeningInventoryCodes.NextOrdinal(Array.Empty<int>()));
        Assert.Equal(5, OpeningInventoryCodes.NextOrdinal(new[] { 1, 4 }));
    }
}

using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class OpeningInventoryCodesTests
{
    [Fact]
    public void Material_change_splits_lots()
    {
        var day = new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        var tw = OpeningInventoryCodes.OpeningLot("F01", day, 1);
        var lata = OpeningInventoryCodes.OpeningLot("F01", day, 2);
        Assert.Equal("LOT-OPEN-F01-260910-0001", tw);
        Assert.Equal("LOT-OPEN-F01-260910-0002", lata);
        Assert.NotEqual(tw, lata);
    }

    [Fact]
    public void Same_material_stacks_share_lot_split_packages()
    {
        var day = new DateTimeOffset(2026, 9, 10, 0, 0, 0, TimeSpan.Zero);
        var lot = OpeningInventoryCodes.OpeningLot("F01", day, 3);
        Assert.Equal("LOT-OPEN-F01-260910-0003", lot);
        Assert.Equal("NW-PKG-F01-26-000003", OpeningInventoryCodes.PackageNo("F01", day, 3));
        Assert.Equal("NW-PKG-F01-26-000004", OpeningInventoryCodes.PackageNo("F01", day, 4));
        Assert.Equal("NWPKG-F01-26-000004", OpeningInventoryCodes.Barcode("NW-PKG-F01-26-000004"));
    }

    [Fact]
    public void Factory_token_from_plant_id()
    {
        Assert.Equal("F01", OpeningInventoryCodes.FactoryToken("F01"));
        Assert.Equal("F01", OpeningInventoryCodes.FactoryToken("PLANT-001"));
    }

    [Fact]
    public void Opening_may_mint_periodic_must_not()
    {
        Assert.True(InventoryCountKinds.MayMintLotPackageBarcode("Opening"));
        Assert.False(InventoryCountKinds.MayMintLotPackageBarcode("Periodic"));
        Assert.False(InventoryCountKinds.MayMintLotPackageBarcode("Blind"));
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

    [Fact]
    public void Qr_path_uses_public_id_only()
    {
        Assert.Equal("/inventory/packages/p/abc", OpeningInventoryCodes.QrPath("abc"));
        Assert.Equal("/inventory/packages/p/abc", PackageIdentityService.QrPath("abc"));
    }
}

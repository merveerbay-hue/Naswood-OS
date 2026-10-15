using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class OpeningInventoryCodesTests
{
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
}

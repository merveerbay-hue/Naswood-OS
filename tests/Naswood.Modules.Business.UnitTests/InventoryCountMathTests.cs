using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class InventoryCountMathTests
{
    [Fact]
    public void CubicMeters_45x90x4000x120()
    {
        Assert.Equal(1.944m, InventoryCountMath.CubicMeters(45, 90, 4000, 120));
    }

    [Fact]
    public void Difference_minus_two()
    {
        Assert.Equal(-2m, InventoryCountMath.Difference(118, 120));
    }

    [Fact]
    public void Difference_plus_three()
    {
        Assert.Equal(3m, InventoryCountMath.Difference(123, 120));
    }

    [Fact]
    public void Panel_square_meters()
    {
        Assert.Equal(59.536m, InventoryCountMath.SquareMeters(1220, 2440, 20));
    }

    [Fact]
    public void Masif_panel_stores_cubic_meters()
    {
        var p = InventoryCountMath.ResolvePolicy("M2", "Masif Panel", """{"stockUom":"M2","mainCategory":"MP"}""");
        Assert.Equal("M3", p.StockUnit);
        Assert.Equal(CountQtyMode.CubicMeter, p.Mode);
        var calc = InventoryCountMath.CalculateStockQty(p, 18, 1220, 2440, 20, null);
        Assert.True(calc.Ok);
        Assert.Equal(InventoryCountMath.CubicMeters(18, 1220, 2440, 20), calc.StockQty);
    }

    [Fact]
    public void Hardware_policy_no_dims()
    {
        var p = InventoryCountMath.ResolvePolicy("PCS", "Hırdavat", """{"stockUom":"PCS","mainCategory":"HW"}""");
        Assert.Equal(CountQtyMode.Piece, p.Mode);
        Assert.False(p.DimsRequired);
        var calc = InventoryCountMath.CalculateStockQty(p, null, null, null, 4500, null);
        Assert.True(calc.Ok);
        Assert.Equal(4500m, calc.StockQty);
    }

    [Fact]
    public void Log_uses_measured_volume()
    {
        var p = InventoryCountMath.ResolvePolicy("M3", "Tomruk", """{"mainCategory":"LOG"}""");
        Assert.Equal(CountQtyMode.MeasuredVolume, p.Mode);
        var calc = InventoryCountMath.CalculateStockQty(p, null, null, null, 35, 18.4m);
        Assert.True(calc.Ok);
        Assert.Equal(18.4m, calc.StockQty);
    }

    [Fact]
    public void Negative_pieces_rejected()
    {
        var p = InventoryCountMath.ResolvePolicy("PCS", "HW", """{"stockUom":"PCS"}""");
        var calc = InventoryCountMath.CalculateStockQty(p, null, null, null, -1, null);
        Assert.False(calc.Ok);
    }

    [Fact]
    public void Same_material_two_physical_keys()
    {
        var a = InventoryCountMath.PhysicalKey("HM-KR-PIN-001", "A-01", "", 45, 90, 3000);
        var b = InventoryCountMath.PhysicalKey("HM-KR-PIN-001", "A-01", "", 45, 90, 4000);
        Assert.NotEqual(a, b);
        Assert.Equal(
            InventoryCountMath.GroupKey("HM-KR-PIN-001", "A-01", "LOT-1"),
            InventoryCountMath.GroupKey("hm-kr-pin-001", "a-01", "lot-1"));
    }
}

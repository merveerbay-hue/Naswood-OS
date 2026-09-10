using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class MasterStockProjectionTests
{
    [Fact]
    public void Four_packages_one_balance_row()
    {
        var balance = InventoryBalance.Create("HM-KR-PIN-001", "WH-RM", "A-01", "LOT-001", 118, 0, "Active", plantId: "F01");
        var material = Material.Create("HM-KR-PIN-001", "Çam Kereste", "PIN", "Lumber", "PCS", "Active",
            """{"woodSpecies":"PIN","stockUom":"PCS"}""", plantId: "F01");
        var pkgs = new[]
        {
            InventoryPackage.Create("PKG-1", "MI-1", "HM-KR-PIN-001", "LOT-001", "WH-RM", "A-01", 30, "PCS", plantId: "F01"),
            InventoryPackage.Create("PKG-2", "MI-1", "HM-KR-PIN-001", "LOT-001", "WH-RM", "A-01", 30, "PCS", plantId: "F01"),
            InventoryPackage.Create("PKG-3", "MI-1", "HM-KR-PIN-001", "LOT-001", "WH-RM", "A-01", 29, "PCS", plantId: "F01"),
            InventoryPackage.Create("PKG-4", "MI-1", "HM-KR-PIN-001", "LOT-001", "WH-RM", "A-01", 29, "PCS", plantId: "F01"),
        };
        var row = MasterStockProjection.ToRow(balance, material, "Hammadde Deposu", "A-01", pkgs.Length, pkgs.Sum(p => p.Quantity), "45×90×4000", 45, 90, 4000);
        Assert.Equal(1, MasterStockProjection.Totals(new[] { row }, 4).MaterialRowCount);
        Assert.Equal(4, row.PackageCount);
        Assert.Equal(118m, row.StockQuantity);
        Assert.Equal("PIN", row.WoodSpecies);
        Assert.False(row.PackageBalanceMismatch);
    }

    [Fact]
    public void Stock_without_packages_stays_on_list()
    {
        var balance = InventoryBalance.Create("HW-001", "WH-RM", "B-01", "LOT-X", 12, 0, "Active", plantId: "F01");
        var row = MasterStockProjection.ToRow(balance, null, "WH-RM", "B-01", 0, 0, "", null, null, null);
        Assert.Equal(0, row.PackageCount);
        Assert.Equal(12m, row.StockQuantity);
        Assert.False(row.PackageBalanceMismatch);
    }

    [Fact]
    public void Mismatch_does_not_rewrite_balance()
    {
        var balance = InventoryBalance.Create("HM-KR-PIN-001", "WH-RM", "A-01", "LOT-001", 120, 0, "Active", plantId: "F01");
        var row = MasterStockProjection.ToRow(balance, null, "WH-RM", "A-01", 4, 118, "", null, null, null);
        Assert.Equal(120m, row.StockQuantity);
        Assert.Equal(120m, balance.QuantityOnHand);
        Assert.True(row.PackageBalanceMismatch);
        Assert.Equal(118m, row.PackageQuantitySum);
    }

    [Fact]
    public void Group_key_does_not_mix_factories()
    {
        var a = MasterStockProjection.PackageMatchKey("F01", "WH-RM", "A-01", "MAT", "LOT-001");
        var b = MasterStockProjection.PackageMatchKey("F02", "WH-RM", "A-01", "MAT", "LOT-001");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void File_name_includes_date_and_time()
    {
        var tz = TimeZoneInfo.Utc;
        var at = new DateTimeOffset(2026, 9, 10, 9, 19, 0, TimeSpan.Zero);
        var name = MasterStockProjection.FileName(at, tz);
        Assert.Equal("NASWOOD_Stok_Listesi_2026-09-10_0919.xlsx", name);
    }

    [Fact]
    public void Historical_as_of_is_detected()
    {
        var tz = TimeZoneInfo.Utc;
        var now = new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        Assert.True(MasterStockProjection.IsHistoricalAsOf(now.AddDays(-1), now, tz));
        Assert.False(MasterStockProjection.IsHistoricalAsOf(null, now, tz));
        Assert.False(MasterStockProjection.IsHistoricalAsOf(now, now, tz));
    }

    [Fact]
    public void Totals_do_not_mix_units()
    {
        var a = InventoryBalance.Create("A", "WH-RM", "A-01", "L1", 10, 0, "Active", plantId: "F01");
        var b = InventoryBalance.Create("B", "WH-RM", "A-01", "L1", 2.5m, 0, "Active", plantId: "F01");
        var ma = Material.Create("A", "Pcs", "", "HW", "PCS", "Active", """{"stockUom":"PCS"}""");
        var mb = Material.Create("B", "Vol", "", "LOG", "M3", "Active", """{"stockUom":"M3","mainCategory":"LOG"}""");
        var rows = new[]
        {
            MasterStockProjection.ToRow(a, ma, "WH-RM", "A-01", 0, 0, "", null, null, null),
            MasterStockProjection.ToRow(b, mb, "WH-RM", "A-01", 0, 0, "", null, null, null),
        };
        var totals = MasterStockProjection.Totals(rows, 0);
        Assert.Equal(2, totals.ByUnit.Count);
        Assert.Contains(totals.ByUnit, u => u.Unit == "PCS" && u.Quantity == 10);
        Assert.Contains(totals.ByUnit, u => u.Unit == "M3" && u.Quantity == 2.5m);
    }

    [Fact]
    public void Parse_actual_dims_from_movement_notes()
    {
        var (t, w, l, label) = MasterStockProjection.ParseActualDims("ok | phys=45×90×4000 | pkg=PKG-1");
        Assert.Equal(45m, t);
        Assert.Equal(90m, w);
        Assert.Equal(4000m, l);
        Assert.Equal("45×90×4000", label);
    }

    [Fact]
    public void Warehouse_filter_label()
    {
        Assert.Equal("Tümü", MasterStockProjection.FilterLabel(null));
        Assert.Equal("WH-RM", MasterStockProjection.FilterLabel("WH-RM"));
    }
}

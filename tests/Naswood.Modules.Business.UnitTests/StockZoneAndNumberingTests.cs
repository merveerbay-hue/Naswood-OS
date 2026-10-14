using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public class StockZoneAndNumberingTests
{
    [Fact]
    public void Location_type_quarantine_defaults_zone()
    {
        var loc = Location.Create("Q-01", "Karantina", "WH-1", "QUARANTINE_AREA", "Active", plantId: "PLANT-001");
        Assert.Equal(StockZoneTypes.Quarantine, loc.StockZoneType);
        var open = Location.Create("A-01", "Açık", "WH-1", "OPEN_AREA", "Active", plantId: "PLANT-001");
        Assert.Equal(StockZoneTypes.Normal, open.StockZoneType);
    }

    [Fact]
    public void Quarantine_package_cannot_go_to_normal_zone()
    {
        var loc = Location.Create("A-01", "A", "WH", "OPEN_AREA", "Active", plantId: "PLANT-001");
        var fail = StockZonePolicy.GuardDestination(InventoryPackageStatuses.Quarantine, loc, "MOVE");
        Assert.Equal("PKG-ZONE-001", fail.Error!.Code);
    }

    [Fact]
    public void Available_package_cannot_go_to_quarantine_zone()
    {
        var loc = Location.Create("Q-01", "Q", "WH", "QUARANTINE_AREA", "Active", plantId: "PLANT-001");
        var fail = StockZonePolicy.GuardDestination(InventoryPackageStatuses.Available, loc, "MOVE");
        Assert.Equal("PKG-ZONE-002", fail.Error!.Code);
    }

    [Fact]
    public void Quarantine_package_can_go_to_quarantine_zone()
    {
        var loc = Location.Create("Q-01", "Q", "WH", "QUARANTINE_AREA", "Active", plantId: "PLANT-001");
        Assert.True(StockZonePolicy.GuardDestination(InventoryPackageStatuses.Quarantine, loc, "MOVE").IsSuccess);
    }

    [Fact]
    public void Pkop_number_matches_plant_year_ordinal()
    {
        var utc = new DateTimeOffset(2026, 9, 10, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal("PKOP-F01-26-000007", PackageOperationCodes.Number("PLANT-001", utc, 7));
        Assert.Equal(7, PackageOperationCodes.ParseOrdinal("PKOP-F01-26-000007", "PLANT-001", utc));
    }
}

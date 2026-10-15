using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.UnitTests;

/// <summary>
/// The domain fills the fields that the removed startup backfills used to repair.
/// </summary>
public sealed class WritePathIdentityTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Package_always_gets_public_id(string? publicId)
    {
        var pkg = NewPackage(publicId: publicId);
        Assert.False(string.IsNullOrWhiteSpace(pkg.PublicId));
    }

    [Theory]
    [InlineData("F01", "F01")]
    [InlineData(null, "")]
    public void Package_current_plant_starts_at_owning_plant(string? plantId, string expected)
    {
        var pkg = NewPackage(plantId: plantId);
        Assert.Equal(expected, pkg.CurrentPlantId);
    }

    [Fact]
    public void Package_requires_material_and_batch_ids()
    {
        Assert.Throws<InvalidOperationException>(() => NewPackage(materialId: Guid.Empty, batchId: Guid.NewGuid()));
        Assert.Throws<InvalidOperationException>(() => NewPackage(materialId: Guid.NewGuid(), batchId: Guid.Empty));
    }

    [Theory]
    [InlineData("  ", "BIN", "NORMAL")]
    [InlineData("", "QUARANTINE", "QUARANTINE")]
    [InlineData("unknown", "QUARANTINE_AREA", "QUARANTINE")]
    [InlineData("rejected", "BIN", "REJECTED")]
    public void Location_always_gets_stock_zone(string zone, string locationType, string expected)
    {
        var loc = Location.Create("A-01", "A01", "WH", locationType, "Active", plantId: "F01", stockZoneType: zone);
        Assert.Equal(expected, loc.StockZoneType);

        loc.Update("A-01", "A01", "WH", locationType, "Active");
        Assert.Equal(expected, loc.StockZoneType);
        loc.Update("A-01", "A01", "WH", "BIN", "Active", stockZoneType: "");
        Assert.Equal("NORMAL", loc.StockZoneType);
    }

    private static InventoryPackage NewPackage(
        string? publicId = null,
        string? plantId = "F01",
        Guid? materialId = null,
        Guid? batchId = null) =>
        InventoryPackage.Create(
            "NW-PKG-F01-26-000001", "MI-1", "YM-PR-AYO-001", "LOT-1", "WH", "A-01", 1, "M3",
            plantId: plantId,
            publicId: publicId,
            materialId: materialId == Guid.Empty ? null : materialId ?? Guid.NewGuid(),
            batchId: batchId == Guid.Empty ? null : batchId ?? Guid.NewGuid());
}

using Microsoft.Extensions.DependencyInjection;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Api.IntegrationTests;

/// <summary>
/// Stock parity plan, difference 7: a package counted on a periodic count line used to disappear from
/// the line because only codes starting with "PKG-" were kept and NOS minted "NW-PKG-…".
/// </summary>
[Collection(ApiCollection.Name)]
public class CountLinePackageTests
{
    private const string Plant = "PLANT-001";
    private readonly NaswoodApiFactory _factory;
    public CountLinePackageTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Periodic_count_line_keeps_the_counted_package()
    {
        await _factory.ResetDatabaseAsync();
        await SeedMastersAsync();
        var received = await ReceiveAsync();

        using var scope = _factory.Services.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        var count = await dispatcher.SendAsync(new CreateInventoryCountCommand(
            "CNT-PKG-1", "WH-FG", "Draft", "", Plant, null, CountType: "Periodic"));
        Assert.True(count.IsSuccess, count.Error?.Message);

        var saved = await dispatcher.SendAsync(new ReplaceInventoryCountLinesCommand(
            count.Value.Id,
            [
                new UpsertInventoryCountLineRequestDto
                {
                    MaterialCode = "AYO-001",
                    LocationCode = "A-01",
                    PackageNumber = received.PackageNumber,
                    PieceCount = 12
                }
            ],
            null));
        Assert.True(saved.IsSuccess, saved.Error?.Message);

        var line = Assert.Single(saved.Value.Lines!, l => l.Role == "PHYSICAL");
        Assert.Equal(received.PackageNumber, line.PackageNumber);
        Assert.Equal(received.PackageNumber, line.Barcode);
        Assert.Equal(received.LotNumber, line.BatchNumber);
        Assert.False(line.LotUnknown);
    }

    [Theory]
    [InlineData("NW-PKG-F01-26-000001", "INV-CNT-070")]
    [InlineData("PKG-000245", "INV-ID-ANT")]
    [InlineData("PKG-ANT-000245", "INV-ID-ANT")]
    [InlineData("PKG-BUC-999999", "INV-CNT-071")]
    public async Task Count_line_package_that_cannot_be_resolved_is_rejected_not_dropped(string packageCode, string errorCode)
    {
        await _factory.ResetDatabaseAsync();
        await SeedMastersAsync();
        await ReceiveAsync();

        using var scope = _factory.Services.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        var count = await dispatcher.SendAsync(new CreateInventoryCountCommand(
            "CNT-PKG-2", "WH-FG", "Draft", "", Plant, null, CountType: "Periodic"));
        Assert.True(count.IsSuccess, count.Error?.Message);

        var saved = await dispatcher.SendAsync(new ReplaceInventoryCountLinesCommand(
            count.Value.Id,
            [
                new UpsertInventoryCountLineRequestDto
                {
                    MaterialCode = "AYO-001",
                    LocationCode = "A-01",
                    PackageNumber = packageCode,
                    PieceCount = 12
                }
            ],
            null));
        Assert.True(saved.IsFailure);
        Assert.Equal(errorCode, saved.Error!.Code);
    }

    private async Task<StockPostLineResultDto> ReceiveAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        var result = await dispatcher.SendAsync(new ExecuteGoodsReceiptCommand(
            "GR-CNT-1", "WH-FG", "REF-1", "",
            QuantityVerified: true,
            ExtractSource: "manual",
            Lines: [new StockPostLineRequestDto { MaterialCode = "AYO-001", LocationCode = "A-01", Quantity = 12, UnitOfMeasure = "PCS" }],
            PlantId: Plant));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return Assert.Single(result.Value.Lines);
    }

    private async Task SeedMastersAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        db.Materials.Add(Material.Create("AYO-001", "Ayous Profil", "", "YM", "PCS", "Active", """{"stockUom":"PCS"}""", plantId: Plant));
        db.Warehouses.Add(Warehouse.Create("WH-FG", "Mamul", "FG", "Active", plantId: Plant));
        db.Locations.Add(Location.Create("A-01", "A01", "WH-FG", "BIN", "Active", plantId: Plant));
        await db.SaveChangesAsync();
    }
}

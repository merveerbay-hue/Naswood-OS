using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Api.IntegrationTests;

/// <summary>
/// Package writes fill the identity fields that the removed startup backfills used to repair.
/// Lifecycle and production write paths are checked from their own tests via <see cref="WritePathIdentity"/>.
/// </summary>
[Collection(ApiCollection.Name)]
public class WritePathIdentityTests
{
    private const string Plant = "PLANT-001";
    private readonly NaswoodApiFactory _factory;
    public WritePathIdentityTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Goods_receipt_package_has_identity_fields()
    {
        await _factory.ResetDatabaseAsync();
        await SeedMastersAsync();
        var writtenAfter = DateTimeOffset.UtcNow;

        using var scope = _factory.Services.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        var result = await dispatcher.SendAsync(new ExecuteGoodsReceiptCommand(
            "GR-WRITEPATH-1",
            "WH-FG",
            "REF-1",
            "",
            QuantityVerified: true,
            ExtractSource: "manual",
            Lines:
            [
                new StockPostLineRequestDto
                {
                    MaterialCode = "AYO-001",
                    LocationCode = "A-01",
                    Quantity = 12,
                    UnitOfMeasure = "PCS"
                }
            ],
            PlantId: Plant));
        Assert.True(result.IsSuccess, result.Error?.Message);

        await WritePathIdentity.AssertCompleteAsync(_factory, writtenAfter, expectedPackages: 1);
    }

    [Fact]
    public async Task Opening_inventory_package_has_identity_fields()
    {
        await _factory.ResetDatabaseAsync();
        var (material, warehouse) = await SeedMastersAsync();
        var writtenAfter = DateTimeOffset.UtcNow;

        using var scope = _factory.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var count = InventoryCount.Create("CNT-WRITEPATH-1", warehouse.Code, "", "", plantId: Plant, countType: "Opening");
        var line = InventoryCountLine.CreatePhysical(
            count.Id, 1, "MANUAL", material.Id, material.Code, material.Name, "A-01", "", lotUnknown: true,
            packageNumber: "", physicalGroupLabel: "", barcode: "", null, null, null, pieceCount: 5, null,
            countedStockQty: 5, stockUnit: "PCS", countUnit: "PCS", keepSeparate: false, notes: "", plantId: Plant);

        var result = await OpeningInventoryPost.ExecuteAsync(
            count, [line], Plant, "admin", "opening", warehouse,
            sp.GetRequiredService<IInventoryBalanceRepository>(),
            sp.GetRequiredService<IInventoryMovementRepository>(),
            sp.GetRequiredService<ILocationRepository>(),
            sp.GetRequiredService<IBatchRepository>(),
            sp.GetRequiredService<IInventoryPackageRepository>(),
            sp.GetRequiredService<IMaterialIdentityRepository>(),
            sp.GetRequiredService<IMaterialRepository>(),
            sp.GetRequiredService<IIdentityCodeAllocator>(),
            CancellationToken.None);
        Assert.True(result.IsSuccess, result.Error?.Message);
        await sp.GetRequiredService<IBusinessUnitOfWork>().SaveChangesAsync();

        await WritePathIdentity.AssertCompleteAsync(_factory, writtenAfter, expectedPackages: 1);
    }

    private async Task<(Material Material, Warehouse Warehouse)> SeedMastersAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var mat = Material.Create("AYO-001", "Ayous Profil", "", "YM", "PCS", "Active", """{"stockUom":"PCS"}""", plantId: Plant);
        var wh = Warehouse.Create("WH-FG", "Mamul", "FG", "Active", plantId: Plant);
        var loc = Location.Create("A-01", "A01", "WH-FG", "BIN", "Active", plantId: Plant);
        db.Materials.Add(mat);
        db.Warehouses.Add(wh);
        db.Locations.Add(loc);
        await db.SaveChangesAsync();
        return (mat, wh);
    }
}

/// <summary>Checks the package columns the old startup backfills filled when a write path left them empty.</summary>
public static class WritePathIdentity
{
    /// <param name="writtenAfter">Only rows the code under test wrote; test seeds may skip optional fields.</param>
    public static async Task AssertCompleteAsync(NaswoodApiFactory factory, DateTimeOffset writtenAfter, int expectedPackages = 0)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var packages = await db.InventoryPackages.AsNoTracking().Where(p => p.CreatedAt > writtenAfter).ToListAsync();
        Assert.NotEmpty(packages);
        if (expectedPackages > 0) Assert.Equal(expectedPackages, packages.Count);
        Assert.All(packages, p =>
        {
            Assert.False(string.IsNullOrEmpty(p.PublicId), $"{p.PackageNumber}: PublicId");
            Assert.NotNull(p.MaterialId);
            Assert.NotNull(p.BatchId);
            Assert.NotNull(p.LocationId);
            Assert.Equal(p.PlantId ?? string.Empty, p.CurrentPlantId);
        });
    }
}

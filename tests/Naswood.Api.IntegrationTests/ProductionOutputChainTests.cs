using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class ProductionOutputChainTests
{
    private const string Plant = "PLANT-001";
    private readonly NaswoodApiFactory _factory;
    public ProductionOutputChainTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Full_chain_multi_source_qc_genealogy_matches_movements_then_reverses()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: true, sourcePackageQty: 10);
        var client = await LoginAsync();

        var preview = await client.PostAsJsonAsync("/api/v1/production-outputs/preview", world.DefaultPost(3, 2));
        Assert.Equal(HttpStatusCode.OK, preview.StatusCode);
        using var previewDoc = JsonDocument.Parse(await preview.Content.ReadAsStringAsync());
        Assert.Equal("Quarantine", previewDoc.RootElement.GetProperty("data").GetProperty("resolvedStockStatus").GetString());

        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.DefaultPost(3, 2));
        Assert.Equal(HttpStatusCode.OK, posted.StatusCode);
        using var postedDoc = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var data = postedDoc.RootElement.GetProperty("data");
        var outputId = data.GetProperty("outputId").GetGuid();
        var lotId = data.GetProperty("productionLotId").GetGuid();
        var lotNo = data.GetProperty("productionLotNumber").GetString()!;
        var number = data.GetProperty("number").GetString()!;
        var outBarcode = data.GetProperty("packages")[0].GetProperty("barcode").GetString()!;
        var outPkgNo = data.GetProperty("packages")[0].GetProperty("packageNo").GetString()!;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var sources = db.ProductionLotSources.Where(x => !x.IsDeleted && x.ProductionLotId == lotId).ToList();
            var consume = db.InventoryMovements.Where(x => !x.IsDeleted && x.DocumentNumber == number
                && x.MovementType == ProductionLotCodes.ConsumptionMovement).ToList();
            Assert.Equal(2, consume.Count);
            foreach (var src in sources)
            {
                var lot = db.Batchs.Single(b => b.Id == src.SourceLotId);
                var move = consume.Single(m => m.LotNumber == lot.BatchNumber && m.Quantity == src.ConsumedQuantity);
                Assert.True(ProductionLotCodes.NotesBindSource(move.Notes, lot.BatchNumber, src.SourceLotId));
            }
        }

        var reverse = await client.PostAsJsonAsync($"/api/v1/production-outputs/{outputId}/reverse", new { reason = "yanlış istif" });
        Assert.Equal(HttpStatusCode.OK, reverse.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var outPkg = db.InventoryPackages.Single(p => !p.IsDeleted && p.BatchId == lotId);
            Assert.Equal(InventoryPackageStatuses.Cancelled, outPkg.Status);
            Assert.Equal(4m, outPkg.Quantity);
            Assert.Equal(outPkgNo, outPkg.PackageNumber);
            Assert.Equal(outBarcode, outPkg.Barcode);
            var srcPkg = db.InventoryPackages.Single(p => p.Id == world.SourcePackageAId);
            Assert.Equal(InventoryPackageStatuses.Available, srcPkg.Status);
            Assert.Equal(10m, srcPkg.Quantity);
            Assert.Equal(world.SourceBarcodeA, srcPkg.Barcode);
            Assert.Equal(world.SourcePackageNoA, srcPkg.PackageNumber);
            Assert.Equal(1, db.InventoryPackages.Count(p => p.PackageNumber == world.SourcePackageNoA && !p.IsDeleted));
        }

        var closedScan = await client.GetAsync($"/api/v1/production-outputs/consume-by-barcode/{Uri.EscapeDataString(outBarcode)}");
        Assert.Equal(HttpStatusCode.BadRequest, closedScan.StatusCode);
        Assert.Equal(0m, BalanceOf(lotNo));
    }

    [Fact]
    public async Task Failed_second_source_leaves_no_partial_rows()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: false, sourcePackageQty: 10);
        var client = await LoginAsync();
        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.DefaultPost(3, 2));
        Assert.Equal(HttpStatusCode.BadRequest, posted.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(0, db.ProductionOutputs.Count(x => !x.IsDeleted));
        Assert.Equal(0, db.Batchs.Count(x => !x.IsDeleted && x.SourceType == ProductionLotCodes.SourceType));
        Assert.Equal(10m, db.InventoryBalances.Single(b => b.BatchNumber == world.SourceLotA.BatchNumber).QuantityOnHand);
    }

    [Fact]
    public async Task Package_integrity_blocks_over_consume_across_two_rows()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: true, sourcePackageQty: 2);
        var client = await LoginAsync();
        var body = world.Post(
            4,
            new[]
            {
                world.Source(world.SourceLotA.Id, 1.2m, world.SourcePackageAId),
                world.Source(world.SourceLotA.Id, 1.2m, world.SourcePackageAId)
            });
        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", body);
        Assert.Equal(HttpStatusCode.BadRequest, posted.StatusCode);
        using var doc = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        Assert.Equal("PRD-OUT-017", doc.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(0, db.ProductionOutputs.Count(x => !x.IsDeleted));
        Assert.Equal(2m, db.InventoryPackages.Single(p => p.Id == world.SourcePackageAId).Quantity);
        Assert.Equal(10m, db.InventoryBalances.Single(b => b.BatchNumber == world.SourceLotA.BatchNumber).QuantityOnHand);
    }

    [Fact]
    public async Task Full_consume_then_reverse_reactivates_same_source_package()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: true, sourcePackageQty: 2);
        var client = await LoginAsync();
        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.Post(
            4,
            new[]
            {
                world.Source(world.SourceLotA.Id, 2m, world.SourcePackageAId),
                world.Source(world.SourceLotB.Id, 2m, null)
            }));
        posted.EnsureSuccessStatusCode();
        using var postedDoc = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var outputId = postedDoc.RootElement.GetProperty("data").GetProperty("outputId").GetGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var pkg = db.InventoryPackages.Single(p => p.Id == world.SourcePackageAId);
            Assert.Equal(InventoryPackageStatuses.Consumed, pkg.Status);
            Assert.Equal(0m, pkg.Quantity);
            Assert.Equal(world.SourcePackageNoA, pkg.PackageNumber);
        }

        (await client.PostAsJsonAsync($"/api/v1/production-outputs/{outputId}/reverse", new { reason = "geri al" })).EnsureSuccessStatusCode();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            Assert.Equal(1, db.InventoryPackages.Count(p => p.PackageNumber == world.SourcePackageNoA && !p.IsDeleted));
            var pkg = db.InventoryPackages.Single(p => p.Id == world.SourcePackageAId);
            Assert.Equal(InventoryPackageStatuses.Available, pkg.Status);
            Assert.Equal(2m, pkg.Quantity);
            Assert.Equal(world.SourceBarcodeA, pkg.Barcode);
            Assert.Equal(world.SourcePackageNoA, pkg.PackageNumber);
        }
    }

    [Fact]
    public async Task Qc_release_makes_output_available_without_qty_change()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: true, sourcePackageQty: 10);
        var client = await LoginAsync();
        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.DefaultPost(3, 2));
        posted.EnsureSuccessStatusCode();
        using var postedDoc = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var data = postedDoc.RootElement.GetProperty("data");
        var outputId = data.GetProperty("outputId").GetGuid();
        var lotNo = data.GetProperty("productionLotNumber").GetString()!;
        var barcode = data.GetProperty("packages")[0].GetProperty("barcode").GetString()!;

        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/api/v1/production-outputs/consume-by-barcode/{Uri.EscapeDataString(barcode)}")).StatusCode);

        var qc = await client.PostAsJsonAsync($"/api/v1/production-outputs/{outputId}/qc", new
        {
            decision = "Released",
            inspectionReference = "INS-CLT-001",
            notes = "pres OK"
        });
        Assert.Equal(HttpStatusCode.OK, qc.StatusCode);
        using var qcDoc = JsonDocument.Parse(await qc.Content.ReadAsStringAsync());
        var qcData = qcDoc.RootElement.GetProperty("data");
        Assert.Equal("Released", qcData.GetProperty("qcDecision").GetString());
        Assert.Equal("INS-CLT-001", qcData.GetProperty("qcInspectionReference").GetString());
        Assert.Equal("Available", qcData.GetProperty("stockStatus").GetString());
        Assert.Equal(4m, qcData.GetProperty("outputQuantity").GetDecimal());

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var dest = db.InventoryBalances.Single(b => b.BatchNumber == lotNo);
            Assert.Equal(4m, dest.QuantityOnHand);
            Assert.Equal("Active", dest.Status);
            var pkg = db.InventoryPackages.Single(p => p.Barcode == barcode);
            Assert.Equal(4m, pkg.Quantity);
            Assert.Equal(InventoryPackageStatuses.Available, pkg.Status);
            Assert.Equal("YM-A01", pkg.LocationCode);
            Assert.Single(db.InventoryMovements.Where(m => m.DocumentNumber == data.GetProperty("number").GetString()
                && m.MovementType == ProductionLotCodes.QcReleaseMovement && m.Quantity == 0));
        }

        var scan = await client.GetAsync($"/api/v1/production-outputs/consume-by-barcode/{Uri.EscapeDataString(barcode)}");
        Assert.Equal(HttpStatusCode.OK, scan.StatusCode);
        using var scanDoc = JsonDocument.Parse(await scan.Content.ReadAsStringAsync());
        Assert.Equal(4m, scanDoc.RootElement.GetProperty("data").GetProperty("availableQuantity").GetDecimal());
    }

    [Fact]
    public async Task Qc_reject_blocks_package_and_keeps_qty()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: true, sourcePackageQty: 10);
        var client = await LoginAsync();
        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.DefaultPost(3, 2));
        posted.EnsureSuccessStatusCode();
        using var postedDoc = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var data = postedDoc.RootElement.GetProperty("data");
        var outputId = data.GetProperty("outputId").GetGuid();
        var lotNo = data.GetProperty("productionLotNumber").GetString()!;
        var barcode = data.GetProperty("packages")[0].GetProperty("barcode").GetString()!;

        var qc = await client.PostAsJsonAsync($"/api/v1/production-outputs/{outputId}/qc", new
        {
            decision = "Rejected",
            inspectionReference = "INS-CLT-BAD",
            notes = "delaminasyon"
        });
        Assert.Equal(HttpStatusCode.OK, qc.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var dest = db.InventoryBalances.Single(b => b.BatchNumber == lotNo);
            Assert.Equal(4m, dest.QuantityOnHand);
            Assert.Equal("Blocked", dest.Status);
            var pkg = db.InventoryPackages.Single(p => p.Barcode == barcode);
            Assert.Equal(InventoryPackageStatuses.Rejected, pkg.Status);
            Assert.Equal(4m, pkg.Quantity);
        }

        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/api/v1/production-outputs/consume-by-barcode/{Uri.EscapeDataString(barcode)}")).StatusCode);
    }

    [Fact]
    public async Task Output_reverse_blocked_after_package_split()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: true, sourcePackageQty: 10);
        var client = await LoginAsync();
        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.DefaultPost(3, 2));
        posted.EnsureSuccessStatusCode();
        using var postedDoc = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var data = postedDoc.RootElement.GetProperty("data");
        var outputId = data.GetProperty("outputId").GetGuid();
        var pkgId = data.GetProperty("packages")[0].GetProperty("packageId").GetGuid();

        var split = await client.PostAsJsonAsync($"/api/v1/packages/{pkgId}/split", new { lines = new[] { new { quantity = 1m } } });
        split.EnsureSuccessStatusCode();

        var reverse = await client.PostAsJsonAsync($"/api/v1/production-outputs/{outputId}/reverse", new { reason = "geri" });
        Assert.Equal(HttpStatusCode.BadRequest, reverse.StatusCode);
        using var err = JsonDocument.Parse(await reverse.Content.ReadAsStringAsync());
        Assert.Equal("PKG-LIFE-006", err.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
    }

    private decimal BalanceOf(string lot)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        return db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == lot).QuantityOnHand;
    }

    private async Task<HttpClient> LoginAsync()
    {
        var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "admin",
            password = "Naswood!Admin1"
        });
        login.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
        var token = document.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<World> SeedAsync(bool twoSourceBalances, decimal sourcePackageQty)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var output = Material.Create("CLT-OUT-001", "CLT Panel", "", "CLT", "PCS", "Active",
            """{"stockUom":"PCS","mainCategory":"CLT","qcHold":true}""", plantId: Plant);
        var rawA = Material.Create("LM-A-001", "Lamel A", "", "YM", "PCS", "Active",
            """{"stockUom":"PCS","mainCategory":"YM"}""", plantId: Plant);
        var rawB = Material.Create("LM-B-001", "Lamel B", "", "YM", "PCS", "Active",
            """{"stockUom":"PCS","mainCategory":"YM"}""", plantId: Plant);
        var wh = Warehouse.Create("WH-SFG", "Yarı mamul", "SFG", "Active", plantId: Plant);
        var loc = Location.Create("YM-A01", "A01", "WH-SFG", "BIN", "Active", plantId: Plant);
        var order = ProductionOrder.Create("PRD-2026-0042", "CLT pres", "Released", "", plantId: Plant);
        var lotA = Batch.Create("LOT-GR-A", rawA.Code, 10, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        var lotB = Batch.Create("LOT-GR-B", rawB.Code, 8, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        db.Materials.AddRange(output, rawA, rawB);
        db.Warehouses.Add(wh);
        db.Locations.Add(loc);
        db.ProductionOrders.Add(order);
        db.Batchs.AddRange(lotA, lotB);
        db.InventoryBalances.Add(InventoryBalance.Create(rawA.Code, "WH-SFG", "YM-A01", lotA.BatchNumber, 10, 0, "Active", plantId: Plant));
        if (twoSourceBalances)
            db.InventoryBalances.Add(InventoryBalance.Create(rawB.Code, "WH-SFG", "YM-A01", lotB.BatchNumber, 8, 0, "Active", plantId: Plant));

        var mint = PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 9);
        var pkgA = InventoryPackage.Create(
            mint.PackageNumber, "MI-SRC-A", rawA.Code, lotA.BatchNumber, "WH-SFG", "YM-A01",
            sourcePackageQty, "PCS", mint.BarcodeValue, "Available", plantId: Plant,
            publicId: mint.PublicId, materialId: rawA.Id, batchId: lotA.Id, warehouseId: wh.Id, locationId: loc.Id);
        db.InventoryPackages.Add(pkgA);
        await db.SaveChangesAsync();

        return new World(order.Id, output.Id, lotA, lotB, pkgA.Id, pkgA.PackageNumber, pkgA.Barcode);
    }

    private sealed record World(
        Guid OrderId,
        Guid OutputMaterialId,
        Batch SourceLotA,
        Batch SourceLotB,
        Guid SourcePackageAId,
        string SourcePackageNoA,
        string SourceBarcodeA)
    {
        public object DefaultPost(decimal consumeA, decimal consumeB)
            => Post(4, new[] { Source(SourceLotA.Id, consumeA, SourcePackageAId), Source(SourceLotB.Id, consumeB, null) });

        public object Post(decimal pieceCount, object[] sources) => new
        {
            productionOrderId = OrderId,
            outputMaterialId = OutputMaterialId,
            warehouseCode = "WH-SFG",
            locationCode = "YM-A01",
            workCenterCode = "WC-CLT",
            stockStatus = "Available",
            plantId = Plant,
            lines = new[] { new { physicalGroupLabel = "İstif A", pieceCount } },
            sources
        };

        public object Source(Guid lotId, decimal qty, Guid? packageId) => new
        {
            sourceLotId = lotId,
            sourceWarehouseCode = "WH-SFG",
            sourceLocationCode = "YM-A01",
            consumedQuantity = qty,
            unit = "PCS",
            sourcePackageId = packageId
        };
    }
}

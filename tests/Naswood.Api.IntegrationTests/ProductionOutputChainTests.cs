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
        var world = await SeedAsync(twoSourceBalances: true, withSourcePackages: true);
        var client = await LoginAsync();

        var preview = await client.PostAsJsonAsync("/api/v1/production-outputs/preview", world.PostBody);
        Assert.Equal(HttpStatusCode.OK, preview.StatusCode);
        using var previewDoc = JsonDocument.Parse(await preview.Content.ReadAsStringAsync());
        var previewData = previewDoc.RootElement.GetProperty("data");
        Assert.Equal("Quarantine", previewData.GetProperty("resolvedStockStatus").GetString());
        Assert.Equal(2, previewData.GetProperty("sourceLotCount").GetInt32());

        var scan = await client.GetAsync($"/api/v1/production-outputs/consume-by-barcode/{world.SourceBarcodeA}");
        Assert.Equal(HttpStatusCode.OK, scan.StatusCode);
        using var scanDoc = JsonDocument.Parse(await scan.Content.ReadAsStringAsync());
        Assert.Equal(world.SourceLotA.Id.ToString(), scanDoc.RootElement.GetProperty("data").GetProperty("sourceLotId").GetString());

        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.PostBody);
        Assert.Equal(HttpStatusCode.OK, posted.StatusCode);
        using var postedDoc = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var data = postedDoc.RootElement.GetProperty("data");
        var outputId = data.GetProperty("outputId").GetGuid();
        var lotId = data.GetProperty("productionLotId").GetGuid();
        var lotNo = data.GetProperty("productionLotNumber").GetString()!;
        var number = data.GetProperty("number").GetString()!;
        Assert.Equal("Quarantine", data.GetProperty("stockStatus").GetString());
        Assert.Equal(2, data.GetProperty("sourceLotCount").GetInt32());

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var sources = db.ProductionLotSources.Where(x => !x.IsDeleted && x.ProductionLotId == lotId).OrderBy(x => x.CreatedAt).ToList();
            Assert.Equal(2, sources.Count);
            var moves = db.InventoryMovements.Where(x => !x.IsDeleted && x.DocumentNumber == number).ToList();
            var consume = moves.Where(m => m.MovementType == ProductionLotCodes.ConsumptionMovement).ToList();
            var output = moves.Where(m => m.MovementType == ProductionLotCodes.OutputMovement).ToList();
            Assert.Equal(2, consume.Count);
            Assert.Single(output);
            Assert.Equal(lotNo, output[0].LotNumber);

            foreach (var src in sources)
            {
                var lot = db.Batchs.Single(b => b.Id == src.SourceLotId);
                var move = consume.Single(m =>
                    string.Equals(m.LotNumber, lot.BatchNumber, StringComparison.OrdinalIgnoreCase)
                    && m.Quantity == src.ConsumedQuantity);
                Assert.True(ProductionLotCodes.NotesBindSource(move.Notes, lot.BatchNumber, src.SourceLotId));
                Assert.Equal(src.SourceMaterialCode, move.MaterialCode);
                if (src.SourcePackageId is Guid pkgId)
                {
                    var pkg = db.InventoryPackages.Single(p => p.Id == pkgId);
                    Assert.Equal(pkg.PackageNumber, move.PackageNumber);
                }
            }

            var passportSources = sources.Select(s => db.Batchs.Single(b => b.Id == s.SourceLotId).BatchNumber).OrderBy(x => x).ToArray();
            var movementLots = consume.Select(m => m.LotNumber).OrderBy(x => x).ToArray();
            Assert.Equal(passportSources, movementLots);

            var dest = db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == lotNo && b.PlantId == Plant);
            Assert.Equal(4m, dest.QuantityOnHand);
            Assert.Equal("Hold", dest.Status);
            var srcA = db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == world.SourceLotA.BatchNumber);
            var srcB = db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == world.SourceLotB.BatchNumber);
            Assert.Equal(7m, srcA.QuantityOnHand);
            Assert.Equal(6m, srcB.QuantityOnHand);
            var outPkg = db.InventoryPackages.Single(p => !p.IsDeleted && p.BatchId == lotId);
            Assert.False(string.IsNullOrWhiteSpace(outPkg.Barcode));
            Assert.Equal("Quarantine", outPkg.Status);
        }

        var passport = await client.GetAsync($"/api/v1/production-lots/{lotId}/passport");
        Assert.Equal(HttpStatusCode.OK, passport.StatusCode);
        using var passDoc = JsonDocument.Parse(await passport.Content.ReadAsStringAsync());
        var passLots = passDoc.RootElement.GetProperty("data").GetProperty("sourceLots")
            .EnumerateArray().Select(x => x.GetProperty("sourceLotNumber").GetString()).OrderBy(x => x).ToArray();
        Assert.Equal(new[] { world.SourceLotA.BatchNumber, world.SourceLotB.BatchNumber }.OrderBy(x => x).ToArray(), passLots);

        var reverse = await client.PostAsJsonAsync($"/api/v1/production-outputs/{outputId}/reverse", new { reason = "yanlış istif" });
        Assert.Equal(HttpStatusCode.OK, reverse.StatusCode);
        using var revDoc = JsonDocument.Parse(await reverse.Content.ReadAsStringAsync());
        Assert.Equal("CANCELLED", revDoc.RootElement.GetProperty("data").GetProperty("status").GetString());

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            Assert.Equal(1, db.ProductionOutputs.Count(x => !x.IsDeleted && x.Id == outputId && x.Status == "CANCELLED"));
            Assert.Equal(0, db.InventoryMovements.Count(x => x.IsDeleted && x.DocumentNumber == number));
            Assert.Equal(2, db.InventoryMovements.Count(x => !x.IsDeleted && x.DocumentNumber == number && x.MovementType == ProductionLotCodes.ConsumptionMovement));
            Assert.Equal(1, db.InventoryMovements.Count(x => !x.IsDeleted && x.DocumentNumber == number && x.MovementType == ProductionLotCodes.OutputMovement));
            Assert.True(db.InventoryMovements.Any(x => !x.IsDeleted && x.DocumentNumber == number && x.MovementType == ProductionLotCodes.OutputReversalMovement));
            Assert.Equal(2, db.InventoryMovements.Count(x => !x.IsDeleted && x.DocumentNumber == number && x.MovementType == ProductionLotCodes.ConsumptionReversalMovement));
            var dest = db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == lotNo);
            Assert.Equal(0m, dest.QuantityOnHand);
            var srcA = db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == world.SourceLotA.BatchNumber);
            var srcB = db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == world.SourceLotB.BatchNumber);
            Assert.Equal(10m, srcA.QuantityOnHand);
            Assert.Equal(8m, srcB.QuantityOnHand);
            Assert.Equal(1, db.Batchs.Count(b => !b.IsDeleted && b.Id == lotId));
            Assert.True(db.InventoryPackages.Any(p => !p.IsDeleted && p.BatchId == lotId && p.Status == "Issued"));
        }
    }

    [Fact]
    public async Task Failed_second_source_leaves_no_partial_rows()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(twoSourceBalances: false, withSourcePackages: false);
        var client = await LoginAsync();
        var posted = await client.PostAsJsonAsync("/api/v1/production-outputs", world.PostBody);
        Assert.Equal(HttpStatusCode.BadRequest, posted.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(0, db.ProductionOutputs.Count(x => !x.IsDeleted));
        Assert.Equal(0, db.ProductionLotSources.Count(x => !x.IsDeleted));
        Assert.Equal(0, db.Batchs.Count(x => !x.IsDeleted && x.SourceType == ProductionLotCodes.SourceType));
        Assert.Equal(0, db.InventoryMovements.Count(x =>
            !x.IsDeleted && (x.MovementType == ProductionLotCodes.ConsumptionMovement || x.MovementType == ProductionLotCodes.OutputMovement)));
        Assert.Equal(0, db.InventoryPackages.Count(x => !x.IsDeleted && x.LotNumber.StartsWith("LOT-PR-")));
        var srcA = db.InventoryBalances.Single(b => !b.IsDeleted && b.BatchNumber == world.SourceLotA.BatchNumber);
        Assert.Equal(10m, srcA.QuantityOnHand);
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

    private async Task<World> SeedAsync(bool twoSourceBalances, bool withSourcePackages)
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
        var balA = InventoryBalance.Create(rawA.Code, "WH-SFG", "YM-A01", lotA.BatchNumber, 10, 0, "Active", plantId: Plant);
        db.Materials.AddRange(output, rawA, rawB);
        db.Warehouses.Add(wh);
        db.Locations.Add(loc);
        db.ProductionOrders.Add(order);
        db.Batchs.AddRange(lotA, lotB);
        db.InventoryBalances.Add(balA);
        if (twoSourceBalances)
            db.InventoryBalances.Add(InventoryBalance.Create(rawB.Code, "WH-SFG", "YM-A01", lotB.BatchNumber, 8, 0, "Active", plantId: Plant));

        string barcodeA = "NWPKG-TEST-A";
        InventoryPackage? pkgA = null;
        if (withSourcePackages)
        {
            var mint = PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 9);
            barcodeA = mint.BarcodeValue;
            pkgA = InventoryPackage.Create(
                mint.PackageNumber, "MI-SRC-A", rawA.Code, lotA.BatchNumber, "WH-SFG", "YM-A01",
                10, "PCS", mint.BarcodeValue, "Available", plantId: Plant,
                publicId: mint.PublicId, materialId: rawA.Id, batchId: lotA.Id, warehouseId: wh.Id, locationId: loc.Id);
            db.InventoryPackages.Add(pkgA);
        }

        await db.SaveChangesAsync();

        return new World(
            lotA,
            lotB,
            barcodeA,
            new
            {
                productionOrderId = order.Id,
                outputMaterialId = output.Id,
                warehouseCode = "WH-SFG",
                locationCode = "YM-A01",
                workCenterCode = "WC-CLT",
                stockStatus = "Available",
                plantId = Plant,
                lines = new[]
                {
                    new { physicalGroupLabel = "İstif A", pieceCount = 4m }
                },
                sources = new object[]
                {
                    new
                    {
                        sourceLotId = lotA.Id,
                        sourceWarehouseCode = "WH-SFG",
                        sourceLocationCode = "YM-A01",
                        consumedQuantity = 3m,
                        unit = "PCS",
                        sourcePackageId = pkgA?.Id
                    },
                    new
                    {
                        sourceLotId = lotB.Id,
                        sourceWarehouseCode = "WH-SFG",
                        sourceLocationCode = "YM-A01",
                        consumedQuantity = 2m,
                        unit = "PCS"
                    }
                }
            });
    }

    private sealed record World(Batch SourceLotA, Batch SourceLotB, string SourceBarcodeA, object PostBody);
}

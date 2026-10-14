using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Users;
using IPlatformUnitOfWork = Naswood.Modules.Platform.Application.Authentication.IPlatformUnitOfWork;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class PackageLifecycleTests
{
    private const string Plant = "PLANT-001";
    private readonly NaswoodApiFactory _factory;
    public PackageLifecycleTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Split_keeps_parent_identity_mints_child_balance_unchanged()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        var key = "PKOP-SPLIT-001";
        var first = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/split", new
        {
            number = key,
            lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 10m, pieceCount = 10m }, new { sourceContentId = world.Content2200Id, quantity = 5m, pieceCount = 5m } }
        });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        using var doc = JsonDocument.Parse(await first.Content.ReadAsStringAsync());
        var data = doc.RootElement.GetProperty("data");
        var childNo = data.GetProperty("target").GetProperty("packageNo").GetString()!;
        var childBarcode = data.GetProperty("target").GetProperty("barcode").GetString()!;
        Assert.Equal(world.PackageNo, data.GetProperty("source").GetProperty("packageNo").GetString());
        Assert.Equal(world.Barcode, data.GetProperty("source").GetProperty("barcode").GetString());
        Assert.Equal(35m, data.GetProperty("source").GetProperty("quantity").GetDecimal());
        Assert.Equal(15m, data.GetProperty("target").GetProperty("quantity").GetDecimal());
        Assert.NotEqual(world.Barcode, childBarcode);

        var replay = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/split", new
        {
            number = key,
            lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 10m, pieceCount = 10m } }
        });
        replay.EnsureSuccessStatusCode();
        using var replayDoc = JsonDocument.Parse(await replay.Content.ReadAsStringAsync());
        Assert.True(replayDoc.RootElement.GetProperty("data").GetProperty("idempotentReplay").GetBoolean());

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            Assert.Equal(50m, db.InventoryBalances.Single(b => b.BatchNumber == world.LotNo).QuantityOnHand);
            Assert.Equal(1, db.InventoryPackages.Count(p => p.PackageNumber == childNo && !p.IsDeleted));
            var parent = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
            Assert.Equal(InventoryPackageStatuses.Available, parent.Status);
            var contents = db.Set<InventoryPackageContent>().Where(c => c.PackageId == parent.Id).ToList();
            Assert.Equal(20m, contents.Single(c => c.LengthMm == 3400).Quantity);
            Assert.Equal(15m, contents.Single(c => c.LengthMm == 2200).Quantity);
            var child = db.InventoryPackages.Single(p => p.PackageNumber == childNo);
            Assert.Equal(world.LotNo, child.LotNumber);
            Assert.Equal(world.MaterialCode, child.MaterialCode);
            Assert.Equal(parent.BatchId, child.BatchId);
        }
    }

    [Fact]
    public async Task Split_overdraw_mints_nothing()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        var res = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/split", new
        {
            lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 120m, pieceCount = 120m } }
        });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        Assert.Equal("PKG-SPLIT-001", doc.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(1, db.InventoryPackages.Count(p => !p.IsDeleted));
        Assert.Equal(50m, db.InventoryPackages.Single().Quantity);
    }

    [Fact]
    public async Task Split_concurrency_does_not_double_spend()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var a = await LoginAsync();
        var b = await LoginAsync();
        var body = new { lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 30m, pieceCount = 30m } } };
        var t1 = a.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/split", body);
        var t2 = b.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/split", body);
        await Task.WhenAll(t1, t2);
        var codes = new[] { t1.Result.StatusCode, t2.Result.StatusCode };
        Assert.Contains(HttpStatusCode.OK, codes);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var parent = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
        var children = db.InventoryPackages.Where(p => p.Id != world.PackageAId && !p.IsDeleted).ToList();
        Assert.True(parent.Quantity + children.Sum(c => c.Quantity) == 50m);
        Assert.True(parent.Quantity >= 0);
        Assert.True(children.Sum(c => c.Quantity) <= 50m);
    }

    [Fact]
    public async Task Merge_creates_new_target_and_closes_sources()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(secondPackage: true);
        var client = await LoginAsync();
        var res = await client.PostAsJsonAsync("/api/v1/packages/merge", new
        {
            number = "PKOP-MERGE-001",
            sourcePackageIds = new[] { world.PackageAId, world.PackageBId }
        });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var target = doc.RootElement.GetProperty("data").GetProperty("target");
        Assert.Equal(80m, target.GetProperty("quantity").GetDecimal());
        Assert.NotEqual(world.Barcode, target.GetProperty("barcode").GetString());

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(80m, db.InventoryBalances.Single(b => b.BatchNumber == world.LotNo).QuantityOnHand);
        Assert.Equal(InventoryPackageStatuses.Merged, db.InventoryPackages.Single(p => p.Id == world.PackageAId).Status);
        Assert.Equal(50m, db.InventoryPackages.Single(p => p.Id == world.PackageAId).Quantity);
        var passport = await client.GetAsync($"/api/v1/packages/by-barcode/{Uri.EscapeDataString(world.Barcode)}");
        passport.EnsureSuccessStatusCode();
        using var pass = JsonDocument.Parse(await passport.Content.ReadAsStringAsync());
        Assert.Equal("Merged", pass.RootElement.GetProperty("data").GetProperty("status").GetString());
        Assert.Contains("AKTİF DEĞİL", pass.RootElement.GetProperty("data").GetProperty("inactiveReason").GetString());
    }

    [Fact]
    public async Task Merge_rejects_different_material_lot_and_mixed_qc()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(secondPackage: true, differentMaterial: true);
        var client = await LoginAsync();
        var res = await client.PostAsJsonAsync("/api/v1/packages/merge", new { sourcePackageIds = new[] { world.PackageAId, world.PackageBId } });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        Assert.Equal("PKG-MERGE-002", doc.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task Merge_rejects_different_lot()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(secondPackage: true, differentLot: true);
        var client = await LoginAsync();
        var res = await client.PostAsJsonAsync("/api/v1/packages/merge", new { sourcePackageIds = new[] { world.PackageAId, world.PackageBId } });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        Assert.Equal("PKG-MERGE-003", doc.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task Merge_rejects_quarantine_plus_available()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(secondPackage: true, quarantineB: true);
        var client = await LoginAsync();
        var res = await client.PostAsJsonAsync("/api/v1/packages/merge", new { sourcePackageIds = new[] { world.PackageAId, world.PackageBId } });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        Assert.Equal("PKG-MERGE-005", doc.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task Repack_mints_new_barcode_and_keeps_balance()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        var res = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/repack", new { number = "PKOP-REPACK-001" });
        res.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var target = doc.RootElement.GetProperty("data").GetProperty("target");
        Assert.Equal(50m, target.GetProperty("quantity").GetDecimal());
        Assert.NotEqual(world.Barcode, target.GetProperty("barcode").GetString());
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(InventoryPackageStatuses.Repacked, db.InventoryPackages.Single(p => p.Id == world.PackageAId).Status);
        Assert.Equal(50m, db.InventoryBalances.Single(b => b.BatchNumber == world.LotNo).QuantityOnHand);
        var old = await client.GetAsync($"/api/v1/packages/by-barcode/{Uri.EscapeDataString(world.Barcode)}");
        old.EnsureSuccessStatusCode();
        using var pass = JsonDocument.Parse(await old.Content.ReadAsStringAsync());
        Assert.Contains("yeniden paketlendi", pass.RootElement.GetProperty("data").GetProperty("inactiveReason").GetString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Full_move_keeps_barcode_partial_move_splits_balance()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        var full = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/relocate", new { warehouseCode = "WH-FG", locationCode = "B-01" });
        full.EnsureSuccessStatusCode();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var pkg = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
            Assert.Equal(world.Barcode, pkg.Barcode);
            Assert.Equal("B-01", pkg.LocationCode);
            Assert.Equal(50m, db.InventoryBalances.Single(b => b.LocationCode == "B-01" && b.BatchNumber == world.LotNo).QuantityOnHand);
        }

        await _factory.ResetDatabaseAsync();
        world = await SeedAsync();
        client = await LoginAsync();
        var partial = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/partial-move", new
        {
            warehouseCode = "WH-FG",
            locationCode = "B-01",
            lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 20m, pieceCount = 20m } }
        });
        Assert.Equal(HttpStatusCode.OK, partial.StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var parent = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
            Assert.Equal(30m, parent.Quantity);
            Assert.Equal("A-01", parent.LocationCode);
            Assert.Equal(world.Barcode, parent.Barcode);
            Assert.Equal(30m, db.InventoryBalances.Single(b => b.LocationCode == "A-01" && b.BatchNumber == world.LotNo).QuantityOnHand);
            Assert.Equal(20m, db.InventoryBalances.Single(b => b.LocationCode == "B-01" && b.BatchNumber == world.LotNo).QuantityOnHand);
            Assert.Equal(2, db.InventoryMovements.Count(m => m.MovementType == "PACKAGE_MOVE" && m.Quantity == 20));
        }
    }

    [Fact]
    public async Task Qc_split_inherits_quarantine_rejected_blocked()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(quarantineA: true);
        var client = await LoginAsync();
        var split = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/split", new { lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 10m, pieceCount = 10m } } });
        split.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await split.Content.ReadAsStringAsync());
        Assert.Equal("Quarantine", doc.RootElement.GetProperty("data").GetProperty("source").GetProperty("status").GetString());
        Assert.Equal("Quarantine", doc.RootElement.GetProperty("data").GetProperty("target").GetProperty("status").GetString());

        await _factory.ResetDatabaseAsync();
        world = await SeedAsync(rejectedA: true);
        client = await LoginAsync();
        foreach (var path in new[]
                 {
                     $"/api/v1/packages/{world.PackageAId}/split",
                     $"/api/v1/packages/{world.PackageAId}/repack"
                 })
        {
            var res = await client.PostAsJsonAsync(path, new { lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 10m, pieceCount = 10m } } });
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            using var err = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            Assert.Equal("PKG-QC-002", err.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
        }
    }

    [Fact]
    public async Task Viewer_gets_403_and_cross_plant_partial_move_is_rejected()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        await SeedViewerAsync();
        var viewer = await LoginAsync("viewer", "Naswood!View1");
        var res = await viewer.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/split", new { lines = new[] { new { quantity = 10m } } });
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);

        var admin = await LoginAsync();
        var cross = await admin.PostAsJsonAsync($"/api/v1/packages/{world.PackageAId}/partial-move", new
        {
            warehouseCode = "WH-F02",
            locationCode = "X-01",
            lines = new[] { new { sourceContentId = world.Content3400Id, quantity = 10m, pieceCount = 10m } }
        });
        Assert.Equal(HttpStatusCode.Forbidden, cross.StatusCode);
    }

    [Fact]
    public async Task Cancelled_and_merged_barcodes_open_passport_but_cannot_consume()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        (await client.PostAsJsonAsync("/api/v1/packages/merge", new
        {
            sourcePackageIds = new[] { world.PackageAId, (await SeedSecondOnSameLot(world)).Id }
        })).EnsureSuccessStatusCode();
        var scan = await client.GetAsync($"/api/v1/packages/by-barcode/{Uri.EscapeDataString(world.Barcode)}");
        scan.EnsureSuccessStatusCode();
        var consume = await client.GetAsync($"/api/v1/production-outputs/consume-by-barcode/{Uri.EscapeDataString(world.Barcode)}");
        Assert.Equal(HttpStatusCode.BadRequest, consume.StatusCode);
    }

    private async Task<InventoryPackage> SeedSecondOnSameLot(World world)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var parent = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
        var mint = Naswood.Modules.Business.Application.Inventory.PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 80);
        var pkg = InventoryPackage.Create(
            mint.PackageNumber, parent.MaterialIdentityNumber, parent.MaterialCode, parent.LotNumber,
            parent.WarehouseCode, parent.LocationCode, 30, "PCS", mint.BarcodeValue, "Available",
            plantId: Plant, publicId: mint.PublicId, materialId: parent.MaterialId, batchId: parent.BatchId,
            warehouseId: parent.WarehouseId, locationId: parent.LocationId);
        db.InventoryPackages.Add(pkg);
        var bal = db.InventoryBalances.Single(b => b.BatchNumber == world.LotNo);
        bal.ApplyReceipt(30);
        await db.SaveChangesAsync();
        return pkg;
    }

    private async Task SeedViewerAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var users = scope.ServiceProvider.GetRequiredService<IAuthUserRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();
        var viewer = AuthUser.Create(
            "viewer", "Viewer", "viewer@naswood.local", hasher.Hash("Naswood!View1"),
            ["COMP-001"], ["PLANT-001"], ["ReadOnly"]);
        await users.AddAsync(viewer);
        await uow.SaveChangesAsync();
    }

    private async Task<HttpClient> LoginAsync(string user = "admin", string pass = "Naswood!Admin1")
    {
        var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { username = user, password = pass });
        login.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
        var token = document.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<World> SeedAsync(
        bool secondPackage = false,
        bool differentMaterial = false,
        bool differentLot = false,
        bool quarantineA = false,
        bool quarantineB = false,
        bool rejectedA = false)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var mat = Material.Create("AYO-001", "Ayous Profil", "", "YM", "PCS", "Active", """{"stockUom":"PCS"}""", plantId: Plant);
        var mat2 = Material.Create("PIN-001", "Pine", "", "YM", "PCS", "Active", """{"stockUom":"PCS"}""", plantId: Plant);
        var wh = Warehouse.Create("WH-FG", "Mamul", "FG", "Active", plantId: Plant);
        var locA = Location.Create("A-01", "A01", "WH-FG", "BIN", "Active", plantId: Plant);
        var locB = Location.Create("B-01", "B01", "WH-FG", "BIN", "Active", plantId: Plant);
        var lot = Batch.Create("LOT-GR-F01-1", mat.Code, 80, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        var lot2 = Batch.Create("LOT-GR-F01-2", differentMaterial ? mat2.Code : mat.Code, 30, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        db.Materials.AddRange(mat, mat2);
        db.Warehouses.Add(wh);
        db.Locations.AddRange(locA, locB);
        db.Batchs.AddRange(lot, lot2);
        db.InventoryBalances.Add(InventoryBalance.Create(mat.Code, "WH-FG", "A-01", lot.BatchNumber, secondPackage && !differentLot && !differentMaterial ? 80 : 50, 0, "Active", plantId: Plant));
        if (secondPackage && (differentLot || differentMaterial))
            db.InventoryBalances.Add(InventoryBalance.Create(differentMaterial ? mat2.Code : mat.Code, "WH-FG", "A-01", lot2.BatchNumber, 30, 0, "Active", plantId: Plant));

        var mintA = Naswood.Modules.Business.Application.Inventory.PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 10);
        var statusA = rejectedA ? "Rejected" : quarantineA ? "Quarantine" : "Available";
        var pkgA = InventoryPackage.Create(
            mintA.PackageNumber, "MI-A", mat.Code, lot.BatchNumber, "WH-FG", "A-01", 50, "PCS", mintA.BarcodeValue, statusA,
            plantId: Plant, publicId: mintA.PublicId, materialId: mat.Id, batchId: lot.Id, warehouseId: wh.Id, locationId: locA.Id);
        db.InventoryPackages.Add(pkgA);
        var c1 = InventoryPackageContent.Create(pkgA.Id, 1, 25, 90, 3400, 30, 30, "PCS", Plant);
        var c2 = InventoryPackageContent.Create(pkgA.Id, 2, 25, 90, 2200, 20, 20, "PCS", Plant);
        db.Set<InventoryPackageContent>().AddRange(c1, c2);

        Guid? pkgBId = null;
        if (secondPackage)
        {
            var mintB = Naswood.Modules.Business.Application.Inventory.PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 11);
            var statusB = quarantineB ? "Quarantine" : "Available";
            var useMat = differentMaterial ? mat2 : mat;
            var useLot = differentLot || differentMaterial ? lot2 : lot;
            var pkgB = InventoryPackage.Create(
                mintB.PackageNumber, "MI-B", useMat.Code, useLot.BatchNumber, "WH-FG", "A-01", 30, "PCS", mintB.BarcodeValue, statusB,
                plantId: Plant, publicId: mintB.PublicId, materialId: useMat.Id, batchId: useLot.Id, warehouseId: wh.Id, locationId: locA.Id);
            db.InventoryPackages.Add(pkgB);
            pkgBId = pkgB.Id;
        }

        await db.SaveChangesAsync();
        return new World(pkgA.Id, pkgBId, mintA.PackageNumber, mintA.BarcodeValue, lot.BatchNumber, mat.Code, c1.Id, c2.Id);
    }

    private sealed record World(
        Guid PackageAId,
        Guid? PackageBId,
        string PackageNo,
        string Barcode,
        string LotNo,
        string MaterialCode,
        Guid Content3400Id,
        Guid Content2200Id);
}

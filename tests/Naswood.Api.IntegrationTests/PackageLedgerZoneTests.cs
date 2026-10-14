using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class PackageLedgerZoneTests
{
    private const string Plant = "PLANT-001";
    private readonly NaswoodApiFactory _factory;
    public PackageLedgerZoneTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Relocate_writes_package_id_and_keeps_package_number_snapshot()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        (await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageId}/relocate", new { warehouseCode = "WH-FG", locationCode = "B-01" })).EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var moves = db.InventoryMovements.Where(m => m.MovementType == "PACKAGE_MOVE" && !m.IsDeleted).ToList();
        Assert.Equal(2, moves.Count);
        Assert.All(moves, m =>
        {
            Assert.Equal(world.PackageId, m.PackageId);
            Assert.Equal(world.PackageNo, m.PackageNumber);
        });
    }

    [Fact]
    public async Task Quarantine_cannot_move_to_normal_available_cannot_move_to_quarantine()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync(quarantine: true);
        var client = await LoginAsync();
        var qToNormal = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageId}/relocate", new { warehouseCode = "WH-FG", locationCode = "B-01" });
        Assert.Equal(HttpStatusCode.BadRequest, qToNormal.StatusCode);
        using var err = JsonDocument.Parse(await qToNormal.Content.ReadAsStringAsync());
        Assert.Equal("PKG-ZONE-001", err.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());

        await _factory.ResetDatabaseAsync();
        world = await SeedAsync();
        client = await LoginAsync();
        var aToQ = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageId}/relocate", new { warehouseCode = "WH-FG", locationCode = "Q-01" });
        Assert.Equal(HttpStatusCode.BadRequest, aToQ.StatusCode);
        using var err2 = JsonDocument.Parse(await aToQ.Content.ReadAsStringAsync());
        Assert.Equal("PKG-ZONE-002", err2.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task Split_mints_pkop_plant_year_number_and_legacy_movement_without_package_id_still_loads()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            db.InventoryMovements.Add(InventoryMovement.Post(
                "LEGACY", "In", "OLD-1", world.MaterialCode, "", world.PackageNo,
                "WH-FG", "A-01", world.LotNo, 1, "PCS", "legacy", plantId: Plant));
            await db.SaveChangesAsync();
            Assert.Null(db.InventoryMovements.Single(m => m.DocumentNumber == "OLD-1").PackageId);
        }

        var client = await LoginAsync();
        var split = await client.PostAsJsonAsync($"/api/v1/packages/{world.PackageId}/split", new
        {
            lines = new[] { new { sourceContentId = world.ContentId, quantity = 10m, pieceCount = 10m } }
        });
        split.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await split.Content.ReadAsStringAsync());
        var number = doc.RootElement.GetProperty("data").GetProperty("number").GetString()!;
        Assert.Matches(@"^PKOP-F01-26-\d{6}$", number);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var audit = db.InventoryMovements.Single(m => m.MovementType == "PACKAGE_SPLIT");
            Assert.Equal(world.PackageId, audit.PackageId);
            var passport = await client.GetAsync($"/api/v1/packages/{world.PackageId}/passport");
            passport.EnsureSuccessStatusCode();
            using var pass = JsonDocument.Parse(await passport.Content.ReadAsStringAsync());
            Assert.Contains(pass.RootElement.GetProperty("data").GetProperty("allowedActions").EnumerateArray().Select(x => x.GetString()), a => a == "MOVE_FULL");
        }
    }

    private async Task<HttpClient> LoginAsync()
    {
        var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { username = "admin", password = "Naswood!Admin1" });
        login.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
        var token = document.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<World> SeedAsync(bool quarantine = false)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var mat = Material.Create("AYO-Z-001", "Ayous", "", "YM", "PCS", "Active", """{"stockUom":"PCS"}""", plantId: Plant);
        var wh = Warehouse.Create("WH-FG", "Mamul", "FG", "Active", plantId: Plant);
        var locA = Location.Create("A-01", "A01", "WH-FG", "OPEN_AREA", "Active", plantId: Plant);
        var locB = Location.Create("B-01", "B01", "WH-FG", "OPEN_AREA", "Active", plantId: Plant);
        var locQ = Location.Create("Q-01", "Karantina", "WH-FG", "QUARANTINE_AREA", "Active", plantId: Plant);
        var lot = Batch.Create("LOT-Z-1", mat.Code, 50, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        db.Materials.Add(mat);
        db.Warehouses.Add(wh);
        db.Locations.AddRange(locA, locB, locQ);
        db.Batchs.Add(lot);
        db.InventoryBalances.Add(InventoryBalance.Create(mat.Code, "WH-FG", quarantine ? "Q-01" : "A-01", lot.BatchNumber, 50, 0, "Active", plantId: Plant));
        var mint = Naswood.Modules.Business.Application.Inventory.PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 21);
        var home = quarantine ? locQ : locA;
        var pkg = InventoryPackage.Create(
            mint.PackageNumber, "MI-Z", mat.Code, lot.BatchNumber, "WH-FG", home.Code, 50, "PCS", mint.BarcodeValue,
            quarantine ? "Quarantine" : "Available",
            plantId: Plant, publicId: mint.PublicId, materialId: mat.Id, batchId: lot.Id, warehouseId: wh.Id, locationId: home.Id);
        db.InventoryPackages.Add(pkg);
        var content = InventoryPackageContent.Create(pkg.Id, 1, 25, 90, 3000, 50, 50, "PCS", Plant);
        db.Set<InventoryPackageContent>().Add(content);
        await db.SaveChangesAsync();
        return new World(pkg.Id, pkg.PackageNumber, lot.BatchNumber, mat.Code, content.Id);
    }

    private sealed record World(Guid PackageId, string PackageNo, string LotNo, string MaterialCode, Guid ContentId);
}

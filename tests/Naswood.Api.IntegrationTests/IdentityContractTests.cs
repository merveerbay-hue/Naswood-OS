using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Infrastructure.Persistence;
using Npgsql;

namespace Naswood.Api.IntegrationTests;

/// <summary>ADR-002 §6 identity contract against real PostgreSQL sequences, indexes and foreign keys.</summary>
[Collection(ApiCollection.Name)]
public class IdentityContractTests
{
    private readonly NaswoodApiFactory _factory;
    public IdentityContractTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Bucak_plant_records_share_one_sequence_per_type()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");
        await SeedPlantAsync("F01");

        var first = await ReceiveAsync("PLANT-001", "GR-SITE-1", Line(12));
        var second = await ReceiveAsync("F01", "GR-SITE-2", Line(5));

        Assert.True(first.IsSuccess, first.Error?.Message);
        Assert.True(second.IsSuccess, second.Error?.Message);
        Assert.Equal("PKG-BUC-000001", Assert.Single(first.Value.Lines).PackageNumber);
        Assert.Equal("PKG-BUC-000002", Assert.Single(second.Value.Lines).PackageNumber);
        Assert.Equal("LOT-BUC-000001", first.Value.Lines[0].LotNumber);
        Assert.Equal("LOT-BUC-000002", second.Value.Lines[0].LotNumber);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var packages = await db.InventoryPackages.AsNoTracking().OrderBy(p => p.PackageNumber).ToListAsync();
        Assert.Equal(["PKG-BUC-000001", "PKG-BUC-000002"], packages.Select(p => p.PackageNumber));
        Assert.All(packages, p => Assert.Equal(p.PackageNumber, p.Barcode));
        Assert.Equal(["PLANT-001", "F01"], packages.Select(p => p.PlantId));
    }

    [Fact]
    public async Task Goods_receipt_mints_one_lot_per_receipt_line()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");

        // Line 1 is split over two locations (one lot); line 2 is the same material again (its own lot).
        var result = await ReceiveAsync("PLANT-001", "GR-LOTS-1",
            Line(4, receiptLineNo: 1),
            Line(6, receiptLineNo: 1, location: "B-01"),
            Line(3, receiptLineNo: 2));

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(["LOT-BUC-000001", "LOT-BUC-000001", "LOT-BUC-000002"], result.Value.Lines.Select(l => l.LotNumber));
        Assert.Equal(["PKG-BUC-000001", "PKG-BUC-000002", "PKG-BUC-000003"], result.Value.Lines.Select(l => l.PackageNumber));
    }

    [Theory]
    [InlineData("LOT-2026-12345", "", "")]
    [InlineData("", "PKG-260915-123456", "")]
    [InlineData("", "", "NWPKG-F01-26-000001")]
    public async Task Goods_receipt_rejects_client_codes(string lot, string package, string barcode)
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");

        var result = await ReceiveAsync("PLANT-001", "GR-CLIENT-1", new StockPostLineRequestDto
        {
            MaterialCode = "AYO-001",
            LocationCode = "A-01",
            Quantity = 1,
            UnitOfMeasure = "PCS",
            LotNumber = lot,
            PackageNumber = package,
            Barcode = barcode
        });

        Assert.True(result.IsFailure);
        Assert.Equal("INV-POST-021", result.Error!.Code);
    }

    [Fact]
    public async Task Plant_without_site_code_cannot_mint()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("F02");

        var result = await ReceiveAsync("F02", "GR-NOSITE-1", Line(1));

        Assert.True(result.IsFailure);
        Assert.Equal(IdentityCodes.ErrorNoSiteCode, result.Error!.Code);
    }

    [Fact]
    public async Task Parallel_receipts_never_share_a_code()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");

        var results = await Task.WhenAll(Enumerable.Range(1, 6)
            .Select(i => ReceiveAsync("PLANT-001", $"GR-PAR-{i}", Line(1), Line(2))));

        Assert.All(results, r => Assert.True(r.IsSuccess, r.Error?.Message));
        var packages = results.SelectMany(r => r.Value.Lines.Select(l => l.PackageNumber)).ToArray();
        var lots = results.SelectMany(r => r.Value.Lines.Select(l => l.LotNumber)).ToArray();
        Assert.Equal(12, packages.Distinct().Count());
        Assert.Equal(12, lots.Distinct().Count());
        Assert.All(packages, p => Assert.Matches(@"^PKG-BUC-0000(0[1-9]|1[0-2])$", p));
    }

    [Fact]
    public async Task Database_rejects_duplicate_package_barcode_and_lot_codes()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");
        var received = await ReceiveAsync("PLANT-001", "GR-UX-1", Line(1));
        Assert.True(received.IsSuccess, received.Error?.Message);
        var existing = received.Value.Lines[0];

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var material = await db.Materials.SingleAsync();
        var lot = await db.Batchs.SingleAsync();

        db.InventoryPackages.Add(InventoryPackage.Create(
            existing.PackageNumber, "MI-DUP", material.Code, lot.BatchNumber, "WH-FG", "A-01", 1, "PCS",
            plantId: "PLANT-001", materialId: material.Id, batchId: lot.Id));
        // Barcode = package code, so either unique index may report the duplicate first.
        AssertViolates(await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync()), "23505", "UX_package");
        db.ChangeTracker.Clear();

        db.Batchs.Add(Batch.Create(existing.LotNumber, "OTHER-MAT", 0, null, "Active", plantId: "F01"));
        AssertViolates(await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync()), "23505", "UX_batch_number");
        db.ChangeTracker.Clear();

        // Deleted rows keep their code: a code is never minted twice.
        var package = await db.InventoryPackages.SingleAsync();
        await db.Database.ExecuteSqlRawAsync(
            "UPDATE business.business_inventory_package SET \"IsDeleted\" = true WHERE \"Id\" = {0}", package.Id);
        db.InventoryPackages.Add(InventoryPackage.Create(
            existing.PackageNumber, "MI-DUP", material.Code, lot.BatchNumber, "WH-FG", "A-01", 1, "PCS",
            plantId: "PLANT-001", materialId: material.Id, batchId: lot.Id));
        AssertViolates(await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync()), "23505", "UX_package");
        db.ChangeTracker.Clear();

        var unfiltered = await db.Database.SqlQueryRaw<string>("""
            SELECT i.relname AS "Value"
            FROM pg_index x
            JOIN pg_class i ON i.oid = x.indexrelid
            JOIN pg_namespace n ON n.oid = i.relnamespace
            WHERE n.nspname = 'business' AND x.indisunique AND x.indpred IS NULL
              AND i.relname IN ('UX_package_number', 'UX_package_barcode', 'UX_batch_number')
            """).ToListAsync();
        Assert.Equal(["UX_batch_number", "UX_package_barcode", "UX_package_number"], unfiltered.Order());
    }

    [Fact]
    public async Task Database_rejects_package_without_its_lot_or_material()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var material = await db.Materials.SingleAsync();
        var lot = Batch.Create("LOT-BUC-900001", material.Code, 1, null, "Active", plantId: "PLANT-001");
        db.Batchs.Add(lot);
        await db.SaveChangesAsync();

        db.InventoryPackages.Add(InventoryPackage.Create(
            "PKG-BUC-900001", "MI-FK", material.Code, lot.BatchNumber, "WH-FG", "A-01", 1, "PCS",
            plantId: "PLANT-001", materialId: material.Id, batchId: Guid.NewGuid()));
        AssertViolates(await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync()), "23503", "FK_package_batch");
        db.ChangeTracker.Clear();

        db.InventoryPackages.Add(InventoryPackage.Create(
            "PKG-BUC-900002", "MI-FK", material.Code, lot.BatchNumber, "WH-FG", "A-01", 1, "PCS",
            plantId: "PLANT-001", materialId: Guid.NewGuid(), batchId: lot.Id));
        AssertViolates(await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync()), "23503", "FK_package_material");
    }

    [Fact]
    public async Task Transfer_number_is_minted_from_the_trf_sequence()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");
        var received = await ReceiveAsync("PLANT-001", "GR-TRF-1", Line(10));
        Assert.True(received.IsSuccess, received.Error?.Message);
        var line = received.Value.Lines[0];

        using var scope = _factory.Services.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        var moved = await dispatcher.SendAsync(new ExecuteStockTransferCommand(
            "", "AYO-001", line.LotNumber, "WH-FG", "A-01", "WH-FG", "B-01", 4, "PCS",
            "", "", "", "PLANT-001", null, null));
        Assert.True(moved.IsSuccess, moved.Error?.Message);
        Assert.Equal("TRF-BUC-000001", moved.Value.DocumentNumber);

        var withClientNumber = await dispatcher.SendAsync(new ExecuteStockTransferCommand(
            "TRF-260915-123456", "AYO-001", line.LotNumber, "WH-FG", "A-01", "WH-FG", "B-01", 1, "PCS",
            "", "", "", "PLANT-001", null, null));
        Assert.True(withClientNumber.IsFailure);
        Assert.Equal("INV-TRF-020", withClientNumber.Error!.Code);
    }

    [Theory]
    [InlineData("PKG-000245", "INV-ID-ANT")]
    [InlineData("PKG-ANT-000245", "INV-ID-ANT")]
    [InlineData("NWPKG-F01-26-000001", "INV-ID-002")]
    [InlineData("LOT-BUC-000001", "INV-ID-002")]
    public async Task Scan_endpoint_applies_the_scanner_rule(string scanned, string errorCode)
    {
        await _factory.ResetDatabaseAsync();
        var client = await LoginAsync();

        var response = await client.GetAsync($"/api/v1/packages/by-barcode/{Uri.EscapeDataString(scanned)}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(errorCode, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Scan_endpoint_opens_local_package_by_code()
    {
        await _factory.ResetDatabaseAsync();
        await SeedPlantAsync("PLANT-001");
        var received = await ReceiveAsync("PLANT-001", "GR-SCAN-1", Line(3));
        Assert.True(received.IsSuccess, received.Error?.Message);
        var client = await LoginAsync();

        var response = await client.GetAsync("/api/v1/packages/by-barcode/pkg-buc-000001");

        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        Assert.Equal("PKG-BUC-000001", data.GetProperty("packageNo").GetString());
        Assert.Equal("PKG-BUC-000001", data.GetProperty("barcode").GetString());
        Assert.False(data.TryGetProperty("qrPath", out _));
    }

    private static void AssertViolates(DbUpdateException ex, string sqlState, string constraint)
    {
        var pg = Assert.IsType<PostgresException>(ex.InnerException);
        Assert.Equal(sqlState, pg.SqlState);
        Assert.StartsWith(constraint, pg.ConstraintName);
    }

    private static StockPostLineRequestDto Line(decimal qty, int? receiptLineNo = null, string location = "A-01") => new()
    {
        MaterialCode = "AYO-001",
        LocationCode = location,
        Quantity = qty,
        UnitOfMeasure = "PCS",
        ReceiptLineNo = receiptLineNo
    };

    private async Task<Naswood.BuildingBlocks.Domain.Result<ExecuteStockDocumentResultDto>> ReceiveAsync(
        string plant, string number, params StockPostLineRequestDto[] lines)
    {
        using var scope = _factory.Services.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
        return await dispatcher.SendAsync(new ExecuteGoodsReceiptCommand(
            number, "WH-FG", "REF", "", QuantityVerified: true, ExtractSource: "manual", Lines: lines, PlantId: plant));
    }

    private async Task SeedPlantAsync(string plant)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        if (!await db.Materials.AnyAsync(m => m.Code == "AYO-001"))
            db.Materials.Add(Material.Create("AYO-001", "Ayous Profil", "", "YM", "PCS", "Active", """{"stockUom":"PCS"}""", plantId: plant));
        db.Warehouses.Add(Warehouse.Create("WH-FG", "Mamul", "FG", "Active", plantId: plant));
        db.Locations.Add(Location.Create("A-01", "A01", "WH-FG", "BIN", "Active", plantId: plant));
        db.Locations.Add(Location.Create("B-01", "B01", "WH-FG", "BIN", "Active", plantId: plant));
        await db.SaveChangesAsync();
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
}

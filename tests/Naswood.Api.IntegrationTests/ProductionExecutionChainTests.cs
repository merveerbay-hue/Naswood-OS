using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Application.Users;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class ProductionExecutionChainTests
{
    private const string Plant = "PLANT-001";
    private readonly NaswoodApiFactory _factory;
    public ProductionExecutionChainTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Full_shop_floor_chain_pause_scrap_wip_final_genealogy_qc_zone()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();

        var plan = await client.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, expectedMaterialId = world.RawId, outputType = "WIP" },
            new { sequence = 20, operationId = world.OpGluId, workCenterId = world.WcGluId, expectedMaterialId = world.WipMatId, outputType = "FINAL_OUTPUT" }
        });
        plan.EnsureSuccessStatusCode();

        var start2early = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 20)}/start", new { });
        Assert.Equal(HttpStatusCode.BadRequest, start2early.StatusCode);
        Assert.Equal("PRD-EXEC-002", await Code(start2early));

        var wrongWc = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { workCenterId = world.WcGluId });
        Assert.Equal(HttpStatusCode.BadRequest, wrongWc.StatusCode);
        Assert.Equal("PRD-EXEC-WC-001", await Code(wrongWc));

        var start1 = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { workCenterId = world.WcFjId });
        start1.EnsureSuccessStatusCode();
        var exec1 = (await Data(start1)).GetProperty("id").GetGuid();
        var replay = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        replay.EnsureSuccessStatusCode();
        Assert.Equal(exec1, (await Data(replay)).GetProperty("id").GetGuid());
        Assert.True((await Data(replay)).GetProperty("idempotentReplay").GetBoolean());

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/production-execution/executions/{exec1}/scan/{Uri.EscapeDataString(world.UnknownBarcode)}")).StatusCode);

        var qScan = await client.GetAsync($"/api/v1/production-execution/executions/{exec1}/scan/{Uri.EscapeDataString(world.QuarantineBarcode)}");
        qScan.EnsureSuccessStatusCode();
        using (var qDoc = JsonDocument.Parse(await qScan.Content.ReadAsStringAsync()))
            Assert.False(qDoc.RootElement.GetProperty("data").GetProperty("canConsume").GetBoolean());

        var consumeQ = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume", new
        { barcode = world.QuarantineBarcode, quantity = 1m });
        Assert.Equal(HttpStatusCode.BadRequest, consumeQ.StatusCode);

        var f02 = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume", new
        { barcode = world.F02Barcode, quantity = 1m });
        Assert.Equal(HttpStatusCode.BadRequest, f02.StatusCode);
        Assert.Equal("PRD-EXEC-PLANT-001", await Code(f02));

        var over = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume", new
        { barcode = world.BarcodeA, quantity = 2.4m, idempotencyKey = "over" });
        Assert.Equal(HttpStatusCode.BadRequest, over.StatusCode);
        Assert.Equal("PRD-EXEC-INPUT-002", await Code(over));

        var c1 = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume", new
        { barcode = world.BarcodeA, quantity = 1.2m, packageContentId = world.ContentAId, pieceCount = 12m, idempotencyKey = "a1" });
        c1.EnsureSuccessStatusCode();
        var c1b = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume", new
        { barcode = world.BarcodeA, quantity = 1.2m, packageContentId = world.ContentAId, pieceCount = 12m, idempotencyKey = "a1" });
        c1b.EnsureSuccessStatusCode();
        Assert.True((await Data(c1b)).GetProperty("idempotentReplay").GetBoolean());

        var c2 = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume", new
        { barcode = world.BarcodeB, quantity = 0.8m, idempotencyKey = "b1" });
        c2.EnsureSuccessStatusCode();

        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/pause", new { })).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/resume", new { })).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/scrap", new { quantity = 0.1m, unit = "M3", reasonCode = "TRIM", note = "uç" })).EnsureSuccessStatusCode();

        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/downtime/start", new { reasonCode = "MATERIAL" })).EnsureSuccessStatusCode();
        var blocked = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/complete", new { outputMaterialId = world.WipMatId, warehouseCode = "WH-SFG", locationCode = "WIP-FJ-OUT", lines = new[] { new { pieceCount = 2m } } });
        Assert.Equal(HttpStatusCode.BadRequest, blocked.StatusCode);
        Assert.Equal("PRD-EXEC-DT-001", await Code(blocked));
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/downtime/end", new { reasonCode = "MATERIAL" })).EnsureSuccessStatusCode();

        var complete1 = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/complete", new
        {
            outputMaterialId = world.WipMatId,
            warehouseCode = "WH-SFG",
            locationCode = "WIP-FJ-OUT",
            structuralProductionLotId = world.PlotId,
            lines = new[] { new { physicalGroupLabel = "FJ", thicknessMm = 40m, widthMm = 100m, lengthMm = 4000m, pieceCount = 2m } }
        });
        complete1.EnsureSuccessStatusCode();
        var loaded1 = await client.GetAsync($"/api/v1/production-execution/executions/{exec1}");
        loaded1.EnsureSuccessStatusCode();
        var d1 = await Data(loaded1);
        var wipLot = d1.GetProperty("productionLotNumber").GetString()!;
        var wipBarcode = d1.GetProperty("packages")[0].GetProperty("barcode").GetString()!;
        Assert.StartsWith("LOT-PR-", wipLot);
        Assert.Equal(world.PlotNumber, d1.GetProperty("structuralProductionLotNumber").GetString());
        Assert.NotEqual(world.PlotNumber, wipLot);

        var complete1b = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/complete", new { });
        complete1b.EnsureSuccessStatusCode();
        Assert.True((await Data(complete1b)).GetProperty("idempotentReplay").GetBoolean());

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var pkg = db.InventoryPackages.Single(p => p.Barcode == wipBarcode);
            Assert.Equal(pkg.BatchId, db.Batchs.Single(b => b.BatchNumber == wipLot).Id);
            Assert.NotEqual(world.PlotId, pkg.BatchId);
            Assert.Equal(2, db.ProductionLotSources.Count(s => s.ProductionLotId == pkg.BatchId));
            Assert.All(db.ProductionLotSources.Where(s => s.ProductionLotId == pkg.BatchId), s => Assert.Equal(exec1, s.ProductionOperationExecutionId));
            Assert.Equal(0.8m, db.InventoryPackages.Single(p => p.Id == world.PackageAId).Quantity);
        }

        var zone = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 20)}/start", new { });
        zone.EnsureSuccessStatusCode();
        var exec2 = (await Data(zone)).GetProperty("id").GetGuid();
        var wipScan = await client.GetAsync($"/api/v1/production-execution/executions/{exec2}/scan/{Uri.EscapeDataString(wipBarcode)}");
        wipScan.EnsureSuccessStatusCode();
        var avail = (await Data(wipScan)).GetProperty("availableQuantity").GetDecimal();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec2}/consume", new { barcode = wipBarcode, quantity = avail, idempotencyKey = "wip" })).EnsureSuccessStatusCode();

        var badZone = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec2}/complete", new
        {
            outputMaterialId = world.CltId,
            warehouseCode = "WH-SFG",
            locationCode = "WIP-FJ-OUT",
            lines = new[] { new { pieceCount = 1m } }
        });
        Assert.Equal(HttpStatusCode.BadRequest, badZone.StatusCode);

        var complete2 = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec2}/complete", new
        {
            outputMaterialId = world.CltId,
            warehouseCode = "WH-SFG",
            locationCode = "Q-01",
            lines = new[] { new { physicalGroupLabel = "GLU", pieceCount = 1m, thicknessMm = 40m, widthMm = 100m, lengthMm = 4000m } }
        });
        if (!complete2.IsSuccessStatusCode)
            throw new InvalidOperationException($"complete2 {await complete2.Content.ReadAsStringAsync()}");
        var loaded2 = await client.GetAsync($"/api/v1/production-execution/executions/{exec2}");
        loaded2.EnsureSuccessStatusCode();
        var d2 = await Data(loaded2);
        Assert.Equal("Quarantine", d2.GetProperty("qcStatus").GetString());
        Assert.StartsWith("LOT-PR-", d2.GetProperty("productionLotNumber").GetString());

        var viewer = await SeedViewerAsync();
        var forbidden = await viewer.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }

    [Fact]
    public async Task Cancel_after_output_uses_existing_reversal_when_wip_unused()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, expectedMaterialId = world.RawId, outputType = "WIP" }
        })).EnsureSuccessStatusCode();
        var exec = (await Data(await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { })))
            .GetProperty("id").GetGuid();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/consume",
            new { barcode = world.BarcodeA, quantity = 1.2m, packageContentId = world.ContentAId, pieceCount = 12m, idempotencyKey = "out-a" }))
            .EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/complete", new
        {
            outputMaterialId = world.WipMatId,
            warehouseCode = "WH-SFG",
            locationCode = "WIP-FJ-OUT",
            lines = new[] { new { physicalGroupLabel = "FJ", thicknessMm = 40m, widthMm = 100m, lengthMm = 4000m, pieceCount = 2m } }
        })).EnsureSuccessStatusCode();
        var before = await Data(await client.GetAsync($"/api/v1/production-execution/executions/{exec}"));
        var wipNo = before.GetProperty("packages")[0].GetProperty("packageNo").GetString();
        var wipBc = before.GetProperty("packages")[0].GetProperty("barcode").GetString();

        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/cancel",
            new { cancelReason = "MACHINE_FAILURE", note = "pres" })).EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal("CANCELLED", db.ProductionOperationExecutions.Single(e => e.Id == exec).Status);
        Assert.Equal(2m, db.InventoryPackages.Single(p => p.Id == world.PackageAId).Quantity);
        Assert.Equal(world.BarcodeA, db.InventoryPackages.Single(p => p.Id == world.PackageAId).Barcode);
        Assert.Equal(2m, db.Set<InventoryPackageContent>().Single(c => c.Id == world.ContentAId).Quantity);
        var wip = db.InventoryPackages.Single(p => p.Barcode == wipBc);
        Assert.Equal(InventoryPackageStatuses.Cancelled, wip.Status);
        Assert.Equal(wipNo, wip.PackageNumber);
        Assert.Equal(1, db.ProductionOutputs.Count(o => o.ProductionOperationExecutionId == exec && o.Status == "CANCELLED"));
        Assert.Contains(db.InventoryMovements, m => m.MovementType == ProductionLotCodes.OutputReversalMovement);
    }

    [Fact]
    public async Task Parallel_consume_cannot_overspend_package()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, expectedMaterialId = (Guid?)null, outputType = "NONE" }
        })).EnsureSuccessStatusCode();
        var start = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        start.EnsureSuccessStatusCode();
        var exec = (await Data(start)).GetProperty("id").GetGuid();

        var a = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/consume", new { barcode = world.BarcodeA, quantity = 1.5m, idempotencyKey = "p1" });
        var b = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/consume", new { barcode = world.BarcodeA, quantity = 1.5m, idempotencyKey = "p2" });
        Assert.True(a.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, b.StatusCode);
        Assert.Equal("PRD-EXEC-INPUT-002", await Code(b));
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var pkg = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
        Assert.True(pkg.Quantity >= 0);
        Assert.True(pkg.Quantity <= 2m);
        var bal = db.InventoryBalances.Single(x => x.BatchNumber == world.LotANo);
        Assert.True(bal.QuantityOnHand >= 0);
    }

    [Fact]
    public async Task Parallel_http_consume_same_package_cannot_double_spend()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        var c2 = await LoginAsync();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, expectedMaterialId = (Guid?)null, outputType = "NONE" }
        })).EnsureSuccessStatusCode();
        var start = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        var exec = (await Data(start)).GetProperty("id").GetGuid();

        var t1 = client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/consume",
            new { barcode = world.BarcodeA, quantity = 1.2m, idempotencyKey = "pa" });
        var t2 = c2.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/consume",
            new { barcode = world.BarcodeA, quantity = 1.2m, idempotencyKey = "pb" });
        await Task.WhenAll(t1, t2);
        var ok = new[] { t1.Result, t2.Result }.Count(r => r.IsSuccessStatusCode);
        var fail = new[] { t1.Result, t2.Result }.Where(r => !r.IsSuccessStatusCode).ToList();
        Assert.Equal(1, ok);
        Assert.Single(fail);
        var code = await Code(fail[0]);
        Assert.True(code is "PRD-EXEC-CONC-001" or "PRD-EXEC-INPUT-002", code);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var pkg = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
        Assert.Equal(0.8m, pkg.Quantity);
        Assert.Equal(1, db.Set<ProductionExecutionConsumption>().Count(x => x.ExecutionId == exec && !x.IsDeleted));
        Assert.Equal(0.8m, db.InventoryBalances.Single(b => b.BatchNumber == world.LotANo).QuantityOnHand);
    }

    [Fact]
    public async Task Parallel_start_same_operation_yields_one_open_execution()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var a = await LoginAsync();
        var b = await LoginAsync();
        (await a.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, outputType = "NONE" }
        })).EnsureSuccessStatusCode();
        var opId = world.GetOpId(a, 10);
        var t1 = a.PostAsJsonAsync($"/api/v1/production-execution/operations/{opId}/start", new { });
        var t2 = b.PostAsJsonAsync($"/api/v1/production-execution/operations/{opId}/start", new { });
        await Task.WhenAll(t1, t2);
        Assert.True(t1.Result.IsSuccessStatusCode);
        Assert.True(t2.Result.IsSuccessStatusCode);
        Assert.Equal((await Data(t1.Result)).GetProperty("id").GetGuid(), (await Data(t2.Result)).GetProperty("id").GetGuid());
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(1, db.ProductionOperationExecutions.Count(e => e.ProductionOperationId == opId && !e.IsDeleted));
    }

    [Fact]
    public async Task Parallel_complete_same_execution_posts_one_output()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var a = await LoginAsync();
        var b = await LoginAsync();
        (await a.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, expectedMaterialId = world.RawId, outputType = "WIP" }
        })).EnsureSuccessStatusCode();
        var exec = (await Data(await a.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(a, 10)}/start", new { })))
            .GetProperty("id").GetGuid();
        (await a.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/consume",
            new { barcode = world.BarcodeA, quantity = 1.2m, idempotencyKey = "c" })).EnsureSuccessStatusCode();
        var body = new
        {
            outputMaterialId = world.WipMatId,
            warehouseCode = "WH-SFG",
            locationCode = "WIP-FJ-OUT",
            lines = new[] { new { physicalGroupLabel = "FJ", thicknessMm = 40m, widthMm = 100m, lengthMm = 4000m, pieceCount = 2m } }
        };
        var t1 = a.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/complete", body);
        var t2 = b.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/complete", body);
        await Task.WhenAll(t1, t2);
        Assert.True(t1.Result.IsSuccessStatusCode, await t1.Result.Content.ReadAsStringAsync());
        Assert.True(t2.Result.IsSuccessStatusCode, await t2.Result.Content.ReadAsStringAsync());
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        Assert.Equal(1, db.ProductionOutputs.Count(o => o.ProductionOperationExecutionId == exec && !o.IsDeleted));
        Assert.Equal(1, db.Batchs.Count(x => x.SourceType == "PRODUCTION" && !x.IsDeleted));
        var lotId = db.ProductionOutputs.Single(o => o.ProductionOperationExecutionId == exec).OutputBatchId;
        Assert.Equal(1, db.InventoryPackages.Count(p => p.BatchId == lotId && !p.IsDeleted));
        Assert.Equal(1, db.InventoryMovements.Count(m => !m.IsDeleted && m.MovementType == ProductionLotCodes.OutputMovement));
    }

    [Fact]
    public async Task Cancel_and_idempotency_and_wip_dependency_matrix()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, expectedMaterialId = world.RawId, outputType = "WIP" },
            new { sequence = 20, operationId = world.OpGluId, workCenterId = world.WcGluId, expectedMaterialId = world.WipMatId, outputType = "FINAL_OUTPUT" }
        })).EnsureSuccessStatusCode();

        var start0 = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        var idle = (await Data(start0)).GetProperty("id").GetGuid();
        var cancelIdle = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{idle}/cancel",
            new { cancelReason = "WRONG_ORDER", note = "yanlış emir", idempotencyKey = "cx0" });
        cancelIdle.EnsureSuccessStatusCode();
        Assert.Equal("CANCELLED", (await Data(cancelIdle)).GetProperty("status").GetString());
        var cancelIdle2 = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{idle}/cancel",
            new { cancelReason = "WRONG_ORDER", note = "yanlış emir", idempotencyKey = "cx0" });
        cancelIdle2.EnsureSuccessStatusCode();
        Assert.True((await Data(cancelIdle2)).GetProperty("idempotentReplay").GetBoolean());

        var start1 = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        start1.EnsureSuccessStatusCode();
        var exec1 = (await Data(start1)).GetProperty("id").GetGuid();

        var sameKey = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume",
            new { barcode = world.BarcodeA, quantity = 1.2m, packageContentId = world.ContentAId, pieceCount = 12m, idempotencyKey = "idem-a" });
        sameKey.EnsureSuccessStatusCode();
        var sameReplay = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume",
            new { barcode = world.BarcodeA, quantity = 1.2m, packageContentId = world.ContentAId, pieceCount = 12m, idempotencyKey = "idem-a" });
        sameReplay.EnsureSuccessStatusCode();
        Assert.True((await Data(sameReplay)).GetProperty("idempotentReplay").GetBoolean());
        var diffPayload = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/consume",
            new { barcode = world.BarcodeB, quantity = 0.5m, idempotencyKey = "idem-a" });
        Assert.Equal(HttpStatusCode.Conflict, diffPayload.StatusCode);
        Assert.Equal("PRD-EXEC-IDEM-001", await Code(diffPayload));

        var cancelOne = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1}/cancel",
            new { cancelReason = "OPERATOR_ERROR", note = "tek paket" });
        cancelOne.EnsureSuccessStatusCode();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var pkg = db.InventoryPackages.Single(p => p.Id == world.PackageAId);
            Assert.Equal(2m, pkg.Quantity);
            Assert.Equal(world.BarcodeA, pkg.Barcode);
            Assert.Equal(InventoryPackageStatuses.Available, pkg.Status);
            Assert.Equal(2m, db.Set<InventoryPackageContent>().Single(c => c.Id == world.ContentAId).Quantity);
            Assert.Equal(2m, db.InventoryBalances.Single(b => b.BatchNumber == world.LotANo).QuantityOnHand);
            Assert.Contains(db.InventoryMovements, m => m.MovementType == ProductionLotCodes.ConsumptionReversalMovement);
            Assert.Contains(db.ProductionExecutionEvents, e => e.ExecutionId == exec1 && e.EventType == "CANCEL");
            Assert.Contains(db.ProductionExecutionEvents, e => e.ExecutionId == exec1 && e.EventType == "REVERSAL");
        }

        var start1b = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        var exec1b = (await Data(start1b)).GetProperty("id").GetGuid();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1b}/consume",
            new { barcode = world.BarcodeA, quantity = 1.0m, packageContentId = world.ContentAId, pieceCount = 10m, idempotencyKey = "a2" })).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1b}/consume",
            new { barcode = world.BarcodeB, quantity = 0.8m, idempotencyKey = "b2" })).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1b}/cancel",
            new { cancelReason = "WRONG_MATERIAL", note = "iki paket" })).EnsureSuccessStatusCode();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            Assert.Equal(2m, db.InventoryPackages.Single(p => p.Barcode == world.BarcodeA).Quantity);
            Assert.Equal(2m, db.InventoryPackages.Single(p => p.Barcode == world.BarcodeB).Quantity);
            Assert.Equal(2m, db.Set<InventoryPackageContent>().Single(c => c.Id == world.ContentAId).Quantity);
        }

        var start1c = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        var exec1c = (await Data(start1c)).GetProperty("id").GetGuid();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1c}/consume",
            new { barcode = world.BarcodeA, quantity = 1.2m, packageContentId = world.ContentAId, pieceCount = 12m, idempotencyKey = "a3" })).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1c}/consume",
            new { barcode = world.BarcodeB, quantity = 0.8m, idempotencyKey = "b3" })).EnsureSuccessStatusCode();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var pkgB = db.InventoryPackages.Single(p => p.Barcode == world.BarcodeB);
            pkgB.GetType().GetMethod("MarkCancelled")!.Invoke(pkgB, null);
            await db.SaveChangesAsync();
        }

        var partial = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1c}/cancel",
            new { cancelReason = "OTHER", note = "B kapalı" });
        Assert.Equal(HttpStatusCode.BadRequest, partial.StatusCode);
        Assert.Equal("PKG-LIFE-006", await Code(partial));
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
            var execNo = db.ProductionOperationExecutions.Single(e => e.Id == exec1c).Number;
            var moves = db.InventoryMovements.Where(m => !m.IsDeleted).ToList();
            Assert.Equal("RUNNING", db.ProductionOperationExecutions.Single(e => e.Id == exec1c).Status);
            Assert.Equal(0.8m, db.InventoryPackages.Single(p => p.Barcode == world.BarcodeA).Quantity);
            Assert.DoesNotContain(moves, m =>
                m.DocumentNumber == execNo && m.MovementType == ProductionLotCodes.ConsumptionReversalMovement);
        }

        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1c}/complete", new
        {
            outputMaterialId = world.WipMatId,
            warehouseCode = "WH-SFG",
            locationCode = "WIP-FJ-OUT",
            lines = new[] { new { physicalGroupLabel = "FJ", thicknessMm = 40m, widthMm = 100m, lengthMm = 4000m, pieceCount = 2m } }
        })).EnsureSuccessStatusCode();

        var editDone = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1c}/consume",
            new { barcode = world.BarcodeA, quantity = 0.1m });
        Assert.Equal(HttpStatusCode.BadRequest, editDone.StatusCode);
        Assert.Equal("PRD-EXEC-003", await Code(editDone));

        var loaded = await Data(await client.GetAsync($"/api/v1/production-execution/executions/{exec1c}"));
        var wipBarcode = loaded.GetProperty("packages")[0].GetProperty("barcode").GetString()!;

        var start2 = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 20)}/start", new { });
        var exec2 = (await Data(start2)).GetProperty("id").GetGuid();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec2}/consume",
            new { barcode = wipBarcode, quantity = loaded.GetProperty("packages")[0].GetProperty("quantity").GetDecimal(), idempotencyKey = "wip" }))
            .EnsureSuccessStatusCode();

        var blocked = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec1c}/cancel",
            new { cancelReason = "QUALITY", note = "wip kullanıldı" });
        Assert.Equal(HttpStatusCode.BadRequest, blocked.StatusCode);
        Assert.Equal("PKG-LIFE-006", await Code(blocked));

        var op = await SeedOperatorAsync();
        var forbidden = await op.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec2}/cancel",
            new { cancelReason = "OTHER", note = "op" });
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }

    [Fact]
    public async Task Shop_floor_feedback_stores_screen_user_and_execution()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, outputType = "NONE" }
        })).EnsureSuccessStatusCode();
        var exec = (await Data(await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { })))
            .GetProperty("id").GetGuid();
        var posted = await client.PostAsJsonAsync("/api/v1/production-execution/feedback", new
        {
            topic = "BARCODE_FAIL",
            note = "okuyucu Enter sonrası boş",
            screen = "/production/shop-floor/executions/" + exec,
            workCenterId = world.WcFjId,
            workCenterCode = "WC-FJ",
            executionId = exec,
            executionNumber = "PEX-TEST",
            productionOrderNumber = "PO-260901"
        });
        posted.EnsureSuccessStatusCode();
        var list = await client.GetAsync("/api/v1/production-execution/feedback");
        list.EnsureSuccessStatusCode();
        var rows = (await Data(list)).EnumerateArray().ToArray();
        Assert.Single(rows);
        Assert.Equal("BARCODE_FAIL", rows[0].GetProperty("topic").GetString());
        Assert.False(string.IsNullOrWhiteSpace(rows[0].GetProperty("userId").GetString()));
        Assert.Equal(exec, rows[0].GetProperty("executionId").GetGuid());
        Assert.Contains("shop-floor", rows[0].GetProperty("screen").GetString());
    }

    [Fact]
    public async Task Wrong_material_is_rejected()
    {
        await _factory.ResetDatabaseAsync();
        var world = await SeedAsync();
        var client = await LoginAsync();
        (await client.PostAsJsonAsync($"/api/v1/production-execution/orders/{world.OrderId}/operations", new[]
        {
            new { sequence = 10, operationId = world.OpFjId, workCenterId = world.WcFjId, expectedMaterialId = world.WipMatId, outputType = "NONE" }
        })).EnsureSuccessStatusCode();
        var start = await client.PostAsJsonAsync($"/api/v1/production-execution/operations/{world.GetOpId(client, 10)}/start", new { });
        var exec = (await Data(start)).GetProperty("id").GetGuid();
        var res = await client.PostAsJsonAsync($"/api/v1/production-execution/executions/{exec}/consume", new { barcode = world.BarcodeA, quantity = 0.5m });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        Assert.Equal("PRD-EXEC-INPUT-001", await Code(res));
    }

    private async Task<World> SeedAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
        var raw = Material.Create("YM-LM-PIN", "Lamel", "", "YM", "M3", "Active", """{"stockUom":"M3"}""", plantId: Plant);
        var wip = Material.Create("YM-LM-PIN-FJ", "FJ Lamel", "", "YM", "M3", "Active", """{"stockUom":"M3"}""", plantId: Plant);
        var clt = Material.Create("HM-GLU-001", "Glulam", "", "GLULAM", "M3", "Active", """{"stockUom":"M3","mainCategory":"GLULAM","qcHold":true}""", plantId: Plant);
        var wh = Warehouse.Create("WH-SFG", "SFG", "SFG", "Active", plantId: Plant);
        var loc = Location.Create("WIP-FJ-OUT", "FJ out", "WH-SFG", "WIP", "Active", plantId: Plant, stockZoneType: "NORMAL");
        var qloc = Location.Create("Q-01", "Karantina", "WH-SFG", "QUARANTINE_AREA", "Active", plantId: Plant, stockZoneType: "QUARANTINE");
        var order = ProductionOrder.Create("PO-260901", "FJ-GLU", "Released", "", plantId: Plant);
        var wcFj = WorkCenter.Create("WC-FJ", "Finger Joint", 10, "Active", plantId: Plant);
        var wcGlu = WorkCenter.Create("WC-GLU", "Glulam", 4, "Active", plantId: Plant);
        var opFj = Operation.Create("OP-FJ", "Finger Joint", "Active", "", plantId: Plant);
        var opGlu = Operation.Create("OP-GLU", "Glulam Press", "Active", "", plantId: Plant);
        var lotA = Batch.Create("LOT-GR-001", raw.Code, 2, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        var lotB = Batch.Create("LOT-GR-004", raw.Code, 2, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        var lotQ = Batch.Create("LOT-GR-Q", raw.Code, 1, null, "Active", plantId: Plant, sourceType: "GOODS_RECEIPT");
        var lotF02 = Batch.Create("LOT-F02", raw.Code, 1, null, "Active", plantId: "PLANT-002", sourceType: "GOODS_RECEIPT");
        db.Materials.AddRange(raw, wip, clt);
        db.Warehouses.Add(wh);
        db.Locations.AddRange(loc, qloc);
        db.ProductionOrders.Add(order);
        db.WorkCenters.AddRange(wcFj, wcGlu);
        db.Operations.AddRange(opFj, opGlu);
        db.Batchs.AddRange(lotA, lotB, lotQ, lotF02);
        db.InventoryBalances.AddRange(
            InventoryBalance.Create(raw.Code, "WH-SFG", "WIP-FJ-OUT", lotA.BatchNumber, 2, 0, "Active", plantId: Plant),
            InventoryBalance.Create(raw.Code, "WH-SFG", "WIP-FJ-OUT", lotB.BatchNumber, 2, 0, "Active", plantId: Plant),
            InventoryBalance.Create(raw.Code, "WH-SFG", "Q-01", lotQ.BatchNumber, 1, 0, "Hold", plantId: Plant));
        var mintA = PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 41);
        var mintB = PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 42);
        var mintQ = PackageIdentityService.Mint(Plant, DateTimeOffset.UtcNow, 43);
        var mint2 = PackageIdentityService.Mint("PLANT-002", DateTimeOffset.UtcNow, 1);
        var pkgA = InventoryPackage.Create(mintA.PackageNumber, "MI-A", raw.Code, lotA.BatchNumber, "WH-SFG", "WIP-FJ-OUT", 2, "M3", mintA.BarcodeValue, "Available", plantId: Plant, publicId: mintA.PublicId, materialId: raw.Id, batchId: lotA.Id, warehouseId: wh.Id, locationId: loc.Id);
        var pkgB = InventoryPackage.Create(mintB.PackageNumber, "MI-B", raw.Code, lotB.BatchNumber, "WH-SFG", "WIP-FJ-OUT", 2, "M3", mintB.BarcodeValue, "Available", plantId: Plant, publicId: mintB.PublicId, materialId: raw.Id, batchId: lotB.Id, warehouseId: wh.Id, locationId: loc.Id);
        var pkgQ = InventoryPackage.Create(mintQ.PackageNumber, "MI-Q", raw.Code, lotQ.BatchNumber, "WH-SFG", "Q-01", 1, "M3", mintQ.BarcodeValue, "Quarantine", plantId: Plant, publicId: mintQ.PublicId, materialId: raw.Id, batchId: lotQ.Id, warehouseId: wh.Id, locationId: qloc.Id);
        var pkgF02 = InventoryPackage.Create(mint2.PackageNumber, "MI-2", raw.Code, lotF02.BatchNumber, "WH-SFG", "WIP-FJ-OUT", 1, "M3", mint2.BarcodeValue, "Available", plantId: "PLANT-002", publicId: mint2.PublicId, materialId: raw.Id, batchId: lotF02.Id);
        db.InventoryPackages.AddRange(pkgA, pkgB, pkgQ, pkgF02);
        var content = InventoryPackageContent.Create(pkgA.Id, 1, 25, 90, 3400, 20, 2, "M3", Plant);
        db.Set<InventoryPackageContent>().Add(content);
        var plot = StructuralProductionLot.Create("PLOT-F01-000125", raw.Id, raw.Code, "MACHINE", DateOnly.FromDateTime(DateTime.UtcNow), "WC-FJ", plantId: Plant);
        db.StructuralProductionLots.Add(plot);
        await db.SaveChangesAsync();
        return new World(order.Id, raw.Id, wip.Id, clt.Id, wcFj.Id, wcGlu.Id, opFj.Id, opGlu.Id, pkgA.Id, content.Id, mintA.BarcodeValue, mintB.BarcodeValue, mintQ.BarcodeValue, mint2.BarcodeValue, lotA.BatchNumber, plot.Id, plot.ProductionLotNumber);
    }

    private async Task<HttpClient> SeedOperatorAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var users = scope.ServiceProvider.GetRequiredService<IAuthUserRepository>();
        var roles = scope.ServiceProvider.GetRequiredService<IRoleCatalogRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();
        if (await roles.GetByCodeAsync("WarehouseOperator") is null)
            await roles.AddAsync(AuthorizationCatalogSeed.CreateWarehouseOperatorRole());
        await users.AddAsync(AuthUser.Create("floorop", "Operator", "op@naswood.local", hasher.Hash("Naswood!Op1"), ["COMP-001"], ["PLANT-001"], ["WarehouseOperator"]));
        await uow.SaveChangesAsync();
        return await LoginAsync("floorop", "Naswood!Op1");
    }

    private async Task<HttpClient> SeedViewerAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var users = scope.ServiceProvider.GetRequiredService<IAuthUserRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();
        await users.AddAsync(AuthUser.Create("viewer", "Viewer", "viewer@naswood.local", hasher.Hash("Naswood!View1"), ["COMP-001"], ["PLANT-001"], ["ReadOnly"]));
        await uow.SaveChangesAsync();
        return await LoginAsync("viewer", "Naswood!View1");
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

    private static async Task<JsonElement> Data(HttpResponseMessage res)
    {
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("data").Clone();
    }

    private static async Task<string?> Code(HttpResponseMessage res)
    {
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("errors")[0].GetProperty("code").GetString();
    }

    private sealed record World(
        Guid OrderId, Guid RawId, Guid WipMatId, Guid CltId,
        Guid WcFjId, Guid WcGluId, Guid OpFjId, Guid OpGluId,
        Guid PackageAId, Guid ContentAId,
        string BarcodeA, string BarcodeB, string QuarantineBarcode, string F02Barcode,
        string LotANo, Guid PlotId, string PlotNumber)
    {
        public string UnknownBarcode => "NWPKG-NONE";

        public Guid GetOpId(HttpClient client, int seq)
        {
            var res = client.GetAsync($"/api/v1/production-execution/orders/{OrderId}").GetAwaiter().GetResult();
            res.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(res.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            foreach (var op in doc.RootElement.GetProperty("data").GetProperty("operations").EnumerateArray())
                if (op.GetProperty("sequence").GetInt32() == seq)
                    return op.GetProperty("id").GetGuid();
            throw new InvalidOperationException("op missing");
        }
    }
}

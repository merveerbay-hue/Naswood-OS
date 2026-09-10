using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public sealed class ProductionExecutionGateway
{
    private readonly IProductionExecutionStore _store;
    private readonly IProductionOrderRepository _orders;
    private readonly IOperationRepository _operations;
    private readonly IWorkCenterRepository _workCenters;
    private readonly IMaterialRepository _materials;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;
    private readonly IBatchRepository _batches;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryPackageRepository _packages;
    private readonly IInventoryMovementRepository _movements;
    private readonly IStructuralProductionLotRepository _plots;
    private readonly IProductionOutputRepository _outputs;
    private readonly IProductionLotSourceRepository _lotSources;
    private readonly ProductionOutputGateway _outputGate;

    public ProductionExecutionGateway(
        IProductionExecutionStore store,
        IProductionOrderRepository orders,
        IOperationRepository operations,
        IWorkCenterRepository workCenters,
        IMaterialRepository materials,
        IWarehouseRepository warehouses,
        ILocationRepository locations,
        IBatchRepository batches,
        IInventoryBalanceRepository balances,
        IInventoryPackageRepository packages,
        IInventoryMovementRepository movements,
        IStructuralProductionLotRepository plots,
        IProductionOutputRepository outputs,
        IProductionLotSourceRepository lotSources,
        ProductionOutputGateway outputGate)
    {
        _store = store;
        _orders = orders;
        _operations = operations;
        _workCenters = workCenters;
        _materials = materials;
        _warehouses = warehouses;
        _locations = locations;
        _batches = batches;
        _balances = balances;
        _packages = packages;
        _movements = movements;
        _plots = plots;
        _outputs = outputs;
        _lotSources = lotSources;
        _outputGate = outputGate;
    }

    public async Task<Result<IReadOnlyList<ShopFloorWorkCenterCardDto>>> ListWorkCentersAsync(
        IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var plant = allowed is { Count: > 0 } ? allowed[0] : null;
        var (items, _) = await _workCenters.SearchAsync(null, plant, 1, 200, cancellationToken).ConfigureAwait(false);
        var cards = new List<ShopFloorWorkCenterCardDto>();
        var today = DateTimeOffset.UtcNow.Date;
        foreach (var wc in items)
        {
            if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, wc.PlantId)) continue;
            var ops = await _store.ListOperationsByWorkCenterAsync(wc.Id, cancellationToken).ConfigureAwait(false);
            var execs = await _store.ListExecutionsByWorkCenterAsync(wc.Id, cancellationToken).ConfigureAwait(false);
            var pending = 0;
            var running = 0;
            foreach (var op in ops)
            {
                if (op.Status == ProductionOperationStatuses.Completed) continue;
                var open = execs.FirstOrDefault(e => e.ProductionOperationId == op.Id
                    && e.Status is ProductionExecutionStatuses.Running or ProductionExecutionStatuses.Paused);
                if (open is not null) running++;
                else if (await CanStartOperationAsync(op, cancellationToken).ConfigureAwait(false))
                    pending++;
            }
            cards.Add(new ShopFloorWorkCenterCardDto
            {
                Id = wc.Id,
                Code = wc.Code,
                Name = wc.Name,
                PendingCount = pending,
                RunningCount = running,
                CompletedTodayCount = execs.Count(e =>
                    e.Status == ProductionExecutionStatuses.Completed && e.CompletedAt is DateTimeOffset c && c.UtcDateTime.Date == today)
            });
        }
        return Result.Success<IReadOnlyList<ShopFloorWorkCenterCardDto>>(cards);
    }

    public async Task<Result<ShopFloorQueueDto>> QueueAsync(Guid workCenterId, IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var wc = await _workCenters.GetByIdAsync(workCenterId, cancellationToken).ConfigureAwait(false);
        if (wc is null || wc.IsDeleted)
            return FailQ("PRD-EXEC-WC-001", "İş merkezi bulunamadı.");
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, wc.PlantId))
            return Result.Failure<ShopFloorQueueDto>(Error.Forbidden("PRD-EXEC-403", "Bu tesiste yetkiniz yok."));

        var ops = await _store.ListOperationsByWorkCenterAsync(wc.Id, cancellationToken).ConfigureAwait(false);
        var execs = await _store.ListExecutionsByWorkCenterAsync(wc.Id, cancellationToken).ConfigureAwait(false);
        var pending = new List<ShopFloorQueueItemDto>();
        var running = new List<ShopFloorQueueItemDto>();
        var done = new List<ShopFloorQueueItemDto>();
        var today = DateTimeOffset.UtcNow.Date;

        foreach (var op in ops)
        {
            var order = await _orders.GetByIdAsync(op.ProductionOrderId, cancellationToken).ConfigureAwait(false);
            if (order is null || order.IsDeleted) continue;
            var master = await _operations.GetByIdAsync(op.OperationId, cancellationToken).ConfigureAwait(false);
            var open = execs.FirstOrDefault(e => e.ProductionOperationId == op.Id
                && e.Status is ProductionExecutionStatuses.Running or ProductionExecutionStatuses.Paused);
            var completed = execs.Where(e => e.ProductionOperationId == op.Id && e.Status == ProductionExecutionStatuses.Completed).ToList();
            if (open is not null)
                running.Add(Item(order, op, master, open));
            else if (completed.Count > 0)
            {
                foreach (var c in completed.Where(e => e.CompletedAt is DateTimeOffset at && at.UtcDateTime.Date == today))
                    done.Add(Item(order, op, master, c));
            }
            else if (await CanStartOperationAsync(op, cancellationToken).ConfigureAwait(false))
                pending.Add(Item(order, op, master, null));
        }

        return Result.Success(new ShopFloorQueueDto
        {
            WorkCenterId = wc.Id,
            WorkCenterCode = wc.Code,
            WorkCenterName = wc.Name,
            Pending = pending,
            Running = running,
            CompletedToday = done
        });
    }

    public async Task<Result<ProductionOrderProgressDto>> OrderProgressAsync(Guid orderId, IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(orderId, cancellationToken).ConfigureAwait(false);
        if (order is null || order.IsDeleted)
            return Result.Failure<ProductionOrderProgressDto>(Error.NotFound("PRD-EXEC-404", "Üretim emri bulunamadı."));
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, order.PlantId))
            return Result.Failure<ProductionOrderProgressDto>(Error.Forbidden("PRD-EXEC-403", "Bu tesiste yetkiniz yok."));
        return Result.Success(await MapOrderAsync(order, cancellationToken).ConfigureAwait(false));
    }

    public async Task<Result<ProductionOrderProgressDto>> UpsertOperationsAsync(
        Guid orderId, IReadOnlyList<UpsertProductionOperationRequestDto> steps, IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(orderId, cancellationToken).ConfigureAwait(false);
        if (order is null || order.IsDeleted)
            return Result.Failure<ProductionOrderProgressDto>(Error.NotFound("PRD-EXEC-404", "Üretim emri bulunamadı."));
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, order.PlantId))
            return Result.Failure<ProductionOrderProgressDto>(Error.Forbidden("PRD-EXEC-403", "Bu tesiste yetkiniz yok."));
        var existing = await _store.ListOperationsByOrderAsync(orderId, cancellationToken).ConfigureAwait(false);
        if (existing.Count > 0)
            return Result.Success(await MapOrderAsync(order, cancellationToken).ConfigureAwait(false));

        var ordered = steps.OrderBy(s => s.Sequence).ToList();
        var first = true;
        foreach (var step in ordered)
        {
            var op = await _operations.GetByIdAsync(step.OperationId, cancellationToken).ConfigureAwait(false);
            if (op is null || op.IsDeleted)
                return Result.Failure<ProductionOrderProgressDto>(Error.Validation("PRD-EXEC-001", "Operasyon master bulunamadı."));
            var wc = await _workCenters.GetByIdAsync(step.WorkCenterId, cancellationToken).ConfigureAwait(false);
            if (wc is null || wc.IsDeleted)
                return Result.Failure<ProductionOrderProgressDto>(Error.Validation("PRD-EXEC-WC-001", "İş merkezi bulunamadı."));
            if (!SamePlant(order.PlantId, wc.PlantId))
                return Result.Failure<ProductionOrderProgressDto>(Error.Validation("PRD-EXEC-PLANT-001", "Üretim emri ve iş merkezi aynı tesiste olmalıdır."));
            var plan = ProductionOperation.Create(
                order.Id, step.Sequence, step.OperationId, step.WorkCenterId, step.ExpectedMaterialId,
                step.OutputType, first ? ProductionOperationStatuses.Ready : ProductionOperationStatuses.Waiting,
                step.StructuralProductionLotId, order.PlantId);
            await _store.AddOperationAsync(plan, cancellationToken).ConfigureAwait(false);
            first = false;
        }
        return Result.Success(await MapOrderAsync(order, cancellationToken).ConfigureAwait(false));
    }

    public async Task<Result<ProductionExecutionPassportDto>> StartAsync(
        Guid operationId, Guid? workCenterId, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var plan = await _store.GetOperationAsync(operationId, cancellationToken).ConfigureAwait(false);
        if (plan is null)
            return NotFound("Operasyon bulunamadı.");
        var order = await _orders.GetByIdAsync(plan.ProductionOrderId, cancellationToken).ConfigureAwait(false);
        if (order is null || order.IsDeleted)
            return NotFound("Üretim emri bulunamadı.");
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, order.PlantId))
            return Forbidden();
        if (IsClosedOrder(order))
            return Fail("PRD-EXEC-001", "Üretim emri bu operasyonu başlatmak için uygun değil.");

        var wc = await _workCenters.GetByIdAsync(plan.WorkCenterId, cancellationToken).ConfigureAwait(false);
        if (wc is null || wc.IsDeleted || !string.Equals(wc.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Fail("PRD-EXEC-WC-001", "İş merkezi aktif değil.");
        if (workCenterId is Guid requested && requested != plan.WorkCenterId)
            return Fail("PRD-EXEC-WC-001", "Operasyon bu iş merkezinde yürütülemez.");
        if (!SamePlant(order.PlantId, wc.PlantId))
            return Fail("PRD-EXEC-PLANT-001", "Üretim emri ve stok aynı tesiste olmalıdır.");

        var open = await _store.GetOpenByOperationAsync(plan.Id, cancellationToken).ConfigureAwait(false);
        if (open is not null && open.Status is ProductionExecutionStatuses.Running or ProductionExecutionStatuses.Paused)
            return await LoadAsync(open.Id, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);

        if (!await CanStartOperationAsync(plan, cancellationToken).ConfigureAwait(false))
            return Fail("PRD-EXEC-002", "Önceki operasyon tamamlanmadan bu operasyon başlatılamaz.");

        var now = DateTimeOffset.UtcNow;
        ProductionOperationExecution exec;
        if (open is not null && open.Status == ProductionExecutionStatuses.NotStarted)
            exec = open;
        else
        {
            var existingNos = await _store.ListNumbersAsync(order.PlantId, cancellationToken).ConfigureAwait(false);
            var ordinal = OpeningInventoryCodes.NextOrdinal(existingNos.Select(x => ProductionExecutionNumbers.ParseOrdinal(x, order.PlantId, now)));
            exec = ProductionOperationExecution.Create(
                ProductionExecutionNumbers.Number(order.PlantId, now, ordinal),
                order.Id, plan.Id, plan.WorkCenterId, order.PlantId,
                structuralProductionLotId: plan.StructuralProductionLotId);
            await _store.AddExecutionAsync(exec, cancellationToken).ConfigureAwait(false);
        }

        try { exec.Start(actor, now); }
        catch (InvalidOperationException)
        {
            return Fail("PRD-EXEC-001", "Operasyon başlatılamıyor.");
        }
        plan.MarkInProgress();
        await _store.AddEventAsync(ProductionExecutionEvent.Create(
            exec.Id, ProductionExecutionEventTypes.Start, now, actor, order.PlantId), cancellationToken).ConfigureAwait(false);
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>> PauseAsync(Guid id, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(id, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        var check = ProductionExecutionPolicy.CanPause(exec.Status);
        if (check.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(check.Error!);
        var now = DateTimeOffset.UtcNow;
        exec.Pause(now);
        await _store.AddEventAsync(ProductionExecutionEvent.Create(
            exec.Id, ProductionExecutionEventTypes.Pause, now, actor, exec.PlantId), cancellationToken).ConfigureAwait(false);
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>> ResumeAsync(Guid id, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(id, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        var check = ProductionExecutionPolicy.CanResume(exec.Status);
        if (check.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(check.Error!);
        var now = DateTimeOffset.UtcNow;
        exec.Resume(now);
        await _store.AddEventAsync(ProductionExecutionEvent.Create(
            exec.Id, ProductionExecutionEventTypes.Resume, now, actor, exec.PlantId), cancellationToken).ConfigureAwait(false);
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionScanDto>> ScanAsync(Guid executionId, string barcode, IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(executionId, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionScanDto>(loaded.Error!);
        var exec = loaded.Value;
        var plan = await _store.GetOperationAsync(exec.ProductionOperationId, cancellationToken).ConfigureAwait(false);
        var hits = await _packages.ListByBarcodeExactAsync(barcode ?? "", cancellationToken).ConfigureAwait(false);
        if (hits.Count == 0)
            return Result.Failure<ProductionExecutionScanDto>(Error.NotFound("PRD-EXEC-INPUT-003", "PAKET BULUNAMADI"));
        var pkg = hits[0];
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, pkg.PlantId))
            return Result.Failure<ProductionExecutionScanDto>(Error.Forbidden("PRD-EXEC-403", "Bu paketi kullanma yetkiniz yok."));

        var consumptions = await _store.ListConsumptionsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        var existing = consumptions.LastOrDefault(c => c.SourcePackageId == pkg.Id);
        var material = await _materials.GetByCodeAsync(pkg.MaterialCode, cancellationToken).ConfigureAwait(false);
        var contents = await _packages.ListContentsAsync(pkg.Id, cancellationToken).ConfigureAwait(false);
        var can = PackageStatePolicy.CanConsume(pkg);
        var reason = PackageStatePolicy.InactiveReason(pkg);
        if (can && plan?.ExpectedMaterialId is Guid exp && pkg.MaterialId is Guid mid && mid != exp)
        {
            can = false;
            reason = "Bu paket operasyonun beklenen girdisiyle eşleşmiyor.";
        }
        if (can && !SamePlant(exec.PlantId, pkg.CurrentPlantId) && !SamePlant(exec.PlantId, pkg.PlantId))
        {
            can = false;
            reason = "Üretim emri ve stok aynı tesiste olmalıdır.";
        }

        Batch? lot = pkg.BatchId is Guid bid ? await _batches.GetByIdAsync(bid, cancellationToken).ConfigureAwait(false) : null;
        var balance = lot is null ? null : await _balances.FindByKeyAsync(
            pkg.MaterialCode, pkg.WarehouseCode, pkg.LocationCode, lot.BatchNumber, pkg.PlantId, cancellationToken).ConfigureAwait(false);
        var available = Math.Min(pkg.Quantity, balance is null ? 0 : balance.QuantityOnHand - balance.QuantityReserved);

        return Result.Success(new ProductionExecutionScanDto
        {
            PackageId = pkg.Id,
            PackageNo = pkg.PackageNumber,
            Barcode = pkg.Barcode,
            MaterialCode = pkg.MaterialCode,
            MaterialName = material?.Name ?? pkg.MaterialCode,
            SourceLotId = lot?.Id,
            SourceLotNumber = lot?.BatchNumber ?? pkg.LotNumber,
            WarehouseCode = pkg.WarehouseCode,
            LocationCode = pkg.LocationCode,
            AvailableQuantity = available,
            Unit = pkg.UnitOfMeasure,
            Status = pkg.Status,
            InactiveReason = reason,
            CanConsume = can && available > 0,
            ExistingConsumptionId = existing?.Id,
            Contents = contents.Select(c => new ProductionExecutionContentScanDto
            {
                Id = c.Id,
                Measurement = PackagePassportComposer.Measurement(c.ThicknessMm, c.WidthMm, c.LengthMm),
                Quantity = c.Quantity,
                PieceCount = c.PieceCount,
                Unit = c.UnitOfMeasure
            }).ToArray()
        });
    }

    public async Task<Result<ProductionExecutionPassportDto>> ConsumeAsync(
        Guid executionId, ConsumeProductionExecutionRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(executionId, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        if (exec.Status == ProductionExecutionStatuses.Completed)
            return Fail("PRD-EXEC-003", "Operasyon zaten tamamlanmış.");
        if (exec.Status == ProductionExecutionStatuses.Cancelled)
            return Fail("PRD-EXEC-001", "İptal edilen icra düzenlenemez.");
        if (exec.Status is not (ProductionExecutionStatuses.Running or ProductionExecutionStatuses.Paused))
            return Fail("PRD-EXEC-001", "Tüketim yalnız açık icrada yapılabilir.");

        var payloadHash = ProductionExecutionConcurrency.ConsumeHash(body.PackageId, body.Barcode, body.Quantity, body.PackageContentId, body.PieceCount);
        if (!string.IsNullOrWhiteSpace(body.IdempotencyKey))
        {
            var replay = await _store.GetByIdempotencyAsync(exec.Id, body.IdempotencyKey, cancellationToken).ConfigureAwait(false);
            if (replay is not null)
            {
                if (!string.Equals(replay.PayloadHash, payloadHash, StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(replay.PayloadHash))
                    return ProductionExecutionConcurrency.IdempotencyConflict<ProductionExecutionPassportDto>();
                return await LoadAsync(exec.Id, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
            }
        }

        InventoryPackage? pkg = null;
        if (body.PackageId is Guid pid)
            pkg = await _packages.GetByIdAsync(pid, cancellationToken).ConfigureAwait(false);
        if (pkg is null && !string.IsNullOrWhiteSpace(body.Barcode))
        {
            var hits = await _packages.ListByBarcodeExactAsync(body.Barcode, cancellationToken).ConfigureAwait(false);
            pkg = hits.FirstOrDefault();
        }
        if (pkg is null)
            return Fail("PRD-EXEC-INPUT-003", "PAKET BULUNAMADI");
        if (!PackageStatePolicy.CanConsume(pkg))
            return Fail("PRD-EXEC-INPUT-001", PackageStatePolicy.InactiveReason(pkg) ?? "Bu paket operasyon için uygun bir girdi değil.");
        if (!SamePlant(exec.PlantId, pkg.CurrentPlantId) && !SamePlant(exec.PlantId, pkg.PlantId))
            return Fail("PRD-EXEC-PLANT-001", "Üretim emri ve stok aynı tesiste olmalıdır.");

        var plan = await _store.GetOperationAsync(exec.ProductionOperationId, cancellationToken).ConfigureAwait(false);
        if (plan?.ExpectedMaterialId is Guid exp)
        {
            if (pkg.MaterialId is Guid mid && mid != exp)
                return Fail("PRD-EXEC-INPUT-001", "Bu paket operasyonun beklenen girdisiyle eşleşmiyor.");
            var expected = await _materials.GetByIdAsync(exp, cancellationToken).ConfigureAwait(false);
            if (expected is not null && !string.Equals(expected.Code, pkg.MaterialCode, StringComparison.OrdinalIgnoreCase))
                return Fail("PRD-EXEC-INPUT-001", "Bu paket operasyonun beklenen girdisiyle eşleşmiyor.");
        }

        if (body.Quantity <= 0)
            return Fail("PRD-EXEC-INPUT-002", "Paketin kullanılabilir miktarı yetersiz.");

        Batch? lot = pkg.BatchId is Guid bid ? await _batches.GetByIdAsync(bid, cancellationToken).ConfigureAwait(false) : null;
        lot ??= await _batches.GetByNumberAndMaterialAsync(pkg.LotNumber, pkg.MaterialCode, pkg.PlantId, cancellationToken).ConfigureAwait(false);
        if (lot is null)
            return Fail("PRD-EXEC-INPUT-001", "Paket lotu bulunamadı.");

        var srcMat = await _materials.GetByCodeAsync(pkg.MaterialCode, cancellationToken).ConfigureAwait(false);
        if (srcMat is null)
            return Fail("PRD-EXEC-INPUT-001", "Bu paket operasyon için uygun bir girdi değil.");

        var balance = await _balances.FindByKeyAsync(
            pkg.MaterialCode, pkg.WarehouseCode, pkg.LocationCode, lot.BatchNumber, exec.PlantId, cancellationToken).ConfigureAwait(false);
        if (balance is null)
            return Fail("PRD-EXEC-INPUT-002", "Paketin kullanılabilir miktarı yetersiz.");
        var available = Math.Min(pkg.Quantity, balance.QuantityOnHand - balance.QuantityReserved);
        if (body.Quantity > available)
            return Fail("PRD-EXEC-INPUT-002", "Paketin kullanılabilir miktarı yetersiz.");

        var measure = "";
        if (body.PackageContentId is Guid cid)
        {
            var contents = await _packages.ListContentsForUpdateAsync(pkg.Id, cancellationToken).ConfigureAwait(false);
            var row = contents.FirstOrDefault(c => c.Id == cid);
            if (row is null)
                return Fail("PRD-EXEC-INPUT-001", "Bu paket operasyon için uygun bir girdi değil.");
            try { row.Reduce(body.Quantity, body.PieceCount); }
            catch (InvalidOperationException ex)
            {
                return Fail("PRD-EXEC-INPUT-002", ex.Message);
            }
            measure = PackagePassportComposer.Measurement(row.ThicknessMm, row.WidthMm, row.LengthMm);
        }

        try { balance.ApplyIssue(body.Quantity); }
        catch (InvalidOperationException ex)
        {
            return Fail("PRD-EXEC-INPUT-002", ex.Message);
        }
        try { pkg.Consume(body.Quantity); }
        catch (InvalidOperationException ex)
        {
            return Fail("PRD-EXEC-INPUT-002", ex.Message);
        }

        var move = InventoryMovement.Post(
            ProductionLotCodes.ConsumptionMovement, "Out", exec.Number,
            pkg.MaterialCode, "", pkg.PackageNumber,
            pkg.WarehouseCode, pkg.LocationCode, lot.BatchNumber, body.Quantity,
            pkg.UnitOfMeasure,
            ProductionLotCodes.ConsumptionNotes(exec.Number, "", lot.BatchNumber, lot.Id, pkg.PackageNumber),
            plantId: exec.PlantId,
            packageId: pkg.Id);
        await _movements.AddAsync(move, cancellationToken).ConfigureAwait(false);

        var rowC = ProductionExecutionConsumption.Create(
            exec.Id, exec.ProductionOrderId, pkg.Id, lot.Id, srcMat.Id, srcMat.Code,
            pkg.PackageNumber, pkg.Barcode, pkg.WarehouseCode, pkg.LocationCode,
            body.PackageContentId, measure, body.Quantity, pkg.UnitOfMeasure, pkg.Quantity,
            body.IdempotencyKey ?? "", exec.PlantId, payloadHash: payloadHash, consumedPieceCount: body.PieceCount);
        await _store.AddConsumptionAsync(rowC, cancellationToken).ConfigureAwait(false);
        exec.AddInput(body.Quantity, pkg.UnitOfMeasure);
        _ = actor;
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>> StartDowntimeAsync(
        Guid id, DowntimeRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(id, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        var reason = ProductionExecutionPolicy.NormalizeDowntimeReason(body.ReasonCode);
        if (reason.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(reason.Error!);
        if (await HasOpenDowntimeAsync(exec.Id, cancellationToken).ConfigureAwait(false))
            return Fail("PRD-EXEC-DT-001", "Aktif duruş zaten açık.");
        var now = DateTimeOffset.UtcNow;
        await _store.AddEventAsync(ProductionExecutionEvent.Create(
            exec.Id, ProductionExecutionEventTypes.DowntimeStart, now, actor, exec.PlantId,
            body.ReasonCode, body.Note), cancellationToken).ConfigureAwait(false);
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>> EndDowntimeAsync(
        Guid id, DowntimeRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(id, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        var events = await _store.ListEventsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        var open = events.LastOrDefault(e => e.EventType == ProductionExecutionEventTypes.DowntimeStart);
        var lastEnd = events.LastOrDefault(e => e.EventType == ProductionExecutionEventTypes.DowntimeEnd);
        if (open is null || (lastEnd is not null && lastEnd.OccurredAt >= open.OccurredAt))
            return Fail("PRD-EXEC-DT-002", "Açık duruş yok.");
        var now = DateTimeOffset.UtcNow;
        exec.AddDowntimeMinutes(Math.Round((decimal)(now - open.OccurredAt).TotalMinutes, 4));
        await _store.AddEventAsync(ProductionExecutionEvent.Create(
            exec.Id, ProductionExecutionEventTypes.DowntimeEnd, now, actor, exec.PlantId,
            body.ReasonCode, body.Note), cancellationToken).ConfigureAwait(false);
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>> AddScrapAsync(
        Guid id, ScrapRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(id, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        if (exec.Status == ProductionExecutionStatuses.Completed)
            return Fail("PRD-EXEC-003", "Operasyon zaten tamamlanmış.");
        if (exec.Status == ProductionExecutionStatuses.Cancelled)
            return Fail("PRD-EXEC-001", "İptal edilen icra düzenlenemez.");
        var reason = ProductionExecutionPolicy.NormalizeScrapReason(body.ReasonCode);
        if (reason.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(reason.Error!);
        var qtyCheck = ProductionExecutionPolicy.GuardScrapQty(body.Quantity, exec.InputQuantity, exec.ScrapQuantity);
        if (qtyCheck.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(qtyCheck.Error!);
        var now = DateTimeOffset.UtcNow;
        exec.AddScrap(body.Quantity);
        await _store.AddScrapAsync(ProductionExecutionScrap.Create(
            exec.Id, body.Quantity, string.IsNullOrWhiteSpace(body.Unit) ? exec.Unit : body.Unit,
            body.ReasonCode, body.Note, actor, now, exec.PlantId), cancellationToken).ConfigureAwait(false);
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>> CompleteAsync(
        Guid id, CompleteProductionExecutionRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(id, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        if (exec.Status == ProductionExecutionStatuses.Cancelled)
            return Fail("PRD-EXEC-001", "İptal edilen icra düzenlenemez.");
        var completeHash = ProductionExecutionConcurrency.CompleteHash(
            body.OutputMaterialId, body.WarehouseCode, body.LocationCode, body.IdempotencyKey, body.Lines);
        if (!string.IsNullOrWhiteSpace(body.IdempotencyKey)
            && string.Equals(exec.CompleteIdempotencyKey, body.IdempotencyKey.Trim(), StringComparison.Ordinal)
            && !string.IsNullOrWhiteSpace(exec.CompletePayloadHash)
            && !string.Equals(exec.CompletePayloadHash, completeHash, StringComparison.OrdinalIgnoreCase))
            return ProductionExecutionConcurrency.IdempotencyConflict<ProductionExecutionPassportDto>();
        if (exec.Status == ProductionExecutionStatuses.Completed)
            return await LoadAsync(exec.Id, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(body.IdempotencyKey))
            exec.RememberCompleteIdempotency(body.IdempotencyKey, completeHash);

        var openDt = await HasOpenDowntimeAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        var can = ProductionExecutionPolicy.CanComplete(exec.Status, openDt);
        if (can.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(can.Error!);

        var plan = await _store.GetOperationAsync(exec.ProductionOperationId, cancellationToken).ConfigureAwait(false);
        if (plan is null) return NotFound("Operasyon bulunamadı.");
        var consumptions = await _store.ListConsumptionsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        if (plan.ExpectedMaterialId is not null && consumptions.Count == 0)
            return Fail("PRD-EXEC-INPUT-001", "Beklenen girdi tüketilmeden operasyon tamamlanamaz.");

        if (body.StructuralProductionLotId is Guid plotId)
        {
            var plot = await _plots.GetByIdAsync(plotId, cancellationToken).ConfigureAwait(false);
            if (plot is null)
                return Fail("PRD-EXEC-001", "PLOT bulunamadı.");
            exec.AttachPlot(plot.Id);
        }

        ProductionOutputResultDto? posted = null;
        if (ProductionOperationOutputTypes.RequiresStockOutput(plan.OutputType))
        {
            if (body.OutputMaterialId is not Guid mid || string.IsNullOrWhiteSpace(body.WarehouseCode) || string.IsNullOrWhiteSpace(body.LocationCode))
                return Fail("PRD-EXEC-OUT-001", "Stoklanabilir çıktı için malzeme, depo ve lokasyon gerekli.");
            if (body.Lines.Count == 0)
                return Fail("PRD-EXEC-OUT-001", "Çıktı ölçü satırı gerekli.");

            var order = await _orders.GetByIdAsync(exec.ProductionOrderId, cancellationToken).ConfigureAwait(false);
            var material = await _materials.GetByIdAsync(mid, cancellationToken).ConfigureAwait(false);
            if (material is null) return Fail("PRD-EXEC-OUT-001", "Çıktı malzemesi Material Master'da olmalı.");
            var wc = await _workCenters.GetByIdAsync(exec.WorkCenterId, cancellationToken).ConfigureAwait(false);
            var loc = await _locations.FindByWarehouseAndCodeAsync(body.WarehouseCode.Trim(), body.LocationCode.Trim(), exec.PlantId, cancellationToken).ConfigureAwait(false);
            if (loc is null) return Fail("PRD-EXEC-OUT-001", "Hedef lokasyon bulunamadı.");
            var qc = ProductionOutputQcPolicy.Resolve(material, wc?.Code, null);
            var stockStatus = ProductionOutputQcPolicy.Apply(qc, null, false);
            if (ProductionOutputQcPolicy.IsHold(stockStatus)
                && !StockZoneTypes.IsQuarantineCompatible(StockZoneTypes.Resolve(loc.StockZoneType, loc.LocationType)))
                return Fail("PRD-EXEC-ZONE-001", "Karantina çıktısı yalnız karantina lokasyonuna yazılabilir.");

            var existingOut = await _outputs.GetByExecutionIdAsync(exec.Id, cancellationToken).ConfigureAwait(false);
            if (existingOut is { Status: ProductionOutputStatuses.Posted })
            {
                exec.Complete(actor, DateTimeOffset.UtcNow);
                plan.MarkCompleted();
                await UnlockNextAsync(plan, cancellationToken).ConfigureAwait(false);
                return await LoadAsync(exec.Id, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
            }

            var postBody = new PostProductionOutputRequestDto
            {
                ProductionOrderId = exec.ProductionOrderId,
                OutputMaterialId = mid,
                WarehouseCode = body.WarehouseCode,
                LocationCode = body.LocationCode,
                WorkCenterCode = wc?.Code ?? "",
                PlantId = exec.PlantId,
                Lines = body.Lines,
                ProductionOperationExecutionId = exec.Id,
                Sources = consumptions.Select(c => new ProductionOutputSourceRequestDto
                {
                    SourceLotId = c.SourceLotId,
                    SourceWarehouseCode = c.SourceWarehouseCode,
                    SourceLocationCode = c.SourceLocationCode,
                    ConsumedQuantity = c.ConsumedQuantity,
                    Unit = c.Unit,
                    SourcePackageId = c.SourcePackageId
                }).ToArray()
            };
            var output = await _outputGate.PostAsync(postBody, allowed, actor, cancellationToken, skipSourceIssue: true).ConfigureAwait(false);
            if (output.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(output.Error!);
            posted = output.Value;
            exec.SetOutput(posted.OutputQuantity, null, loc.Id, posted.OutputId);
        }

        var now = DateTimeOffset.UtcNow;
        exec.Complete(actor, now);
        plan.MarkCompleted();
        await UnlockNextAsync(plan, cancellationToken).ConfigureAwait(false);
        await _store.AddEventAsync(ProductionExecutionEvent.Create(
            exec.Id, ProductionExecutionEventTypes.Complete, now, actor, exec.PlantId), cancellationToken).ConfigureAwait(false);
        _ = posted;
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>> CancelAsync(
        Guid id, CancelProductionExecutionRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var loaded = await RequireActiveAsync(id, allowed, cancellationToken).ConfigureAwait(false);
        if (loaded.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(loaded.Error!);
        var exec = loaded.Value;
        var reasonOk = ProductionExecutionPolicy.NormalizeCancelReason(body.CancelReason);
        if (reasonOk.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(reasonOk.Error!);
        var reason = body.CancelReason.Trim().ToUpperInvariant();
        var note = body.Note ?? "";
        var cancelHash = ProductionExecutionConcurrency.CancelHash(reason, note, body.IdempotencyKey);

        if (!string.IsNullOrWhiteSpace(body.IdempotencyKey)
            && string.Equals(exec.CancelIdempotencyKey, body.IdempotencyKey.Trim(), StringComparison.Ordinal)
            && !string.IsNullOrWhiteSpace(exec.CancelPayloadHash)
            && !string.Equals(exec.CancelPayloadHash, cancelHash, StringComparison.OrdinalIgnoreCase))
            return ProductionExecutionConcurrency.IdempotencyConflict<ProductionExecutionPassportDto>();

        if (exec.Status == ProductionExecutionStatuses.Cancelled)
            return await LoadAsync(exec.Id, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(body.IdempotencyKey))
            exec.RememberCancelIdempotency(body.IdempotencyKey, cancelHash);

        var output = exec.ProductionOutputId is Guid oid
            ? await _outputs.GetByIdAsync(oid, cancellationToken).ConfigureAwait(false)
            : null;
        output ??= await _outputs.GetByExecutionIdAsync(exec.Id, cancellationToken).ConfigureAwait(false);

        var blocked = await DownstreamBlocksCancelAsync(exec, output, cancellationToken).ConfigureAwait(false);
        if (blocked.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(blocked.Error!);

        var now = DateTimeOffset.UtcNow;
        if (output is { Status: ProductionOutputStatuses.Posted })
        {
            var reversed = await _outputGate.ReverseAsync(output.Id, reason, allowed, actor, cancellationToken).ConfigureAwait(false);
            if (reversed.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(reversed.Error!);
            await RestoreConsumptionContentsAsync(exec, cancellationToken).ConfigureAwait(false);
        }
        else if (output is null || output.Status != ProductionOutputStatuses.Cancelled)
        {
            var restored = await ReverseConsumptionsAsync(exec, cancellationToken).ConfigureAwait(false);
            if (restored.IsFailure) return Result.Failure<ProductionExecutionPassportDto>(restored.Error!);
        }

        exec.Cancel(actor, reason, note, now);
        var plan = await _store.GetOperationAsync(exec.ProductionOperationId, cancellationToken).ConfigureAwait(false);
        plan?.MarkReady();
        await _store.AddEventAsync(ProductionExecutionEvent.Create(
            exec.Id, ProductionExecutionEventTypes.Cancel, now, actor, exec.PlantId, reason, note), cancellationToken).ConfigureAwait(false);
        var consumptions = await _store.ListConsumptionsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        if (consumptions.Count > 0 || output is not null)
        {
            await _store.AddEventAsync(ProductionExecutionEvent.Create(
                exec.Id, ProductionExecutionEventTypes.Reversal, now, actor, exec.PlantId, reason, note), cancellationToken).ConfigureAwait(false);
        }
        return await LoadAsync(exec.Id, allowed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>?> TryRecoverStartAsync(
        Guid operationId, IReadOnlyList<string>? allowed, Exception ex, CancellationToken cancellationToken)
    {
        if (!ProductionExecutionConcurrency.IsUniqueViolation(ex, "open_operation"))
            return null;
        _store.ClearTracker();
        var open = await _store.GetCommittedOpenByOperationAsync(operationId, cancellationToken).ConfigureAwait(false);
        if (open is null) return Fail("PRD-EXEC-001", "Operasyon başlatılamıyor.");
        return await LoadAsync(open.Id, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>?> TryRecoverConsumeAsync(
        Guid executionId, ConsumeProductionExecutionRequestDto body, IReadOnlyList<string>? allowed, Exception ex, CancellationToken cancellationToken)
    {
        if (ProductionExecutionConcurrency.IsOptimisticConflict(ex))
            return ProductionExecutionConcurrency.PackageConflict<ProductionExecutionPassportDto>();
        if (!ProductionExecutionConcurrency.IsUniqueViolation(ex, "consume_idem"))
            return null;
        _store.ClearTracker();
        var replay = await _store.GetByIdempotencyAsync(executionId, body.IdempotencyKey, cancellationToken).ConfigureAwait(false);
        if (replay is null)
            return ProductionExecutionConcurrency.PackageConflict<ProductionExecutionPassportDto>();
        var hash = ProductionExecutionConcurrency.ConsumeHash(body.PackageId, body.Barcode, body.Quantity, body.PackageContentId, body.PieceCount);
        if (!string.Equals(replay.PayloadHash, hash, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(replay.PayloadHash))
            return ProductionExecutionConcurrency.IdempotencyConflict<ProductionExecutionPassportDto>();
        return await LoadAsync(executionId, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
    }

    public async Task<Result<ProductionExecutionPassportDto>?> TryRecoverCompleteAsync(
        Guid executionId, IReadOnlyList<string>? allowed, Exception ex, CancellationToken cancellationToken)
    {
        if (!ProductionExecutionConcurrency.IsOptimisticConflict(ex)
            && !ProductionExecutionConcurrency.IsUniqueViolation(ex, "prd_out_execution")
            && !ProductionExecutionConcurrency.IsUniqueViolation(ex, "23505"))
            return null;
        _store.ClearTracker();
        var committed = await _store.GetCommittedExecutionAsync(executionId, cancellationToken).ConfigureAwait(false);
        if (committed?.Status == ProductionExecutionStatuses.Completed)
            return await LoadAsync(executionId, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
        var existingOut = await _outputs.GetByExecutionIdAsync(executionId, cancellationToken).ConfigureAwait(false);
        if (existingOut is { Status: ProductionOutputStatuses.Posted })
            return await LoadAsync(executionId, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
        if (ProductionExecutionConcurrency.IsOptimisticConflict(ex))
            return ProductionExecutionConcurrency.PackageConflict<ProductionExecutionPassportDto>();
        return null;
    }

    public async Task<Result<ProductionExecutionPassportDto>?> TryRecoverCancelAsync(
        Guid executionId, IReadOnlyList<string>? allowed, Exception ex, CancellationToken cancellationToken)
    {
        if (!ProductionExecutionConcurrency.IsOptimisticConflict(ex) && !ProductionExecutionConcurrency.IsUniqueViolation(ex))
            return null;
        _store.ClearTracker();
        var committed = await _store.GetCommittedExecutionAsync(executionId, cancellationToken).ConfigureAwait(false);
        if (committed?.Status == ProductionExecutionStatuses.Cancelled)
            return await LoadAsync(executionId, allowed, cancellationToken, idempotent: true).ConfigureAwait(false);
        if (ProductionExecutionConcurrency.IsOptimisticConflict(ex))
            return ProductionExecutionConcurrency.PackageConflict<ProductionExecutionPassportDto>();
        return null;
    }

    private async Task<Result> DownstreamBlocksCancelAsync(
        ProductionOperationExecution exec, ProductionOutput? output, CancellationToken cancellationToken)
    {
        var outputIds = new List<Guid>();
        if (output?.OutputBatchId is Guid lotId)
        {
            var pkgs = await _packages.ListByBatchIdAsync(lotId, cancellationToken).ConfigureAwait(false);
            outputIds.AddRange(pkgs.Select(p => p.Id));
        }
        if (outputIds.Count == 0) return Result.Success();

        var laterConsume = await _store.ListConsumptionsBySourcePackageIdsAsync(outputIds, cancellationToken).ConfigureAwait(false);
        if (laterConsume.Any(c => c.ExecutionId != exec.Id))
            return Result.Failure(Error.Validation("PKG-LIFE-006", "Paket başka üretimde kullanıldığı için işlem geri alınamaz."));

        var laterSource = await _lotSources.ListBySourcePackageIdsAsync(outputIds, cancellationToken).ConfigureAwait(false);
        if (laterSource.Any(s => output is null || s.ProductionOutputId != output.Id))
            return Result.Failure(Error.Validation("PKG-LIFE-006", "Paket başka üretimde kullanıldığı için işlem geri alınamaz."));
        return Result.Success();
    }

    private async Task<Result> ReverseConsumptionsAsync(ProductionOperationExecution exec, CancellationToken cancellationToken)
    {
        var rows = await _store.ListConsumptionsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        foreach (var row in rows)
        {
            var pkg = await _packages.GetByIdAsync(row.SourcePackageId, cancellationToken).ConfigureAwait(false);
            if (pkg is null)
                return Result.Failure(Error.Validation("PKG-LIFE-006", "Tüketilen paket restore edilemedi."));
            var sameNo = pkg.PackageNumber;
            var sameBarcode = pkg.Barcode;
            try { pkg.Restore(row.ConsumedQuantity); }
            catch (InvalidOperationException)
            {
                return Result.Failure(Error.Validation("PKG-LIFE-006", "Paket başka üretimde kullanıldığı için işlem geri alınamaz."));
            }
            if (!string.Equals(pkg.PackageNumber, sameNo, StringComparison.Ordinal)
                || !string.Equals(pkg.Barcode, sameBarcode, StringComparison.Ordinal))
                return Result.Failure(Error.Validation("PRD-OUT-026", "Kaynak paket kimliği reversal'da değişemez."));

            if (row.PackageContentId is Guid cid)
            {
                var contents = await _packages.ListContentsForUpdateAsync(pkg.Id, cancellationToken).ConfigureAwait(false);
                var content = contents.FirstOrDefault(c => c.Id == cid);
                if (content is null)
                    return Result.Failure(Error.Validation("PKG-LIFE-006", "Paket içeriği restore edilemedi."));
                content.Restore(row.ConsumedQuantity, row.ConsumedPieceCount);
            }

            var lot = await _batches.GetByIdAsync(row.SourceLotId, cancellationToken).ConfigureAwait(false);
            if (lot is null)
                return Result.Failure(Error.Validation("PRD-EXEC-INPUT-001", "Paket lotu bulunamadı."));
            var balance = await _balances.FindByKeyAsync(
                pkg.MaterialCode, row.SourceWarehouseCode, row.SourceLocationCode, lot.BatchNumber, exec.PlantId, cancellationToken).ConfigureAwait(false);
            if (balance is null)
            {
                balance = InventoryBalance.Create(
                    pkg.MaterialCode, row.SourceWarehouseCode, row.SourceLocationCode, lot.BatchNumber, 0, 0, "Active", plantId: exec.PlantId);
                await _balances.AddAsync(balance, cancellationToken).ConfigureAwait(false);
            }
            balance.ApplyReceipt(row.ConsumedQuantity);

            await _movements.AddAsync(InventoryMovement.Post(
                ProductionLotCodes.ConsumptionReversalMovement, "In", exec.Number,
                pkg.MaterialCode, "", pkg.PackageNumber,
                row.SourceWarehouseCode, row.SourceLocationCode, lot.BatchNumber, row.ConsumedQuantity,
                row.Unit,
                ProductionLotCodes.ConsumptionNotes(exec.Number, "", lot.BatchNumber, lot.Id, pkg.PackageNumber) + " reverse=1",
                plantId: exec.PlantId,
                packageId: pkg.Id), cancellationToken).ConfigureAwait(false);
        }
        return Result.Success();
    }

    private async Task RestoreConsumptionContentsAsync(ProductionOperationExecution exec, CancellationToken cancellationToken)
    {
        var rows = await _store.ListConsumptionsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        foreach (var row in rows)
        {
            if (row.PackageContentId is not Guid cid) continue;
            var contents = await _packages.ListContentsForUpdateAsync(row.SourcePackageId, cancellationToken).ConfigureAwait(false);
            var content = contents.FirstOrDefault(c => c.Id == cid);
            content?.Restore(row.ConsumedQuantity, row.ConsumedPieceCount);
        }
    }

    public async Task<Result<ProductionExecutionPassportDto>> LoadAsync(
        Guid id, IReadOnlyList<string>? allowed, CancellationToken cancellationToken, bool idempotent = false)
    {
        var exec = await _store.GetExecutionAsync(id, cancellationToken).ConfigureAwait(false);
        if (exec is null)
            return NotFound("İcra bulunamadı.");
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, exec.PlantId))
            return Forbidden();
        var plan = await _store.GetOperationAsync(exec.ProductionOperationId, cancellationToken).ConfigureAwait(false);
        var order = await _orders.GetByIdAsync(exec.ProductionOrderId, cancellationToken).ConfigureAwait(false);
        var master = plan is null ? null : await _operations.GetByIdAsync(plan.OperationId, cancellationToken).ConfigureAwait(false);
        var wc = await _workCenters.GetByIdAsync(exec.WorkCenterId, cancellationToken).ConfigureAwait(false);
        var events = await _store.ListEventsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        var inputs = await _store.ListConsumptionsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        var scraps = await _store.ListScrapsAsync(exec.Id, cancellationToken).ConfigureAwait(false);
        var now = DateTimeOffset.UtcNow;
        var openDt = HasOpenDowntime(events);
        var plotNo = "";
        if (exec.StructuralProductionLotId is Guid pid)
        {
            var plot = await _plots.GetByIdAsync(pid, cancellationToken).ConfigureAwait(false);
            plotNo = plot?.ProductionLotNumber ?? "";
        }

        ProductionOutput? output = null;
        if (exec.ProductionOutputId is Guid oid)
            output = await _outputs.GetByIdAsync(oid, cancellationToken).ConfigureAwait(false);
        output ??= await _outputs.GetByExecutionIdAsync(exec.Id, cancellationToken).ConfigureAwait(false);

        var packages = Array.Empty<ProductionOutputPackageCreatedDto>();
        var sources = Array.Empty<ProductionLotSourceDto>();
        if (output?.OutputBatchId is Guid lotId)
        {
            var lotPass = await _outputGate.LoadLotAsync(lotId, allowed, cancellationToken).ConfigureAwait(false);
            if (lotPass.IsSuccess)
            {
                packages = lotPass.Value.Packages.ToArray();
                sources = lotPass.Value.SourceLots.ToArray();
            }
        }

        var actions = new List<string>();
        if (exec.Status == ProductionExecutionStatuses.Running)
        {
            actions.AddRange(["PAUSE", "DOWNTIME", "SCAN", "CONSUME", "SCRAP", "COMPLETE"]);
        }
        else if (exec.Status == ProductionExecutionStatuses.Paused)
        {
            actions.AddRange(["RESUME", "DOWNTIME", "SCAN", "CONSUME", "SCRAP", "COMPLETE"]);
        }
        if (openDt) actions.Remove("COMPLETE");

        return Result.Success(new ProductionExecutionPassportDto
        {
            Id = exec.Id,
            Number = exec.Number,
            Status = exec.Status,
            ProductionOrderId = exec.ProductionOrderId,
            ProductionOrderNumber = order?.Code ?? "",
            ProductionOrderName = order?.Name ?? "",
            ProductionOperationId = exec.ProductionOperationId,
            Sequence = plan?.Sequence ?? 0,
            OperationCode = master?.Code ?? "",
            OperationName = master?.Name ?? "",
            WorkCenterId = exec.WorkCenterId,
            WorkCenterCode = wc?.Code ?? "",
            WorkCenterName = wc?.Name ?? "",
            PlantId = exec.PlantId ?? "",
            StartedByUserId = exec.StartedByUserId,
            CompletedByUserId = exec.CompletedByUserId,
            StartedAt = exec.StartedAt,
            CompletedAt = exec.CompletedAt,
            TotalRunMinutes = exec.LiveRunMinutes(now),
            TotalPauseMinutes = exec.LivePauseMinutes(now),
            TotalDowntimeMinutes = exec.TotalDowntimeMinutes,
            InputQuantity = exec.InputQuantity,
            OutputQuantity = exec.OutputQuantity,
            ScrapQuantity = exec.ScrapQuantity,
            Unit = exec.Unit,
            OutputType = plan?.OutputType ?? "",
            StructuralProductionLotId = exec.StructuralProductionLotId,
            StructuralProductionLotNumber = plotNo,
            ProductionOutputId = output?.Id,
            ProductionOutputNumber = output?.Number ?? "",
            ProductionLotId = output?.OutputBatchId,
            ProductionLotNumber = output?.OutputLotNumber ?? "",
            QcStatus = output?.StockStatus ?? "",
            ExpectedMaterialId = plan?.ExpectedMaterialId,
            IdempotentReplay = idempotent,
            AllowedActions = actions,
            Events = events.Select(e => new ProductionExecutionEventDto
            {
                EventType = e.EventType,
                OccurredAt = e.OccurredAt,
                UserId = e.UserId,
                ReasonCode = e.ReasonCode,
                Note = e.Note
            }).ToArray(),
            Inputs = await MapInputsAsync(inputs, cancellationToken).ConfigureAwait(false),
            Scraps = scraps.Select(s => new ProductionExecutionScrapDto
            {
                Id = s.Id,
                Quantity = s.Quantity,
                Unit = s.Unit,
                ReasonCode = s.ReasonCode,
                Note = s.Note,
                UserId = s.UserId,
                RecordedAt = s.RecordedAt
            }).ToArray(),
            Packages = packages,
            SourceLots = sources
        });
    }

    private async Task<IReadOnlyList<ProductionExecutionConsumptionDto>> MapInputsAsync(
        IReadOnlyList<ProductionExecutionConsumption> inputs, CancellationToken cancellationToken)
    {
        var rows = new List<ProductionExecutionConsumptionDto>();
        foreach (var i in inputs)
        {
            var lot = await _batches.GetByIdAsync(i.SourceLotId, cancellationToken).ConfigureAwait(false);
            rows.Add(new ProductionExecutionConsumptionDto
            {
                Id = i.Id,
                SourcePackageId = i.SourcePackageId,
                Barcode = i.Barcode,
                PackageNumber = i.PackageNumber,
                MaterialCode = i.SourceMaterialCode,
                LotNumber = lot?.BatchNumber ?? "",
                SourceLotId = i.SourceLotId,
                PhysicalMeasure = i.PhysicalMeasure,
                ConsumedQuantity = i.ConsumedQuantity,
                RemainingPackageQuantity = i.RemainingPackageQuantity,
                Unit = i.Unit
            });
        }
        return rows;
    }

    private async Task UnlockNextAsync(ProductionOperation completed, CancellationToken cancellationToken)
    {
        var next = await _store.FindByOrderSequenceAsync(completed.ProductionOrderId, completed.Sequence + 10, cancellationToken).ConfigureAwait(false);
        next ??= (await _store.ListOperationsByOrderAsync(completed.ProductionOrderId, cancellationToken).ConfigureAwait(false))
            .Where(o => o.Sequence > completed.Sequence && o.Status != ProductionOperationStatuses.Completed)
            .OrderBy(o => o.Sequence).FirstOrDefault();
        next?.MarkReady();
    }

    private async Task<bool> CanStartOperationAsync(ProductionOperation plan, CancellationToken cancellationToken)
    {
        if (plan.Status == ProductionOperationStatuses.Completed) return false;
        var siblings = await _store.ListOperationsByOrderAsync(plan.ProductionOrderId, cancellationToken).ConfigureAwait(false);
        var prev = siblings.Where(o => o.Sequence < plan.Sequence).OrderByDescending(o => o.Sequence).FirstOrDefault();
        if (prev is null) return true;
        return prev.Status == ProductionOperationStatuses.Completed;
    }

    private async Task<Result<ProductionOperationExecution>> RequireActiveAsync(Guid id, IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var exec = await _store.GetExecutionAsync(id, cancellationToken).ConfigureAwait(false);
        if (exec is null) return Result.Failure<ProductionOperationExecution>(Error.NotFound("PRD-EXEC-404", "İcra bulunamadı."));
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, exec.PlantId))
            return Result.Failure<ProductionOperationExecution>(Error.Forbidden("PRD-EXEC-403", "Bu tesiste yetkiniz yok."));
        return Result.Success(exec);
    }

    private async Task<bool> HasOpenDowntimeAsync(Guid executionId, CancellationToken cancellationToken)
        => HasOpenDowntime(await _store.ListEventsAsync(executionId, cancellationToken).ConfigureAwait(false));

    private static bool HasOpenDowntime(IReadOnlyList<ProductionExecutionEvent> events)
    {
        var start = events.LastOrDefault(e => e.EventType == ProductionExecutionEventTypes.DowntimeStart);
        if (start is null) return false;
        var end = events.LastOrDefault(e => e.EventType == ProductionExecutionEventTypes.DowntimeEnd);
        return end is null || end.OccurredAt < start.OccurredAt;
    }

    private async Task<ProductionOrderProgressDto> MapOrderAsync(ProductionOrder order, CancellationToken cancellationToken)
    {
        var ops = await _store.ListOperationsByOrderAsync(order.Id, cancellationToken).ConfigureAwait(false);
        var execs = await _store.ListExecutionsByOrderAsync(order.Id, cancellationToken).ConfigureAwait(false);
        var rows = new List<ProductionOperationPlanDto>();
        foreach (var op in ops)
        {
            var master = await _operations.GetByIdAsync(op.OperationId, cancellationToken).ConfigureAwait(false);
            var wc = await _workCenters.GetByIdAsync(op.WorkCenterId, cancellationToken).ConfigureAwait(false);
            var mat = op.ExpectedMaterialId is Guid mid ? await _materials.GetByIdAsync(mid, cancellationToken).ConfigureAwait(false) : null;
            var open = execs.FirstOrDefault(e => e.ProductionOperationId == op.Id
                && e.Status is ProductionExecutionStatuses.Running or ProductionExecutionStatuses.Paused);
            rows.Add(new ProductionOperationPlanDto
            {
                Id = op.Id,
                Sequence = op.Sequence,
                OperationId = op.OperationId,
                OperationCode = master?.Code ?? "",
                OperationName = master?.Name ?? "",
                WorkCenterId = op.WorkCenterId,
                WorkCenterCode = wc?.Code ?? "",
                WorkCenterName = wc?.Name ?? "",
                ExpectedMaterialId = op.ExpectedMaterialId,
                ExpectedMaterialCode = mat?.Code ?? "",
                OutputType = op.OutputType,
                Status = op.Status,
                StructuralProductionLotId = op.StructuralProductionLotId,
                ActiveExecutionId = open?.Id,
                CanStart = await CanStartOperationAsync(op, cancellationToken).ConfigureAwait(false) && open is null
            });
        }
        return new ProductionOrderProgressDto
        {
            Id = order.Id,
            Number = order.Code,
            Name = order.Name,
            Status = order.Status,
            PlantId = order.PlantId ?? "",
            CompletedOperations = ops.Count(o => o.Status == ProductionOperationStatuses.Completed),
            TotalOperations = ops.Count,
            Operations = rows
        };
    }

    private static ShopFloorQueueItemDto Item(ProductionOrder order, ProductionOperation op, Operation? master, ProductionOperationExecution? exec)
        => new()
        {
            ProductionOrderId = order.Id,
            ProductionOrderNumber = order.Code,
            ProductionOrderName = order.Name,
            ProductionOperationId = op.Id,
            Sequence = op.Sequence,
            OperationName = master?.Name ?? "",
            Status = exec?.Status ?? op.Status,
            ExecutionId = exec?.Id,
            ExecutionNumber = exec?.Number ?? ""
        };

    private static bool SamePlant(string? a, string? b)
        => string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)
            || string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool IsClosedOrder(ProductionOrder order)
    {
        var s = (order.Status ?? "").Trim();
        return s.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)
            || s.Equals("Closed", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<Result<ShopFloorFeedbackDto>> SubmitFeedbackAsync(
        ShopFloorFeedbackRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken cancellationToken)
    {
        var topic = (body.Topic ?? "").Trim().ToUpperInvariant();
        if (!ShopFloorFeedbackTopics.All.Contains(topic))
            return Result.Failure<ShopFloorFeedbackDto>(Error.Validation("PRD-FB-001", "Geçerli bir sorun türü seçin."));
        string? plant = allowed is { Count: > 0 } ? allowed[0] : null;
        if (body.ExecutionId is Guid eid)
        {
            var exec = await _store.GetExecutionAsync(eid, cancellationToken).ConfigureAwait(false);
            if (exec is not null)
            {
                if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, exec.PlantId))
                    return Result.Failure<ShopFloorFeedbackDto>(Error.Forbidden("PRD-EXEC-403", "Bu tesiste yetkiniz yok."));
                plant = exec.PlantId ?? plant;
            }
        }
        var row = ShopFloorFieldFeedback.Create(
            topic, body.Note, body.Screen, actor,
            body.WorkCenterId, body.WorkCenterCode, body.ExecutionId, body.ExecutionNumber,
            body.ProductionOrderNumber, plant);
        await _store.AddFeedbackAsync(row, cancellationToken).ConfigureAwait(false);
        return Result.Success(MapFeedback(row));
    }

    public async Task<Result<IReadOnlyList<ShopFloorFeedbackDto>>> ListFeedbackAsync(
        IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var plant = allowed is { Count: > 0 } ? allowed[0] : null;
        var rows = await _store.ListFeedbackAsync(plant, 100, cancellationToken).ConfigureAwait(false);
        return Result.Success<IReadOnlyList<ShopFloorFeedbackDto>>(rows.Select(MapFeedback).ToArray());
    }

    private static ShopFloorFeedbackDto MapFeedback(ShopFloorFieldFeedback row)
        => new()
        {
            Id = row.Id,
            Topic = row.Topic,
            Note = row.Note,
            Screen = row.Screen,
            UserId = row.UserId,
            WorkCenterId = row.WorkCenterId,
            WorkCenterCode = row.WorkCenterCode,
            ExecutionId = row.ExecutionId,
            ExecutionNumber = row.ExecutionNumber,
            ProductionOrderNumber = row.ProductionOrderNumber,
            PlantId = row.PlantId ?? "",
            OccurredAt = row.CreatedAt
        };

    private static Result<ProductionExecutionPassportDto> Fail(string code, string msg)
        => Result.Failure<ProductionExecutionPassportDto>(Error.Validation(code, msg));
    private static Result<ShopFloorQueueDto> FailQ(string code, string msg)
        => Result.Failure<ShopFloorQueueDto>(Error.Validation(code, msg));
    private static Result<ProductionExecutionPassportDto> NotFound(string msg)
        => Result.Failure<ProductionExecutionPassportDto>(Error.NotFound("PRD-EXEC-404", msg));
    private static Result<ProductionExecutionPassportDto> Forbidden()
        => Result.Failure<ProductionExecutionPassportDto>(Error.Forbidden("PRD-EXEC-403", "Bu tesiste yetkiniz yok."));
}

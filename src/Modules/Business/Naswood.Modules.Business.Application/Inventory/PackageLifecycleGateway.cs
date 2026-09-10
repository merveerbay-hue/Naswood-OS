using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public sealed record SplitPackageCommand(
    Guid PackageId,
    SplitPackageRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<PackageOperationResultDto>>;

public sealed record MergePackagesCommand(
    MergePackagesRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<PackageOperationResultDto>>;

public sealed record RepackPackageCommand(
    Guid PackageId,
    RepackPackageRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<PackageOperationResultDto>>;

public sealed record PartialMovePackageCommand(
    Guid PackageId,
    PartialMovePackageRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<PackageOperationResultDto>>;

public static class PackageGenealogy
{
    public static bool WouldCycle(
        Guid sourcePackageId,
        Guid targetPackageId,
        IReadOnlyList<PackageRelation> existing)
    {
        if (sourcePackageId == targetPackageId) return true;
        var outgoing = existing.ToLookup(r => r.SourcePackageId);
        var seen = new HashSet<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(targetPackageId);
        while (stack.Count > 0)
        {
            var n = stack.Pop();
            if (n == sourcePackageId) return true;
            if (!seen.Add(n)) continue;
            foreach (var edge in outgoing[n])
                stack.Push(edge.TargetPackageId);
        }
        return false;
    }
}

public sealed class PackageLifecycleGateway
{
    private readonly IInventoryPackageRepository _packages;
    private readonly IPackageOperationRepository _operations;
    private readonly IPackageRelationRepository _relations;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryMovementRepository _movements;
    private readonly ILocationRepository _locations;
    private readonly IWarehouseRepository _warehouses;
    private readonly PackagePassportLoader _loader;
    private readonly IBusinessUnitOfWork _uow;

    public PackageLifecycleGateway(
        IInventoryPackageRepository packages,
        IPackageOperationRepository operations,
        IPackageRelationRepository relations,
        IInventoryBalanceRepository balances,
        IInventoryMovementRepository movements,
        ILocationRepository locations,
        IWarehouseRepository warehouses,
        PackagePassportLoader loader,
        IBusinessUnitOfWork uow)
    {
        _packages = packages;
        _operations = operations;
        _relations = relations;
        _balances = balances;
        _movements = movements;
        _locations = locations;
        _warehouses = warehouses;
        _loader = loader;
        _uow = uow;
    }

    public Task<Result<PackageOperationResultDto>> SplitAsync(
        Guid packageId, SplitPackageRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken ct)
        => ExecuteSplitLikeAsync(packageId, body.Number, body.PhysicalGroupLabel, body.Lines, null, null, PackageOperationTypes.Split, allowed, actor, ct);

    public Task<Result<PackageOperationResultDto>> PartialMoveAsync(
        Guid packageId, PartialMovePackageRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken ct)
        => ExecuteSplitLikeAsync(packageId, body.Number, body.PhysicalGroupLabel, body.Lines, body.WarehouseCode, body.LocationCode, PackageOperationTypes.PartialMove, allowed, actor, ct);

    public async Task<Result<PackageOperationResultDto>> MergeAsync(
        MergePackagesRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken ct)
    {
        var ids = (body.SourcePackageIds ?? []).Where(x => x != Guid.Empty).Distinct().ToArray();
        if (ids.Length < 2)
            return Fail("PKG-MERGE-001", "Birleştirmek için en az iki paket seçin.");

        var sources = new List<InventoryPackage>();
        foreach (var id in ids)
        {
            var pkg = await _packages.GetByIdAsync(id, ct).ConfigureAwait(false);
            if (pkg is null || pkg.IsDeleted)
                return Fail("INV-PKG-404", "Paket bulunamadı.");
            sources.Add(pkg);
        }

        var plantId = sources[0].PlantId ?? "";
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, plantId))
            return Result.Failure<PackageOperationResultDto>(Error.Forbidden("INV-PKG-403", "Bu tesiste paket birleştirme yetkiniz yok."));

        foreach (var src in sources)
        {
            if (!string.Equals(src.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                return Fail("PKG-MERGE-007", "Farklı tesisteki paketler birleştirilemez.");
            if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, src.PlantId))
                return Result.Failure<PackageOperationResultDto>(Error.Forbidden("INV-PKG-403", "Kaynak paket tesisi yetki dışı."));
            var g = PackageStatePolicy.GuardRejected(src, "MERGE");
            if (g.IsFailure) return Result.Failure<PackageOperationResultDto>(g.Error!);
            if (!PackageStatePolicy.CanMerge(src))
                return Fail("PKG-LIFE-006", $"Paket birleştirilemez ({src.PackageNumber}).");
        }

        if (sources.Select(s => s.MaterialId).Distinct().Count() != 1)
            return Fail("PKG-MERGE-002", "Farklı malzemelere ait paketler birleştirilemez.");
        if (sources.Select(s => s.BatchId).Distinct().Count() != 1)
            return Fail("PKG-MERGE-003", "Farklı lotlara ait paketler birleştirilemez.");
        if (sources.Select(s => $"{s.WarehouseCode}|{s.LocationCode}".ToUpperInvariant()).Distinct().Count() != 1)
            return Fail("PKG-MERGE-004", "Farklı lokasyondaki paketler birleştirilemez. Önce taşıyın.");
        if (sources.Select(s => s.Status).Distinct(StringComparer.OrdinalIgnoreCase).Count() != 1)
            return Fail("PKG-MERGE-005", "Karantina ve available paketler karıştırılamaz.");

        var replay = await ReplayIfExistsAsync(body.Number, plantId, allowed, ct).ConfigureAwait(false);
        if (replay is not null) return replay;

        var existingRels = await _relations.ListByPackageIdsAsync(ids, ct).ConfigureAwait(false);
        var number = SystemIdentifier.Ensure(body.Number, "PKOP");
        var op = PackageOperation.Create(number, PackageOperationTypes.Merge, sources[0].WarehouseCode, sources[0].LocationCode, actor, plantId);
        var total = sources.Sum(s => s.Quantity);
        var first = sources[0];
        var child = await MintChildAsync(first, total, first.Status, first.WarehouseCode, first.LocationCode, first.WarehouseId, first.LocationId, body.PhysicalGroupLabel, ct).ConfigureAwait(false);

        foreach (var src in sources)
        {
            if (PackageGenealogy.WouldCycle(src.Id, child.Id, existingRels))
                return Fail("PKG-LIFE-008", "Paket soy ağacında döngü oluşturulamaz.");
            src.MarkMerged();
        }

        var rebuilt = new List<InventoryPackageContent>();
        var lineNo = 1;
        var buckets = new List<(decimal? T, decimal? W, decimal? L, decimal Qty, decimal? Pcs, string Uom)>();
        foreach (var src in sources)
        {
            foreach (var row in await _packages.ListContentsAsync(src.Id, ct).ConfigureAwait(false))
            {
                var idx = buckets.FindIndex(b => b.T == row.ThicknessMm && b.W == row.WidthMm && b.L == row.LengthMm);
                if (idx < 0)
                    buckets.Add((row.ThicknessMm, row.WidthMm, row.LengthMm, row.Quantity, row.PieceCount, row.UnitOfMeasure));
                else
                {
                    var b = buckets[idx];
                    buckets[idx] = (b.T, b.W, b.L, b.Qty + row.Quantity, (b.Pcs ?? 0) + (row.PieceCount ?? 0), b.Uom);
                }
            }
        }
        foreach (var b in buckets.Where(x => x.Qty > 0))
            rebuilt.Add(InventoryPackageContent.Create(child.Id, lineNo++, b.T, b.W, b.L, b.Pcs is > 0 ? b.Pcs : null, b.Qty, b.Uom, plantId));

        await _packages.AddAsync(child, ct).ConfigureAwait(false);
        if (rebuilt.Count > 0)
            await _packages.AddContentsAsync(rebuilt, ct).ConfigureAwait(false);
        await _operations.AddAsync(op, ct).ConfigureAwait(false);
        var relRows = sources.Select(s => PackageRelation.Create(op.Id, s.Id, child.Id, PackageRelationTypes.Merge, s.Quantity, s.UnitOfMeasure, plantId)).ToArray();
        await _relations.AddRangeAsync(relRows, ct).ConfigureAwait(false);
        await AuditZeroAsync("PACKAGE_MERGE", op.Number, child, $"merge sources={string.Join(',', sources.Select(s => s.PackageNumber))}", ct).ConfigureAwait(false);
        return await CommitAsync(op, sources[0], child, sources, allowed, false,
            $"{sources.Count} paket birleştirildi. Yeni paket: {child.PackageNumber}", reprintOriginal: false, printTarget: true, ct).ConfigureAwait(false);
    }

    public async Task<Result<PackageOperationResultDto>> RepackAsync(
        Guid packageId, RepackPackageRequestDto body, IReadOnlyList<string>? allowed, string actor, CancellationToken ct)
    {
        var src = await _packages.GetByIdAsync(packageId, ct).ConfigureAwait(false);
        if (src is null || src.IsDeleted)
            return Result.Failure<PackageOperationResultDto>(Error.NotFound("INV-PKG-404", "Paket bulunamadı."));
        var plantId = src.PlantId ?? "";
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, plantId))
            return Result.Failure<PackageOperationResultDto>(Error.Forbidden("INV-PKG-403", "Bu paketi yeniden paketleme yetkiniz yok."));
        var g = PackageStatePolicy.GuardRejected(src, "REPACK");
        if (g.IsFailure) return Result.Failure<PackageOperationResultDto>(g.Error!);
        if (!PackageStatePolicy.CanRepack(src))
            return Fail("PKG-LIFE-006", "Paket yeniden paketlenemez.");

        var replay = await ReplayIfExistsAsync(body.Number, plantId, allowed, ct).ConfigureAwait(false);
        if (replay is not null) return replay;

        var number = SystemIdentifier.Ensure(body.Number, "PKOP");
        var op = PackageOperation.Create(number, PackageOperationTypes.Repack, src.WarehouseCode, src.LocationCode, actor, plantId);
        var child = await MintChildAsync(src, src.Quantity, src.Status, src.WarehouseCode, src.LocationCode, src.WarehouseId, src.LocationId, body.PhysicalGroupLabel, ct).ConfigureAwait(false);
        var contents = await _packages.ListContentsAsync(src.Id, ct).ConfigureAwait(false);
        var copies = contents.Select((c, i) => InventoryPackageContent.Create(
            child.Id, i + 1, c.ThicknessMm, c.WidthMm, c.LengthMm, c.PieceCount, c.Quantity, c.UnitOfMeasure, plantId)).ToArray();
        src.MarkRepacked();
        await _packages.AddAsync(child, ct).ConfigureAwait(false);
        if (copies.Length > 0)
            await _packages.AddContentsAsync(copies, ct).ConfigureAwait(false);
        await _operations.AddAsync(op, ct).ConfigureAwait(false);
        await _relations.AddRangeAsync(
            [PackageRelation.Create(op.Id, src.Id, child.Id, PackageRelationTypes.Repack, src.Quantity, src.UnitOfMeasure, plantId)], ct).ConfigureAwait(false);
        await AuditZeroAsync("PACKAGE_REPACK", op.Number, child, $"repack from={src.PackageNumber}", ct).ConfigureAwait(false);
        return await CommitAsync(op, src, child, [src], allowed, false,
            $"Yeni fiziksel paket oluşturuldu. Old: CLOSED · New: {child.PackageNumber}", reprintOriginal: false, printTarget: true, ct).ConfigureAwait(false);
    }

    private async Task<Result<PackageOperationResultDto>> ExecuteSplitLikeAsync(
        Guid packageId,
        string? numberHint,
        string? physicalGroupLabel,
        IReadOnlyList<PackageContentSplitLineDto> lines,
        string? destWarehouse,
        string? destLocation,
        string operationType,
        IReadOnlyList<string>? allowed,
        string actor,
        CancellationToken ct)
    {
        var parent = await _packages.GetByIdAsync(packageId, ct).ConfigureAwait(false);
        if (parent is null || parent.IsDeleted)
            return Result.Failure<PackageOperationResultDto>(Error.NotFound("INV-PKG-404", "Paket bulunamadı."));
        var plantId = parent.PlantId ?? "";
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, plantId))
            return Result.Failure<PackageOperationResultDto>(Error.Forbidden("INV-PKG-403", "Bu tesiste paket işlemi yetkiniz yok."));

        var action = operationType == PackageOperationTypes.PartialMove ? "MOVE" : "SPLIT";
        var g = PackageStatePolicy.GuardRejected(parent, action);
        if (g.IsFailure) return Result.Failure<PackageOperationResultDto>(g.Error!);
        if (operationType == PackageOperationTypes.PartialMove)
        {
            if (!PackageStatePolicy.CanMove(parent))
                return Fail("PKG-LIFE-006", "Paket taşınamaz.");
        }
        else if (!PackageStatePolicy.CanSplit(parent))
            return Fail("PKG-LIFE-006", "Paket bölünemez.");

        var replay = await ReplayIfExistsAsync(numberHint, plantId, allowed, ct).ConfigureAwait(false);
        if (replay is not null) return replay;

        var contents = (await _packages.ListContentsForUpdateAsync(parent.Id, ct).ConfigureAwait(false)).ToList();
        var allocated = Allocate(parent, contents, lines ?? []);
        if (allocated.IsFailure) return Result.Failure<PackageOperationResultDto>(allocated.Error!);
        var plan = allocated.Value;
        if (plan.Total <= 0)
            return Fail("PKG-SPLIT-002", "Dağıtılan miktar 0'dan büyük olmalı.");
        if (plan.Total > parent.Quantity)
            return Fail("PKG-SPLIT-001", "Dağıtılan miktar mevcut paket miktarını aşıyor.");
        if (plan.Total == parent.Quantity)
        {
            return operationType == PackageOperationTypes.PartialMove
                ? Fail("PKG-SPLIT-003", "Tüm miktar taşınıyorsa tam lokasyon değişikliği kullanın.")
                : Fail("PKG-SPLIT-003", "Tüm miktar yeni pakete geçiyorsa yeniden paketleme kullanın.");
        }

        Guid? destWhId = parent.WarehouseId;
        Guid? destLocId = parent.LocationId;
        var childWh = parent.WarehouseCode;
        var childLoc = parent.LocationCode;
        if (operationType == PackageOperationTypes.PartialMove)
        {
            childWh = (destWarehouse ?? "").Trim();
            childLoc = (destLocation ?? "").Trim();
            if (childWh.Length == 0 || childLoc.Length == 0)
                return Fail("PKG-MOVE-001", "Hedef depo ve lokasyon zorunlu.");
            if (string.Equals(childWh, parent.WarehouseCode, StringComparison.OrdinalIgnoreCase)
                && string.Equals(childLoc, parent.LocationCode, StringComparison.OrdinalIgnoreCase))
                return Fail("PKG-MOVE-002", "Kısmi taşımada hedef lokasyon kaynak ile aynı olamaz.");
            if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, plantId))
                return Result.Failure<PackageOperationResultDto>(Error.Forbidden("INV-PKG-403", "Hedef tesis yetki dışı."));
            var loc = await _locations.FindByWarehouseAndCodeAsync(childWh, childLoc, plantId, ct).ConfigureAwait(false);
            if (loc is null)
                return Result.Failure<PackageOperationResultDto>(Error.Forbidden("INV-PKG-403", $"Lokasyon '{childLoc}' bu tesise ait değil."));
            var destWh = await _warehouses.GetByCodeAndPlantAsync(childWh, plantId, ct).ConfigureAwait(false);
            destWhId = destWh?.Id;
            destLocId = loc.Id;
        }

        var number = SystemIdentifier.Ensure(numberHint, "PKOP");
        var op = PackageOperation.Create(number, operationType, parent.WarehouseCode, parent.LocationCode, actor, plantId);
        var child = await MintChildAsync(parent, plan.Total, parent.Status, childWh, childLoc, destWhId, destLocId, physicalGroupLabel, ct).ConfigureAwait(false);

        foreach (var step in plan.Rows)
            step.Content.Reduce(step.Quantity, step.PieceCount);
        parent.ReducePhysical(plan.Total);

        var childContents = plan.Rows.Select((s, i) => InventoryPackageContent.Create(
            child.Id, i + 1, s.Content.ThicknessMm, s.Content.WidthMm, s.Content.LengthMm, s.PieceCount, s.Quantity, s.Content.UnitOfMeasure, plantId)).ToList();
        if (contents.Count == 0)
            childContents.Add(InventoryPackageContent.Create(child.Id, 1, null, null, null, null, plan.Total, parent.UnitOfMeasure, plantId));

        await _packages.AddAsync(child, ct).ConfigureAwait(false);
        if (childContents.Count > 0)
            await _packages.AddContentsAsync(childContents, ct).ConfigureAwait(false);
        await _operations.AddAsync(op, ct).ConfigureAwait(false);
        var relType = operationType == PackageOperationTypes.PartialMove ? PackageRelationTypes.PartialMove : PackageRelationTypes.Split;
        await _relations.AddRangeAsync(
            [PackageRelation.Create(op.Id, parent.Id, child.Id, relType, plan.Total, parent.UnitOfMeasure, plantId)], ct).ConfigureAwait(false);

        if (operationType == PackageOperationTypes.PartialMove)
        {
            var moved = await TransferBalanceAsync(parent, child, childWh, childLoc, plan.Total, op.Number, actor, ct).ConfigureAwait(false);
            if (moved.IsFailure) return Result.Failure<PackageOperationResultDto>(moved.Error!);
        }
        else
        {
            await AuditZeroAsync("PACKAGE_SPLIT", op.Number, parent, $"split {plan.Total} → {child.PackageNumber}", ct).ConfigureAwait(false);
        }

        var msg = operationType == PackageOperationTypes.PartialMove
            ? $"Kısmi taşıma: orijinal {parent.PackageNumber} {parent.Quantity} {parent.UnitOfMeasure}; yeni {child.PackageNumber} {child.Quantity} {child.UnitOfMeasure}."
            : $"Paket bölündü. Orijinal: {parent.PackageNumber} {parent.Quantity} {parent.UnitOfMeasure}. Yeni paket: {child.PackageNumber} {child.Quantity} {child.UnitOfMeasure}.";
        return await CommitAsync(op, parent, child, [parent], allowed, false, msg, reprintOriginal: true, printTarget: true, ct).ConfigureAwait(false);
    }

    private async Task<Result> TransferBalanceAsync(
        InventoryPackage parent, InventoryPackage child, string toWh, string toLoc, decimal qty, string doc, string actor, CancellationToken ct)
    {
        var plantId = parent.PlantId;
        var source = await _balances.FindByKeyAsync(parent.MaterialCode, parent.WarehouseCode, parent.LocationCode, parent.LotNumber, plantId, ct).ConfigureAwait(false);
        if (source is null)
            return Result.Failure(Error.Validation("INV-TRF-011", "Kaynak stok bakiyesi yok."));
        try { source.ApplyIssue(qty); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Validation("INV-TRF-012", ex.Message));
        }
        var dest = await _balances.FindByKeyAsync(parent.MaterialCode, toWh, toLoc, parent.LotNumber, plantId, ct).ConfigureAwait(false);
        if (dest is null)
        {
            dest = InventoryBalance.Create(parent.MaterialCode, toWh, toLoc, parent.LotNumber, 0, 0, "Active", plantId: plantId);
            await _balances.AddAsync(dest, ct).ConfigureAwait(false);
        }
        dest.ApplyReceipt(qty);
        var note = $"from={parent.PackageNumber} to={child.PackageNumber} partial {parent.WarehouseCode}/{parent.LocationCode}→{toWh}/{toLoc} qty={qty} by={actor}";
        await _movements.AddAsync(InventoryMovement.Post("PACKAGE_MOVE", "Out", doc, parent.MaterialCode, parent.MaterialIdentityNumber, parent.PackageNumber, parent.WarehouseCode, parent.LocationCode, parent.LotNumber, qty, parent.UnitOfMeasure, note, plantId: plantId), ct).ConfigureAwait(false);
        await _movements.AddAsync(InventoryMovement.Post("PACKAGE_MOVE", "In", doc, child.MaterialCode, child.MaterialIdentityNumber, child.PackageNumber, toWh, toLoc, child.LotNumber, qty, child.UnitOfMeasure, note, plantId: plantId), ct).ConfigureAwait(false);
        return Result.Success();
    }

    private async Task AuditZeroAsync(string type, string doc, InventoryPackage pkg, string notes, CancellationToken ct)
    {
        await _movements.AddAsync(InventoryMovement.Post(
            type, "In", doc, pkg.MaterialCode, pkg.MaterialIdentityNumber, pkg.PackageNumber,
            pkg.WarehouseCode, pkg.LocationCode, pkg.LotNumber, 0, pkg.UnitOfMeasure, notes, plantId: pkg.PlantId), ct).ConfigureAwait(false);
    }

    private async Task<InventoryPackage> MintChildAsync(
        InventoryPackage parent,
        decimal qty,
        string status,
        string warehouseCode,
        string locationCode,
        Guid? warehouseId,
        Guid? locationId,
        string? physicalGroupLabel,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var existing = await _packages.ListPackageNumbersAsync(ct).ConfigureAwait(false);
        var ordinal = OpeningInventoryCodes.NextOrdinal(existing.Select(x => OpeningInventoryCodes.ParsePackageOrdinal(x, parent.PlantId, now)));
        var mint = PackageIdentityService.Mint(parent.PlantId, now, ordinal);
        return InventoryPackage.Create(
            mint.PackageNumber,
            parent.MaterialIdentityNumber,
            parent.MaterialCode,
            parent.LotNumber,
            warehouseCode,
            locationCode,
            qty,
            parent.UnitOfMeasure,
            mint.BarcodeValue,
            status,
            plantId: parent.PlantId,
            publicId: mint.PublicId,
            physicalGroupLabel: physicalGroupLabel ?? "",
            sourcePlantId: parent.SourcePlantId,
            materialId: parent.MaterialId,
            batchId: parent.BatchId,
            warehouseId: warehouseId,
            locationId: locationId);
    }

    private async Task<Result<PackageOperationResultDto>?> ReplayIfExistsAsync(
        string? number, string? plantId, IReadOnlyList<string>? allowed, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(number)) return null;
        var existing = await _operations.GetByNumberAsync(number.Trim(), plantId, ct).ConfigureAwait(false);
        if (existing is null) return null;
        var rels = await _relations.ListByOperationIdAsync(existing.Id, ct).ConfigureAwait(false);
        var sources = new List<InventoryPackage>();
        InventoryPackage? target = null;
        foreach (var r in rels)
        {
            var src = await _packages.GetByIdAsync(r.SourcePackageId, ct).ConfigureAwait(false);
            var tgt = await _packages.GetByIdAsync(r.TargetPackageId, ct).ConfigureAwait(false);
            if (src is not null) sources.Add(src);
            target = tgt ?? target;
        }
        var sourceDto = sources.Count > 0 ? (await _loader.LoadAsync(sources[0], allowed, ct).ConfigureAwait(false)).Value : null;
        var targetDto = target is null ? null : (await _loader.LoadAsync(target, allowed, ct).ConfigureAwait(false)).Value;
        var sourceDtos = new List<PackagePassportDto>();
        foreach (var s in sources.DistinctBy(x => x.Id))
        {
            var loaded = await _loader.LoadAsync(s, allowed, ct).ConfigureAwait(false);
            if (loaded.IsSuccess) sourceDtos.Add(loaded.Value);
        }
        return Result.Success(new PackageOperationResultDto
        {
            OperationId = existing.Id,
            Number = existing.Number,
            OperationType = existing.OperationType,
            IdempotentReplay = true,
            Source = sourceDto,
            Target = targetDto,
            Sources = sourceDtos,
            Message = "İşlem daha önce kaydedildi.",
            ReprintOriginal = existing.OperationType is PackageOperationTypes.Split or PackageOperationTypes.PartialMove,
            PrintTarget = true
        });
    }

    private async Task<Result<PackageOperationResultDto>> CommitAsync(
        PackageOperation op,
        InventoryPackage source,
        InventoryPackage target,
        IReadOnlyList<InventoryPackage> sources,
        IReadOnlyList<string>? allowed,
        bool replay,
        string message,
        bool reprintOriginal,
        bool printTarget,
        CancellationToken ct)
    {
        try
        {
            await _uow.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("Concurrency", StringComparison.OrdinalIgnoreCase)
            || ex.InnerException?.GetType().Name.Contains("Concurrency", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Result.Failure<PackageOperationResultDto>(Error.Conflict("PKG-LIFE-009", "Paket aynı anda başka bir işlemde kullanıldı. Yeniden deneyin."));
        }

        var srcDto = await _loader.LoadAsync(source, allowed, ct).ConfigureAwait(false);
        var tgtDto = await _loader.LoadAsync(target, allowed, ct).ConfigureAwait(false);
        if (srcDto.IsFailure) return Result.Failure<PackageOperationResultDto>(srcDto.Error!);
        if (tgtDto.IsFailure) return Result.Failure<PackageOperationResultDto>(tgtDto.Error!);
        var extras = new List<PackagePassportDto>();
        foreach (var s in sources)
        {
            var loaded = await _loader.LoadAsync(s, allowed, ct).ConfigureAwait(false);
            if (loaded.IsSuccess) extras.Add(loaded.Value);
        }
        return Result.Success(new PackageOperationResultDto
        {
            OperationId = op.Id,
            Number = op.Number,
            OperationType = op.OperationType,
            IdempotentReplay = replay,
            Source = srcDto.Value,
            Target = tgtDto.Value,
            Sources = extras,
            Message = message,
            ReprintOriginal = reprintOriginal,
            PrintTarget = printTarget
        });
    }

    private static Result<Allocation> Allocate(
        InventoryPackage parent,
        IReadOnlyList<InventoryPackageContent> contents,
        IReadOnlyList<PackageContentSplitLineDto> lines)
    {
        if (lines.Count == 0)
            return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-002", "Bölünecek ölçü satırı gerekli."));

        if (contents.Count == 0)
        {
            var qty = lines.Sum(l => l.Quantity);
            if (lines.Any(l => l.Quantity <= 0))
                return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-002", "Dağıtılan miktar 0'dan büyük olmalı."));
            if (qty > parent.Quantity)
                return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-001", "Dağıtılan miktar mevcut paket miktarını aşıyor."));
            return Result.Success(new Allocation(qty, []));
        }

        var left = contents.ToDictionary(c => c.Id, c => (Qty: c.Quantity, Pcs: c.PieceCount));
        var rows = new List<AllocatedRow>();
        foreach (var line in lines)
        {
            if (line.Quantity <= 0)
                return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-002", "Dağıtılan miktar 0'dan büyük olmalı."));
            InventoryPackageContent? row = null;
            if (line.SourceContentId is Guid cid)
                row = contents.FirstOrDefault(c => c.Id == cid);
            row ??= contents.FirstOrDefault(c => c.SameMeasurement(line.ThicknessMm, line.WidthMm, line.LengthMm));
            if (row is null && contents.Count == 1 && lines.Count == 1)
                row = contents[0];
            if (row is null)
                return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-004", "Ölçü satırı pakette bulunamadı."));
            var rem = left[row.Id];
            if (line.Quantity > rem.Qty)
                return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-001", "Dağıtılan miktar mevcut paket miktarını aşıyor."));
            decimal? pcs = line.PieceCount;
            if (pcs is decimal takePcs)
            {
                if (rem.Pcs is decimal have && takePcs > have)
                    return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-001", "Dağıtılan adet mevcut paket adedini aşıyor."));
            }
            else if (row.PieceCount is decimal havePcs && row.Quantity > 0)
                pcs = Math.Round(havePcs * (line.Quantity / row.Quantity), 4);
            left[row.Id] = (rem.Qty - line.Quantity, rem.Pcs is decimal p ? p - (pcs ?? 0) : rem.Pcs);
            rows.Add(new AllocatedRow(row, line.Quantity, pcs));
        }

        var total = rows.Sum(r => r.Quantity);
        if (total > parent.Quantity)
            return Result.Failure<Allocation>(Error.Validation("PKG-SPLIT-001", "Dağıtılan miktar mevcut paket miktarını aşıyor."));
        return Result.Success(new Allocation(total, rows));
    }

    private static Result<PackageOperationResultDto> Fail(string code, string message)
        => Result.Failure<PackageOperationResultDto>(Error.Validation(code, message));

    private sealed record AllocatedRow(InventoryPackageContent Content, decimal Quantity, decimal? PieceCount);
    private sealed record Allocation(decimal Total, IReadOnlyList<AllocatedRow> Rows);
}

public sealed class SplitPackageCommandHandler : ICommandHandler<SplitPackageCommand, Result<PackageOperationResultDto>>
{
    private readonly PackageLifecycleGateway _gate;
    public SplitPackageCommandHandler(PackageLifecycleGateway gate) => _gate = gate;
    public Task<Result<PackageOperationResultDto>> HandleAsync(SplitPackageCommand command, CancellationToken cancellationToken = default)
        => _gate.SplitAsync(command.PackageId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken);
}

public sealed class MergePackagesCommandHandler : ICommandHandler<MergePackagesCommand, Result<PackageOperationResultDto>>
{
    private readonly PackageLifecycleGateway _gate;
    public MergePackagesCommandHandler(PackageLifecycleGateway gate) => _gate = gate;
    public Task<Result<PackageOperationResultDto>> HandleAsync(MergePackagesCommand command, CancellationToken cancellationToken = default)
        => _gate.MergeAsync(command.Body, command.AllowedPlantIds, command.Actor, cancellationToken);
}

public sealed class RepackPackageCommandHandler : ICommandHandler<RepackPackageCommand, Result<PackageOperationResultDto>>
{
    private readonly PackageLifecycleGateway _gate;
    public RepackPackageCommandHandler(PackageLifecycleGateway gate) => _gate = gate;
    public Task<Result<PackageOperationResultDto>> HandleAsync(RepackPackageCommand command, CancellationToken cancellationToken = default)
        => _gate.RepackAsync(command.PackageId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken);
}

public sealed class PartialMovePackageCommandHandler : ICommandHandler<PartialMovePackageCommand, Result<PackageOperationResultDto>>
{
    private readonly PackageLifecycleGateway _gate;
    public PartialMovePackageCommandHandler(PackageLifecycleGateway gate) => _gate = gate;
    public Task<Result<PackageOperationResultDto>> HandleAsync(PartialMovePackageCommand command, CancellationToken cancellationToken = default)
        => _gate.PartialMoveAsync(command.PackageId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken);
}

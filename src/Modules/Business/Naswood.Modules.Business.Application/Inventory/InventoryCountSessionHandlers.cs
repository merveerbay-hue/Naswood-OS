using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public static class InventoryCountSessionComposer
{
    public static IReadOnlyList<InventoryCountLineDto> ToLineDtos(IReadOnlyList<InventoryCountLine> lines)
    {
        var physicalKeys = lines
            .Where(l => l.Role == "PHYSICAL" && !l.IsDeleted)
            .GroupBy(l => InventoryCountMath.PhysicalKey(l.MaterialCode, l.LocationCode, l.BatchNumber, l.ThicknessMm, l.WidthMm, l.LengthMm))
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

        var grouped = Group(lines);

        return lines
            .Where(l => !l.IsDeleted)
            .OrderBy(l => l.LineNo)
            .Select(l =>
            {
                var key = InventoryCountMath.GroupKey(l.MaterialCode, l.LocationCode, l.BatchNumber);
                grouped.TryGetValue(key, out var g);
                var system = g?.SystemQty ?? (l.Role == "SNAPSHOT" ? l.SystemQuantityAtStart : 0);
                var counted = g?.CountedQty ?? l.CountedQuantity;
                var hasPhysical = g?.HasPhysical ?? l.Role == "PHYSICAL";
                var status = InventoryCountMath.LineStatus(
                    l.Role == "SNAPSHOT" ? l.SystemQuantityAtStart : system,
                    l.Role == "PHYSICAL" ? l.CountedQuantity : counted,
                    l.Role == "PHYSICAL" || hasPhysical);
                if (l.Role == "SNAPSHOT")
                    status = InventoryCountMath.LineStatus(l.SystemQuantityAtStart, counted, hasPhysical);
                var physKey = InventoryCountMath.PhysicalKey(l.MaterialCode, l.LocationCode, l.BatchNumber, l.ThicknessMm, l.WidthMm, l.LengthMm);
                var dup = l.Role == "PHYSICAL" && physicalKeys.TryGetValue(physKey, out var n) && n > 1 && !l.KeepSeparate;
                return new InventoryCountLineDto
                {
                    Id = l.Id,
                    LineNo = l.LineNo,
                    Role = l.Role,
                    Source = l.Source,
                    MaterialId = l.MaterialId,
                    MaterialCode = l.MaterialCode,
                    MaterialName = l.MaterialName,
                    LocationCode = l.LocationCode,
                    BatchNumber = l.BatchNumber,
                    LotUnknown = l.LotUnknown,
                    PackageNumber = l.PackageNumber,
                    ThicknessMm = l.ThicknessMm,
                    WidthMm = l.WidthMm,
                    LengthMm = l.LengthMm,
                    PieceCount = l.PieceCount,
                    MeasuredVolumeM3 = l.MeasuredVolumeM3,
                    SystemQuantityAtStart = l.Role == "SNAPSHOT" ? l.SystemQuantityAtStart : system,
                    CountedQuantity = l.Role == "PHYSICAL" ? l.CountedQuantity : counted,
                    Difference = InventoryCountMath.Difference(
                        l.Role == "PHYSICAL" ? l.CountedQuantity : counted,
                        l.Role == "SNAPSHOT" ? l.SystemQuantityAtStart : system),
                    StockUnit = l.StockUnit,
                    CountUnit = l.CountUnit,
                    CalculatedStockQty = l.CalculatedStockQty,
                    LineStatus = dup ? "DUPLICATE" : status,
                    Duplicate = dup,
                    KeepSeparate = l.KeepSeparate,
                    Approved = l.Approved,
                    Notes = l.Notes
                };
            })
            .ToArray();
    }

    public static InventoryCountSummaryDto Summarize(IReadOnlyList<InventoryCountLineDto> lines, bool mid)
    {
        var groups = lines
            .GroupBy(l => InventoryCountMath.GroupKey(l.MaterialCode, l.LocationCode, l.BatchNumber))
            .ToArray();
        int matched = 0, variance = 0, unexpected = 0, missing = 0, unmatched = 0;
        foreach (var g in groups)
        {
            var snap = g.FirstOrDefault(x => x.Role == "SNAPSHOT");
            var phys = g.Where(x => x.Role == "PHYSICAL").ToArray();
            var system = snap?.SystemQuantityAtStart ?? 0;
            var counted = phys.Length > 0 ? phys.Sum(x => x.CountedQuantity) : 0m;
            var st = InventoryCountMath.LineStatus(system, counted, phys.Length > 0);
            switch (st)
            {
                case "MATCHED": matched++; break;
                case "UNEXPECTED": unexpected++; break;
                case "MISSING": missing++; break;
                default: variance++; break;
            }
        }
        return new InventoryCountSummaryDto
        {
            TotalLines = lines.Count,
            Matched = matched,
            Variance = variance,
            Unexpected = unexpected,
            Missing = missing,
            Unmatched = unmatched,
            Duplicates = lines.Count(l => l.Duplicate),
            MidCountMovement = mid
        };
    }

    public static Dictionary<string, CountGroup> Group(IReadOnlyList<InventoryCountLine> lines)
    {
        var map = new Dictionary<string, CountGroup>(StringComparer.Ordinal);
        foreach (var l in lines.Where(x => !x.IsDeleted))
        {
            var key = InventoryCountMath.GroupKey(l.MaterialCode, l.LocationCode, l.BatchNumber);
            if (!map.TryGetValue(key, out var g))
            {
                g = new CountGroup(l.MaterialCode, l.LocationCode, InventoryCountMath.NormalizeLot(l.BatchNumber), l.StockUnit);
                map[key] = g;
            }
            if (l.Role == "SNAPSHOT")
            {
                g.SystemQty += l.SystemQuantityAtStart;
                g.Snapshot = l;
            }
            else
            {
                g.CountedQty += l.CountedQuantity;
                g.HasPhysical = true;
                g.Physical.Add(l);
            }
        }
        return map;
    }

    public sealed class CountGroup
    {
        public CountGroup(string materialCode, string locationCode, string lot, string unit)
        {
            MaterialCode = materialCode;
            LocationCode = locationCode;
            Lot = lot;
            Unit = unit;
        }

        public string MaterialCode { get; }
        public string LocationCode { get; }
        public string Lot { get; }
        public string Unit { get; }
        public decimal SystemQty { get; set; }
        public decimal CountedQty { get; set; }
        public bool HasPhysical { get; set; }
        public InventoryCountLine? Snapshot { get; set; }
        public List<InventoryCountLine> Physical { get; } = [];
    }
}

public sealed record ReplaceInventoryCountLinesCommand(
    Guid CountId,
    IReadOnlyList<UpsertInventoryCountLineRequestDto> Lines,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<InventoryCountDto>>;

public sealed record CompleteInventoryCountCommand(
    Guid CountId,
    string Actor,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<InventoryCountDto>>;

public sealed record CancelInventoryCountCommand(
    Guid CountId,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<InventoryCountDto>>;

public sealed record PostInventoryCountCommand(
    Guid CountId,
    string Actor,
    string Reason,
    bool ApproveAllVariances,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<InventoryCountPostResultDto>>;

public sealed class ReplaceInventoryCountLinesCommandHandler : ICommandHandler<ReplaceInventoryCountLinesCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IMaterialRepository _materials;
    private readonly ILocationRepository _locations;
    private readonly IBusinessUnitOfWork _uow;

    public ReplaceInventoryCountLinesCommandHandler(
        IInventoryCountRepository repo,
        IMaterialRepository materials,
        ILocationRepository locations,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _materials = materials;
        _locations = locations;
        _uow = uow;
    }

    public async Task<Result<InventoryCountDto>> HandleAsync(ReplaceInventoryCountLinesCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.CountId, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<InventoryCountDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<InventoryCountDto>(Error.Forbidden("INV-CNT-403", "Bu sayım belgesini değiştirme yetkiniz yok."));
        if (e.IsTerminal())
            return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-048", "Posted/cancelled counts cannot accept lines."));

        var plantId = e.PlantId?.Trim() ?? string.Empty;
        var existing = await _repo.ListLinesAsync(e.Id, cancellationToken).ConfigureAwait(false);
        foreach (var line in existing.Where(l => l.Role == "PHYSICAL"))
            _repo.RemoveLine(line);

        var lineNo = existing.Where(l => l.Role == "SNAPSHOT").Select(l => l.LineNo).DefaultIfEmpty(0).Max();
        var fieldPackages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var dto in command.Lines ?? [])
        {
            Material? material = null;
            if (dto.MaterialId is { } mid && mid != Guid.Empty)
                material = await _materials.GetByIdAsync(mid, cancellationToken).ConfigureAwait(false);
            if (material is null && !string.IsNullOrWhiteSpace(dto.MaterialCode))
                material = await _materials.GetByCodeAsync(dto.MaterialCode.Trim(), cancellationToken).ConfigureAwait(false);
            if (material is null)
                continue;

            var locCode = string.IsNullOrWhiteSpace(dto.LocationCode) ? e.LocationCode : dto.LocationCode.Trim();
            if (string.IsNullOrWhiteSpace(locCode))
            {
                var snap = existing.FirstOrDefault(l =>
                    l.Role == "SNAPSHOT"
                    && string.Equals(l.MaterialCode, material.Code, StringComparison.OrdinalIgnoreCase));
                locCode = snap?.LocationCode ?? string.Empty;
            }
            if (string.IsNullOrWhiteSpace(locCode))
            {
                var (locs, _) = await _locations.SearchAsync(null, plantId, e.WarehouseCode, null, 1, 50, cancellationToken).ConfigureAwait(false);
                locCode = locs.FirstOrDefault(l =>
                    InventoryCountMath.IsActiveStatus(l.Status) && !InventoryCountMath.IsWipLocationType(l.LocationType))?.Code
                    ?? locs.FirstOrDefault()?.Code
                    ?? string.Empty;
            }
            if (string.IsNullOrWhiteSpace(locCode))
                return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-037", "Bu depoda stok lokasyonu yok — önce lokasyon tanımlayın veya mevcut stok satırından lokasyon kullanın."));

            var loc = await _locations.FindByWarehouseAndCodeAsync(e.WarehouseCode, locCode, plantId, cancellationToken).ConfigureAwait(false);
            if (loc is null)
                return Result.Failure<InventoryCountDto>(Error.Forbidden("INV-CNT-403", $"Lokasyon '{locCode}' bu tesis/depo altında değil."));
            if (!InventoryCountMath.IsActiveStatus(loc.Status))
                return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-017", $"Pasif lokasyon '{loc.Code}' için sayım satırı yazılamaz."));
            if (InventoryCountMath.IsWipLocationType(loc.LocationType))
                return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-038", "WIP / üretim noktası sayım lokasyonu değildir."));

            if (dto.PieceCount is < 0)
                return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-051", "Negatif fiziksel adet girilemez."));

            var policy = InventoryCountMath.ResolvePolicy(material.UnitOfMeasure, material.Category, material.DefinitionJson);
            var calc = InventoryCountMath.CalculateStockQty(policy, dto.ThicknessMm, dto.WidthMm, dto.LengthMm, dto.PieceCount, dto.MeasuredVolumeM3);
            if (!calc.Ok)
                return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-008", calc.Error ?? "Geçersiz ölçü."));

            var lotUnknown = dto.LotUnknown || string.IsNullOrWhiteSpace(dto.BatchNumber);
            var batchNumber = lotUnknown ? InventoryCountLots.Unknown : dto.BatchNumber.Trim();
            if (lotUnknown)
            {
                var snaps = existing.Where(l =>
                    l.Role == "SNAPSHOT"
                    && string.Equals(l.MaterialCode, material.Code, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(l.LocationCode, loc.Code, StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(l.BatchNumber)).ToArray();
                if (snaps.Length == 1)
                {
                    batchNumber = snaps[0].BatchNumber;
                    lotUnknown = false;
                }
            }
            lineNo++;
            var source = string.IsNullOrWhiteSpace(dto.Source) ? "MANUAL" : dto.Source.Trim().ToUpperInvariant();
            if (source is not "MANUAL" and not "EXCEL" and not "AI") source = "MANUAL";
            var group = (dto.PhysicalGroupLabel ?? string.Empty).Trim();
            var notes = dto.Notes ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(group))
            {
                var tag = $"İstif: {group}";
                notes = string.IsNullOrWhiteSpace(notes) ? tag : $"{tag} | {notes}";
            }
            var packageNumber = string.Empty;
            var barcode = string.Empty;
            if (!string.IsNullOrWhiteSpace(dto.PackageNumber) && dto.PackageNumber.Trim().StartsWith("PKG-", StringComparison.OrdinalIgnoreCase))
            {
                packageNumber = dto.PackageNumber.Trim();
                barcode = string.IsNullOrWhiteSpace(dto.Barcode) ? packageNumber : dto.Barcode.Trim();
            }
            else if (!string.IsNullOrWhiteSpace(group))
            {
                var key = $"{material.Code}\u001f{group}";
                if (!fieldPackages.TryGetValue(key, out var minted))
                {
                    minted = SystemIdentifier.Ensure(null, "PKG");
                    fieldPackages[key] = minted;
                }
                packageNumber = minted;
                barcode = minted;
            }
            if (!string.IsNullOrWhiteSpace(barcode))
            {
                var bc = $"Barkod: {barcode}";
                notes = string.IsNullOrWhiteSpace(notes) ? bc : $"{notes} | {bc}";
            }
            var line = InventoryCountLine.CreatePhysical(
                e.Id,
                lineNo,
                source,
                material.Id,
                material.Code,
                material.Name,
                loc.Code,
                batchNumber,
                lotUnknown,
                packageNumber,
                dto.ThicknessMm,
                dto.WidthMm,
                dto.LengthMm,
                dto.PieceCount,
                dto.MeasuredVolumeM3,
                calc.StockQty,
                policy.StockUnit,
                policy.CountUnit,
                dto.KeepSeparate,
                notes,
                plantId);
            if (dto.Approved) line.SetApproved(true);
            await _repo.AddLineAsync(line, cancellationToken).ConfigureAwait(false);
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var all = await _repo.ListLinesAsync(e.Id, cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e, InventoryCountSessionComposer.ToLineDtos(all)));
    }
}

public sealed class CompleteInventoryCountCommandHandler : ICommandHandler<CompleteInventoryCountCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IInventoryMovementRepository _movements;
    private readonly IBusinessUnitOfWork _uow;

    public CompleteInventoryCountCommandHandler(
        IInventoryCountRepository repo,
        IInventoryMovementRepository movements,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _movements = movements;
        _uow = uow;
    }

    public async Task<Result<InventoryCountDto>> HandleAsync(CompleteInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.CountId, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<InventoryCountDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<InventoryCountDto>(Error.Forbidden("INV-CNT-403", "Bu sayımı tamamlama yetkiniz yok."));
        try { e.Complete(command.Actor); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-048", ex.Message));
        }
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var lines = await _repo.ListLinesAsync(e.Id, cancellationToken).ConfigureAwait(false);
        var dtos = InventoryCountSessionComposer.ToLineDtos(lines);
        var mid = 0;
        if (e.SnapshotAt is not null && !string.IsNullOrWhiteSpace(e.PlantId))
            mid = await _movements.CountPostedAfterAsync(e.PlantId, e.WarehouseCode, string.IsNullOrWhiteSpace(e.LocationCode) ? null : e.LocationCode, e.SnapshotAt.Value, e.Number, cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e, dtos, mid > 0, mid, InventoryCountSessionComposer.Summarize(dtos, mid > 0)));
    }
}

public sealed class CancelInventoryCountCommandHandler : ICommandHandler<CancelInventoryCountCommand, Result<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public CancelInventoryCountCommandHandler(IInventoryCountRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<InventoryCountDto>> HandleAsync(CancelInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.CountId, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<InventoryCountDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<InventoryCountDto>(Error.Forbidden("INV-CNT-403", "Bu sayımı iptal etme yetkiniz yok."));
        try { e.Cancel(); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<InventoryCountDto>(Error.Validation("INV-CNT-048", ex.Message));
        }
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(InventoryCountMapper.ToDto(e));
    }
}

public sealed class PostInventoryCountCommandHandler : ICommandHandler<PostInventoryCountCommand, Result<InventoryCountPostResultDto>>
{
    private readonly IInventoryCountRepository _repo;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryMovementRepository _movements;
    private readonly ILocationRepository _locations;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public PostInventoryCountCommandHandler(
        IInventoryCountRepository repo,
        IInventoryBalanceRepository balances,
        IInventoryMovementRepository movements,
        ILocationRepository locations,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _balances = balances;
        _movements = movements;
        _locations = locations;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<InventoryCountPostResultDto>> HandleAsync(PostInventoryCountCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.CountId, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<InventoryCountPostResultDto>(Error.NotFound("BUS-001", "InventoryCount was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<InventoryCountPostResultDto>(Error.Forbidden("INV-CNT-403", "Bu sayımı onaylama yetkiniz yok."));
        if (e.Status == InventoryCountStatuses.Cancelled)
            return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-048", "İptal edilen sayım stok değiştirmez."));
        if (e.Status == InventoryCountStatuses.Posted)
        {
            var prior = await _movements.ListByDocumentAsync(e.Number, e.PlantId, cancellationToken).ConfigureAwait(false);
            return Result.Success(ToPostResult(e, prior));
        }

        var plantId = e.PlantId?.Trim() ?? string.Empty;
        var wh = await _warehouses.GetByCodeAndPlantAsync(e.WarehouseCode, plantId, cancellationToken).ConfigureAwait(false);
        if (wh is null || !InventoryCountMath.IsActiveStatus(wh.Status))
            return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-016", "Pasif veya yabancı depo için sayım post edilemez."));

        var raw = await _repo.ListLinesAsync(e.Id, cancellationToken).ConfigureAwait(false);
        var groups = InventoryCountSessionComposer.Group(raw);
        var adjustments = new List<InventoryCountAdjustmentDto>();

        try { e.Approve(command.Actor); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-048", ex.Message));
        }

        foreach (var g in groups.Values)
        {
            if (!g.HasPhysical && g.SystemQty == 0) continue;
            var counted = g.HasPhysical ? g.CountedQty : 0m;
            var diff = InventoryCountMath.Difference(counted, g.SystemQty);
            if (diff == 0) continue;

            var loc = await _locations.FindByWarehouseAndCodeAsync(e.WarehouseCode, g.LocationCode, plantId, cancellationToken).ConfigureAwait(false);
            if (loc is null)
                return Result.Failure<InventoryCountPostResultDto>(Error.Forbidden("INV-CNT-403", $"Lokasyon '{g.LocationCode}' bu tesise ait değil."));
            if (!InventoryCountMath.IsActiveStatus(loc.Status))
                return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-017", $"Pasif lokasyon '{loc.Code}' için sayım post edilemez."));

            var lot = InventoryCountMath.NormalizeLot(g.Lot);
            var balance = await _balances.FindByKeyAsync(g.MaterialCode, e.WarehouseCode, loc.Code, lot == InventoryCountLots.Unknown ? g.Snapshot?.BatchNumber ?? lot : lot, plantId, cancellationToken).ConfigureAwait(false);
            // Prefer exact snapshot batch if present
            if (g.Snapshot is not null)
            {
                balance = await _balances.FindByKeyAsync(g.MaterialCode, e.WarehouseCode, loc.Code, g.Snapshot.BatchNumber, plantId, cancellationToken).ConfigureAwait(false);
                lot = string.IsNullOrWhiteSpace(g.Snapshot.BatchNumber) ? InventoryCountLots.Unknown : g.Snapshot.BatchNumber;
            }

            var qty = Math.Abs(diff);
            var direction = diff > 0 ? "In" : "Out";
            try
            {
                if (balance is null)
                {
                    if (diff < 0)
                        return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-030", $"Sistem bakiyesi yokken eksi fark post edilemez ({g.MaterialCode})."));
                    balance = InventoryBalance.Create(g.MaterialCode, e.WarehouseCode, loc.Code, lot, 0, 0, "Active", plantId: plantId);
                    await _balances.AddAsync(balance, cancellationToken).ConfigureAwait(false);
                    balance.ApplyReceipt(qty);
                }
                else
                {
                    if (!string.Equals(balance.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                        return Result.Failure<InventoryCountPostResultDto>(Error.Forbidden("INV-CNT-403", "Bakiye başka fabrikaya ait."));
                    if (diff > 0) balance.ApplyReceipt(qty);
                    else balance.ApplyIssue(qty);
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-030", ex.Message));
            }

            var note = Truncate(
                $"count={e.Number}; old={g.SystemQty}; counted={counted}; diff={diff}; unit={g.Unit}; reason={command.Reason}; approvedBy={command.Actor}",
                2000);
            var movement = InventoryMovement.Post(
                "INVENTORY_COUNT_ADJUSTMENT",
                direction,
                e.Number,
                g.MaterialCode,
                string.Empty,
                string.Empty,
                e.WarehouseCode,
                loc.Code,
                lot,
                qty,
                g.Unit,
                note,
                plantId: plantId);
            await _movements.AddAsync(movement, cancellationToken).ConfigureAwait(false);
            adjustments.Add(new InventoryCountAdjustmentDto
            {
                MaterialCode = g.MaterialCode,
                LocationCode = loc.Code,
                LotNumber = lot,
                OldQuantity = g.SystemQty,
                CountedQuantity = counted,
                Difference = diff,
                Unit = g.Unit,
                Direction = direction,
                MovementNumber = movement.MovementNumber
            });
        }

        e.MarkPosted();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(new InventoryCountPostResultDto
        {
            CountId = e.Id,
            CountNumber = e.Number,
            Status = e.Status,
            AdjustmentCount = adjustments.Count,
            Adjustments = adjustments
        });
    }

    private static InventoryCountPostResultDto ToPostResult(InventoryCount e, IReadOnlyList<InventoryMovement> prior) =>
        new()
        {
            CountId = e.Id,
            CountNumber = e.Number,
            Status = e.Status,
            AdjustmentCount = prior.Count,
            Adjustments = prior.Select(m => new InventoryCountAdjustmentDto
            {
                MaterialCode = m.MaterialCode,
                LocationCode = m.LocationCode,
                LotNumber = m.LotNumber,
                OldQuantity = 0,
                CountedQuantity = m.Quantity,
                Difference = m.Direction.Equals("In", StringComparison.OrdinalIgnoreCase) ? m.Quantity : -m.Quantity,
                Unit = m.UnitOfMeasure,
                Direction = m.Direction,
                MovementNumber = m.MovementNumber
            }).ToArray()
        };

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= max ? value : value[..max];
    }
}

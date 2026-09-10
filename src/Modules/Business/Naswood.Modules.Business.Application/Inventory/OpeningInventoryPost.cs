using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

internal static class OpeningInventoryPost
{
    public static async Task<Result<InventoryCountPostResultDto>> ExecuteAsync(
        InventoryCount count,
        IReadOnlyList<InventoryCountLine> lines,
        string plantId,
        string actor,
        string reason,
        IInventoryBalanceRepository balances,
        IInventoryMovementRepository movements,
        ILocationRepository locations,
        IBatchRepository batches,
        IInventoryPackageRepository packages,
        IMaterialIdentityRepository identities,
        CancellationToken cancellationToken)
    {
        var physical = lines.Where(l => !l.IsDeleted && l.Role == "PHYSICAL" && l.CalculatedStockQty > 0).ToArray();
        if (physical.Length == 0)
            return Result.Failure<InventoryCountPostResultDto>(Error.Validation(
                "INV-CNT-060",
                "Açılış sayımında post edilecek fiziksel satır yok."));

        var now = DateTimeOffset.UtcNow;
        var day = OpeningInventoryCodes.LotDateStamp(now);
        var existingLots = await batches.ListOpeningLotNumbersAsync(OpeningInventoryCodes.LotPrefix + day + "-", plantId, cancellationToken).ConfigureAwait(false);
        var lotOrdinal = OpeningInventoryCodes.NextOrdinal(existingLots.Select(x => OpeningInventoryCodes.ParseLotOrdinal(x, day)));
        var existingPkgs = await packages.ListPackageNumbersAsync(cancellationToken).ConfigureAwait(false);
        var pkgOrdinal = OpeningInventoryCodes.NextOrdinal(existingPkgs.Select(OpeningInventoryCodes.ParsePackageOrdinal));

        var lotByMaterial = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var pkgByStack = new Dictionary<string, (string Pkg, string Barcode)>(StringComparer.OrdinalIgnoreCase);
        var adjustments = new List<InventoryCountAdjustmentDto>();

        foreach (var g in physical.GroupBy(l => l.MaterialCode.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            var materialCode = g.Key;
            if (!lotByMaterial.TryGetValue(materialCode, out var lot))
            {
                lot = OpeningInventoryCodes.OpeningLot(now, lotOrdinal++);
                lotByMaterial[materialCode] = lot;
                var batch = await batches.GetByNumberAndMaterialAsync(lot, materialCode, plantId, cancellationToken).ConfigureAwait(false);
                if (batch is null)
                {
                    batch = Batch.Create(lot, materialCode, 0, null, "Active", plantId: plantId, sourceType: OpeningInventoryCodes.SourceType);
                    await batches.AddAsync(batch, cancellationToken).ConfigureAwait(false);
                }
            }

            foreach (var stack in g.GroupBy(l => StackKey(l), StringComparer.OrdinalIgnoreCase))
            {
                var stackLines = stack.ToArray();
                var qty = stackLines.Sum(l => l.CalculatedStockQty);
                if (qty <= 0) continue;
                var locCode = stackLines[0].LocationCode;
                var loc = await locations.FindByWarehouseAndCodeAsync(count.WarehouseCode, locCode, plantId, cancellationToken).ConfigureAwait(false);
                if (loc is null)
                    return Result.Failure<InventoryCountPostResultDto>(Error.Forbidden("INV-CNT-403", $"Lokasyon '{locCode}' bu tesise ait değil."));
                if (!InventoryCountMath.IsActiveStatus(loc.Status))
                    return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-017", $"Pasif lokasyon '{loc.Code}' için açılış post edilemez."));

                if (!pkgByStack.TryGetValue($"{materialCode}\u001f{stack.Key}", out var pkg))
                {
                    var packageNo = OpeningInventoryCodes.PackageNo(pkgOrdinal++);
                    var barcode = OpeningInventoryCodes.Barcode(packageNo);
                    pkg = (packageNo, barcode);
                    pkgByStack[$"{materialCode}\u001f{stack.Key}"] = pkg;

                    var mi = SystemIdentifier.Ensure(null, "MI");
                    var identity = MaterialIdentity.CreateRoot(
                        mi, materialCode, lot, count.WarehouseCode, loc.Code, qty,
                        stackLines[0].StockUnit, count.Number, plantId: plantId);
                    await identities.AddAsync(identity, cancellationToken).ConfigureAwait(false);

                    var package = InventoryPackage.Create(
                        packageNo, mi, materialCode, lot, count.WarehouseCode, loc.Code,
                        qty, stackLines[0].StockUnit, barcode: barcode, status: "Available", plantId: plantId);
                    await packages.AddAsync(package, cancellationToken).ConfigureAwait(false);
                }

                foreach (var line in stackLines)
                    line.AssignOpeningIdentity(lot, pkg.Pkg, pkg.Barcode);

                var balance = await balances.FindByKeyAsync(materialCode, count.WarehouseCode, loc.Code, lot, plantId, cancellationToken).ConfigureAwait(false);
                if (balance is null)
                {
                    balance = InventoryBalance.Create(materialCode, count.WarehouseCode, loc.Code, lot, 0, 0, "Active", plantId: plantId);
                    await balances.AddAsync(balance, cancellationToken).ConfigureAwait(false);
                }
                try { balance.ApplyReceipt(qty); }
                catch (InvalidOperationException ex)
                {
                    return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-030", ex.Message));
                }

                var first = stackLines[0];
                var phys = first.ThicknessMm is > 0 && first.WidthMm is > 0 && first.LengthMm is > 0
                    ? $"phys={first.ThicknessMm:0.####}×{first.WidthMm:0.####}×{first.LengthMm:0.####}"
                    : null;
                var note = string.Join(" | ", new[]
                {
                    $"count={count.Number}",
                    $"source={OpeningInventoryCodes.SourceType}",
                    $"lot={lot}",
                    $"pkg={pkg.Pkg}",
                    $"barcode={pkg.Barcode}",
                    string.IsNullOrWhiteSpace(first.PhysicalGroupLabel) ? null : $"istif={first.PhysicalGroupLabel}",
                    phys,
                    $"qty={qty}",
                    $"unit={first.StockUnit}",
                    $"reason={reason}",
                    $"approvedBy={actor}"
                }.Where(s => !string.IsNullOrWhiteSpace(s)));

                var movement = InventoryMovement.Post(
                    OpeningInventoryCodes.MovementType,
                    "In",
                    count.Number,
                    materialCode,
                    string.Empty,
                    pkg.Pkg,
                    count.WarehouseCode,
                    loc.Code,
                    lot,
                    qty,
                    first.StockUnit,
                    note.Length > 2000 ? note[..2000] : note,
                    plantId: plantId);
                await movements.AddAsync(movement, cancellationToken).ConfigureAwait(false);
                adjustments.Add(new InventoryCountAdjustmentDto
                {
                    MaterialCode = materialCode,
                    LocationCode = loc.Code,
                    LotNumber = lot,
                    OldQuantity = 0,
                    CountedQuantity = qty,
                    Difference = qty,
                    Unit = first.StockUnit,
                    Direction = "In",
                    MovementNumber = movement.MovementNumber
                });
            }
        }

        return Result.Success(new InventoryCountPostResultDto
        {
            CountId = count.Id,
            CountNumber = count.Number,
            Status = InventoryCountStatuses.Posted,
            AdjustmentCount = adjustments.Count,
            Adjustments = adjustments
        });
    }

    private static string StackKey(InventoryCountLine line)
    {
        var group = (line.PhysicalGroupLabel ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(group)) return $"{line.LocationCode}|{group}";
        return $"LINE-{line.Id:N}";
    }
}

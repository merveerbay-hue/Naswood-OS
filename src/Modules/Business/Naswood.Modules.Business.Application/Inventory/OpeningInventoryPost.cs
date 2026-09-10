using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public static class OpeningInventoryPost
{
    public static async Task<Result<InventoryCountPostResultDto>> ExecuteAsync(
        InventoryCount count,
        IReadOnlyList<InventoryCountLine> lines,
        string plantId,
        string actor,
        string reason,
        Warehouse warehouse,
        IInventoryBalanceRepository balances,
        IInventoryMovementRepository movements,
        ILocationRepository locations,
        IBatchRepository batches,
        IInventoryPackageRepository packages,
        IMaterialIdentityRepository identities,
        IMaterialRepository materials,
        CancellationToken cancellationToken)
    {
        var physical = lines.Where(l => !l.IsDeleted && l.Role == "PHYSICAL" && l.CalculatedStockQty > 0).ToArray();
        if (physical.Length == 0)
            return Result.Failure<InventoryCountPostResultDto>(Error.Validation(
                "INV-CNT-060",
                "Açılış sayımında post edilecek fiziksel satır yok."));

        var resolved = new List<(InventoryCountLine Line, Material Material, Location Location)>(physical.Length);
        foreach (var line in physical)
        {
            Material? material = null;
            if (line.MaterialId is Guid mid)
                material = await materials.GetByIdAsync(mid, cancellationToken).ConfigureAwait(false);
            material ??= await materials.GetByCodeAsync(line.MaterialCode.Trim(), cancellationToken).ConfigureAwait(false);
            if (material is null || material.IsDeleted)
                return Result.Failure<InventoryCountPostResultDto>(Error.Validation(
                    "INV-CNT-061",
                    $"Malzeme kartı bulunamadı: {line.MaterialCode}"));
            if (line.MaterialId is Guid given && given != material.Id)
                return Result.Failure<InventoryCountPostResultDto>(Error.Validation(
                    "INV-CNT-062",
                    $"Satır malzeme kimliği ile kod uyuşmuyor: {line.MaterialCode}"));

            var loc = await locations.FindByWarehouseAndCodeAsync(count.WarehouseCode, line.LocationCode, plantId, cancellationToken).ConfigureAwait(false);
            if (loc is null)
                return Result.Failure<InventoryCountPostResultDto>(Error.Forbidden("INV-CNT-403", $"Lokasyon '{line.LocationCode}' bu tesise ait değil."));
            if (!InventoryCountMath.IsActiveStatus(loc.Status))
                return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-017", $"Pasif lokasyon '{loc.Code}' için açılış post edilemez."));

            resolved.Add((line, material, loc));
        }

        var now = DateTimeOffset.UtcNow;
        var lotPrefix = OpeningInventoryCodes.LotPrefixForDay(plantId, now);
        var existingLots = await batches.ListOpeningLotNumbersAsync(lotPrefix, plantId, cancellationToken).ConfigureAwait(false);
        var lotOrdinal = OpeningInventoryCodes.NextOrdinal(existingLots.Select(x => OpeningInventoryCodes.ParseLotOrdinal(x, plantId, now)));
        var existingPkgs = await packages.ListPackageNumbersAsync(cancellationToken).ConfigureAwait(false);
        var pkgOrdinal = OpeningInventoryCodes.NextOrdinal(
            existingPkgs.Select(x => OpeningInventoryCodes.ParsePackageOrdinal(x, plantId, now)));

        var lotByMaterial = new Dictionary<Guid, Batch>();
        var createdLots = new List<string>();
        var createdPackages = new List<OpeningPackageCreatedDto>();
        var adjustments = new List<InventoryCountAdjustmentDto>();

        foreach (var g in resolved.GroupBy(x => x.Material.Id))
        {
            var material = g.First().Material;
            if (!lotByMaterial.TryGetValue(material.Id, out var batch))
            {
                var lot = OpeningInventoryCodes.OpeningLot(plantId, now, lotOrdinal++);
                batch = await batches.GetByNumberAndMaterialAsync(lot, material.Code, plantId, cancellationToken).ConfigureAwait(false);
                if (batch is null)
                {
                    batch = Batch.Create(
                        lot, material.Code, 0, null, "Active",
                        plantId: plantId,
                        sourceType: OpeningInventoryCodes.SourceType,
                        sourceReferenceNo: count.Number);
                    await batches.AddAsync(batch, cancellationToken).ConfigureAwait(false);
                    createdLots.Add(lot);
                }
                lotByMaterial[material.Id] = batch;
            }

            foreach (var stack in g.GroupBy(x => PackageIdentityService.StackKey(
                         count.Id, material.Id, batch.Id, x.Location.Id, x.Line.PhysicalGroupLabel, x.Line.Id)))
            {
                var stackRows = stack.ToArray();
                var qty = stackRows.Sum(x => x.Line.CalculatedStockQty);
                if (qty <= 0) continue;
                var loc = stackRows[0].Location;
                var first = stackRows[0].Line;

                var mint = PackageIdentityService.Mint(plantId, now, pkgOrdinal++);
                var dup = await packages.ListByBarcodeExactAsync(mint.BarcodeValue, cancellationToken).ConfigureAwait(false);
                if (dup.Count > 0)
                    return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-PKG-061", $"Barkod çakışması: {mint.BarcodeValue}"));

                var mi = SystemIdentifier.Ensure(null, "MI");
                var identity = MaterialIdentity.CreateRoot(
                    mi, material.Code, batch.BatchNumber, count.WarehouseCode, loc.Code, qty,
                    first.StockUnit, count.Number, plantId: plantId);
                await identities.AddAsync(identity, cancellationToken).ConfigureAwait(false);

                var package = InventoryPackage.Create(
                    mint.PackageNumber, mi, material.Code, batch.BatchNumber, count.WarehouseCode, loc.Code,
                    qty, first.StockUnit, barcode: mint.BarcodeValue, status: "Available",
                    plantId: plantId,
                    publicId: mint.PublicId,
                    physicalGroupLabel: first.PhysicalGroupLabel ?? string.Empty,
                    sourcePlantId: plantId,
                    materialId: material.Id,
                    batchId: batch.Id,
                    warehouseId: warehouse.Id,
                    locationId: loc.Id);
                await packages.AddAsync(package, cancellationToken).ConfigureAwait(false);

                var contents = stackRows.Select((row, i) => InventoryPackageContent.Create(
                    package.Id, i + 1, row.Line.ThicknessMm, row.Line.WidthMm, row.Line.LengthMm,
                    row.Line.PieceCount, row.Line.CalculatedStockQty, row.Line.StockUnit, plantId)).ToArray();
                await packages.AddContentsAsync(contents, cancellationToken).ConfigureAwait(false);

                foreach (var row in stackRows)
                    row.Line.AssignOpeningIdentity(batch.BatchNumber, mint.PackageNumber, mint.BarcodeValue);

                var balance = await balances.FindByKeyAsync(material.Code, count.WarehouseCode, loc.Code, batch.BatchNumber, plantId, cancellationToken).ConfigureAwait(false);
                if (balance is null)
                {
                    balance = InventoryBalance.Create(material.Code, count.WarehouseCode, loc.Code, batch.BatchNumber, 0, 0, "Active", plantId: plantId);
                    await balances.AddAsync(balance, cancellationToken).ConfigureAwait(false);
                }
                try { balance.ApplyReceipt(qty); }
                catch (InvalidOperationException ex)
                {
                    return Result.Failure<InventoryCountPostResultDto>(Error.Validation("INV-CNT-030", ex.Message));
                }

                var note = string.Join(" | ", new[]
                {
                    $"count={count.Number}",
                    $"source={OpeningInventoryCodes.SourceType}",
                    $"lot={batch.BatchNumber}",
                    $"pkg={mint.PackageNumber}",
                    $"barcode={mint.BarcodeValue}",
                    string.IsNullOrWhiteSpace(first.PhysicalGroupLabel) ? null : $"istif={first.PhysicalGroupLabel}",
                    $"rows={stackRows.Length}",
                    $"qty={qty}",
                    $"unit={first.StockUnit}",
                    $"reason={reason}",
                    $"approvedBy={actor}"
                }.Where(s => !string.IsNullOrWhiteSpace(s)));

                var movement = InventoryMovement.Post(
                    OpeningInventoryCodes.MovementType,
                    "In",
                    count.Number,
                    material.Code,
                    mi,
                    mint.PackageNumber,
                    count.WarehouseCode,
                    loc.Code,
                    batch.BatchNumber,
                    qty,
                    first.StockUnit,
                    note.Length > 2000 ? note[..2000] : note,
                    plantId: plantId);
                await movements.AddAsync(movement, cancellationToken).ConfigureAwait(false);
                adjustments.Add(new InventoryCountAdjustmentDto
                {
                    MaterialCode = material.Code,
                    LocationCode = loc.Code,
                    LotNumber = batch.BatchNumber,
                    OldQuantity = 0,
                    CountedQuantity = qty,
                    Difference = qty,
                    Unit = first.StockUnit,
                    Direction = "In",
                    MovementNumber = movement.MovementNumber
                });
                createdPackages.Add(new OpeningPackageCreatedDto
                {
                    PackageId = package.Id,
                    PackageNo = mint.PackageNumber,
                    Barcode = mint.BarcodeValue,
                    PublicId = package.PublicId,
                    MaterialCode = material.Code,
                    LotNumber = batch.BatchNumber,
                    PhysicalGroupLabel = first.PhysicalGroupLabel ?? string.Empty,
                    Quantity = qty,
                    Unit = first.StockUnit
                });
            }
        }

        return Result.Success(new InventoryCountPostResultDto
        {
            CountId = count.Id,
            CountNumber = count.Number,
            Status = InventoryCountStatuses.Posted,
            AdjustmentCount = adjustments.Count,
            Adjustments = adjustments,
            LotCount = createdLots.Count,
            PackageCount = createdPackages.Count,
            Lots = createdLots,
            Packages = createdPackages
        });
    }
}

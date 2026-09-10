using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IProductionOutputRepository
{
    Task AddAsync(ProductionOutput entity, CancellationToken cancellationToken = default);
    Task<ProductionOutput?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductionOutput?> GetByNumberAsync(string number, string? plantId, CancellationToken cancellationToken = default);
}

public interface IProductionLotSourceRepository
{
    Task AddRangeAsync(IReadOnlyList<ProductionLotSource> rows, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionLotSource>> ListByProductionLotIdAsync(Guid productionLotId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionLotSource>> ListByOutputIdAsync(Guid productionOutputId, CancellationToken cancellationToken = default);
}

public sealed record PreviewProductionOutputQuery(
    PreviewProductionOutputRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<ProductionOutputPreviewDto>>;

public sealed record PostProductionOutputCommand(
    PostProductionOutputRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<ProductionOutputResultDto>>;

public sealed record GetProductionLotPassportQuery(
    Guid ProductionLotId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<ProductionLotPassportDto>>;

public sealed class PreviewProductionOutputQueryHandler : IQueryHandler<PreviewProductionOutputQuery, Result<ProductionOutputPreviewDto>>
{
    private readonly ProductionOutputGateway _gate;
    public PreviewProductionOutputQueryHandler(ProductionOutputGateway gate) => _gate = gate;

    public Task<Result<ProductionOutputPreviewDto>> HandleAsync(PreviewProductionOutputQuery query, CancellationToken cancellationToken = default)
        => _gate.PreviewAsync(query.Body, query.AllowedPlantIds, cancellationToken);
}

public sealed class PostProductionOutputCommandHandler : ICommandHandler<PostProductionOutputCommand, Result<ProductionOutputResultDto>>
{
    private readonly ProductionOutputGateway _gate;
    private readonly IBusinessUnitOfWork _uow;

    public PostProductionOutputCommandHandler(ProductionOutputGateway gate, IBusinessUnitOfWork uow)
    {
        _gate = gate;
        _uow = uow;
    }

    public async Task<Result<ProductionOutputResultDto>> HandleAsync(PostProductionOutputCommand command, CancellationToken cancellationToken = default)
    {
        var posted = await _gate.PostAsync(command.Body, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (posted.IsFailure) return posted;
        if (!posted.Value.IdempotentReplay)
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return posted;
    }
}

public sealed class GetProductionLotPassportQueryHandler : IQueryHandler<GetProductionLotPassportQuery, Result<ProductionLotPassportDto>>
{
    private readonly ProductionOutputGateway _gate;
    public GetProductionLotPassportQueryHandler(ProductionOutputGateway gate) => _gate = gate;

    public Task<Result<ProductionLotPassportDto>> HandleAsync(GetProductionLotPassportQuery query, CancellationToken cancellationToken = default)
        => _gate.LoadLotAsync(query.ProductionLotId, query.AllowedPlantIds, cancellationToken);
}

public sealed class ProductionOutputGateway
{
    private readonly IProductionOrderRepository _orders;
    private readonly IProductionOutputRepository _outputs;
    private readonly IProductionLotSourceRepository _sources;
    private readonly IMaterialRepository _materials;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;
    private readonly IBatchRepository _batches;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryPackageRepository _packages;
    private readonly IInventoryMovementRepository _movements;
    private readonly IMaterialIdentityRepository _identities;

    public ProductionOutputGateway(
        IProductionOrderRepository orders,
        IProductionOutputRepository outputs,
        IProductionLotSourceRepository sources,
        IMaterialRepository materials,
        IWarehouseRepository warehouses,
        ILocationRepository locations,
        IBatchRepository batches,
        IInventoryBalanceRepository balances,
        IInventoryPackageRepository packages,
        IInventoryMovementRepository movements,
        IMaterialIdentityRepository identities)
    {
        _orders = orders;
        _outputs = outputs;
        _sources = sources;
        _materials = materials;
        _warehouses = warehouses;
        _locations = locations;
        _batches = batches;
        _balances = balances;
        _packages = packages;
        _movements = movements;
        _identities = identities;
    }

    public async Task<Result<ProductionOutputPreviewDto>> PreviewAsync(
        PreviewProductionOutputRequestDto body,
        IReadOnlyList<string>? allowed,
        CancellationToken cancellationToken)
    {
        var ctx = await ResolveContextAsync(body, allowed, cancellationToken).ConfigureAwait(false);
        if (ctx.IsFailure) return Result.Failure<ProductionOutputPreviewDto>(ctx.Error!);
        var c = ctx.Value;
        var stacks = ProductionOutputComposer.GroupStacks(c.Order.Id, Guid.Empty, c.Material.Id, c.Location.Id, c.Lines);
        var preview = ProductionOutputComposer.ToPreview(
            c.Order.Code, c.Material, c.Warehouse.Code, c.Location.Code, body.WorkCenterCode ?? "",
            c.Lines, stacks, body.Sources?.Sum(s => s.ConsumedQuantity) ?? 0);
        preview.SourceLotCount = body.Sources?.Count ?? 0;
        return Result.Success(preview);
    }

    public async Task<Result<ProductionOutputResultDto>> PostAsync(
        PostProductionOutputRequestDto body,
        IReadOnlyList<string>? allowed,
        string actor,
        CancellationToken cancellationToken)
    {
        var plantId = string.IsNullOrWhiteSpace(body.PlantId) ? null : body.PlantId.Trim();
        if (!string.IsNullOrWhiteSpace(body.Number))
        {
            var existing = await _outputs.GetByNumberAsync(body.Number.Trim(), plantId, cancellationToken).ConfigureAwait(false);
            if (existing is not null && existing.Status == ProductionOutputStatuses.Posted)
                return await ReplayAsync(existing, cancellationToken).ConfigureAwait(false);
        }

        var ctx = await ResolveContextAsync(body, allowed, cancellationToken).ConfigureAwait(false);
        if (ctx.IsFailure) return Result.Failure<ProductionOutputResultDto>(ctx.Error!);
        var c = ctx.Value;

        var now = DateTimeOffset.UtcNow;
        var lotPrefix = ProductionLotCodes.LotPrefixForDay(c.PlantId, now);
        var existingLots = await _batches.ListOpeningLotNumbersAsync(lotPrefix, c.PlantId, cancellationToken).ConfigureAwait(false);
        var lotOrdinal = OpeningInventoryCodes.NextOrdinal(existingLots.Select(x => ProductionLotCodes.ParseLotOrdinal(x, c.PlantId, now)));
        var lotNo = ProductionLotCodes.ProductionLot(c.PlantId, now, lotOrdinal);
        var lot = Batch.Create(
            lotNo, c.Material.Code, 0, null, "Active",
            plantId: c.PlantId,
            sourceType: ProductionLotCodes.SourceType,
            sourceReferenceNo: c.Order.Code);
        await _batches.AddAsync(lot, cancellationToken).ConfigureAwait(false);

        var number = SystemIdentifier.Ensure(body.Number, "POUT");
        var doc = ProductionOutput.Create(
            number, c.Order.Id, c.Material.Id, c.Material.Code,
            c.Warehouse.Code, c.Location.Code, c.Warehouse.Id, c.Location.Id,
            body.WorkCenterCode ?? "", body.StockStatus, c.PlantId);
        await _outputs.AddAsync(doc, cancellationToken).ConfigureAwait(false);

        var inputQty = 0m;
        var sourceLots = new List<string>();
        var sourceRows = new List<ProductionLotSource>();
        foreach (var src in body.Sources ?? Array.Empty<ProductionOutputSourceRequestDto>())
        {
            if (src.ConsumedQuantity <= 0)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-010", "Kaynak tüketim miktarı pozitif olmalı."));
            var sourceLot = await _batches.GetByIdAsync(src.SourceLotId, cancellationToken).ConfigureAwait(false);
            if (sourceLot is null || sourceLot.IsDeleted)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-011", "Kaynak lot bulunamadı."));
            if (!string.IsNullOrWhiteSpace(sourceLot.PlantId)
                && !string.Equals(sourceLot.PlantId, c.PlantId, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<ProductionOutputResultDto>(Error.Forbidden("PRD-OUT-403", "Kaynak lot başka tesise ait."));

            var srcMat = await _materials.GetByCodeAsync(sourceLot.MaterialCode, cancellationToken).ConfigureAwait(false);
            if (srcMat is null)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-012", $"Kaynak malzeme yok: {sourceLot.MaterialCode}"));

            var srcWh = src.SourceWarehouseCode.Trim();
            var srcLoc = src.SourceLocationCode.Trim();
            var srcBalance = await _balances.FindByKeyAsync(sourceLot.MaterialCode, srcWh, srcLoc, sourceLot.BatchNumber, c.PlantId, cancellationToken).ConfigureAwait(false);
            if (srcBalance is null)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-013", $"Kaynak bakiye yok: {sourceLot.BatchNumber}"));
            try { srcBalance.ApplyIssue(src.ConsumedQuantity); }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-014", ex.Message));
            }

            if (src.SourcePackageId is Guid pkgId)
            {
                var srcPkg = await _packages.GetByIdAsync(pkgId, cancellationToken).ConfigureAwait(false);
                if (srcPkg is null || srcPkg.IsDeleted)
                    return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-015", "Kaynak paket bulunamadı."));
                try { srcPkg.Issue(src.ConsumedQuantity); }
                catch (InvalidOperationException ex)
                {
                    return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-015", ex.Message));
                }
            }

            var consume = InventoryMovement.Post(
                ProductionLotCodes.ConsumptionMovement, "Out", number,
                sourceLot.MaterialCode, "", src.SourcePackageId?.ToString() ?? "",
                srcWh, srcLoc, sourceLot.BatchNumber, src.ConsumedQuantity,
                string.IsNullOrWhiteSpace(src.Unit) ? srcBalance.Status : src.Unit,
                $"prd={c.Order.Code} outLot={lotNo}",
                plantId: c.PlantId);
            await _movements.AddAsync(consume, cancellationToken).ConfigureAwait(false);
            inputQty += src.ConsumedQuantity;
            sourceLots.Add(sourceLot.BatchNumber);
            sourceRows.Add(ProductionLotSource.Create(
                lot.Id, sourceLot.Id, srcMat.Id, sourceLot.MaterialCode,
                src.ConsumedQuantity, string.IsNullOrWhiteSpace(src.Unit) ? c.Lines[0].Unit : src.Unit,
                c.Order.Id, doc.Id, src.SourcePackageId, c.PlantId));
        }

        if (sourceRows.Count > 0)
            await _sources.AddRangeAsync(sourceRows, cancellationToken).ConfigureAwait(false);

        var stacks = ProductionOutputComposer.GroupStacks(c.Order.Id, lot.Id, c.Material.Id, c.Location.Id, c.Lines);
        var existingPkgs = await _packages.ListPackageNumbersAsync(cancellationToken).ConfigureAwait(false);
        var pkgOrdinal = OpeningInventoryCodes.NextOrdinal(
            existingPkgs.Select(x => OpeningInventoryCodes.ParsePackageOrdinal(x, c.PlantId, now)));
        var created = new List<ProductionOutputPackageCreatedDto>();
        var packageStatus = string.Equals(body.StockStatus, "Quarantine", StringComparison.OrdinalIgnoreCase)
            || string.Equals(body.StockStatus, "Hold", StringComparison.OrdinalIgnoreCase)
            ? "Quarantine"
            : "Available";
        var outputQty = c.Lines.Sum(l => l.Quantity);

        foreach (var stack in stacks)
        {
            var qty = stack.Sum(x => x.Quantity);
            var mint = PackageIdentityService.Mint(c.PlantId, now, pkgOrdinal++);
            var dup = await _packages.ListByBarcodeExactAsync(mint.BarcodeValue, cancellationToken).ConfigureAwait(false);
            if (dup.Count > 0)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("INV-PKG-061", $"Barkod çakışması: {mint.BarcodeValue}"));

            var mi = SystemIdentifier.Ensure(null, "MI");
            await _identities.AddAsync(MaterialIdentity.CreateRoot(
                mi, c.Material.Code, lotNo, c.Warehouse.Code, c.Location.Code, qty, stack.First().Unit, number, plantId: c.PlantId),
                cancellationToken).ConfigureAwait(false);

            var package = InventoryPackage.Create(
                mint.PackageNumber, mi, c.Material.Code, lotNo, c.Warehouse.Code, c.Location.Code,
                qty, stack.First().Unit, mint.BarcodeValue, packageStatus,
                plantId: c.PlantId,
                publicId: mint.PublicId,
                physicalGroupLabel: stack.First().PhysicalGroupLabel,
                sourcePlantId: c.PlantId,
                materialId: c.Material.Id,
                batchId: lot.Id,
                warehouseId: c.Warehouse.Id,
                locationId: c.Location.Id);
            ProductionLotCodes.EnsurePackageMatchesLot(package, lot, c.Material);
            await _packages.AddAsync(package, cancellationToken).ConfigureAwait(false);
            var contents = stack.Select((row, i) => InventoryPackageContent.Create(
                package.Id, i + 1, row.ThicknessMm, row.WidthMm, row.LengthMm, row.PieceCount, row.Quantity, row.Unit, c.PlantId)).ToArray();
            await _packages.AddContentsAsync(contents, cancellationToken).ConfigureAwait(false);

            var movement = InventoryMovement.Post(
                ProductionLotCodes.OutputMovement, "In", number,
                c.Material.Code, mi, mint.PackageNumber,
                c.Warehouse.Code, c.Location.Code, lotNo, qty, stack.First().Unit,
                $"prd={c.Order.Code} lot={lotNo} pkg={mint.PackageNumber} source={ProductionLotCodes.SourceType}",
                plantId: c.PlantId);
            await _movements.AddAsync(movement, cancellationToken).ConfigureAwait(false);
            created.Add(new ProductionOutputPackageCreatedDto
            {
                PackageId = package.Id,
                PackageNo = mint.PackageNumber,
                Barcode = mint.BarcodeValue,
                PublicId = package.PublicId,
                PhysicalGroupLabel = stack.First().PhysicalGroupLabel,
                Quantity = qty,
                Unit = stack.First().Unit
            });
        }

        var dest = await _balances.FindByKeyAsync(c.Material.Code, c.Warehouse.Code, c.Location.Code, lotNo, c.PlantId, cancellationToken).ConfigureAwait(false);
        if (dest is null)
        {
            dest = InventoryBalance.Create(
                c.Material.Code, c.Warehouse.Code, c.Location.Code, lotNo, 0, 0,
                packageStatus == "Quarantine" ? "Hold" : "Active",
                plantId: c.PlantId);
            await _balances.AddAsync(dest, cancellationToken).ConfigureAwait(false);
        }
        dest.ApplyReceipt(outputQty);
        lot.ApplyReceipt(outputQty, packageStatus == "Quarantine" ? "Hold" : "Active");
        doc.MarkPosted(lot.Id, lotNo, inputQty, outputQty, 0, c.Lines[0].Unit, actor);

        return Result.Success(new ProductionOutputResultDto
        {
            OutputId = doc.Id,
            Number = doc.Number,
            Status = doc.Status,
            ProductionLotId = lot.Id,
            ProductionLotNumber = lotNo,
            SourceType = ProductionLotCodes.SourceType,
            OutputMaterialCode = c.Material.Code,
            OutputQuantity = outputQty,
            InputQuantity = inputQty,
            Unit = c.Lines[0].Unit,
            PackageCount = created.Count,
            SourceLotCount = sourceLots.Count,
            IdempotentReplay = false,
            Packages = created,
            SourceLotNumbers = sourceLots
        });
    }

    public async Task<Result<ProductionLotPassportDto>> LoadLotAsync(Guid lotId, IReadOnlyList<string>? allowed, CancellationToken cancellationToken)
    {
        var lot = await _batches.GetByIdAsync(lotId, cancellationToken).ConfigureAwait(false);
        if (lot is null || lot.IsDeleted)
            return Result.Failure<ProductionLotPassportDto>(Error.NotFound("PRD-OUT-404", "Üretim lotu bulunamadı."));
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, lot.PlantId))
            return Result.Failure<ProductionLotPassportDto>(Error.Forbidden("PRD-OUT-403", "Bu üretim lotunu görüntüleme yetkiniz yok."));

        var material = await _materials.GetByCodeAsync(lot.MaterialCode, cancellationToken).ConfigureAwait(false);
        var pkgs = await _packages.ListByBatchIdAsync(lot.Id, cancellationToken).ConfigureAwait(false);
        var sources = await _sources.ListByProductionLotIdAsync(lot.Id, cancellationToken).ConfigureAwait(false);
        var sourceDtos = new List<ProductionLotSourceDto>();
        foreach (var s in sources)
        {
            var srcLot = await _batches.GetByIdAsync(s.SourceLotId, cancellationToken).ConfigureAwait(false);
            sourceDtos.Add(new ProductionLotSourceDto
            {
                SourceLotId = s.SourceLotId,
                SourceLotNumber = srcLot?.BatchNumber ?? "",
                SourceMaterialCode = s.SourceMaterialCode,
                ConsumedQuantity = s.ConsumedQuantity,
                Unit = s.Unit,
                SourcePackageId = s.SourcePackageId
            });
        }

        ProductionOrder? order = null;
        var firstSource = sources.FirstOrDefault();
        if (firstSource is not null)
            order = await _orders.GetByIdAsync(firstSource.ProductionOrderId, cancellationToken).ConfigureAwait(false);

        return Result.Success(new ProductionLotPassportDto
        {
            ProductionLotId = lot.Id,
            LotNumber = lot.BatchNumber,
            SourceType = lot.SourceType,
            OutputMaterialCode = lot.MaterialCode,
            OutputMaterialName = material?.Name ?? lot.MaterialCode,
            ProductionOrderId = order?.Id,
            ProductionOrderNumber = order?.Code ?? lot.SourceReferenceNo,
            PlantId = lot.PlantId ?? "",
            WorkCenterCode = "",
            OutputQuantity = lot.Quantity,
            Unit = material?.UnitOfMeasure ?? "",
            Status = lot.Status,
            CreatedAt = lot.CreatedAt,
            CreatedBy = "",
            Packages = pkgs.Select(p => new ProductionOutputPackageCreatedDto
            {
                PackageId = p.Id,
                PackageNo = p.PackageNumber,
                Barcode = p.Barcode,
                PublicId = p.PublicId,
                PhysicalGroupLabel = p.PhysicalGroupLabel,
                Quantity = p.Quantity,
                Unit = p.UnitOfMeasure
            }).ToArray(),
            SourceLots = sourceDtos
        });
    }

    private async Task<Result<Resolved>> ResolveContextAsync(
        PreviewProductionOutputRequestDto body,
        IReadOnlyList<string>? allowed,
        CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(body.ProductionOrderId, cancellationToken).ConfigureAwait(false);
        if (order is null || order.IsDeleted)
            return Result.Failure<Resolved>(Error.NotFound("PRD-OUT-003", "Üretim emri bulunamadı."));
        var plantId = string.IsNullOrWhiteSpace(body.PlantId) ? order.PlantId ?? "" : body.PlantId.Trim();
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, plantId))
            return Result.Failure<Resolved>(Error.Forbidden("PRD-OUT-403", "Bu tesiste üretim çıkışı yetkiniz yok."));
        if (!string.IsNullOrWhiteSpace(order.PlantId)
            && !string.Equals(order.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<Resolved>(Error.Forbidden("PRD-OUT-403", "Üretim emri başka tesise ait."));

        var material = await _materials.GetByIdAsync(body.OutputMaterialId, cancellationToken).ConfigureAwait(false);
        if (material is null || material.IsDeleted)
            return Result.Failure<Resolved>(Error.Validation("PRD-OUT-004", "Çıktı malzemesi Material Master'da olmalı."));
        if (!string.Equals(material.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<Resolved>(Error.Validation("PRD-OUT-004", "Çıktı malzemesi aktif değil."));

        var warehouse = await _warehouses.GetByCodeAndPlantAsync(body.WarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (warehouse is null || warehouse.IsDeleted)
            return Result.Failure<Resolved>(Error.Validation("PRD-OUT-005", "Hedef depo bu tesiste tanımlı değil."));
        if (!string.Equals(warehouse.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<Resolved>(Error.Validation("PRD-OUT-005", "Hedef depo pasif."));
        if (!string.IsNullOrWhiteSpace(warehouse.PlantId)
            && !string.Equals(warehouse.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<Resolved>(Error.Forbidden("PRD-OUT-403", "Hedef depo başka tesise ait."));

        var location = await _locations.FindByWarehouseAndCodeAsync(warehouse.Code, body.LocationCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (location is null || location.IsDeleted)
            return Result.Failure<Resolved>(Error.Validation("PRD-OUT-006", "Hedef lokasyon bu depoda tanımlı değil."));
        if (!InventoryCountMath.IsActiveStatus(location.Status))
            return Result.Failure<Resolved>(Error.Validation("PRD-OUT-006", "Hedef lokasyon pasif."));
        if (!string.IsNullOrWhiteSpace(location.PlantId)
            && !string.Equals(location.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<Resolved>(Error.Forbidden("PRD-OUT-403", "Hedef lokasyon başka tesise ait."));

        var resolvedLines = ProductionOutputComposer.ResolveLines(material, body.Lines);
        if (resolvedLines.IsFailure) return Result.Failure<Resolved>(resolvedLines.Error!);

        return Result.Success(new Resolved(order, material, warehouse, location, plantId, resolvedLines.Value.Lines));
    }

    private async Task<Result<ProductionOutputResultDto>> ReplayAsync(ProductionOutput existing, CancellationToken cancellationToken)
    {
        var pkgs = existing.OutputBatchId is Guid bid
            ? await _packages.ListByBatchIdAsync(bid, cancellationToken).ConfigureAwait(false)
            : Array.Empty<InventoryPackage>();
        var sources = await _sources.ListByOutputIdAsync(existing.Id, cancellationToken).ConfigureAwait(false);
        var sourceNos = new List<string>();
        foreach (var s in sources)
        {
            var lot = await _batches.GetByIdAsync(s.SourceLotId, cancellationToken).ConfigureAwait(false);
            if (lot is not null) sourceNos.Add(lot.BatchNumber);
        }
        return Result.Success(new ProductionOutputResultDto
        {
            OutputId = existing.Id,
            Number = existing.Number,
            Status = existing.Status,
            ProductionLotId = existing.OutputBatchId ?? Guid.Empty,
            ProductionLotNumber = existing.OutputLotNumber,
            SourceType = ProductionLotCodes.SourceType,
            OutputMaterialCode = existing.OutputMaterialCode,
            OutputQuantity = existing.OutputQuantity,
            InputQuantity = existing.InputQuantity,
            Unit = existing.UnitOfMeasure,
            PackageCount = pkgs.Count,
            SourceLotCount = sourceNos.Count,
            IdempotentReplay = true,
            Packages = pkgs.Select(p => new ProductionOutputPackageCreatedDto
            {
                PackageId = p.Id,
                PackageNo = p.PackageNumber,
                Barcode = p.Barcode,
                PublicId = p.PublicId,
                PhysicalGroupLabel = p.PhysicalGroupLabel,
                Quantity = p.Quantity,
                Unit = p.UnitOfMeasure
            }).ToArray(),
            SourceLotNumbers = sourceNos
        });
    }

    private sealed record Resolved(
        ProductionOrder Order,
        Material Material,
        Warehouse Warehouse,
        Location Location,
        string PlantId,
        IReadOnlyList<ProductionOutputResolvedLine> Lines);
}

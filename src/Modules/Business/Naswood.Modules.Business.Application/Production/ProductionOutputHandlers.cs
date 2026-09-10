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

public sealed record ReverseProductionOutputCommand(
    Guid OutputId,
    string Reason,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<ProductionOutputResultDto>>;

public sealed record ScanProductionConsumptionQuery(
    string Barcode,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<ProductionConsumptionScanDto>>;

public sealed record DecideProductionOutputQcCommand(
    Guid OutputId,
    string Decision,
    string InspectionReference,
    string Notes,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<ProductionOutputResultDto>>;

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

public sealed class ReverseProductionOutputCommandHandler : ICommandHandler<ReverseProductionOutputCommand, Result<ProductionOutputResultDto>>
{
    private readonly ProductionOutputGateway _gate;
    private readonly IBusinessUnitOfWork _uow;

    public ReverseProductionOutputCommandHandler(ProductionOutputGateway gate, IBusinessUnitOfWork uow)
    {
        _gate = gate;
        _uow = uow;
    }

    public async Task<Result<ProductionOutputResultDto>> HandleAsync(ReverseProductionOutputCommand command, CancellationToken cancellationToken = default)
    {
        var reversed = await _gate.ReverseAsync(command.OutputId, command.Reason, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (reversed.IsFailure) return reversed;
        if (!reversed.Value.IdempotentReplay)
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return reversed;
    }
}

public sealed class ScanProductionConsumptionQueryHandler : IQueryHandler<ScanProductionConsumptionQuery, Result<ProductionConsumptionScanDto>>
{
    private readonly ProductionOutputGateway _gate;
    public ScanProductionConsumptionQueryHandler(ProductionOutputGateway gate) => _gate = gate;

    public Task<Result<ProductionConsumptionScanDto>> HandleAsync(ScanProductionConsumptionQuery query, CancellationToken cancellationToken = default)
        => _gate.ScanConsumptionAsync(query.Barcode, query.AllowedPlantIds, cancellationToken);
}

public sealed class DecideProductionOutputQcCommandHandler : ICommandHandler<DecideProductionOutputQcCommand, Result<ProductionOutputResultDto>>
{
    private readonly ProductionOutputGateway _gate;
    private readonly IBusinessUnitOfWork _uow;

    public DecideProductionOutputQcCommandHandler(ProductionOutputGateway gate, IBusinessUnitOfWork uow)
    {
        _gate = gate;
        _uow = uow;
    }

    public async Task<Result<ProductionOutputResultDto>> HandleAsync(DecideProductionOutputQcCommand command, CancellationToken cancellationToken = default)
    {
        var decided = await _gate.DecideQcAsync(
            command.OutputId, command.Decision, command.InspectionReference, command.Notes,
            command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (decided.IsFailure) return decided;
        if (!decided.Value.IdempotentReplay)
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return decided;
    }
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
        var qc = ProductionOutputQcPolicy.Resolve(c.Material, body.WorkCenterCode, body.StockStatus);
        preview.ResolvedStockStatus = ProductionOutputQcPolicy.Apply(qc, body.StockStatus, body.AllowQcOverride);
        preview.QcHoldRequired = qc.HoldRequired;
        preview.QcPolicySource = qc.PolicySource;
        preview.QcOverrideRequired = qc.OverrideRequired && !body.AllowQcOverride;
        var sourcePreview = new List<ProductionLotSourceDto>();
        foreach (var src in body.Sources ?? Array.Empty<ProductionOutputSourceRequestDto>())
        {
            var lot = await _batches.GetByIdAsync(src.SourceLotId, cancellationToken).ConfigureAwait(false);
            sourcePreview.Add(new ProductionLotSourceDto
            {
                SourceLotId = src.SourceLotId,
                SourceLotNumber = lot?.BatchNumber ?? "",
                SourceMaterialCode = lot?.MaterialCode ?? "",
                ConsumedQuantity = src.ConsumedQuantity,
                Unit = src.Unit,
                SourcePackageId = src.SourcePackageId
            });
        }
        preview.Sources = sourcePreview;
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
        var qc = ProductionOutputQcPolicy.Resolve(c.Material, body.WorkCenterCode, body.StockStatus);
        var stockStatus = ProductionOutputQcPolicy.Apply(qc, body.StockStatus, body.AllowQcOverride);

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
            body.WorkCenterCode ?? "", stockStatus, c.PlantId);
        await _outputs.AddAsync(doc, cancellationToken).ConfigureAwait(false);

        var prepared = await PrepareSourcesAsync(body.Sources, c, cancellationToken).ConfigureAwait(false);
        if (prepared.IsFailure) return Result.Failure<ProductionOutputResultDto>(prepared.Error!);

        var inputQty = 0m;
        var sourceLots = new List<string>();
        var sourceRows = new List<ProductionLotSource>();
        foreach (var row in prepared.Value)
        {
            try { row.Balance.ApplyIssue(row.Request.ConsumedQuantity); }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-014", ex.Message));
            }

            var pkgNo = "";
            if (row.Package is not null)
            {
                try { row.Package.Consume(row.Request.ConsumedQuantity); }
                catch (InvalidOperationException ex)
                {
                    return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-015", ex.Message));
                }
                pkgNo = row.Package.PackageNumber;
            }

            var consume = InventoryMovement.Post(
                ProductionLotCodes.ConsumptionMovement, "Out", number,
                row.Lot.MaterialCode, "", pkgNo,
                row.WarehouseCode, row.LocationCode, row.Lot.BatchNumber, row.Request.ConsumedQuantity,
                row.Unit,
                ProductionLotCodes.ConsumptionNotes(c.Order.Code, lotNo, row.Lot.BatchNumber, row.Lot.Id, pkgNo),
                plantId: c.PlantId);
            await _movements.AddAsync(consume, cancellationToken).ConfigureAwait(false);
            inputQty += row.Request.ConsumedQuantity;
            sourceLots.Add(row.Lot.BatchNumber);
            sourceRows.Add(ProductionLotSource.Create(
                lot.Id, row.Lot.Id, row.Material.Id, row.Lot.MaterialCode,
                row.Request.ConsumedQuantity, row.Unit,
                c.Order.Id, doc.Id, row.Request.SourcePackageId, c.PlantId));
        }

        if (sourceRows.Count > 0)
            await _sources.AddRangeAsync(sourceRows, cancellationToken).ConfigureAwait(false);

        var stacks = ProductionOutputComposer.GroupStacks(c.Order.Id, lot.Id, c.Material.Id, c.Location.Id, c.Lines);
        var existingPkgs = await _packages.ListPackageNumbersAsync(cancellationToken).ConfigureAwait(false);
        var pkgOrdinal = OpeningInventoryCodes.NextOrdinal(
            existingPkgs.Select(x => OpeningInventoryCodes.ParsePackageOrdinal(x, c.PlantId, now)));
        var created = new List<ProductionOutputPackageCreatedDto>();
        var packageStatus = ProductionOutputQcPolicy.IsHold(stockStatus) ? "Quarantine" : "Available";
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
                ProductionLotCodes.OutputNotes(c.Order.Code, lotNo, mint.PackageNumber),
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
        dest.ApplyReceipt(outputQty, packageStatus == "Quarantine" ? "Hold" : "Active");
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
            SourceLotNumbers = sourceLots,
            StockStatus = stockStatus
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

    public async Task<Result<ProductionOutputResultDto>> ReverseAsync(
        Guid outputId,
        string reason,
        IReadOnlyList<string>? allowed,
        string actor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-020", "İptal nedeni zorunlu."));
        var doc = await _outputs.GetByIdAsync(outputId, cancellationToken).ConfigureAwait(false);
        if (doc is null || doc.IsDeleted)
            return Result.Failure<ProductionOutputResultDto>(Error.NotFound("PRD-OUT-404", "Üretim çıkışı bulunamadı."));
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, doc.PlantId))
            return Result.Failure<ProductionOutputResultDto>(Error.Forbidden("PRD-OUT-403", "Bu tesiste üretim çıkışı yetkiniz yok."));
        if (doc.Status == ProductionOutputStatuses.Cancelled)
        {
            var replay = await ReplayAsync(doc, cancellationToken).ConfigureAwait(false);
            if (replay.IsFailure) return replay;
            return Result.Success(replay.Value with { Reversed = true, CancelReason = doc.CancelReason, IdempotentReplay = true });
        }
        if (doc.Status != ProductionOutputStatuses.Posted)
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-021", "Sadece işlenmiş üretim çıkışı tersine çevrilebilir."));

        var prior = await _movements.ListByDocumentAsync(doc.Number, doc.PlantId, cancellationToken).ConfigureAwait(false);
        if (prior.Any(m => m.MovementType == ProductionLotCodes.OutputReversalMovement))
        {
            var replay = await ReplayAsync(doc, cancellationToken).ConfigureAwait(false);
            if (replay.IsFailure) return replay;
            return Result.Success(replay.Value with { Reversed = true, IdempotentReplay = true });
        }

        if (doc.OutputBatchId is not Guid lotId)
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-022", "Üretim lotu yok."));
        var lot = await _batches.GetByIdAsync(lotId, cancellationToken).ConfigureAwait(false);
        if (lot is null)
            return Result.Failure<ProductionOutputResultDto>(Error.NotFound("PRD-OUT-404", "Üretim lotu bulunamadı."));

        var dest = await _balances.FindByKeyAsync(
            doc.OutputMaterialCode, doc.WarehouseCode, doc.LocationCode, doc.OutputLotNumber, doc.PlantId, cancellationToken).ConfigureAwait(false);
        if (dest is null)
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-023", "Çıktı bakiyesi bulunamadı."));
        try { dest.ApplyIssue(doc.OutputQuantity); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-023", ex.Message));
        }
        try { lot.ApplyIssue(doc.OutputQuantity); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-023", ex.Message));
        }

        var listed = await _packages.ListByBatchIdAsync(lot.Id, cancellationToken).ConfigureAwait(false);
        foreach (var listedPkg in listed)
        {
            var pkg = await _packages.GetByIdAsync(listedPkg.Id, cancellationToken).ConfigureAwait(false);
            if (pkg is null || pkg.IsDeleted) continue;
            var snapshotQty = pkg.Quantity;
            var snapshotNo = pkg.PackageNumber;
            var snapshotBarcode = pkg.Barcode;
            pkg.MarkCancelled();
            if (!string.Equals(pkg.PackageNumber, snapshotNo, StringComparison.Ordinal)
                || !string.Equals(pkg.Barcode, snapshotBarcode, StringComparison.Ordinal))
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-024", "Output paket kimliği reversal'da değişemez."));

            await _movements.AddAsync(InventoryMovement.Post(
                ProductionLotCodes.OutputReversalMovement, "Out", doc.Number,
                doc.OutputMaterialCode, pkg.MaterialIdentityNumber, pkg.PackageNumber,
                doc.WarehouseCode, doc.LocationCode, doc.OutputLotNumber, snapshotQty == 0 ? doc.OutputQuantity : snapshotQty,
                pkg.UnitOfMeasure,
                ProductionLotCodes.OutputNotes(lot.SourceReferenceNo, doc.OutputLotNumber, pkg.PackageNumber) + " reverse=1",
                plantId: doc.PlantId), cancellationToken).ConfigureAwait(false);
        }

        var sources = await _sources.ListByOutputIdAsync(doc.Id, cancellationToken).ConfigureAwait(false);
        foreach (var src in sources)
        {
            var sourceLot = await _batches.GetByIdAsync(src.SourceLotId, cancellationToken).ConfigureAwait(false);
            if (sourceLot is null) continue;
            var consumeMove = prior.FirstOrDefault(m =>
                m.MovementType == ProductionLotCodes.ConsumptionMovement
                && string.Equals(m.LotNumber, sourceLot.BatchNumber, StringComparison.OrdinalIgnoreCase)
                && m.Quantity == src.ConsumedQuantity);
            var wh = consumeMove?.WarehouseCode ?? "";
            var loc = consumeMove?.LocationCode ?? "";
            if (wh.Length == 0 || loc.Length == 0)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-025", $"Kaynak hareketi bulunamadı: {sourceLot.BatchNumber}"));
            var srcBalance = await _balances.FindByKeyAsync(sourceLot.MaterialCode, wh, loc, sourceLot.BatchNumber, doc.PlantId, cancellationToken).ConfigureAwait(false);
            if (srcBalance is null)
            {
                srcBalance = InventoryBalance.Create(sourceLot.MaterialCode, wh, loc, sourceLot.BatchNumber, 0, 0, "Active", plantId: doc.PlantId);
                await _balances.AddAsync(srcBalance, cancellationToken).ConfigureAwait(false);
            }
            srcBalance.ApplyReceipt(src.ConsumedQuantity);

            var pkgNo = consumeMove?.PackageNumber ?? "";
            if (src.SourcePackageId is Guid pkgId)
            {
                var srcPkg = await _packages.GetByIdAsync(pkgId, cancellationToken).ConfigureAwait(false);
                if (srcPkg is not null)
                {
                    var sameNo = srcPkg.PackageNumber;
                    var sameBarcode = srcPkg.Barcode;
                    srcPkg.Restore(src.ConsumedQuantity);
                    if (!string.Equals(srcPkg.PackageNumber, sameNo, StringComparison.Ordinal)
                        || !string.Equals(srcPkg.Barcode, sameBarcode, StringComparison.Ordinal))
                        return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-026", "Kaynak paket kimliği reversal'da değişemez."));
                    pkgNo = srcPkg.PackageNumber;
                }
            }

            await _movements.AddAsync(InventoryMovement.Post(
                ProductionLotCodes.ConsumptionReversalMovement, "In", doc.Number,
                sourceLot.MaterialCode, "", pkgNo, wh, loc, sourceLot.BatchNumber, src.ConsumedQuantity, src.Unit,
                ProductionLotCodes.ConsumptionNotes(lot.SourceReferenceNo, doc.OutputLotNumber, sourceLot.BatchNumber, sourceLot.Id, pkgNo) + " reverse=1",
                plantId: doc.PlantId), cancellationToken).ConfigureAwait(false);
        }

        doc.MarkCancelled(actor, reason);
        var result = await ReplayAsync(doc, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) return result;
        return Result.Success(result.Value with { Reversed = true, CancelReason = reason, IdempotentReplay = false, Status = doc.Status });
    }

    public async Task<Result<ProductionConsumptionScanDto>> ScanConsumptionAsync(
        string barcode,
        IReadOnlyList<string>? allowed,
        CancellationToken cancellationToken)
    {
        var hits = await _packages.ListByBarcodeExactAsync(barcode ?? "", cancellationToken).ConfigureAwait(false);
        if (hits.Count == 0)
            return Result.Failure<ProductionConsumptionScanDto>(Error.NotFound("PRD-OUT-030", "Barkod ile paket bulunamadı."));
        if (hits.Count > 1)
            return Result.Failure<ProductionConsumptionScanDto>(Error.Validation("PRD-OUT-031", "Barkod birden fazla pakete ait."));
        var pkg = hits[0];
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, pkg.PlantId))
            return Result.Failure<ProductionConsumptionScanDto>(Error.Forbidden("PRD-OUT-403", "Bu paketi kullanma yetkiniz yok."));
        if (!InventoryPackageStatuses.IsConsumable(pkg.Status))
            return Result.Failure<ProductionConsumptionScanDto>(Error.Validation("PRD-OUT-032", $"Paket tüketilebilir değil ({pkg.Status})."));

        Batch? lot = pkg.BatchId is Guid bid
            ? await _batches.GetByIdAsync(bid, cancellationToken).ConfigureAwait(false)
            : null;
        lot ??= await _batches.GetByNumberAndMaterialAsync(pkg.LotNumber, pkg.MaterialCode, pkg.PlantId, cancellationToken).ConfigureAwait(false);
        if (lot is null)
            return Result.Failure<ProductionConsumptionScanDto>(Error.Validation("PRD-OUT-033", "Paket lotu bulunamadı."));

        var balance = await _balances.FindByKeyAsync(
            pkg.MaterialCode, pkg.WarehouseCode, pkg.LocationCode, lot.BatchNumber, pkg.PlantId, cancellationToken).ConfigureAwait(false);
        var available = Math.Min(pkg.Quantity, balance is null ? 0 : balance.QuantityOnHand - balance.QuantityReserved);
        if (available <= 0)
            return Result.Failure<ProductionConsumptionScanDto>(Error.Validation("PRD-OUT-034", "Tüketilecek stok yok."));

        return Result.Success(new ProductionConsumptionScanDto
        {
            PackageId = pkg.Id,
            PackageNo = pkg.PackageNumber,
            Barcode = pkg.Barcode,
            SourceLotId = lot.Id,
            SourceLotNumber = lot.BatchNumber,
            MaterialCode = pkg.MaterialCode,
            WarehouseCode = pkg.WarehouseCode,
            LocationCode = pkg.LocationCode,
            AvailableQuantity = available,
            Unit = pkg.UnitOfMeasure,
            PlantId = pkg.PlantId ?? "",
            Status = pkg.Status
        });
    }

    public async Task<Result<ProductionOutputResultDto>> DecideQcAsync(
        Guid outputId,
        string decision,
        string inspectionReference,
        string notes,
        IReadOnlyList<string>? allowed,
        string actor,
        CancellationToken cancellationToken)
    {
        var doc = await _outputs.GetByIdAsync(outputId, cancellationToken).ConfigureAwait(false);
        if (doc is null || doc.IsDeleted)
            return Result.Failure<ProductionOutputResultDto>(Error.NotFound("PRD-OUT-404", "Üretim çıkışı bulunamadı."));
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, doc.PlantId))
            return Result.Failure<ProductionOutputResultDto>(Error.Forbidden("PRD-OUT-403", "Bu tesiste QC yetkiniz yok."));
        if (doc.Status != ProductionOutputStatuses.Posted)
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-040", "QC kararı yalnızca işlenmiş çıkış için verilir."));

        var next = ProductionQcDecisions.Normalize(decision);
        var priorMoves = await _movements.ListByDocumentAsync(doc.Number, doc.PlantId, cancellationToken).ConfigureAwait(false);
        var already = !string.IsNullOrWhiteSpace(doc.QcDecision)
            && string.Equals(doc.QcDecision, next, StringComparison.OrdinalIgnoreCase);
        if (already || priorMoves.Any(m =>
                m.MovementType is ProductionLotCodes.QcReleaseMovement or ProductionLotCodes.QcRejectMovement
                && string.Equals(doc.QcDecision, next, StringComparison.OrdinalIgnoreCase)))
        {
            var replay = await ReplayAsync(doc, cancellationToken).ConfigureAwait(false);
            if (replay.IsFailure) return replay;
            return Result.Success(replay.Value with { IdempotentReplay = true });
        }

        if (!ProductionOutputQcPolicy.IsHold(doc.StockStatus)
            && !string.Equals(doc.StockStatus, "Quarantine", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-041", "QC release/reject yalnızca QUARANTINE çıktısı için."));

        try { doc.RecordQc(decision, actor, inspectionReference, notes); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-042", ex.Message));
        }

        var released = string.Equals(doc.QcDecision, ProductionQcDecisions.Released, StringComparison.OrdinalIgnoreCase);
        var pkgStatus = released ? InventoryPackageStatuses.Available : InventoryPackageStatuses.Rejected;
        var balanceStatus = released ? "Active" : "Blocked";
        var qtyBefore = doc.OutputQuantity;

        if (doc.OutputBatchId is Guid lotId)
        {
            var listed = await _packages.ListByBatchIdAsync(lotId, cancellationToken).ConfigureAwait(false);
            foreach (var listedPkg in listed)
            {
                var pkg = await _packages.GetByIdAsync(listedPkg.Id, cancellationToken).ConfigureAwait(false);
                if (pkg is null || pkg.IsDeleted) continue;
                var qty = pkg.Quantity;
                try { pkg.ApplyStockStatus(pkgStatus); }
                catch (InvalidOperationException ex)
                {
                    return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-043", ex.Message));
                }
                if (pkg.Quantity != qty)
                    return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-043", "QC miktarı değiştiremez."));
            }

            var lot = await _batches.GetByIdAsync(lotId, cancellationToken).ConfigureAwait(false);
            lot?.SetStatus(balanceStatus);
        }

        var dest = await _balances.FindByKeyAsync(
            doc.OutputMaterialCode, doc.WarehouseCode, doc.LocationCode, doc.OutputLotNumber, doc.PlantId, cancellationToken).ConfigureAwait(false);
        if (dest is not null)
        {
            if (dest.QuantityOnHand != qtyBefore)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-043", "QC bakiyeyi değiştiremez; önce miktarı kontrol edin."));
            dest.SetStatus(balanceStatus);
            if (dest.QuantityOnHand != qtyBefore)
                return Result.Failure<ProductionOutputResultDto>(Error.Validation("PRD-OUT-043", "QC bakiyeyi değiştiremez."));
        }

        await _movements.AddAsync(InventoryMovement.Post(
            released ? ProductionLotCodes.QcReleaseMovement : ProductionLotCodes.QcRejectMovement,
            "Status",
            doc.Number,
            doc.OutputMaterialCode, "", "",
            doc.WarehouseCode, doc.LocationCode, doc.OutputLotNumber,
            0,
            doc.UnitOfMeasure,
            ProductionLotCodes.QcNotes(doc.QcDecision, doc.QcInspectionReference, actor),
            plantId: doc.PlantId), cancellationToken).ConfigureAwait(false);

        var result = await ReplayAsync(doc, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure) return result;
        return Result.Success(result.Value with
        {
            IdempotentReplay = false,
            StockStatus = doc.StockStatus,
            QcDecision = doc.QcDecision,
            QcDecidedBy = doc.QcDecidedBy,
            QcDecidedAt = doc.QcDecidedAt,
            QcInspectionReference = doc.QcInspectionReference,
            QcNotes = doc.QcNotes
        });
    }

    private async Task<Result<IReadOnlyList<PreparedSource>>> PrepareSourcesAsync(
        IReadOnlyList<ProductionOutputSourceRequestDto>? sources,
        Resolved c,
        CancellationToken cancellationToken)
    {
        var rows = new List<PreparedSource>();
        foreach (var src in sources ?? Array.Empty<ProductionOutputSourceRequestDto>())
        {
            if (src.ConsumedQuantity <= 0)
                return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Validation("PRD-OUT-010", "Kaynak tüketim miktarı pozitif olmalı."));
            var sourceLot = await _batches.GetByIdAsync(src.SourceLotId, cancellationToken).ConfigureAwait(false);
            if (sourceLot is null || sourceLot.IsDeleted)
                return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Validation("PRD-OUT-011", "Kaynak lot bulunamadı."));
            if (!string.IsNullOrWhiteSpace(sourceLot.PlantId)
                && !string.Equals(sourceLot.PlantId, c.PlantId, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Forbidden("PRD-OUT-403", "Kaynak lot başka tesise ait."));

            var srcMat = await _materials.GetByCodeAsync(sourceLot.MaterialCode, cancellationToken).ConfigureAwait(false);
            if (srcMat is null)
                return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Validation("PRD-OUT-012", $"Kaynak malzeme yok: {sourceLot.MaterialCode}"));

            var srcWh = src.SourceWarehouseCode.Trim();
            var srcLoc = src.SourceLocationCode.Trim();
            var srcBalance = await _balances.FindByKeyAsync(sourceLot.MaterialCode, srcWh, srcLoc, sourceLot.BatchNumber, c.PlantId, cancellationToken).ConfigureAwait(false);
            if (srcBalance is null)
                return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Validation("PRD-OUT-013", $"Kaynak bakiye yok: {sourceLot.BatchNumber}"));

            InventoryPackage? srcPkg = null;
            if (src.SourcePackageId is Guid pkgId)
            {
                srcPkg = await _packages.GetByIdAsync(pkgId, cancellationToken).ConfigureAwait(false);
                if (srcPkg is null || srcPkg.IsDeleted)
                    return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Validation("PRD-OUT-015", "Kaynak paket bulunamadı."));
                if (srcPkg.BatchId is Guid pkgLot && pkgLot != sourceLot.Id)
                    return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Validation("PRD-OUT-016", "Paket lotu kaynak lot ile aynı değil."));
            }

            var unit = string.IsNullOrWhiteSpace(src.Unit)
                ? (string.IsNullOrWhiteSpace(srcMat.UnitOfMeasure) ? c.Lines[0].Unit : srcMat.UnitOfMeasure)
                : src.Unit.Trim();
            rows.Add(new PreparedSource(src, sourceLot, srcMat, srcBalance, srcPkg, srcWh, srcLoc, unit));
        }

        foreach (var group in rows.GroupBy(r => (r.Lot.MaterialCode, r.WarehouseCode, r.LocationCode, r.Lot.BatchNumber, r.Balance.Id)))
        {
            var requested = group.Sum(x => x.Request.ConsumedQuantity);
            var available = group.First().Balance.QuantityOnHand - group.First().Balance.QuantityReserved;
            if (requested > available)
                return Result.Failure<IReadOnlyList<PreparedSource>>(Error.Validation(
                    "PRD-OUT-014",
                    $"Yetersiz bakiye {group.Key.BatchNumber}: {available} / {requested}."));
        }

        var packaged = rows.Where(r => r.Package is not null).ToArray();
        if (packaged.Length > 0)
        {
            var snaps = new Dictionary<Guid, PackagePhysicalSnapshot>();
            foreach (var row in packaged)
            {
                var pkg = row.Package!;
                if (snaps.ContainsKey(pkg.Id)) continue;
                var contents = await _packages.ListContentsAsync(pkg.Id, cancellationToken).ConfigureAwait(false);
                snaps[pkg.Id] = new PackagePhysicalSnapshot(
                    pkg.Id, pkg.PackageNumber, pkg.Status, pkg.Quantity, contents.Sum(c => c.Quantity));
            }
            var check = PackageConsumptionIntegrity.Validate(
                packaged.Select(r => (r.Package!.Id, r.Request.ConsumedQuantity)).ToArray(),
                snaps);
            if (check.IsFailure)
                return Result.Failure<IReadOnlyList<PreparedSource>>(check.Error!);
        }

        return Result.Success<IReadOnlyList<PreparedSource>>(rows);
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
            SourceLotNumbers = sourceNos,
            StockStatus = existing.StockStatus,
            Reversed = existing.Status == ProductionOutputStatuses.Cancelled,
            CancelReason = existing.CancelReason,
            QcDecision = existing.QcDecision,
            QcDecidedBy = existing.QcDecidedBy,
            QcDecidedAt = existing.QcDecidedAt,
            QcInspectionReference = existing.QcInspectionReference,
            QcNotes = existing.QcNotes
        });
    }

    private sealed record PreparedSource(
        ProductionOutputSourceRequestDto Request,
        Batch Lot,
        Material Material,
        InventoryBalance Balance,
        InventoryPackage? Package,
        string WarehouseCode,
        string LocationCode,
        string Unit);

    private sealed record Resolved(
        ProductionOrder Order,
        Material Material,
        Warehouse Warehouse,
        Location Location,
        string PlantId,
        IReadOnlyList<ProductionOutputResolvedLine> Lines);
}

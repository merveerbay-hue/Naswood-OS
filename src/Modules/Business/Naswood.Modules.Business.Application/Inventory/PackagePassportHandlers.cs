using System.Globalization;
using System.Text.Json;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public sealed record GetPackagePassportByIdQuery(Guid Id, IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<PackagePassportDto>>;

public sealed record GetPackagePassportByBarcodeQuery(string Barcode, IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<PackagePassportDto>>;

public sealed record GetPackagePassportByPublicIdQuery(string PublicId, IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<PackagePassportDto>>;

public sealed record RecordPackageLabelPrintCommand(Guid Id, IReadOnlyList<string>? AllowedPlantIds)
    : ICommand<Result<PackagePassportDto>>;

public sealed record RelocatePackageCommand(
    Guid Id,
    string WarehouseCode,
    string LocationCode,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor)
    : ICommand<Result<PackagePassportDto>>;

public static class PackagePassportComposer
{
    public static string Measurement(decimal? t, decimal? w, decimal? l)
    {
        var parts = new[] { t, w, l }.Where(v => v is > 0).Select(v => v!.Value.ToString("0.####", CultureInfo.InvariantCulture));
        return string.Join("×", parts);
    }

    public static PackageContentDto ToContent(InventoryPackageContent c) => new()
    {
        Id = c.Id,
        LineNo = c.LineNo,
        ThicknessMm = c.ThicknessMm,
        WidthMm = c.WidthMm,
        LengthMm = c.LengthMm,
        PieceCount = c.PieceCount,
        Quantity = c.Quantity,
        UnitOfMeasure = c.UnitOfMeasure,
        Measurement = Measurement(c.ThicknessMm, c.WidthMm, c.LengthMm)
    };
}

public sealed class PackagePassportLoader
{
    private readonly IInventoryPackageRepository _packages;
    private readonly IInventoryMovementRepository _movements;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IMaterialRepository _materials;
    private readonly IBatchRepository _batches;
    private readonly IProductionLotSourceRepository _lotSources;
    private readonly IProductionOrderRepository _orders;
    private readonly IPackageRelationRepository _relations;

    public PackagePassportLoader(
        IInventoryPackageRepository packages,
        IInventoryMovementRepository movements,
        IInventoryBalanceRepository balances,
        IMaterialRepository materials,
        IBatchRepository batches,
        IProductionLotSourceRepository lotSources,
        IProductionOrderRepository orders,
        IPackageRelationRepository relations)
    {
        _packages = packages;
        _movements = movements;
        _balances = balances;
        _materials = materials;
        _batches = batches;
        _lotSources = lotSources;
        _orders = orders;
        _relations = relations;
    }

    public async Task<Result<PackagePassportDto>> LoadAsync(
        InventoryPackage pkg,
        IReadOnlyList<string>? allowed,
        CancellationToken cancellationToken)
    {
        if (allowed is { Count: > 0 } && !PlantAccess.CanAccess(allowed, pkg.PlantId))
            return Result.Failure<PackagePassportDto>(Error.Forbidden("INV-PKG-403", "Bu paketi görüntüleme yetkiniz yok."));

        var contents = await _packages.ListContentsAsync(pkg.Id, cancellationToken).ConfigureAwait(false);
        var moves = await _movements.ListByPackageNumberAsync(pkg.PackageNumber, pkg.PlantId, cancellationToken).ConfigureAwait(false);
        var material = pkg.MaterialId is Guid materialId
            ? await _materials.GetByIdAsync(materialId, cancellationToken).ConfigureAwait(false)
            : null;
        material ??= await _materials.GetByCodeAsync(pkg.MaterialCode, cancellationToken).ConfigureAwait(false);
        var batch = pkg.BatchId is Guid batchId
            ? await _batches.GetByIdAsync(batchId, cancellationToken).ConfigureAwait(false)
            : null;
        batch ??= await _batches.GetByNumberAndMaterialAsync(pkg.LotNumber, pkg.MaterialCode, pkg.PlantId, cancellationToken).ConfigureAwait(false);
        var balance = await _balances.FindByKeyAsync(
            pkg.MaterialCode, pkg.WarehouseCode, pkg.LocationCode, pkg.LotNumber, pkg.PlantId, cancellationToken).ConfigureAwait(false);

        var def = ParseDef(material?.DefinitionJson);
        var totalPcs = contents.Sum(c => c.PieceCount ?? 0);
        var contentQty = contents.Count == 0 ? pkg.Quantity : contents.Sum(c => c.Quantity);
        var mismatch = balance is not null && contentQty != balance.QuantityOnHand;

        var rels = await _relations.ListByPackageIdAsync(pkg.Id, cancellationToken).ConfigureAwait(false);
        var relatedIds = rels.SelectMany(r => new[] { r.SourcePackageId, r.TargetPackageId }).Distinct().ToArray();
        var relatedPkgs = new Dictionary<Guid, InventoryPackage>();
        foreach (var rid in relatedIds)
        {
            var row = await _packages.GetByIdAsync(rid, cancellationToken).ConfigureAwait(false);
            if (row is not null) relatedPkgs[rid] = row;
        }

        var sourceLots = Array.Empty<string>();
        var productionOrder = batch?.SourceReferenceNo ?? string.Empty;
        if (batch is not null && string.Equals(batch.SourceType, ProductionLotCodes.SourceType, StringComparison.OrdinalIgnoreCase))
        {
            var links = await _lotSources.ListByProductionLotIdAsync(batch.Id, cancellationToken).ConfigureAwait(false);
            var names = new List<string>();
            foreach (var link in links)
            {
                var src = await _batches.GetByIdAsync(link.SourceLotId, cancellationToken).ConfigureAwait(false);
                if (src is not null) names.Add(src.BatchNumber);
            }
            sourceLots = names.ToArray();
            var first = links.FirstOrDefault();
            if (first is not null)
            {
                var order = await _orders.GetByIdAsync(first.ProductionOrderId, cancellationToken).ConfigureAwait(false);
                if (order is not null) productionOrder = order.Code;
            }
        }

        return Result.Success(new PackagePassportDto
        {
            Id = pkg.Id,
            PackageNo = pkg.PackageNumber,
            Barcode = pkg.Barcode,
            PublicId = pkg.PublicId,
            QrPath = PackageIdentityService.QrPath(pkg.PublicId),
            MaterialCode = pkg.MaterialCode,
            MaterialName = material?.Name ?? pkg.MaterialCode,
            MaterialGroup = Read(def, "mainCategory") ?? material?.Category ?? string.Empty,
            MaterialType = Read(def, "materialTypeToken") ?? Read(def, "materialType") ?? string.Empty,
            WoodSpecies = Read(def, "woodToken") ?? Read(def, "species") ?? string.Empty,
            Quality = Read(def, "grade") ?? string.Empty,
            StockUnit = Read(def, "stockUom") ?? material?.UnitOfMeasure ?? pkg.UnitOfMeasure,
            CountUnit = Read(def, "countUom") ?? "PCS",
            LotNumber = pkg.LotNumber,
            SourceType = batch?.SourceType ?? string.Empty,
            SourceReferenceNo = batch?.SourceReferenceNo ?? string.Empty,
            ProductionOrderNumber = productionOrder,
            SourceLotCount = sourceLots.Length,
            SourceLotNumbers = sourceLots,
            Factory = pkg.PlantId ?? string.Empty,
            WarehouseCode = pkg.WarehouseCode,
            LocationCode = pkg.LocationCode,
            Status = pkg.Status,
            Quantity = pkg.Quantity,
            UnitOfMeasure = pkg.UnitOfMeasure,
            TotalPieceCount = totalPcs > 0 ? totalPcs : null,
            PhysicalGroupLabel = pkg.PhysicalGroupLabel,
            CreatedAt = pkg.CreatedAt,
            LastMovementAt = moves.Count == 0 ? null : moves.Max(m => m.CreatedAt),
            LabelPrintedAt = pkg.LabelPrintedAt,
            LabelPrintCount = pkg.LabelPrintCount,
            PackageBalanceMismatch = mismatch,
            AllowedActions = PackageStatePolicy.AllowedActions(pkg),
            InactiveReason = PackageStatePolicy.InactiveReason(pkg),
            LabelHint = PackageStatePolicy.LabelHint(pkg, rels),
            Relations = rels.Select(r => new PackageRelationRowDto
            {
                RelationType = r.RelationType,
                SourcePackageId = r.SourcePackageId,
                TargetPackageId = r.TargetPackageId,
                SourcePackageNo = relatedPkgs.TryGetValue(r.SourcePackageId, out var src) ? src.PackageNumber : "",
                TargetPackageNo = relatedPkgs.TryGetValue(r.TargetPackageId, out var tgt) ? tgt.PackageNumber : "",
                Quantity = r.Quantity,
                Unit = r.Unit,
                Direction = r.SourcePackageId == pkg.Id ? "OUT" : "IN"
            }).ToArray(),
            Contents = contents.Select(PackagePassportComposer.ToContent).ToArray(),
            Movements = moves.Select(m => new PackageMovementRowDto
            {
                At = m.CreatedAt,
                Action = m.MovementType,
                FromLocation = m.Direction == "Out" ? $"{m.WarehouseCode}/{m.LocationCode}" : string.Empty,
                ToLocation = m.Direction == "In" ? $"{m.WarehouseCode}/{m.LocationCode}" : string.Empty,
                Quantity = m.Quantity,
                Reference = m.DocumentNumber,
                Unit = m.UnitOfMeasure
            }).ToArray()
        });
    }

    private static Dictionary<string, string> ParseDef(string? json)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(json)) return map;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return map;
            foreach (var p in doc.RootElement.EnumerateObject())
            {
                if (p.Value.ValueKind == JsonValueKind.String)
                    map[p.Name] = p.Value.GetString() ?? string.Empty;
            }
        }
        catch (JsonException) { }
        return map;
    }

    private static string? Read(Dictionary<string, string> map, string key)
        => map.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v) ? v : null;
}

public sealed class GetPackagePassportByIdQueryHandler : IQueryHandler<GetPackagePassportByIdQuery, Result<PackagePassportDto>>
{
    private readonly IInventoryPackageRepository _packages;
    private readonly PackagePassportLoader _loader;

    public GetPackagePassportByIdQueryHandler(IInventoryPackageRepository packages, PackagePassportLoader loader)
    {
        _packages = packages;
        _loader = loader;
    }

    public async Task<Result<PackagePassportDto>> HandleAsync(GetPackagePassportByIdQuery query, CancellationToken cancellationToken = default)
    {
        var pkg = await _packages.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (pkg is null || pkg.IsDeleted)
            return Result.Failure<PackagePassportDto>(Error.NotFound("INV-PKG-404", "Paket bulunamadı."));
        return await _loader.LoadAsync(pkg, query.AllowedPlantIds, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetPackagePassportByBarcodeQueryHandler : IQueryHandler<GetPackagePassportByBarcodeQuery, Result<PackagePassportDto>>
{
    private readonly IInventoryPackageRepository _packages;
    private readonly PackagePassportLoader _loader;

    public GetPackagePassportByBarcodeQueryHandler(IInventoryPackageRepository packages, PackagePassportLoader loader)
    {
        _packages = packages;
        _loader = loader;
    }

    public async Task<Result<PackagePassportDto>> HandleAsync(GetPackagePassportByBarcodeQuery query, CancellationToken cancellationToken = default)
    {
        var key = (query.Barcode ?? string.Empty).Trim();
        if (key.Length == 0)
            return Result.Failure<PackagePassportDto>(Error.Validation("INV-PKG-070", "Barkod gerekli."));
        var hits = await _packages.ListByBarcodeExactAsync(key, cancellationToken).ConfigureAwait(false);
        if (hits.Count == 0)
            return Result.Failure<PackagePassportDto>(Error.NotFound("INV-PKG-404", "Barkod sistemde bulunamadı."));
        if (hits.Count > 1)
            return Result.Failure<PackagePassportDto>(Error.Conflict("INV-PKG-071", "Aynı barkod birden fazla pakete bağlı — veri bütünlüğü hatası."));
        return await _loader.LoadAsync(hits[0], query.AllowedPlantIds, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetPackagePassportByPublicIdQueryHandler : IQueryHandler<GetPackagePassportByPublicIdQuery, Result<PackagePassportDto>>
{
    private readonly IInventoryPackageRepository _packages;
    private readonly PackagePassportLoader _loader;

    public GetPackagePassportByPublicIdQueryHandler(IInventoryPackageRepository packages, PackagePassportLoader loader)
    {
        _packages = packages;
        _loader = loader;
    }

    public async Task<Result<PackagePassportDto>> HandleAsync(GetPackagePassportByPublicIdQuery query, CancellationToken cancellationToken = default)
    {
        var pkg = await _packages.GetByPublicIdAsync(query.PublicId, cancellationToken).ConfigureAwait(false);
        if (pkg is null || pkg.IsDeleted)
            return Result.Failure<PackagePassportDto>(Error.NotFound("INV-PKG-404", "Barkod sistemde bulunamadı."));
        return await _loader.LoadAsync(pkg, query.AllowedPlantIds, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class RecordPackageLabelPrintCommandHandler : ICommandHandler<RecordPackageLabelPrintCommand, Result<PackagePassportDto>>
{
    private readonly IInventoryPackageRepository _packages;
    private readonly PackagePassportLoader _loader;
    private readonly IBusinessUnitOfWork _uow;

    public RecordPackageLabelPrintCommandHandler(
        IInventoryPackageRepository packages,
        PackagePassportLoader loader,
        IBusinessUnitOfWork uow)
    {
        _packages = packages;
        _loader = loader;
        _uow = uow;
    }

    public async Task<Result<PackagePassportDto>> HandleAsync(RecordPackageLabelPrintCommand command, CancellationToken cancellationToken = default)
    {
        var pkg = await _packages.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (pkg is null || pkg.IsDeleted)
            return Result.Failure<PackagePassportDto>(Error.NotFound("INV-PKG-404", "Paket bulunamadı."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, pkg.PlantId))
            return Result.Failure<PackagePassportDto>(Error.Forbidden("INV-PKG-403", "Bu paketin etiketini yazdırma yetkiniz yok."));
        pkg.RecordLabelPrint();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return await _loader.LoadAsync(pkg, command.AllowedPlantIds, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class RelocatePackageCommandHandler : ICommandHandler<RelocatePackageCommand, Result<PackagePassportDto>>
{
    private readonly IInventoryPackageRepository _packages;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryMovementRepository _movements;
    private readonly ILocationRepository _locations;
    private readonly IWarehouseRepository _warehouses;
    private readonly IMaterialIdentityRepository _identities;
    private readonly PackagePassportLoader _loader;
    private readonly IBusinessUnitOfWork _uow;

    public RelocatePackageCommandHandler(
        IInventoryPackageRepository packages,
        IInventoryBalanceRepository balances,
        IInventoryMovementRepository movements,
        ILocationRepository locations,
        IWarehouseRepository warehouses,
        IMaterialIdentityRepository identities,
        PackagePassportLoader loader,
        IBusinessUnitOfWork uow)
    {
        _packages = packages;
        _balances = balances;
        _movements = movements;
        _locations = locations;
        _warehouses = warehouses;
        _identities = identities;
        _loader = loader;
        _uow = uow;
    }

    public async Task<Result<PackagePassportDto>> HandleAsync(RelocatePackageCommand command, CancellationToken cancellationToken = default)
    {
        var pkg = await _packages.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (pkg is null || pkg.IsDeleted)
            return Result.Failure<PackagePassportDto>(Error.NotFound("INV-PKG-404", "Paket bulunamadı."));
        var plantId = pkg.PlantId ?? string.Empty;
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<PackagePassportDto>(Error.Forbidden("INV-PKG-403", "Bu paketi taşıma yetkiniz yok."));
        var closed = PackageStatePolicy.GuardRejected(pkg, "MOVE");
        if (closed.IsFailure)
            return Result.Failure<PackagePassportDto>(closed.Error!);
        if (!PackageStatePolicy.CanMove(pkg))
            return Result.Failure<PackagePassportDto>(Error.Validation("PKG-LIFE-006", "Paket taşınamaz."));

        var toWh = command.WarehouseCode.Trim();
        var toLoc = command.LocationCode.Trim();
        if (string.Equals(pkg.WarehouseCode, toWh, StringComparison.OrdinalIgnoreCase)
            && string.Equals(pkg.LocationCode, toLoc, StringComparison.OrdinalIgnoreCase))
            return await _loader.LoadAsync(pkg, command.AllowedPlantIds, cancellationToken).ConfigureAwait(false);

        var loc = await _locations.FindByWarehouseAndCodeAsync(toWh, toLoc, plantId, cancellationToken).ConfigureAwait(false);
        if (loc is null)
            return Result.Failure<PackagePassportDto>(Error.Forbidden("INV-PKG-403", $"Lokasyon '{toLoc}' bu tesise ait değil."));

        var fromWh = pkg.WarehouseCode;
        var fromLoc = pkg.LocationCode;
        var qty = pkg.Quantity;
        var lot = pkg.LotNumber;
        var material = pkg.MaterialCode;

        var source = await _balances.FindByKeyAsync(material, fromWh, fromLoc, lot, plantId, cancellationToken).ConfigureAwait(false);
        if (source is null)
            return Result.Failure<PackagePassportDto>(Error.Validation("INV-TRF-011", "Kaynak stok bakiyesi yok."));
        try { source.ApplyIssue(qty); }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<PackagePassportDto>(Error.Validation("INV-TRF-012", ex.Message));
        }

        var dest = await _balances.FindByKeyAsync(material, toWh, toLoc, lot, plantId, cancellationToken).ConfigureAwait(false);
        if (dest is null)
        {
            dest = InventoryBalance.Create(material, toWh, toLoc, lot, 0, 0, "Active", plantId: plantId);
            await _balances.AddAsync(dest, cancellationToken).ConfigureAwait(false);
        }
        dest.ApplyReceipt(qty);

        var destWh = await _warehouses.GetByCodeAndPlantAsync(toWh, plantId, cancellationToken).ConfigureAwait(false);
        pkg.Relocate(toWh, toLoc, destWh?.Id, loc.Id);
        if (!string.IsNullOrWhiteSpace(pkg.MaterialIdentityNumber))
        {
            var mi = await _identities.GetByNumberAsync(pkg.MaterialIdentityNumber, plantId, cancellationToken).ConfigureAwait(false);
            mi?.Relocate(toWh, toLoc);
        }

        var doc = $"PKG-MOVE-{pkg.PackageNumber}";
        var note = $"pkg={pkg.PackageNumber} barcode={pkg.Barcode} {fromWh}/{fromLoc}→{toWh}/{toLoc} by={command.Actor}";
        await _movements.AddAsync(InventoryMovement.Post("PACKAGE_MOVE", "Out", doc, material, pkg.MaterialIdentityNumber, pkg.PackageNumber, fromWh, fromLoc, lot, qty, pkg.UnitOfMeasure, note, plantId: plantId), cancellationToken).ConfigureAwait(false);
        await _movements.AddAsync(InventoryMovement.Post("PACKAGE_MOVE", "In", doc, material, pkg.MaterialIdentityNumber, pkg.PackageNumber, toWh, toLoc, lot, qty, pkg.UnitOfMeasure, note, plantId: plantId), cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return await _loader.LoadAsync(pkg, command.AllowedPlantIds, cancellationToken).ConfigureAwait(false);
    }
}

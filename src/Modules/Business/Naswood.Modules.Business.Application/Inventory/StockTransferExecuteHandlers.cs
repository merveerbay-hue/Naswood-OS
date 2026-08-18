using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

/// <summary>
/// Intra-factory stock location transfer.
/// Reuses balance ApplyIssue/ApplyReceipt + InventoryMovement ledger; does not rewrite GR/GI.
/// </summary>
public sealed record ExecuteStockTransferCommand(
    string Number,
    string MaterialCode,
    string LotNumber,
    string FromWarehouseCode,
    string FromLocationCode,
    string ToWarehouseCode,
    string ToLocationCode,
    decimal Quantity,
    string UnitOfMeasure,
    string PackageNumber,
    string MaterialIdentityNumber,
    string Notes,
    string? PlantId,
    string? ToPlantId,
    IReadOnlyList<string>? AllowedPlantIds = null) : ICommand<Result<ExecuteStockTransferResultDto>>;

public sealed class ExecuteStockTransferCommandHandler
    : ICommandHandler<ExecuteStockTransferCommand, Result<ExecuteStockTransferResultDto>>
{
    private readonly IStockTransferRepository _transfers;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryMovementRepository _movements;
    private readonly IInventoryPackageRepository _packages;
    private readonly IMaterialIdentityRepository _identities;
    private readonly IWarehouseRepository _warehouses;
    private readonly ILocationRepository _locations;
    private readonly IBusinessUnitOfWork _uow;

    public ExecuteStockTransferCommandHandler(
        IStockTransferRepository transfers,
        IInventoryBalanceRepository balances,
        IInventoryMovementRepository movements,
        IInventoryPackageRepository packages,
        IMaterialIdentityRepository identities,
        IWarehouseRepository warehouses,
        ILocationRepository locations,
        IBusinessUnitOfWork uow)
    {
        _transfers = transfers;
        _balances = balances;
        _movements = movements;
        _packages = packages;
        _identities = identities;
        _warehouses = warehouses;
        _locations = locations;
        _uow = uow;
    }

    public async Task<Result<ExecuteStockTransferResultDto>> HandleAsync(
        ExecuteStockTransferCommand command,
        CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(command.PlantId) ? "PLANT-001" : command.PlantId.Trim();
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Forbidden(
                "INV-TRF-403",
                "Bu tesiste transfer yetkiniz yok."));

        // Rule 10 — inter-factory transfer is a future module.
        if (!string.IsNullOrWhiteSpace(command.ToPlantId)
            && !string.Equals(command.ToPlantId.Trim(), plantId, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-010",
                "Fabrikalar arası transfer bu modülde desteklenmez. Kaynak ve hedef aynı fabrikada olmalıdır."));
        }

        if (string.IsNullOrWhiteSpace(command.MaterialCode))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation("INV-TRF-001", "MaterialCode is required."));
        if (command.Quantity <= 0)
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation("INV-TRF-002", "Transfer quantity must be positive."));
        if (string.IsNullOrWhiteSpace(command.FromWarehouseCode) || string.IsNullOrWhiteSpace(command.FromLocationCode))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation("INV-TRF-003", "Source warehouse and location are required."));
        if (string.IsNullOrWhiteSpace(command.ToWarehouseCode) || string.IsNullOrWhiteSpace(command.ToLocationCode))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation("INV-TRF-004", "Target warehouse and location are required."));

        var materialCode = command.MaterialCode.Trim();
        var lotNumber = string.IsNullOrWhiteSpace(command.LotNumber) ? string.Empty : command.LotNumber.Trim();
        var fromWh = command.FromWarehouseCode.Trim();
        var fromLoc = command.FromLocationCode.Trim();
        var toWh = command.ToWarehouseCode.Trim();
        var toLoc = command.ToLocationCode.Trim();
        var uom = string.IsNullOrWhiteSpace(command.UnitOfMeasure) ? "Piece" : command.UnitOfMeasure.Trim();

        // Rule 8 — same source and target location blocked.
        if (string.Equals(fromWh, toWh, StringComparison.OrdinalIgnoreCase)
            && string.Equals(fromLoc, toLoc, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-005",
                "Kaynak ve hedef lokasyon aynı olamaz."));
        }

        var fromWarehouse = await _warehouses.GetByCodeAndPlantAsync(fromWh, plantId, cancellationToken).ConfigureAwait(false);
        if (fromWarehouse is null || fromWarehouse.IsDeleted)
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-017",
                $"Kaynak depo '{fromWh}' bu tesiste ({plantId}) tanımlı değil."));
        if (!string.Equals(fromWarehouse.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-016",
                $"Kaynak depo '{fromWh}' pasif — transfer yapılamaz."));
        if (!string.IsNullOrWhiteSpace(fromWarehouse.PlantId)
            && !string.Equals(fromWarehouse.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Forbidden(
                "INV-TRF-403",
                $"Kaynak depo '{fromWh}' seçili fabrikaya ait değil."));

        var toWarehouse = await _warehouses.GetByCodeAndPlantAsync(toWh, plantId, cancellationToken).ConfigureAwait(false);
        if (toWarehouse is null || toWarehouse.IsDeleted)
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-017",
                $"Hedef depo '{toWh}' bu tesiste ({plantId}) tanımlı değil."));
        if (!string.Equals(toWarehouse.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-016",
                $"Hedef depo '{toWh}' pasif — transfer yapılamaz."));
        if (!string.IsNullOrWhiteSpace(toWarehouse.PlantId)
            && !string.Equals(toWarehouse.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Forbidden(
                "INV-TRF-403",
                $"Hedef depo '{toWh}' seçili fabrikaya ait değil."));

        var fromLocation = await _locations.FindByWarehouseAndCodeAsync(fromWh, fromLoc, plantId, cancellationToken).ConfigureAwait(false);
        if (fromLocation is null || fromLocation.IsDeleted)
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-019",
                $"Kaynak lokasyon '{fromLoc}' depo '{fromWh}' / tesis '{plantId}' altında tanımlı değil."));
        if (!string.Equals(fromLocation.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-015",
                $"Kaynak lokasyon '{fromLoc}' pasif — transfer yapılamaz."));

        var toLocation = await _locations.FindByWarehouseAndCodeAsync(toWh, toLoc, plantId, cancellationToken).ConfigureAwait(false);
        if (toLocation is null || toLocation.IsDeleted)
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-019",
                $"Hedef lokasyon '{toLoc}' depo '{toWh}' / tesis '{plantId}' altında tanımlı değil."));
        if (!string.Equals(toLocation.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-015",
                $"Hedef lokasyon '{toLoc}' pasif — transfer yapılamaz."));

        // Canonical master casing.
        fromWh = fromWarehouse.Code;
        toWh = toWarehouse.Code;
        fromLoc = fromLocation.Code;
        toLoc = toLocation.Code;

        var packageNumber = string.IsNullOrWhiteSpace(command.PackageNumber) ? string.Empty : command.PackageNumber.Trim();
        var miNumber = string.IsNullOrWhiteSpace(command.MaterialIdentityNumber) ? string.Empty : command.MaterialIdentityNumber.Trim();
        InventoryPackage? package = null;
        MaterialIdentity? identity = null;

        if (!string.IsNullOrWhiteSpace(packageNumber))
        {
            package = await _packages.GetByNumberAsync(packageNumber, cancellationToken).ConfigureAwait(false);
            if (package is null)
                return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                    "INV-TRF-013",
                    $"Paket '{packageNumber}' bulunamadı."));
            if (!string.Equals(package.MaterialCode, materialCode, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(package.LotNumber ?? string.Empty, lotNumber, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(package.WarehouseCode, fromWh, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(package.LocationCode, fromLoc, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                    "INV-TRF-013",
                    "Paket kaynak lokasyon / malzeme / lot ile uyuşmuyor."));
            }
            if (package.Quantity < command.Quantity)
                return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                    "INV-TRF-012",
                    "Paket miktarı yetersiz."));
            if (string.IsNullOrWhiteSpace(miNumber))
                miNumber = package.MaterialIdentityNumber ?? string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(miNumber))
        {
            identity = await _identities.GetByNumberAsync(miNumber, cancellationToken).ConfigureAwait(false);
        }

        var source = await _balances.FindByKeyAsync(materialCode, fromWh, fromLoc, lotNumber, plantId, cancellationToken).ConfigureAwait(false);
        if (source is null)
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation(
                "INV-TRF-011",
                $"Kaynak stok yok: {materialCode}/{fromWh}/{fromLoc}/{lotNumber}."));

        try
        {
            source.ApplyIssue(command.Quantity);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation("INV-TRF-012", ex.Message));
        }

        var destStatus = string.IsNullOrWhiteSpace(source.Status) ? "Active" : source.Status;
        var dest = await _balances.FindByKeyAsync(materialCode, toWh, toLoc, lotNumber, plantId, cancellationToken).ConfigureAwait(false);
        if (dest is null)
        {
            dest = InventoryBalance.Create(
                materialCode, toWh, toLoc, lotNumber, command.Quantity, 0, destStatus, plantId: plantId);
            await _balances.AddAsync(dest, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            try
            {
                dest.ApplyReceipt(command.Quantity);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<ExecuteStockTransferResultDto>(Error.Validation("INV-TRF-012", ex.Message));
            }
        }

        // Full-package relocate preserves package identity; partial package split is out of scope.
        if (package is not null && package.Quantity == command.Quantity)
            package.Relocate(toWh, toLoc);

        if (identity is not null
            && string.Equals(identity.WarehouseCode, fromWh, StringComparison.OrdinalIgnoreCase)
            && string.Equals(identity.LocationCode, fromLoc, StringComparison.OrdinalIgnoreCase)
            && identity.Quantity == command.Quantity)
        {
            identity.Relocate(toWh, toLoc);
        }

        var transferNumber = SystemIdentifier.Ensure(command.Number, "TRF");
        var headerNote = Truncate(
            $"{fromWh}/{fromLoc}→{toWh}/{toLoc} {materialCode} {lotNumber} qty={command.Quantity}"
            + (string.IsNullOrWhiteSpace(command.Notes) ? string.Empty : $" | {command.Notes.Trim()}"),
            200);

        var transfer = StockTransfer.Create(
            transferNumber,
            fromWh,
            toWh,
            "Posted",
            headerNote,
            plantId: plantId);
        await _transfers.AddAsync(transfer, cancellationToken).ConfigureAwait(false);

        var moveNotes = Truncate(
            string.IsNullOrWhiteSpace(command.Notes) ? $"ref={transferNumber}" : $"ref={transferNumber} | {command.Notes.Trim()}",
            2000);

        // Ledger pair — same DocumentNumber (transfer ref). Qty stored positive; Direction carries sign semantics (existing ledger pattern).
        var outMove = InventoryMovement.Post(
            "TRANSFER_OUT",
            "Out",
            transferNumber,
            materialCode,
            miNumber,
            packageNumber,
            fromWh,
            fromLoc,
            lotNumber,
            command.Quantity,
            uom,
            moveNotes,
            plantId: plantId);
        await _movements.AddAsync(outMove, cancellationToken).ConfigureAwait(false);

        var inMove = InventoryMovement.Post(
            "TRANSFER_IN",
            "In",
            transferNumber,
            materialCode,
            miNumber,
            packageNumber,
            toWh,
            toLoc,
            lotNumber,
            command.Quantity,
            uom,
            moveNotes,
            plantId: plantId);
        await _movements.AddAsync(inMove, cancellationToken).ConfigureAwait(false);

        // Single SaveChanges — source issue + dest receipt + both movements commit together or not at all.
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(new ExecuteStockTransferResultDto
        {
            DocumentId = transfer.Id,
            DocumentNumber = transfer.Number,
            Status = transfer.Status,
            PlantId = plantId,
            MaterialCode = materialCode,
            LotNumber = lotNumber,
            FromWarehouseCode = fromWh,
            FromLocationCode = fromLoc,
            ToWarehouseCode = toWh,
            ToLocationCode = toLoc,
            Quantity = command.Quantity,
            OutMovementNumber = outMove.MovementNumber,
            InMovementNumber = inMove.MovementNumber
        });
    }

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= max ? value : value[..max];
    }
}

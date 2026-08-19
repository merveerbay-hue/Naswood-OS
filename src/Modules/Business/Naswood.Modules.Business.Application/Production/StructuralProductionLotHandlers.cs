using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IStructuralProductionLotRepository
{
    Task<StructuralProductionLot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> NumberExistsAsync(string productionLotNumber, CancellationToken cancellationToken = default);
    Task<int> CountByPlantAsync(string plantId, CancellationToken cancellationToken = default);
    Task AddAsync(StructuralProductionLot entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<StructuralProductionLot> Items, int Total)> SearchAsync(
        string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default);
}

public sealed record SearchStructuralProductionLotQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedStructuralProductionLotDto>>;

public sealed record GetStructuralProductionLotByIdQuery(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<StructuralProductionLotDto>>;

public sealed record CreateStructuralProductionLotCommand(
    Guid MaterialId,
    string ActualGradingMethod,
    string? WorkCenterCode,
    DateOnly ProductionDate,
    string? ShiftCode,
    string Notes,
    IReadOnlyList<StructuralProductionLotInputRequestDto>? Inputs,
    string PlantId,
    IReadOnlyList<string>? AllowedPlantIds,
    string? CreatedBy) : ICommand<Result<StructuralProductionLotDto>>;

public sealed record UpdateStructuralProductionLotDraftCommand(
    Guid Id,
    string? WorkCenterCode,
    DateOnly ProductionDate,
    string? ShiftCode,
    string Notes,
    IReadOnlyList<StructuralProductionLotInputRequestDto>? Inputs,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<StructuralProductionLotDto>>;

public sealed record TransitionStructuralProductionLotStatusCommand(
    Guid Id,
    string Status,
    IReadOnlyList<string>? AllowedPlantIds,
    string? Actor) : ICommand<Result<StructuralProductionLotDto>>;

public sealed record CancelStructuralProductionLotCommand(
    Guid Id,
    string Reason,
    IReadOnlyList<string>? AllowedPlantIds,
    string? CancelledBy) : ICommand<Result<StructuralProductionLotDto>>;

public static class StructuralProductionLotMapper
{
    public static StructuralProductionLotDto ToDto(StructuralProductionLot e) => new()
    {
        Id = e.Id,
        ProductionLotNumber = e.ProductionLotNumber,
        MaterialId = e.MaterialId,
        MaterialCode = e.MaterialCode,
        ActualGradingMethod = e.ActualGradingMethod,
        WorkCenterCode = e.WorkCenterCode,
        ProductionDate = e.ProductionDate,
        ShiftCode = e.ShiftCode,
        StartedAt = e.StartedAt,
        CompletedAt = e.CompletedAt,
        Status = e.Status,
        Notes = e.Notes,
        PlantId = e.PlantId,
        CreatedBy = e.CreatedBy,
        ReleasedAt = e.ReleasedAt,
        ReleasedBy = e.ReleasedBy,
        CancellationReason = e.CancellationReason,
        CancelledAt = e.CancelledAt,
        CancelledBy = e.CancelledBy,
        CreatedAt = e.CreatedAt,
        Inputs = e.Inputs.Select(i => new StructuralProductionLotInputDto
        {
            Id = i.Id,
            SourceBatchId = i.SourceBatchId,
            SourceInventoryBalanceId = i.SourceInventoryBalanceId,
            SourcePackageId = i.SourcePackageId,
            Quantity = i.Quantity,
            Unit = i.Unit
        }).ToArray()
    };
}

public sealed class SearchStructuralProductionLotQueryHandler
    : IQueryHandler<SearchStructuralProductionLotQuery, Result<PagedStructuralProductionLotDto>>
{
    private readonly IStructuralProductionLotRepository _repo;
    public SearchStructuralProductionLotQueryHandler(IStructuralProductionLotRepository repo) => _repo = repo;

    public async Task<Result<PagedStructuralProductionLotDto>> HandleAsync(
        SearchStructuralProductionLotQuery query, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(query.PlantId) ? null : query.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<PagedStructuralProductionLotDto>(Error.Validation(
                "PRD-SPL-004", "Plant / factory context is required."));
        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, plantId))
            return Result.Failure<PagedStructuralProductionLotDto>(Error.Forbidden(
                "PRD-SPL-403", "Bu tesisin yapısal üretim lotlarını görüntüleme yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, plantId, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedStructuralProductionLotDto
        {
            Items = items.Select(StructuralProductionLotMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetStructuralProductionLotByIdQueryHandler
    : IQueryHandler<GetStructuralProductionLotByIdQuery, Result<StructuralProductionLotDto>>
{
    private readonly IStructuralProductionLotRepository _repo;
    public GetStructuralProductionLotByIdQueryHandler(IStructuralProductionLotRepository repo) => _repo = repo;

    public async Task<Result<StructuralProductionLotDto>> HandleAsync(
        GetStructuralProductionLotByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<StructuralProductionLotDto>(Error.NotFound("BUS-001", "StructuralProductionLot was not found."));
        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<StructuralProductionLotDto>(Error.Forbidden(
                "PRD-SPL-403", "Bu yapısal üretim lotunu görüntüleme yetkiniz yok."));
        return Result.Success(StructuralProductionLotMapper.ToDto(e));
    }
}

public sealed class CreateStructuralProductionLotCommandHandler
    : ICommandHandler<CreateStructuralProductionLotCommand, Result<StructuralProductionLotDto>>
{
    private readonly IStructuralProductionLotRepository _repo;
    private readonly IMaterialRepository _materials;
    private readonly IBatchRepository _batches;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryPackageRepository _packages;
    private readonly IWorkCenterRepository _workCenters;
    private readonly IBusinessUnitOfWork _uow;

    public CreateStructuralProductionLotCommandHandler(
        IStructuralProductionLotRepository repo,
        IMaterialRepository materials,
        IBatchRepository batches,
        IInventoryBalanceRepository balances,
        IInventoryPackageRepository packages,
        IWorkCenterRepository workCenters,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _materials = materials;
        _batches = batches;
        _balances = balances;
        _packages = packages;
        _workCenters = workCenters;
        _uow = uow;
    }

    public async Task<Result<StructuralProductionLotDto>> HandleAsync(
        CreateStructuralProductionLotCommand command, CancellationToken cancellationToken = default)
    {
        var plantId = command.PlantId.Trim();
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<StructuralProductionLotDto>(Error.Forbidden(
                "PRD-SPL-403", "Bu tesiste yapısal üretim lotu oluşturma yetkiniz yok."));

        var material = await _materials.GetByIdAsync(command.MaterialId, cancellationToken).ConfigureAwait(false);
        if (material is null || material.IsDeleted)
            return Result.Failure<StructuralProductionLotDto>(Error.NotFound("BUS-001", "Material was not found."));

        if (!MaterialCompliance.IsStructuralTimber(material.DefinitionJson))
            return Result.Failure<StructuralProductionLotDto>(Error.Validation(
                "PRD-SPL-SCOPE",
                "Production Lot yalnızca STRUCTURAL_TIMBER kapsamındaki malzemeler için oluşturulabilir."));

        var method = (command.ActualGradingMethod ?? string.Empty).Trim().ToUpperInvariant();
        if (method is not ("VISUAL" or "MACHINE"))
            return Result.Failure<StructuralProductionLotDto>(Error.Validation(
                "PRD-SPL-METHOD", "ActualGradingMethod must be VISUAL or MACHINE."));

        var supported = MaterialCompliance.GetSupportedGradingMethods(material.DefinitionJson);
        if (!supported.Any(m => string.Equals(m, method, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure<StructuralProductionLotDto>(Error.Validation(
                "PRD-SPL-METHOD",
                $"Malzeme bu yöntemi desteklemiyor: {method}. Desteklenen: {string.Join(", ", supported)}."));

        // Work Center is optional — missing master must not block lot creation.
        string? workCenterCode = null;
        if (!string.IsNullOrWhiteSpace(command.WorkCenterCode))
        {
            var wc = await _workCenters.GetByCodeAndPlantAsync(command.WorkCenterCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
            workCenterCode = wc?.Code ?? command.WorkCenterCode.Trim();
        }

        var inputsReq = command.Inputs ?? Array.Empty<StructuralProductionLotInputRequestDto>();
        var sourceCheck = await StructuralProductionLotSourceGuard.ValidateAsync(
            inputsReq, plantId, material.Code, _batches, _balances, _packages, cancellationToken).ConfigureAwait(false);
        if (sourceCheck.IsFailure)
            return Result.Failure<StructuralProductionLotDto>(sourceCheck.Error!);

        var number = await MintNumberAsync(plantId, cancellationToken).ConfigureAwait(false);
        var lot = StructuralProductionLot.Create(
            number,
            material.Id,
            material.Code,
            method,
            command.ProductionDate,
            workCenterCode,
            command.ShiftCode,
            command.Notes,
            command.CreatedBy,
            plantId: plantId);

        var inputs = inputsReq.Select(req => StructuralProductionLotInput.Create(
            lot.Id,
            req.Quantity,
            req.Unit,
            req.SourceBatchId,
            req.SourceInventoryBalanceId,
            req.SourcePackageId)).ToList();
        lot.ReplaceDraftInputs(inputs);

        await _repo.AddAsync(lot, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StructuralProductionLotMapper.ToDto(lot));
    }

    private async Task<string> MintNumberAsync(string plantId, CancellationToken cancellationToken)
    {
        // PlantId in this system is the plant code (e.g. F01) from Plant master / JWT claims.
        var plantCode = plantId.Trim().ToUpperInvariant();
        for (var attempt = 0; attempt < 8; attempt++)
        {
            var count = await _repo.CountByPlantAsync(plantId, cancellationToken).ConfigureAwait(false);
            var seq = count + 1 + attempt;
            var candidate = $"PLOT-{plantCode}-{seq:000000}";
            if (!await _repo.NumberExistsAsync(candidate, cancellationToken).ConfigureAwait(false))
                return candidate;
        }
        return $"PLOT-{plantCode}-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}

internal static class StructuralProductionLotSourceGuard
{
    public static async Task<Result> ValidateAsync(
        IReadOnlyList<StructuralProductionLotInputRequestDto> inputs,
        string plantId,
        string materialCode,
        IBatchRepository batches,
        IInventoryBalanceRepository balances,
        IInventoryPackageRepository packages,
        CancellationToken cancellationToken)
    {
        foreach (var req in inputs)
        {
            if (req.Quantity <= 0)
                return Result.Failure(Error.Validation("PRD-SPL-012", "Source quantity must be positive."));
            if (req.SourceBatchId is null && req.SourceInventoryBalanceId is null && req.SourcePackageId is null)
                return Result.Failure(Error.Validation("PRD-SPL-011", "Each source line needs Batch, Balance, or Package id."));

            if (req.SourceBatchId is Guid batchId)
            {
                var batch = await batches.GetByIdAsync(batchId, cancellationToken).ConfigureAwait(false);
                if (batch is null || batch.IsDeleted)
                    return Result.Failure(Error.Validation("PRD-SPL-013", $"Source batch {batchId} not found."));
                if (!string.IsNullOrWhiteSpace(batch.PlantId)
                    && !string.Equals(batch.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure(Error.Forbidden("PRD-SPL-403", "Kaynak batch başka tesise ait."));
                if (!string.Equals(batch.MaterialCode, materialCode, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure(Error.Validation("PRD-SPL-014", "Kaynak batch malzeme kodu Production Lot malzemesi ile uyuşmuyor."));
            }

            if (req.SourceInventoryBalanceId is Guid balId)
            {
                var bal = await balances.GetByIdAsync(balId, cancellationToken).ConfigureAwait(false);
                if (bal is null || bal.IsDeleted)
                    return Result.Failure(Error.Validation("PRD-SPL-013", $"Source balance {balId} not found."));
                if (!string.IsNullOrWhiteSpace(bal.PlantId)
                    && !string.Equals(bal.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure(Error.Forbidden("PRD-SPL-403", "Kaynak stok bakiyesi başka tesise ait."));
                if (!string.Equals(bal.MaterialCode, materialCode, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure(Error.Validation("PRD-SPL-014", "Kaynak bakiye malzeme kodu Production Lot malzemesi ile uyuşmuyor."));
            }

            if (req.SourcePackageId is Guid pkgId)
            {
                var pkg = await packages.GetByIdAsync(pkgId, cancellationToken).ConfigureAwait(false);
                if (pkg is null || pkg.IsDeleted)
                    return Result.Failure(Error.Validation("PRD-SPL-013", $"Source package {pkgId} not found."));
                if (!string.IsNullOrWhiteSpace(pkg.PlantId)
                    && !string.Equals(pkg.PlantId, plantId, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure(Error.Forbidden("PRD-SPL-403", "Kaynak paket başka tesise ait."));
                if (!string.Equals(pkg.MaterialCode, materialCode, StringComparison.OrdinalIgnoreCase))
                    return Result.Failure(Error.Validation("PRD-SPL-014", "Kaynak paket malzeme kodu Production Lot malzemesi ile uyuşmuyor."));
            }
        }
        return Result.Success();
    }
}

public sealed class UpdateStructuralProductionLotDraftCommandHandler
    : ICommandHandler<UpdateStructuralProductionLotDraftCommand, Result<StructuralProductionLotDto>>
{
    private readonly IStructuralProductionLotRepository _repo;
    private readonly IMaterialRepository _materials;
    private readonly IBatchRepository _batches;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IInventoryPackageRepository _packages;
    private readonly IWorkCenterRepository _workCenters;
    private readonly IBusinessUnitOfWork _uow;

    public UpdateStructuralProductionLotDraftCommandHandler(
        IStructuralProductionLotRepository repo,
        IMaterialRepository materials,
        IBatchRepository batches,
        IInventoryBalanceRepository balances,
        IInventoryPackageRepository packages,
        IWorkCenterRepository workCenters,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _materials = materials;
        _batches = batches;
        _balances = balances;
        _packages = packages;
        _workCenters = workCenters;
        _uow = uow;
    }

    public async Task<Result<StructuralProductionLotDto>> HandleAsync(
        UpdateStructuralProductionLotDraftCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<StructuralProductionLotDto>(Error.NotFound("BUS-001", "StructuralProductionLot was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<StructuralProductionLotDto>(Error.Forbidden(
                "PRD-SPL-403", "Bu yapısal üretim lotunu değiştirme yetkiniz yok."));
        if (!e.IsDraft)
            return Result.Failure<StructuralProductionLotDto>(Error.Validation(
                "PRD-SPL-IMMUTABLE",
                "DRAFT dışındaki Production Lot’ta Plant/Material/yöntem/numara/kaynak değiştirilemez. İptal + yeni lot kullanın."));

        var plantId = e.PlantId!.Trim();
        string? workCenterCode = null;
        if (!string.IsNullOrWhiteSpace(command.WorkCenterCode))
        {
            var wc = await _workCenters.GetByCodeAndPlantAsync(command.WorkCenterCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
            workCenterCode = wc?.Code ?? command.WorkCenterCode.Trim();
        }

        var material = await _materials.GetByIdAsync(e.MaterialId, cancellationToken).ConfigureAwait(false);
        var materialCode = material?.Code ?? e.MaterialCode;
        var inputsReq = command.Inputs ?? Array.Empty<StructuralProductionLotInputRequestDto>();
        var sourceCheck = await StructuralProductionLotSourceGuard.ValidateAsync(
            inputsReq, plantId, materialCode, _batches, _balances, _packages, cancellationToken).ConfigureAwait(false);
        if (sourceCheck.IsFailure)
            return Result.Failure<StructuralProductionLotDto>(sourceCheck.Error!);

        try
        {
            e.ReplaceDraftDetails(workCenterCode, command.ProductionDate, command.ShiftCode, command.Notes);
            var inputs = inputsReq.Select(req => StructuralProductionLotInput.Create(
                e.Id, req.Quantity, req.Unit, req.SourceBatchId, req.SourceInventoryBalanceId, req.SourcePackageId)).ToList();
            e.ReplaceDraftInputs(inputs);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<StructuralProductionLotDto>(Error.Validation("PRD-SPL-IMMUTABLE", ex.Message));
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StructuralProductionLotMapper.ToDto(e));
    }
}

public sealed class TransitionStructuralProductionLotStatusCommandHandler
    : ICommandHandler<TransitionStructuralProductionLotStatusCommand, Result<StructuralProductionLotDto>>
{
    private readonly IStructuralProductionLotRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public TransitionStructuralProductionLotStatusCommandHandler(
        IStructuralProductionLotRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        "DRAFT", "IN_PROGRESS", "PENDING_CLASSIFICATION", "PENDING_QUALITY", "RELEASED", "QUARANTINED", "CANCELLED"
    };

    public async Task<Result<StructuralProductionLotDto>> HandleAsync(
        TransitionStructuralProductionLotStatusCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<StructuralProductionLotDto>(Error.NotFound("BUS-001", "StructuralProductionLot was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<StructuralProductionLotDto>(Error.Forbidden(
                "PRD-SPL-403", "Bu yapısal üretim lotunu güncelleme yetkiniz yok."));

        var status = (command.Status ?? string.Empty).Trim().ToUpperInvariant();
        if (!Allowed.Contains(status) || status == "CANCELLED")
            return Result.Failure<StructuralProductionLotDto>(Error.Validation(
                "PRD-SPL-STATUS", "Use Cancel endpoint for CANCELLED; status must be a valid production-lot state."));

        try
        {
            if (status == "RELEASED")
                e.MarkReleased(command.Actor);
            else
                e.TransitionStatus(status);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<StructuralProductionLotDto>(Error.Validation("PRD-SPL-STATUS", ex.Message));
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StructuralProductionLotMapper.ToDto(e));
    }
}

public sealed class CancelStructuralProductionLotCommandHandler
    : ICommandHandler<CancelStructuralProductionLotCommand, Result<StructuralProductionLotDto>>
{
    private readonly IStructuralProductionLotRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public CancelStructuralProductionLotCommandHandler(
        IStructuralProductionLotRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<StructuralProductionLotDto>> HandleAsync(
        CancelStructuralProductionLotCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<StructuralProductionLotDto>(Error.NotFound("BUS-001", "StructuralProductionLot was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<StructuralProductionLotDto>(Error.Forbidden(
                "PRD-SPL-403", "Bu yapısal üretim lotunu iptal etme yetkiniz yok."));
        if (string.IsNullOrWhiteSpace(command.Reason))
            return Result.Failure<StructuralProductionLotDto>(Error.Validation(
                "PRD-SPL-CANCEL", "CancellationReason is required."));

        try
        {
            e.Cancel(command.Reason, command.CancelledBy);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<StructuralProductionLotDto>(Error.Validation("PRD-SPL-CANCEL", ex.Message));
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(StructuralProductionLotMapper.ToDto(e));
    }
}

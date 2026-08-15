using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IMaterialIdentityRepository
{
    Task AddAsync(MaterialIdentity entity, CancellationToken cancellationToken = default);
    Task<MaterialIdentity?> GetByNumberAsync(string identityNumber, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<MaterialIdentity> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public interface IInventoryPackageRepository
{
    Task AddAsync(InventoryPackage entity, CancellationToken cancellationToken = default);
    Task<InventoryPackage?> GetByNumberAsync(string packageNumber, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<InventoryPackage> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public interface IInventoryMovementRepository
{
    Task AddAsync(InventoryMovement entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<InventoryMovement> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record ExecuteGoodsReceiptCommand(
    string Number,
    string WarehouseCode,
    string Reference,
    string Notes,
    bool QuantityVerified,
    string ExtractSource,
    IReadOnlyList<StockPostLineRequestDto> Lines) : ICommand<Result<ExecuteStockDocumentResultDto>>;

public sealed record ExecuteGoodsIssueCommand(
    string Number,
    string WarehouseCode,
    string Reference,
    string Notes,
    IReadOnlyList<StockPostLineRequestDto> Lines) : ICommand<Result<ExecuteStockDocumentResultDto>>;

public sealed record SearchInventoryPackageQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedInventoryPackageDto>>;
public sealed record SearchMaterialIdentityQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedMaterialIdentityDto>>;
public sealed record SearchInventoryMovementQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedInventoryMovementDto>>;

public sealed class ExecuteGoodsReceiptCommandHandler : ICommandHandler<ExecuteGoodsReceiptCommand, Result<ExecuteStockDocumentResultDto>>
{
    private readonly IGoodsReceiptRepository _receipts;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IBatchRepository _batches;
    private readonly IMaterialRepository _materials;
    private readonly IMaterialIdentityRepository _identities;
    private readonly IInventoryPackageRepository _packages;
    private readonly IInventoryMovementRepository _movements;
    private readonly IBusinessUnitOfWork _uow;

    public ExecuteGoodsReceiptCommandHandler(
        IGoodsReceiptRepository receipts,
        IInventoryBalanceRepository balances,
        IBatchRepository batches,
        IMaterialRepository materials,
        IMaterialIdentityRepository identities,
        IInventoryPackageRepository packages,
        IInventoryMovementRepository movements,
        IBusinessUnitOfWork uow)
    {
        _receipts = receipts;
        _balances = balances;
        _batches = batches;
        _materials = materials;
        _identities = identities;
        _packages = packages;
        _movements = movements;
        _uow = uow;
    }

    public async Task<Result<ExecuteStockDocumentResultDto>> HandleAsync(ExecuteGoodsReceiptCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Lines is null || command.Lines.Count == 0)
            return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-001", "At least one receipt line is required."));
        if (string.IsNullOrWhiteSpace(command.WarehouseCode))
            return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-002", "WarehouseCode is required."));
        if (!command.QuantityVerified)
            return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation(
                "INV-POST-010",
                "Quantity must be verified before posting a goods receipt to stock."));
        if (string.Equals(command.ExtractSource?.Trim(), "demo", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation(
                "INV-POST-012",
                "Demo OCR extract cannot be posted as a real goods receipt."));

        var receipt = GoodsReceipt.Create(
            SystemIdentifier.Ensure(command.Number, "GR"),
            command.WarehouseCode.Trim(),
            command.Reference ?? string.Empty,
            "Posted",
            Truncate(command.Notes, 2000));
        receipt.MarkPosted();
        await _receipts.AddAsync(receipt, cancellationToken).ConfigureAwait(false);

        var results = new List<StockPostLineResultDto>();
        var lineNo = 0;
        foreach (var line in command.Lines)
        {
            lineNo++;
            if (string.IsNullOrWhiteSpace(line.MaterialCode))
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-003", $"Line {lineNo}: MaterialCode is required."));
            if (line.Quantity <= 0)
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-004", $"Line {lineNo}: Quantity must be positive."));

            var materialCode = line.MaterialCode.Trim();
            if (string.Equals(materialCode, "MAT-UNKNOWN", StringComparison.OrdinalIgnoreCase))
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation(
                    "INV-POST-011",
                    "Geçerli bir malzeme kartı seçilmeden mal kabul stok girişi yapılamaz."));

            var material = await _materials.GetByCodeAsync(materialCode, cancellationToken).ConfigureAwait(false);
            if (material is null)
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation(
                    "INV-POST-011",
                    "Geçerli bir malzeme kartı seçilmeden mal kabul stok girişi yapılamaz."));
            if (!string.IsNullOrWhiteSpace(line.MaterialId)
                && Guid.TryParse(line.MaterialId, out var materialId)
                && material.Id != materialId)
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation(
                    "INV-POST-014",
                    "Geçerli bir malzeme kartı seçilmeden mal kabul stok girişi yapılamaz. (materialId/code uyuşmazlığı)"));
            if (!string.IsNullOrWhiteSpace(material.Status)
                && !string.Equals(material.Status, "Active", StringComparison.OrdinalIgnoreCase))
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation(
                    "INV-POST-013",
                    $"Line {lineNo}: Material '{materialCode}' is not Active (status: {material.Status})."));

            // Prefer canonical master code casing.
            materialCode = material.Code;
            var locationCode = string.IsNullOrWhiteSpace(line.LocationCode) ? "RECV" : line.LocationCode.Trim();
            var lotNumber = string.IsNullOrWhiteSpace(line.LotNumber)
                ? SystemIdentifier.Ensure(null, "LOT")
                : line.LotNumber.Trim();
            var uom = string.IsNullOrWhiteSpace(line.UnitOfMeasure) ? "Piece" : line.UnitOfMeasure.Trim();
            var miNumber = string.IsNullOrWhiteSpace(line.MaterialIdentityNumber)
                ? SystemIdentifier.Ensure(null, "MI")
                : line.MaterialIdentityNumber.Trim();
            var packageNumber = string.IsNullOrWhiteSpace(line.PackageNumber)
                ? SystemIdentifier.Ensure(null, "PKG")
                : line.PackageNumber.Trim();

            var identity = MaterialIdentity.CreateRoot(
                miNumber, materialCode, lotNumber, receipt.WarehouseCode, locationCode, line.Quantity, uom, receipt.Number);
            await _identities.AddAsync(identity, cancellationToken).ConfigureAwait(false);

            var package = InventoryPackage.Create(
                packageNumber, miNumber, materialCode, lotNumber, receipt.WarehouseCode, locationCode, line.Quantity, uom, line.Barcode);
            await _packages.AddAsync(package, cancellationToken).ConfigureAwait(false);

            var batch = Batch.Create(lotNumber, materialCode, line.Quantity, null, "Active");
            await _batches.AddAsync(batch, cancellationToken).ConfigureAwait(false);

            var balance = await _balances.FindByKeyAsync(materialCode, receipt.WarehouseCode, locationCode, lotNumber, cancellationToken).ConfigureAwait(false);
            if (balance is null)
            {
                balance = InventoryBalance.Create(materialCode, receipt.WarehouseCode, locationCode, lotNumber, line.Quantity, 0, "Active");
                await _balances.AddAsync(balance, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                balance.ApplyReceipt(line.Quantity);
            }

            var movement = InventoryMovement.Post(
                "GoodsReceipt", "In", receipt.Number, materialCode, miNumber, packageNumber,
                receipt.WarehouseCode, locationCode, lotNumber, line.Quantity, uom, command.Notes ?? string.Empty);
            await _movements.AddAsync(movement, cancellationToken).ConfigureAwait(false);

            results.Add(new StockPostLineResultDto
            {
                MaterialCode = materialCode,
                MaterialIdentityNumber = miNumber,
                PackageNumber = packageNumber,
                LotNumber = lotNumber,
                MovementNumber = movement.MovementNumber,
                Quantity = line.Quantity
            });
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(new ExecuteStockDocumentResultDto
        {
            DocumentId = receipt.Id,
            DocumentNumber = receipt.Number,
            Status = receipt.Status,
            Lines = results
        });
    }

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= max ? value : value[..max];
    }
}

public sealed class ExecuteGoodsIssueCommandHandler : ICommandHandler<ExecuteGoodsIssueCommand, Result<ExecuteStockDocumentResultDto>>
{
    private readonly IGoodsIssueRepository _issues;
    private readonly IInventoryBalanceRepository _balances;
    private readonly IMaterialIdentityRepository _identities;
    private readonly IInventoryPackageRepository _packages;
    private readonly IInventoryMovementRepository _movements;
    private readonly IBusinessUnitOfWork _uow;

    public ExecuteGoodsIssueCommandHandler(
        IGoodsIssueRepository issues,
        IInventoryBalanceRepository balances,
        IMaterialIdentityRepository identities,
        IInventoryPackageRepository packages,
        IInventoryMovementRepository movements,
        IBusinessUnitOfWork uow)
    {
        _issues = issues;
        _balances = balances;
        _identities = identities;
        _packages = packages;
        _movements = movements;
        _uow = uow;
    }

    public async Task<Result<ExecuteStockDocumentResultDto>> HandleAsync(ExecuteGoodsIssueCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Lines is null || command.Lines.Count == 0)
            return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-001", "At least one issue line is required."));
        if (string.IsNullOrWhiteSpace(command.WarehouseCode))
            return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-002", "WarehouseCode is required."));

        var issue = GoodsIssue.Create(
            SystemIdentifier.Ensure(command.Number, "GI"),
            command.WarehouseCode.Trim(),
            command.Reference ?? string.Empty,
            "Posted",
            Truncate(command.Notes, 2000));
        issue.MarkPosted();
        await _issues.AddAsync(issue, cancellationToken).ConfigureAwait(false);

        var results = new List<StockPostLineResultDto>();
        var lineNo = 0;
        foreach (var line in command.Lines)
        {
            lineNo++;
            if (line.Quantity <= 0)
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-004", $"Line {lineNo}: Quantity must be positive."));

            InventoryPackage? package = null;
            if (!string.IsNullOrWhiteSpace(line.PackageNumber) || !string.IsNullOrWhiteSpace(line.Barcode))
            {
                var key = !string.IsNullOrWhiteSpace(line.PackageNumber) ? line.PackageNumber.Trim() : line.Barcode.Trim();
                package = await _packages.GetByNumberAsync(key, cancellationToken).ConfigureAwait(false);
            }

            var materialCode = package?.MaterialCode ?? line.MaterialCode.Trim();
            if (string.IsNullOrWhiteSpace(materialCode))
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-003", $"Line {lineNo}: MaterialCode or PackageNumber is required."));

            var locationCode = package?.LocationCode
                ?? (string.IsNullOrWhiteSpace(line.LocationCode) ? "PICK" : line.LocationCode.Trim());
            var lotNumber = package?.LotNumber
                ?? (string.IsNullOrWhiteSpace(line.LotNumber) ? string.Empty : line.LotNumber.Trim());
            var miNumber = package?.MaterialIdentityNumber
                ?? (string.IsNullOrWhiteSpace(line.MaterialIdentityNumber) ? string.Empty : line.MaterialIdentityNumber.Trim());
            var packageNumber = package?.PackageNumber
                ?? (!string.IsNullOrWhiteSpace(line.PackageNumber) ? line.PackageNumber.Trim() : string.Empty);
            var uom = package?.UnitOfMeasure
                ?? (string.IsNullOrWhiteSpace(line.UnitOfMeasure) ? "Piece" : line.UnitOfMeasure.Trim());
            var warehouseCode = package?.WarehouseCode ?? issue.WarehouseCode;

            try
            {
                if (package is not null) package.Issue(line.Quantity);

                if (!string.IsNullOrWhiteSpace(miNumber))
                {
                    var identity = await _identities.GetByNumberAsync(miNumber, cancellationToken).ConfigureAwait(false);
                    identity?.Reduce(line.Quantity);
                }

                var balance = await _balances.FindByKeyAsync(materialCode, warehouseCode, locationCode, lotNumber, cancellationToken).ConfigureAwait(false);
                if (balance is null)
                    return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-011", $"No stock balance for {materialCode}/{warehouseCode}/{locationCode}/{lotNumber}. Receive stock first."));
                balance.ApplyIssue(line.Quantity);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<ExecuteStockDocumentResultDto>(Error.Validation("INV-POST-012", ex.Message));
            }

            var movement = InventoryMovement.Post(
                "GoodsIssue", "Out", issue.Number, materialCode, miNumber, packageNumber,
                warehouseCode, locationCode, lotNumber, line.Quantity, uom, command.Notes ?? string.Empty);
            await _movements.AddAsync(movement, cancellationToken).ConfigureAwait(false);

            results.Add(new StockPostLineResultDto
            {
                MaterialCode = materialCode,
                MaterialIdentityNumber = miNumber,
                PackageNumber = packageNumber,
                LotNumber = lotNumber,
                MovementNumber = movement.MovementNumber,
                Quantity = line.Quantity
            });
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(new ExecuteStockDocumentResultDto
        {
            DocumentId = issue.Id,
            DocumentNumber = issue.Number,
            Status = issue.Status,
            Lines = results
        });
    }

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= max ? value : value[..max];
    }
}

public sealed class SearchInventoryPackageQueryHandler : IQueryHandler<SearchInventoryPackageQuery, Result<PagedInventoryPackageDto>>
{
    private readonly IInventoryPackageRepository _repo;
    public SearchInventoryPackageQueryHandler(IInventoryPackageRepository repo) => _repo = repo;

    public async Task<Result<PagedInventoryPackageDto>> HandleAsync(SearchInventoryPackageQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedInventoryPackageDto
        {
            Items = items.Select(e => new InventoryPackageDto
            {
                Id = e.Id,
                PackageNumber = e.PackageNumber,
                MaterialIdentityNumber = e.MaterialIdentityNumber,
                MaterialCode = e.MaterialCode,
                LotNumber = e.LotNumber,
                WarehouseCode = e.WarehouseCode,
                LocationCode = e.LocationCode,
                Quantity = e.Quantity,
                UnitOfMeasure = e.UnitOfMeasure,
                Barcode = e.Barcode,
                Status = e.Status,
                CreatedAt = e.CreatedAt
            }).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class SearchMaterialIdentityQueryHandler : IQueryHandler<SearchMaterialIdentityQuery, Result<PagedMaterialIdentityDto>>
{
    private readonly IMaterialIdentityRepository _repo;
    public SearchMaterialIdentityQueryHandler(IMaterialIdentityRepository repo) => _repo = repo;

    public async Task<Result<PagedMaterialIdentityDto>> HandleAsync(SearchMaterialIdentityQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedMaterialIdentityDto
        {
            Items = items.Select(e => new MaterialIdentityDto
            {
                Id = e.Id,
                IdentityNumber = e.IdentityNumber,
                MaterialCode = e.MaterialCode,
                LotNumber = e.LotNumber,
                WarehouseCode = e.WarehouseCode,
                LocationCode = e.LocationCode,
                Quantity = e.Quantity,
                UnitOfMeasure = e.UnitOfMeasure,
                Status = e.Status,
                RootGoodsReceiptNumber = e.RootGoodsReceiptNumber,
                CreatedAt = e.CreatedAt
            }).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class SearchInventoryMovementQueryHandler : IQueryHandler<SearchInventoryMovementQuery, Result<PagedInventoryMovementDto>>
{
    private readonly IInventoryMovementRepository _repo;
    public SearchInventoryMovementQueryHandler(IInventoryMovementRepository repo) => _repo = repo;

    public async Task<Result<PagedInventoryMovementDto>> HandleAsync(SearchInventoryMovementQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedInventoryMovementDto
        {
            Items = items.Select(e => new InventoryMovementDto
            {
                Id = e.Id,
                MovementNumber = e.MovementNumber,
                MovementType = e.MovementType,
                Direction = e.Direction,
                DocumentNumber = e.DocumentNumber,
                MaterialCode = e.MaterialCode,
                PackageNumber = e.PackageNumber,
                Quantity = e.Quantity,
                WarehouseCode = e.WarehouseCode,
                Status = e.Status,
                CreatedAt = e.CreatedAt
            }).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Warehouse> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchWarehouseQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedWarehouseDto>>;
public sealed record GetWarehouseByIdQuery(Guid Id) : IQuery<Result<WarehouseDto>>;
public sealed record CreateWarehouseCommand(string Code, string Name, string WarehouseType, string Status, string? PlantId, string Description = "") : ICommand<Result<WarehouseDto>>;
public sealed record UpdateWarehouseCommand(Guid Id, string Code, string Name, string WarehouseType, string Status, string? PlantId, string? Description = null) : ICommand<Result<WarehouseDto>>;
public sealed record DeleteWarehouseCommand(Guid Id) : ICommand<Result>;

/// <summary>Supported WarehouseType codes — production + technical/ops. Material is never bound to a warehouse.</summary>
public static class WarehouseTypes
{
    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        "RAW_MATERIAL",
        "SEMI_FINISHED",
        "FINISHED_GOODS",
        "QUARANTINE",
        "SCRAP",
        "LOG_YARD",
        "HARDWARE",
        "ELECTRICAL",
        "MECHANICAL",
        "MAINTENANCE",
        "CHEMICAL",
        "PACKAGING",
        "PPE",
        "GENERAL_CONSUMABLE",
    };

    public static string Normalize(string? type)
    {
        var t = (type ?? string.Empty).Trim().ToUpperInvariant();
        return string.IsNullOrEmpty(t) ? "RAW_MATERIAL" : t;
    }

    public static bool IsKnown(string? type) => All.Contains(Normalize(type));
}

public static class WarehouseMapper
{
    public static WarehouseDto ToDto(Warehouse e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Name = e.Name,
        WarehouseType = e.WarehouseType,
        Description = e.Description,
        Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchWarehouseQueryHandler : IQueryHandler<SearchWarehouseQuery, Result<PagedWarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    public SearchWarehouseQueryHandler(IWarehouseRepository repo) => _repo = repo;
    public async Task<Result<PagedWarehouseDto>> HandleAsync(SearchWarehouseQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedWarehouseDto
        {
            Items = items.Select(WarehouseMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetWarehouseByIdQueryHandler : IQueryHandler<GetWarehouseByIdQuery, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    public GetWarehouseByIdQueryHandler(IWarehouseRepository repo) => _repo = repo;
    public async Task<Result<WarehouseDto>> HandleAsync(GetWarehouseByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WarehouseDto>(Error.NotFound("BUS-001", "Warehouse was not found."));
        return Result.Success(WarehouseMapper.ToDto(e));
    }
}

public sealed class CreateWarehouseCommandHandler : ICommandHandler<CreateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateWarehouseCommandHandler(IWarehouseRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WarehouseDto>> HandleAsync(CreateWarehouseCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Failure<WarehouseDto>(Error.Validation("BUS-WH-001", "Warehouse name is required."));

        var warehouseType = WarehouseTypes.Normalize(command.WarehouseType);
        if (!WarehouseTypes.IsKnown(warehouseType))
            return Result.Failure<WarehouseDto>(Error.Validation("BUS-WH-002", $"Unknown WarehouseType '{command.WarehouseType}'."));

        var code = SystemIdentifier.Ensure(command.Code, "WH");
        var existing = await _repo.GetByCodeAsync(code, cancellationToken).ConfigureAwait(false);
        if (existing is not null && !existing.IsDeleted)
            return Result.Failure<WarehouseDto>(Error.Conflict("BUS-WH-003", "Bu depo kodu zaten kullanılıyor."));

        var status = string.IsNullOrWhiteSpace(command.Status) ? "Active" : command.Status.Trim();
        var e = Warehouse.Create(code, command.Name.Trim(), warehouseType, status, plantId: command.PlantId, description: command.Description ?? string.Empty);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WarehouseMapper.ToDto(e));
    }
}

public sealed class UpdateWarehouseCommandHandler : ICommandHandler<UpdateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IWarehouseRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateWarehouseCommandHandler(IWarehouseRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WarehouseDto>> HandleAsync(UpdateWarehouseCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WarehouseDto>(Error.NotFound("BUS-001", "Warehouse was not found."));

        var warehouseType = WarehouseTypes.Normalize(command.WarehouseType);
        if (!WarehouseTypes.IsKnown(warehouseType))
            return Result.Failure<WarehouseDto>(Error.Validation("BUS-WH-002", $"Unknown WarehouseType '{command.WarehouseType}'."));

        e.Update(command.Code, command.Name, warehouseType, command.Status, command.PlantId, command.Description);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WarehouseMapper.ToDto(e));
    }
}

public sealed class DeleteWarehouseCommandHandler : ICommandHandler<DeleteWarehouseCommand, Result>
{
    private readonly IWarehouseRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteWarehouseCommandHandler(IWarehouseRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteWarehouseCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Warehouse was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

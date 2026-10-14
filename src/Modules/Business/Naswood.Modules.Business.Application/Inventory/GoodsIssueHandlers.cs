using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IGoodsIssueRepository
{
    Task<GoodsIssue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(GoodsIssue entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<GoodsIssue> Items, int Total)> SearchAsync(string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default);
    Task<int> CountOpenAsync(string plantId, CancellationToken cancellationToken = default);
}

public sealed record SearchGoodsIssueQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedGoodsIssueDto>>;

public sealed record GetGoodsIssueByIdQuery(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<GoodsIssueDto>>;

public sealed record CreateGoodsIssueCommand(
    string Number,
    string WarehouseCode,
    string Reference,
    string Status,
    string Notes,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<GoodsIssueDto>>;

public sealed record UpdateGoodsIssueCommand(
    Guid Id,
    string Number,
    string WarehouseCode,
    string Reference,
    string Status,
    string Notes,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<GoodsIssueDto>>;

public sealed record DeleteGoodsIssueCommand(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result>;

public static class GoodsIssueMapper
{
    public static GoodsIssueDto ToDto(GoodsIssue e) => new()
    {
        Id = e.Id,
        Number = e.Number,
        WarehouseCode = e.WarehouseCode,
        Reference = e.Reference,
        Status = e.Status,
        Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchGoodsIssueQueryHandler : IQueryHandler<SearchGoodsIssueQuery, Result<PagedGoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    public SearchGoodsIssueQueryHandler(IGoodsIssueRepository repo) => _repo = repo;

    public async Task<Result<PagedGoodsIssueDto>> HandleAsync(SearchGoodsIssueQuery query, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(query.PlantId) ? null : query.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<PagedGoodsIssueDto>(Error.Validation(
                "INV-GI-004",
                "Plant / factory context is required."));

        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, plantId))
            return Result.Failure<PagedGoodsIssueDto>(Error.Forbidden(
                "INV-GI-403",
                "Bu tesisin mal çıkış belgelerini görüntüleme yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, plantId, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedGoodsIssueDto
        {
            Items = items.Select(GoodsIssueMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetGoodsIssueByIdQueryHandler : IQueryHandler<GetGoodsIssueByIdQuery, Result<GoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    public GetGoodsIssueByIdQueryHandler(IGoodsIssueRepository repo) => _repo = repo;

    public async Task<Result<GoodsIssueDto>> HandleAsync(GetGoodsIssueByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<GoodsIssueDto>(Error.NotFound("BUS-001", "GoodsIssue was not found."));

        if (query.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<GoodsIssueDto>(Error.Forbidden(
                "INV-GI-403",
                "Bu mal çıkış belgesini görüntüleme yetkiniz yok."));

        return Result.Success(GoodsIssueMapper.ToDto(e));
    }
}

public sealed class CreateGoodsIssueCommandHandler : ICommandHandler<CreateGoodsIssueCommand, Result<GoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public CreateGoodsIssueCommandHandler(
        IGoodsIssueRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<GoodsIssueDto>> HandleAsync(CreateGoodsIssueCommand command, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(command.PlantId) ? null : command.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<GoodsIssueDto>(Error.Validation(
                "INV-GI-004",
                "Plant / factory context is required."));

        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<GoodsIssueDto>(Error.Forbidden(
                "INV-GI-403",
                "Bu tesise mal çıkış belgesi oluşturma yetkiniz yok."));

        if (string.IsNullOrWhiteSpace(command.WarehouseCode))
            return Result.Failure<GoodsIssueDto>(Error.Validation("INV-GI-015", "WarehouseCode is required."));

        var wh = await _warehouses.GetByCodeAndPlantAsync(command.WarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (wh is null)
            return Result.Failure<GoodsIssueDto>(Error.Validation(
                "INV-GI-015",
                "Warehouse must belong to the resolved plant."));

        if (!string.Equals(wh.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<GoodsIssueDto>(Error.Validation("INV-GI-016", "Warehouse must be Active."));

        var e = GoodsIssue.Create(
            command.Number,
            command.WarehouseCode.Trim(),
            command.Reference,
            command.Status,
            command.Notes,
            plantId: plantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsIssueMapper.ToDto(e));
    }
}

public sealed class UpdateGoodsIssueCommandHandler : ICommandHandler<UpdateGoodsIssueCommand, Result<GoodsIssueDto>>
{
    private readonly IGoodsIssueRepository _repo;
    private readonly IWarehouseRepository _warehouses;
    private readonly IBusinessUnitOfWork _uow;

    public UpdateGoodsIssueCommandHandler(
        IGoodsIssueRepository repo,
        IWarehouseRepository warehouses,
        IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _warehouses = warehouses;
        _uow = uow;
    }

    public async Task<Result<GoodsIssueDto>> HandleAsync(UpdateGoodsIssueCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<GoodsIssueDto>(Error.NotFound("BUS-001", "GoodsIssue was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<GoodsIssueDto>(Error.Forbidden(
                "INV-GI-403",
                "Bu mal çıkış belgesini değiştirme yetkiniz yok."));

        var plantId = string.IsNullOrWhiteSpace(e.PlantId) ? null : e.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<GoodsIssueDto>(Error.Validation(
                "INV-GI-004",
                "Plant / factory context is required."));

        var wh = await _warehouses.GetByCodeAndPlantAsync(command.WarehouseCode.Trim(), plantId, cancellationToken).ConfigureAwait(false);
        if (wh is null)
            return Result.Failure<GoodsIssueDto>(Error.Validation(
                "INV-GI-015",
                "Warehouse must belong to the resolved plant."));

        e.Update(command.Number, command.WarehouseCode.Trim(), command.Reference, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(GoodsIssueMapper.ToDto(e));
    }
}

public sealed class DeleteGoodsIssueCommandHandler : ICommandHandler<DeleteGoodsIssueCommand, Result>
{
    private readonly IGoodsIssueRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public DeleteGoodsIssueCommandHandler(IGoodsIssueRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(DeleteGoodsIssueCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure(Error.NotFound("BUS-001", "GoodsIssue was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure(Error.Forbidden(
                "INV-GI-403",
                "Bu mal çıkış belgesini silme yetkiniz yok."));

        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

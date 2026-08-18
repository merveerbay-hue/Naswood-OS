using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IWorkCenterRepository
{
    Task<WorkCenter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(WorkCenter entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<WorkCenter> Items, int Total)> SearchAsync(
        string? q,
        string? plantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Work Center = production point (Kesim, Fırın, CNC…).
/// Not a Warehouse and not a Stock Location. WIP stock lives on LocationType=WIP.
/// </summary>
public sealed record SearchWorkCenterQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds = null) : IQuery<Result<PagedWorkCenterDto>>;

public sealed record GetWorkCenterByIdQuery(Guid Id, IReadOnlyList<string>? AllowedPlantIds = null) : IQuery<Result<WorkCenterDto>>;
public sealed record CreateWorkCenterCommand(
    string Code,
    string Name,
    decimal CapacityPerHour,
    string Status,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds = null) : ICommand<Result<WorkCenterDto>>;
public sealed record UpdateWorkCenterCommand(
    Guid Id,
    string Code,
    string Name,
    decimal CapacityPerHour,
    string Status,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds = null) : ICommand<Result<WorkCenterDto>>;
public sealed record DeleteWorkCenterCommand(Guid Id, IReadOnlyList<string>? AllowedPlantIds = null) : ICommand<Result>;

public static class WorkCenterMapper
{
    public static WorkCenterDto ToDto(WorkCenter e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Name = e.Name,
        CapacityPerHour = e.CapacityPerHour,
        Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchWorkCenterQueryHandler : IQueryHandler<SearchWorkCenterQuery, Result<PagedWorkCenterDto>>
{
    private readonly IWorkCenterRepository _repo;
    public SearchWorkCenterQueryHandler(IWorkCenterRepository repo) => _repo = repo;

    public async Task<Result<PagedWorkCenterDto>> HandleAsync(SearchWorkCenterQuery query, CancellationToken cancellationToken = default)
    {
        if (query.AllowedPlantIds is { Count: > 0 }
            && !string.IsNullOrWhiteSpace(query.PlantId)
            && !PlantAccess.CanAccess(query.AllowedPlantIds, query.PlantId))
            return Result.Failure<PagedWorkCenterDto>(Error.Forbidden("BUS-WC-403", "Bu tesise erişim yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, query.PlantId, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedWorkCenterDto
        {
            Items = items.Select(WorkCenterMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetWorkCenterByIdQueryHandler : IQueryHandler<GetWorkCenterByIdQuery, Result<WorkCenterDto>>
{
    private readonly IWorkCenterRepository _repo;
    public GetWorkCenterByIdQueryHandler(IWorkCenterRepository repo) => _repo = repo;

    public async Task<Result<WorkCenterDto>> HandleAsync(GetWorkCenterByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WorkCenterDto>(Error.NotFound("BUS-001", "WorkCenter was not found."));
        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<WorkCenterDto>(Error.Forbidden("BUS-WC-403", "Bu tesise erişim yetkiniz yok."));
        return Result.Success(WorkCenterMapper.ToDto(e));
    }
}

public sealed class CreateWorkCenterCommandHandler : ICommandHandler<CreateWorkCenterCommand, Result<WorkCenterDto>>
{
    private readonly IWorkCenterRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateWorkCenterCommandHandler(IWorkCenterRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<Result<WorkCenterDto>> HandleAsync(CreateWorkCenterCommand command, CancellationToken cancellationToken = default)
    {
        var plantId = PlantAccess.Normalize(command.PlantId);
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<WorkCenterDto>(Error.Forbidden("BUS-WC-403", "Bu tesiste iş merkezi oluşturma yetkiniz yok."));

        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Failure<WorkCenterDto>(Error.Validation("BUS-WC-001", "Work center name is required."));

        var status = string.IsNullOrWhiteSpace(command.Status) ? "Active" : command.Status.Trim();
        var e = WorkCenter.Create(
            SystemIdentifier.Ensure(command.Code, "WC"),
            command.Name.Trim(),
            command.CapacityPerHour,
            status,
            plantId: plantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WorkCenterMapper.ToDto(e));
    }
}

public sealed class UpdateWorkCenterCommandHandler : ICommandHandler<UpdateWorkCenterCommand, Result<WorkCenterDto>>
{
    private readonly IWorkCenterRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateWorkCenterCommandHandler(IWorkCenterRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<Result<WorkCenterDto>> HandleAsync(UpdateWorkCenterCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WorkCenterDto>(Error.NotFound("BUS-001", "WorkCenter was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<WorkCenterDto>(Error.Forbidden("BUS-WC-403", "Bu tesiste iş merkezi değiştirme yetkiniz yok."));

        e.Update(command.Code, command.Name, command.CapacityPerHour, command.Status, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WorkCenterMapper.ToDto(e));
    }
}

public sealed class DeleteWorkCenterCommandHandler : ICommandHandler<DeleteWorkCenterCommand, Result>
{
    private readonly IWorkCenterRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteWorkCenterCommandHandler(IWorkCenterRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<Result> HandleAsync(DeleteWorkCenterCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "WorkCenter was not found."));
        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure(Error.Forbidden("BUS-WC-403", "Bu tesiste iş merkezi silme yetkiniz yok."));

        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

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
    Task<(IReadOnlyList<WorkCenter> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchWorkCenterQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedWorkCenterDto>>;
public sealed record GetWorkCenterByIdQuery(Guid Id) : IQuery<Result<WorkCenterDto>>;
public sealed record CreateWorkCenterCommand(string Code, string Name, decimal CapacityPerHour, string Status, string? PlantId) : ICommand<Result<WorkCenterDto>>;
public sealed record UpdateWorkCenterCommand(Guid Id, string Code, string Name, decimal CapacityPerHour, string Status, string? PlantId) : ICommand<Result<WorkCenterDto>>;
public sealed record DeleteWorkCenterCommand(Guid Id) : ICommand<Result>;

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
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedWorkCenterDto
        {
            Items = items.Select(WorkCenterMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
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
        var e = WorkCenter.Create(SystemIdentifier.Ensure(command.Code, "WC"), command.Name, command.CapacityPerHour, command.Status, plantId: command.PlantId);
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
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

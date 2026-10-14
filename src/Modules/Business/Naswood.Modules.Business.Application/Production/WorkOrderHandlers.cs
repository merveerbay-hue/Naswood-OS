using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IWorkOrderRepository
{
    Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(WorkOrder entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<WorkOrder> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchWorkOrderQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedWorkOrderDto>>;
public sealed record GetWorkOrderByIdQuery(Guid Id) : IQuery<Result<WorkOrderDto>>;
public sealed record CreateWorkOrderCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<WorkOrderDto>>;
public sealed record UpdateWorkOrderCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<WorkOrderDto>>;
public sealed record DeleteWorkOrderCommand(Guid Id) : ICommand<Result>;

public static class WorkOrderMapper
{
    public static WorkOrderDto ToDto(WorkOrder e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Name = e.Name,
        Status = e.Status,
        Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchWorkOrderQueryHandler : IQueryHandler<SearchWorkOrderQuery, Result<PagedWorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;
    public SearchWorkOrderQueryHandler(IWorkOrderRepository repo) => _repo = repo;
    public async Task<Result<PagedWorkOrderDto>> HandleAsync(SearchWorkOrderQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedWorkOrderDto
        {
            Items = items.Select(WorkOrderMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetWorkOrderByIdQueryHandler : IQueryHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;
    public GetWorkOrderByIdQueryHandler(IWorkOrderRepository repo) => _repo = repo;
    public async Task<Result<WorkOrderDto>> HandleAsync(GetWorkOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WorkOrderDto>(Error.NotFound("BUS-001", "WorkOrder was not found."));
        return Result.Success(WorkOrderMapper.ToDto(e));
    }
}

public sealed class CreateWorkOrderCommandHandler : ICommandHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateWorkOrderCommandHandler(IWorkOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WorkOrderDto>> HandleAsync(CreateWorkOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = WorkOrder.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WorkOrderMapper.ToDto(e));
    }
}

public sealed class UpdateWorkOrderCommandHandler : ICommandHandler<UpdateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IWorkOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateWorkOrderCommandHandler(IWorkOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<WorkOrderDto>> HandleAsync(UpdateWorkOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<WorkOrderDto>(Error.NotFound("BUS-001", "WorkOrder was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(WorkOrderMapper.ToDto(e));
    }
}

public sealed class DeleteWorkOrderCommandHandler : ICommandHandler<DeleteWorkOrderCommand, Result>
{
    private readonly IWorkOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteWorkOrderCommandHandler(IWorkOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteWorkOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "WorkOrder was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

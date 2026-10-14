using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IMachineRepository
{
    Task<Machine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Machine entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Machine> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchMachineQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedMachineDto>>;
public sealed record GetMachineByIdQuery(Guid Id) : IQuery<Result<MachineDto>>;
public sealed record CreateMachineCommand(string Code, string Name, string WorkCenterCode, string Status, decimal OeeTarget) : ICommand<Result<MachineDto>>;
public sealed record UpdateMachineCommand(Guid Id, string Code, string Name, string WorkCenterCode, string Status, decimal OeeTarget) : ICommand<Result<MachineDto>>;
public sealed record DeleteMachineCommand(Guid Id) : ICommand<Result>;

public static class MachineMapper
{
    public static MachineDto ToDto(Machine e) => new()
    {
        Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            WorkCenterCode = e.WorkCenterCode,
            Status = e.Status,
            OeeTarget = e.OeeTarget,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchMachineQueryHandler : IQueryHandler<SearchMachineQuery, Result<PagedMachineDto>>
{
    private readonly IMachineRepository _repo;
    public SearchMachineQueryHandler(IMachineRepository repo) => _repo = repo;
    public async Task<Result<PagedMachineDto>> HandleAsync(SearchMachineQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedMachineDto
        {
            Items = items.Select(MachineMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetMachineByIdQueryHandler : IQueryHandler<GetMachineByIdQuery, Result<MachineDto>>
{
    private readonly IMachineRepository _repo;
    public GetMachineByIdQueryHandler(IMachineRepository repo) => _repo = repo;
    public async Task<Result<MachineDto>> HandleAsync(GetMachineByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<MachineDto>(Error.NotFound("BUS-001", "Machine was not found."));
        return Result.Success(MachineMapper.ToDto(e));
    }
}

public sealed class CreateMachineCommandHandler : ICommandHandler<CreateMachineCommand, Result<MachineDto>>
{
    private readonly IMachineRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateMachineCommandHandler(IMachineRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<MachineDto>> HandleAsync(CreateMachineCommand command, CancellationToken cancellationToken = default)
    {
        var e = Machine.Create(SystemIdentifier.Ensure(command.Code, "MC"), command.Name, command.WorkCenterCode, command.Status, command.OeeTarget);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(MachineMapper.ToDto(e));
    }
}

public sealed class UpdateMachineCommandHandler : ICommandHandler<UpdateMachineCommand, Result<MachineDto>>
{
    private readonly IMachineRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateMachineCommandHandler(IMachineRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<MachineDto>> HandleAsync(UpdateMachineCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<MachineDto>(Error.NotFound("BUS-001", "Machine was not found."));
        e.Update(command.Code, command.Name, command.WorkCenterCode, command.Status, command.OeeTarget);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(MachineMapper.ToDto(e));
    }
}

public sealed class DeleteMachineCommandHandler : ICommandHandler<DeleteMachineCommand, Result>
{
    private readonly IMachineRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteMachineCommandHandler(IMachineRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteMachineCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Machine was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

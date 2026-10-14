using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IOperationRepository
{
    Task<Operation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Operation entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Operation> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchOperationQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedOperationDto>>;
public sealed record GetOperationByIdQuery(Guid Id) : IQuery<Result<OperationDto>>;
public sealed record CreateOperationCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<OperationDto>>;
public sealed record UpdateOperationCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<OperationDto>>;
public sealed record DeleteOperationCommand(Guid Id) : ICommand<Result>;

public static class OperationMapper
{
    public static OperationDto ToDto(Operation e) => new()
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

public sealed class SearchOperationQueryHandler : IQueryHandler<SearchOperationQuery, Result<PagedOperationDto>>
{
    private readonly IOperationRepository _repo;
    public SearchOperationQueryHandler(IOperationRepository repo) => _repo = repo;
    public async Task<Result<PagedOperationDto>> HandleAsync(SearchOperationQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedOperationDto
        {
            Items = items.Select(OperationMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetOperationByIdQueryHandler : IQueryHandler<GetOperationByIdQuery, Result<OperationDto>>
{
    private readonly IOperationRepository _repo;
    public GetOperationByIdQueryHandler(IOperationRepository repo) => _repo = repo;
    public async Task<Result<OperationDto>> HandleAsync(GetOperationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<OperationDto>(Error.NotFound("BUS-001", "Operation was not found."));
        return Result.Success(OperationMapper.ToDto(e));
    }
}

public sealed class CreateOperationCommandHandler : ICommandHandler<CreateOperationCommand, Result<OperationDto>>
{
    private readonly IOperationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateOperationCommandHandler(IOperationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<OperationDto>> HandleAsync(CreateOperationCommand command, CancellationToken cancellationToken = default)
    {
        var e = Operation.Create(SystemIdentifier.Ensure(command.Code, "OP"), command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(OperationMapper.ToDto(e));
    }
}

public sealed class UpdateOperationCommandHandler : ICommandHandler<UpdateOperationCommand, Result<OperationDto>>
{
    private readonly IOperationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateOperationCommandHandler(IOperationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<OperationDto>> HandleAsync(UpdateOperationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<OperationDto>(Error.NotFound("BUS-001", "Operation was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(OperationMapper.ToDto(e));
    }
}

public sealed class DeleteOperationCommandHandler : ICommandHandler<DeleteOperationCommand, Result>
{
    private readonly IOperationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteOperationCommandHandler(IOperationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteOperationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Operation was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

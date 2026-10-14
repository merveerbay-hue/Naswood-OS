using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Shift entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Shift> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchShiftQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedShiftDto>>;
public sealed record GetShiftByIdQuery(Guid Id) : IQuery<Result<ShiftDto>>;
public sealed record CreateShiftCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ShiftDto>>;
public sealed record UpdateShiftCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ShiftDto>>;
public sealed record DeleteShiftCommand(Guid Id) : ICommand<Result>;

public static class ShiftMapper
{
    public static ShiftDto ToDto(Shift e) => new()
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

public sealed class SearchShiftQueryHandler : IQueryHandler<SearchShiftQuery, Result<PagedShiftDto>>
{
    private readonly IShiftRepository _repo;
    public SearchShiftQueryHandler(IShiftRepository repo) => _repo = repo;
    public async Task<Result<PagedShiftDto>> HandleAsync(SearchShiftQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedShiftDto
        {
            Items = items.Select(ShiftMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetShiftByIdQueryHandler : IQueryHandler<GetShiftByIdQuery, Result<ShiftDto>>
{
    private readonly IShiftRepository _repo;
    public GetShiftByIdQueryHandler(IShiftRepository repo) => _repo = repo;
    public async Task<Result<ShiftDto>> HandleAsync(GetShiftByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ShiftDto>(Error.NotFound("BUS-001", "Shift was not found."));
        return Result.Success(ShiftMapper.ToDto(e));
    }
}

public sealed class CreateShiftCommandHandler : ICommandHandler<CreateShiftCommand, Result<ShiftDto>>
{
    private readonly IShiftRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateShiftCommandHandler(IShiftRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ShiftDto>> HandleAsync(CreateShiftCommand command, CancellationToken cancellationToken = default)
    {
        var e = Shift.Create(SystemIdentifier.Ensure(command.Code, "SHIFT"), command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ShiftMapper.ToDto(e));
    }
}

public sealed class UpdateShiftCommandHandler : ICommandHandler<UpdateShiftCommand, Result<ShiftDto>>
{
    private readonly IShiftRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateShiftCommandHandler(IShiftRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ShiftDto>> HandleAsync(UpdateShiftCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ShiftDto>(Error.NotFound("BUS-001", "Shift was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ShiftMapper.ToDto(e));
    }
}

public sealed class DeleteShiftCommandHandler : ICommandHandler<DeleteShiftCommand, Result>
{
    private readonly IShiftRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteShiftCommandHandler(IShiftRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteShiftCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Shift was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

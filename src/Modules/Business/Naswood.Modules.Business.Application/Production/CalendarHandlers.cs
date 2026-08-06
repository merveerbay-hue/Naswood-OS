using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface ICalendarRepository
{
    Task<Calendar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Calendar entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Calendar> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchCalendarQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedCalendarDto>>;
public sealed record GetCalendarByIdQuery(Guid Id) : IQuery<Result<CalendarDto>>;
public sealed record CreateCalendarCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<CalendarDto>>;
public sealed record UpdateCalendarCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<CalendarDto>>;
public sealed record DeleteCalendarCommand(Guid Id) : ICommand<Result>;

public static class CalendarMapper
{
    public static CalendarDto ToDto(Calendar e) => new()
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

public sealed class SearchCalendarQueryHandler : IQueryHandler<SearchCalendarQuery, Result<PagedCalendarDto>>
{
    private readonly ICalendarRepository _repo;
    public SearchCalendarQueryHandler(ICalendarRepository repo) => _repo = repo;
    public async Task<Result<PagedCalendarDto>> HandleAsync(SearchCalendarQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedCalendarDto
        {
            Items = items.Select(CalendarMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetCalendarByIdQueryHandler : IQueryHandler<GetCalendarByIdQuery, Result<CalendarDto>>
{
    private readonly ICalendarRepository _repo;
    public GetCalendarByIdQueryHandler(ICalendarRepository repo) => _repo = repo;
    public async Task<Result<CalendarDto>> HandleAsync(GetCalendarByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<CalendarDto>(Error.NotFound("BUS-001", "Calendar was not found."));
        return Result.Success(CalendarMapper.ToDto(e));
    }
}

public sealed class CreateCalendarCommandHandler : ICommandHandler<CreateCalendarCommand, Result<CalendarDto>>
{
    private readonly ICalendarRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateCalendarCommandHandler(ICalendarRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<CalendarDto>> HandleAsync(CreateCalendarCommand command, CancellationToken cancellationToken = default)
    {
        var e = Calendar.Create(SystemIdentifier.Ensure(command.Code, "CAL"), command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(CalendarMapper.ToDto(e));
    }
}

public sealed class UpdateCalendarCommandHandler : ICommandHandler<UpdateCalendarCommand, Result<CalendarDto>>
{
    private readonly ICalendarRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateCalendarCommandHandler(ICalendarRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<CalendarDto>> HandleAsync(UpdateCalendarCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<CalendarDto>(Error.NotFound("BUS-001", "Calendar was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(CalendarMapper.ToDto(e));
    }
}

public sealed class DeleteCalendarCommandHandler : ICommandHandler<DeleteCalendarCommand, Result>
{
    private readonly ICalendarRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteCalendarCommandHandler(ICalendarRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteCalendarCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Calendar was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IFinishedGoodRepository
{
    Task<FinishedGood?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(FinishedGood entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<FinishedGood> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchFinishedGoodQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedFinishedGoodDto>>;
public sealed record GetFinishedGoodByIdQuery(Guid Id) : IQuery<Result<FinishedGoodDto>>;
public sealed record CreateFinishedGoodCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<FinishedGoodDto>>;
public sealed record UpdateFinishedGoodCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<FinishedGoodDto>>;
public sealed record DeleteFinishedGoodCommand(Guid Id) : ICommand<Result>;

public static class FinishedGoodMapper
{
    public static FinishedGoodDto ToDto(FinishedGood e) => new()
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

public sealed class SearchFinishedGoodQueryHandler : IQueryHandler<SearchFinishedGoodQuery, Result<PagedFinishedGoodDto>>
{
    private readonly IFinishedGoodRepository _repo;
    public SearchFinishedGoodQueryHandler(IFinishedGoodRepository repo) => _repo = repo;
    public async Task<Result<PagedFinishedGoodDto>> HandleAsync(SearchFinishedGoodQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedFinishedGoodDto
        {
            Items = items.Select(FinishedGoodMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetFinishedGoodByIdQueryHandler : IQueryHandler<GetFinishedGoodByIdQuery, Result<FinishedGoodDto>>
{
    private readonly IFinishedGoodRepository _repo;
    public GetFinishedGoodByIdQueryHandler(IFinishedGoodRepository repo) => _repo = repo;
    public async Task<Result<FinishedGoodDto>> HandleAsync(GetFinishedGoodByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<FinishedGoodDto>(Error.NotFound("BUS-001", "FinishedGood was not found."));
        return Result.Success(FinishedGoodMapper.ToDto(e));
    }
}

public sealed class CreateFinishedGoodCommandHandler : ICommandHandler<CreateFinishedGoodCommand, Result<FinishedGoodDto>>
{
    private readonly IFinishedGoodRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateFinishedGoodCommandHandler(IFinishedGoodRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<FinishedGoodDto>> HandleAsync(CreateFinishedGoodCommand command, CancellationToken cancellationToken = default)
    {
        var e = FinishedGood.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(FinishedGoodMapper.ToDto(e));
    }
}

public sealed class UpdateFinishedGoodCommandHandler : ICommandHandler<UpdateFinishedGoodCommand, Result<FinishedGoodDto>>
{
    private readonly IFinishedGoodRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateFinishedGoodCommandHandler(IFinishedGoodRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<FinishedGoodDto>> HandleAsync(UpdateFinishedGoodCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<FinishedGoodDto>(Error.NotFound("BUS-001", "FinishedGood was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(FinishedGoodMapper.ToDto(e));
    }
}

public sealed class DeleteFinishedGoodCommandHandler : ICommandHandler<DeleteFinishedGoodCommand, Result>
{
    private readonly IFinishedGoodRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteFinishedGoodCommandHandler(IFinishedGoodRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteFinishedGoodCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "FinishedGood was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IBomRepository
{
    Task<Bom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Bom entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Bom> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchBomQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedBomDto>>;
public sealed record GetBomByIdQuery(Guid Id) : IQuery<Result<BomDto>>;
public sealed record CreateBomCommand(string Number, string MaterialCode, int Version, string Status, string Notes) : ICommand<Result<BomDto>>;
public sealed record UpdateBomCommand(Guid Id, string Number, string MaterialCode, int Version, string Status, string Notes) : ICommand<Result<BomDto>>;
public sealed record DeleteBomCommand(Guid Id) : ICommand<Result>;

public static class BomMapper
{
    public static BomDto ToDto(Bom e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            MaterialCode = e.MaterialCode,
            Version = e.Version,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchBomQueryHandler : IQueryHandler<SearchBomQuery, Result<PagedBomDto>>
{
    private readonly IBomRepository _repo;
    public SearchBomQueryHandler(IBomRepository repo) => _repo = repo;
    public async Task<Result<PagedBomDto>> HandleAsync(SearchBomQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedBomDto
        {
            Items = items.Select(BomMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetBomByIdQueryHandler : IQueryHandler<GetBomByIdQuery, Result<BomDto>>
{
    private readonly IBomRepository _repo;
    public GetBomByIdQueryHandler(IBomRepository repo) => _repo = repo;
    public async Task<Result<BomDto>> HandleAsync(GetBomByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<BomDto>(Error.NotFound("BUS-001", "Bom was not found."));
        return Result.Success(BomMapper.ToDto(e));
    }
}

public sealed class CreateBomCommandHandler : ICommandHandler<CreateBomCommand, Result<BomDto>>
{
    private readonly IBomRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateBomCommandHandler(IBomRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<BomDto>> HandleAsync(CreateBomCommand command, CancellationToken cancellationToken = default)
    {
        var e = Bom.Create(command.Number, command.MaterialCode, command.Version, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(BomMapper.ToDto(e));
    }
}

public sealed class UpdateBomCommandHandler : ICommandHandler<UpdateBomCommand, Result<BomDto>>
{
    private readonly IBomRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateBomCommandHandler(IBomRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<BomDto>> HandleAsync(UpdateBomCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<BomDto>(Error.NotFound("BUS-001", "Bom was not found."));
        e.Update(command.Number, command.MaterialCode, command.Version, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(BomMapper.ToDto(e));
    }
}

public sealed class DeleteBomCommandHandler : ICommandHandler<DeleteBomCommand, Result>
{
    private readonly IBomRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteBomCommandHandler(IBomRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteBomCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Bom was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

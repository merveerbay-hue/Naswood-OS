using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IPackagingRepository
{
    Task<Packaging?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Packaging entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Packaging> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchPackagingQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedPackagingDto>>;
public sealed record GetPackagingByIdQuery(Guid Id) : IQuery<Result<PackagingDto>>;
public sealed record CreatePackagingCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<PackagingDto>>;
public sealed record UpdatePackagingCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<PackagingDto>>;
public sealed record DeletePackagingCommand(Guid Id) : ICommand<Result>;

public static class PackagingMapper
{
    public static PackagingDto ToDto(Packaging e) => new()
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

public sealed class SearchPackagingQueryHandler : IQueryHandler<SearchPackagingQuery, Result<PagedPackagingDto>>
{
    private readonly IPackagingRepository _repo;
    public SearchPackagingQueryHandler(IPackagingRepository repo) => _repo = repo;
    public async Task<Result<PagedPackagingDto>> HandleAsync(SearchPackagingQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedPackagingDto
        {
            Items = items.Select(PackagingMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetPackagingByIdQueryHandler : IQueryHandler<GetPackagingByIdQuery, Result<PackagingDto>>
{
    private readonly IPackagingRepository _repo;
    public GetPackagingByIdQueryHandler(IPackagingRepository repo) => _repo = repo;
    public async Task<Result<PackagingDto>> HandleAsync(GetPackagingByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PackagingDto>(Error.NotFound("BUS-001", "Packaging was not found."));
        return Result.Success(PackagingMapper.ToDto(e));
    }
}

public sealed class CreatePackagingCommandHandler : ICommandHandler<CreatePackagingCommand, Result<PackagingDto>>
{
    private readonly IPackagingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreatePackagingCommandHandler(IPackagingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PackagingDto>> HandleAsync(CreatePackagingCommand command, CancellationToken cancellationToken = default)
    {
        var e = Packaging.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PackagingMapper.ToDto(e));
    }
}

public sealed class UpdatePackagingCommandHandler : ICommandHandler<UpdatePackagingCommand, Result<PackagingDto>>
{
    private readonly IPackagingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdatePackagingCommandHandler(IPackagingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PackagingDto>> HandleAsync(UpdatePackagingCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PackagingDto>(Error.NotFound("BUS-001", "Packaging was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PackagingMapper.ToDto(e));
    }
}

public sealed class DeletePackagingCommandHandler : ICommandHandler<DeletePackagingCommand, Result>
{
    private readonly IPackagingRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeletePackagingCommandHandler(IPackagingRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeletePackagingCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Packaging was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

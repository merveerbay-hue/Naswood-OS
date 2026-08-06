using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IMaterialRepository
{
    Task<Material?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Material entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Material> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchMaterialQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedMaterialDto>>;
public sealed record GetMaterialByIdQuery(Guid Id) : IQuery<Result<MaterialDto>>;
public sealed record CreateMaterialCommand(string Code, string Name, string Description, string Category, string UnitOfMeasure, string Status) : ICommand<Result<MaterialDto>>;
public sealed record UpdateMaterialCommand(Guid Id, string Code, string Name, string Description, string Category, string UnitOfMeasure, string Status) : ICommand<Result<MaterialDto>>;
public sealed record DeleteMaterialCommand(Guid Id) : ICommand<Result>;

public static class MaterialMapper
{
    public static MaterialDto ToDto(Material e) => new()
    {
        Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            Description = e.Description,
            Category = e.Category,
            UnitOfMeasure = e.UnitOfMeasure,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchMaterialQueryHandler : IQueryHandler<SearchMaterialQuery, Result<PagedMaterialDto>>
{
    private readonly IMaterialRepository _repo;
    public SearchMaterialQueryHandler(IMaterialRepository repo) => _repo = repo;
    public async Task<Result<PagedMaterialDto>> HandleAsync(SearchMaterialQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedMaterialDto
        {
            Items = items.Select(MaterialMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetMaterialByIdQueryHandler : IQueryHandler<GetMaterialByIdQuery, Result<MaterialDto>>
{
    private readonly IMaterialRepository _repo;
    public GetMaterialByIdQueryHandler(IMaterialRepository repo) => _repo = repo;
    public async Task<Result<MaterialDto>> HandleAsync(GetMaterialByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<MaterialDto>(Error.NotFound("BUS-001", "Material was not found."));
        return Result.Success(MaterialMapper.ToDto(e));
    }
}

public sealed class CreateMaterialCommandHandler : ICommandHandler<CreateMaterialCommand, Result<MaterialDto>>
{
    private readonly IMaterialRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateMaterialCommandHandler(IMaterialRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<MaterialDto>> HandleAsync(CreateMaterialCommand command, CancellationToken cancellationToken = default)
    {
        var e = Material.Create(SystemIdentifier.Ensure(command.Code, "MAT"), command.Name, command.Description, command.Category, command.UnitOfMeasure, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(MaterialMapper.ToDto(e));
    }
}

public sealed class UpdateMaterialCommandHandler : ICommandHandler<UpdateMaterialCommand, Result<MaterialDto>>
{
    private readonly IMaterialRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateMaterialCommandHandler(IMaterialRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<MaterialDto>> HandleAsync(UpdateMaterialCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<MaterialDto>(Error.NotFound("BUS-001", "Material was not found."));
        e.Update(command.Code, command.Name, command.Description, command.Category, command.UnitOfMeasure, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(MaterialMapper.ToDto(e));
    }
}

public sealed class DeleteMaterialCommandHandler : ICommandHandler<DeleteMaterialCommand, Result>
{
    private readonly IMaterialRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteMaterialCommandHandler(IMaterialRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteMaterialCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Material was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

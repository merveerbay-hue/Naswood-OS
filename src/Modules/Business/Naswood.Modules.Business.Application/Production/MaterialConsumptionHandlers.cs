using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IMaterialConsumptionRepository
{
    Task<MaterialConsumption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(MaterialConsumption entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<MaterialConsumption> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchMaterialConsumptionQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedMaterialConsumptionDto>>;
public sealed record GetMaterialConsumptionByIdQuery(Guid Id) : IQuery<Result<MaterialConsumptionDto>>;
public sealed record CreateMaterialConsumptionCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<MaterialConsumptionDto>>;
public sealed record UpdateMaterialConsumptionCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<MaterialConsumptionDto>>;
public sealed record DeleteMaterialConsumptionCommand(Guid Id) : ICommand<Result>;

public static class MaterialConsumptionMapper
{
    public static MaterialConsumptionDto ToDto(MaterialConsumption e) => new()
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

public sealed class SearchMaterialConsumptionQueryHandler : IQueryHandler<SearchMaterialConsumptionQuery, Result<PagedMaterialConsumptionDto>>
{
    private readonly IMaterialConsumptionRepository _repo;
    public SearchMaterialConsumptionQueryHandler(IMaterialConsumptionRepository repo) => _repo = repo;
    public async Task<Result<PagedMaterialConsumptionDto>> HandleAsync(SearchMaterialConsumptionQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedMaterialConsumptionDto
        {
            Items = items.Select(MaterialConsumptionMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetMaterialConsumptionByIdQueryHandler : IQueryHandler<GetMaterialConsumptionByIdQuery, Result<MaterialConsumptionDto>>
{
    private readonly IMaterialConsumptionRepository _repo;
    public GetMaterialConsumptionByIdQueryHandler(IMaterialConsumptionRepository repo) => _repo = repo;
    public async Task<Result<MaterialConsumptionDto>> HandleAsync(GetMaterialConsumptionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<MaterialConsumptionDto>(Error.NotFound("BUS-001", "MaterialConsumption was not found."));
        return Result.Success(MaterialConsumptionMapper.ToDto(e));
    }
}

public sealed class CreateMaterialConsumptionCommandHandler : ICommandHandler<CreateMaterialConsumptionCommand, Result<MaterialConsumptionDto>>
{
    private readonly IMaterialConsumptionRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateMaterialConsumptionCommandHandler(IMaterialConsumptionRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<MaterialConsumptionDto>> HandleAsync(CreateMaterialConsumptionCommand command, CancellationToken cancellationToken = default)
    {
        var e = MaterialConsumption.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(MaterialConsumptionMapper.ToDto(e));
    }
}

public sealed class UpdateMaterialConsumptionCommandHandler : ICommandHandler<UpdateMaterialConsumptionCommand, Result<MaterialConsumptionDto>>
{
    private readonly IMaterialConsumptionRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateMaterialConsumptionCommandHandler(IMaterialConsumptionRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<MaterialConsumptionDto>> HandleAsync(UpdateMaterialConsumptionCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<MaterialConsumptionDto>(Error.NotFound("BUS-001", "MaterialConsumption was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(MaterialConsumptionMapper.ToDto(e));
    }
}

public sealed class DeleteMaterialConsumptionCommandHandler : ICommandHandler<DeleteMaterialConsumptionCommand, Result>
{
    private readonly IMaterialConsumptionRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteMaterialConsumptionCommandHandler(IMaterialConsumptionRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteMaterialConsumptionCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "MaterialConsumption was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

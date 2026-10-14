using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IProductionLineRepository
{
    Task<ProductionLine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductionLine entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ProductionLine> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchProductionLineQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedProductionLineDto>>;
public sealed record GetProductionLineByIdQuery(Guid Id) : IQuery<Result<ProductionLineDto>>;
public sealed record CreateProductionLineCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionLineDto>>;
public sealed record UpdateProductionLineCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionLineDto>>;
public sealed record DeleteProductionLineCommand(Guid Id) : ICommand<Result>;

public static class ProductionLineMapper
{
    public static ProductionLineDto ToDto(ProductionLine e) => new()
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

public sealed class SearchProductionLineQueryHandler : IQueryHandler<SearchProductionLineQuery, Result<PagedProductionLineDto>>
{
    private readonly IProductionLineRepository _repo;
    public SearchProductionLineQueryHandler(IProductionLineRepository repo) => _repo = repo;
    public async Task<Result<PagedProductionLineDto>> HandleAsync(SearchProductionLineQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedProductionLineDto
        {
            Items = items.Select(ProductionLineMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetProductionLineByIdQueryHandler : IQueryHandler<GetProductionLineByIdQuery, Result<ProductionLineDto>>
{
    private readonly IProductionLineRepository _repo;
    public GetProductionLineByIdQueryHandler(IProductionLineRepository repo) => _repo = repo;
    public async Task<Result<ProductionLineDto>> HandleAsync(GetProductionLineByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionLineDto>(Error.NotFound("BUS-001", "ProductionLine was not found."));
        return Result.Success(ProductionLineMapper.ToDto(e));
    }
}

public sealed class CreateProductionLineCommandHandler : ICommandHandler<CreateProductionLineCommand, Result<ProductionLineDto>>
{
    private readonly IProductionLineRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateProductionLineCommandHandler(IProductionLineRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionLineDto>> HandleAsync(CreateProductionLineCommand command, CancellationToken cancellationToken = default)
    {
        var e = ProductionLine.Create(SystemIdentifier.Ensure(command.Code, "LINE"), command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionLineMapper.ToDto(e));
    }
}

public sealed class UpdateProductionLineCommandHandler : ICommandHandler<UpdateProductionLineCommand, Result<ProductionLineDto>>
{
    private readonly IProductionLineRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateProductionLineCommandHandler(IProductionLineRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionLineDto>> HandleAsync(UpdateProductionLineCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionLineDto>(Error.NotFound("BUS-001", "ProductionLine was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionLineMapper.ToDto(e));
    }
}

public sealed class DeleteProductionLineCommandHandler : ICommandHandler<DeleteProductionLineCommand, Result>
{
    private readonly IProductionLineRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteProductionLineCommandHandler(IProductionLineRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteProductionLineCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "ProductionLine was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

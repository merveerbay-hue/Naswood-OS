using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IProductionOrderRepository
{
    Task<ProductionOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductionOrder entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ProductionOrder> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchProductionOrderQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedProductionOrderDto>>;
public sealed record GetProductionOrderByIdQuery(Guid Id) : IQuery<Result<ProductionOrderDto>>;
public sealed record CreateProductionOrderCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionOrderDto>>;
public sealed record UpdateProductionOrderCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionOrderDto>>;
public sealed record DeleteProductionOrderCommand(Guid Id) : ICommand<Result>;

public static class ProductionOrderMapper
{
    public static ProductionOrderDto ToDto(ProductionOrder e) => new()
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

public sealed class SearchProductionOrderQueryHandler : IQueryHandler<SearchProductionOrderQuery, Result<PagedProductionOrderDto>>
{
    private readonly IProductionOrderRepository _repo;
    public SearchProductionOrderQueryHandler(IProductionOrderRepository repo) => _repo = repo;
    public async Task<Result<PagedProductionOrderDto>> HandleAsync(SearchProductionOrderQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedProductionOrderDto
        {
            Items = items.Select(ProductionOrderMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetProductionOrderByIdQueryHandler : IQueryHandler<GetProductionOrderByIdQuery, Result<ProductionOrderDto>>
{
    private readonly IProductionOrderRepository _repo;
    public GetProductionOrderByIdQueryHandler(IProductionOrderRepository repo) => _repo = repo;
    public async Task<Result<ProductionOrderDto>> HandleAsync(GetProductionOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionOrderDto>(Error.NotFound("BUS-001", "ProductionOrder was not found."));
        return Result.Success(ProductionOrderMapper.ToDto(e));
    }
}

public sealed class CreateProductionOrderCommandHandler : ICommandHandler<CreateProductionOrderCommand, Result<ProductionOrderDto>>
{
    private readonly IProductionOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateProductionOrderCommandHandler(IProductionOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionOrderDto>> HandleAsync(CreateProductionOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = ProductionOrder.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionOrderMapper.ToDto(e));
    }
}

public sealed class UpdateProductionOrderCommandHandler : ICommandHandler<UpdateProductionOrderCommand, Result<ProductionOrderDto>>
{
    private readonly IProductionOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateProductionOrderCommandHandler(IProductionOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionOrderDto>> HandleAsync(UpdateProductionOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionOrderDto>(Error.NotFound("BUS-001", "ProductionOrder was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionOrderMapper.ToDto(e));
    }
}

public sealed class DeleteProductionOrderCommandHandler : ICommandHandler<DeleteProductionOrderCommand, Result>
{
    private readonly IProductionOrderRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteProductionOrderCommandHandler(IProductionOrderRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteProductionOrderCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "ProductionOrder was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

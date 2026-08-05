using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IProductionParameterRepository
{
    Task<ProductionParameter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductionParameter entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ProductionParameter> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchProductionParameterQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedProductionParameterDto>>;
public sealed record GetProductionParameterByIdQuery(Guid Id) : IQuery<Result<ProductionParameterDto>>;
public sealed record CreateProductionParameterCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionParameterDto>>;
public sealed record UpdateProductionParameterCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionParameterDto>>;
public sealed record DeleteProductionParameterCommand(Guid Id) : ICommand<Result>;

public static class ProductionParameterMapper
{
    public static ProductionParameterDto ToDto(ProductionParameter e) => new()
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

public sealed class SearchProductionParameterQueryHandler : IQueryHandler<SearchProductionParameterQuery, Result<PagedProductionParameterDto>>
{
    private readonly IProductionParameterRepository _repo;
    public SearchProductionParameterQueryHandler(IProductionParameterRepository repo) => _repo = repo;
    public async Task<Result<PagedProductionParameterDto>> HandleAsync(SearchProductionParameterQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedProductionParameterDto
        {
            Items = items.Select(ProductionParameterMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetProductionParameterByIdQueryHandler : IQueryHandler<GetProductionParameterByIdQuery, Result<ProductionParameterDto>>
{
    private readonly IProductionParameterRepository _repo;
    public GetProductionParameterByIdQueryHandler(IProductionParameterRepository repo) => _repo = repo;
    public async Task<Result<ProductionParameterDto>> HandleAsync(GetProductionParameterByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionParameterDto>(Error.NotFound("BUS-001", "ProductionParameter was not found."));
        return Result.Success(ProductionParameterMapper.ToDto(e));
    }
}

public sealed class CreateProductionParameterCommandHandler : ICommandHandler<CreateProductionParameterCommand, Result<ProductionParameterDto>>
{
    private readonly IProductionParameterRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateProductionParameterCommandHandler(IProductionParameterRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionParameterDto>> HandleAsync(CreateProductionParameterCommand command, CancellationToken cancellationToken = default)
    {
        var e = ProductionParameter.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionParameterMapper.ToDto(e));
    }
}

public sealed class UpdateProductionParameterCommandHandler : ICommandHandler<UpdateProductionParameterCommand, Result<ProductionParameterDto>>
{
    private readonly IProductionParameterRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateProductionParameterCommandHandler(IProductionParameterRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionParameterDto>> HandleAsync(UpdateProductionParameterCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionParameterDto>(Error.NotFound("BUS-001", "ProductionParameter was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionParameterMapper.ToDto(e));
    }
}

public sealed class DeleteProductionParameterCommandHandler : ICommandHandler<DeleteProductionParameterCommand, Result>
{
    private readonly IProductionParameterRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteProductionParameterCommandHandler(IProductionParameterRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteProductionParameterCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "ProductionParameter was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

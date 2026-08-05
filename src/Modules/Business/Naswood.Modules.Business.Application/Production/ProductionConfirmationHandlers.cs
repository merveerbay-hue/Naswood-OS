using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IProductionConfirmationRepository
{
    Task<ProductionConfirmation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductionConfirmation entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ProductionConfirmation> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchProductionConfirmationQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedProductionConfirmationDto>>;
public sealed record GetProductionConfirmationByIdQuery(Guid Id) : IQuery<Result<ProductionConfirmationDto>>;
public sealed record CreateProductionConfirmationCommand(string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionConfirmationDto>>;
public sealed record UpdateProductionConfirmationCommand(Guid Id, string Code, string Name, string Status, string Notes, string? PlantId) : ICommand<Result<ProductionConfirmationDto>>;
public sealed record DeleteProductionConfirmationCommand(Guid Id) : ICommand<Result>;

public static class ProductionConfirmationMapper
{
    public static ProductionConfirmationDto ToDto(ProductionConfirmation e) => new()
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

public sealed class SearchProductionConfirmationQueryHandler : IQueryHandler<SearchProductionConfirmationQuery, Result<PagedProductionConfirmationDto>>
{
    private readonly IProductionConfirmationRepository _repo;
    public SearchProductionConfirmationQueryHandler(IProductionConfirmationRepository repo) => _repo = repo;
    public async Task<Result<PagedProductionConfirmationDto>> HandleAsync(SearchProductionConfirmationQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedProductionConfirmationDto
        {
            Items = items.Select(ProductionConfirmationMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetProductionConfirmationByIdQueryHandler : IQueryHandler<GetProductionConfirmationByIdQuery, Result<ProductionConfirmationDto>>
{
    private readonly IProductionConfirmationRepository _repo;
    public GetProductionConfirmationByIdQueryHandler(IProductionConfirmationRepository repo) => _repo = repo;
    public async Task<Result<ProductionConfirmationDto>> HandleAsync(GetProductionConfirmationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionConfirmationDto>(Error.NotFound("BUS-001", "ProductionConfirmation was not found."));
        return Result.Success(ProductionConfirmationMapper.ToDto(e));
    }
}

public sealed class CreateProductionConfirmationCommandHandler : ICommandHandler<CreateProductionConfirmationCommand, Result<ProductionConfirmationDto>>
{
    private readonly IProductionConfirmationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateProductionConfirmationCommandHandler(IProductionConfirmationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionConfirmationDto>> HandleAsync(CreateProductionConfirmationCommand command, CancellationToken cancellationToken = default)
    {
        var e = ProductionConfirmation.Create(command.Code, command.Name, command.Status, command.Notes, plantId: command.PlantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionConfirmationMapper.ToDto(e));
    }
}

public sealed class UpdateProductionConfirmationCommandHandler : ICommandHandler<UpdateProductionConfirmationCommand, Result<ProductionConfirmationDto>>
{
    private readonly IProductionConfirmationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateProductionConfirmationCommandHandler(IProductionConfirmationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ProductionConfirmationDto>> HandleAsync(UpdateProductionConfirmationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ProductionConfirmationDto>(Error.NotFound("BUS-001", "ProductionConfirmation was not found."));
        e.Update(command.Code, command.Name, command.Status, command.Notes, command.PlantId);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ProductionConfirmationMapper.ToDto(e));
    }
}

public sealed class DeleteProductionConfirmationCommandHandler : ICommandHandler<DeleteProductionConfirmationCommand, Result>
{
    private readonly IProductionConfirmationRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteProductionConfirmationCommandHandler(IProductionConfirmationRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteProductionConfirmationCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "ProductionConfirmation was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

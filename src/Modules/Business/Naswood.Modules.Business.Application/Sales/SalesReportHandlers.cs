using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface ISalesReportRepository
{
    Task<SalesReportDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(SalesReportDefinition entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SalesReportDefinition> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchSalesReportQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedSalesReportDto>>;
public sealed record GetSalesReportByIdQuery(Guid Id) : IQuery<Result<SalesReportDto>>;
public sealed record CreateSalesReportCommand(string ReportCode, string Name, string Category, string Description) : ICommand<Result<SalesReportDto>>;
public sealed record UpdateSalesReportCommand(Guid Id, string ReportCode, string Name, string Category, string Description) : ICommand<Result<SalesReportDto>>;
public sealed record DeleteSalesReportCommand(Guid Id) : ICommand<Result>;

public static class SalesReportMapper
{
    public static SalesReportDto ToDto(SalesReportDefinition e) => new()
    {
        Id = e.Id,
            ReportCode = e.ReportCode,
            Name = e.Name,
            Category = e.Category,
            Description = e.Description,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchSalesReportQueryHandler : IQueryHandler<SearchSalesReportQuery, Result<PagedSalesReportDto>>
{
    private readonly ISalesReportRepository _repo;
    public SearchSalesReportQueryHandler(ISalesReportRepository repo) => _repo = repo;
    public async Task<Result<PagedSalesReportDto>> HandleAsync(SearchSalesReportQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedSalesReportDto
        {
            Items = items.Select(SalesReportMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetSalesReportByIdQueryHandler : IQueryHandler<GetSalesReportByIdQuery, Result<SalesReportDto>>
{
    private readonly ISalesReportRepository _repo;
    public GetSalesReportByIdQueryHandler(ISalesReportRepository repo) => _repo = repo;
    public async Task<Result<SalesReportDto>> HandleAsync(GetSalesReportByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SalesReportDto>(Error.NotFound("BUS-001", "SalesReport was not found."));
        return Result.Success(SalesReportMapper.ToDto(e));
    }
}

public sealed class CreateSalesReportCommandHandler : ICommandHandler<CreateSalesReportCommand, Result<SalesReportDto>>
{
    private readonly ISalesReportRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateSalesReportCommandHandler(ISalesReportRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SalesReportDto>> HandleAsync(CreateSalesReportCommand command, CancellationToken cancellationToken = default)
    {
        var e = SalesReportDefinition.Create(command.ReportCode, command.Name, command.Category, command.Description);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SalesReportMapper.ToDto(e));
    }
}

public sealed class UpdateSalesReportCommandHandler : ICommandHandler<UpdateSalesReportCommand, Result<SalesReportDto>>
{
    private readonly ISalesReportRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateSalesReportCommandHandler(ISalesReportRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<SalesReportDto>> HandleAsync(UpdateSalesReportCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<SalesReportDto>(Error.NotFound("BUS-001", "SalesReport was not found."));
        e.Update(command.ReportCode, command.Name, command.Category, command.Description);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SalesReportMapper.ToDto(e));
    }
}

public sealed class DeleteSalesReportCommandHandler : ICommandHandler<DeleteSalesReportCommand, Result>
{
    private readonly ISalesReportRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteSalesReportCommandHandler(ISalesReportRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteSalesReportCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "SalesReport was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

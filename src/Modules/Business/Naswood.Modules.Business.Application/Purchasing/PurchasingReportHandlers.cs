using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface IPurchasingReportRepository
{
    Task<PurchasingReportDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PurchasingReportDefinition entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<PurchasingReportDefinition> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchPurchasingReportQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedPurchasingReportDto>>;
public sealed record GetPurchasingReportByIdQuery(Guid Id) : IQuery<Result<PurchasingReportDto>>;
public sealed record CreatePurchasingReportCommand(string ReportCode, string Name, string Category, string Description) : ICommand<Result<PurchasingReportDto>>;
public sealed record UpdatePurchasingReportCommand(Guid Id, string ReportCode, string Name, string Category, string Description) : ICommand<Result<PurchasingReportDto>>;
public sealed record DeletePurchasingReportCommand(Guid Id) : ICommand<Result>;

public static class PurchasingReportMapper
{
    public static PurchasingReportDto ToDto(PurchasingReportDefinition e) => new()
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

public sealed class SearchPurchasingReportQueryHandler : IQueryHandler<SearchPurchasingReportQuery, Result<PagedPurchasingReportDto>>
{
    private readonly IPurchasingReportRepository _repo;
    public SearchPurchasingReportQueryHandler(IPurchasingReportRepository repo) => _repo = repo;
    public async Task<Result<PagedPurchasingReportDto>> HandleAsync(SearchPurchasingReportQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedPurchasingReportDto
        {
            Items = items.Select(PurchasingReportMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetPurchasingReportByIdQueryHandler : IQueryHandler<GetPurchasingReportByIdQuery, Result<PurchasingReportDto>>
{
    private readonly IPurchasingReportRepository _repo;
    public GetPurchasingReportByIdQueryHandler(IPurchasingReportRepository repo) => _repo = repo;
    public async Task<Result<PurchasingReportDto>> HandleAsync(GetPurchasingReportByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchasingReportDto>(Error.NotFound("BUS-001", "PurchasingReport was not found."));
        return Result.Success(PurchasingReportMapper.ToDto(e));
    }
}

public sealed class CreatePurchasingReportCommandHandler : ICommandHandler<CreatePurchasingReportCommand, Result<PurchasingReportDto>>
{
    private readonly IPurchasingReportRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreatePurchasingReportCommandHandler(IPurchasingReportRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchasingReportDto>> HandleAsync(CreatePurchasingReportCommand command, CancellationToken cancellationToken = default)
    {
        var e = PurchasingReportDefinition.Create(command.ReportCode, command.Name, command.Category, command.Description);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchasingReportMapper.ToDto(e));
    }
}

public sealed class UpdatePurchasingReportCommandHandler : ICommandHandler<UpdatePurchasingReportCommand, Result<PurchasingReportDto>>
{
    private readonly IPurchasingReportRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdatePurchasingReportCommandHandler(IPurchasingReportRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<PurchasingReportDto>> HandleAsync(UpdatePurchasingReportCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<PurchasingReportDto>(Error.NotFound("BUS-001", "PurchasingReport was not found."));
        e.Update(command.ReportCode, command.Name, command.Category, command.Description);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(PurchasingReportMapper.ToDto(e));
    }
}

public sealed class DeletePurchasingReportCommandHandler : ICommandHandler<DeletePurchasingReportCommand, Result>
{
    private readonly IPurchasingReportRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeletePurchasingReportCommandHandler(IPurchasingReportRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeletePurchasingReportCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "PurchasingReport was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

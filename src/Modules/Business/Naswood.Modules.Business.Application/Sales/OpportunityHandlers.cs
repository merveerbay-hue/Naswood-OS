using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface IOpportunityRepository
{
    Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Opportunity entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Opportunity> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchOpportunityQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedOpportunityDto>>;
public sealed record GetOpportunityByIdQuery(Guid Id) : IQuery<Result<OpportunityDto>>;
public sealed record CreateOpportunityCommand(string Number, string CustomerCode, string Title, decimal Amount, string Stage, string Status) : ICommand<Result<OpportunityDto>>;
public sealed record UpdateOpportunityCommand(Guid Id, string Number, string CustomerCode, string Title, decimal Amount, string Stage, string Status) : ICommand<Result<OpportunityDto>>;
public sealed record DeleteOpportunityCommand(Guid Id) : ICommand<Result>;

public static class OpportunityMapper
{
    public static OpportunityDto ToDto(Opportunity e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            CustomerCode = e.CustomerCode,
            Title = e.Title,
            Amount = e.Amount,
            Stage = e.Stage,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchOpportunityQueryHandler : IQueryHandler<SearchOpportunityQuery, Result<PagedOpportunityDto>>
{
    private readonly IOpportunityRepository _repo;
    public SearchOpportunityQueryHandler(IOpportunityRepository repo) => _repo = repo;
    public async Task<Result<PagedOpportunityDto>> HandleAsync(SearchOpportunityQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedOpportunityDto
        {
            Items = items.Select(OpportunityMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetOpportunityByIdQueryHandler : IQueryHandler<GetOpportunityByIdQuery, Result<OpportunityDto>>
{
    private readonly IOpportunityRepository _repo;
    public GetOpportunityByIdQueryHandler(IOpportunityRepository repo) => _repo = repo;
    public async Task<Result<OpportunityDto>> HandleAsync(GetOpportunityByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<OpportunityDto>(Error.NotFound("BUS-001", "Opportunity was not found."));
        return Result.Success(OpportunityMapper.ToDto(e));
    }
}

public sealed class CreateOpportunityCommandHandler : ICommandHandler<CreateOpportunityCommand, Result<OpportunityDto>>
{
    private readonly IOpportunityRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateOpportunityCommandHandler(IOpportunityRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<OpportunityDto>> HandleAsync(CreateOpportunityCommand command, CancellationToken cancellationToken = default)
    {
        var e = Opportunity.Create(command.Number, command.CustomerCode, command.Title, command.Amount, command.Stage, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(OpportunityMapper.ToDto(e));
    }
}

public sealed class UpdateOpportunityCommandHandler : ICommandHandler<UpdateOpportunityCommand, Result<OpportunityDto>>
{
    private readonly IOpportunityRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateOpportunityCommandHandler(IOpportunityRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<OpportunityDto>> HandleAsync(UpdateOpportunityCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<OpportunityDto>(Error.NotFound("BUS-001", "Opportunity was not found."));
        e.Update(command.Number, command.CustomerCode, command.Title, command.Amount, command.Stage, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(OpportunityMapper.ToDto(e));
    }
}

public sealed class DeleteOpportunityCommandHandler : ICommandHandler<DeleteOpportunityCommand, Result>
{
    private readonly IOpportunityRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteOpportunityCommandHandler(IOpportunityRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteOpportunityCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Opportunity was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

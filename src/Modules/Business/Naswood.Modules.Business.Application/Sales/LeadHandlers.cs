using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Lead entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Lead> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchLeadQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedLeadDto>>;
public sealed record GetLeadByIdQuery(Guid Id) : IQuery<Result<LeadDto>>;
public sealed record CreateLeadCommand(string Code, string Name, string CompanyName, string Email, string Source, string Status) : ICommand<Result<LeadDto>>;
public sealed record UpdateLeadCommand(Guid Id, string Code, string Name, string CompanyName, string Email, string Source, string Status) : ICommand<Result<LeadDto>>;
public sealed record DeleteLeadCommand(Guid Id) : ICommand<Result>;

public static class LeadMapper
{
    public static LeadDto ToDto(Lead e) => new()
    {
        Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            CompanyName = e.CompanyName,
            Email = e.Email,
            Source = e.Source,
            Status = e.Status,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchLeadQueryHandler : IQueryHandler<SearchLeadQuery, Result<PagedLeadDto>>
{
    private readonly ILeadRepository _repo;
    public SearchLeadQueryHandler(ILeadRepository repo) => _repo = repo;
    public async Task<Result<PagedLeadDto>> HandleAsync(SearchLeadQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedLeadDto
        {
            Items = items.Select(LeadMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetLeadByIdQueryHandler : IQueryHandler<GetLeadByIdQuery, Result<LeadDto>>
{
    private readonly ILeadRepository _repo;
    public GetLeadByIdQueryHandler(ILeadRepository repo) => _repo = repo;
    public async Task<Result<LeadDto>> HandleAsync(GetLeadByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<LeadDto>(Error.NotFound("BUS-001", "Lead was not found."));
        return Result.Success(LeadMapper.ToDto(e));
    }
}

public sealed class CreateLeadCommandHandler : ICommandHandler<CreateLeadCommand, Result<LeadDto>>
{
    private readonly ILeadRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateLeadCommandHandler(ILeadRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<LeadDto>> HandleAsync(CreateLeadCommand command, CancellationToken cancellationToken = default)
    {
        var e = Lead.Create(command.Code, command.Name, command.CompanyName, command.Email, command.Source, command.Status);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(LeadMapper.ToDto(e));
    }
}

public sealed class UpdateLeadCommandHandler : ICommandHandler<UpdateLeadCommand, Result<LeadDto>>
{
    private readonly ILeadRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateLeadCommandHandler(ILeadRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<LeadDto>> HandleAsync(UpdateLeadCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<LeadDto>(Error.NotFound("BUS-001", "Lead was not found."));
        e.Update(command.Code, command.Name, command.CompanyName, command.Email, command.Source, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(LeadMapper.ToDto(e));
    }
}

public sealed class DeleteLeadCommandHandler : ICommandHandler<DeleteLeadCommand, Result>
{
    private readonly ILeadRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteLeadCommandHandler(ILeadRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteLeadCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Lead was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

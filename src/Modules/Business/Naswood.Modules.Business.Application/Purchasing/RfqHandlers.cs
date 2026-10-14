using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Purchasing;
using Naswood.Modules.Business.Domain.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public interface IRfqRepository
{
    Task<Rfq?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Rfq entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Rfq> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchRfqQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedRfqDto>>;
public sealed record GetRfqByIdQuery(Guid Id) : IQuery<Result<RfqDto>>;
public sealed record CreateRfqCommand(string Number, string Title, DateOnly? DueDate, string Status, string Notes) : ICommand<Result<RfqDto>>;
public sealed record UpdateRfqCommand(Guid Id, string Number, string Title, DateOnly? DueDate, string Status, string Notes) : ICommand<Result<RfqDto>>;
public sealed record DeleteRfqCommand(Guid Id) : ICommand<Result>;

public static class RfqMapper
{
    public static RfqDto ToDto(Rfq e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            Title = e.Title,
            DueDate = e.DueDate,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchRfqQueryHandler : IQueryHandler<SearchRfqQuery, Result<PagedRfqDto>>
{
    private readonly IRfqRepository _repo;
    public SearchRfqQueryHandler(IRfqRepository repo) => _repo = repo;
    public async Task<Result<PagedRfqDto>> HandleAsync(SearchRfqQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedRfqDto
        {
            Items = items.Select(RfqMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetRfqByIdQueryHandler : IQueryHandler<GetRfqByIdQuery, Result<RfqDto>>
{
    private readonly IRfqRepository _repo;
    public GetRfqByIdQueryHandler(IRfqRepository repo) => _repo = repo;
    public async Task<Result<RfqDto>> HandleAsync(GetRfqByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<RfqDto>(Error.NotFound("BUS-001", "Rfq was not found."));
        return Result.Success(RfqMapper.ToDto(e));
    }
}

public sealed class CreateRfqCommandHandler : ICommandHandler<CreateRfqCommand, Result<RfqDto>>
{
    private readonly IRfqRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateRfqCommandHandler(IRfqRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<RfqDto>> HandleAsync(CreateRfqCommand command, CancellationToken cancellationToken = default)
    {
        var e = Rfq.Create(command.Number, command.Title, command.DueDate, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(RfqMapper.ToDto(e));
    }
}

public sealed class UpdateRfqCommandHandler : ICommandHandler<UpdateRfqCommand, Result<RfqDto>>
{
    private readonly IRfqRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateRfqCommandHandler(IRfqRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<RfqDto>> HandleAsync(UpdateRfqCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<RfqDto>(Error.NotFound("BUS-001", "Rfq was not found."));
        e.Update(command.Number, command.Title, command.DueDate, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(RfqMapper.ToDto(e));
    }
}

public sealed class DeleteRfqCommandHandler : ICommandHandler<DeleteRfqCommand, Result>
{
    private readonly IRfqRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteRfqCommandHandler(IRfqRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteRfqCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Rfq was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

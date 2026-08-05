using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface IDeliveryRepository
{
    Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Delivery entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Delivery> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchDeliveryQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedDeliveryDto>>;
public sealed record GetDeliveryByIdQuery(Guid Id) : IQuery<Result<DeliveryDto>>;
public sealed record CreateDeliveryCommand(string Number, string ShipmentNumber, string CustomerCode, string Status, string Notes) : ICommand<Result<DeliveryDto>>;
public sealed record UpdateDeliveryCommand(Guid Id, string Number, string ShipmentNumber, string CustomerCode, string Status, string Notes) : ICommand<Result<DeliveryDto>>;
public sealed record DeleteDeliveryCommand(Guid Id) : ICommand<Result>;

public static class DeliveryMapper
{
    public static DeliveryDto ToDto(Delivery e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            ShipmentNumber = e.ShipmentNumber,
            CustomerCode = e.CustomerCode,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchDeliveryQueryHandler : IQueryHandler<SearchDeliveryQuery, Result<PagedDeliveryDto>>
{
    private readonly IDeliveryRepository _repo;
    public SearchDeliveryQueryHandler(IDeliveryRepository repo) => _repo = repo;
    public async Task<Result<PagedDeliveryDto>> HandleAsync(SearchDeliveryQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedDeliveryDto
        {
            Items = items.Select(DeliveryMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetDeliveryByIdQueryHandler : IQueryHandler<GetDeliveryByIdQuery, Result<DeliveryDto>>
{
    private readonly IDeliveryRepository _repo;
    public GetDeliveryByIdQueryHandler(IDeliveryRepository repo) => _repo = repo;
    public async Task<Result<DeliveryDto>> HandleAsync(GetDeliveryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<DeliveryDto>(Error.NotFound("BUS-001", "Delivery was not found."));
        return Result.Success(DeliveryMapper.ToDto(e));
    }
}

public sealed class CreateDeliveryCommandHandler : ICommandHandler<CreateDeliveryCommand, Result<DeliveryDto>>
{
    private readonly IDeliveryRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateDeliveryCommandHandler(IDeliveryRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<DeliveryDto>> HandleAsync(CreateDeliveryCommand command, CancellationToken cancellationToken = default)
    {
        var e = Delivery.Create(command.Number, command.ShipmentNumber, command.CustomerCode, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(DeliveryMapper.ToDto(e));
    }
}

public sealed class UpdateDeliveryCommandHandler : ICommandHandler<UpdateDeliveryCommand, Result<DeliveryDto>>
{
    private readonly IDeliveryRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateDeliveryCommandHandler(IDeliveryRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<DeliveryDto>> HandleAsync(UpdateDeliveryCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<DeliveryDto>(Error.NotFound("BUS-001", "Delivery was not found."));
        e.Update(command.Number, command.ShipmentNumber, command.CustomerCode, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(DeliveryMapper.ToDto(e));
    }
}

public sealed class DeleteDeliveryCommandHandler : ICommandHandler<DeleteDeliveryCommand, Result>
{
    private readonly IDeliveryRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteDeliveryCommandHandler(IDeliveryRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteDeliveryCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Delivery was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

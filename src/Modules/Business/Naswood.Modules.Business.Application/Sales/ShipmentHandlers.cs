using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Sales;
using Naswood.Modules.Business.Domain.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Shipment entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Shipment> Items, int Total)> SearchAsync(string? q, int page, int pageSize, CancellationToken cancellationToken = default);
}

public sealed record SearchShipmentQuery(string? Q, int Page, int PageSize) : IQuery<Result<PagedShipmentDto>>;
public sealed record GetShipmentByIdQuery(Guid Id) : IQuery<Result<ShipmentDto>>;
public sealed record CreateShipmentCommand(string Number, string SalesOrderNumber, string WarehouseCode, string Status, string Notes) : ICommand<Result<ShipmentDto>>;
public sealed record UpdateShipmentCommand(Guid Id, string Number, string SalesOrderNumber, string WarehouseCode, string Status, string Notes) : ICommand<Result<ShipmentDto>>;
public sealed record DeleteShipmentCommand(Guid Id) : ICommand<Result>;

public static class ShipmentMapper
{
    public static ShipmentDto ToDto(Shipment e) => new()
    {
        Id = e.Id,
            Number = e.Number,
            SalesOrderNumber = e.SalesOrderNumber,
            WarehouseCode = e.WarehouseCode,
            Status = e.Status,
            Notes = e.Notes,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchShipmentQueryHandler : IQueryHandler<SearchShipmentQuery, Result<PagedShipmentDto>>
{
    private readonly IShipmentRepository _repo;
    public SearchShipmentQueryHandler(IShipmentRepository repo) => _repo = repo;
    public async Task<Result<PagedShipmentDto>> HandleAsync(SearchShipmentQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedShipmentDto
        {
            Items = items.Select(ShipmentMapper.ToDto).ToArray(),
            Page = page, PageSize = pageSize, TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetShipmentByIdQueryHandler : IQueryHandler<GetShipmentByIdQuery, Result<ShipmentDto>>
{
    private readonly IShipmentRepository _repo;
    public GetShipmentByIdQueryHandler(IShipmentRepository repo) => _repo = repo;
    public async Task<Result<ShipmentDto>> HandleAsync(GetShipmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ShipmentDto>(Error.NotFound("BUS-001", "Shipment was not found."));
        return Result.Success(ShipmentMapper.ToDto(e));
    }
}

public sealed class CreateShipmentCommandHandler : ICommandHandler<CreateShipmentCommand, Result<ShipmentDto>>
{
    private readonly IShipmentRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public CreateShipmentCommandHandler(IShipmentRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ShipmentDto>> HandleAsync(CreateShipmentCommand command, CancellationToken cancellationToken = default)
    {
        var e = Shipment.Create(command.Number, command.SalesOrderNumber, command.WarehouseCode, command.Status, command.Notes);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ShipmentMapper.ToDto(e));
    }
}

public sealed class UpdateShipmentCommandHandler : ICommandHandler<UpdateShipmentCommand, Result<ShipmentDto>>
{
    private readonly IShipmentRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public UpdateShipmentCommandHandler(IShipmentRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<ShipmentDto>> HandleAsync(UpdateShipmentCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure<ShipmentDto>(Error.NotFound("BUS-001", "Shipment was not found."));
        e.Update(command.Number, command.SalesOrderNumber, command.WarehouseCode, command.Status, command.Notes);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(ShipmentMapper.ToDto(e));
    }
}

public sealed class DeleteShipmentCommandHandler : ICommandHandler<DeleteShipmentCommand, Result>
{
    private readonly IShipmentRepository _repo;
    private readonly IBusinessUnitOfWork _uow;
    public DeleteShipmentCommandHandler(IShipmentRepository repo, IBusinessUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> HandleAsync(DeleteShipmentCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted) return Result.Failure(Error.NotFound("BUS-001", "Shipment was not found."));
        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

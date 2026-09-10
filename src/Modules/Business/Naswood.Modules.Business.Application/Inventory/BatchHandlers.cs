using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Contracts.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IBatchRepository
{
    Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Lot lookup by BatchNumber + MaterialCode.
    /// When <paramref name="plantId"/> is set, only that plant's batch is returned (posting paths).
    /// </summary>
    Task<Batch?> GetByNumberAndMaterialAsync(
        string batchNumber,
        string materialCode,
        string? plantId = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(Batch entity, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Batch> Items, int Total)> SearchAsync(
        string? q, int page, int pageSize, string? plantId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListOpeningLotNumbersAsync(
        string prefix, string? plantId = null, CancellationToken cancellationToken = default);
}

public sealed record SearchBatchQuery(
    string? Q,
    int Page,
    int PageSize,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<PagedBatchDto>>;

public sealed record GetBatchByIdQuery(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : IQuery<Result<BatchDto>>;

public sealed record CreateBatchCommand(
    string BatchNumber,
    string MaterialCode,
    decimal Quantity,
    DateOnly? ExpiryDate,
    string Status,
    string? PlantId,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<BatchDto>>;

public sealed record UpdateBatchCommand(
    Guid Id,
    string BatchNumber,
    string MaterialCode,
    decimal Quantity,
    DateOnly? ExpiryDate,
    string Status,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<BatchDto>>;

public sealed record DeleteBatchCommand(
    Guid Id,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result>;

public static class BatchMapper
{
    public static BatchDto ToDto(Batch e) => new()
    {
        Id = e.Id,
        BatchNumber = e.BatchNumber,
        MaterialCode = e.MaterialCode,
        Quantity = e.Quantity,
        ExpiryDate = e.ExpiryDate,
        Status = e.Status,
        SourceType = e.SourceType,
        CompanyId = e.CompanyId,
        PlantId = e.PlantId,
        CreatedAt = e.CreatedAt
    };
}

public sealed class SearchBatchQueryHandler : IQueryHandler<SearchBatchQuery, Result<PagedBatchDto>>
{
    private readonly IBatchRepository _repo;
    public SearchBatchQueryHandler(IBatchRepository repo) => _repo = repo;

    public async Task<Result<PagedBatchDto>> HandleAsync(SearchBatchQuery query, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(query.PlantId) ? null : query.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<PagedBatchDto>(Error.Validation(
                "INV-LOT-004",
                "Plant / factory context is required."));

        if (query.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(query.AllowedPlantIds, plantId))
            return Result.Failure<PagedBatchDto>(Error.Forbidden(
                "INV-LOT-403",
                "Bu tesisin lot/batch kayıtlarını görüntüleme yetkiniz yok."));

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Q, page, pageSize, plantId, cancellationToken).ConfigureAwait(false);
        return Result.Success(new PagedBatchDto
        {
            Items = items.Select(BatchMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetBatchByIdQueryHandler : IQueryHandler<GetBatchByIdQuery, Result<BatchDto>>
{
    private readonly IBatchRepository _repo;
    public GetBatchByIdQueryHandler(IBatchRepository repo) => _repo = repo;

    public async Task<Result<BatchDto>> HandleAsync(GetBatchByIdQuery query, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<BatchDto>(Error.NotFound("BUS-001", "Batch was not found."));

        if (query.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(query.AllowedPlantIds, e.PlantId))
            return Result.Failure<BatchDto>(Error.Forbidden(
                "INV-LOT-403",
                "Bu lot/batch kaydını görüntüleme yetkiniz yok."));

        return Result.Success(BatchMapper.ToDto(e));
    }
}

public sealed class CreateBatchCommandHandler : ICommandHandler<CreateBatchCommand, Result<BatchDto>>
{
    private readonly IBatchRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public CreateBatchCommandHandler(IBatchRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<BatchDto>> HandleAsync(CreateBatchCommand command, CancellationToken cancellationToken = default)
    {
        var plantId = string.IsNullOrWhiteSpace(command.PlantId) ? null : command.PlantId.Trim();
        if (string.IsNullOrWhiteSpace(plantId))
            return Result.Failure<BatchDto>(Error.Validation(
                "INV-LOT-004",
                "Plant / factory context is required."));

        if (command.AllowedPlantIds is { Count: > 0 } && !PlantAccess.CanAccess(command.AllowedPlantIds, plantId))
            return Result.Failure<BatchDto>(Error.Forbidden(
                "INV-LOT-403",
                "Bu tesise lot/batch oluşturma yetkiniz yok."));

        var e = Batch.Create(
            SystemIdentifier.Ensure(command.BatchNumber, "LOT"),
            command.MaterialCode,
            command.Quantity,
            command.ExpiryDate,
            command.Status,
            plantId: plantId);
        await _repo.AddAsync(e, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(BatchMapper.ToDto(e));
    }
}

public sealed class UpdateBatchCommandHandler : ICommandHandler<UpdateBatchCommand, Result<BatchDto>>
{
    private readonly IBatchRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public UpdateBatchCommandHandler(IBatchRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<BatchDto>> HandleAsync(UpdateBatchCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure<BatchDto>(Error.NotFound("BUS-001", "Batch was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure<BatchDto>(Error.Forbidden(
                "INV-LOT-403",
                "Bu lot/batch kaydını değiştirme yetkiniz yok."));

        e.Update(command.BatchNumber, command.MaterialCode, command.Quantity, command.ExpiryDate, command.Status);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(BatchMapper.ToDto(e));
    }
}

public sealed class DeleteBatchCommandHandler : ICommandHandler<DeleteBatchCommand, Result>
{
    private readonly IBatchRepository _repo;
    private readonly IBusinessUnitOfWork _uow;

    public DeleteBatchCommandHandler(IBatchRepository repo, IBusinessUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(DeleteBatchCommand command, CancellationToken cancellationToken = default)
    {
        var e = await _repo.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (e is null || e.IsDeleted)
            return Result.Failure(Error.NotFound("BUS-001", "Batch was not found."));

        if (command.AllowedPlantIds is { Count: > 0 }
            && !PlantAccess.CanAccess(command.AllowedPlantIds, e.PlantId))
            return Result.Failure(Error.Forbidden(
                "INV-LOT-403",
                "Bu lot/batch kaydını silme yetkiniz yok."));

        e.SoftDelete();
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Production;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure.Production;

public sealed class ProductionExecutionStore : IProductionExecutionStore
{
    private readonly BusinessDbContext _db;
    public ProductionExecutionStore(BusinessDbContext db) => _db = db;

    public Task AddOperationAsync(ProductionOperation entity, CancellationToken cancellationToken = default)
        => _db.Set<ProductionOperation>().AddAsync(entity, cancellationToken).AsTask();

    public Task<ProductionOperation?> GetOperationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionOperation>().Local.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        return local is not null
            ? Task.FromResult<ProductionOperation?>(local)
            : _db.Set<ProductionOperation>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionOperation>> ListOperationsByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _db.Set<ProductionOperation>().Where(x => !x.IsDeleted && x.ProductionOrderId == orderId)
            .OrderBy(x => x.Sequence).ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<ProductionOperation>> ListOperationsByWorkCenterAsync(Guid workCenterId, CancellationToken cancellationToken = default)
        => await _db.Set<ProductionOperation>().Where(x => !x.IsDeleted && x.WorkCenterId == workCenterId)
            .OrderBy(x => x.Sequence).ToListAsync(cancellationToken).ConfigureAwait(false);

    public Task<ProductionOperation?> FindByOrderSequenceAsync(Guid orderId, int sequence, CancellationToken cancellationToken = default)
        => _db.Set<ProductionOperation>().FirstOrDefaultAsync(
            x => !x.IsDeleted && x.ProductionOrderId == orderId && x.Sequence == sequence, cancellationToken);

    public Task AddExecutionAsync(ProductionOperationExecution entity, CancellationToken cancellationToken = default)
        => _db.Set<ProductionOperationExecution>().AddAsync(entity, cancellationToken).AsTask();

    public Task<ProductionOperationExecution?> GetExecutionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionOperationExecution>().Local.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        return local is not null
            ? Task.FromResult<ProductionOperationExecution?>(local)
            : _db.Set<ProductionOperationExecution>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ProductionOperationExecution?> GetOpenByOperationAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionOperationExecution>().Local.FirstOrDefault(x =>
            !x.IsDeleted && x.ProductionOperationId == operationId
            && (x.Status == ProductionExecutionStatuses.Running || x.Status == ProductionExecutionStatuses.Paused
                || x.Status == ProductionExecutionStatuses.NotStarted));
        if (local is not null) return local;
        return await _db.Set<ProductionOperationExecution>().FirstOrDefaultAsync(
            x => !x.IsDeleted && x.ProductionOperationId == operationId
                 && (x.Status == ProductionExecutionStatuses.Running || x.Status == ProductionExecutionStatuses.Paused
                     || x.Status == ProductionExecutionStatuses.NotStarted),
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ProductionOperationExecution>> ListExecutionsByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        => await _db.Set<ProductionOperationExecution>().Where(x => !x.IsDeleted && x.ProductionOrderId == orderId)
            .OrderBy(x => x.CreatedAt).ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<ProductionOperationExecution>> ListExecutionsByWorkCenterAsync(Guid workCenterId, CancellationToken cancellationToken = default)
        => await _db.Set<ProductionOperationExecution>().Where(x => !x.IsDeleted && x.WorkCenterId == workCenterId)
            .OrderByDescending(x => x.UpdatedAt).ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<string>> ListNumbersAsync(string? plantId, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ProductionOperationExecution>().AsNoTracking().Where(x => !x.IsDeleted && x.Number != "");
        if (!string.IsNullOrWhiteSpace(plantId))
        {
            var p = plantId.Trim();
            q = q.Where(x => x.PlantId == p);
        }
        return await q.Select(x => x.Number).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task AddEventAsync(ProductionExecutionEvent entity, CancellationToken cancellationToken = default)
        => _db.Set<ProductionExecutionEvent>().AddAsync(entity, cancellationToken).AsTask();

    public async Task<IReadOnlyList<ProductionExecutionEvent>> ListEventsAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionExecutionEvent>().Local.Where(x => !x.IsDeleted && x.ExecutionId == executionId).ToList();
        var stored = await _db.Set<ProductionExecutionEvent>()
            .Where(x => !x.IsDeleted && x.ExecutionId == executionId)
            .OrderBy(x => x.OccurredAt).ToListAsync(cancellationToken).ConfigureAwait(false);
        return stored.Concat(local.Where(l => stored.All(s => s.Id != l.Id))).OrderBy(x => x.OccurredAt).ToArray();
    }

    public Task AddConsumptionAsync(ProductionExecutionConsumption entity, CancellationToken cancellationToken = default)
        => _db.Set<ProductionExecutionConsumption>().AddAsync(entity, cancellationToken).AsTask();

    public Task<ProductionExecutionConsumption?> GetByIdempotencyAsync(Guid executionId, string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key)) return Task.FromResult<ProductionExecutionConsumption?>(null);
        var k = key.Trim();
        return _db.Set<ProductionExecutionConsumption>().FirstOrDefaultAsync(
            x => !x.IsDeleted && x.ExecutionId == executionId && x.IdempotencyKey == k, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionExecutionConsumption>> ListConsumptionsAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionExecutionConsumption>().Local.Where(x => !x.IsDeleted && x.ExecutionId == executionId).ToList();
        var stored = await _db.Set<ProductionExecutionConsumption>()
            .Where(x => !x.IsDeleted && x.ExecutionId == executionId)
            .OrderBy(x => x.CreatedAt).ToListAsync(cancellationToken).ConfigureAwait(false);
        return stored.Concat(local.Where(l => stored.All(s => s.Id != l.Id))).OrderBy(x => x.CreatedAt).ToArray();
    }

    public Task AddScrapAsync(ProductionExecutionScrap entity, CancellationToken cancellationToken = default)
        => _db.Set<ProductionExecutionScrap>().AddAsync(entity, cancellationToken).AsTask();

    public async Task<IReadOnlyList<ProductionExecutionScrap>> ListScrapsAsync(Guid executionId, CancellationToken cancellationToken = default)
    {
        var local = _db.Set<ProductionExecutionScrap>().Local.Where(x => !x.IsDeleted && x.ExecutionId == executionId).ToList();
        var stored = await _db.Set<ProductionExecutionScrap>()
            .Where(x => !x.IsDeleted && x.ExecutionId == executionId)
            .OrderBy(x => x.RecordedAt).ToListAsync(cancellationToken).ConfigureAwait(false);
        return stored.Concat(local.Where(l => stored.All(s => s.Id != l.Id))).OrderBy(x => x.RecordedAt).ToArray();
    }
}

using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public interface IProductionExecutionStore
{
    Task AddOperationAsync(ProductionOperation entity, CancellationToken cancellationToken = default);
    Task<ProductionOperation?> GetOperationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionOperation>> ListOperationsByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionOperation>> ListOperationsByWorkCenterAsync(Guid workCenterId, CancellationToken cancellationToken = default);
    Task<ProductionOperation?> FindByOrderSequenceAsync(Guid orderId, int sequence, CancellationToken cancellationToken = default);

    Task AddExecutionAsync(ProductionOperationExecution entity, CancellationToken cancellationToken = default);
    Task<ProductionOperationExecution?> GetExecutionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductionOperationExecution?> GetOpenByOperationAsync(Guid operationId, CancellationToken cancellationToken = default);
    Task<ProductionOperationExecution?> GetCommittedOpenByOperationAsync(Guid operationId, CancellationToken cancellationToken = default);
    Task<ProductionOperationExecution?> GetCommittedExecutionAsync(Guid id, CancellationToken cancellationToken = default);
    void ClearTracker();
    Task<IReadOnlyList<ProductionOperationExecution>> ListExecutionsByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionOperationExecution>> ListExecutionsByWorkCenterAsync(Guid workCenterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListNumbersAsync(string? plantId, CancellationToken cancellationToken = default);

    Task AddEventAsync(ProductionExecutionEvent entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionExecutionEvent>> ListEventsAsync(Guid executionId, CancellationToken cancellationToken = default);

    Task AddConsumptionAsync(ProductionExecutionConsumption entity, CancellationToken cancellationToken = default);
    Task<ProductionExecutionConsumption?> GetByIdempotencyAsync(Guid executionId, string key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionExecutionConsumption>> ListConsumptionsAsync(Guid executionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionExecutionConsumption>> ListConsumptionsBySourcePackageIdsAsync(IReadOnlyList<Guid> packageIds, CancellationToken cancellationToken = default);

    Task AddScrapAsync(ProductionExecutionScrap entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductionExecutionScrap>> ListScrapsAsync(Guid executionId, CancellationToken cancellationToken = default);
}

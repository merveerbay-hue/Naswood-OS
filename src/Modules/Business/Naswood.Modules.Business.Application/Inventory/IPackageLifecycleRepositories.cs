using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public interface IPackageOperationRepository
{
    Task AddAsync(PackageOperation entity, CancellationToken cancellationToken = default);
    Task<PackageOperation?> GetByNumberAsync(string number, string? plantId, CancellationToken cancellationToken = default);
}

public interface IPackageRelationRepository
{
    Task AddRangeAsync(IReadOnlyList<PackageRelation> rows, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PackageRelation>> ListByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PackageRelation>> ListByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PackageRelation>> ListByPackageIdsAsync(IReadOnlyList<Guid> packageIds, CancellationToken cancellationToken = default);
}

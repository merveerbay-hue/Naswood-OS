using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Contracts.Production;

namespace Naswood.Modules.Business.Application.Production;

public sealed record GetProductionDashboardQuery() : IQuery<Result<ProductionDashboardDto>>;

public sealed class GetProductionDashboardQueryHandler : IQueryHandler<GetProductionDashboardQuery, Result<ProductionDashboardDto>>
{
    public Task<Result<ProductionDashboardDto>> HandleAsync(GetProductionDashboardQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success(new ProductionDashboardDto
        {
            OpenProductionOrders = 0,
            ActiveWorkOrders = 0,
            WipQuantity = 0,
            ScrapRate = 0,
        }));
    }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Contracts.Sales;

namespace Naswood.Modules.Business.Application.Sales;

public sealed record GetSalesDashboardQuery() : IQuery<Result<SalesDashboardDto>>;

public sealed class GetSalesDashboardQueryHandler : IQueryHandler<GetSalesDashboardQuery, Result<SalesDashboardDto>>
{
    public Task<Result<SalesDashboardDto>> HandleAsync(GetSalesDashboardQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success(new SalesDashboardDto
        {
            OpenOrders = 0,
            PipelineAmount = 0,
            OverdueDeliveries = 0,
            RevenueMtd = 0,
        }));
    }
}

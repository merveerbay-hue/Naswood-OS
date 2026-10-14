using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Contracts.Purchasing;

namespace Naswood.Modules.Business.Application.Purchasing;

public sealed record GetPurchasingDashboardQuery() : IQuery<Result<PurchasingDashboardDto>>;

public sealed class GetPurchasingDashboardQueryHandler : IQueryHandler<GetPurchasingDashboardQuery, Result<PurchasingDashboardDto>>
{
    public Task<Result<PurchasingDashboardDto>> HandleAsync(GetPurchasingDashboardQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success(new PurchasingDashboardDto
        {
            OpenPurchaseOrders = 0,
            PendingApprovals = 0,
            OverdueReceipts = 0,
            SpendMtd = 0,
        }));
    }
}

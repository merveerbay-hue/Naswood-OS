using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Health;

namespace Naswood.Modules.Platform.Application.Health;

public sealed class GetLivenessQueryHandler
    : IQueryHandler<GetLivenessQuery, Result<LivenessDto>>
{
    public Task<Result<LivenessDto>> HandleAsync(
        GetLivenessQuery query,
        CancellationToken cancellationToken = default)
    {
        var dto = new LivenessDto
        {
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow
        };

        return Task.FromResult(Result.Success(dto));
    }
}

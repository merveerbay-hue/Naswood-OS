using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Health;

namespace Naswood.Modules.Platform.Application.Health;

public sealed record GetHealthReportQuery : IQuery<Result<HealthReportDto>>;

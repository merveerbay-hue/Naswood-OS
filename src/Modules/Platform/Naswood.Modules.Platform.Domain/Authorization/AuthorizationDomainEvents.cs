using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

public sealed record AuthorizationDenied : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required string Permission { get; init; }

    public required string Reason { get; init; }
}

public sealed record PermissionCatalogChanged : DomainEventBase
{
    public required string ChangeType { get; init; }
}

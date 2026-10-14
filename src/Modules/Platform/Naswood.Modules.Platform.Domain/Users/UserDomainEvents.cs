using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Users;

public sealed record UserCreated : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required string Username { get; init; }
}

public sealed record UserUpdated : DomainEventBase
{
    public required Guid UserId { get; init; }
}

public sealed record UserActivated : DomainEventBase
{
    public required Guid UserId { get; init; }
}

public sealed record UserDeactivated : DomainEventBase
{
    public required Guid UserId { get; init; }
}

public sealed record UserArchived : DomainEventBase
{
    public required Guid UserId { get; init; }
}

public sealed record UserOrganizationChanged : DomainEventBase
{
    public required Guid UserId { get; init; }
}

public sealed record UserLocked : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required string Username { get; init; }

    public required string Reason { get; init; }
}

public sealed record UserUnlocked : DomainEventBase
{
    public required Guid UserId { get; init; }
}

public sealed record UserPasswordReset : DomainEventBase
{
    public required Guid UserId { get; init; }
}

public sealed record UserRoleAssigned : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }
}

public sealed record UserPlantAssigned : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required IReadOnlyList<string> PlantIds { get; init; }
}

public sealed record UserSoftDeleted : DomainEventBase
{
    public required Guid UserId { get; init; }
}

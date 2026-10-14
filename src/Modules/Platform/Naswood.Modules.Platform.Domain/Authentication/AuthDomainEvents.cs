using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authentication;

public sealed record AuthUserAuthenticated : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required Guid SessionId { get; init; }

    public required string Username { get; init; }
}

public sealed record AuthUserLoggedOut : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required Guid SessionId { get; init; }
}

public sealed record AuthSessionCreated : DomainEventBase
{
    public required Guid SessionId { get; init; }

    public required Guid UserId { get; init; }
}

public sealed record AuthSessionExpired : DomainEventBase
{
    public required Guid SessionId { get; init; }

    public required Guid UserId { get; init; }
}

public sealed record AuthTokenRefreshed : DomainEventBase
{
    public required Guid SessionId { get; init; }

    public required Guid UserId { get; init; }
}

public sealed record AuthAuthenticationFailed : DomainEventBase
{
    public required string Username { get; init; }

    public required string Reason { get; init; }
}

public sealed record AuthAccountLocked : DomainEventBase
{
    public required Guid UserId { get; init; }

    public required string Username { get; init; }
}

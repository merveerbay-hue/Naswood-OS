using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authentication;

public sealed class AuthSession : AggregateRoot<Guid>
{
    private AuthSession()
    {
    }

    private AuthSession(
        Guid id,
        Guid userId,
        Guid accessTokenId,
        string refreshTokenHash,
        string companyId,
        string plantId,
        DeviceInfo device,
        bool rememberMe,
        DateTimeOffset createdAt,
        DateTimeOffset absoluteExpiresAt,
        DateTimeOffset refreshExpiresAt)
        : base(id)
    {
        UserId = userId;
        AccessTokenId = accessTokenId;
        RefreshTokenHash = refreshTokenHash;
        CompanyId = companyId;
        PlantId = plantId;
        Device = device;
        RememberMe = rememberMe;
        CreatedAt = createdAt;
        LastActivityAt = createdAt;
        AbsoluteExpiresAt = absoluteExpiresAt;
        RefreshExpiresAt = refreshExpiresAt;
        Status = AuthSessionStatus.Active;
    }

    public Guid UserId { get; private set; }

    public Guid AccessTokenId { get; private set; }

    public string RefreshTokenHash { get; private set; } = string.Empty;

    public string CompanyId { get; private set; } = string.Empty;

    public string PlantId { get; private set; } = string.Empty;

    public DeviceInfo Device { get; private set; } = new(null, null, null, null, null, null);

    public bool RememberMe { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset LastActivityAt { get; private set; }

    public DateTimeOffset AbsoluteExpiresAt { get; private set; }

    public DateTimeOffset RefreshExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public AuthSessionStatus Status { get; private set; }

    public static AuthSession Create(
        Guid userId,
        Guid accessTokenId,
        string refreshTokenHash,
        string companyId,
        string plantId,
        DeviceInfo device,
        bool rememberMe,
        DateTimeOffset createdAt,
        TimeSpan absoluteLifetime,
        TimeSpan refreshLifetime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshTokenHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(companyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(plantId);

        var session = new AuthSession(
            UuidV7.NewGuid(),
            userId,
            accessTokenId,
            refreshTokenHash,
            companyId.Trim(),
            plantId.Trim(),
            device,
            rememberMe,
            createdAt,
            createdAt.Add(absoluteLifetime),
            createdAt.Add(refreshLifetime));

        session.RaiseDomainEvent(new AuthSessionCreated
        {
            SessionId = session.Id,
            UserId = userId
        });

        return session;
    }

    public Result EnsureUsable(DateTimeOffset utcNow, TimeSpan idleTimeout)
    {
        var check = CheckUsable(utcNow, idleTimeout);
        if (check.IsFailure && check.Error!.Code == AuthErrors.SessionExpired().Code)
        {
            MarkExpired(utcNow);
        }

        return check;
    }

    public Result CheckUsable(DateTimeOffset utcNow, TimeSpan idleTimeout)
    {
        if (Status is AuthSessionStatus.Revoked or AuthSessionStatus.Closed)
        {
            return Result.Failure(AuthErrors.RefreshTokenInvalid());
        }

        if (Status == AuthSessionStatus.Expired ||
            utcNow >= AbsoluteExpiresAt ||
            utcNow >= RefreshExpiresAt ||
            utcNow - LastActivityAt > idleTimeout)
        {
            return Result.Failure(AuthErrors.SessionExpired());
        }

        return Result.Success();
    }

    public void RotateTokens(
        Guid newAccessTokenId,
        string newRefreshTokenHash,
        DateTimeOffset utcNow,
        TimeSpan refreshLifetime)
    {
        AccessTokenId = newAccessTokenId;
        RefreshTokenHash = newRefreshTokenHash;
        LastActivityAt = utcNow;
        RefreshExpiresAt = utcNow.Add(refreshLifetime);
        Status = AuthSessionStatus.Refreshed;

        RaiseDomainEvent(new AuthTokenRefreshed
        {
            SessionId = Id,
            UserId = UserId
        });
    }

    public void Touch(DateTimeOffset utcNow) => LastActivityAt = utcNow;

    public void Revoke(DateTimeOffset utcNow, bool logout)
    {
        if (Status is AuthSessionStatus.Revoked or AuthSessionStatus.Closed)
        {
            return;
        }

        Status = logout ? AuthSessionStatus.Closed : AuthSessionStatus.Revoked;
        RevokedAt = utcNow;

        if (logout)
        {
            RaiseDomainEvent(new AuthUserLoggedOut
            {
                UserId = UserId,
                SessionId = Id
            });
        }
    }

    public void MarkExpired(DateTimeOffset utcNow)
    {
        if (Status is AuthSessionStatus.Expired or AuthSessionStatus.Revoked or AuthSessionStatus.Closed)
        {
            return;
        }

        Status = AuthSessionStatus.Expired;
        RevokedAt = utcNow;
        RaiseDomainEvent(new AuthSessionExpired
        {
            SessionId = Id,
            UserId = UserId
        });
    }
}

using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Application.Authentication;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(AuthUser user, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}

public interface IAuthSessionRepository
{
    Task<AuthSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AuthSession?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);

    Task AddAsync(AuthSession session, CancellationToken cancellationToken = default);
}

public interface ILoginHistoryRepository
{
    Task AddAsync(LoginHistoryEntry entry, CancellationToken cancellationToken = default);
}

public interface IPlatformUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}

public interface ITokenService
{
    IssuedAccessToken CreateAccessToken(
        AuthUser user,
        Guid sessionId,
        Guid accessTokenId,
        string companyId,
        string plantId,
        DateTimeOffset issuedAt);

    string CreateRefreshToken();

    string HashRefreshToken(string refreshToken);
}

public sealed record IssuedAccessToken(string Token, Guid TokenId, DateTimeOffset ExpiresAt, int ExpiresInSeconds);

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public interface IAuthRequestContext
{
    string? IpAddress { get; }

    string CorrelationId { get; }

    Guid? UserId { get; }

    Guid? SessionId { get; }

    string? CompanyId { get; }

    string? PlantId { get; }
}

public interface IOutboxWriter
{
    Task EnqueueAsync(
        string eventType,
        object payload,
        Guid? userId,
        string correlationId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default);
}

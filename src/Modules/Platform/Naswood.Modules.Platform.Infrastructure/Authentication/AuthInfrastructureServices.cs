using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Naswood.Modules.Platform.Infrastructure.Authentication;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private readonly AuthenticationOptions _options;

    public BcryptPasswordHasher(IOptions<AuthenticationOptions> options) => _options = options.Value;

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, _options.BcryptWorkFactor);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

public sealed class JwtTokenService : ITokenService
{
    private readonly AuthenticationOptions _options;

    public JwtTokenService(IOptions<AuthenticationOptions> options) => _options = options.Value;

    public IssuedAccessToken CreateAccessToken(
        AuthUser user,
        Guid sessionId,
        Guid accessTokenId,
        string companyId,
        string plantId,
        DateTimeOffset issuedAt)
    {
        var expires = issuedAt.AddMinutes(_options.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString("D")),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Jti, accessTokenId.ToString("D")),
            new("session_id", sessionId.ToString("D")),
            new("company_id", companyId),
            new("plant_id", plantId)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        var encoded = new JwtSecurityTokenHandler().WriteToken(token);
        return new IssuedAccessToken(
            encoded,
            accessTokenId,
            expires,
            (int)TimeSpan.FromMinutes(_options.AccessTokenMinutes).TotalSeconds);
    }

    public string CreateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hash);
    }
}

public sealed class EfOutboxWriter : IOutboxWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly PlatformDbContext _db;
    private readonly IClock _clock;

    public EfOutboxWriter(PlatformDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task EnqueueAsync(
        string eventType,
        object payload,
        Guid? userId,
        string correlationId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default)
    {
        var message = new OutboxMessage
        {
            Id = UuidV7.NewGuid(),
            EventType = eventType,
            Payload = JsonSerializer.Serialize(payload, JsonOptions),
            UserId = userId,
            CorrelationId = correlationId,
            OccurredAt = occurredAt,
            CreatedAt = _clock.UtcNow
        };

        await _db.OutboxMessages.AddAsync(message, cancellationToken).ConfigureAwait(false);
    }
}

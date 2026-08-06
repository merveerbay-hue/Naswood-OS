using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Infrastructure.Persistence;

public sealed class AuthUserRepository : IAuthUserRepository
{
    private readonly PlatformDbContext _db;

    public AuthUserRepository(PlatformDbContext db) => _db = db;

    public Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        _db.AuthUsers.FirstOrDefaultAsync(
            x => x.Username == username,
            cancellationToken);

    public Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.AuthUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(AuthUser user, CancellationToken cancellationToken = default) =>
        await _db.AuthUsers.AddAsync(user, cancellationToken).ConfigureAwait(false);

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _db.AuthUsers.AnyAsync(cancellationToken);
}

public sealed class AuthSessionRepository : IAuthSessionRepository
{
    private readonly PlatformDbContext _db;

    public AuthSessionRepository(PlatformDbContext db) => _db = db;

    public Task<AuthSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.AuthSessions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<AuthSession?> GetByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default) =>
        _db.AuthSessions.FirstOrDefaultAsync(
            x => x.RefreshTokenHash == refreshTokenHash,
            cancellationToken);

    public async Task AddAsync(AuthSession session, CancellationToken cancellationToken = default) =>
        await _db.AuthSessions.AddAsync(session, cancellationToken).ConfigureAwait(false);
}

public sealed class LoginHistoryRepository : ILoginHistoryRepository
{
    private readonly PlatformDbContext _db;

    public LoginHistoryRepository(PlatformDbContext db) => _db = db;

    public async Task AddAsync(LoginHistoryEntry entry, CancellationToken cancellationToken = default) =>
        await _db.LoginHistory.AddAsync(entry, cancellationToken).ConfigureAwait(false);
}

public sealed class PlatformUnitOfWork : IPlatformUnitOfWork
{
    private readonly PlatformDbContext _db;

    public PlatformUnitOfWork(PlatformDbContext db) => _db = db;

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}

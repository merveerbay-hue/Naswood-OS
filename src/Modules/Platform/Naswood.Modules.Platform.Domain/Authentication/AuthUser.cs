using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authentication;

/// <summary>
/// Credential identity used by Authentication. User Management CRUD is out of
/// scope for TASK-001; this aggregate stores only auth-required state.
/// </summary>
public sealed class AuthUser : AggregateRoot<Guid>
{
    public const int MaxFailedAttempts = 5;

    private readonly List<string> _companyIds = [];
    private readonly List<string> _plantIds = [];
    private readonly List<string> _roles = [];

    private AuthUser()
    {
    }

    private AuthUser(
        Guid id,
        string username,
        string displayName,
        string? email,
        string passwordHash,
        IEnumerable<string> companyIds,
        IEnumerable<string> plantIds,
        IEnumerable<string> roles,
        DateTimeOffset? passwordExpiresAt)
        : base(id)
    {
        Username = username;
        DisplayName = displayName;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
        IsDeleted = false;
        IsLocked = false;
        FailedLoginCount = 0;
        PasswordExpiresAt = passwordExpiresAt;
        _companyIds.AddRange(companyIds);
        _plantIds.AddRange(plantIds);
        _roles.AddRange(roles);
    }

    public string Username { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public bool IsDeleted { get; private set; }

    public bool IsLocked { get; private set; }

    public string? LockReason { get; private set; }

    public int FailedLoginCount { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public DateTimeOffset? PasswordExpiresAt { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    public IReadOnlyCollection<string> CompanyIds => _companyIds.AsReadOnly();

    public IReadOnlyCollection<string> PlantIds => _plantIds.AsReadOnly();

    public IReadOnlyCollection<string> Roles => _roles.AsReadOnly();

    public static AuthUser Create(
        string username,
        string displayName,
        string? email,
        string passwordHash,
        IEnumerable<string> companyIds,
        IEnumerable<string> plantIds,
        IEnumerable<string> roles,
        DateTimeOffset? passwordExpiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        return new AuthUser(
            UuidV7.NewGuid(),
            username.Trim(),
            displayName.Trim(),
            string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            passwordHash,
            companyIds.Select(c => c.Trim()).Where(c => c.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase),
            plantIds.Select(p => p.Trim()).Where(p => p.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase),
            roles.Select(r => r.Trim()).Where(r => r.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase),
            passwordExpiresAt);
    }

    public Result EnsureCanAuthenticate(DateTimeOffset utcNow)
    {
        if (IsDeleted || !IsActive)
        {
            return Result.Failure(AuthErrors.AccountDisabled());
        }

        if (IsLocked)
        {
            return Result.Failure(AuthErrors.AccountLocked());
        }

        if (PasswordExpiresAt is not null && PasswordExpiresAt <= utcNow)
        {
            return Result.Failure(AuthErrors.PasswordExpired());
        }

        return Result.Success();
    }

    public Result<(string CompanyId, string PlantId)> ResolveCompanyAndPlant(
        string? requestedCompanyId,
        string? requestedPlantId)
    {
        var companyId = ResolveSingleOrRequested(_companyIds, requestedCompanyId);
        if (companyId is null)
        {
            return Result.Failure<(string CompanyId, string PlantId)>(AuthErrors.CompanyOrPlantRequired());
        }

        var plantId = ResolveSingleOrRequested(_plantIds, requestedPlantId);
        if (plantId is null)
        {
            return Result.Failure<(string CompanyId, string PlantId)>(AuthErrors.CompanyOrPlantRequired());
        }

        if (!_companyIds.Contains(companyId, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Failure<(string CompanyId, string PlantId)>(AuthErrors.InvalidCredentials());
        }

        if (!_plantIds.Contains(plantId, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Failure<(string CompanyId, string PlantId)>(AuthErrors.InvalidCredentials());
        }

        return Result.Success((CompanyId: companyId, PlantId: plantId));
    }

    public void RegisterFailedLogin(DateTimeOffset utcNow)
    {
        FailedLoginCount += 1;

        if (FailedLoginCount >= MaxFailedAttempts)
        {
            IsLocked = true;
            LockedAt = utcNow;
            LockReason = "Exceeded maximum failed login attempts.";
            RaiseDomainEvent(new AuthAccountLocked
            {
                UserId = Id,
                Username = Username
            });
        }
    }

    public void RegisterSuccessfulLogin(DateTimeOffset utcNow, Guid sessionId)
    {
        FailedLoginCount = 0;
        LastLoginAt = utcNow;
        RaiseDomainEvent(new AuthUserAuthenticated
        {
            UserId = Id,
            SessionId = sessionId,
            Username = Username
        });
    }

    private static string? ResolveSingleOrRequested(IReadOnlyList<string> assigned, string? requested)
    {
        if (!string.IsNullOrWhiteSpace(requested))
        {
            return requested.Trim();
        }

        return assigned.Count == 1 ? assigned[0] : null;
    }
}

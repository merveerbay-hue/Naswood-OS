using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.Domain.Authentication;

/// <summary>
/// Platform identity aggregate shared by Authentication, Authorization and User Management.
/// Credentials and lifecycle live on one model — User Management must not duplicate identity.
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
        DateTimeOffset? passwordExpiresAt,
        UserAccountStatus status,
        string? employeeNumber,
        string? firstName,
        string? lastName)
        : base(id)
    {
        Username = username;
        DisplayName = displayName;
        Email = email;
        PasswordHash = passwordHash;
        Status = status;
        IsActive = status == UserAccountStatus.Active;
        IsDeleted = false;
        IsLocked = false;
        FailedLoginCount = 0;
        PasswordExpiresAt = passwordExpiresAt;
        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
        _companyIds.AddRange(companyIds);
        _plantIds.AddRange(plantIds);
        _roles.AddRange(roles);
    }

    public string Username { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string PasswordHash { get; private set; } = string.Empty;

    public UserAccountStatus Status { get; private set; } = UserAccountStatus.Active;

    public bool IsActive { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public Guid? DeletedBy { get; private set; }

    public string? DeleteReason { get; private set; }

    public bool IsLocked { get; private set; }

    public string? LockReason { get; private set; }

    public int FailedLoginCount { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public DateTimeOffset? PasswordExpiresAt { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    public string? EmployeeNumber { get; private set; }

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string? Phone { get; private set; }

    public string? MobilePhone { get; private set; }

    public string? AvatarUrl { get; private set; }

    public string? DepartmentCode { get; private set; }

    public string? PositionCode { get; private set; }

    public Guid? ManagerUserId { get; private set; }

    public string? CostCenter { get; private set; }

    public DateOnly? HireDate { get; private set; }

    public string? EmploymentType { get; private set; }

    public string? EmployeeCategory { get; private set; }

    public string? Language { get; private set; }

    public string? TimeZone { get; private set; }

    public string? DateFormat { get; private set; }

    public string? NumberFormat { get; private set; }

    public string? Currency { get; private set; }

    public string? Theme { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public IReadOnlyCollection<string> CompanyIds => _companyIds.AsReadOnly();

    public IReadOnlyCollection<string> PlantIds => _plantIds.AsReadOnly();

    public IReadOnlyCollection<string> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Bootstrap / test factory — creates an immediately Active credential identity.
    /// </summary>
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
            NormalizeOptional(email),
            passwordHash,
            NormalizeCodes(companyIds),
            NormalizeCodes(plantIds),
            NormalizeCodes(roles),
            passwordExpiresAt,
            UserAccountStatus.Active,
            employeeNumber: null,
            firstName: null,
            lastName: null);
    }

    /// <summary>
    /// User Management registration — starts in Pending Activation until activated.
    /// </summary>
    public static AuthUser Register(
        string username,
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        IEnumerable<string> companyIds,
        IEnumerable<string> plantIds,
        IEnumerable<string> roles,
        string? departmentCode,
        string? positionCode,
        Guid? createdBy)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            throw new ArgumentException("Employee number is required.", nameof(employeeNumber));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        var roleList = NormalizeCodes(roles).ToArray();
        if (roleList.Length == 0)
        {
            throw new ArgumentException("At least one role is required.", nameof(roles));
        }

        var companyList = NormalizeCodes(companyIds).ToArray();
        if (companyList.Length == 0)
        {
            throw new ArgumentException("At least one company is required.", nameof(companyIds));
        }

        var plantList = NormalizeCodes(plantIds).ToArray();
        if (plantList.Length == 0)
        {
            throw new ArgumentException("At least one plant is required.", nameof(plantIds));
        }

        var first = firstName.Trim();
        var last = lastName.Trim();
        var user = new AuthUser(
            UuidV7.NewGuid(),
            username.Trim(),
            $"{first} {last}",
            email.Trim(),
            passwordHash,
            companyList,
            plantList,
            roleList,
            passwordExpiresAt: null,
            UserAccountStatus.PendingActivation,
            employeeNumber.Trim(),
            first,
            last)
        {
            DepartmentCode = NormalizeOptionalCode(departmentCode),
            PositionCode = NormalizeOptionalCode(positionCode),
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        user.RaiseDomainEvent(new UserCreated
        {
            UserId = user.Id,
            Username = user.Username
        });

        return user;
    }

    public Result EnsureCanAuthenticate(DateTimeOffset utcNow)
    {
        if (IsDeleted || Status != UserAccountStatus.Active || !IsActive)
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
            Touch(null, utcNow);
            RaiseDomainEvent(new AuthAccountLocked
            {
                UserId = Id,
                Username = Username
            });
            RaiseDomainEvent(new UserLocked
            {
                UserId = Id,
                Username = Username,
                Reason = LockReason
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

    public Result UpdateProfile(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? mobilePhone,
        string? costCenter,
        DateOnly? hireDate,
        string? employmentType,
        string? employeeCategory,
        string? language,
        string? timeZone,
        string? dateFormat,
        string? numberFormat,
        string? currency,
        string? theme,
        Guid? updatedBy,
        DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure(UserErrors.Validation("First name and last name are required."));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure(UserErrors.Validation("Email is required."));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DisplayName = $"{FirstName} {LastName}";
        Email = email.Trim();
        Phone = NormalizeOptional(phone);
        MobilePhone = NormalizeOptional(mobilePhone);
        CostCenter = NormalizeOptional(costCenter);
        HireDate = hireDate;
        EmploymentType = NormalizeOptional(employmentType);
        EmployeeCategory = NormalizeOptional(employeeCategory);
        Language = NormalizeOptional(language);
        TimeZone = NormalizeOptional(timeZone);
        DateFormat = NormalizeOptional(dateFormat);
        NumberFormat = NormalizeOptional(numberFormat);
        Currency = NormalizeOptional(currency);
        Theme = NormalizeOptional(theme);
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserUpdated { UserId = Id });
        return Result.Success();
    }

    public Result Activate(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        if (Status is UserAccountStatus.Archived)
        {
            return Result.Failure(UserErrors.InvalidStatusTransition(Status.ToString(), nameof(UserAccountStatus.Active)));
        }

        Status = UserAccountStatus.Active;
        IsActive = true;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserActivated { UserId = Id });
        return Result.Success();
    }

    public Result Deactivate(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        if (Status is UserAccountStatus.Archived)
        {
            return Result.Failure(UserErrors.InvalidStatusTransition(Status.ToString(), nameof(UserAccountStatus.Inactive)));
        }

        Status = UserAccountStatus.Inactive;
        IsActive = false;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserDeactivated { UserId = Id });
        return Result.Success();
    }

    public Result Suspend(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        Status = UserAccountStatus.Suspended;
        IsActive = false;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserDeactivated { UserId = Id });
        return Result.Success();
    }

    public Result SoftDelete(Guid? deletedBy, string? reason, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        IsDeleted = true;
        IsActive = false;
        Status = UserAccountStatus.Archived;
        DeletedAt = utcNow;
        DeletedBy = deletedBy;
        DeleteReason = NormalizeOptional(reason);
        Touch(deletedBy, utcNow);
        RaiseDomainEvent(new UserSoftDeleted { UserId = Id });
        RaiseDomainEvent(new UserArchived { UserId = Id });
        return Result.Success();
    }

    public Result Lock(string reason, Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure(UserErrors.Validation("Lock reason is required."));
        }

        IsLocked = true;
        LockedAt = utcNow;
        LockReason = reason.Trim();
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserLocked
        {
            UserId = Id,
            Username = Username,
            Reason = LockReason
        });
        return Result.Success();
    }

    public Result Unlock(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        IsLocked = false;
        LockedAt = null;
        LockReason = null;
        FailedLoginCount = 0;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserUnlocked { UserId = Id });
        return Result.Success();
    }

    public Result ResetPassword(string passwordHash, Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure(UserErrors.Validation("Password hash is required."));
        }

        PasswordHash = passwordHash;
        PasswordExpiresAt = null;
        FailedLoginCount = 0;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserPasswordReset { UserId = Id });
        return Result.Success();
    }

    public Result AssignOrganization(
        IEnumerable<string> companyIds,
        IEnumerable<string> plantIds,
        string? departmentCode,
        string? positionCode,
        Guid? managerUserId,
        Guid? updatedBy,
        DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        var companies = NormalizeCodes(companyIds).ToArray();
        var plants = NormalizeCodes(plantIds).ToArray();
        if (companies.Length == 0)
        {
            return Result.Failure(UserErrors.Validation("At least one company is required."));
        }

        if (plants.Length == 0)
        {
            return Result.Failure(UserErrors.Validation("At least one plant is required."));
        }

        _companyIds.Clear();
        _companyIds.AddRange(companies);
        _plantIds.Clear();
        _plantIds.AddRange(plants);
        DepartmentCode = NormalizeOptionalCode(departmentCode);
        PositionCode = NormalizeOptionalCode(positionCode);
        ManagerUserId = managerUserId;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserOrganizationChanged { UserId = Id });
        RaiseDomainEvent(new UserPlantAssigned { UserId = Id, PlantIds = plants });
        return Result.Success();
    }

    public Result AssignRoles(IEnumerable<string> roles, Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        var roleList = NormalizeCodes(roles).ToArray();
        if (roleList.Length == 0)
        {
            return Result.Failure(UserErrors.RoleRequired());
        }

        _roles.Clear();
        _roles.AddRange(roleList);
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserRoleAssigned { UserId = Id, Roles = roleList });
        return Result.Success();
    }

    public Result AssignPlants(IEnumerable<string> plantIds, Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(UserErrors.NotFound());
        }

        var plants = NormalizeCodes(plantIds).ToArray();
        if (plants.Length == 0)
        {
            return Result.Failure(UserErrors.Validation("At least one plant is required."));
        }

        _plantIds.Clear();
        _plantIds.AddRange(plants);
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new UserOrganizationChanged { UserId = Id });
        RaiseDomainEvent(new UserPlantAssigned { UserId = Id, PlantIds = plants });
        return Result.Success();
    }

    private void Touch(Guid? actorId, DateTimeOffset utcNow)
    {
        UpdatedAt = utcNow;
        UpdatedBy = actorId;
    }

    private static string? ResolveSingleOrRequested(IReadOnlyList<string> assigned, string? requested)
    {
        if (!string.IsNullOrWhiteSpace(requested))
        {
            return requested.Trim();
        }

        return assigned.Count == 1 ? assigned[0] : null;
    }

    private static IEnumerable<string> NormalizeCodes(IEnumerable<string> values) =>
        values
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeOptionalCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}

using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

/// <summary>
/// Role aggregates permission codes. Mutations owned by TASK-004.
/// </summary>
public sealed class RoleDefinition : AggregateRoot<Guid>
{
    public static readonly HashSet<string> SystemRoleCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Administrator",
        "ReadOnly"
    };

    private readonly List<string> _permissionCodes = [];

    private RoleDefinition()
    {
    }

    private RoleDefinition(
        Guid id,
        string code,
        string name,
        IEnumerable<string> permissionCodes,
        bool isActive,
        bool isSystem,
        string? description,
        string? companyCode,
        string? plantCode,
        string? departmentCode,
        string? category,
        RoleLifecycleStatus status)
        : base(id)
    {
        Code = code;
        Name = name;
        IsActive = isActive;
        IsSystem = isSystem;
        Description = description;
        CompanyCode = companyCode;
        PlantCode = plantCode;
        DepartmentCode = departmentCode;
        Category = category;
        Status = status;
        Version = 1;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
        _permissionCodes.AddRange(permissionCodes);
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? CompanyCode { get; private set; }

    public string? PlantCode { get; private set; }

    public string? DepartmentCode { get; private set; }

    public string? Category { get; private set; }

    public RoleLifecycleStatus Status { get; private set; } = RoleLifecycleStatus.Active;

    public bool IsActive { get; private set; }

    public bool IsSystem { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public Guid? DeletedBy { get; private set; }

    public int Version { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public IReadOnlyCollection<string> PermissionCodes => _permissionCodes.AsReadOnly();

    public static RoleDefinition Create(
        string code,
        string name,
        IEnumerable<string> permissionCodes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedCode = code.Trim();
        return new RoleDefinition(
            UuidV7.NewGuid(),
            normalizedCode,
            name.Trim(),
            NormalizePermissions(permissionCodes),
            isActive: true,
            isSystem: SystemRoleCodes.Contains(normalizedCode),
            description: null,
            companyCode: null,
            plantCode: null,
            departmentCode: null,
            category: null,
            status: RoleLifecycleStatus.Active);
    }

    public static RoleDefinition CreateManaged(
        string code,
        string name,
        string? description,
        string? companyCode,
        string? plantCode,
        string? departmentCode,
        string? category,
        IEnumerable<string> permissionCodes,
        Guid? createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var role = new RoleDefinition(
            UuidV7.NewGuid(),
            code.Trim(),
            name.Trim(),
            NormalizePermissions(permissionCodes),
            isActive: true,
            isSystem: SystemRoleCodes.Contains(code.Trim()),
            description: NormalizeOptional(description),
            companyCode: NormalizeOptionalCode(companyCode),
            plantCode: NormalizeOptionalCode(plantCode),
            departmentCode: NormalizeOptionalCode(departmentCode),
            category: NormalizeOptional(category),
            status: RoleLifecycleStatus.Active)
        {
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        role.RaiseDomainEvent(new RoleCreated { RoleId = role.Id, Code = role.Code });
        return role;
    }

    public RoleDefinition Clone(string newCode, string newName, Guid? createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);

        var clone = CreateManaged(
            newCode,
            newName,
            Description,
            CompanyCode,
            PlantCode,
            DepartmentCode,
            Category,
            _permissionCodes,
            createdBy);

        clone.RaiseDomainEvent(new RoleCloned { SourceRoleId = Id, RoleId = clone.Id });
        return clone;
    }

    public Result Update(
        string name,
        string? description,
        string? companyCode,
        string? plantCode,
        string? departmentCode,
        string? category,
        IEnumerable<string>? permissionCodes,
        Guid? updatedBy,
        DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(RoleErrors.Validation("Role name is required."));
        }

        Name = name.Trim();
        Description = NormalizeOptional(description);
        CompanyCode = NormalizeOptionalCode(companyCode);
        PlantCode = NormalizeOptionalCode(plantCode);
        DepartmentCode = NormalizeOptionalCode(departmentCode);
        Category = NormalizeOptional(category);

        if (permissionCodes is not null)
        {
            _permissionCodes.Clear();
            _permissionCodes.AddRange(NormalizePermissions(permissionCodes));
            RaiseDomainEvent(new RolePermissionChanged
            {
                RoleId = Id,
                Permissions = _permissionCodes.ToArray()
            });
        }

        Version += 1;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new RoleUpdated { RoleId = Id });
        return Result.Success();
    }

    public Result AssignPermissions(
        IEnumerable<string> permissionCodes,
        Guid? updatedBy,
        DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        var added = false;
        foreach (var code in NormalizePermissions(permissionCodes))
        {
            if (_permissionCodes.Contains(code, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            _permissionCodes.Add(code);
            added = true;
        }

        if (added)
        {
            Version += 1;
            Touch(updatedBy, utcNow);
            RaiseDomainEvent(new RolePermissionChanged
            {
                RoleId = Id,
                Permissions = _permissionCodes.ToArray()
            });
            RaiseDomainEvent(new RoleUpdated { RoleId = Id });
        }

        return Result.Success();
    }

    public Result RemovePermissions(
        IEnumerable<string> permissionCodes,
        Guid? updatedBy,
        DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        var removed = false;
        foreach (var code in NormalizePermissions(permissionCodes))
        {
            var existing = _permissionCodes.FirstOrDefault(p =>
                string.Equals(p, code, StringComparison.OrdinalIgnoreCase));
            if (existing is null)
            {
                continue;
            }

            _permissionCodes.Remove(existing);
            removed = true;
        }

        if (removed)
        {
            Version += 1;
            Touch(updatedBy, utcNow);
            RaiseDomainEvent(new RolePermissionChanged
            {
                RoleId = Id,
                Permissions = _permissionCodes.ToArray()
            });
            RaiseDomainEvent(new RoleUpdated { RoleId = Id });
        }

        return Result.Success();
    }

    public Result Activate(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        IsActive = true;
        Status = RoleLifecycleStatus.Active;
        Version += 1;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new RoleActivated { RoleId = Id });
        return Result.Success();
    }

    public Result Deactivate(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        if (IsSystem)
        {
            return Result.Failure(RoleErrors.Validation("System roles cannot be deactivated."));
        }

        IsActive = false;
        Status = RoleLifecycleStatus.Inactive;
        Version += 1;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new RoleDeactivated { RoleId = Id });
        return Result.Success();
    }

    public Result SoftDelete(Guid? deletedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(RoleErrors.NotFound());
        }

        if (IsSystem || SystemRoleCodes.Contains(Code))
        {
            return Result.Failure(RoleErrors.SystemRoleProtected());
        }

        IsDeleted = true;
        IsActive = false;
        Status = RoleLifecycleStatus.Archived;
        DeletedAt = utcNow;
        DeletedBy = deletedBy;
        Version += 1;
        Touch(deletedBy, utcNow);
        RaiseDomainEvent(new RoleSoftDeleted { RoleId = Id });
        return Result.Success();
    }

    public bool HasPermission(string permissionCode) =>
        IsActive &&
        !IsDeleted &&
        _permissionCodes.Contains(permissionCode, StringComparer.OrdinalIgnoreCase);

    private void Touch(Guid? actorId, DateTimeOffset utcNow)
    {
        UpdatedAt = utcNow;
        UpdatedBy = actorId;
    }

    private static IEnumerable<string> NormalizePermissions(IEnumerable<string> permissionCodes) =>
        permissionCodes
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeOptionalCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}

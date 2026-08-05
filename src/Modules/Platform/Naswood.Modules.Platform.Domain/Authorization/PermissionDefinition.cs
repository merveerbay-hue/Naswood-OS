using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

/// <summary>
/// Permission catalog entry. Soft-deactivation preferred; reserved seeds cannot be deleted.
/// </summary>
public sealed class PermissionDefinition : AggregateRoot<Guid>
{
    private readonly List<string> _dependsOn = [];

    private PermissionDefinition()
    {
    }

    private PermissionDefinition(
        Guid id,
        string code,
        string module,
        string? entity,
        string action,
        string? field,
        string displayName,
        bool isActive,
        bool isReserved,
        string? category,
        string? description)
        : base(id)
    {
        Code = code;
        Module = module;
        Entity = entity;
        Action = action;
        Field = field;
        DisplayName = displayName;
        IsActive = isActive;
        IsReserved = isReserved;
        Category = category;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public string Code { get; private set; } = string.Empty;

    public string Module { get; private set; } = string.Empty;

    /// <summary>Feature / document entity (TASK example "feature").</summary>
    public string? Entity { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? Field { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;

    public string? Category { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsReserved { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public IReadOnlyCollection<string> DependsOn => _dependsOn.AsReadOnly();

    public static PermissionDefinition Create(
        string code,
        string module,
        string action,
        string displayName,
        string? entity = null,
        string? field = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(module);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        return new PermissionDefinition(
            UuidV7.NewGuid(),
            code.Trim(),
            module.Trim(),
            string.IsNullOrWhiteSpace(entity) ? null : entity.Trim(),
            action.Trim(),
            string.IsNullOrWhiteSpace(field) ? null : field.Trim(),
            displayName.Trim(),
            isActive: true,
            isReserved: true,
            category: null,
            description: null);
    }

    public static PermissionDefinition CreateManaged(
        string code,
        string module,
        string action,
        string displayName,
        string? entity,
        string? field,
        string? category,
        string? description,
        IEnumerable<string>? dependsOn,
        Guid? createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(module);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        var permission = new PermissionDefinition(
            UuidV7.NewGuid(),
            code.Trim(),
            module.Trim(),
            string.IsNullOrWhiteSpace(entity) ? null : entity.Trim(),
            action.Trim(),
            string.IsNullOrWhiteSpace(field) ? null : field.Trim(),
            displayName.Trim(),
            isActive: true,
            isReserved: false,
            category: string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
            description: string.IsNullOrWhiteSpace(description) ? null : description.Trim())
        {
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        if (dependsOn is not null)
        {
            permission._dependsOn.AddRange(
                dependsOn.Select(d => d.Trim()).Where(d => d.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        permission.RaiseDomainEvent(new PermissionCreated
        {
            PermissionId = permission.Id,
            Code = permission.Code
        });
        return permission;
    }

    public Result Update(
        string displayName,
        string? category,
        string? description,
        IEnumerable<string>? dependsOn,
        Guid? updatedBy,
        DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(PermissionErrors.NotFound());
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result.Failure(PermissionErrors.Validation("Display name is required."));
        }

        DisplayName = displayName.Trim();
        Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (dependsOn is not null)
        {
            _dependsOn.Clear();
            _dependsOn.AddRange(
                dependsOn.Select(d => d.Trim()).Where(d => d.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new PermissionUpdated { PermissionId = Id });
        return Result.Success();
    }

    public Result SoftDelete(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(PermissionErrors.NotFound());
        }

        if (IsReserved)
        {
            return Result.Failure(PermissionErrors.ReservedProtected());
        }

        IsDeleted = true;
        IsActive = false;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new PermissionDeactivated { PermissionId = Id });
        return Result.Success();
    }

    public Result Deactivate(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(PermissionErrors.NotFound());
        }

        IsActive = false;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new PermissionDeactivated { PermissionId = Id });
        return Result.Success();
    }

    public Result Activate(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsDeleted)
        {
            return Result.Failure(PermissionErrors.NotFound());
        }

        IsActive = true;
        Touch(updatedBy, utcNow);
        RaiseDomainEvent(new PermissionUpdated { PermissionId = Id });
        return Result.Success();
    }

    private void Touch(Guid? actorId, DateTimeOffset utcNow)
    {
        UpdatedAt = utcNow;
        UpdatedBy = actorId;
    }
}

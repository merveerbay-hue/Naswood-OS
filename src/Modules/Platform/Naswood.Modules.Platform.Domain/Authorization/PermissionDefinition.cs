using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

/// <summary>
/// Immutable permission definition. Codes are stable; TASK-005 owns mutations later.
/// </summary>
public sealed class PermissionDefinition : Entity<Guid>
{
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
        bool isActive)
        : base(id)
    {
        Code = code;
        Module = module;
        Entity = entity;
        Action = action;
        Field = field;
        DisplayName = displayName;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;

    public string Module { get; private set; } = string.Empty;

    public string? Entity { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? Field { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

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
            isActive: true);
    }
}

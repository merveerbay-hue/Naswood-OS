using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

/// <summary>
/// Role aggregates permission codes. Mutations owned by TASK-004; seeded here for evaluation.
/// </summary>
public sealed class RoleDefinition : AggregateRoot<Guid>
{
    private readonly List<string> _permissionCodes = [];

    private RoleDefinition()
    {
    }

    private RoleDefinition(
        Guid id,
        string code,
        string name,
        IEnumerable<string> permissionCodes,
        bool isActive)
        : base(id)
    {
        Code = code;
        Name = name;
        IsActive = isActive;
        _permissionCodes.AddRange(permissionCodes);
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<string> PermissionCodes => _permissionCodes.AsReadOnly();

    public static RoleDefinition Create(
        string code,
        string name,
        IEnumerable<string> permissionCodes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new RoleDefinition(
            UuidV7.NewGuid(),
            code.Trim(),
            name.Trim(),
            permissionCodes
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase),
            isActive: true);
    }

    public bool HasPermission(string permissionCode) =>
        IsActive &&
        _permissionCodes.Contains(permissionCode, StringComparer.OrdinalIgnoreCase);
}

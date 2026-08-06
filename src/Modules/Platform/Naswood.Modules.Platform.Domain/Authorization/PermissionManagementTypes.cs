using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

public static class PermissionErrors
{
    public static Error NotFound() =>
        Error.NotFound("PERM-001", "Permission was not found.");

    public static Error CodeTaken() =>
        Error.Conflict("PERM-002", "Permission code is already in use.");

    public static Error ReservedProtected() =>
        Error.Conflict("PERM-003", "Reserved permissions cannot be deleted.");

    public static Error InvalidModule(string module) =>
        Error.Validation("PERM-004", $"Module '{module}' is not recognized.");

    public static Error InvalidAction(string action) =>
        Error.Validation("PERM-005", $"Action '{action}' is not a valid standard or known action.");

    public static Error Validation(string message) =>
        Error.Validation("PERM-006", message);

    public static Error DependencyMissing(string code) =>
        Error.Validation("PERM-007", $"Permission dependency '{code}' does not exist.");
}

public sealed record PermissionCreated : DomainEventBase
{
    public required Guid PermissionId { get; init; }

    public required string Code { get; init; }
}

public sealed record PermissionUpdated : DomainEventBase
{
    public required Guid PermissionId { get; init; }
}

public sealed record PermissionDeactivated : DomainEventBase
{
    public required Guid PermissionId { get; init; }
}

/// <summary>
/// Canonical modules from TASK-005 Module Permissions section.
/// </summary>
public static class PermissionModules
{
    public static readonly HashSet<string> Known = new(StringComparer.OrdinalIgnoreCase)
    {
        "Platform",
        "Inventory",
        "Purchasing",
        "Sales",
        "Production",
        "Quality",
        "Maintenance",
        "Finance",
        "Analytics",
        "AI",
        "Authorization",
        "Administration",
        "Warehouse"
    };
}

/// <summary>
/// Standard actions from TASK-005 plus common platform actions already seeded.
/// </summary>
public static class PermissionActions
{
    public static readonly HashSet<string> Standard = new(StringComparer.OrdinalIgnoreCase)
    {
        "View", "Create", "Edit", "Update", "Delete", "Approve", "Reject", "Release", "Cancel",
        "Print", "Export", "Import", "Archive", "Execute", "Manage", "Configure", "Assign",
        "Audit", "Chat", "Own", "Lock", "Unlock", "ResetPassword", "AssignRole", "Clone"
    };
}

using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

public enum RoleLifecycleStatus
{
    Draft = 0,
    Configured = 1,
    Active = 2,
    Inactive = 3,
    Archived = 4
}

public static class RoleErrors
{
    public static Error NotFound() =>
        Error.NotFound("ROLE-001", "Role was not found.");

    public static Error CodeTaken() =>
        Error.Conflict("ROLE-002", "Role code is already in use.");

    public static Error NameTaken() =>
        Error.Conflict("ROLE-003", "Role name is already in use within the company.");

    public static Error SystemRoleProtected() =>
        Error.Conflict("ROLE-004", "Reserved system roles cannot be deleted.");

    public static Error PermissionNotFound(string code) =>
        Error.Validation("ROLE-005", $"Permission '{code}' does not exist.");

    public static Error Validation(string message) =>
        Error.Validation("ROLE-006", message);

    public static Error InactiveCannotAssign() =>
        Error.Validation("ROLE-007", "Inactive roles cannot be assigned.");

    public static Error CompanyNotFound(string code) =>
        Error.Validation("ROLE-008", $"Company '{code}' does not exist.");
}

public sealed record RoleCreated : DomainEventBase
{
    public required Guid RoleId { get; init; }

    public required string Code { get; init; }
}

public sealed record RoleUpdated : DomainEventBase
{
    public required Guid RoleId { get; init; }
}

public sealed record RoleActivated : DomainEventBase
{
    public required Guid RoleId { get; init; }
}

public sealed record RoleDeactivated : DomainEventBase
{
    public required Guid RoleId { get; init; }
}

public sealed record RolePermissionChanged : DomainEventBase
{
    public required Guid RoleId { get; init; }

    public required IReadOnlyList<string> Permissions { get; init; }
}

public sealed record RoleCloned : DomainEventBase
{
    public required Guid SourceRoleId { get; init; }

    public required Guid RoleId { get; init; }
}

public sealed record RoleSoftDeleted : DomainEventBase
{
    public required Guid RoleId { get; init; }
}

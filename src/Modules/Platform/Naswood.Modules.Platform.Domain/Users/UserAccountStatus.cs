namespace Naswood.Modules.Platform.Domain.Users;

/// <summary>
/// User account lifecycle states from User Management design / TASK-003.
/// Only <see cref="Active"/> users may authenticate (unless also locked).
/// </summary>
public enum UserAccountStatus
{
    Draft = 0,
    PendingActivation = 1,
    Active = 2,
    Suspended = 3,
    Inactive = 4,
    Archived = 5
}

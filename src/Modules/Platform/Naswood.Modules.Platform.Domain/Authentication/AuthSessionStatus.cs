namespace Naswood.Modules.Platform.Domain.Authentication;

public enum AuthSessionStatus
{
    Active = 0,
    Refreshed = 1,
    Expired = 2,
    Revoked = 3,
    Closed = 4
}

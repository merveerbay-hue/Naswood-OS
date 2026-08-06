using Naswood.Modules.Platform.Contracts.Users;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Application.Users;

public static class UserDtoMapper
{
    public static UserDto ToDto(AuthUser user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        EmployeeNumber = user.EmployeeNumber,
        FirstName = user.FirstName,
        LastName = user.LastName,
        DisplayName = user.DisplayName,
        Email = user.Email,
        Phone = user.Phone,
        MobilePhone = user.MobilePhone,
        AvatarUrl = user.AvatarUrl,
        Status = user.Status.ToString(),
        IsActive = user.IsActive,
        IsLocked = user.IsLocked,
        LockReason = user.LockReason,
        CompanyIds = user.CompanyIds.ToArray(),
        PlantIds = user.PlantIds.ToArray(),
        Roles = user.Roles.ToArray(),
        DepartmentCode = user.DepartmentCode,
        PositionCode = user.PositionCode,
        ManagerUserId = user.ManagerUserId,
        CostCenter = user.CostCenter,
        HireDate = user.HireDate,
        EmploymentType = user.EmploymentType,
        EmployeeCategory = user.EmployeeCategory,
        Language = user.Language,
        TimeZone = user.TimeZone,
        DateFormat = user.DateFormat,
        NumberFormat = user.NumberFormat,
        Currency = user.Currency,
        Theme = user.Theme,
        LastLoginAt = user.LastLoginAt,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };
}

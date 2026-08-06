using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public sealed record RoleSearchCriteria(
    string? Code,
    string? Name,
    string? CompanyCode,
    string? PlantCode,
    bool? IsActive,
    int Page,
    int PageSize);

public interface IRoleManagementRepository
{
    Task<RoleDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RoleDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, Guid? excludingRoleId, CancellationToken cancellationToken = default);

    Task<bool> NameExistsInCompanyAsync(
        string name,
        string? companyCode,
        Guid? excludingRoleId,
        CancellationToken cancellationToken = default);

    Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<RoleDefinition> Items, int TotalCount)> SearchAsync(
        RoleSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> FindUserIdsByRoleCodeAsync(
        string roleCode,
        CancellationToken cancellationToken = default);
}

public sealed record GetRoleByIdQuery(Guid RoleId) : IQuery<Result<RoleDetailDto>>;

public sealed record SearchRolesQuery(
    string? Code,
    string? Name,
    string? CompanyCode,
    string? PlantCode,
    bool? IsActive,
    int Page,
    int PageSize) : IQuery<Result<PagedRolesDto>>;

public sealed record CreateRoleCommand(
    string Code,
    string Name,
    string? Description,
    string? CompanyCode,
    string? PlantCode,
    string? DepartmentCode,
    string? Category,
    IReadOnlyList<string> Permissions) : ICommand<Result<RoleDetailDto>>;

public sealed record UpdateRoleCommand(
    Guid RoleId,
    string Name,
    string? Description,
    string? CompanyCode,
    string? PlantCode,
    string? DepartmentCode,
    string? Category,
    IReadOnlyList<string>? Permissions) : ICommand<Result<RoleDetailDto>>;

public sealed record DeleteRoleCommand(Guid RoleId) : ICommand<Result>;

public sealed record CloneRoleCommand(Guid RoleId, string Code, string Name) : ICommand<Result<RoleDetailDto>>;

public sealed record ActivateRoleCommand(Guid RoleId) : ICommand<Result<RoleDetailDto>>;

public sealed record DeactivateRoleCommand(Guid RoleId) : ICommand<Result<RoleDetailDto>>;

public sealed record AssignRolePermissionsCommand(
    Guid RoleId,
    IReadOnlyList<string> Permissions) : ICommand<Result<RoleDetailDto>>;

public sealed record RemoveRolePermissionsCommand(
    Guid RoleId,
    IReadOnlyList<string> Permissions) : ICommand<Result<RoleDetailDto>>;

public sealed record AssignRoleToUserCommand(Guid RoleId, Guid UserId) : ICommand<Result>;

public sealed record RemoveRoleFromUserCommand(Guid RoleId, Guid UserId) : ICommand<Result>;

public static class RoleDtoMapper
{
    public static RoleDetailDto ToDetail(RoleDefinition role) => new()
    {
        Id = role.Id,
        Code = role.Code,
        Name = role.Name,
        Description = role.Description,
        CompanyCode = role.CompanyCode,
        PlantCode = role.PlantCode,
        DepartmentCode = role.DepartmentCode,
        Category = role.Category,
        Status = role.Status.ToString(),
        IsActive = role.IsActive,
        IsSystem = role.IsSystem,
        Version = role.Version,
        Permissions = role.PermissionCodes.ToArray(),
        CreatedAt = role.CreatedAt,
        UpdatedAt = role.UpdatedAt
    };
}

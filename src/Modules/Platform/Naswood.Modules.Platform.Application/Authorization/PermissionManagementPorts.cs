using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public sealed record PermissionSearchCriteria(
    string? Code,
    string? Module,
    string? Feature,
    string? Action,
    string? Category,
    bool? IsActive,
    int Page,
    int PageSize);

public interface IPermissionManagementRepository
{
    Task<PermissionDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PermissionDefinition?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken = default);

    Task AddAsync(PermissionDefinition permission, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PermissionDefinition> Items, int TotalCount)> SearchAsync(
        PermissionSearchCriteria criteria,
        CancellationToken cancellationToken = default);
}

public sealed record SearchPermissionsQuery(
    string? Code,
    string? Module,
    string? Feature,
    string? Action,
    string? Category,
    bool? IsActive,
    int Page,
    int PageSize) : IQuery<Result<PagedPermissionsDto>>;

public sealed record GetPermissionByIdQuery(Guid PermissionId) : IQuery<Result<PermissionDetailDto>>;

public sealed record CreatePermissionCommand(
    string Code,
    string Module,
    string? Feature,
    string Action,
    string? Field,
    string DisplayName,
    string? Category,
    string? Description,
    IReadOnlyList<string>? DependsOn) : ICommand<Result<PermissionDetailDto>>;

public sealed record UpdatePermissionCommand(
    Guid PermissionId,
    string DisplayName,
    string? Category,
    string? Description,
    IReadOnlyList<string>? DependsOn,
    bool? IsActive) : ICommand<Result<PermissionDetailDto>>;

public sealed record DeletePermissionCommand(Guid PermissionId) : ICommand<Result>;

public sealed record GetPermissionTemplatesQuery : IQuery<Result<IReadOnlyList<PermissionTemplateDto>>>;

public sealed record ValidatePermissionCommand(
    string Code,
    string Module,
    string? Feature,
    string Action,
    IReadOnlyList<string>? DependsOn) : ICommand<Result<PermissionValidationResultDto>>;

public static class PermissionDtoMapper
{
    public static PermissionDetailDto ToDetail(PermissionDefinition permission) => new()
    {
        Id = permission.Id,
        Code = permission.Code,
        Module = permission.Module,
        Feature = permission.Entity,
        Action = permission.Action,
        Field = permission.Field,
        DisplayName = permission.DisplayName,
        Category = permission.Category,
        Description = permission.Description,
        IsActive = permission.IsActive,
        IsReserved = permission.IsReserved,
        DependsOn = permission.DependsOn.ToArray(),
        CreatedAt = permission.CreatedAt,
        UpdatedAt = permission.UpdatedAt
    };
}

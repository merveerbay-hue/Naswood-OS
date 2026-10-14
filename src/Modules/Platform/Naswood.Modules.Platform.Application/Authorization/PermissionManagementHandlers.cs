using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Contracts.Authorization;
using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

public sealed class SearchPermissionsQueryHandler
    : IQueryHandler<SearchPermissionsQuery, Result<PagedPermissionsDto>>
{
    private readonly IPermissionManagementRepository _permissions;

    public SearchPermissionsQueryHandler(IPermissionManagementRepository permissions) =>
        _permissions = permissions;

    public async Task<Result<PagedPermissionsDto>> HandleAsync(
        SearchPermissionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 200);
        var (items, total) = await _permissions.SearchAsync(
                new PermissionSearchCriteria(
                    query.Code,
                    query.Module,
                    query.Feature,
                    query.Action,
                    query.Category,
                    query.IsActive,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new PagedPermissionsDto
        {
            Items = items.Select(PermissionDtoMapper.ToDetail).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetPermissionByIdQueryHandler
    : IQueryHandler<GetPermissionByIdQuery, Result<PermissionDetailDto>>
{
    private readonly IPermissionManagementRepository _permissions;

    public GetPermissionByIdQueryHandler(IPermissionManagementRepository permissions) =>
        _permissions = permissions;

    public async Task<Result<PermissionDetailDto>> HandleAsync(
        GetPermissionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var permission = await _permissions.GetByIdAsync(query.PermissionId, cancellationToken)
            .ConfigureAwait(false);
        if (permission is null || permission.IsDeleted)
        {
            return Result.Failure<PermissionDetailDto>(PermissionErrors.NotFound());
        }

        return Result.Success(PermissionDtoMapper.ToDetail(permission));
    }
}

public sealed class GetPermissionTemplatesQueryHandler
    : IQueryHandler<GetPermissionTemplatesQuery, Result<IReadOnlyList<PermissionTemplateDto>>>
{
    public Task<Result<IReadOnlyList<PermissionTemplateDto>>> HandleAsync(
        GetPermissionTemplatesQuery query,
        CancellationToken cancellationToken = default)
    {
        var actions = new[] { "View", "Create", "Update", "Delete", "Export", "Import" };
        IReadOnlyList<PermissionTemplateDto> templates =
        [
            Build("inventory-document", "Inventory Document", "Inventory", "GoodsReceipt", actions),
            Build("purchasing-document", "Purchasing Document", "Purchasing", "PurchaseOrder", actions),
            Build("sales-document", "Sales Document", "Sales", "SalesOrder", actions),
            Build("production-document", "Production Document", "Production", "ProductionOrder", actions),
            Build("quality-document", "Quality Document", "Quality", "QualityInspection", actions),
            Build("maintenance-document", "Maintenance Document", "Maintenance", "MaintenanceOrder", actions),
            Build("finance-module", "Finance Module", "Finance", null, ["View", "Export"]),
            Build("platform-admin", "Platform Administration", "Administration", "User",
                ["View", "Create", "Update", "Delete"])
        ];

        return Task.FromResult(Result.Success(templates));
    }

    private static PermissionTemplateDto Build(
        string key,
        string name,
        string module,
        string? feature,
        IReadOnlyList<string> actions)
    {
        var codes = actions
            .Select(action => feature is null ? $"{module}.{action}" : $"{feature}.{action}")
            .ToArray();

        return new PermissionTemplateDto
        {
            Key = key,
            Name = name,
            Module = module,
            Feature = feature,
            Actions = actions,
            GeneratedCodes = codes
        };
    }
}

public sealed class ValidatePermissionCommandHandler
    : ICommandHandler<ValidatePermissionCommand, Result<PermissionValidationResultDto>>
{
    private readonly IPermissionManagementRepository _permissions;

    public ValidatePermissionCommandHandler(IPermissionManagementRepository permissions) =>
        _permissions = permissions;

    public async Task<Result<PermissionValidationResultDto>> HandleAsync(
        ValidatePermissionCommand command,
        CancellationToken cancellationToken = default)
    {
        var errors = await PermissionRules.CollectValidationErrorsAsync(
                command.Code,
                command.Module,
                command.Feature,
                command.Action,
                command.DependsOn,
                _permissions,
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new PermissionValidationResultDto
        {
            IsValid = errors.Count == 0,
            Errors = errors
        });
    }
}

public sealed class CreatePermissionCommandHandler
    : ICommandHandler<CreatePermissionCommand, Result<PermissionDetailDto>>
{
    private readonly IPermissionManagementRepository _permissions;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public CreatePermissionCommandHandler(
        IPermissionManagementRepository permissions,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _permissions = permissions;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<PermissionDetailDto>> HandleAsync(
        CreatePermissionCommand command,
        CancellationToken cancellationToken = default)
    {
        var errors = await PermissionRules.CollectValidationErrorsAsync(
                command.Code,
                command.Module,
                command.Feature,
                command.Action,
                command.DependsOn,
                _permissions,
                cancellationToken)
            .ConfigureAwait(false);
        if (errors.Count > 0)
        {
            return Result.Failure<PermissionDetailDto>(PermissionErrors.Validation(errors[0]));
        }

        var displayName = string.IsNullOrWhiteSpace(command.DisplayName)
            ? command.Code
            : command.DisplayName;

        var permission = PermissionDefinition.CreateManaged(
            command.Code,
            command.Module,
            command.Action,
            displayName,
            command.Feature,
            command.Field,
            command.Category,
            command.Description,
            command.DependsOn,
            _context.UserId);

        await _permissions.AddAsync(permission, cancellationToken).ConfigureAwait(false);
        await PermissionMutationSupport.PersistAsync(
                permission,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success(PermissionDtoMapper.ToDetail(permission));
    }
}

public sealed class UpdatePermissionCommandHandler
    : ICommandHandler<UpdatePermissionCommand, Result<PermissionDetailDto>>
{
    private readonly IPermissionManagementRepository _permissions;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public UpdatePermissionCommandHandler(
        IPermissionManagementRepository permissions,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _permissions = permissions;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result<PermissionDetailDto>> HandleAsync(
        UpdatePermissionCommand command,
        CancellationToken cancellationToken = default)
    {
        var permission = await _permissions.GetByIdAsync(command.PermissionId, cancellationToken)
            .ConfigureAwait(false);
        if (permission is null || permission.IsDeleted)
        {
            return Result.Failure<PermissionDetailDto>(PermissionErrors.NotFound());
        }

        if (command.DependsOn is not null)
        {
            foreach (var dependency in command.DependsOn)
            {
                if (!await _permissions.CodeExistsAsync(dependency, null, cancellationToken).ConfigureAwait(false))
                {
                    return Result.Failure<PermissionDetailDto>(PermissionErrors.DependencyMissing(dependency));
                }
            }
        }

        var updated = permission.Update(
            command.DisplayName,
            command.Category,
            command.Description,
            command.DependsOn,
            _context.UserId,
            _clock.UtcNow);
        if (updated.IsFailure)
        {
            return Result.Failure<PermissionDetailDto>(updated.Error!);
        }

        if (command.IsActive == true)
        {
            permission.Activate(_context.UserId, _clock.UtcNow);
        }
        else if (command.IsActive == false)
        {
            permission.Deactivate(_context.UserId, _clock.UtcNow);
        }

        await PermissionMutationSupport.PersistAsync(
                permission,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success(PermissionDtoMapper.ToDetail(permission));
    }
}

public sealed class DeletePermissionCommandHandler : ICommandHandler<DeletePermissionCommand, Result>
{
    private readonly IPermissionManagementRepository _permissions;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly IPermissionCache _permissionCache;

    public DeletePermissionCommandHandler(
        IPermissionManagementRepository permissions,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuthRequestContext context,
        IClock clock,
        IPermissionCache permissionCache)
    {
        _permissions = permissions;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _context = context;
        _clock = clock;
        _permissionCache = permissionCache;
    }

    public async Task<Result> HandleAsync(
        DeletePermissionCommand command,
        CancellationToken cancellationToken = default)
    {
        var permission = await _permissions.GetByIdAsync(command.PermissionId, cancellationToken)
            .ConfigureAwait(false);
        if (permission is null || permission.IsDeleted)
        {
            return Result.Failure(PermissionErrors.NotFound());
        }

        var deleted = permission.SoftDelete(_context.UserId, _clock.UtcNow);
        if (deleted.IsFailure)
        {
            return deleted;
        }

        await PermissionMutationSupport.PersistAsync(
                permission,
                _outbox,
                _unitOfWork,
                _context,
                _clock,
                cancellationToken)
            .ConfigureAwait(false);
        _permissionCache.InvalidateAll();
        return Result.Success();
    }
}

internal static class PermissionRules
{
    public static async Task<IReadOnlyList<string>> CollectValidationErrorsAsync(
        string code,
        string module,
        string? feature,
        string action,
        IReadOnlyList<string>? dependsOn,
        IPermissionManagementRepository permissions,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add("Permission code is required.");
        }

        if (string.IsNullOrWhiteSpace(module) || !PermissionModules.Known.Contains(module.Trim()))
        {
            errors.Add(PermissionErrors.InvalidModule(module ?? string.Empty).Message);
        }

        if (string.IsNullOrWhiteSpace(action) || !PermissionActions.Standard.Contains(action.Trim()))
        {
            errors.Add(PermissionErrors.InvalidAction(action ?? string.Empty).Message);
        }

        // Feature maps to Entity; when provided must be non-empty (no separate feature master in Sprint 00).
        if (feature is not null && string.IsNullOrWhiteSpace(feature))
        {
            errors.Add("Feature cannot be blank when provided.");
        }

        if (!string.IsNullOrWhiteSpace(code) &&
            await permissions.CodeExistsAsync(code.Trim(), null, cancellationToken).ConfigureAwait(false))
        {
            errors.Add(PermissionErrors.CodeTaken().Message);
        }

        if (dependsOn is not null)
        {
            foreach (var dependency in dependsOn)
            {
                if (!await permissions.CodeExistsAsync(dependency, null, cancellationToken).ConfigureAwait(false))
                {
                    errors.Add(PermissionErrors.DependencyMissing(dependency).Message);
                }
            }
        }

        return errors;
    }
}

internal static class PermissionMutationSupport
{
    public static async Task PersistAsync(
        PermissionDefinition permission,
        IOutboxWriter outbox,
        IPlatformUnitOfWork unitOfWork,
        IAuthRequestContext context,
        IClock clock,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in permission.DomainEvents)
        {
            await outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    context.UserId,
                    context.CorrelationId,
                    clock.UtcNow,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        permission.ClearDomainEvents();
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

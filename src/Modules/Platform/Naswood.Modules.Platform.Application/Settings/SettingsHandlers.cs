using System.Text.Json;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Audit;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Contracts.Settings;
using Naswood.Modules.Platform.Domain.Settings;

namespace Naswood.Modules.Platform.Application.Settings;

public sealed record SettingSearchCriteria(
    string? Category,
    string? Key,
    string? Scope,
    string? CompanyId,
    string? PlantId,
    int Page,
    int PageSize);

public interface ISettingRepository
{
    Task<SettingEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> KeyExistsAsync(
        string key,
        SettingScope scope,
        string? companyId,
        string? plantId,
        Guid? userId,
        Guid? excludingId,
        CancellationToken cancellationToken = default);

    Task AddAsync(SettingEntry setting, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SettingEntry> Items, int TotalCount)> SearchAsync(
        SettingSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SettingEntry>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}

public sealed record SearchSettingsQuery(
    string? Category,
    string? Key,
    string? Scope,
    string? CompanyId,
    string? PlantId,
    int Page,
    int PageSize) : IQuery<Result<PagedSettingsDto>>;

public sealed record GetSettingByIdQuery(Guid Id) : IQuery<Result<SettingDto>>;

public sealed record GetSettingCategoriesQuery : IQuery<Result<IReadOnlyList<string>>>;

public sealed record CreateSettingCommand(
    string Category,
    string Key,
    string Name,
    string? Description,
    string Value,
    string DataType,
    string? DefaultValue,
    string Scope,
    string? CompanyId,
    string? PlantId,
    Guid? UserId,
    string? ValidationRule,
    bool IsRequired) : ICommand<Result<SettingDto>>;

public sealed record UpdateSettingCommand(Guid Id, string Value) : ICommand<Result<SettingDto>>;

public sealed record ResetSettingCommand(Guid? Id, string? Key) : ICommand<Result<SettingDto>>;

public sealed record ExportSettingsQuery : IQuery<Result<string>>;

public sealed record ImportSettingsCommand(string JsonContent) : ICommand<Result<int>>;

public static class SettingDtoMapper
{
    public static SettingDto ToDto(SettingEntry setting) => new()
    {
        Id = setting.Id,
        Category = setting.Category,
        Key = setting.Key,
        Name = setting.Name,
        Description = setting.Description,
        Value = setting.Value,
        DataType = setting.DataType.ToString(),
        DefaultValue = setting.DefaultValue,
        Scope = setting.Scope.ToString(),
        CompanyId = setting.CompanyId,
        PlantId = setting.PlantId,
        UserId = setting.UserId,
        IsRequired = setting.IsRequired,
        IsSystem = setting.IsSystem,
        IsActive = setting.IsActive,
        Version = setting.Version,
        UpdatedAt = setting.UpdatedAt
    };
}

public sealed class SearchSettingsQueryHandler : IQueryHandler<SearchSettingsQuery, Result<PagedSettingsDto>>
{
    private readonly ISettingRepository _settings;

    public SearchSettingsQueryHandler(ISettingRepository settings) => _settings = settings;

    public async Task<Result<PagedSettingsDto>> HandleAsync(
        SearchSettingsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _settings.SearchAsync(
                new SettingSearchCriteria(
                    query.Category,
                    query.Key,
                    query.Scope,
                    query.CompanyId,
                    query.PlantId,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new PagedSettingsDto
        {
            Items = items.Select(SettingDtoMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetSettingByIdQueryHandler : IQueryHandler<GetSettingByIdQuery, Result<SettingDto>>
{
    private readonly ISettingRepository _settings;

    public GetSettingByIdQueryHandler(ISettingRepository settings) => _settings = settings;

    public async Task<Result<SettingDto>> HandleAsync(
        GetSettingByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var setting = await _settings.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (setting is null || !setting.IsActive)
        {
            return Result.Failure<SettingDto>(SettingErrors.NotFound());
        }

        return Result.Success(SettingDtoMapper.ToDto(setting));
    }
}

public sealed class GetSettingCategoriesQueryHandler
    : IQueryHandler<GetSettingCategoriesQuery, Result<IReadOnlyList<string>>>
{
    public Task<Result<IReadOnlyList<string>>> HandleAsync(
        GetSettingCategoriesQuery query,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success(SettingCategories.All));
}

public sealed class CreateSettingCommandHandler : ICommandHandler<CreateSettingCommand, Result<SettingDto>>
{
    private readonly ISettingRepository _settings;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuditWriter _audit;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public CreateSettingCommandHandler(
        ISettingRepository settings,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuditWriter audit,
        IAuthRequestContext context,
        IClock clock)
    {
        _settings = settings;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _audit = audit;
        _context = context;
        _clock = clock;
    }

    public async Task<Result<SettingDto>> HandleAsync(
        CreateSettingCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<SettingDataType>(command.DataType, true, out var dataType))
        {
            return Result.Failure<SettingDto>(SettingErrors.Validation("Invalid data type."));
        }

        if (!Enum.TryParse<SettingScope>(command.Scope, true, out var scope))
        {
            return Result.Failure<SettingDto>(SettingErrors.Validation("Invalid scope."));
        }

        if (!SettingCategories.All.Contains(command.Category, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Failure<SettingDto>(SettingErrors.Validation($"Unknown category '{command.Category}'."));
        }

        if (!SettingValueValidator.IsValid(dataType, command.Value))
        {
            return Result.Failure<SettingDto>(SettingErrors.InvalidDataType());
        }

        if (await _settings.KeyExistsAsync(
                    command.Key,
                    scope,
                    command.CompanyId,
                    command.PlantId,
                    command.UserId,
                    null,
                    cancellationToken)
                .ConfigureAwait(false))
        {
            return Result.Failure<SettingDto>(SettingErrors.KeyTaken());
        }

        var setting = SettingEntry.Create(
            command.Category,
            command.Key,
            command.Name,
            command.Description,
            command.Value,
            dataType,
            command.DefaultValue ?? command.Value,
            scope,
            command.CompanyId,
            command.PlantId,
            command.UserId,
            command.ValidationRule,
            command.IsRequired,
            isSystem: false,
            _context.UserId);

        await _settings.AddAsync(setting, cancellationToken).ConfigureAwait(false);
        await PersistAsync(setting, "SettingCreated", cancellationToken).ConfigureAwait(false);
        return Result.Success(SettingDtoMapper.ToDto(setting));
    }

    private async Task PersistAsync(SettingEntry setting, string action, CancellationToken cancellationToken)
    {
        await _audit.WriteAsync(
                new Domain.Audit.AuditWriteModel
                {
                    OccurredAt = _clock.UtcNow,
                    UserId = _context.UserId,
                    Module = "Administration",
                    Entity = "Setting",
                    EntityId = setting.Id.ToString("D"),
                    Action = action,
                    NewValuesJson = JsonSerializer.Serialize(new { setting.Key, setting.Value }),
                    CorrelationId = _context.CorrelationId,
                    CompanyId = _context.CompanyId,
                    PlantId = _context.PlantId,
                    IpAddress = _context.IpAddress,
                    SessionId = _context.SessionId
                },
                cancellationToken)
            .ConfigureAwait(false);

        foreach (var domainEvent in setting.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    _context.UserId,
                    _context.CorrelationId,
                    _clock.UtcNow,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        setting.ClearDomainEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

public sealed class UpdateSettingCommandHandler : ICommandHandler<UpdateSettingCommand, Result<SettingDto>>
{
    private readonly ISettingRepository _settings;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuditWriter _audit;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public UpdateSettingCommandHandler(
        ISettingRepository settings,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuditWriter audit,
        IAuthRequestContext context,
        IClock clock)
    {
        _settings = settings;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _audit = audit;
        _context = context;
        _clock = clock;
    }

    public async Task<Result<SettingDto>> HandleAsync(
        UpdateSettingCommand command,
        CancellationToken cancellationToken = default)
    {
        var setting = await _settings.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (setting is null || !setting.IsActive)
        {
            return Result.Failure<SettingDto>(SettingErrors.NotFound());
        }

        var updated = setting.UpdateValue(command.Value, _context.UserId, _clock.UtcNow);
        if (updated.IsFailure)
        {
            return Result.Failure<SettingDto>(updated.Error!);
        }

        await _audit.WriteAsync(
                new Domain.Audit.AuditWriteModel
                {
                    OccurredAt = _clock.UtcNow,
                    UserId = _context.UserId,
                    Module = "Administration",
                    Entity = "Setting",
                    EntityId = setting.Id.ToString("D"),
                    Action = "SettingUpdated",
                    NewValuesJson = JsonSerializer.Serialize(new { setting.Key, setting.Value }),
                    CorrelationId = _context.CorrelationId,
                    CompanyId = _context.CompanyId,
                    PlantId = _context.PlantId,
                    IpAddress = _context.IpAddress,
                    SessionId = _context.SessionId
                },
                cancellationToken)
            .ConfigureAwait(false);

        foreach (var domainEvent in setting.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    _context.UserId,
                    _context.CorrelationId,
                    _clock.UtcNow,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        setting.ClearDomainEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SettingDtoMapper.ToDto(setting));
    }
}

public sealed class ResetSettingCommandHandler : ICommandHandler<ResetSettingCommand, Result<SettingDto>>
{
    private readonly ISettingRepository _settings;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuditWriter _audit;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public ResetSettingCommandHandler(
        ISettingRepository settings,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuditWriter audit,
        IAuthRequestContext context,
        IClock clock)
    {
        _settings = settings;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _audit = audit;
        _context = context;
        _clock = clock;
    }

    public async Task<Result<SettingDto>> HandleAsync(
        ResetSettingCommand command,
        CancellationToken cancellationToken = default)
    {
        SettingEntry? setting = null;
        if (command.Id is not null)
        {
            setting = await _settings.GetByIdAsync(command.Id.Value, cancellationToken).ConfigureAwait(false);
        }
        else if (!string.IsNullOrWhiteSpace(command.Key))
        {
            var (items, _) = await _settings.SearchAsync(
                    new SettingSearchCriteria(null, command.Key, null, null, null, 1, 1),
                    cancellationToken)
                .ConfigureAwait(false);
            setting = items.FirstOrDefault();
        }

        if (setting is null || !setting.IsActive)
        {
            return Result.Failure<SettingDto>(SettingErrors.NotFound());
        }

        var reset = setting.ResetToDefault(_context.UserId, _clock.UtcNow);
        if (reset.IsFailure)
        {
            return Result.Failure<SettingDto>(reset.Error!);
        }

        await _audit.WriteAsync(
                new Domain.Audit.AuditWriteModel
                {
                    OccurredAt = _clock.UtcNow,
                    UserId = _context.UserId,
                    Module = "Administration",
                    Entity = "Setting",
                    EntityId = setting.Id.ToString("D"),
                    Action = "SettingReset",
                    CorrelationId = _context.CorrelationId,
                    CompanyId = _context.CompanyId,
                    PlantId = _context.PlantId,
                    IpAddress = _context.IpAddress,
                    SessionId = _context.SessionId
                },
                cancellationToken)
            .ConfigureAwait(false);

        setting.ClearDomainEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success(SettingDtoMapper.ToDto(setting));
    }
}

public sealed class ExportSettingsQueryHandler : IQueryHandler<ExportSettingsQuery, Result<string>>
{
    private readonly ISettingRepository _settings;

    public ExportSettingsQueryHandler(ISettingRepository settings) => _settings = settings;

    public async Task<Result<string>> HandleAsync(
        ExportSettingsQuery query,
        CancellationToken cancellationToken = default)
    {
        var items = await _settings.ListActiveAsync(cancellationToken).ConfigureAwait(false);
        var payload = items.Select(s => new
        {
            s.Category,
            s.Key,
            s.Name,
            s.Description,
            s.Value,
            DataType = s.DataType.ToString(),
            s.DefaultValue,
            Scope = s.Scope.ToString(),
            s.CompanyId,
            s.PlantId,
            s.IsRequired
        });
        return Result.Success(JsonSerializer.Serialize(payload));
    }
}

public sealed class ImportSettingsCommandHandler : ICommandHandler<ImportSettingsCommand, Result<int>>
{
    private readonly IDispatcher _dispatcher;

    public ImportSettingsCommandHandler(IDispatcher dispatcher) => _dispatcher = dispatcher;

    public async Task<Result<int>> HandleAsync(
        ImportSettingsCommand command,
        CancellationToken cancellationToken = default)
    {
        List<CreateSettingRequestDto>? rows;
        try
        {
            rows = JsonSerializer.Deserialize<List<CreateSettingRequestDto>>(
                command.JsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            return Result.Failure<int>(SettingErrors.Validation(ex.Message));
        }

        if (rows is null || rows.Count == 0)
        {
            return Result.Failure<int>(SettingErrors.Validation("Import payload is empty."));
        }

        var created = 0;
        foreach (var row in rows)
        {
            var result = await _dispatcher.SendAsync(
                    new CreateSettingCommand(
                        row.Category,
                        row.Key,
                        row.Name,
                        row.Description,
                        row.Value,
                        row.DataType,
                        row.DefaultValue,
                        row.Scope,
                        row.CompanyId,
                        row.PlantId,
                        row.UserId,
                        row.ValidationRule,
                        row.IsRequired),
                    cancellationToken)
                .ConfigureAwait(false);
            if (result.IsSuccess)
            {
                created++;
            }
        }

        return Result.Success(created);
    }
}

public static class SettingsCatalogSeed
{
    public static IReadOnlyList<SettingEntry> CreateDefaults() =>
    [
        SettingEntry.Create(
            "Localization", "platform.language", "Default Language", "Platform default language",
            "tr-TR", SettingDataType.Text, "tr-TR", SettingScope.Global, null, null, null, null, true, true, null),
        SettingEntry.Create(
            "Localization", "platform.timezone", "Default Time Zone", "Platform default time zone",
            "Europe/Istanbul", SettingDataType.Text, "Europe/Istanbul", SettingScope.Global, null, null, null, null, true, true, null),
        SettingEntry.Create(
            "Security", "security.sessionIdleMinutes", "Session Idle Minutes", "Idle timeout for sessions",
            "30", SettingDataType.Number, "30", SettingScope.Global, null, null, null, null, true, true, null),
        SettingEntry.Create(
            "Platform", "platform.theme", "Default Theme", "Default UI theme",
            "light", SettingDataType.Text, "light", SettingScope.Global, null, null, null, null, false, true, null)
    ];
}

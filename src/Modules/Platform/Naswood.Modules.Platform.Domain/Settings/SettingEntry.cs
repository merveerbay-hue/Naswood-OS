using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Settings;

public enum SettingScope
{
    Global = 0,
    Company = 1,
    Plant = 2,
    User = 3
}

public enum SettingDataType
{
    Text = 0,
    Number = 1,
    Boolean = 2,
    Date = 3,
    Time = 4,
    Currency = 5,
    Percentage = 6,
    List = 7,
    Json = 8,
    File = 9
}

public static class SettingErrors
{
    public static Error NotFound() =>
        Error.NotFound("SET-001", "Setting was not found.");

    public static Error KeyTaken() =>
        Error.Conflict("SET-002", "Setting key already exists for this scope.");

    public static Error SystemProtected() =>
        Error.Conflict("SET-003", "System settings cannot be deleted.");

    public static Error Validation(string message) =>
        Error.Validation("SET-004", message);

    public static Error InvalidDataType() =>
        Error.Validation("SET-005", "Setting value does not match the data type.");
}

public static class SettingCategories
{
    public static readonly IReadOnlyList<string> All =
    [
        "General", "Security", "Authentication", "Authorization", "Localization",
        "Company", "Plant", "Inventory", "Production", "Quality", "Maintenance",
        "Finance", "Notification", "Email", "File Storage", "AI", "Digital Twin",
        "Integration", "System", "Purchasing", "Sales", "Platform"
    ];
}

public sealed record SettingCreated : DomainEventBase
{
    public required Guid SettingId { get; init; }

    public required string Key { get; init; }
}

public sealed record SettingUpdated : DomainEventBase
{
    public required Guid SettingId { get; init; }
}

public sealed record SettingReset : DomainEventBase
{
    public required Guid SettingId { get; init; }
}

public sealed class SettingEntry : AggregateRoot<Guid>
{
    private SettingEntry()
    {
    }

    private SettingEntry(
        Guid id,
        string category,
        string key,
        string name,
        string? description,
        string value,
        SettingDataType dataType,
        string defaultValue,
        SettingScope scope,
        string? companyId,
        string? plantId,
        Guid? userId,
        string? validationRule,
        bool isRequired,
        bool isSystem,
        bool isEncrypted)
        : base(id)
    {
        Category = category;
        Key = key;
        Name = name;
        Description = description;
        Value = value;
        DataType = dataType;
        DefaultValue = defaultValue;
        Scope = scope;
        CompanyId = companyId;
        PlantId = plantId;
        UserId = userId;
        ValidationRule = validationRule;
        IsRequired = isRequired;
        IsSystem = isSystem;
        IsEncrypted = isEncrypted;
        IsActive = true;
        Version = 1;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public string Category { get; private set; } = string.Empty;

    public string Key { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string Value { get; private set; } = string.Empty;

    public SettingDataType DataType { get; private set; }

    public string DefaultValue { get; private set; } = string.Empty;

    public SettingScope Scope { get; private set; }

    public string? CompanyId { get; private set; }

    public string? PlantId { get; private set; }

    public Guid? UserId { get; private set; }

    public string? ValidationRule { get; private set; }

    public bool IsRequired { get; private set; }

    public bool IsSystem { get; private set; }

    public bool IsEncrypted { get; private set; }

    public bool IsActive { get; private set; }

    public int Version { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public static SettingEntry Create(
        string category,
        string key,
        string name,
        string? description,
        string value,
        SettingDataType dataType,
        string defaultValue,
        SettingScope scope,
        string? companyId,
        string? plantId,
        Guid? userId,
        string? validationRule,
        bool isRequired,
        bool isSystem,
        Guid? createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!SettingCategories.All.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Unknown category '{category}'.", nameof(category));
        }

        var setting = new SettingEntry(
            UuidV7.NewGuid(),
            category.Trim(),
            key.Trim(),
            name.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            value ?? string.Empty,
            dataType,
            defaultValue ?? string.Empty,
            scope,
            string.IsNullOrWhiteSpace(companyId) ? null : companyId.Trim().ToUpperInvariant(),
            string.IsNullOrWhiteSpace(plantId) ? null : plantId.Trim().ToUpperInvariant(),
            userId,
            string.IsNullOrWhiteSpace(validationRule) ? null : validationRule.Trim(),
            isRequired,
            isSystem,
            isEncrypted: false)
        {
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        setting.RaiseDomainEvent(new SettingCreated { SettingId = setting.Id, Key = setting.Key });
        return setting;
    }

    public Result UpdateValue(string value, Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (!IsActive)
        {
            return Result.Failure(SettingErrors.NotFound());
        }

        if (IsRequired && string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure(SettingErrors.Validation("Value is required."));
        }

        if (!SettingValueValidator.IsValid(DataType, value))
        {
            return Result.Failure(SettingErrors.InvalidDataType());
        }

        Value = value ?? string.Empty;
        Version += 1;
        UpdatedAt = utcNow;
        UpdatedBy = updatedBy;
        RaiseDomainEvent(new SettingUpdated { SettingId = Id });
        return Result.Success();
    }

    public Result ResetToDefault(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (!IsActive)
        {
            return Result.Failure(SettingErrors.NotFound());
        }

        Value = DefaultValue;
        Version += 1;
        UpdatedAt = utcNow;
        UpdatedBy = updatedBy;
        RaiseDomainEvent(new SettingReset { SettingId = Id });
        return Result.Success();
    }

    public Result SoftDeactivate(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (IsSystem)
        {
            return Result.Failure(SettingErrors.SystemProtected());
        }

        IsActive = false;
        UpdatedAt = utcNow;
        UpdatedBy = updatedBy;
        RaiseDomainEvent(new SettingUpdated { SettingId = Id });
        return Result.Success();
    }
}

public static class SettingValueValidator
{
    public static bool IsValid(SettingDataType dataType, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return dataType switch
        {
            SettingDataType.Boolean => bool.TryParse(value, out _),
            SettingDataType.Number or SettingDataType.Currency or SettingDataType.Percentage =>
                decimal.TryParse(value, out _),
            SettingDataType.Date => DateOnly.TryParse(value, out _),
            SettingDataType.Time => TimeOnly.TryParse(value, out _),
            SettingDataType.Json => LooksLikeJson(value),
            _ => true
        };
    }

    private static bool LooksLikeJson(string value)
    {
        var trimmed = value.Trim();
        return (trimmed.StartsWith('{') && trimmed.EndsWith('}')) ||
               (trimmed.StartsWith('[') && trimmed.EndsWith(']'));
    }
}

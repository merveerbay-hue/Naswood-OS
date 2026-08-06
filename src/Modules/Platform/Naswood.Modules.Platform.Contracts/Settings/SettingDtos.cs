namespace Naswood.Modules.Platform.Contracts.Settings;

public sealed class SettingDto
{
    public required Guid Id { get; init; }

    public required string Category { get; init; }

    public required string Key { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string Value { get; init; }

    public required string DataType { get; init; }

    public required string DefaultValue { get; init; }

    public required string Scope { get; init; }

    public string? CompanyId { get; init; }

    public string? PlantId { get; init; }

    public Guid? UserId { get; init; }

    public required bool IsRequired { get; init; }

    public required bool IsSystem { get; init; }

    public required bool IsActive { get; init; }

    public required int Version { get; init; }

    public required DateTimeOffset UpdatedAt { get; init; }
}

public sealed class PagedSettingsDto
{
    public required IReadOnlyList<SettingDto> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required int TotalPages { get; init; }
}

public sealed class CreateSettingRequestDto
{
    public required string Category { get; init; }

    public required string Key { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string Value { get; init; }

    public required string DataType { get; init; }

    public string? DefaultValue { get; init; }

    public string Scope { get; init; } = "Global";

    public string? CompanyId { get; init; }

    public string? PlantId { get; init; }

    public Guid? UserId { get; init; }

    public string? ValidationRule { get; init; }

    public bool IsRequired { get; init; }
}

public sealed class UpdateSettingRequestDto
{
    public required string Value { get; init; }
}

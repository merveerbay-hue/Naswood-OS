using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Organization;

/// <summary>
/// Platform organization reference used for User Management existence validation.
/// Codes align with Authentication company/plant string identifiers.
/// Full Master Data CRUD is owned by Organization / Master Data modules.
/// </summary>
public sealed class CompanyReference : AggregateRoot<Guid>
{
    private CompanyReference()
    {
    }

    private CompanyReference(Guid id, string code, string name, bool isActive)
        : base(id)
    {
        Code = code;
        Name = name;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public static CompanyReference Create(string code, string name) =>
        new(UuidV7.NewGuid(), RequireCode(code), RequireName(name), isActive: true);

    private static string RequireCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return code.Trim().ToUpperInvariant();
    }

    private static string RequireName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.Trim();
    }
}

public sealed class PlantReference : AggregateRoot<Guid>
{
    private PlantReference()
    {
    }

    private PlantReference(Guid id, string code, string name, string companyCode, bool isActive)
        : base(id)
    {
        Code = code;
        Name = name;
        CompanyCode = companyCode;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string CompanyCode { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public static PlantReference Create(string code, string name, string companyCode) =>
        new(
            UuidV7.NewGuid(),
            Require(code),
            RequireName(name),
            Require(companyCode),
            isActive: true);

    private static string Require(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim().ToUpperInvariant();
    }

    private static string RequireName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.Trim();
    }
}

public sealed class DepartmentReference : AggregateRoot<Guid>
{
    private DepartmentReference()
    {
    }

    private DepartmentReference(Guid id, string code, string name, bool isActive)
        : base(id)
    {
        Code = code;
        Name = name;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public static DepartmentReference Create(string code, string name) =>
        new(UuidV7.NewGuid(), Require(code), RequireName(name), isActive: true);

    private static string Require(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim().ToUpperInvariant();
    }

    private static string RequireName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.Trim();
    }
}

public sealed class PositionReference : AggregateRoot<Guid>
{
    private PositionReference()
    {
    }

    private PositionReference(Guid id, string code, string title, bool isActive)
        : base(id)
    {
        Code = code;
        Title = title;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public static PositionReference Create(string code, string title) =>
        new(UuidV7.NewGuid(), Require(code), RequireName(title), isActive: true);

    private static string Require(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim().ToUpperInvariant();
    }

    private static string RequireName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.Trim();
    }
}

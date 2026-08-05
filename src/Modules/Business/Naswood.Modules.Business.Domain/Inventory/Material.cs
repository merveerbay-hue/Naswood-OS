using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class Material : BusinessEntity
{
    private Material() { }

    private Material(Guid id, string code, string name, string description, string category, string unitOfMeasure, string status, string companyId, string? plantId)
        : base(id)
    {
        Code = code;
        Name = name;
        Description = description;
        Category = category;
        UnitOfMeasure = unitOfMeasure;
        Status = status;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string UnitOfMeasure { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public static Material Create(string code, string name, string description, string category, string unitOfMeasure, string status, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new Material(UuidV7.NewGuid(), code, name, description, category, unitOfMeasure, status, companyId, plantId);
    }

    public void Update(string code, string name, string description, string category, string unitOfMeasure, string status)
    {
        Code = code;
        Name = name;
        Description = description;
        Category = category;
        UnitOfMeasure = unitOfMeasure;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

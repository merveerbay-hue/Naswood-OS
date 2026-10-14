using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Business.Domain.Common;

public abstract class BusinessEntity : AggregateRoot<Guid>
{
    protected BusinessEntity() { }
    protected BusinessEntity(Guid id) : base(id) { }

    public string CompanyId { get; protected set; } = "COMP-001";
    public string? PlantId { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }
}

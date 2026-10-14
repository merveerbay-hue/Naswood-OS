using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Sales;

public sealed class SalesReportDefinition : BusinessEntity
{
    private SalesReportDefinition() { }
    private SalesReportDefinition(Guid id, string reportCode, string name, string category, string description)
        : base(id)
    {
        ReportCode = reportCode; Name = name; Category = category; Description = description;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }
    public string ReportCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public static SalesReportDefinition Create(string reportCode, string name, string category, string description) =>
        new(UuidV7.NewGuid(), reportCode, name, category, description);
    public void Update(string reportCode, string name, string category, string description)
    {
        ReportCode = reportCode; Name = name; Category = category; Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    public void SoftDelete() { IsDeleted = true; UpdatedAt = DateTimeOffset.UtcNow; }
}

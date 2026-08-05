using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Common;

namespace Naswood.Modules.Business.Domain.Inventory;

public sealed class StockTransfer : BusinessEntity
{
    private StockTransfer() { }

    private StockTransfer(Guid id, string number, string fromWarehouseCode, string toWarehouseCode, string status, string notes, string companyId, string? plantId)
        : base(id)
    {
        Number = number;
        FromWarehouseCode = fromWarehouseCode;
        ToWarehouseCode = toWarehouseCode;
        Status = status;
        Notes = notes;
        CompanyId = companyId;
        PlantId = plantId;
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; } = string.Empty;
    public string FromWarehouseCode { get; private set; } = string.Empty;
    public string ToWarehouseCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public static StockTransfer Create(string number, string fromWarehouseCode, string toWarehouseCode, string status, string notes, string companyId = "COMP-001", string? plantId = "PLANT-001")
    {
        return new StockTransfer(UuidV7.NewGuid(), number, fromWarehouseCode, toWarehouseCode, status, notes, companyId, plantId);
    }

    public void Update(string number, string fromWarehouseCode, string toWarehouseCode, string status, string notes)
    {
        Number = number;
        FromWarehouseCode = fromWarehouseCode;
        ToWarehouseCode = toWarehouseCode;
        Status = status;
        Notes = notes;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

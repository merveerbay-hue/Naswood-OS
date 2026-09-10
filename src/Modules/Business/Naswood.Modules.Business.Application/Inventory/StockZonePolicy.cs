using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

/// <summary>
/// Physical zone vs package stock status. QC changes status only; relocate is a separate PACKAGE_MOVE.
/// </summary>
public static class StockZonePolicy
{
    public static Result GuardDestination(string packageStatus, Location location, string action)
    {
        var zone = StockZoneTypes.Resolve(location.StockZoneType, location.LocationType);
        if (string.Equals(packageStatus, InventoryPackageStatuses.Rejected, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(Error.Validation("PKG-QC-002", "Reddedilmiş paket bu işleme alınamaz."));
        if (string.Equals(packageStatus, InventoryPackageStatuses.Quarantine, StringComparison.OrdinalIgnoreCase)
            && !StockZoneTypes.IsQuarantineCompatible(zone))
            return Result.Failure(Error.Validation("PKG-ZONE-001", "Karantina paketi yalnız karantina lokasyonuna taşınabilir."));
        if (string.Equals(packageStatus, InventoryPackageStatuses.Available, StringComparison.OrdinalIgnoreCase)
            && !StockZoneTypes.IsNormalStock(zone))
            return Result.Failure(Error.Validation("PKG-ZONE-002", "Available paket yalnız normal stok lokasyonuna taşınabilir."));
        if (InventoryPackageStatuses.IsClosed(packageStatus)
            && !string.Equals(packageStatus, InventoryPackageStatuses.Quarantine, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(Error.Validation("PKG-LIFE-006", $"Paket artık aktif değil ({packageStatus})."));
        _ = action;
        return Result.Success();
    }
}

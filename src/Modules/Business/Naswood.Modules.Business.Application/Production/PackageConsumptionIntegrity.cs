using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Production;

public sealed record PackagePhysicalSnapshot(
    Guid PackageId,
    string PackageNo,
    string Status,
    decimal Quantity,
    decimal ContentQuantity);

/// <summary>
/// Extra physical guard on top of InventoryBalance. Canonical qty remains the balance.
/// </summary>
public static class PackageConsumptionIntegrity
{
    public static Result Validate(
        IReadOnlyList<(Guid PackageId, decimal ConsumedQuantity)> rows,
        IReadOnlyDictionary<Guid, PackagePhysicalSnapshot> packages)
    {
        foreach (var group in rows.GroupBy(r => r.PackageId))
        {
            if (!packages.TryGetValue(group.Key, out var pkg))
                return Result.Failure(Error.Validation("PRD-OUT-015", "Kaynak paket bulunamadı."));
            if (!InventoryPackageStatuses.IsConsumable(pkg.Status))
                return Result.Failure(Error.Validation("PRD-OUT-032", $"Paket tüketilebilir değil: {pkg.PackageNo} ({pkg.Status})."));
            var requested = group.Sum(x => x.ConsumedQuantity);
            var physical = PhysicalAvailable(pkg.Quantity, pkg.ContentQuantity);
            if (requested > physical)
                return Result.Failure(Error.Validation(
                    "PRD-OUT-017",
                    $"Paket {pkg.PackageNo} fiziksel içeriği {physical}, istenen tüketim {requested}."));
        }
        return Result.Success();
    }

    public static decimal PhysicalAvailable(decimal packageQuantity, decimal contentQuantity)
        => contentQuantity > 0 ? Math.Min(packageQuantity, contentQuantity) : packageQuantity;
}

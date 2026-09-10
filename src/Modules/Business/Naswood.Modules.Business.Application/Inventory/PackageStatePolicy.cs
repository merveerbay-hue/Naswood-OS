using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Inventory;

public static class PackageActions
{
    public const string Move = "MOVE";
    public const string Split = "SPLIT";
    public const string Repack = "REPACK";
    public const string Consume = "CONSUME";
    public const string Merge = "MERGE";
    public const string PrintLabel = "PRINT_LABEL";
}

/// <summary>Central package status transitions. Operations (split/merge) are events, not statuses.</summary>
public static class PackageStatePolicy
{
    public static bool CanConsume(InventoryPackage pkg)
        => InventoryPackageStatuses.IsConsumable(pkg.Status) && pkg.Quantity > 0;

    public static bool CanSplit(InventoryPackage pkg)
        => InventoryPackageStatuses.IsPhysicalActive(pkg.Status) && pkg.Quantity > 0;

    public static bool CanMerge(InventoryPackage pkg)
        => InventoryPackageStatuses.IsPhysicalActive(pkg.Status) && pkg.Quantity > 0;

    public static bool CanRepack(InventoryPackage pkg)
        => InventoryPackageStatuses.IsPhysicalActive(pkg.Status) && pkg.Quantity > 0;

    public static bool CanMove(InventoryPackage pkg)
        => InventoryPackageStatuses.IsPhysicalActive(pkg.Status) && pkg.Quantity > 0;

    public static bool CanOperationalScan(InventoryPackage pkg)
        => CanConsume(pkg);

    public static Result GuardRejected(InventoryPackage pkg, string action)
    {
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Rejected, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(Error.Validation("PKG-QC-002", $"Reddedilmiş paket bu işleme alınamaz ({action})."));
        if (InventoryPackageStatuses.IsClosed(pkg.Status)
            && !string.Equals(pkg.Status, InventoryPackageStatuses.Quarantine, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(Error.Validation("PKG-LIFE-006", $"Paket artık aktif değil ({pkg.Status})."));
        return Result.Success();
    }

    public static Result GuardTransition(string from, string to, string reason)
    {
        if (string.Equals(from, InventoryPackageStatuses.Rejected, StringComparison.OrdinalIgnoreCase)
            && string.Equals(to, InventoryPackageStatuses.Available, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(Error.Validation("PKG-QC-002", "Reddedilmiş paket doğrudan AVAILABLE olamaz."));
        if (string.Equals(from, InventoryPackageStatuses.Consumed, StringComparison.OrdinalIgnoreCase)
            && string.Equals(to, InventoryPackageStatuses.Available, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(reason, "REVERSAL", StringComparison.OrdinalIgnoreCase))
            return Result.Failure(Error.Validation("PKG-LIFE-006", "Tüketilen paket yalnız geçerli reversal ile açılır."));
        return Result.Success();
    }

    public static IReadOnlyList<string> AllowedActions(InventoryPackage pkg)
    {
        var actions = new List<string> { PackageActions.PrintLabel };
        if (CanMove(pkg)) actions.Add(PackageActions.Move);
        if (CanSplit(pkg)) actions.Add(PackageActions.Split);
        if (CanRepack(pkg)) actions.Add(PackageActions.Repack);
        if (CanMerge(pkg)) actions.Add(PackageActions.Merge);
        if (CanConsume(pkg)) actions.Add(PackageActions.Consume);
        return actions;
    }

    public static string? InactiveReason(InventoryPackage pkg)
    {
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Merged, StringComparison.OrdinalIgnoreCase))
            return "PAKET ARTIK AKTİF DEĞİL — birleştirildi.";
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Repacked, StringComparison.OrdinalIgnoreCase))
            return "PAKET ARTIK AKTİF DEĞİL — yeniden paketlendi.";
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Cancelled, StringComparison.OrdinalIgnoreCase))
            return "PAKET ARTIK AKTİF DEĞİL — iptal edildi.";
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Consumed, StringComparison.OrdinalIgnoreCase))
            return "Pakette tüketilecek miktar yok.";
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Rejected, StringComparison.OrdinalIgnoreCase))
            return "Paket reddedildi / bloke.";
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Closed, StringComparison.OrdinalIgnoreCase))
            return "PAKET ARTIK AKTİF DEĞİL.";
        return null;
    }

    public static string? LabelHint(InventoryPackage pkg, IReadOnlyList<PackageRelation> relations)
    {
        if (relations.Any(r =>
                r.SourcePackageId == pkg.Id
                && (r.RelationType == PackageRelationTypes.Split || r.RelationType == PackageRelationTypes.PartialMove)))
            return "Orijinal paket etiketi güncellenmeli.";
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Merged, StringComparison.OrdinalIgnoreCase))
            return "Kaynak paket etiketlerini kullanım dışı bırakın.";
        if (string.Equals(pkg.Status, InventoryPackageStatuses.Repacked, StringComparison.OrdinalIgnoreCase))
            return "Eski barkodu fiziksel paketten kaldırın. Yeni etiket basılmalı.";
        return null;
    }
}

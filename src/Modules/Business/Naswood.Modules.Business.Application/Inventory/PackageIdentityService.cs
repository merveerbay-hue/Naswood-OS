using System.Globalization;

namespace Naswood.Modules.Business.Application.Inventory;

public sealed record PackageIdentityMint(string PackageNumber, string BarcodeValue, string PublicId);

/// <summary>
/// Single package identity mint for Opening, Goods Receipt, and future production output.
/// Does not post stock.
/// </summary>
public static class PackageIdentityService
{
    public static PackageIdentityMint Mint(string? plantId, DateTimeOffset utc, int ordinal)
    {
        var packageNo = OpeningInventoryCodes.PackageNo(plantId, utc, ordinal);
        return new PackageIdentityMint(
            packageNo,
            OpeningInventoryCodes.Barcode(packageNo),
            Guid.NewGuid().ToString("N"));
    }

    public static string QrPath(string publicId)
        => $"/inventory/packages/p/{(publicId ?? string.Empty).Trim()}";

    public static string StackKey(
        Guid countId,
        Guid materialId,
        Guid openingLotId,
        Guid locationId,
        string? physicalGroupLabel,
        Guid lineId)
    {
        var group = (physicalGroupLabel ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(group))
            return $"{countId:N}|{materialId:N}|{openingLotId:N}|{locationId:N}|LINE-{lineId:N}";
        return $"{countId:N}|{materialId:N}|{openingLotId:N}|{locationId:N}|{group.ToUpper(CultureInfo.GetCultureInfo("tr-TR"))}";
    }
}

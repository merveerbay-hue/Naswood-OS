using System.Globalization;
using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Business.Application.Inventory;

/// <summary>Minted package identity. The barcode is the package code (ADR-002 §6).</summary>
public sealed record PackageIdentityMint(string PackageNumber, string PublicId)
{
    public string BarcodeValue => PackageNumber;
}

/// <summary>
/// Single package identity mint for Opening, Goods Receipt, production output and package operations.
/// Does not post stock.
/// </summary>
public static class PackageIdentityService
{
    public static async Task<PackageIdentityMint> MintAsync(
        IIdentityCodeAllocator allocator,
        string siteCode,
        CancellationToken cancellationToken)
    {
        var code = await allocator.NextAsync(siteCode, IdentityType.Pkg, cancellationToken).ConfigureAwait(false);
        return new PackageIdentityMint(code, Guid.NewGuid().ToString("N"));
    }

    /// <summary>Site code of the plant, or a validation failure when the plant cannot mint identity codes.</summary>
    public static async Task<Result<string>> RequireSiteCodeAsync(
        IIdentityCodeAllocator allocator,
        string? plantId,
        CancellationToken cancellationToken)
    {
        var site = await allocator.ResolveSiteCodeAsync(plantId, cancellationToken).ConfigureAwait(false);
        return site is null
            ? Result.Failure<string>(Error.Validation(
                IdentityCodes.ErrorNoSiteCode,
                $"Tesis '{plantId}' için tesis kodu tanımlı değil; paket/lot kimliği basılamaz."))
            : Result.Success(site);
    }

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

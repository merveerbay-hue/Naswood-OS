namespace Naswood.Modules.Business.Application.Inventory;

/// <summary>
/// Opening = inventory initialization (may mint lot/package/barcode).
/// Periodic / Blind = reconciliation (never mints new identity).
/// </summary>
public static class InventoryCountKinds
{
    public const string Opening = "Opening";
    public const string Periodic = "Periodic";
    public const string Blind = "Blind";

    public static string Normalize(string? raw)
    {
        var v = (raw ?? string.Empty).Trim();
        if (v.Equals("Opening", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Açılış", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Acilis", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Initialization", StringComparison.OrdinalIgnoreCase)
            || v.Equals("OPENING_INVENTORY", StringComparison.OrdinalIgnoreCase))
            return Opening;
        if (v.Equals("Blind", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Kör", StringComparison.OrdinalIgnoreCase)
            || v.Equals("Kor", StringComparison.OrdinalIgnoreCase))
            return Blind;
        return Periodic;
    }

    public static bool IsOpening(string? raw) => Normalize(raw) == Opening;

    public static bool MayMintLotPackageBarcode(string? raw) => IsOpening(raw);
}

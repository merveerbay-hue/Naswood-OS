using System.Globalization;
using System.Text.RegularExpressions;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public static class ProductionLotCodes
{
    public const string SourceType = "PRODUCTION";
    public const string OutputMovement = "PRODUCTION_OUTPUT";
    public const string ConsumptionMovement = "PRODUCTION_CONSUMPTION";
    public const string LotPrefix = "LOT-PR-";

    public static string FactoryToken(string? plantId) => OpeningInventoryCodes.FactoryToken(plantId);

    public static string ProductionLot(string? plantId, DateTimeOffset utc, int ordinal)
        => $"{LotPrefix}{FactoryToken(plantId)}-{utc.UtcDateTime.ToString("yyMMdd", CultureInfo.InvariantCulture)}-{ordinal:0000}";

    public static string LotPrefixForDay(string? plantId, DateTimeOffset utc)
        => $"{LotPrefix}{FactoryToken(plantId)}-{utc.UtcDateTime.ToString("yyMMdd", CultureInfo.InvariantCulture)}-";

    public static int ParseLotOrdinal(string? lotNumber, string? plantId, DateTimeOffset utc)
    {
        var factory = FactoryToken(plantId);
        var day = utc.UtcDateTime.ToString("yyMMdd", CultureInfo.InvariantCulture);
        var m = Regex.Match(lotNumber ?? "", $@"^LOT-PR-{Regex.Escape(factory)}-{Regex.Escape(day)}-(\d+)$", RegexOptions.IgnoreCase);
        return m.Success && int.TryParse(m.Groups[1].Value, out var n) ? n : 0;
    }

    public static string StackKey(
        Guid productionOrderId,
        Guid productionLotId,
        Guid materialId,
        Guid locationId,
        string? physicalGroupLabel,
        Guid rowId)
        => PackageIdentityService.StackKey(productionOrderId, materialId, productionLotId, locationId, physicalGroupLabel, rowId);

    public static void EnsurePackageMatchesLot(InventoryPackage package, Batch lot, Material material)
    {
        if (package.MaterialId != material.Id || lot.Id != package.BatchId)
            throw new InvalidOperationException("Package MaterialId/BatchId must match the production lot.");
        if (!string.Equals(package.MaterialCode, material.Code, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Package MaterialCode snapshot must match output material.");
        if (!string.Equals(package.LotNumber, lot.BatchNumber, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Package LotNumber snapshot must match the production lot.");
    }
}

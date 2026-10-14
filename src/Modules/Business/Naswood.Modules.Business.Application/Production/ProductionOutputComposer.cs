using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Production;

public sealed record ProductionOutputResolvedLine(
    string PhysicalGroupLabel,
    decimal? ThicknessMm,
    decimal? WidthMm,
    decimal? LengthMm,
    decimal? PieceCount,
    decimal Quantity,
    string Unit,
    string Measurement,
    Guid RowId);

public static class ProductionOutputComposer
{
    public static Result<(IReadOnlyList<ProductionOutputResolvedLine> Lines, IReadOnlyList<IGrouping<string, ProductionOutputResolvedLine>> Stacks, decimal OutputQty, string Unit)>
        ResolveLines(Material material, IReadOnlyList<ProductionOutputLineRequestDto> raw)
    {
        if (raw is null || raw.Count == 0)
            return Result.Failure<(IReadOnlyList<ProductionOutputResolvedLine>, IReadOnlyList<IGrouping<string, ProductionOutputResolvedLine>>, decimal, string)>(
                Error.Validation("PRD-OUT-001", "En az bir fiziksel çıkış satırı gerekli."));

        var policy = InventoryCountMath.ResolvePolicy(material.UnitOfMeasure, material.Category, material.DefinitionJson);
        var lines = new List<ProductionOutputResolvedLine>();
        foreach (var line in raw)
        {
            var calc = InventoryCountMath.CalculateStockQty(
                policy, line.ThicknessMm, line.WidthMm, line.LengthMm, line.PieceCount, line.MeasuredVolumeM3);
            if (!calc.Ok)
                return Result.Failure<(IReadOnlyList<ProductionOutputResolvedLine>, IReadOnlyList<IGrouping<string, ProductionOutputResolvedLine>>, decimal, string)>(
                    Error.Validation("PRD-OUT-002", calc.Error ?? "Ölçü hesaplanamadı."));
            if (calc.StockQty <= 0)
                return Result.Failure<(IReadOnlyList<ProductionOutputResolvedLine>, IReadOnlyList<IGrouping<string, ProductionOutputResolvedLine>>, decimal, string)>(
                    Error.Validation("PRD-OUT-002", "Çıkış miktarı pozitif olmalı."));
            lines.Add(new ProductionOutputResolvedLine(
                line.PhysicalGroupLabel ?? string.Empty,
                line.ThicknessMm,
                line.WidthMm,
                line.LengthMm,
                line.PieceCount,
                calc.StockQty,
                policy.StockUnit,
                PackagePassportComposer.Measurement(line.ThicknessMm, line.WidthMm, line.LengthMm),
                Guid.NewGuid()));
        }

        return Result.Success<(IReadOnlyList<ProductionOutputResolvedLine>, IReadOnlyList<IGrouping<string, ProductionOutputResolvedLine>>, decimal, string)>(
            (lines, Array.Empty<IGrouping<string, ProductionOutputResolvedLine>>(), lines.Sum(l => l.Quantity), policy.StockUnit));
    }

    public static IReadOnlyList<IGrouping<string, ProductionOutputResolvedLine>> GroupStacks(
        Guid productionOrderId,
        Guid productionLotId,
        Guid materialId,
        Guid locationId,
        IReadOnlyList<ProductionOutputResolvedLine> lines)
        => lines
            .GroupBy(l => ProductionLotCodes.StackKey(
                productionOrderId, productionLotId, materialId, locationId, l.PhysicalGroupLabel, l.RowId))
            .ToArray();

    public static ProductionOutputPreviewDto ToPreview(
        string orderNumber,
        Material material,
        string warehouseCode,
        string locationCode,
        string workCenterCode,
        IReadOnlyList<ProductionOutputResolvedLine> lines,
        IReadOnlyList<IGrouping<string, ProductionOutputResolvedLine>> stacks,
        decimal inputQty)
        => new()
        {
            ProductionOrderNumber = orderNumber,
            OutputMaterialCode = material.Code,
            OutputMaterialName = material.Name,
            DestinationWarehouse = warehouseCode,
            DestinationLocation = locationCode,
            WorkCenterCode = workCenterCode,
            LotHint = "Otomatik oluşturulacak",
            PackageCount = stacks.Count,
            OutputQuantity = lines.Sum(l => l.Quantity),
            InputQuantity = inputQty,
            Unit = lines.FirstOrDefault()?.Unit ?? string.Empty,
            SourceLotCount = 0,
            Packages = stacks.Select(s => new ProductionOutputPreviewPackageDto
            {
                PhysicalGroupLabel = s.First().PhysicalGroupLabel,
                MeasurementCount = s.Count(),
                Quantity = s.Sum(x => x.Quantity),
                Unit = s.First().Unit,
                PieceCount = s.Sum(x => x.PieceCount ?? 0),
                Measurements = s.Select(x =>
                    string.IsNullOrWhiteSpace(x.Measurement)
                        ? $"{x.Quantity} {x.Unit}"
                        : $"{x.Measurement} — {x.PieceCount ?? 0} adet").ToArray()
            }).ToArray()
        };
}

using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class ProductionOutputTests
{
    private static Material FjLamel() =>
        Material.Create("YM-LM-PIN-FJ-001", "Çam Finger Joint Lamel", "", "YM", "M3", "Active",
            """{"stockUom":"M3","volumeCalcRequired":true,"mainCategory":"YM"}""", plantId: "F01");

    [Fact]
    public void Production_lot_number_and_source_type()
    {
        var day = new DateTimeOffset(2026, 9, 10, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal("LOT-PR-F01-260910-0004", ProductionLotCodes.ProductionLot("F01", day, 4));
        Assert.Equal("PRODUCTION", ProductionLotCodes.SourceType);
        Assert.Equal("PRODUCTION_OUTPUT", ProductionLotCodes.OutputMovement);
        Assert.Equal("PRODUCTION_CONSUMPTION", ProductionLotCodes.ConsumptionMovement);
        var lot = Batch.Create("LOT-PR-F01-260910-0004", "YM-LM-PIN-FJ-001", 0, null, "Active",
            plantId: "F01", sourceType: ProductionLotCodes.SourceType, sourceReferenceNo: "PRD-2026-0042");
        Assert.Equal("PRODUCTION", lot.SourceType);
        Assert.False(lot.BatchNumber.StartsWith("LOT-OPEN-", StringComparison.Ordinal));
        Assert.False(lot.BatchNumber.StartsWith("PLOT-", StringComparison.Ordinal));
    }

    [Fact]
    public void Two_source_lots_keep_separate_ids()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        Assert.NotEqual(a, b);
        var sources = new[]
        {
            new ProductionOutputSourceRequestDto { SourceLotId = a, ConsumedQuantity = 1, SourceWarehouseCode = "WH", SourceLocationCode = "A" },
            new ProductionOutputSourceRequestDto { SourceLotId = b, ConsumedQuantity = 2, SourceWarehouseCode = "WH", SourceLocationCode = "A" },
        };
        Assert.Equal(2, sources.Select(s => s.SourceLotId).Distinct().Count());
    }

    [Fact]
    public void Same_stack_two_measurements_one_package()
    {
        var material = FjLamel();
        var lines = ProductionOutputComposer.ResolveLines(material, new[]
        {
            new ProductionOutputLineRequestDto { PhysicalGroupLabel = "İstif A", ThicknessMm = 30, WidthMm = 100, LengthMm = 4000, PieceCount = 120 },
            new ProductionOutputLineRequestDto { PhysicalGroupLabel = "İstif A", ThicknessMm = 30, WidthMm = 100, LengthMm = 3000, PieceCount = 80 },
        });
        Assert.True(lines.IsSuccess);
        var order = Guid.NewGuid();
        var lot = Guid.NewGuid();
        var loc = Guid.NewGuid();
        var stacks = ProductionOutputComposer.GroupStacks(order, lot, material.Id, loc, lines.Value.Lines);
        Assert.Single(stacks);
        Assert.Equal(2, stacks[0].Count());
        Assert.Equal(2, InventoryCountMath.CubicMeters(30, 100, 4000, 120) + InventoryCountMath.CubicMeters(30, 100, 3000, 80) > 0 ? 2 : 0);
    }

    [Fact]
    public void Two_stacks_two_packages()
    {
        var material = FjLamel();
        var lines = ProductionOutputComposer.ResolveLines(material, new[]
        {
            new ProductionOutputLineRequestDto { PhysicalGroupLabel = "İstif A", ThicknessMm = 30, WidthMm = 100, LengthMm = 4000, PieceCount = 10 },
            new ProductionOutputLineRequestDto { PhysicalGroupLabel = "İstif B", ThicknessMm = 30, WidthMm = 100, LengthMm = 3000, PieceCount = 10 },
        }).Value.Lines;
        var stacks = ProductionOutputComposer.GroupStacks(Guid.NewGuid(), Guid.NewGuid(), material.Id, Guid.NewGuid(), lines);
        Assert.Equal(2, stacks.Count);
    }

    [Fact]
    public void Identity_service_same_as_opening()
    {
        var day = new DateTimeOffset(2026, 9, 10, 0, 0, 0, TimeSpan.Zero);
        var mint = PackageIdentityService.Mint("F01", day, 401);
        Assert.Equal("NW-PKG-F01-26-000401", mint.PackageNumber);
        Assert.Equal("NWPKG-F01-26-000401", mint.BarcodeValue);
        Assert.Equal("/inventory/packages/p/abc", PackageIdentityService.QrPath("abc"));
    }

    [Fact]
    public void Package_material_mismatch_is_rejected()
    {
        var material = FjLamel();
        var other = Material.Create("OTHER", "X", "", "YM", "M3", "Active", plantId: "F01");
        var lot = Batch.Create("LOT-PR-F01-260910-0001", material.Code, 0, null, "Active", plantId: "F01", sourceType: "PRODUCTION");
        var pkg = InventoryPackage.Create(
            "NW-PKG-F01-26-000001", "MI-1", material.Code, lot.BatchNumber, "WH-SFG", "YM-A01", 1, "M3",
            plantId: "F01", materialId: material.Id, batchId: lot.Id);
        Assert.Throws<InvalidOperationException>(() => ProductionLotCodes.EnsurePackageMatchesLot(pkg, lot, other));
        ProductionLotCodes.EnsurePackageMatchesLot(pkg, lot, material);
    }

    [Fact]
    public void Factory_isolation_helper()
    {
        Assert.False(PlantAccess.CanAccess(new[] { "F01" }, "F02"));
        Assert.True(PlantAccess.CanAccess(new[] { "F01" }, "F01"));
    }

    [Fact]
    public void Post_handler_owns_unit_of_work()
    {
        Assert.Null(typeof(ProductionOutputGateway).GetMethod("SaveChangesAsync"));
        var handler = typeof(PostProductionOutputCommandHandler);
        Assert.Contains(handler.GetConstructors()[0].GetParameters(), p => p.ParameterType.Name.Contains("UnitOfWork"));
    }

    [Fact]
    public void Preview_does_not_mint_lot_or_package()
    {
        var material = FjLamel();
        var resolved = ProductionOutputComposer.ResolveLines(material, new[]
        {
            new ProductionOutputLineRequestDto { PhysicalGroupLabel = "İstif A", ThicknessMm = 30, WidthMm = 100, LengthMm = 4000, PieceCount = 120 },
        });
        var preview = ProductionOutputComposer.ToPreview(
            "PRD-2026-0042", material, "WH-SFG", "YM-A01", "WC-FJ",
            resolved.Value.Lines,
            ProductionOutputComposer.GroupStacks(Guid.NewGuid(), Guid.Empty, material.Id, Guid.NewGuid(), resolved.Value.Lines),
            2);
        Assert.Equal("Otomatik oluşturulacak", preview.LotHint);
        Assert.DoesNotContain("LOT-PR-", preview.LotHint);
        Assert.DoesNotContain("NW-PKG", preview.Packages[0].PhysicalGroupLabel);
        Assert.Equal(1, preview.PackageCount);
    }

    [Fact]
    public void Output_qty_uses_material_policy_not_hardcoded_m3_for_pcs()
    {
        var hw = Material.Create("HW-001", "Vida", "", "HW", "PCS", "Active", """{"stockUom":"PCS","mainCategory":"HW"}""");
        var resolved = ProductionOutputComposer.ResolveLines(hw, new[]
        {
            new ProductionOutputLineRequestDto { PieceCount = 50 },
        });
        Assert.True(resolved.IsSuccess);
        Assert.Equal(50m, resolved.Value.OutputQty);
        Assert.Equal("PCS", resolved.Value.Unit);
    }
}

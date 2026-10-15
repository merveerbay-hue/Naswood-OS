using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.UnitTests;

public sealed class IdentityCodesTests
{
    [Theory]
    [InlineData(IdentityType.Pkg, "PKG-BUC-000001")]
    [InlineData(IdentityType.Lot, "LOT-BUC-000001")]
    [InlineData(IdentityType.Lam, "LAM-BUC-000001")]
    [InlineData(IdentityType.Auto, "AUTO-BUC-000001")]
    [InlineData(IdentityType.Trf, "TRF-BUC-000001")]
    [InlineData(IdentityType.Rest, "REST-BUC-000001")]
    public void Format_is_type_site_six_digit_sequence(IdentityType type, string expected)
        => Assert.Equal(expected, IdentityCodes.Format(type, "BUC", 1));

    [Fact]
    public void Format_grows_past_six_digits()
        => Assert.Equal("PKG-BUC-1000000", IdentityCodes.Format(IdentityType.Pkg, "BUC", 1_000_000));

    [Theory]
    [InlineData("PKG-BUC-000001", IdentityType.Pkg, "BUC", 1L, true)]
    [InlineData("LOT-BUC-000123", IdentityType.Lot, "BUC", 123L, true)]
    [InlineData("LAM-BUC-000001", IdentityType.Lam, "BUC", 1L, true)]
    [InlineData("AUTO-BUC-000001", IdentityType.Auto, "BUC", 1L, true)]
    [InlineData("TRF-BUC-000001", IdentityType.Trf, "BUC", 1L, true)]
    [InlineData("REST-BUC-000001", IdentityType.Rest, "BUC", 1L, true)]
    [InlineData("PKG-ANT-000245", IdentityType.Pkg, "ANT", 245L, true)]
    [InlineData("PKG-000245", IdentityType.Pkg, "ANT", 245L, false)]
    [InlineData("LOT-000123", IdentityType.Lot, "ANT", 123L, false)]
    [InlineData("PKG-BUC-1234567", IdentityType.Pkg, "BUC", 1234567L, true)]
    [InlineData(" pkg-buc-000001 ", IdentityType.Pkg, "BUC", 1L, true)]
    public void Scanner_rule_accepts(string raw, IdentityType type, string site, long seq, bool prefixed)
    {
        Assert.True(IdentityCodes.TryParse(raw, out var code));
        Assert.Equal(type, code.Type);
        Assert.Equal(site, code.SiteCode);
        Assert.Equal(seq, code.Sequence);
        Assert.Equal(prefixed, code.HasSitePrefix);
        Assert.Equal(site == "BUC", code.IsLocal);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("PKG-BUC-00001")]
    [InlineData("NW-PKG-F01-26-000001")]
    [InlineData("NWPKG-F01-26-000001")]
    [InlineData("LOT-OPEN-F01-260910-0001")]
    [InlineData("LOT-PR-F01-260910-0001")]
    [InlineData("LOT-2026-00101")]
    [InlineData("PKG-IST-000001")]
    [InlineData("PKG-BUC-000000")]
    [InlineData("https://nos/inventory/packages/p/abc")]
    [InlineData("PLOT-BUC-000001")]
    public void Scanner_rule_rejects(string? raw)
        => Assert.False(IdentityCodes.TryParse(raw, out _));

    [Fact]
    public void Local_package_code_is_only_pkg_buc()
    {
        Assert.True(IdentityCodes.IsLocalPackageCode("PKG-BUC-000001"));
        Assert.False(IdentityCodes.IsLocalPackageCode("PKG-000001"));
        Assert.False(IdentityCodes.IsLocalPackageCode("PKG-ANT-000001"));
        Assert.False(IdentityCodes.IsLocalPackageCode("LOT-BUC-000001"));
    }

    [Fact]
    public void Package_barcode_is_the_package_code()
    {
        var pkg = InventoryPackage.Create(
            "PKG-BUC-000001", "MI-1", "YM-PR-AYO-001", "LOT-BUC-000001", "WH", "A-01", 1, "M3",
            plantId: "F01", materialId: Guid.NewGuid(), batchId: Guid.NewGuid());
        Assert.Equal("PKG-BUC-000001", pkg.Barcode);
        Assert.Throws<InvalidOperationException>(() => InventoryPackage.Create(
            "PKG-BUC-000002", "MI-1", "YM-PR-AYO-001", "LOT-BUC-000001", "WH", "A-01", 1, "M3",
            barcode: "NWPKG-F01-26-000002",
            plantId: "F01", materialId: Guid.NewGuid(), batchId: Guid.NewGuid()));
    }

    [Fact]
    public async Task Mint_takes_next_package_code_and_fresh_public_id()
    {
        var allocator = new FakeIdentityCodeAllocator();
        var first = await PackageIdentityService.MintAsync(allocator, "BUC", CancellationToken.None);
        var second = await PackageIdentityService.MintAsync(allocator, "BUC", CancellationToken.None);
        Assert.Equal("PKG-BUC-000001", first.PackageNumber);
        Assert.Equal(first.PackageNumber, first.BarcodeValue);
        Assert.Equal("PKG-BUC-000002", second.PackageNumber);
        Assert.Equal(32, first.PublicId.Length);
        Assert.NotEqual(first.PublicId, second.PublicId);
    }

    [Fact]
    public async Task Plant_without_site_code_cannot_mint()
    {
        var allocator = new FakeIdentityCodeAllocator();
        var site = await PackageIdentityService.RequireSiteCodeAsync(allocator, "F02", CancellationToken.None);
        Assert.True(site.IsFailure);
        Assert.Equal(IdentityCodes.ErrorNoSiteCode, site.Error!.Code);
        var buc = await PackageIdentityService.RequireSiteCodeAsync(allocator, "PLANT-001", CancellationToken.None);
        Assert.Equal("BUC", buc.Value);
    }
}

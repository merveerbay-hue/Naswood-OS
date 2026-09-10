using Naswood.Modules.Business.Application.Production;

namespace Naswood.Modules.Business.UnitTests;

public class ProductionExecutionConcurrencyTests
{
    [Fact]
    public void Consume_hash_changes_with_package_or_qty()
    {
        var pkg = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var a = ProductionExecutionConcurrency.ConsumeHash(pkg, "NW-1", 1.2m, null, null);
        var b = ProductionExecutionConcurrency.ConsumeHash(pkg, "NW-1", 1.2m, null, null);
        var c = ProductionExecutionConcurrency.ConsumeHash(pkg, "NW-1", 1.3m, null, null);
        var d = ProductionExecutionConcurrency.ConsumeHash(Guid.NewGuid(), "NW-1", 1.2m, null, null);
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.NotEqual(a, d);
    }

    [Fact]
    public void Cancel_reasons_are_closed()
    {
        foreach (var reason in ProductionExecutionPolicy.CancelReasons)
            Assert.True(ProductionExecutionPolicy.NormalizeCancelReason(reason).IsSuccess);
        Assert.False(ProductionExecutionPolicy.NormalizeCancelReason("").IsSuccess);
    }
}

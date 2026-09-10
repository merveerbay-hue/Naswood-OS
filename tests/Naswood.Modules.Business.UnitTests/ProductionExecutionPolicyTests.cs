using Naswood.Modules.Business.Application.Production;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.UnitTests;

public class ProductionExecutionPolicyTests
{
    [Fact]
    public void Numbering_is_pex_plant_year_ordinal()
    {
        var utc = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal("PEX-F01-26-000007", ProductionExecutionNumbers.Number("PLANT-001", utc, 7));
        Assert.Equal(7, ProductionExecutionNumbers.ParseOrdinal("PEX-F01-26-000007", "PLANT-001", utc));
        Assert.Equal(0, ProductionExecutionNumbers.ParseOrdinal("PO-1", "PLANT-001", utc));
    }

    [Fact]
    public void Sequence_and_complete_rules()
    {
        Assert.Equal("PRD-EXEC-003", ProductionExecutionPolicy.CanStart(ProductionExecutionStatuses.Completed).Error!.Code);
        Assert.True(ProductionExecutionPolicy.CanPause(ProductionExecutionStatuses.Running).IsSuccess);
        Assert.Equal("PRD-EXEC-DT-001", ProductionExecutionPolicy.CanComplete(ProductionExecutionStatuses.Running, true).Error!.Code);
        Assert.True(ProductionExecutionPolicy.CanComplete(ProductionExecutionStatuses.Paused, false).IsSuccess);
        Assert.Equal("PRD-EXEC-005", ProductionExecutionPolicy.CanCancel(ProductionExecutionStatuses.Running, true).Error!.Code);
        Assert.Equal("PRD-EXEC-SCRAP-001", ProductionExecutionPolicy.GuardScrapQty(0, 2, 0).Error!.Code);
        Assert.Equal("PRD-EXEC-SCRAP-001", ProductionExecutionPolicy.GuardScrapQty(3, 2, 0).Error!.Code);
        Assert.True(ProductionExecutionPolicy.GuardScrapQty(1, 2, 0).IsSuccess);
    }

    [Fact]
    public void Output_types_distinguish_stock()
    {
        Assert.False(ProductionOperationOutputTypes.RequiresStockOutput("NONE"));
        Assert.True(ProductionOperationOutputTypes.RequiresStockOutput("WIP"));
        Assert.True(ProductionOperationOutputTypes.RequiresStockOutput("FINAL_OUTPUT"));
    }
}

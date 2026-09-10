using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public static class ProductionExecutionPolicy
{
    public static readonly string[] DowntimeReasons = ["MACHINE", "MATERIAL", "QUALITY", "SETUP", "MAINTENANCE", "OTHER"];
    public static readonly string[] ScrapReasons = ["CUTTING", "DEFECT", "CRACK", "MOISTURE", "QUALITY", "MACHINE", "SETUP", "TRIM", "OTHER"];

    public static Result CanStart(string status)
    {
        if (status == ProductionExecutionStatuses.Completed)
            return Fail("PRD-EXEC-003", "Operasyon zaten tamamlanmış.");
        if (status == ProductionExecutionStatuses.Cancelled)
            return Fail("PRD-EXEC-001", "İptal edilen icra yeniden başlatılamaz.");
        if (status is ProductionExecutionStatuses.NotStarted or ProductionExecutionStatuses.Running)
            return Result.Success();
        return Fail("PRD-EXEC-001", "Operasyon başlatılamıyor.");
    }

    public static Result CanPause(string status)
        => status == ProductionExecutionStatuses.Running
            ? Result.Success()
            : Fail("PRD-EXEC-004", "Yalnız çalışan icra duraklatılabilir.");

    public static Result CanResume(string status)
        => status == ProductionExecutionStatuses.Paused
            ? Result.Success()
            : Fail("PRD-EXEC-004", "Yalnız duraklatılmış icra sürdürülebilir.");

    public static Result CanComplete(string status, bool openDowntime)
    {
        if (status == ProductionExecutionStatuses.Completed)
            return Result.Success();
        if (openDowntime)
            return Fail("PRD-EXEC-DT-001", "Aktif duruş kapatılmadan operasyon tamamlanamaz.");
        if (status is ProductionExecutionStatuses.Running or ProductionExecutionStatuses.Paused)
            return Result.Success();
        return Fail("PRD-EXEC-001", "Operasyon tamamlanamıyor.");
    }

    public static Result CanCancel(string status, bool hasPostedStock)
    {
        if (status is ProductionExecutionStatuses.Completed or ProductionExecutionStatuses.Cancelled)
            return Fail("PRD-EXEC-003", "Kapalı icra iptal edilemez.");
        if (hasPostedStock)
            return Fail("PRD-EXEC-005", "Stok hareketi oluşmuş icra basit iptal edilemez; mevcut reversal kullanın.");
        return Result.Success();
    }

    public static Result NormalizeDowntimeReason(string? reason)
    {
        var r = (reason ?? string.Empty).Trim().ToUpperInvariant();
        return DowntimeReasons.Contains(r)
            ? Result.Success()
            : Fail("PRD-EXEC-DT-002", "Geçerli duruş nedeni seçin.");
    }

    public static Result NormalizeScrapReason(string? reason)
    {
        var r = (reason ?? string.Empty).Trim().ToUpperInvariant();
        return ScrapReasons.Contains(r)
            ? Result.Success()
            : Fail("PRD-EXEC-SCRAP-001", "Geçerli fire nedeni seçin.");
    }

    public static Result GuardScrapQty(decimal qty, decimal consumedInput, decimal existingScrap)
    {
        if (qty <= 0)
            return Fail("PRD-EXEC-SCRAP-001", "Fire miktarı pozitif olmalı.");
        if (consumedInput > 0 && existingScrap + qty > consumedInput)
            return Fail("PRD-EXEC-SCRAP-001", "Fire, tüketilen girdiyi aşamaz.");
        return Result.Success();
    }

    private static Result Fail(string code, string message) => Result.Failure(Error.Validation(code, message));
}

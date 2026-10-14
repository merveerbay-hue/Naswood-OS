using System.Security.Cryptography;
using System.Text;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Contracts.Production;

namespace Naswood.Modules.Business.Application.Production;

public static class ProductionExecutionConcurrency
{
    public static bool IsOptimisticConflict(Exception ex)
    {
        for (var e = ex; e is not null; e = e.InnerException)
        {
            if (e.GetType().Name.Contains("Concurrency", StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static bool IsUniqueViolation(Exception ex, string? constraintHint = null)
    {
        for (var e = ex; e is not null; e = e.InnerException)
        {
            var sql = e.GetType().GetProperty("SqlState")?.GetValue(e) as string;
            if (sql != "23505") continue;
            if (string.IsNullOrWhiteSpace(constraintHint)) return true;
            var name = (e.GetType().GetProperty("ConstraintName")?.GetValue(e) as string)
                ?? e.Message;
            if (name.Contains(constraintHint, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static Result<T> PackageConflict<T>()
        => Result.Failure<T>(Error.Conflict("PRD-EXEC-CONC-001", "Paket miktarı başka bir işlem tarafından değiştirildi. Barkodu yeniden okutun."));

    public static Result<T> IdempotencyConflict<T>()
        => Result.Failure<T>(Error.Conflict("PRD-EXEC-IDEM-001", "Aynı idempotency anahtarı farklı bir tüketim ile kullanıldı."));

    public static string ConsumeHash(Guid? packageId, string? barcode, decimal quantity, Guid? contentId, decimal? pieceCount)
        => Sha256($"{packageId:D}|{(barcode ?? "").Trim().ToUpperInvariant()}|{quantity:0.####}|{contentId:D}|{pieceCount:0.####}");

    public static string CompleteHash(
        Guid? materialId, string? warehouse, string? location, string? key,
        IReadOnlyList<ProductionOutputLineRequestDto>? lines)
    {
        var linePart = string.Join("|", (lines ?? []).Select(l =>
            $"{l.PhysicalGroupLabel}|{l.ThicknessMm:0.####}|{l.WidthMm:0.####}|{l.LengthMm:0.####}|{l.PieceCount:0.####}|{l.MeasuredVolumeM3:0.######}"));
        return Sha256($"{materialId:D}|{(warehouse ?? "").Trim().ToUpperInvariant()}|{(location ?? "").Trim().ToUpperInvariant()}|{(key ?? "").Trim()}|{linePart}");
    }

    public static string CancelHash(string? reason, string? note, string? key)
        => Sha256($"{(reason ?? "").Trim().ToUpperInvariant()}|{(note ?? "").Trim()}|{(key ?? "").Trim()}");

    private static string Sha256(string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes);
    }
}

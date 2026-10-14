namespace Naswood.Modules.Business.Application.Common;

/// <summary>
/// Factory/Plant access helpers. PlantId maps to Factory (Ana Üs / Diğer Tesis).
/// HomePlantId = first assigned plant on the user account.
/// </summary>
public static class PlantAccess
{
    public static bool CanAccess(IReadOnlyList<string>? allowedPlantIds, string? plantId)
    {
        if (string.IsNullOrWhiteSpace(plantId)) return false;
        if (allowedPlantIds is null || allowedPlantIds.Count == 0) return false;
        return allowedPlantIds.Any(p => string.Equals(p, plantId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static string Normalize(string? plantId, string fallback = "PLANT-001") =>
        string.IsNullOrWhiteSpace(plantId) ? fallback : plantId.Trim();
}

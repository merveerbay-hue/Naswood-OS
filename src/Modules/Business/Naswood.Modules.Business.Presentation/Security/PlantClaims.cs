using System.Security.Claims;

namespace Naswood.Modules.Business.Presentation.Security;

/// <summary>Reads factory/plant claims from the access token (PlantId ≈ FactoryId).</summary>
public static class PlantClaims
{
    public const string WorkingPlant = "plant_id";
    public const string HomePlant = "home_plant_id";
    public const string AllowedPlant = "plant_ids";

    public static string? WorkingPlantId(ClaimsPrincipal? user) =>
        user?.FindFirstValue(WorkingPlant);

    public static string? HomePlantId(ClaimsPrincipal? user) =>
        user?.FindFirstValue(HomePlant) ?? WorkingPlantId(user);

    public static IReadOnlyList<string> AllowedPlantIds(ClaimsPrincipal? user)
    {
        if (user is null) return Array.Empty<string>();
        var fromClaims = user.FindAll(AllowedPlant)
            .Select(c => c.Value?.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Cast<string>()
            .ToArray();
        if (fromClaims.Length > 0) return fromClaims;

        var working = WorkingPlantId(user);
        return string.IsNullOrWhiteSpace(working) ? Array.Empty<string>() : [working];
    }

    /// <summary>
    /// Resolve view/work plant: explicit query/body, else session working plant, else home.
    /// Must be in AllowedPlantIds.
    /// </summary>
    public static (string? PlantId, string? Error) ResolveRequestedPlant(
        ClaimsPrincipal? user,
        string? requestedPlantId)
    {
        var allowed = AllowedPlantIds(user);
        var candidate = !string.IsNullOrWhiteSpace(requestedPlantId)
            ? requestedPlantId.Trim()
            : WorkingPlantId(user) ?? HomePlantId(user);

        if (string.IsNullOrWhiteSpace(candidate))
            return (null, "Plant / factory context is required.");

        if (allowed.Count > 0
            && !allowed.Any(p => string.Equals(p, candidate, StringComparison.OrdinalIgnoreCase)))
            return (null, "Bu tesise erişim yetkiniz yok.");

        return (candidate, null);
    }
}

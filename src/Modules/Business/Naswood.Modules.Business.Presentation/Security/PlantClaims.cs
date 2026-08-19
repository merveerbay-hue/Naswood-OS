using System.Security.Claims;

namespace Naswood.Modules.Business.Presentation.Security;

/// <summary>
/// Factory/plant claims from the access token (PlantId ≈ FactoryId).
/// Operasyon kullanıcıları: yalnızca Ana Üs. Üst Yönetici: AllowedPlantIds arasında switch.
/// </summary>
public static class PlantClaims
{
    public const string WorkingPlant = "plant_id";
    public const string HomePlant = "home_plant_id";
    public const string AllowedPlant = "plant_ids";
    public const string PlantSwitch = "plant_switch";

    public static string? WorkingPlantId(ClaimsPrincipal? user) =>
        user?.FindFirstValue(WorkingPlant);

    public static string? HomePlantId(ClaimsPrincipal? user) =>
        user?.FindFirstValue(HomePlant) ?? WorkingPlantId(user);

    /// <summary>Üst Yönetici JWT claim — issued when role may switch plants.</summary>
    public static bool CanSwitchPlant(ClaimsPrincipal? user) =>
        string.Equals(user?.FindFirstValue(PlantSwitch), "1", StringComparison.Ordinal);

    /// <summary>
    /// Plants the caller may access.
    /// When CanSwitchPlant is false, always returns only HomeFactoryId (never other factories).
    /// </summary>
    public static IReadOnlyList<string> AllowedPlantIds(ClaimsPrincipal? user)
    {
        if (user is null) return Array.Empty<string>();

        var home = HomePlantId(user);
        if (!CanSwitchPlant(user))
        {
            return string.IsNullOrWhiteSpace(home)
                ? Array.Empty<string>()
                : [home.Trim()];
        }

        var fromClaims = user.FindAll(AllowedPlant)
            .Select(c => c.Value?.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Cast<string>()
            .ToArray();
        if (fromClaims.Length > 0) return fromClaims;

        return string.IsNullOrWhiteSpace(home) ? Array.Empty<string>() : [home.Trim()];
    }

    /// <summary>
    /// Resolve view/work plant.
    /// - PlantId omitted → HomeFactory / working plant.
    /// - CanSwitchPlant=false → requested must be HomeFactoryId (else 403); resolve to home.
    /// - CanSwitchPlant=true → requested must be in AllowedPlantIds (else 403).
    /// </summary>
    public static (string? PlantId, string? Error) ResolveRequestedPlant(
        ClaimsPrincipal? user,
        string? requestedPlantId)
    {
        var home = HomePlantId(user);
        var canSwitch = CanSwitchPlant(user);
        var allowed = AllowedPlantIds(user);

        if (!canSwitch)
        {
            if (string.IsNullOrWhiteSpace(home))
                return (null, "Plant / factory context is required.");

            if (!string.IsNullOrWhiteSpace(requestedPlantId)
                && !string.Equals(requestedPlantId.Trim(), home, StringComparison.OrdinalIgnoreCase))
            {
                return (null, "Bu hesap yalnızca Ana Fabrika bağlamında çalışabilir; tesis değiştirme yetkisi yok.");
            }

            return (home.Trim(), null);
        }

        var candidate = !string.IsNullOrWhiteSpace(requestedPlantId)
            ? requestedPlantId.Trim()
            : WorkingPlantId(user) ?? home;

        if (string.IsNullOrWhiteSpace(candidate))
            return (null, "Plant / factory context is required.");

        if (allowed.Count == 0
            || !allowed.Any(p => string.Equals(p, candidate, StringComparison.OrdinalIgnoreCase)))
            return (null, "Bu tesise erişim yetkiniz yok.");

        return (candidate, null);
    }
}

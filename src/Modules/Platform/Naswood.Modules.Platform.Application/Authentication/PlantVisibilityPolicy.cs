namespace Naswood.Modules.Platform.Application.Authentication;

/// <summary>
/// Factory / plant visibility by persona:
/// - Operatör / Mühendis / Depo Sorumlusu → only Ana Üs (HomeFactory)
/// - Üst Yönetici → may switch among authorized plants (home unchanged)
/// - Sistem Yöneticisi → plant definition + user-factory assignment
/// </summary>
public static class PlantVisibilityPolicy
{
    /// <summary>Roles allowed to switch working plant among PlantIds without changing HomePlantId.</summary>
    public static readonly HashSet<string> PlantSwitchRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Administrator",
        "Executive",
        "UpperManagement",
        "CEO",
        "Director"
    };

    /// <summary>Roles locked to a single Home Factory (Ana Üs).</summary>
    public static readonly HashSet<string> HomeFactoryOnlyRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "WarehouseOperator",
        "Operator",
        "Engineer",
        "QualityEngineer",
        "WarehouseResponsible",
        "DepoSorumlusu",
        "Buyer",
        "ReadOnly"
    };

    public static bool CanSwitchPlant(IEnumerable<string>? roles)
    {
        if (roles is null) return false;
        foreach (var role in roles)
        {
            if (string.IsNullOrWhiteSpace(role)) continue;
            if (PlantSwitchRoles.Contains(role.Trim()))
                return true;
        }
        return false;
    }

    public static bool IsHomeFactoryLocked(IEnumerable<string>? roles) => !CanSwitchPlant(roles);

    /// <summary>
    /// Plants the user may see in menus, lists, and JWT plant_ids claims.
    /// Home-locked personas never receive other factory codes — even if mis-assigned in DB.
    /// </summary>
    public static IReadOnlyList<string> VisiblePlantIds(
        IEnumerable<string>? roles,
        string? homePlantId,
        IReadOnlyList<string>? assignedPlantIds)
    {
        var assigned = (assignedPlantIds ?? Array.Empty<string>())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (CanSwitchPlant(roles))
            return assigned;

        var home = string.IsNullOrWhiteSpace(homePlantId)
            ? assigned.FirstOrDefault()
            : homePlantId.Trim();

        return string.IsNullOrWhiteSpace(home) ? Array.Empty<string>() : [home];
    }

    /// <summary>Operatör-tier users may only be assigned a single plant (Ana Üs).</summary>
    public static bool RequiresSinglePlantAssignment(IEnumerable<string>? roles)
    {
        if (roles is null) return false;
        var list = roles.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim()).ToArray();
        if (list.Length == 0) return false;
        if (CanSwitchPlant(list)) return false;
        return list.Any(r => HomeFactoryOnlyRoles.Contains(r)) || list.All(r => !PlantSwitchRoles.Contains(r));
    }
}

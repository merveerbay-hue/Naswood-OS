using System.Text.Json;
using Naswood.Modules.Business.Domain.Inventory;

namespace Naswood.Modules.Business.Application.Production;

public sealed record ProductionOutputQcDecision(
    string ResolvedStatus,
    bool HoldRequired,
    string PolicySource,
    bool OverrideRequired);

/// <summary>
/// Output stock status comes from material / operation policy, not a free operator dropdown.
/// Operator may tighten to Quarantine. Loosening a required hold needs an explicit override.
/// </summary>
public static class ProductionOutputQcPolicy
{
    public static ProductionOutputQcDecision Resolve(
        Material material,
        string? workCenterCode,
        string? requestedStatus)
    {
        var requested = Normalize(requestedStatus);
        var explicitStatus = ReadExplicitStatus(material.DefinitionJson);
        if (explicitStatus is not null)
            return Decide(explicitStatus, "material", requested);

        if (LooksLikeHoldProcess(workCenterCode))
            return Decide("Quarantine", "workCenter", requested);

        if (LooksLikeHoldProcess(material.Category) || LooksLikeHoldProcess(ReadMainCategory(material.DefinitionJson)))
            return Decide("Quarantine", "category", requested);

        var fallback = string.IsNullOrWhiteSpace(requested) ? "Available" : requested;
        return Decide(fallback, "request", requested);
    }

    public static string Apply(ProductionOutputQcDecision decision, string? requestedStatus, bool allowOverride)
    {
        var requested = Normalize(requestedStatus);
        if (decision.HoldRequired && allowOverride && requested == "Available")
            return "Available";
        if (decision.HoldRequired)
            return "Quarantine";
        return IsHold(requested) ? "Quarantine" : decision.ResolvedStatus;
    }

    public static bool IsHold(string? status)
    {
        var s = (status ?? string.Empty).Trim().ToUpperInvariant().Replace('_', '-');
        return s is "QUARANTINE" or "HOLD" or "QC-HOLD" or "QCHOLD";
    }

    public static string Normalize(string? status)
    {
        var s = (status ?? string.Empty).Trim();
        if (s.Length == 0) return "";
        if (IsHold(s)) return "Quarantine";
        if (string.Equals(s, "Available", StringComparison.OrdinalIgnoreCase)
            || string.Equals(s, "Active", StringComparison.OrdinalIgnoreCase))
            return "Available";
        return s;
    }

    private static ProductionOutputQcDecision Decide(string policyStatus, string source, string requested)
    {
        var hold = IsHold(policyStatus);
        if (hold && requested == "Available")
            return new("Quarantine", true, source, true);
        if (hold)
            return new("Quarantine", true, source, false);
        if (IsHold(requested))
            return new("Quarantine", false, source, false);
        return new(string.IsNullOrWhiteSpace(policyStatus) ? "Available" : policyStatus, false, source, false);
    }

    private static string? ReadExplicitStatus(string? definitionJson)
    {
        var root = Parse(definitionJson);
        if (root is null) return null;
        if (ReadBool(root.Value, "qcHold", "QcHold", "quarantineOnOutput", "QuarantineOnOutput"))
            return "Quarantine";
        var raw = ReadString(root.Value, "outputStockStatus", "OutputStockStatus", "qcStatus", "QcStatus");
        return string.IsNullOrWhiteSpace(raw) ? null : (IsHold(raw) ? "Quarantine" : Normalize(raw));
    }

    private static string? ReadMainCategory(string? definitionJson)
    {
        var root = Parse(definitionJson);
        return root is null ? null : ReadString(root.Value, "mainCategory", "MainCategory");
    }

    public static bool LooksLikeHoldProcess(string? token)
    {
        var t = (token ?? string.Empty).Trim().ToUpperInvariant();
        if (t.Length == 0) return false;
        return t is "CLT" or "GLULAM" or "GLT" or "LAM" or "GL"
            || t.Contains("CLT", StringComparison.Ordinal)
            || t.Contains("GLULAM", StringComparison.Ordinal)
            || t.Contains("GLT", StringComparison.Ordinal)
            || t.Contains("-LAM", StringComparison.Ordinal)
            || t.StartsWith("LAM", StringComparison.Ordinal);
    }

    private static JsonElement? Parse(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? ReadString(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.String)
                return el.GetString();
        }
        return null;
    }

    private static bool ReadBool(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(name, out var el))
                continue;
            if (el.ValueKind == JsonValueKind.True) return true;
            if (el.ValueKind == JsonValueKind.False) return false;
            if (el.ValueKind == JsonValueKind.String
                && bool.TryParse(el.GetString(), out var b))
                return b;
        }
        return false;
    }
}

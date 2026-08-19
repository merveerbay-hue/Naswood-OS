using System.Text.Json;
using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Business.Application.Inventory;

/// <summary>
/// EN 14081 product-scope flags stored in Material.DefinitionJson (not production-lot results).
/// Legacy / missing keys → NORMAL_STOCK (safe default).
/// </summary>
public static class MaterialCompliance
{
    public const string ScopeNormalStock = "NORMAL_STOCK";
    public const string ScopeStructuralTimber = "STRUCTURAL_TIMBER";
    public const string MethodVisual = "VISUAL";
    public const string MethodMachine = "MACHINE";

    private static readonly HashSet<string> AllowedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        MethodVisual,
        MethodMachine
    };

    public static Result ValidateDefinitionJson(string? definitionJson)
    {
        if (string.IsNullOrWhiteSpace(definitionJson))
            return Result.Success(); // empty JSON → treat as legacy NORMAL_STOCK

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(definitionJson);
        }
        catch (JsonException)
        {
            return Result.Failure(Error.Validation("BUS-MAT-JSON", "DefinitionJson is not valid JSON."));
        }

        using (doc)
        {
            var root = doc.RootElement;
            var scope = ReadScope(root);
            var methods = ReadMethods(root);

            if (IsExcludedFromEn14081(root) && string.Equals(scope, ScopeStructuralTimber, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure(Error.Validation(
                    "BUS-MAT-EN14081-EXCL",
                    "Thermowood, finger-jointed, or other excluded product types cannot use STRUCTURAL_TIMBER compliance scope."));
            }

            if (string.Equals(scope, ScopeNormalStock, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(scope))
            {
                if (methods.Count > 0)
                {
                    return Result.Failure(Error.Validation(
                        "BUS-MAT-EN14081-SCOPE",
                        "NORMAL_STOCK materials cannot declare SupportedGradingMethods (VISUAL/MACHINE)."));
                }
                return Result.Success();
            }

            if (!string.Equals(scope, ScopeStructuralTimber, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure(Error.Validation(
                    "BUS-MAT-EN14081-SCOPE",
                    "ComplianceScope must be NORMAL_STOCK or STRUCTURAL_TIMBER."));
            }

            if (methods.Count == 0)
            {
                return Result.Failure(Error.Validation(
                    "BUS-MAT-EN14081-METHOD",
                    "STRUCTURAL_TIMBER requires at least one SupportedGradingMethod (VISUAL and/or MACHINE)."));
            }

            foreach (var m in methods)
            {
                if (!AllowedMethods.Contains(m))
                {
                    return Result.Failure(Error.Validation(
                        "BUS-MAT-EN14081-METHOD",
                        $"Unsupported grading method '{m}'. Allowed: VISUAL, MACHINE."));
                }
            }

            return Result.Success();
        }
    }

    /// <summary>Normalize JSON for persistence: legacy empty → NORMAL_STOCK; strip methods on normal stock.</summary>
    public static string EnsureSafeDefaults(string? definitionJson)
    {
        if (string.IsNullOrWhiteSpace(definitionJson))
        {
            return JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                ["complianceScope"] = ScopeNormalStock,
                ["supportedGradingMethods"] = Array.Empty<string>()
            });
        }

        try
        {
            using var doc = JsonDocument.Parse(definitionJson);
            var dict = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in doc.RootElement.EnumerateObject())
                dict[p.Name] = p.Value.Clone();

            var scope = dict.TryGetValue("complianceScope", out var scopeEl)
                ? scopeEl.GetString()?.Trim()
                : null;
            if (string.IsNullOrWhiteSpace(scope))
                scope = ScopeNormalStock;

            var methods = new List<string>();
            if (dict.TryGetValue("supportedGradingMethods", out var methodsEl)
                && methodsEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in methodsEl.EnumerateArray())
                {
                    var s = item.GetString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(s))
                        methods.Add(s);
                }
            }

            if (string.Equals(scope, ScopeNormalStock, StringComparison.OrdinalIgnoreCase))
                methods.Clear();

            // Rebuild via JsonNode-like approach: serialize a merged dictionary of raw JSON
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                foreach (var kv in dict)
                {
                    if (string.Equals(kv.Key, "complianceScope", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(kv.Key, "supportedGradingMethods", StringComparison.OrdinalIgnoreCase))
                        continue;
                    writer.WritePropertyName(kv.Key);
                    kv.Value.WriteTo(writer);
                }
                writer.WriteString("complianceScope", scope);
                writer.WritePropertyName("supportedGradingMethods");
                writer.WriteStartArray();
                foreach (var m in methods.Distinct(StringComparer.OrdinalIgnoreCase))
                    writer.WriteStringValue(m.ToUpperInvariant());
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
        catch (JsonException)
        {
            return definitionJson ?? string.Empty;
        }
    }

    private static string ReadScope(JsonElement root)
    {
        if (root.TryGetProperty("complianceScope", out var el)
            || root.TryGetProperty("ComplianceScope", out el))
        {
            return el.GetString()?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }

    private static List<string> ReadMethods(JsonElement root)
    {
        var list = new List<string>();
        JsonElement el;
        if (!root.TryGetProperty("supportedGradingMethods", out el)
            && !root.TryGetProperty("SupportedGradingMethods", out el))
            return list;

        if (el.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in el.EnumerateArray())
            {
                var s = item.GetString()?.Trim();
                if (!string.IsNullOrWhiteSpace(s)) list.Add(s);
            }
        }
        else if (el.ValueKind == JsonValueKind.String)
        {
            var s = el.GetString()?.Trim();
            if (!string.IsNullOrWhiteSpace(s) && !string.Equals(s, "NOT_APPLICABLE", StringComparison.OrdinalIgnoreCase))
                list.Add(s);
        }
        return list;
    }

    /// <summary>
    /// Products excluded from EN 14081 structural timber flow (keep NORMAL_STOCK).
    /// </summary>
    public static bool IsExcludedFromEn14081(JsonElement root)
    {
        var main = ReadString(root, "mainCategory") ?? ReadString(root, "MainCategory");
        if (string.Equals(main, "TW", StringComparison.OrdinalIgnoreCase))
            return true;

        if (ReadBool(root, "isThermowood") || ReadBool(root, "IsThermowood"))
            return true;

        var productType = ReadString(root, "productTypeToken") ?? ReadString(root, "ProductTypeToken");
        if (string.Equals(productType, "FJ", StringComparison.OrdinalIgnoreCase))
            return true;

        // Explicit future flags (chemical / fire-retardant) if present in JSON
        if (ReadBool(root, "isChemicallyModified") || ReadBool(root, "isFireRetardantTreated"))
            return true;

        return false;
    }

    private static string? ReadString(JsonElement root, string name) =>
        root.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()
            : null;

    private static bool ReadBool(JsonElement root, string name) =>
        root.TryGetProperty(name, out var el)
        && ((el.ValueKind == JsonValueKind.True)
            || (el.ValueKind == JsonValueKind.String
                && bool.TryParse(el.GetString(), out var b)
                && b));
}

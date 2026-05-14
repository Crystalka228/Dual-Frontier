using System;
using System.Collections.Generic;
using System.Text.Json;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

internal static class ManifestParser
{
    /// <summary>
    /// Parses a mod manifest JSON string and returns a fully populated
    /// <see cref="ModManifest"/>. Supports both the v1 (legacy, string-array
    /// dependencies, no <c>kind</c>/<c>apiVersion</c>/<c>hotReload</c>/
    /// <c>capabilities</c>/<c>replaces</c>) and v2 schemas transparently.
    /// </summary>
    /// <param name="json">Raw JSON content of <c>mod.manifest.json</c>.</param>
    /// <param name="sourcePath">
    /// Path shown in exception messages. Use the manifest file path.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the JSON is malformed, required fields are missing or empty,
    /// a field cannot be parsed into its target type, or a capability string is
    /// invalid.
    /// </exception>
    public static ModManifest Parse(string json, string sourcePath = "")
    {
        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            });
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse mod manifest at '{sourcePath}': {ex.Message}", ex);
        }

        using (doc)
        {
            JsonElement root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                throw new InvalidOperationException(
                    $"Mod manifest at '{sourcePath}' must be a JSON object.");

            // K8.3+K8.4 — strict v3-only manifest gate. Missing or non-"3"
            // value rejects with ValidationErrorKind.IncompatibleContractsVersion
            // semantic. No grace period; no backward compatibility.
            string manifestVersion = ReadRequiredString(root, "manifestVersion", sourcePath);
            if (manifestVersion != "3")
                throw new InvalidOperationException(
                    $"Mod manifest at '{sourcePath}' declares manifestVersion: " +
                    $"'{manifestVersion}' — only '3' is accepted post-K8.3+K8.4 " +
                    $"combined milestone. v1/v2 manifests must be updated to v3 " +
                    $"format (ValidationErrorKind.IncompatibleContractsVersion).");

            string id = ReadRequiredString(root, "id", sourcePath);
            string name = ReadRequiredString(root, "name", sourcePath);
            string version = ReadRequiredString(root, "version", sourcePath);

            string author = ReadOptionalString(root, "author", "", sourcePath);
            string requiresVersion = ReadOptionalString(root, "requiresContractsVersion", "1.0.0", sourcePath);
            string entryAssembly = ReadOptionalString(root, "entryAssembly", "", sourcePath);
            string entryType = ReadOptionalString(root, "entryType", "", sourcePath);

            ModKind kind = ReadKind(root, sourcePath);
            VersionConstraint? apiVersion = ReadApiVersion(root, sourcePath);
            bool hotReload = ReadOptionalBool(root, "hotReload", false, sourcePath);
            IReadOnlyList<string> replaces = ReadReplaces(root, sourcePath);
            IReadOnlyList<ModDependency> dependencies = ReadDependencies(root, sourcePath);
            ManifestCapabilities capabilities = ReadCapabilities(root, sourcePath);

            return new ModManifest
            {
                ManifestVersion = manifestVersion,
                Id = id,
                Name = name,
                Version = version,
                Author = author,
                RequiresContractsVersion = requiresVersion,
                EntryAssembly = entryAssembly,
                EntryType = entryType,
                Kind = kind,
                ApiVersion = apiVersion,
                HotReload = hotReload,
                Replaces = replaces,
                Dependencies = dependencies,
                Capabilities = capabilities,
            };
        }
    }

    // System.Text.Json's TryGetProperty is case-sensitive; this walks the object once.
    private static bool TryGetPropertyCi(JsonElement obj, string name, out JsonElement value)
    {
        foreach (JsonProperty p in obj.EnumerateObject())
        {
            if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = p.Value;
                return true;
            }
        }
        value = default;
        return false;
    }

    private static string ReadRequiredString(JsonElement root, string key, string sourcePath)
    {
        if (!TryGetPropertyCi(root, key, out JsonElement element))
            throw new InvalidOperationException(
                $"Mod manifest at '{sourcePath}' is missing required field '{key}'.");

        if (element.ValueKind == JsonValueKind.Null)
            throw new InvalidOperationException(
                $"Mod manifest at '{sourcePath}' has null required field '{key}'.");

        if (element.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException(
                $"Mod manifest field '{key}' at '{sourcePath}' must be a string.");

        string? value = element.GetString();
        if (string.IsNullOrEmpty(value))
            throw new InvalidOperationException(
                $"Mod manifest at '{sourcePath}' has empty required field '{key}'.");

        return value;
    }

    private static string ReadOptionalString(JsonElement root, string key, string defaultValue, string sourcePath)
    {
        if (!TryGetPropertyCi(root, key, out JsonElement element))
            return defaultValue;

        if (element.ValueKind == JsonValueKind.Null)
            return defaultValue;

        if (element.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException(
                $"Mod manifest field '{key}' at '{sourcePath}' must be a string.");

        return element.GetString() ?? defaultValue;
    }

    private static bool ReadOptionalBool(JsonElement root, string key, bool defaultValue, string sourcePath)
    {
        if (!TryGetPropertyCi(root, key, out JsonElement element))
            return defaultValue;

        if (element.ValueKind == JsonValueKind.Null)
            return defaultValue;

        if (element.ValueKind == JsonValueKind.True)
            return true;

        if (element.ValueKind == JsonValueKind.False)
            return false;

        throw new InvalidOperationException(
            $"Mod manifest field '{key}' at '{sourcePath}' must be a boolean.");
    }

    private static ModKind ReadKind(JsonElement root, string sourcePath)
    {
        if (!TryGetPropertyCi(root, "kind", out JsonElement element))
            return ModKind.Regular;

        if (element.ValueKind == JsonValueKind.Null)
            return ModKind.Regular;

        if (element.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException(
                $"Mod manifest field 'kind' at '{sourcePath}' must be a string.");

        string value = element.GetString()!;

        if (string.Equals(value, "regular", StringComparison.OrdinalIgnoreCase))
            return ModKind.Regular;

        if (string.Equals(value, "shared", StringComparison.OrdinalIgnoreCase))
            return ModKind.Shared;

        throw new InvalidOperationException(
            $"Mod manifest at '{sourcePath}' has unknown 'kind' value '{value}'. " +
            "Expected 'regular' or 'shared'.");
    }

    private static VersionConstraint? ReadApiVersion(JsonElement root, string sourcePath)
    {
        if (!TryGetPropertyCi(root, "apiVersion", out JsonElement element))
            return null;

        if (element.ValueKind == JsonValueKind.Null)
            return null;

        if (element.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException(
                $"Mod manifest field 'apiVersion' at '{sourcePath}' must be a string.");

        string value = element.GetString()!;

        try
        {
            return VersionConstraint.Parse(value);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                $"Mod manifest at '{sourcePath}' has invalid 'apiVersion': {ex.Message}", ex);
        }
        catch (ArgumentNullException ex)
        {
            throw new InvalidOperationException(
                $"Mod manifest at '{sourcePath}' has invalid 'apiVersion': {ex.Message}", ex);
        }
    }

    private static IReadOnlyList<string> ReadReplaces(JsonElement root, string sourcePath)
    {
        if (!TryGetPropertyCi(root, "replaces", out JsonElement element))
            return Array.Empty<string>();

        if (element.ValueKind == JsonValueKind.Null)
            return Array.Empty<string>();

        if (element.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException(
                $"Mod manifest field 'replaces' at '{sourcePath}' must be an array.");

        List<string> result = new();
        foreach (JsonElement entry in element.EnumerateArray())
        {
            if (entry.ValueKind != JsonValueKind.String)
                throw new InvalidOperationException(
                    $"Mod manifest field 'replaces' at '{sourcePath}' must contain only strings.");

            string? text = entry.GetString();
            if (string.IsNullOrEmpty(text))
                throw new InvalidOperationException(
                    $"Mod manifest field 'replaces' at '{sourcePath}' contains an empty string.");

            result.Add(text);
        }

        return result.Count == 0 ? Array.Empty<string>() : result;
    }

    private static IReadOnlyList<ModDependency> ReadDependencies(JsonElement root, string sourcePath)
    {
        if (!TryGetPropertyCi(root, "dependencies", out JsonElement element))
            return Array.Empty<ModDependency>();

        if (element.ValueKind == JsonValueKind.Null)
            return Array.Empty<ModDependency>();

        if (element.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException(
                $"Mod manifest field 'dependencies' at '{sourcePath}' must be an array.");

        JsonValueKind formatKind = JsonValueKind.Undefined;
        foreach (JsonElement first in element.EnumerateArray())
        {
            formatKind = first.ValueKind;
            break;
        }

        if (formatKind == JsonValueKind.Undefined)
            return Array.Empty<ModDependency>();

        if (formatKind != JsonValueKind.String && formatKind != JsonValueKind.Object)
            throw new InvalidOperationException(
                $"Mod manifest field 'dependencies' at '{sourcePath}' must contain " +
                "strings (v1) or objects (v2).");

        List<ModDependency> result = new();
        foreach (JsonElement entry in element.EnumerateArray())
        {
            if (entry.ValueKind != formatKind)
                throw new InvalidOperationException(
                    $"Mod manifest field 'dependencies' at '{sourcePath}' mixes string " +
                    "and object entries; choose one format.");

            if (formatKind == JsonValueKind.String)
            {
                string? depId = entry.GetString();
                if (string.IsNullOrEmpty(depId))
                    throw new InvalidOperationException(
                        $"Mod manifest field 'dependencies' at '{sourcePath}' contains " +
                        "an empty mod id.");
                result.Add(new ModDependency(depId, null, IsOptional: false));
            }
            else
            {
                result.Add(ReadDependencyObject(entry, sourcePath));
            }
        }

        return result.Count == 0 ? Array.Empty<ModDependency>() : result;
    }

    private static ModDependency ReadDependencyObject(JsonElement entry, string sourcePath)
    {
        if (!TryGetPropertyCi(entry, "id", out JsonElement idEl))
            throw new InvalidOperationException(
                $"Mod manifest dependency at '{sourcePath}' is missing required field 'id'.");

        if (idEl.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException(
                $"Mod manifest dependency at '{sourcePath}' has non-string 'id'.");

        string? depId = idEl.GetString();
        if (string.IsNullOrEmpty(depId))
            throw new InvalidOperationException(
                $"Mod manifest dependency at '{sourcePath}' has empty 'id'.");

        VersionConstraint? version = null;
        if (TryGetPropertyCi(entry, "version", out JsonElement verEl) &&
            verEl.ValueKind != JsonValueKind.Null)
        {
            if (verEl.ValueKind != JsonValueKind.String)
                throw new InvalidOperationException(
                    $"Mod manifest dependency '{depId}' at '{sourcePath}' has non-string 'version'.");

            string verText = verEl.GetString()!;
            try
            {
                version = VersionConstraint.Parse(verText);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException(
                    $"Mod manifest dependency '{depId}' at '{sourcePath}' has invalid 'version': {ex.Message}", ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new InvalidOperationException(
                    $"Mod manifest dependency '{depId}' at '{sourcePath}' has invalid 'version': {ex.Message}", ex);
            }
        }

        bool isOptional = false;
        if (TryGetPropertyCi(entry, "optional", out JsonElement optEl) &&
            optEl.ValueKind != JsonValueKind.Null)
        {
            if (optEl.ValueKind == JsonValueKind.True)
                isOptional = true;
            else if (optEl.ValueKind == JsonValueKind.False)
                isOptional = false;
            else
                throw new InvalidOperationException(
                    $"Mod manifest dependency '{depId}' at '{sourcePath}' has non-boolean 'optional'.");
        }

        return new ModDependency(depId, version, isOptional);
    }

    private static ManifestCapabilities ReadCapabilities(JsonElement root, string sourcePath)
    {
        if (!TryGetPropertyCi(root, "capabilities", out JsonElement element))
            return ManifestCapabilities.Empty;

        if (element.ValueKind == JsonValueKind.Null)
            return ManifestCapabilities.Empty;

        if (element.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException(
                $"Mod manifest field 'capabilities' at '{sourcePath}' must be an object.");

        IEnumerable<string>? required = ReadCapabilityArray(element, "required", sourcePath);
        IEnumerable<string>? provided = ReadCapabilityArray(element, "provided", sourcePath);

        try
        {
            return ManifestCapabilities.Parse(required, provided);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException(
                $"Mod manifest at '{sourcePath}' has invalid capability token: {ex.Message}", ex);
        }
    }

    private static IEnumerable<string>? ReadCapabilityArray(JsonElement obj, string key, string sourcePath)
    {
        if (!TryGetPropertyCi(obj, key, out JsonElement element))
            return null;

        if (element.ValueKind == JsonValueKind.Null)
            return null;

        if (element.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException(
                $"Mod manifest field 'capabilities.{key}' at '{sourcePath}' must be an array.");

        List<string> result = new();
        foreach (JsonElement entry in element.EnumerateArray())
        {
            if (entry.ValueKind != JsonValueKind.String)
                throw new InvalidOperationException(
                    $"Mod manifest field 'capabilities.{key}' at '{sourcePath}' must contain only strings.");

            string? text = entry.GetString();
            if (text is not null)
                result.Add(text);
        }

        return result;
    }
}

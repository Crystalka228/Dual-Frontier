using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DualFrontier.Contracts.Distribution;

namespace DualFrontier.Application.Distribution;

/// <summary>
/// Reads <c>game.manifest.json</c> STRICTLY.
///
/// <para>
/// <b>The contrast with <c>ManifestParser</c> is deliberate.</b> The mod manifest parser asks
/// for the keys it knows and never looks at the rest, so an unknown key is silently dropped —
/// three shipped mod manifests carry a <c>description</c> the parser has never read. It also
/// defaults nearly everything: an absent <c>kind</c>, <c>apiVersion</c>, <c>hotReload</c>,
/// <c>replaces</c> or <c>capabilities</c> all become quiet defaults. That forgiveness is right
/// for third-party content the host cannot fix.
/// </para>
///
/// <para>
/// A distribution manifest is not third-party content; it is the shipped definition of the
/// product, and a typo in it is a build error someone can fix before release. So this loader
/// demands every field and REFUSES an unknown key, naming it. That refusal has no precedent in
/// the codebase — nothing here has ever rejected an unrecognised key before — which is exactly
/// why it is stated here rather than assumed.
/// </para>
///
/// <para>
/// <b>Resolution never uses the current directory.</b> The Launcher's working directory is not a
/// property the repository agrees on: a test comment, two mod project comments and the
/// composition root's own default each imply a different answer, and no code path had ever
/// exercised it because production never loaded a mod. Rather than pick a winner, this resolves
/// the way <c>AssetManager</c> already does — an explicit path if given, otherwise a walk upward
/// from the directory holding the binary. In a published distribution that walk terminates
/// immediately, because the manifest sits beside the executable; in the repository it climbs to
/// the root. Both work without anyone having to know the working directory.
/// </para>
/// </summary>
internal static class DistributionManifestLoader
{
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>
    /// Loads the manifest from <paramref name="explicitPath"/>, or finds it by walking upward
    /// from the directory holding the running assembly when no path is given.
    /// </summary>
    /// <exception cref="FileNotFoundException">No manifest could be located.</exception>
    /// <exception cref="InvalidOperationException">The manifest is malformed, incomplete, or carries an unknown key.</exception>
    internal static DistributionManifest Load(string? explicitPath = null)
    {
        string path = explicitPath ?? Locate();
        string json = File.ReadAllText(path);
        return Parse(json, path);
    }

    /// <summary>
    /// The directory the manifest was found in — the DISTRIBUTION ROOT. The composer derives the
    /// mods root and the asset roots from it, so the manifest and the content it names cannot
    /// disagree about where they live.
    /// </summary>
    internal static string RootFor(string manifestPath)
        => Path.GetDirectoryName(Path.GetFullPath(manifestPath))
           ?? throw new InvalidOperationException(
               $"Distribution manifest path '{manifestPath}' has no containing directory.");

    /// <summary>
    /// Walks upward from <c>AppContext.BaseDirectory</c> looking for the manifest. Returns the
    /// first hit; throws naming every directory tried, because "manifest not found" with no list
    /// is the least actionable failure a launcher can produce.
    /// </summary>
    internal static string Locate()
    {
        var tried = new List<string>();
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            string candidate = Path.Combine(dir.FullName, DistributionManifest.FileName);
            tried.Add(candidate);
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }

        throw new FileNotFoundException(
            $"No {DistributionManifest.FileName} found. Tried, in order: {string.Join(", ", tried)}.");
    }

    internal static DistributionManifest Parse(string json, string sourcePath)
    {
        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(json, DocumentOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Distribution manifest at '{sourcePath}' is not valid JSON: {ex.Message}", ex);
        }

        using (doc)
        {
            JsonElement root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException(
                    $"Distribution manifest at '{sourcePath}' must be a JSON object.");
            }

            RejectUnknownKeys(root, sourcePath, "",
                "manifestVersion", "product", "rootMods", "scenario",
                "assetRoots", "saveNamespace", "minEngineCapabilities");

            string version = RequiredString(root, "manifestVersion", sourcePath, "");
            if (version != DistributionManifest.SupportedVersion)
            {
                throw new InvalidOperationException(
                    $"Distribution manifest at '{sourcePath}' declares manifestVersion " +
                    $"'{version}'; this build supports '{DistributionManifest.SupportedVersion}'.");
            }

            return new DistributionManifest(
                version,
                ReadProduct(RequiredObject(root, "product", sourcePath, ""), sourcePath),
                RequiredStringArray(root, "rootMods", sourcePath, ""),
                ReadScenario(RequiredObject(root, "scenario", sourcePath, ""), sourcePath),
                RequiredStringArray(root, "assetRoots", sourcePath, ""),
                RequiredString(root, "saveNamespace", sourcePath, ""),
                RequiredStringArray(root, "minEngineCapabilities", sourcePath, ""));
        }
    }

    private static ProductInfo ReadProduct(JsonElement product, string sourcePath)
    {
        RejectUnknownKeys(product, sourcePath, "product.", "id", "name", "version");
        return new ProductInfo(
            RequiredString(product, "id", sourcePath, "product."),
            RequiredString(product, "name", sourcePath, "product."),
            RequiredString(product, "version", sourcePath, "product."));
    }

    private static ScenarioConfig ReadScenario(JsonElement scenario, string sourcePath)
    {
        RejectUnknownKeys(scenario, sourcePath, "scenario.",
            "id", "worldSeed", "mapWidth", "mapHeight", "obstacleCount",
            "obstacleSeed", "factorySeed", "itemFactorySeed", "counts");

        JsonElement counts = RequiredObject(scenario, "counts", sourcePath, "scenario.");
        RejectUnknownKeys(counts, sourcePath, "scenario.counts.",
            "pawns", "food", "water", "beds", "decorations");

        // Seeds are unconstrained -- any int is a legal seed, including a negative one. Extents
        // and populations are not: a map with no area and a colony of minus five are not
        // scenarios the seeder can decline gracefully, they are numbers that turn into an
        // overflow the moment an array is sized from them. The engine-side spawn factories
        // guarded their own arguments; those factories are gone, and this is the boundary the
        // numbers now enter through, so the guard belongs here. A distribution is fixable before
        // release -- refusing it by name is more useful than any runtime recovery.
        return new ScenarioConfig(
            RequiredString(scenario, "id", sourcePath, "scenario."),
            RequiredInt(scenario, "worldSeed", sourcePath, "scenario."),
            PositiveInt(scenario, "mapWidth", sourcePath, "scenario."),
            PositiveInt(scenario, "mapHeight", sourcePath, "scenario."),
            NonNegativeInt(scenario, "obstacleCount", sourcePath, "scenario."),
            RequiredInt(scenario, "obstacleSeed", sourcePath, "scenario."),
            RequiredInt(scenario, "factorySeed", sourcePath, "scenario."),
            RequiredInt(scenario, "itemFactorySeed", sourcePath, "scenario."),
            new ScenarioCounts(
                NonNegativeInt(counts, "pawns", sourcePath, "scenario.counts."),
                NonNegativeInt(counts, "food", sourcePath, "scenario.counts."),
                NonNegativeInt(counts, "water", sourcePath, "scenario.counts."),
                NonNegativeInt(counts, "beds", sourcePath, "scenario.counts."),
                NonNegativeInt(counts, "decorations", sourcePath, "scenario.counts.")));
    }

    /// <summary>
    /// The strict half. Case-SENSITIVE by design: the mod parser matches keys case-insensitively
    /// because it must tolerate hand-authored third-party files, but a distribution manifest ships
    /// with the product, so <c>RootMods</c> where <c>rootMods</c> was meant is a defect to name
    /// rather than a spelling to absorb.
    /// </summary>
    private static void RejectUnknownKeys(
        JsonElement obj, string sourcePath, string prefix, params string[] known)
    {
        foreach (JsonProperty property in obj.EnumerateObject())
        {
            bool recognised = false;
            for (int i = 0; i < known.Length; i++)
            {
                if (string.Equals(property.Name, known[i], StringComparison.Ordinal))
                {
                    recognised = true;
                    break;
                }
            }

            if (!recognised)
            {
                throw new InvalidOperationException(
                    $"Distribution manifest at '{sourcePath}' has unknown key " +
                    $"'{prefix}{property.Name}'. Known keys here: {string.Join(", ", known)}. " +
                    "Unlike a mod manifest, a distribution manifest rejects what it does not " +
                    "recognise, so a typo is a loud failure rather than a silently ignored field.");
            }
        }
    }

    private static JsonElement Require(JsonElement obj, string key, string sourcePath, string prefix)
        => obj.TryGetProperty(key, out JsonElement value)
            ? value
            : throw new InvalidOperationException(
                $"Distribution manifest at '{sourcePath}' is missing required field '{prefix}{key}'.");

    private static string RequiredString(JsonElement obj, string key, string sourcePath, string prefix)
    {
        JsonElement value = Require(obj, key, sourcePath, prefix);
        return value.ValueKind == JsonValueKind.String
            ? value.GetString()!
            : throw new InvalidOperationException(
                $"Distribution manifest field '{prefix}{key}' at '{sourcePath}' must be a string.");
    }

    private static int RequiredInt(JsonElement obj, string key, string sourcePath, string prefix)
    {
        JsonElement value = Require(obj, key, sourcePath, prefix);
        return value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int i)
            ? i
            : throw new InvalidOperationException(
                $"Distribution manifest field '{prefix}{key}' at '{sourcePath}' must be a 32-bit integer.");
    }

    /// <summary>A count: zero is legal, negative is not.</summary>
    private static int NonNegativeInt(JsonElement obj, string key, string sourcePath, string prefix)
    {
        int value = RequiredInt(obj, key, sourcePath, prefix);
        return value >= 0
            ? value
            : throw new InvalidOperationException(
                $"Distribution manifest field '{prefix}{key}' at '{sourcePath}' is {value}; " +
                "a count cannot be negative.");
    }

    /// <summary>An extent: zero leaves nowhere to stand, so it must be at least one.</summary>
    private static int PositiveInt(JsonElement obj, string key, string sourcePath, string prefix)
    {
        int value = RequiredInt(obj, key, sourcePath, prefix);
        return value >= 1
            ? value
            : throw new InvalidOperationException(
                $"Distribution manifest field '{prefix}{key}' at '{sourcePath}' is {value}; " +
                "a map extent must be at least one tile.");
    }

    private static JsonElement RequiredObject(JsonElement obj, string key, string sourcePath, string prefix)
    {
        JsonElement value = Require(obj, key, sourcePath, prefix);
        return value.ValueKind == JsonValueKind.Object
            ? value
            : throw new InvalidOperationException(
                $"Distribution manifest field '{prefix}{key}' at '{sourcePath}' must be an object.");
    }

    private static IReadOnlyList<string> RequiredStringArray(
        JsonElement obj, string key, string sourcePath, string prefix)
    {
        JsonElement value = Require(obj, key, sourcePath, prefix);
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException(
                $"Distribution manifest field '{prefix}{key}' at '{sourcePath}' must be an array of strings.");
        }

        var list = new List<string>();
        foreach (JsonElement element in value.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.String)
            {
                throw new InvalidOperationException(
                    $"Distribution manifest field '{prefix}{key}' at '{sourcePath}' must contain " +
                    "only strings.");
            }
            list.Add(element.GetString()!);
        }
        return list;
    }
}

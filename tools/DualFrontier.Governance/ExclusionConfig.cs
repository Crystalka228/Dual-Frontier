using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace DualFrontier.Governance;

/// <summary>
/// The path-exclusion set + extension allow-list, seeded from
/// <c>tools/governance/SCOPE_EXCLUSIONS.yaml</c>. Glob semantics mirror
/// sync_register.ps1 <c>Test-PathExcluded</c> (<c>**</c> → <c>.*</c>,
/// <c>*</c> → <c>[^/]*</c>, whole-string match) with literal characters escaped.
///
/// A governed-scope <c>.md</c> that is not excluded and carries no frontmatter is
/// a G-PATH ERROR (FRAMEWORK 14.2 -- the DF fail-closed hardening over the 1.x
/// warn-and-skip); this type provides the "excluded?" half of that decision.
/// </summary>
public sealed class ExclusionConfig
{
    private readonly IReadOnlyList<Regex> _excluded;

    /// <summary>The raw glob patterns (for diagnostics / reporting).</summary>
    public IReadOnlyList<string> ExcludedPatterns { get; }

    /// <summary>The enrollment extension allow-list (default: <c>.md</c> only).</summary>
    public IReadOnlyList<string> IncludedExtensions { get; }

    private ExclusionConfig(IReadOnlyList<string> patterns, IReadOnlyList<string> extensions)
    {
        ExcludedPatterns = patterns;
        IncludedExtensions = extensions;
        _excluded = patterns.Select(GlobToRegex).ToArray();
    }

    /// <summary>True when a project-relative POSIX path is excluded from enrollment.</summary>
    public bool IsExcluded(string relPathPosix) => _excluded.Any(rx => rx.IsMatch(relPathPosix));

    /// <summary>True when a path carries an allow-listed extension.</summary>
    public bool IsIncludedExtension(string path) =>
        IncludedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

    private static Regex GlobToRegex(string glob)
    {
        // Escape literals first, then re-open the glob wildcards. ** must be
        // rewritten before * so the single-star rule does not consume it.
        string pattern = Regex.Escape(glob)
            .Replace(@"\*\*", ".*")
            .Replace(@"\*", "[^/]*");
        return new Regex("^" + pattern + "$", RegexOptions.Compiled);
    }

    /// <summary>Loads the config from a YAML file; a missing file yields the safe default (.md only, nothing excluded).</summary>
    public static ExclusionConfig Load(string yamlPath)
    {
        if (!File.Exists(yamlPath))
        {
            return new ExclusionConfig(Array.Empty<string>(), new[] { ".md" });
        }

        // Ignore config-level metadata the exclusion loader does not model
        // (e.g. schema_version); the frontmatter reader stays strict (C4).
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
        var raw = deserializer.Deserialize<ExclusionsFile>(File.ReadAllText(yamlPath));

        var patterns = raw?.excluded_paths?
            .Select(e => e.pattern)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray() ?? Array.Empty<string>();

        var extensions = raw?.included_extensions?.ToArray() ?? new[] { ".md" };

        return new ExclusionConfig(patterns!, extensions);
    }

    // YamlDotNet DTOs for SCOPE_EXCLUSIONS.yaml (snake_case field names match the file).
    private sealed class ExclusionsFile
    {
        public List<ExcludedPath>? excluded_paths { get; set; }
        public List<string>? included_extensions { get; set; }
    }

    private sealed class ExcludedPath
    {
        public string pattern { get; set; } = string.Empty;
        public string? rationale { get; set; }
    }
}

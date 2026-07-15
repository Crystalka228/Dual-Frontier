namespace DualFrontier.Governance;

/// <summary>
/// Repo-root discovery and the canonical governance paths. <see cref="RepoRoot"/>
/// walks up from a start directory (default: the executing assembly) to the
/// directory holding <c>DualFrontier.sln</c> -- the CensusMetaTests.RepoRoot
/// pattern (tests/DualFrontier.Analyzers.Tests), reused so the tool and the test
/// suite agree on the root. Commands that operate on a scratch copy pass an
/// explicit root instead (the migrate --target case), so every path method takes
/// the root as an argument rather than resolving it internally.
/// </summary>
public static class RepoPaths
{
    /// <summary>The file whose presence marks the repository root.</summary>
    public const string SolutionMarker = "DualFrontier.sln";

    /// <summary>
    /// Walks up from <paramref name="start"/> (default: <see cref="AppContext.BaseDirectory"/>)
    /// to the directory containing <see cref="SolutionMarker"/>.
    /// </summary>
    public static string RepoRoot(string? start = null)
    {
        var dir = new DirectoryInfo(start ?? AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, SolutionMarker)))
        {
            dir = dir.Parent;
        }

        return dir?.FullName
            ?? throw new InvalidOperationException(
                $"repo root ({SolutionMarker}) not found from {start ?? AppContext.BaseDirectory}");
    }

    /// <summary>The <c>docs/governance/</c> directory under a root.</summary>
    public static string GovernanceDir(string repoRoot) => Path.Combine(repoRoot, "docs", "governance");

    /// <summary>The derived archive register (<c>docs/governance/REGISTER.yaml</c>).</summary>
    public static string RegisterPath(string repoRoot) => Path.Combine(GovernanceDir(repoRoot), "REGISTER.yaml");

    /// <summary>The derived boot subset (<c>docs/governance/CURRENT_AUTHORITY_SURFACE.yaml</c>).</summary>
    public static string AuthoritySurfacePath(string repoRoot) =>
        Path.Combine(GovernanceDir(repoRoot), "CURRENT_AUTHORITY_SURFACE.yaml");

    /// <summary>The exclusion config seed (<c>tools/governance/SCOPE_EXCLUSIONS.yaml</c>).</summary>
    public static string ExclusionsPath(string repoRoot) =>
        Path.Combine(repoRoot, "tools", "governance", "SCOPE_EXCLUSIONS.yaml");
}

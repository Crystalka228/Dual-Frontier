using System.Text.RegularExpressions;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Represents a dependency on another mod, with an optional version constraint
/// and an optional/required flag.
/// </summary>
public sealed record ModDependency(string ModId, VersionConstraint? Version, bool IsOptional)
{
    private static readonly Regex s_modIdPattern =
        new(@"^[a-z0-9]+(\.[a-z0-9]+)+$", RegexOptions.Compiled);

    /// <summary>Gets the reverse-domain mod identifier (e.g. <c>com.example.voidmagic</c>).</summary>
    public string ModId { get; init; } = ValidateModId(ModId);

    /// <summary>Gets the optional version constraint; <see langword="null"/> means any version is accepted.</summary>
    public VersionConstraint? Version { get; init; } = Version;

    /// <summary>Gets a value indicating whether this dependency is optional.</summary>
    public bool IsOptional { get; init; } = IsOptional;

    /// <summary>Creates a required dependency on <paramref name="modId"/>.</summary>
    public static ModDependency Required(string modId, VersionConstraint? version = null)
        => new(modId, version, false);

    /// <summary>Creates an optional dependency on <paramref name="modId"/>.</summary>
    public static ModDependency Optional(string modId, VersionConstraint? version = null)
        => new(modId, version, true);

    /// <summary>
    /// Returns <see langword="true"/> when the described available mod satisfies this dependency.
    /// </summary>
    public bool IsSatisfiedBy(string availableModId, ContractsVersion? availableVersion)
    {
        if (availableModId != ModId) return false;
        if (Version is null) return true;
        if (availableVersion is null) return false;
        return Version.Value.IsSatisfiedBy(availableVersion.Value);
    }

    /// <summary>Returns a human-readable representation of this dependency.</summary>
    public override string ToString()
    {
        var v = Version.HasValue ? $"@{Version.Value}" : "";
        var o = IsOptional ? " (optional)" : "";
        return $"{ModId}{v}{o}";
    }

    private static string ValidateModId(string modId)
    {
        if (string.IsNullOrWhiteSpace(modId))
            throw new ArgumentException("ModDependency.ModId must not be empty.", "ModId");
        if (!s_modIdPattern.IsMatch(modId))
            throw new ArgumentException(
                "ModDependency.ModId must be in reverse-domain format.", "ModId");
        return modId;
    }
}

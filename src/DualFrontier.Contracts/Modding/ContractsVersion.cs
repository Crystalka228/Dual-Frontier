using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Immutable semantic version of the <c>DualFrontier.Contracts</c> assembly.
/// Mods declare the minimum version they require in
/// <see cref="ModManifest.RequiresContractsVersion"/>; the loader compares
/// against <see cref="Current"/> and rejects incompatible mods.
/// </summary>
public readonly struct ContractsVersion : IEquatable<ContractsVersion>
{
    /// <summary>
    /// Version currently exported by this build of the contracts assembly.
    /// Bumped manually whenever a breaking change ships.
    /// </summary>
    public static readonly ContractsVersion Current = new(1, 0, 0);

    /// <summary>
    /// Major component: bumped on breaking changes.
    /// </summary>
    public int Major { get; }

    /// <summary>
    /// Minor component: bumped on backward-compatible additions.
    /// </summary>
    public int Minor { get; }

    /// <summary>
    /// Patch component: bumped on bug fixes that do not touch API surface.
    /// </summary>
    public int Patch { get; }

    /// <summary>
    /// Creates a version with the given components. All components must be
    /// non-negative; otherwise <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public ContractsVersion(int major, int minor, int patch)
    {
        if (major < 0) throw new ArgumentOutOfRangeException(nameof(major));
        if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor));
        if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch));
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    /// <summary>
    /// Parses a <c>MAJOR.MINOR.PATCH</c> string. Throws
    /// <see cref="FormatException"/> for any other format. Caret/tilde
    /// prefixes are not supported — comparison is strict version-against-version.
    /// </summary>
    public static ContractsVersion Parse(string text)
    {
        if (text is null)
            throw new ArgumentNullException(nameof(text));

        string trimmed = text.Trim();
        // Caret/tilde префиксы не поддерживаются — сравнение строгое.
        if (trimmed.Length > 0 && (trimmed[0] == '^' || trimmed[0] == '~'))
            trimmed = trimmed.Substring(1);

        string[] parts = trimmed.Split('.');
        if (parts.Length != 3)
            throw new FormatException(
                $"Invalid ContractsVersion '{text}'. Expected MAJOR.MINOR.PATCH.");

        if (!int.TryParse(parts[0], out int major) ||
            !int.TryParse(parts[1], out int minor) ||
            !int.TryParse(parts[2], out int patch))
        {
            throw new FormatException(
                $"Invalid ContractsVersion '{text}'. Components must be integers.");
        }

        return new ContractsVersion(major, minor, patch);
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="required"/> is less than or
    /// equal to <paramref name="available"/>. Major version asymmetry means
    /// the mod is incompatible even if minor/patch would permit.
    /// </summary>
    public static bool IsCompatible(ContractsVersion required, ContractsVersion available)
    {
        // Мажорная версия должна совпадать — иначе контракты несовместимы.
        if (required.Major != available.Major)
            return false;
        if (required.Minor > available.Minor)
            return false;
        if (required.Minor == available.Minor && required.Patch > available.Patch)
            return false;
        return true;
    }

    /// <inheritdoc />
    public bool Equals(ContractsVersion other)
        => Major == other.Major && Minor == other.Minor && Patch == other.Patch;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ContractsVersion v && Equals(v);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch);

    /// <inheritdoc />
    public override string ToString() => $"{Major}.{Minor}.{Patch}";

    /// <summary>Structural equality.</summary>
    public static bool operator ==(ContractsVersion left, ContractsVersion right) => left.Equals(right);

    /// <summary>Structural inequality.</summary>
    public static bool operator !=(ContractsVersion left, ContractsVersion right) => !left.Equals(right);
}

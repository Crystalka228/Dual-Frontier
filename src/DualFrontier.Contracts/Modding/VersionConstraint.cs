using System;
using System.Globalization;

namespace DualFrontier.Contracts.Modding;

/// <summary>Discriminates the constraint operator in a <see cref="VersionConstraint"/>.</summary>
public enum VersionConstraintKind
{
    /// <summary>Requires exactly the given version.</summary>
    Exact,

    /// <summary>Requires the same Major; Minor.Patch must be ≥ the constraint's floor.</summary>
    Caret,
}

/// <summary>
/// Typed version constraint used in Manifest v2 to express compatibility requirements.
/// Replaces the raw <c>string RequiresContractsVersion</c> field and the caret-stripping
/// workaround in <see cref="ContractsVersion.Parse"/>.
/// </summary>
public readonly struct VersionConstraint : IEquatable<VersionConstraint>
{
    private const string CaretPrefix = "^";

    private readonly VersionConstraintKind _kind;
    private readonly ContractsVersion _floor;

    private VersionConstraint(VersionConstraintKind kind, ContractsVersion floor)
    {
        _kind = kind;
        _floor = floor;
    }

    /// <summary>The constraint operator: <see cref="VersionConstraintKind.Exact"/> or <see cref="VersionConstraintKind.Caret"/>.</summary>
    public VersionConstraintKind Kind => _kind;

    /// <summary>The minimum (floor) version encoded in the constraint string.</summary>
    public ContractsVersion Floor => _floor;

    /// <summary>
    /// Parses a constraint string of the form <c>[^]MAJOR.MINOR.PATCH</c>.
    /// Leading/trailing whitespace is stripped; whitespace after <c>^</c> is not permitted.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
    /// <exception cref="FormatException">
    /// The string is empty, has a wrong component count, contains non-integer or negative
    /// components, or has whitespace between the <c>^</c> prefix and the version number.
    /// </exception>
    public static VersionConstraint Parse(string text)
    {
        if (text is null)
            throw new ArgumentNullException(nameof(text));

        string trimmed = text.Trim();

        if (trimmed.StartsWith("~", StringComparison.Ordinal))
        {
            throw new FormatException(
                $"Tilde version constraints (e.g. '{text}') are not supported. " +
                "Use caret syntax instead: '^X.Y.Z' pins major and allows higher minor/patch.");
        }

        VersionConstraintKind kind;
        string versionText;

        if (trimmed.StartsWith(CaretPrefix, StringComparison.Ordinal))
        {
            kind = VersionConstraintKind.Caret;
            versionText = trimmed.Substring(CaretPrefix.Length);
        }
        else
        {
            kind = VersionConstraintKind.Exact;
            versionText = trimmed;
        }

        string[] parts = versionText.Split('.');
        if (parts.Length != 3)
            throw new FormatException(
                $"Invalid VersionConstraint '{text}'. Expected [^]MAJOR.MINOR.PATCH.");

        if (!int.TryParse(parts[0], NumberStyles.None, null, out int major) ||
            !int.TryParse(parts[1], NumberStyles.None, null, out int minor) ||
            !int.TryParse(parts[2], NumberStyles.None, null, out int patch))
        {
            throw new FormatException(
                $"Invalid VersionConstraint '{text}'. Components must be non-negative integers.");
        }

        return new VersionConstraint(kind, new ContractsVersion(major, minor, patch));
    }

    /// <summary>
    /// Returns <c>true</c> when <paramref name="available"/> satisfies this constraint.
    /// <list type="bullet">
    ///   <item><see cref="VersionConstraintKind.Exact"/>: <paramref name="available"/> must equal <see cref="Floor"/>.</item>
    ///   <item><see cref="VersionConstraintKind.Caret"/>: same Major; Minor.Patch must be ≥ <see cref="Floor"/>.</item>
    /// </list>
    /// </summary>
    public bool IsSatisfiedBy(ContractsVersion available)
    {
        if (_kind == VersionConstraintKind.Exact)
            return available == _floor;

        if (available.Major != _floor.Major)
            return false;
        if (available.Minor > _floor.Minor)
            return true;
        return available.Minor == _floor.Minor && available.Patch >= _floor.Patch;
    }

    /// <inheritdoc/>
    public bool Equals(VersionConstraint other) => _kind == other._kind && _floor == other._floor;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VersionConstraint v && Equals(v);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_kind, _floor);

    /// <summary>Returns the constraint in its canonical string form: <c>MAJOR.MINOR.PATCH</c> or <c>^MAJOR.MINOR.PATCH</c>.</summary>
    public override string ToString() =>
        _kind == VersionConstraintKind.Caret
            ? $"{CaretPrefix}{_floor}"
            : _floor.ToString();

    /// <summary>Structural equality.</summary>
    public static bool operator ==(VersionConstraint left, VersionConstraint right) => left.Equals(right);

    /// <summary>Structural inequality.</summary>
    public static bool operator !=(VersionConstraint left, VersionConstraint right) => !left.Equals(right);
}

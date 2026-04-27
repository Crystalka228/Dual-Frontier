using System.Text.RegularExpressions;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Validated, deduplicated set of capability tokens declared by a mod.
/// Capabilities tell the loader which subsystems a mod intends to use
/// (e.g. <c>ecs.systems</c>, <c>contracts.publish</c>).
/// </summary>
public readonly record struct ManifestCapabilities
{
    private static readonly Regex s_tokenPattern =
        new(@"^[a-z][a-z0-9]*(-[a-z0-9]+)*(\.[a-z][a-z0-9]*(-[a-z0-9]+)*)+$",
            RegexOptions.Compiled);

    private static readonly HashSet<string> s_emptySet = [];

    private static readonly ManifestCapabilities _empty = new(new HashSet<string>());

    private readonly HashSet<string>? _tokens;

    private ManifestCapabilities(HashSet<string> tokens) => _tokens = tokens;

    /// <summary>Gets the validated, deduplicated set of capability tokens. Sorted for deterministic output.</summary>
    public IReadOnlySet<string> Tokens => _tokens ?? s_emptySet;

    /// <summary>Gets a value indicating whether no capabilities are declared.</summary>
    public bool IsEmpty => _tokens is null || _tokens.Count == 0;

    /// <summary>Gets the canonical empty instance.</summary>
    public static ManifestCapabilities Empty => _empty;

    /// <summary>
    /// Parses and validates a collection of capability tokens.
    /// Each token is trimmed; duplicates are silently dropped.
    /// </summary>
    /// <param name="tokens">Raw token strings. <see langword="null"/> is treated as empty.</param>
    /// <returns>A validated <see cref="ManifestCapabilities"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when any token does not conform to the capability format
    /// <c>segment('.'segment)+</c> where <c>segment = [a-z][a-z0-9]*(-[a-z0-9]+)*</c>.
    /// </exception>
    public static ManifestCapabilities Parse(IEnumerable<string>? tokens)
    {
        if (tokens is null)
            return Empty;

        var set = new HashSet<string>();
        foreach (var token in tokens)
        {
            var trimmed = token?.Trim() ?? string.Empty;
            if (!s_tokenPattern.IsMatch(trimmed))
                throw new ArgumentException(
                    $"Invalid capability token: '{trimmed}'", nameof(tokens));
            set.Add(trimmed);
        }

        return set.Count == 0 ? Empty : new ManifestCapabilities(set);
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="capability"/> is present in the set.
    /// Returns <see langword="false"/> for <see langword="null"/>, empty, or whitespace without throwing.
    /// </summary>
    /// <param name="capability">The token to look up.</param>
    public bool Contains(string? capability)
    {
        if (string.IsNullOrWhiteSpace(capability))
            return false;
        return _tokens?.Contains(capability) ?? false;
    }

    /// <summary>
    /// Returns <c>"(none)"</c> when empty; otherwise a sorted, comma-separated list of tokens.
    /// </summary>
    public override string ToString()
    {
        if (IsEmpty)
            return "(none)";
        return string.Join(", ", _tokens!.Order());
    }

    /// <summary>Returns <see langword="true"/> when both instances contain the same tokens, regardless of insertion order.</summary>
    public bool Equals(ManifestCapabilities other)
    {
        var a = _tokens ?? s_emptySet;
        var b = other._tokens ?? s_emptySet;
        return a.Count == b.Count && a.SetEquals(b);
    }

    /// <summary>Returns an order-independent hash code based on the token set contents.</summary>
    public override int GetHashCode()
    {
        if (_tokens is null || _tokens.Count == 0)
            return 0;
        var hash = 0;
        foreach (var token in _tokens)
            hash ^= token.GetHashCode();
        return hash;
    }
}

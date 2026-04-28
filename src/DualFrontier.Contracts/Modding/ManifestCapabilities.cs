using System.Text.RegularExpressions;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Validated, deduplicated pair of capability-token sets declared by a mod manifest.
/// Capabilities follow the format <c>&lt;provider&gt;.&lt;verb&gt;:&lt;FullyQualifiedTypeName&gt;</c>
/// as defined in MOD_OS_ARCHITECTURE §3.2.
/// </summary>
public readonly record struct ManifestCapabilities
{
    // --- static readonly ---

    /// <summary>Authoritative capability-string pattern from MOD_OS_ARCHITECTURE §2.3.</summary>
    private static readonly Regex s_capabilityPattern =
        new(@"^(kernel|mod\.[a-z0-9.]+)\.(publish|subscribe|read|write):[A-Za-z][A-Za-z0-9_.]+$",
            RegexOptions.Compiled);

    private static readonly HashSet<string> s_emptySet = [];

    private static readonly ManifestCapabilities _empty =
        new(new HashSet<string>(), new HashSet<string>());

    // --- private fields ---

    private readonly HashSet<string>? _required;
    private readonly HashSet<string>? _provided;

    // --- constructor ---

    private ManifestCapabilities(HashSet<string> required, HashSet<string> provided)
    {
        _required = required;
        _provided = provided;
    }

    // --- public properties ---

    /// <summary>Gets the capabilities the mod requires from the kernel or its dependencies.</summary>
    public IReadOnlySet<string> Required => _required ?? s_emptySet;

    /// <summary>Gets the capabilities the mod itself exposes to other mods.</summary>
    public IReadOnlySet<string> Provided => _provided ?? s_emptySet;

    /// <summary>Gets a value indicating whether both <see cref="Required"/> and <see cref="Provided"/> are empty.</summary>
    public bool IsEmpty =>
        (_required is null || _required.Count == 0) &&
        (_provided is null || _provided.Count == 0);

    // --- public static members ---

    /// <summary>Gets the canonical empty instance with no required or provided capabilities.</summary>
    public static ManifestCapabilities Empty => _empty;

    /// <summary>
    /// Parses and validates two collections of capability tokens into a <see cref="ManifestCapabilities"/> instance.
    /// Each token is trimmed before validation; duplicates within each set are silently dropped.
    /// </summary>
    /// <param name="required">
    /// Capabilities the mod needs from the kernel or its dependencies.
    /// <see langword="null"/> is treated as an empty set.
    /// </param>
    /// <param name="provided">
    /// Capabilities the mod exports for other mods.
    /// <see langword="null"/> is treated as an empty set.
    /// </param>
    /// <returns>A validated <see cref="ManifestCapabilities"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when any token in either set does not conform to the authoritative pattern
    /// <c>^(kernel|mod\.[a-z0-9.]+)\.(publish|subscribe|read|write):[A-Za-z][A-Za-z0-9_.]+$</c>.
    /// The offending token is included in the exception message.
    /// </exception>
    public static ManifestCapabilities Parse(
        IEnumerable<string>? required,
        IEnumerable<string>? provided)
    {
        var req = ParseSet(required);
        var prov = ParseSet(provided);

        if (req.Count == 0 && prov.Count == 0)
            return Empty;

        return new ManifestCapabilities(req, prov);
    }

    // --- public methods ---

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="capability"/> is present in the <see cref="Required"/> set.
    /// Returns <see langword="false"/> for <see langword="null"/>, empty, or whitespace without throwing.
    /// </summary>
    /// <param name="capability">The capability token to look up.</param>
    public bool RequiresCapability(string? capability)
    {
        if (string.IsNullOrWhiteSpace(capability))
            return false;
        return _required?.Contains(capability) ?? false;
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="capability"/> is present in the <see cref="Provided"/> set.
    /// Returns <see langword="false"/> for <see langword="null"/>, empty, or whitespace without throwing.
    /// </summary>
    /// <param name="capability">The capability token to look up.</param>
    public bool ProvidesCapability(string? capability)
    {
        if (string.IsNullOrWhiteSpace(capability))
            return false;
        return _provided?.Contains(capability) ?? false;
    }

    /// <summary>
    /// Returns <c>"(none)"</c> when empty; otherwise a sorted representation of required
    /// and provided tokens, each side prefixed with its label and separated by <c>"; "</c>.
    /// Empty sides are omitted entirely.
    /// </summary>
    public override string ToString()
    {
        if (IsEmpty)
            return "(none)";

        var parts = new List<string>(2);

        if (_required is { Count: > 0 })
            parts.Add("required: " + string.Join(", ", _required.Order()));

        if (_provided is { Count: > 0 })
            parts.Add("provided: " + string.Join(", ", _provided.Order()));

        return string.Join("; ", parts);
    }

    /// <summary>Returns <see langword="true"/> when both instances contain the same required and provided token sets, regardless of insertion order.</summary>
    public bool Equals(ManifestCapabilities other)
    {
        var req = _required ?? s_emptySet;
        var otherReq = other._required ?? s_emptySet;

        if (req.Count != otherReq.Count || !req.SetEquals(otherReq))
            return false;

        var prov = _provided ?? s_emptySet;
        var otherProv = other._provided ?? s_emptySet;
        return prov.Count == otherProv.Count && prov.SetEquals(otherProv);
    }

    /// <summary>Returns an order-independent hash code based on the contents of both sets.</summary>
    public override int GetHashCode()
    {
        var reqHash = 0;
        if (_required is not null)
            foreach (var token in _required)
                reqHash ^= token.GetHashCode();

        var provHash = 0;
        if (_provided is not null)
            foreach (var token in _provided)
                provHash ^= token.GetHashCode();

        return HashCode.Combine(reqHash, provHash);
    }

    // --- private methods ---

    private static HashSet<string> ParseSet(IEnumerable<string>? tokens)
    {
        if (tokens is null)
            return [];

        var set = new HashSet<string>();
        foreach (var token in tokens)
        {
            var trimmed = token?.Trim() ?? string.Empty;
            if (!s_capabilityPattern.IsMatch(trimmed))
                throw new ArgumentException(
                    $"Invalid capability token: '{trimmed}'", nameof(tokens));
            set.Add(trimmed);
        }
        return set;
    }
}

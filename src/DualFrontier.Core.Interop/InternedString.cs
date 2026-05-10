using System;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed handle over a string interned in <c>dualfrontier::StringPool</c>.
///
/// <para>
/// <b>Same-pool equality (fast path).</b> <c>==</c>,
/// <see cref="Equals(InternedString)"/>, and <see cref="GetHashCode"/>
/// compare by <c>(Id, Generation)</c>. Within a single
/// <see cref="NativeWorld"/>, equal pairs always refer to identical content
/// and unequal pairs to distinct content; comparison is a single 64-bit
/// equality test.
/// </para>
///
/// <para>
/// <b>Cross-pool equality.</b> If two <see cref="InternedString"/> values
/// were issued by <i>different</i> <see cref="NativeWorld"/> instances,
/// <c>==</c> may return <c>true</c> for unrelated content (false positive,
/// when the two pools happen to issue the same id for different strings)
/// or <c>false</c> for identical content (false negative, when the same
/// content received different ids in each pool). Use
/// <see cref="EqualsByContent"/> for cross-pool comparisons; it resolves
/// both sides and compares string content.
/// </para>
///
/// <para>
/// <b>Solution A applicability.</b> Per K-L11 (production storage backbone),
/// production code uses one <see cref="NativeWorld"/> and the same-pool
/// fast path is sufficient. Multi-world scenarios are limited to test
/// fixtures, research code, and any future relaxation of K-L11; the
/// cross-pool method exists for those scenarios and for future-proofing
/// the API surface.
/// </para>
///
/// <para>
/// <b>Stale ids.</b> Resolution may return <c>null</c> if the generation
/// tag is stale (the id was reclaimed during a
/// <see cref="NativeWorld.ClearModScope(string)"/> and possibly reissued
/// for different content) or if the id was issued by a world other than
/// the one supplied to <see cref="Resolve"/>. Callers that hold stale ids
/// should re-intern from the original content rather than guess at
/// recovery.
/// </para>
///
/// <para>
/// <b>Save/load (LOCKED, per K8.1 brief §1.2).</b> Components serialise
/// resolved string content, not the <c>(Id, Generation)</c> pair. On
/// reload the content is re-interned and a fresh pair is issued. The
/// generation tag is the safety net for any path that did persist an id
/// (in-memory snapshots, diagnostic dumps); it is not the primary
/// persistence mechanism. Cross-snapshot reconciliation that needs to
/// compare interned values uses <see cref="EqualsByContent"/>, since
/// snapshots may have been written by different worlds.
/// </para>
/// </summary>
public readonly struct InternedString : IEquatable<InternedString>, IComparable<InternedString>
{
    /// <summary>The empty/uninitialized sentinel. Equivalent to <c>default(InternedString)</c>; Id = 0, Generation = 0.</summary>
    public static readonly InternedString Empty = default;

    /// <summary>The native pool's id for this string. 0 = empty sentinel.</summary>
    public uint Id { get; }

    /// <summary>The generation tag at the moment this id was issued.</summary>
    public uint Generation { get; }

    internal InternedString(uint id, uint generation)
    {
        Id = id;
        Generation = generation;
    }

    /// <summary>True if this is the empty/uninitialized sentinel.</summary>
    public bool IsEmpty => Id == 0;

    /// <summary>
    /// Ordinal handle comparison on (Id, Generation) — required by
    /// <see cref="NativeMap{TKey,TValue}"/> and <see cref="NativeSet{T}"/>
    /// where <c>TKey</c>/<c>T</c> must implement <see cref="IComparable{T}"/>.
    /// This is handle equality, not content equality: two interned strings
    /// with identical content but distinct generations compare unequal.
    /// For content-equal comparison across pools or across reclaim cycles,
    /// use <see cref="EqualsByContent"/> at the call site and canonicalize
    /// keys via a single <see cref="NativeWorld.InternString"/> at insertion.
    /// </summary>
    public int CompareTo(InternedString other)
    {
        int idCompare = Id.CompareTo(other.Id);
        if (idCompare != 0) return idCompare;
        return Generation.CompareTo(other.Generation);
    }

    /// <summary>
    /// Resolves to the underlying string content via the supplied
    /// <see cref="NativeWorld"/>. Returns <c>null</c> if the id is the
    /// empty sentinel, the generation tag is stale, or the world is
    /// not the one that issued the id.
    /// </summary>
    public string? Resolve(NativeWorld world)
    {
        if (world is null) throw new ArgumentNullException(nameof(world));
        if (IsEmpty) return null;
        return world.ResolveInternedString(this);
    }

    /// <summary>
    /// Compares this <see cref="InternedString"/> with <paramref name="other"/>
    /// by resolved content rather than by <c>(Id, Generation)</c>. Use this for
    /// cross-pool comparisons where the two values were issued by different
    /// <see cref="NativeWorld"/> instances; <c>==</c> and
    /// <see cref="Equals(InternedString)"/> compare only the id pair and may
    /// return false positives or false negatives across pools.
    ///
    /// Empty values compare equal to each other regardless of world. If both
    /// sides resolve successfully, content is compared via ordinal string
    /// equality. If either resolution returns <c>null</c> (stale generation,
    /// wrong world), the method returns <c>false</c>.
    ///
    /// Fast path: when <paramref name="thisWorld"/> and
    /// <paramref name="otherWorld"/> are the same instance and the
    /// <c>(Id, Generation)</c> pairs are equal, returns <c>true</c> without
    /// resolving. Same-pool callers pay no resolution cost on equal ids.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either world is <c>null</c>. Cross-pool semantics requires
    /// both worlds to be supplied — the method cannot infer issuer.
    /// </exception>
    public bool EqualsByContent(InternedString other, NativeWorld thisWorld, NativeWorld otherWorld)
    {
        if (thisWorld is null) throw new ArgumentNullException(nameof(thisWorld));
        if (otherWorld is null) throw new ArgumentNullException(nameof(otherWorld));

        if (IsEmpty && other.IsEmpty) return true;
        if (IsEmpty || other.IsEmpty) return false;

        if (ReferenceEquals(thisWorld, otherWorld) && Equals(other))
        {
            return true;
        }

        string? thisContent = Resolve(thisWorld);
        string? otherContent = other.Resolve(otherWorld);
        if (thisContent is null || otherContent is null) return false;
        return string.Equals(thisContent, otherContent, StringComparison.Ordinal);
    }

    public bool Equals(InternedString other) =>
        Id == other.Id && Generation == other.Generation;

    public override bool Equals(object? obj) =>
        obj is InternedString other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Generation);

    public static bool operator ==(InternedString a, InternedString b) => a.Equals(b);
    public static bool operator !=(InternedString a, InternedString b) => !a.Equals(b);

    public override string ToString() =>
        IsEmpty ? "InternedString(empty)" : $"InternedString(Id={Id}, Gen={Generation})";
}

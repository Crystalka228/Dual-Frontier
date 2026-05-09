using System;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed handle over a string interned in <c>dualfrontier::StringPool</c>.
///
/// Equality is by (Id, Generation) — both halves must match for two
/// <see cref="InternedString"/> values to compare equal. This is the
/// fast path: same-pool comparisons are a single 64-bit equality test.
///
/// To recover the underlying content, call <see cref="Resolve"/> with
/// the <see cref="NativeWorld"/> the id was issued by. Resolution may
/// return <c>null</c> if the generation tag is stale (the id was
/// reclaimed during a <see cref="NativeWorld.ClearModScope(string)"/>
/// and possibly reissued for different content). Callers that hold
/// stale IDs should re-intern from the original content rather than
/// guess at recovery.
///
/// Save/load (LOCKED, per K8.1 brief §1.2): components serialise the
/// resolved string, not the (Id, Generation) pair. On reload the
/// content is re-interned and a fresh pair is issued. The generation
/// tag is the safety net for any path that did persist an id (in-memory
/// snapshots, diagnostic dumps).
/// </summary>
public readonly struct InternedString : IEquatable<InternedString>
{
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

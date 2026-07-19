using System;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// Contracts-safe handle over a string interned in the engine string pool,
/// obtained from <see cref="ISystemContext.InternString"/> and resolved via
/// <see cref="ISystemContext.Resolve"/>. The promotion of the engine-internal
/// interned-string value: a pure <c>(Id, Generation)</c> pair, so it is
/// <c>unmanaged</c> and may be carried as a component field.
///
/// <para>
/// <b>Same-pool equality.</b> Within one world, equal pairs refer to identical
/// content; equality is a single 64-bit compare. Cross-pool comparison is not
/// meaningful on the handle alone (production runs one world — К-L11).
/// </para>
///
/// <para>
/// <b>Forge discipline.</b> The constructor is <c>internal</c>; only the engine
/// issues a valid handle. <c>default</c> is the empty sentinel.
/// </para>
/// </summary>
public readonly struct StringHandle : IEquatable<StringHandle>
{
    /// <summary>The empty/uninitialized sentinel (Id = 0). Equal to <c>default</c>.</summary>
    public static readonly StringHandle Empty = default;

    /// <summary>The engine pool id for this string. 0 = empty sentinel.</summary>
    public uint Id { get; }

    /// <summary>The generation tag at the moment this id was issued.</summary>
    public uint Generation { get; }

    internal StringHandle(uint id, uint generation)
    {
        Id = id;
        Generation = generation;
    }

    /// <summary>True if this is the empty sentinel.</summary>
    public bool IsEmpty => Id == 0;

    public bool Equals(StringHandle other) => Id == other.Id && Generation == other.Generation;

    public override bool Equals(object? obj) => obj is StringHandle other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Generation);

    public static bool operator ==(StringHandle a, StringHandle b) => a.Equals(b);

    public static bool operator !=(StringHandle a, StringHandle b) => !a.Equals(b);

    public override string ToString()
        => IsEmpty ? "StringHandle(empty)" : $"StringHandle(Id={Id}, Gen={Generation})";
}

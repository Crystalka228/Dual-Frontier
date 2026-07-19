using System;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// Contracts-safe handle over a per-entity variable-length list (a "composite")
/// owned by the engine world. Obtained from
/// <see cref="ISystemContext.CreateComposite{T}"/> and operated through the
/// context's composite methods. The promotion of the engine-internal composite
/// value: a pure <c>CompositeId</c>, so it is <c>unmanaged</c> and may be
/// carried as a component field (the reason the engine composite was itself made
/// a value type — К8.2 v2, preserving К-L3 «unmanaged»).
///
/// <para>
/// <b>Forge discipline.</b> The constructor is <c>internal</c>; only the engine
/// issues a valid handle. <c>default</c> is the invalid sentinel
/// (<see cref="IsValid"/> is <c>false</c>).
/// </para>
/// </summary>
/// <typeparam name="T">The unmanaged element type.</typeparam>
public readonly struct CompositeHandle<T> : IEquatable<CompositeHandle<T>> where T : unmanaged
{
    /// <summary>The engine composite id. 0 = invalid sentinel.</summary>
    public uint CompositeId { get; }

    internal CompositeHandle(uint compositeId) => CompositeId = compositeId;

    /// <summary>True when this handle refers to a real composite (non-default).</summary>
    public bool IsValid => CompositeId != 0;

    public bool Equals(CompositeHandle<T> other) => CompositeId == other.CompositeId;

    public override bool Equals(object? obj) => obj is CompositeHandle<T> other && Equals(other);

    public override int GetHashCode() => CompositeId.GetHashCode();

    public static bool operator ==(CompositeHandle<T> a, CompositeHandle<T> b) => a.Equals(b);

    public static bool operator !=(CompositeHandle<T> a, CompositeHandle<T> b) => !a.Equals(b);

    public override string ToString()
        => IsValid ? $"CompositeHandle<{typeof(T).Name}>(Id={CompositeId})" : $"CompositeHandle<{typeof(T).Name}>(invalid)";
}

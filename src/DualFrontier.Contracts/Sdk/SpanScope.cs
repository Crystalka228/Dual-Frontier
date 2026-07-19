using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// Scoped, read-only view over a component storage span, obtained from
/// <see cref="ISystemContext.AcquireSpan{T}"/>. The Contracts-safe promotion of
/// the engine-internal read lease: it exposes the dense component data and
/// (entity, component) pairs without naming any <c>Core.Interop</c> type.
///
/// <para>
/// <b>Lifetime — preserved exactly (no weakening).</b> Acquire in a
/// <c>using</c> scope; <see cref="Dispose"/> releases the native span-lock so
/// mutation is permitted again. While ANY span is live on the world, mutations
/// are rejected native-side — identical to the <c>SystemBase</c> contract this
/// promotes. A <see langword="ref"/> <see langword="struct"/>: it cannot escape
/// to the heap, be boxed, or be stored in a field, so it cannot outlive its scope.
/// </para>
///
/// <para>
/// <b>Allocation.</b> Stack-only; construction captures the engine lease as an
/// opaque <see cref="object"/> and the spans by value — zero per-tick
/// allocation over the <c>SystemBase</c> path (CONCURRENCY_AND_MEMORY_MODEL;
/// KERNEL_ARCHITECTURE §2, К-L3.1 Path α).
/// </para>
///
/// <para>
/// <b>Forge discipline.</b> The constructor is <c>internal</c>; only the engine
/// (via <c>InternalsVisibleTo</c>) can produce a valid instance. A mod cannot
/// fabricate one.
/// </para>
/// </summary>
/// <typeparam name="T">The unmanaged component type.</typeparam>
public readonly ref struct SpanScope<T> where T : unmanaged
{
    // The real Core.Interop lease, held unnamed as an IDisposable-capable object.
    private readonly object? _lease;
    private readonly ReadOnlySpan<T> _components;
    private readonly ReadOnlySpan<int> _indices;

    internal SpanScope(object lease, ReadOnlySpan<T> components, ReadOnlySpan<int> indices)
    {
        _lease = lease;
        _components = components;
        _indices = indices;
    }

    /// <summary>Number of components in the span.</summary>
    public int Count => _components.Length;

    /// <summary>
    /// Read-only span over the dense component data. Valid for the lifetime of
    /// this scope; do not retain the returned span after <see cref="Dispose"/>.
    /// </summary>
    public ReadOnlySpan<T> Components => _components;

    /// <summary>
    /// Iterate <c>(EntityId, T)</c> pairs over the span.
    ///
    /// Caveat (preserved from the engine lease, K7-deferred): the
    /// <see cref="EntityId"/> is reconstructed with <c>Version = 1</c> because
    /// the span ABI does not carry per-entity versions. Safe for fresh entities
    /// and snapshot-then-record flows (the recorded command is version-checked
    /// at flush, so a stale id is rejected).
    /// </summary>
    public PairsEnumerable Pairs => new PairsEnumerable(_components, _indices);

    /// <summary>Releases the underlying span-lock. Idempotent.</summary>
    public void Dispose() => (_lease as IDisposable)?.Dispose();

    /// <summary>Allocation-free enumerable over (entity, component) pairs.</summary>
    public readonly ref struct PairsEnumerable
    {
        private readonly ReadOnlySpan<T> _components;
        private readonly ReadOnlySpan<int> _indices;

        internal PairsEnumerable(ReadOnlySpan<T> components, ReadOnlySpan<int> indices)
        {
            _components = components;
            _indices = indices;
        }

        public PairsEnumerator GetEnumerator() => new PairsEnumerator(_components, _indices);
    }

    /// <summary>Allocation-free enumerator over (entity, component) pairs.</summary>
    public ref struct PairsEnumerator
    {
        private readonly ReadOnlySpan<T> _components;
        private readonly ReadOnlySpan<int> _indices;
        private int _index;

        internal PairsEnumerator(ReadOnlySpan<T> components, ReadOnlySpan<int> indices)
        {
            _components = components;
            _indices = indices;
            _index = -1;
        }

        public bool MoveNext() => ++_index < _components.Length;

        public (EntityId Entity, T Component) Current
            => (new EntityId(_indices[_index], 1), _components[_index]);
    }
}

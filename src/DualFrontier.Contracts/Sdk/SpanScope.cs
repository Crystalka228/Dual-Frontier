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
    private readonly ReadOnlySpan<int> _versions;

    internal SpanScope(object lease, ReadOnlySpan<T> components, ReadOnlySpan<int> indices,
                       ReadOnlySpan<int> versions)
    {
        _lease = lease;
        _components = components;
        _indices = indices;
        _versions = versions;
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
    /// The <see cref="EntityId"/> carries the entity's TRUE version, read from the
    /// world's own per-slot version table through the engine lease's versions view
    /// (ID-B / К-L22; IDENTITY_AND_ABI_CONTRACT §2, the version-0 resolution —
    /// finding F-59). An id this enumerator yields is therefore a valid write key
    /// even when its index has been recycled: <c>BeginBatch</c> writes keyed on it
    /// pass the flush-time version check, which is the mod-facing loop this SDK
    /// exists to make work.
    ///
    /// <para>
    /// Earlier revisions fabricated the version here — <c>1</c> until W3, then
    /// <c>0</c> — because the span ABI carried entity indices only. Version 1
    /// matched no entity at all, so batched writes vanished silently; version 0
    /// matched a never-recycled slot and nothing else. Neither is fabricated any
    /// more: managed code no longer invents a version it did not receive from the
    /// world.
    /// </para>
    /// </summary>
    public PairsEnumerable Pairs => new PairsEnumerable(_components, _indices, _versions);

    /// <summary>Releases the underlying span-lock. Idempotent.</summary>
    public void Dispose() => (_lease as IDisposable)?.Dispose();

    /// <summary>Allocation-free enumerable over (entity, component) pairs.</summary>
    public readonly ref struct PairsEnumerable
    {
        private readonly ReadOnlySpan<T> _components;
        private readonly ReadOnlySpan<int> _indices;
        private readonly ReadOnlySpan<int> _versions;

        internal PairsEnumerable(ReadOnlySpan<T> components, ReadOnlySpan<int> indices,
                                 ReadOnlySpan<int> versions)
        {
            _components = components;
            _indices = indices;
            _versions = versions;
        }

        public PairsEnumerator GetEnumerator()
            => new PairsEnumerator(_components, _indices, _versions);
    }

    /// <summary>Allocation-free enumerator over (entity, component) pairs.</summary>
    public ref struct PairsEnumerator
    {
        private readonly ReadOnlySpan<T> _components;
        private readonly ReadOnlySpan<int> _indices;
        private readonly ReadOnlySpan<int> _versions;
        private int _index;

        internal PairsEnumerator(ReadOnlySpan<T> components, ReadOnlySpan<int> indices,
                                 ReadOnlySpan<int> versions)
        {
            _components = components;
            _indices = indices;
            _versions = versions;
            _index = -1;
        }

        public bool MoveNext() => ++_index < _components.Length;

        public (EntityId Entity, T Component) Current
            // TRUE version from the world's version table. _versions is keyed by
            // ENTITY INDEX, _components/_indices by dense position — hence the
            // double indirection.
            => (new EntityId(_indices[_index], _versions[_indices[_index]]),
                _components[_index]);
    }
}

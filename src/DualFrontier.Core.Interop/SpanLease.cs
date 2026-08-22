using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Lease of read-only access to a native component storage span.
/// Disposing the lease releases the span back to the native side, allowing
/// mutation again.
///
/// Provides ReadOnlySpan&lt;T&gt; over the dense data, ReadOnlySpan&lt;int&gt;
/// over the parallel entity-index array, <see cref="Versions"/> over the world's
/// per-slot version table, and (since K5) <see cref="Pairs"/> for (EntityId, T)
/// iteration. Lease pooling remains deferred — K7 will measure first.
///
/// Lifetime contract (mirrors df_capi.h):
///   * While ANY SpanLease is active on the owning <see cref="NativeWorld"/>,
///     mutation calls (Add/Remove/Destroy/Flush) are silently rejected by the
///     native side — the throw is caught at the C ABI boundary.
///   * Since ID-B a lease also holds a versions view, so entity CREATION is
///     rejected for the lease's lifetime too — the versions table can resize.
///     Dispose the lease before minting entities.
///   * Caller MUST <see cref="Dispose"/> the lease before issuing mutations.
///   * Multiple concurrent leases are allowed (different OR same type).
/// </summary>
public sealed unsafe class SpanLease<T> : IDisposable where T : unmanaged
{
    private readonly NativeWorld _world;
    private readonly uint _typeId;
    private readonly void* _densePtr;
    private readonly int* _indicesPtr;
    private readonly int _count;
    private readonly int* _versionsPtr;
    private readonly int _versionsCount;
    private bool _released;

    internal SpanLease(NativeWorld world, uint typeId,
                       void* densePtr, int* indicesPtr, int count,
                       int* versionsPtr, int versionsCount)
    {
        _world = world;
        _typeId = typeId;
        _densePtr = densePtr;
        _indicesPtr = indicesPtr;
        _count = count;
        _versionsPtr = versionsPtr;
        _versionsCount = versionsCount;
        _released = false;
    }

    /// <summary>Number of components in the span.</summary>
    public int Count => _count;

    /// <summary>
    /// Read-only span over the dense component data. Valid until
    /// <see cref="Dispose"/> is called.
    /// </summary>
    public ReadOnlySpan<T> Span
    {
        get
        {
            if (_released) throw new ObjectDisposedException(nameof(SpanLease<T>));
            return new ReadOnlySpan<T>(_densePtr, _count);
        }
    }

    /// <summary>
    /// Read-only span over entity indices, parallel to <see cref="Span"/>.
    /// <c>indices[i]</c> is the entity index for <c>span[i]</c>.
    /// </summary>
    public ReadOnlySpan<int> Indices
    {
        get
        {
            if (_released) throw new ObjectDisposedException(nameof(SpanLease<T>));
            return new ReadOnlySpan<int>(_indicesPtr, _count);
        }
    }

    /// <summary>
    /// Read-only view over the world's per-slot version table, acquired
    /// alongside the component span and released with it.
    ///
    /// <para>
    /// <b>Indexed by ENTITY INDEX, not by dense position.</b> This span is NOT
    /// parallel to <see cref="Span"/> or <see cref="Indices"/>: its length is the
    /// world's version-table size, not <see cref="Count"/>. The correct read for
    /// dense position <c>i</c> is <c>Versions[Indices[i]]</c>. Indexing it with a
    /// dense position instead reads some unrelated slot's generation.
    /// </para>
    ///
    /// <para>
    /// This is the surface that ends version fabrication (К-L22): the version a
    /// caller writes into an <see cref="EntityId"/> now comes from the world
    /// rather than from a guess.
    /// </para>
    /// </summary>
    public ReadOnlySpan<int> Versions
    {
        get
        {
            if (_released) throw new ObjectDisposedException(nameof(SpanLease<T>));
            return new ReadOnlySpan<int>(_versionsPtr, _versionsCount);
        }
    }

    /// <summary>
    /// Iterate (EntityId, T) pairs over the span. Resolves K1 skeleton's
    /// deferred paired-iteration helper.
    ///
    /// <para>
    /// Since ID-B the version is TRUE, not reconstructed: the pair reads
    /// <c>Versions[entityIndex]</c> from the world's own version table
    /// (<c>df_world_acquire_versions</c>), so the id names the entity that
    /// actually occupies the slot — including a slot whose index has been
    /// RECYCLED, which is precisely the case every fabricated version got wrong.
    /// A write batch keyed on an id this enumerator yields therefore survives
    /// the flush-time version check.
    /// </para>
    ///
    /// <para>
    /// History, because the shape of the bug is instructive. K5 reconstructed
    /// <c>Version = 1</c> — a version NO entity ever carries, since versions
    /// start at 0 and only grow — so every batched write keyed on a span id was
    /// recorded and then silently dropped at flush, and the canonical
    /// read-span-then-write-batch loop wrote nothing at all. W3 corrected that to
    /// <c>Version = 0</c>, which is right for a never-recycled index and wrong
    /// for a recycled one; the K7-era note here excused the gap on the grounds
    /// that "the span ABI does not carry per-entity versions". That is no longer
    /// true, and the deferral ends here (finding F-59).
    /// </para>
    /// </summary>
    public PairsEnumerable Pairs => new PairsEnumerable(this);

    public readonly struct PairsEnumerable
    {
        private readonly SpanLease<T> _lease;
        internal PairsEnumerable(SpanLease<T> lease) => _lease = lease;
        public PairsEnumerator GetEnumerator() => new PairsEnumerator(_lease);
    }

    public ref struct PairsEnumerator
    {
        private readonly SpanLease<T> _lease;
        private int _index;

        internal PairsEnumerator(SpanLease<T> lease)
        {
            _lease = lease;
            _index = -1;
        }

        public bool MoveNext() => ++_index < _lease.Count;

        public (EntityId Entity, T Component) Current
        {
            get
            {
                // Versions is entity-index-keyed, Span/Indices are dense-keyed —
                // hence the double indirection. See SpanLease{T}.Versions.
                int entityIndex = _lease.Indices[_index];
                return (new EntityId(entityIndex, _lease.Versions[entityIndex]),
                        _lease.Span[_index]);
            }
        }
    }

    public void Dispose()
    {
        if (_released) return;
        // Reverse acquisition order: the versions view was taken after the
        // component span, so it is released first.
        NativeMethods.df_world_release_versions(_world.HandleForInternalUse);
        NativeMethods.df_world_release_span(_world.HandleForInternalUse, _typeId);
        _released = true;
    }
}

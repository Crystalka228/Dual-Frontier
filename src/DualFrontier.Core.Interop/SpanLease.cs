using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Lease of read-only access to a native component storage span.
/// Disposing the lease releases the span back to the native side, allowing
/// mutation again.
///
/// Provides ReadOnlySpan&lt;T&gt; over the dense data, ReadOnlySpan&lt;int&gt;
/// over the parallel entity-index array, and (since K5)
/// <see cref="Pairs"/> for (EntityId, T) iteration. Lease pooling remains
/// deferred — K7 will measure first.
///
/// Lifetime contract (mirrors df_capi.h):
///   * While ANY SpanLease is active on the owning <see cref="NativeWorld"/>,
///     mutation calls (Add/Remove/Destroy/Flush) are silently rejected by the
///     native side — the throw is caught at the C ABI boundary.
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
    private bool _released;

    internal SpanLease(NativeWorld world, uint typeId,
                       void* densePtr, int* indicesPtr, int count)
    {
        _world = world;
        _typeId = typeId;
        _densePtr = densePtr;
        _indicesPtr = indicesPtr;
        _count = count;
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
    /// Iterate (EntityId, T) pairs over the span. Resolves K1 skeleton's
    /// deferred paired-iteration helper.
    ///
    /// Caveat: <see cref="EntityId"/> is reconstructed with <c>Version=1</c>
    /// because <see cref="SpanLease{T}"/> does not currently track per-entity
    /// versions. Suitable for fresh entities (test scenarios) and for
    /// snapshot-then-record flows (the recorded command is validated by
    /// version at flush time, so stale ids are rejected). Production-grade
    /// version reconstruction would require either a per-pair P/Invoke or
    /// extending the span ABI to return parallel version arrays — deferred
    /// to K7 once a measurement shows correctness pressure.
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
                int entityIndex = _lease.Indices[_index];
                return (new EntityId(entityIndex, 1), _lease.Span[_index]);
            }
        }
    }

    public void Dispose()
    {
        if (_released) return;
        NativeMethods.df_world_release_span(_world.HandleForInternalUse, _typeId);
        _released = true;
    }
}

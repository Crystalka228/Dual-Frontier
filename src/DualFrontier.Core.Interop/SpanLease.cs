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
    /// Version reconstruction: the span ABI returns entity INDICES only
    /// (<c>df_world_acquire_span</c> hands back <c>const int32_t** out_indices_ptr</c>,
    /// no parallel version array), so the pair must supply a version itself. It
    /// uses <c>Version=0</c> — the same reconstruction every span consumer in
    /// <c>src/DualFrontier.Systems</c> already performs, and the version a
    /// never-recycled entity actually carries (versions start at 0 and only grow;
    /// see <c>EntityIdPacking</c>).
    ///
    /// <para>
    /// W3 correction: this previously reconstructed <c>Version=1</c>, which is the
    /// version NO freshly created entity has. A batched write keyed on such an id is
    /// recorded and then rejected by the version check at flush — silently — so the
    /// canonical read-span-then-write-batch loop wrote nothing at all. See the D-2
    /// regression tests.
    /// </para>
    ///
    /// <para>
    /// Still open: an entity whose index has been RECYCLED carries a version above 0
    /// and is not reconstructible from the span alone. That needs a parallel version
    /// array across the span ABI (a native change) and is tracked as its own ROADMAP
    /// finding; it is the identical limitation every engine-side span consumer already
    /// has, not a new one introduced here.
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
                int entityIndex = _lease.Indices[_index];
                return (new EntityId(entityIndex, 0), _lease.Span[_index]);
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

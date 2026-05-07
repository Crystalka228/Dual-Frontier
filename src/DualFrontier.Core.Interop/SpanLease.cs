using System;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Lease of read-only access to a native component storage span.
/// Disposing the lease releases the span back to the native side, allowing
/// mutation again.
///
/// K1 SKELETON: provides ReadOnlySpan&lt;T&gt; over the dense data and
/// ReadOnlySpan&lt;int&gt; over the parallel entity-index array. K5 will
/// extend with paired iteration helpers and lease pooling.
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

    public void Dispose()
    {
        if (_released) return;
        NativeMethods.df_world_release_span(_world.HandleForInternalUse, _typeId);
        _released = true;
    }
}

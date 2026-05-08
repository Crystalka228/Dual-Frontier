using System;
using System.Runtime.CompilerServices;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Command Buffer for bulk component mutations. System code records updates,
/// adds, removes; native side validates and applies atomically at Flush time.
///
/// Architectural property (K5 Q2): managed code never directly mutates native
/// memory. All mutations are commands — recorded against a native-side
/// batch, applied atomically when the batch is flushed.
///
/// Usage pattern:
/// <code>
/// using var batch = world.BeginBatch&lt;NeedsComponent&gt;();
/// foreach (var (entity, needs) in batch)
/// {
///     var modified = needs;
///     modified.Satiety -= 0.002f;
///     batch.Update(entity, modified);
/// }
/// // On Dispose: native auto-flushes pending commands atomically.
/// </code>
///
/// Lifecycle:
///   * Construction increments the world's active-batches counter.
///   * <see cref="Update"/>/<see cref="Add"/>/<see cref="Remove"/> append to the
///     native command buffer (one P/Invoke per call в K5; bulk transmit
///     deferred to K7+ once measured).
///   * <see cref="Flush"/> applies commands atomically. Returns count of
///     successful commands (entities still alive at flush time).
///   * <see cref="Cancel"/> discards commands without applying.
///   * <see cref="Dispose"/> releases the native handle. If the batch was
///     neither flushed nor cancelled, the native dtor auto-flushes.
///
/// Mutation rejection contract:
///   While ANY batch is active on the world, direct mutations via
///   <see cref="NativeWorld.AddComponent{T}"/> /
///   <see cref="NativeWorld.RemoveComponent{T}"/> /
///   <see cref="NativeWorld.DestroyEntity"/> /
///   <see cref="NativeWorld.FlushDestroyedEntities"/> /
///   <see cref="NativeWorld.AddComponents{T}"/> are rejected (no-op'd silently
///   at the C ABI boundary). Caller must Dispose all batches before resuming
///   direct mutations. Multiple concurrent batches are allowed.
/// </summary>
public sealed unsafe class WriteBatch<T> : IDisposable where T : unmanaged
{
    private readonly NativeWorld _world;
    private readonly uint _typeId;
    private IntPtr _batchHandle;
    private bool _flushed;
    private bool _cancelled;

    /// <summary>
    /// True if the batch can still record commands (not flushed, not
    /// cancelled, not disposed).
    /// </summary>
    public bool IsActive => !_flushed && !_cancelled && _batchHandle != IntPtr.Zero;

    internal WriteBatch(NativeWorld world, uint typeId, IntPtr batchHandle)
    {
        _world = world;
        _typeId = typeId;
        _batchHandle = batchHandle;
        _flushed = false;
        _cancelled = false;
    }

    /// <summary>
    /// Record a component update for an entity. Update applies only if the
    /// component is currently present on the entity (use <see cref="Add"/>
    /// to set unconditionally).
    /// </summary>
    /// <returns>True if recorded successfully; false on validation failure.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Update(EntityId entity, T value)
    {
        ThrowIfNotActive();
        T temp = value;
        int result = NativeMethods.df_batch_record_update(
            _batchHandle, EntityIdPacking.Pack(entity), &temp);
        return result != 0;
    }

    /// <summary>
    /// Record a component add for an entity. Add applies unconditionally —
    /// overwrites the existing component if present.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(EntityId entity, T value)
    {
        ThrowIfNotActive();
        T temp = value;
        int result = NativeMethods.df_batch_record_add(
            _batchHandle, EntityIdPacking.Pack(entity), &temp);
        return result != 0;
    }

    /// <summary>
    /// Record a component remove for an entity. Remove applies only if the
    /// component is currently present.
    /// </summary>
    public bool Remove(EntityId entity)
    {
        ThrowIfNotActive();
        int result = NativeMethods.df_batch_record_remove(
            _batchHandle, EntityIdPacking.Pack(entity));
        return result != 0;
    }

    /// <summary>
    /// Apply all recorded commands atomically. Returns count of successful
    /// commands (entities still alive at flush time, command preconditions
    /// met).
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// If the batch was already flushed or was cancelled.
    /// </exception>
    public int Flush()
    {
        ThrowIfNotActive();
        _flushed = true;
        int result = NativeMethods.df_batch_flush(_batchHandle);
        if (result < 0)
        {
            throw new InvalidOperationException(
                "Flush failed — batch state corrupt or already flushed on the native side.");
        }
        return result;
    }

    /// <summary>
    /// Cancel the batch — discard recorded commands without applying.
    /// Subsequent <see cref="Dispose"/> is a no-op for commands.
    /// </summary>
    public void Cancel()
    {
        if (_flushed || _cancelled || _batchHandle == IntPtr.Zero) return;
        _cancelled = true;
        NativeMethods.df_batch_cancel(_batchHandle);
    }

    /// <summary>
    /// Iterate (EntityId, T) pairs over current native storage for type T.
    /// Iteration sees a snapshot — recorded commands are NOT visible until
    /// after <see cref="Flush"/>.
    ///
    /// Caveat: returned EntityId reconstructs version=1 (see
    /// <see cref="SpanLease{T}.Pairs"/>). Production accuracy via per-pair
    /// version lookup is deferred to K7 if measurement shows correctness
    /// issues. For systems that immediately re-record an Update, this is
    /// safe — flush rejects stale entities by version-mismatch.
    /// </summary>
    public BatchEnumerator GetEnumerator()
    {
        ThrowIfNotActive();
        return new BatchEnumerator(_world.AcquireSpan<T>());
    }

    public void Dispose()
    {
        if (_batchHandle == IntPtr.Zero) return;
        // Native dtor auto-flushes if !flushed && !cancelled.
        NativeMethods.df_batch_destroy(_batchHandle);
        _batchHandle = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }

    ~WriteBatch()
    {
        if (_batchHandle != IntPtr.Zero)
        {
            NativeMethods.df_batch_destroy(_batchHandle);
            _batchHandle = IntPtr.Zero;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfNotActive()
    {
        if (_batchHandle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(WriteBatch<T>));
        if (_flushed)
            throw new InvalidOperationException("Batch already flushed.");
        if (_cancelled)
            throw new InvalidOperationException("Batch was cancelled.");
    }

    /// <summary>
    /// Enumerator yielding (EntityId, T) pairs over the read-time native
    /// storage snapshot. The snapshot is held alive by an internal
    /// <see cref="SpanLease{T}"/> for the duration of iteration; recorded
    /// batch commands do NOT show up until after <see cref="Flush"/>.
    /// </summary>
    public ref struct BatchEnumerator
    {
        private SpanLease<T> _lease;
        private int _index;

        internal BatchEnumerator(SpanLease<T> lease)
        {
            _lease = lease;
            _index = -1;
        }

        public bool MoveNext()
        {
            return ++_index < _lease.Count;
        }

        public (EntityId Entity, T Component) Current
        {
            get
            {
                int entityIndex = _lease.Indices[_index];
                // Version=1 simplification — see WriteBatch.GetEnumerator
                // remarks. Production-grade version reconstruction is K7 work.
                return (new EntityId(entityIndex, 1), _lease.Span[_index]);
            }
        }

        public void Dispose()
        {
            _lease?.Dispose();
            _lease = null!;
        }
    }
}

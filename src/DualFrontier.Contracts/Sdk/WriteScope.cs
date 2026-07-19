using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// Scoped command buffer for bulk component mutations, obtained from
/// <see cref="ISystemContext.BeginBatch{T}"/>. The Contracts-safe promotion of
/// the engine-internal write batch: recorded commands are applied atomically at
/// <see cref="Flush"/> (or on <see cref="Dispose"/> auto-flush), never by direct
/// native writes — no <c>Core.Interop</c> type is named.
///
/// <para>
/// <b>Lifetime — preserved exactly.</b> Use in a <c>using</c> scope. While ANY
/// batch is live, direct mutations are rejected native-side. If neither flushed
/// nor cancelled, <see cref="Dispose"/> auto-flushes (mirrors the engine batch
/// dtor). A <see langword="ref"/> <see langword="struct"/>: cannot escape its scope.
/// </para>
///
/// <para>
/// <b>Allocation.</b> Stack-only; delegates each recording call to the engine
/// capability with the batch passed as an opaque <see cref="object"/> — zero
/// per-tick allocation over the <c>SystemBase</c> path (KERNEL_ARCHITECTURE §2,
/// К-L3.1). The constructor is <c>internal</c> (forge discipline).
/// </para>
/// </summary>
/// <typeparam name="T">The unmanaged component type.</typeparam>
public readonly ref struct WriteScope<T> where T : unmanaged
{
    private readonly IWriteBatchCapability? _capability;
    // The real Core.Interop batch, held unnamed as an object (and IDisposable).
    private readonly object? _batch;

    internal WriteScope(IWriteBatchCapability capability, object batch)
    {
        _capability = capability;
        _batch = batch;
    }

    /// <summary>Record a component update (applies only if present at flush).</summary>
    public bool Update(EntityId entity, T value)
        => _capability is not null && _capability.Update<T>(_batch!, entity, value);

    /// <summary>Record an unconditional component add/overwrite.</summary>
    public bool Add(EntityId entity, T value)
        => _capability is not null && _capability.Add<T>(_batch!, entity, value);

    /// <summary>Record a component remove (applies only if present at flush).</summary>
    public bool Remove(EntityId entity)
        => _capability is not null && _capability.Remove<T>(_batch!, entity);

    /// <summary>Apply recorded commands atomically; returns the successful count.</summary>
    public int Flush() => _capability is null ? 0 : _capability.Flush<T>(_batch!);

    /// <summary>Discard recorded commands without applying.</summary>
    public void Cancel() => _capability?.Cancel<T>(_batch!);

    /// <summary>Releases the batch; auto-flushes if not flushed/cancelled. Idempotent.</summary>
    public void Dispose() => (_batch as IDisposable)?.Dispose();
}

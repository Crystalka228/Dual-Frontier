using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// Engine-side capability that backs <see cref="WriteScope{T}"/>'s recording
/// operations. INTERNAL by design: the batch-write scope is a Contracts value
/// type a mod holds, but the underlying command-buffer object lives in the
/// engine and must never be named or forged from a mod assembly.
///
/// <para>
/// The engine (the SDK system-context view, <c>DualFrontier.Application</c>)
/// implements this interface and is granted visibility via
/// <c>InternalsVisibleTo</c>. A mod's AssemblyLoadContext cannot see an
/// <c>internal</c> type here, so a mod cannot implement it or call its members.
/// The opaque <see cref="object"/> handle is the real
/// <c>DualFrontier.Core.Interop.WriteBatch&lt;T&gt;</c>, passed back unnamed and
/// re-cast engine-side; no <c>Core.Interop</c> type crosses into Contracts.
/// </para>
///
/// <para>
/// Allocation contract (CONCURRENCY_AND_MEMORY_MODEL / KERNEL_ARCHITECTURE
/// §2 "Write protocol", К-L3.1): these calls add NO per-tick allocation over
/// the <c>SystemBase</c> path — the batch object already exists; passing it as
/// <see cref="object"/> and re-casting is allocation-free.
/// </para>
/// </summary>
internal interface IWriteBatchCapability
{
    /// <summary>Record a component update (applies only if present at flush).</summary>
    bool Update<T>(object batch, EntityId entity, T value) where T : unmanaged;

    /// <summary>Record an unconditional component add/overwrite.</summary>
    bool Add<T>(object batch, EntityId entity, T value) where T : unmanaged;

    /// <summary>Record a component remove (applies only if present at flush).</summary>
    bool Remove<T>(object batch, EntityId entity) where T : unmanaged;

    /// <summary>Apply recorded commands atomically; returns the successful count.</summary>
    int Flush<T>(object batch) where T : unmanaged;

    /// <summary>Discard recorded commands without applying.</summary>
    void Cancel<T>(object batch) where T : unmanaged;
}

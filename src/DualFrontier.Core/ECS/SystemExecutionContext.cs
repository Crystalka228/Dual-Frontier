using System;
using System.Threading;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Per-system execution context pushed by <see cref="DualFrontier.Core.Scheduling.ParallelSystemScheduler"/>
/// before each <c>SystemBase.Update</c> and popped after (always, even on
/// exceptions). Holds the <see cref="NativeWorld"/> handle systems reach
/// through <c>SystemBase.NativeWorld</c>, the <see cref="IGameServices"/>
/// aggregator, and — for mod-origin systems — the owning mod id and fault
/// sink.
///
/// Stored in a <see cref="ThreadLocal{T}"/> slot so each scheduler thread
/// has its own current context. <c>async</c>/<c>await</c> inside a system
/// would suspend on a different thread where <see cref="Current"/> is null;
/// it is forbidden in Domain code per TechArch 11.7.
///
/// K8.3+K8.4 cutover: the managed-<c>World</c> access surface (GetComponent /
/// SetComponent / Query / GetSystem) is removed — systems read and write
/// component storage exclusively through <see cref="NativeWorld"/>. Isolation
/// is enforced at compile time by the <c>[SystemAccess]</c> attribute (which
/// <see cref="DualFrontier.Core.Scheduling.DependencyGraph"/> consumes for
/// edge-building) and by the future A'.9 Roslyn analyzer; the runtime
/// guard methods that previously threw <c>IsolationViolationException</c>
/// are deleted.
/// </summary>
public sealed class SystemExecutionContext
{
    private static readonly ThreadLocal<SystemExecutionContext?> _current = new();

    private readonly string _systemName;
    // F-54 (W2): the former `_allowedBuses` field is retired -- it was 1 write / 0 reads
    // (ParallelSystemScheduler fed [SystemAccess].Buses in; nothing ever read it back). Per-system
    // bus-publish scoping, if ever wanted, is future design on the FQN capability gate
    // (RestrictedModApi.EnforceCapability), not a revival of this dead field.
    private readonly SystemOrigin _origin;
    private readonly string? _modId;
    private readonly IModFaultSink _faultSink;
    private readonly IGameServices? _services;
    private readonly NativeWorld _nativeWorld;
    // K8.3+K8.4 — Path β bridge. Null for Core systems and for tests that
    // don't exercise managed-class storage. The scheduler passes the
    // ModRegistry as the resolver (ModRegistry implements
    // IManagedStorageResolver and tracks per-mod RestrictedModApi instances).
    private readonly IManagedStorageResolver? _managedStorageResolver;

    /// <summary>
    /// Creates a context for the given system. The scheduler supplies the
    /// system metadata (origin, modId) and the native world handle.
    /// <paramref name="nativeWorld"/> is required — production systems read
    /// and write component storage through it.
    /// </summary>
    /// <param name="systemName">Display name for diagnostics.</param>
    /// <param name="origin">Core vs Mod provenance of the system.</param>
    /// <param name="modId">Mod identifier when <paramref name="origin"/> is Mod; otherwise null.</param>
    /// <param name="faultSink">Destination for mod-origin fault reports (e.g. surfaced via <c>ModLoader.HandleModFault</c>).</param>
    /// <param name="nativeWorld">Native world handle exposed to the system via <c>SystemBase.NativeWorld</c>. Required.</param>
    /// <param name="services">Domain-bus aggregator exposed via <c>SystemBase.Services</c>; null for tests that do not exercise publication.</param>
    /// <param name="managedStorageResolver">K8.3+K8.4 — optional Path β resolver. Production passes the ModRegistry; null in tests and core-only builds.</param>
    internal SystemExecutionContext(
        string systemName,
        SystemOrigin origin,
        string? modId,
        IModFaultSink faultSink,
        NativeWorld nativeWorld,
        IGameServices? services = null,
        IManagedStorageResolver? managedStorageResolver = null)
    {
        _systemName = systemName ?? throw new ArgumentNullException(nameof(systemName));
        _faultSink = faultSink ?? throw new ArgumentNullException(nameof(faultSink));
        _nativeWorld = nativeWorld ?? throw new ArgumentNullException(nameof(nativeWorld));

        _origin = origin;
        _modId = modId;
        _services = services;
        _managedStorageResolver = managedStorageResolver;
    }

    /// <summary>
    /// Domain-bus aggregator supplied by the scheduler. Null when no
    /// services were provided at construction time (isolated unit tests).
    /// Exposed internally; systems reach it via <c>SystemBase.Services</c>.
    /// </summary>
    internal IGameServices? Services => _services;

    /// <summary>
    /// Native world handle supplied by the scheduler. Non-null — required
    /// at construction. Exposed internally; systems reach it via
    /// <c>SystemBase.NativeWorld</c>.
    /// </summary>
    internal NativeWorld NativeWorld => _nativeWorld;

    /// <summary>
    /// K8.3+K8.4 — Resolves the calling system's owning mod to its Path β
    /// <see cref="ManagedStore{T}"/>. Returns null when:
    /// <list type="bullet">
    ///   <item>No resolver was supplied at construction (tests, builds
    ///         without mod loading).</item>
    ///   <item>The system origin is <see cref="SystemOrigin.Core"/> — no
    ///         owning mod, so no per-mod store. Core systems use NativeWorld
    ///         for component storage.</item>
    ///   <item>The mod has not registered <typeparamref name="T"/> via
    ///         <c>IModApi.RegisterManagedComponent&lt;T&gt;</c>.</item>
    /// </list>
    /// </summary>
    internal ManagedStore<T>? ResolveManagedStore<T>() where T : class, IComponent
    {
        if (_managedStorageResolver is null) return null;
        if (_modId is null) return null;
        return _managedStorageResolver.Resolve<T>(_modId);
    }

    /// <summary>
    /// Routes a fault thrown by the system/subscriber bound to this context under
    /// the D2 origin-asymmetric policy (CONCURRENCY_AND_MEMORY_MODEL §7): a
    /// mod-origin fault is reported to the <see cref="IModFaultSink"/> and
    /// CONTAINED — the caller continues; a core-origin fault (or one from a
    /// context with no mod identity) is RECORDED on the existing logging surface
    /// and the caller must RETHROW, so fail-fast is preserved. The record is a
    /// DEBUG-build diagnostic aid; the rethrow is the config-independent contract
    /// (ENGINE_LIFECYCLE_AND_TRANSACTIONS §4, fault class 1 — a core bug crashes
    /// in both DEBUG and RELEASE). This is the single definition of the policy;
    /// the domain bus (both delivery modes) and the scheduler's per-phase catch
    /// call it. First read site of <c>_faultSink</c> since the K8.3+K8.4 cutover.
    /// </summary>
    /// <param name="ex">The fault to route.</param>
    /// <param name="modId">The offending mod id when the result is
    /// <see cref="FaultDisposition.ContainedMod"/>; otherwise null.</param>
    /// <returns>Whether the fault was contained at the sink or must be rethrown.</returns>
    internal FaultDisposition RouteFault(Exception ex, out string? modId)
    {
        if (_origin == SystemOrigin.Mod && _modId is not null)
        {
            _faultSink.ReportFault(_modId, ex.Message);
            modId = _modId;
            return FaultDisposition.ContainedMod;
        }

        modId = null;
        System.Diagnostics.Debug.WriteLine(
            $"[EQ-A1] core-origin fault in system '{_systemName}' (origin={_origin}): {ex}");
        return FaultDisposition.RethrowCore;
    }

    /// <summary>
    /// Current execution context for the calling thread. Null when the
    /// calling thread does not belong to the scheduler (e.g. the Launcher
    /// render thread, a test that has not pushed a context).
    /// </summary>
    public static SystemExecutionContext? Current => _current.Value;

    /// <summary>
    /// Pushes <paramref name="context"/> onto the calling thread's slot.
    /// Throws <see cref="InvalidOperationException"/> if a context is
    /// already set — indicates a scheduler bug (nested push without pop).
    /// Called by <see cref="DualFrontier.Core.Scheduling.ParallelSystemScheduler"/>
    /// before <c>SystemBase.Update</c>.
    /// </summary>
    /// <param name="context">The context to make current on this thread.</param>
    internal static void PushContext(SystemExecutionContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (_current.Value is not null)
            throw new InvalidOperationException(
                "SystemExecutionContext is already set on this thread — nested push detected. " +
                "This indicates a scheduler bug: PushContext was called without a matching PopContext.");
        _current.Value = context;
    }

    /// <summary>
    /// Pops the context from the calling thread's slot. Called by
    /// <see cref="DualFrontier.Core.Scheduling.ParallelSystemScheduler"/>
    /// in a <c>finally</c> block after <c>SystemBase.Update</c> so the slot
    /// is always cleared, even on exceptions.
    /// </summary>
    internal static void PopContext()
    {
        _current.Value = null;
    }
}

/// <summary>
/// Outcome of <see cref="SystemExecutionContext.RouteFault"/>: whether a fault
/// was contained at the mod-fault sink (mod origin — caller continues) or must
/// be rethrown (core origin — fail-fast). The D2 origin-asymmetric policy
/// (CONCURRENCY_AND_MEMORY_MODEL §7).
/// </summary>
internal enum FaultDisposition
{
    /// <summary>Mod-origin fault reported to the sink; caller continues.</summary>
    ContainedMod,

    /// <summary>Core-origin fault recorded; caller must rethrow (fail-fast).</summary>
    RethrowCore,
}

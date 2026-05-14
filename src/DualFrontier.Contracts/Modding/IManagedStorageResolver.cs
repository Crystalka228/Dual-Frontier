using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Resolves a mod-id to a <see cref="ManagedStore{T}"/> instance. The
/// implementation lives in <c>DualFrontier.Application.Modding.ModRegistry</c>
/// — knows mod-id → RestrictedModApi mapping; cannot be linked from this
/// XML doc because Contracts sits below Application in the assembly graph.
/// Surfaced to <c>SystemExecutionContext</c> via the
/// <c>ParallelSystemScheduler</c> constructor so system bodies reach per-mod
/// Path β storage via <c>SystemBase.ManagedStore&lt;T&gt;()</c>.
///
/// Lives in <c>DualFrontier.Contracts.Modding</c> rather than
/// <c>DualFrontier.Application.Modding</c> so <c>DualFrontier.Core</c> can
/// reference the abstraction without inverting the assembly dependency
/// direction (Application depends on Core, not vice versa).
///
/// K8.3+K8.4 combined milestone (2026-05-14) introduces this interface.
/// Implementations return null when the requested type was not registered
/// by the named mod; SystemBase.ManagedStore&lt;T&gt; surfaces null to
/// callers so they degrade gracefully.
/// </summary>
public interface IManagedStorageResolver
{
    /// <summary>
    /// Returns the <see cref="ManagedStore{T}"/> for the mod identified by
    /// <paramref name="modId"/>, or <c>null</c> if the mod has not registered
    /// <typeparamref name="T"/> via
    /// <c>IModApi.RegisterManagedComponent&lt;T&gt;</c>. Returns <c>null</c>
    /// for unknown <paramref name="modId"/>; Core-origin systems (whose modId
    /// is null) receive null at the SystemExecutionContext layer before this
    /// method is reached.
    /// </summary>
    ManagedStore<T>? Resolve<T>(string modId) where T : class, IComponent;
}

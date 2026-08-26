using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Projects <see cref="ModRegistry"/>'s current registration list into the
/// per-system metadata dictionary the scheduler needs for fault-routing
/// origin propagation. Called by <see cref="DualFrontier.Application.Loop.GameBootstrap"/>
/// at startup (initial core-only state) and by <see cref="ModIntegrationPipeline"/>
/// at every successful Apply / UnloadMod / UnloadAll boundary so the
/// scheduler's metadata stays in sync with the active mod set.
/// </summary>
internal static class SystemMetadataBuilder
{
    /// <summary>
    /// Builds an immutable snapshot of every registered system's metadata.
    /// The returned dictionary is keyed by <see cref="SystemBase"/> instance
    /// (reference equality) so the scheduler can look up the metadata for
    /// a system encountered during phase iteration without paying for
    /// reflection or string comparison.
    /// </summary>
    /// <param name="registry">Source of truth for registered systems.</param>
    /// <returns>
    /// Read-only dictionary mapping each system instance to its
    /// <see cref="SystemMetadata"/>. Systems registered as core have
    /// <c>Origin=Core, ModId=null</c>; mod systems carry their owning
    /// <c>modId</c>.
    /// </returns>
    public static IReadOnlyDictionary<SystemBase, SystemMetadata> Build(ModRegistry registry)
    {
        if (registry is null) throw new System.ArgumentNullException(nameof(registry));

        var lookup = new Dictionary<SystemBase, SystemMetadata>();
        foreach (SystemRegistration reg in registry.GetAllSystems())
        {
            lookup[reg.Instance] = new SystemMetadata(
                reg.Origin, reg.ModId, ResolveTicksPerUpdate(reg.Instance));
        }
        return lookup;
    }

    /// <summary>
    /// Resolves a system's tick cadence from its <c>[TickRate]</c> declaration,
    /// normalising a missing or non-positive rate to
    /// <see cref="DualFrontier.Core.Scheduling.TickRates.REALTIME"/>. Reads the
    /// <see cref="SystemBase"/> hook
    /// rather than the concrete type so an SDK system wrapped in
    /// <c>SystemAdapter&lt;T&gt;</c> forwards its INNER rate (W1 BD-1) -- the
    /// adapter's own <c>[TickRate]</c> is a DFK013 bridge placeholder.
    ///
    /// <para>
    /// F-60(a): this runs at LOAD time, once per system per rebuild, and its
    /// result is carried in the metadata table. It previously ran lazily on the
    /// tick path and was memoised into a <c>ConcurrentDictionary&lt;Type,int&gt;</c>
    /// on <c>TickScheduler</c>, whose strong <see cref="System.Type"/> keys
    /// (for an SDK mod system, <c>SystemAdapter&lt;TSystem&gt;</c> closed over a
    /// collectible-ALC type) had no eviction and rooted the mod's ALC for the
    /// remaining session -- so a mod that ticked even once could never be
    /// reclaimed.
    /// </para>
    /// </summary>
    private static int ResolveTicksPerUpdate(SystemBase system)
    {
        TickRateAttribute? attribute = system.TickRateDeclaration;
        if (attribute is null)
            return DualFrontier.Core.Scheduling.TickRates.REALTIME;

        int value = attribute.TicksPerUpdate;
        return value > 0 ? value : DualFrontier.Core.Scheduling.TickRates.REALTIME;
    }
}

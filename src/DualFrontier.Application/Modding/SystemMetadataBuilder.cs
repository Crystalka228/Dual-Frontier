using System.Collections.Generic;
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
            lookup[reg.Instance] = new SystemMetadata(reg.Origin, reg.ModId);
        }
        return lookup;
    }
}

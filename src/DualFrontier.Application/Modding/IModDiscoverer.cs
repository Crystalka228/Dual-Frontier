using System.Collections.Generic;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Locates available mods for the editing session. Implementations may
/// scan a directory tree (<see cref="DefaultModDiscoverer"/>), enumerate
/// embedded vanilla manifests, query a Steam Workshop adapter, etc.
/// The contract is purely about producing a list — composition,
/// dependency resolution, and load-order all happen later inside
/// <see cref="ModIntegrationPipeline.Apply"/>.
/// </summary>
public interface IModDiscoverer
{
    /// <summary>
    /// Returns every mod the discoverer can locate, with the absolute
    /// or pipeline-acceptable path (the same string passed to
    /// <see cref="ModIntegrationPipeline.Apply"/>) and the parsed
    /// manifest. Order is implementation-defined; the controller
    /// re-orders as needed for display.
    /// </summary>
    IReadOnlyList<DiscoveredModInfo> Discover();
}

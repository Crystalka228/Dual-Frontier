using DualFrontier.Contracts.Services;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// The construction-time dependency surface handed to a system's factory when it
/// is registered (<c>RegisterSystem&lt;T&gt;(Func&lt;ISystemServices, T&gt;)</c>).
/// A system resolves the services it needs here ONCE, at construction, not per
/// tick (contrast <see cref="ISystemContext"/>, the per-tick world surface).
///
/// <para>
/// Core and mod systems share one construction mechanism (one internal
/// <c>ModRegistry</c> factory path). A mod reaches it through
/// <c>IModApi.RegisterSystem&lt;T&gt;(Func&lt;ISystemServices, T&gt;)</c>, and the
/// factory receives whatever <see cref="ISystemServices"/> the host installed; the
/// parameterless <c>IModApi.RegisterSystem&lt;T&gt;()</c> serves a system that
/// needs nothing at construction. When the host installs none, which is the
/// production composition, every member refuses with a named error at the point
/// of use, so a mod that needs a game service builds that service itself and
/// closes over it in the factory. Either way the mod uses only the SDK a
/// third-party mod has: the boundary-law B-3 endpoint ("everything vanilla needs
/// arrives through the same SDK any third-party mod has").
/// </para>
///
/// <para>
/// <b>Measured consumption only.</b> <see cref="Pathfinding"/> is the member a
/// host that installs a pathfinding service exposes; the production composition
/// installs none, because pathfinding is game content that the mod providing it
/// closes over. No speculative members: this surface grows when a measured
/// consumer appears, never ahead of one.
/// </para>
/// </summary>
public interface ISystemServices
{
    /// <summary>The A* pathfinding service over the walkability grid.</summary>
    IPathfindingService Pathfinding { get; }

}

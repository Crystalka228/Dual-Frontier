using DualFrontier.Contracts.Services;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// The construction-time dependency surface handed to a system's factory when it
/// is registered (<c>RegisterSystem&lt;T&gt;(Func&lt;ISystemServices, T&gt;)</c>).
/// This is where a system resolves the engine services it needs ONCE, at
/// construction — not per tick (contrast <see cref="ISystemContext"/>, the
/// per-tick world surface).
///
/// <para>
/// It unifies the construction MECHANISM across core and mods (one internal
/// <c>ModRegistry</c> factory path). A mod reaches it through
/// <c>IModApi.RegisterSystem&lt;T&gt;(Func&lt;ISystemServices, T&gt;)</c>, so a
/// service-dependent <see cref="Sdk.ISimulationSystem"/> is authored with the same
/// surface a core system is built with; the parameterless
/// <c>IModApi.RegisterSystem&lt;T&gt;()</c> remains for a system that needs no
/// service at construction. This is the boundary-law B-3 endpoint ("everything
/// vanilla needs arrives through the same SDK any third-party mod has").
/// </para>
///
/// <para>
/// <b>Day-one surface = measured consumption only.</b> The single member is
/// <see cref="Pathfinding"/> — the one service the measured harness injects
/// (into <c>MovementSystem</c>). No speculative members: this surface grows when
/// a measured consumer appears, never ahead of one.
/// </para>
/// </summary>
public interface ISystemServices
{
    /// <summary>The A* pathfinding service over the walkability grid.</summary>
    IPathfindingService Pathfinding { get; }

}

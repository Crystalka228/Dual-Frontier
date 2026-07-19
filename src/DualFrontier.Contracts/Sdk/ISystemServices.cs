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
/// It unifies construction across core and mods: the same factory entry builds
/// both, so a system that needs a service (e.g. pathfinding) is authorable as a
/// mod exactly as vanilla authors it — boundary law B-3, "everything vanilla
/// needs arrives through the same SDK any third-party mod has."
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

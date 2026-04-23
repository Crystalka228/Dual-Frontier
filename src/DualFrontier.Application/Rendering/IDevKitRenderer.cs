using DualFrontier.Application.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Application.Rendering;

/// <summary>
/// DevKit-tier extension of <see cref="IRenderer"/>. Implemented only by
/// backends that carry development tooling — the Godot DevKit renderer is
/// the canonical consumer. The Native production renderer never references
/// this interface: its debug surface is deliberately empty to keep the
/// shipped runtime lean.
/// </summary>
/// <remarks>
/// Every method here is a visualisation or inspection aid for developers.
/// Nothing in this contract should ever be called from gameplay logic:
/// systems publish domain events, the DevKit backend reacts. If a feature
/// ever needs to be available in Native, it should be promoted to
/// <see cref="IRenderer"/> — never cross-cast.
/// </remarks>
[DevKitOnly("Debug visualisation surface — not shipped to players.")]
public interface IDevKitRenderer : IRenderer
{
    /// <summary>
    /// Draws a labelled marker at the given tile coordinate. Used for
    /// visualising pathfinding targets, spawner tiles, AI inspection
    /// cursors. The marker persists until the next frame; call every
    /// frame to keep it visible.
    /// </summary>
    /// <param name="position">Tile coordinate to highlight.</param>
    /// <param name="label">Short text shown next to the marker.</param>
    void DrawDebugGizmo(GridVector position, string label);

    /// <summary>
    /// Toggles the per-system profiler overlay. When visible, the overlay
    /// shows phase count, per-phase parallelism, and per-system wall-clock
    /// time averaged over the last N ticks.
    /// </summary>
    /// <param name="visible">True to show, false to hide.</param>
    void ShowSystemProfiler(bool visible);

    /// <summary>
    /// Highlights a single entity with a coloured outline in the next
    /// rendered frame. Used by the entity inspector panel when the user
    /// selects a row. Passing <see cref="EntityId.Invalid"/> clears the
    /// current highlight.
    /// </summary>
    /// <param name="id">Entity to highlight, or <see cref="EntityId.Invalid"/>.</param>
    void HighlightEntity(EntityId id);
}

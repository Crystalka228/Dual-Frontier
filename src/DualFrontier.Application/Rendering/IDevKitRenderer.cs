using DualFrontier.Application.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Application.Rendering;

/// <summary>
/// DevKit-tier extension of <see cref="IRenderer"/> — reserved abstraction
/// surface для future first-party DevKit backend. Currently dormant: no
/// active consumer post-Godot deprecation (К-extensions cascade #2,
/// 2026-05-23). The production runtime (<c>DualFrontier.Launcher</c>) never
/// references this interface — debug surface deliberately empty к keep
/// shipped runtime lean.
/// </summary>
/// <remarks>
/// Reserved для forward DevKit work: when first-party developer tooling
/// materializes, it MAY implement this contract atop the Vulkan substrate
/// (<c>DualFrontier.Runtime</c> primitives) либо supersede it с a
/// Vulkan-native equivalent. Until then the abstraction stays в place as
/// architectural intent — DevKit/Production renderer split survives the
/// Godot deprecation per Q-G-1 LOCKED. Every method here is a visualisation
/// or inspection aid for developers; nothing here should ever be called from
/// gameplay logic. If a feature ever needs to be available in production, it
/// should be promoted к <see cref="IRenderer"/> — never cross-cast.
/// </remarks>
[DevKitOnly("Reserved DevKit surface — not shipped to players. Currently dormant.")]
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

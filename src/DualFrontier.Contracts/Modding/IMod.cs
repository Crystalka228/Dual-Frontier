using System;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Mod entry point. Each mod assembly contains exactly one class implementing
/// this interface. ModLoader resolves the implementation via reflection and
/// calls <see cref="Initialize"/> on start-up and <see cref="Unload"/> on
/// unload (hot reload or game close).
/// </summary>
public interface IMod
{
    /// <summary>
    /// TODO: Phase 2 — called once on mod load. Here the mod registers its
    /// components/systems through <paramref name="api"/> and subscribes to
    /// any events of interest. Blocking operations are forbidden — the
    /// method must return quickly; loading of other mods waits.
    /// </summary>
    void Initialize(IModApi api);

    /// <summary>
    /// TODO: Phase 2 — called on mod unload. Must unsubscribe from all
    /// events and release resources. After return the mod is unloaded from
    /// its AssemblyLoadContext and must not retain references to core
    /// objects.
    /// </summary>
    void Unload();
}

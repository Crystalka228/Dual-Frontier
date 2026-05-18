using System;
using System.Collections.Generic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// K10.2 Item 21 — Per-mod sub-scheduler instance (К-L12 separation).
///
/// Each mod ALC owns a sub-scheduler tracking the systems registered by
/// that mod. К10.2 lands the instance ownership + teardown primitive
/// consumption pattern; detailed mod-system priority arbitration within
/// mod sub-scheduler stays К8.5 scope (mod authoring practice documentation).
///
/// К-L9 «Vanilla = mods» preserved: vanilla mods get a sub-scheduler same as
/// third-party mods. Registration uniformity preserved через IModApi.
/// </summary>
public sealed class ModSubScheduler
{
    private readonly string _modId;
    private readonly List<SystemBase> _systems = new();

    public ModSubScheduler(string modId)
    {
        _modId = modId ?? throw new ArgumentNullException(nameof(modId));
    }

    public string ModId => _modId;

    public IReadOnlyList<SystemBase> Systems => _systems;

    public int SystemCount => _systems.Count;

    public void AddSystem(SystemBase system)
    {
        if (system is null) throw new ArgumentNullException(nameof(system));
        _systems.Add(system);
    }

    /// <summary>
    /// Called by <see cref="ModRegistry.RemoveSubScheduler"/> consumed at
    /// Step 3.5 native primitive consumption pattern (per S3-Q1 L3 layering).
    /// Disposes systems that implement <see cref="IDisposable"/>; clears the
    /// system list. Native-side teardown is handled by the native primitive
    /// (Item 32 <c>df_scheduler_unload_mod_native_state</c>) в parallel.
    /// </summary>
    public void Teardown()
    {
        // SystemBase.Dispose is internal; DualFrontier.Application has
        // InternalsVisibleTo access. Calls OnDispose hook on each system.
        foreach (var system in _systems)
        {
            try { system.Dispose(); }
            catch { /* best-effort per §9.5.1 failure semantics */ }
        }
        _systems.Clear();
    }
}

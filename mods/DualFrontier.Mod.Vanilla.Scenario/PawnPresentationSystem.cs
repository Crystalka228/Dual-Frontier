using System;
using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Contracts.Sdk;

namespace DualFrontier.Mod.Vanilla.Scenario;

/// <summary>
/// Keeps the renderer's view of the colonists in step with the world, by comparing what the
/// world holds against what it last reported.
///
/// <para>
/// <b>Why a diff and not event subscriptions.</b> The engine used to translate five domain
/// events into render commands from inside its composition root, which is one of the reasons it
/// had to name the game's event types. Moving those subscriptions here looked like the obvious
/// port, and it does not work: the SDK's subscribe is gated on an owner-namespaced capability,
/// engine-owned event types resolve to the "kernel" owner, and the kernel-provided token set has
/// been empty since the capability ledger became owner-scoped — so the token could never be
/// satisfied. Declaring nothing instead drops into a grace path that logs a warning per call,
/// which at three hundred spawn events and one per colonist step per tick is not a diagnostic,
/// it is noise.
/// </para>
///
/// <para>
/// So the presentation reads the world instead of listening to it. That turns out to be the
/// better shape regardless: a diff cannot miss an event, cannot double-count one, and needs no
/// ordering guarantees between the system that moves a colonist and the system that draws it. It
/// reports what IS rather than what happened, which is what a renderer wants.
/// </para>
///
/// <para>
/// <b>Colonists only, deliberately.</b> The span walked is the identity component, not position,
/// so items are not reported. That preserves exactly what shipped before this wave: the engine
/// emitted an item-spawned command for every item, and the renderer's handler for it was an
/// empty stub, so items have never been drawn. Widening the diff to all positioned entities
/// would have silently added 255 sprites and called it a port.
/// </para>
/// </summary>
[TickRate(TickRates.NORMAL)]
[SystemAccess(
    reads: new[] { typeof(IdentityComponent), typeof(PositionComponent) },
    writes: new Type[0])]
public sealed class PawnPresentationSystem : ISimulationSystem
{
    private readonly Dictionary<EntityId, GridVector> _reported = new();
    private readonly List<EntityId> _seen = new();
    private readonly List<EntityId> _gone = new();

    // Held so OnDispose can retract. ISimulationSystem.OnDispose() is parameterless, so a system
    // that leaves presentation state behind has no way to clean it up unless it keeps the
    // reference itself. Safe for the same reason it is safe in the weather mod: the presentation
    // members touch no world state -- they hand a command to the engine's sink -- so nothing
    // reachable through this field can go stale. Do NOT copy this pattern for component access.
    private ISystemContext? _presentation;

    public void Initialize(ISystemContext context)
    {
        // Assigned on every graph REBUILD, not once per instance. Re-assignment is harmless here:
        // unlike the weather mod this system subscribes to nothing, so it needs no guard latch.
        _presentation = context;
    }

    public void Tick(ISystemContext context)
    {
        _seen.Clear();

        // Both spans are released before any presentation call: the sink enqueues onto a
        // concurrent queue and touches no world state, but holding a read lease across work that
        // does not need it is the habit that turns into a mutation-while-live defect later.
        var current = new List<(EntityId Entity, GridVector Position)>();
        using (SpanScope<IdentityComponent> identities = context.AcquireSpan<IdentityComponent>())
        {
            foreach ((EntityId entity, IdentityComponent _) in identities.Pairs)
                _seen.Add(entity);
        }

        for (int i = 0; i < _seen.Count; i++)
        {
            if (context.TryGetComponent(_seen[i], out PositionComponent position))
                current.Add((_seen[i], position.Position));
        }

        for (int i = 0; i < current.Count; i++)
        {
            (EntityId entity, GridVector position) = current[i];
            if (!_reported.TryGetValue(entity, out GridVector last))
            {
                context.ShowEntitySprite(entity, position.X, position.Y);
                _reported[entity] = position;
            }
            else if (last.X != position.X || last.Y != position.Y)
            {
                context.MoveEntitySprite(entity, position.X, position.Y);
                _reported[entity] = position;
            }
        }

        if (_reported.Count == current.Count) return;

        // Something left. Rebuilding the live set costs one pass and only runs on the ticks where
        // the counts actually disagree, which for a colony is rare.
        var live = new HashSet<EntityId>();
        for (int i = 0; i < current.Count; i++) live.Add(current[i].Entity);

        _gone.Clear();
        foreach (EntityId reported in _reported.Keys)
        {
            if (!live.Contains(reported)) _gone.Add(reported);
        }

        for (int i = 0; i < _gone.Count; i++)
        {
            context.HideEntitySprite(_gone[i]);
            _reported.Remove(_gone[i]);
        }
    }

    /// <summary>
    /// Releases every sprite this system reported.
    ///
    /// <para>
    /// This used to clear the dictionary and nothing else, while its own summary said what it
    /// ought to do — the comment stated the obligation and the body did not carry it out. Forgetting
    /// the ids locally does not retract anything: the renderer holds its own registration per
    /// entity, so unloading the scenario left the whole colony drawn on a scene the simulation had
    /// stopped maintaining, with nothing left running that could ever notice them gone.
    /// </para>
    ///
    /// <para>
    /// The mod cannot reach the world at dispose time, but it does not need to: it knows exactly
    /// what it told the renderer, because that is what the dictionary is. The null check is
    /// load-bearing rather than defensive — the sink accessor throws when no sink is installed,
    /// and although the unload chain swallows a throw here best-effort, one escaping mid-loop
    /// would skip every remaining retraction.
    /// </para>
    /// </summary>
    public void OnDispose()
    {
        if (_presentation is not null)
        {
            foreach (EntityId reported in _reported.Keys)
                _presentation.HideEntitySprite(reported);
            _presentation = null;
        }

        _reported.Clear();
    }
}

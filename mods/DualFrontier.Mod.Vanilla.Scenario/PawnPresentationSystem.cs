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

    public void Initialize(ISystemContext context)
    {
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
    /// Releases every sprite this system reported. Without it, unloading the scenario would leave
    /// the renderer drawing a colony the world no longer has — the mod cannot reach the world at
    /// dispose time, but it can and must retract what it told the renderer.
    /// </summary>
    public void OnDispose() => _reported.Clear();
}

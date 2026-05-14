using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Moves pawns one tile per tick toward their target.
/// Picks random wander target when idle.
/// Publishes PawnMovedEvent after each step.
/// Phase 3. Tick: NORMAL.
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(PositionComponent), typeof(MovementComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class MovementSystem : SystemBase
{
    private const int StepCooldownTicks = 3;
    private const int MapWidth  = 200;
    private const int MapHeight = 200;

    private readonly IPathfindingService _pathfinding;
    private readonly Random _rng = new(42);

    public MovementSystem(IPathfindingService pathfinding)
        => _pathfinding = pathfinding ?? throw new ArgumentNullException(nameof(pathfinding));

    protected override void OnInitialize()
    {
        // M8.5 — ConsumeSystem decides where the pawn must travel for
        // food / water but cannot write MovementComponent (single-writer
        // invariant). Both events are dispatched at the phase boundary
        // with this system's captured context, which permits the writes.
        Services.Pawns.Subscribe<PawnConsumeTargetEvent>(OnConsumeTarget);
        Services.Pawns.Subscribe<PawnConsumeFinishedEvent>(OnConsumeFinished);

        // M8.6 — SleepSystem mirrors the same pattern for the sleep loop.
        Services.Pawns.Subscribe<PawnSleepTargetEvent>(OnSleepTarget);
        Services.Pawns.Subscribe<PawnSleepFinishedEvent>(OnSleepFinished);
    }

    // K8.3+K8.4 Phase 4 dual-write helpers — legacy mirror removed Phase 5 commit 21.
    private void WriteMove(EntityId entity, MovementComponent move)
    {
        using (var batch = NativeWorld.BeginBatch<MovementComponent>())
            batch.Update(entity, move);
        SetComponent(entity, move);
    }

    private void WritePos(EntityId entity, PositionComponent pos)
    {
        using (var batch = NativeWorld.BeginBatch<PositionComponent>())
            batch.Update(entity, pos);
        SetComponent(entity, pos);
    }

    private void OnConsumeTarget(PawnConsumeTargetEvent evt)
    {
        var move = GetComponent<MovementComponent>(evt.PawnId);
        move.Target = evt.TargetTile;
        move.HasTarget = true;
        // Drop any stale wander path so the next Update repaths toward the
        // newly assigned consume target instead of finishing the old route.
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    private void OnConsumeFinished(PawnConsumeFinishedEvent evt)
    {
        var move = GetComponent<MovementComponent>(evt.PawnId);
        move.HasTarget = false;
        move.Target = default;
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    private void OnSleepTarget(PawnSleepTargetEvent evt)
    {
        var move = GetComponent<MovementComponent>(evt.PawnId);
        move.Target = evt.TargetTile;
        move.HasTarget = true;
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    private void OnSleepFinished(PawnSleepFinishedEvent evt)
    {
        var move = GetComponent<MovementComponent>(evt.PawnId);
        move.HasTarget = false;
        move.Target = default;
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    public override void Update(float delta)
    {
        foreach (var entity in Query<MovementComponent>())
        {
            var pos  = GetComponent<PositionComponent>(entity);
            var move = GetComponent<MovementComponent>(entity);

            if (move.StepCooldown > 0)
            {
                move.StepCooldown--;
                WriteMove(entity, move);
                continue;
            }

            int pathCount = move.Path.IsValid ? move.Path.CountFor(entity) : 0;
            if (move.PathStepIndex >= pathCount)
            {
                GridVector target;
                bool isExternalTarget;

                if (move.HasTarget)
                {
                    target = move.Target;
                    isExternalTarget = true;

                    // If pawn is already at target, do nothing — wait for an
                    // arrival-handler system (M8.5 ConsumeSystem) to clear
                    // Target on this tick. Pathfinding from start==end returns
                    // an empty path which would otherwise loop indefinitely.
                    if (target.X == pos.Position.X && target.Y == pos.Position.Y)
                    {
                        WriteMove(entity, move);
                        continue;
                    }
                }
                else
                {
                    target = new GridVector(
                        _rng.Next(0, MapWidth),
                        _rng.Next(0, MapHeight));
                    isExternalTarget = false;
                }

                if (_pathfinding.TryFindPath(pos.Position, target, out var path)
                    && path.Count > 0)
                {
                    move.Target = target;
                    move.HasTarget = true;
                    if (!move.Path.IsValid)
                        move.Path = NativeWorld.CreateComposite<GridVector>();
                    else
                        move.Path.ClearFor(entity);
                    foreach (GridVector step in path)
                        move.Path.Add(entity, step);
                    move.PathStepIndex = 0;
                }
                else if (isExternalTarget)
                {
                    // External target unreachable — clear so caller (e.g.
                    // ConsumeSystem) can retarget on a later tick instead of
                    // the pawn freezing in place.
                    move.HasTarget = false;
                    move.Target = default;
                }

                WriteMove(entity, move);
                continue;
            }

            move.Path.TryGetAt(entity, move.PathStepIndex, out GridVector next);
            move.PathStepIndex++;
            pos.Position = next;

            move.StepCooldown = StepCooldownTicks;

            WritePos(entity, pos);
            WriteMove(entity, move);

            Services.Pawns.Publish(new PawnMovedEvent
            {
                PawnId = entity,
                X      = next.X,
                Y      = next.Y
            });
        }
    }

    private static void ResetPath(ref MovementComponent move, EntityId entity)
    {
        if (move.Path.IsValid) move.Path.ClearFor(entity);
        move.PathStepIndex = 0;
    }
}

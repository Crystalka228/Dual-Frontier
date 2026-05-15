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
        Services.Pawns.Subscribe<PawnConsumeTargetEvent>(OnConsumeTarget);
        Services.Pawns.Subscribe<PawnConsumeFinishedEvent>(OnConsumeFinished);

        Services.Pawns.Subscribe<PawnSleepTargetEvent>(OnSleepTarget);
        Services.Pawns.Subscribe<PawnSleepFinishedEvent>(OnSleepFinished);
    }

    private void OnConsumeTarget(PawnConsumeTargetEvent evt)
    {
        if (!NativeWorld.TryGetComponent<MovementComponent>(evt.PawnId, out MovementComponent move)) return;
        move.Target = evt.TargetTile;
        move.HasTarget = true;
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    private void OnConsumeFinished(PawnConsumeFinishedEvent evt)
    {
        if (!NativeWorld.TryGetComponent<MovementComponent>(evt.PawnId, out MovementComponent move)) return;
        move.HasTarget = false;
        move.Target = default;
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    private void OnSleepTarget(PawnSleepTargetEvent evt)
    {
        if (!NativeWorld.TryGetComponent<MovementComponent>(evt.PawnId, out MovementComponent move)) return;
        move.Target = evt.TargetTile;
        move.HasTarget = true;
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    private void OnSleepFinished(PawnSleepFinishedEvent evt)
    {
        if (!NativeWorld.TryGetComponent<MovementComponent>(evt.PawnId, out MovementComponent move)) return;
        move.HasTarget = false;
        move.Target = default;
        ResetPath(ref move, evt.PawnId);
        WriteMove(evt.PawnId, move);
    }

    private void WriteMove(EntityId entity, MovementComponent move)
    {
        using WriteBatch<MovementComponent> batch = NativeWorld.BeginBatch<MovementComponent>();
        batch.Update(entity, move);
    }

    public override void Update(float delta)
    {
        // Snapshot the MovementComponent span and per-entity PositionComponent
        // reads, then apply (Position, Movement) writes via two batches once
        // the lease is released. Two BeginBatch scopes are needed because
        // each batch is single-type.
        var movePending = new List<(EntityId Entity, MovementComponent Move)>();
        var posPending = new List<(EntityId Entity, PositionComponent Pos)>();
        var movedEvents = new List<PawnMovedEvent>();

        var snapshot = new List<(EntityId Entity, MovementComponent Move, PositionComponent Pos)>();
        using (SpanLease<MovementComponent> moves = NativeWorld.AcquireSpan<MovementComponent>())
        {
            ReadOnlySpan<MovementComponent> moveSpan = moves.Span;
            ReadOnlySpan<int> moveIndices = moves.Indices;
            for (int i = 0; i < moves.Count; i++)
            {
                var entity = new EntityId(moveIndices[i], 0);
                if (!NativeWorld.TryGetComponent<PositionComponent>(entity, out PositionComponent pos)) continue;
                snapshot.Add((entity, moveSpan[i], pos));
            }
        }

        foreach ((EntityId entity, MovementComponent moveOriginal, PositionComponent posOriginal) in snapshot)
        {
            MovementComponent move = moveOriginal;
            PositionComponent pos = posOriginal;

            if (move.StepCooldown > 0)
            {
                move.StepCooldown--;
                movePending.Add((entity, move));
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

                    if (target.X == pos.Position.X && target.Y == pos.Position.Y)
                    {
                        movePending.Add((entity, move));
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
                    move.HasTarget = false;
                    move.Target = default;
                }

                movePending.Add((entity, move));
                continue;
            }

            move.Path.TryGetAt(entity, move.PathStepIndex, out GridVector next);
            move.PathStepIndex++;
            pos.Position = next;
            move.StepCooldown = StepCooldownTicks;

            posPending.Add((entity, pos));
            movePending.Add((entity, move));
            movedEvents.Add(new PawnMovedEvent
            {
                PawnId = entity,
                X      = next.X,
                Y      = next.Y,
            });
        }

        if (posPending.Count > 0)
        {
            using WriteBatch<PositionComponent> batch = NativeWorld.BeginBatch<PositionComponent>();
            foreach ((EntityId entity, PositionComponent pos) in posPending)
                batch.Update(entity, pos);
        }
        if (movePending.Count > 0)
        {
            using WriteBatch<MovementComponent> batch = NativeWorld.BeginBatch<MovementComponent>();
            foreach ((EntityId entity, MovementComponent move) in movePending)
                batch.Update(entity, move);
        }
        foreach (PawnMovedEvent evt in movedEvents)
            Services.Pawns.Publish(evt);
    }

    private static void ResetPath(ref MovementComponent move, EntityId entity)
    {
        if (move.Path.IsValid) move.Path.ClearFor(entity);
        move.PathStepIndex = 0;
    }
}

using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.ECS;
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
    }

    private void OnConsumeTarget(PawnConsumeTargetEvent evt)
    {
        var move = GetComponent<MovementComponent>(evt.PawnId);
        move.Target = evt.TargetTile;
        // Drop any stale wander path so the next Update repaths toward the
        // newly assigned consume target instead of finishing the old route.
        move.Path.Clear();
        SetComponent(evt.PawnId, move);
    }

    private void OnConsumeFinished(PawnConsumeFinishedEvent evt)
    {
        var move = GetComponent<MovementComponent>(evt.PawnId);
        move.Target = null;
        move.Path.Clear();
        SetComponent(evt.PawnId, move);
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
                SetComponent(entity, move);
                continue;
            }

            if (move.Path.Count == 0)
            {
                GridVector target;
                bool isExternalTarget;

                if (move.Target.HasValue)
                {
                    target = move.Target.Value;
                    isExternalTarget = true;

                    // If pawn is already at target, do nothing — wait for an
                    // arrival-handler system (M8.5 ConsumeSystem) to clear
                    // Target on this tick. Pathfinding from start==end returns
                    // an empty path which would otherwise loop indefinitely.
                    if (target.X == pos.Position.X && target.Y == pos.Position.Y)
                    {
                        SetComponent(entity, move);
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
                    move.Path   = new List<GridVector>(path);
                }
                else if (isExternalTarget)
                {
                    // External target unreachable — clear so caller (e.g.
                    // ConsumeSystem) can retarget on a later tick instead of
                    // the pawn freezing in place.
                    move.Target = null;
                }

                SetComponent(entity, move);
                continue;
            }

            var next = move.Path[0];
            move.Path.RemoveAt(0);
            pos.Position = next;

            move.StepCooldown = StepCooldownTicks;

            SetComponent(entity, pos);
            SetComponent(entity, move);

            Services.Pawns.Publish(new PawnMovedEvent
            {
                PawnId = entity,
                X      = next.X,
                Y      = next.Y
            });
        }
    }
}

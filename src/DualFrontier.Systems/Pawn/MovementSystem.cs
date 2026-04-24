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
    private const int MapWidth  = 50;
    private const int MapHeight = 50;

    private readonly IPathfindingService _pathfinding;
    private readonly Random _rng = new(42);

    public MovementSystem(IPathfindingService pathfinding)
        => _pathfinding = pathfinding ?? throw new ArgumentNullException(nameof(pathfinding));

    protected override void OnInitialize() { }

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
                var target = new GridVector(
                    _rng.Next(0, MapWidth),
                    _rng.Next(0, MapHeight));

                if (_pathfinding.TryFindPath(pos.Position, target, out var path)
                    && path.Count > 0)
                {
                    move.Target = target;
                    move.Path   = new List<GridVector>(path);
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

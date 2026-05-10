using System;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Closes the autonomous consume loop. For pawns with <see cref="JobKind.Eat"/>:
/// (1) if no Job.Target — finds nearest matching consumable based on the
///     pawn's most-critical need, publishes <see cref="PawnConsumeTargetEvent"/>
///     so JobSystem and MovementSystem can set their targets;
/// (2) if at Movement.Target tile — publishes
///     <see cref="NeedsRestoredEvent"/> for restoration AND
///     <see cref="PawnConsumeFinishedEvent"/> so JobSystem clears the job
///     and MovementSystem clears its target. Decrements Charges (for
///     <see cref="ConsumableComponent"/>) in place — that field belongs
///     to ConsumeSystem's own write set.
///
/// Why every cross-component update is event-driven: the dependency graph
/// forbids two systems from declaring write access to the same component.
/// NeedsComponent / JobComponent / MovementComponent each already have a
/// declared owner (NeedsSystem / JobSystem / MovementSystem). The deferred-
/// event pattern moves the writes into those owners' captured contexts at
/// the phase boundary, preserving the single-writer invariant while still
/// letting ConsumeSystem own the "decision to consume" logic.
///
/// Disambiguation: a pawn with both Satiety and Hydration critical seeks the
/// more critical of the two (lower value first). For Hydration,
/// <see cref="WaterSourceComponent"/> is prioritised over
/// <see cref="ConsumableComponent"/> with RestoresKind=Hydration (infinite
/// source, no Charges depletion).
///
/// Filtering: <see cref="ConsumableComponent"/> entities with Charges &lt;= 0
/// are ignored — cosmetic limitation, they remain visible in the world but
/// are functionally inert. M9+ adds a destruction mechanism via IModApi
/// extension.
///
/// Linear scan over all 255 items × 50 pawns each NORMAL tick = 12,750
/// distance checks. SpatialGrid integration deferred to M8.7 where runtime
/// spatial queries actually matter.
/// </summary>
[SystemAccess(
    reads: new[]
    {
        typeof(NeedsComponent),
        typeof(JobComponent),
        typeof(MovementComponent),
        typeof(PositionComponent),
        typeof(WaterSourceComponent),
    },
    writes: new[]
    {
        typeof(ConsumableComponent),
    },
    bus: nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class ConsumeSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (EntityId pawn in Query<JobComponent, NeedsComponent>())
        {
            JobComponent job = GetComponent<JobComponent>(pawn);
            if (job.Current != JobKind.Eat) continue;

            PositionComponent pawnPos = GetComponent<PositionComponent>(pawn);
            MovementComponent move    = GetComponent<MovementComponent>(pawn);
            NeedsComponent needs      = GetComponent<NeedsComponent>(pawn);

            // Phase 2: arrival check (if we have a target and we're standing on it).
            if (job.Target.HasValue && move.HasTarget &&
                pawnPos.Position.X == move.Target.X &&
                pawnPos.Position.Y == move.Target.Y)
            {
                ApplyRestoration(pawn, job.Target.Value);
                Services.Pawns.Publish(new PawnConsumeFinishedEvent { PawnId = pawn });
                continue;
            }

            // Phase 1: target selection (if no Job.Target yet).
            if (!job.Target.HasValue)
            {
                EntityId? target = FindNearestConsumable(pawnPos.Position, needs);
                if (target.HasValue)
                {
                    PositionComponent targetPos = GetComponent<PositionComponent>(target.Value);
                    Services.Pawns.Publish(new PawnConsumeTargetEvent
                    {
                        PawnId     = pawn,
                        Target     = target.Value,
                        TargetTile = targetPos.Position,
                    });
                }
                // else: no consumable available — leave Target null. JobSystem
                // will retry next tick or downgrade to Idle once needs recover.
            }
        }
    }

    /// <summary>
    /// Finds the nearest consumable entity that matches the pawn's most
    /// critical need. For Hydration, <see cref="WaterSourceComponent"/>
    /// is prioritised over <see cref="ConsumableComponent"/> with
    /// RestoresKind=Hydration.
    /// </summary>
    private EntityId? FindNearestConsumable(GridVector pawnPos, NeedsComponent needs)
    {
        bool needsSatiety   = needs.Satiety   <= NeedsComponent.CriticalThreshold;
        bool needsHydration = needs.Hydration <= NeedsComponent.CriticalThreshold;

        bool seekSatiety;
        if (needsSatiety && needsHydration)
            seekSatiety = needs.Satiety <= needs.Hydration;
        else if (needsSatiety)
            seekSatiety = true;
        else if (needsHydration)
            seekSatiety = false;
        else
            return null;

        if (seekSatiety)
            return FindNearestFood(pawnPos);

        EntityId? water = FindNearestWaterSource(pawnPos);
        return water ?? FindNearestPackagedDrink(pawnPos);
    }

    private EntityId? FindNearestFood(GridVector pawnPos)
    {
        EntityId? best = null;
        int bestDist = int.MaxValue;
        foreach (EntityId candidate in Query<ConsumableComponent, PositionComponent>())
        {
            ConsumableComponent c = GetComponent<ConsumableComponent>(candidate);
            if (c.RestoresKind != NeedKind.Satiety) continue;
            if (c.Charges <= 0) continue;

            PositionComponent pos = GetComponent<PositionComponent>(candidate);
            int dist = ChebyshevDistance(pawnPos, pos.Position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = candidate;
            }
        }
        return best;
    }

    private EntityId? FindNearestWaterSource(GridVector pawnPos)
    {
        EntityId? best = null;
        int bestDist = int.MaxValue;
        foreach (EntityId candidate in Query<WaterSourceComponent, PositionComponent>())
        {
            PositionComponent pos = GetComponent<PositionComponent>(candidate);
            int dist = ChebyshevDistance(pawnPos, pos.Position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = candidate;
            }
        }
        return best;
    }

    private EntityId? FindNearestPackagedDrink(GridVector pawnPos)
    {
        EntityId? best = null;
        int bestDist = int.MaxValue;
        foreach (EntityId candidate in Query<ConsumableComponent, PositionComponent>())
        {
            ConsumableComponent c = GetComponent<ConsumableComponent>(candidate);
            if (c.RestoresKind != NeedKind.Hydration) continue;
            if (c.Charges <= 0) continue;

            PositionComponent pos = GetComponent<PositionComponent>(candidate);
            int dist = ChebyshevDistance(pawnPos, pos.Position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = candidate;
            }
        }
        return best;
    }

    /// <summary>
    /// Publishes a <see cref="NeedsRestoredEvent"/> describing the restoration
    /// (NeedsSystem applies it on the same tick at the phase boundary). For
    /// <see cref="ConsumableComponent"/> targets, also decrements Charges in
    /// place — that field belongs to ConsumeSystem's own write set.
    /// </summary>
    private void ApplyRestoration(EntityId pawn, EntityId target)
    {
        foreach (EntityId e in Query<ConsumableComponent>())
        {
            if (e == target)
            {
                ConsumableComponent c = GetComponent<ConsumableComponent>(target);
                Services.Pawns.Publish(new NeedsRestoredEvent
                {
                    PawnId = pawn,
                    Need   = c.RestoresKind,
                    Amount = c.RestorationAmount,
                });
                c.Charges--;
                SetComponent(target, c);
                return;
            }
        }

        foreach (EntityId e in Query<WaterSourceComponent>())
        {
            if (e == target)
            {
                WaterSourceComponent w = GetComponent<WaterSourceComponent>(target);
                Services.Pawns.Publish(new NeedsRestoredEvent
                {
                    PawnId = pawn,
                    Need   = NeedKind.Hydration,
                    Amount = w.RestorationAmount,
                });
                return;
            }
        }
        // Target has neither component — caller already commits to clearing
        // Job.Target on the same tick (via PawnConsumeFinishedEvent); silent
        // no-op preserves the "no fake state" discipline.
    }

    private static int ChebyshevDistance(GridVector a, GridVector b)
    {
        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);
        return Math.Max(dx, dy);
    }
}

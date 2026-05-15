using System;
using DualFrontier.Components.Items;
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
/// Closes the autonomous consume loop. For pawns with <see cref="JobKind.Eat"/>:
/// (1) if no Job.Target — finds nearest matching consumable based on the
///     pawn's most-critical need, publishes <see cref="PawnConsumeTargetEvent"/>;
/// (2) if at Movement.Target tile — publishes
///     <see cref="NeedsRestoredEvent"/> for restoration AND
///     <see cref="PawnConsumeFinishedEvent"/>. Decrements Charges (for
///     <see cref="ConsumableComponent"/>) in place.
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
        // Snapshot Job/Position/Movement/Needs reads first so the inner
        // FindNearest* helpers can open their own AcquireSpan calls without
        // racing this outer iteration. JobComponent span is the iteration
        // anchor; entities without NeedsComponent are skipped.
        var pawns = new System.Collections.Generic.List<(EntityId Pawn, JobComponent Job, PositionComponent Pos, MovementComponent Move, NeedsComponent Needs)>();
        using (SpanLease<JobComponent> jobs = NativeWorld.AcquireSpan<JobComponent>())
        {
            ReadOnlySpan<JobComponent> jobSpan = jobs.Span;
            ReadOnlySpan<int> jobIndices = jobs.Indices;
            for (int i = 0; i < jobs.Count; i++)
            {
                JobComponent job = jobSpan[i];
                if (job.Current != JobKind.Eat) continue;
                var pawn = new EntityId(jobIndices[i], 0);
                if (!NativeWorld.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)) continue;
                if (!NativeWorld.TryGetComponent<PositionComponent>(pawn, out PositionComponent pos)) continue;
                if (!NativeWorld.TryGetComponent<MovementComponent>(pawn, out MovementComponent move)) continue;
                pawns.Add((pawn, job, pos, move, needs));
            }
        }

        foreach ((EntityId pawn, JobComponent job, PositionComponent pawnPos, MovementComponent move, NeedsComponent needs) in pawns)
        {
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
                if (target.HasValue
                    && NativeWorld.TryGetComponent<PositionComponent>(target.Value, out PositionComponent targetPos))
                {
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
        using SpanLease<ConsumableComponent> consumables = NativeWorld.AcquireSpan<ConsumableComponent>();
        ReadOnlySpan<ConsumableComponent> consumableSpan = consumables.Span;
        ReadOnlySpan<int> consumableIndices = consumables.Indices;
        for (int i = 0; i < consumables.Count; i++)
        {
            ConsumableComponent c = consumableSpan[i];
            if (c.RestoresKind != NeedKind.Satiety) continue;
            if (c.Charges <= 0) continue;

            var candidate = new EntityId(consumableIndices[i], 0);
            if (!NativeWorld.TryGetComponent<PositionComponent>(candidate, out PositionComponent pos)) continue;
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
        using SpanLease<WaterSourceComponent> waters = NativeWorld.AcquireSpan<WaterSourceComponent>();
        ReadOnlySpan<int> waterIndices = waters.Indices;
        for (int i = 0; i < waters.Count; i++)
        {
            var candidate = new EntityId(waterIndices[i], 0);
            if (!NativeWorld.TryGetComponent<PositionComponent>(candidate, out PositionComponent pos)) continue;
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
        using SpanLease<ConsumableComponent> consumables = NativeWorld.AcquireSpan<ConsumableComponent>();
        ReadOnlySpan<ConsumableComponent> consumableSpan = consumables.Span;
        ReadOnlySpan<int> consumableIndices = consumables.Indices;
        for (int i = 0; i < consumables.Count; i++)
        {
            ConsumableComponent c = consumableSpan[i];
            if (c.RestoresKind != NeedKind.Hydration) continue;
            if (c.Charges <= 0) continue;

            var candidate = new EntityId(consumableIndices[i], 0);
            if (!NativeWorld.TryGetComponent<PositionComponent>(candidate, out PositionComponent pos)) continue;
            int dist = ChebyshevDistance(pawnPos, pos.Position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = candidate;
            }
        }
        return best;
    }

    private void ApplyRestoration(EntityId pawn, EntityId target)
    {
        if (NativeWorld.TryGetComponent<ConsumableComponent>(target, out ConsumableComponent c))
        {
            Services.Pawns.Publish(new NeedsRestoredEvent
            {
                PawnId = pawn,
                Need   = c.RestoresKind,
                Amount = c.RestorationAmount,
            });
            c.Charges--;
            using WriteBatch<ConsumableComponent> batch = NativeWorld.BeginBatch<ConsumableComponent>();
            batch.Update(target, c);
            return;
        }

        if (NativeWorld.TryGetComponent<WaterSourceComponent>(target, out WaterSourceComponent w))
        {
            Services.Pawns.Publish(new NeedsRestoredEvent
            {
                PawnId = pawn,
                Need   = NeedKind.Hydration,
                Amount = w.RestorationAmount,
            });
        }
        // Target has neither component — silent no-op preserves the
        // "no fake state" discipline.
    }

    private static int ChebyshevDistance(GridVector a, GridVector b)
    {
        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);
        return Math.Max(dx, dy);
    }
}

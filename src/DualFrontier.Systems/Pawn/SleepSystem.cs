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
/// Multi-tick sleep state machine on <see cref="BedComponent"/>. 3-phase
/// tick: sleeping (active sleepers restore needs, possibly wake) → arrival
/// (just-arrived pawns claim beds) → targeting (Sleep-job pawns без target
/// find nearest bed).
/// <para>
/// Hybrid restoration formula (master plan AD-3): pawn occupying bed
/// restores Sleep at <see cref="BedComponent.SleepRestorationPerTick"/>
/// per NORMAL tick, plus Comfort at 30% of that rate. The two needs are
/// updated via two <see cref="NeedsRestoredEvent"/> publications which
/// route through <c>NeedsSystem</c>'s captured context — the only system
/// allowed to write <see cref="NeedsComponent"/>.
/// </para>
/// <para>
/// Single-writer of <see cref="BedComponent"/> at runtime; <c>ItemFactory</c>
/// writes at bootstrap-time only.
/// </para>
/// </summary>
[SystemAccess(
    reads: new[]
    {
        typeof(BedComponent),
        typeof(NeedsComponent),
        typeof(JobComponent),
        typeof(PositionComponent),
    },
    writes: new[] { typeof(BedComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class SleepSystem : SystemBase
{
    private const float WakeThreshold       = 0.95f;
    private const float ComfortToSleepRatio = 0.3f;

    public override void Update(float delta)
    {
        // Phase 1: Sleeping — process active sleepers (occupied beds).
        foreach (EntityId bed in Query<BedComponent>())
        {
            BedComponent bedComp = GetComponent<BedComponent>(bed);
            if (!bedComp.Occupant.HasValue) continue;

            EntityId pawn = bedComp.Occupant.Value;
            NeedsComponent needs = GetComponent<NeedsComponent>(pawn);

            // Wake check using current Sleep value.
            if (needs.Sleep >= WakeThreshold)
            {
                bedComp.Occupant = null;
                SetComponent(bed, bedComp);
                Services.Pawns.Publish(new PawnSleepFinishedEvent
                {
                    PawnId = pawn,
                });
                continue;
            }

            // Apply restoration: hybrid formula ΔSleep + ΔComfort × 0.3.
            Services.Pawns.Publish(new NeedsRestoredEvent
            {
                PawnId = pawn,
                Need   = NeedKind.Sleep,
                Amount = bedComp.SleepRestorationPerTick,
            });
            Services.Pawns.Publish(new NeedsRestoredEvent
            {
                PawnId = pawn,
                Need   = NeedKind.Comfort,
                Amount = bedComp.SleepRestorationPerTick * ComfortToSleepRatio,
            });
        }

        // Phase 2: Arrival/Claim — pawns с Sleep job AND target reached.
        foreach (EntityId pawn in Query<JobComponent, PositionComponent>())
        {
            JobComponent job = GetComponent<JobComponent>(pawn);
            if (job.Current != JobKind.Sleep || !job.Target.HasValue) continue;

            // If this pawn just crossed the wake threshold, Phase 1 above
            // already released the bed and queued PawnSleepFinishedEvent —
            // don't reclaim before the event flushes at the phase boundary.
            NeedsComponent needs = GetComponent<NeedsComponent>(pawn);
            if (needs.Sleep >= WakeThreshold) continue;

            EntityId bed = job.Target.Value;
            BedComponent bedComp = GetComponent<BedComponent>(bed);

            // Already in this bed? Skip — the sleeping phase above handles it.
            if (bedComp.Occupant.HasValue
                && bedComp.Occupant.Value.Index   == pawn.Index
                && bedComp.Occupant.Value.Version == pawn.Version) continue;

            PositionComponent pawnPos = GetComponent<PositionComponent>(pawn);
            PositionComponent bedPos  = GetComponent<PositionComponent>(bed);

            bool atBed = pawnPos.Position.X == bedPos.Position.X
                      && pawnPos.Position.Y == bedPos.Position.Y;

            if (!atBed) continue;

            if (bedComp.Occupant.HasValue)
            {
                // Race: another pawn claimed first. Abort this pawn's job
                // so JobSystem reassigns next tick.
                Services.Pawns.Publish(new PawnSleepFinishedEvent
                {
                    PawnId = pawn,
                });
                continue;
            }

            // Claim bed (sole runtime writer).
            bedComp.Occupant = pawn;
            SetComponent(bed, bedComp);
        }

        // Phase 3: Targeting — pawns с Sleep job AND no Job.Target.
        foreach (EntityId pawn in Query<JobComponent, PositionComponent>())
        {
            JobComponent job = GetComponent<JobComponent>(pawn);
            if (job.Current != JobKind.Sleep) continue;
            if (job.Target.HasValue) continue;

            PositionComponent pawnPos = GetComponent<PositionComponent>(pawn);
            EntityId? target = FindNearestUnoccupiedBed(pawnPos.Position);
            if (!target.HasValue) continue;

            PositionComponent bedPos = GetComponent<PositionComponent>(target.Value);

            Services.Pawns.Publish(new PawnSleepTargetEvent
            {
                PawnId     = pawn,
                Target     = target.Value,
                TargetTile = bedPos.Position,
            });
        }
    }

    private EntityId? FindNearestUnoccupiedBed(GridVector pawnPos)
    {
        EntityId? bestBed = null;
        int bestDistance = int.MaxValue;

        foreach (EntityId bed in Query<BedComponent, PositionComponent>())
        {
            BedComponent bedComp = GetComponent<BedComponent>(bed);
            if (bedComp.Occupant.HasValue) continue;

            PositionComponent pos = GetComponent<PositionComponent>(bed);
            int dist = ChebyshevDistance(pawnPos, pos.Position);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestBed = bed;
            }
        }

        return bestBed;
    }

    private static int ChebyshevDistance(GridVector a, GridVector b)
    {
        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);
        return Math.Max(dx, dy);
    }
}

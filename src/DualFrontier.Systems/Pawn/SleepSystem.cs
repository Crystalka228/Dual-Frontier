using System;
using System.Collections.Generic;
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
/// Multi-tick sleep state machine on <see cref="BedComponent"/>. 3-phase
/// tick: sleeping (active sleepers restore needs, possibly wake) → arrival
/// (just-arrived pawns claim beds) → targeting (Sleep-job pawns without
/// target find nearest bed).
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
        var bedWrites = new List<(EntityId Bed, BedComponent State)>();
        var sleepFinishedPublications = new List<EntityId>();
        var restorationPublications = new List<NeedsRestoredEvent>();
        var sleepTargetPublications = new List<PawnSleepTargetEvent>();

        // Phase 1: Sleeping — process active sleepers (occupied beds).
        // Snapshot the whole BedComponent span; reads of NeedsComponent for
        // the occupant pawn are independent P/Invokes and don't conflict
        // with the lease.
        var bedsSnapshot = new List<(EntityId Bed, BedComponent State)>();
        using (SpanLease<BedComponent> beds = NativeWorld.AcquireSpan<BedComponent>())
        {
            ReadOnlySpan<BedComponent> bedSpan = beds.Span;
            ReadOnlySpan<int> bedIndices = beds.Indices;
            for (int i = 0; i < beds.Count; i++)
            {
                bedsSnapshot.Add((new EntityId(bedIndices[i], 0), bedSpan[i]));
            }
        }

        foreach ((EntityId bed, BedComponent bedCompOriginal) in bedsSnapshot)
        {
            BedComponent bedComp = bedCompOriginal;
            if (!bedComp.Occupant.HasValue) continue;

            EntityId pawn = bedComp.Occupant.Value;
            if (!NativeWorld.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)) continue;

            if (needs.Sleep >= WakeThreshold)
            {
                bedComp.Occupant = null;
                bedWrites.Add((bed, bedComp));
                sleepFinishedPublications.Add(pawn);
                continue;
            }

            restorationPublications.Add(new NeedsRestoredEvent
            {
                PawnId = pawn,
                Need   = NeedKind.Sleep,
                Amount = bedComp.SleepRestorationPerTick,
            });
            restorationPublications.Add(new NeedsRestoredEvent
            {
                PawnId = pawn,
                Need   = NeedKind.Comfort,
                Amount = bedComp.SleepRestorationPerTick * ComfortToSleepRatio,
            });
        }

        // Phase 2: Arrival/Claim — pawns with Sleep job AND target reached.
        // Snapshot the JobComponent span; per-pawn reads of NeedsComponent,
        // PositionComponent, BedComponent are independent.
        var jobSnapshot = new List<(EntityId Pawn, JobComponent Job)>();
        using (SpanLease<JobComponent> jobs = NativeWorld.AcquireSpan<JobComponent>())
        {
            ReadOnlySpan<JobComponent> jobSpan = jobs.Span;
            ReadOnlySpan<int> jobIndices = jobs.Indices;
            for (int i = 0; i < jobs.Count; i++)
            {
                jobSnapshot.Add((new EntityId(jobIndices[i], 0), jobSpan[i]));
            }
        }

        foreach ((EntityId pawn, JobComponent job) in jobSnapshot)
        {
            if (job.Current != JobKind.Sleep || !job.Target.HasValue) continue;
            if (!NativeWorld.TryGetComponent<PositionComponent>(pawn, out PositionComponent pawnPos)) continue;

            if (NativeWorld.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
                && needs.Sleep >= WakeThreshold) continue;

            EntityId bed = job.Target.Value;
            if (!NativeWorld.TryGetComponent<BedComponent>(bed, out BedComponent bedComp)) continue;

            if (bedComp.Occupant.HasValue
                && bedComp.Occupant.Value.Index   == pawn.Index
                && bedComp.Occupant.Value.Version == pawn.Version) continue;

            if (!NativeWorld.TryGetComponent<PositionComponent>(bed, out PositionComponent bedPos)) continue;

            bool atBed = pawnPos.Position.X == bedPos.Position.X
                      && pawnPos.Position.Y == bedPos.Position.Y;
            if (!atBed) continue;

            if (bedComp.Occupant.HasValue)
            {
                sleepFinishedPublications.Add(pawn);
                continue;
            }

            bedComp.Occupant = pawn;
            bedWrites.Add((bed, bedComp));
        }

        // Phase 3: Targeting — pawns with Sleep job AND no Job.Target.
        foreach ((EntityId pawn, JobComponent job) in jobSnapshot)
        {
            if (job.Current != JobKind.Sleep) continue;
            if (job.Target.HasValue) continue;
            if (!NativeWorld.TryGetComponent<PositionComponent>(pawn, out PositionComponent pawnPos)) continue;

            EntityId? target = FindNearestUnoccupiedBed(pawnPos.Position);
            if (!target.HasValue) continue;

            if (!NativeWorld.TryGetComponent<PositionComponent>(target.Value, out PositionComponent bedPos)) continue;

            sleepTargetPublications.Add(new PawnSleepTargetEvent
            {
                PawnId     = pawn,
                Target     = target.Value,
                TargetTile = bedPos.Position,
            });
        }

        // Single batch for all BedComponent writes accumulated above.
        if (bedWrites.Count > 0)
        {
            using WriteBatch<BedComponent> batch = NativeWorld.BeginBatch<BedComponent>();
            foreach ((EntityId bed, BedComponent state) in bedWrites)
                batch.Update(bed, state);
        }

        foreach (NeedsRestoredEvent evt in restorationPublications)
            Services.Pawns.Publish(evt);
        foreach (EntityId pawn in sleepFinishedPublications)
            Services.Pawns.Publish(new PawnSleepFinishedEvent { PawnId = pawn });
        foreach (PawnSleepTargetEvent evt in sleepTargetPublications)
            Services.Pawns.Publish(evt);
    }

    private EntityId? FindNearestUnoccupiedBed(GridVector pawnPos)
    {
        EntityId? bestBed = null;
        int bestDistance = int.MaxValue;

        using SpanLease<BedComponent> beds = NativeWorld.AcquireSpan<BedComponent>();
        ReadOnlySpan<BedComponent> bedSpan = beds.Span;
        ReadOnlySpan<int> bedIndices = beds.Indices;
        for (int i = 0; i < beds.Count; i++)
        {
            BedComponent bedComp = bedSpan[i];
            if (bedComp.Occupant.HasValue) continue;

            var bed = new EntityId(bedIndices[i], 0);
            if (!NativeWorld.TryGetComponent<PositionComponent>(bed, out PositionComponent pos)) continue;
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

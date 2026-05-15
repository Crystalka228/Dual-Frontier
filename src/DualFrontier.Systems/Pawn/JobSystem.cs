using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

[SystemAccess(
    reads:  new[] { typeof(NeedsComponent), typeof(SkillsComponent),
                    typeof(PositionComponent) },
    writes: new[] { typeof(JobComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class JobSystem : SystemBase
{
    private readonly HashSet<EntityId> _urgentPawns = new();

    protected override void OnInitialize()
    {
        Services.Pawns.Subscribe<NeedsCriticalEvent>(OnNeedsCritical);

        // M8.5 — ConsumeSystem owns the consume decision but cannot write
        // JobComponent (single-writer invariant). Both events are dispatched
        // at the phase boundary with this system's captured context, which
        // permits the JobComponent write.
        Services.Pawns.Subscribe<PawnConsumeTargetEvent>(OnConsumeTarget);
        Services.Pawns.Subscribe<PawnConsumeFinishedEvent>(OnConsumeFinished);

        // M8.6 — SleepSystem mirrors the same pattern for the sleep loop.
        Services.Pawns.Subscribe<PawnSleepTargetEvent>(OnSleepTarget);
        Services.Pawns.Subscribe<PawnSleepFinishedEvent>(OnSleepFinished);
    }

    private void OnNeedsCritical(NeedsCriticalEvent evt)
    {
        _urgentPawns.Add(evt.PawnId);
    }

    private void OnConsumeTarget(PawnConsumeTargetEvent evt)
    {
        if (!NativeWorld.TryGetComponent<JobComponent>(evt.PawnId, out JobComponent job)) return;
        job.Target = evt.Target;
        WriteJob(evt.PawnId, job);
    }

    private void OnConsumeFinished(PawnConsumeFinishedEvent evt)
    {
        if (!NativeWorld.TryGetComponent<JobComponent>(evt.PawnId, out JobComponent job)) return;
        job.Current = JobKind.Idle;
        job.Target  = null;
        WriteJob(evt.PawnId, job);
    }

    private void OnSleepTarget(PawnSleepTargetEvent evt)
    {
        if (!NativeWorld.TryGetComponent<JobComponent>(evt.PawnId, out JobComponent job)) return;
        job.Target = evt.Target;
        WriteJob(evt.PawnId, job);
    }

    private void OnSleepFinished(PawnSleepFinishedEvent evt)
    {
        if (!NativeWorld.TryGetComponent<JobComponent>(evt.PawnId, out JobComponent job)) return;
        job.Current = JobKind.Idle;
        job.Target  = null;
        WriteJob(evt.PawnId, job);
    }

    private void WriteJob(EntityId pawn, JobComponent job)
    {
        using WriteBatch<JobComponent> batch = NativeWorld.BeginBatch<JobComponent>();
        batch.Update(pawn, job);
    }

    public override void Update(float delta)
    {
        HashSet<EntityId>? urgentSnapshot = null;
        if (_urgentPawns.Count > 0)
        {
            urgentSnapshot = new HashSet<EntityId>(_urgentPawns);
            _urgentPawns.Clear();
        }

        var pendingWrites = new List<(EntityId Entity, JobComponent Job)>();

        using (SpanLease<JobComponent> jobs = NativeWorld.AcquireSpan<JobComponent>())
        {
            ReadOnlySpan<JobComponent> jobSpan = jobs.Span;
            ReadOnlySpan<int> jobIndices = jobs.Indices;

            for (int i = 0; i < jobs.Count; i++)
            {
                var entity = new EntityId(jobIndices[i], 0);
                if (!NativeWorld.TryGetComponent<NeedsComponent>(entity, out NeedsComponent needs))
                    continue;

                JobComponent job = jobSpan[i];

                bool isUrgent = urgentSnapshot != null
                             && urgentSnapshot.Contains(entity);

                if (!isUrgent && job.Current != JobKind.Idle)
                    continue;

                JobKind next = PickJob(needs);
                if (next == job.Current)
                    continue;

                job.Current = next;
                pendingWrites.Add((entity, job));
            }
        }

        if (pendingWrites.Count > 0)
        {
            using WriteBatch<JobComponent> batch = NativeWorld.BeginBatch<JobComponent>();
            foreach ((EntityId entity, JobComponent job) in pendingWrites)
                batch.Update(entity, job);
        }
    }

    private static JobKind PickJob(NeedsComponent needs)
    {
        if (needs.Satiety   <= NeedsComponent.CriticalThreshold) return JobKind.Eat;
        if (needs.Hydration <= NeedsComponent.CriticalThreshold) return JobKind.Eat;
        if (needs.Sleep     <= NeedsComponent.CriticalThreshold) return JobKind.Sleep;
        return JobKind.Idle;
    }
}

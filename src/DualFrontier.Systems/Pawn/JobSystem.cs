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
    // Entities flagged via NeedsCriticalEvent between our ticks. Processed
    // even if their current job is not Idle, so a starving hauler drops the
    // haul to eat rather than waiting for the job to complete.
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

    // K8.3+K8.4 Phase 4 — every JobComponent SetComponent gets a paired
    // NativeWorld.BeginBatch update for the dual-write transition (legacy
    // mirror removed Phase 5 commit 21).
    private void WriteJob(EntityId pawnId, JobComponent job)
    {
        using (var batch = NativeWorld.BeginBatch<JobComponent>())
            batch.Update(pawnId, job);
        SetComponent(pawnId, job);
    }

    private void OnConsumeTarget(PawnConsumeTargetEvent evt)
    {
        var job = GetComponent<JobComponent>(evt.PawnId);
        job.Target = evt.Target;
        WriteJob(evt.PawnId, job);
    }

    private void OnConsumeFinished(PawnConsumeFinishedEvent evt)
    {
        var job = GetComponent<JobComponent>(evt.PawnId);
        job.Current = JobKind.Idle;
        job.Target  = null;
        WriteJob(evt.PawnId, job);
    }

    private void OnSleepTarget(PawnSleepTargetEvent evt)
    {
        var job = GetComponent<JobComponent>(evt.PawnId);
        job.Target = evt.Target;
        WriteJob(evt.PawnId, job);
    }

    private void OnSleepFinished(PawnSleepFinishedEvent evt)
    {
        var job = GetComponent<JobComponent>(evt.PawnId);
        job.Current = JobKind.Idle;
        job.Target  = null;
        WriteJob(evt.PawnId, job);
    }

    public override void Update(float delta)
    {
        HashSet<EntityId>? urgentSnapshot = null;
        if (_urgentPawns.Count > 0)
        {
            urgentSnapshot = new HashSet<EntityId>(_urgentPawns);
            _urgentPawns.Clear();
        }

        foreach (var entity in Query<NeedsComponent, JobComponent>())
        {
            var job = GetComponent<JobComponent>(entity);

            bool isUrgent = urgentSnapshot != null
                         && urgentSnapshot.Contains(entity);

            if (!isUrgent && job.Current != JobKind.Idle)
                continue;

            var needs = GetComponent<NeedsComponent>(entity);
            JobKind next = PickJob(needs);
            if (next == job.Current)
                continue;

            job.Current = next;
            WriteJob(entity, job);
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

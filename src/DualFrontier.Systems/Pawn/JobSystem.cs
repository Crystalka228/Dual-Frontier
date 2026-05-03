using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;
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
    }

    private void OnNeedsCritical(NeedsCriticalEvent evt)
    {
        _urgentPawns.Add(evt.PawnId);
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
            SetComponent(entity, job);
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

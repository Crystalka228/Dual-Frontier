using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

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
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        foreach (var entity in Query<JobComponent>())
        {
            var needs = GetComponent<NeedsComponent>(entity);
            var job   = GetComponent<JobComponent>(entity);

            if (job.Current != JobKind.Idle)
                continue;

            JobKind next = PickJob(needs);
            if (next == job.Current)
                continue;

            job.Current = next;
            SetComponent(entity, job);
        }
    }

    private static JobKind PickJob(NeedsComponent needs)
    {
        if (needs.Hunger >= NeedsComponent.CriticalThreshold) return JobKind.Eat;
        if (needs.Thirst >= NeedsComponent.CriticalThreshold) return JobKind.Eat;
        if (needs.Rest   >= NeedsComponent.CriticalThreshold) return JobKind.Sleep;
        return JobKind.Idle;
    }
}
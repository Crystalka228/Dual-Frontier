using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Назначение джобов пешкам по приоритетам нужд, навыков и
/// расстояния до цели. Единственная система, которая ПИШЕТ
/// <see cref="JobComponent"/>.
///
/// Фаза: 3 (пешки).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent), typeof(SkillsComponent), typeof(PositionComponent) },
    writes: new[] { typeof(JobComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class JobSystem : SystemBase
{
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        foreach (var entityId in Query<JobComponent, NeedsComponent>())
        {
            var job = GetComponent<JobComponent>(entityId);
            var needs = GetComponent<NeedsComponent>(entityId);

            if (job.IsInterrupted)
            {
                job.Current = JobKind.Idle;
                job.Target = null;
                job.TicksAtJob = 0;
                job.IsInterrupted = false;
            }
            else if (needs.IsExhausted)
            {
                job.Current = JobKind.Sleep;
                job.Target = null;
                job.TicksAtJob = 0;
            }
            else if (needs.IsHungry)
            {
                job.Current = JobKind.Eat;
                job.Target = null;
                job.TicksAtJob = 0;
            }
            else if (job.IsIdle)
            {
                // no change needed
            }
            else
            {
                job.TicksAtJob++;
            }

            SetComponent(entityId, job);
        }
    }
}

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Events.Pawn;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Назначение джобов пешкам по приоритетам нужд. Единственная система,
/// которая ПИШЕТ <see cref="JobComponent"/>. Phase 3 MVP реализует
/// inline priority logic (Sleep > Eat > keep-current); интеграция с
/// behaviour tree и целями жоба отложена до Phase 4.
///
/// Фаза: 3 (пешки).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(NeedsComponent) },
    writes: new[] { typeof(JobComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
public sealed class JobSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entityId in Query<JobComponent, NeedsComponent>())
        {
            var job = GetComponent<JobComponent>(entityId);
            var needs = GetComponent<NeedsComponent>(entityId);

            JobKind previous = job.Current;

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
            else if (!job.IsIdle)
            {
                job.TicksAtJob++;
            }

            SetComponent(entityId, job);

            if (job.Current != previous)
                Services.Pawns.Publish(new JobAssignedEvent
                {
                    PawnId = entityId,
                    Job = job.Current
                });
        }
    }
}

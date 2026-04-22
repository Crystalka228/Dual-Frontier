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
    /// <summary>
    /// TODO: Подписаться на JobCompletedEvent, JobAbortedEvent, MoodBreakEvent.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 3 — подписка на события джобов");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 3 — перебрать свободных пешек, выбрать лучший доступный джоб.
    }
}

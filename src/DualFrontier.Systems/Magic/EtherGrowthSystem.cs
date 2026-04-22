using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Рост эфирных узлов в мире под влиянием доминирующей школы
/// магии в округе. Публикует <c>EtherSurgeEvent</c> при
/// пересечении порога плотности.
///
/// Фаза: 2 (после MapSystem).
/// Тик: SLOW (60 фреймов) — эфир растёт медленно.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(EtherComponent), typeof(SchoolComponent) },
    writes: new[] { typeof(EtherComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.SLOW)]
public sealed class EtherGrowthSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на EtherDrainedEvent (когда голем/заклинание тянут эфир из узла).
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 2 — подписка на события эфирных узлов");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 2 — диффузия/рост эфира по соседним узлам, проверка порогов.
    }
}

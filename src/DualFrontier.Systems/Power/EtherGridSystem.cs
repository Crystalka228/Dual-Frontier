using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Building;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Эфирная сеть: узлы связаны по пространственному радиусу,
/// плотность эфира перетекает между соседними узлами.
/// Кормит <c>ConverterSystem</c> и магические постройки.
///
/// Фаза: 2.
/// Тик: NORMAL (15 фреймов).
/// </summary>
// TODO: когда другой агент добавит EtherNodeComponent в Components.Building/World —
// заменить здесь EtherComponent на EtherNodeComponent.
[SystemAccess(
    reads:  new[] { typeof(EtherComponent), typeof(PowerConsumerComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.NORMAL)]
public sealed class EtherGridSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на EtherNodeChangedEvent, EtherDrainedEvent.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 2 — подписка на события эфирной сети");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 2 — продвинуть перетекание плотностей, распределить потребителям.
    }
}

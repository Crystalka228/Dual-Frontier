using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Combat;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Объединяет несколько <see cref="DamageIntent"/> от разных систем
/// (Physical, Magic, Status) в общий порядок применения урона в одном тике.
/// Гарантирует детерминированную сортировку: по <c>(EntityId, DamageKind ordinal)</c>,
/// после чего последовательно публикует <see cref="DamageEvent"/> для
/// <c>DamageSystem</c>.
///
/// Детерминизм критичен для мультиплеера/реплеев: один и тот же набор
/// намерений в пределах тика должен давать один и тот же порядок
/// применения урона независимо от порядка публикации (TechArch v0.2 §12.4).
///
/// Фаза: 2 (после CombatSystem, SpellSystem, StatusEffectSystem).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(DamageIntent) },
    writes: new[] { typeof(DamageEvent) },
    buses:  new[] { nameof(IGameServices.Combat), nameof(IGameServices.Magic) }
)]
[TickRate(TickRates.NORMAL)]
public sealed class ComboResolutionSystem : SystemBase
{
    /// <summary>
    /// TODO: Фаза 4 — подписаться на DamageIntent из Combat и Magic шин.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 4 — подписка на DamageIntent из Combat и Magic шин");
    }

    /// <summary>
    /// Основной тик: после сбора всех <see cref="DamageIntent"/> текущей фазы
    /// вызывает <see cref="ResolvePending"/>, который сортирует и применяет
    /// урон детерминированно.
    /// </summary>
    public override void Update(float delta)
    {
        // TODO: Фаза 4 — вызвать ResolvePending() один раз за тик.
    }

    /// <summary>
    /// Ставит <paramref name="intent"/> в очередь текущего тика. Применение
    /// будет выполнено в <see cref="ResolvePending"/> после сбора всех
    /// намерений.
    /// </summary>
    /// <param name="intent">Намерение нанести урон от любой системы-источника.</param>
    public void OnDamageIntent(DamageIntent intent)
    {
        throw new NotImplementedException("TODO: Фаза 4 — постановка DamageIntent в очередь тика");
    }

    /// <summary>
    /// Сортирует накопленные <see cref="DamageIntent"/> по
    /// <c>(EntityId, DamageKind ordinal)</c>, применяет их в этом порядке и
    /// публикует итоговые <see cref="DamageEvent"/> в <c>DamageSystem</c>.
    /// После вызова очередь текущего тика очищается.
    /// </summary>
    public void ResolvePending()
    {
        throw new NotImplementedException("TODO: Фаза 4 — детерминированная сортировка и публикация DamageEvent");
    }
}

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Components.Combat;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Combat;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Инициация боевых действий. В модели TechArch v0.2 §12.4 делегирует
/// проверку ресурсов через <see cref="CompoundShotIntent"/> —
/// CompositeResolutionSystem опрашивает Inventory и Magic шины и отвечает
/// <see cref="ShootGranted"/>/<see cref="ShootRefused"/>. CombatSystem
/// больше не публикует <c>AmmoIntent</c> напрямую.
///
/// Работает одновременно с Combat и Magic шинами (манна стрелка — часть
/// составного выстрела).
///
/// Фаза: 1 (параллельно с ManaSystem, WeatherSystem).
/// Тик: FAST (3 фрейма) — отзывчивость боя.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent), typeof(ManaComponent) },
    writes: new[] { typeof(HealthComponent) },
    buses:  new[] { nameof(IGameServices.Combat), nameof(IGameServices.Magic) }
)]
[TickRate(TickRates.FAST)]
public sealed class CombatSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на ShootAttemptEvent, ShootGranted, ShootRefused.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 5 — боевая система");
    }

    public override void Update(float delta)
    {
        // TODO: обработка активных боевых действий
    }

    /// <summary>
    /// Публикует <see cref="CompoundShotIntent"/> для двухфазного коммита
    /// (вместо прямого <c>AmmoIntent</c>, как было в v0.1). Ответ придёт
    /// от <c>CompositeResolutionSystem</c> в виде <see cref="ShootGranted"/>
    /// или <see cref="ShootRefused"/>.
    /// TODO: Фаза 4 — сформировать intent по текущему оружию/цели и
    /// опубликовать в Combat шину.
    /// </summary>
    /// <param name="intent">Готовый intent для публикации (в будущем — параметры
    /// выстрела, из которых intent собирается внутри).</param>
    public void OnCompoundShotIntent(CompoundShotIntent intent)
    {
        throw new NotImplementedException("TODO: Фаза 4 — делегирование выстрела через CompoundShotIntent");
    }
}

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Управление связями "хозяин-голем": поддерживает активную
/// связь за счёт маны хозяина, распадает связь при нехватке.
/// Публикует <c>GolemActivatedEvent</c> / <c>GolemBondBrokenEvent</c>
/// и обрабатывает <see cref="GolemOwnershipTransferRequest"/> (смена
/// владельца) с итоговым отложенным <see cref="GolemOwnershipChanged"/>.
///
/// Фаза: 4 (после ManaSystem).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(GolemBondComponent), typeof(ManaComponent) },
    writes: new[] { typeof(GolemBondComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.NORMAL)]
public sealed class GolemSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на GolemCommandEvent, GolemOwnershipTransferRequest,
    /// ManaGranted, ManaRefused.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 4 — подписка на события големов");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 4 — проверить активные связи, списать поддержку, распад при нехватке.
    }

    /// <summary>
    /// Обрабатывает смену владельца голема: валидирует целевой режим
    /// (<c>OwnershipMode</c>), обновляет <see cref="GolemBondComponent"/>
    /// и публикует <see cref="GolemOwnershipChanged"/>. Событие помечено
    /// <c>[Deferred]</c>, поэтому реальные мутации подписчиков произойдут
    /// на следующем тике.
    /// </summary>
    /// <param name="req">Запрос на передачу/смену владения големом.</param>
    public void OnGolemOwnershipTransferRequest(GolemOwnershipTransferRequest req)
    {
        throw new NotImplementedException("TODO: Фаза 6 — обработка смены владельца голема и публикация GolemOwnershipChanged");
    }

    /// <summary>
    /// Чтобы избежать feedback loop (v02 §12.3): GolemSystem читает состояние
    /// манны за ПРЕДЫДУЩИЙ тик, а не текущий. Реальная реализация —
    /// <c>Mana[N-1]</c> snapshot (buffered-state), чтобы одновременно
    /// работающая ManaSystem не видела изменений, сделанных текущим
    /// тиком GolemSystem, и наоборот.
    /// </summary>
    /// <param name="mage">Маг-хозяин голема, чьё состояние манны читается.</param>
    private float ReadPreviousTickManaState(EntityId mage)
    {
        throw new NotImplementedException("TODO: Фаза 6 — чтение Mana[N-1] snapshot для предотвращения feedback loop");
    }
}

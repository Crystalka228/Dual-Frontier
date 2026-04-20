using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Combat;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Обрабатывает <see cref="CompoundShotIntent"/>: собирает ответы от Inventory
/// и Magic шин по <see cref="TransactionId"/>, публикует <see cref="ShootGranted"/>
/// либо <see cref="ShootRefused"/>.
///
/// Двухфазный коммит: на один <c>CompoundShotIntent</c> должны прийти оба
/// частичных ответа — <see cref="AmmoGranted"/>/<see cref="AmmoRefused"/> из
/// Inventory шины и <see cref="ManaGranted"/>/<see cref="ManaRefused"/> из
/// Magic шины. Только когда оба получены — система принимает решение и
/// публикует итоговый <c>ShootGranted</c>/<c>ShootRefused</c> в Combat шину.
///
/// Фаза: 2 (после CombatSystem, ManaSystem, InventorySystem).
/// Тик: FAST (3 фрейма) — отзывчивость боя.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(AmmoGranted), typeof(AmmoRefused), typeof(ManaGranted), typeof(ManaRefused) },
    writes: new[] { typeof(ShootGranted), typeof(ShootRefused) },
    buses:  new[] { nameof(IGameServices.Combat), nameof(IGameServices.Inventory), nameof(IGameServices.Magic) }
)]
[TickRate(TickRates.FAST)]
public sealed class CompositeResolutionSystem : SystemBase
{
    /// <summary>
    /// TODO: Фаза 4 — подписаться на CompoundShotIntent, AmmoGranted,
    /// AmmoRefused, ManaGranted, ManaRefused.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 4 — подписка на частичные ответы двухфазного коммита");
    }

    /// <summary>
    /// Основной тик: разрешение накопленных незавершённых транзакций.
    /// Большинство работы выполняется реактивно в обработчиках ниже, этот
    /// метод остаётся для периодических таймаут-проверок (см. §12.4).
    /// </summary>
    public override void Update(float delta)
    {
        // TODO: Фаза 4 — очистка просроченных незавершённых транзакций, публикация ShootRefused(TimedOut).
    }

    /// <summary>
    /// Начинает двухфазный commit: сохраняет <paramref name="intent"/> в
    /// таблице ожидания по его <see cref="TransactionId"/> и рассылает
    /// частичные запросы в Inventory и Magic шины.
    /// </summary>
    /// <param name="intent">Намерение составного выстрела, полученное от
    /// CombatSystem.</param>
    public void OnCompoundShotIntent(CompoundShotIntent intent)
    {
        throw new NotImplementedException("TODO: Фаза 4 — инициация двухфазного коммита для CompoundShotIntent");
    }

    /// <summary>
    /// Частичный ответ «патрон выдан» по <see cref="TransactionId"/>.
    /// Сохраняется в таблице ожидания; если второй частичный ответ уже
    /// есть — вызывает <see cref="TryResolve"/>.
    /// </summary>
    /// <param name="evt">Подтверждение выдачи патрона от InventoryBus.</param>
    public void OnAmmoGranted(AmmoGranted evt)
    {
        throw new NotImplementedException("TODO: Фаза 4 — регистрация частичного AmmoGranted по TxId");
    }

    /// <summary>
    /// Кэшируем отказ в выдаче патрона — итоговым результатом транзакции
    /// будет <see cref="ShootRefused"/> с причиной <see cref="ShotRefusalReason.NoAmmo"/>.
    /// </summary>
    /// <param name="evt">Отказ InventoryBus.</param>
    public void OnAmmoRefused(AmmoRefused evt)
    {
        throw new NotImplementedException("TODO: Фаза 4 — регистрация частичного AmmoRefused по TxId");
    }

    /// <summary>
    /// Второй частичный ответ — списание маны подтверждено. Аналогично
    /// <see cref="OnAmmoGranted"/>, при наличии обоих ответов — разрешаем
    /// транзакцию через <see cref="TryResolve"/>.
    /// </summary>
    /// <param name="evt">Подтверждение списания маны от MagicBus.</param>
    public void OnManaGranted(ManaGranted evt)
    {
        throw new NotImplementedException("TODO: Фаза 4 — регистрация частичного ManaGranted по TxId");
    }

    /// <summary>
    /// Кэшируем отказ в списании маны — итоговым результатом транзакции
    /// будет <see cref="ShootRefused"/> с причиной <see cref="ShotRefusalReason.NoMana"/>.
    /// </summary>
    /// <param name="evt">Отказ MagicBus.</param>
    public void OnManaRefused(ManaRefused evt)
    {
        throw new NotImplementedException("TODO: Фаза 4 — регистрация частичного ManaRefused по TxId");
    }

    /// <summary>
    /// Пытается разрешить транзакцию по <paramref name="id"/>: если оба
    /// частичных ответа пришли — публикует <see cref="ShootGranted"/>
    /// (оба Granted) либо <see cref="ShootRefused"/> (любой Refused) и
    /// удаляет запись из таблицы ожидания. Если ответы ещё не оба — no-op.
    /// </summary>
    /// <param name="id">Идентификатор транзакции для разрешения.</param>
    private void TryResolve(TransactionId id)
    {
        throw new NotImplementedException("TODO: Фаза 4 — объединение Ammo* и Mana* ответов в итоговый ShootGranted/ShootRefused");
    }
}

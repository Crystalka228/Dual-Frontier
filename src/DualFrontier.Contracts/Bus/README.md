# Bus — Доменные шины событий

## Назначение
Определяет контракт базовой шины событий `IEventBus` и шесть доменных шин
(Combat, Inventory, Magic, Pawn, Power, World), собранных в агрегатор `IGameServices`.
Разделение на домены снижает lock contention, упрощает дебаг и профилирование
нагрузки на конкретный домен.

## Зависимости
- `DualFrontier.Contracts.Core` (маркер `IEvent`).

## Что внутри
- `IEventBus.cs` — базовый контракт шины одного домена (Publish/Subscribe/Unsubscribe).
- `IGameServices.cs` — агрегатор всех доменных шин, точка доступа для систем.
- `ICombatBus.cs` — шина боя: ShootAttempt, DamageEvent, DeathEvent.
- `IInventoryBus.cs` — шина склада: AmmoRequest/Result, ItemAdded/Removed.
- `IMagicBus.cs` — шина магии: ManaRequest/Result, SpellCast, EtherSurge.
- `IPawnBus.cs` — шина пешек: MoodBreak, DeathReaction, SkillGain.
- `IPowerBus.cs` — шина энергосети: ConverterPowerOutput.
- `IWorldBus.cs` — шина мира: EtherNodeChanged, WeatherChanged, RaidIncoming.

## Правила
- Каждая доменная шина — отдельный экземпляр со своим набором подписок.
- Система пишет только в "свою" шину (декларируется в `[SystemAccess(bus: ...)]`).
- Подписка обязана отписываться при утилизации — иначе утечка памяти.
- Обработчики вызываются синхронно. Не блокировать в обработчике — это
  блокирует всю фазу планировщика.

## Примеры использования
```csharp
public sealed class CombatSystem
{
    private readonly IGameServices _bus;

    public CombatSystem(IGameServices bus) => _bus = bus;

    public void OnShot(EntityId shooter)
        => _bus.Combat.Publish(new ShootAttemptEvent(shooter));
}
```

## TODO
- [x] Фаза 1 — реализован `DomainEventBus` с ConcurrentDictionary подписок.
- [x] Фаза 1 — реализован `GameServices` — композиция шести шин.
- [ ] Фаза 2 — добавить метрики (счётчик событий/сек на шину) для профайлера.

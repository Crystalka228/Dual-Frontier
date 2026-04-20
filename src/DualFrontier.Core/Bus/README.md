# Bus — Реализация доменных шин

## Назначение
Реализация контрактов `IEventBus` и `IGameServices` из
`DualFrontier.Contracts.Bus`. Каждая доменная шина — отдельный экземпляр
`DomainEventBus` со своим набором подписок. `IntentBatcher` используется
системами для реализации двухшаговой модели Intent → Granted/Refused.

## Зависимости
- `DualFrontier.Contracts.Bus` — контракты шин.
- `DualFrontier.Contracts.Core` — маркер `IEvent`.

## Что внутри
- `DomainEventBus.cs` — реализация одной доменной шины с `ConcurrentDictionary`
  подписок для потокобезопасной работы.
- `GameServices.cs` — композиция пяти доменных шин, реализует `IGameServices`.
- `IntentBatcher.cs` — собирает intents в пределах фазы и отдаёт батчем
  обработчику в следующей фазе.

## Правила
- Шина не держит ссылок на сами системы: только на делегаты `Action<T>`.
- Подписка и отписка — потокобезопасные операции (ConcurrentDictionary).
- Обработчики вызываются синхронно. Для длительной работы обработчик обязан
  переложить задачу на IntentBatcher.

## Примеры использования
```csharp
var services = new GameServices();
services.Combat.Publish(new ShootAttemptEvent(shooterId));
services.Inventory.Subscribe<AmmoIntent>(batcher.Collect);
```

## TODO
- [ ] Фаза 1 — реализовать `DomainEventBus` с поддержкой `[Deferred]`/`[Immediate]`.
- [ ] Фаза 1 — реализовать `IntentBatcher` с двухфазным сбором и drain.
- [ ] Фаза 2 — добавить телеметрию (счётчики событий/сек на шину).

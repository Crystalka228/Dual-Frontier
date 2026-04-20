# Power Systems

## Назначение
Сети энергопитания: электрическая сеть, эфирная сеть и
конвертеры между ними. См. GDD раздел 9 "Энергосистемы".

## Зависимости
- `DualFrontier.Contracts` — атрибуты, `IWorldBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Building` — `PowerConsumerComponent`,
  `PowerProducerComponent`, `EtherNodeComponent`.
- `DualFrontier.Events.Power` — `PowerOnlineEvent`,
  `PowerOutageEvent`, `EtherNodeChangedEvent`.

## Что внутри
- `ElectricGridSystem.cs` — NORMAL: баланс генерации/потребления.
- `EtherGridSystem.cs` — NORMAL: сеть эфирных узлов.
- `ConverterSystem.cs` — NORMAL: эфир→электричество (КПД 30% per GDD 9).

## Правила
- Шина домена — `nameof(IGameServices.World)` (электрика и эфир
  идут через "мир", пока нет отдельной шины Power).
- Конвертер — КПД 30% per GDD 9: 10 эфира → 3 электричества.
- При сбое публикуем `PowerOutageEvent`, чтобы Presentation мог
  мигнуть лампочками.

## Примеры использования
```csharp
// Внутри ConverterSystem:
// produced = incomingEther * 0.3f; // КПД 30% per GDD 9
```

## TODO
- [ ] Реализовать `ElectricGridSystem`: union-find по кабелям.
- [ ] Реализовать `EtherGridSystem`: плотность и передача по узлам.
- [ ] Реализовать `ConverterSystem` с коэффициентом 0.3f.
- [ ] Рассмотреть выделение отдельного `IPowerBus` при росте трафика.

# Building

## Назначение
Компоненты для построек и их инфраструктуры: потребители и производители
энергии (электричество и эфир), хранилища, верстаки.

## Зависимости
- `DualFrontier.Contracts` — `IComponent`, `EntityId`.

## Что внутри
- `PowerConsumerComponent.cs` — потребитель энергии (тип + расход в тик).
- `PowerProducerComponent.cs` — производитель энергии (тип + выработка).
- `StorageComponent.cs` — хранилище (capacity + список items).
- `WorkbenchComponent.cs` — верстак (тип + скорость работы).

## Правила
- Две независимые сети энергии: электричество и эфир. Поле
  `PowerType` указывает, в какую сеть подключена постройка.
  Смешение в одной сети запрещено валидатором PowerSystem.
- Перегрузка сети публикует `GridOverloadEvent`.
- `StorageComponent.Items` — это entity-IDs предметов в мире
  (предметы — отдельные сущности со своими компонентами).

## Примеры использования
```csharp
var reactor = world.CreateEntity();
world.AddComponent(reactor, new PowerProducerComponent { /* Type = PowerType.Electric, Output = 1000 */ });

var golemForge = world.CreateEntity();
world.AddComponent(golemForge, new PowerConsumerComponent { /* Type = PowerType.Ether, WattsPerTick = 50 */ });
```

## TODO
- [ ] Определить `PowerType` enum (Electric, Ether).
- [ ] Определить `WorkbenchKind` enum (Cooking, Smithing, Research, GolemForge …).
- [ ] Продумать дерадацию/поломку оборудования — отдельный `DurabilityComponent`.

# DualFrontier.Contracts

## Назначение
Публичные контракты проекта Dual Frontier — маркер-интерфейсы ECS, базовые
типы идентификаторов, доменные шины событий, API модов и атрибуты декларации
доступа систем. Это единственная сборка, на которую могут и должны ссылаться
моды, внешние инструменты и все остальные сборки решения. Никакой логики —
только типы и сигнатуры.

## Зависимости
- `System` (BCL) — больше никаких ссылок не допускается.

## Что внутри
- `Core/` — маркер-интерфейсы ECS (`IEntity`, `IComponent`, `IEvent`, `IQuery`,
  `IQueryResult`, `ICommand`) и идентификатор `EntityId`.
- `Bus/` — базовая шина `IEventBus`, агрегатор `IGameServices`, доменные шины
  (`ICombatBus`, `IInventoryBus`, `IMagicBus`, `IPawnBus`, `IWorldBus`).
- `Modding/` — API модов: `IMod`, `IModApi`, `IModContract`, `ModManifest`.
- `Attributes/` — декларативные атрибуты: `SystemAccessAttribute`,
  `DeferredAttribute`, `ImmediateAttribute`, `TickRateAttribute`.

## Правила
- Здесь **только** интерфейсы, атрибуты и простые `record struct` / `sealed class`
  манифесты. Никакой реализации и никаких приватных полей.
- Запрещены ссылки на Godot, на `DualFrontier.Core` и на любые сборки кроме BCL.
- Изменение сигнатуры = ломающее изменение для всех модов. Меняй осознанно и
  бампай major версию контрактов.
- XML `<summary>` обязателен для каждого публичного типа и члена.

## Примеры использования
```csharp
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

[SystemAccess(reads: new[] { typeof(IComponent) }, writes: new Type[0], bus: nameof(IGameServices.Combat))]
public sealed class ExampleSystem { }
```

## TODO
- [ ] Фаза 0 — зафиксировать финальный набор маркер-интерфейсов ECS.
- [ ] Фаза 0 — согласовать сигнатуры `IModApi` с системой загрузки модов.
- [ ] Фаза 2 — добавить версионирование контрактов (атрибут `[ContractVersion]`).
- [ ] Фаза 2 — документировать SemVer политику ломающих изменений.

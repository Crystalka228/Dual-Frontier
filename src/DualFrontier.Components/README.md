# DualFrontier.Components

## Назначение
Сборка компонентов ECS. Чистые POCO-данные, без логики. Каждый компонент
реализует `IComponent` из `DualFrontier.Contracts.Core`. Логика, изменяющая
эти данные, живёт в `DualFrontier.Systems`.

Компоненты описывают все игровые сущности: пешек, здания, магические узлы,
оружие и боеприпасы, энергосеть и т. д. Разбиты по доменам (Shared / Pawn /
Magic / Combat / Building / World), по одной папке на домен.

## Зависимости
- `DualFrontier.Contracts` — маркер `IComponent`, `EntityId`.

Компоненты НЕ зависят от `Systems`, `Events`, `Core`, движка Godot или любой
другой сборки проекта. Это обеспечивает изоляцию данных от логики.

## Что внутри
- `Shared/` — базовые компоненты для любой entity (позиция, здоровье, фракция, раса).
- `Pawn/` — специфично для разумных пешек (нужды, навыки, настроение, работа, соц. связи).
- `Magic/` — мана, школы магии, уровень эфира, связь с големом (GDD 4–5).
- `Combat/` — оружие, броня, щиты, боеприпасы (GDD 6, Combat Extended).
- `Building/` — потребители/производители энергии, хранилища, верстаки.
- `World/` — тайлы, эфирные узлы, биомы.

## Правила
- Только POCO — поля `public`, без методов (кроме expression-bodied, как
  `IsDead => Current <= 0`).
- Никаких ссылок на другие сборки кроме `Contracts`.
- Именование: `XxxComponent.cs`, класс `public sealed class XxxComponent : IComponent`.
- Коллекции инициализируются лениво или системами — не в ctor компонента.
- Любое изменение состояния компонента — только через систему с
  `[SystemAccess(writes: ...)]`.

## Примеры использования
```csharp
// Система читает и изменяет компоненты, но сам компонент — просто данные.
[SystemAccess(reads: new[] { typeof(HealthComponent) })]
public class DeathReporterSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<HealthComponent>())
        {
            var health = GetComponent<HealthComponent>(entity);
            if (health.IsDead) { /* publish DeathEvent */ }
        }
    }
}
```

## TODO
- [ ] Заполнить TODO-поля в каждом компоненте (Фаза 1–2).
- [ ] Определить enum-типы (`RaceKind`, `SkillKind`, `JobKind`, `MagicSchool`,
      `DamageType`, `ShieldKind`, `AmmoType`, `PowerType`, `WorkbenchKind`,
      `TerrainKind`, `BiomeKind`, `GridVector`) — в соответствующих доменных папках.
- [ ] Написать unit-тесты на сериализацию компонентов (Фаза 3, для save/load).

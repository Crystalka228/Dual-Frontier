# Pawn

## Назначение
Компоненты, специфичные для разумных пешек: нужды (голод, сон, комфорт),
навыки, настроение, текущая работа, социальные связи. На базе этих данных
работает AI пешек (см. `DualFrontier.AI`).

## Зависимости
- `DualFrontier.Contracts` — `IComponent`, `EntityId`.

## Что внутри
- `NeedsComponent.cs` — физиологические нужды (голод, сон, комфорт).
- `SkillsComponent.cs` — уровни навыков по `SkillKind`.
- `MindComponent.cs` — настроение и порог срыва.
- `JobComponent.cs` — текущая работа и её цель.
- `SocialComponent.cs` — отношения с другими пешками.

## Правила
- `Dictionary`-поля инициализируются системами при создании пешки, а не
  в конструкторе компонента (мы хотим pooling без лишних аллокаций).
- `Mood` и `MoodBreakThreshold` — в одной структуре, чтобы MoodSystem
  читала их атомарно.
- Прямая правка `Relations` из не-социальной системы запрещена — только через
  `[SystemAccess(writes: SocialComponent)]` у SocialSystem.

## Примеры использования
```csharp
[SystemAccess(writes: new[] { typeof(NeedsComponent) })]
public class NeedsDecaySystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var e in Query<NeedsComponent>())
        {
            var needs = GetComponent<NeedsComponent>(e);
            // needs.Hunger -= hungerRate * delta;
        }
    }
}
```

## TODO
- [ ] Определить `SkillKind` enum (Construction, Mining, Cooking, Combat, Magic …) — GDD.
- [ ] Определить `JobKind` enum (Idle, Build, Haul, Research, Fight …).
- [ ] Мысли/черты характера (`TraitsComponent`) — Фаза 3.

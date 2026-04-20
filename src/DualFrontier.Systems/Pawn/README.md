# Pawn Systems

## Назначение
Системы жизненного цикла пешки: нужды (голод/сон/отдых), выбор
текущей работы, настроение/ломки, социальные связи и рост
навыков. См. GDD разделы "Пешки" и "Нужды".

## Зависимости
- `DualFrontier.Contracts` — атрибуты, шина `IPawnBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Pawn` — `NeedsComponent`, `MindComponent`,
  `JobComponent`, `SkillsComponent`, `SocialComponent`.
- `DualFrontier.Components.Shared` — `PositionComponent`, `HealthComponent`.
- `DualFrontier.Events.Pawn` — `MoodBreakEvent`, `SkillGainEvent`, ...

## Что внутри
- `NeedsSystem.cs` — SLOW: деградация нужд во времени.
- `JobSystem.cs` — NORMAL: выбор и назначение джобов пешкам.
- `MoodSystem.cs` — SLOW: пересчёт настроения по нуждам и здоровью.
- `SocialSystem.cs` — RARE: социальные связи и их влияние на mind.
- `SkillSystem.cs` — NORMAL: рост навыков по опыту.

## Правила
- Шина домена — `nameof(IGameServices.Pawns)`.
- Writes / reads строго по декларации атрибута `SystemAccess`.
- `JobSystem` — единственная система, которая ПИШЕТ `JobComponent`.
- Ломка настроения публикуется `MoodBreakEvent` в `Pawns`-шину,
  `JobSystem` реагирует на неё сменой задачи.

## Примеры использования
```csharp
// Внутри JobSystem.Update:
foreach (var pawn in Query<NeedsComponent, SkillsComponent, PositionComponent>()) {
    ref var needs = ref GetComponent<NeedsComponent>(pawn);
    // TODO: выбрать джоб по самой горящей потребности
}
```

## TODO
- [ ] Реализовать `NeedsSystem`: падение сытости/сна во времени.
- [ ] Реализовать `JobSystem`: приоритеты джобов по нуждам и навыкам.
- [ ] Реализовать `MoodSystem`: формула mood = f(needs, health, events).
- [ ] Реализовать `SocialSystem`: граф дружб/вражды.
- [ ] Реализовать `SkillSystem`: кривая опыта и деградация.

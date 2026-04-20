# Pawn Events

## Назначение
События, относящиеся к пешкам как личностям: психологический срыв, реакция
на чужую смерть, прокачка навыка.

## Зависимости
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Что внутри
- `MoodBreakEvent.cs` — настроение пешки ушло ниже порога.
- `DeathReactionEvent.cs` — пешка увидела смерть сородича.
- `SkillGainEvent.cs` — навык повышен.

## Правила
- `MoodBreakEvent` публикуется MoodSystem один раз на переход — повторный
  срыв без восстановления не считается новым событием.
- `DeathReactionEvent` подписывается на deferred `DeathEvent` из Combat:
  реакция происходит в следующей фазе, когда тело уже удалено из сцены.
- `SkillGainEvent` — для UI/статистики; сам уровень хранится в
  `SkillsComponent`, событие несёт delta.

## Примеры использования
```csharp
// MoodSystem (SLOW tick):
if (mind.Mood < mind.MoodBreakThreshold && !already)
    _bus.Publish(new MoodBreakEvent { /* PawnId = pawn, Severity = ... */ });
```

## TODO
- [ ] Уточнить градацию срывов (minor / major / berserk) — GDD психологии.
- [ ] Добавить `RelationshipChangedEvent` — для SocialSystem, Фаза 3.

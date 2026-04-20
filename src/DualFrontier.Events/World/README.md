# World Events

## Назначение
События мирового уровня: изменение эфирных узлов, смена погоды, входящий
рейд. Широковещательные — подписчиков много (AI, MoodSystem, UI, аудио).

## Зависимости
- `DualFrontier.Contracts` — `IEvent`, `EntityId`.

## Что внутри
- `EtherNodeChangedEvent.cs` — параметры эфирного узла изменились (уровень/радиус).
- `WeatherChangedEvent.cs` — сменилась погода.
- `RaidIncomingEvent.cs` — к колонии приближается рейд.

## Правила
- `EtherNodeChangedEvent` публикуется EtherFieldSystem после пересчёта
  поля; ManaSystem использует его для пересчёта регенерации маны.
- `RaidIncomingEvent` — с запасом по времени (фаза подготовки);
  конкретное столкновение триггерит уже `ShootAttemptEvent` и т. п.

## Примеры использования
```csharp
// MoodSystem на плохую погоду добавляет штраф:
_bus.Subscribe<WeatherChangedEvent>(evt =>
{
    // if (evt.Kind == WeatherKind.Storm) { ... }
});
```

## TODO
- [ ] Определить `WeatherKind` enum (Clear, Rain, Storm, EtherStorm …) — Фаза 4.
- [ ] Добавить `SeasonChangedEvent` — если будут сезоны, Фаза 6.

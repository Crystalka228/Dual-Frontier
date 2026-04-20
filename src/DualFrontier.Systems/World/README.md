# World Systems

## Назначение
Глобальные системы мира: карта (тайлы, декорации), погода и
биомы. Они редко тикают и редко меняются — но их события
(`WeatherChangedEvent`, `BiomeShiftEvent`) читают почти все.

## Зависимости
- `DualFrontier.Contracts` — атрибуты, `IWorldBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.World` — `TileComponent`, `BiomeComponent`.
- `DualFrontier.Events.World` — `WeatherChangedEvent`,
  `BiomeShiftEvent`, `MapRegionLoadedEvent`.

## Что внутри
- `MapSystem.cs` — RARE: подгрузка/выгрузка регионов карты.
- `WeatherSystem.cs` — RARE: смена погоды, публикация события.
- `BiomeSystem.cs` — RARE: ползучие сдвиги биомов (например, эфир).

## Правила
- Шина домена — `nameof(IGameServices.World)`.
- Все три системы — RARE (3600 фреймов ≈ раз в минуту реального времени),
  чтобы не нагружать основной цикл.
- `WeatherSystem` не пишет в `BiomeComponent` напрямую — только
  через публикацию события; `BiomeSystem` реагирует.

## Примеры использования
```csharp
// Внутри WeatherSystem:
worldBus.Publish(new WeatherChangedEvent(from: Clear, to: EtherStorm));
```

## TODO
- [ ] Реализовать `MapSystem`: стриминг регионов по центру камеры.
- [ ] Реализовать `WeatherSystem`: марковская цепь погоды.
- [ ] Реализовать `BiomeSystem`: влияние эфира на тип биома.

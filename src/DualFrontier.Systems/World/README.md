# World Systems

## Purpose
Global world systems: map (tiles, decorations), weather, and biomes. They
tick rarely and change rarely — but their events
(`WeatherChangedEvent`, `BiomeShiftEvent`) are read by almost everyone.

## Dependencies
- `DualFrontier.Contracts` — attributes, `IWorldBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.World` — `TileComponent`.
- `DualFrontier.Events.World` — `WeatherChangedEvent`.

## Contents
- `MapSystem.cs` — RARE: load/unload of map regions.
- `WeatherSystem.cs` — RARE: weather change, publishes the event.

## Rules
- Domain bus — `nameof(IGameServices.World)`.
- Both systems are RARE (3600 frames ≈ once per real-time minute), so
  they do not weigh on the main loop.

## Usage examples
```csharp
// Inside WeatherSystem:
worldBus.Publish(new WeatherChangedEvent(from: Clear, to: EtherStorm));
```

## TODO
- [ ] Implement `MapSystem`: region streaming based on the camera center.
- [ ] Implement `WeatherSystem`: a Markov chain of weather.

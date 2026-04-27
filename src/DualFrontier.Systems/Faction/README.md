# Faction Systems

## Purpose
Meta-game relations between factions: diplomacy, trade, and raids. See the
GDD "Factions" section.

## Dependencies
- `DualFrontier.Contracts` — attributes, `IWorldBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Shared` — `FactionComponent`.
- `DualFrontier.Components.Pawn` — `SocialComponent` (for individual pawn influence on reputation).
- `DualFrontier.Events.World` — `RaidIncomingEvent`, `TradeCaravanEvent`.

## Contents
- `RelationSystem.cs` — RARE: evolution of inter-faction relations.
- `TradeSystem.cs` — RARE: arrival of trade caravans.
- `RaidSystem.cs` — RARE: raid spawning, publishes `RaidIncomingEvent`.

## Rules
- Domain bus — `nameof(IGameServices.World)`.
- All three systems are RARE; meta-game events are infrequent.
- `RaidSystem` is the only system that decides when a raid happens; the rest
  of the world reacts to `RaidIncomingEvent`.

## Usage examples
```csharp
// Inside RaidSystem:
worldBus.Publish(new RaidIncomingEvent(fromFaction: "raiders", strength: 12));
```

## TODO
- [ ] Implement `RelationSystem`: relation matrix [-100..100].
- [ ] Implement `TradeSystem`: per-faction goods table.
- [ ] Implement `RaidSystem`: raid-strength formula derived from colony wealth.

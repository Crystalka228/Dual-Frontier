---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-SYSTEMS-POWER
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-SYSTEMS-POWER
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-SYSTEMS-POWER
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-SYSTEMS-POWER
---
# Power Systems

## Purpose
Power-supply networks: the electric grid, the ether grid, and converters
between them. See GDD section 9 "Power systems".

## Dependencies
- `DualFrontier.Contracts` — attributes, `IWorldBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Building` — `PowerConsumerComponent`,
  `PowerProducerComponent`, `EtherNodeComponent`.
- `DualFrontier.Events.Power` — `PowerOnlineEvent`,
  `PowerOutageEvent`, `EtherNodeChangedEvent`.

## Contents
- `ElectricGridSystem.cs` — NORMAL: balance of generation/consumption.
- `EtherGridSystem.cs` — NORMAL: ether-node network.
- `ConverterSystem.cs` — NORMAL: ether → electricity (30% efficiency per GDD 9).

## Rules
- Domain bus — `nameof(IGameServices.World)` (electricity and ether
  go through "world" until a dedicated Power bus exists).
- Converter — 30% efficiency per GDD 9: 10 ether → 3 electricity.
- On a fault publish `PowerOutageEvent` so Presentation can blink the lights.

## Usage examples
```csharp
// Inside ConverterSystem:
// produced = incomingEther * 0.3f; // 30% efficiency per GDD 9
```

## TODO
- [x] `ElectricGridSystem` — priority watt distribution
      (sort consumers by Priority desc, allocate until supply is exhausted,
      publish `PowerGrantedEvent` per consumer and `GridOverloadEvent`
      whenever any consumer ends up unpowered).
- [x] `ConverterSystem` — 30% efficiency ether ↔ electricity (mirrors
      `consumer.IsPowered` into `producer.CurrentWatts`).
- [ ] Register `ConverterSystem` in `GameBootstrap` —
      requires `[Deferred]` semantics in `DomainEventBus` (cycle
      `ElectricGrid ↔ Converter` via `PowerConsumer/PowerProducer`).
- [ ] Implement `EtherGridSystem`: node density and transfer.
- [ ] Consider splitting out a dedicated `IPowerBus` once traffic grows.

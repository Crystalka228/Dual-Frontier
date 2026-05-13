---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-SYSTEMS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-SYSTEMS
---
# DualFrontier.Systems

## Purpose
All Dual Frontier game logic. Each system is a `SystemBase` subclass that
declares its READ/WRITE dependencies via `[SystemAccess]` and its update
frequency via `[TickRate]`. The Core scheduler uses these declarations to
build the dependency graph and execute phases in parallel (see TechArch
11.4–11.6).

## Dependencies
- `DualFrontier.Contracts` — `SystemAccessAttribute`, `TickRateAttribute`,
  `IGameServices` and the domain buses (`ICombatBus`, `IMagicBus`, …),
  `EntityId`, `IEvent`, `IComponent`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`, the execution context,
  and the scheduler.
- `DualFrontier.Components` — every domain component the systems read and
  write.
- `DualFrontier.Events` — the events the systems publish and subscribe to.

## Contents
- `Pawn/` — needs, jobs, mood, skills, movement, sleep, consume, comfort.
- `Magic/` — mana, spells, golems, ether fields, rituals.
- `Combat/` — combat initiation, projectiles, damage, effects.
- `Inventory/` — storage, hauling, crafting.
- `Power/` — electric grid, ether grid, converters.
- `World/` — map, weather.
- `Faction/` — relations, trade, raids.

## Rules
- Each system is `public sealed class XSystem : SystemBase`.
- Each system MUST have `[SystemAccess(reads, writes, bus)]` and
  `[TickRate(TickRates.X)]`.
- Direct access to `World` is forbidden — only through
  `SystemExecutionContext` in `SystemBase`.
- Async APIs (`async`/`await`, `Task.Run`) are forbidden — they break the
  `ThreadLocal` isolation guard (see THREADING).
- Inter-system communication only through buses (`IGameServices.X`)
  and component access from the system's READ/WRITE sets.
- The bus property name in `bus:` is set via `nameof(IGameServices.Combat)` —
  not as a string literal.

## Usage examples
```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(ArmorComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
public sealed class CombatSystem : SystemBase { /* ... */ }
```

## TODO
- [ ] Implement `Update` bodies in every system once `SystemBase` and
      `SystemExecutionContext` are ready.
- [ ] Subscribe to relevant events in each system's `OnInitialize()`.
- [ ] Cover dependency-graph tests (READ/WRITE conflicts → error).
- [ ] Add integration tests for "one phase — one bus pass".

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CONTRACTS-ATTRIBUTES
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CONTRACTS-ATTRIBUTES
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CONTRACTS-ATTRIBUTES
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CONTRACTS-ATTRIBUTES
---
# Attributes — Declarative attributes

## Purpose
Attributes that systems use to declare their dependencies and runtime behavior.
The scheduler builds the parallelism and tick-rate graphs by reading these
attributes through reflection at startup.

## Dependencies
- `System` (BCL)

## Contents
- `SystemAccessAttribute.cs` — declares READ/WRITE components and the bus name.
  The basis of the dependency graph and the isolation guard.
- `DeferredAttribute.cs` — the event is delivered in the next scheduler phase.
- `ImmediateAttribute.cs` — the event preempts the current phase for instant delivery.
- `TickRateAttribute.cs` — the call frequency for a system's `Update` (REALTIME / FAST / NORMAL / SLOW / RARE).

## Rules
- If a system has no `[SystemAccess]`, the scheduler treats it as "writes to
  everything" and blocks parallelism. This is a deliberate fail-safe.
- `[Deferred]` and `[Immediate]` are mutually exclusive. If neither is set, the
  event is delivered synchronously in the current phase.
- `TickRateAttribute` applies only to systems (not to components or events).

## Usage examples
```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat))]
[TickRate(TickRates.FAST)]
public sealed class CombatSystem : SystemBase { /* ... */ }

[Deferred]
public sealed record DeathEvent(EntityId Who) : IEvent;
```

## TODO
- [ ] Phase 1 — add a Roslyn analyzer that emits a CS warning when a system
      lacks `[SystemAccess]`.
- [ ] Phase 2 — add `[Phase(int)]` for manually overriding a phase for diagnostics.

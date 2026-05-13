---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-THREADING
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-THREADING
---
# Multithreading

RimWorld is single-threaded: every system runs in sequence. On multi-core hardware 7 of 8 cores sit idle. Dual Frontier builds the dependency graph once at startup and runs unrelated systems in parallel. With no changes to game code — purely through the `[SystemAccess]` declaration.

## DependencyGraph

Every system declares which components it reads and which it writes. At startup `DependencyGraph` collects every declaration and builds the graph:

- Two systems conflict if one writes a component that the other reads or writes.
- Two systems writing different components do not conflict.
- Two systems reading the same component do not conflict.

```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
public sealed class CombatSystem : SystemBase { /* ... */ }
```

The attribute is read once via reflection when the system is registered — there is no reparse at runtime; the declaration becomes part of the graph in memory.

## Topological sort into phases (v0.2: 5 phases)

The scheduler applies a topological sort to the graph and groups unrelated systems into phases. Each phase is a set of systems executed in parallel. v0.2 introduces a fixed scaffold of five phases that every tick traverses.

```
┌───────────────────────────────────────────────────────────────┐
│ Phase 1 — Input & Sensors                                     │
│   Read the world, publish Intents.                            │
│   CombatSystem, SpellSystem, SensorSystem, JobSystem.         │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 2 — Intent Collection                                   │
│   CompositeResolutionSystem waits for multi-bus responses by  │
│   TransactionId; produces final ShootGranted/ShootRefused.    │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 3 — Resolution                                          │
│   Resolution systems decide granted/refused:                  │
│   InventorySystem (AmmoIntent), ManaSystem (ManaIntent and    │
│   lease open/drain), TargetResolutionSystem.                  │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 4 — Apply & Damage                                      │
│   ComboResolutionSystem merges DamageIntent[] → DamageEvent   │
│   in stable order. Apply statuses, deaths.                    │
└──────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ Phase 5 — Feedback snapshot                                   │
│   Mana[N] → ManaSnapshot[N]; EtherSnapshot, HealthSnapshot.   │
│   Read in next tick's Phase 1 by observer systems.            │
└───────────────────────────────────────────────────────────────┘
```

The `Mana[N-1]` snapshot is read by `GolemSystem` on the next tick — this resolves the "ManaSystem writes, GolemSystem reads" loop without a component conflict. Details: [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md).

Within each phase, unrelated systems execute in parallel via `Parallel.ForEach` (see below). The graph is built once at game startup, so scheduling overhead is zero at runtime. Declaration changes require a restart — but C# compilation already forces a restart on code edits.

## Parallel execution

`ParallelSystemScheduler` runs every system in one phase through `Parallel.ForEach` with a core-count cap: `N-2` for the game, leaving cores for the Godot main thread and the OS scheduler.

```csharp
// Simplified skeleton — full version in Scheduling/ParallelSystemScheduler.cs
void RunPhase(SystemPhase phase, float delta)
{
    Parallel.ForEach(phase.Systems, system =>
    {
        SystemExecutionContext.PushContext(system);
        try { system.Update(delta); }
        finally { SystemExecutionContext.PopContext(); }
    });
    // Delivery of [Deferred] events between phases.
    FlushDeferredEvents();
}
```

Each system gets its own `SystemExecutionContext` via `ThreadLocal<T>`. The context holds the list of permitted components and the name of the active bus. Isolation-guard access is O(1) via `HashSet`.

## TickRates

Not every system needs to tick every frame. A real frame is about 16 ms at 60 FPS. `[TickRate]` sets the frequency.

| Tick     | Period       | Use                                                  |
|----------|--------------|------------------------------------------------------|
| REALTIME | every frame  | Projectile physics, `ProjectileSystem`.              |
| FAST     | 3 frames     | `CombatSystem`, `DamageSystem` — combat responsiveness. |
| NORMAL   | 15 frames    | `JobSystem`, `ManaSystem`, `SkillSystem`.            |
| SLOW     | 60 frames    | `NeedsSystem`, `MoodSystem`, `EtherGrowthSystem`.    |
| RARE     | 3600 frames  | `SocialSystem`, `RaidSystem`, `WeatherSystem`.       |

```csharp
[SystemAccess(reads: [...], writes: [...])]
[TickRate(TickRates.SLOW)]
public sealed class NeedsSystem : SystemBase { /* ... */ }
```

`TickScheduler` keeps a frame counter and calls `Update` only on systems whose counter has reached zero. Different systems in the same phase may have different tick rates — the scheduler accounts for this without breaking the graph.

## Rule: async is forbidden

Inside systems, `async` / `await` are forbidden. The reason lies in `SystemExecutionContext`: it lives in `ThreadLocal`, bound to the current thread, and `await` switches execution to another thread after the return — on the new thread the context is different, the isolation guard will not see the declaration, and even if it did, the component write would happen without synchronization with the dependency graph.

What to do instead of `async`:

- Long-running work (pathfinding, serialization) — through the `Application` layer, on a separate thread, with the result returned via an event or a command.
- I/O — only in Application, never in systems.
- Waiting is unnecessary: the scheduler will call the system in the next phase or the next tick.

In DEBUG, the isolation guard catches `Task`, `ValueTask`, and `await` via stack-trace analysis and throws `IsolationViolationException` with the message: `"System 'XXX' uses async. Move the work to Application."`.

## Debugging conflicts

A common error when adding a system is a cycle or a declaration conflict. Scheduler messages:

### Cycle in the graph

```
[SCHEDULER ERROR] Cyclic dependency detected:
  CombatSystem (W:Health) → DamageSystem (R:Health, W:Health)
  DamageSystem (W:Health) → CombatSystem (R:Health)
Resolve: break the cycle with a [Deferred] event.
```

The fix: one of the systems publishes a `[Deferred]` event instead of writing directly; the inter-phase delivery breaks the cycle.

### Declaration violation at runtime

```
[IsolationViolationException]
System 'PoisonSystem'
accessed 'BloodComponent'
without an access declaration.
Add: [SystemAccess(reads: new[]{typeof(BloodComponent)})]
```

This is not a scheduler failure but an isolation-guard failure — see [ISOLATION](./ISOLATION.md).

### Two systems write the same component in one phase

```
[SCHEDULER ERROR] Write conflict in phase 2:
  PoisonSystem writes HealthComponent
  DamageSystem writes HealthComponent
Resolve: one system must read instead of write, or move to a different phase.
```

The fix: either one system becomes a reader, or one of them moves to a later phase via a `[Deferred]` event.

## See also

- [ECS](./ECS.md)
- [ISOLATION](./ISOLATION.md)
- [EVENT_BUS](./EVENT_BUS.md)
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — Phase 5 and reading `Mana[N-1]`.
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md) — Phase 2, waiting for multi-bus responses.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — Phase 4, ordering `DamageIntent`.
- [GPU_COMPUTE](./GPU_COMPUTE.md) — async fence-based sync between CPU tick loop and GPU compute dispatch; see "Architectural integration → Two-phase model fit" and "Failure modes → Async sync hazards".

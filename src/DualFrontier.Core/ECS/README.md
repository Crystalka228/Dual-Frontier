# ECS — Entity Component System

## Purpose
The ECS architecture core. World stores all entities and their components,
ComponentStore provides O(1) access by component type, and
SystemExecutionContext is the isolation guard that verifies that a
system never reaches for an undeclared component.

## Dependencies
- DualFrontier.Contracts (the `IComponent` and `EntityId` interfaces)

## Contents
- World.cs — registry of all entities; the entry point for systems
- ComponentStore.cs — typed storage for components of a single type
- SystemBase.cs — base class for every game system
- SystemExecutionContext.cs — isolation guard (`ThreadLocal` context)
- IsolationViolationException.cs — exception thrown on an isolation violation

## Rules
- Systems access World ONLY through SystemExecutionContext.
- A direct `world.GetComponent<T>()` from a system = crash.
- A system MUST declare READ/WRITE through `[SystemAccess]`.
- ComponentStore itself is NOT thread-safe — protection comes through the
  dependency graph at the `ParallelSystemScheduler` level.

## Usage examples
```csharp
[SystemAccess(reads: new[] { typeof(HealthComponent) }, writes: new Type[0], bus: nameof(IGameServices.Pawns))]
public class DamageReporterSystem : SystemBase {
    public override void Update(float delta) {
        // foreach (var entity in Query<HealthComponent>()) { ... }
        // var health = GetComponent<HealthComponent>(entity);
        // Access permitted — HealthComponent is in reads
    }
}
```

## TODO
- [ ] Implement `ComponentStore<T>` on top of a SparseSet structure.
- [ ] Implement `EntityId` generation with versions.
- [ ] Implement `Query<T1, T2, ...>` for entity searches by component set.
- [ ] Implement `SystemExecutionContext` with `ThreadLocal`.
- [ ] Write tests that trigger `IsolationViolationException`.

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CORE-ECS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CORE-ECS
---

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
register_id: DOC-F-SRC-CORE-ECS
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: Core ECS submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---

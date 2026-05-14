# DualFrontier.Components

## Purpose
The ECS components assembly. Pure POCO data, no logic. Every component
implements `IComponent` from `DualFrontier.Contracts.Core`. Logic that mutates
this data lives in `DualFrontier.Systems`.

Components describe every game entity: pawns, buildings, magic nodes, the
power grid, and so on. They are split by domain (Shared / Pawn / Magic /
Combat / Building / World), one folder per domain.

## Dependencies
- `DualFrontier.Contracts` — the `IComponent` marker, `EntityId`.

Components do NOT depend on `Systems`, `Events`, `Core`, the Godot engine, or
any other project assembly. This isolates data from logic.

## Contents
- `Shared/` — base components for any entity (position, health, faction, race).
- `Pawn/` — specific to sapient pawns (needs, skills, mood, work, social ties).
- `Magic/` — mana, ether level, golem bond (GDD 4–5).
- `Combat/` — armor (GDD 6, Combat Extended).
- `Building/` — power consumers/producers, storages, workbenches.
- `World/` — tiles, ether nodes.

## Rules
- POCO only — `public` fields, no methods (except expression-bodied ones like
  `IsDead => Current <= 0`).
- No references to other assemblies besides `Contracts`.
- Naming: `XxxComponent.cs`, class `public sealed class XxxComponent : IComponent`.
- Collections are initialized lazily or by systems — not in the component's ctor.
- Any component-state change happens only through a system with
  `[SystemAccess(writes: ...)]`.

## Usage examples
```csharp
// A system reads and mutates components, but the component itself is just data.
[SystemAccess(reads: new[] { typeof(HealthComponent) })]
public class DeathReporterSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<HealthComponent>())
        {
            var health = GetComponent<HealthComponent>(entity);
            if (health.IsDead) { /* publish DeathEvent */ }
        }
    }
}
```

## TODO
- [ ] Fill the TODO fields in every component (Phase 1–2).
- [ ] Define enum types (`RaceKind`, `SkillKind`, `JobKind`, `DamageType`,
      `PowerType`, `WorkbenchKind`, `TerrainKind`) — in their respective
      domain folders.
      (`GridVector` is already defined in `DualFrontier.Contracts.Math`.)
- [ ] Write unit tests for component serialization (Phase 3, for save/load).

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-COMPONENTS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-COMPONENTS
---

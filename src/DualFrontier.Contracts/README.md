---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-SRC-CONTRACTS
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-SRC-CONTRACTS
---
# DualFrontier.Contracts

## Purpose
Public contracts of the Dual Frontier project — ECS marker interfaces, base
identifier types, domain event buses, the mod API, and system access-declaration
attributes. This is the only assembly that mods, external tools, and every
other assembly in the solution may and should reference. No logic — only types
and signatures.

## Dependencies
- `System` (BCL) — no other references allowed.

## Contents
- `Core/` — ECS marker interfaces (`IEntity`, `IComponent`, `IEvent`, `IQuery`,
  `IQueryResult`, `ICommand`) and the `EntityId` identifier.
- `Bus/` — the base `IEventBus`, the `IGameServices` aggregator, and the domain
  buses (`ICombatBus`, `IInventoryBus`, `IMagicBus`, `IPawnBus`, `IWorldBus`).
- `Modding/` — mod API: `IMod`, `IModApi`, `IModContract`, `ModManifest`.
- `Attributes/` — declarative attributes: `SystemAccessAttribute`,
  `DeferredAttribute`, `ImmediateAttribute`, `TickRateAttribute`.

## Rules
- This folder contains **only** interfaces, attributes, and simple `record struct` /
  `sealed class` manifests. No implementation and no private fields.
- References to Godot, to `DualFrontier.Core`, or to any assembly outside the BCL
  are forbidden.
- A signature change is a breaking change for every mod. Make changes deliberately
  and bump the contracts' major version.
- An XML `<summary>` is mandatory for every public type and member.

## Usage examples
```csharp
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

[SystemAccess(reads: new[] { typeof(IComponent) }, writes: new Type[0], bus: nameof(IGameServices.Combat))]
public sealed class ExampleSystem { }
```

## TODO
- [ ] Phase 0 — pin the final set of ECS marker interfaces.
- [ ] Phase 0 — align `IModApi` signatures with the mod loading subsystem.
- [ ] Phase 2 — add contract versioning (`[ContractVersion]` attribute).
- [ ] Phase 2 — document the SemVer policy for breaking changes.

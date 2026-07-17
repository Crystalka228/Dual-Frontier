---
register_id: DOC-A-CONTRACTS
project: Dual Frontier
category: A
tier: 1
lifecycle: SUPERSEDED
owner: Crystalka
version: 1.1
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: null
title: Contract system (historical; superseded by authored rework)
superseded_by: DOC-A-CONTRACTS_V2
last_modified_commit: 6888246
review_cadence: on-change+annual
reviewer: Crystalka
risks_referenced:
- RISK-003
special_case_rationale: Superseded by DOC-A-CONTRACTS_V2 per corpus rework EVT-2026-07-15-CORPUS_REWORK_R2_PLATFORM. Last-ratified reference preserved at docs/architecture/historical/CONTRACTS.md; successor ratified LOCKED v1.0.0 2026-07-17 (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION) — authority-gap window closed.
---

# Contract system

The `DualFrontier.Contracts` assembly is the only module visible to every layer: the core, the systems, the mods, and external tools. A contract is a public interface that declares intent (what can be done) with no hints about implementation. A contract does not change for convenience: either add a new event, or cut a new interface version.

## Marker interfaces

Four base marker interfaces have no methods and serve only for typing. They let the scheduler and the event bus operate generically, without depending on concrete game types.

### IEvent

Marker for any message traveling on a bus. Events are `record`s with `init` properties, immutable. Examples: `ShootAttemptEvent`, `AmmoIntent`, `ManaGranted`, `DeathEvent`.

### IQuery and IQueryResult

A synchronous bus request with a typed response. Used sparingly — only for reading aggregated data (for example, `GetPawnsInRadiusQuery`). Most interactions go through the two-step Intent→Granted model.

### ICommand

An imperative intent to change state. Unlike an event, an `ICommand` is addressed — it has an expected handler. In Dual Frontier, commands live mostly in the Application layer for `IRenderCommand` (Domain → Presentation). Domain logic prefers events.

### IComponent

Marker for pure data attached to an `EntityId`. No logic in components. Example declaration:

```csharp
public sealed class HealthComponent : IComponent
{
    public float Current;
    public float Maximum;
    public bool IsDead => Current <= 0;
}
```

## Five domain buses

A single bus for everything is a bottleneck under load. Lock contention at 100+ systems. The solution: a separate bus per domain. Less contention, easier to debug, easier to profile.

```csharp
public interface IGameServices
{
    ICombatBus    Combat    { get; }
    IInventoryBus Inventory { get; }
    IMagicBus     Magic     { get; }
    IPawnBus      Pawns     { get; }
    IWorldBus     World     { get; }
}
```

| Bus           | Writers                          | Readers                              | Key events                                        |
|---------------|----------------------------------|--------------------------------------|---------------------------------------------------|
| CombatBus     | CombatSystem, ProjectileSystem   | DamageSystem, StatusEffectSystem     | ShootAttempt, DamageEvent, DeathEvent             |
| InventoryBus  | HaulSystem, CraftSystem          | InventorySystem, JobSystem           | AmmoIntent/Granted, ItemAdded/Removed/Reserved    |
| MagicBus      | SpellSystem, GolemSystem         | ManaSystem, EtherGrowthSystem        | ManaIntent/Granted, SpellCast, EtherSurge         |
| PawnBus       | NeedsSystem, MoodSystem          | JobSystem, SocialSystem              | MoodBreak, DeathReaction, SkillGain               |
| WorldBus      | BiomeSystem, WeatherSystem       | RaidSystem                           | EtherNodeChanged, WeatherChanged, RaidIncoming    |

Each bus is its own `ConcurrentDictionary` of subscribers. `CombatSystem` writes only to `Combat`; `InventorySystem` writes only to `Inventory`. There is no shared lock point. A system declares the bus it uses inside `[SystemAccess]`, and the dependency graph + future A'.9 Roslyn analyzer verify that publication targets only that bus.

The bus list is canonical per [src/DualFrontier.Contracts/Bus/IGameServices.cs](../../src/DualFrontier.Contracts/Bus/IGameServices.cs).

## IModContract — API between mods

Mods MUST NOT reference each other directly: that creates a loading cycle and hard dependencies. Instead, a mod can publish a contract interface that implements `IModContract`. Another mod requests the contract by type:

```csharp
public interface IVoidMagicContract : IModContract
{
    bool CanCastVoid(EntityId caster);
    void EmitVoidSurge(EntityId source);
}

public class ArtifactMod : IMod
{
    public void Initialize(IModApi api)
    {
        if (api.TryGetContract<IVoidMagicContract>(out var voidMagic))
        {
            api.Subscribe<VoidSpellCastEvent>(OnVoidMagicDetected);
        }
        // VoidMagic mod is not loaded — simply do not subscribe.
        // No crash, no hard dependency.
    }
}
```

A contract is the same public interface, but housed in an assembly both mods know. Typically this is a separate `ModName.Contracts` assembly, published as a NuGet package or sitting alongside in the `mods/` directory.

## Contract evolution

Contracts are long-lived. Every change is either breaking or non-breaking.

### Non-breaking (always allowed)

- Adding a new `IEvent`, `IQuery`, `ICommand`, or `IComponent`.
- Adding a new `init` field to a `record` (old `record`s that do not mention it do not break).
- Adding a method to an interface with a `default` implementation (C# 8+).
- Adding a new domain bus (a new property on `IGameServices`).

### Breaking (only with a major version bump)

- Removing or renaming a field on an `IEvent`.
- Removing a method from an interface.
- Changing the type of a parameter on an existing method.
- Changing phase semantics (was `[Deferred]`, became synchronous).

Breaking changes require bumping the assembly's major version and updating the migration document.

## Versioning

`DualFrontier.Contracts` uses semantic versioning `MAJOR.MINOR.PATCH`.

- `MAJOR` — breaking changes. Old mods require rebuilding.
- `MINOR` — added contracts. Old mods continue to work.
- `PATCH` — documentation, XML comments, internal fixes.

A mod manifest (`mod.manifest.json`) declares the minimum required contracts version. The loader refuses to load a mod built against a major version newer than the core.

```json
{
  "id": "com.example.voidmagic",
  "name": "Void Magic",
  "version": "1.2.0",
  "requiresContracts": "^1.0.0"
}
```

## See also

- [EVENT_BUS](./EVENT_BUS.md)
- [MODDING](./MODDING.md)
- [ECS](./ECS.md)
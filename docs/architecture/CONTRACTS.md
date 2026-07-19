---
register_id: DOC-A-CONTRACTS_V2
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 2.0.0
first_authored: 2026-07-15
last_modified: 2026-07-19
content_language: en
next_review_due: 2027-Q3
title: Contract system (authored rework; evolution rules tightened, version-gate truth corrected)
supersedes:
- DOC-A-CONTRACTS
review_cadence: on-change+annual
last_review_date: 2026-07-19
last_review_event: 'W2_BUS_CAPABILITY C7 (BD-3a): the five genre bus interfaces (ICombatBus/IInventoryBus/IMagicBus/IPawnBus/IWorldBus) and IGameServices left DualFrontier.Contracts for the engine-internal DualFrontier.Core.Bus -- a breaking interface removal, ContractsVersion.Current 1.0.0 -> 2.0.0 (MAJOR). §2 rewritten to record the departure + the BD-3b five-genre -> one-router collapse (the getters are now cosmetic bridges over one DomainEventBus); §4 generalizes the members-bearing-interface breaking rule off the departed IGameServices example; §5 current-version corrected 1.0.0 -> 2.0.0 (:20); §6 bus-publish-scoping gap dissolved with [SystemAccess(bus:)] (F-54). MAJOR 1.1.0 -> 2.0.0. EVT-2026-07-19-W2_BUS_CAPABILITY.'
reviewer: Crystalka
special_case_rationale: Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist item [1]). Successor of DOC-A-CONTRACTS per EVT-2026-07-15-CORPUS_REWORK_R2_PLATFORM; evolution rules tightened, version-gate truth corrected against code.
---

# Contract system

`DualFrontier.Contracts` is the only assembly every layer can see: core, systems, mods, external tools. A contract declares intent with no hint of implementation; it does not change for convenience — add a new type, or cut a new interface version.

> **Ratified successor (LOCKED v1.0.0 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION, 2026-07-17).** Successor of `docs/architecture/historical/CONTRACTS.md` (DOC-A-CONTRACTS, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`.

| Field | Value |
|---|---|
| Role | normative (ratified successor) |
| Successor of | `docs/architecture/historical/CONTRACTS.md` (DOC-A-CONTRACTS, now SUPERSEDED) |
| Scope | Contract-surface census: marker interfaces, the domain-bus surface's departure to Core (W2/BD-3), mod-to-mod channel, evolution and versioning rules |
| Non-goals | Bus delivery/phase mechanics (EVENT_BUS.md); mod capability grammar, ALC lifecycle (MOD_OS_ARCHITECTURE.md, MODDING.md); ECS rules (ECS.md); analyzer implementation detail (ANALYZER_RULES.md) |
| Authority domains | Contract-surface type census; breaking-vs-non-breaking classification; `IModContract` channel; version-gate semantics |
| Defers to | EVENT_BUS.md → delivery/phase semantics · MOD_OS_ARCHITECTURE.md → capability grammar, ALC · MODDING.md → author guide · ANALYZER_RULES.md → enforcement reality · KERNEL_ARCHITECTURE.md Part 0 → К-L4/К-L9 law |

## §1 Marker interfaces

Five base interfaces in `src/DualFrontier.Contracts/Core/` carry no members, letting the scheduler and buses stay generic.

- **`IEvent`** (`IEvent.cs`) — a message on a domain bus; immutable `record`s. `[Deferred]` (`Attributes/DeferredAttribute.cs`) queues delivery to the next phase; `[Immediate]` (`Attributes/ImmediateAttribute.cs`) interrupts the current one — honored by the mode resolver, though no shipped event carries it yet. Default is synchronous, within-phase.
- **`IQuery`** / **`IQueryResult`** (`IQuery.cs`, `IQueryResult.cs`) — synchronous request/typed-response, used sparingly; most interactions use the two-step Intent → Granted/Refused model instead (`AmmoIntent.cs:6-11` names it, citing "TechArch 11.5"; mechanics in [EVENT_BUS.md](./EVENT_BUS.md)).
- **`ICommand`** (`ICommand.cs`) — addressed, imperative "do X"; mostly `IRenderCommand` (Domain → Presentation). Domain logic prefers events.
- **`IComponent`** (`IComponent.cs`) — pure data on an `EntityId`, no logic. Real example:

```csharp
[ModAccessible(Read = true, Write = true)]
public struct HealthComponent : IComponent
{
    public float Current;
    public float Maximum;
    public bool IsDead => Current <= 0;
}
```

(`src/DualFrontier.Components/Shared/HealthComponent.cs:10-16`.) A `struct`, not a `class`: per К-L3 (KERNEL_ARCHITECTURE.md Part 0), unmanaged-struct storage is the default Path α; a `class` component needs the opt-in `[ManagedStorage]` Path β.

## §2 The domain-bus surface left Contracts (W2/BD-3, 2026-07-19)

Through W1 the five domain-bus interfaces (`ICombatBus`/`IInventoryBus`/`IMagicBus`/`IPawnBus`/`IWorldBus`) and their aggregator `IGameServices` were contract surface here. **At W2/BD-3a they relocated to `DualFrontier.Core.Bus` (engine-internal) and are no longer part of `DualFrontier.Contracts`.** Their removal from the contract assembly is the breaking change carried by §5's MAJOR bump — `ContractsVersion.Current` 1.0.0 → 2.0.0. `IEvent` and the generic routing marker stay in Contracts; the buses do not.

Simultaneously (BD-3b) the five-genre taxonomy **collapsed to one generic router**: the five `IGameServices` getters are now cosmetic bridges over a single `DomainEventBus` (`src/DualFrontier.Core/Bus/GameServices.cs`), and delivery no longer partitions by genre. The `[SystemAccess(..., bus: nameof(IGameServices.Combat))]` form is deleted with the taxonomy (F-54) — `[SystemAccess]` now carries reads/writes only.

Why the collapse, not five independent buses: the original rationale ("a single bus is a lock-contention bottleneck past ~100 systems") never materialized — no shipped workload approached it, and the genre partitioning instead silently dropped cross-genre mod deliveries. Delivery mechanics and the retired genre rationale are now [EVENT_BUS.md](./EVENT_BUS.md)'s domain (§1 records the inversion; §4 the mod routing). The event/component types that once populated these buses migrate to the vanilla slice mods in later waves ([VANILLA_SEPARATION_MIGRATION_PLAN.md](./VANILLA_SEPARATION_MIGRATION_PLAN.md) §5); the engine keeps only `IEvent` + generic routing (BD-3). The former Writers/Readers/Key-events census is recoverable from this file's git history at ≤ `6b0b7d6`.

## §3 IModContract — the mod-to-mod channel

Mods cannot reference each other's assemblies — each loads into its own `AssemblyLoadContext` ([MODDING.md](./MODDING.md)). `IModContract` (`Modding/IModContract.cs`) is the sanctioned alternative: publish via `IModApi.PublishContract<T>`, retrieve via `IModApi.TryGetContract<T>(out T? contract)` (`IModApi.cs:85-93`) — `false` means degrade gracefully, not crash:

```csharp
public interface IVoidMagicContract : IModContract { bool CanCastVoid(EntityId caster); }

public class ArtifactMod : IMod
{
    public void Initialize(IModApi api)
    {
        if (api.TryGetContract<IVoidMagicContract>(out var vm))
            api.Subscribe<VoidSpellCastEvent>(OnVoidMagicDetected);
        // Not loaded: simply don't subscribe. No crash, no hard dependency.
    }
}
```

The contract interface lives where both mods can see it — typically a `ModName.Contracts` package, or (per `ModKind.Shared`, `ModManifest.cs:7-17`) a type-vendor assembly in the shared ALC. Both are structurally legal; only the fixture test suite exercises the Shared path today — all seven shipped mods reference `DualFrontier.Contracts` alone.

## §4 Contract evolution

Every change is breaking or non-breaking. Two predecessor classifications were optimistic; both corrected here.

**Non-breaking (verified safe).** A new `IEvent`/`IQuery`/`ICommand`/`IComponent` type — no existing consumer references a type that doesn't exist yet. A new **optional** `init` property on a property-body `record` (no positional constructor), via object-initializer syntax — every production event record verified here uses this shape (`PawnSpawnedEvent.cs:10-20`: `PawnId`/`X`/`Y` are `{ get; init; }`, not positional). Safe only while the record stays property-body (a positional record's constructor signature changing breaks binary compat for old callers) and the new property is not `required` (that forces every site, including existing ones, to set it). An interface method with a C# 8+ default implementation.

**Breaking (major bump required).** Removing/renaming an `IEvent`/`IQuery`/`ICommand`/`IComponent` field. Removing an interface method or changing a parameter type. **Removing an interface entirely** — W2/BD-3a removed the five bus interfaces + `IGameServices` from the assembly (§2), the breaking change this document's 2.0.0 bump records. **Adding a member to any members-bearing contract interface without a default implementation** — the predecessor called a new bus non-breaking; it is not. Every direct implementer (and any mod-side test double implementing the interface) fails to compile without the match. (The former worked example was `IGameServices`; it left the contract assembly at W2 — §2 — so it is no longer a contract-evolution concern, but the principle stands for every remaining members-bearing interface.) Minor-safe only if the new member ships a default implementation from day one — and even then a silently-inherited default is the correctness risk the version bump exists to flag. Changing phase semantics (`[Deferred]` ⇄ synchronous): [EVENT_BUS.md](./EVENT_BUS.md) owns phase-semantics truth; land the change there first.

### §4.1 W1 SDK system-authoring surface (added 2026-07-19, MINOR)

W1 (VANILLA_SEPARATION_MIGRATION_PLAN BD-1) added the SDK system-authoring surface to `DualFrontier.Contracts` — new TYPES only, so non-breaking (MINOR); `ContractsVersion.Current` is unchanged:

- `Sdk/ISimulationSystem` — the mod/vanilla system contract (`Initialize`/`Tick`/`OnDispose` over `ISystemContext`; no `float delta` — SimTick arrives via the context).
- `Sdk/ISystemContext` — the per-tick capability surface (component access over the measured span/batch/try-has-get/intern/composite union, `Publish`/`Subscribe` routed through the live capability gate, `CurrentTick`). NO fields/managed-store/services — a deliberate day-one omission (audience-driven deferral, N17).
- `Sdk/ISystemServices` — the construction-time DI surface (`Pathfinding` only, extensible).
- `Sdk/SpanScope<T>` / `Sdk/WriteScope<T>` — allocation-free `ref struct` scopes over the read lease / write batch; `Sdk/StringHandle` / `Sdk/CompositeHandle<T>` — unmanaged value handles. All forge-proof (internal ctors; a mod cannot fabricate one — `InternalsVisibleTo` to `DualFrontier.Application`, an engine→engine grant that does not move the BoundaryRatchet).
- `Services/IPathfindingService` — RELOCATED from `DualFrontier.AI` (its only types, `GridVector` + BCL, were already Contracts-safe) so `ISystemServices` can name it (boundary law B-3, SDK sufficiency). The concrete `AStarPathfinding` stays engine-side.

`SystemBase`'s authoring path remains as the recorded bridge (retires at W5, when the last `src/` harness system migrates); engine-side `SystemAdapter<T>` (in `DualFrontier.Application`) wraps an `ISimulationSystem` onto the executor.

## §5 Versioning and the version gate

`DualFrontier.Contracts` versions as `MAJOR.MINOR.PATCH` (`Modding/ContractsVersion.cs`); the running build is the hardcoded `ContractsVersion.Current` (`:20`, presently `2.0.0` after the W2/BD-3a bus-interface removal — §2, §4), bumped manually per breaking release.

Two declaration paths, both wired into production (not test-only):

- **v1 legacy** — `RequiresContractsVersion` (JSON key `requiresContractsVersion`, not `requiresContracts`; `ManifestParser.cs:66`), checked by `ContractsVersion.IsCompatible` (`:84-94`).
- **v2 typed** — optional `ApiVersion` (JSON key `apiVersion`), a `VersionConstraint` (caret or exact; tilde rejected, `VersionConstraint.cs:56-61`), checked by `IsSatisfiedBy` (`:100-110`).

Both run inside `ContractValidator.ValidateContractsVersions` (`ContractValidator.cs:124-178`), invoked from `ModIntegrationPipeline`'s production `Apply` (`:371`) — real, wired Phase-A enforcement, unit-tested (`ContractValidatorTests.cs`), though nothing in-repo re-runs those tests on every change (there is no CI).

State the direction precisely: **not** "refuses newer major only." Both paths require the running build's major to **exactly equal** the mod's required major — a mismatch either direction is refused. Within a matching major it is a genuine floor: required minor.patch must be ≤ the running build's, with no way to express a ceiling short of the next major — except the v2 exact form (`apiVersion` without `^`), which pins the version outright.

```json
{ "manifestVersion": "3", "id": "com.example.voidmagic", "apiVersion": "^2.0.0" }
```

## §6 Enforcement reality

The predecessor pointed to a "future A'.9 Roslyn analyzer" verifying that bus publications match `[SystemAccess]`. A'.9 has since shipped: [ANALYZER_RULES.md](./ANALYZER_RULES.md) documents 17 enforced rules, wired into all 12 `src/` projects. None of the 17 is a bus-publish-scoping check, and none of the deferred IDs (DFK012, DFK015, DFK018, the DFK020 family) names one either.

**W2/BD-3 dissolved this gap rather than filling it.** `[SystemAccess]` no longer declares buses — its `bus:`/`Buses` members were deleted with the genre taxonomy (F-54, W2 C2/C4) — and one generic router replaced the five genre buses (§2). There is no per-genre publish scope left for an analyzer to verify; the capability gate (`kernel.publish:<FQN>`, MOD_OS_ARCHITECTURE.md §3.6) is now the only publish-authorization boundary, and it is enforced at runtime, not by dependency graph.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| EVENT_BUS.md | defers-to | delivery/phase semantics |
| MOD_OS_ARCHITECTURE.md | defers-to | capability grammar, ALC, manifest schema |
| MODDING.md | cites | ALC resolution truth (no refusal list) + author guide |
| ECS.md | defers-to | component storage/access rules |
| ANALYZER_RULES.md | cites | shipped enforcement (17 rules, A'.9.1) |
| KERNEL_ARCHITECTURE.md | cites | К-L3/К-L4 storage-path + type-id law, К-L9 |
| ARCHITECTURE.md | cites | umbrella census; sibling document |
| EXECUTION_AUTHORITY_MATRIX.md | cites | R5/R6 event-routing cutover context |

## Amendment protocol

Tier 1, LOCKED — amendments via FRAMEWORK.md §7.2 protocol. Amendment: surface the change to the owner (Crystalka); semver per FRAMEWORK.md §7.2 (PATCH correction, MINOR additive, MAJOR breaking-surface/evolution-rule change); propagate to citing documents.

## Change history

| Version | Date | Change |
|---|---|---|
| **2.0.0** | 2026-07-19 | **MAJOR — W2_BUS_CAPABILITY C7 (BD-3a/BD-3b).** The five bus interfaces + `IGameServices` left `DualFrontier.Contracts` for `DualFrontier.Core.Bus` (breaking interface removal); `ContractsVersion.Current` 1.0.0 → 2.0.0 (`:20`). §2 rewritten from the five-bus canon to the departure record + the one-router collapse (getters now cosmetic bridges); §4 generalizes the members-bearing-interface breaking rule off the departed `IGameServices`, and names interface *removal* as breaking; §5 current version corrected 1.0.0 → 2.0.0; §6 bus-publish-scoping gap dissolved with `[SystemAccess(bus:)]` (F-54). Scope row updated. EVT-2026-07-19-W2_BUS_CAPABILITY. |
| 0.1.1 | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R2-1/R2-2/R2-3): SEED-2 cross-reference row reworded to the refusal-list-retirement truth ("ALC resolution truth (no refusal list) + author guide", matching ARCHITECTURE.md's row); §5 floor claim gains the v2 exact-pin exception; §5 JSON illustration upgraded to a valid v3 manifest fragment (`manifestVersion` + `apiVersion`). |
| **0.1.0** (AUTHORED, pending ratification) | 2026-07-15 | Reclassified `IGameServices`-property and record-field evolution as breaking/caveated (§4); stated the version gate's true exact-major-both-directions behavior, fixed the stale JSON key (§5); replaced the stale "future A'.9 analyzer" pointer with verified enforcement reality (§6); real `HealthComponent` struct example (§1). |
| 1.1 | 2026-05-12 | Predecessor's last LOCKED version. See `historical/CONTRACTS.md`. |
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-J-COMBO_RESOLUTION
category: J
tier: 1
lifecycle: Draft
owner: Crystalka
version: "0.1.0"
next_review_due: 2027-07-15
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-J-COMBO_RESOLUTION
---
# Combo damage resolution

Game-design rule for merging simultaneous damage from multiple systems onto one target into a single deterministic application order.

> **Document class: mechanic design (Category J, Tier 1, Draft).** Reclassified from Category A per the corpus rework of 2026-07-15; successor of `docs/architecture/historical/COMBO_RESOLUTION.md` (DOC-A-COMBO_RESOLUTION, now SUPERSEDED). Describes game-mechanic intent; the engine surfaces it constrains are listed in `constrains`. Ratifiable Draft → LOCKED per FRAMEWORK §7 when the mechanic ships.

## Status

| Field | Value |
|---|---|
| Role | mechanic-design-draft |
| Successor of | `docs/architecture/historical/COMBO_RESOLUTION.md` (DOC-A-COMBO_RESOLUTION, now SUPERSEDED) |
| Scope | The game-design rule for merging simultaneous damage from several systems onto one target into one deterministic application order, and the mod-facing contract (damage-kind ordinal registration) that comes with it. |
| Non-goals | Does not define scheduler phase/ordering mechanics, bus delivery semantics, component write semantics, or determinism-class law — those belong to architecture (see Defers to). Does not assert that `ComboResolutionSystem` is executable today. |
| Authority domains | Combat: which `DamageKind` wins a simultaneous-hit tie (ordinal priority) and why per-target atomicity is a design requirement; the game-design want that combat be replayable/deterministic (the *want*, not the engine mechanism that would deliver it). |
| Defers to | [THREADING.md](../architecture/THREADING.md) — scheduler phase model and parallel-execution ordering; [EVENT_BUS.md](../architecture/EVENT_BUS.md) — Intent→Granted two-step bus mechanics; [TIME_AND_CONSISTENCY_MODEL.md](../architecture/TIME_AND_CONSISTENCY_MODEL.md) (AUTHORED draft) — determinism-class vocabulary (§5) |
| Constrains | [EVENT_BUS.md](../architecture/EVENT_BUS.md) (DOC-A-EVENT_BUS_V2), [ECS.md](../architecture/ECS.md) (DOC-A-ECS_V2) |

Dual Frontier lets several systems deal damage to the same target in the same tick: physical from `CombatSystem`, magical from `SpellSystem`, status from `StatusEffectSystem`. If each system wrote damage independently, the combat outcome would depend on scheduling order. The combo-resolution mechanic is the game-design answer: every damage system publishes a `DamageIntent` instead of applying damage directly, and a single mediator system collects, sorts, and applies them in a fixed order.

> **Implementation status (verified at HEAD `35364c2`, 2026-07-15).** This document is **mechanic design, not shipped behavior**. `ComboResolutionSystem` (`src/DualFrontier.Systems/Combat/ComboResolutionSystem.cs`) exists as a scaffolded stub tagged `[ReservedStub(ReservedStubPurpose.BuildComposition, …)]`: both `OnDamageIntent` (`ComboResolutionSystem.cs:60`) and `ResolvePending` (`ComboResolutionSystem.cs:76`) `throw new NotImplementedException`; `Update` is an empty TODO (`:42-45`). `DamageKind.cs` (the ordinal table in §3) and the `ComboResolutionTests` replay suite (§4) do not exist anywhere in the repository (re-verified, zero matches). `DamageIntent` exists (`src/DualFrontier.Events/Combat/DamageIntent.cs:15-19`) but its `DamageKind` field is a raw `string` (`:19`) with an inline `TODO: Phase 4 — replace with the DamageType enum` — note the code's own planned name (`DamageType`) does not match this document's `DamageKind.cs` — the exact future type is not yet settled. `DamageEvent` exists (`src/DualFrontier.Events/Combat/DamageEvent.cs`) but carries no data today; every field is commented out as TODO (`:12-16`). `IModApi.RegisterDamageKind`, the mod-registration hook described in §3, has no matching member on `IModApi` today (verified, zero matches repo-wide). Forward milestone: the original content-roadmap's Phase 4/5 (Combat) scope is now [ROADMAP.md](../ROADMAP.md) **M9 — Vanilla.Combat**, specified in detail (naming the sibling composite-shot mechanic end-to-end; this mechanic's scope rides M9's damage-model bullet and its bridge-stub `replaces` clause) but currently marked **DEFERRED** pending the V substrate. `ComboResolutionSystem`'s own `[BridgeImplementation(Phase = 5, …)]` tag (`:29`) uses that same original-content-Phase numbering, not a scheduler `PhaseId` — see the footnote in §2.

## 1. Problem

Applying damage directly in event-publish order yields a different result for the same world state. Scenario: a pawn is hit at the same time by a bullet, a fireball, and a poisoned dart.

- If fire is applied first, the pawn may pick up a `Burning` status that increases physical-damage vulnerability, and the bullet hits harder.
- If the bullet is applied first, there is no vulnerability and the bullet does standard damage.
- If poison is applied first and the pawn dies from the status, the rest of the damage "vanishes" and the kill counter goes to the dart, not the bullet.

Each outcome is admissible by the rules, but the choice would depend on the order the scheduler happens to run `CombatSystem`, `SpellSystem`, and `StatusEffectSystem` in — i.e. it would be non-deterministic (systems in one phase run in parallel with no guaranteed order; see [THREADING.md](../architecture/THREADING.md) for the current phase/scheduling model). Left unresolved: replay breaks, and integration tests go flaky.

## 2. Solution

Every damage system publishes a single `DamageIntent` instead of applying damage directly. `ComboResolutionSystem` collects the queue of intents for the tick, sorts it by a stable key, and applies them in strict order, publishing the resulting `DamageEvent`s.

```
Within one SimTick, once the damage-intent producers have run:

  CombatSystem       ─┐
  SpellSystem        ─┤── DamageIntent[] ──► ComboResolutionSystem
  StatusEffectSystem ─┘                                │
                                                        │ sort by (EntityId, DamageKind ordinal)
                                                        ▼
                                               apply in strict order
                                                        │
                                                        ▼
                                              publish DamageEvent (one per applied intent)
```

*Phase-numbering footnote.* Three unrelated things in this corpus are called "Phase N" and should not be conflated: (a) the scheduler's old five-phase scaffold ("Phase 4 — Apply & Damage"), which [THREADING.md](../architecture/THREADING.md) v2.0.0 removed — current phase count is scheduler-computed and data-dependent, so this document no longer names one; (b) `ComboResolutionSystem`'s `[BridgeImplementation(Phase = 5, …)]` tag (`ComboResolutionSystem.cs:29`), which is the *original content-roadmap* numbering (Combat was originally "Phase 5") now tracked as [ROADMAP.md](../ROADMAP.md) M9; (c) the `TODO: Phase 4` comments inside the stub methods themselves, which also use the original content-roadmap numbering. None of the three is a live scheduler `PhaseId`.

## 3. Sort key

The sort is lexicographic on the pair `(EntityId, DamageKind ordinal)`:

1. First, every intent is grouped by `EntityId` — one target is processed atomically.
2. Within a group, the order is set by the `DamageKind` ordinal.

The intended ordinal table (design target — `DamageKind.cs` does not exist yet, see the banner above):

| DamageKind | Ordinal |
|------------|---------|
| Physical   | 0       |
| Fire       | 1       |
| Arcane     | 2       |
| Status     | 3       |

The string `DamageKind` field is not meant to participate in the sort directly — only through the constant table above. This matters for mods: a mod that adds a new damage type would register it via `IModApi.RegisterDamageKind(name, ordinal)` and MUST pick a non-conflicting ordinal. Without registration, a mod's damage would go to the end of the queue. As the banner states, this registration hook does not exist on `IModApi` today — it is a mod-contract requirement this mechanic imposes once built, not a shipped capability.

### Open question: ordinal collisions

The sort key `(EntityId, DamageKind ordinal)` is **not total**: two intents of the *same* kind on the *same* target in the *same* tick (e.g. two simultaneous physical hits from a shotgun spread) tie, and neither the predecessor document nor this one says how ties break. [TIME_AND_CONSISTENCY_MODEL.md](../architecture/TIME_AND_CONSISTENCY_MODEL.md) (AUTHORED draft) §5.2 names the same gap under condition **C1** for its D2-island proposal and suggests a tiebreaker (publisher `SystemId` + per-publisher intent sequence). Until ratified, treat same-kind ties as an open design question, not implemented behavior.

## 4. Determinism guarantee (design intent)

Given identical input world state and an identical set of `DamageIntent`s, the design intent is that the resulting `DamageEvent` stream is byte-identical between runs: a replay system could reproduce a fight from a command log without storing intermediate state, combat tests would not flake on thread-schedule, and network sync (if ever added) would only need to replicate the `DamageIntent` stream.

This is a **determinism class D2 (bounded island)** claim in the vocabulary of [TIME_AND_CONSISTENCY_MODEL.md](../architecture/TIME_AND_CONSISTENCY_MODEL.md) (AUTHORED draft) §5: identical initial state + identical input stream ⇒ byte-identical event stream. That draft names this exact guarantee explicitly in its §5.1 claim-assignment table and records the class actually held **today** as **D0** — best-effort, no cross-run guarantee (the corpus-wide engine default; `ComboResolutionSystem` throwing `NotImplementedException` means no ordering is exercised at all yet). §5.2 offers two ratifiable paths, neither chosen yet:

> **FENCED (target / planned — not current truth):**
> - **Option A — downgrade to D1 scope.** Re-word the guarantee to *stable intra-tick ordering only*: given one world state and one `DamageIntent` multiset within a tick, the applied order and resulting `DamageEvent` set are identical regardless of thread schedule — replay-from-command-log is dropped.
> - **Option B — commit to D2 as a bounded determinism island.** The engine stays D1 overall; only the `DamageIntent → ComboResolutionSystem → DamageEvent` path is D2, gated on six named conditions (total sort key — see the ordinal-collision question above; no GPU-derived inputs; the RNG law; bit-stable arithmetic; intent-stream capture; D1-stable island inputs). See TIME_AND_CONSISTENCY_MODEL.md §5.2 conditions C1–C6 for the full list.
>
> Until one option is ratified, this document's determinism guarantee reads as **aspirational D2, effective D0** — the same reading the predecessor document's NON-NORMATIVE banner already licensed.

Consequences once/if D2 is reached (the game-design payoff, unchanged from the original design):

- A replay system can reproduce a fight from a command log without storing intermediate state.
- No flaky combat tests: application order is fixed rather than determined by the thread scheduler.
- Network synchronization (if added later) does not require replicating damage events — the `DamageIntent` stream is enough.

## 5. Why sort and not a queue

One could imagine applying damage in publish order through a lock-free queue instead of sorting. Rejected, because:

- Publish order depends on system traversal order inside the scheduler's parallel phase execution (see [THREADING.md](../architecture/THREADING.md) for the current model).
- The scheduler does not guarantee a stable order among systems running in the same phase.
- Even with an ordered queue, two systems publishing from different threads can interleave differently run to run.

Sorting on a stable key removes the dependency on whoever published first. The game-design payoff this document owns is *that* determinism is wanted; *how* the scheduler achieves parallel-safe delivery is THREADING's and EVENT_BUS's law, not this document's.

## 6. Relationship to composite requests

`ComboResolutionSystem` and `CompositeResolutionSystem` ([COMPOSITE_REQUESTS.md](./COMPOSITE_REQUESTS.md)) solve related but different game-design problems:

- **Composite** collects **heterogeneous** responses from **different buses** for one transaction and emits a single result.
- **Combo** collects **homogeneous** `DamageIntent`s on a single target and orders them for application.

Both are mediator-pattern mechanics that collect intents before committing an outcome; they do not compete, and a single action (e.g. an enchanted shot that both consumes ammo/mana AND deals damage) can pass through both in sequence.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [EVENT_BUS.md](../architecture/EVENT_BUS.md) | constrains | This mechanic requires the two-step Intent→Granted collection model; EVENT_BUS.md is the substrate it constrains. |
| [ECS.md](../architecture/ECS.md) | constrains | Requires atomic per-target component/event writes; ECS.md's write-batch/phase-commit semantics are the substrate it constrains. |
| [THREADING.md](../architecture/THREADING.md) | defers-to | Scheduler phase model and parallel-execution ordering guarantees — this document does not define them. |
| [TIME_AND_CONSISTENCY_MODEL.md](../architecture/TIME_AND_CONSISTENCY_MODEL.md) (AUTHORED draft) | defers-to | Determinism classes D0/D1/D2 and the COMBO-vs-VULKAN resolution options (§5, §5.2). |
| [COMPOSITE_REQUESTS.md](./COMPOSITE_REQUESTS.md) | cites | Sibling mediator-pattern mechanic — multi-bus transactions vs single-target damage merge. |
| [ROADMAP.md](../ROADMAP.md) | cites | M9 — Vanilla.Combat, the forward milestone that would implement this mechanic (currently DEFERRED). |

## Amendment protocol

While Draft, revise freely as the combat mechanic design evolves (FRAMEWORK §3.3 Draft mutability) — no deliberation milestone required. Ratification to LOCKED requires a Crystalka deliberation milestone per FRAMEWORK §7 (amendment milestone protocol) once `ComboResolutionSystem` ships real behavior; from LOCKED onward, Tier 1 change authority (FRAMEWORK §3.2.1) governs further amendment.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review correction (CORPUS_CLOSURE_INVERSION_B, D1 R4-1): banner's ROADMAP-M9 parenthetical no longer attributes the composite-shot chain quote to "this exact mechanic" — M9 names the sibling COMPOSITE_REQUESTS mechanic end-to-end; combo's scope rides the damage-model bullet + bridge-stub `replaces` clause. |
| 0.1.0 | 2026-07-15 | Reclassified from Category A (`DOC-A-COMBO_RESOLUTION`, now historical) to Category J per FRAMEWORK §3.1; restyled with a re-verified implementation-status banner; engine-law statements (scheduler phase claims, bus mechanics) removed and rerouted to THREADING.md / EVENT_BUS.md / TIME_AND_CONSISTENCY_MODEL.md (AUTHORED draft). |

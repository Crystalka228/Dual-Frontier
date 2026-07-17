---
register_id: DOC-J-COMPOSITE_REQUESTS
project: Dual Frontier
category: J
tier: 1
lifecycle: Draft
owner: Crystalka
version: 0.1.0
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: 2027-07-15
title: Composite requests — multi-bus transaction mechanic design (Category J; reclassified from DOC-A-COMPOSITE_REQUESTS)
supersedes:
- DOC-A-COMPOSITE_REQUESTS
constrains:
- DOC-A-EVENT_BUS_V2
- DOC-A-CONTRACTS_V2
last_modified_commit: 0145f1b
review_cadence: on-change+annual
reviewer: Crystalka
---

# Composite requests

Game-design rule for actions that must atomically succeed or fail across more than one resource bus at once (for example, a shot that needs both ammunition and mana).

> **Document class: mechanic design (Category J, Tier 1, Draft).** Reclassified from Category A per the corpus rework of 2026-07-15; successor of `docs/architecture/historical/COMPOSITE_REQUESTS.md` (DOC-A-COMPOSITE_REQUESTS, now SUPERSEDED). Describes game-mechanic intent; the engine surfaces it constrains are listed in `constrains`. Ratifiable Draft → LOCKED per FRAMEWORK §7 when the mechanic ships.

## Status

| Field | Value |
|---|---|
| Role | mechanic-design-draft |
| Successor of | `docs/architecture/historical/COMPOSITE_REQUESTS.md` (DOC-A-COMPOSITE_REQUESTS, now SUPERSEDED) |
| Scope | The game-design rule for actions that need a coordinated yes/no across two or more resource buses in one tick (compound shot, craft, ritual), and the rollback game-design contract when only part of the action is granted. |
| Non-goals | Does not define the engine's transaction/stage vocabulary, bus delivery semantics, or mod-contract versioning — those belong to architecture (see Defers to). Does not assert that `CompositeResolutionSystem` is executable today. |
| Authority domains | Combat/Magic/Crafting: which composite actions exist (compound shot; craft; ritual) and their refusal/refund game-design semantics — not the engine mechanism that would deliver atomicity. |
| Defers to | [EVENT_BUS.md](../architecture/EVENT_BUS.md) — Intent→Granted/Refused bus delivery; [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](../architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (AUTHORED draft) — the canonical "transaction"/stage vocabulary; [THREADING.md](../architecture/THREADING.md) — phase timing |
| Constrains | [EVENT_BUS.md](../architecture/EVENT_BUS.md) (DOC-A-EVENT_BUS_V2), [CONTRACTS.md](../architecture/CONTRACTS.md) (DOC-A-CONTRACTS_V2) |

A one-step Intent solves the problem when exactly one resource on one bus is required. But even a "mana arrow" shot needs two confirmations at once: a bullet on the Inventory bus and mana on the Magic bus. A direct "get both responses and act" approach produces race conditions inside a multithreaded phase. This mechanic's answer is a two-phase collect through a single mediator system.

> **Implementation status (verified at HEAD `35364c2`, 2026-07-15).** This document is **mechanic design, not shipped behavior**. `CompositeResolutionSystem` (`src/DualFrontier.Systems/Combat/CompositeResolutionSystem.cs`) has **six** stub methods, all tagged `[ReservedStub(ReservedStubPurpose.BuildComposition, …)]` and all `throw new NotImplementedException`: `OnCompoundShotIntent` (`:63`), `OnAmmoGranted` (`:79`), `OnAmmoRefused` (`:94`), `OnManaGranted` (`:111`), `OnManaRefused` (`:126`), `TryResolve` (`:145`); `Update` performs no work (`:45-48`). `AmmoRefunded` (§4), `CompoundCraftIntent`, and `CompoundRitualIntent` (§5) do not exist anywhere in the repository (re-verified, zero matches). `CompoundShotIntent`, `ShootGranted`, `ShootRefused`, and `ShotRefusalReason` do exist as plain event/enum types (`src/DualFrontier.Events/Combat/`). `TransactionId` also exists (`src/DualFrontier.Events/Combat/TransactionId.cs:11`) but its own factory is a stub: `TransactionId.New()` throws `NotImplementedException` (`:23`), with a TODO noting the intended implementation is `Interlocked.Increment` on a private field — not yet built. Forward milestone: the original content-roadmap's Phase 4/5 (Combat) scope is now [ROADMAP.md](../ROADMAP.md) **M9 — Vanilla.Combat**, which names this exact mechanic ("the composite shot pattern… `CompoundShotIntent` → `CompositeResolutionSystem` → `ShootGranted`/`ShootRefused` — implemented end-to-end") but is currently marked **DEFERRED** pending the V substrate. `CompositeResolutionSystem`'s `[BridgeImplementation(Phase = 5, …)]` tag (`:31`) uses that same original-content-Phase numbering, not a scheduler `PhaseId` — see [COMBO_RESOLUTION.md](./COMBO_RESOLUTION.md)'s Solution-section phase-numbering footnote for the general caveat.

## 1. Problem

The initiator (e.g. `CombatSystem`) publishes two independent intents: `AmmoIntent` and `ManaIntent`. Both are processed by batch systems within one phase in parallel. Four outcomes are possible: (granted, granted), (granted, refused), (refused, granted), (refused, refused). In three of four cases the action is impossible, but the partially debited resources have already been spent — the bullet is reserved, mana is gone — and the rollback has to be done manually on each bus's side. Any error is a permanent leak.

On top of that: the initiator must collect both responses and figure out that they are about the same action. In batches with hundreds of intents, correlating by requester alone breaks down — one pawn within a tick may receive ammo and mana confirmations from completely unrelated events.

## 2. Solution

A request that touches N buses is published **once** as `CompoundShotIntent` (or another composite type) with a shared `TransactionId`. A dedicated mediator system, `CompositeResolutionSystem`, subscribed to every involved bus, collects partial responses by `TransactionId` and publishes the final `ShootGranted` or `ShootRefused` (with `ShotRefusalReason`).

### Sequence

```
 Shooter                 Combat              Inventory           Magic           CompositeResolution
    │                       │                    │                 │                      │
    │── CompoundShotIntent ─┤                    │                 │                      │
    │    (TransactionId)    │                    │                 │                      │
    │                       ├───── AmmoIntent ──►│                 │                      │
    │                       ├────────────── ManaIntent ───────────►│                      │
    │                       │                    │                 │                      │
    │                       │                 (batch phase)                                │
    │                       │                    │                 │                      │
    │                       │◄── AmmoGranted ────┤                 │                      │
    │                       │                    │                 │                      │
    │                       │◄──────────────── ManaGranted ────────┤                      │
    │                       │                    │                 │                      │
    │                       ├── forwards partials by TransactionId ────────────────────►  │
    │                       │                                                              │
    │                       │                                   (collects pair)            │
    │                       │                                                              │
    │                       │◄──────────── ShootGranted ──────────────────────────────────┤
    │◄── ShootGranted ──────┤                                                              │
    │                       │                                                              │
```

`CompositeResolutionSystem` mediates between the intent-collection and application steps of a tick — see [THREADING.md](../architecture/THREADING.md) for the current phase model (this document does not name a specific phase number; see [COMBO_RESOLUTION.md](./COMBO_RESOLUTION.md)'s Solution-section footnote on the three overloaded meanings of "Phase N" in this corpus).

## 3. Determinism rules and the TransactionId vocabulary

- `TransactionId` is meant to be monotonic within one tick: the design intent is an atomic counter held by `CompositeResolutionSystem`.
- Every partial response is meant to carry its `TransactionId`. Without one, a response would be treated as part of an ordinary Intent/Granted exchange and not enter composite resolution.
- `CompositeResolutionSystem` is meant to keep a `TransactionId → PartialState` dictionary with the expected response count. As soon as every response is collected, the final result publishes and the entry is removed.
- If fewer responses than expected arrive by the end of the phase, the transaction is meant to be marked stranded and `ShootRefused` published with reason `PartialTimeout`.

**Vocabulary note.** [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](../architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (AUTHORED draft) §1 defines the corpus's canonical "transaction" vocabulary: a seven-stage machine (prepare → validate → quiesce → commit → reclaim → recover → resume) for engine-level transitions such as mod apply/unload and world shutdown. This mechanic's "transaction" (`TransactionId`) is a narrower, tick-scoped request-correlation concept — collecting partial bus responses for one composite intent — and does not itself instantiate that seven-stage machine. Where the two vocabularies might be confused, ENGINE_LIFECYCLE_AND_TRANSACTIONS.md's is authoritative for the term "transaction" at the engine level; this document's "two-phase commit" language is game-mechanic color, not a claim on that vocabulary.

### Open question: who assigns the TransactionId, and when?

The sequence diagram in §2 shows the **Shooter** already carrying a `TransactionId` when it first publishes `CompoundShotIntent` — implying the ID must be minted *before* `CompositeResolutionSystem` ever observes the request. But the prose above says the counter is "held by `CompositeResolutionSystem`" — implying the mediator is the one who assigns it. The two descriptions are in tension: either the Shooter (or `CombatSystem`, intercepting `CompoundShotIntent` first) needs its own way to mint an ID before forwarding `AmmoIntent`/`ManaIntent`, or the diagram is incomplete about an allocation round-trip. Neither the predecessor document nor this one resolves it, and the code does not help settle it either: `TransactionId.New()` is a bare stub (`TransactionId.cs:23`) with no caller shown anywhere. This is an open design question, not implemented behavior.

## 4. Rollback on partial refusal

> **FENCED (target / planned — not current truth):** If one bus answers Granted and the other Refused, the design intent is that the already-debited resource is returned via a Refund event on the corresponding bus. For example, `AmmoGranted` means the bullet is already reserved in `InventorySystem`. On final `ShootRefused`, `CompositeResolutionSystem` would publish `AmmoRefunded`, and `InventorySystem` would return the bullet to storage. Refund events are meant to be marked `[Deferred]` — their delivery must not compete with new intents in the same phase; the bus receiving a Refund would process it in the next phase alongside new Intents.
>
> None of this is built: `AmmoRefunded` does not exist as a type (verified, zero matches), and `CompositeResolutionSystem` has no refund-publishing code path (all six of its methods are stubs — see the banner above).

## 5. Generalization

Any request that touches more than one bus is meant to go through `CompositeResolutionSystem`. The design's list of supported composite intents:

- `CompoundShotIntent` — bullet + mana (enchanted shot). **Exists as a type today**; unhandled (see the banner above).
- `CompoundCraftIntent` — materials + power + workbench.
- `CompoundRitualIntent` — ether + mana + anchor item.

> **FENCED (target / planned — not current truth):** `CompoundCraftIntent` and `CompoundRitualIntent` do not exist anywhere in the repository (re-verified, zero matches) — both are pure forward design. Each composite intent is meant to be described by its own pair of `...Granted` / `...Refused` events with a corresponding refusal enum, following `CompoundShotIntent` / `ShootGranted` / `ShootRefused` / `ShotRefusalReason` as the template.

## 6. Related task: damage merging

The same pattern of "collect many intents and emit one deterministic result" is used for damage, but with a different mediator: `ComboResolutionSystem`. See [COMBO_RESOLUTION.md](./COMBO_RESOLUTION.md). The difference: composite waits for responses from different buses for one transaction; combo collects homogeneous `DamageIntent`s on a single target and orders them.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [EVENT_BUS.md](../architecture/EVENT_BUS.md) | constrains | This mechanic requires the two-step Intent→Granted/Refused model and `[Deferred]` refund delivery; EVENT_BUS.md is the substrate it constrains. |
| [CONTRACTS.md](../architecture/CONTRACTS.md) | constrains | Composite intent/event types (`CompoundCraftIntent`, `CompoundRitualIntent`, and their responses) are mod-extensible surface; CONTRACTS.md is the type-contract substrate it constrains. |
| [THREADING.md](../architecture/THREADING.md) | defers-to | Phase timing between intent collection and application — this document does not define it. |
| [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](../architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (AUTHORED draft) | defers-to | Canonical engine "transaction"/seven-stage vocabulary (§1); this document's `TransactionId` is a narrower, distinct concept (§3). |
| [COMBO_RESOLUTION.md](./COMBO_RESOLUTION.md) | cites | Sibling mediator-pattern mechanic — single-target damage merge vs multi-bus transactions. |
| [RESOURCE_MODELS.md](./RESOURCE_MODELS.md) | cites | Choosing between Intent and Lease for the individual resources a composite request bundles. |
| [ROADMAP.md](../ROADMAP.md) | cites | M9 — Vanilla.Combat, the forward milestone that would implement this mechanic end-to-end (currently DEFERRED). |

## Amendment protocol

While Draft, revise freely as the composite-action design evolves (FRAMEWORK §3.3 Draft mutability) — no deliberation milestone required. Ratification to LOCKED requires a Crystalka deliberation milestone per FRAMEWORK §7 (amendment milestone protocol) once `CompositeResolutionSystem` ships real behavior; from LOCKED onward, Tier 1 change authority (FRAMEWORK §3.2.1) governs further amendment.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.0 | 2026-07-15 | Reclassified from Category A (`DOC-A-COMPOSITE_REQUESTS`, now historical) to Category J per FRAMEWORK §3.1; restyled with a re-verified implementation-status banner; "transaction" vocabulary rerouted to ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft); TransactionId-assignment ambiguity recorded as an open question (§3). |
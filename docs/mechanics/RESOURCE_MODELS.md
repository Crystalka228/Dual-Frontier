---
register_id: DOC-J-RESOURCE_MODELS
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
title: Resource models — Intent vs Lease mechanic design (Category J; reclassified from DOC-A-RESOURCE_MODELS; honesty banner added per N-7)
supersedes:
- DOC-A-RESOURCE_MODELS
constrains:
- DOC-A-EVENT_BUS_V2
last_modified_commit: 0145f1b
review_cadence: on-change+annual
reviewer: Crystalka
---

# Resource models

Game-design rule for choosing between one-shot (Intent) and continuous (Lease) resource consumption, and the reserve-then-consume mechanics a Lease uses.

> **Document class: mechanic design (Category J, Tier 1, Draft).** Reclassified from Category A per the corpus rework of 2026-07-15; successor of `docs/architecture/historical/RESOURCE_MODELS.md` (DOC-A-RESOURCE_MODELS, now SUPERSEDED). Describes game-mechanic intent; the engine surfaces it constrains are listed in `constrains`. Ratifiable Draft → LOCKED per FRAMEWORK §7 when the mechanic ships.

## Status

| Field | Value |
|---|---|
| Role | mechanic-design-draft |
| Successor of | `docs/architecture/historical/RESOURCE_MODELS.md` (DOC-A-RESOURCE_MODELS, now SUPERSEDED) |
| Scope | The game-design rule for picking between discrete (Intent) and continuous (Lease) consumption for any spendable resource (ammunition, mana, ether, shield durability), and the reserve-then-consume mechanic a Lease uses. |
| Non-goals | Does not define event-bus deferred/lease delivery mechanics — those belong to EVENT_BUS.md (see Defers to). Does not claim any of the resource-model code paths are executable today (see banner). |
| Authority domains | Combat/Magic economy: which scenario uses Intent vs Lease, and the reserve-then-consume rule that protects a Lease from being drained out from under a parallel spend — a game-balance/UX rule, not the bus mechanism that would carry it. |
| Defers to | [EVENT_BUS.md](../architecture/EVENT_BUS.md) — Intent→Granted/Refused and Lease event delivery mechanics |
| Constrains | [EVENT_BUS.md](../architecture/EVENT_BUS.md) (DOC-A-EVENT_BUS_V2) |

A resource in Dual Frontier is any quantity that one system spends and another tracks: ammunition in inventory, a mage's mana, shield durability, ritual slots. Left to individual judgement, the same scenario ends up modeled differently in different places. This mechanic pins two models and a rule for choosing between them.

> **Implementation status (verified at HEAD `35364c2`, 2026-07-15) — banner added; the predecessor document had none (session finding N-7: RESOURCE_MODELS was the one gameplay-protocol doc with an unqualified present-tense claim over a partially-implemented surface, unlike its COMBO_RESOLUTION/COMPOSITE_REQUESTS siblings).** Per-claim status:
>
> - **Intent model — mana half.** `ManaIntent`/`ManaGranted`/`ManaRefused` exist as types (`src/DualFrontier.Events/Magic/`). `ManaSystem.OnManaIntent` exists but is a stub: `throw new NotImplementedException` (`src/DualFrontier.Systems/Magic/ManaSystem.cs:61`).
> - **Intent model — ammo half.** `AmmoIntent`/`AmmoGranted`/`AmmoRefused` exist as types (`src/DualFrontier.Events/Combat/`), but `InventorySystem` (`src/DualFrontier.Systems/Inventory/InventorySystem.cs`) has **no handler for any of them at all** — not even a stub. Its only wired subscriptions are `ItemAddedEvent`/`ItemRemovedEvent`/`ItemReservedEvent` (`:36-38`), a real, working, and unrelated storage-cache mechanism. The Ammo Intent handshake this document describes is less present in code than the Mana Intent stub.
> - **Intent model — target request.** The `TargetIntent` / `TargetGranted` / `TargetRefused` trio named in the choice table below does **not exist anywhere in the repository** — re-verified, zero matches for any of the three names.
> - **Lease model.** The full surface exists on disk as scaffolding: `ManaLeaseRegistry` (`src/DualFrontier.Systems/Magic/Internal/ManaLeaseRegistry.cs`, `internal`) with methods `Open` (`:42`), `Close` (`:61`), `DrainTick` (`:78`), `TryGet` (`:95`), `ActiveCountForCaster` (`:111`) — **every one throws `NotImplementedException`**; `ManaLease` (`Internal/ManaLease.cs`) is a real record shape (`Id`, `Caster`, `DrainPerTick`, `MinDurationTicks`, `MaxDurationTicks`, `TicksElapsed`, `TotalDrained`) whose only behavior method, `AdvanceTick`, is also a stub (`:91`). `ManaSystem` holds a live `_registry = new ManaLeaseRegistry()` field (`ManaSystem.cs:33`) and declares the right `[SystemAccess]` shape, but every handler that would use it — `OnManaLeaseOpenRequest` (`:96`), `DrainActiveLeases` (`:111`), `OnManaLeaseCloseRequest` (`:130`) — is a stub too. `ManaLeaseOpenRequest`/`Opened`/`Refused`/`Closed`, `RefusalReason`, and `CloseReason` all exist as real event/enum types (`src/DualFrontier.Events/Magic/`).
> - **Verdict.** "Partially implemented" undersells it in one direction and oversells it in another: the *data shapes* for Intent (mana) and Lease are real and the method surface for Lease is complete, but **zero executable behavior exists for either model** — every handler is a `NotImplementedException` stub, consistent with the `[ReservedStub(ReservedStubPurpose.BuildComposition, …)]` convention used throughout `DualFrontier.Systems`. Forward milestone: [ROADMAP.md](../ROADMAP.md) **M10.A — Vanilla.Magic** (mana/lease side) and **M9 — Vanilla.Combat** (ammo side, currently DEFERRED).

## 1. Two models

The model is chosen by a single question: is the resource consumed once or continuously?

### Intent → Granted/Refused — discrete request

Used for one-shot consumption. The step is atomic: either the resource is granted and debited, or refused. Examples:

- One bullet for a shot (`AmmoIntent` → `AmmoGranted` / `AmmoRefused`).
- A single mana charge for an instant spell (`ManaIntent` → `ManaGranted` / `ManaRefused`).
- A one-shot target for a job (`TargetIntent` → `TargetGranted` / `TargetRefused`) — design-only; see banner.

The two-step handshake itself (publish the intent in one phase, batch response in the next) is [EVENT_BUS.md](../architecture/EVENT_BUS.md)'s mechanism, not this document's.

### Lease — continuous consumption

Used when the resource drains every tick while the action lasts. Examples:

- Spell concentration (channel): mana drains every tick while the mage holds the channel.
- Shield maintenance: shield durability or mana decreases while the shield is up.
- Active ritual: ether drains while the ritual runs.

Lease is reserve-then-consume (§4). On open, `MinDurationTicks * DrainPerTick` is reserved immediately: this guarantees the lease will not abort on the very first tick due to a parallel intent's spend. After the reservation, drain proceeds as the action plays out.

```
Open → Active (drain per tick) → Closed
```

The lifecycle and events are [EVENT_BUS.md](../architecture/EVENT_BUS.md)'s "Lease model" section, not this document's.

## 2. Choice table

| Scenario                              | Model            | Example event                                    |
|---------------------------------------|------------------|--------------------------------------------------|
| Shot, single bullet                   | Intent           | `AmmoIntent` → `AmmoGranted`                     |
| Instant spell, fixed mana             | Intent           | `ManaIntent` → `ManaGranted`                     |
| Target request for a job              | Intent           | `TargetIntent` → `TargetGranted` (design-only; see banner) |
| Spell channel, mana drain per tick    | Lease            | `ManaLeaseOpenRequest` → `ManaLeaseOpened`       |
| Shield maintenance                    | Lease            | `ManaLeaseOpenRequest` → `ManaLeaseOpened`       |
| Active ritual                         | Lease            | `ManaLeaseOpenRequest` → `ManaLeaseOpened`       |

## 3. Choice rule

- If the resource is spent **once** per action — Intent.
- If the resource is spent **every tick** — Lease.

Edge cases resolve in favor of Lease when the duration is not bounded to a single tick: it is simpler to close a lease with an explicit event than to emulate continuity through a chain of per-tick Intents.

## 4. Reserve-then-consume in detail

On `ManaLeaseOpenRequest` the lease registry (`ManaLeaseRegistry`) is meant to check for `MinDurationTicks * DrainPerTick` of mana. If insufficient — `ManaLeaseRefused` with `RefusalReason`. If sufficient — the reserve is debited from `ManaComponent` immediately, the lease receives a `LeaseId`, and `ManaLeaseOpened` publishes. From there each tick `ManaSystem` is meant to drain `DrainPerTick` from the already-reserved pool; on reserve exhaustion the lease either extends or closes. `ManaLeaseRegistry.Open` (`Internal/ManaLeaseRegistry.cs:42`) is exactly this logic and, per the banner, is currently a stub.

This differs from the naive scheme of "check at start, debit per tick": under the naive scheme a parallel intent can eat the mana before the first drain and the lease aborts even though the player has already seen the start animation.

Lease closure (design intent):

- Explicit `ManaLeaseClosed` (`[Deferred]`) event by the owner's choice.
- `MaxDurationTicks` expiration.
- Abort — mana shortage even after extension.
- Owner death (`DeathEvent`).

In every case, `CloseReason` in the event is meant to name the cause — needed by the UI and the log. `CloseReason` exists today as a 5-member enum (`src/DualFrontier.Events/Magic/CloseReason.cs`) that has already drifted from this list (it adds `SpellInterrupted` and `GolemDeactivated`, and does not spell out `MaxDurationTicks` as its own value) — a naming/coverage reconciliation this document does not resolve.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [EVENT_BUS.md](../architecture/EVENT_BUS.md) | constrains | Intent→Granted/Refused two-step delivery and the Lease model's event lifecycle are the substrate this mechanic constrains. |
| [COMPOSITE_REQUESTS.md](./COMPOSITE_REQUESTS.md) | cites | An Intent that spans multiple buses at once. |
| [COMBO_RESOLUTION.md](./COMBO_RESOLUTION.md) | cites | Combining multiple damage intents — a different merge problem over the same Intent primitive. |
| [GOLEM_OWNERSHIP.md](./GOLEM_OWNERSHIP.md) | cites | A lease-refusal reason (`NoActiveBond`) is keyed to golem-bond state. |
| [ROADMAP.md](../ROADMAP.md) | cites | M10.A — Vanilla.Magic (mana/lease) and M9 — Vanilla.Combat (ammo, DEFERRED), the forward milestones that would implement this mechanic. |

## Amendment protocol

While Draft, revise freely as the resource-economy design evolves (FRAMEWORK §3.3 Draft mutability) — no deliberation milestone required. Ratification to LOCKED requires a Crystalka deliberation milestone per FRAMEWORK §7 (amendment milestone protocol) once the Intent and Lease handlers ship real behavior; from LOCKED onward, Tier 1 change authority (FRAMEWORK §3.2.1) governs further amendment.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 | 2026-07-17 | HALT-1-ratified review correction (CORPUS_CLOSURE_INVERSION_B, D1 R4-5): §2 choice-table Target-request cell gains the design-only marker matching the §1 bullet's flag (a table-only reader no longer misses it). |
| 0.1.0 | 2026-07-15 | Reclassified from Category A (`DOC-A-RESOURCE_MODELS`, now historical) to Category J per FRAMEWORK §3.1; **added the implementation-status banner the predecessor lacked** (session finding N-7), with a per-claim verified status for the Intent and Lease surfaces; engine-law statements (bus mechanics) removed and rerouted to EVENT_BUS.md. |
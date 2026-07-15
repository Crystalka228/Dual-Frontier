---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-J-GOLEM_OWNERSHIP
category: J
tier: 1
lifecycle: Draft
owner: Crystalka
version: "0.1.0"
next_review_due: 2027-07-15
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-J-GOLEM_OWNERSHIP
---
# Golem ownership

Game-design state machine for the mage-golem bond: who owns a golem, how ownership contests and transfers, and the single-writer rule that protects the bond component. (Renamed from "Golem ownership transitions" — the old title collided with the architecture corpus's own "ownership/lifetime" vocabulary; see Status.)

> **Document class: mechanic design (Category J, Tier 1, Draft).** Reclassified from Category A per the corpus rework of 2026-07-15; successor of `docs/architecture/historical/OWNERSHIP_TRANSITION.md` (DOC-A-OWNERSHIP_TRANSITION, now SUPERSEDED). Describes game-mechanic intent; the engine surfaces it constrains are listed in `constrains`. Ratifiable Draft → LOCKED per FRAMEWORK §7 when the mechanic ships.

## Status

| Field | Value |
|---|---|
| Role | mechanic-design-draft |
| Successor of | `docs/architecture/historical/OWNERSHIP_TRANSITION.md` (DOC-A-OWNERSHIP_TRANSITION, now SUPERSEDED) — renamed to `GOLEM_OWNERSHIP.md` per session finding N-6: the old title/id collided with the architecture corpus's A2 "ownership/lifetime" vocabulary despite this document being a gameplay protocol with (per the session's audit) exactly one architecture-shaped fragment: the single-writer rule (§6). |
| Scope | The mage↔golem bond as a game mechanic: the four bond states, the seven legal transitions between them, the events a transition raises, and the game-design rationale for the single-writer rule that protects `GolemBondComponent`. |
| Non-goals | Does not define ECS write-conflict detection, the scheduler's previous-tick-snapshot mechanism, or fault-taxonomy law — those belong to architecture (see Defers to). Does not assert that any transition in §2 currently executes (see banner). |
| Authority domains | Magic: the four legal bond states and which transitions between them are game-design-legal; the bond-strength game-balance intent (harder to seize a long-held, battle-tested bond) — not the engine mechanism that would enforce either. |
| Defers to | [ECS.md](../architecture/ECS.md) — component storage and single-writer enforcement; [THREADING.md](../architecture/THREADING.md) — startup write-conflict detection and the previous-tick-snapshot rule; [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](../architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (AUTHORED draft) — fault-taxonomy law (that draft's §4; this document's own open question is §8) |
| Constrains | [ECS.md](../architecture/ECS.md) (DOC-A-ECS_V2), [THREADING.md](../architecture/THREADING.md) (DOC-A-THREADING_V2) |

A golem belongs to a mage through `GolemBondComponent`. The bond is not a binary "exists / does not exist" state — it passes through several states: bonded, contested, abandoned, transferred. This mechanic pins the full list of states, the permitted transitions, and the game-design rules for executing them.

> **Implementation status (verified at HEAD `35364c2`, 2026-07-15) — corrected from the predecessor's "shipped" self-assessment.** The predecessor document (verified 2026-06-02) declared this system "shipped" with the states/transitions below "implemented." At HEAD `35364c2` that no longer holds, and this rewrite flags the drift explicitly since neither the session report's one-line note ("verified-shipped gameplay protocol," Appendix A) nor the predecessor's own banner caught it. The **data shapes are real**: `GolemBondComponent` (`src/DualFrontier.Components/Magic/GolemBondComponent.cs:17`), `GolemSystem` (`src/DualFrontier.Systems/Magic/GolemSystem.cs:28`), `GolemOwnershipChanged` (`src/DualFrontier.Events/Magic/GolemOwnershipChanged.cs:19`), `GolemOwnershipTransferRequest` (`src/DualFrontier.Events/Magic/GolemOwnershipTransferRequest.cs:15`), and `OwnershipMode` (`src/DualFrontier.Contracts/Enums/OwnershipMode.cs:10`, four members: `Bonded`/`Contested`/`Abandoned`/`Transferred`) all exist — that part of the predecessor's "5 types exist" claim holds. But **every behavior is a stub**: `GolemSystem.OnGolemOwnershipTransferRequest` throws `NotImplementedException` (`GolemSystem.cs:57`), the previous-tick-mana-read helper `ReadPreviousTickManaState` also throws (`GolemSystem.cs:76`), and `GolemSystem.Update` performs no work (`:36-39`, TODO only). No transition in §2 currently executes. `GolemOwnershipRefused` and a standalone `GolemBondStrength` formula file remain **absent** (re-verified, zero matches repo-wide) — that part of the predecessor's "TODO Phase 6" flag still holds. `InvalidOwnershipTransitionException` (§8) also does not exist anywhere in code (zero matches): the illegal-transition guard is design intent only, despite the predecessor stating it in the present tense. Forward milestone: [ROADMAP.md](../ROADMAP.md) **M10.A — Vanilla.Magic** ("Five golem types per [GDD] §5.1… Replaces kernel `ManaSystem`, `SpellSystem`, `GolemSystem`, `EtherGrowthSystem`, `RitualSystem` bridges"), the original content-roadmap's Phase 6 scope.

## 1. States

- **Bonded** — normal state. The golem has a living mage owner, mana drips from their pool, and the golem follows orders.
- **Contested** — the bond is being contested: another mage is running an interception ritual. The current owner remains, but the `TicksSinceContested` timer ticks.
- **Abandoned** — the owner is dead or has released the bond intentionally. The golem stands inactive and is available for a new bond.
- **Transferred** — an intermediate state spanning a single tick: the bond has already been broken from the previous owner, but the new one has not formally accepted it. Used only for the correct ordering of `[Deferred]` events.

## 2. Transition table

| From          | To            | Trigger                                                          |
|---------------|---------------|--------------------------------------------------------------------|
| `Bonded`      | `Contested`   | Another mage started an interception ritual — attacks the bond.  |
| `Contested`   | `Bonded`      | The `TicksSinceContested` timer expired; the original owner held. |
| `Contested`   | `Transferred` | The attacking mage finished the ritual; the bond passes to them. |
| `Transferred` | `Bonded`      | The new owner accepts the bond (one tick to formalize).          |
| `Bonded`      | `Abandoned`   | Owner dies (`DeathEvent`) or voluntarily breaks the bond.        |
| `Contested`   | `Abandoned`   | Owner dies during contestation — the bond is released.           |
| `Abandoned`   | `Bonded`      | A new mage performs the bond ritual.                             |

Every other transition is design-illegal — see §8 for how an attempt is meant to be handled, and why that handling is an open fault-taxonomy question rather than settled law.

## 3. State diagram

```
                  ritual succeeds
   ┌──────────────────────────────────────────────┐
   │                                              │
   ▼                                              │
┌──────────┐   contender starts ritual   ┌─────────────┐
│ Bonded   │ ───────────────────────────►│ Contested   │
│          │                             │             │
│          │◄────── timer expires ───────│             │
└────┬─────┘   (owner held)              └──────┬──────┘
     │                                          │
     │ owner dies            contender wins     │
     │ or releases           ritual             │
     ▼                                          ▼
┌────────────┐                          ┌──────────────┐
│ Abandoned  │                          │ Transferred  │
│            │                          │ (one tick)   │
└────┬───────┘                          └──────┬───────┘
     │                                         │
     │ new mage runs bond ritual               │ new owner accepts
     │                                         │
     └───────────────► Bonded ◄────────────────┘
```

## 4. Events

Every transition is meant to publish `GolemOwnershipChanged` (`[Deferred]`). Event fields, as shipped (`GolemOwnershipChanged.cs:19-23`):

- `Golem` — `EntityId` of the golem.
- `PreviousMage` — `EntityId?` of the previous owner (null when coming from `Abandoned`).
- `NewMage` — `EntityId?` of the new owner (null when going to `Abandoned`).
- `Mode` — resulting `OwnershipMode`.

(The predecessor document named these fields `GolemId`/`PreviousOwnerId`/`NewOwnerId`/`PreviousMode`+`NewMode` — the shipped record carries a single resulting `Mode`, not a before/after pair; the field list above is the corrected, current shape.)

A separate inbound event, `GolemOwnershipTransferRequest`, requests the start of an interception ritual (`Golem`, `NewMage`, target `Mode` — `GolemOwnershipTransferRequest.cs:15-18`). Handling is meant to be: `GolemSystem` checks admissibility (the golem is not in `Transferred`, the attacker is actually a mage, the target is in range) and either moves it to `Contested` or replies with `GolemOwnershipRefused`.

> **FENCED (target / planned — not current truth):** `GolemOwnershipRefused` does not exist (banner above). The handler that would run this logic, `GolemSystem.OnGolemOwnershipTransferRequest`, is a stub (`GolemSystem.cs:57`).

## 5. The `GolemBondComponent` component

Current shape (`src/DualFrontier.Components/Magic/GolemBondComponent.cs:17-49`):

```csharp
[ModAccessible(Read = true, Write = true)]
public struct GolemBondComponent : IComponent
{
    public EntityId? BondedMage { get; init; }    // null when Abandoned
    public OwnershipMode Mode { get; init; }       // Bonded | Contested | Abandoned | Transferred
    public int TicksSinceContested { get; init; }  // timer counter; 0 when not Contested
    public int BondStrength { get; init; }         // TODO Phase 6: formula based on mage level/school
}
```

Two shape corrections from the predecessor document, both re-verified: this is a `struct` with `{ get; init; }` properties today, not a `sealed class` with public fields; and `BondStrength` is `int`, not `float`. The game-design intent for `BondStrength` is unchanged — it is meant to grow with bond duration and shared successful battles, and shrink with long idleness and after every contestation, participating in contest resolution (0..1 conceptually per the original design, though the shipped field type is now an integer). The formula itself does not exist yet (banner above) — no `GolemBondStrength.cs` file, no inline computation.

## 6. Mutation rule (single-writer)

**Only `GolemSystem` may write `GolemBondComponent`.** Other systems observe `GolemOwnershipChanged` events but do not mutate the component directly. This is the one fragment of the predecessor document the session's audit called architecture-shaped (N-6) — it is also this document's `constrains` anchor into ECS.md/THREADING.md. Current declaration (`GolemSystem.cs:21-25`):

```csharp
[SystemAccess(
    reads:  new[] { typeof(GolemBondComponent), typeof(ManaComponent) },
    writes: new[] { typeof(GolemBondComponent) },
    bus:    nameof(IGameServices.Magic)
)]
public sealed class GolemSystem : SystemBase { /* ... */ }
```

Re-verified: `GolemSystem` is still the **only** system in the codebase declaring `writes: … GolemBondComponent` (repo-wide grep, one hit). An attempt by another system to declare the same write crashes the scheduler at startup — a write-write conflict raised by `DependencyGraph.BuildWriteConflictException` (`src/DualFrontier.Core/Scheduling/DependencyGraph.cs:114,226`). The conflict-detection mechanism itself is [THREADING.md](../architecture/THREADING.md)'s law, not this document's; this document only asserts the game-design requirement (exactly one writer) that the mechanism enforces.

**Correction from the predecessor's `reads` claim.** The predecessor quoted `reads: { ManaSnapshotComponent, PositionComponent }`. The shipped declaration reads `{ GolemBondComponent, ManaComponent }` instead — no `PositionComponent`, and `ManaComponent` directly rather than a snapshot type. See §7.

## 7. Previous-tick mana read

The game-design intent (unchanged from the predecessor) is that `GolemSystem` reads the mage's mana through a previous-tick snapshot rather than the live value, to avoid a feedback cycle with `ManaSystem` — see [FEEDBACK_LOOPS.md](./FEEDBACK_LOOPS.md) for the worked example and the player-facing cost of that choice.

The rule that makes this safe — *any cycle-closing read goes through a `_Previous` snapshot of the previous tick* — is normative in [THREADING.md](../architecture/THREADING.md), not here (see FEEDBACK_LOOPS.md's own engine-rule pointer section for the same redirection). What this document adds: at HEAD `35364c2`, the mechanism is **not wired to that rule at all**. `GolemSystem.ReadPreviousTickManaState` is a private stub that throws `NotImplementedException` (`GolemSystem.cs:76`), and — more importantly — there is no `ManaSnapshotComponent` or any `*ManaSnapshot*` type anywhere in the repository (re-verified, zero matches). The current `[SystemAccess]` declaration (§6) reads `ManaComponent` directly, the same component `ManaSystem` writes. Today this does not manifest as a live cycle (`ManaSystem` does not read `GolemBondComponent`, so there is no cross-dependency for the scheduler to reject), but it means the snapshot-based cycle-avoidance pattern this document and FEEDBACK_LOOPS.md describe is **design intent only** here, not a wired mechanism — flagging this precisely so a future implementer does not assume the reads set already does what the comment above `ReadPreviousTickManaState` says it does.

## 8. Known open question: the DEBUG/RELEASE fault-taxonomy fork

Every other transition in §2 being "forbidden" needs an enforcement story. The predecessor document's story was: attempting one throws `InvalidOwnershipTransitionException` in DEBUG and is silently ignored in RELEASE (with an error counter increment for diagnostics). Re-verified: `InvalidOwnershipTransitionException` does not exist anywhere in the repository (zero matches) — this was, and remains, design intent, not shipped enforcement.

> **FENCED (target / planned — not current truth):** [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](../architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (AUTHORED draft) §4 names this *exact* sentence from the predecessor document as its own counter-example for why a `#if DEBUG`-throw / RELEASE-silent fork needs justification: "Every `#if DEBUG`-throw / RELEASE-silent fork MUST be justified against this table, per class. Silent RELEASE handling is legal only for a class whose row already names an owner, a counter, and a user-visible surface… An illegal transition is class 1 (a bug); class-1 faults get the same response in both configurations — a bug does not become acceptable because the build is RELEASE." Under that draft's taxonomy, an illegal bond transition is a **class-1 contract violation**, whose proposed response is a crash (fail-fast) in *both* configurations — not the DEBUG-throw/RELEASE-silent split this document's predecessor described. This document does not resolve the question; it is recorded here as open, owned by the draft above pending ratification.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [ECS.md](../architecture/ECS.md) | constrains | Component storage and the single-writer enforcement `GolemBondComponent` relies on. |
| [THREADING.md](../architecture/THREADING.md) | constrains | Startup write-conflict detection (§6) and the previous-tick-snapshot rule (§7) this mechanic relies on. |
| [FEEDBACK_LOOPS.md](./FEEDBACK_LOOPS.md) | cites | The Golem/Mana cycle is FEEDBACK_LOOPS.md's primary worked example; this document's §7 is the ownership-side half of that story. |
| [EVENT_BUS.md](../architecture/EVENT_BUS.md) | cites | `[Deferred]` delivery mode for `GolemOwnershipChanged`. |
| [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](../architecture/ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) (AUTHORED draft) | defers-to | Fault taxonomy and the DEBUG/RELEASE-fork law (that draft's §4); this document's own open question is §8. |
| [RESOURCE_MODELS.md](./RESOURCE_MODELS.md) | cites | `RefusalReason.NoActiveBond` is keyed to the `Abandoned` state here. |
| [ROADMAP.md](../ROADMAP.md) | cites | M10.A — Vanilla.Magic, the forward milestone that would implement this mechanic. |

## Amendment protocol

While Draft, revise freely as the golem-bond mechanic design evolves (FRAMEWORK §3.3 Draft mutability) — no deliberation milestone required. Ratification to LOCKED requires a Crystalka deliberation milestone per FRAMEWORK §7 (amendment milestone protocol) once `GolemSystem` ships real transition behavior; from LOCKED onward, Tier 1 change authority (FRAMEWORK §3.2.1) governs further amendment.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.0 | 2026-07-15 | Renamed from `OWNERSHIP_TRANSITION.md` and reclassified from Category A (`DOC-A-OWNERSHIP_TRANSITION`, now historical) to Category J per FRAMEWORK §3.1 and session finding N-6 (title collision with architecture vocabulary); implementation-status banner corrected from "shipped" to "data shapes real, all behavior stubbed" on re-verification at HEAD `35364c2`; `GolemBondComponent`/`[SystemAccess]` code snippets updated to the current shape; DEBUG/RELEASE fault-taxonomy fork rerouted to ENGINE_LIFECYCLE_AND_TRANSACTIONS.md (AUTHORED draft) §4, which independently cites the same predecessor line as its own counter-example. |

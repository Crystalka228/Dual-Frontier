---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-J-FEEDBACK_LOOPS
category: J
tier: 1
lifecycle: Draft
owner: Crystalka
version: "0.1.0"
next_review_due: 2027-07-15
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-J-FEEDBACK_LOOPS
---
# Feedback loops

Game-mechanic catalogue of which systems participate in a same-tick feedback cycle (golem upkeep vs. mage mana, and similar pairs) and what relying on the engine's snapshot rule costs the player.

> **Document class: mechanic design (Category J, Tier 1, Draft).** Reclassified from Category A per the corpus rework of 2026-07-15; successor of `docs/architecture/historical/FEEDBACK_LOOPS.md` (DOC-A-FEEDBACK_LOOPS, now SUPERSEDED). Describes game-mechanic intent; the engine surfaces it constrains are listed in `constrains`. Ratifiable Draft → LOCKED per FRAMEWORK §7 when the mechanic ships.

## Status

| Field | Value |
|---|---|
| Role | mechanic-design-draft |
| Successor of | `docs/architecture/historical/FEEDBACK_LOOPS.md` (DOC-A-FEEDBACK_LOOPS, now SUPERSEDED) |
| Scope | **Gameplay residue only.** Which game mechanics close a same-tick feedback cycle (golem/mana, ether growth, shield maintenance), the player-facing latency their fix costs, and applicability guidance for designers/modders adding a new one. Does not define the fix itself. |
| Non-goals | Does **not** define, own, or restate the cycle-detection/snapshot engine rule or its enforcement mechanism, and does not re-verify the predecessor's DD-1 question about it — [THREADING.md](../architecture/THREADING.md) is normative for all of that (see §3). |
| Authority domains | Magic/Combat economy: which gameplay mechanics are cycle participants, and the game-design acceptance of a +1-tick reaction latency as the price of determinism for those mechanics. |
| Defers to | [THREADING.md](../architecture/THREADING.md) — the cycle/snapshot engine rule itself, its enforcement mechanism, and the verified truth of the predecessor's DD-1 note |
| Constrains | [THREADING.md](../architecture/THREADING.md) (DOC-A-THREADING_V2) |

The system dependency graph does not allow cycles over the same components — otherwise the scheduler cannot build phases. But game logic regularly demands feedback: one system writes a resource and another reads that same resource to make a decision. This document catalogues where that shows up in Dual Frontier's mechanics and what it costs the player; the fix itself is engine law owned elsewhere.

> **The engine rule has moved.** The normative statement of the cycle/snapshot rule — *any cycle-closing read goes through a `_Previous` snapshot of the previous tick* — used to be stated and half-owned by this document (including a predecessor self-flagged note, DD-1, about whether its specific enforcement exception type still applies after a runtime isolation guard was removed). As of this rework, [THREADING.md](../architecture/THREADING.md) is the sole normative home for the rule, its enforcement mechanism, and the DD-1 question. This document is downstream of it: it exists to catalogue which *mechanics* rely on the rule and what relying on it costs the player, and does not restate, re-derive, or re-verify the rule's enforcement mechanism (see §3).

## 1. Problem

The recurring example is a golem and its mage owner's mana (see [GOLEM_OWNERSHIP.md](./GOLEM_OWNERSHIP.md) for the ownership side of this same pair).

- `ManaSystem` writes `ManaComponent` (drains mana to maintain the golem).
- `GolemSystem` reads mana (to decide whether there is enough for the next tick; if not, the golem deactivates).

If both systems live in one phase, that is a read/write conflict the scheduler rejects. If they live in different phases of the same tick, the outcome depends on order: does `GolemSystem` read mana *before* the drain or *after*? Both options are "correct" in isolation, but even a small phase rearrangement changes the result — and that breaks determinism.

An explicit system order looks like a simple fix but is fragile: every new system that writes mana (rituals, potions, ether converters) would have to be hand-inserted in the right spot. A mutex on the component would block the entire phase's parallelism — unacceptable for a mechanic this common.

## 2. How the mechanic uses the snapshot pattern

The shape every cycle-closing mechanic follows: on tick N, `ManaSystem` performs its drain and writes the actual `ManaComponent` — call it `Mana[N]`. At tick finalization the scheduler copies it into a snapshot, `ManaSnapshot[N]`. On tick N+1, the reading system (`GolemSystem`) reads through a previous-tick accessor and gets `ManaSnapshot[N]` — the state captured *after* the previous tick's drain, not the value being written concurrently this tick.

```
tick N-1:                       tick N:                         tick N+1:
  ManaSystem writes Mana[N-1]     ManaSystem writes Mana[N]        ManaSystem writes Mana[N+1]
  ...                             GolemSystem reads ManaSnapshot[N-1]   GolemSystem reads ManaSnapshot[N]
  snapshot: ManaSnapshot[N-1]     snapshot: ManaSnapshot[N]        snapshot: ManaSnapshot[N+1]
```

The reading system sees a stable snapshot: the read does not intersect a write in any phase, so the scheduler sees no conflict. This is the shape the rule provides; §3 is where the rule itself is normative.

## 3. The engine rule (pointer, not restated here)

The rule — *any read that closes a cycle in the dependency graph must go through a `_Previous` snapshot of the previous tick* — is normative in [THREADING.md](../architecture/THREADING.md), including its enforcement mechanism and the predecessor document's self-flagged DD-1 question about whether that mechanism still throws the exception type it named, after the runtime per-access isolation guard was removed at К8.3+К8.4. This document does not restate, re-derive, or re-verify any of that — it only catalogues which gameplay mechanics rely on the rule (§6) and what relying on it costs the player (§4).

## 4. Cost

- **+1 tick latency.** The golem reacts to mana exhaustion with a one-tick delay. At `TickRates.NORMAL` (15 frames, ~250 ms at 30 Hz) this is invisible to the player and does not break balance.
- **+1 copy pass** per tick for each component participating in a cycle. The snapshot is a flat copy of the data structure, O(N) in the entity count for that component — on a real profile, a cost smaller than frame-time noise.

The game-design trade-off this document owns: determinism is worth a tick of delay. A single tick of reaction lag is the price for reproducible combat, correct replay, and stable tests — the engine mechanism that delivers the reproducibility is THREADING's law (§3); this is the design decision to pay for it.

## 5. Alternatives considered

1. **Explicit system order in one phase.** Rejected: any new system that writes mana breaks the order; the graph becomes fragile and implicit. Sooner or later a mod adds a potion system, and the golem starts behaving differently depending on where in the chain that new system slots in.
2. **Mutex on the component.** Rejected: it blocks phase parallelism and returns to a single-threaded update model for that resource. It also conflicts with the declarative access model systems use to describe their reads/writes — that model knows nothing about mutexes.
3. **Event-based pull through `[Deferred]`.** Used in part elsewhere (cycle-breaking in the dependency graph through a deferred event), but it does not solve this problem: `GolemSystem` needs the **value** of mana, not a notification that it changed. An event would answer "when," not "how much."

## 6. Applicability

- `GolemSystem` reads a mana snapshot (previous tick) to decide whether to keep a golem active — see [GOLEM_OWNERSHIP.md](./GOLEM_OWNERSHIP.md)'s previous-tick-mana-read section for this pair's current implementation status (design intent only at HEAD `35364c2`: no `ManaSnapshotComponent`-shaped type exists yet).
- An ether-surge mechanic would read an ether snapshot when deciding on a surge while an ether-growth mechanic updates the live value.
- A shield-break mechanic would read a mana snapshot for shields powered by mana.

Not every reading system needs a snapshot — only those that participate in a cycle. A system that reads a resource it does not itself write, and that nothing downstream of it writes back into, has no cycle to break.

> **Code-truth footnote (re-verified at HEAD `35364c2`, 2026-07-15).** Of the three examples above, only the first two correspond to anything on disk today, and only partially: `EtherGrowthSystem` exists (`src/DualFrontier.Systems/Magic/EtherGrowthSystem.cs`) but its `Update` is an empty TODO and it does not read any snapshot — it reads and writes `EtherComponent` directly (`:16-19`). An "ether-surge" system and a "shield-break" system do not exist under any name (re-verified, zero matches for `EtherSurgeSystem` or `ShieldBreakSystem`), and no `EtherSnapshot`-shaped type exists either (zero matches). These three bullets remain useful applicability *guidance* — which shape of mechanic would need a snapshot, if and when it is built — not an inventory of shipped systems.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [THREADING.md](../architecture/THREADING.md) | constrains | Normative home of the cycle/snapshot rule, its enforcement mechanism, and the DD-1 verification question (this document points to it from its own §3). |
| [GOLEM_OWNERSHIP.md](./GOLEM_OWNERSHIP.md) | cites | The Golem/Mana pair is this document's primary worked example; that document's previous-tick-mana-read section covers the ownership-side implementation status. |
| [EVENT_BUS.md](../architecture/EVENT_BUS.md) | cites | `[Deferred]` delivery as the alternative considered and rejected in this document's §5 (alternative 3) for this problem shape. |
| [COMBO_RESOLUTION.md](./COMBO_RESOLUTION.md) | cites | Sibling determinism-motivated mechanic — ordering damage vs. breaking a read/write cycle. |

## Amendment protocol

While Draft, revise freely as the set of cycle-participating mechanics grows (FRAMEWORK §3.3 Draft mutability) — no deliberation milestone required. Ratification to LOCKED requires a Crystalka deliberation milestone per FRAMEWORK §7 (amendment milestone protocol) once the cataloged mechanics ship; from LOCKED onward, Tier 1 change authority (FRAMEWORK §3.2.1) governs further amendment.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.0 | 2026-07-15 | Reclassified from Category A (`DOC-A-FEEDBACK_LOOPS`, now historical) to Category J per FRAMEWORK §3.1; the engine cycle/snapshot rule (formerly stated here, including the DD-1 self-flagged enforcement question) removed and rerouted to THREADING.md as sole normative owner; worked examples re-verified against code and annotated with current build status; document scope narrowed to gameplay applications only. |

# Feedback-loop resolution

The system dependency graph does not allow cycles over the same components — otherwise the scheduler cannot build phases. But game logic regularly demands feedback: one system writes a resource and another reads that same resource to make a decision. v0.2 resolves such loops with a single technique — reading a snapshot of the previous tick.

## Problem

The classic example is a golem and a mage's mana.

- `ManaSystem` writes `ManaComponent` (drains mana to maintain the golem).
- `GolemSystem` reads `ManaComponent` (to decide whether there is enough mana for the next tick; if not, the golem deactivates).

If both systems live in one phase — read/write conflict, the scheduler complains. If they live in different phases of the same tick, the outcome depends on order: does `GolemSystem` read mana _before_ the drain or _after_? Both options are "right", but even a small phase rearrangement changes the result — and that kills determinism.

An explicit system order looks like a simple fix but is fragile: every new system that writes mana (rituals, potions, ether converters) must be hand-inserted in the right spot. A mutex on the component blocks the entire phase's parallelism — unacceptable.

## Solution: Mana[N-1]

Any read that closes a cycle goes **through a snapshot of the previous tick**. On tick N the work looks like this:

1. `ManaSystem` performs its drain and writes the actual `ManaComponent` — this is `Mana[N]`.
2. At the tick boundary (Phase 5 — Feedback snapshot, see [THREADING](/docs/architecture/THREADING.md)), the scheduler copies `ManaComponent` into `ManaSnapshot` — that is `ManaSnapshot[N]`.
3. On tick N+1 `GolemSystem` reads through `ReadPreviousTickManaState()` — it gets `ManaSnapshot[N]`, the state captured _after_ the previous tick's drain.

```
tick N-1:                       tick N:                         tick N+1:
  ManaSystem writes Mana[N-1]     ManaSystem writes Mana[N]        ManaSystem writes Mana[N+1]
  ...                             GolemSystem reads ManaSnapshot[N-1]   GolemSystem reads ManaSnapshot[N]
  snapshot: ManaSnapshot[N-1]     snapshot: ManaSnapshot[N]        snapshot: ManaSnapshot[N+1]
```

`GolemSystem` sees a stable snapshot: the read does not intersect a write in any phase, and the scheduler sees no conflict.

## Cost

- **+1 tick latency.** The golem reacts to mana exhaustion with a one-tick delay. At `TickRates.NORMAL` (15 frames, ~250 ms) this is invisible to the player and does not break balance.
- **+1 copy pass** per tick for each component participating in the loop. The snapshot is a flat copy of the data structure, O(N) in the entity count for that component. On a real profile — tens of microseconds, smaller than the frame-time noise floor.

Determinism >> latency: a single tick of delay is the price we pay for reproducible combat, correct replay, and stable tests.

## Alternatives considered

1. **Explicit system order in one phase.** Rejected: any new system that writes mana breaks the order; the graph becomes fragile and implicit. Sooner or later a mod adds `PotionSystem`, and the golem starts behaving differently depending on where in the chain the system slots in.
2. **Mutex on the component.** Rejected: it blocks phase parallelism and returns us to RimWorld's single-threaded mode. It also conflicts with the `[SystemAccess]` declaration — the isolation guard knows nothing about mutexes.
3. **Event-based pull through `[Deferred]`.** Used in part (cycle-breaking in the graph through a deferred event), but it does not solve the original problem: `GolemSystem` needs the **value** of mana, not a notification that it changed. The event would answer "when", not "how much".

## Rule

> Any read that closes a cycle in the dependency graph must go through a `_Previous` snapshot of the previous tick.

The rule is enforced by a test in `DualFrontier.Core.Tests/Scheduling/`: `DependencyGraph` at startup marks every cycle and requires that at least one side of the cycle uses a `*Snapshot` component. If a `[SystemAccess]` declaration contains `reads: ManaComponent` in a system participating in a cycle through `ManaSystem`, an `IsolationViolationException` is thrown at registration.

## Applicability

- `GolemSystem` reads `ManaSnapshot` (previous tick).
- `EtherSurgeSystem` reads `EtherSnapshot` when deciding on a surge while `EtherGrowthSystem` updates ether.
- `ShieldBreakSystem` reads `ManaSnapshot` for shields powered by mana.

Not every reading system is required to use a snapshot — only those that participate in a cycle. `DamageSystem` reads `HealthComponent` directly because there is no "Health → Damage → Health" cycle: `DamageSystem` is the writer itself.

## See also

- [THREADING](/docs/architecture/THREADING.md) — Phase 5 (Feedback snapshot).
- [EVENT_BUS](/docs/architecture/EVENT_BUS.md) — `[Deferred]` as an alternative for breaking a write cycle.
- [COMBO_RESOLUTION](/docs/architecture/COMBO_RESOLUTION.md) — determinism in damage application.
- [OWNERSHIP_TRANSITION](/docs/architecture/OWNERSHIP_TRANSITION.md) — `GolemSystem` and its mana-snapshot read.

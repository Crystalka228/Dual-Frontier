# Combo damage resolution

The same target in one tick often takes damage from several systems: physical from `CombatSystem`, magical from `SpellSystem`, status from `StatusEffectSystem`. If each system writes `HealthComponent` directly in publish order, the combat outcome depends on the thread scheduler. v0.2 introduces deterministic ordering through `ComboResolutionSystem`.

## Problem

Applying damage directly in event-publish order yields a different result for the same world state. Scenario: a pawn is hit at the same time by a bullet, a fireball, and a poisoned dart.

- If fire is applied first, the pawn may pick up a `Burning` status that increases physical-damage vulnerability, and the bullet hits harder.
- If the bullet is applied first, there is no vulnerability and the bullet does standard damage.
- If poison is applied first and the pawn dies from the status, the rest of the damage "vanishes" and the kill counter goes to the dart, not the bullet.

Each outcome is admissible by the rules, but the choice depends on `Parallel.ForEach` and thread scheduling — i.e. it is non-deterministic. Replay breaks. Integration tests become flaky.

## Solution

Every damage system publishes a single `DamageIntent` instead of writing `HealthComponent` directly. `ComboResolutionSystem` collects the queue of intents for the tick, sorts it by a stable key, and applies them in strict order, publishing the resulting `DamageEvent`s.

```
tick N, Phase 4 (Apply & Damage):

  CombatSystem       ─┐
  SpellSystem        ─┤── DamageIntent[] ──► ComboResolutionSystem
  StatusEffectSystem ─┘                                │
                                                       │ sort by (EntityId, DamageKind ordinal)
                                                       │
                                                       ▼
                                              apply in strict order
                                                       │
                                                       ▼
                                             publish DamageEvent (one per applied intent)
```

### Sort key

The sort is lexicographic on the pair `(EntityId, DamageKind ordinal)`:

1. First, every intent is grouped by `EntityId` — one target is processed atomically.
2. Within a group, the order is set by the `DamageKind` ordinal.

The ordinal table is fixed in `DamageKind.cs` (TODO Phase 4):

| DamageKind | Ordinal |
|------------|---------|
| Physical   | 0       |
| Fire       | 1       |
| Arcane     | 2       |
| Status     | 3       |

The string `DamageKind` field does not participate in the sort directly — only through the constant table. This matters for mods: a mod that adds a new damage type registers it via `IModApi.RegisterDamageKind(name, ordinal)` and MUST pick a non-conflicting ordinal. Without registration, a mod's damage goes to the end of the queue.

### Determinism guarantee

Given identical input world state and an identical set of `DamageIntent`s, the resulting `DamageEvent` stream is byte-identical between runs. This is enforced by a replay test in `DualFrontier.Systems.Tests/Combat/ComboResolutionTests` (TODO Phase 5): the same seed and the same event sequence produce the same result.

Consequences:

- A replay system can reproduce a fight from a command log without storing intermediate state.
- No flaky combat tests: application order is fixed rather than determined by the thread scheduler.
- Network synchronization (if added later) does not require replicating damage events — the `DamageIntent` stream is enough.

## Why sort and not a queue

One could apply damage in publish order through a lock-free queue. But:

- Publish order depends on the system traversal order in the `Parallel.ForEach` phase.
- `Parallel.ForEach` does not guarantee a stable order inside partitions.
- Even with an ordered queue, two systems writing into the queue from different threads can interleave differently.

Sorting on a stable key removes the dependency on whoever published first.

## Relationship to composite requests

`ComboResolutionSystem` and `CompositeResolutionSystem` solve related but different problems:

- **Composite** collects **heterogeneous** responses from **different buses** for one transaction (`TransactionId`) and emits a single result — see [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md).
- **Combo** collects **homogeneous** `DamageIntent`s on a single target and orders them for application.

Both systems live in the same family of phases (Intent Collection / Apply & Damage), but they work with different intent types and do not compete.

## See also

- [EVENT_BUS](./EVENT_BUS.md) — the two-step Intent → Granted model, batch processing.
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md) — multi-bus requests.
- [THREADING](./THREADING.md) — Phase 4 (Apply & Damage).
- [FEEDBACK_LOOPS](./FEEDBACK_LOOPS.md) — why determinism beats latency.

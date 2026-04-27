# Combat Systems

## Purpose
Combat: shot initiation, projectile flight, damage computation, shields, and
status effects. See the GDD "Combat" section.

## Dependencies
- `DualFrontier.Contracts` — attributes, `ICombatBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Combat` — `WeaponComponent`,
  `ArmorComponent`, `ShieldComponent`, `ProjectileComponent`.
- `DualFrontier.Components.Shared` — `HealthComponent`, `PositionComponent`.
- `DualFrontier.Events.Combat` — `ShootAttemptEvent`, `DamageEvent`,
  `DeathEvent`, `StatusAppliedEvent`.

## Contents
- `CombatSystem.cs` — FAST: initiates attacks, requests ammunition (AmmoIntent).
- `ProjectileSystem.cs` — REALTIME: projectile flight, collisions.
- `DamageSystem.cs` — FAST: damage application accounting for armor and shields.
- `ShieldSystem.cs` — FAST: shield regeneration / absorption.
- `StatusEffectSystem.cs` — FAST: status-effect ticks (burning, poison).

## Rules
- Domain bus — `nameof(IGameServices.Combat)`.
- `CombatSystem` does NOT write damage directly — it publishes
  `DamageEvent`, which `DamageSystem` processes.
- Ammunition flows through `AmmoIntent` / `AmmoGranted` / `AmmoRefused` on the
  `Inventory` bus, non-blocking.
- `ProjectileSystem` ticks at REALTIME (every frame), because visual
  correctness depends on it.

## Usage examples
```csharp
// Inside CombatSystem:
combatBus.Publish(new ShootAttemptEvent(shooterId, targetId));
// Later — on AmmoGranted — publish DamageEvent for DamageSystem.
```

## TODO
- [ ] Implement `CombatSystem`: shot state machine.
- [ ] Implement `ProjectileSystem`: linear projectile motion.
- [ ] Implement `DamageSystem`: damage formula = atk - armor + crit.
- [ ] Implement `ShieldSystem`: magical absorption.
- [ ] Implement `StatusEffectSystem`: queue of active effects.

## v02 Addendum (TechArch §12.4)

- `CompositeResolutionSystem.cs` — FAST, multi-bus (Combat + Inventory + Magic):
  two-phase commit by `TransactionId`. Subscribed to `CompoundShotIntent`,
  `AmmoGranted` / `AmmoRefused`, `ManaGranted` / `ManaRefused`. When both
  partial responses are collected, publishes the final `ShootGranted` or
  `ShootRefused` to the Combat bus.
- `ComboResolutionSystem.cs` — NORMAL, multi-bus (Combat + Magic):
  collects `DamageIntent`s from Physical/Magic/Status systems and applies
  them in a deterministic order (sorted by
  `(EntityId, DamageKind ordinal)`), publishing the resulting `DamageEvent`s.
- `CombatSystem` updated: now declares the `Combat` and `Magic` buses
  (the shot's mana cost is part of the compound shot) and delegates resource
  checks through `CompoundShotIntent` instead of a direct `AmmoIntent`.

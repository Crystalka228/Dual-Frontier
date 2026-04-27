# Magic Systems

## Purpose
Dual Frontier's magic subsystem: mana regeneration, spell casting, golem
control, ether-node growth, and rituals. See the GDD sections "Magic",
"Schools of magic", "Golems".

## Dependencies
- `DualFrontier.Contracts` — attributes, `IMagicBus`.
- `DualFrontier.Core` — `SystemBase`, `TickRates`.
- `DualFrontier.Components.Magic` — `ManaComponent`, `SchoolComponent`,
  `GolemBondComponent`, `EtherComponent`.
- `DualFrontier.Events.Magic` — `ManaRequest`, `SpellCastEvent`,
  `EtherSurgeEvent`, `GolemActivatedEvent`.

## Contents
- `ManaSystem.cs` — NORMAL: mana regeneration and consumption.
- `SpellSystem.cs` — FAST: spell execution (publishes results).
- `GolemSystem.cs` — NORMAL: maintaining the golem-owner bond.
- `EtherGrowthSystem.cs` — SLOW: ether-field growth from a school.
- `RitualSystem.cs` — RARE: long collective rituals.

## Rules
- Domain bus — `nameof(IGameServices.Magic)`.
- `SpellSystem` does not write to components — it only publishes events;
  damage is applied by `DamageSystem` from `Combat/`.
- `ManaSystem` is the only system that writes `ManaComponent` (apart from
  rituals).
- Ether and mana are different pools: ether in the world (nodes), mana on the
  pawn.

## Usage examples
```csharp
// Inside SpellSystem:
magicBus.Publish(new SpellCastEvent(casterId, spellId, target));
```

## TODO
- [ ] Implement `ManaSystem`: regen formula based on school and stats.
- [ ] Implement `SpellSystem`: pool of active casts.
- [ ] Implement `GolemSystem`: bond decay on mana shortage.
- [ ] Implement `EtherGrowthSystem`: ether diffusion across the grid.
- [ ] Implement `RitualSystem`: multi-pawn rituals.

## v02 Addendum (TechArch §12.2, §12.3, §12.5)

### ManaSystem — mana lease

`ManaSystem` now handles continuous mana leases:
- `OnManaLeaseOpenRequest(ManaLeaseOpenRequest)` — checks `Mana.Current[N-1]`,
  opens the lease through `Internal/ManaLeaseRegistry`, and publishes
  `ManaLeaseOpened` or `ManaLeaseRefused` with the appropriate `RefusalReason`.
- `DrainActiveLeases()` — per-tick mana drain from every active lease;
  publishes `ManaLeaseClosed` for any that have expired.
- `OnManaLeaseCloseRequest(LeaseId, CloseReason)` — explicit lease closure
  (for example, the cast was interrupted from outside).

### GolemSystem — ownership transfer

`GolemSystem` subscribes to `GolemOwnershipTransferRequest` and publishes the
deferred (`[Deferred]`) `GolemOwnershipChanged` event. To prevent a
feedback loop (§12.3), the owner's mana state is read via
`ReadPreviousTickManaState()` — the `Mana[N-1]` snapshot.

### Internal/ subdirectory

`Magic/Internal/` — private types of the magic subsystem that do not cross
the assembly boundary:
- `ManaLease.cs` — record of one active lease.
- `ManaLeaseRegistry.cs` — collection of active leases, `LeaseId` issuance,
  per-tick drain.
- `README.md` — package rules.

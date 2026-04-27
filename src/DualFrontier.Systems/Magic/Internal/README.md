# Magic / Internal

## Purpose

The `Internal/` folder holds helper types of the magic subsystem that **must
not cross the assembly boundary** of `DualFrontier.Systems`. These are
internal data structures known only to ManaSystem, GolemSystem, SpellSystem,
and the other systems in this assembly.

## Rules

- Every type in this folder is declared `internal` (or `internal sealed`).
- **No** public events, contracts, or components live here — they belong in
  `DualFrontier.Events.Magic`, `DualFrontier.Contracts`, and
  `DualFrontier.Components.Magic` respectively.
- Types from `Internal/` MUST NOT appear in the signatures of public system
  methods. If some piece of information is needed from outside, a public event
  or component is created in the appropriate assembly.
- `ManaLeaseRegistry` is the single owner of active mana leases.
  Owned by `ManaSystem` (future: through a DI container, Phase 5).
  Other magic systems may receive a reference to it, but only within this
  assembly.

## Contents

- `ManaLease.cs` — internal record of one active mana lease (identifier,
  caster, drain, counters).
- `ManaLeaseRegistry.cs` — collection of active leases, `LeaseId` issuance,
  per-tick drain, expiration lookup.

## Relation to TechArch v0.2

See §12.2 "Continuous mana leases" — the rationale for the two-step
`ManaLeaseOpenRequest` / `ManaLeaseOpened` / `ManaLeaseClosed` model and why
the registry lives inside ManaSystem rather than on a public bus.

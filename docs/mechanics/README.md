# docs/mechanics — Game Mechanics index

*This folder contains Category J game mechanics design documents (Tier 1 or Tier 2 per A'.4.5 governance framework).*

---

## Purpose

Category J documents describe **what mechanics are** — gameplay design intent, player-facing rules, balance considerations. Distinct from Category A architecture documents, which describe **how the engine supports** mechanics.

**Test for A vs J classification** (per FRAMEWORK.md §3.1):
Would another game engine implementing the same mechanic produce a different J document but a similar A document? If yes — split clean: J covers design intent, A covers engine pattern.

## Tier split

Category J is per-document tiered:

- **Tier 1 J — Foundational mechanics**: mechanics that constrain architecture (race biology determining tech branches, combat resolution model determining ECS Combat domain, mana lease model in code). Change protocol: deliberation milestone + amendment. Review cadence: on-change + annual.
- **Tier 2 J — Tunable mechanics**: mechanics that don't constrain architecture (HP/damage numbers, recipe trees, faction relation matrices, balance values). Change protocol: Live mutation per playtesting. Review cadence: on-closure + quarterly drift.

Per-document `tier` field in REGISTER.yaml entry. `constrains: [DOC-A-XXX]` cross-references architecture surfaces for Tier 1 J.

## Authoring

New mechanic: `docs/mechanics/<slug>.md` (kebab-case). Register ID `DOC-J-<SLUG_UPPER>`. Tier assignment per the architecture-constraint test above.

## Borderline architecture-vs-mechanics

A'.4.5 deliberation flagged three documents currently in `docs/architecture/` as borderline:
- [COMBO_RESOLUTION.md](../architecture/COMBO_RESOLUTION.md)
- [COMPOSITE_REQUESTS.md](../architecture/COMPOSITE_REQUESTS.md)
- [RESOURCE_MODELS.md](../architecture/RESOURCE_MODELS.md)

Inline classification by execution agent at A'.4.5 enrollment with `special_case_rationale` populated. Future surface for migration if review confirms primarily-mechanic content.

## See also

- [docs/governance/FRAMEWORK.md](../governance/FRAMEWORK.md) §3 classification model
- [docs/governance/REGISTER.yaml](../governance/REGISTER.yaml) operational SoT

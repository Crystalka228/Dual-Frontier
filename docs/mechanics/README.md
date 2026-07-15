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

## Borderline architecture-vs-mechanics — migration executed (2026-07-15)

The A'.4.5 borderline surface was resolved by the corpus rework (EVT-2026-07-15-CORPUS_REWORK_R4_MECHANICS): five gameplay-protocol documents migrated here as Category J Tier 1 Draft, superseding their Category-A predecessors (now `docs/architecture/historical/`):
- [COMBO_RESOLUTION.md](./COMBO_RESOLUTION.md)
- [COMPOSITE_REQUESTS.md](./COMPOSITE_REQUESTS.md)
- [RESOURCE_MODELS.md](./RESOURCE_MODELS.md)
- [GOLEM_OWNERSHIP.md](./GOLEM_OWNERSHIP.md) (renamed from OWNERSHIP_TRANSITION — architecture-vocabulary collision)
- [FEEDBACK_LOOPS.md](./FEEDBACK_LOOPS.md) (engine cycle/snapshot rule moved to [THREADING.md](../architecture/THREADING.md); gameplay residue here)

Naming note: the migrated five keep their established SCREAMING_SNAKE names for citation continuity; newly authored mechanics follow the kebab-case convention above.

## See also

- [docs/governance/FRAMEWORK.md](../governance/FRAMEWORK.md) §3 classification model
- [docs/governance/REGISTER.yaml](../governance/REGISTER.yaml) operational SoT

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-G-MECHANICS_README
category: G
tier: 2
lifecycle: Live
owner: Crystalka
version: "1.0"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-G-MECHANICS_README
---

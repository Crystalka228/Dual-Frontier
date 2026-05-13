---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_2_CLASS_COMPONENT
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_2_CLASS_COMPONENT
---
# K8.2 — 7 class components redesigned to unmanaged structs

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K8, K-L3 implication (Hybrid Path retirement), K8.1 (prerequisite — provides primitives)
**Prerequisite**: K8.1 closure

## Goal

Convert the 7 class components retained under K4's Hybrid Path to unmanaged structs using K8.1 native-side reference primitives. After K8.2 closure, K-L3 holds without exception.

## Components in scope (per K4 Hybrid Path closure record)

- **MovementComponent** (List of GridVector) → struct using `NativeComposite<GridVector>` for path waypoints
- **IdentityComponent** (string fields) → struct using `InternedString` for name + tag
- **SkillsComponent** (Dictionary<SkillKind, int>) → struct using `NativeMap<SkillKind, int>`
- **SocialComponent** (Dictionary<EntityId, RelationshipKind>) → struct using `NativeMap<EntityId, RelationshipKind>`
- **StorageComponent** (Dictionary<ItemKind, int> + HashSet<EntityId>) → struct using `NativeMap<ItemKind, int>` + `NativeSet<EntityId>`
- **WorkbenchComponent** (string field for recipe name) → struct using `InternedString`
- **FactionComponent** (string field for faction id) → struct using `InternedString`

## Time estimate

1-2 weeks at hobby pace.

## Deliverables (high-level)

- 7 component files converted from `class` to `struct` using K8.1 primitives
- Consumer code (systems and tests) updated to use new component shapes
- Bridge tests: each component exercised via NativeWorld round-trip
- Per-component atomic commits (7 atomic commits)

## TODO

- [ ] Author full brief
- [ ] Per-component design (especially Storage with combined map+set)
- [ ] Decide on iteration order semantics for each Dictionary→NativeMap conversion
- [ ] Migration test: managed-Dictionary-based test fixtures vs NativeMap-based — equivalence proof
- [ ] Estimate consumer code touch surface (which systems and how many tests reference each component's reference-type fields)

**Brief authoring trigger**: after K8.1 closure.

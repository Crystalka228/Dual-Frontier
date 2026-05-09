# K8.4 — ManagedWorld retired as production; Mod API v3 ships

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` K-L11, K-L8 in-production implication, MOD_OS_ARCHITECTURE.md §4.6 (IModApi v2 → v3)
**Prerequisite**: K8.3 closure (all production systems on NativeWorld)

## Goal

Remove `World` class from production code paths. `World` retained only as test fixture (renamed to `ManagedTestWorld` for clarity) and research reference. Bootstrap two-phase model becomes the only entry to production. Mod API v3 ships with NativeWorld-only access.

## Time estimate

1 week at hobby pace.

## Deliverables (high-level)

- `World` renamed to `ManagedTestWorld` (or moved to tests project) — explicitly non-production
- `GameBootstrap.CreateLoop` rewritten to construct `NativeWorld` via `Bootstrap.Run`
- `IModApi` v3 ships: replace v2 World access patterns with NativeWorld access patterns
- MOD_OS_ARCHITECTURE.md amendment to v1.7 documenting Mod API v3
- Mod manifest version bumped (mods declaring v2 manifest receive deprecation warning, mods declaring v3 manifest are required for K8.4+)
- Bridge tests: full bootstrap → tick → unload cycle exercised end-to-end on NativeWorld

## TODO

- [ ] Author full brief
- [ ] Mod API v3 surface design (what changes from v2; what stays the same; transition contract)
- [ ] Mod manifest v2 → v3 — opt-in or required? (recommended: required, with deprecation period for v2 manifests recorded in K8.5)
- [ ] Decide ManagedTestWorld rename location (does it stay in DualFrontier.Core.ECS namespace or move to a tests-only namespace?)
- [ ] MOD_OS_ARCHITECTURE.md v1.7 amendment scope (which sections need rewording)

**Brief authoring trigger**: after K8.3 closure.

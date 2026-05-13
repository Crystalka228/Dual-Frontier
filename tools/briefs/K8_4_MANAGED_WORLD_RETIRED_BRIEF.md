---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_4
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_4
---
# K8.4 — ManagedWorld retired as production; Mod API v3 ships

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` K-L11, K-L8 in-production implication, MOD_OS_ARCHITECTURE.md §4.6 (IModApi v2 → v3)
**Prerequisite**: K8.3 closure (all production systems on NativeWorld)

## Goal

Remove kernel-side `World` class from production code paths. `World` retained only as test fixture (renamed to `ManagedTestWorld` for clarity per `KERNEL_ARCHITECTURE.md` K-L11) and research reference. Bootstrap two-phase model becomes the only entry to production. Mod API v3 ships with two component-registration paths per K-L3.1: Path α via existing `RegisterComponent<T>` (NativeWorld), Path β via new `RegisterManagedComponent<T>` (per-mod `ManagedStore<T>` in `RestrictedModApi` instance). `SystemBase.ManagedStore<T>()` accessor ships alongside existing `SystemBase.NativeWorld`.

## Time estimate

1 week at hobby pace.

## Deliverables (high-level)

- `World` renamed to `ManagedTestWorld` (or moved to tests project) — explicitly non-production
- `GameBootstrap.CreateLoop` rewritten to construct `NativeWorld` via `Bootstrap.Run`
- `IModApi` v3 ships: replace v2 World access patterns with NativeWorld access patterns
- `RegisterManagedComponent<T> where T : class, IComponent` added to `IModApi` v3 (per K-L3.1 Q2.β-i lock). `RestrictedModApi` implementation creates per-mod `ManagedStore<T>` instance held in the mod's `RestrictedModApi` instance; reclaimed on `AssemblyLoadContext.Unload`.
- `SystemBase.ManagedStore<T>()` accessor ships (parallel to `SystemBase.NativeWorld` K8.2 v2 plumbing). Resolves via `SystemExecutionContext.Current.ModId` to owning mod's per-mod store. Type `T` must be a class annotated with `[ManagedStorage]`; absence triggers load-time `MissingManagedStorageAttribute` error.
- `MOD_OS_ARCHITECTURE.md` v1.7+ references (already amended at K-L3.1 amendment time per `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` §2); K8.4 brief verifies the v1.7 wording against shipped Mod API v3 surface and adjusts further if needed (per migration plan §6 sequence).
- Mod manifest version bumped (mods declaring v2 manifest receive deprecation warning, mods declaring v3 manifest are required for K8.4+)
- Bridge tests: full bootstrap → tick → unload cycle exercised end-to-end on NativeWorld

## TODO

- [ ] Author full brief
- [ ] Mod API v3 surface design (what changes from v2; what stays the same; transition contract)
- [ ] Mod manifest v2 → v3 — opt-in or required? (recommended: required, with deprecation period for v2 manifests recorded in K8.5)
- [ ] Decide ManagedTestWorld rename location (does it stay in DualFrontier.Core.ECS namespace or move to a tests-only namespace?)
- [ ] MOD_OS_ARCHITECTURE.md v1.7 amendment scope (which sections need rewording)
- [ ] Per-mod `ManagedStore<T>` implementation in `RestrictedModApi` — concrete data structure choice (Dictionary<EntityId, T>, custom hashmap, etc.); per-store lifecycle parallel to subscription cleanup (UnsubscribeAll precedent)
- [ ] `MissingManagedStorageAttribute` error kind addition to `ValidationErrorKind` enum (per K-L3.1 Q5.b deferred enforcement — runtime check until M3.5 analyzer ships)

**Brief authoring trigger**: after K8.3 closure.

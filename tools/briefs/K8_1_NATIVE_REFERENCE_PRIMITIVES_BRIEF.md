# K8.1 — Native-side reference handling primitives

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K8 (Solution A sub-milestone series), K-L3 implication, K-L11
**Prerequisite**: K8.0 closure (Solution A LOCKED in v1.2)

## Goal

Native-side primitives that allow currently class-based components (Movement/Identity/Skills/Social/Storage/Workbench/Faction) to be redesigned as `unmanaged` structs in K8.2.

## Time estimate

1-2 weeks at hobby pace.

## Deliverables (high-level)

- **String interning**: native-side string pool. Managed `string` fields become `uint32_t` interned IDs. C ABI: `df_world_intern_string`, `df_world_resolve_string`.
- **Keyed map**: native-side fixed-key dictionary primitive. Replaces `Dictionary<TKey, TValue>` patterns in components like Skills/Social. C ABI: `df_world_map_set`, `df_world_map_get`, `df_world_map_remove`, `df_world_map_iterate`.
- **Composite component**: native-side variable-length data attached to a parent component. Replaces `List<T>` patterns in components like Movement (path waypoints), Storage (item list). C ABI: `df_world_composite_set`, `df_world_composite_get_count`, `df_world_composite_get_at`.
- **HashSet primitive** (if Storage's reservation set genuinely needs a set, not a list): set membership semantics. C ABI: `df_world_set_add`, `df_world_set_contains`, `df_world_set_remove`.
- Managed bridge: `InternedString` struct, `NativeMap<TKey, TValue>`, `NativeComposite<T>`, `NativeSet<T>` in `DualFrontier.Core.Interop.Marshalling`.
- Selftest scenarios: round-trip + collision + iterate for each primitive.
- Bridge tests: equivalence vs managed Dictionary/List/HashSet behaviors.

## TODO

- [ ] Author full brief
- [ ] Define string pool eviction policy (are interned strings ever freed? Or pool grows monotonically per-session?)
- [ ] Define keyed map ordering semantics (insertion-order vs hash-iteration; matters for determinism)
- [ ] Define composite reallocation strategy (in-place grow vs detach-and-replace)
- [ ] Decide if K8.1 includes managed-world-side identical primitives for parity testing (recommended: yes, to prove component redesigns can be verified pre-K8.4 without NativeWorld dependency)

**Brief authoring trigger**: after K8.0 closure.

# K6 — Second-graph rebuild on mod change

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K6
**Prerequisite**: K5 complete

## Goal

Managed dependency graph rebuilds when mods load/unload. AssemblyLoadContext integration. Native side untouched throughout.

## Time estimate

3-5 days

## Deliverables (high-level)

- `SystemGraph.Rebuild(modRegistry)` method
- `ModLoader.UnloadMod(modId)` + `ReloadMod(modId)`
- `PhaseCoordinator.OnModChanged()` (pause/rebuild/resume tick)
- Tests: rebuild correctness, unload+reload cycle, topological invariants

## TODO

- [ ] Author full brief
- [ ] Include AssemblyLoadContext lifecycle handling
- [ ] Include GC.Collect coordination spec
- [ ] Include edge cases (leaked refs, partial unload)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K5 closure.

# Brief: MOD_OS_ARCHITECTURE.md v1.6 amendment + Tier 1 documentation alignment

**Status**: EXECUTED 2026-05-08
**Source architect**: Opus session 2026-05-08, calibration of GPU_COMPUTE.md v2.0 integration
**Closure**: 11 atomic commits on `main` (260103b..d5dcdde)

---

## Context

GPU_COMPUTE.md v2.0 LOCKED introduced K9 (field storage abstraction) + G0–G9 (Vulkan compute integration) milestones. The MOD_OS_ARCHITECTURE.md spec ratified the API surface and capability syntax extensions that gate K9 implementation. This brief was the deterministic execution plan for that ratification (v1.6) plus three companion document alignments.

A previous attempt corrupted MOD_OS_ARCHITECTURE.md by using a `$` regex character inside Cursor's `edit_file` oldText/newText payloads — the file editor treated the payload as a regex pattern instead of literal text, causing whole-document duplication. The corrupted state was stashed (label: `corrupt-v1.6-attempt-from-regex-bug`) before clean execution against v1.5 baseline.

This brief avoids `$`, `^`, `\b`, and other regex metacharacters in any edit boundaries. Anthropic's `Edit` tool uses literal string matching (not regex), which provides defense-in-depth against the same bug class.

## Methodology

Read-first / brief-second / execute-third — analogous к the established 4-agent pipeline:
- Architect (Opus) reads exhaustively, drafts deterministic execution brief
- Execution session (Opus or Sonnet) applies edits per brief without interpretation
- Brief itself is the contract: pre-flight checks, exact text, stop conditions, atomic commits

Read-only investigation phase consumes full context window for doc reading without burning budget on edit tool calls. Execution session is constrained: only edits, no creative interpretation. If the brief is incomplete, halt and escalate.

## Phases (executed)

### Phase 0 — Pre-flight (executed)

Verified file state before any edit:
1. `docs/MOD_OS_ARCHITECTURE.md` at `LOCKED v1.5` baseline (997 lines, after stash of corrupted v1.6 attempt)
2. `docs/GPU_COMPUTE.md` at `LOCKED v2.0`
3. `docs/KERNEL_ARCHITECTURE.md` at `Version 1.0`
4. `docs/MIGRATION_PROGRESS.md` showing K0–K5 closed, K6 next
5. `tools/briefs/` containing K0–K8, no K9, no G-series
6. Working tree clean

### Phase 1 — MOD_OS_ARCHITECTURE.md v1.5 → v1.6 (executed, 7 commits)

1. Status line bump v1.5 → v1.6
2. Added v1.6 version history entry (8 sub-bullets summarizing the changes)
3. §3.2: extended verb syntax with `field.read`, `field.write`, `field.acquire`, `field.conductivity`, `field.storage`, `field.dispatch`, `pipeline.register` (11 verbs total) + Field verb semantics block
4. §2.3 step 6: extended regex to include all 11 verbs
5. §4.6 NEW (134 lines): IModApi v3 surface — `Fields` and `ComputePipelines` sub-APIs. Sub-sections §4.6.1 Surface (full C# interface), §4.6.2 Capability cross-check, §4.6.3 Backward compatibility, §4.6.4 Mod startup example
6. §11.1: M3.5 row added (capability registry refresh for field types via `[FieldAccessible]`, deferred, unblocked at K9 in-progress)
7. §11.2: 6 new ValidationErrorKind entries (`FieldRegistrationConflict`, `InvalidFieldDimensions`, `FieldCapabilityMismatch`, `ComputePipelineCompilationFailed`, `ComputePipelineRegistrationConflict`, `ComputeUnsupportedWarning`)

Final size: 1155 lines (997 baseline + 158 net additions). No regressions.

### Phase 2 — KERNEL_ARCHITECTURE.md surgical updates (executed, 2 commits)

1. Exec summary: status snapshot line added (K0–K5 closed `547c919` 2026-05-08, 538 tests passing); estimated scope extended for K9; combined timeline with К9 + G-series → 15-25 weeks total
2. Part 8 sequencing options: β6 sequencing decision RESOLVED 2026-05-07 noted; new "Sequencing options including К9 + G-series" sub-section with three options (β6+G-sequential recommended baseline, β6+G-overlap, β6+G-runtime-first)
3. Master plan K9 row already present (added in earlier GPU_COMPUTE.md v2.0 rewrite). No-op.

### Phase 3 — K9 brief skeleton (executed, 1 commit)

`tools/briefs/K9_FIELD_STORAGE_BRIEF.md` created. Status: NOT STARTED. Awaits K6, K7, K8 closure per β6 sequencing.

### Phase 4 — G-series brief skeletons + MODULE update (executed, 1 commit)

10 files created in `tools/briefs/`:
- G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md (~1w, prereqs K9 + M9.0–M9.4)
- G1_MANA_DIFFUSION_BRIEF.md (~1w, first production-shaped use case)
- G2_ELECTRICITY_ANISOTROPIC_BRIEF.md (~1w, conductivity map)
- G3_STORAGE_CAPACITANCE_BRIEF.md (~3-5d, batteries / tanks)
- G4_MULTI_FIELD_COEXISTENCE_BRIEF.md (~3-5d, three vanilla mods concurrent)
- G5_PROJECTILE_DOMAIN_B_BRIEF.md (~1w, ProjectileSystem reactivation)
- G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md (~3-5d, flow field primitives)
- G7_VANILLA_MOVEMENT_BRIEF.md (~1w, replaces per-pawn A*)
- G8_LOCAL_AVOIDANCE_BRIEF.md (~3-5d, RVO/boids)
- G9_EIKONAL_UPGRADE_BRIEF.md (~1w if shipped, evidence-gated)

`tools/briefs/MODULE.md` updated: added `G{N}_TITLE_BRIEF.md` convention; current inventory now includes K9 + G0–G9 with sequencing notes and cross-reference to MOD_OS v1.6 §4.6.

### Phase 5 — Closure verification (executed)

- `dotnet build`: clean (doc-only changes)
- `dotnet test`: 538 tests passing baseline preserved
- `git log --oneline`: 11 atomic commits verified
- Structural sanity: MOD_OS at v1.6, version history complete, §3.2 lists 11 verbs, §2.3 regex extended, §4.6 inserted between §4.5 and §5, M3.5 between M3.4 and M4, §11.2 lists 14 entries (8 + 6 new)

## Final atomic commit summary

```
d5dcdde docs(briefs): add G0–G9 skeletons для GPU compute roadmap
356ea50 docs(briefs): add K9 field storage skeleton
38bbc7c docs(kernel): add β6+G sequencing options для К9 and G-series
84a0fdf docs(kernel): add status snapshot and update combined timeline для К9+G-series
5889815 docs(modos): extend §11.2 with K9 and G0 ValidationErrorKind entries
96171f4 docs(modos): add M3.5 deferred milestone for field capability registry
00ec712 docs(modos): add §4.6 IModApi v3 Fields and Compute Pipelines surface
959d215 docs(modos): extend §2.3 step 6 regex with field and pipeline verbs
ff503f6 docs(modos): extend §3.2 with field and pipeline verbs
6b33e24 docs(modos): add v1.6 version history entry
260103b docs(modos): bump status to v1.6
```

## Methodological lessons

1. **Tool-specific bug isolation**: Cursor's `edit_file` regex-mode is not equivalent to Anthropic `Edit`'s literal-mode. Briefs authored against one editor may carry baggage (e.g. `$`-avoidance) that's inert in the other. Worth noting in future briefs which editor they target.
2. **Pre-flight saved the day**: the user's `git checkout HEAD --` revert had not landed in the working tree; pre-flight read of the file caught the corrupted v1.6 state before any new edit was attempted. Defensive `git stash` (rather than destructive checkout) preserved the corrupted artifact for inspection.
3. **Atomic commits per logical change**: 11 small commits made each step reviewable independently. Each commit message uses scope-prefix (`docs(modos):`, `docs(kernel):`, `docs(briefs):`) consistent with repository conventions.
4. **Triple binding risk** (per Crystalka feedback memory): when capability strings appear simultaneously in spec / code / tests, all three must be updated atomically when capability syntax changes. The v1.6 amendment touches the spec only — code-side enforcement (Roslyn analyzer for `[FieldAccessible]`, `KernelCapabilityRegistry` extension) is M3.5 deferred work, with the explicit understanding that no code currently consumes `field.*` capabilities.

## End of brief

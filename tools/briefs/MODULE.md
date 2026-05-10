# tools/briefs — Brief Inventory

**Purpose**: Brief skeletons и full briefs для milestone execution.

**Reference**: `docs/methodology/METHODOLOGY.md`, `docs/architecture/KERNEL_ARCHITECTURE.md`, `docs/architecture/RUNTIME_ARCHITECTURE.md`

**Convention**:
- `K{N}_TITLE_BRIEF.md` — kernel milestone brief
- `M{N.M}_TITLE_BRIEF.md` — runtime milestone brief
- `G{N}_TITLE_BRIEF.md` — GPU compute milestone brief (per `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap)

**Status spectrum**:
- SKELETON — placeholder с TODO list (created at scaffolding time)
- DRAFT — brief authoring в progress
- READY — brief reviewed и approved для execution
- EXECUTED — milestone complete, brief retained для reference

**Current inventory**:
- K-series (kernel): K0-K8 skeletons (this scaffolding) + K9 skeleton (field storage abstraction, per `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K9 + `docs/architecture/GPU_COMPUTE.md` v2.0 K9 spec)
- M-series (runtime): per RUNTIME_ARCHITECTURE.md Part 2 (М9.0-М9.8 — TODO when приступаем к runtime)
- G-series (GPU compute integration): G0-G9 skeletons per `docs/architecture/GPU_COMPUTE.md` v2.0 Roadmap. Gated on K9 + M9.0–M9.4. G0 plumbing → G1 first shader (mana) → G2 anisotropic (electricity) → G3 storage cells → G4 multi-field coexistence → G5 Domain B (`ProjectileSystem` reactivation) → G6 flow field infrastructure → G7 `Vanilla.Movement` → G8 local avoidance → G9 eikonal upgrade (evidence-gated, optional). API contract specified в `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.6 §4.6 (IModApi v3 — Fields and Compute Pipelines).

**Workflow**:
1. Skeleton created at scaffolding time (this prompt's output)
2. Full brief authored when ready к execute milestone
3. Crystalka pastes full brief к Claude Code session
4. Execution + atomic commit
5. Brief marked EXECUTED, retained for history

**Cross-reference**: every brief должен reference its parent architecture document section.

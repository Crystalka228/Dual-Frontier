# tools/briefs — Brief Inventory

**Purpose**: Brief skeletons и full briefs для milestone execution.

**Reference**: `docs/METHODOLOGY.md`, `docs/KERNEL_ARCHITECTURE.md`, `docs/RUNTIME_ARCHITECTURE.md`

**Convention**:
- `K{N}_TITLE_BRIEF.md` — kernel milestone brief
- `M{N.M}_TITLE_BRIEF.md` — runtime milestone brief

**Status spectrum**:
- SKELETON — placeholder с TODO list (created at scaffolding time)
- DRAFT — brief authoring в progress
- READY — brief reviewed и approved для execution
- EXECUTED — milestone complete, brief retained для reference

**Current inventory**:
- K-series (kernel): K0-K8 skeletons (this scaffolding)
- M-series (runtime): per RUNTIME_ARCHITECTURE.md Part 2 (М9.0-М9.8 — TODO when приступаем к runtime)

**Workflow**:
1. Skeleton created at scaffolding time (this prompt's output)
2. Full brief authored when ready к execute milestone
3. Crystalka pastes full brief к Claude Code session
4. Execution + atomic commit
5. Brief marked EXECUTED, retained for history

**Cross-reference**: every brief должен reference its parent architecture document section.

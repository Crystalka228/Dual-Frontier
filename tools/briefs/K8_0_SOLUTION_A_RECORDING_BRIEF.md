---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_0
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_0
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_0
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_0
---
# K8.0 — Architectural decision recording (Solution A: single NativeWorld backbone)

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-09
**Status**: EXECUTED (2026-05-09, branch `feat/k8-0-solution-a-recording`, closure `9f9dc05`..`5fa3f1d`) — architectural decision brief, fourth brief type alongside «implementation», «skeleton», «closure-shaped implementation». See `docs/MIGRATION_PROGRESS.md` K8.0 closure section for closure summary.
**Reference docs**: `docs/architecture/KERNEL_ARCHITECTURE.md` v1.1 LOCKED §K8 (deliverables superseded by K8.0 closure), `docs/reports/PERFORMANCE_REPORT_K7.md` (K7 evidence base informing K8.0 decision), `docs/MIGRATION_PROGRESS.md` (live tracker — K8.0 row added on closure), `docs/methodology/METHODOLOGY.md`, `docs/methodology/CODING_STANDARDS.md`
**Companion**: `docs/MIGRATION_PROGRESS.md` (live tracker — K8.0 entry recorded; K8.1-K8.5 skeleton entries added)
**Methodology lineage**: `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md` (read-first/brief-second/execute-third pivot), `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md` (Anthropic `Edit` literal-mode semantics), `tools/briefs/K6_MOD_REBUILD_BRIEF.md` (closure-shaped brief format precedent)
**Predecessor**: K7 (`72ea8b5..60482f4`) — performance measurement, evidence base for this decision
**Target**: fresh feature branch `feat/k8-0-solution-a-recording` from `main` after K7 closure
**Estimated time**: 2-3 hours auto-mode (1-2 days at hobby pace ~1h/day)
**Estimated LOC delta**: ~+400 / -50 (KERNEL spec amendments + K8.1-K8.5 skeletons + MIGRATION_PROGRESS K8.0 closure section)

---

## Goal

Record the LOCKED architectural decision arising from the K7 performance evidence and Crystalka's stated philosophical commitment ("игра это стресс тест, чистая инженерия и исследование, архитектура которая будет работать десятилетиями без костылей"):

> **Solution A — single NativeWorld backbone**. All production storage migrates to `NativeWorld` (C++ kernel via Interop). Production systems migrate to `SpanLease<T>` reads + `WriteBatch<T>` writes. The seven class-based components (`MovementComponent`, `IdentityComponent`, `SkillsComponent`, `SocialComponent`, `StorageComponent`, `WorkbenchComponent`, `FactionComponent`) redesigned via native-side reference primitives (string interning, keyed maps, composite components) so they fit the `unmanaged` constraint. `ManagedWorld` retained as research artifact and test fixture only — not a production path.

K8.0 is **a pure documentation milestone**. Its deliverables are spec amendments, decision log entries, and milestone skeletons for the K8.1-K8.5 implementation series. **No source code is touched. No tests change. No native code is touched.** The executor's job is to render decisions verbally and accurately into LOCKED documents.

Subsequent K8.x milestones (authored sequentially after K8.0 closure) execute the migration:
- **K8.1** — Native-side reference handling primitives (string interning, keyed map, composite components). Foundation for K8.2.
- **K8.2** — Per-component redesign: 7 class components → struct equivalents using K8.1 primitives.
- **K8.3** — Production system migration: 12 vanilla systems → NativeWorld access patterns (`SpanLease<T>` / `WriteBatch<T>`).
- **K8.4** — `ManagedWorld` retired as production storage. Mod API v3 ships with NativeWorld-only access.
- **K8.5** — Mod ecosystem migration prep: documentation, migration guide, deprecation notices.

K8.0 authors skeletons for K8.1-K8.5; subsequent full briefs authored when triggered by predecessor closure.

---

## Phase 0 — Pre-flight verification

### 0.1 — Working tree clean

```
git status
```

**Expected**: `nothing to commit, working tree clean` on branch `main`.

**Halt condition**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K8-0-WIP"` and re-verify.

### 0.2 — Prerequisite milestone closed

```
git log --oneline -10
```

**Expected**: K7 closure commits visible (most recent: `60482f4` per K7 closure record). K7 row in MIGRATION_PROGRESS.md status DONE; K8 row status NOT STARTED.

**Halt condition**: K7 not closed. K8.0 references the K7 evidence base directly; without K7, the architectural rationale for Solution A is incomplete.

### 0.3 — Prerequisite documents at expected versions

```
head -5 docs/architecture/KERNEL_ARCHITECTURE.md
head -5 docs/MIGRATION_PROGRESS.md
ls docs/reports/PERFORMANCE_REPORT_K7.md
```

**Expected**:
- `KERNEL_ARCHITECTURE.md` Status: AUTHORITATIVE LOCKED v1.1
- `MIGRATION_PROGRESS.md` Last updated: 2026-05-09 (K7 closure)
- `PERFORMANCE_REPORT_K7.md` exists, status FINAL

**Halt condition**: any spec at unexpected version, or PERFORMANCE_REPORT_K7.md missing. K8.0 amendments build atop K7 closure state; mismatch indicates intervening work.

### 0.4 — Crystalka's K8 outcome decision recorded

The K8.0 brief encodes a specific Crystalka decision: **Solution A** chosen over Solutions B and C (and over Outcomes 2 and 3 from the K7 brief framing). Before any edit, the executor verifies this brief's `Goal` section accurately reflects the user's expressed commitment.

**Verification text** (read literally):

> Solution A — single NativeWorld backbone. All production storage migrates to NativeWorld. All production systems migrate to SpanLease/WriteBatch. Class components redesigned via native-side reference primitives. ManagedWorld retained as research artifact only.

If the brief author misrepresented the user's choice (e.g., the user said «hybrid» but the brief assumes Solution A), the executor halts. **No K8.0 amendments land on a misrepresented architectural commitment.**

The K8.0 brief is authored on `2026-05-09` per Crystalka's stated commitment in chat session: "У меня нет давления по времени в проекте, игра это стресс тест, тут всё чистая инженирия и исследование, так что можно развивать максимально сложную архитектуру которая будет работать десятилетиями без костылей".

**Halt condition**: any apparent mismatch between Crystalka's expressed commitment and the brief's encoded direction. Halt and request clarification before proceeding.

### 0.5 — Build and test baseline

```
dotnet build
dotnet test
```

**Expected**: build clean, **553 tests passing** (post-K7 baseline; K7 added benchmarks but no tests).

**Halt condition**: any regression. K8.0 starts from a known-good baseline; the milestone touches no source files, so the baseline must remain unchanged at K8.0 closure.

---

## Phase 1 — Architectural design (LOCKED — read-only, no edits)

This phase is the **architectural foundation** for the K8 series. The executor reads this section as the design contract; decisions here are LOCKED by Crystalka's stated commitment and the K7 evidence base.

### 1.1 — The decision and its scope

**Decision**: Solution A — single NativeWorld backbone for production storage.

**What this means structurally**:

1. **NativeWorld becomes THE production storage**. The current `World` class (managed, in `DualFrontier.Core.ECS`) is no longer the production path. After K8.4 closure, `World` is retained only as test fixture and research reference; production code constructs `NativeWorld` via the `Bootstrap` two-phase model.

2. **All vanilla components are unmanaged structs**. The 24 components already converted in K4 stay as-is. The 7 components currently class-based (per K4 Hybrid Path closure record: `MovementComponent`, `IdentityComponent`, `SkillsComponent`, `SocialComponent`, `StorageComponent`, `WorkbenchComponent`, `FactionComponent`) get redesigned in K8.2 using native-side reference primitives authored in K8.1.

3. **All production systems use `SpanLease<T>` + `WriteBatch<T>`**. The 12 vanilla systems currently using `World.GetComponent<T>(id)` / `World.SetComponent<T>(id, value)` get migrated in K8.3 to acquire spans for reads and queue writes through batches.

4. **Mod API v3 reflects NativeWorld-only**. The `IModApi` v2 currently presented to mods exposes managed `World` access patterns (per MOD_OS_ARCHITECTURE.md §4.6). v3 (authored as part of K8.4) exposes only `NativeWorld`-compatible access. Mod authors must migrate.

5. **K-L11 added to LOCKED foundational decisions**. The new entry codifies that NativeWorld is the production storage authority. Future architectural questions about "should X go through managed or native?" are settled by K-L11: if it's production storage, it's native.

### 1.2 — Why Solution A over Solutions B and C

**Solution A** (single NativeWorld backbone): chosen.

**Solution B** (storage abstraction `IComponentStore`): rejected.
- Adds a permanent runtime polymorphism layer between systems and storage.
- The abstraction exists only because the project couldn't decide which backend wins. K7 evidence + Crystalka's «no compromises» commitment makes the decision; the abstraction is no longer architecturally justified.
- Persistent costline structural: every `IComponentStore.GetComponent<T>` call has virtual dispatch overhead; every system writes against the abstraction with no clear backend awareness; refactoring across the abstraction is harder, not easier.
- Solution B is a "костыль" in the precise sense: it exists to defer a decision that the project is now committed to making.

**Solution C** (explicit hybrid: struct components on NativeWorld, class components stay on ManagedWorld): rejected.
- Permanently bifurcated storage backbone. Two storage authorities, two ownership models, two access patterns.
- Mod authors must understand which backend handles which component type. Adds permanent mental overhead to every mod author for the lifetime of the game and its modding ecosystem.
- Cross-storage queries (a system needs both struct and class components) become friction-heavy. Either the system queries each backend and merges in managed code (allocation pressure), or the abstraction layer (Solution B) re-emerges.
- The "structural costline" risk: Solution C looks clean today but creates friction every time a future component blurs the line. Each new component author must ask "should this be struct or class?" and the answer becomes "struct unless you really need a reference type" — at which point we're back to «class-based components prohibited» (K-L3) and the K8.1 reference-primitives work was needed anyway.

**Solution A wins on long-horizon cleanness**:
- Single storage authority. Single ownership boundary. Single mental model for every system author and mod author.
- K-L3 «unmanaged structs only» becomes truly enforced (currently softened to «Hybrid Path» by K4 lessons). K8.1 native-side primitives let truly all components live in native storage without reference-type fields leaking.
- K9 (RawTileField field storage) lands cleanly — same backbone.
- G-series (GPU compute over field storage) lands cleanly — same backbone.
- Future Phase 5 GPU-compute integration has zero architectural friction; data is already in native memory.

The cost — K8.1 native-side reference primitives + K8.2 component redesigns + K8.3 system migrations + K8.4 ManagedWorld retirement — is a 4-8 week investment that yields decade-scale cleanness.

### 1.3 — K-L11 (new LOCKED foundational decision)

**K-L11**: Production storage backbone

**Choice**: NativeWorld single source of truth.

**Rationale**: K7 evidence (V3 dominates V2 by 4-32× across §8 metrics with zero GC collections vs V2's 11/1/1 gen0/1/2) combined with «no compromises» commitment establishes NativeWorld as the production storage authority. ManagedWorld retained as test fixture and research artifact only.

**Implication**:
- All production systems (vanilla and third-party mods) read via `SpanLease<T>` and write via `WriteBatch<T>`.
- All production components are `unmanaged` structs (K-L3 fully realized; «Hybrid Path» exception removed in K8.2).
- `World` class is not a production storage option after K8.4; it's a development-time helper for tests that need a synchronous in-memory store with no native dependency.

**This is a permanent LOCKED commitment.** Departure requires explicit re-architecture milestone with K-L11 amendment to K-L11.1 or successor.

### 1.4 — K-L3 amendment (Hybrid Path retired)

**Current K-L3 wording** (v1.1):
> Component constraint: Unmanaged structs only (Path α). Storage requires blittable layout; class-based prohibited.

**Implication line currently reads**:
> All managed components must convert от class к struct (Phase 7 effort, 50-80 components). Mod components subject к same constraint.

**K4 closure introduced "Hybrid Path"** as a softening: 7 components (Movement/Identity/Skills/Social/Storage/Workbench/Faction) stayed as classes due to reference-type fields. This was operationally pragmatic but architecturally inconsistent with K-L3's literal wording.

**K8.0 amendment** (v1.2):

K-L3 wording stays unchanged (still "unmanaged structs only"). The implication line gets a new sub-paragraph:

> **K-L3 fully realized via K8.1 native-side reference primitives**: the seven class-based components retained under K4's Hybrid Path are redesigned in K8.2 using string interning (for string fields), native-side keyed maps (for Dictionary fields), and composite component patterns (for List/HashSet fields). After K8.2 closure, K-L3 holds without exception across vanilla and mod components alike. Hybrid Path is retired as a transitional pattern.

This makes K-L3's LOCKED status truly LOCKED — no exception-paths, no workarounds.

### 1.5 — K-L8 amendment (production status confirmed)

**Current K-L8 wording** (v1.1):
> Component lifetime: Native owns storage, managed holds opaque IntPtr. Single ownership boundary; managed holds handle only.

**K-L8 already says native owns storage** — but the document's tone treats this as a future state. K8.0 amendment confirms it as **production state**:

K-L8 wording stays unchanged. A new implication line is added:

> **K-L8 in production after K8.4**: post-K8.4 closure, `NativeWorld` is the only production storage path. `World` class is retained as test fixture and research reference (per K-L11). Production code constructs `NativeWorld` via `Bootstrap` two-phase model; no production code path constructs `World` directly.

### 1.6 — §K8 wording reconciliation (v1.1 → v1.2)

**Current §K8 framing** (v1.1, around line 727):
> ## K8 — Decision step + production cutover
>
> Three outcomes: ...

**K8.0 reconciles** the §K8 section to reflect that the decision was made: Solution A (effectively K7 brief's Outcome 1, hardened by «no compromises»). The "three outcomes" framing is replaced with a "decision recorded" framing pointing to K8.0..K8.5 sub-milestones.

New §K8 wording (full replacement of the existing section body):

```markdown
### K8 — Production storage cutover (Solution A: NativeWorld backbone)

**Decision** (recorded by K8.0 closure, 2026-MM-DD): Solution A — single NativeWorld backbone (per K-L11). Choice rationale in Part 4 Decisions log.

**Sub-milestone series**:
- **K8.0** — Architectural decision recording (this milestone; LOCKED v1.2)
- **K8.1** — Native-side reference handling primitives (string interning, keyed maps, composite components)
- **K8.2** — Per-component redesign for 7 class components (Movement, Identity, Skills, Social, Storage, Workbench, Faction)
- **K8.3** — Production system migration (12 vanilla systems → SpanLease/WriteBatch)
- **K8.4** — ManagedWorld retired as production; Mod API v3 ships
- **K8.5** — Mod ecosystem migration prep (documentation + migration guide)

**Cumulative time**: 4-8 weeks at hobby pace.

**LOC delta**: substantial — K8.1 adds ~600-1000 LOC (native + bridge); K8.2 modifies 7 component files plus their consumers; K8.3 modifies 12 system files plus tests; K8.4 deletes managed `World` production path; K8.5 adds documentation.
```

Old wording (the three-outcomes framing) is preserved in git history. Future readers see only the post-decision framing in HEAD.

### 1.7 — K9 sequencing relative to K8.x

K9 (RawTileField field storage) is currently authored as a full brief but blocked on «K6 complete» per its skeleton. Post-K8.0, K9's prerequisite changes:

**K9 new prerequisite**: K8.1 closure (native-side reference primitives may be reused by `RawTileField<T>` storage flags).

**K9 sequence options**:
- **Option a**: K9 runs in parallel with K8.2-K8.4 (independent migration tracks)
- **Option b**: K9 runs after K8.4 closure (full backbone migrated first, K9 lands on stable native)
- **Option c**: K9 runs immediately after K8.1 (K9 doesn't need K8.2/3/4; K8.1 primitives are sufficient)

**LOCKED choice (this brief): Option c**.

**Rationale**:
- K9 doesn't depend on K8.2/3/4 — RawTileField is parallel storage to RawComponentStore, not derived from it.
- Running K9 after K8.1 keeps the K-series tightly sequenced (K8.0 → K8.1 → K9 → K8.2 → ...).
- G-series gates on K9; getting K9 closed earlier unblocks future G0-G9 work.
- K8.2-K8.4 are larger milestones; running K9 between K8.1 and K8.2 provides natural pause/breathing room without blocking K-series progress.

**Sequencing recorded**: K8.0 closure updates `MIGRATION_PROGRESS.md` Cross-series coupling section to reflect K9-after-K8.1.

### 1.8 — Methodology note on the new brief type

K8.0 is **the first instance** of a fourth brief type in the project's pipeline:

| Type | Purpose | Examples |
|---|---|---|
| **Implementation brief** | Detailed instructions to write new code | K1, K2, K3, K5, K7, K8.1+ (future), K9 (full) |
| **Skeleton** | Place-holder pending authoring | G0–G9, K8.1-K8.5 (this milestone authors) |
| **Closure-shaped implementation brief** | Verify existing fulfillment + reconcile drift + fill adjacent gaps | K6, K6.1 |
| **Architectural decision brief** (NEW) | Record a LOCKED architectural commitment in spec; no source touched | K8.0 (this brief) |

The architectural decision brief variant is appropriate when:
1. A major directional decision has been made (Solution A here) that requires explicit LOCKED-document amendment.
2. The decision unlocks a series of subsequent implementation milestones whose skeletons should be authored at the same time the directional choice is recorded.
3. The risk of leaving the decision implicit (e.g., as a chat-log artifact only) is high — future readers and executors need spec wording, not chat-log archeology.

K8.0 establishes the pattern. Future architectural inflection points (e.g., M9.x decision branches if Vulkan runtime hits an unexpected fork) may use the same brief shape.

---

## Phase 2 — KERNEL_ARCHITECTURE.md v1.2 amendment

The executor amends `docs/architecture/KERNEL_ARCHITECTURE.md` from v1.1 to v1.2 with the changes designed in Phase 1. All edits are deterministic; the executor uses Anthropic `Edit` tool literal-mode semantics throughout.

### 2.1 — Bump version + status line

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Edit 1**: Replace
```
**Version**: 1.1
**Date**: 2026-05-07
**Status**: AUTHORITATIVE LOCKED — operational reference document, K6 wording reconciled with implementation reality
```
with
```
**Version**: 1.2
**Date**: 2026-05-09
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added, K-L3/K-L8 implications extended)
```

**Atomic commit**:
```
docs(kernel): bump status to v1.2 — Solution A commitment recorded
```

### 2.2 — Add K-L11 to Part 0 LOCKED decisions table

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Edit**: After the K-L10 row in the Part 0 table, before the existing «Implication of K-L3» line, insert a new row:

```
| K-L11 | Production storage backbone | NativeWorld single source of truth (Solution A); ManagedWorld retained as test fixture and research artifact only | K7 evidence (V3 dominates V2 by 4-32× across §8 metrics) + «no compromises» commitment; single ownership boundary, single mental model |
```

Then add new Implication line after the existing K-L9 implication:

```
**Implication of K-L11**: All production storage is NativeWorld. After K8.4 closure, no production code path constructs `World` directly. `World` class is retained for tests and research reference only. K8.1-K8.5 sub-milestones execute the migration; see Part 2 §K8.
```

**Atomic commit**:
```
docs(kernel): add K-L11 production storage backbone — Solution A LOCKED
```

### 2.3 — Extend K-L3 implication line

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Edit**: Find the existing K-L3 implication:
```
**Implication of K-L3**: All managed components must convert от class к struct (Phase 7 effort, 50-80 components). Mod components subject к same constraint.
```

Replace with:
```
**Implication of K-L3**: All managed components must convert от class к struct (K4 closed 24 of 31; K8.2 closes the remaining 7 via native-side reference primitives from K8.1). Mod components subject к same constraint. **K4's "Hybrid Path" softening retired in K8.2** — after K8.2 closure, K-L3 holds without exception across vanilla and mod components alike.
```

**Atomic commit**:
```
docs(kernel): K-L3 implication extended — Hybrid Path retirement in K8.2
```

### 2.4 — Extend K-L8 implication

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Edit**: After the existing K-L9 implication line in Part 0, insert:

```
**Implication of K-L8 in production**: Post-K8.4 closure, NativeWorld is the only production storage path. World class retained as test fixture and research reference (per K-L11). Production code constructs NativeWorld via Bootstrap two-phase model; no production code path constructs World directly.
```

(This is a separate implication line because K-L8 was already written; the new line clarifies the post-K8.4 production state.)

**Atomic commit**:
```
docs(kernel): K-L8 in-production implication added
```

### 2.5 — Replace §K8 body with sub-milestone framing

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Edit**: Find the existing `### K8 — Decision step + production cutover` section in Part 2 (around line 727 in v1.1). Replace its full body (from the `### K8` header through the end of the section, before the next `### K9` header) with the wording designed in §1.6 of this brief:

```markdown
### K8 — Production storage cutover (Solution A: NativeWorld backbone)

**Decision** (recorded by K8.0 closure, 2026-MM-DD; see `docs/MIGRATION_PROGRESS.md` K8.0 closure section): Solution A — single NativeWorld backbone (per K-L11). Choice rationale in Part 4 Decisions log.

**Sub-milestone series**:
- **K8.0** — Architectural decision recording (this milestone; LOCKED v1.2)
- **K8.1** — Native-side reference handling primitives (string interning, keyed maps, composite components)
- **K8.2** — Per-component redesign for 7 class components (Movement, Identity, Skills, Social, Storage, Workbench, Faction)
- **K8.3** — Production system migration (12 vanilla systems → SpanLease/WriteBatch)
- **K8.4** — ManagedWorld retired as production; Mod API v3 ships
- **K8.5** — Mod ecosystem migration prep (documentation + migration guide)

**Cumulative time**: 4-8 weeks at hobby pace.

**LOC delta**: substantial — K8.1 adds ~600-1000 LOC (native + bridge); K8.2 modifies 7 component files plus their consumers; K8.3 modifies 12 system files plus tests; K8.4 deletes managed World production path; K8.5 adds documentation.
```

The old "three outcomes" body is preserved in git history; HEAD reflects the post-decision framing.

**Atomic commit**:
```
docs(kernel): §K8 reconciled to Solution A sub-milestone series
```

### 2.6 — Update master plan table (Part 2)

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Edit**: Find the master plan table (around line 574 in v1.1):
```
| K8 | Decision step + production cutover | 1 week | +/- (refactor) |
```

Replace with:
```
| K8.0 | Architectural decision recording (Solution A) | 1-2 days | +/- (docs only) |
| K8.1 | Native-side reference handling primitives | 1-2 weeks | +600-1000 |
| K8.2 | 7 class components redesigned to structs | 1-2 weeks | -200/+300 |
| K8.3 | 12 vanilla systems migrated to SpanLease/WriteBatch | 2-3 weeks | -400/+600 |
| K8.4 | ManagedWorld retired; Mod API v3 ships | 1 week | -2000/+200 |
| K8.5 | Mod ecosystem migration prep | 3-5 days | +500 (docs) |
```

(K9 row stays unchanged, but moves logically after K8.1 per §1.7 sequencing decision; brief does not reorder rows in the table — sequencing tracked in MIGRATION_PROGRESS.md.)

**Atomic commit**:
```
docs(kernel): expand master plan with K8.0-K8.5 sub-milestones
```

### 2.7 — Add Part 4 Decisions log entry for Solution A

**File**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Edit**: In Part 4 «Decisions log», after the existing «Q4 — Two-phase entry point: clean separation» resolved entry, add a new resolved entry:

```markdown
**Solution A — single NativeWorld backbone (resolved 2026-05-09 per K8.0)**:
- K7 evidence: V3 (NativeWorld) dominates V2 (managed-with-structs) by 4× mean tick / 32× p99 / 27× total allocation / 0 vs 13 GC collections across 10k ticks
- Two alternatives considered:
  - Solution B (storage abstraction `IComponentStore` with managed and native impls): rejected — adds permanent runtime polymorphism layer, defers a decision the project is now committed to making, "structural костыль"
  - Solution C (explicit hybrid: struct components on Native, class components on Managed): rejected — bifurcated storage, permanent mental overhead for every mod author, cross-storage queries become friction
- **Solution A chosen**: single source of truth, K-L3 fully realized via K8.1 native-side reference primitives, K-L11 codifies commitment
- Crystalka commitment per chat session (2026-05-09): «игра это стресс тест, тут всё чистая инженирия и исследование, так что можно развивать максимально сложную архитектуру которая будет работать десятилетиями без костылей»
- Migration roadmap: K8.0 (decision) → K8.1 (primitives) → K8.2 (components) → K8.3 (systems) → K8.4 (retire managed) → K8.5 (mod ecosystem)
```

**Atomic commit**:
```
docs(kernel): Part 4 Decisions log — Solution A rationale recorded
```

---

## Phase 3 — K8.1-K8.5 skeleton authoring

The executor creates five new skeleton brief files in `tools/briefs/`. Each skeleton follows the existing skeleton format (cf. `tools/briefs/G0_*` for reference if those exist, or use the K7 skeleton from before its full authoring as a template — Status SKELETON, brief authoring trigger noted).

### 3.1 — K8.1 skeleton

**File**: `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` (NEW)

**Content**:

```markdown
# K8.1 — Native-side reference handling primitives

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K8 (Solution A sub-milestone series), K-L3 implication, K-L11
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
```

### 3.2 — K8.2 skeleton

**File**: `tools/briefs/K8_2_CLASS_COMPONENT_REDESIGN_BRIEF.md` (NEW)

**Content**:

```markdown
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
```

### 3.3 — K8.3 skeleton

**File**: `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` (NEW)

**Content**:

```markdown
# K8.3 — 12 vanilla systems migrated to SpanLease/WriteBatch

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K8, K-L7 (span protocol), K-L11 (NativeWorld backbone)
**Prerequisite**: K8.2 closure (all components are unmanaged structs)

## Goal

Migrate the 12 vanilla production systems from `World.GetComponent` / `World.SetComponent` access patterns to `NativeWorld` + `SpanLease<T>` reads + `WriteBatch<T>` writes. After K8.3 closure, all production system code runs against NativeWorld.

## Systems in scope (per `GameBootstrap.coreSystems`)

NeedsSystem, MoodSystem, JobSystem, ConsumeSystem, SleepSystem, ComfortAuraSystem, MovementSystem, PawnStateReporterSystem, InventorySystem, HaulSystem, ElectricGridSystem, ConverterSystem.

## Time estimate

2-3 weeks at hobby pace.

## Deliverables (high-level)

- Each system rewritten to use SpanLease/WriteBatch access patterns
- Per-system atomic commits (12 atomic commits)
- Each system's tests updated to construct NativeWorld via test fixture
- New shared test fixture `NativeWorldTestFixture` for system tests
- Performance comparison commit: K8.3 vs K7 V2 baseline (sanity check that production-system-on-NativeWorld matches K7 V3 numbers within 20%)

## TODO

- [ ] Author full brief
- [ ] Per-system access pattern analysis (read-only vs read-write; phase placement; deferred-event publish ordering)
- [ ] Test fixture design — how does a system test construct a NativeWorld with the right components registered?
- [ ] Migrate test count (some tests currently use ManagedWorld directly; switching to NativeWorld may surface latent assumptions)
- [ ] Decide if K8.3 includes a benchmark commit confirming production-on-NativeWorld matches K7 V3 within 20% (recommended: yes)

**Brief authoring trigger**: after K8.2 closure.
```

### 3.4 — K8.4 skeleton

**File**: `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` (NEW)

**Content**:

```markdown
# K8.4 — ManagedWorld retired as production; Mod API v3 ships

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` K-L11, K-L8 in-production implication, MOD_OS_ARCHITECTURE.md §4.6 (IModApi v2 → v3)
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
```

### 3.5 — K8.5 skeleton

**File**: `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` (NEW)

**Content**:

```markdown
# K8.5 — Mod ecosystem migration prep (documentation + guide)

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` K-L9 (mod parity), MOD_OS_ARCHITECTURE.md §4.6 (Mod API v3, post-K8.4)
**Prerequisite**: K8.4 closure (Mod API v3 shipped)

## Goal

Documentation deliverables enabling existing mod authors to migrate from v2 to v3. Project does not yet have an external mod ecosystem (game pre-release), but documentation is authored against the assumption that a future mod ecosystem will need it. This is forward-looking infrastructure work.

## Time estimate

3-5 days at hobby pace.

## Deliverables (high-level)

- `docs/MOD_AUTHORING_GUIDE.md` rewritten for v3 (NativeWorld access patterns, SpanLease/WriteBatch usage, performance considerations, debugging tips)
- `docs/MOD_API_V2_TO_V3_MIGRATION.md` step-by-step migration guide
- Sample mod project updated: `samples/SampleMod/` migrated v2 → v3 as exemplar
- DEPRECATED notices in any v2 reference documentation (e.g., MOD_OS_ARCHITECTURE.md §4.6 v2 sections explicitly marked DEPRECATED with v3 cross-reference)
- Optional: video/blog-post outline if Crystalka wants to publish migration content (deferred decision)

## TODO

- [ ] Author full brief
- [ ] Sample mod project status (does it exist post-K8.4? If not, K8.5 creates it)
- [ ] Identify all v2 documentation that needs DEPRECATED notice or rewrite

**Brief authoring trigger**: after K8.4 closure.
```

### 3.6 — Atomic commits for Phase 3

Five sequential commits (one per skeleton):

```
docs(briefs): K8.1 skeleton — native-side reference primitives
docs(briefs): K8.2 skeleton — 7 class components redesigned to structs
docs(briefs): K8.3 skeleton — 12 vanilla systems migrated to SpanLease/WriteBatch
docs(briefs): K8.4 skeleton — ManagedWorld retired; Mod API v3 ships
docs(briefs): K8.5 skeleton — mod ecosystem migration prep
```

---

## Phase 4 — MIGRATION_PROGRESS.md update

The executor amends `docs/MIGRATION_PROGRESS.md` to reflect K8.0 closure and the K8.1-K8.5 sub-milestone roadmap.

### 4.1 — Update Last updated

```
**Last updated**: 2026-05-09 (K7 closure)
```

becomes

```
**Last updated**: 2026-05-09 (K8.0 closure — Solution A architectural commitment recorded)
```

### 4.2 — Update Current state snapshot

- `Active phase`: K8 → K8.1 (next per K8.0 closure roadmap)
- `Last completed milestone`: K7 → K8.0
- `Tests passing`: 553 (unchanged; K8.0 touched no source)
- `Sequencing strategy`: append «K8 split into sub-milestones K8.0-K8.5 per K8.0 closure»

### 4.3 — Replace K8 row in Overview table with K8.0-K8.5

**Current K-series Overview row**:
```
| K8 | Decision step + production cutover | NOT STARTED | 1 week | — | — |
```

Replace with:
```
| K8.0 | Architectural decision recording (Solution A) | DONE | 1-2 days | <commit SHA range> | <date> |
| K8.1 | Native-side reference handling primitives | NOT STARTED | 1-2 weeks | — | — |
| K8.2 | 7 class components redesigned to structs | NOT STARTED | 1-2 weeks | — | — |
| K8.3 | 12 vanilla systems migrated to SpanLease/WriteBatch | NOT STARTED | 2-3 weeks | — | — |
| K8.4 | ManagedWorld retired; Mod API v3 ships | NOT STARTED | 1 week | — | — |
| K8.5 | Mod ecosystem migration prep | NOT STARTED | 3-5 days | — | — |
```

### 4.4 — Add K8.0 closure section after K7

**File**: `docs/MIGRATION_PROGRESS.md`

After the K7 closure section (where K7 row is recorded), add:

```markdown
### K8.0 — Architectural decision recording (Solution A: NativeWorld backbone)

- **Status**: DONE (`<commit SHA range>`, <date>)
- **Brief**: `tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md` (FULL EXECUTED)
- **Brief type**: Architectural decision brief (fourth brief type — see brief §1.8)
- **Decision recorded**: Solution A — single NativeWorld backbone for production storage. ManagedWorld retained as test fixture and research reference only. K-L11 added to LOCKED foundational decisions; K-L3 and K-L8 implications extended; §K8 reconciled to K8.0-K8.5 sub-milestone series.
- **Context**:
  - K7 evidence (`docs/reports/PERFORMANCE_REPORT_K7.md`): V3 (NativeWorld) dominates V2 (managed-with-structs) by 4× mean tick / 32× p99 / 27× total allocation / 0 vs 13 GC collections across 10k ticks on Skarlet hardware
  - Crystalka commitment (chat session 2026-05-09): «игра это стресс тест, тут всё чистая инженирия и исследование, так что можно развивать максимально сложную архитектуру которая будет работать десятилетиями без костылей»
  - Solution B (storage abstraction layer) and Solution C (explicit hybrid) rejected — both are "structural costlines" relative to long-horizon cleanness
- **Migration roadmap**: K8.0 → K8.1 (primitives) → K9 (RawTileField; sequencing decision per brief §1.7) → K8.2 (component redesigns) → K8.3 (system migrations) → K8.4 (ManagedWorld retirement, Mod API v3) → K8.5 (mod ecosystem prep)
- **Test count**: 553 (unchanged — K8.0 is documentation-only)
- **Lessons learned**:
  - Architectural decision briefs (fourth brief type) are appropriate when a major directional choice needs LOCKED-spec recording. K8.0 establishes the pattern; future architectural inflection points may use the same shape.
  - Solution A's cost is bounded (4-8 weeks across K8.1-K8.5) but yields decade-scale cleanness. The «no compromises» commitment makes the trade calculation explicit: shorter-term pragmatism (Solutions B/C) creates structural costlines that propagate through every future system author and mod author. The decision is recorded, not deferred.
  - K9 sequencing decision (Option c — K9 between K8.1 and K8.2) was made in this brief. This unblocks G-series earlier without disrupting K8 series flow. Recorded in brief §1.7.
```

### 4.5 — Add K8.1-K8.5 skeleton entries (high-level only)

After the K8.0 closure section, add a brief subsection:

```markdown
### K8.1-K8.5 — Sub-milestones

Each sub-milestone has a SKELETON brief in `tools/briefs/`. Full brief authoring is triggered sequentially after each predecessor closure. See K8.0 closure migration roadmap above for the order.

- **K8.1**: `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` (skeleton)
- **K8.2**: `tools/briefs/K8_2_CLASS_COMPONENT_REDESIGN_BRIEF.md` (skeleton)
- **K8.3**: `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` (skeleton)
- **K8.4**: `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` (skeleton)
- **K8.5**: `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` (skeleton)
```

### 4.6 — Add D5 to Decisions log

After the existing D4 entry, add:

```markdown
### D5 — Solution A (single NativeWorld backbone) chosen for K8 series

- **Date**: 2026-05-09
- **Decision**: Solution A — NativeWorld single source of truth for production storage. ManagedWorld retained as test fixture only.
- **Alternatives rejected**:
  - Solution B (storage abstraction `IComponentStore`): permanent runtime polymorphism layer; defers a decision the project is committed to making; "structural костыль".
  - Solution C (explicit hybrid: struct components on Native, class components on Managed): bifurcated storage; permanent mental overhead for every mod author; cross-storage queries become friction.
- **Rationale**: K7 evidence (V3 dominates V2 by 4-32× across §8 metrics) + Crystalka commitment («архитектура на десятилетия без костылей») + K-L11 codification.
- **Reversal trigger**: only if K8.1 native-side reference primitives prove fundamentally infeasible (e.g., cannot match managed Dictionary semantics within reasonable performance budget). Reversal would re-open Solution C as fallback, with explicit re-architecture milestone.
- **Implementation roadmap**: K8.0 (this milestone) → K8.1 → K9 → K8.2 → K8.3 → K8.4 → K8.5.
```

### 4.7 — Update K9 prerequisite line

Find the K9 entry in the K-series Overview table:

```
| K9 | Field storage abstraction (`RawTileField<T>`) | NOT STARTED | 1-2 weeks | — | — |
```

(Stays unchanged — K9 row remains NOT STARTED. But add a note: see Cross-series coupling section update.)

In Cross-series coupling section, add:

```
**K9 sequencing post-K8.0**: K9 prerequisite changes from "K6 complete" to "K8.1 closure" per K8.0 brief §1.7 sequencing decision (Option c). Rationale: K9 reuses K8.1 native-side reference primitives where applicable; K9 doesn't depend on K8.2/3/4. K9 runs between K8.1 and K8.2 for natural pause in K-series progression.
```

### 4.8 — Atomic commits for Phase 4

```
docs(migration): K8.0 closure recorded — Solution A LOCKED, K8.1-K8.5 sub-milestones added
docs(migration): D5 decision log — Solution A rationale and reversal trigger
```

---

## Phase 5 — Final verification

### 5.1 — Build + test gate

```
dotnet build
dotnet test
```

**Expected**: 0 errors, 0 warnings, 553 tests passing (K8.0 touched no source files; tests must remain at K7 baseline).

**Halt condition**: any regression. K8.0 is pure documentation; source-tree state must be byte-identical to pre-K8.0.

### 5.2 — Pre-commit grep

Verify no stale references to old §K8 «three outcomes» framing remain in docs:

```
grep -rn "Outcome 1\|Outcome 2\|Outcome 3" docs/ tools/briefs/
```

**Expected**: matches in `PERFORMANCE_REPORT_K7.md` (correct, references K8 outcome direction advisory) and possibly K7 brief (correct, historical context). NO matches in `KERNEL_ARCHITECTURE.md` post-§K8 reconciliation.

If `KERNEL_ARCHITECTURE.md` still contains "Outcome 1" / "Outcome 2" / "Outcome 3" references, the §K8 reconciliation in Phase 2.5 was incomplete — halt.

### 5.3 — Spec consistency check

```
grep -n "K-L11\|Solution A" docs/architecture/KERNEL_ARCHITECTURE.md
```

**Expected**: K-L11 referenced in Part 0 (table row + implication line), Part 2 §K8 (decision line), Part 4 (decisions log entry). 5+ matches.

```
grep -n "K8.0\|K8\.1\|K8\.2\|K8\.3\|K8\.4\|K8\.5" docs/MIGRATION_PROGRESS.md
```

**Expected**: K8.0 closure section, K8.1-K8.5 skeleton entries section, K8.1-K8.5 in Overview table. 15+ matches.

### 5.4 — Skeleton files consistency check

```
ls tools/briefs/K8_*.md
```

**Expected**:
- `K8_0_SOLUTION_A_RECORDING_BRIEF.md` — this brief, status post-execution becomes EXECUTED
- `K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` — skeleton
- `K8_2_CLASS_COMPONENT_REDESIGN_BRIEF.md` — skeleton
- `K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` — skeleton
- `K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` — skeleton
- `K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` — skeleton

6 files total.

### 5.5 — Mark K8.0 brief EXECUTED

**File**: `tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md`

Status line: `AUTHORED` → `EXECUTED (2026-MM-DD, branch <branch>, closure <commit SHA range>)`. Add link to MIGRATION_PROGRESS.md K8.0 closure section.

**Atomic commit** (final):
```
docs(briefs): mark K8.0 brief as EXECUTED with closure refs
```

---

## Atomic commit log expected

Approximate commit count: **15-17**:

**Phase 2 — KERNEL_ARCHITECTURE.md (7 commits)**:
1. `docs(kernel): bump status to v1.2 — Solution A commitment recorded`
2. `docs(kernel): add K-L11 production storage backbone — Solution A LOCKED`
3. `docs(kernel): K-L3 implication extended — Hybrid Path retirement in K8.2`
4. `docs(kernel): K-L8 in-production implication added`
5. `docs(kernel): §K8 reconciled to Solution A sub-milestone series`
6. `docs(kernel): expand master plan with K8.0-K8.5 sub-milestones`
7. `docs(kernel): Part 4 Decisions log — Solution A rationale recorded`

**Phase 3 — Skeletons (5 commits)**:
8. `docs(briefs): K8.1 skeleton — native-side reference primitives`
9. `docs(briefs): K8.2 skeleton — 7 class components redesigned to structs`
10. `docs(briefs): K8.3 skeleton — 12 vanilla systems migrated to SpanLease/WriteBatch`
11. `docs(briefs): K8.4 skeleton — ManagedWorld retired; Mod API v3 ships`
12. `docs(briefs): K8.5 skeleton — mod ecosystem migration prep`

**Phase 4 — MIGRATION_PROGRESS.md (2 commits)**:
13. `docs(migration): K8.0 closure recorded — Solution A LOCKED, K8.1-K8.5 sub-milestones added`
14. `docs(migration): D5 decision log — Solution A rationale and reversal trigger`

**Phase 5 — Brief mark (1 commit)**:
15. `docs(briefs): mark K8.0 brief as EXECUTED with closure refs`

A merge commit is **not** in this list — fast-forward merge to main.

---

## Cross-cutting design constraints

1. **No source code changes**. K8.0 is documentation-only. Any edit to `src/`, `tests/`, or `native/` is a brief violation — halt and review.

2. **No test count change**. Test baseline at K8.0 closure must be exactly 553 (the K7 baseline). Source-tree state byte-identical to pre-K8.0.

3. **K-L11 is permanent**. The new LOCKED decision codifies Solution A. Future amendments require explicit re-architecture milestone, not casual spec edits. The brief's Phase 2.2 wording reflects this — K-L11 is added in the same form as K-L1 through K-L10.

4. **No regex metacharacters in `Edit` tool boundaries** (per `MOD_OS_V16_AMENDMENT_CLOSURE.md`).

5. **Atomic commits per logical change** (project standing rule). One commit per brief sub-step. Each commit must build cleanly; though K8.0 changes no code, the build/test gate at Phase 5 verifies no document edit accidentally invalidates a code reference.

6. **Architectural decision brief discipline**: the Phase 1 design contract is LOCKED by Crystalka's commitment + K7 evidence. The executor does NOT re-evaluate Solution A vs B vs C; the executor records the LOCKED decision. If a tension arises during execution (e.g., wording feels wrong, K-L11 phrasing seems too strong), the executor halts and escalates — the brief author re-considers.

7. **Skeleton brief authoring discipline**: K8.1-K8.5 skeletons are SKELETONS — short, focused, with clear "Brief authoring trigger" lines. The executor does NOT write full implementation briefs in K8.0; that's premature optimization. Full briefs land in their own milestones.

8. **«Data exists or it doesn't» applied to documentation**: every reference to K-L11, Solution A, K8.0-K8.5 in the amended documents resolves to a real anchor (table row, section, decisions log entry). No dangling references.

9. **K9 sequencing is recorded**: §1.7 LOCKS K9-after-K8.1 (Option c). This is the second decision K8.0 makes (alongside Solution A); it gets recorded in MIGRATION_PROGRESS.md Cross-series coupling section per Phase 4.7.

10. **Methodology note land in the brief itself**: §1.8 (architectural decision brief as fourth brief type) is documentation in the brief itself; it doesn't need to land in a separate methodology document. Future K-series briefs reference this section if they take the same shape.

---

## Stop conditions

The executor halts and escalates the brief authoring session if any of the following:

1. Phase 0 pre-flight check fails — working tree dirty, K7 not closed, specs at unexpected version, or PERFORMANCE_REPORT_K7.md missing.

2. Phase 0.4 detects mismatch between brief's encoded direction (Solution A) and Crystalka's expressed commitment. Halt — no LOCKED amendments land on a misrepresented architectural choice.

3. Phase 2 edit touches `KERNEL_ARCHITECTURE.md` in a place not designed in §1.3-1.6 of this brief. The wording is LOCKED here; deviations halt.

4. Phase 2.5 «Outcome 1/2/3» wording reconciliation leaves stale references elsewhere (caught by Phase 5.2 grep). If grep finds stale references, halt for re-edit.

5. Phase 3 skeleton authoring deviates from the format. Each skeleton must have status SKELETON, brief authoring trigger, deliverables list, TODO list, prerequisite. Brief format consistency matters for future executor runs.

6. Phase 4 MIGRATION_PROGRESS.md edit accidentally invalidates K7 closure section or earlier closure sections. Halt — closures are append-only; edits to past closures require explicit decision.

7. Any phase touches source files (per cross-cutting constraint #1).

8. Phase 5 build/test gate fails. K8.0 must not regress baseline; if it does, an edit accidentally affected a referenced code path.

9. The `Edit` tool reports unexpected behavior on any oldText/newText pair.

10. K-L11 wording lands but Solution A rationale is unclear or seems too strong/weak. The wording is LOCKED here; if uncertain, halt and request brief author re-review.

The fallback in every halt case is `git stash push -m "k8-0-WIP-halt-$(date +%s)"` and report to the brief author.

---

## Brief authoring lineage

- **2026-05-09** — K7 closed (`72ea8b5..60482f4`); K7 evidence base in `docs/reports/PERFORMANCE_REPORT_K7.md` available. Crystalka selects Solution A in chat session per «архитектура на десятилетия без костылей» commitment. K8.0 brief authored same day to record the architectural decision in LOCKED specs and author K8.1-K8.5 skeletons.
- **(date TBD)** — Executed and closed at K8.0 milestone closure.

The brief was authored read-first / brief-second per the methodology pivot. Source documents read during authoring: `KERNEL_ARCHITECTURE.md` v1.1 LOCKED (Part 0 K-L1..K-L10, §K8, Part 4 Decisions log), `MIGRATION_PROGRESS.md` (K7 closure, K9 prerequisites, decisions log D1..D4), `PERFORMANCE_REPORT_K7.md` (V2 vs V3 evidence base). The "no compromises" rule applied: Solution A locked, B/C rejected with rationale, K9 sequencing recorded as Option c not deferred.

---

## Methodology note

K8.0 introduces the **fourth brief type** in the project's pipeline. Per §1.8, the brief type catalogue now reads:

| Type | Purpose | Examples |
|---|---|---|
| **Implementation brief** | Detailed instructions to write new code | K1, K2, K3, K5, K7, K8.1+ (future), K9 (full) |
| **Skeleton** | Place-holder pending authoring | G0–G9, K8.1-K8.5 (this milestone authors) |
| **Closure-shaped implementation brief** | Verify existing fulfillment + reconcile drift + fill adjacent gaps | K6, K6.1 |
| **Architectural decision brief** | Record a LOCKED architectural commitment in spec; no source touched | K8.0 (this brief) |

The architectural decision brief variant is appropriate at directional inflection points where a chat-log decision needs LOCKED-spec recording. K8.0 establishes the pattern; future inflection points (potentially M9.x mid-runtime decisions, or G-series Phase 5 GPU compute commitment) may use the same shape.

The pattern's value: the decision is **explicit and persistent**. Future Crystalka 6 months from now reading the spec sees K-L11 and the §K8 framing without needing to reconstruct chat-log context. Future executors authored against the LOCKED decision do not re-litigate Solution A vs B vs C — that question is settled.

---

**Brief end.** Awaits Crystalka's review and feed to Claude Code session for execution.

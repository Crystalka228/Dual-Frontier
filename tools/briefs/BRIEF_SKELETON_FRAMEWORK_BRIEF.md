# Brief Skeleton Framework — Execution Brief

**Brief authored**: 2026-05-17 (Claude Opus 4.7, deliberation mode)
**Target session**: Claude Code execution mode
**Brief type**: Skeleton framework authoring + schema extension
**Estimated time**: 2-3 hours auto-mode (3-4 days at hobby pace)
**Branch**: `claude/brief-skeleton-framework`
**Parent**: К10 amendments application closure (commit `5649a51`, 2026-05-17)

---

## 0. Executive summary

Establish skeleton brief framework for remaining К-series + А' forward work. Four skeleton brief files authored at Tier 3 with new lifecycle value `AUTHORED-SKELETON`. Schema extension (8 → 9 lifecycle values) applied к FRAMEWORK.md + sync_register.ps1 + query_register.ps1. All four skeleton briefs enrolled в REGISTER.yaml.

**Goal**: Forward operational plan visible в register — К10 execution → К10 cross-doc cascade → К-closure report → Roslyn analyzer skeletons present, awaiting full brief authoring at proper milestone timing.

**Out of scope**:
- К8.5 brief — already AUTHORED (DOC-D-K8_5 в REGISTER); not skeleton
- Phase B M-series skeletons — explicitly excluded per Crystalka Q1 lock
- FO research forks — explicitly excluded per Crystalka Q1 lock
- Full brief authoring для skeletons — separate deliberation sessions (К10 execution brief is largest, requires Opus deliberation session per session-mode boundary)

**Four skeleton briefs к author**:

1. **К10 execution brief** (DOC-D-K10_EXECUTION) — implementation of 46-item К10 specification
2. **К10 cross-document amendments cascade brief** (DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE) — 8-document queue amendments (KERNEL_ARCHITECTURE, VULKAN_SUBSTRATE, MOD_OS, DualFrontier.Persistence, KernelCapabilityRegistry, PHASE_A_PRIME_SEQUENCING, README, К-closure report scoping)
3. **А'.8 K-closure report brief** (DOC-D-A_PRIME_8_K_CLOSURE_REPORT) — К-series formal closure + provisional Lessons promotion review
4. **А'.9 Roslyn analyzer brief** (DOC-D-A_PRIME_9_ROSLYN_ANALYZER) — К-Lxx invariant encoding + first-run analyzer milestone

**Target outcomes**:

- 1 atomic commit: FRAMEWORK.md schema extension (8 → 9 lifecycle values)
- 1 atomic commit: sync_register.ps1 + query_register.ps1 tooling update
- 4 atomic commits: each skeleton brief authored (1 commit per brief = atomic compilable unit per Lesson #8)
- 1 atomic commit: REGISTER.yaml — 4 new DOC-D entries with `AUTHORED-SKELETON` lifecycle
- 1 atomic commit: closure protocol (sync_register --validate, render regen, EVT entry, PENDING-COMMIT backfill)

**Total**: 7 atomic commits expected.

**Lesson discipline reminders**:

- **Lesson #7**: APIs transcribed verbatim. Tooling change requires reading actual sync_register.ps1 source — no paraphrasing of enum array.
- **Lesson #8**: Each skeleton brief commit produces compilable state (well-formed markdown + valid REGISTER entry).
- **Lesson #11**: Architectural reduction check — К8.5 NOT in scope (already AUTHORED, would be redundant entry). Apply check before adding entries.
- **Lesson #20**: Schema extension chosen over special_case_rationale workaround — К-L14 application (architectural cleanliness over effort cost).
- **Lesson #22**: Read existing FRAMEWORK.md, sync_register.ps1, REGISTER.yaml К-series entries before authoring tooling change or skeleton entries.

---

## 1. Phase 0 — Preflight reads (mandatory)

### 1.1 Schema specification documents

- `docs/governance/FRAMEWORK.md` v1.0 LOCKED — read fully. §3.3 (eight lifecycle states) is target of amendment.
- `docs/governance/SYNTHESIS_RATIONALE.md` v1.0 LOCKED — read source-standard provenance for lifecycle vocabulary. ISO 9001 7.5 contributes document-control field semantics; new lifecycle value should align с ISO 9001 control discipline.
- `docs/governance/REGISTER.yaml` — read fully (192 KB). Navigation source of truth. Particularly read all Tier 3 Category D entries for skeleton brief enrollment pattern.

### 1.2 Tooling source

- `tools/governance/sync_register.ps1` — read fully. Particular attention к:
  - Line ~115 `$VALID_LIFECYCLES` array definition
  - Line ~155 forbidden Tier × Lifecycle combinations (`'1+AUTHORED','3+LOCKED','5+STALE','4+LOCKED'`)
  - Schema metadata version check (line ~80)
- `tools/governance/query_register.ps1` — read fully. Lifecycle parameter docstring (lines ~12-13) lists 8 values inline; needs update.
- `tools/governance/render_register.ps1` — read fully. No hardcoded lifecycle enum (renders whatever value is in YAML); no change needed.

### 1.3 К10 amendment context

- `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 LOCKED — К10 specification (1875 lines). Used as authority for К10 execution brief skeleton + cross-doc amendments scope.
- `K10_DELIBERATION_STATE.md` — Project file, attached to session. Used for skeleton brief context.
- `docs/methodology/METHODOLOGY.md` v1.8 — Lessons #11/#20/#22 + Provisional Lessons section. К-closure report skeleton scope includes provisional lesson promotion review.

### 1.4 Related architecture documents

- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` Live — Phase A' sequencing. Verify А'.6 К8.5 → А'.7 К10 → А'.8 K-closure → А'.9 analyzer sequence is current.
- `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.4 — К/M boundary. К-closure report skeleton references К-series → M-series transition.

### 1.5 Existing brief precedents

Read examples of recently-authored briefs к match formatting + Phase 0/N/closure conventions:

- `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md` — recent full execution brief (precedent for К10 execution brief skeleton structure)
- `tools/briefs/CLEANUP_CASCADE_BRIEF.md` — recent multi-document amendment brief (precedent for К10 cross-doc cascade)
- `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` — К-series spec implementation precedent
- `tools/briefs/COMPOSITE_NAMESPACE_RATIFICATION_BRIEF.md` — recent ratification cascade precedent

### 1.6 Working tree state

```bash
git status
```

Must be clean before starting. If not clean, halt HG-1.

```bash
git log --oneline -10 main
```

Verify recent commits include К10 amendments closure (`5649a51` or its merged equivalent on main). If main doesn't reflect К10 closure, halt HG-3 (unexpected state).

---

## 2. Phase 1 — Schema extension: AUTHORED-SKELETON lifecycle value

### 2.1 Scope

FRAMEWORK.md §3.3 (eight lifecycle states) extends к nine. New value: `AUTHORED-SKELETON`.

### 2.2 FRAMEWORK.md amendment

#### 2.2.1 §3.3 table extension

Locate existing table:

```
| State | Description | Mutability |
|---|---|---|
| **Draft** | Authored but not finalized; revisable freely | Full mutation |
| **Live** | Actively maintained; mutable on every relevant milestone | Full mutation per closure |
| **LOCKED** | Change authority via formal amendment milestone only | Restricted mutation |
| **EXECUTED** | Brief that has been run; historical immutable | Only «Lessons learned» appendable |
| **AUTHORED** | Brief authored, awaits execution; revisable via patch brief | Patch-brief mutation pattern |
| **DEPRECATED** | Superseded by successor; retained for historical context | Read-only with cross-reference |
| **SUPERSEDED** | Replaced by newer version of same logical document | Read-only with cross-reference |
| **STALE** | Known out-of-date; awaits update or formal archive | Surfaced by audit; not steady state |
```

Insert new row **between AUTHORED and DEPRECATED** (logical placement — AUTHORED-SKELETON is upstream of AUTHORED):

```
| **AUTHORED-SKELETON** | Skeleton brief authored — title + scope + sub-milestones + reads + halt + Q-N seeded + closure protocol stub. Awaits full brief authoring at proper milestone timing | Patch-brief mutation pattern; full brief authoring promotes к AUTHORED |
```

#### 2.2.2 §3.3 prose surrounding text

Add new paragraph after table:

> The AUTHORED-SKELETON lifecycle (added 2026-05-17 per К10 forward planning skeleton framework brief) captures the upstream state of brief authoring — when the project's forward operational plan is visible in the register before full briefs are authored. Skeletons carry expected scope, sub-milestones, Phase 0 reads, halt conditions, and Q-N seeds; they are revisable via patch brief pattern (precedent: K9_BRIEF_REFRESH_PATCH). Full brief authoring promotes the lifecycle к AUTHORED. Distinguishes from full AUTHORED state where brief is execution-ready.

#### 2.2.3 §3.3.1 transition matrix extension

Locate existing transition matrix. Add new transitions:

```
Draft → AUTHORED-SKELETON     (skeleton authored from forward planning)
AUTHORED-SKELETON → AUTHORED   (full brief authoring complete; promotion)
AUTHORED-SKELETON → DEPRECATED (skeleton abandoned without promotion)
AUTHORED-SKELETON → SUPERSEDED (skeleton replaced by new skeleton; rare)
```

#### 2.2.4 §3.4 allowed-combinations matrix

Update Category D row:

Existing:
```
| D | 3 | AUTHORED, EXECUTED, DEPRECATED, SUPERSEDED | Brief-specific |
```

Replace with:
```
| D | 3 | AUTHORED-SKELETON, AUTHORED, EXECUTED, DEPRECATED, SUPERSEDED | Brief-specific |
```

#### 2.2.5 §3.4.1 forbidden combinations

Add new line к forbidden combinations list:

```
- Tier 1 + Lifecycle AUTHORED-SKELETON (skeletons are briefs Tier 3 only)
- Tier 2 + Lifecycle AUTHORED-SKELETON (skeletons are briefs Tier 3 only)
- Tier 4 + Lifecycle AUTHORED-SKELETON (module-local cannot be skeleton)
- Tier 5 + Lifecycle AUTHORED-SKELETON (ideas not skeletons)
```

#### 2.2.6 Version increment

FRAMEWORK.md frontmatter:

```yaml
version: "1.0"
```

Update к:

```yaml
version: "1.1"
```

Add new version entry к document prose top (after existing «*Version: 1.0 (2026-05-12). Schema lock at A'.4.5 closure. [...]*»):

> *Version: 1.1 (2026-05-17). Schema extension AUTHORED-SKELETON lifecycle added per К10 forward planning skeleton framework. §3.3 eight → nine lifecycle values; §3.3.1 transition matrix extended с four new transitions; §3.4 allowed-combinations matrix updated Category D row; §3.4.1 forbidden combinations extended. Schema version stays 1.0 (minor change, no breaking enum removal); tooling forward-compatible.*

### 2.3 Schema metadata

REGISTER.yaml schema_version field stays "1.0" (minor extension, not breaking). Tooling change validates new value through enum array update — backward compatible.

### 2.4 Atomic commit 1: FRAMEWORK.md schema extension

```
git checkout -b claude/brief-skeleton-framework
[edit docs/governance/FRAMEWORK.md per §2.2.1-2.2.6]
git add docs/governance/FRAMEWORK.md
git commit -m "governance(framework): add AUTHORED-SKELETON lifecycle value per brief skeleton framework

FRAMEWORK.md v1.0 → v1.1 — schema extension for brief skeleton framework.

Changes per К10 forward planning:
- §3.3 lifecycle states 8 → 9 (AUTHORED-SKELETON added between AUTHORED and DEPRECATED)
- §3.3 prose paragraph explaining distinction from AUTHORED
- §3.3.1 transition matrix +4 transitions (Draft → AUTHORED-SKELETON, AUTHORED-SKELETON → AUTHORED, AUTHORED-SKELETON → DEPRECATED, AUTHORED-SKELETON → SUPERSEDED)
- §3.4 Category D row updated с new lifecycle value
- §3.4.1 forbidden combinations +4 entries (Tier 1/2/4/5 + AUTHORED-SKELETON forbidden)

Schema version 1.0 preserved (minor extension, no breaking enum removal). Tooling update в next commit.

Refs: DOC-A-FRAMEWORK, EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK"
```

### 2.5 Phase 1 outcome

1 atomic commit. FRAMEWORK.md v1.0 → v1.1. Proceed к Phase 2 tooling update.

---

## 3. Phase 2 — Tooling update: sync_register.ps1 + query_register.ps1

### 3.1 Scope

Update PowerShell tooling к accept new lifecycle value. Two scripts affected:

- `tools/governance/sync_register.ps1` — `$VALID_LIFECYCLES` array extension
- `tools/governance/query_register.ps1` — docstring parameter help text update

`tools/governance/render_register.ps1` — no change (renders whatever value present, no enum check).

### 3.2 sync_register.ps1 extension

Locate `$VALID_LIFECYCLES` array (verify line number in actual file during Phase 0; approximate line 115):

**Existing (verified via Phase 0 read)**:
```powershell
$VALID_LIFECYCLES = @('Draft','Live','LOCKED','EXECUTED','AUTHORED','DEPRECATED','SUPERSEDED','STALE')
```

**Updated**:
```powershell
$VALID_LIFECYCLES = @('Draft','Live','LOCKED','EXECUTED','AUTHORED','AUTHORED-SKELETON','DEPRECATED','SUPERSEDED','STALE')
```

### 3.3 sync_register.ps1 forbidden combinations extension

Locate forbidden combinations check (verify line number; approximate line 155):

**Existing (verified via Phase 0 read)**:
```powershell
$combo = "$tier+$lifecycle"
if ($combo -in @('1+AUTHORED','3+LOCKED','5+STALE','4+LOCKED')) {
    if (-not $doc.special_case_rationale) {
        [void]$errors.Add("$id : forbidden combination tier=$tier + lifecycle=$lifecycle without special_case_rationale")
    }
}
```

**Updated** (add Tier 1/2/4/5 + AUTHORED-SKELETON forbidden combos):

```powershell
$combo = "$tier+$lifecycle"
if ($combo -in @('1+AUTHORED','1+AUTHORED-SKELETON','2+AUTHORED-SKELETON','3+LOCKED','5+STALE','4+LOCKED','4+AUTHORED-SKELETON','5+AUTHORED-SKELETON')) {
    if (-not $doc.special_case_rationale) {
        [void]$errors.Add("$id : forbidden combination tier=$tier + lifecycle=$lifecycle without special_case_rationale")
    }
}
```

### 3.4 query_register.ps1 docstring update

Locate Lifecycle parameter help text (approximate line 12-13):

**Existing**:
```
.PARAMETER Lifecycle
    Filter by lifecycle state (Draft/Live/LOCKED/EXECUTED/AUTHORED/DEPRECATED/SUPERSEDED/STALE).
```

**Updated**:
```
.PARAMETER Lifecycle
    Filter by lifecycle state (Draft/Live/LOCKED/EXECUTED/AUTHORED/AUTHORED-SKELETON/DEPRECATED/SUPERSEDED/STALE).
```

### 3.5 Validation test (before commit)

After edits, run validate against existing register к confirm no regression:

```powershell
pwsh tools/governance/sync_register.ps1 -Validate
```

Expected: exit 0 (existing register has no AUTHORED-SKELETON entries yet; new enum value passes through validation without affecting existing entries).

**If non-zero exit**: halt SC-3 (tooling regression). Surface к Crystalka с full output.

### 3.6 Atomic commit 2: tooling update

```
git add tools/governance/sync_register.ps1 tools/governance/query_register.ps1
git commit -m "governance(tooling): add AUTHORED-SKELETON к valid lifecycles in sync_register + query_register

Tooling update per FRAMEWORK.md v1.1 schema extension (commit prior).

Changes:
- sync_register.ps1 \$VALID_LIFECYCLES array: 8 → 9 values (AUTHORED-SKELETON added)
- sync_register.ps1 forbidden combinations: +4 entries (Tier 1/2/4/5 + AUTHORED-SKELETON forbidden without rationale)
- query_register.ps1 docstring: Lifecycle parameter help text updated с new value

render_register.ps1 unchanged — no hardcoded enum check (renders whatever value present).

Validation run pre-commit: existing register unchanged behavior (no AUTHORED-SKELETON entries yet; new enum value passes through validation).

Refs: DOC-A-FRAMEWORK §3.3 v1.1, EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK"
```

### 3.7 Phase 2 outcome

1 atomic commit. Tooling forward-compatible с new lifecycle value. Proceed к Phase 3 skeleton authoring.

---

## 4. Phase 3 — Skeleton brief 1: К10 execution brief

### 4.1 Scope

Author skeleton at `tools/briefs/K10_EXECUTION_BRIEF.md`. Captures К10 execution plan structure pre-full-authoring.

К10 = 46-item kernel scheduler implementation. Substantial — single largest К-series milestone post-К-L3.1 era. Skeleton sets expectations for full brief authoring at Opus deliberation session timing (post-К8.5 closure per Phase A' sequencing).

### 4.2 Skeleton content к author

File: `tools/briefs/K10_EXECUTION_BRIEF.md`

```markdown
# К10 Native Kernel Scheduler — Execution Brief (SKELETON)

**Brief authored**: 2026-05-17 (skeleton; Claude Opus 4.7 К10 deliberation arc closure)
**Lifecycle**: AUTHORED-SKELETON — awaits full brief authoring at proper milestone timing (post-К8.5 closure per Phase A' sequencing)
**Target session**: Future Opus deliberation session (К10 execution brief authoring)
**Subsequent execution**: Claude Code execution mode for implementation
**Estimated full brief size**: 1500-2500 lines (substantial; К10 spec is 46 items + 12 К-L invariants + TLA+ scope)
**Parent**: К10 specification `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 LOCKED

---

## 0. Executive summary (skeleton)

Implement К10 specification: full native kernel scheduler, bus, control plane migration, V substrate interlock, TLA+ formal verification.

**Reference specification**: `docs/architecture/KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 (1875 lines), particularly:
- Part 3: 46-item К10 scope (Items 1-46 grouped by S lock origin)
- Part 4: К10 deliverables
- Part 5.1.A: 18 К10 architecture realization predictions (closure gate)
- Part 8: Risk register R-K10-1..R-K10-9

**К-L invariants targeted**: К-L7.1 (sub), К-L12 (full native kernel scheduling), К-L13 (on-demand activation), К-L14 (performance from architecture — *architecturally established* at this milestone), К-L15 (native bus three-tier), К-L16 (simulation tick pipeline depth), К-L17 (display composition multi-layer), К-L18 (mod lifecycle quiescent state), К-L19 (hardware tier commitment).

**К-L6 SUPERSEDED** rationale realized в this implementation.

---

## 1. Phase 0 — Preflight reads (skeleton)

Full brief к specify:
- К10 specification full read (Parts 0-11)
- KERNEL_ARCHITECTURE.md v1.6 (post-amendment) К-L table
- VULKAN_SUBSTRATE.md v1.0 LOCKED + V interlock sections (post-amendment)
- MOD_OS_ARCHITECTURE.md v1.8 (post-amendment) capability + lifecycle sections
- GameBootstrap.cs (production wiring ground truth per Lesson #22)
- ThreadPool.h, scheduler.cpp (native subsystem entry points)
- IGameServices.cs + SystemExecutionContext.cs (managed API boundary)
- TLA+ tooling availability (apalache, TLC) — capability check

---

## 2. Expected sub-milestones (skeleton)

К10 split into sub-milestones at brief authoring time. Skeleton ordering:

**Sub-milestone K10.A — Native scheduler primitives** (Items 1, 2, 3, 4, 5):
- Native dependency graph (replacing managed)
- Native thread pool extension
- Wake-up registry (5 wake types)
- Wake registry diagnostic API
- Dynamic per-tick graph computation

**Sub-milestone K10.B — Scheduling policies** (Items 6, 7, 8):
- Priority-based dispatch
- Resource quotas
- Preemption semantics

**Sub-milestone K10.C — Memory and IPC** (Items 9, 10, 11, 12, 13):
- Shared memory regions
- CPU affinity hints
- Work stealing
- Phase barrier semantics

**Sub-milestone K10.D — Native execution layer** (Items 14, 15, 16):
- Core systems migration к native (К11+ scope flag)
- Batched callback ABI
- К-L6 supersession + К-L12/13/14 amendment landing

**Sub-milestone K10.E — State and observability** (Items 17, 18, 19, 20):
- Write-through hook for NativeWorld (StateChange wake trigger)
- Formal verification — TLA+ spec authoring
- Observability hooks
- Scheduler intrinsics

**Sub-milestone K10.F — Mod integration** (Items 21, 22, 23):
- Mod scheduler authority (per-mod sub-schedulers)
- Hot-patching primitives
- Real-time scheduling guarantees

**Sub-milestone K10.G — Test infrastructure and migration** (Items 24, 25):
- Test infrastructure migration (catch2/gtest + ManagedTestScheduler fixture)
- К-closure report contributions

**Sub-milestone K10.H — S1 lock items (native bus)** (Items 26, 27, 28, 29, 30):
- Native bus implementation (three-tier dispatcher)
- Managed bus facade + C ABI bridge
- Event type registry (tier-annotated)
- Subscriber contract enforcement
- Background work queue + idle-slot scheduling

**Sub-milestone K10.I — S2 + S3 items** (Items 17 amended + 18 extended + 19 extended + 21 amended + 31, 32):
- Hybrid filter primitive + commit-time hook
- TLA+ spec extensions
- Filter metrics
- Per-mod sub-scheduler teardown encapsulated
- Background queue save-integrated storage
- Native unload primitive + ModUnloadResult

**Sub-milestone K10.J — S8 + S-TLA items** (Items 33-46):
- Pipeline depth mechanism
- Pipeline drain/refill protocols
- Phase.Compute scheduler integration
- Pipeline slot read API
- Filter primitive pipeline integration
- Display composition framework
- Intent overlay layer infrastructure
- Combat feedback layer infrastructure
- К-L18 quiescent state enforcement
- Settings menu / mod management UI integration
- Async compute queue mandate
- Hardware capability check
- TLA+ specification authoring (12 invariants)
- Safety verification CI gate
- Liveness verification targeted

Full brief к decide sub-milestone consolidation (precedent: К8.3+К8.4 combined into A'.5).

---

## 3. Halt conditions (skeleton)

Full brief к specify halt protocol. Anticipated halt classes:

- **HG-1**: Working tree dirty before execution
- **HG-2**: К10 specification version mismatch (post-amendment cascade may shift v2.0 → v2.x)
- **HG-3**: GameBootstrap.cs / production wiring divergence от К10 spec assumptions (Lesson #22 risk class)
- **SC-1**: Hardware capability check fails (RDNA 1+/Turing+/Arc+ requirement per К-L19)
- **SC-2**: TLA+ tooling unavailable (apalache/TLC) — К10 deliverable incomplete
- **SC-3**: Batched callback ABI overhead measurement violates Prediction 6 (80-95% reduction expected) — К-L14 falsification candidate
- **SC-4**: Pipeline depth D=2 produces simulation thread fence wait — К-L16 violation

---

## 4. Q-N seeds (skeleton)

From К10 spec Part 6, 49+ open Q-N candidates. Full brief к surface execution-time questions. Anticipated:

- Q-N-EXEC-1: К10.A native scheduler primitive ordering — replace managed scheduler atomically, или incremental с feature flag?
- Q-N-EXEC-2: TLA+ spec authoring parallelization с implementation — TLA+ authored по mere completion of corresponding К-L invariant code, или batched at end?
- Q-N-EXEC-3: К11+ Core systems migration flag — мark items K11-1..K11-5 as deferred-execution vs deferred-authoring?
- Q-N-EXEC-4: Hardware capability check failure UX — fail-fast at startup с diagnostic, or graceful degradation path?
- Q-N-EXEC-5: Background queue save format versioning — schema migration approach для save-game backward compatibility?

---

## 5. Closure protocol stub (skeleton)

Full brief к specify. Expected:
- sync_register.ps1 --validate exit 0 gate
- All 12 К-L safety verification CI gate runs clean
- 18 К10 architecture realization predictions measured + recorded
- К-closure report contributions authored (A'.8 prerequisite)
- New EVT entry — EVT-2026-XX-XX-K10-EXECUTION-CLOSURE
- К10 specification version v2.0 → v3.0 (post-execution version reflecting realized state)

---

## 6. Reference appendices (skeleton)

Full brief к include:
- A: К10 specification verbatim sections relevant к each sub-milestone
- B: TLA+ tooling reference + apalache/TLC usage patterns
- C: Async compute queue Vulkan API surface (vkGetPhysicalDeviceQueueFamilyProperties etc) — verbatim per Lesson #7
- D: К-Lxx invariant verbatim text для each invariant targeted
- E: Performance measurement protocol matching Predictions 1-18

---

**End of skeleton.**

**Promotion к AUTHORED** triggers: full brief authoring at Opus deliberation session post-К8.5 closure. Estimated session duration: ~4-8 hours deliberation (К10 substantial; multiple Q rounds expected). Skeleton к patch-brief mutation pattern if К10 spec amended pre-execution (precedent: K8_3_BRIEF_REFRESH_PATCH).
```

### 4.3 Atomic commit 3: К10 execution skeleton

```
git add tools/briefs/K10_EXECUTION_BRIEF.md
git commit -m "docs(briefs): К10 execution brief skeleton authored

Skeleton brief for К10 native kernel scheduler execution. Lifecycle: AUTHORED-SKELETON per new schema (FRAMEWORK §3.3).

Captures:
- Reference к К10 specification v2.0 LOCKED (46 items, 12 К-L invariants)
- 10 expected sub-milestones (K10.A through K10.J) covering full К10 scope
- 4 anticipated halt classes (HG-1/2/3, SC-1..4)
- 5 Q-N execution-time seed questions
- Closure protocol stub

Promotion к AUTHORED gates на full brief authoring at Opus deliberation session post-К8.5 closure.

Refs: DOC-D-K10_EXECUTION (enrollment Phase 7), EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK"
```

### 4.4 Phase 3 outcome

1 atomic commit. К10 execution skeleton authored. Proceed к Phase 4.

---

## 5. Phase 4 — Skeleton brief 2: К10 cross-document amendments cascade

### 5.1 Scope

Author skeleton at `tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md`.

К10 amendment landed in main spec document (KERNEL_FULL_NATIVE_SCHEDULER.md v2.0). Cross-document amendments queue captured в spec Part 7 — 9 documents need propagation. This skeleton tracks that cascade.

### 5.2 Skeleton content к author

File: `tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md`

```markdown
# К10 Cross-Document Amendments Cascade — Execution Brief (SKELETON)

**Brief authored**: 2026-05-17 (skeleton; К10 amendments closure cascade)
**Lifecycle**: AUTHORED-SKELETON
**Target session**: Claude Code execution mode (mechanical cascade application)
**Estimated full brief size**: 800-1500 lines
**Parent**: К10 specification `KERNEL_FULL_NATIVE_SCHEDULER.md` v2.0 Part 7 (cross-document amendments massive extension)

---

## 0. Executive summary (skeleton)

Propagate К10 architectural decisions across 8 dependent documents. К10 specification authority established (v2.0 LOCKED); downstream documents need amendments к reflect К-L invariants, Item references, capability tokens, hardware requirements.

**Documents queue** (per KERNEL_FULL_NATIVE_SCHEDULER.md Part 7):

1. KERNEL_ARCHITECTURE.md (DOC-A-KERNEL) — К-L invariant table extension (К-L7.1 sub + К-L12-К-L19 + К-L6 SUPERSEDED note)
2. VULKAN_SUBSTRATE.md (DOC-A-VULKAN_SUBSTRATE) — substantial amendment §0/§2/§3.4/§4/§5.5/§7.2/§7.3 (К-L7.1, К-L16, К-L17, К-L19 integration)
3. MOD_OS_ARCHITECTURE.md (DOC-A-MOD_OS) — §3.2 capability section (tier-prefixed tokens, V resource tokens, К-L17 layer tokens), §4 IModApi layer registration, §9.5 unload chain, §11 hot reload К-L18 compliance
4. DualFrontier.Persistence (source module) — save/load integration для background queue (S3), pipeline slot snapshot serialization (К-L16)
5. KernelCapabilityRegistry.cs (source) — tier-prefixed token generation, К-L17 layer tokens, V resource capability tokens
6. PHASE_A_PRIME_SEQUENCING.md (DOC-A-PHASE_A_PRIME_SEQUENCING) — §2 sequencing wording cleanup (S5 lock), Q-K-1 retroactive lock resolution
7. README.md (DOC-G-README) — hardware requirements section (К-L19 tier commitment)
8. К-closure report (А'.8 deferred — pending creation; cross-references к К10 contributions)

---

## 1. Phase 0 — Preflight reads (skeleton)

Full brief к specify. Anticipated:
- KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED Part 7 cross-document amendments section
- Each target document current state (8 documents listed)
- К10 specification К-L invariant verbatim texts (К-L7.1, К-L15-19)
- REGISTER.yaml entries for affected documents (versions, paths, current lifecycles)

---

## 2. Expected commit structure (skeleton)

One atomic commit per document amendment (Lesson #8 — atomic compilable unit). Sequencing:

1. Commit 1: KERNEL_ARCHITECTURE.md — К-L table extension
2. Commit 2: VULKAN_SUBSTRATE.md — substantial multi-section amendment
3. Commit 3: MOD_OS_ARCHITECTURE.md — capability + lifecycle amendments
4. Commit 4: PHASE_A_PRIME_SEQUENCING.md — wording cleanup
5. Commit 5: README.md — hardware requirements section
6. Commit 6: KernelCapabilityRegistry.cs — code changes для capability scanning extensions
7. Commit 7: DualFrontier.Persistence — save/load integration (Items 31)
8. Commit 8: REGISTER.yaml version bumps + EVT entry + closure protocol
9. Commit 9: K-closure report scoping (А'.8 prerequisite — может be deferred к dedicated K-closure brief)

Total: 8-9 commits expected.

**Lesson #11 application**: cross-doc cascade may be combined с К10 execution per «atomic commit as compilable unit» — if К10 execution commit lands K-L code changes, related doc amendments may bundle. К8.3+К8.4 combined precedent. Full brief к decide at authoring time.

---

## 3. Halt conditions (skeleton)

- HG-1: Working tree dirty
- HG-2: К10 specification version drifted post-skeleton authoring (re-verify spec content)
- HG-3: Cross-doc amendment surfaces contradiction between К10 spec и existing target document — escalate (precedent: K8.3 v2.0 premise miss)
- SC-1: Capability tokens shape contradicts MOD_OS_ARCHITECTURE current capability section pattern — escalate
- SC-2: К-L19 hardware tier wording в README disconnects от user expectations (UX concern) — surface к Crystalka

---

## 4. Q-N seeds (skeleton)

- Q-N-XDOC-1: К-closure report (А'.8) scoping в this brief or separate brief?
- Q-N-XDOC-2: Code changes (KernelCapabilityRegistry, DualFrontier.Persistence) bundled с doc amendments или separated к К10 execution?
- Q-N-XDOC-3: README hardware requirements wording — technical (Vulkan 1.3 + async compute) или user-facing (GPU model list)?
- Q-N-XDOC-4: VULKAN_SUBSTRATE amendment scope is substantial — split into VULKAN_SUBSTRATE multi-section + V0 specification amendment если V0 brief existed (it doesn't currently — V substrate is single doc).

---

## 5. Closure protocol stub (skeleton)

Full brief к specify. Anticipated:
- sync_register.ps1 --validate exit 0 gate
- All 8 documents versions bumped
- REGISTER.yaml audit_trail entry (EVT-2026-XX-XX-K10-CROSS-DOC-CASCADE)
- К10 specification cross-document amendments queue marked CLOSED в Part 7
- Phase A' sequencing reflects cascade completion

---

**End of skeleton.**

**Promotion к AUTHORED** triggers: К10 execution closure (А'.7) или earlier if Crystalka prioritizes cascade pre-execution. Skeleton к patch-brief mutation pattern if К10 spec amended further pre-cascade.
```

### 5.3 Atomic commit 4: К10 cross-doc cascade skeleton

```
git add tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md
git commit -m "docs(briefs): К10 cross-document amendments cascade skeleton authored

Skeleton brief for К10 architectural propagation across 8 dependent documents. Lifecycle: AUTHORED-SKELETON.

Captures:
- 8-document queue from KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 Part 7
- 8-9 expected atomic commits (one per document amendment)
- 3 anticipated halt classes (HG-1/2/3) + 2 soft conditions
- 4 Q-N execution-time seed questions
- Closure protocol stub

Promotion к AUTHORED gates на full brief authoring (post-К10 execution or earlier per Crystalka prioritization).

Refs: DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE (enrollment Phase 7), EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK"
```

### 5.4 Phase 4 outcome

1 atomic commit. К10 cross-doc cascade skeleton authored. Proceed к Phase 5.

---

## 6. Phase 5 — Skeleton brief 3: А'.8 K-closure report

### 6.1 Scope

Author skeleton at `tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md`.

А'.8 K-closure report = К-series formal closure. Substantial scope (~3-6 weeks per session memory). Includes provisional Lessons promotion review, К-L14 evidence state framing, Roslyn analyzer rule specification preparation.

### 6.2 Skeleton content к author

File: `tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md`

```markdown
# А'.8 K-Series Closure Report — Execution Brief (SKELETON)

**Brief authored**: 2026-05-17 (skeleton; post-К10 forward planning)
**Lifecycle**: AUTHORED-SKELETON
**Target session**: Future Opus deliberation session (substantial К-closure report authoring)
**Subsequent execution**: Claude Code execution mode for mechanical landing
**Estimated full brief size**: 1200-2000 lines (substantial — К-closure report itself + brief specifying authoring)
**Parent**: К-series complete arc through К10 execution closure

---

## 0. Executive summary (skeleton)

Author К-series formal closure report at `docs/reports/K_CLOSURE_REPORT.md`. Document captures:

- **К-series complete arc narrative**: K0 (2026-05-07) → К10 (А'.7 execution date) full chronology
- **К-Lxx invariant series final state**: 20 invariants (К-L1..L19 + К-L3.1 sub + К-L7.1 sub, К-L6 SUPERSEDED)
- **Empirical results**: К10 performance predictions §5.1.A measurement results (18 predictions), К-L14 evidence state (architecturally established at К10; measurably pending К11+)
- **Lessons formalized**: review provisional pool (9 candidates: #9, #10, #14, #15, #16, #17, #18, #19, #21) с promotion decisions per accumulated evidence
- **Roslyn analyzer rule specification**: К-Lxx invariant encoding plan для А'.9 — dual purpose (closure record + analyzer input)
- **К-series → M-series transition**: handoff document for Phase B M-series

**К-L14 evidence framing** (load-bearing для closure narrative):
- *Architecturally established* at К10 closure (clean architectural surface, principled invariants)
- *Measurably confirmed-or-falsified* deferred к К11+ (Core systems native migration measurements)

---

## 1. Phase 0 — Preflight reads (skeleton)

Full brief к specify. Anticipated:

- All К-series briefs (K0-K10 + sub-briefs): full read for chronology narrative authoring
- KERNEL_ARCHITECTURE.md final state (post-cross-doc cascade): К-L table авторitative reference
- KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 + К10 execution closure version (v3.0 если bumped): performance predictions actual vs predicted
- All К-series closure reports (K7 performance, K8.1-K9 lessons, A'.5 cutover): empirical evidence aggregation
- METHODOLOGY.md v1.8: Lesson formalization model + provisional pool current state
- PIPELINE_METRICS.md: К-series pipeline performance data
- All К-series CAPAs: closure analysis (5 CAPAs closed across К-series: K8.2-V2-REFRAMING, K8.3-PREMISE-MISS, K8.34-API-SURFACE-MISS, K8.34-MID-TRANSITION-DRIFT, K10-related если any)
- К10 deliberation state (K10_DELIBERATION_STATE.md Project file) — К-L14 falsifiable claim evidence assembly
- COMPOSITE_NAMESPACE_DELIBERATION_STATE.md — context для М-series transition framing

---

## 2. Expected report structure (skeleton)

К-closure report `docs/reports/K_CLOSURE_REPORT.md` к contain:

**Part 0: Abstract** — К-series complete; 20 К-L invariants final; falsifiable claims (К-L14) committed

**Part 1: Chronology**
- 1.1 K0 (2026-05-07) → K9 (2026-05-11) initial К-series
- 1.2 A'.4.5 governance interlude (2026-05-12)
- 1.3 K8.3+K8.4 combined v2.0 cutover (2026-05-14)
- 1.4 K8.5 + К10 (post-2026-05-17)
- 1.5 К-closure timeline summary

**Part 2: К-Lxx invariant series**
- 2.1 К-L1..L11 (pre-К10) — original invariants table + history
- 2.2 К-L12..L19 (К10 additions) — full text + rationale
- 2.3 К-L6 SUPERSEDED — supersession rationale
- 2.4 Sub-invariants К-L3.1, К-L7.1 — rationale for sub-invariant pattern

**Part 3: Empirical results**
- 3.1 К10 architecture realization predictions (Predictions 1-18) — measurement results
- 3.2 К11+ performance predictions (Predictions K11-1..K11-5) — deferred к К11+ post-К-series
- 3.3 К-L14 evidence state — architectural confirmation; measurement pending

**Part 4: Lessons formalized**
- 4.1 Provisional pool review (9 candidates):
  - #9 Survey phase before brief authoring — promote / hold provisional / merge
  - #10 Architecture audit + tech debt inventory in one pass — promote / hold / merge
  - #14 Pre-existing drift cleanup respect deferrals — promote / hold / merge
  - #15 Emotional projection avoidance — promote / hold / merge
  - #16 Brief length correlates с deliberation complexity — promote / hold / merge
  - #17 Performance reasoning tactical vs strategic — promote / hold / merge (may merge с #20)
  - #18 Boundary crossing batching pattern — promote / hold / merge
  - #19 On-demand activation pattern — promote / hold / merge
  - #21 Redundancy check before default-inclusion — promote / hold / merge
- 4.2 К-series-specific lessons surfaced — preserve as К-closure record
- 4.3 METHODOLOGY.md v1.8 → v1.9 amendments

**Part 5: Pipeline metrics**
- 5.1 К-series pipeline performance — sessions per closure, halt count per milestone, CAPA volume
- 5.2 К-L14 *meta* claim (clean complex architecture → performance) — pipeline-level evidence

**Part 6: Roslyn analyzer rule specification**
- 6.1 Each К-Lxx invariant → Roslyn rule mapping
- 6.2 First-run analyzer milestone scope (А'.9)
- 6.3 Expected fix budget within А'.9 (К-Lxx violations surfaced post-execution)

**Part 7: К → M transition**
- 7.1 М-series readiness criteria
- 7.2 М-K demonstration buckets (composite namespace per Q-G-1/Q-G-2 LOCK)
- 7.3 М-V demonstrations (V substrate primitives V0/V1/V2 ready post-cross-doc cascade)
- 7.4 Phase B M-series authority handoff

**Part 8: Open work post-К-closure**
- 8.1 К11+ Core systems native migration (Predictions K11-1..K11-5 measurement gate)
- 8.2 К-extensions (если any surface)
- 8.3 М-series first sprint scoping (М-K mod selection)

---

## 3. Halt conditions (skeleton)

- HG-1: Working tree dirty
- HG-2: К10 execution incomplete (predictions not measured) — К-closure cannot proceed без empirical evidence
- HG-3: Provisional pool review surfaces lesson that contradicts existing formal Lesson — escalate
- SC-1: К-L14 measurement falsifies prediction (performance не tracks architectural cleanliness) — research framework crisis level event; surface к Crystalka

---

## 4. Q-N seeds (skeleton)

- Q-N-KCL-1: Provisional pool promotion criteria — three strong applications established, или review based on application quality?
- Q-N-KCL-2: К-closure report scope — pure К-series narrative, или include Roslyn analyzer rule spec inline (А'.8 + А'.9 partial overlap)?
- Q-N-KCL-3: М-series transition framing — К-closure report authoritative для М-series scope, или handoff к dedicated М-series scoping brief?
- Q-N-KCL-4: К-L14 falsifiability evidence — how is «measurably confirmed-or-falsified» operationally defined? Quantitative threshold?
- Q-N-KCL-5: К-extensions vs М-series prioritization — после К-closure, К11+ или М-K first?

---

## 5. Closure protocol stub (skeleton)

Full brief к specify. Anticipated:
- sync_register.ps1 --validate exit 0 gate
- К-series briefs all lifecycle EXECUTED (verification gate)
- К-closure report authored at `docs/reports/K_CLOSURE_REPORT.md` Tier 2 Live
- METHODOLOGY.md v1.8 → v1.9 (provisional pool promotions landed)
- New EVT — EVT-2026-XX-XX-K-CLOSURE-REPORT
- А'.8 milestone marked complete в PHASE_A_PRIME_SEQUENCING.md
- Roslyn analyzer rule specification handoff к А'.9

---

**End of skeleton.**

**Promotion к AUTHORED** triggers: К10 execution closure (А'.7). Skeleton к patch-brief mutation pattern if К-series scope changes (e.g., К-extensions surface).
```

### 6.3 Atomic commit 5: А'.8 K-closure report skeleton

```
git add tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md
git commit -m "docs(briefs): А'.8 K-series closure report skeleton authored

Skeleton brief for К-series formal closure report. Lifecycle: AUTHORED-SKELETON.

Captures:
- Substantial scope (~3-6 weeks per session memory)
- 8-part К-closure report structure (chronology, К-L invariants, empirical results, lessons, pipeline metrics, Roslyn analyzer prep, K→M transition, open work)
- Provisional pool review framework (9 candidates: #9, #10, #14, #15, #16, #17, #18, #19, #21)
- К-L14 evidence framing (architecturally established at К10; measurably pending К11+)
- 4 anticipated halt classes
- 5 Q-N execution-time seed questions

Promotion к AUTHORED gates на К10 execution closure (А'.7). К-closure substantial — separate Opus deliberation session expected для full brief authoring.

Refs: DOC-D-A_PRIME_8_K_CLOSURE_REPORT (enrollment Phase 7), EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK"
```

### 6.4 Phase 5 outcome

1 atomic commit. А'.8 K-closure report skeleton authored. Proceed к Phase 6.

---

## 7. Phase 6 — Skeleton brief 4: А'.9 Roslyn analyzer

### 7.1 Scope

Author skeleton at `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md`.

А'.9 = Roslyn analyzer milestone. Post-К-closure (per Phase A' sequencing). Dual purpose: (1) active M-series migration verifier; (2) debugger for bugs that compile + pass tests but violate К-Lxx invariants. First run expected к surface pre-existing debt — fix budget внутри А'.9 scope.

### 7.2 Skeleton content к author

File: `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md`

```markdown
# А'.9 Roslyn Architectural Analyzer — Execution Brief (SKELETON)

**Brief authored**: 2026-05-17 (skeleton; post-К-closure forward planning)
**Lifecycle**: AUTHORED-SKELETON
**Target session**: Future Opus deliberation session (Roslyn analyzer brief authoring)
**Subsequent execution**: Claude Code execution mode для implementation
**Estimated full brief size**: 1000-1500 lines
**Parent**: К-closure report (А'.8) Roslyn analyzer rule specification section (Part 6)

---

## 0. Executive summary (skeleton)

Implement Roslyn architectural analyzer encoding К-Lxx invariants as compile-time rules.

**Dual purpose**:

1. **Active M-series migration verifier** — catches drift в file movements, namespace changes, ModApi registration patterns that test suite doesn't catch
2. **К-Lxx invariant compile-time enforcement** — bugs that compile + pass tests but violate К-Lxx invariants surface at build time

**First run expected к surface pre-existing debt** — fix budget внутри А'.9 scope. Linux `-Werror` adoption precedent: massive cleanup campaign precedes flag enablement. Analyzer first run = cleanup phase.

**К-Lxx invariants targeted** (20 invariants final post-К-closure):

К-L1 (C++20 native language), К-L2 (Pure P/Invoke), К-L3 + К-L3.1 (component storage paths), К-L4 (explicit registry), К-L5 (declarative bootstrap graph), К-L6 SUPERSEDED, К-L7 + К-L7.1 (span protocol), К-L8 (component lifetime), К-L9 (mod parity), К-L10 (decision rule §8 metrics), К-L11 (NativeWorld single source of truth), К-L12 (full native kernel scheduling), К-L13 (on-demand activation), К-L14 (performance from architecture — *meta-invariant*; not directly encodable), К-L15 (native bus three-tier), К-L16 (pipeline depth), К-L17 (display composition), К-L18 (mod lifecycle quiescent state), К-L19 (hardware tier).

Note: К-L14 is meta-invariant (causal claim about architecture → performance); not encodable as Roslyn rule. К-L6 SUPERSEDED; not analyzed. Remaining 18 invariants targeted.

---

## 1. Phase 0 — Preflight reads (skeleton)

Full brief к specify. Anticipated:

- К-closure report (А'.8 output) Part 6 — Roslyn analyzer rule specification: authoritative input
- All 20 К-Lxx invariants verbatim text — KERNEL_ARCHITECTURE.md final state
- Roslyn analyzer SDK documentation — capability check
- DiagnosticAnalyzer + CodeFixProvider API surface — implementation pattern
- Existing project analyzer configuration (если any) — `.editorconfig`, `Directory.Build.props`
- Test fixtures for К-Lxx invariants — analyzer test corpus

---

## 2. Expected sub-milestones (skeleton)

**Sub-milestone A9.A — Analyzer scaffolding**:
- New project: `tools/analyzers/DualFrontier.Analyzers/`
- DiagnosticAnalyzer base class
- Build integration (Directory.Build.props references)
- Test project: `tools/analyzers/DualFrontier.Analyzers.Tests/`

**Sub-milestone A9.B — Per-invariant rule authoring** (per К-Lxx, ~18 rules):
- DF001: К-L1 — C++20 native language (CMake CXX_STANDARD check; meta-rule, build-time)
- DF002: К-L2 — Pure P/Invoke (no SWIG/C++/CLI references)
- DF003: К-L3 + К-L3.1 — Component storage path declarations
- DF004: К-L4 — Component types registered via ComponentTypeRegistry
- DF005: К-L5 — Bootstrap graph declarative
- DF007: К-L7 — Span protocol (read-only spans + WriteBatch)
- DF007.1: К-L7.1 — GPU compute pipeline slot binding
- DF008: К-L8 — Component lifetime native ownership
- DF009: К-L9 — Vanilla = mods (IModApi registration uniformity)
- DF010: К-L10 — Decision rule metrics adherence
- DF011: К-L11 — NativeWorld single source of truth (no ManagedTestWorld production references)
- DF012: К-L12 — Full native kernel scheduling (no managed scheduler production references)
- DF013: К-L13 — On-demand activation (wake-up registration patterns)
- DF015: К-L15 — Native bus three-tier (tier declarations per event type)
- DF016: К-L16 — Pipeline depth (sim/display thread separation)
- DF017: К-L17 — Display composition multi-layer
- DF018: К-L18 — Mod lifecycle quiescent state
- DF019: К-L19 — Hardware tier (Vulkan 1.3 + async compute capability checks)

Approximate 17-18 rules.

**Sub-milestone A9.C — First run + pre-existing debt fix**:
- Analyzer enabled с warnings (not errors initially)
- First run on current codebase — surfaces violations
- Fix budget within А'.9 scope (cleanup phase, similar к cleanup cascade 2026-05-16)
- Each violation surfaced → either (a) fixed in code, (b) suppressed с rationale [#pragma warning disable + comment], (c) rule false-positive → rule refinement

**Sub-milestone A9.D — Warning → Error promotion**:
- After cleanup phase, analyzer rules promoted from warning к error severity
- Build fails on К-Lxx violation
- CodeFixProviders authored where mechanical fix possible

**Sub-milestone A9.E — Test infrastructure**:
- Per-rule test suite (one positive + one negative case minimum)
- Analyzer test fixture (Microsoft.CodeAnalysis.Testing pattern)

---

## 3. Halt conditions (skeleton)

- HG-1: Working tree dirty
- HG-2: К-closure report (А'.8) not landed — analyzer rule spec input missing
- HG-3: К-Lxx invariant interpretation ambiguous — analyzer rule under-specified, surface к Crystalka
- SC-1: Pre-existing debt fix budget exceeded — surface к Crystalka для scope decision (А'.9 vs deferred к follow-up milestone)
- SC-2: Roslyn analyzer SDK limitations encountered (e.g., К-Lxx invariant not encodable as static analysis) — surface к Crystalka

---

## 4. Q-N seeds (skeleton)

- Q-N-A9-1: Per-К-Lxx rule design — strict invariant encoding или heuristic detection? Some К-Lxx (К-L11 «NativeWorld single source of truth») are architectural; analyzer can catch some violations not all
- Q-N-A9-2: К-L14 (meta-invariant) — encoding strategy or explicit «not encoded» rationale в analyzer documentation?
- Q-N-A9-3: Severity ramp — initial warning + cleanup + promote к error, или from-day-one error?
- Q-N-A9-4: Test suite coverage — exhaustive (every К-Lxx +/- case) or representative?
- Q-N-A9-5: CodeFixProvider scope — provide automated fixes where possible, or analyzer-only? Mechanical fixes risk obscuring violations
- Q-N-A9-6: Fix budget — hard cap (e.g., 100 violations) or open-ended within А'.9 scope?

---

## 5. Closure protocol stub (skeleton)

Full brief к specify. Anticipated:
- sync_register.ps1 --validate exit 0 gate
- All 17-18 analyzer rules authored + tested
- First-run cleanup complete (build clean post-cleanup phase)
- Analyzer enabled в CI (gating build)
- К-Lxx invariants encoded as enforcement (К-Lxx → DFNNN rule mapping documented)
- METHODOLOGY.md mention of analyzer enforcement layer
- New EVT — EVT-2026-XX-XX-A_PRIME_9-ROSLYN-ANALYZER
- А'.9 milestone marked complete; Phase B M-series gate unblocks

---

## 6. Reference appendices (skeleton)

Full brief к include:
- A: Each К-Lxx invariant verbatim + analyzer rule mapping rationale
- B: Roslyn analyzer SDK reference patterns (verbatim API surface per Lesson #7)
- C: Microsoft.CodeAnalysis.Testing fixture patterns
- D: First-run cleanup precedent (А'.5 cleanup cascade pattern application)

---

**End of skeleton.**

**Promotion к AUTHORED** triggers: К-closure report (А'.8) landed с Roslyn analyzer rule specification section authoritative. Skeleton к patch-brief mutation if К-closure report surfaces additional rules.
```

### 7.3 Atomic commit 6: А'.9 Roslyn analyzer skeleton

```
git add tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md
git commit -m "docs(briefs): А'.9 Roslyn architectural analyzer skeleton authored

Skeleton brief for Roslyn analyzer encoding К-Lxx invariants. Lifecycle: AUTHORED-SKELETON.

Captures:
- Dual purpose (M-series migration verifier + К-Lxx invariant compile-time enforcement)
- 17-18 expected analyzer rules (К-L1..L19 minus К-L6 SUPERSEDED minus К-L14 meta-invariant)
- 5 expected sub-milestones (A9.A scaffolding, A9.B per-invariant rules, A9.C first-run cleanup, A9.D warning→error promotion, A9.E test infrastructure)
- First-run pre-existing debt fix budget within А'.9 scope (Linux -Werror precedent)
- 5 anticipated halt classes
- 6 Q-N execution-time seed questions

Promotion к AUTHORED gates на К-closure report (А'.8) landed с analyzer rule spec section authoritative.

Refs: DOC-D-A_PRIME_9_ROSLYN_ANALYZER (enrollment Phase 7), EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK"
```

### 7.4 Phase 6 outcome

1 atomic commit. А'.9 Roslyn analyzer skeleton authored. Proceed к Phase 7 enrollment.

---

## 8. Phase 7 — REGISTER.yaml enrollment: 4 new DOC-D entries

### 8.1 Scope

Enroll all 4 skeleton briefs в REGISTER.yaml. Add 4 DOC-D entries with `lifecycle: AUTHORED-SKELETON`.

### 8.2 Location в REGISTER.yaml

Skeleton entries belong в Tier 3 Briefs (Category D) section. Locate existing entries (К-series briefs). Insert new entries после К-series но before А'-cycle briefs (logical: К10 belongs к К-series, А' briefs belong к А'-cycle).

Actually, looking at existing pattern:
- К-series: DOC-D-K0..K9, K8.x, K_L3_1, K_LESSONS_BATCH
- А'-cycle: DOC-D-A_PRIME_0_5, A_PRIME_0_7, A_PRIME_1, A_PRIME_4_5_*

К10 execution brief belongs к К-series cluster.
К10 cross-doc cascade brief — belongs к К-series cluster (К10 derivative).
А'.8 K-closure — belongs к А'-cycle cluster.
А'.9 analyzer — belongs к А'-cycle cluster.

Insert ordering:
1. DOC-D-K10_EXECUTION — после DOC-D-K_LESSONS_BATCH (end of К-series)
2. DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE — после DOC-D-K10_EXECUTION
3. DOC-D-A_PRIME_8_K_CLOSURE_REPORT — после DOC-D-A_PRIME_4_5_PASS_5 (end of А'-cycle)
4. DOC-D-A_PRIME_9_ROSLYN_ANALYZER — после DOC-D-A_PRIME_8_K_CLOSURE_REPORT

### 8.3 Entry text

#### 8.3.1 DOC-D-K10_EXECUTION

```yaml
  - id: DOC-D-K10_EXECUTION
    path: tools/briefs/K10_EXECUTION_BRIEF.md
    title: "К10 Native Kernel Scheduler — Execution (SKELETON)"
    category: D
    tier: 3
    lifecycle: AUTHORED-SKELETON
    owner: Crystalka
    version: "0.1"
    last_modified: "2026-05-17"
    last_modified_commit: "PENDING-COMMIT-3"
    content_language: en
    review_cadence: on-status-transition
    last_review_date: "2026-05-17"
    last_review_event: "Skeleton authored per К10 forward planning skeleton framework brief"
    reviewer: Crystalka
    risks_referenced: [RISK-003, RISK-004, RISK-013]
    special_case_rationale: "Skeleton brief awaiting full brief authoring at Opus deliberation session post-К8.5 closure. Captures 10 expected sub-milestones (K10.A through K10.J), 4 halt classes, 5 Q-N seeds. К10 specification authority at DOC-A-KERNEL_FULL_NATIVE_SCHEDULER v2.0 LOCKED."
```

#### 8.3.2 DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE

```yaml
  - id: DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE
    path: tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md
    title: "К10 Cross-Document Amendments Cascade (SKELETON)"
    category: D
    tier: 3
    lifecycle: AUTHORED-SKELETON
    owner: Crystalka
    version: "0.1"
    last_modified: "2026-05-17"
    last_modified_commit: "PENDING-COMMIT-4"
    content_language: en
    review_cadence: on-status-transition
    last_review_date: "2026-05-17"
    last_review_event: "Skeleton authored per К10 forward planning skeleton framework brief"
    reviewer: Crystalka
    risks_referenced: [RISK-004]
    special_case_rationale: "Skeleton brief for К10 architectural propagation across 8 dependent documents per KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 Part 7. Awaits full brief authoring at К10 execution closure (А'.7) timing or earlier per Crystalka prioritization."
```

#### 8.3.3 DOC-D-A_PRIME_8_K_CLOSURE_REPORT

```yaml
  - id: DOC-D-A_PRIME_8_K_CLOSURE_REPORT
    path: tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md
    title: "А'.8 K-Series Closure Report (SKELETON)"
    category: D
    tier: 3
    lifecycle: AUTHORED-SKELETON
    owner: Crystalka
    version: "0.1"
    last_modified: "2026-05-17"
    last_modified_commit: "PENDING-COMMIT-5"
    content_language: en
    review_cadence: on-status-transition
    last_review_date: "2026-05-17"
    last_review_event: "Skeleton authored per К10 forward planning skeleton framework brief"
    reviewer: Crystalka
    risks_referenced: [RISK-014]
    capa_entries_referenced: []
    special_case_rationale: "Skeleton brief for К-series formal closure report. Substantial scope (~3-6 weeks). 8-part structure (chronology, К-L invariants, empirical results, lessons, pipeline metrics, Roslyn analyzer prep, K→M transition, open work). Awaits full brief authoring at К10 execution closure (А'.7) timing."
```

#### 8.3.4 DOC-D-A_PRIME_9_ROSLYN_ANALYZER

```yaml
  - id: DOC-D-A_PRIME_9_ROSLYN_ANALYZER
    path: tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md
    title: "А'.9 Roslyn Architectural Analyzer (SKELETON)"
    category: D
    tier: 3
    lifecycle: AUTHORED-SKELETON
    owner: Crystalka
    version: "0.1"
    last_modified: "2026-05-17"
    last_modified_commit: "PENDING-COMMIT-6"
    content_language: en
    review_cadence: on-status-transition
    last_review_date: "2026-05-17"
    last_review_event: "Skeleton authored per К10 forward planning skeleton framework brief"
    reviewer: Crystalka
    risks_referenced: [RISK-004]
    special_case_rationale: "Skeleton brief for Roslyn architectural analyzer encoding К-Lxx invariants. Dual purpose (M-series migration verifier + К-Lxx compile-time enforcement). 17-18 expected analyzer rules. Awaits full brief authoring at К-closure report (А'.8) landed timing."
```

### 8.4 register_version bump + DOC-A-FRAMEWORK update

REGISTER.yaml schema metadata top:

```yaml
schema_version: "1.0"
register_version: "1.5"  # from prior К10 closure
last_modified: "2026-05-17"
last_modified_commit: "PENDING-COMMIT-7"  # backfilled Phase 8
last_modified_by: "Claude Code"
```

Update к:

```yaml
schema_version: "1.0"
register_version: "1.6"
last_modified: "2026-05-17"
last_modified_commit: "PENDING-COMMIT-7"
last_modified_by: "Claude Code"
```

Update DOC-A-FRAMEWORK entry:

```yaml
  - id: DOC-A-FRAMEWORK
    path: docs/governance/FRAMEWORK.md
    [...]
    version: "1.1"  # was 1.0
    last_modified: "2026-05-17"  # was 2026-05-12
    last_modified_commit: "PENDING-COMMIT-1"  # backfilled Phase 8
    last_review_date: "2026-05-17"
    last_review_event: "Schema extension AUTHORED-SKELETON lifecycle value per К10 forward planning skeleton framework"
    next_review_due: "2027-05-17"
```

### 8.5 Atomic commit 7: REGISTER enrollment

```
git add docs/governance/REGISTER.yaml
git commit -m "governance(register): enroll 4 skeleton briefs + bump DOC-A-FRAMEWORK к v1.1

REGISTER.yaml register_version 1.5 → 1.6 per К10 forward planning skeleton framework.

Changes:
- DOC-D-K10_EXECUTION (NEW): Tier 3 Category D, AUTHORED-SKELETON, v0.1 — К10 execution
- DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE (NEW): Tier 3 Category D, AUTHORED-SKELETON, v0.1 — К10 cross-doc cascade
- DOC-D-A_PRIME_8_K_CLOSURE_REPORT (NEW): Tier 3 Category D, AUTHORED-SKELETON, v0.1 — А'.8 K-closure
- DOC-D-A_PRIME_9_ROSLYN_ANALYZER (NEW): Tier 3 Category D, AUTHORED-SKELETON, v0.1 — А'.9 Roslyn analyzer
- DOC-A-FRAMEWORK (UPDATE): version 1.0 → 1.1, last_modified bump

PENDING-COMMIT placeholders filled in Phase 8 closure commit per FRAMEWORK §8.3 backfilling protocol.

Refs: EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK (added Phase 8)"
```

### 8.6 Phase 7 outcome

1 atomic commit. 4 new skeleton entries + DOC-A-FRAMEWORK bump + register_version 1.5 → 1.6.

---

## 9. Phase 8 — Closure protocol

### 9.1 Scope

Final closure operations matching К10 amendments application closure pattern:

1. Run sync_register.ps1 --validate (must exit 0)
2. Run render_register.ps1 (regenerate REGISTER_RENDER.md)
3. Backfill PENDING-COMMIT hashes (FRAMEWORK §8.3)
4. Add EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK audit trail entry
5. Final atomic commit с closure

### 9.2 Validation run

```powershell
pwsh tools/governance/sync_register.ps1 -Validate
```

Expected exit code: 0 (pass). New AUTHORED-SKELETON entries pass validation per Tier 3 + AUTHORED-SKELETON allowed combination (not в forbidden list).

**If non-zero exit**: halt HG-2. Common causes:
- AUTHORED-SKELETON not в `$VALID_LIFECYCLES` (verify Phase 2 commit landed)
- Forbidden combination misconfigured (Tier 3 + AUTHORED-SKELETON should be allowed)
- Missing path (verify all 4 skeleton brief files exist)

### 9.3 Frontmatter sync

```powershell
pwsh tools/governance/sync_register.ps1
```

Default (no -Validate switch) runs both sync + validate. Auto-generates frontmatter mirror в each skeleton brief file.

### 9.4 Render regeneration

```powershell
pwsh tools/governance/render_register.ps1
```

Regenerates REGISTER_RENDER.md с new 4 skeleton entries в Category D section.

### 9.5 PENDING-COMMIT backfill

После Phase 1-7 commits, capture hashes:

```bash
COMMIT_FRAMEWORK=$(git log -n 1 --format="%H" -- docs/governance/FRAMEWORK.md)
COMMIT_TOOLING=$(git log -n 1 --format="%H" -- tools/governance/sync_register.ps1)
COMMIT_K10_EXEC=$(git log -n 1 --format="%H" -- tools/briefs/K10_EXECUTION_BRIEF.md)
COMMIT_K10_XDOC=$(git log -n 1 --format="%H" -- tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md)
COMMIT_KCLOSURE=$(git log -n 1 --format="%H" -- tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md)
COMMIT_ROSLYN=$(git log -n 1 --format="%H" -- tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md)
COMMIT_REGISTER=$(git log -n 1 --format="%H" -- docs/governance/REGISTER.yaml)
```

Update REGISTER.yaml — replace PENDING-COMMIT-N placeholders:
- DOC-A-FRAMEWORK.last_modified_commit: replace «PENDING-COMMIT-1» с $COMMIT_FRAMEWORK
- DOC-D-K10_EXECUTION.last_modified_commit: replace «PENDING-COMMIT-3» с $COMMIT_K10_EXEC
- DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE.last_modified_commit: replace «PENDING-COMMIT-4» с $COMMIT_K10_XDOC
- DOC-D-A_PRIME_8_K_CLOSURE_REPORT.last_modified_commit: replace «PENDING-COMMIT-5» с $COMMIT_KCLOSURE
- DOC-D-A_PRIME_9_ROSLYN_ANALYZER.last_modified_commit: replace «PENDING-COMMIT-6» с $COMMIT_ROSLYN
- Schema metadata last_modified_commit: leave as «PENDING-COMMIT-7» per FRAMEWORK §8.3 Option B (backfill next governance commit)

Note: COMMIT_TOOLING (sync_register.ps1 update) doesn't have register entry tracking it (governance tooling itself; meta-level). No backfill needed.

### 9.6 EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK entry

Add к REGISTER.yaml `audit_trail` collection:

```yaml
  - id: EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK
    date: "2026-05-17"
    event: "Brief skeleton framework — schema extension AUTHORED-SKELETON + 4 К/А' forward planning skeleton briefs authored"
    event_type: governance_event
    documents_affected:
      - DOC-A-FRAMEWORK                                     # v1.0 → v1.1
      - DOC-D-K10_EXECUTION                                 # NEW skeleton
      - DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE              # NEW skeleton
      - DOC-D-A_PRIME_8_K_CLOSURE_REPORT                    # NEW skeleton
      - DOC-D-A_PRIME_9_ROSLYN_ANALYZER                     # NEW skeleton
      - DOC-G-REGISTER                                      # register_version 1.5 → 1.6
      - DOC-G-REGISTER_RENDER                               # regenerated
      - DOC-G-VALIDATION_REPORT                             # regenerated
    commits:
      range: "PENDING-COMMIT-1..PENDING-COMMIT-8"
      key_commits:
        - hash: "PENDING-COMMIT-1"
          summary: "FRAMEWORK.md v1.0 → v1.1 — AUTHORED-SKELETON lifecycle value added (§3.3 + §3.3.1 + §3.4 + §3.4.1)"
        - hash: "PENDING-COMMIT-2"
          summary: "sync_register.ps1 + query_register.ps1 tooling update — AUTHORED-SKELETON enum + forbidden combos"
        - hash: "PENDING-COMMIT-3"
          summary: "К10 execution brief skeleton authored"
        - hash: "PENDING-COMMIT-4"
          summary: "К10 cross-document amendments cascade brief skeleton authored"
        - hash: "PENDING-COMMIT-5"
          summary: "А'.8 K-series closure report brief skeleton authored"
        - hash: "PENDING-COMMIT-6"
          summary: "А'.9 Roslyn architectural analyzer brief skeleton authored"
        - hash: "PENDING-COMMIT-7"
          summary: "REGISTER enrollment — 4 skeleton briefs + DOC-A-FRAMEWORK bump + register_version 1.5 → 1.6"
        - hash: "PENDING-COMMIT-8"
          summary: "Phase 8 closure — PENDING-COMMIT backfill + EVT entry + render/validation regen"
    governance_impact: |
      Brief skeleton framework established. Forward operational plan for К-series + А' остающееся visible в register.

      Schema extension: FRAMEWORK.md v1.0 → v1.1 added AUTHORED-SKELETON lifecycle value (8 → 9 values).
      Tooling forward-compatible с new enum value (sync_register.ps1 + query_register.ps1 updated; render_register.ps1 unchanged — no hardcoded enum).
      
      Four skeleton briefs authored at Tier 3 Category D:
      - DOC-D-K10_EXECUTION: К10 native kernel scheduler implementation (46 items + 12 К-L invariants)
      - DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE: К10 propagation across 8 dependent documents
      - DOC-D-A_PRIME_8_K_CLOSURE_REPORT: К-series formal closure (substantial scope)
      - DOC-D-A_PRIME_9_ROSLYN_ANALYZER: К-Lxx invariant Roslyn analyzer encoding (dual purpose)
      
      Skeleton briefs carry: expected sub-milestones, Phase 0 reads, halt classes, Q-N execution seeds, closure protocol stubs.
      Promotion к AUTHORED gates per skeleton brief individual triggers:
      - К10 execution: post-К8.5 closure (Opus deliberation session for full brief)
      - К10 cross-doc cascade: post-К10 execution closure (А'.7) or earlier per Crystalka prioritization
      - А'.8 K-closure: post-К10 execution closure (Opus deliberation session)
      - А'.9 Roslyn analyzer: post-К-closure report landed (А'.8 → А'.9 sequence)

      М-series + FO research forks explicitly OUT of scope per Crystalka Q1 lock 2026-05-17.

      К8.5 brief NOT enrolled (already DOC-D-K8_5 AUTHORED in register — Lesson #11 redundancy check applied).
    cross_references:
      capa_entries: []  # no CAPAs opened или closed at this event
      risks:
        - RISK-004  # cross-document drift — skeleton framework establishes forward-planning visibility
```

### 9.7 Atomic commit 8: Phase 8 closure

```
git add docs/governance/REGISTER.yaml docs/governance/REGISTER_RENDER.md docs/governance/VALIDATION_REPORT.md tools/briefs/K10_EXECUTION_BRIEF.md tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md
git commit -m "governance(closure): brief skeleton framework closure — EVT + PENDING-COMMIT backfill + frontmatter sync + render regen

Brief skeleton framework closure per FRAMEWORK §6 post-session protocol.

Changes:
- REGISTER.yaml: EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK audit trail entry added
- REGISTER.yaml: PENDING-COMMIT-1 through PENDING-COMMIT-6 backfilled с actual hashes
- REGISTER.yaml: PENDING-COMMIT-7 (schema metadata self-reference) left per FRAMEWORK §8.3 Option B
- REGISTER_RENDER.md: regenerated с 4 new DOC-D AUTHORED-SKELETON entries
- VALIDATION_REPORT.md: regenerated post sync_register.ps1 --validate (exit 0)
- 4 skeleton brief files: frontmatter mirrors synced

Forward planning visibility established:
- К10 execution skeleton ready для Opus deliberation session post-К8.5
- К10 cross-doc cascade skeleton ready для post-К10 execution or earlier
- А'.8 K-closure skeleton ready для post-К10 execution Opus deliberation
- А'.9 Roslyn analyzer skeleton ready для post-К-closure landing

Schema extension AUTHORED-SKELETON proven через 4 enrollment cases. К-L14 application (architectural cleanliness over special_case_rationale workaround). 

Refs: EVT-2026-05-17-BRIEF-SKELETON-FRAMEWORK"
```

### 9.8 Phase 8 outcome

1 atomic commit (Phase 8 closure). После this commit, всё skeleton framework work landed.

**Total**: 8 atomic commits (Phases 1-8).

---

## 10. Phase 9 — Push к origin

### 10.1 Scope

Push branch `claude/brief-skeleton-framework` к origin. Crystalka manually opens PR per repository convention.

### 10.2 Push command

```bash
git push -u origin claude/brief-skeleton-framework
```

### 10.3 Phase 9 outcome

Branch pushed к origin. Crystalka reviews и merges.

---

## 11. Halt conditions

Per METHODOLOGY §3 stop-escalate-lock.

### 11.1 Hard gates (HG-N)

**HG-1**: Working tree dirty before Phase 0. Surface к Crystalka; resolve workspace.

**HG-2**: sync_register.ps1 --validate exit non-zero в Phase 8. Surface с full output.

**HG-3**: Recent commit log shows governance state inconsistent — e.g., FRAMEWORK already at v1.1, AUTHORED-SKELETON already в enum. Surface к Crystalka.

### 11.2 Soft conditions (SC-N)

**SC-1**: К8.5 brief found NOT в register as AUTHORED — verify state, surface если discrepancy.

**SC-2**: К10 specification version drifted post-skeleton authoring (v2.0 → v2.x via amendment). Skeletons referencing v2.0 may need patch. Halt и surface.

**SC-3**: Tooling regression — sync_register.ps1 --validate exit non-zero после Phase 2 edits на existing register без AUTHORED-SKELETON entries (should pass). Indicates tooling break, not skeleton-related. Surface с full output.

**SC-4**: Existing entry conflicts с new skeleton — DOC-D-K10_* prefix collision with existing DOC-D-K10_* entry. Verify uniqueness during Phase 7.

**SC-5**: К-closure report skeleton scope contradicts К10 execution skeleton scope (e.g., К-closure assumes K11+ measurements; К10 execution defers them). Surface к Crystalka.

### 11.3 Halt artifact pattern

If halt condition fires:

```
docs/scratch/BRIEF_SKELETON_FRAMEWORK/HALT_REPORT.md
```

Content: halt reason, condition, findings, options, recommended action. Commit halt artifact, await Crystalka direction.

---

## 12. Closure summary

### 12.1 Expected commit count

8 atomic commits:

1. FRAMEWORK.md v1.0 → v1.1 (schema extension)
2. Tooling update (sync_register.ps1 + query_register.ps1)
3. К10 execution skeleton
4. К10 cross-doc cascade skeleton
5. А'.8 K-closure skeleton
6. А'.9 Roslyn analyzer skeleton
7. REGISTER.yaml enrollment + version bumps
8. Phase 8 closure (EVT + backfill + render regen + sync)

### 12.2 Expected document state at closure

- `docs/governance/FRAMEWORK.md` — v1.1, 9 lifecycle states (AUTHORED-SKELETON added)
- `tools/governance/sync_register.ps1` — updated `$VALID_LIFECYCLES` array + forbidden combos
- `tools/governance/query_register.ps1` — updated Lifecycle parameter docstring
- `tools/briefs/K10_EXECUTION_BRIEF.md` — NEW skeleton, v0.1, AUTHORED-SKELETON
- `tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md` — NEW skeleton, v0.1, AUTHORED-SKELETON
- `tools/briefs/A_PRIME_8_K_CLOSURE_REPORT_BRIEF.md` — NEW skeleton, v0.1, AUTHORED-SKELETON
- `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` — NEW skeleton, v0.1, AUTHORED-SKELETON
- `docs/governance/REGISTER.yaml` — register_version 1.6, 4 new DOC-D entries, DOC-A-FRAMEWORK v1.1, EVT-2026-05-17 audit trail entry
- `docs/governance/REGISTER_RENDER.md` — regenerated с 4 new skeleton entries
- `docs/governance/VALIDATION_REPORT.md` — regenerated, all checks pass

### 12.3 Next operational phase

Forward planning visible. Next session priorities:

1. К8.5 execution (А'.6, pre-existing AUTHORED brief)
2. К10 execution brief authoring (Opus deliberation session, post-К8.5)
3. К10 execution (А'.7, Claude Code session)
4. К10 cross-doc cascade brief authoring + execution
5. А'.8 K-closure deliberation session + execution
6. А'.9 Roslyn analyzer brief authoring + execution
7. Phase B M-series

«Без костылей.» Skeleton framework establishes forward visibility без premature commitment. Each skeleton к AUTHORED promotion gates на proper milestone timing.

---

## Appendix A — Lesson application discipline

During execution, apply lessons actively:

**Lesson #7** — Verify sync_register.ps1 `$VALID_LIFECYCLES` line content via Phase 0 read; don't paraphrase array contents. Same для query_register.ps1 Lifecycle parameter docstring.

**Lesson #8** — Each skeleton brief = 1 atomic commit (compilable unit). FRAMEWORK amendment = 1 atomic commit. Tooling update = 1 atomic commit. REGISTER enrollment = 1 atomic commit. Closure = 1 atomic commit. Each commit produces well-formed state.

**Lesson #11** — К8.5 redundancy check applied (NOT enrolled as skeleton since DOC-D-K8_5 already AUTHORED). Apply same check before adding entries.

**Lesson #20** — Schema extension chosen over special_case_rationale workaround. К-L14 application — architectural cleanliness over effort cost (full FRAMEWORK + tooling change).

**Lesson #22** — Read existing FRAMEWORK.md §3.3 structure before authoring amendment. Read sync_register.ps1 `$VALID_LIFECYCLES` line before authoring update. Read existing Category D entries before authoring new DOC-D entries. Local conventions are authority.

---

## Appendix B — К8.5 exclusion rationale

К8.5 brief is **already enrolled** в REGISTER as DOC-D-K8_5 with lifecycle AUTHORED. This is **full AUTHORED** (not skeleton). К8.5 brief was authored 2026-05-10 pre-К10 deliberation; awaits execution as А'.6.

**Lesson #11 redundancy check applied**: adding К8.5 as skeleton would be redundant entry. Skip from skeleton framework scope.

If К8.5 brief needs re-authoring post-К10 deliberation (К10 spec changes may require К8.5 patch), use patch-brief mutation pattern (precedent: K9_BRIEF_REFRESH_PATCH). К8.5 stays AUTHORED until executed (transitions к EXECUTED) или patched (stays AUTHORED).

---

## Appendix C — Forward sequencing visualization

After this brief lands:

```
[CURRENT STATE]
    ↓
A'.6 К8.5 execution
    │   (DOC-D-K8_5 AUTHORED → EXECUTED)
    ↓
К10 execution brief authoring (Opus deliberation)
    │   (DOC-D-K10_EXECUTION SKELETON → AUTHORED)
    ↓
A'.7 К10 execution (Claude Code)
    │   (DOC-D-K10_EXECUTION AUTHORED → EXECUTED)
    │   (К10 specification v2.0 → v3.0 post-execution)
    ↓
К10 cross-doc cascade brief authoring + execution
    │   (DOC-D-K10_CROSS_DOC_AMENDMENTS_CASCADE SKELETON → AUTHORED → EXECUTED)
    │   (8 dependent documents amended; K-closure report scoped)
    ↓
A'.8 K-closure deliberation + authoring + execution
    │   (DOC-D-A_PRIME_8_K_CLOSURE_REPORT SKELETON → AUTHORED → EXECUTED)
    │   (Provisional Lessons promotion review; К-L14 evidence framing)
    ↓
A'.9 Roslyn analyzer brief authoring + execution
    │   (DOC-D-A_PRIME_9_ROSLYN_ANALYZER SKELETON → AUTHORED → EXECUTED)
    │   (К-Lxx invariants encoded; first-run cleanup; warning → error promotion)
    ↓
Phase B M-series begins (М-K vanilla mods первые)
```

Each lifecycle transition surfaces в REGISTER audit_trail. Forward visibility maintained throughout К-series tail и А'-cycle completion.

---

**End of Brief**

Brief skeleton framework execution brief. ~2-3 hours auto-mode. 8 atomic commits. Schema extension + 4 skeleton briefs + REGISTER enrollment + closure protocol. К-L14 architectural cleanliness preserved (full schema extension, no workaround).

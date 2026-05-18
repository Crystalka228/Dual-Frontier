# K8.5 Deferral Cascade — Execution Brief

**Brief authored**: 2026-05-18 (Claude Opus 4.7, deliberation mode, K8.5 brief drift surface session)
**Target session**: Claude Code execution mode
**Brief type**: Governance amendment + register reclassification + deferral marker cascade
**Estimated time**: 1.5-2.5 hours auto-mode (~1 day at hobby pace)
**Branch**: `claude/k8-5-deferral-cascade`
**Parent**: K8.5 brief drift surfaced via Lesson #22 reconnaissance (2026-05-18 session)

---

## 0. Executive summary

K8.5 brief (`tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md`) **currently classified** in REGISTER.yaml as DOC-D-K8_5 with `lifecycle: AUTHORED`. Actual content is **skeleton-grade** (file header «Status: SKELETON», first TODO line «Author full brief», trigger note «Brief authoring trigger: after K8.4 closure»).

Brief content premise — «mod ecosystem migration prep from v2 to v3 для external mod authors» — **mismatches current reality**:
- Vanilla mods (6 assemblies in `mods/DualFrontier.Mod.Vanilla.*/`) exist as **structural placeholders only**, implementation deferred к Phase B per composite namespace ratification (PR #34, 2026-05-16)
- No external mod ecosystem exists (project pre-release, no v2 codebase к migrate)
- v2 vs v3 distinction в skeleton refers к pre-K8.3+K8.4 vs post-K8.3+K8.4 API surface, but К-series capstone direction (К10 specification v2.0 LOCKED) means К-series complete at К10 + К-closure, not at К8.5

**Decision**: K8.5 deferred к **post-Phase B initial M-series sprint** (when mod authors as documentation audience exist). Phase A'.6 slot marked **SKIPPED** with deferral marker. Phase A' continues А'.5 → (А'.6 skipped) → А'.7 К10 → А'.8 K-closure → А'.9 Roslyn analyzer.

**Authority for decision**: Crystalka direction 2026-05-18 («просто пропускаем и двигаемся дальше только лишь фиксируем что K8.5 пропущен»).

**Cascade scope**:
1. REGISTER.yaml — DOC-D-K8_5 lifecycle AUTHORED → AUTHORED-SKELETON, deferral note in `special_case_rationale`
2. PHASE_A_PRIME_SEQUENCING.md (Tier 2 Live) — §2 sequence amended (А'.6 marked SKIPPED), §3 duration table updated, §5.2/§5.3/§5.5/§5.6 amendment references updated, §6 completion gate updated, §7.1 К-closure approach updated
3. CAPA entry: open+close в same governance commit per A'.4.5 precedent
4. EVT-2026-05-18-K8_5-DEFERRAL audit trail entry
5. Lesson #23 candidate authored: register classification drift from actual document state

**Out of scope**:
- KERNEL_ARCHITECTURE.md (К8.5 canonical identifier upstream of execution sequencing — preserved unchanged)
- MIGRATION_PLAN_KERNEL_TO_VANILLA.md (К8.5 fact в plan unchanged; only execution sequence shifts)
- К10 specification (orthogonal к К8.5 disposition)
- К8.5 skeleton brief file itself (preserved as historical record; lifecycle reclassification is metadata-only)
- METHODOLOGY.md formal Lesson promotion (Lesson #23 added к Provisional Lessons pool only; formalization at А'.8 K-closure per existing timing lock)

**Target outcomes**: 5 atomic commits expected.

**Lesson discipline reminders**:
- **Lesson #7** (verbatim APIs): PHASE_A_PRIME_SEQUENCING.md amendments quoted verbatim against current state pre-edit
- **Lesson #8** (atomic compilable commits): each commit produces well-formed governance state; sync_register --validate exit 0 across intermediate states
- **Lesson #11** (architectural reduction redundancy check): К8.5 brief itself preserved; only metadata (lifecycle) reclassified
- **Lesson #20** (tactical heuristics): defer decision validated by absence of audience (architectural reason), not effort cost
- **Lesson #22** (read existing code first): full PHASE_A_PRIME_SEQUENCING read pre-edit; K8.5 brief actual content verified

---

## 1. Phase 0 — Preflight reads (mandatory)

### 1.1 Authority documents

- `docs/governance/FRAMEWORK.md` v1.1 — §3.3 (AUTHORED-SKELETON lifecycle), §3.3.1 (transitions), §8.3 (PENDING-COMMIT backfilling protocol)
- `docs/governance/REGISTER.yaml` — read fully, particular attention к:
  - DOC-D-K8_5 entry (current state pre-amendment)
  - DOC-A-PHASE_A_PRIME_SEQUENCING entry (Tier 2 Live tracking)
  - schema_version, register_version (currently 1.6 per brief skeleton framework closure)
  - capa_entries collection (last CAPA number to allocate next)
  - audit_trail collection (last EVT entry shape для new entry authoring)

### 1.2 Drift-affected document

- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (Tier 2 Live) — read fully. 26 KB, §2 sequence + §3 duration + §4 invariants + §5 amendments + §6 completion gate + §7 open questions. Note: §4.1 «Sequencing is total, not partial-ordering. No skipping.» — invariant text. K8.5 deferral is NOT skipping execution (К8.5 still planned к execute, just deferred), but Phase A' slot А'.6 IS skipped per current decision. §4.1 wording may need refinement.

### 1.3 Source documents (read-only)

- `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` — read fully (3 KB). Confirms actual skeleton state. NOT edited by this brief (preserved as historical record).
- `docs/architecture/KERNEL_ARCHITECTURE.md` — confirm Part 2 lines 605 + 767 К8.5 canonical identifier exists (verify upstream authority unchanged). Read only those lines; full read not required.
- `docs/architecture/COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` (Project file, not repo) — Q-K-1 reconciliation context. К-1 retroactive lock pointer mentions «K8.5 brief authoring approaches» trigger — needs update via this cascade.

### 1.4 Tooling

- `tools/governance/sync_register.ps1` — verify --validate behavior with AUTHORED-SKELETON lifecycle (post-brief-skeleton-framework Phase 2 commit `e43349d`); confirm Tier 3 + AUTHORED-SKELETON passes per allowed-combinations matrix update

### 1.5 Working tree state

```bash
git status
git log --oneline -5 main
```

Must be clean. Recent commits should include brief skeleton framework PR #N merge (8 commits `15ffa0a` → `ce0791d`). If main doesn't reflect brief skeleton framework merge, halt HG-3.

```bash
git ls-files docs/governance/REGISTER.yaml
git ls-files docs/architecture/PHASE_A_PRIME_SEQUENCING.md
git ls-files tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md
```

All three must exist on main.

---

## 2. Phase 1 — REGISTER.yaml: DOC-D-K8_5 reclassification

### 2.1 Scope

Update DOC-D-K8_5 entry in REGISTER.yaml:
- `lifecycle`: `AUTHORED` → `AUTHORED-SKELETON`
- `version`: `"1.0"` → `"0.1"` (skeleton convention matching К10 execution / К10 cross-doc cascade / А'.8 K-closure / А'.9 analyzer skeletons)
- `last_modified`: bump к `"2026-05-18"`
- `last_modified_commit`: placeholder `"PENDING-COMMIT-K8_5-1"` (backfilled Phase 5)
- `last_review_date`: `"2026-05-18"`
- `last_review_event`: `"Reclassified AUTHORED → AUTHORED-SKELETON per K8.5 deferral cascade (actual content skeleton-grade; mod authors audience deferred к post-Phase B)"`
- `next_review_due`: `"null"` (skeleton awaits trigger, no scheduled review)
- `special_case_rationale`: replace existing OR add new rationale (verify current state in Phase 0):
  > "Skeleton brief awaiting full brief authoring at proper milestone timing. Content (mod ecosystem migration prep from v2 to v3) premised on external mod authors audience; vanilla mods deferred к Phase B per composite namespace ratification (PR #34, 2026-05-16) means no current audience. Promotion к AUTHORED triggers when Phase B initial M-series sprint begins establishing mod author audience. Deferred from Phase А'.6 slot 2026-05-18 per Crystalka direction."

### 2.2 register_version bump

REGISTER.yaml metadata:
- `register_version`: `"1.6"` → `"1.7"`
- `last_modified`: `"2026-05-18"`
- `last_modified_commit`: `"PENDING-COMMIT-K8_5-CLOSURE"` (backfilled Phase 5)
- `last_modified_by`: `"Claude Code"`

### 2.3 DOC-A-PHASE_A_PRIME_SEQUENCING bump

DOC-A-PHASE_A_PRIME_SEQUENCING entry — bump `last_modified` к `"2026-05-18"`, `last_modified_commit` к `"PENDING-COMMIT-K8_5-2"` (backfilled Phase 5). Content version stays "Live" (Tier 2 mutable). Add `last_review_event`: `"§2 sequence + §3 duration table amended per K8.5 deferral cascade"`.

### 2.4 Atomic commit 1: REGISTER.yaml reclassification

```bash
git checkout -b claude/k8-5-deferral-cascade
# [edit docs/governance/REGISTER.yaml per §2.1-2.3]
git add docs/governance/REGISTER.yaml
git commit -m "governance(register): K8.5 lifecycle AUTHORED → AUTHORED-SKELETON per deferral cascade

DOC-D-K8_5 reclassified per K8.5 brief drift surface session (2026-05-18).

Changes:
- DOC-D-K8_5.lifecycle: AUTHORED → AUTHORED-SKELETON
- DOC-D-K8_5.version: 1.0 → 0.1 (skeleton convention)
- DOC-D-K8_5.special_case_rationale: deferral note added (mod authors audience deferred к Phase B per composite namespace ratification)
- DOC-A-PHASE_A_PRIME_SEQUENCING.last_modified bump (downstream amendments in subsequent commit)
- register_version 1.6 → 1.7

K8.5 brief skeleton content preserved unchanged at tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md (historical record).

Refs: DOC-D-K8_5, DOC-A-PHASE_A_PRIME_SEQUENCING, EVT-2026-05-18-K8_5-DEFERRAL (added Phase 5)"
```

### 2.5 Phase 1 outcome

1 atomic commit. DOC-D-K8_5 metadata reflects actual skeleton state. Proceed к Phase 2.

---

## 3. Phase 2 — PHASE_A_PRIME_SEQUENCING.md amendments

### 3.1 Scope

Tier 2 Live document — mutation allowed per closure protocol (review_cadence: on-closure+quarterly).

Multiple sections amended:

### 3.2 §2 sequence amendment

#### 3.2.1 Q-K-1 reconciliation note update

Locate existing note (added 2026-05-16, composite namespace ratification cascade). Current text says:
> "Resolution of the A'-cycle renumbering question (whether to propagate A'.6 = K8.5 + A'.7 = analyzer through body/§3 table, or to leave the closure note's intent unimplemented) is a downstream **sequencing-label concern**, deferred to subsequent deliberation when K8.5 brief authoring approaches."

Add update paragraph after existing note:

> **Update 2026-05-18 (K8.5 deferral cascade)**: K8.5 brief authoring does not approach within Phase A' execution. K8.5 deferred к post-Phase B initial M-series sprint when mod authors audience exists (per Crystalka direction 2026-05-18, surfaced via skeleton drift detection — actual brief content remains skeleton-grade, premise of v2-to-v3 mod authors migration mismatches reality given vanilla mods deferred к Phase B per composite namespace ratification PR #34). Phase A' execution sequence revised: А'.5 (CLOSED) → **А'.6 SKIPPED (deferral marker)** → А'.7 К10 (next) → А'.8 K-closure → А'.9 Roslyn analyzer → [M8.4 Phase B begins]. Q-K-1 retroactive lock trigger updated: was «K8.5 brief authoring approaches»; is now «post-Phase B initial M-series sprint when mod authors audience exists». Q-K-1 reconciliation remains pending until that trigger.

#### 3.2.2 §2 sequence diagram amendment

Locate sequence diagram. Replace А'.6 and А'.7 entries:

**Existing**:
```
  ├─ Phase A'.6 — (formerly K8.4 standalone — combined into A'.5 above) K8.5 skeleton execution
  │   Brief: K8.5 mod-ecosystem migration prep (re-numbered after K8.3+K8.4 combination)
  │   Scope: per the original A'.7 K8.5 description below — mod authoring guide updated post-K-L3.1 + dual API documentation + compatibility test plan published
  │   Note: A'.6 was originally «K8.4 skeleton execution» before the K8.3+K8.4 combination; that scope is now closed within A'.5 per above.
  │
  ├─ Phase A'.7 — K8.5 skeleton execution
  │   Brief: K8.5 skeleton authored, post-Phase-A'.1 amendment if disposition B/C applied
  │   Scope: Phase A → B handoff (docs, capability annotations, readiness gate)
  │   K8.5 is the natural place for MOD_OS_ARCHITECTURE.md v1.7 amendment per Migration Plan §6.2
  │   Estimated time: 3-5 days hobby pace, ~1-2 hours auto-mode
  │   Executor: Cloud Code
  │
```

**Replaced with**:
```
  ├─ Phase A'.6 — SKIPPED — K8.5 deferred к post-Phase B (2026-05-18 deferral cascade)
  │   Original scope: K8.5 mod-ecosystem migration prep (mod authoring guide + dual API documentation + compatibility test plan)
  │   Deferral rationale: K8.5 brief content skeleton-grade; premise (mod ecosystem migration from v2 to v3) requires external mod authors audience; vanilla mods deferred к Phase B per composite namespace ratification (PR #34, 2026-05-16) means audience absent. Documentation milestone needs target audience к serve.
  │   K8.5 canonical identifier preserved per KERNEL_ARCHITECTURE.md Part 2 lines 605 + 767 (unchanged).
  │   DOC-D-K8_5 reclassified AUTHORED → AUTHORED-SKELETON per K8.5 deferral cascade (EVT-2026-05-18-K8_5-DEFERRAL).
  │   Promotion trigger: post-Phase B initial M-series sprint when mod authors audience exists.
  │   Phase A' execution proceeds к А'.7 К10 without K8.5 intermediate step.
  │
  ├─ Phase A'.7 — К10 native kernel scheduler execution
  │   Brief: K10_EXECUTION_BRIEF.md (currently DOC-D-K10_EXECUTION lifecycle AUTHORED-SKELETON; promotion к AUTHORED at brief authoring session post-А'.5 closure)
  │   Specification: KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED — 46-item К10 scope, 12 К-L invariants targeted (K-L7.1 sub + K-L12-19), TLA+ formal verification
  │   Scope: full native kernel scheduler + bus + control plane migration + V substrate interlock + TLA+ formal verification
  │   К-L6 SUPERSEDED rationale realized; К-L12-19 + К-L7.1 candidates promoted к LOCKED at К10 execution closure
  │   Estimated time: substantial; 1500-2500 line execution brief; multi-session Claude Code execution possible
  │   Executor: Opus deliberation (brief authoring) + Claude Code (execution)
  │
```

#### 3.2.3 К10 cross-doc cascade note (optional, may bundle with К10 execution per К10 cross-doc cascade brief skeleton §1.3 «may bundle с К10 execution»)

No separate sub-phase entry needed; К10 execution closure carries cross-doc cascade per К10 cross-doc cascade brief skeleton.

### 3.3 §3 duration table amendment

Locate table. Replace rows А'.5, А'.6, А'.7:

**Existing rows**:
```
| A'.5 K8.3 | 4-6 weeks | 6-9 weeks |
| A'.6 K8.4 | 1-2 weeks | 7-11 weeks |
| A'.7 K8.5 | 3-5 days | 8-12 weeks |
```

**Replaced with**:
```
| A'.5 K8.3+K8.4 combined (CLOSED 2026-05-14) | 4 commits, 1 session | <1 day |
| A'.6 K8.5 SKIPPED (deferred к post-Phase B) | — | — |
| A'.7 K10 native kernel scheduler | substantial; multi-session | TBD (post-execution measurement) |
```

Update cumulative timeline note:
> «**Total Phase A' duration: ~10-16 weeks at hobby pace** (~2.5-4 months).» 

Replace with:
> «**Total Phase A' duration**: revised estimate post-K8.5 deferral. А'.5 closed in 1 session 2026-05-14 (compressed vs. 4-6 weeks original estimate). А'.7 К10 execution substantial (multi-session expected). Phase A' duration TBD pending К10 execution closure measurements; original 10-16 weeks estimate retained as rough order-of-magnitude pending K10 measurement. The А'.0/А'.0.5/А'.0.7 doc-only deliberation+refresh sessions were sub-day each as predicted.»

### 3.4 §4.1 sequencing invariant refinement

Current text: «Sequencing is total, not partial-ordering. Each Phase A'.N depends on A'.<N-1> closure. **No skipping**. No parallelism within A'...»

Add clarification paragraph at end of §4.1:

> **Refinement 2026-05-18 (K8.5 deferral cascade)**: «No skipping» applies к **execution work**, not к slot allocation. Phase А'.6 slot marked SKIPPED because the milestone originally planned for that slot (K8.5) is deferred к post-Phase B per audience-absence architectural reason — not because Phase A' work itself is skipped. Phase A' execution proceeds от А'.5 closure к А'.7 К10 directly. К8.5 milestone preserves canonical identity per KERNEL_ARCHITECTURE.md Part 2; only its Phase A' execution slot defers. Sequencing invariant violation candidate: «А'.7 depends on А'.6 closure» — checked: К10 execution (А'.7) has no dependency on К8.5 outputs (K8.5 documentation milestone, К10 architectural milestone, orthogonal scopes). Invariant preserved by dependency check, not by slot-numbering continuity.

### 3.5 §5.2 + §5.3 amendment references update

**§5.2 MOD_OS_ARCHITECTURE.md** locate:
> «**A'.6 K8.4 amendment**: §4.6 IModApi v3 surface finalized per K-L3.1 Q1+Q2 lock.
> **A'.7 K8.5 amendment**: §11 migration phases table extended with M8.4-M10.B rows per Migration Plan §6.2 schedule.»

Replace with:
> «**A'.5 K8.3+K8.4 combined amendment (DONE 2026-05-14)**: §4.6 IModApi v3 surface finalized per K-L3.1 Q1+Q2 lock + post-cutover NativeWorld single source of truth wording.
> **A'.6 K8.5 amendment**: DEFERRED. Originally §11 migration phases table extended with M8.4-M10.B rows per Migration Plan §6.2 schedule. Deferral к post-Phase B per K8.5 deferral cascade (2026-05-18).
> **A'.7 K10 amendment**: cross-doc cascade per `tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md` skeleton (AUTHORED-SKELETON; promotion к AUTHORED at К10 execution closure or earlier per prioritization). MOD_OS section updates per K10 specification v2.0 Part 7 cross-document amendments queue (capability section tier-prefixed tokens, §9.5 unload chain extension, §11 К-L18 quiescent state compliance).»

**§5.3 MIGRATION_PLAN_KERNEL_TO_VANILLA.md** locate:
> «**A'.4-A'.7**: per-milestone scope clarifications as needed.»

Replace with:
> «**A'.4-A'.5**: per-milestone scope clarifications as needed (A'.4 K9 DONE; A'.5 K8.3+K8.4 DONE).
> **A'.6 K8.5**: DEFERRED — no MIGRATION_PLAN amendment trigger fires within Phase A' for K8.5.
> **A'.7 К10**: К10 native kernel scheduler may produce Migration Plan amendments per К10 execution closure; scope deferred к К10 execution brief authoring.»

### 3.6 §5.5 MIGRATION_PROGRESS amendment references update

Locate:
> «A'.4-A'.7 per-milestone closure entries (existing precedent, K8.1/K8.1.1/K-Lessons shape)»

Replace with:
> «A'.4 K9 closure entry (DONE 2026-05-11)
> A'.4.5 Document Control Register closure entry (DONE 2026-05-12)
> A'.5 K8.3+K8.4 combined closure entry (DONE 2026-05-14)
> A'.6 K8.5 DEFERRAL entry (this cascade 2026-05-18; closure-as-deferral, not closure-as-execution)
> A'.7 К10 closure entry (planned post-К10 execution)»

### 3.6.1 §5.6 ROADMAP.md amendment references update

Locate:
> «Phase A' is the natural occasion for full ROADMAP refresh — this is **not** scheduled here as separate milestone but as **A'.7 K8.5 housekeeping** or **A'.8 K-closure report** companion update.»

Replace with:
> «Phase A' is the natural occasion for full ROADMAP refresh — this is **not** scheduled here as separate milestone but as **А'.8 K-closure report companion update** (К8.5 housekeeping path retired per K8.5 deferral cascade 2026-05-18).»

### 3.7 §6 completion gate amendment

Locate:
> «- [ ] K-series fully closed: 31 components struct-or-explicit-managed-bridge per K-L3.1 lock; 31+3 systems migrated to SpanLease/WriteBatch (or fewer per K8.3 scope amendment); managed World retired as production path; capability annotation pass complete; M-series migration guide published»

Replace with:
> «- [x] K-series substrate closed via К8.3+К8.4 cutover (А'.5 closure 2026-05-14): 10 production systems migrated к NativeWorld AcquireSpan/BeginBatch, managed World retired как ManagedTestWorld test fixture, Mod API v3 closed, runtime isolation guard removed per K-L11
> - [ ] К10 native kernel scheduler executed (А'.7): full native scheduler + bus + control plane + V substrate interlock + TLA+ formal verification per KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED; К-L12-19 + К-L7.1 invariants promoted к LOCKED
> - [ ] K-closure report authored (А'.8): formal К-Lxx invariant enumeration + analyzer rule specification surface + Provisional Lessons promotion review (9 candidates)
> - [ ] Architectural analyzer milestone executed (А'.9): rules encoding K-closure report invariants; first run on existing codebase passes (debt fixed during analyzer milestone scope per Linux -Werror precedent); CI integration active
> - [ ] **М-series migration guide deferred к post-Phase B (К8.5 deferred)**: documentation milestone awaits mod authors audience; not a Phase A' completion gate.»

### 3.8 §7.1 К-closure report authoring approach update

Locate:
> «Default lean: **(c) skeleton-first**. Each milestone (K9, K8.3, K8.4, K8.5) populates its section as it closes; A'.8 finalizes.»

Replace with:
> «Default lean: **(c) skeleton-first**. Each milestone (K9, K8.3+K8.4 combined, К10) populates its section as it closes; А'.8 finalizes. К8.5 deferred к post-Phase B (2026-05-18 deferral cascade) means К-closure report does not include К8.5 section initially; К8.5 closure section added post-Phase B при К8.5 execution.»

### 3.9 Atomic commit 2: PHASE_A_PRIME_SEQUENCING.md amendments

```bash
# [edit docs/architecture/PHASE_A_PRIME_SEQUENCING.md per §3.2-3.8]
git add docs/architecture/PHASE_A_PRIME_SEQUENCING.md
git commit -m "docs(architecture): PHASE_A_PRIME_SEQUENCING amended per K8.5 deferral cascade

Phase A' execution sequence revised — К8.5 skipped, К10 next milestone.

Changes:
- §2 Q-K-1 reconciliation note: trigger updated к post-Phase B M-series sprint
- §2 sequence diagram: А'.6 marked SKIPPED with deferral rationale, А'.7 reframed К10 (was K8.5)
- §3 duration table: А'.5 actual measured (1 session), А'.6 SKIPPED, А'.7 К10 substantial TBD
- §4.1 sequencing invariant refinement: «no skipping» applies к execution work не slot allocation; dependency check confirms К10 has no К8.5 dependency
- §5.2 MOD_OS amendment references: К8.5 amendment DEFERRED, К10 cross-doc cascade reference added
- §5.3 MIGRATION_PLAN amendment references: К8.5 amendment DEFERRED
- §5.5 MIGRATION_PROGRESS amendment references: А'.6 SKIPPED-as-deferral entry instead of execution closure
- §5.6 ROADMAP amendment references: К8.5 housekeeping path retired
- §6 completion gate: К8.5 М-series migration guide deferred (not Phase A' gate)
- §7.1 К-closure report approach: К8.5 section deferred к post-Phase B

К8.5 canonical identifier preserved per KERNEL_ARCHITECTURE.md Part 2 lines 605 + 767 (upstream authority unchanged).

Refs: DOC-A-PHASE_A_PRIME_SEQUENCING, DOC-D-K8_5, EVT-2026-05-18-K8_5-DEFERRAL"
```

### 3.10 Phase 2 outcome

1 atomic commit. Phase A' sequence reflects K8.5 deferral. Proceed к Phase 3.

---

## 4. Phase 3 — CAPA entry: open+close в same commit

### 4.1 Scope

CAPA pattern per A'.4.5 precedent: open+close в same governance commit when:
1. Issue surfaced (skeleton-state register classification drift)
2. Resolution completes within this cascade (reclassification + deferral marker)
3. No follow-up work outside this brief scope

### 4.2 CAPA-2026-05-18-K8_5-DRIFT entry

Add к REGISTER.yaml `capa_entries` collection:

```yaml
  - id: CAPA-2026-05-18-K8_5-DRIFT
    title: "DOC-D-K8_5 lifecycle classification drift surfaced via Lesson #22 reconnaissance"
    opened_date: "2026-05-18"
    closed_date: "2026-05-18"
    closure_status: CLOSED
    trigger: |
      K8.5 brief reconnaissance per Lesson #22 (read existing code before brief authoring) surfaced that DOC-D-K8_5 register entry classified lifecycle AUTHORED, but actual brief content (tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md) is skeleton-grade:
      - File header: «Status: SKELETON»
      - First TODO line: «Author full brief»
      - Trigger note: «Brief authoring trigger: after K8.4 closure»
      
      Register classification did not reflect actual document state. Additionally, brief content premise (v2-to-v3 mod authors migration) mismatched current reality (vanilla mods deferred к Phase B per composite namespace ratification PR #34, no v2 codebase exists).
    root_cause: |
      Register classification (DOC-D-K8_5 lifecycle: AUTHORED) was applied at enrollment time (A'.4.5 closure, 2026-05-12) based on enrollment evidence (brief file exists, brief authored by Opus). Actual content state (skeleton vs full) was not verified during enrollment.
      
      Additionally, post-enrollment events (composite namespace ratification PR #34 deferring vanilla mods к Phase B, 2026-05-16) drifted brief's premise from reality without triggering register classification review.
      
      No tooling currently detects register classification vs actual content state mismatch.
    corrective_action: |
      DOC-D-K8_5 reclassified AUTHORED → AUTHORED-SKELETON per K8.5 deferral cascade (this cascade, commit `PENDING-COMMIT-K8_5-1`). Phase A'.6 slot marked SKIPPED with deferral marker. K8.5 promoted к post-Phase B initial M-series sprint trigger.
    preventive_action: |
      Lesson #23 candidate authored in METHODOLOGY.md Provisional Lessons pool: «Register classification can drift from actual document state. Lesson #22 (read existing code before authoring) extended: read register entry AND open document content before assuming register classification accurate. Especially after schema extensions (AUTHORED-SKELETON post-FRAMEWORK v1.1) — pre-existing entries may need reclassification к match actual state.»
      
      Lesson formalization deferred к А'.8 K-closure report per existing timing lock (S6 lock 2026-05-17).
      
      Architectural analyzer milestone (А'.9) may encode register classification verification rules per К-closure report Roslyn analyzer rule specification (К-closure report authoring decision).
    closure_evidence: |
      Commit `PENDING-COMMIT-K8_5-1` (Phase 1) — REGISTER.yaml DOC-D-K8_5 lifecycle reclassified
      Commit `PENDING-COMMIT-K8_5-2` (Phase 2) — PHASE_A_PRIME_SEQUENCING.md sequence amended
      Commit `PENDING-COMMIT-K8_5-CLOSURE` (Phase 5) — CAPA entry + EVT entry + render regen
      sync_register.ps1 --validate exit 0 (Phase 5 gate)
    cross_references:
      documents_affected:
        - DOC-D-K8_5
        - DOC-A-PHASE_A_PRIME_SEQUENCING
        - DOC-G-REGISTER
        - DOC-B-METHODOLOGY  # via Lesson #23 candidate (Provisional Lessons pool)
      risks:
        - RISK-004  # cross-document drift class — register drift case
```

CAPA counter: current count is 11 per К10 amendments closure context. After this entry: 12.

### 4.3 CAPA entry committed в Phase 5 closure

Per atomic commit pattern, CAPA addition bundles с EVT entry + PENDING-COMMIT backfill + render regen в Phase 5 closure commit. NOT a separate commit.

### 4.4 Phase 3 outcome

No separate commit. CAPA entry authored для Phase 5 inclusion.

---

## 5. Phase 4 — Lesson #23 candidate в METHODOLOGY.md Provisional Lessons

### 5.1 Scope

Lesson #23 candidate added к Provisional Lessons pool (created in METHODOLOGY v1.8 per S6 lock 2026-05-17). Formal promotion deferred к А'.8 K-closure report.

### 5.2 Lesson #23 candidate text

Add к Provisional Lessons section (after Lesson #21 candidate per current pool ordering: #9, #10, #14, #15, #16, #17, #18, #19, #21):

```markdown
### Provisional Lesson #23 candidate — Register classification can drift from actual document state

**Origin**: K8.5 deferral cascade (2026-05-18). Lesson #22 reconnaissance surfaced register classification drift — DOC-D-K8_5 entry classified `lifecycle: AUTHORED`, actual content skeleton-grade. Mismatch existed since A'.4.5 enrollment (2026-05-12) and persisted through subsequent governance cycles without detection.

**Pattern**: Register classification applied at enrollment time based on enrollment evidence (file exists, brief authored). Actual content state may differ at enrollment OR drift post-enrollment as project context shifts (e.g., audience deferral, scope reframing, schema extensions making prior classification obsolete).

**Application**: Extend Lesson #22 (read existing code before authoring) к include register entry vs actual content reconciliation. Pre-flight reads for brief authoring against existing registered documents should:
1. Read register entry (lifecycle, version, special_case_rationale)
2. Read document content itself  
3. Reconcile entry classification with actual content state
4. If mismatch surfaced, reclassification cascade may be required before main brief work

**Strong applications counted**: 1 (К8.5 deferral cascade 2026-05-18).

**Formalization gate**: 2+ strong applications + formalization review at А'.8 K-closure report per S6 lock 2026-05-17.

**Complementary к**: Lesson #22 (read existing code first); Lesson #11 (architectural reduction redundancy check — applied к exclude К8.5 from skeleton framework based on register state, then refuted by content state — Lesson #11 also benefits from content-based reduction check).

**Potential analyzer encoding**: register classification verification rules (DOC-D entries cross-referenced with actual file content); deferred к А'.9 analyzer rule specification phase.
```

### 5.3 Phase 4 outcome

No separate commit. Lesson #23 candidate authored для Phase 5 inclusion.

---

## 6. Phase 5 — Closure protocol

### 6.1 Scope

Final closure operations:
1. Add EVT-2026-05-18-K8_5-DEFERRAL audit_trail entry
2. Add CAPA-2026-05-18-K8_5-DRIFT capa_entries entry (Phase 3 authored)
3. METHODOLOGY.md Provisional Lessons section extension with Lesson #23 candidate (Phase 4 authored)
4. Backfill PENDING-COMMIT-K8_5-N placeholders with actual hashes
5. Run sync_register.ps1 (sync + validate exit 0)
6. Render REGISTER_RENDER.md regeneration
7. Single atomic commit bundling all closure items

### 6.2 EVT-2026-05-18-K8_5-DEFERRAL entry

Add к REGISTER.yaml `audit_trail` collection:

```yaml
  - id: EVT-2026-05-18-K8_5-DEFERRAL
    date: "2026-05-18"
    event: "K8.5 deferral cascade — DOC-D-K8_5 reclassified AUTHORED → AUTHORED-SKELETON, Phase A'.6 slot SKIPPED, milestone deferred к post-Phase B"
    event_type: governance_event
    documents_affected:
      - DOC-D-K8_5                                          # lifecycle reclassified
      - DOC-A-PHASE_A_PRIME_SEQUENCING                      # §2 + §3 + §4.1 + §5.x + §6 + §7.1 amended
      - DOC-B-METHODOLOGY                                   # Lesson #23 candidate added к Provisional Lessons pool
      - DOC-G-REGISTER                                      # register_version 1.6 → 1.7
      - DOC-G-REGISTER_RENDER                               # regenerated
      - DOC-G-VALIDATION_REPORT                             # regenerated
    commits:
      range: "PENDING-COMMIT-K8_5-1..PENDING-COMMIT-K8_5-CLOSURE"
      key_commits:
        - hash: "PENDING-COMMIT-K8_5-1"
          summary: "REGISTER.yaml — DOC-D-K8_5 lifecycle AUTHORED → AUTHORED-SKELETON + version 1.0 → 0.1 + deferral note"
        - hash: "PENDING-COMMIT-K8_5-2"
          summary: "PHASE_A_PRIME_SEQUENCING.md — §2 sequence + §3 duration + §4.1 invariant + §5.x amendment refs + §6 completion gate + §7.1 К-closure approach amended"
        - hash: "PENDING-COMMIT-K8_5-3"
          summary: "METHODOLOGY.md Provisional Lessons — Lesson #23 candidate added (register classification drift from actual document state)"
        - hash: "PENDING-COMMIT-K8_5-CLOSURE"
          summary: "Closure — CAPA-2026-05-18-K8_5-DRIFT entry + EVT-2026-05-18-K8_5-DEFERRAL entry + PENDING-COMMIT backfill + REGISTER_RENDER/VALIDATION_REPORT regen"
    governance_impact: |
      K8.5 brief drift surfaced via Lesson #22 reconnaissance (K8.5 brief content skeleton-grade; premise of v2-to-v3 mod authors migration mismatched reality given vanilla mods deferred к Phase B per composite namespace ratification PR #34, 2026-05-16).
      
      Crystalka direction 2026-05-18: «просто пропускаем и двигаемся дальше только лишь фиксируем что K8.5 пропущен».
      
      Phase A' execution sequence revised:
      - А'.5 K8.3+K8.4 combined cutover (CLOSED 2026-05-14)
      - **А'.6 SKIPPED** — K8.5 deferred к post-Phase B initial M-series sprint
      - А'.7 К10 native kernel scheduler — next milestone, brief authoring at К10 execution brief Opus deliberation session
      - А'.8 K-closure report
      - А'.9 Roslyn analyzer
      - Phase B M-series begins
      
      DOC-D-K8_5 lifecycle reclassified AUTHORED → AUTHORED-SKELETON; version 1.0 → 0.1 per skeleton convention; special_case_rationale updated с deferral note.
      
      K8.5 canonical identifier preserved per KERNEL_ARCHITECTURE.md Part 2 lines 605 + 767 (upstream authority unchanged).
      
      Q-K-1 retroactive lock trigger updated: was «K8.5 brief authoring approaches» (deferred per composite namespace ratification 2026-05-16); is now «post-Phase B initial M-series sprint when mod authors audience exists».
      
      CAPA-2026-05-18-K8_5-DRIFT opened+closed within this cascade. Root cause: register classification (AUTHORED) was applied at enrollment time without actual content state verification; post-enrollment context drift (PR #34 deferring vanilla mods) shifted brief premise from reality. Preventive: Lesson #23 candidate added к Provisional Lessons pool (formalization at А'.8 per S6 timing lock).
    cross_references:
      capa_entries:
        - CAPA-2026-05-18-K8_5-DRIFT  # opened + closed within this cascade
      risks:
        - RISK-004  # cross-document drift class — register drift sub-case
```

EVT counter: current count 14 per brief skeleton framework closure context. After this entry: 15.

### 6.3 PENDING-COMMIT backfill

After Phases 1-4 commits, capture hashes:

```bash
COMMIT_K8_5_1=$(git log -n 1 --format="%H" -- docs/governance/REGISTER.yaml)  # actually Phase 1 specific
# Capture в Phase 1 immediately; Phase 5 closure backfills earlier placeholders

# Phase 5 closure capture self-reference last:
COMMIT_K8_5_CLOSURE=$(git rev-parse HEAD)  # captured after Phase 5 commit pushed locally
```

Update REGISTER.yaml — replace PENDING-COMMIT-K8_5-N placeholders:
- DOC-D-K8_5.last_modified_commit: replace «PENDING-COMMIT-K8_5-1» с actual hash
- DOC-A-PHASE_A_PRIME_SEQUENCING.last_modified_commit: replace «PENDING-COMMIT-K8_5-2» с actual hash
- DOC-B-METHODOLOGY.last_modified_commit: replace «PENDING-COMMIT-K8_5-3» с actual hash
- Schema metadata last_modified_commit: leave as «PENDING-COMMIT-K8_5-CLOSURE» per FRAMEWORK §8.3 Option B (backfill next governance commit)
- CAPA-2026-05-18-K8_5-DRIFT.closure_evidence: replace placeholders с actual hashes
- EVT-2026-05-18-K8_5-DEFERRAL.commits.range + key_commits: replace placeholders с actual hashes

### 6.4 sync_register.ps1 + render_register.ps1

```powershell
pwsh tools/governance/sync_register.ps1 -Validate
```

Expected exit code: 0. Sync runs:

```powershell
pwsh tools/governance/sync_register.ps1
```

Auto-generates frontmatter mirrors. Tier 3 + AUTHORED-SKELETON allowed combo verified (added в brief skeleton framework Phase 2 commit `e43349d`). DOC-D-K8_5 frontmatter mirror file `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` will receive updated metadata block (lifecycle: AUTHORED-SKELETON, version: 0.1).

```powershell
pwsh tools/governance/render_register.ps1
```

Regenerates REGISTER_RENDER.md.

### 6.5 Atomic commit 3 (closure): bundles items 1-4

```bash
git add docs/governance/REGISTER.yaml docs/governance/REGISTER_RENDER.md docs/governance/VALIDATION_REPORT.md docs/methodology/METHODOLOGY.md tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md
git commit -m "governance(closure): K8.5 deferral cascade — CAPA + EVT + Lesson #23 candidate + PENDING-COMMIT backfill + render regen

K8.5 deferral cascade closure per FRAMEWORK §6 post-session protocol.

Changes:
- REGISTER.yaml: CAPA-2026-05-18-K8_5-DRIFT entry added (OPEN+CLOSED within this commit, A'.4.5 precedent)
- REGISTER.yaml: EVT-2026-05-18-K8_5-DEFERRAL audit_trail entry added
- REGISTER.yaml: PENDING-COMMIT-K8_5-1..3 backfilled с actual hashes
- REGISTER.yaml: PENDING-COMMIT-K8_5-CLOSURE (schema metadata self-reference) left per FRAMEWORK §8.3 Option B
- METHODOLOGY.md: Provisional Lessons pool extended с Lesson #23 candidate (register classification drift from actual document state)
- REGISTER_RENDER.md: regenerated с updated DOC-D-K8_5 entry + new CAPA + EVT
- VALIDATION_REPORT.md: regenerated post sync_register.ps1 --validate (exit 0)
- tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md: frontmatter mirror synced с new lifecycle/version

K8.5 deferral cascade complete:
- DOC-D-K8_5 lifecycle accurately reflects skeleton-grade content state
- Phase A'.6 slot marked SKIPPED with deferral marker
- К10 promoted к next milestone (А'.7) per skeleton framework precedent
- Lesson #23 candidate captured (register classification drift) — Provisional pool, formalization at А'.8
- CAPA-2026-05-18-K8_5-DRIFT documents the drift surface + resolution

K8.5 canonical identifier preserved per KERNEL_ARCHITECTURE.md Part 2 (upstream authority unchanged). Promotion trigger: post-Phase B initial M-series sprint когда mod authors audience exists.

Refs: EVT-2026-05-18-K8_5-DEFERRAL, CAPA-2026-05-18-K8_5-DRIFT, DOC-D-K8_5, DOC-A-PHASE_A_PRIME_SEQUENCING, DOC-B-METHODOLOGY"
```

### 6.6 Phase 5 outcome

1 atomic commit (Phase 5 closure). 3 atomic commits total post-Phase 5. (Phases 3+4 don't produce separate commits; they bundle with Phase 5).

**Total atomic commits**: 3 (Phase 1 + Phase 2 + Phase 5 closure).

---

## 7. Phase 6 — Push к origin

### 7.1 Scope

Push branch `claude/k8-5-deferral-cascade` к origin. Crystalka manually opens PR per repository convention (Claude Code auto-mode blocks PR creation per established pattern).

### 7.2 Push command

```bash
git push -u origin claude/k8-5-deferral-cascade
```

### 7.3 Phase 6 outcome

Branch pushed. Crystalka manually opens PR.

---

## 8. Halt conditions

Per METHODOLOGY §3 stop-escalate-lock.

### 8.1 Hard gates (HG-N)

**HG-1**: Working tree dirty before Phase 0. Surface к Crystalka; resolve workspace.

**HG-2**: sync_register.ps1 --validate exit non-zero. Surface с full output.

**HG-3**: Main branch doesn't reflect brief skeleton framework PR merge (commits `15ffa0a` → `ce0791d`). Surface к Crystalka — brief depends on AUTHORED-SKELETON lifecycle existence в tooling.

### 8.2 Soft conditions (SC-N)

**SC-1**: DOC-D-K8_5 current `special_case_rationale` field contains rationale that needs preservation in updated text (verify in Phase 0). Surface and incorporate.

**SC-2**: PHASE_A_PRIME_SEQUENCING.md §4.1 invariant text «No skipping» strictly interpreted leads к contradiction with «А'.6 SKIPPED» wording. Refinement paragraph (§3.4 в this brief) explains the resolution; if Claude Code execution discovers further conflicts, surface к Crystalka.

**SC-3**: К8.5 canonical identifier upstream references in `KERNEL_ARCHITECTURE.md` Part 2 lines 605 + 767 OR `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` referenced sections found к contradict deferral wording. Verify upstream unchanged; surface если deferral wording leaks upstream к К8.5 canonical identifier description.

**SC-4**: CAPA counter / EVT counter conflicts in REGISTER.yaml (e.g., CAPA-2026-05-18-K8_5-DRIFT collides with existing entry, или EVT-2026-05-18-K8_5-DEFERRAL collides). Verify counters in Phase 0 reads.

**SC-5**: DOC-B-METHODOLOGY version bump needed (v1.8 → v1.9?) для Lesson #23 candidate addition. Provisional pool extension в-place may not require version bump (precedent: pool itself authored в v1.8); verify if any precedent suggests bump. Default lean: no bump (pool is mutable section, Lesson #23 candidate is non-breaking extension).

### 8.3 Halt artifact pattern

If halt condition fires:

```
docs/scratch/K8_5_DEFERRAL_CASCADE/HALT_REPORT.md
```

Content: halt reason, condition, findings, options, recommended action. Commit halt artifact, await Crystalka direction.

---

## 9. Closure summary

### 9.1 Expected commit count

3 atomic commits:

1. REGISTER.yaml DOC-D-K8_5 reclassification (Phase 1)
2. PHASE_A_PRIME_SEQUENCING.md amendments (Phase 2)
3. Phase 5 closure (CAPA + EVT + Lesson #23 candidate + METHODOLOGY pool extension + PENDING-COMMIT backfill + render regen + frontmatter sync)

### 9.2 Expected document state at closure

- `docs/governance/REGISTER.yaml`:
  - DOC-D-K8_5: lifecycle AUTHORED-SKELETON, version 0.1, deferral note
  - register_version 1.6 → 1.7
  - CAPA-2026-05-18-K8_5-DRIFT entry (CLOSED)
  - EVT-2026-05-18-K8_5-DEFERRAL entry
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`:
  - §2 Q-K-1 reconciliation note updated (trigger changes)
  - §2 sequence diagram: А'.6 SKIPPED, А'.7 К10
  - §3 duration table updated
  - §4.1 sequencing invariant refinement paragraph added
  - §5.2-5.6 amendment references updated
  - §6 completion gate updated
  - §7.1 К-closure approach updated
- `docs/methodology/METHODOLOGY.md`:
  - Provisional Lessons pool extended с Lesson #23 candidate
- `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md`:
  - Frontmatter mirror synced (lifecycle: AUTHORED-SKELETON, version: 0.1)
  - Body content preserved unchanged (historical record)
- `docs/governance/REGISTER_RENDER.md`: regenerated
- `docs/governance/VALIDATION_REPORT.md`: regenerated, exit 0

### 9.3 Next operational phase

After K8.5 deferral cascade lands:

1. К10 execution brief authoring — Opus deliberation session, promotes DOC-D-K10_EXECUTION AUTHORED-SKELETON → AUTHORED
2. А'.7 К10 execution — Claude Code session (substantial; multi-session expected)
3. К10 cross-doc cascade — brief authoring + execution (may bundle с К10 execution per К10 cross-doc cascade brief skeleton)
4. А'.8 K-closure deliberation + authoring + execution
5. А'.9 Roslyn analyzer brief authoring + execution
6. Phase B M-series begins
7. К8.5 promoted к AUTHORED when initial M-series sprint establishes mod authors audience

---

## 10. Lesson application discipline

During execution, apply lessons actively:

**Lesson #7** — PHASE_A_PRIME_SEQUENCING.md amendments quote existing text verbatim before replacement; don't paraphrase. Same для CAPA + EVT entry shapes (use existing entries as templates for field structure).

**Lesson #8** — Each commit produces well-formed governance state. Phase 1 commit: REGISTER valid post-reclassification. Phase 2 commit: PHASE_A_PRIME_SEQUENCING amended, REGISTER unchanged. Phase 5 commit: closure protocol bundled (all derived artifacts synced).

**Lesson #11** — K8.5 brief skeleton file itself NOT edited (Lesson #11 redundancy check applied — file content preserved as historical record; only metadata reclassified).

**Lesson #20** — Defer decision validated by **architectural reason** (audience absence) not tactical cost ("К8.5 takes too long" would be tactical and rejected). К-L14 application.

**Lesson #22** — Read register entry AND document content; reconcile. K8.5 cascade itself is the application of this lesson (drift surfaced via reconnaissance, not assumed).

---

## 11. Appendix A — Lesson #11 vs Lesson #23 candidate complement

Lesson #11 (architectural reduction methodology) was applied к exclude K8.5 from brief skeleton framework scope based on register classification (DOC-D-K8_5 lifecycle: AUTHORED). This led к correct exclusion (K8.5 NOT added к skeleton framework — Lesson #11 redundancy check successful).

**However**: register classification was inaccurate. K8.5 brief was actually skeleton-grade content. So the redundancy check protected against adding K8.5 к skeleton framework, but не surfaced that K8.5 itself needed reclassification.

**Lesson #23 candidate** complements Lesson #11: Lesson #11 prevents redundant additions based on register state; Lesson #23 ensures register state reflects reality before applying Lesson #11.

**Combined application**: before applying Lesson #11 redundancy check к exclude, verify register classification matches actual document state. If mismatch surfaces, reclassification cascade may be required (this brief is the cascade for K8.5).

---

## 12. Appendix B — K8.5 promotion path post-Phase B

When К8.5 promotion trigger fires (post-Phase B initial M-series sprint когда mod authors audience exists), expected work:

1. K8.5 brief refresh (skeleton → full AUTHORED brief authoring at Opus deliberation session, promotion DOC-D-K8_5 AUTHORED-SKELETON → AUTHORED, version 0.1 → 1.0)
2. Brief execution (Claude Code session, DOC-D-K8_5 AUTHORED → EXECUTED, version 1.0 → 1.1)
3. Scope refinement at brief authoring time — content premise updated to match actual post-Phase B reality (мayb may not include v2-to-v3 framing depending on Phase B M-series API surface; mod authoring guide structure determined by actual M-series demonstrations available к reference)
4. К-closure report (А'.8) К8.5 section addendum (if К-closure landed by then; К-closure may also be amended in similar deferral-restore pattern)

К8.5 promotion is **not** a Phase A' completion gate per §6 amendment (this brief Phase 2 §3.7). It's a Phase B follow-up milestone.

---

**End of Brief**

K8.5 deferral cascade execution brief. ~1.5-2.5 hours auto-mode. 3 atomic commits. Schema-compatible (FRAMEWORK v1.1 + AUTHORED-SKELETON lifecycle). Governance protocol full path: REGISTER amendment + downstream Tier 2 doc amendment + CAPA open+close + EVT entry + Lesson candidate addition + closure protocol gates. К-L14 + governance discipline preserved.

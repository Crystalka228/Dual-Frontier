---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-PHASE_A_PRIME_SEQUENCING
category: A
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-PHASE_A_PRIME_SEQUENCING
---
# Phase A' sequencing — K-L3.1 to M-series begins

**Authored**: 2026-05-10 (Opus, post-K8.2 v2 closure cleanup session)
**Status**: REFERENCE document for future Opus sessions and Crystalka deliberations
**Purpose**: anchor sequencing logic between K-series closure (K8.2 v2) and Phase B (M-series migration) start. Document is read at the beginning of any K-series-related session as orienting reference; it is not a brief, not LOCKED architecture, not deliberation surface.
**Authority**: Crystalka declarations 2026-05-10:
- «Без костылей у меня много времени, а также требуется архитектурная чистота, чтобы проект жил десятилетиями»
- «Тут наверно должен быть мост где что-то что можно преобразовать из классов модов в struct, а что нельзя остаётся в managed»
- «После того закончим миграцию K. Наконец можно внедрить анализатор чтобы не уронить архетектуру потом противоречиями)), но потом после отчёта и закрытия K»
- «Тут ещё один смысл что анализатор будет верификатором миграции и будет нашим дебагером на баги которые не ловят тесты»

**Relationship to other documents**: this document is **subordinate** to `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 LOCKED — that plan governs Phase A and Phase B at strategic level. This document **inserts a new structural unit (Phase A')** between Phase A closure (K8.5) and Phase B start (M8.4), reflecting Crystalka declarations made post-plan-lock. When Migration Plan v1.1 amendment ships (per K-L3.1 amendment brief), this Phase A' formalization is integrated into Migration Plan §0.1 sequence diagram + §1.x Phase A description.

---

## §1 Why Phase A' exists as separate unit

Migration Plan v1.0 §0.1 sequence diagram:

```
Phase A: K-series kernel completion       (4-8 weeks at hobby pace)
  K8.2 → K8.3 → K8.4 → K8.5
       └──────────────────────┘
                    │
                    ▼
Phase B: M-series mod-OS migration         (5-10 weeks at hobby pace)
  M8.4 → M8.5 → M8.6 → M8.7 → M9 → M10.x
```

Single arrow Phase A → Phase B. Two new milestones declared post-plan-lock 2026-05-10 don't fit either bucket:

- **K-closure report**: not a K-Lxx milestone (no architecture change, no code change beyond doc); not an M-milestone (kernel-side, before any mod migration). Closes K-series formally with historical enumeration + analyzer specification surface.
- **Architectural analyzer milestone**: not a K-Lxx milestone (kernel architecture is closed by K-closure-report; analyzer encodes it, doesn't add to it); not an M-milestone (analyzer must ship **before** M8.4 to verify it during migration).

Both belong **between** Phase A and Phase B. Naming the structural unit «Phase A'» makes this explicit rather than implicit. Migration Plan v1.0 «single arrow» becomes Plan v1.1 «sequence with Phase A' bridge».

K-L3.1 architectural decision session **also** belongs to Phase A' (it precedes the deferred K8.3-K8.5 execution). So Phase A' contains everything between K8.2 v2 closure (already DONE 2026-05-09) and M8.4 begin.

---

## §2 Full Phase A' sequence

```
[K8.2 v2 closure — DONE 2026-05-09, commits 7527d00 on main]
  │
  ├─ Phase A'.0 — K-L3.1 architectural decision session — DONE 2026-05-10
  │   Brief: K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md (EXECUTED)
  │   Addendum: K_L3_1_BRIEF_ADDENDUM_1.md (APPLIED)
  │   Output: amendment plan at /docs/architecture/K_L3_1_AMENDMENT_PLAN.md
  │   Closure commit: 45d831c on main
  │   Code changes: zero
  │   Test count: zero delta
  │
  ├─ Phase A'.0.5 — Documentation reorganization + cross-ref refresh + module-local refresh + pipeline-terminology scrub + cleanup campaign — DONE 2026-05-10
  │   Brief: A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md (EXECUTED)
  │   Scope: 36 files relocated to docs/architecture/ + docs/methodology/ + docs/reports/ per Crystalka organizational principle; ~250 stale cross-references updated to repo-rooted absolute form; 5 component READMEs deleted-stub cleanup (Phase 5); 6 Systems READMEs + 2 Events READMEs + 2 kernel-area READMEs refreshed (Phase 6); pipeline terminology mechanically scrubbed in active narratives (Phase 7) + A'.0.7 deferral markers on METHODOLOGY/PIPELINE_METRICS/MAXIMUM_ENGINEERING_REFACTOR
  │   A'.2 (was: README cleanup) folded into Phase 5 of this milestone per execution unification (single-session pipeline reality renders milestone splitting unnecessary)
  │   Closure commits: 27523ac through Phase 9 closure on main (Phase 0 brief authoring + Phases 1-9 atomic commits, ~25 total)
  │   Code changes: zero
  │   Test count: zero delta
  │   Estimated time: 2-4 hours auto-mode (matched actual)
  │
  ├─ Phase A'.0.7 — Methodology pipeline restructure rewrite — DONE 2026-05-10
  │   Scope: substantive rewrite of METHODOLOGY.md (§0/§2.1/§2.2/§3/§4/§5/§8/§9) + PIPELINE_METRICS.md (entire empirical record per-era split) + MAXIMUM_ENGINEERING_REFACTOR.md (4-agent layout/hardware-variance/pipeline-mapping sub-sections) for new 2-agent pipeline shape per Crystalka direction 2026-05-10
  │   Q-A07-1..12 deliberation locks recorded; Q-A07-6 audience contract «agent-as-primary-reader» surfaced
  │   Closure commit: 9d4da64 on main; methodology corpus v1.5→v1.6 / v0.1→v0.2 / v1.0→v1.1
  │   Code changes: zero
  │   Test count: zero delta
  │
  ├─ Phase A'.1 — Amendment brief execution — DONE 2026-05-10
  │   Two parallel landings:
  │   - A'.1.K: K-L3.1 amendment plan landed (KERNEL v1.3→v1.5, MOD_OS v1.6→v1.7, MIGRATION_PLAN v1.0→v1.1); closure commit 0789bd4
  │   - A'.1.M: A'.0.7 methodology rewrite landed; closure commit 9d4da64
  │   Code changes: zero
  │
  ├─ Phase A'.2 — REMOVED (folded into A'.0.5 Phase 5 per execution unification 2026-05-10)
  │
  ├─ Phase A'.3 — Push to origin — DONE 2026-05-10
  │   ~25 commits drained from local-only state through 38c2e19 on origin/main
  │   No code changes
  │
  ├─ Phase A'.4 — K9 field storage execution — DONE 2026-05-11
  │   RawTileField C++ core + IModApi v3 Fields surface + CPU IsotropicDiffusionKernel reference
  │   27 bridge tests + 8 selftest scenarios added (21→29 total native)
  │   FIELDS.md Draft → Live
  │   Commits: ce4dba8..80c9ba6; closure entry in MIGRATION_PROGRESS K9 section
  │   K9 lessons learned recorded (7 items)
  │
  ├─ Phase A'.4.5 — Document Control Register — DONE 2026-05-12
  │   Brief: A_PRIME_4_5_DOCUMENT_CONTROL_REGISTER_BRIEF.md (deliberation-mode); deliberation closure document (23 Q-locks + 39 production entries); 5 Pass execution-context briefs (Pass 1-5)
  │   Scope: synthesized governance framework from DO-178C / ISO 9001 / ISO 26262 / IEC 61508 / FDA 21 CFR Part 11 (9 selected + 11 deselected elements); REGISTER.yaml schema v1.0 + 229 documents enrolled; 3 PowerShell tooling scripts (sync_register/query_register/render_register); 4 new folders (docs/governance, docs/ideas, docs/mechanics, tools/governance); METHODOLOGY v1.6 → v1.7 (§12 register integration + §7.1 7th invocation)
  │   Deliverables: FRAMEWORK.md v1.0 LOCKED + SYNTHESIS_RATIONALE.md v1.0 LOCKED + REGISTER.yaml (Tier 2 Live; schema LOCKED) + REGISTER_RENDER.md + BYPASS_LOG.md + VALIDATION_REPORT.md + SCOPE_EXCLUSIONS.yaml + tools/governance/MODULE.md
  │   Validation clean: 229 documents enrolled, 13 REQ + 14 RISK + 3 CAPA + 9 EVT, 0 errors, 0 warnings; per-category A=30 B=6 C=3 D=48 E=54 F=78 G=8 H=2 I=0 J=0
  │   Code changes: zero (governance + docs + tooling only)
  │   Executor: Claude Code session ~5-7 hours auto-mode
  │
  ├─ Phase A'.5 — K8.3 skeleton execution
  │   Brief: K8.3 skeleton authored, post-Phase-A'.1 amendment if disposition B/C applied
  │   Scope: 31 production systems → SpanLease/WriteBatch (per Migration Plan §1.2 reformulation)
  │   System access patterns reflect Q3 K-L3.1 lock
  │   Estimated time: 4-6 weeks hobby pace, ~12-20 hours auto-mode
  │   Executor: Cloud Code
  │
  ├─ Phase A'.6 — K8.4 skeleton execution
  │   Brief: K8.4 skeleton authored, post-Phase-A'.1 amendment if disposition B/C applied
  │   Scope: managed World retired; Mod API v3 ships; [ModAccessible] propagation
  │   Mod API v3 surface reflects Q1+Q2+Q6 K-L3.1 lock (storage path declaration, where it lives, capability extension)
  │   Estimated time: 1-2 weeks hobby pace, ~4-8 hours auto-mode
  │   Executor: Cloud Code
  │
  ├─ Phase A'.7 — K8.5 skeleton execution
  │   Brief: K8.5 skeleton authored, post-Phase-A'.1 amendment if disposition B/C applied
  │   Scope: Phase A → B handoff (docs, capability annotations, readiness gate)
  │   K8.5 is the natural place for MOD_OS_ARCHITECTURE.md v1.7 amendment per Migration Plan §6.2
  │   Estimated time: 3-5 days hobby pace, ~1-2 hours auto-mode
  │   Executor: Cloud Code
  │
  ├─ Phase A'.8 — K-closure report
  │   Scope: structured document enumerating final K-Lxx invariants
  │   Dual purpose: (1) historical record of K-series, (2) formal analyzer rule specification surface
  │   Format: each invariant has formal statement + violation example + compliance example
  │   Estimated time: 1-2 sessions chat + 30-60 min auto-mode for amendments to MIGRATION_PROGRESS
  │   Executor: Opus (deliberation) + Cloud Code (commit + tracker update)
  │
  ├─ Phase A'.9 — Architectural analyzer milestone
  │   Scope: Roslyn analyzer encoding K-Lxx invariants per K-closure report
  │   Dual purpose: (1) M-series migration verifier (catches drift in M-milestone migrations that tests don't see), (2) architectural debugger (surfaces bugs that compile + pass tests but violate invariants)
  │   First run on existing post-K8.5 codebase may surface pre-analyzer debt — fix budget included in milestone scope
  │   M3.4 capability analyzer merge decision: at analyzer brief authoring time
  │   Track B activation candidate per ROADMAP «Maximum Engineering Refactor» §96-114
  │   Estimated time: 2-4 weeks hobby pace, undetermined auto-mode (depends on rule scope)
  │   Executor: Opus (architectural deliberation, rule spec) + Cloud Code (Roslyn implementation)
  │
  └─ [M8.4 begins — Phase B]
      Vanilla.World migration. First M-milestone runs under analyzer protection.
```

---

## §3 Cumulative Phase A' duration estimate

At hobby pace (~1h/day):

| Phase | Estimate | Cumulative |
|---|---|---|
| A'.0 K-L3.1 (DONE 2026-05-10) | 2-4 hours session | <1 day |
| A'.0.5 Documentation reorganization (DONE 2026-05-10) | 2-4 hours auto-mode | <1 day |
| A'.0.7 Methodology rewrite (DONE 2026-05-10) | 1-2 hours session + auto-mode | <1 day |
| A'.1 Amendment (DONE 2026-05-10) | 30-60 min auto-mode | ~1 day |
| A'.2 (REMOVED — folded into A'.0.5) | — | — |
| A'.3 Push (DONE 2026-05-10) | minutes | ~1 day |
| A'.4 K9 (DONE 2026-05-11) | 1-2 weeks | 2-3 weeks |
| A'.4.5 Document Control Register (DONE 2026-05-12) | ~5-7 hours auto-mode | ~3 weeks |
| A'.5 K8.3 | 4-6 weeks | 6-9 weeks |
| A'.6 K8.4 | 1-2 weeks | 7-11 weeks |
| A'.7 K8.5 | 3-5 days | 8-12 weeks |
| A'.8 K-closure report | 1-2 sessions | 8-12 weeks (overlap-able) |
| A'.9 Analyzer | 2-4 weeks | 10-16 weeks |

**Total Phase A' duration: ~10-16 weeks at hobby pace** (~2.5-4 months). The A'.0/A'.0.5/A'.0.7 doc-only deliberation+refresh sessions are sub-day each and do not materially extend cumulative timeline.

This duration reflects «без костылей + decade-horizon» commitment. Faster paths exist (skip K-L3.1, skip closure report, skip analyzer) but each shortcut creates structural debt this phase exists to prevent.

---

## §4 Phase A' invariants

Three invariants govern Phase A' execution:

### §4.1 Sequencing is total, not partial-ordering

Each Phase A'.N depends on A'.<N-1> closure. No skipping. No parallelism within A' (except possibly A'.8 K-closure report drafting in parallel with A'.7 K8.5 execution if K8.5 deliverables are stable enough to enumerate).

Reasoning: each milestone produces input to the next. K-L3.1 → amendment plan → amended skeleton briefs → executable kernel migrations → closure report enumerating all final state → analyzer encoding closure report. Any reorder breaks the input chain.

**Exception**: Phase A'.4 K9 may be re-ordered relative to A'.5-A'.7 (K8.3-K8.5) per Migration Plan Option c sequencing logic — K9 is kernel-side independent of K8.3-K8.5 except in IModApi v3 surface which K8.4 ships. The ordering above places K9 first as default, but if K-L3.1 deliberation surfaces reasons to defer K9 (e.g., K9 skeleton requires significant amendment vs K8.3-K8.5 minimal amendment), order may be K8.3 → K8.4 → K8.5 → K9 → K-closure report → analyzer. Decision at A'.1 amendment brief time.

### §4.2 No M-series work during Phase A'

Vanilla mod skeletons (already DONE in M8.1, commit `cafedcf`) are **untouched** throughout Phase A'. Vanilla mod assemblies remain empty. No file movements from `src/` to `mods/`. No registration code in vanilla mod `Initialize` methods.

Reasoning: M-series work depends on stable post-K8.5 + post-analyzer state. M-series work during Phase A' would create rework when analyzer fires post-A'.9.

### §4.3 No new architectural decisions outside K-L3.1 scope

K-L3.1 is the **only** architectural decision session in Phase A'. All other phases are execution against the K-L3.1 lock + Migration Plan. Architectural surprises during execution invoke METHODOLOGY §3 «stop, escalate, lock» — not improvised resolution.

Specifically during analyzer milestone (A'.9): analyzer rules **encode** K-closure report invariants; they do not **add** new invariants. If analyzer authoring surfaces an invariant not enumerated in closure report, it's an architectural decision for separate session, not analyzer scope expansion.

---

## §5 Document amendments triggered by Phase A'

Multiple LOCKED documents accumulate amendments through Phase A'. Coordinated amendment plan tracked here (synthesized from K-L3.1 brief §5 + downstream phases):

### §5.1 KERNEL_ARCHITECTURE.md

- **A'.1 amendment**: v1.4 → v1.5 (K-L3 reformulation per K-L3.1 lock). May bump v1.4 → v2.0 if K-L3.1 synthesis form §4.B introduces K-L12 (semantic change).
- **A'.4-A'.7 incremental amendments**: Part 2 K8.x rows updated as each milestone closes (existing precedent).
- **A'.8 K-closure report**: KERNEL_ARCHITECTURE either receives final consolidating amendment, or K-closure report is authored as separate document referencing KERNEL_ARCHITECTURE invariants. Decision at A'.8 deliberation.

### §5.2 MOD_OS_ARCHITECTURE.md

- **A'.1 amendment**: v1.6 → v1.7 (lines 1149-1150 reformulation per K-L3.1 lock + Q6 capability extension). May bump to v2.0 if K-L3.1 introduces structurally significant change (e.g., separate ManagedAccessible attribute per Q6.b).
- **A'.6 K8.4 amendment**: §4.6 IModApi v3 surface finalized per K-L3.1 Q1+Q2 lock.
- **A'.7 K8.5 amendment**: §11 migration phases table extended with M8.4-M10.B rows per Migration Plan §6.2 schedule.

### §5.3 MIGRATION_PLAN_KERNEL_TO_VANILLA.md

- **A'.1 amendment**: v1.0 → v1.1 (decision #9 added per K-L3.1 lock; line 62 framing corrected; sections 1.2/1.3/1.4 scope wording updated per K-L3.1 Q3+Q1+Q2 answers; **Phase A' formalization integrated into §0.1 sequence**).
- **A'.4-A'.7**: per-milestone scope clarifications as needed.

### §5.4 METHODOLOGY.md

- **A'.0 K-L3.1**: possibly new lesson «architectural decisions get formal recording before execution» (analog of K8.0 precedent). Decision at K-L3.1 closure.
- **A'.0.5 (DONE)**: A'.0.7 deferral marker added at top of file noting which substantive sections require A'.0.7 rewrite. No content modified.
- **A'.0.7**: substantive rewrite of §0 Abstract, §2.1 Role distribution, §2.2 Contracts as IPC, §3 Pipeline economics, §4 Empirical results, §5 Threat model, §8 Reproducibility for new 2-agent pipeline. Possibly new lesson «pipeline restructure as architectural deliberation» (analog of K-L3.1). Decision at A'.0.7 closure.
- **A'.8 K-closure report authoring**: may produce new methodology lesson «closure reports as analyzer specification surface». Decision at A'.8.

### §5.5 MIGRATION_PROGRESS.md

- **A'.0 K-L3.1 closure entry** (filled at K-L3.1 amendment plan execution per A'.1)
- **A'.0.5 closure entry** (this milestone, recorded in Phase 9 closure commits)
- **A'.0.7 closure entry** (after A'.0.7 milestone executes)
- **A'.1 amendment closure entry**
- **A'.2 — N/A (folded into A'.0.5)**
- **A'.4-A'.7** per-milestone closure entries (existing precedent, K8.1/K8.1.1/K-Lessons shape)
- **A'.8 K-closure report entry** (special — closure of closure)
- **A'.9 analyzer milestone closure entry**

### §5.6 ROADMAP.md

ROADMAP currently «Updated 2026-05-03» (stale; M8.0/M8.1/M8.2 closures and K0-K8.2v2 closures not reflected). Phase A' is the natural occasion for full ROADMAP refresh — this is **not** scheduled here as separate milestone but as **A'.7 K8.5 housekeeping** or **A'.8 K-closure report** companion update.

### §5.7 New documents Phase A' may produce

- **K-closure report** (A'.8) — likely at `docs/architecture/K_SERIES_CLOSURE_REPORT.md` or analog
- **Analyzer specification document** (A'.9 input) — likely at `docs/architecture/ARCHITECTURAL_ANALYZER_SPEC.md` or analog
- **Possibly: MOD_OS_ARCHITECTURE.md v2.0** if K-L3.1 synthesis is structurally significant — may warrant fresh document version rather than continued v1.x corrections

---

## §6 Phase A' completion gate

Before Phase B (M8.4) begins, the following are TRUE:

- [ ] K-L3.1 decision LOCKED with amendment plan executed
- [ ] All 4 LOCKED docs synced to post-K-L3.1 architecture (KERNEL/MOD_OS/MIGRATION_PLAN/MIGRATION_PROGRESS)
- [ ] All 4 skeleton briefs (K9, K8.3, K8.4, K8.5) reviewed for post-K-L3.1 consistency, amended per disposition, executed successfully
- [ ] K-series fully closed: 31 components struct-or-explicit-managed-bridge per K-L3.1 lock; 31+3 systems migrated to SpanLease/WriteBatch (or fewer per K8.3 scope amendment); managed World retired as production path; capability annotation pass complete; M-series migration guide published
- [ ] K-closure report authored, MIGRATION_PROGRESS reflects K-series end state with formal invariant enumeration
- [ ] Architectural analyzer milestone executed: rules encoding K-closure report invariants; first run on existing codebase passes (any surfaced debt fixed during analyzer milestone scope); CI integration active
- [ ] Test baseline holds (specific count TBD, plausibly 600-900 per Migration Plan §1.5)
- [ ] No uncommitted work, no orphan branches, working tree clean
- [ ] origin synced

**On all checks passing**: M8.4 brief authoring begins. M-series proceeds under analyzer protection per Migration Plan §3 sequence.

---

## §7 Open questions for Phase A' execution

These surface as candidates for K-L3.1 deliberation or per-phase brief authoring:

### §7.1 — K-closure report authoring approach

- **(a) Single chat session** with Crystalka deliberation, output drafted in session
- **(b) Multi-session iterative** — Opus drafts, Crystalka reviews per K-Lxx, iterates
- **(c) Skeleton-first** — skeleton authored late in K8.5, populated incrementally during A'.4-A'.7, finalized in dedicated session

Default lean: **(c) skeleton-first**. Each milestone (K9, K8.3, K8.4, K8.5) populates its section as it closes; A'.8 finalizes.

### §7.2 — Analyzer milestone scope contraction

K-Lxx alone is ~11 invariants (K-L1 through K-L11 plus possible K-L12 from K-L3.1). Each may decompose to multiple analyzer rules. Scope risk: 30-50 rules → milestone explosion.

Mitigation candidates:
- **(a) Tiered rules**: critical rules ship in A'.9, optional/advisory rules deferred
- **(b) Rule prioritization**: M-series-critical rules first (those catching M-migration drift); steady-state architectural enforcement rules deferred
- **(c) Skeleton analyzer**: A'.9 ships analyzer infrastructure + 5-10 most critical rules; remaining rules added as separate post-A'.9 micro-milestones, each rule one commit

Default lean: **(b)** for M-series-criticality + **(c)** for infrastructure pattern.

### §7.3 — Pre-existing debt fix budget within A'.9

Analyzer first run may surface debt. Two scopes:
- **Tight**: only debt that violates rules of M-series-critical category (Q-A1 above) gets fixed; other debt fixed in steady-state post-A'.9
- **Comprehensive**: all surfaced debt fixed within A'.9 milestone, even if rules are advisory tier

Default lean: **tight** (per «без костылей» — fix only what blocks Phase B; document remaining for post-A'.9 cleanup tier).

### §7.4 — Push to origin frequency during Phase A'

Currently 25 commits ahead origin (per K8.2 v2 closure). Push deferred to A'.3. Question: push at every A'.N closure, or single batched push at end of A'?

- **Per-phase push**: visibility on remote, smaller revertibility scopes
- **Batched final push**: single coherent «Phase A' complete» pushable unit

Default lean: **per-phase push** post-A'.3 (so A'.3 is the single «backlog drain» push, then A'.4 onward pushes per closure). Matches existing M-phase precedent.

---

## §8 What this document is not

- **Not a brief**: this is reference, not executable.
- **Not LOCKED architecture**: Migration Plan v1.0 is LOCKED; this document inserts a new structural unit subordinate to it. When Migration Plan v1.1 ships, Phase A' is integrated upstream and this document becomes superseded.
- **Not exhaustive**: per-phase briefs are authored at phase activation; this document anchors sequencing, not phase contents.
- **Not Crystalka deliberation surface**: open questions in §7 surface for future deliberation; they are not «need-Crystalka-now» items.
- **Not commit-required artifact**: stays at `/mnt/user-data/outputs/` as session reference; if Crystalka wants it in repo, it lives at `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` or analog. Repo placement is post-K-L3.1 decision.

---

## §9 Provenance

- **Authored**: 2026-05-10, immediately after K-L3.1 brief + addendum + Crystalka analyzer-as-verifier clarification
- **Triggers**: four Crystalka declarations (per §«Authority» above), structural insufficiency of Migration Plan v1.0 §0.1 «single arrow» between Phase A and Phase B
- **Authority status**: subordinate to Migration Plan v1.0 LOCKED; integrates upstream into Migration Plan v1.1 via K-L3.1 amendment brief
- **Memory tracker**: `userMemories` 2026-05-10 entries 2-4 (K-L3 Bridge concept, skeleton state, analyzer milestone with dual purpose)
- **Companion documents**: `K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`, `K_L3_1_BRIEF_ADDENDUM_1.md` (both at `/mnt/user-data/outputs/`)

---

**Document end. Read at session start to orient on Phase A' state. Update via amendment when Migration Plan v1.1 integrates Phase A' upstream.**

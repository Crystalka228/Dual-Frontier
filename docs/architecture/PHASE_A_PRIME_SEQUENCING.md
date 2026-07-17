---
register_id: DOC-A-PHASE_A_PRIME_SEQUENCING
project: Dual Frontier
category: A
tier: 2
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-06-12
last_modified: 2026-06-12
content_language: en
next_review_due: 2026-Q3
title: Phase A' sequencing — K-L3.1 to M-series begins
last_modified_commit: f2e6df2
review_cadence: on-closure+quarterly
last_review_date: 2026-05-24
last_review_event: "A'.9.0 Reconnaissance / К-extensions cascade #4 β1+β2 (bundled per brief Q-J-8 squashing allowance) — A'.9.0 RECONNAISSANCE entry added with full cascade narrative (designation, execution branch main per Crystalka ratification, scope, 7 reconnaissance domains covered, Phase 0 anomalies surfaced — pre-existing ANALYZER_RULES.md + A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md AUTHORED-SKELETONs, Lessons surfaced including #N14 third application HIGH promotion now + #N13 second application + observational reconnaissance evidence type formalized, К-L14 verification #13 first observational reconnaissance evidence, 8 atomic commits enumeration). Phase A'.9 milestone block expanded к multi-cascade decomposition (A'.9.0 reconnaissance + A'.9.1 analyzer infrastructure + A'.9.2 severity promotion + A'.9.3+ DC###/DL### rules + M3.4 deferred per report §10 prerequisite 7). Phase post-A'.9 К-L20 Mod API lock cascade entry added (forward sequencing с DF020 family activation reference). Prior context: A'.8 К-series formal closure (Commit 4) — А'.8 entry expanded recording Q-N deliberation Session 2 Day 2 outcomes (10/10 Q-N LOCKED)."
reviewer: Crystalka
special_case_rationale: 'Category A + Tier 2 + Live override: document is mutable per phase closure, subordinate to MIGRATION_PLAN_KERNEL_TO_VANILLA — not LOCKED architecture per Pass 2 §1.3'
---

# Phase A' sequencing — K-L3.1 to M-series begins

> **Document class: planning-record.** Forward state authority: [docs/ROADMAP.md](../ROADMAP.md). This document is not a roadmap.

**Authored**: 2026-05-10 (Opus, post-K8.2 v2 closure cleanup session)
**Status**: REFERENCE document for future Opus sessions and Crystalka deliberations — current phase/milestone state is authoritative in [docs/ROADMAP.md](../ROADMAP.md), not here
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

> **Q-K-1 reconciliation note (added 2026-05-16, composite namespace ratification cascade):**
>
> **K8.5 is the canonical kernel-series milestone identifier** per `KERNEL_ARCHITECTURE.md` Part 2 master plan (lines 605 + 767): «Mod ecosystem migration prep (documentation + migration guide)». A'-cycle sequencing labels A'.6 and A'.7 below are **sequencing pointers** to K8.5 as the milestone being executed within their phase — they are not **alternate identities** for K8.5.
>
> The A'.5 closure note (below) records post-K8.3+K8.4-absorption renumbering intent: «A'.6 = K8.5 (mod ecosystem prep), A'.7 = Roslyn analyzer». The body subsections for A'.6 / A'.7 / A'.8 / A'.9, and the §3 duration estimate table, retain **pre-renumbering structure**: A'.5=K8.3, A'.6=K8.4 (absorbed), A'.7=K8.5, A'.8=K-closure report, A'.9=architectural analyzer milestone. Both A'.6 and A'.7 body subsections describe «K8.5 skeleton execution» — a duplicate-pointer artifact of the partial renumbering.
>
> Resolution of the A'-cycle renumbering question (whether to propagate A'.6 = K8.5 + A'.7 = analyzer through body/§3 table, or to leave the closure note's intent unimplemented) is a downstream **sequencing-label concern**, deferred to subsequent deliberation when K8.5 brief authoring approaches. K8.5's canonical identity is unaffected by the renumbering question.
>
> See `docs/scratch/RATIFICATION_EXECUTION/Q_K_1_REPORT.md` for full verbatim findings and recommendation match analysis. Q-K-1 retroactive lock pending subsequent deliberation per `docs/architecture/COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` §5.
>
> **Update 2026-05-18 (K8.5 deferral cascade):** K8.5 brief authoring does not approach within Phase A' execution. K8.5 deferred к post-Phase B initial M-series sprint when mod authors audience exists (per Crystalka direction 2026-05-18, surfaced via skeleton drift detection — actual brief content remains skeleton-grade, premise of v2-to-v3 mod authors migration mismatches reality given vanilla mods deferred к Phase B per composite namespace ratification PR #34). Phase A' execution sequence revised: А'.5 (CLOSED) → А'.6 SKIPPED (deferral marker) → А'.7 К10 (next) → А'.8 K-closure → А'.9 Roslyn analyzer → [M8.4 Phase B begins]. Q-K-1 retroactive lock trigger updated: was «K8.5 brief authoring approaches»; is now «post-Phase B initial M-series sprint when mod authors audience exists». Q-K-1 reconciliation remains pending until that trigger.

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
  ├─ Phase A'.5 — K8.3+K8.4 combined storage cutover — DONE 2026-05-14
  │   Brief: tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md (v2.0 supersedes v1.0 + patch v1)
  │   Scope: 10 production systems migrated to NativeWorld AcquireSpan/BeginBatch (12 → 10: ElectricGridSystem + ConverterSystem deleted as disposable vanilla CPU systems, electricity deferred to GPU compute brief); managed World retired from production as ManagedTestWorld test fixture; runtime isolation guard removed (compile-time [SystemAccess] + future A'.9 analyzer); Mod API v3 closed (RegisterComponent + RegisterManagedComponent + Fields + ComputePipelines); manifestVersion strict v3-only parser. Storage backend is binary, not divisible — atomic single-commit cutover (brief v2.0 §1.4 + Lesson #8).
  │   Outcome: 4 commits (24e5f56 revert + 54c6658 cutover + 2 closure commits)
  │   3 halts protected the milestone (storage-location, API-surface, mid-transition drift) — methodology working as designed
  │   K8.3 and K8.4 absorbed into A'.5; A'.6 = K8.5 (mod ecosystem prep), A'.7 = Roslyn analyzer; horizon item: «electricity on GPU compute» (separate future brief, not yet sequenced)
  │
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
  ├─ Phase A'.7.x — BUS_ARCHITECTURE_AMENDMENT (К-extensions cascade #0, CLOSED 2026-05-21)
  │   Brief: A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md (EXECUTED)
  │   Designation: «К-extensions cascade #0» per Q-N-7X-12 — chronologically pre-A'.8 closure,
  │   architecturally a К-extension that adds К-L15.1 to the К-series surface.
  │   Scope: К-L15.1 «Three-tier independence» AUTHORED → LOCKED (2-layer sub-invariant к К-L15:
  │   per-tier state isolation + per-tier mutex runtime isolation + cross-tier re-entrant publish
  │   safe). Investigation atomization (β1-β7) + Bug #1 (BusFacade coalesceKey overload) + Bug #2
  │   (DrainBackgroundBatch wiring) + Bug #3 (O(N²) → O(N) coalesce) + Bug #4 (single-mutex
  │   contention closed via per-tier state split) + Group B cross-test pollution (Dispose
  │   extension) + governance (METHODOLOGY §12.7 Modding suite verification gate per
  │   CAPA-K10_3-V2-SOFT-HALT step (c)). KERNEL_ARCHITECTURE.md v2.3 → v2.4, METHODOLOGY.md
  │   v1.8 → v1.9, 5 CAPAs + EVT-2026-05-21-A_PRIME_7_X-CLOSURE.
  │   Cumulative К-Lxx series: 20 → 21 (К-L15.1 added; К-L15 stays AUTHORED candidate until A'.8
  │   per К-L7.1 precedent).
  │   Commits: 13 atomic on claude/scheduler-stress-test-KmVM3 (b59ab2d..PENDING-COMMIT-A_PRIME_7_X-CLOSURE)
  │   Executor: Claude Opus 4.7 (deliberation) + Claude Code (execution).
  │
  ├─ Phase A'.7.5 — BUS_SOURCE_SPLIT (К-extensions cascade #1 sub-milestone, CLOSED 2026-05-22)
  │   Brief: A_PRIME_7_5_BUS_SOURCE_SPLIT_BRIEF.md (EXECUTED)
  │   Designation: «К-extensions cascade #1 sub-milestone» per A'.7.x EVT closure
  │   governance_impact §16.5 deferral note. Pure source-level reorganization
  │   implementing К-L15.1 compile-time layer; К-L15.1 invariant text UNCHANGED
  │   (parent A'.7.x γ4 LOCKED 2-layer formulation: state + runtime; A'.7.5 ε2
  │   lands 3rd layer compile-time per source-level concern separation).
  │   Scope: bus_native.cpp (240 lines, 15 df_bus_* C ABI functions) → bus_common.cpp
  │   (cross-tier accessors + clear + unsubscribe) + bus_fast.cpp + bus_normal.cpp +
  │   bus_background.cpp (per-tier C ABI). background_queue.cpp preserved distinct
  │   (К10.2 Item 26/30 separation per gap audit G-2 layer-collapse note). Helper
  │   primitives migrated к bus_native_internal.h (ε1). Stale O(N²) + bus_native.cpp
  │   reference comments cleaned up (ε3).
  │   К-L impact: NONE — К-L15.1 invariant text preserved verbatim; compile-time layer
  │   materialization adds implementation depth without amendment. Cumulative К-Lxx
  │   series stays 21 (no new sub-invariants).
  │   К-L14 verification #9 clean cascade — empirical baseline preserved (97 selftest
  │   ALL PASSED; Core.Tests.Bus 20/20 PASS; pre-existing test pollution failures
  │   verified pre-existing on ε1 state, NOT caused by source split).
  │   Commits: 5 atomic on feature/a_prime_7_5-bus-source-split (c1d10b0..PENDING-COMMIT-A_PRIME_7_5-CLOSURE)
  │   Executor: Claude Opus 4.7 (brief authoring + execution) + Crystalka (Q-N ratification).
  │
  ├─ Phase A'.8 — К-SERIES FORMAL CLOSURE (Phase A' formal closure event, CLOSED 2026-05-23)
  │   Brief: K_CLOSURE_AUTHORING_BRIEF.md (RATIFIED 2026-05-23 → EXECUTED at Commit 6)
  │   Designation: К-series formal closure event. Phase A' formal closure event boundary.
  │   Scope: K_CLOSURE_REPORT.md authored Tier 1 AUTHORED Category A (per Q-N-8-4 amendment к
  │   Meta-Q1 Session 1 LOCKED — initial AUTHORED; LOCKED transition deferred к downstream review)
  │   с 12 sections per Q-N-8-8 LOCKED (S-LOCK-3): exec summary + К-L14 thesis canonical text +
  │   К-Lxx 21 invariants enumeration + К-L14 evidence baseline 9 verifications + cascades closure
  │   summaries + V substrate progress + Lessons promotion + Roslyn analyzer rule specs + Phase A'
  │   chronology + forward sequencing + hypothesis tracking + cross-reference integrity + closure
  │   metrics. KERNEL_ARCHITECTURE.md v2.4 → v2.5 (8 К-L LOCK batch per Q-N-8-1: К-L7.1 + К-L12-L18;
  │   К-L14 abbreviated row + cross-reference к K_CLOSURE_REPORT.md §1.2 per Q-N-8-2 hybrid (c)).
  │   METHODOLOGY.md v1.9 → v1.10 (12 FORMALIZE + 9 DEFER + 1 SUNSET Lessons batch per Q-N-8-5).
  │   REGISTER.yaml register_version 2.2 → 2.3 (3 new DOC enrollments: DOC-A-K_CLOSURE_REPORT,
  │   DOC-A-ANALYZER_RULES skeleton, DOC-A-K_L14_EVIDENCE_DASHBOARD skeleton; EVT-2026-05-23-A_PRIME_8_K_CLOSURE-RATIFICATION).
  │   К-L14 falsifiability criterion 6 (soft-halt rate exceeds X%) introduced as Provisional per
  │   Q-N-8-7 LOCKED (threshold TBD pending 2nd soft-halt observation).
  │   К-extensions designation operationalized per Q-N-8-10 LOCKED: cascade #0 (А'.7.x) + #1
  │   (А'.7.5) closed pre-closure architecturally as К-extensions; cascade #2 (Godot removal)
  │   deferred к post-closure per Q-N-8-6 LOCKED.
  │   Cumulative К-Lxx series: 21 invariants final (К-L1..L19 + К-L6 SUPERSEDED + 3 subs
  │   К-L3.1/L7.1/L15.1; К-L20 reserved post-Mod API lock).
  │   К-L14 evidence baseline: 9 verifications (8 clean + 1 honest soft-halt annotation
  │   verification #7 К10.3 v2 retroactively closed by А'.7.x).
  │   Commits: 6 atomic on feature/a_prime_8-k-closure-report (per Q-N-8-9 LOCKED S-LOCK-8).
  │   К-closure event boundary: K_CLOSURE_REPORT.md AUTHORED ratification commit (Commit 1) push
  │   к origin/main = formal К-series closure timestamp (per Q-N-8-4 amendment к Q8 LOCKED Session 1).
  │   Q-N deliberation: 10/10 Q-N LOCKED Session 2 Day 2 (Q-N-8-1 through Q-N-8-10; §8.4 order
  │   B-class → A-class → C-class).
  │   Executor: Claude Opus 4.7 (brief authoring + Q-N deliberation) + Claude Code (execution) +
  │   Crystalka (Q-N ratification).
  │
  ├─ Phase post-A'.8 — К-extensions cascade #2 — GODOT FULL DEPRECATION + LAUNCHER FORMALIZATION (CLOSED 2026-05-23)
  │   Brief: tools/briefs/K_EXT_2_GODOT_DEPRECATION_BRIEF.md (AUTHORED → EXECUTED)
  │   Designation: «К-extensions cascade #2» per A'.8 К-closure Q-N-8-11 forward sequencing
  │   + Q-N-8-6 LOCKED post-closure deferral. Original Godot branch
  │   `claude/godot-removal-deliberation-Vfg2R` (2ba8130, single commit) discarded as
  │   obsolete precursor (S-LOCK-1) — clean redo on current main (9ea5dbe).
  │   Scope (per Q-G-0 Option X single large cascade):
  │   - Phase α: Physical purge — DualFrontier.Presentation.Native removed (5 files); tracked
  │     DualFrontier.Presentation removed (32 files including project.godot + Nodes/UI/addons +
  │     Scenes/main.tscn); Kenney assets rescued via git mv к assets/kenney/ (12 files).
  │   - Phase β: Documentation cleanup tiered (Q-G-10) — Tier 1 mandatory 16 Application/* files
  │     (brief expected 8; expanded к include Commands/README + SimStateLayer + DevKitOnlyAttribute
  │     surfacing Presentation.Native textual references); Tier 2 mandatory 6 active arch docs
  │     (ARCHITECTURE, ECS, ISOLATION, THREADING, KERNEL, VULKAN_SUBSTRATE v1.1→v1.1.1 patch);
  │     Tier 3/4 not triggered; β5 historical archives (GODOT_INTEGRATION + VISUAL_ENGINE
  │     closure addendum).
  │   - Phase γ: Architectural decisions ratified (Q-G-1 IDevKitRenderer dormant; Q-G-2
  │     Presentation.Native removed; Q-G-3 IRenderCommand pure marker — Execute() stripped
  │     from 6 Command records; Q-G-5 no new К-L + Lesson #N12 candidate captured).
  │   - Phase δ: Launcher project scaffold (DualFrontier.Launcher infrastructure-only per
  │     Q-G-6 (b1) с Defensive Reserved Stub dispatcher — Lesson #N12 first application;
  │     brief amendment Crystalka Option A mid-cascade — Program.cs adapted к existing
  │     GameLoop self-ticking background-thread architecture; Application.csproj
  │     InternalsVisibleTo Presentation → Launcher).
  │   - Phase ε: Governance cascade (KERNEL v2.5 → v2.5.1 patch bump + versioning convention
  │     codified Q-G-12; METHODOLOGY v1.10 → v1.11 Lesson #N12 + #25 refined; new
  │     K_EXTENSIONS_LEDGER.md companion artifact per Q-G-11; REGISTER 3-commit cascade
  │     hybrid per Q-G-13; brief AUTHORED → EXECUTED + closure section per Q-G-14
  │     verification protocol).
  │   К-L impact: zero (К-L count unchanged: 21 final).
  │   Lessons surfaced:
  │     • Lesson #N12 (Provisional, NEW): «Defensive Reserved Stub Pattern» — paired discipline
  │       с Lesson #25 refined; first application captured cascade #2.
  │     • Lesson #25 refined per Crystalka 2026-05-23 framing: lying-test prevention principle
  │       («не тестировать пустые интерфейсы и пустую реализацию которые врут в тестах»).
  │     • Lesson #14 PROMOTED third application (Godot deprecation arc completion як pre-existing
  │     • drift cleanup на separate branch claude/k-ext-2-godot-deprecation).
  │   К-L14 verification #11 (first removal-type evidence per Q-G-14 honest-framed protocol):
  │     • Substrate (DualFrontier.Runtime) primitives unchanged through removal of dead consumer
  │     • scaffold (Presentation.Native + Presentation) + addition of new consumer (Launcher).
  │     • К-L14 thesis preservation: substrate exhibits stability across consumer churn.
  │   Cumulative К-Lxx series: 21 invariants (unchanged from A'.8 closure).
  │   Commits: cascade atomic on claude/k-ext-2-godot-deprecation branch (α0 + α1 + α1.5 +
  │   β1 + β2 + β5 + δ1 + δ3 + ε1-ε7 = ~16 commits).
  │   Executor: Claude Opus 4.7 (deliberation + brief authoring + execution) + Crystalka
  │   (Q-N ratification + mid-cascade Option A amendment ratification).
  │
  ├─ Phase post-A'.8 — К-extensions cascade #3 — LAUNCHER VISUAL IMPLEMENTATION (CLOSED 2026-05-23)
  │   Brief: tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md (AUTHORED → EXECUTED)
  │   Designation: «К-extensions cascade #3» per cascade #2 closure forward sequencing
  │   (cascade #2 §4 forward roadmap + Crystalka direction «после исполнения в сесcии claude code
  │   я приложу отчёт и мы продолжим уже делать второй»). Executed on new branch
  │   `claude/k-ext-3-launcher-visual` off cascade #2 closure merge к origin/main (12512d0).
  │   Brief shape: Path B Hybrid (Phase 0 fully-locked + execution intent architecturally
  │   specified + mid-cascade checkpoint allowed) per honest match between doubly-new
  │   territory (visual implementation + Vulkan recording integration + scene state) and
  │   uncertainty.
  │   Scope (per Q-H-1 LOCKED minimum-scope + Q-H-3 Path γ-A procedural-only asset strategy):
  │   - Phase α0: Brief amendment commit (S-LOCK-4 silent stubs per Crystalka mid-cascade
  │     ratification 2026-05-23 — Phase 0 §2.5 + §2.8 reads surfaced production-fires
  │     conflict с defensive throw design; GameBootstrap publishes ~255 ItemSpawnedCommand
  │     at composition + GameLoop emits TickAdvancedCommand every 33ms + PawnStateReporterSystem
  │     emits PawnStateCommand periodically). R-2 verification gate resolved analytically
  │     (skip empirical R-2 на cascade #2 state since defensive throws would crash; Phase γ
  │     smoke replaces). R-1 verification gate executed in parallel; outcome documented
  │     в δ7 closure section.
  │   - Phase α1: LauncherProceduralAtlas (Q-H-17 Option C copy) — production-side copy
  │     of SmokeTest's ProceduralAtlas preserves S-LOCK-2 substrate isolation (no PngDecoder
  │     palette extension; deferred к когда Vanilla mods consumer need materializes —
  │     Lesson #N15 first-application opportunity reserved).
  │   - Phase α2: SceneState + PawnSpriteEntry types (Q-H-2 + S-LOCK-3) — minimum scope
  │     sprite registry only, no camera/HUD/layer machinery.
  │   - Phase β: Atomic dispatcher + renderer + program commit (Lesson #8 compilable unit) —
  │     RenderCommandDispatcher pawn-3 real implementations (HandlePawnSpawned/Moved/Died
  │     с deterministic per-PawnId tile assignment) + 3 deferred arms silent stubs
  │     (HandlePawnState/ItemSpawned/TickAdvanced с DO NOT TEST comments per S-LOCK-4
  │     amended + Q-H-6 test discipline); LauncherRenderer Vulkan integration via
  │     Runtime.RecordSpritesFrame V0.C.2 batched API one-liner (per §2.0 empirical
  │     finding — replaces brief's manual ~80-line draft с ~10 lines); Program.cs
  │     composition root extended с atlas upload (LauncherProceduralAtlas →
  │     VulkanImage.CreateFromPngImage → SpriteTexture) + SceneState constructor injection
  │     per S-LOCK-10. gameContext.Loop.Start() (NOT gameContext.GameLoop.* per cascade #2
  │     Crystalka Option A amendment).
  │   - Phase γ: Launcher visual smoke verification (manual interaction — pawns visible
  │     as procedural-colored 16×16 tiles, movement observed on PawnMoved dispatch,
  │     despawn observed on PawnDied dispatch, graceful close).
  │   - Phase δ: Governance cascade (KERNEL v2.5.1 → v2.5.2 patch + cascade #3 chronicle +
  │     К-L14 #12 cross-ref; METHODOLOGY v1.11 → v1.12 minor — Lesson #N12 semantic refined
  │     с production-fires/test-only-fires sub-pattern split + #N13 commit integrity +
  │     #N14 Phase 0 empirical Provisional candidates; K_EXTENSIONS_LEDGER §3.4 entry;
  │     PHASE_A_PRIME_SEQUENCING cascade #3 entry; REGISTER 2-commit cascade + sync;
  │     brief AUTHORED → EXECUTED + closure section).
  │   К-L impact: zero (К-L count unchanged: 21 final). Cascade focused on consumer
  │   materialization, не invariant extension.
  │   Lessons surfaced:
  │     • Lesson #N12 SEMANTIC REFINED — second application + sub-pattern split into
  │       (A) test-only-fires defensive throws + (B) production-fires silent stubs.
  │       Promotion criterion amended к require substantially-different sub-pattern OR
  │       different domain.
  │     • Lesson #N13 (Provisional, NEW) — Commit integrity verification before commit
  │       (first observation cascade #2 α1 sln mutation claim/diff mismatch; explicit
  │       application cascade #3 α0 verified git status matched message).
  │     • Lesson #N14 (Provisional, NEW) — Phase 0 reads empirical assumed-state coverage
  │       (first observations cascade #2 α1 directory state divergence + cascade #3 §2.0
  │       production composition divergence).
  │   К-L14 verification #12 — first clean additive evidence (substrate primitives
  │   completely untouched через consumer materialization; cascade #2 #11 = removal-type
  │   evidence; cascade #3 #12 = additive-type evidence).
  │     • Substrate (Runtime + PngDecoder + SpriteRenderer + ProceduralAtlas) primitives
  │       unchanged через addition of 3 dispatch arm implementations + SceneState +
  │       LauncherRenderer Vulkan integration.
  │     • К-L14 thesis preservation: substrate stability через consumer churn empirically
  │       verified by additive consumer materialization without substrate API touches.
  │   Cumulative К-Lxx series: 21 invariants (unchanged).
  │   Commits: cascade atomic on claude/k-ext-3-launcher-visual branch (α0 brief
  │   amendment + α1 ProceduralAtlas + α2 SceneState/PawnSpriteEntry + β dispatcher/
  │   renderer/program + δ1 KERNEL + δ2 METHODOLOGY + δ3 sequencing entry + δ4 LEDGER +
  │   δ5/δ6 REGISTER cascade + δ7 closure = ~12 commits).
  │   Executor: Claude Opus 4.7 (deliberation + brief authoring + execution) + Crystalka
  │   (Q-N ratification + mid-cascade S-LOCK-4 amendment ratification).
  │
  ├─ Phase post-A'.8 — A'.9.0 RECONNAISSANCE / К-extensions cascade #4 (CLOSED 2026-05-24)
  │   Designation: «A'.9.0 Reconnaissance / К-extensions cascade #4» dual designation per
  │   brief §0.5 (first A'.9 milestone-internal cascade; К-extensions cascade #5+ continues
  │   sequence per Q-K-44 recommendation).
  │   Execution branch: `main` (per Crystalka ratification pre-execution — matches cascade
  │   #3 pattern; brief literal «feature branch off 8ea0d03» overridden because HEAD had
  │   advanced к 4981d78 = Crystalka CI logs commit post-cascade-#3).
  │   Scope: comprehensive 7-domain A'.9 Roslyn analyzer milestone architecture
  │   reconnaissance via multi-agent dispatch (7 sub-agents per S-LOCK-5: 3 parallel batch
  │   A in α1 + 3 parallel batch B in α2 + 1 sequential C1 in α3). Produced governance
  │   artifact `docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` (Tier 2 Live Category
  │   A, ~3340 lines).
  │   **Zero production code changes** per S-LOCK-1 — no analyzer project created, no src/
  │   modifications, no test changes, no build config changes (all deferred к Brief A'.9.1
  │   cascade). A'.9.1 Analyzer Infrastructure brief authored post-A'.9.0 closure against
  │   report §10 prerequisites + §11 Q-K candidates.
  │   Reconnaissance domains covered:
  │     • Domain 1 (К-L analyzability): 22-row matrix, 9 P0 / 8 P1 / 3 P2 / 3 P3
  │     • Domain 2 (FORMALIZE Lessons analyzability): 12-row matrix; 11 T6 + 1 T2 (Lesson
  │       #8 auxiliary tooling)
  │     • Domain 3 (cascade #2/#3 surfaced rule candidates): 10 candidates; cross-cascade
  │       observation Lesson #N12 underlies 4 candidates
  │     • Domain 4 (Mod OS К-L20 prep): 20 candidate DFK020 sub-rules + 6 precursor
  │       relationships A'.9-era → К-L20 era
  │     • Domain 5 (Roslyn ecosystem desk research): SDK 5.3.0 + xUnit framework variant
  │     • Domain 6 (Build/CI surface): Option C hybrid + Directory.Build.props centralized
  │     • Domain 7 (Suppression governance): near-zero baseline (5 pragmas + 0 attribute)
  │   Phase 0 anomalies surfaced: pre-existing ANALYZER_RULES.md v0.1 + A_PRIME_9_ROSLYN_
  │   ANALYZER_BRIEF.md v0.1 AUTHORED-SKELETONs (deliberation agent structural anchor
  │   missed these; recon scope adapted к score-against-existing-taxonomy).
  │   Lessons surfaced/refined:
  │     • Lesson #N14 THIRD application surfaced (HIGH promotion proximity now): cascade-
  │       level Phase 0 empirical state coverage applied at meta-level (deliberation
  │       agent gap surfaced by execution agent Phase 0; 3 applications cumulative).
  │     • Lesson #N13 second application surfaced: commit integrity verification at every
  │       commit (α0-α4+β diff vs message claims).
  │     • Observational reconnaissance evidence type FORMALIZED (5th К-L14 evidence type
  │       NEW category per S-LOCK-6 framing).
  │   К-L14 verification #13 — first observational reconnaissance evidence (5th evidence
  │   type NEW category). CLEAN per degenerate criteria (S-LOCK-1 zero-production-code-
  │   touch preserved К-L14 thesis trivially; observational baseline established for A'.9.1).
  │   Cumulative К-Lxx series: 21 invariants (unchanged).
  │   К-L14 evidence active log: 12 entries (9 baseline + #11 + #12 + #13; #10 vacated).
  │   45 Q-K candidates aggregated for Brief A'.9.1 deliberation (42 sub-agent + 3 cross-
  │   cutting α4 synthesis).
  │   Commits: cascade atomic on main (α0 a233639 + α1 baf28dd + α2 98ae26a + α3 1123aac
  │   + α4 f017455 + β1+β2 bundled + β3 REGISTER + γ1 closure = 8 commits within Q-J-8
  │   budget 4-8; β1+β2 squashed per brief Q-J-8 «squashing acceptable где compilable»
  │   allowance).
  │   Executor: Claude Code Opus 4.7 (Phase 0 + multi-agent dispatch + α4 synthesis +
  │   Phase β/γ governance + closure) + Crystalka (pre-execution ratification: branch
  │   strategy + build halt resolution).
  │   KERNEL v2.5.2 → v2.5.3 patch bump per Q-G-12 LOCKED versioning convention
  │   (chronicle + cross-ref = patch).
  │
  ├─ Phase A'.9 — Architectural analyzer milestone (multi-cascade)
  │   Scope: Roslyn analyzer encoding K-Lxx invariants per K-closure report + ANALYZER_
  │   RULES.md v0.1 AUTHORED-SKELETON. Decomposed per A'.9.0 recon report §10 prerequisite
  │   7 + Q-K-43:
  │     • A'.9.0 — Reconnaissance (CLOSED 2026-05-24, this entry above)
  │     • A'.9.1 — Analyzer Infrastructure (К-extensions cascade #5 per Q-K-44): scaffold
  │       tools/DualFrontier.Analyzers/ + tests/DualFrontier.Analyzers.Tests/, Directory.
  │       Build.props centralized, .editorconfig baseline, 17 P0+P1 DF### rules + DF999
  │       self-policing, cleanup phase (suggestion severity)
  │     • A'.9.2 — Severity promotion (cleanup → error) + optional code-fix providers for
  │       Trivial-feasibility rules (DFK002, DFK004, DFK011 within tests)
  │     • A'.9.3+ — DC### cascade-derived rules + DL### Lesson-derived auxiliary tooling +
  │       M3.4 deferred analyzer milestones materialization
  │   Dual purpose preserved: (1) M-series migration verifier (catches drift in M-milestone
  │   migrations that tests don't see), (2) architectural debugger (surfaces bugs that
  │   compile + pass tests but violate invariants).
  │   First run on existing post-K8.5 codebase expected к surface pre-analyzer debt — fix
  │   budget included in A'.9.1 cleanup phase scope.
  │   Track B activation candidate per ROADMAP «Maximum Engineering Refactor» §96-114.
  │   Executor: Opus (architectural deliberation, rule spec) + Cloud Code (Roslyn impl).
  │
  ├─ Phase post-A'.9 — К-L20 Mod API lock cascade
  │   Designation: TBD per K_CLOSURE §9.5 Q1-Q8 deliberation
  │   Scope: К-L20 canonical text LOCKED + DFK020 family activation (20 sub-rules per
  │   A'.9.0 report §6.2) + Mod API surface freeze per A'.9 closure baseline snapshot
  │   Forward analyzer enforcement: analyzer enables Mod API enforcement automation per
  │   memory + Domain 4 precursor relationships (DFK003.1→DFK020.3, DFK009→DFK020.{1,2,8,9},
  │   DFK012→DFK020.8, DFK015→DFK020.{9,10,11}, DFK018→K-L18+K-L20, M3.4→DFK020.{10,11,16}).
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
| A'.5 K8.3+K8.4 combined (CLOSED 2026-05-14) | 4 commits, 1 session | <1 day |
| A'.6 K8.5 SKIPPED (deferred к post-Phase B) | — | — |
| A'.7 K10 native kernel scheduler | substantial; multi-session | TBD (post-execution measurement) |
| A'.8 K-closure report | 1-2 sessions | 8-12 weeks (overlap-able) |
| A'.9 Analyzer | 2-4 weeks | 10-16 weeks |

**Total Phase A' duration: revised estimate post-K8.5 deferral.** А'.5 closed in 1 session 2026-05-14 (compressed vs. 4-6 weeks original estimate). А'.7 К10 execution substantial (multi-session expected). Phase A' duration TBD pending К10 execution closure measurements; original 10-16 weeks estimate retained as rough order-of-magnitude pending K10 measurement. The А'.0/А'.0.5/А'.0.7 doc-only deliberation+refresh sessions were sub-day each as predicted.

This duration reflects «без костылей + decade-horizon» commitment. Faster paths exist (skip K-L3.1, skip closure report, skip analyzer) but each shortcut creates structural debt this phase exists to prevent.

---

## §4 Phase A' invariants

Three invariants govern Phase A' execution:

### §4.1 Sequencing is total, not partial-ordering

Each Phase A'.N depends on A'.<N-1> closure. No skipping. No parallelism within A' (except possibly A'.8 K-closure report drafting in parallel with A'.7 K8.5 execution if K8.5 deliverables are stable enough to enumerate).

Reasoning: each milestone produces input to the next. K-L3.1 → amendment plan → amended skeleton briefs → executable kernel migrations → closure report enumerating all final state → analyzer encoding closure report. Any reorder breaks the input chain.

**Exception**: Phase A'.4 K9 may be re-ordered relative to A'.5-A'.7 (K8.3-K8.5) per Migration Plan Option c sequencing logic — K9 is kernel-side independent of K8.3-K8.5 except in IModApi v3 surface which K8.4 ships. The ordering above places K9 first as default, but if K-L3.1 deliberation surfaces reasons to defer K9 (e.g., K9 skeleton requires significant amendment vs K8.3-K8.5 minimal amendment), order may be K8.3 → K8.4 → K8.5 → K9 → K-closure report → analyzer. Decision at A'.1 amendment brief time.

**Refinement 2026-05-18 (K8.5 deferral cascade):** «No skipping» applies к execution work, not к slot allocation. Phase А'.6 slot marked SKIPPED because the milestone originally planned for that slot (K8.5) is deferred к post-Phase B per audience-absence architectural reason — not because Phase A' work itself is skipped. Phase A' execution proceeds от А'.5 closure к А'.7 К10 directly. К8.5 milestone preserves canonical identity per KERNEL_ARCHITECTURE.md Part 2; only its Phase A' execution slot defers. Sequencing invariant violation candidate: «А'.7 depends on А'.6 closure» — checked: К10 execution (А'.7) has no dependency on К8.5 outputs (K8.5 documentation milestone, К10 architectural milestone, orthogonal scopes). Invariant preserved by dependency check, not by slot-numbering continuity.

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
- **A'.5 K8.3+K8.4 combined amendment (DONE 2026-05-14)**: §4.6 IModApi v3 surface finalized per K-L3.1 Q1+Q2 lock + post-cutover NativeWorld single source of truth wording.
- **A'.6 K8.5 amendment: DEFERRED**. Originally §11 migration phases table extended with M8.4-M10.B rows per Migration Plan §6.2 schedule. Deferral к post-Phase B per K8.5 deferral cascade (2026-05-18).
- **A'.7 K10 amendment**: cross-doc cascade per `tools/briefs/K10_CROSS_DOC_AMENDMENTS_CASCADE_BRIEF.md` skeleton (AUTHORED-SKELETON; promotion к AUTHORED at К10 execution closure or earlier per prioritization). MOD_OS section updates per K10 specification v2.0 Part 7 cross-document amendments queue (capability section tier-prefixed tokens, §9.5 unload chain extension, §11 К-L18 quiescent state compliance).

### §5.3 MIGRATION_PLAN_KERNEL_TO_VANILLA.md

- **A'.1 amendment**: v1.0 → v1.1 (decision #9 added per K-L3.1 lock; line 62 framing corrected; sections 1.2/1.3/1.4 scope wording updated per K-L3.1 Q3+Q1+Q2 answers; **Phase A' formalization integrated into §0.1 sequence**).
- **A'.4-A'.5**: per-milestone scope clarifications as needed (A'.4 K9 DONE; A'.5 K8.3+K8.4 DONE).
- **A'.6 K8.5: DEFERRED** — no MIGRATION_PLAN amendment trigger fires within Phase A' for K8.5.
- **A'.7 К10**: К10 native kernel scheduler may produce Migration Plan amendments per К10 execution closure; scope deferred к К10 execution brief authoring.

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
- **A'.4 K9 closure entry** (DONE 2026-05-11)
- **A'.4.5 Document Control Register closure entry** (DONE 2026-05-12)
- **A'.5 K8.3+K8.4 combined closure entry** (DONE 2026-05-14)
- **A'.6 K8.5 DEFERRAL entry** (this cascade 2026-05-18; closure-as-deferral, not closure-as-execution)
- **A'.7 К10 closure entry** (planned post-К10 execution)
- **A'.8 K-closure report entry** (special — closure of closure)
- **A'.9 analyzer milestone closure entry**

### §5.6 ROADMAP.md

ROADMAP currently «Updated 2026-05-03» (stale; M8.0/M8.1/M8.2 closures and K0-K8.2v2 closures not reflected). Phase A' is the natural occasion for full ROADMAP refresh — this is **not** scheduled here as separate milestone but as **А'.8 K-closure report** companion update (К8.5 housekeeping path retired per K8.5 deferral cascade 2026-05-18).

### §5.7 New documents Phase A' may produce

- **K-closure report** (A'.8) — shipped at `docs/architecture/K_CLOSURE_REPORT.md`
- **Analyzer specification document** (A'.9 input) — likely at `docs/architecture/ARCHITECTURAL_ANALYZER_SPEC.md` or analog
- **Possibly: MOD_OS_ARCHITECTURE.md v2.0** if K-L3.1 synthesis is structurally significant — may warrant fresh document version rather than continued v1.x corrections

---

## §6 Phase A' completion gate

Before Phase B (M8.4) begins, the following are TRUE:

- [ ] K-L3.1 decision LOCKED with amendment plan executed
- [ ] All 4 LOCKED docs synced to post-K-L3.1 architecture (KERNEL/MOD_OS/MIGRATION_PLAN/MIGRATION_PROGRESS)
- [ ] All 4 skeleton briefs (K9, K8.3, K8.4, K8.5) reviewed for post-K-L3.1 consistency, amended per disposition, executed successfully
- [x] K-series substrate closed via К8.3+К8.4 cutover (А'.5 closure 2026-05-14): 10 production systems migrated к NativeWorld AcquireSpan/BeginBatch, managed World retired как ManagedTestWorld test fixture, Mod API v3 closed, runtime isolation guard removed per K-L11
- [ ] К10 native kernel scheduler executed (А'.7): full native scheduler + bus + control plane + V substrate interlock + TLA+ formal verification per KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED; К-L12-19 + К-L7.1 invariants promoted к LOCKED
- [ ] K-closure report authored (А'.8): formal К-Lxx invariant enumeration + analyzer rule specification surface + Provisional Lessons promotion review (9 candidates)
- [ ] Architectural analyzer milestone executed (А'.9): rules encoding K-closure report invariants; first run on existing codebase passes (debt fixed during analyzer milestone scope per Linux -Werror precedent); CI integration active
- [ ] М-series migration guide deferred к post-Phase B (К8.5 deferred): documentation milestone awaits mod authors audience; not a Phase A' completion gate.
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

Default lean: **(c) skeleton-first**. Each milestone (K9, K8.3+K8.4 combined, К10) populates its section as it closes; А'.8 finalizes. К8.5 deferred к post-Phase B (2026-05-18 deferral cascade) means К-closure report does not include К8.5 section initially; К8.5 closure section added post-Phase B при К8.5 execution.

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
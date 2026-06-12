---
register_id: DOC-D-ARCHITECTURE_TRUTH_CASCADE_BRIEF
project: Dual Frontier
category: D
tier: 4
lifecycle: Draft (→ LOCKED on Crystalka ratification → EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-06-11'
content_language: en
authored_by: Claude Opus (deliberation session, Architecture Truth Cascade prep)
basis: ARCHITECTURE TRUTH RECON REPORT 2026-06-11 (R1–R8)
---

# ARCHITECTURE TRUTH CASCADE — Execution Brief

Single-pass execution. Executor: **Claude Code, Fable 5, LOCAL on Skarlet**. Repository: `D:\Colony_Simulator\Colony_Simulator`. No other tree is involved.

**Brief-integration notice** (first live application of TESTING_STRATEGY v2.0.0 §6): this brief CITES standing law by anchor instead of restating it. Binding by citation: commit-body structure & marker law — CODING_STANDARDS v2.0.0 §8/§5; truth law (existence test, forbidden enforcement verbs) — STANDING_LAW_CASCADE_BRIEF §8 as codified in the v2.0.0 docs; mutability license & `Skeleton revisions` form — RESERVED_SURFACE_MUTABILITY v1.0; session closure sequence — METHODOLOGY v1.13.0 (session closure protocol); census method discipline — TESTING_STRATEGY v2.0.0 §4. A conflict between this brief and any standing doc means THIS BRIEF IS WRONG — halt and escalate (METHODOLOGY v1.13.0, anti-pattern rule).

---

## §1 Mission

Three fronts, one cascade:
**A — Architecture docs to code-truth**: the five remaining drifted bodies are rewritten or sectionally cured; six planning-class docs are reclassified with banners; after this cascade no architecture document answers "what's next" — only "what is". ROADMAP.md alone carries forward state.
**B — ROADMAP reflects reality**: realized work is marked realized (with evidence), pending stays pending, unrepresented cascades gain representation.
**C — Code comments cite living law**: ~15–23 stale comment sites fixed; a citation-form rule is added to CODING_STANDARDS so the two breakage classes found (version pins, URL anchors) cannot recur.

Deliverables:

| # | Artifact | Action | Version |
|---|---|---|---|
| D1 | MOD_OS_ARCHITECTURE.md | sectional rewrite (§0, §2, §4.1–4.5, §10.1, §11 dedup-out, See-also, status line, changelog backfill v1.9–v1.11) | 1.11 → 1.12.0 |
| D2 | VULKAN_SUBSTRATE.md | body scrub + status-truth (≈400–500 lines) | 1.1.2 → 1.2.0 |
| D3 | THREADING.md | rewrite to native-truth | 1.1.1 → 2.0.0 |
| D4 | EVENT_BUS.md | rewrite to native-truth + native-tier spec added | 1.1.1 → 2.0.0 |
| D5 | ARCHITECTURE.md | rewrite to thin code-truth overview (~100–120 ln) | 0.4.1 → 1.0.0 |
| D6 | Reclassification batch ×6 (PHASE_A_PRIME_SEQUENCING, MIGRATION_PLAN_KERNEL_TO_VANILLA, KERNEL_FULL_NATIVE_SCHEDULER, FHE_INTEGRATION_CONTRACT, ARCHITECTURE_TYPE_SYSTEM, MAX_ENG_REFACTOR_TRACK_B) | class banners + pointers | PATCH each |
| D7 | docs/ROADMAP.md | reconciliation (M8, V-split, К-ext #0–#3, pins, F-ledger updates) | Live |
| D8 | MIGRATION_PROGRESS + MIGRATION_PLAN | chronicle catch-up; desync + stale-pin fix | Live; 1.4 → 1.4.1 |
| D9 | DD-3 hygiene | DF→DFK living rename (10 sites, 5 docs) + K_CLOSURE archival mapping banner + dangling-ref fixes + FRAMEWORK §8.1 count PATCH | per-doc PATCH |
| D10 | Comment fixes | managed (~12–20 sites) + native (~3 sites), comment-only commits | — |
| D11 | CODING_STANDARDS.md | citation-form rule (§6 extension) | 2.0.0 → 2.1.0 |
| D12 | REGISTER cascade + validate + render | closure | 2.16 → 2.17 |

## §2 Established facts (recon digest — re-verify в Phase 0 where marked ▲)

▲ main @ `b38929e` (or ahead if Crystalka pushed/ratified — any extra commits must be governance-only; otherwise H1), tree clean, register 2.16 / 272 docs / 39 EVT.
- 35 docs in docs/architecture/**; 14 already drift-processed; deep-rewrite set and per-section classifications per recon R2a (the executor receives the recon report as input — read it, don't re-derive).
- Known code-truths anchoring the rewrites: manifests are strict v3 (`manifestVersion: "3"` on disk); `RegisterComponent<T> where T : unmanaged`; `IsolationViolationException` deleted (only `ModIsolationException` exists); capability/shared-library/package-manager tiers shipped M2–M5 (`SharedModLoadContext`, `KernelCapabilityRegistry`, `VersionConstraint`); native scheduler truth = `system_graph.cpp` + К-L13 runnable subsets, managed `ParallelSystemScheduler` = adapter facade; 5 domain buses match `IGameServices.cs`, native Fast/Normal/Background tiers exist (`bus_fast/normal/background.cpp`); V0+V1 shipped (`V1DiffusionPipeline.cs`, `diffusion.comp(.spv)`), V2 not (`wave.comp` absent); M8 vanilla mods on disk as strict-v3 skeletons; Launcher is the presentation reality (Presentation/Godot gone, cascade #2).
- Comment staleness concentrates in: IVE citations (3 of 4 sites), Godot claims (~⅓ of 15 managed + thread_pool.h:26 native), ANALYZER_RULES anchors in ReservedStubAttribute (both broken at v0.2.0), `NATIVE_CORE.md` (dangling, 2 native headers).
- F-ledger live states per recon R3 cross-check (incl. F-2 count now 123; F-7 also misses #N11).

## §3 Phase 0 (orchestrator, serial)

1. ▲ Verify §2 base state; classify any post-`b38929e` commits; non-governance extras → **H1**.
2. **Baseline gates**: full managed + native build (commands per DEVELOPMENT_HYGIENE v2.0.0) and full test run. Record results; expected ≈1034/1036 with the two F-10 pre-existing failures — the BASELINE, not a halt. Closure must match or improve the baseline (**H2** on regression).
3. REGISTER enum vocabulary read (Lesson #N14) — empirical shapes only for D12.
4. Mandatory reads: the ARCHITECTURE TRUTH RECON report (full), the five rewrite targets, ROADMAP.md, K_EXTENSIONS_LEDGER.md, RESERVED_SURFACE_MUTABILITY v1.0, CODING_STANDARDS v2.0.0 §5/§6/§8, TESTING_STRATEGY v2.0.0 §4/§6, METHODOLOGY v1.13.0 closure protocol.
5. Validate-gate protocol as established: every validate run's VALIDATION_REPORT.md lands inside the commit that ran it. `-Sync` forbidden. No pushes (Crystalka's act).

## §4 Topology

```
Orchestrator
 ├─ Phase 0
 ├─ Wave W — 4 writer agents, parallel, disjoint files:
 │    W1: D1 MOD_OS            W2: D2 VULKAN
 │    W3: D3+D4+D5 (THREADING, EVENT_BUS, ARCHITECTURE)
 │    W4: D6 reclass batch + D8 + D9 doc-side hygiene
 ├─ Checkpoint C-W (truth-law sample audit; zero-roadmap-load check; citation-form compliance vs §7)
 ├─ CF — comment-fix agent (D10), AFTER C-W (so fixed citations target final doc structure)
 └─ Serial closure (orchestrator only): D7 ROADMAP reconciliation, D11, commits C1–C17, D12, closure report
```

Hard rules: **ROADMAP.md is single-writer** — only the orchestrator edits it (W1/W2 hand over relocation/dedup payloads as drafts; **H7** on any agent touching it directly). Only the orchestrator stages/commits. Writers write granted files in place, never stage. Each rewrite carries frontmatter per the register mirror shape and ends with Amendment protocol + Change history per the v2.0.0 house pattern.

## §5 Writer specifications

Global: truth law binds (existence test; no enforcement verbs without an on-disk enforcer; future capability only as `Planned — see ROADMAP.md §…` pointers). All citations follow §7 form (no version pins between living docs, no URL anchors). Recon section classifications are the work order; writers verify against code before writing (recon is input, code is truth).

### W1 — D1 MOD_OS_ARCHITECTURE 1.12.0
Per recon R2a profile: **§0** status matrix → realized state (Publish/Subscribe live, capability model / shared library / package manager shipped M2–M5 with evidence names). **§2** → "Manifest v3": real schema with `manifestVersion`, strict-v3 semantics, kill "backward-compatible v2 extension" framing; **§3.6 case 3** grace-period residue removed. **§4.1** signature → `where T : unmanaged`; **§4.5** grace-period section removed/replaced with the strict-v3 statement (resolve the §4.5-vs-§4.6.3 contradiction in favor of code; DRIFT-011 finally dies); §4.6 doc-comment fossils ("K9 not yet landed", "G0") cleaned. **§10.1** threat rows → current fault model (`ModIsolationException`, ModFaultHandler path); §10.4 "(still open per ROADMAP)" → closed M7.3. **§11** → dedup against ROADMAP M-rows: relocate ONLY non-duplicated narration (hand payload to orchestrator for D7), replace section with a pointer. **See-also**: `../ROADMAP.md` path fix, Persistence README depth fix, VULKAN "v2.0" pin removed per §7. **Status line** v1.7 → current; **changelog**: backfill v1.9–v1.11 one-liners (dates/scope from git history of `f09055a` ancestry — empirical, not invented) + v1.12.0 entry.

### W2 — D2 VULKAN_SUBSTRATE 1.2.0
Per recon R2a: preamble/exec-summary de-Godot ("migration from dual-backend" → completed-migration framing). **§1.1/§1.2** V0/V1 → realized (closure evidence; exit-criteria tables become record, not future); §1.3 V2 stays genuinely pending. **§2.1** target tree → the REAL tree (Launcher in, Presentation out, real mods, `wave.comp` absent), **§2.2/§2.3/§2.4** Presentation/`ParallelSystemScheduler.ExecuteTick()` residues fixed. **§4.1** migration-guide tense → record; **§4.2** R-table → relocate status to ROADMAP V-split (payload to orchestrator), keep design rationale; phantom `tools/build-all.ps1` deliverable corrected to what exists. **§6.1/§6.3** status tables → V0 ✅ / V1 ✅ / V2 ⏭ / M-V ⏭ (mirror of D7, single source = ROADMAP, tables here become pointers or evidence-marked records). §9 R1 risk → marked moot post-V0. Closing note "v1.0" desync fixed. See-also labels: VISUAL_ENGINE/GODOT_INTEGRATION marked historical. Part 12 "KERNEL Part 2" → ROADMAP §Native-tracks pointer.

### W3 — D3 THREADING 2.0.0, D4 EVENT_BUS 2.0.0, D5 ARCHITECTURE 1.0.0
**D3**: native-truth inversion — scheduling spec = `system_graph.cpp` + К-L12/К-L13 (runnable subsets, on-demand wake), managed layer described as adapter facade; keep the valid К8.3 paragraph; TickRates section grounded in native wake; async-ban section re-grounded ISOLATION-style (infra shipped — 17 stubs wired; detection = Phase β pointer); `IsolationViolationException` debugging sample → current fault path.
**D4**: delivery truth = native Fast/Normal/Background tiers (`bus_*.cpp`) + managed `DomainEventBus` as the mod-facing facade; the missing native-tier spec WRITTEN (this is the one place new spec is authored — from code, ~30–50 lines); `SetComponent`/`_allowedWrites` deleted-path content removed; Intent→Granted, lease model, subscription lifecycle kept (SPEC-CURRENT per recon); profiling section → current tooling truth or removed.
**D5**: thin overview — purpose, the four-layer map with the REAL assembly set (incl. Core.Interop, Runtime, Launcher, Native kernel; Power gone), dependency-rules table updated, scenario shading dropped, then pointers (KERNEL for kernel truth, MOD_OS for mod system, VULKAN for GPU, THREADING/EVENT_BUS for concurrency/messaging). RimWorld-problems framing compressed to 3 lines of motivation or dropped. register_id retained.

### W4 — D6 reclass batch + D8 + D9 doc-side
**D6** (one banner pattern, six docs, PATCH each): a short class-declaration banner after frontmatter — `Document class: <planning-record | deliberation-record | forward-design-contract | design-draft>. Forward state authority: docs/ROADMAP.md. This document is not a roadmap.` + a one-line pointer where a reader would look for status. KERNEL_FULL_NATIVE_SCHEDULER additionally: fix the internal "К10: AUTHORED (this doc)" self-status to realized-with-pointer.
**D8**: MIGRATION_PROGRESS — snapshot table to current (active = this cascade context; last completed per chronicle truth) + chronicle entries А'.8 → present (К-ext #2, #3, A'.9.0, A'.9.1 Phase 0/α/β-prep, DD recon + refactor branch, Standing-Law C1–C10), each entry one line + hash, from git history — empirical. MIGRATION_PLAN — frontmatter/body version desync (1.4), stale authority pins removed per §7, PATCH 1.4.1.
**D9**: DF→DFK living rename — exactly the 10 living instances (KERNEL ×1, K_EXTENSIONS_LEDGER ×2, K_L14_EVIDENCE_DASHBOARD ×1, PHASE_A_PRIME_SEQUENCING ×5, ANALYZER_RULES ×1) via find→map→review table in the commit body (non-uniform mapping: DFK###/DFL###/DF999-stays/К-L20-family); **K_CLOSURE_REPORT gets ONE archival mapping banner** ("rule IDs herein use the pre-A'.9.1 DF### namespace; current mapping: ANALYZER_RULES §4") — snapshot integrity preserved, 85 edits NOT made. Dangling refs: `K_SERIES_CLOSURE_REPORT.md` ×3 → `K_CLOSURE_REPORT.md`; `K10_DELIBERATION_STATE.md` refs in living docs → mark as external-archive citations; living-doc references to `GPU_COMPUTE.md`/`RUNTIME_ARCHITECTURE.md` → "(superseded; see VULKAN_SUBSTRATE / KERNEL_ARCHITECTURE)" inline only where living normative text cites them (snapshots untouched); verify MOD_API_CONTRACT / MOD_AUTHORING_GUIDE / MODDING_MIGRATION_GUIDE forward-refs each carry a ROADMAP pointer. FRAMEWORK §8.1 "Four meta-entries" → "Five" (PATCH; ratified by this brief — Д-5).

### CF — D10 comment fixes (after C-W)
Comment-only diffs; managed and native as separate commits; build gate after each (H2 insurance). Site list (recon R4/R5 verdicts govern; audit-then-fix where marked):
1. `SystemOrigin.cs:7`, `IModFaultSink.cs:10` + the third stale IVE site (grep all 4; the SystemExecutionContext "previously threw" historical stays) → current fault-path truth; IModFaultSink's "Phase 2 scheduled" → realized (K6).
2. Godot class: audit all 15 managed sites; fix STALE-CLAIM only (`GameLoop.cs:17` confirmed; historical-reference sites like DevKitOnlyAttribute stay VALID).
3. `ReservedStubAttribute.cs:31` "enforces" → truth phrasing ("registered; detection lands at Phase β"); `:42` anchor+§4.4 → §7-form citation of ANALYZER_RULES §4 (topic, no anchor).
4. `ParallelSystemScheduler` framing: audit 27 sites; fix only comments claiming the managed scheduler IS the scheduler (facade references stay).
5. Native: `thread_pool.h:26` Godot-core-reservation → current threading truth; `component_store.h:23` + `df_capi.h:34` `NATIVE_CORE.md` → repoint to the authority actually covering the cited content (expected KERNEL_ARCHITECTURE; verify coverage — if no doc covers it, point to KERNEL + record gap as an F-entry, **H4** only if the content contradiction is architectural).

## §6 D7 — ROADMAP reconciliation (orchestrator, serial)

1. **M8 row**: attempt the row's own acceptance criterion (smoke-load per its definition) in-session. Green → ✅ with evidence; not green / not runnable → honest state "skeletons on disk (strict v3), acceptance criterion pending" + what's missing. No faked flips.
2. **V-row split**: V0 ✅ (closure refs) / V1 ✅ (`V1DiffusionPipeline.cs`, `diffusion.comp.spv`) / V2 ⏭ / M-V ⏭ — replacing the monolithic pending row (line 49 + §799 block), consuming W2's payload.
3. **К-ext #0–#3 representation**: a compact К-extensions block — one row each (#0 А'.7.x, #1 А'.7.5, #2 Godot deprecation+Launcher, #3 Launcher Visual) with hash + pointer to K_EXTENSIONS_LEDGER as the detail authority; #4/#5 rows if absent.
4. **MOD_OS §11 payload**: merge W1's non-duplicated narration into the M-rows' notes; no duplicate table.
5. **Version-pin removals**: line 15 + see-also:958 MOD_OS "v1.5" pins → unpinned citations per §7.
6. **F-ledger updates** (may land here or in the D12 commit — keep one place): F-2 count → 123 (post-C10 note); F-7 wording → "#N11/#N15/#N16 absent"; seed F-14 MOD_OS changelog gap (CLOSED→C2), F-15 MIGRATION desyncs (CLOSED→C9), F-16 FRAMEWORK meta-count (CLOSED→C11), F-17 anchor/version-pin citation fragility (CLOSED→C14), F-18 NATIVE_CORE.md dangling (CLOSED→C13), F-19 A_PRIME_9_RECON lifecycle Live→EXECUTED (CLOSED→C16); plus any CF-discovered gap.

## §7 D11 — CODING_STANDARDS 2.1.0: citation-form rule (§6 extension)

One new rule block (SYNTH-2 obligation — the behavior change lands in the owning doc in the same cascade): **Internal citations from code and living documents use stable identifiers and section topics. Forbidden: version pins of living documents (the register owns versions; pins appear only inside dated snapshots/records) and URL-fragment anchors (heading slugs break under restructure — recon Anomaly 4). Canonical forms: `MOD_OS_ARCHITECTURE §3.2 (capability verbs)`, `К-L13`, `ANALYZER_RULES §4 (rule registry)`.** Changelog entry; MINOR.

## §8 Commit plan (intended form; deviations per mutability license, recorded)

| # | Subject | Content |
|---|---|---|
| C1 | `governance(arch-truth): enroll Architecture Truth Cascade brief + validation checkpoint` | brief + VALIDATION_REPORT |
| C2 | `docs(mod-os): sectional code-truth rewrite v1.12.0` | D1 + ROADMAP §11-dedup payload (cohesive move) |
| C3 | `docs(vulkan): body scrub to realized state v1.2.0` | D2 + ROADMAP V-split (cohesive move) |
| C4 | `docs(threading): native-truth rewrite v2.0.0` | D3 |
| C5 | `docs(event-bus): native-truth rewrite v2.0.0 + native tier spec` | D4 |
| C6 | `docs(architecture): thin code-truth overview v1.0.0` | D5 |
| C7 | `docs(architecture): reclassification banner batch ×6` | D6 |
| C8 | `governance(roadmap): reconciliation — M8, К-ext rows, pins` | D7 remainder |
| C9 | `docs(migration): MIGRATION_PROGRESS catch-up + MIGRATION_PLAN 1.4.1` | D8 |
| C10 | `docs(dd-3): DF→DFK living rename + K_CLOSURE archival mapping banner` | D9 part |
| C11 | `docs(dd-3): dangling references + FRAMEWORK §8.1 count PATCH` | D9 part |
| C12 | `src(comments): managed citation truth pass` | D10 managed (comment-only; build gate) |
| C13 | `native(comments): citation truth pass` | D10 native (comment-only; build gate) |
| C14 | `docs(methodology): CODING_STANDARDS 2.1.0 — citation-form rule` | D11 |
| C15 | `governance(roadmap): F-ledger updates F-2/F-7 + seed F-14..F-19` | §6.6 (if not folded into C8) |
| C16 | `governance(register): Architecture Truth Cascade REGISTER closure (2.16 → 2.17)` | D12 + validate folded |
| C17 | `governance(register): render regeneration + header backfill` | render + Option-B backfill |

## §9 D12 — REGISTER cascade (C16)

Empirical shapes only (Phase 0.3). Enroll: this brief (DOC-D, → EXECUTED at closure). Bumps: D1 1.12.0, D2 1.2.0, D3/D4 2.0.0, D5 1.0.0, D6 ×6 PATCH, D8 1.4.1 + Live-doc touch convention, D9-touched docs PATCH, D11 2.1.0, FRAMEWORK PATCH. Lifecycle: A_PRIME_9_RECONNAISSANCE_REPORT Live → EXECUTED (Д-5, ratified herein). EVT `EVT-ARCH_TRUTH-CLOSURE` (39 → 40) with C1–C15 real hashes; no new PENDING-COMMIT except the header self-reference, backfilled at C17. register_version 2.16 → 2.17. Validate exit 0 (**H3**), same protocol as Standing-Law.

## §10 Halt conditions

H1 base-state mismatch beyond governance-only commits · H2 build/test regression vs Phase-0 baseline (comment commits included) · H3 validate nonzero · H4 truth-law unresolvable without architectural decision (incl. NATIVE_CORE.md repoint with architectural contradiction; MOD_OS §4 contradiction resolving any way other than strict-v3 code truth) · H5 REGISTER enum gap · H6 any semantic change to a LOCKED doc beyond the scopes ratified herein (the W-specs + Д-5 are the full grant) · H7 non-orchestrator ROADMAP write · no pushes, no `-Sync`, no history rewrites. On halt: stop, report verbatim, await Crystalka; in-session re-confirmation expected per house protocol.

## §11 Closure protocol & report

Execute METHODOLOGY v1.13.0 session closure protocol. Closure report (chat): commits table (hash | subject); versions table; **ROADMAP delta summary** (rows flipped with evidence, rows honestly left pending); comment-fix table (site | before-class | after, per R4/R5 verdict vocabulary); census re-runs for the patterns this cascade touched (IVE → 1 historical only, Godot stale-claims → 0, old-form DF in living docs → 0, NATIVE_CORE.md → 0, broken anchors → 0 — each with its verbatim expression); F-ledger final states; gates table (baseline vs closure build/test — must match 1034/1036 or better); Skeleton revisions consolidated; self-attestation (no pushes, single render run, snapshots unedited beyond the one K_CLOSURE banner); Crystalka manual checklist (push; ratify lifecycle states; F-4/F-9, F-5, F-7, F-10, F-12 remain his queue).

## §12 Out of scope

KERNEL_ARCHITECTURE rewrites (F-4/F-9 architect-owned; its 1 DF-rename instance in D9 is the sole permitted touch) · A'.9.1 Phase β (next cascade) · doc_role schema + hybrid reverse register (tooling cascade, F-2/F-13) · К-L20 LOCK cascade · branch pruning (F-11) · `project.godot` (F-5) · the two failing tests (F-10 — baseline, not target) · snapshots/EXECUTED-doc content beyond the K_CLOSURE mapping banner · NIH (not involved) · pushes.

---

*Authored 2026-06-11 from ARCHITECTURE TRUTH RECON (R1–R8). Ratification: Crystalka. Без костылей.*

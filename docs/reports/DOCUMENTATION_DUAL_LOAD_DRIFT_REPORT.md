---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT
category: E
tier: 2
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: on-refactor-cascade-execution
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT
---
---
register_id: DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT
category: E
tier: 2
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
last_modified: "2026-06-02"
last_modified_commit: "PENDING-COMMIT-DOC_DRIFT_RECON-ENROLLMENT"
content_language: en
review_cadence: on-refactor-cascade-execution
next_review_due: on-refactor-cascade-execution
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT
source_brief: DOC-D-DOC_DRIFT_RECONNAISSANCE_BRIEF
---
# Documentation Dual-Load Drift Reconnaissance — Report

**Execution date**: 2026-06-02
**Execution model**: Claude Code orchestrator (Opus) + 10 parallel read-only sub-agents (category-cluster decomposition)
**Source brief**: `DOC-D-DOC_DRIFT_RECONNAISSANCE_BRIEF` (AUTHORED 2026-05-25)
**Branch**: `claude/doc-drift-reconnaissance-503aH` (tip at execution: `f94bb84`)
**Discipline**: READ-ONLY (S-LOCK equivalent) — zero substrate, zero production code, zero existing-document edits. This report is the only write.
**Status**: AUTHORED — pending Crystalka determination of refactor work-volume.

> К-L14 thesis instrumentation: this reconnaissance measures **architectural integrity** (one of the three К-L14 falsifiability metrics — defect rate, architectural integrity, pipeline economics) applied to the documentation layer. It alters no substrate.

---

## §6.0 — Executive summary

### Drift magnitude

The reconnaissance scanned **all 267 REGISTER-enrolled documents** (register_version 2.10), classified each, and deep-analyzed the **LIVING-SPEC subset** section-by-section against an empirically-established code-truth manifest.

| Metric | Value |
|---|---|
| Documents in scope (REGISTER v2.10) | 267 |
| Documents classified LIVING-SPEC (deep analysis) | ~46 |
| Documents classified HISTORICAL-SNAPSHOT (quick-pass, drift-exempt) | ~213 |
| Documents flagged MISCLASSIFIED (role ≠ registration) | 7 |
| **Total drift line-items (living-spec)** | **~105** |
| — SPEC-STALE (type A) | ~32 |
| — ROADMAP-PENDING (type B, misplaced-but-legit) | ~21 |
| — ROADMAP-REALIZED (type C) | ~19 |
| — MIXED-INTERWOVEN (type D, the structural root) | ~14 |
| — ORPHAN-REFERENCE (type E) | ~16 |

### Document inventory summary (by class)

- **LIVING-SPEC** (~46): Category A architecture+governance specs, Category B methodology, Category C trackers, Category F module navigation, Category G/H meta.
- **HISTORICAL-SNAPSHOT** (~213): Category D briefs (77), Category E reports/audits/prompts/scratch (62), plus SUPERSEDED/EXECUTED/DEPRECATED entries across A. **Confirmed clean** — Category D+E quick-pass found **0 hard misclassifications** across 139 docs; snapshots being stale is their nature, not drift.
- **MISCLASSIFIED** (7): see §6.4. A structural governance signal, not isolated noise.

### Refactor scope estimate (high-level)

The drift is **not** uniformly distributed — it concentrates around **four un-propagated structural events** and **one taxonomic gap**. The refactor is too large for a single cascade; recommended decomposition is **3 cascades** (DD-1 spec-truth restoration, DD-2 spec/roadmap structural separation, DD-3 misclassification + orphan + desync hygiene) — see §6.10. Estimated aggregate effort: **moderate-to-large** (~20–30 document touch-points, ~3 of them near-rewrites).

### Top-3 highest-value findings (the MIXED-INTERWOVEN root specimens)

1. **`ANALYZER_RULES.md` — bidirectional dual-load.** §4 is titled "active rules — first-batch **enforcement** surface" and marks all 17 rules Status=Active / P0=Error / P1=Error, but every shipped stub is `defaultSeverity: Info` with an empty `Initialize()` emitting **zero diagnostics by design** (Phase β detection PENDING). Simultaneously §11/§4.6 narrate the *already-shipped* stub materialization as a future "will materialize" event. Current-state is **overclaimed** (stubs sold as active enforcers) **and** realized work is **underclaimed** (shipped stubs narrated as future) — in the same document.
2. **`KERNEL_ARCHITECTURE.md` — dual-load by title.** Self-titled "Architecture **&** Roadmap." Part 0/1 are descriptive spec; Part 1 §1.1 embeds a "post-K8 **target**" project tree (roadmap) inside the IS-architecture body, now contradicted by the realized layout; Part 2/3 narrate the shipped K0–K10 milestones in future tense ("Goal:", estimates). The Part 0 invariant table is clean and **must remain untouched** (S-LOCK-1).
3. **The Godot-deprecation un-propagation cluster.** `ARCHITECTURE.md` is a wholesale pre-kernel managed-ECS snapshot frozen LOCKED; `THREADING.md` + `EVENT_BUS.md` document the retired managed scheduler/bus as whole truth while the authoritative native К10 scheduler + Fast/Normal/Background bus tiers are entirely absent; `VULKAN_SUBSTRATE.md` left its whole §4/§6 body in present-tense "current Godot" framing after a header patch *declared* Godot deletion complete; `DEVELOPMENT_HYGIENE.md` references phantom `DualFrontier.Presentation` projects and `build-all` scripts that do not exist.

---

## §6.1 — Methodology

### Multi-agent decomposition

Per the brief's category-cluster strategy (Crystalka ratification 2026-05-25), the orchestrator established a code-truth manifest at R0, then dispatched **10 read-only sub-agents** (general-purpose, fresh context each) across two waves:

| Cluster | Agent | Document set | Depth |
|---|---|---|---|
| C1 | arch-core | KERNEL_ARCHITECTURE, K_CLOSURE_REPORT, ARCHITECTURE | DEEP |
| C2 | arch-analyzer | ANALYZER_RULES (prime specimen) | DEEP |
| C3 | arch-ledger | K_EXTENSIONS_LEDGER, K_L14_EVIDENCE_DASHBOARD, PHASE_A_PRIME_SEQUENCING, A_PRIME_9_RECONNAISSANCE_REPORT | DEEP + append-only handling |
| C4 | governance | FRAMEWORK, SYNTHESIS_RATIONALE, PROJECT_AXIOMS, REGISTER_RENDER, VALIDATION_REPORT, BYPASS_LOG | DEEP |
| C5 | methodology | METHODOLOGY + 5 Category-B docs | DEEP |
| C6a | arch-substrate | KERNEL_FULL_NATIVE_SCHEDULER, MOD_OS, VULKAN_SUBSTRATE, THREADING, ISOLATION, ECS, EVENT_BUS, PERFORMANCE, OWNERSHIP_TRANSITION, FIELDS, FEEDBACK_LOOPS | DEEP |
| C6b | arch-contracts | CONTRACTS, MODDING, MOD_PIPELINE, COMBO_RESOLUTION, COMPOSITE_REQUESTS, RESOURCE_MODELS, FHE_INTEGRATION_CONTRACT, MIGRATION_PLAN, ARCHITECTURE_TYPE_SYSTEM, MAX_ENG_REFACTOR_TRACK_B, historical/{GODOT_INTEGRATION,VISUAL_ENGINE} | DEEP |
| C7 | roadmap-cat | ROADMAP, MIGRATION_PROGRESS, IDEAS_RESERVOIR, TRANSLATION_GLOSSARY, TRANSLATION_PLAN | DEEP (roadmap-nature) |
| C8 | snapshots | Category D (77) + E (62) | QUICK-PASS + misclassification flag |
| C9 | modules | Category F (72) + G (8) | QUICK-PASS + misclassification flag |

### Drift taxonomy (the analytical backbone)

| Type | Code | Health | Temporal nature | Refactor action |
|---|---|---|---|---|
| SPEC-CURRENT | A✓ | healthy | present, accurate | keep in spec |
| SPEC-STALE | type A | drift | past-as-present | update to code-truth |
| ROADMAP-PENDING | type B | misplaced | future, accurate | extract to roadmap layer |
| ROADMAP-REALIZED | type C | drift | future-as-unrealized (but realized) | move to spec OR retire |
| MIXED-INTERWOVEN | type D | drift (root) | entangled | structural separation |
| ORPHAN-REFERENCE | type E | drift | dangling | resolve / remove |

### Code-truth grounding approach

Option (c) hybrid (brief §3.3): the orchestrator established the project inventory + phase-completion ledger + key-type locations centrally (cheap, high-leverage), and sub-agents performed targeted code reads for their specific drift claims. Every SPEC-STALE and ROADMAP-REALIZED claim in this report is backed by a code/commit citation. The orchestrator independently re-verified the highest-impact orphan-reference claims (§6.2 closing note).

### Limitations / coverage gaps

- Category D/E (139 snapshots) received quick-pass confirmation, not line-item analysis — appropriate per the snapshot-exemption contract, but means individual snapshot internal staleness is not catalogued (by design).
- Category F (72 module docs) was sampled (~22 read in depth, all 80 grepped for red flags), not fully deep-read — tier-4 navigation altitude justifies sampling.
- The `367 - id:` lines in REGISTER.yaml include non-document entries (requirements, risks, events); the **267** document count was confirmed by `category:` field count and reconciles with VALIDATION_REPORT.md (267 enrolled). No §9 halt-1 mismatch.

---

## §6.2 — Code-truth manifest

### Project inventory
- **12 managed `src/` projects**: DualFrontier.{AI, Application, Components, Contracts, Core, Core.Interop, Crypto.Future, Events, Launcher, Persistence, Runtime, Systems}.
- **Native kernel**: `native/DualFrontier.Core.Native` (C++; `system_graph.cpp` = К10.1 authoritative scheduler; `world.cpp`, `component_store.cpp`, `bus_fast/normal/background.cpp`, `wake_registry.cpp`, `pipeline_slot.cpp`, `bootstrap_graph.cpp`, `composite.cpp`, `tile_field.cpp`, `thread_pool.cpp`, …).
- **Analyzer**: `tools/DualFrontier.Analyzers` — TFM `netstandard2.0`, `Microsoft.CodeAnalysis.CSharp 5.3.0`, CPM enabled (`ManagePackageVersionsCentrally=true`).
- **2 tools**, ~30 test/fixture projects, **7 mods** (Vanilla.{Combat,Core,Inventory,Magic,Pawn,World} + Mod.Example).

### Key type inventory (verified locations)
| Type | К-L anchor | Path | Status |
|---|---|---|---|
| NativeWorld | К-L11 SSoT | src/DualFrontier.Core.Interop/NativeWorld.cs | ✓ |
| DependencyGraph | К-L5 (adapter-facade post-К10.1) | src/DualFrontier.Core/Scheduling/DependencyGraph.cs | ✓ |
| SystemGraph | К10.1 native authoritative | native …/system_graph.cpp + src …/SystemGraphInterop.cs | ✓ |
| ManagedSystemDispatcher | К-L12 | src/DualFrontier.Application/Scheduler/ManagedSystemDispatcher.cs | ✓ |
| WakeAttributes | К-L13 | src/DualFrontier.Contracts/Scheduling/WakeAttributes.cs | ✓ |
| PipelineSlotInterop | К-L16 | src/DualFrontier.Core.Interop/PipelineSlotInterop.cs | ✓ |
| ReservedStubAttribute | Q-L-10 | src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs | ✓ |

### Phase completion ledger (what HAS shipped vs PENDING)
- **К0–К10 migration**: SHIPPED. К8.3+К8.4 managed-world retirement landed **2026-05-14** (A'.5) — `World`, `GetComponent`/`SetComponent`, `IsolationViolationException`, managed `ParallelSystemScheduler`, managed `ComponentStore<T>` hot path **retired from production**. К9 field storage CLOSED 2026-05-11 (`RawTileField` shipped). К10/К10.1 native scheduler LOCKED.
- **Godot**: FULLY DEPRECATED (К-extensions cascade #2). `src/DualFrontier.Presentation*` projects GONE; `DualFrontier.Launcher` (Vulkan/Silk.NET) is the live render host. ⚠ **`project.godot` still present at repo root** (contradicts VULKAN_SUBSTRATE.md R.8 closure criterion).
- **A'.9.1 analyzer infrastructure**: SHIPPED (Phase α `5030fa2..a23556f` + β-prep `588c667..a213954`). **17 rule STUBS** on disk: 9 Architecture (DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017) + 5 NativeBoundary (DFK001, DFK002, DFK007_1, DFK015_1, DFK019_A) + 3 Discipline (DFL025_A, DFL025_B, DF999). **All are non-detecting stubs** (Info severity, zero diagnostics). **Phase β detection-logic = PENDING.** DF→DFK rename executed (`586bf59`). PROJECT_AXIOMS.md v1.0 Tier-1 LOCKED (`8e1a18a`).
- 21 К-L invariants final (К-L6 SUPERSEDED).

### Native/managed surface
- Native-authoritative: storage (`component_store`), scheduling (`system_graph`), bus tiers (`bus_fast/normal/background`), wake (`wake_registry`), pipeline (`pipeline_slot`), composite.
- Managed: interop shims (`Core.Interop`), contracts, batched dispatch (`ManagedSystemDispatcher`, К-L12). The 17 analyzer rules (once detection lands) will enforce the К-L boundary at compile-time.

### Orphan-reference independent verification (orchestrator-confirmed absent)
`docs/architecture/MOD_API_CONTRACT.md` ✗absent · `docs/formal/*.tla` ✗absent · `tools/build-all.*` ✗absent · `.github/workflows/` ✗absent · `PerformanceGates.cs` ✗absent · `src/**/ShieldSystem.cs` ✗absent · `project.godot` **present** (should be absent per Godot deprecation).

---

## §6.3 — Per-category drift inventory

### §6.3.1 — Core architecture (C1)

**`docs/architecture/ARCHITECTURE.md`** (LOCKED v0.4) — **classification: LIVING-SPEC, wholesale SPEC-STALE.**
- *(SPEC-STALE)* Entire doc describes a fully-managed ECS (`World`, `ComponentStore`, `ParallelSystemScheduler`, `DependencyGraph`) with **zero mention** of the native C++ kernel, NativeWorld SSoT, К-L invariants, or К10.x. The whole K0–K10.3 migration is invisible. → **near-rewrite or supersede.**
- *(ORPHAN-REFERENCE)* Assembly diagram omits Core.Interop/Runtime/Persistence/Launcher; Presentation retired to Launcher. → update-to-code.

**`docs/architecture/KERNEL_ARCHITECTURE.md`** (LOCKED v2.5.3, ~95 KB) — **LIVING-SPEC, dual-load by title ("Architecture & Roadmap").**
- *(MIXED-INTERWOVEN, root)* Part 1 §1.1 "post-K8 **target**" project tree embedded in the IS-architecture body; contradicted by realized layout (Modding under Application, Presentation→Launcher, `system_graph.cpp`). → structural-separation.
- *(ROADMAP-REALIZED)* Part 2 K0–K9 milestone bodies written future-tense ("Goal:", estimates) though all shipped. → move-to-spec/chronicle or retire.
- *(MIXED-INTERWOVEN)* Part 3 migration strategy blends past-tense closure with pending K8.5 bullets; internal "12 vanilla systems" vs Part 0 "10 production systems" contradiction. → structural-separation.
- *(ROADMAP-PENDING)* Header pre-execution estimates ("5–8 weeks…") retained as live planning text. → extract-to-roadmap.
- *(SPEC-CURRENT ✓ — DO NOT TOUCH)* Part 0 invariant table (21 invariants, К-L6 SUPERSEDED) matches manifest exactly. **S-LOCK-1: sacrosanct.**
- *(SNAPSHOT, exempt)* Chronicle/version-history cascade entries.

**`docs/architecture/K_CLOSURE_REPORT.md`** (AUTHORED v1.0, ~190 KB) — **LIVING-SPEC (co-canonical closure report).**
- *(ROADMAP-REALIZED + ORPHAN-REFERENCE)* §7.2/§7.3 enumerate analyzer rules as forthcoming under **DF### IDs** (DF001..DF020) and singular package "DualFrontier.Analyzer"; realized as **DFK###** under plural `DualFrontier.Analyzers`, with DFL025_A/B + DF999 not in the closure tables. → move-to-spec (qualify infra-realized/detection-pending) + resolve-ref.
- *(SPEC-CURRENT ✓)* §2 invariant enumeration (co-canonical with KERNEL Part 0); §1/§3/§4/§8 closure narratives are SNAPSHOT (exempt).

### §6.3.2 — Analyzer + rules (C2)

**`docs/architecture/ANALYZER_RULES.md`** (AUTHORED-SKELETON) — **LIVING-SPEC, the prime dual-load specimen. dual-load density: HIGH.**
- *(SPEC-STALE / overclaim)* §4 "active rules — enforcement surface", §10.1–3 Status=Active, P0/P1 "Error severity" — but all 17 stubs ship `Info`, `Initialize()` returns zero diagnostics. **No enforcement surface exists — only a non-detecting stub surface.** → update-to-code.
- *(ROADMAP-REALIZED)* §4.6/§11 narrate stub materialization as future ("will materialize all 17 rules") though the 17 stub `.cs` files already exist. → update-to-code.
- *(MIXED-INTERWOVEN, root ×3)* §4 (spec) ↔ §5/§6/§7/§8 (deferred roadmap) ↔ §10.5/§11 (forward plan) interwoven; reader cannot tell "what IS" from "what WILL be" without opening `Rules/`. → structural-separation.
- *(ROADMAP-PENDING ×3)* §4.1/§4.2 "Error post-promotion" columns; §5 (К-L20 LOCK family); §6 (hardware-tier). → extract-to-roadmap.
- *(ORPHAN-REFERENCE)* §5 Bridge/К-L20 references nonexistent `MOD_API_CONTRACT.md`. → resolve-ref.
- **Per-doc structural-separation map** (deliverable): PURE-SPEC body = §1 framing, §2 template, §3 namespaces, §9 reserved namespaces, + a corrected §4/§10 registry stating ground truth ("17 Info stubs shipped, detection PENDING Phase β, zero diagnostics"). EXTRACT to roadmap = §4.1/§4.2 promotion columns, §5, §6, §10.5, §11, front-matter forward-sequencing. EXTRACT to exclusion/decisions layer = §7 (outside-Roslyn), §8 (dropped tooling). RESOLVE-REF = MOD_API_CONTRACT.md.

### §6.3.3 — Ledger + evidence (C3) — append-only handling

**`K_EXTENSIONS_LEDGER.md`** (Live, APPEND-ONLY) — past entries #0–#4 exempt. §4 forward roadmap presents A'.9.1 (cascade #5) as anticipated/future though SHIPPED *(ROADMAP-REALIZED)*; "DF###" nomenclature *(ORPHAN-REFERENCE)*. → refresh §4, append cascade #5 realization, note Phase β still pending.

**`K_L14_EVIDENCE_DASHBOARD.md`** (AUTHORED-SKELETON, APPEND-ONLY) — past verifications exempt. §5 verification #14 (A'.9.1) legitimately pending Phase β *(ROADMAP-PENDING)*. §6 self-declared promotion gate (3+ post-closure verifications) is **already met** while header still reads AUTHORED-SKELETON v0.1 *(SPEC-STALE, internal)* → REGISTER lifecycle re-evaluation candidate (AUTHORED-SKELETON → Tier-2 Live).

**`PHASE_A_PRIME_SEQUENCING.md`** (Live, Tier-2) — **MISCLASSIFIED.** Self-declares "REFERENCE document… not living-spec… becomes superseded by Migration Plan v1.1," and carries a **stale `/mnt/user-data/outputs/` placement clause** while sitting committed in-repo. Sequencing IS roadmap by nature; ~half realized (A'.0–A'.8, cascades #2/#3/#4, A'.9.0/.1) / ~half pending (К10 lock, A'.9.2/.3, К-L20). → **reclassify to roadmap-layer.**

**`A_PRIME_9_RECONNAISSANCE_REPORT.md`** (Live, Tier-2) — **HISTORICAL-SNAPSHOT, correctly.** Recommendations consumed downstream by shipped A'.9.1 — confirms recon-input role, not living spec. Housekeeping note: its "post-A'.9.1 closure" review trigger has now arrived.

### §6.3.4 — Governance trio (C4)

**`REGISTER_RENDER.md`** (Live, Tier-2) — **highest-value count drift in cluster.** *(SPEC-STALE ×3)* register_version **2.0 vs SoT 2.10**, **253 vs 267** docs, missing `DOC-A-PROJECT_AXIOMS` row, KERNEL shown **v2.3 vs v2.5**, risks 12 vs 14, CAPA 0 vs 17; plus a render-script bug emitting literal `$(System.Collections.Hashtable.last_modified_commit)` on every entry. Violates its own `register_rendered_derivative` contract. → **regenerate via render script + fix variable-expansion defect.**

**`FRAMEWORK.md`** (LOCKED, Tier-1) — *(SPEC-STALE)* §8.1 says **four** meta-entries; register holds **five** (PROJECT_AXIOMS as `register_framing`); §8.2 schema-coupling rule omits it. → surface to Crystalka (LOCKED protocol text — do not auto-edit semantics).

**`PROJECT_AXIOMS.md`** (LOCKED, Tier-1) — *(ROADMAP-PENDING→REALIZED)* §4.2/§4.4 "Evidence #14 expected at A'.9.1 closure" is roadmap text inside a LOCKED axiom doc that A'.9.1-shipped likely realizes; *(ORPHAN-REFERENCE)* line-pinned cross-ref into FRAMEWORK §0 (line 21→moved to 23 after v1.1.1 patch). → verify dashboard #14; convert line-pins to section anchors.

**Clean**: `SYNTHESIS_RATIONALE.md` (PATCH correctly synced), `VALIDATION_REPORT.md` (HISTORICAL-SNAPSHOT, current — 267 enrolled/264 synced, the 3-doc gap isolates exactly the newest A'.9.1 enrollments), `BYPASS_LOG.md` (APPEND-ONLY, correctly empty).

### §6.3.5 — Methodology (C5)

**`DEVELOPMENT_HYGIENE.md`** (LOCKED v1.0) — **MISCLASSIFIED-leaning (wholesale stale).** *(SPEC-STALE)* boundary table lists `DualFrontier.Presentation.Native`, `DualFrontier.Presentation` (Godot DevKit) — none exist. *(ORPHAN-REFERENCE ×3)* `./tools/build-all.sh|ps1` (absent), CODING_STANDARDS "Commit messages" section (does not exist), `./ROADMAP.md` relative link (wrong path). → near-rewrite for post-Godot project set.

**`METHODOLOGY.md`** (LOCKED v1.12.1) — *(SPEC-STALE)* frontmatter v1.12.1 has no §10 change-history/version-note backing (1.11/1.12/1.12.1 rows absent — governance auto-sync drift); §4.1 "3 methodology docs" vs 6 actual. *(ORPHAN-REFERENCE, inverse)* brief manifest references Lesson **#N18** which is **entirely absent** from the doc (tops out at #N17). *(ROADMAP-REALIZED, partial)* #N14/#N17 "FORMALIZE candidacy at A'.9.1 Phase δ" — A'.9.1 infra shipped; re-verify δ-closure. → reconcile frontmatter↔changelog; capture or retract #N18.

**`MAXIMUM_ENGINEERING_REFACTOR.md`** (LOCKED) — *(ROADMAP-REALIZED)* Track B "No track activated" while the A'.9.1 analyzer (17 stubs, CPM, netstandard2.0) **is** Track B's B-α Roslyn proposal, shipped — at `tools/` not the doc's claimed `src/` path. *(SPEC-STALE)* §2.6 risk references removed Godot runtime. → mark Track B ACTIVATED (detection Phase β pending), fix path.

**`CODING_STANDARDS.md`** / **`TESTING_STRATEGY.md`** — *(ROADMAP-PENDING / overclaim)* both assert analyzer/CI enforcement as live: "checked by the analyzer" (analyzer is stubs; only built-in IDE0161 enforces today); "PerformanceGates.cs … CI gate" with **no `.github/workflows/` and no `PerformanceGates.cs`** in repo. *(SPEC-STALE)* TESTING_STRATEGY layout under-counts test projects (lists 4; 11 exist). → qualify enforcement claims as roadmap; refresh layout.

**`PIPELINE_METRICS.md`** — **exemplary dual-load management** (every § carries a transferability label; §6 "Forward measurement plan" enumerates the backlog explicitly). Only residue: §1.3 Godot 4.3 mention, already inside annotated-historical scope. **Model for the other docs.**

### §6.3.6 — Substrate architecture (C6a)

- **`KERNEL_FULL_NATIVE_SCHEDULER.md`** (LOCKED v2.0) — **MISCLASSIFIED** (pre-impl deliberation snapshot mislabeled LOCKED descriptive). *(ROADMAP-REALIZED)* §4.1 marks К10 "AUTHORED (this doc)" though native scheduler shipped; *(ORPHAN-REFERENCE)* phantom `docs/formal/SCHEDULER_TLA_PLUS.tla`, "forthcoming" К10 execution brief, sister deliberation-state doc — none exist; *(MIXED-INTERWOVEN)* body pre-ratification framing vs register LOCKED. Highest density in cluster.
- **`THREADING.md`** — *(SPEC-STALE ×2)* documents managed `ParallelSystemScheduler` + `Parallel.ForEach` as the live scheduler (native К10 absent); cites deleted `IsolationViolationException`.
- **`EVENT_BUS.md`** — *(SPEC-STALE ×2)* documents managed `DomainEventBus` 3-mode model + dead `SetComponent`/`_allowedWrites`; native Fast/Normal/Background bus tiers undocumented.
- **`VULKAN_SUBSTRATE.md`** — *(SPEC-STALE)* §4/§6 body present-tense "current Godot dual-backend" after header patch declared deletion complete; *(ORPHAN-REFERENCE)* §6 R.8 "grep godot empty" marked complete but `project.godot` still on disk.
- **`PERFORMANCE.md`** — *(SPEC-STALE)* hot-path table benchmarks dead managed `ComponentStore<T>`; *(MIXED-INTERWOVEN)* GPU/pathfinding "G5+/TODO" roadmap spliced into descriptive perf sections.
- **`FIELDS.md`** — *(ROADMAP-REALIZED)* repeated "lands at K9 implementation" TBDs for layouts that shipped (K9 closed, `RawTileField` live); *(ROADMAP-PENDING)* save/load milestone legitimately deferred.
- **`FEEDBACK_LOOPS.md`**, **`ECS.md`** — single stale clause each (deleted `IsolationViolationException`; managed-scheduler name). ECS otherwise strongly SPEC-CURRENT.
- **`ISOLATION.md`**, **`OWNERSHIP_TRANSITION.md`** — clean LOCKED specs; only interwoven "A'.9 planned" / "TODO Phase 6" roadmap threads (accurately future).
- **`MOD_OS_ARCHITECTURE.md`** — **best-disciplined doc** (roadmap quarantined to §11/§12 milestone tables, version history tracks each M-phase against named commits). Minor: §4.7 code-comment "K9 not yet landed" stale.

### §6.3.7 — Contracts + modding (C6b)

- **Healthy**: `MODDING.md` (IModApi v3 matches code exactly), `RESOURCE_MODELS.md` (lease subsystem shipped and matches).
- *(SPEC-STALE)* `CONTRACTS.md` + `MOD_PIPELINE.md` — manifest examples show **v1 schema** (`requiresContracts`) while `ManifestParser.cs` hard-rejects anything lacking `manifestVersion=="3"`. `MOD_PIPELINE.md` also has dangling `./ROADMAP.md` link *(ORPHAN-REFERENCE)*.
- *(MIXED-INTERWOVEN)* `COMBO_RESOLUTION.md` + `COMPOSITE_REQUESTS.md` — present-tense sequence diagrams describe systems that `throw NotImplementedException("TODO Phase 4/5")`; honest inline TODOs but design-spec narrated as current behavior.
- **MISCLASSIFIED (4) — roadmap-layer artifacts registered as Tier-1 descriptive architecture**: `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (self-labels "architectural roadmap"; phantom `ShieldSystem` in inventory; body v1.3 vs frontmatter v1.4), `FHE_INTEGRATION_CONTRACT.md` (pure forward-contract, honestly dormant; code-comment path bug), `ARCHITECTURE_TYPE_SYSTEM.md` (Draft forward-spec, pilot analyzers unbuilt-by-design), `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` (activation brief = 100% sequencing).
- **Exempt**: `historical/GODOT_INTEGRATION.md`, `historical/VISUAL_ENGINE.md` — correctly SUPERSEDED with banners; no live doc treats them as current.

### §6.3.8 — Roadmap-category, snapshots, modules (C7/C8/C9)

**C7 (Category C + H)**:
- `ROADMAP.md` — already the canonical roadmap layer, structurally viable as consolidation home, BUT its machine-readable Status table is a **full milestone-cycle stale** (M8 + K-series + V-substrate marked Pending/next while all shipped on disk; stamp "2026-05-03"). *(ROADMAP-REALIZED ×2, SPEC-STALE)*. → re-sync status table; closed-phase prose could migrate to a history doc, freeing roadmap to be forward-only.
- `MIGRATION_PROGRESS.md` — **internal self-contradiction**: "Current state snapshot" (Active=Cleanup cascade, 620 tests, Next=K8.5) is flatly contradicted by its own body (A'.8 closed, 1022 tests, K10 DONE). Companion-version pins stale (KERNEL v1.0 vs v2.5). *(SPEC-STALE ×2, ROADMAP-REALIZED ×2)*. → re-sync header snapshot; it should FEED a single roadmap, not be one.
- `IDEAS_RESERVOIR.md` — **clean exemplar** of roadmap/backlog hygiene (explicitly separated from ROADMAP; §5 reservoir→research→LOCKED transition rule is a usable template). Do not touch.
- `TRANSLATION_GLOSSARY.md` — spec-stable; only latent risk is 3 §15 "doc-only / not yet emitted by code" string contracts predating the K10 rewrite (verify before relying).
- `TRANSLATION_PLAN.md` — **MIXED**: healthy translation plan welded to a stale descriptive inventory built on the **deleted IPowerBus/Godot world** (its anchoring "five-vs-six buses incl. IPowerBus" example references a power subsystem deleted in the A'.5 cutover). *(SPEC-STALE, ROADMAP-REALIZED, ORPHAN-REFERENCE)*.

**C8 (Category D + E, 139 docs)** — **CONFIRMED genuine snapshot population, 0 hard misclassifications.** Systemic note: several AUTHORED-SKELETON briefs (K10*, A'.9 Roslyn, A'.8 K-closure) describe now-DONE work but were never lifecycle-promoted from SKELETON — **lifecycle under-advancement** (register's AUTHORED-SKELETON count overstates genuinely-pending work). DF→DFK residue in ~8 briefs is snapshot-exempt and self-documented in the Phase 0 closure report.

**C9 (Category F + G, 80 docs)** — module navigation docs largely track current code. Material drift confined to **3 Application sub-docs**: `Scene/README.md` (Godot as live dual-runtime), `Input/README.md` (forward Godot/Presentation phase checklist referencing dead projects), `Application/README.md` (open Phase 3 checkbox to verify). Category G READMEs clean. (`PresentationBridge` is a live SSoT command-queue type — false-positive for "Presentation" grep, not Godot residue.)

---

## §6.4 — Systemic drift pattern analysis

The ~105 line-items are not independent. They collapse into **five systemic generators**:

### Pattern 1 — Un-propagated structural events (the dominant generator)
Three large completed events never propagated to the descriptive-spec layer:

| Event | Shipped | Living-spec docs still describing the OLD reality |
|---|---|---|
| **Godot Full Deprecation** (cascade #2) | Presentation projects deleted; Launcher/Vulkan live | ARCHITECTURE, THREADING(implied), EVENT_BUS(implied), VULKAN_SUBSTRATE §4/§6, DEVELOPMENT_HYGIENE, PERFORMANCE, PIPELINE_METRICS §1.3, MAX_ENG_REFACTOR §2.6, Scene/Input READMEs — **+ `project.godot` still on disk** |
| **К8.3+К8.4 managed-world retirement** (2026-05-14) | World/GetComponent/SetComponent/IsolationViolationException/managed scheduler/ComponentStore<T> deleted | ARCHITECTURE, THREADING, EVENT_BUS, FEEDBACK_LOOPS, PERFORMANCE, ECS (one clause) |
| **К10/К10.1 native scheduler** (LOCKED) | native system_graph.cpp authoritative; managed = adapter-facade | ARCHITECTURE, THREADING, EVENT_BUS, ECS (clause); KERNEL_FULL_NATIVE_SCHEDULER describes it as future "AUTHORED" |

**Root insight**: these are SPEC-STALE at scale because the docs carry *no structural signal* distinguishing "current truth" from "prior truth" — exactly the dual-load thesis. A descriptive spec with no roadmap/chronicle separation cannot represent "this used to be true" without lying about the present.

### Pattern 2 — The A'.9.1 analyzer ROADMAP-REALIZED wave
The single most cross-cutting realized-roadmap event. A'.9.1 infrastructure shipped (17 stubs) but is narrated as **future** across: ANALYZER_RULES §11, K_EXTENSIONS_LEDGER §4, K_L14_EVIDENCE_DASHBOARD §5, MIGRATION_PROGRESS, ROADMAP, K_CLOSURE §7, methodology Track-B, PHASE_A_PRIME_SEQUENCING, PROJECT_AXIOMS §4.4. **And simultaneously over-claimed as active enforcement** in ANALYZER_RULES §4. This is the cleanest illustration of why roadmap content must retire on realization: nine documents drifted from one un-retired plan.

### Pattern 3 — DF### → DFK### orphan-reference chain
The Q-L-14 rename (`586bf59`) left dangling `DF###` IDs in living-spec cross-refs: K_CLOSURE §7 (DF001..DF020), KERNEL_ARCHITECTURE, K_EXTENSIONS_LEDGER, K_L14_EVIDENCE_DASHBOARD, PHASE_A_PRIME_SEQUENCING, ANALYZER_RULES residue. (Snapshot briefs with DF### are exempt.) Anyone cross-referencing DF003 to code finds DFK003.

### Pattern 4 — Misclassification cluster (a structural governance signal — §9 halt-3)
**Seven** documents have role ≠ registration, all in the same direction: **roadmap / forward-contract / deliberation / activation artifacts registered as Tier-1 Category-A descriptive architecture (or Tier-2 living-spec)**:

| Document | Registered as | Actually is |
|---|---|---|
| PHASE_A_PRIME_SEQUENCING.md | A / Tier-2 / Live | roadmap-layer (self-declared) |
| MIGRATION_PLAN_KERNEL_TO_VANILLA.md | A / Tier-1 / LOCKED | roadmap (self-labeled "architectural roadmap") |
| FHE_INTEGRATION_CONTRACT.md | A / Tier-1 / LOCKED | forward-contract (honestly dormant) |
| ARCHITECTURE_TYPE_SYSTEM.md | A / Tier-1 / Draft | forward-spec (activation-gated) |
| MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md | A / Tier-1 / Draft | activation brief (100% sequencing) |
| KERNEL_FULL_NATIVE_SCHEDULER.md | A / Tier-1 / LOCKED | pre-impl deliberation snapshot |
| DEVELOPMENT_HYGIENE.md | B / Tier-1 / LOCKED | wholesale-stale (effectively superseded) |

**Meta-finding (surfaced per §9 halt-3)**: the register taxonomy lacks a first-class distinction between **descriptive-spec** (must track code) and **forward-spec / roadmap / reserved-contract** (expected to churn/retire). The dual-load problem is partly a *schema* gap, not only a per-document authoring habit. This strengthens the case for the δ-flavored target architecture (§6.6).

### Pattern 5 — Internal count / version desync
Self-referential drift independent of code: REGISTER_RENDER (2.0 vs 2.10), MIGRATION_PROGRESS (snapshot vs body), METHODOLOGY (frontmatter vs changelog; #N18 absent), MIGRATION_PLAN (body v1.3 vs frontmatter v1.4), FRAMEWORK (4 vs 5 meta-entries). Symptom of frontmatter-mirror/body auto-sync gaps.

### Root-cause narrative
The brief's diagnosis is **confirmed empirically**: living-spec documents that also carry roadmap accumulate divergence faster than any single-cascade audit catches, because the document structure does not distinguish "current truth" from "future intention." Patterns 1–3 are all the *same failure* viewed from different angles — a completed event (deprecation, retirement, realization) has no place to "land" in a dual-loaded doc except by overwriting prose that simultaneously serves as both record and plan. The best-disciplined docs (PIPELINE_METRICS, MOD_OS, IDEAS_RESERVOIR) are precisely those that **already fence roadmap from spec** — empirical proof that the structural separation works.

---

## §6.5 — Drift classification summary

### Counts per drift type × cluster

| Cluster | STALE (A) | PENDING (B) | REALIZED (C) | MIXED (D) | ORPHAN (E) |
|---|---|---|---|---|---|
| C1 arch-core | 2 | 1 | 3 | 2 | 2 |
| C2 analyzer | 3 | 3 | 2 | 3 | 1 |
| C3 ledger | 1 | 2 | 2 | 2 | 1 |
| C4 governance | 4 | 1 | 0 | 0 | 1 |
| C5 methodology | 6 | 3 | 2 | 0 | 4 |
| C6a substrate | 8 | 5 | 4 | 3 | 3 |
| C6b contracts | 4 | 6 | 1 | 4 | 3 |
| C7 roadmap-cat | 4 | 0 | 5 | 0 | 1 |
| C9 modules | 3 | 0 | 0 | 0 | 0 |
| **Total** | **~35** | **~21** | **~19** | **~14** | **~16** |

### Living-spec docs ranked by dual-load density (refactor priority)

| Rank | Document | Density | Driver |
|---|---|---|---|
| 1 | ANALYZER_RULES.md | HIGH | bidirectional (overclaim + realized-as-future) + §4/5/10 interweave |
| 2 | KERNEL_ARCHITECTURE.md | HIGH | "Architecture & Roadmap" — target-tree + realized-K-series-as-roadmap |
| 3 | KERNEL_FULL_NATIVE_SCHEDULER.md | HIGH | pre-impl snapshot mislabeled LOCKED + phantom deliverables |
| 4 | ARCHITECTURE.md | HIGH (stale) | wholesale pre-kernel snapshot |
| 5 | VULKAN_SUBSTRATE.md | MEDIUM-HIGH | present-tense Godot body post-deletion |
| 6 | DEVELOPMENT_HYGIENE.md | HIGH | phantom Presentation + build scripts |
| 7 | MIGRATION_PROGRESS.md | HIGH | self-contradicting snapshot table |
| 8 | REGISTER_RENDER.md | HIGH (stale) | 5 versions behind + render bug |
| 9 | THREADING.md / EVENT_BUS.md | MEDIUM | native scheduler/bus absent |
| 10 | TRANSLATION_PLAN.md / PERFORMANCE.md | MEDIUM | mixed plan+stale-inventory / hot-path+roadmap |

### Refactor-surface magnitude
~**24–28 living-spec documents** require some edit; of those, **3–4 are near-rewrites** (ARCHITECTURE, DEVELOPMENT_HYGIENE, ANALYZER_RULES restructure, VULKAN_SUBSTRATE body), **7 reclassifications** (Pattern 4), **1 regeneration** (REGISTER_RENDER), the remainder targeted line-fixes. **Zero** Tier-1 К-L invariant text edits required (§8.6 / §9 halt-6 preserved).

---

## §6.6 — Target architecture proposal

### Option space evaluation (brief §6.7)

| Option | Verdict |
|---|---|
| α — single ROADMAP.md | Partially adopt — `docs/ROADMAP.md` already exists and is structurally viable as the forward-state narrative home. But alone it does not fix the misclassification/schema gap (Pattern 4). |
| β — per-domain roadmap docs | Reject as primary — multiplies files; the repo already has ROADMAP + IDEAS_RESERVOIR providing the scheduled/dormant split. |
| γ — fenced NON-NORMATIVE roadmap sections | Adopt as complement — for living-spec docs that must retain co-located forward pointers (e.g. ISOLATION "A'.9 planned"), a fenced "§N — Forward roadmap (NON-NORMATIVE)" section. PIPELINE_METRICS/MOD_OS already demonstrate this works. |
| δ — roadmap-in-REGISTER | Adopt as backbone — directly fixes Pattern 4. Extend REGISTER schema with a `doc_role` field (DESCRIPTIVE-SPEC / FORWARD-SPEC / ROADMAP / RESERVED-CONTRACT / SNAPSHOT) and per-doc `forward_work` metadata (machine-navigable). Most PA-001-aligned. |
| ε — hybrid (δ + γ) | **RECOMMENDED.** |

### Recommended separation architecture — **Option ε (δ + γ), anchored on the existing ROADMAP.md**

1. **REGISTER schema extension (δ)** — add `doc_role` to every document, making the descriptive/forward/snapshot distinction a **machine-first** property (not an authoring convention). This resolves the 7-doc misclassification cluster structurally and lets `sync_register.ps1` / future analyzers *enforce* that a DESCRIPTIVE-SPEC doc carries no future-tense roadmap prose.
2. **Scheduled roadmap consolidates into `docs/ROADMAP.md` (α)** — extracted roadmap content (KERNEL Part 2/3, ANALYZER_RULES §5/§6/§10.5/§11, etc.) lands in the already-canonical roadmap doc, which first re-syncs its stale status table. `IDEAS_RESERVOIR.md` remains the dormant-backlog home (the scheduled/dormant split is already correct — preserve it).
3. **Fenced NON-NORMATIVE sections (γ)** — living-spec docs needing co-located forward pointers keep a single clearly-fenced roadmap section that references ROADMAP/REGISTER forward-work IDs, never interwoven into normative body. PIPELINE_METRICS is the reference implementation.
4. **Reclassify Pattern-4 docs** — move forward-contracts/activation-briefs/migration-roadmaps/deliberation-snapshots out of Tier-1 descriptive Category-A into the appropriate `doc_role`.

### Rationale (axiom-anchored)
- **PA-001 (AI-agent-first / machine-first contracts)** → δ backbone: roadmap as queryable REGISTER metadata, not buried prose. Wins.
- **PA-002 (без костылей / clean separation)** → reclassification + fenced sections give every sentence exactly one temporal contract.
- **PA-003 (complexity justified for clean execution)** → the schema extension is justified by 105 line-items + a structural misclassification cluster; this is not gold-plating.
- **PA-004** + existing "docs are machine-first contracts between agents" principle → the recommended architecture makes drift *mechanically detectable* going forward.

---

## §6.8 — Refactor plan (per-document migration)

Legend: **STAY** = remains as spec · **EXTRACT→ROADMAP** = move to docs/ROADMAP.md (or fenced §) · **FIX** = update to code-truth · **RECLASSIFY** = REGISTER doc_role change · **REGEN** = regenerate from tool.

| Document | Action(s) |
|---|---|
| ANALYZER_RULES.md | STAY §1/§2/§3/§9 + corrected §4/§10 registry (FIX overclaim→Info-stub ground truth); EXTRACT→ROADMAP §4.1/4.2 promotion cols, §5, §6, §10.5, §11; EXTRACT→exclusion-layer §7/§8; resolve MOD_API_CONTRACT ref. **Coordinate with A'.9.1 Phase β (§6.10).** |
| KERNEL_ARCHITECTURE.md | STAY Part 0 (untouched, S-LOCK-1) + Part 1 realized structure; EXTRACT→ROADMAP Part 2/3 K-series + header estimates; FIX §1.1 target-tree→realized; reconcile 10-vs-12 systems count. |
| K_CLOSURE_REPORT.md | FIX §7 DF###→DFK### + plural package + infra-realized/detection-pending; otherwise SNAPSHOT-stable. |
| ARCHITECTURE.md | **near-rewrite** to native-kernel reality OR supersede + redirect (decide in DD-1). |
| THREADING.md / EVENT_BUS.md / ECS.md / FEEDBACK_LOOPS.md / PERFORMANCE.md | FIX native scheduler + bus tiers + remove deleted IsolationViolationException/SetComponent/managed ComponentStore; PERFORMANCE additionally EXTRACT→ROADMAP the GPU/pathfinding G5+ blocks. |
| VULKAN_SUBSTRATE.md | FIX §4/§6 body to Vulkan-only present tense; resolve R.8 (delete `project.godot` or correct criterion). |
| FIELDS.md | FIX "lands at K9"→shipped; EXTRACT→ROADMAP save/load milestone. |
| KERNEL_FULL_NATIVE_SCHEDULER.md | RECLASSIFY (deliberation-snapshot/forward); FIX К10 "AUTHORED"→shipped; resolve phantom TLA+/brief refs. |
| MOD_OS / ISOLATION / OWNERSHIP_TRANSITION / RESOURCE_MODELS / MODDING | light FIX only (already well-disciplined); MOD_OS fix §4.7 K9 comment. |
| CONTRACTS.md / MOD_PIPELINE.md | FIX manifest example v1→v3; MOD_PIPELINE fix ./ROADMAP.md link. |
| COMBO_RESOLUTION / COMPOSITE_REQUESTS | structural-separation: fence "design-spec (not yet implemented)" from present-tense behavior prose. |
| MIGRATION_PLAN / FHE_CONTRACT / ARCHITECTURE_TYPE_SYSTEM / TRACK_B_ACTIVATION | RECLASSIFY (Pattern 4); MIGRATION_PLAN also FIX ShieldSystem phantom + body version. |
| FRAMEWORK.md / PROJECT_AXIOMS.md | surface count drifts to Crystalka (LOCKED — no semantic auto-edit); PROJECT_AXIOMS convert line-pins→anchors, verify #14. |
| REGISTER_RENDER.md | REGEN + fix render-script variable-expansion bug. |
| METHODOLOGY.md | FIX frontmatter↔changelog; resolve #N18 absence; refresh doc-count. |
| DEVELOPMENT_HYGIENE.md | **near-rewrite** for post-Godot project set; fix 3 orphan refs. |
| CODING_STANDARDS / TESTING_STRATEGY | qualify analyzer/CI enforcement as roadmap; refresh test-project layout. |
| MAXIMUM_ENGINEERING_REFACTOR.md | mark Track B ACTIVATED (detection pending); fix src/→tools/ path; drop Godot risk. |
| ROADMAP.md | re-sync status table (retire M8/K/V realized rows); receive extracted roadmap content. |
| MIGRATION_PROGRESS.md | re-sync header snapshot to body; fix companion-version pins. |
| TRANSLATION_PLAN.md | FIX IPowerBus/Godot stale inventory; separate plan from descriptive inventory. |
| Scene/Input/Application READMEs | FIX Godot-as-live framing; retarget to Launcher. |

**New document proposals**: none strictly required — `docs/ROADMAP.md` (consolidation home) and `IDEAS_RESERVOIR.md` (backlog) already exist. Optionally a `docs/architecture/ARCHITECTURE_HISTORY.md` could absorb realized closed-phase prose from KERNEL/ROADMAP if Crystalka prefers a clean spec/chronicle split (γ).

---

## §6.9 — REGISTER cascade outline (for the future refactor)

- **Schema extension**: add `doc_role` enum + optional `forward_work` field to the document schema (FRAMEWORK §schema bump → coupled SYNTHESIS/REGISTER schema_version evaluation per §8.2).
- **Reclassifications (7)**: Pattern-4 docs receive `doc_role` = FORWARD-SPEC / ROADMAP / RESERVED-CONTRACT / SNAPSHOT; PHASE_A_PRIME_SEQUENCING + KERNEL_FULL_NATIVE_SCHEDULER lifecycle re-evaluation.
- **Lifecycle transitions**: K_L14_EVIDENCE_DASHBOARD AUTHORED-SKELETON→Tier-2 Live (gate met); stale-skeleton briefs (K10*, A'.9 Roslyn, A'.8 K-closure) → EXECUTED/SUPERSEDED to stop overstating pending work.
- **Version bumps**: every refactored living-spec doc (patch for chronicle/cross-ref per Q-G-12; minor for content extraction). KERNEL Part 0 untouched → patch only.
- **audit_trail**: one EVT per cascade (DD-1/DD-2/DD-3) + register_version increments (2.10 → 2.11 → …).
- **Validation gate**: `sync_register.ps1 -Validate` exit 0 after each cascade; REGISTER_RENDER regenerated.

---

## §6.10 — Work-volume estimate

### Per-document effort tiers
- **Near-rewrite (high)**: ARCHITECTURE, DEVELOPMENT_HYGIENE, ANALYZER_RULES (restructure), VULKAN_SUBSTRATE body — ~4 docs.
- **Moderate (extract/re-sync)**: KERNEL_ARCHITECTURE, ROADMAP, MIGRATION_PROGRESS, THREADING, EVENT_BUS, PERFORMANCE, TRANSLATION_PLAN, REGISTER_RENDER (regen) — ~8 docs.
- **Light (line-fixes/reclassify)**: the remaining ~12–16 docs + 3 module READMEs.

### Sequencing — recommended 3-cascade decomposition (adaptive gate analogous to Q-L-1)

**Cascade DD-1 — Spec-truth restoration** (urgent accuracy; independent, blocks nothing):
Fix the Pattern-1 un-propagated events (Godot, К8.3/8.4, К10) in ARCHITECTURE, THREADING, EVENT_BUS, ECS, FEEDBACK_LOOPS, PERFORMANCE, VULKAN_SUBSTRATE, DEVELOPMENT_HYGIENE, FIELDS + resolve `project.godot`. **No structural changes — pure code-truth alignment.** Can run in parallel with A'.9.1.

**Cascade DD-2 — Spec/roadmap structural separation** (the root remediation):
REGISTER `doc_role` schema extension; ROADMAP.md re-sync + receive extracted content; KERNEL Part 2/3 + ANALYZER_RULES §5/6/10.5/11 extraction; fenced NON-NORMATIVE sections; COMBO/COMPOSITE design-fence. **⚠ Coordinate ANALYZER_RULES portion with A'.9.1 Phase β** (Q-DD-8 / §9 halt-7): Phase β detection-logic implementation will itself convert the §4 "stub→active" status as rules become real, so either (a) let A'.9.1 Phase β land first and fix §4 naturally, then DD-2 does the §5/6/10.5/11 extraction, or (b) bundle the ANALYZER_RULES restructure into A'.9.1 Phase β. **Crystalka ratifies ordering.**

**Cascade DD-3 — Reclassification + orphan + desync hygiene**:
7-doc reclassification; DF###→DFK### living-spec cross-ref fixes; manifest v1→v3 examples; count/version desyncs (METHODOLOGY, MIGRATION_PLAN, FRAMEWORK surface); REGISTER_RENDER regen + render-script bug; module READMEs.

### Interaction with A'.9.1 (the one real dependency)
`ANALYZER_RULES.md` is both the prime dual-load specimen **and** the active A'.9.1 Phase β surface. This is the only hard sequencing coupling. Recommendation: **DD-1 parallel to A'.9.1; DD-2's ANALYZER_RULES portion sequenced with/after A'.9.1 Phase β; DD-3 last.** All read-only reconnaissance findings here block nothing.

---

## §6.11 — Appendices

### Appendix A — Full per-document drift line-item tables
The complete section-level line-items (with code-truth citations and proposed actions) are reproduced inline in §6.3.1–§6.3.8. Each SPEC-STALE / ROADMAP-REALIZED claim carries a file/commit citation. Raw per-cluster sub-agent inventories are preserved in the execution transcript (Appendix D).

### Appendix B — Code-truth manifest full detail
Reproduced in §6.2 (project inventory, key-type table, phase-completion ledger, native/managed surface, orphan-reference verification).

### Appendix C — Cross-reference resolution map (orphan chains)
| Dangling reference | Found in | Resolution |
|---|---|---|
| `MOD_API_CONTRACT.md` | ANALYZER_RULES §5, K_CLOSURE | mark forward-reference (К-L20 deferred) or author when К-L20 locks |
| `docs/formal/SCHEDULER_TLA_PLUS.tla` | KERNEL_FULL_NATIVE_SCHEDULER | remove or mark never-authored |
| `DF001..DF020` | K_CLOSURE §7, KERNEL, ledger, dashboard, sequencing, ANALYZER_RULES | rename → DFK### / DFL### (living-spec only; snapshots exempt) |
| `tools/build-all.sh\|ps1` | DEVELOPMENT_HYGIENE | remove (no such script) |
| CODING_STANDARDS "Commit messages" § | DEVELOPMENT_HYGIENE | fix anchor or inline |
| `./ROADMAP.md` (wrong relative path) | DEVELOPMENT_HYGIENE, MOD_PIPELINE | → `../ROADMAP.md` |
| `PerformanceGates.cs` / `.github/workflows` | TESTING_STRATEGY | reclassify CI claim as roadmap |
| `ShieldSystem` | MIGRATION_PLAN inventory | remove phantom |
| `DualFrontier.Presentation*` | DEVELOPMENT_HYGIENE, Scene/Input READMEs | retarget to Launcher |
| `docs/FHE_INTEGRATION_CONTRACT.md` (code comment) | IHomomorphicComputeProvider.cs | → `docs/architecture/FHE_INTEGRATION_CONTRACT.md` |

### Appendix D — Sub-agent raw inventories (traceability)
10 sub-agents (C1–C9, C6 split a/b) returned structured §7-schema inventories. Aggregate: 8 deep clusters + 2 quick-pass; ~105 living-spec drift line-items; 139-doc snapshot population confirmed clean (0 hard misclassifications); 7-doc misclassification cluster surfaced as meta-finding.

---

## Ratification surface (Q-DD decisions — defaults taken in this execution)

| Q-DD | Decision | This execution |
|---|---|---|
| Q-DD-1 | Cascade formal ID | standalone "Documentation Dual-Load Drift Reconnaissance" (no К-ext number — touched no К-L invariant) |
| Q-DD-2 | Decomposition | 10 clusters (8 deep + 2 quick-pass), finalized from empirical REGISTER R0 counts |
| Q-DD-3 | Code-truth grounding | (c) hybrid |
| Q-DD-4 | Snapshot exemption | confirmed — briefs/reports/recon/logs HISTORICAL-SNAPSHOT, drift-exempt |
| Q-DD-5 | Append-only ledger | §3.5 special handling applied (past entries exempt, forward sections in-scope) |
| Q-DD-6 | Report depth | single comprehensive report (~105 items < halt-5 threshold) |
| Q-DD-7 | REGISTER enrollment | report PLACED with enrollment-ready frontmatter; **REGISTER enrollment left for Crystalka ratification** (this session held read-only discipline — no REGISTER.yaml edit). Proposed: DOC-E Tier-2 Live/AUTHORED. |
| Q-DD-8 | Sequencing vs A'.9.1 | parallel reconnaissance (done); refactor ordering deferred to Crystalka — see §6.10 ANALYZER_RULES coupling |
| Q-DD-9 | Target architecture | report recommends freely → Option ε (δ+γ on existing ROADMAP.md) |
| Q-DD-10 | docs/reports/ folder | existing (report placed there) |

## Halt-condition disposition
- **halt-1** (REGISTER count) — NOT triggered (267 confirmed = expected).
- **halt-3** (misclassification → governance issue) — **TRIGGERED, surfaced** as Pattern 4 meta-finding (§6.4). 7 docs, one consistent direction → schema gap, recommend δ.
- **halt-5** (scope explosion) — NOT triggered (~105 < 1000; single report appropriate).
- **halt-6** (LOCKED Tier-1 substrate) — **respected**: zero К-L invariant text edits required; KERNEL Part 0 untouched. Refactor extracts roadmap/chronicle only.
- **halt-7** (A'.9.1 interaction) — **surfaced**: ANALYZER_RULES.md sequencing coupling explicit in §6.10.

---

**К-L14 thesis preserved.** Read-only discipline held — zero substrate, zero production code, zero existing-document edits. This report (architectural-integrity measurement) is the only artifact written. PA-001 + PA-002 + PA-003 + PA-004 anchored. Без костылей.

**Forward**: Crystalka determines refactor work-volume FROM this report → ratifies Q-DD-8 sequencing (DD-1 parallel / DD-2 vs A'.9.1 Phase β ordering) → (future) DD-1/DD-2/DD-3 refactor cascades separate spec from roadmap, eliminating the structural drift generator.

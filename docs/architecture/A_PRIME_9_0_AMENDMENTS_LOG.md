# A'.9.0 Reconnaissance Cascade — Post-Closure Amendments Log

**Artifact designation**: `A_PRIME_9_0_AMENDMENTS_LOG`  
**Parent cascade**: A'.9.0 Reconnaissance (К-extensions cascade #4 cross-reference)  
**Parent report**: `docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` (Tier 2 Live Category A)  
**Parent brief**: `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md` (DOC-D Tier 3 Category D, status EXECUTED)  
**Amendments authoring date**: 2026-05-24  
**Amendments deliberation session**: Continuation of 2026-05-24 A'.9.0 deliberation session — post-closure Crystalka review surfaced 4 amendments  
**Status**: AUTHORED — pending Crystalka RATIFICATION + push к origin/main

---

## §0 — Provenance + framing

### 0.1 — Why this artifact exists

A'.9.0 Reconnaissance cascade closed 2026-05-24 with 8 atomic commits (`a233639..8a0ec32`) executing brief specification cleanly. Post-execution Crystalka review surfaced **4 amendments** что warrant capture before push к origin/main:

1. **Defect 1** — Numerical inconsistency between report §1.2 executive summary (42 Q-K candidates) и §11 intro (45 total Q-K candidates)
2. **Defect 2** — §2.1 «Phase 0 anomalies surfaced» section duplicated (appears twice with identical content)
3. **5-rule deferral** — DF009/DF012/DF015/DF018/DF020 moved от A'.9.1 first-batch к К-L20 LOCK cascade scope per honest Mod-OS-coupling principle
4. **Test exclusion principle formalization** — Reserved-stub modules excluded от test execution per cascade #3 precedent; mechanism = xUnit `[Trait("Category", "ReservedStub")]` + `dotnet test --filter "Category!=ReservedStub"`; analyzer rule shape DFL025 family proposed

Three options для capturing these amendments were considered:
- **Option α** — Edit report + brief in-place before push (rewrite git history OR new commit exceeding Q-J-8 budget)
- **Option β** — Push as-is + Brief A'.9.1 Phase 0 absorbs amendments
- **Option γ** — Hybrid: push as-is + standalone amendments log captures refinements forward

**Option γ ratified** per Crystalka direction. Reasoning:
- A'.9.0 closure as executed remains historically accurate (no rewrite of committed history)
- Amendments captured timestamped + reasoned (governance discipline preserved)
- Brief A'.9.1 has single forward-input artifact (report + amendments log together)
- Sets precedent для future post-closure refinements (likely к recur)
- К-L14 thesis preserved — no substrate touch, pure governance artifact addition
- Lesson #N13 commit integrity discipline matched — honest discovery + clean correction, не silent fixup

### 0.2 — Authority chain

Authority chain post-amendments:
- `A_PRIME_9_RECONNAISSANCE_REPORT.md` — primary deliverable, captures empirical reconnaissance state at execution time
- **`A_PRIME_9_0_AMENDMENTS_LOG.md`** (this artifact) — post-closure refinements capturing Crystalka review outputs
- Brief A'.9.1 deliberation = **report + amendments log together** as input surface

When amendments log directives conflict with report content, amendments log directives **supersede** для forward sequencing. Report content remains historically accurate как executed.

### 0.3 — Cascade #3 retroactive empirical check outcome (Q3 from deliberation)

Per Crystalka direction Q3: ran Filesystem MCP empirical check к verify cascade #3 deferred dispatch arm test exclusion status.

**Search executed** (Filesystem MCP):
- `tests/` directory tree listing (29 directories enumerated)
- `tests/DualFrontier.Application.Tests/Bridge/` contents (1 file — `VResourceCleanupTests.cs`)
- Pattern searches: `*Dispatch*`, `*Launcher*`, `*PawnState*`, `*ItemSpawned*`, `*TickAdvanced*`, `*RenderCommand*` — **0 matches across all 6 patterns**

**Empirical finding**: **Possibility A confirmed** — cascade #3 excluded deferred dispatch arms от testing **by absence-based discipline** (no test classes exist against `HandlePawnState` / `HandleItemSpawned` / `HandleTickAdvanced` / `RenderCommandDispatcher` / `Launcher`).

**Implications**:
- Cascade #3 К-L14 verification #12 status remains **CLEAN** — no retroactive correction surface
- К-L14 thesis preservation **stronger than initially read** — discipline already applied empirically
- Crystalka direction «не включать в тесты» was interpreted **literally** by cascade #3 execution: tests не existed, не «tests with Skip attribute»
- **Forward concern**: absence-based discipline is **fragile** — future cascades могут drift (developer adds test against reserved-stub без realizing principle)
- Trait-based mechanism (Amendment #4) is **forward discipline strengthening** — codifies what cascade #3 achieved by convention

**Conclusion**: No retroactive cascade #3 amendment required. Forward formalization (Amendment #4) addresses fragility concern.

---

## §1 — Amendment #1: Q-K count divergence correction (Defect 1)

### 1.1 — Defect description

`A_PRIME_9_RECONNAISSANCE_REPORT.md` contains numerical inconsistency between two sections:

**§1.2 Executive Summary (line ~64-71)** states:
> «**42 Q-K candidates** aggregated across all 7 domains' §X.99 subsections for Brief A'.9.1 deliberation»

Followed by per-domain breakdown summing to 42:
- §3.99 К-L domain: 7 Q-Ks
- §4.99 Lessons domain: 5 Q-Ks
- §5.99 Cascade #2/#3 domain: 7 Q-Ks
- §6.99 Mod OS К-L20 domain: 6 Q-Ks
- §7.99 Roslyn ecosystem domain: 6 Q-Ks
- §8.99 Build/CI domain: 6 Q-Ks
- §9.99 Suppression governance domain: 5 Q-Ks
- **Total: 42**

**§11 introduction (line ~3187-3189)** states:
> «**45 total Q-K candidates** (42 from sub-agents + 3 cross-cutting from α4 synthesis)»

§11.8 «Cross-cutting Q-Ks surfaced during α4 synthesis» enumerates Q-K-43 / Q-K-44 / Q-K-45.

**Mismatch**: §1.2 executive summary count (42) **omits** the 3 cross-cutting Q-Ks added during α4 synthesis. §11 actual total (45) is correct per enumeration.

### 1.2 — Risk if uncorrected

Brief A'.9.1 deliberation could lock Q-K surface as 42 candidates (reading executive summary), missing Q-K-43 / Q-K-44 / Q-K-45 (cross-cutting candidates: cascade shape, naming convention, documentation forward propagation). These three are **architecturally significant**:
- Q-K-43 — A'.9.1 cascade shape (single vs three sub-cascades) — biggest architectural decision для A'.9.1
- Q-K-44 — Naming convention dual designation continuation
- Q-K-45 — Documentation forward propagation plan для A'.9.1+ artifacts

Missing these from Brief A'.9.1 lock surface = silent scope gap.

### 1.3 — Correction directive для Brief A'.9.1 deliberation

**Brief A'.9.1 deliberation MUST lock Q-K candidates at total = 45** (per §11 enumeration).

Specific Q-K surface for Brief A'.9.1 deliberation:
- §11.1 К-L domain: Q-K-1 through Q-K-7 (7 candidates)
- §11.2 Lessons domain: Q-K-8 through Q-K-12 (5 candidates)
- §11.3 Cascade #2/#3 domain: Q-K-13 through Q-K-19 (7 candidates)
- §11.4 Mod OS К-L20 domain: Q-K-20 through Q-K-25 (6 candidates) **[note: see Amendment #3 — 5-rule deferral expands this domain's effective scope]**
- §11.5 Roslyn ecosystem domain: Q-K-26 through Q-K-31 (6 candidates)
- §11.6 Build/CI domain: Q-K-32 through Q-K-37 (6 candidates)
- §11.7 Suppression governance domain: Q-K-38 through Q-K-42 (5 candidates)
- §11.8 Cross-cutting α4 synthesis: Q-K-43 / Q-K-44 / Q-K-45 (3 candidates)
- **Plus Amendment Q-K (this log)**: Q-K-46 (test exclusion mechanism formalization) — see Amendment #4

**Brief A'.9.1 effective Q-K total = 46** (post-amendments log addition of Q-K-46).

### 1.4 — Report correction status

Report **NOT edited retroactively** per Option γ Hybrid discipline. Report content historically accurate at execution time; amendments log supersedes для forward sequencing.

**Future forward propagation** (Q-K-45 governance integration): when Brief A'.9.1 closure produces updated report consolidating amendments, §1.2 executive summary count updated к 45+ (whatever cumulative total).

---

## §2 — Amendment #2: §2.1 duplication removal (Defect 2)

### 2.1 — Defect description

`A_PRIME_9_RECONNAISSANCE_REPORT.md` §2 «Reconnaissance scope + methodology executed» contains **§2.1 «Phase 0 anomalies surfaced» section duplicated**:

**First instance** (approximately lines 90-117) — natural position:
- Appears after §2.0 «Execution narrative»
- Before §2.2 «Multi-agent dispatch executed (S-LOCK-5 recommendation applied)»

**Second instance** (approximately lines 186-213) — duplicate:
- Appears after §2.3 «S-LOCK compliance verification»
- Before §3 «К-L invariants analyzability matrix»
- Content identical к first instance verbatim

**Suspected cause**: α4 synthesis commit (`f017455`) appended content к report wholesale без deduplication check. Likely result of editing strategy that prepended/appended new sections without diffing against existing structure.

### 2.2 — Risk if uncorrected

Future agents reading report may interpret duplication as **intentional structure** (e.g., «§2.1 in two locations because anomalies span both pre-execution и post-S-LOCK-verification contexts»). This could lead to:
- Incorrect interpretation of «section structure carries semantic meaning»
- Future report templates copying duplicate-section pattern erroneously
- Wasted reader attention parsing duplicate content
- Citation ambiguity («§2.1» could mean either instance)

For authoritative Tier 2 governance artifact, structural cleanliness matters.

### 2.3 — Correction directive для Brief A'.9.1 deliberation

**Brief A'.9.1 Phase 0 task (post-deliberation, pre-cascade-α): deduplicate report §2.1**.

Specific action:
- Identify both instances of «§2.1 — Phase 0 anomalies surfaced»
- Verify content identity (line-by-line diff)
- **Keep first instance** (lines ~90-117) — natural position after §2.0 execution narrative
- **Delete second instance** (lines ~186-213) — synthesis artifact

This deduplication is mechanical (no architectural decision required) and can be batched с other Brief A'.9.1 Phase 0 cleanup work as single commit.

### 2.4 — Report correction status

Report **NOT edited retroactively** per Option γ Hybrid discipline. Future Brief A'.9.1 Phase 0 cleanup commit absorbs deduplication.

**Optional**: If Brief A'.9.1 deliberation prefers to address pre-push (still in A'.9.0 governance window), Option α (in-place edit + new commit) remains available но requires Q-J-8 budget amendment (would be 9th commit, exceeds 4-8 budget) OR γ1 commit history rewrite.

**Default per Option γ ratification**: defer к Brief A'.9.1 Phase 0.

---

## §3 — Amendment #3: 5-rule deferral от A'.9.1 first-batch к К-L20 LOCK cascade

### 3.1 — Deferral directive

Per Crystalka direction 2026-05-24 (post-execution review): **5 rules deferred** от A'.9.1 first-batch implementation к К-L20 LOCK cascade scope:

```
DF009 — Vanilla = mods (К-L9 mod parity)
DF012 — native scheduler sovereignty (К-L12)
DF015 — bus capability declaration (К-L15 three-tier dispatch + capability surface)
DF018 — mod unload quiescence sequence (К-L18 lifecycle)
DF020 — Mod API forward compatibility (К-L20 family — 20 sub-rules)
```

### 3.2 — Honest scoping rationale

Common architectural property uniting these 5 rules: **all Mod-OS-coupled** — depend on Mod API surface stability которая not formally LOCKED until К-L20 milestone.

Per-rule rationale:

**DF009 (К-L9 Vanilla = mods)**:
- Mod parity surface defines what «IModApi» means
- Pre-К-L20 LOCK: IModApi surface volatile (Mod API contract not finalized)
- К-L20 LOCK clarifies what «same surface for vanilla + third-party» actually means
- **Pre-LOCK shipping = analyzer rule against moving target**
- Report §11.5 Q-K-26 already recommended Error → Warning downgrade until К-L20 LOCK; deferral is stronger discipline

**DF012 (К-L12 native scheduler sovereignty)**:
- К-L12 scheduling authority gates всё mod facade enforcement
- Managed-side scheduler facade contract (К-L9 «facade preserves Vanilla = mods») depends on К-L20 LOCK для definition
- Pre-LOCK: facade boundary не finalized — analyzer cannot verify «mod stays on managed-side, не touches native scheduler» without stable boundary

**DF015 (К-L15 bus capability declaration)**:
- К-L15 capability surface ties tightly к К-L20 mod API capabilities (capability tokens, FQN scoping, tier registration)
- Pre-К-L20 LOCK: capability vocabulary не finalized
- Analyzer rule against capability syntax requires stable vocabulary — premature pre-LOCK

**DF018 (К-L18 mod unload quiescence sequence)**:
- Lifecycle sequence (`PauseAsync` → `WaitForQuiescenceAsync` → operation → `ResumeAsync`) is part of К-L20 Mod API contract
- К-L20 LOCK may refine sequence (К10.3 v2 §9.5 unload chain extended 8 → 9 step с V resource cleanup placeholder per K_CLOSURE §2.21)
- Pre-LOCK: sequence may still change

**DF020 (К-L20 family)**:
- Already deferred per K_CLOSURE §7.3 + ANALYZER_RULES.md §1 «reserved post-Mod API lock»
- Domain 4 §6.2 enumerated 20 candidate DF020 sub-rules but K-L20 LOCK cascade narrows/expands при deliberation
- This amendment **explicitly groups** DF020 family с other 4 Mod-OS-coupled rules as «К-L20 LOCK cascade batch» rather than scattered deferrals

**Architectural cleanliness**: separating «foundational architectural rules» (storage, span protocol, bootstrap, type IDs, native bindings) от «mod-coupled rules» (parity, scheduler-facade, bus-capability, lifecycle, API surface) is **honest sequencing**. Foundation ships first; Mod surface waits для К-L20 stability.

### 3.3 — Refined A'.9.1 first-batch composition

**Original Brief A'.9.1 first-batch per report §10 Prerequisite 1**: 17 rules (9 P0 + 8 P1)
- P0: DF001, DF002, DF003, DF003.1, DF004, DF005, DF007, DF011, DF015
- P1: DF007.1, DF009, DF010, DF012, DF015.1, DF017, DF018, DF019

**Post-deferral A'.9.1 first-batch**: **13 rules** (8 P0 + 5 P1)
- **P0** (8 rules): DF001, DF002, DF003, DF003.1, DF004, DF005, DF007, DF011
- **P1** (5 rules): DF007.1, DF010, DF015.1, DF017, DF019
- **Deferred к К-L20 LOCK cascade** (5 rules): DF009, DF012, DF015, DF018, DF020 family

### 3.4 — Refined К-L20 LOCK cascade batch

**Original К-L20 LOCK cascade scope** (per Domain 4 §6.2): 20 candidate DF020 sub-rules

**Post-deferral К-L20 LOCK cascade scope**: **5 K-L-specific rules + 20 DF020 sub-rules = up to 25 rules**
- **K-L-specific (4 rules)**: DF009 (К-L9 mod parity), DF012 (К-L12 native scheduler sovereignty), DF015 (К-L15 bus capability), DF018 (К-L18 lifecycle quiescence)
- **DF020 family (20 sub-rules)**: namespace/type restrictions, API usage restrictions, manifest field static cross-check rules, forward-compatibility grace-period rules

К-L20 LOCK cascade thus becomes **substantial cascade** — likely multi-stage decomposition (analogous к A'.9.1 ξ/χ/ψ progression per §8.4 recommendation). Brief К-L20 LOCK deliberation will scope this honestly.

### 3.5 — Brief A'.9.1 deliberation directives

Brief A'.9.1 deliberation MUST ratify:
1. **Refined first-batch = 13 rules** (per §3.3 above), не 17
2. **К-L20 LOCK cascade reservation** для 5 deferred rules + DF020 family
3. **Report §10 Prerequisite 1 effective overrides** к amendments log §3.3
4. **Report §10 Prerequisite 7 cascade decomposition** updated:
   - A'.9.1 ships 13-rule batch
   - A'.9.2 may include cleanup phase + severity promotion + optional code-fix providers
   - A'.9.N retains DC###/DL### cascade-derived + Lesson-derived auxiliary rules
   - **К-L20 LOCK cascade** explicit reserved slot для 5 deferred + 20 DF020 family rules

### 3.6 — Cleanup-phase χ violation count impact

Report §11.8 Q-K-43 (A'.9.1 cascade shape — single vs three sub-cascades) decision depends on cleanup-phase χ violation count estimate (50-200 violations across 17 rules × 12 src projects).

**With deferral к 13-rule batch**:
- Smaller rule set → smaller violation surface
- Estimated reduction: ~24% rules deferred (4 of 17, excluding DF020 which was already P2 deferred)
- Likely χ violation count reduction proportional or larger (Mod-OS-coupled rules likely had high violation count в Launcher composition)
- **Q-K-43 default shifts**: more likely к **(a) Single A'.9.1 cascade с ξ/χ/ψ phases** rather than three sub-cascades

Brief A'.9.1 deliberation should empirically estimate χ violation count post-scaffold (analyzer project scaffolded with 13 rules implemented stub-form; dry-run violation enumeration) before locking cascade shape.


---

## §4 — Amendment #4: Test exclusion principle для reserved-stub modules

### 4.1 — Principle statement (verbatim Crystalka direction 2026-05-24)

> «Все модули с заглушками, которые необходимы для билда требуется исключить из тестирования, так как там нечего тестировать, и это лишь будет врать о проделанной работе»

**Principle decoded**:
- Modules containing reserved-stub patterns (Lesson #N12 — defensive throws OR silent stubs) shipped solely для build composition requirements
- Such modules excluded от test execution by default
- Rationale: stubs implement no meaningful behavior к test; tests against stubs are **vacuous** = **lying tests** (false claim of «work done» where work не done)

### 4.2 — Theoretical foundation — Lesson #25 refined direct extension

Lesson #25 refined (METHODOLOGY v1.10+) established «tests must not lie about behavior». This principle **extends Lesson #25 refined to reserved-stub modules** with specific mechanism.

**Lying-test taxonomy для reserved-stub modules**:

**Pattern A — Defensive throw stubs**:
- Test exists против defensive-throw method → either passes (testing throw semantics только — lying about meaningful work) OR fails (failure is expected behavior, not bug — lying about regression)
- Either outcome **misrepresents «work done»** к future agents

**Pattern B — Silent stubs (cascade #3 sub-pattern B precedent)**:
- Test asserts «code did nothing» (silent stub behavior)
- 100% false positive — test reports «behavior works» when behavior is null
- **Most insidious form** — looks like passing test green-tick, indicates nothing

**Both patterns lie**. Test exclusion is honest discipline.

### 4.3 — Cascade #3 precedent (empirical evidence)

Cascade #3 already applied this principle:
- 3 deferred dispatch arms (HandlePawnState / HandleItemSpawned / HandleTickAdvanced) marked as silent stubs (Lesson #N12 sub-pattern B per cascade #3 mid-cascade ratification)
- Crystalka direction at cascade #3: «**поставить заглушку, главное не включать в тесты**»
- **Empirical verification (this deliberation Q3)**: Filesystem MCP searches confirmed **zero tests exist** against deferred arms, RenderCommandDispatcher, or Launcher
- **Cascade #3 К-L14 #12 status verified CLEAN** — no retroactive correction needed

**Discipline strength assessment**: Cascade #3 achieved exclusion **by absence** (no test classes ever created). This is **fragile discipline** — future cascades могут drift (developer adds test against reserved-stub без realizing principle). Formal mechanism (this amendment) strengthens discipline forward.

### 4.4 — Mechanism ratification (Q4 from deliberation)

**Mechanism**: **xUnit `[Trait("Category", "ReservedStub")]` + `dotnet test --filter "Category!=ReservedStub"`**

Per Crystalka ratification: «(b) xUnit `[Trait("Category", "ReservedStub")]` + dotnet test filter — удобно так как сразу понятно для чего».

**Why this mechanism wins over alternatives**:

1. **Self-documenting at point of code** — `[Trait("Category", "ReservedStub")]` сразу читается future reader: «это тест против reserved-stub module — skipped by default».
2. **Discoverable via standard tooling**:
   - `dotnet test --filter "Category=ReservedStub"` — explicit diagnostic run (rare)
   - `dotnet test --filter "Category!=ReservedStub"` — default exclusion от CI/closure protocol
   - Visual Studio / Rider Test Explorer shows category groupings natively
3. **No csproj surgery** — single attribute application, no per-module exclusion drift
4. **Analyzer rule shape simple** — T2 syntax tree analysis (per S-LOCK-4 rubric); trivial code-fix feasibility
5. **Closure protocol integration clean** — METHODOLOGY §12.7 verification gate uses filter as default invocation
6. **Composable** — multiple Trait attributes per test compose без conflict (e.g., `[Trait("Tier", "Integration")]`)

### 4.5 — Analyzer rule shape proposal — DFL025 family

DFL025 namespace (Lesson-derived auxiliary rules — see Q-K-8 in report §11.2; namespace ratification deferred к Brief A'.9.1 deliberation):

**DFL025-A — Test class against reserved-stub-marked module MUST carry `[Trait("Category", "ReservedStub")]` attribute**:
- **Detection**: test class имеет method invoking type marked с `[ReservedStub]` attribute (cascade #2/#3 attribute infrastructure per Q-K-14 report §11.3)
- **Tier**: T2 syntax tree analysis (analyzability rubric per S-LOCK-4)
- **Priority**: P1 (high — codifies cascade #3 discipline)
- **Severity**: Warning (encourages discipline; doesn't block build initially; promotes к Error post-cleanup)
- **Code-fix feasibility**: Trivial — analyzer auto-adds Trait attribute к test class
- **False-positive risk**: Low — clear detection pattern

**DFL025-B — Test methods против reserved-stub modules SHOULD use `[Fact(Skip="...")]` if standalone**:
- Edge case: single test method without class-level grouping
- **Tier**: T2 syntax tree analysis
- **Priority**: P2 (medium — fringe case)
- **Severity**: Suggestion
- **Code-fix feasibility**: Trivial

**DFL025-C — Closure protocol verification scripts MUST pass `--filter "Category!=ReservedStub"` к dotnet test**:
- **Note**: Shell-level rule, **NOT Roslyn analyzer**
- **Tier**: T6 (process invariant — alternative tooling `tools/governance/check_test_invocation.ps1` или similar)
- **Priority**: P1 — governance discipline reminder
- **Severity**: Suggestion / Warning (script linter level)
- Defer к Brief A'.9.1 deliberation для exact tooling decision

### 4.6 — Brief A'.9.1 deliberation directives

Brief A'.9.1 deliberation MUST ratify:

1. **DFL025 family inclusion в A'.9.1 cascade scope** — analyzer rule shape (DFL025-A + DFL025-B) shipped at A'.9.1 alongside DF### foundational rules; DFL025-C deferred к governance tooling cascade
2. **`[ReservedStub]` attribute infrastructure** (per Q-K-14 report §11.3) becomes **mandatory prerequisite** для DFL025 detection — analyzer infrastructure cascade must include attribute introduction
3. **METHODOLOGY v1.13+ codification path** — test exclusion principle becomes formal Lesson surface candidate:
   - (a) Lesson #25 refined extension sub-clause «test exclusion for reserved-stub modules»
   - (b) New FORMALIZE candidate Lesson #N17 «Reserved-stub test exclusion discipline»
   - (c) Defer formal codification к Brief A'.9.1 deliberation post-empirical-application (current default per Q2 ratification)
4. **Cascade #3 retroactive applicability**: cascade #3 deferred dispatch arms get `[Trait("Category", "ReservedStub")]` attribute retroactively if/when tests are added для them in future (currently absent — no retroactive action needed)

### 4.7 — Lesson #25 refined extension proposal text

**Provisional METHODOLOGY v1.13+ codification candidate**:

> **Lesson #25 refined (3rd extension)** — Test exclusion principle для reserved-stub modules:
> Modules containing reserved-stub patterns (Lesson #N12 — defensive throws OR silent stubs) ship solely для build composition. Such modules MUST be excluded от test execution by default. Tests против reserved-stub modules are **vacuous**: they assert «code does nothing meaningful» which is **not work done**, regardless of test outcome (pass = misrepresenting null behavior as «working»; fail = misrepresenting expected behavior as «regression»).
> **Mechanism**: xUnit `[Trait("Category", "ReservedStub")]` on test class; closure protocol `dotnet test --filter "Category!=ReservedStub"` default invocation. Analyzer rule DFL025 family enforces.
> **First formal application**: cascade #3 (2026-05-24) silent stubs для PawnState/ItemSpawned/TickAdvanced dispatch arms — discipline applied by absence (no tests existed); Brief A'.9.1 codifies mechanism forward.
> **Second formal application**: Brief A'.9.1 (TBD) — analyzer infrastructure cascade likely produces reserved-stub modules для build composition (analyzer-self exclusion, Fixture exclusion patterns) — first explicit `[Trait]`-based exclusion application.

This text serves as **draft proposal** для Brief A'.9.1 deliberation METHODOLOGY codification decision. Final wording deferred к Brief A'.9.1 closure (per Q2 (c) ratification — formal codification post-empirical-evidence).

### 4.8 — New Q-K candidate added к report §11 forward surface

**Q-K-46** (NEW — added by Amendment #4 к §11 forward surface): 

**Test exclusion principle formalization target** — Lesson #25 refined extension vs new Lesson #N17 vs deferred codification.
- **Context**: Amendment #4 establishes principle + mechanism + analyzer rule shape; formal METHODOLOGY codification target options listed §4.6 #3 above
- **Options**:
  - (a) Lesson #25 refined extension sub-clause (preserves Lesson evolution arc; smaller METHODOLOGY churn)
  - (b) New Lesson #N17 FORMALIZE candidate (distinct principle warrants own designation; matches Lesson #N12 sub-pattern A/B precedent)
  - (c) Defer formal codification к Brief A'.9.1 closure post-empirical-application (current default per Q2 ratification)
- **Recommendation**: **(c) defer** per Q2 ratification — promotion criterion requires «substantially different second application empirical evidence» (cascade #3 = first application by absence; Brief A'.9.1 = second application с formal mechanism). FORMALIZE promotion при Brief A'.9.1 closure honest.

**Brief A'.9.1 effective Q-K total post-Amendment #4 = 46** (45 from §11 + Q-K-46 new).

---

## §5 — Cumulative impact on Brief A'.9.1 deliberation

### 5.1 — Pre-deliberation cleanup tasks (Brief A'.9.1 Phase 0)

Brief A'.9.1 deliberation MUST include Phase 0 cleanup commit addressing:

1. **Defect 2 deduplication** (per Amendment #2): remove duplicate §2.1 from report (lines ~186-213)
2. **§1.2 executive summary correction** (per Amendment #1): update Q-K count к 45 + acknowledge cross-cutting Q-Ks per §11.8
3. **Optional §10 + §11 footnote**: cross-reference amendments log explicitly so future report-only readers find amendments

These cleanup tasks are **mechanical** — no architectural decision required; single commit absorbs all three.

### 5.2 — Brief A'.9.1 scope refinements

Brief A'.9.1 deliberation MUST address:

1. **Refined first-batch = 13 rules** (per Amendment #3 §3.3) — 8 P0 + 5 P1, excluding 5 Mod-OS-coupled rules
2. **К-L20 LOCK cascade reservation** (per Amendment #3 §3.4) — 4 K-L-specific + 20 DF020 sub-rules deferred к dedicated cascade
3. **DFL025 family analyzer rules** (per Amendment #4 §4.5) — DFL025-A + DFL025-B shipped at A'.9.1; DFL025-C deferred к governance tooling cascade
4. **`[ReservedStub]` attribute infrastructure mandatory prerequisite** (per Amendment #4 §4.6 #2) — analyzer infrastructure cascade must include attribute introduction
5. **METHODOLOGY v1.13+ codification path** для test exclusion principle (per Amendment #4 §4.6 #3) — defer per Q2 (c) ratification

### 5.3 — Updated cascade decomposition forward roadmap

**A'.9.1 — Analyzer Infrastructure cascade** (refined scope):
- Phase α (ξ scaffolding) — analyzer project + base classes + test framework + 4 attribute infrastructure (`[ReservedStub]` + `[MarkerInterface]` + ratified attributes per Q-K-14)
- Phase β (χ cleanup OR continued α) — 13 DF### rules implementation + DFL025 family (DFL025-A + DFL025-B)
- Phase γ — `.editorconfig` baseline + suppression cleanup + DF999 self-policing rule
- Phase δ — governance cascade (KERNEL v2.5.3+ chronicle, LEDGER, SEQUENCING, REGISTER, K_L14 #14 если applicable)
- Phase ε — closure + verification

**A'.9.2 — Cleanup + Severity Promotion cascade**:
- Cleanup-phase χ completion (если split от A'.9.1 per Q-K-43 decision)
- Severity promotion (suggestion → error)
- Optional code-fix providers для Trivial-feasibility rules

**A'.9.3+ — DC###/DL### cascade-derived + Lesson-derived rules**:
- IRenderCommand marker pattern (DFC001)
- Defensive Reserved Stub Pattern с `[ReservedStub]` (DFL025 family extension)
- Banned-namespace via BannedApiAnalyzer (per Q-K-16 recommendation)
- PublicApiAnalyzers adoption (per Q-K-18 recommendation)
- Other cascade-surfaced rules per report §5

**Post-A'.9 / V-extension cascade**:
- Per Crystalka «расширять V» direction
- Substrate work between A'.9 и К-L20 LOCK

**К-L20 LOCK cascade**:
- 4 K-L-specific deferred rules (DF009, DF012, DF015, DF018)
- 20 DF020 family sub-rules (namespace/type + API usage + manifest cross-check + grace period)
- MOD_API_CONTRACT.md Tier 1 LOCK transition
- Substantial cascade — likely multi-stage decomposition

**Post-К-L20 — RT discussion + implementation**:
- RTX через Vulkan extension (pure substrate extension per Crystalka 2026-05-24)
- Pathfinding + ballistics через RT cores
- «Позже» per Crystalka direction

---

## §6 — Closure protocol для this amendments log

### 6.1 — Authoring sequence

1. **Amendments log authored** (this artifact) — staging via bash `/home/claude/staging/` per established pattern
2. **Crystalka ratification** — review amendments log content + structure
3. **Commit** — single atomic commit adding amendments log к `docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md`
4. **REGISTER cascade** — enrollment as Tier 4 Category D AUTHORED + register_version bump 2.6 → 2.7 + audit_trail event
5. **Push к origin/main** — final commit pushed alongside existing 8 A'.9.0 commits

### 6.2 — Atomic commit specification

**Commit message template** для amendments log addition:

```
governance(amendments): A_PRIME_9_0_AMENDMENTS_LOG — Post-closure 4-amendment capture

Per Crystalka post-execution review 2026-05-24: 4 amendments surfaced
require capture before push к origin/main. Option γ Hybrid amendments log
selected (preserves A'.9.0 closure integrity + provides Brief A'.9.1
forward-input artifact).

Added:
- docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md (Tier 4 AUTHORED Category D)

Amendments captured:
- Amendment #1: Q-K count divergence correction (Defect 1 — 42 vs 45)
- Amendment #2: §2.1 duplication removal (Defect 2 — synthesis artifact)
- Amendment #3: 5-rule deferral от A'.9.1 first-batch к К-L20 LOCK cascade
  (DF009 + DF012 + DF015 + DF018 + DF020 family; Mod-OS-coupling rationale)
- Amendment #4: Test exclusion principle formalization (Crystalka direction
  + mechanism = xUnit Trait + dotnet test filter + DFL025 family analyzer rules)

Cascade #3 retroactive empirical check (Q3): Possibility A confirmed —
cascade #3 К-L14 #12 status remains CLEAN; no retroactive correction needed.

Brief A'.9.1 deliberation directives:
- Phase 0 cleanup tasks (Defect 1 + Defect 2 corrections)
- Refined first-batch = 13 rules (not 17)
- К-L20 LOCK cascade reservation для 5 deferred rules
- DFL025 family analyzer rules ship at A'.9.1
- Q-K-46 new candidate (test exclusion formalization target)

S-LOCK-1 satisfied (zero production code touched).
К-L14 thesis preserved (governance artifact only).
Build verified: dotnet build exit 0.
sync_register.ps1 --validate gate: will pass post-REGISTER cascade.
```

### 6.3 — REGISTER cascade specification

**REGISTER mutations**:

**Addition**:
- `DOC-D-A_PRIME_9_0_AMENDMENTS_LOG` (Tier 4 AUTHORED Category D)

**Frontmatter** (auto-synced via sync_register.ps1):
```yaml
register_id: DOC-D-A_PRIME_9_0_AMENDMENTS_LOG
category: D
tier: 4
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
last_modified: "2026-05-24"
last_modified_commit: "PENDING-COMMIT"
content_language: mixed
review_cadence: on-Brief-A'.9.1-deliberation
last_review_date: "2026-05-24"
last_review_event: "A'.9.0 post-closure amendments authored — 4 amendments captured per Crystalka review; Option γ Hybrid"
next_review_due: Brief A'.9.1 deliberation session
reviewer: Crystalka
special_case_rationale: "Post-closure amendments artifact preserving A'.9.0 cascade closure integrity while capturing 4 refinements forward для Brief A'.9.1 deliberation input"
```

**Metadata**:
- `register_version`: 2.6 → 2.7
- New EVT в audit_trail:

```yaml
- event_id: EVT-2026-05-24-A_PRIME_9_0_AMENDMENTS
  date: "2026-05-24"
  type: governance_amendment
  cascade: A'.9.0 Reconnaissance (post-closure)
  title: "A'.9.0 Post-Closure Amendments Capture (4 amendments)"
  summary: "Post-execution Crystalka review surfaced 4 amendments: Defect 1 (Q-K count divergence), Defect 2 (§2.1 duplication), 5-rule deferral (Mod-OS-coupled rules → К-L20 LOCK cascade), Test exclusion principle formalization (xUnit Trait + DFL025 family). Option γ Hybrid amendments log captures forward для Brief A'.9.1 deliberation без rewriting committed history."
  affected_docs:
    - DOC-D-A_PRIME_9_0_AMENDMENTS_LOG (newly enrolled)
  decisions_ratified:
    - Option γ Hybrid amendments log structure
    - Section-per-amendment format (Q5 (i))
    - 5-rule deferral (DF009, DF012, DF015, DF018, DF020 family) к К-L20 LOCK cascade
    - Test exclusion mechanism = xUnit Trait + dotnet test filter (Q4 (b))
    - Formal codification deferred к Brief A'.9.1 closure (Q2 (c))
    - Cascade #3 retroactive empirical check ran (Q3) — Possibility A confirmed CLEAN
  lessons_surfaced:
    - Lesson #N13 commit integrity discipline applied (honest discovery + clean correction, не silent fixup)
    - Lesson #N14 fourth application candidate (empirical state coverage — cascade #3 retroactive check via Filesystem MCP)
    - Lesson #25 refined 3rd extension candidate (test exclusion principle для reserved-stub modules — METHODOLOGY v1.13+ codification target)
  k_l_impact: "К-L count unchanged: 21. К-L14 evidence count unchanged: 13 (no new substrate touch; pure governance addition)."
  verification:
    type: "Governance amendment (post-cascade-closure refinement capture)"
    criterion: "Option γ Hybrid discipline preservation — closure integrity + forward input artifact"
    status: "[TBD — populate post-ratification]"
  parent_cascade: A'.9.0 / К-extensions cascade #4
  amendments_log: docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md
  cross_references:
    - docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md (primary deliverable, historically accurate)
    - tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md (status EXECUTED)
```

### 6.4 — К-L14 thesis preservation note

This amendments log:
- **Zero production code touched** — pure governance artifact addition
- **Zero substrate API surface modified** — no К-L change
- **Zero test code touched** — discipline directive, не test infrastructure change
- **К-L14 evidence count unchanged**: 13 (cascade #3 #12 + A'.9.0 #13 baseline preserved)

**К-L14 thesis preserved**. Honest framing: amendments log is **governance refinement artifact**, not architectural change. К-L14 evidence framework continues unmodified.

### 6.5 — Brief A'.9.1 deliberation startup readiness

Post-push, Brief A'.9.1 deliberation input surface:
- **`A_PRIME_9_RECONNAISSANCE_REPORT.md`** — empirical reconnaissance baseline (historically accurate at execution time)
- **`A_PRIME_9_0_AMENDMENTS_LOG.md`** (this artifact) — post-closure refinements + directives
- **Combined effective scope**: 46 Q-K candidates + 10 Brief A'.9.1 prerequisites + 4 amendments directives + Phase 0 cleanup task list

Brief A'.9.1 deliberation may start immediately после Crystalka ratification of this amendments log + push к origin/main.

---

## §7 — Cross-references

### 7.1 — Parent artifacts

- `docs/architecture/A_PRIME_9_RECONNAISSANCE_REPORT.md` (Tier 2 Live Category A) — primary A'.9.0 deliverable
- `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md` (DOC-D Tier 3 EXECUTED) — A'.9.0 brief

### 7.2 — Forward artifacts (to-be-authored)

- `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` — Brief A'.9.1 (to be authored против report + this amendments log)
- `docs/architecture/MOD_API_CONTRACT.md` (Tier 2 AUTHORED-SKELETON) — pre-authored at A'.9 milestone closure per Q-K-25 recommendation

### 7.3 — Authoritative governance artifacts

- `docs/architecture/KERNEL_ARCHITECTURE.md` v2.5.3 (post-A'.9.0 closure)
- `docs/methodology/METHODOLOGY.md` v1.12 (cascade #3 closure; v1.13+ candidate for test exclusion codification at Brief A'.9.1 closure per Q2 (c))
- `docs/architecture/K_EXTENSIONS_LEDGER.md` (§3.5 cascade #4 entry)
- `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` (#13 entry)
- `docs/governance/REGISTER.yaml` register_version 2.7 (post-amendments-log)

### 7.4 — Cascade #3 retroactive check trail (Q3 empirical)

Filesystem MCP searches executed 2026-05-24:
- `tests/` directory listing — 29 directories (10 test projects + 19 Fixture.* projects; no Launcher-specific test project)
- `tests/DualFrontier.Application.Tests/Bridge/` — 1 file (`VResourceCleanupTests.cs`)
- Pattern searches (all returned 0 matches): `*Dispatch*`, `*Launcher*`, `*PawnState*`, `*ItemSpawned*`, `*TickAdvanced*`, `*RenderCommand*`

**Conclusion**: Cascade #3 К-L14 #12 status remains CLEAN. No retroactive correction surface.

---

## §8 — Appendix — Deliberation question lock summary

Per deliberation session 2026-05-24 (continuation post-A'.9.0 execution):

| Q | Decision | Rationale |
|---|---|---|
| Q1 | Option γ Hybrid amendments log | Preserves A'.9.0 closure integrity без rewriting committed history |
| Q2 | (c) Defer formal METHODOLOGY codification к Brief A'.9.1 closure | Promotion criterion requires second formal application empirical evidence |
| Q3 | Run cascade #3 retroactive empirical check now | Better discover Possibility C now than during Brief A'.9.1 Phase 0 surprise |
| Q4 | (b) xUnit Trait + dotnet test filter mechanism | Self-documenting + standard tooling + no csproj surgery + analyzer rule simple |
| Q5 | (i) Section-per-amendment format | Clear per-amendment provenance для future agents |

**Empirical findings post-Q3**:
- Possibility A confirmed (cascade #3 excluded by absence)
- К-L14 #12 status CLEAN preserved
- Forward fragility concern → Amendment #4 mechanism addresses

---

**End of A'.9.0 Reconnaissance Cascade Amendments Log**

**Authoring metadata**:
- Authored: 2026-05-24 by Claude Opus 4.7 (deliberation mode)
- Authored on behalf of: Crystalka (Volodymyr, solo dev)
- Authoring session: A'.9.0 deliberation session continuation 2026-05-24 (post-execution review)
- Authoring filesystem: bash staging pattern (`/home/claude/staging/` → `/mnt/user-data/outputs/`)
- Project: Dual Frontier (Crystalka228/Dual-Frontier)
- К-L14 thesis preservation: zero substrate touch; governance artifact only

**Status at authoring**: AUTHORED — pending Crystalka RATIFICATION + commit + REGISTER cascade + push к origin/main  
**Status transitions**: AUTHORED → RATIFIED (Crystalka) → COMMITTED → PUSHED → CONSUMED (Brief A'.9.1 deliberation)

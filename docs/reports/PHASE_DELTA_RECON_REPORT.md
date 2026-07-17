---
register_id: DOC-E-A_PRIME_9_1_PHASE_DELTA_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-07-01
last_modified: 2026-07-01
content_language: en
next_review_due: null
title: "PHASE DELTA RECON REPORT — 2026-07-01 (R1-R8) — pre-brief reconnaissance for the A'.9.1 Phase δ governance closure (base state; METHODOLOGY lesson-entry shapes + #N census + per-lesson source tables + the #N17 transcription source; dashboard #14 shape + Live gate arithmetic; arc-closure surface + phase-hash table + ANALYZER_RULES promotion blocker; F-ledger sweep F-7/F-27 full texts; rider shapes; 12 anomalies + scale estimate)"
last_modified_commit: f2841c1
review_cadence: none-historical-record
last_review_date: 2026-07-02
last_review_event: Read-only durable recon 2026-07-01 (validate deliberately NOT run — the known write-trap; REGISTER.yaml read directly; every census expression recorded verbatim). Its R2.4 per-lesson source tables and R4.3 phase-hash table were load-bearing brief inputs (brief preamble names them); the §2 re-verify set confirmed at Phase 0 of the execution session (HEAD 4cc5e7e = origin/main, register 2.20 / 279 DOC / 43 EVT — H1 clear). Enrolled at the arc-closure REGISTER cascade.
reviewer: Crystalka
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedent: DOC-E-A_PRIME_9_1_PHASE_GAMMA_RECON_REPORT — the first durable-report recon; this is the second). Basis of DOC-D-A_PRIME_9_1_PHASE_DELTA_BRIEF; substituted for a survey wave per the brief preamble.'
---

# PHASE DELTA RECON REPORT -- 2026-07-01

Pre-brief reconnaissance for the A'.9.1 Phase delta governance closure. Read-only session;
the only file created is this report (untracked; the Phase delta cascade enrolls it at C1).
Every census records its expression verbatim. `sync_register.ps1 -Validate` was NOT run
(known write-trap); REGISTER.yaml read directly.

---

## R1 Base state

- **Branch**: `main`. **HEAD**: `4cc5e7ee7f1970e1bb8680daca172220b2fb428a` (`4cc5e7e`,
  `governance(roadmap): ledger Phase gamma execution residue (F-27)`).
- **HEAD == origin/main** (`git rev-parse origin/main` -> `4cc5e7e...`). The Phase gamma
  cascade, recorded at its closure as not-pushed, has since been pushed by the operator —
  the kickoff's "or ahead" branch resolves to **at origin**.
- **Tree**: clean (`git status --porcelain=v1` -> empty). No WIP, no stray unstaged files.
- **Register**: `register_version: "2.20"` (REGISTER.yaml:11), `last_modified: 2026-07-01`,
  `last_modified_commit: "8b26925"`.
- **Document count**: **279** — expression: `^  - id: DOC-` on `docs/governance/REGISTER.yaml` -> 279.
- **EVT count**: **43** — expression: `^  - id: EVT-` on `docs/governance/REGISTER.yaml` -> 43.
- Matches the gamma-closure EVT self-description (register 2.20, 279 docs, C1–C6 recorded REAL,
  single sanctioned self-reference backfilled at C8 render).

---

## R2 METHODOLOGY + lesson candidates

### R2.1 Document state

- `docs/methodology/METHODOLOGY.md`: frontmatter **v1.13.0**, lifecycle **LOCKED**, Tier 1,
  Category B, `DOC-B-METHODOLOGY`. 1116 lines (`wc -l`).
- **Bump-class precedents** (from the changelog itself):
  - v1.9 -> **v1.10** (2026-05-23): the A'.8 batch of **12 FORMALIZE + 9 DEFER + 1 SUNSET** = **MINOR**.
  - v1.12 -> **v1.12.1** (2026-05-24): ONE Provisional appended (#N17) + #N14 4th application = **PATCH**.
  - v1.12.1 -> **v1.13.0** (2026-06-11): two new sections (§12.8/§12.9) + one checklist item,
    "no existing rule inverted" = **MINOR**.
  - §10 change-history law reserves **major** versions for "substantial methodological shifts
    (changes to pipeline configuration, changes to role distribution, additions or removals of
    methodological devices)".
  - **Conclusion**: four lesson formalizations at delta = **MINOR 1.13.0 -> 1.14.0**, exactly the
    v1.10 precedent class. Nothing in the amendment conventions demands otherwise.

### R2.2 Lesson entry formats on disk (the templates the four new entries must reconcile with)

**Shape 1 — formalized full-section H4** (Phase A' lessons). One complete entry verbatim
(METHODOLOGY.md:885-891):

> #### Lesson #7 — A brief that prescribes an API must transcribe the API, not paraphrase it
>
> When a brief tells the executor to call a constructor, a helper, or a file path, the brief author must open the actual source and copy the real signature into the brief at authoring time. «K2-era registry ready» is a note, not a signature. CAPA-2026-05-13's lesson («read entry-point files in full») addressed transitional-state comments; this lesson addresses *API surface*. A brief is a contract for mechanical execution; a contract cannot reference an interface it has not read.
>
> **Origin**: A'.5 K8.3+K8.4 combined milestone v1.0 (2026-05-14) prescribed `new ComponentTypeRegistry()` — no such ctor existed; invented a helper that already existed; gave a wrong sln path; the factory bulk-write shape was incompatible with the real K8.1-primitive structure. Caught at execution time by the executor reading the kernel files; resolved by patch v1 (5 findings). Formalized at A'.5 closure (2026-05-14, brief v2.0 §9.4). CAPA-2026-05-14-K8.34-API-SURFACE-MISS opened by patch v1, closed at v2.0 closure.
>
> **Falsifiable claim**: briefs authored under this lesson will not incur API-surface halts during execution. Counter-examples (brief that paraphrased API and executed cleanly anyway) would force re-examination — but the failure mode is asymmetric (paraphrase that happens to be correct doesn't validate the practice; one paraphrase that's wrong invalidates it).

Status vocabulary observed: heading `#### Lesson #NN — <title>`; body statement; **Origin**;
**Falsifiable claim**; optionally **Application track record** / **Mental check** blocks
(#11/#20/#22 carry these).

**Shape 2 — Provisional-pool bold bullet** (the "12 DEFER" list, METHODOLOGY.md:1019-1032).
The #N12/#N13/#N14/#N17 entries are single long bold-bullet paragraphs with inline
**Sub-pattern / Rationale / Pattern / Applications / Promotion gate** segments (see the #N17
text quoted in R2.5 below — it is the FORMALIZE source text).

**Shape 3 — batch promotion-outcomes block** (METHODOLOGY.md:1000-1017): the A'.8 precedent —
a "**12 FORMALIZED at A'.8 closure**" list (one line per lesson with [PROMOTED/strengthened/
FORMALIZED] markers), a "**1 SUNSET**" line, a "**12 DEFER**" carry list; per-lesson rationale
delegated to `K_CLOSURE_REPORT.md §6`. Note: the individually-headed sub-sections BELOW the
pool (#9/#10/#14/#16/#17/#21 at lines 1036-1092) still carry "(provisional)" headings and
"Promotion gate: 2+ more strong applications" text although the A'.8 block promoted them —
the doc's own precedent is that the block is authoritative and stale stub headings survive.
The delta brief should specify the target shape explicitly to avoid inheriting this tension.

**Shape 4 — the brief's own proposed FORMALIZED template** (A_PRIME_9_1 brief §9.4,
lines 2021-2077): `## Lesson #N17 — Audience-driven tooling deferral (FORMALIZED — was
Provisional candidate)` with H3 subsections `### Statement / ### Falsifiability mechanism /
### Empirical applications / ### Cross-references / ### Status / ### Analogous pattern с
Lesson #25 refined`. **House-form mismatch**: the brief template uses H2/H3; METHODOLOGY's
existing lesson sections are H4 under `### Phase A' lessons` / `### Provisional Lessons`.
The delta brief must pick (recommend: H4 house form, content per brief §9.4).

### R2.3 Lesson-number census

Expression: `#N[0-9]+` on `docs/methodology/METHODOLOGY.md` (content mode). Matches at lines
19/21/23/25 (changelog) + 1014 + 1022-1032 (pool). N-series numbers **present in the body**:

| # | Status in METHODOLOGY v1.13.0 |
|---|---|
| #N2 | FORMALIZED at A'.8 (line 1014, promotion-outcomes block) |
| #N3, #N5, #N6, #N7, #N8, #N9, #N10 | Provisional pool one-liners (lines 1022-1028) |
| #N12, #N13, #N14 | Provisional pool full entries (lines 1029-1031); **#N14 marked "PROMOTION CRITERION MET — FORMALIZE candidacy at A'.9.1 Phase δ closure per Q-L-26 default (c)"** |
| #N17 | Provisional pool full entry (line 1032, appended v1.12.1 2026-05-24) |

**Absent from METHODOLOGY**: #N1, #N4, #N11, #N15, #N16, #N18, #N19, #N20.

Old-series lessons coexist in the same document: #7/#8/#11/#20/#22 formalized full sections;
#9/#10/#14/#16/#17/#21 promoted at A'.8 (stale "(provisional)" stubs remain); #15 SUNSET;
#18/#19 provisional carried; #23 candidate. **Collision warning**: bare **#17/#18/#19**
(old series, lines 1067/1073/1080) are DIFFERENT lessons from **#N17/#N18/#N19**. Any census,
citation, or formalization prose at delta must use the unambiguous `#Nxx` forms.

**F-7 gap confirmed and enriched** — expression: `#N(1|4|11|15|16)\b` repo-wide:
- **#N4** — ASSIGNED semantics outside METHODOLOGY: "Lesson candidate #N4" = atomic discipline
  through substantial cascade (REGISTER.yaml:8648 "surfaced в A'.7.x δ6 closure as Lesson
  candidate #N4"; A_PRIME_7_X_BUS_ARCHITECTURE_AMENDMENT_BRIEF.md:138, :517).
- **#N15** — ASSIGNED: "К-L14 substrate extension protocol" (K_EXT_3_LAUNCHER brief ×8 sites
  incl. :1504/:1833/:2026; K_EXTENSIONS_LEDGER.md:224; PHASE_A_PRIME_SEQUENCING.md:285;
  REGISTER.yaml:9180 "Lesson #N15 first application reserved").
- **#N16** — ASSIGNED: "pre-authoring empirical grounding (incorporated в #N14 effectively)"
  (K_EXT_3 brief :1505, :1834, :2238).
- **#N1, #N11** — ZERO carriers repo-wide. Never assigned.

The F-7 "assign-or-declare-intentional" decision therefore adjudicates numbers that (for
N4/N15/N16) already carry on-disk semantics in execution-tier artifacts, and (for N1/N11)
are genuinely free. F-7's own text scopes the gap as "#N11/#N15/#N16 absent in `#N1x` form"
— accurate for METHODOLOGY; the N4/N15/N16 external assignments are the context the operator
needs at delta.

### R2.4 Per-lesson source table (formalization sources + application instances)

**#N17 — Audience-driven tooling deferral** (Provisional -> FORMALIZE at delta)

| Surface | Location | Content |
|---|---|---|
| Canonical Provisional text | METHODOLOGY.md:1032 | Full pool entry (statement + PA anchors + 5 empirical applications + promotion gate); quoted in R2.5 |
| Changelog record | METHODOLOGY.md:19 (v1.12.1) | "appended к Provisional pool… FORMALIZE codification deferred к A'.9.1 Phase δ closure per Q-L-26 default (c)" |
| FORMALIZED entry template | A_PRIME_9_1 brief:2021-2077 (§9.4) | Ready-to-land text incl. falsifiability mechanism + #25-analogy table |
| Candidate first capture | A_PRIME_9_1_PHASE_0_CLOSURE_REPORT.md:430-491 (§6) + :147-156 (location Options A/B/C) | Option B (METHODOLOGY inline) ratified — REGISTER.yaml:9649, :9664 |
| Axiom cross-refs | PROJECT_AXIOMS.md:65, :208 | #N17 = formal codification of PA-001 application |
| Applications | ROADMAP.md:908 (hardware-tier deferral "per Lesson #N17 Provisional"); ANALYZER_RULES.md:196 (same, §6-relocation note); tools/DualFrontier.Analyzers/Rules/NativeBoundary/DFK019_AStaticVulkanApiAnalyzer.cs:25 (code citation); 5 Q-L ratifications 2026-05-24 (Q-L-8/12/13/15/16) enacted across α — e.g. commit `16436b7` body "NOT INCLUDED per ratifications" list | 1st formal cascade-level application = A'.9.1 itself; promotion criterion met per brief :1976 |
| Evidence hashes | `bb6807c` (Provisional landed + Phase 0 closure), `4fa76ed` (register), `16436b7` (Q-L-12/13/15 enactment record) | |
| Rider obligation | ROADMAP.md:986 — F-27 item (e) | #N17 narrative carries dotted DFK019.A/.B forms; "natural fix at #N17 formalization" |
| Numbering-collision context | A_PRIME_9_0_AMENDMENTS_LOG.md:376, :396-400 | "#N17" was briefly proposed there for a DIFFERENT candidate («Reserved-stub test exclusion discipline»); resolved historically — audience-driven deferral took #N17. Historical artifact (execution tier, LEAVE); relevant to F-7 numbering care |

**#N18 — Pre-flight empirical scope verification** (never codified -> FORMALIZE at delta;
timing lock: this cascade IS the unlock event)

| Surface | Location | Content |
|---|---|---|
| Candidate designation + name | DOCUMENTATION_DUAL_LOAD_DRIFT_RECONNAISSANCE_BRIEF.md:56 | "Lesson #N18 candidate (pre-flight empirical scope verification) — estimates off 2.7× because doc-claimed scope diverged from code reality" |
| Application-law wording | same brief :117 | "orchestrator MUST empirically count documents per category from REGISTER before sub-agent allocation. Do NOT estimate — read REGISTER, count, allocate."; also :161, :218 |
| Honest not-codified note | RESERVED_SURFACE_MUTABILITY.md:22 | "Lesson #N18 … is **not yet codified** in METHODOLOGY.md, whose lessons top out at #N17. The numbering gap is tracked … as F-7, architect-owned." |
| Fix demands | DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md:276, :367; DOCUMENTATION_DUAL_LOAD_DRIFT_REFACTOR_PROGRESS.md:116 | "#N18 absent" resolution demanded |
| Timing lock | STANDING_LAW_CASCADE_BRIEF.md:307 ("NOT included: Lesson #N18 formalization (timing-locked to A'.9.1 Phase δ)"); ROADMAP F-7 owner cell; ROADMAP.md:885 | |
| Brief-bloat framing | STANDING_LAW_CASCADE_BRIEF.md:225 | defensive literal transcription = "the Lesson #N18 bloat root" |

Application instances (the citable empirical base):

| Instance | Record location | Evidence hash |
|---|---|---|
| **A'.4.5 enrollment 195 -> 229** | K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md:2858 — "document count drift observed during A'.4.5 enrollment (pass 5 underestimate of ~195 vs actual 229) did not recur. **Future briefs incorporate Phase 0 grep-count step per A'.4.5 lesson.**" Estimate carriers: A_PRIME_4_5_PASS_3_SCHEMA…:574 ("Total documents: 195"), PASS_5_PRODUCTION_ENTRIES:784/:1034, PASS_2:230; actual: METHODOLOGY.md:458 "229 documents enrolled at A'.4.5 closure" | The K8.34 verification_outcome record carries no inline commit hash — gap declared; the instance is cited by file+line |
| **A'.9.1 DF->DFK rename ~195 -> 531** | A_PRIME_9_1_PHASE_0_CLOSURE_REPORT.md:40 (F4 row), :175, :261 (§3.4): "531 total `DF[0-9]{3}` occurrences across 15 files (brief estimate ~195 — off by 2.7×)"; also METHODOLOGY.md:1031 (inside the **#N14** pool entry, as #N14's 4th application) | report enrolled `bb6807c`; rename executed `586bf59` |
| **CPM ~30 -> 11** | commit `16436b7` body (the record): "11 csproj files contain PackageReference entries (brief §6.3 estimated ~30; actual is lower because most src/, mods/, and Fixture.* csprojs use ProjectReference only…). Surface к Crystalka via this commit message for forward calibration." Estimate carriers: A_PRIME_9_1 brief:280 ("~30 csproj migrations"), PHASE_0_CLOSURE_REPORT:373 | `16436b7` |
| **Phase β phantom projects** | PHASE_BETA_BRIEF.md:51: "brief §2.3 (and the A'.9.1 brief) name `Application.Scheduler / Application.Modding / Core.Scheduling` -- these DO NOT EXIST (**Lesson #N18 recurrence**). On-disk truth = 12 projects…" | brief enrolled `1bc0df2` |

**Overlap the delta brief must adjudicate**: (a) METHODOLOGY's **#N14** ("Phase 0 reads
empirical assumed-state coverage", PROMOTION MET, FORMALIZE due at delta per the v1.12.1
changelog) already carries the rename-531 instance; (b) **#N16** ("pre-authoring empirical
grounding") is recorded as "incorporated в #N14 effectively" (K_EXT_3:1834); (c) **F-25**'s
resolution cell carries a further un-numbered delta lesson-candidate — "The pre-measure
discipline lesson stands — **Phase δ lesson-candidate alongside #N20**" (census-delta
pre-measure; ROADMAP.md:984). The #N14 / #N16 / #N18 / F-25-lesson cluster is one semantic
family (measure-before-authoring); the brief decides: distinct #N18 entry + #N14
formalization, or a merged formalization. All four textual bases are located above.

**#N19 — Detection via canonical FQN strings** (referenced as law since Phase β; never in
METHODOLOGY)

| Surface | Location | Content |
|---|---|---|
| Canonical wording | PHASE_BETA_BRIEF.md:95 (§7 global writer spec) | "**Detection via canonical FQN strings, not CLR type references** (Lesson #N19 -- the analyzer csproj deliberately has no `ProjectReference` to Contracts; a rule that needs a type identity matches its fully-qualified display string, e.g. `symbol.ToDisplayString() == \"DualFrontier.Contracts.Analyzer.ReservedStubAttribute\"`)" |
| Detection-kind taxonomy | PHASE_BETA_BRIEF.md:46 ("SYNTAX / FQN-STRING per Lesson #N19 / SEMANTIC"); :106 ("### W2 -- FQN-STRING tier (Lesson #N19 canonical)") | |
| Recon tier table | PHASE_BETA_RECON_REPORT.md:153 | "Tier 2 — FQN-STRING (Lesson #N19 canonical): DFL025-A (documented), DFK011, DFK003.1, DFK019.A (+ DFK002 by FQN)" |
| Code carriers | tools/DualFrontier.Analyzers/Rules/Discipline/ReservedStubAnalysis.cs:10 (XML doc: "Lesson #N19 — the analyzer holds no compile-time reference to DualFrontier.Contracts"); DFL025_AReservedStubInvocationAnalyzer.cs:92; DFL025_BStandaloneSkipAnalyzer.cs:76 | |
| Test carriers | tests/DualFrontier.Analyzers.Tests/Verifiers/CSharpAnalyzerVerifier.cs:14; Rules/Discipline/DFL025_BStandaloneSkipTests.cs:13 | |
| Evidence hashes | W2 wave commit `666c369` (FQN-string detection DFL025_A/DFK011/DFK003_1/DFK019_A); also `d7cff93` (W1, DFK002 by FQN), `77ae9e0` (W4); brief enrolled `1bc0df2` | Application = the shipped Phase β detection layer itself |

**#N20 — Eradication class derived from the repo's own ignore/config classification**
(F-26 candidate -> FORMALIZE at delta)

| Surface | Location | Content |
|---|---|---|
| Candidate statement | GODOT_ERADICATION_BRIEF_PATCH.md:40-43 | "Two recon passes missed them because both swept by an enumerated extension list that did not include `.uid` (**Lesson #N20 candidate: derive the eradication class from the repo's own ignore/config classification, not a fixed list**)." |
| Owner + timing | GODOT_ERADICATION_BRIEF_PATCH.md:202-207 (F-26 seed) | "the METHODOLOGY lesson (derive the class from the repo's own ignore/config classification) is the Lesson #N20 candidate, **owner: architect, formalize at next closure**. Recorded so the next cleanup cascade inherits the corrected method." Delta IS the next closure |
| Ledger record | ROADMAP.md:985 (F-26 row, CLOSED) | Same sentence, plus "204 source `.cs.uid` sidecars (+ 33 in bin/obj); two recon passes (R2 + the brief digest) inherited the gap. Surfaced by the Godot Eradication pre-ratification pressure-test (F-1)" |
| Application | Godot Eradication Cascade: all `.uid` deleted at C4 `be7d4c2` (F-1 folded via PATCH 1; F-5 scope-closure); brief+PATCH enrolled `077a8c8` | Single application (Godot); the F-26 owner cell is the formalization mandate |

### R2.5 The #N17 Provisional source text (verbatim, METHODOLOGY.md:1032)

> **Lesson #N17 (Provisional NEW К-extensions cascade #5 / A'.9.1) — Audience-driven tooling deferral**: Tooling infrastructure что serves a specific consumer audience (human IDE workflow / external community / multi-environment deployment / multi-tier hardware) ships only when that audience materializes. Pre-emptive shipping = kostyl pattern violating PA-002 (без костылей) + К-L14 substrate minimality (PA-004). Activation triggers documented per-deferral. Anchored в PA-001 (current audience profile = AI agents permanently per FRAMEWORK §0 «agent-as-primary-reader» + SYNTHESIS_RATIONALE §0 Q-A07-6 inheritance). **Paired discipline с Lesson #25 refined**: Lesson #25 says «design abstractions when consumer materializes»; Lesson #N17 says «ship enforcement infrastructure when audience materializes» — both anchored в audience-driven materialization criterion (Lesson #25 for code-level abstractions; Lesson #N17 for tooling-layer surface). **5 empirical applications at A'.9.1 deliberation 2026-05-24** (per session log batch 2 §5.1 — see brief A'.9.1 §1.3 ratifications): (1) Code-fix providers — Q-L-15 PERMANENTLY DROPPED (PA-001 axiom anchor — AI-agent-first consumer profile permanent; diagnostic message quality elevated к compensate). (2) PublicApiAnalyzers — Q-L-13 audience-driven DEFERRED (community ecosystem absent; activation triggers documented). (3) BannedApiAnalyzer — Q-L-12 DROPPED (closed historical concern Godot cascade #2; documentation discipline sufficient). (4) DFK019.B hardware tier capability — Q-L-8 SPLIT (DFK019.A static Vulkan API surface ships A'.9.1; DFK019.B hardware tier runtime check deferred к hardware tier expansion cascade — multi-hardware-tier audience absent). (5) DFK016 threshold customization API — Q-L-16 deliberation reasoning (multi-hardware-tier audience absent; DFK016 itself retained α at Warning severity post Phase 0 ratification 2026-05-24). **Promotion gate**: second formal cascade-level application с distinct audience surface (e.g. cascade C ships Lesson #N17 deferrals для different audience — community contributor vs solo + AI agents). A'.9.1 = first formal cascade-level application (5 simultaneous Q-L decisions). FORMALIZE candidacy at A'.9.1 Phase δ closure per Q-L-26 default (c).

Note the dotted `DFK019.A/.B` forms inside — the F-27(e) rider fixes them at formalization
(underscore forms are law since the Phase β descriptor-ID adjudication).

### R2.6 In-repo lessons ledger besides METHODOLOGY

**None.** Expression: Glob `docs/**/*LESSON*` -> no files. `A_PRIME_7_X_LESSON_CANDIDATES.md`
(referenced by the arc brief §4.2 Task 4) confirmed non-existent (Phase 0 closure report F7;
Option B "METHODOLOGY inline" ratified). `K_CLOSURE_REPORT.md §6` holds the A'.8
promotion-decision record (per-lesson FORMALIZE rationale) — an archival decision record the
METHODOLOGY pool cites, not a living ledger. **METHODOLOGY.md is the SoT for lesson state.**

---

## R3 K-L14 Evidence Dashboard

- **Path**: `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` (300 lines).
  Register: `DOC-A-K_L14_EVIDENCE_DASHBOARD`, Category A, Tier 2, lifecycle
  **AUTHORED-SKELETON**, version **0.1.1**.
- **Highest evidence number**: **#13** (A'.9.0 Reconnaissance, 2026-05-24). #10 VACATED
  (original Godot branch discarded per S-LOCK-1). Active log: 12 verifications (9 baseline +
  #11 + #12 + #13); cumulative "11 clean + 1 honest soft-halt annotation (#7)".
- **Forward slot** (§5 forward table, quoted): `| 14 (candidate) | A'.9.1 Analyzer
  Infrastructure (К-extensions cascade #5 per Q-K-44) | Post-A'.9.0 closure | Production code
  addition — analyzer NuGet infrastructure + first DF### rules; substrate stability via
  in-repo build tooling |`.
- **Hash-citation convention**: hashes enter via the template's `**Cascade**: <brief
  reference / commit range>` field plus the **Cross-references** block (cascade closure report
  link, brief path, CAPAs, К-L invariants affected). Entry #13 cites commit-stage labels
  (α0-α4+β) and artifact paths rather than raw SHAs; the #14 entry should carry the real
  ranges (R4.3 table below supplies them).

**§3 forward verification template (verbatim)**:

```markdown
### Verification #N — <Cascade Name>

**Date**: YYYY-MM-DD
**Cascade**: <brief reference / commit range>
**Status**: Clean / Soft-halted / Falsifying observation
**К-L LOCK transitions**: <list, if any>
**Performance metric**: <empirical baseline, if measurable>

**К-L14 contribution narrative** (1-2 paragraphs):
<Describe how this cascade contributes к К-L14 empirical evidence. Confirming, neutral, or disconfirming observation.>

**Cross-references**:
- Cascade closure report: <link>
- Brief: <link>
- Relevant CAPAs (if any): <list>
- К-L invariants affected: <list>

**Falsifiability criteria status post-this-verification**:
<Update active criteria 1-4 status; deferred criterion 5; provisional criterion 6 (soft-halt rate)>
```

**Latest entry (#13) — structure as landed** (dashboard §2.5; realized entries carry more
than the template): header fields (Date / Cascade / Status "CLEAN (degenerate pass per
observational evidence framing — 5th evidence type, NEW category per S-LOCK-6)" / К-L LOCK
transitions: none / Performance metric: N/A), **К-L14 contribution narrative** (evidence-type
taxonomy positioning), a **pass-criteria table** (criterion | status | evidence), an
**evidence-type taxonomy block** (5 types codified), an **honest framing** paragraph,
**cross-references** (report, brief, ledger §3.5, KERNEL chronicle, К-L affected, CAPAs),
**falsifiability criteria 1-6 status** (six lines), **lessons surfaced/refined** (three
bullets incl. #N14/#N13 promotion-proximity updates), **brief amendment events**, and the
**cumulative line**: "12 verifications в active log (9 baseline + #11 cascade #2 + #12
cascade #3 + #13 cascade #4; #10 slot vacated). 11 clean + 1 honest soft-halt annotation
(#7). К-L14 thesis remains не-falsified by accumulated evidence."

**What the #14 entry shape demands**: date; cascade designation + real commit ranges
(Phase 0/α/β-prep/β/γ per R4.3); status CLEAN (no soft-halts across the arc); К-L LOCK
transitions: none (К-L count 21 final); performance metric (analyzer build-time cost — the
arc brief §3.1 carried "+1-3s per dotnet build" as the expectation); contribution narrative
as the NEW evidence type ("first analyzer implementation evidence — tooling addition";
#13 codified types 1-5, the arc brief calls this "Type 6 NEW category"); criteria 1-6
status; lessons surfaced (#N17/#N18/#N19/#N20 formalizations + #N14); cumulative line
(13 verifications active, 12 clean + 1 soft-halt).

**Divergences the delta brief must adjudicate**:
1. **Numbering**: the arc brief §3.3 (:351-357) reserves forward candidates "#15 (DFK016
   Phase 0 decision) / #16 (Phase γ promotion) / #17 (METHODOLOGY codification)" — splitting
   the arc into up to four entries. The canonical surfaces all say **one #14 entry for the
   arc**: dashboard §5 forward table, ROADMAP Phase-δ row ("K_L14_EVIDENCE_DASHBOARD
   verification #14 (A'.9.1) recorded"), the gamma-closure EVT ("Phase δ (governance closure:
   lessons #N17/#N18/#N19/#N20, К-L14 Evidence #14, A'.9.1 brief EXECUTED transition) is
   next"), and PHASE_GAMMA_BRIEF §12. Recommend single #14; the brief should say so.
2. **Template form**: brief §3.2 proposes a YAML entry (richer fields:
   falsifiability_metric_shift, defect_rate_baseline, architectural_integrity_metric,
   pipeline_economics); the dashboard's own §3 markdown template is the maintenance law.
   Reconcile (recommend: dashboard §3 form, with the YAML's extra facts folded into the
   narrative).
3. **Dashboard lifecycle**: §6 promotion gate "AUTHORED-SKELETON -> Tier 2 Live: 3+
   post-closure verifications appended; dashboard schema stable" — #11/#12/#13 already = 3;
   #14 makes 4. The gate is arithmetically satisfied at delta; no surface currently schedules
   the flip. Candidate delta decision (operator ratifies or defers).

---

## R4 A'.9.1 arc-closure surface

### R4.1 Brief lifecycle (both surfaces)

- File `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` (2354 lines): frontmatter
  `lifecycle: AUTHORED`, `version: "1.0"`; body Status (line 24) "AUTHORED — pending
  Crystalka ratification + execution session handoff".
- REGISTER entry `DOC-D-A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF` (REGISTER.yaml:2366-2376):
  Category D, Tier 4, `lifecycle: AUTHORED`, version 1.0, last_modified_commit `bb6807c`.
- The Phase-0 EVT records the expected path: "status transitions expected AUTHORED →
  RATIFIED → HANDOFF → EXECUTED" (REGISTER.yaml:9693-9696). Both surfaces flip to
  **EXECUTED** at delta. (The phase sub-briefs are already EXECUTED:
  DOC-D-A_PRIME_9_1_PHASE_GAMMA_BRIEF + DOC-E recon per the gamma EVT lifecycle_transitions;
  beta pair likewise at 7d1fdb9.)

### R4.2 The brief's OWN closure demands (§8.2 "Cascade closure deliverables (Phase δ)")

Verbatim list (brief :1829-1876):
1. **K_EXTENSIONS_LEDGER.md §3.6 entry** (К-ext cascade #5) — verbatim template at §9.1.
2. **K_L14_EVIDENCE_DASHBOARD.md #14 entry** (per §3.2 template).
3. **KERNEL_ARCHITECTURE.md v2.5.3 → v2.5.4 chronicle entry** (template §9.3).
4. **METHODOLOGY.md v1.12 → v1.13 entry** (Q-L-26 default (c)) — #N17 codification
   (template §9.4).
5. **REGISTER.yaml cascade** — register_version bump + `EVT-2026-MM-DD-A_PRIME_9_1-CASCADE-CLOSURE`.
Plus §8.3 push protocol (push = operator act, in-session re-confirmation expected) and
§3.6.1's commit split: "**Phase δ (governance cascade)** — 1-2 atomic commits (closure
report + REGISTER cascade)".

**Demanded items ABSENT from this kickoff's four fronts** (flag per T4):
- **K_EXTENSIONS_LEDGER §3.6 entry** — ledger is Tier 2 `lifecycle: Live` (REGISTER.yaml:783-790);
  sections run §3.1-§3.5 (highest = cascade #4); **§3.6 does not exist** — expression:
  `§3\.6` on `docs/architecture/K_EXTENSIONS_LEDGER.md` -> 0 matches.
- **KERNEL_ARCHITECTURE chronicle entry** — live KERNEL is **v2.6.1** (register head), not the
  template's v2.5.3; the entry lands as a v2.6.1 -> v2.6.2-class patch bump (Q-G-12
  chronicle+cross-ref = patch).
- **ANALYZER_RULES lifecycle promotion** (from the ROADMAP δ row, not the brief): see R4.4 —
  one promotion criterion is unmet on disk (DECISION-NEEDED).
- **A closure report artifact** (§3.6.1) — the β/γ precedent was chat-level closure reports
  per their briefs' §12; the arc brief's δ split names "closure report + REGISTER cascade".
- **Candidate (demanded by neither brief nor kickoff)**: MIGRATION_PROGRESS.md chronicle
  catch-up — its "Last updated" chronicle (line 17) ends at Architecture Truth Cascade
  (2026-06-12, "IN PROGRESS"); the A'.9.1 arc is chronicled only through β-prep (2026-05-25
  entry); Godot Eradication, Phase β, and Phase γ are unchronicled. §12.9 (a) tracker
  write-back is the closure law class; surface for the brief to include or defer.

**Stale-version deltas inside the brief's §8/§9 templates** (mutable surface; land as
`Skeleton revisions` records at delta): register_version "2.10" -> live 2.20 -> 2.21;
METHODOLOGY "v1.12 → v1.13" -> live 1.13.0 -> 1.14.0; KERNEL "v2.5.3 → v2.5.4" -> live
2.6.1; "15-16 active rules" -> **17 shipped** (11 Error + 5 Warning + DFL025_B
suggestion-tier); dotted rule-IDs in templates (DFK003.1, DFK019.A/.B, DFL025-A/-B) ->
underscore forms per the Phase β adjudication; lessons expectation "#N17 (+#N25-refined)"
-> four lessons #N17/#N18/#N19/#N20 (+#N14 promotion) per the gamma-closure charter.

### R4.3 Phase-hash table

Expression: `git log --pretty="%h %ad %s" --date=short --since=2026-06-28` +
`git log … 1bc0df2^..b116727` + `git log … --since=2026-05-23 --until=2026-05-26` (read-only).

| Phase | Closing range (first -> last) | Date | Notes |
|---|---|---|---|
| Phase 0 (recon + brief authoring) | `bb6807c` -> `4fa76ed` | 2026-05-24 | bb6807c = "Brief AUTHORED + Phase 0 closure + Lesson #N17 Provisional"; 4fa76ed = Phase 0 REGISTER cascade |
| Phase α (9 commits) | `5030fa2` -> `a23556f` | 2026-05-24 | 5030fa2, a156d10, 16436b7, 586bf59, d0b4c41, fc66156, f4a94e6, 8e1a18a, a23556f — matches brief §9.1's 9-commit list 1:1 |
| Phase β-prep (β1-β4) | `588c667` -> `a213954` | 2026-05-25 | 17 stubs + src/Directory.Build.props wiring; + `f94bb84` "Add Phase β-prep execution prompt" (unprefixed, prompt artifact) |
| Phase β (C1-C12) | `1bc0df2` -> `b116727` | 2026-07-01 | 12 commits; detection + tests + census meta-tests + triage + REGISTER 2.18 -> 2.19 + render |
| Phase γ (C1-C8) | `524dd31` -> `cc2f71a` | 2026-07-01 | 8 commits; ship ruleset `3272d74`, .editorconfig `18a4dac`, ANALYZER_RULES 0.4.0 `b0d9480`, standing-law PATCH `d5d6fe2`, F-12+F-25 `5c97885`, REGISTER 2.19 -> 2.20 `8b26925`, render `cc2f71a` |
| Phase γ execution residue | `4cc5e7e` | 2026-07-01 | F-27 ledger seed (post-closure residue commit) |

Kickoff expected "gamma C1-C7 endpoints"; measured truth = **C1-C8 + one residue commit**
(9 commits total `524dd31..4cc5e7e`).

### R4.4 ROADMAP Analyzer track (docs/ROADMAP.md:848-925)

Current rows: relocation banner (:850-857); **Ground state (2026-06-11)** paragraph (:859);
**Phase β — detection implementation** (:861-871); **Phase γ — severity promotion**
(:873-878, promotion-map line carries "(Executed 2026-07-01 from descriptor Info …)");
**Phase δ — closure + governance** (:880-886, verbatim):

> ### A'.9.1 Phase δ — closure + governance
>
> - K_EXTENSIONS_LEDGER §3.6 cascade #5 entry (realization + Phase β/γ outcomes).
> - K_L14_EVIDENCE_DASHBOARD verification #14 (A'.9.1) recorded.
> - KERNEL_ARCHITECTURE chronicle entry.
> - METHODOLOGY Lessons: #N17 / #N18 FORMALIZE-candidacy evaluation timing-locked to this phase.
> - REGISTER governance cascade (enrollment/version sync); ANALYZER_RULES.md lifecycle promotion AUTHORED-SKELETON → Tier 1 LOCKED gated on completed implementation + first-run cleanup phase (relocated v0.1 §11 criteria: per-rule §2 templates populated, per-rule test coverage, Phase γ promotion executed, cleanup outcomes recorded per CAPA cascade).

then **К-L20 LOCK cascade rule family (deferred)** (:888-904, incl. the 7-row deferral table
and the MOD_API_CONTRACT.md forward-reference note); **Hardware tier expansion cascade
(deferred — audience-driven)** (:906-915, DFK019.B + DFK016 threshold API); **PublicApiAnalyzers
deferral — activation conditions (Q-L-13)** (:917-923).

**Flips DONE-with-hash at delta**: Phase β section -> DONE `1bc0df2..b116727`; Phase γ
section -> DONE `524dd31..cc2f71a` (+ residue `4cc5e7e`); the **ground-state paragraph
(:859) must be refreshed** — it still asserts "every descriptor `DiagnosticSeverity.Info`,
every `Initialize()` registers no analysis actions, zero diagnostics by design … Everything
below is scheduled, not shipped", false since β/γ (17 detecting rules at shipped severities);
Phase δ items -> executed-with-hashes at this cascade's own closure. Note: the γ exit-gate
line (:877) "until then, no «enforced» claim is true anywhere in the doc set" is satisfied —
enforcement claims are now legal doc-wide.

**Remain forward** (no flip): К-L20 LOCK cascade family (DFK009/DFK012/DFK015/DFK018 +
DFK020 family + DFC001.A/.B -> post-A'.9); hardware tier expansion (DFK019_B runtime tier +
DFK016 threshold API — audience-driven per #N17); PublicApiAnalyzers activation conditions.

**ANALYZER_RULES promotion blocker (DECISION-NEEDED)**: the δ-row gate requires "per-rule §2
templates populated", but ANALYZER_RULES.md §10 (:255) declares: "Landing zone for per-rule
§2-template entries. **Empty at v0.4.0** — the §4.1 registry is the single live surface;
per-rule §2-template population was deferred through Phase β/γ and is a **Phase δ+ item**".
Test coverage ✓ (β), Phase γ promotion ✓, cleanup outcomes recorded ✓ (2 DFK-WAIVERs
census-pinned, §12 baseline note). The delta brief must choose: (a) populate §10 per-rule
templates at delta (large authoring scope), (b) keep AUTHORED-SKELETON and defer promotion
(the "δ+" wording permits), or (c) re-ratify the relocated v0.1 criteria. Operator gate.

**The γ-tier charter of delta** (PHASE_GAMMA_BRIEF.md:118, §12 operator checklist, verbatim
tail): "**Phase delta is next** -- A'.9.1 governance closure where the timing-locked lessons
#N17/#N18/#N19/#N20 formalize, K-L14 Evidence #14 lands, and the A'.9.1 brief goes EXECUTED."
Same forward pointer in the gamma REGISTER EVT (REGISTER.yaml:10546-10547).

---

## R5 F-ledger sweep

Full current table (docs/ROADMAP.md:958-987), condensed to ID | finding (abbrev) | sev |
state | owner — full texts live at the cited lines; F-7 and F-27 quoted in full below:

| F-# | Finding (abbreviated) | Sev | State | Resolution / owner |
|---|---|---|---|---|
| F-1 | REGISTER_RENDER 15+ versions stale | S2 | CLOSED | Render regenerated at Standing-Law C10; defect split to F-13 |
| F-13 | render_register.ps1 variable-expansion defect (272 literal `$(…hashtable…)` rows) | S3 | OPEN | Register-tooling cascade (with F-2) |
| F-2 | PENDING-COMMIT placeholders; backfill discipline open (123 measured 2026-06-11) | S2 | OPEN | Future register-tooling cascade (hybrid reverse-register, Q-T-2) |
| F-3 | METHODOLOGY v1.12.1 changelog desync | S3 | CLOSED | Backfilled C7 `b58eed4` (v1.13.0) |
| F-4 | KERNEL Part 0 bi-script K-L/К-L inconsistency | S3 | OPEN | KERNEL amendment (architect); bi-script rule pinned interim |
| F-5 | project.godot + Godot-era files tracked post-deprecation | S3 | CLOSED | Executed C4 `be7d4c2` (Godot Eradication); full inert surface deleted |
| F-6 | Tooling reality undocumented (PS 5.1, validate write-trap) | S2 | CLOSED | C5 `9da4760` (DEV_HYGIENE 2.0.0 §4) |
| F-7 | (quoted in full below) | N | OPEN | Architect |
| F-8 | S-LOCK coverage audit | N | CLOSED | Audit empty 2026-06-11 |
| F-9 | KERNEL Part 0 counting-convention divergence risk | S3 | OPEN | KERNEL amendment (architect; may merge F-4) |
| F-10 | Two pre-existing test failures on main (stress + timing) | S1 | OPEN | Crystalka / next code cascade: isolate then fix or reclassify |
| F-11 | 41 merged local branches — pruning candidates | N | OPEN | Crystalka at leisure |
| F-12 | DFK019_A γ severity discrepancy (Warning vs Error) | S2 | CLOSED | Ratified 2026-07-01; executed γ C2 `3272d74` |
| F-14 | Un-ledgered version-bump class (MOD_OS/VULKAN changelogs) | S3 | CLOSED | C2 `9676f54` + C3 `60175a1` (Arch Truth) |
| F-15 | MIGRATION tracker desyncs | S2 | CLOSED | C7 `f2e6df2` + C9 `061fbc0` |
| F-16 | FRAMEWORK meta-entry count 4 vs 5 | S3 | CLOSED | C11 `7a074e4` (FRAMEWORK 1.1.2) |
| F-17 | Citation fragility classes (version pins / URL anchors) | S2 | CLOSED | §6.1 law C14 `bdb5283`; instances C2-C12 |
| F-18 | NATIVE_CORE.md cited but absent | S3 | CLOSED | C13 `476757b` + C12 `89d34f6` |
| F-19 | A_PRIME_9 recon report registered Live though consumed | S3 | CLOSED | Live -> EXECUTED at Arch-Truth C16 |
| F-20 | Stale text inside string literals (RestrictedModApi, ModIntegrationPipeline) | S3 | OPEN | Next code cascade (string = code-token; pair with test sweep) |
| F-21 | Doc-citation staleness (src READMEs, 5 vanilla mods pin MOD_OS v1.5) | S3 | OPEN | Next hygiene pass (§6.1 sweep) |
| F-22 | Dead managed surface (SystemBase.Context, SystemRegistry) | N | OPEN | Architect review — delete or [ReservedStub] per #N12 |
| F-23 | ISOLATION.md internals stale (THREADING §11.7; fault-handling 6-step fiction) | S3 | OPEN | Next doc pass |
| F-24 | Lifecycle-vs-body mismatch (ARCHITECTURE_TYPE_SYSTEM, MAX_ENG Track B Draft-vs-LOCKED) | N | OPEN | Architect — align at next touch |
| F-25 | Marker-family census delta unrecorded at source (stub 51/20, deferred 82/51) | S3 | CLOSED | Folded γ C5 `d5d6fe2`; "pre-measure discipline lesson stands — **Phase δ lesson-candidate alongside #N20**" |
| F-26 | Eradication recon swept by enumerated extension list, missed `.uid` class | S2 | CLOSED | Godot-specific closed C4 `be7d4c2`; **method lesson = Lesson #N20 candidate (owner: architect; formalize at next closure)** |
| F-27 | (quoted in full below) | S3 | OPEN | **Phase δ governance cascade** |

**F-7 full text (the DECISION-NEEDED item)**:

> | F-7 | METHODOLOGY lesson numbering gap: **#N11/#N15/#N16 absent** in `#N1x` form (pool: N10, N12, N13, N14, N17 — recon R3 re-measure 2026-06-11 widened the original #N15/#N16 finding); #N18 referenced by the DD recon brief but never codified | N | OPEN | Architect — assign the numbers or declare the gap intentional; #N18 formalization timing-locked to A'.9.1 Phase δ |

Delta-relevant enrichment (R2.3): #N15/#N16 (and #N4, outside F-7's `#N1x` scope) carry
assigned semantics in execution-tier artifacts; #N1/#N11 have zero carriers. The
assign-or-declare decision should account for those semantics and for the old-series
#17/#18/#19 collision surface.

**F-27 full text (the CLOSEABLE-AT-DELTA item)**:

> | F-27 | Post-Phase-γ doc-staleness residue (execution sweep, 2026-07-01) — living-prose sites still carrying pre-β/pre-γ state outside the Phase γ grant: (a) `RESERVED_SURFACE_MUTABILITY` §3 item 1 lines 57–58 assert the dot/hyphen descriptor-ID duality (`DiagnosticId = "DFK003.1"` / `"DFL025-A"`) — false since the β underscore adjudication (recon Anomaly 2); (b) `TESTING_STRATEGY` §5.3 item 2 says DFL025-A «analyzer detection is Phase β scope — today the convention binds by review» — detection landed β, binds at Warning since γ; (c) `ANALYZER_RULES` §9 DFL### note («DFL025-A + DFL025-B shipped as non-detecting stubs», hyphen forms) — past-tense provenance without the realized tail; (d) the 17 descriptor `Description` strings still say «Phase β cleanup-phase will populate detection patterns» (C2 grant was severity-only by brief letter); (e) `METHODOLOGY` Lesson #N17 narrative carries dotted DFK019.A/.B forms (living doc; natural fix at #N17 formalization) | S3 | OPEN | Phase δ governance cascade — fold as PATCH riders alongside the timing-locked lessons (#N17/#N18/#N19/#N20); sites enumerated verbatim here so the δ brief needs no re-recon |

Note (d) is a **code-token** change (descriptor `Description` strings in
`tools/DualFrontier.Analyzers/Rules/**`) — analyzer-project strings, outside src/ production,
but the delta brief must grant those files explicitly if folding (d).

**Classification of the 13 OPEN items**:
- **CLOSEABLE-AT-DELTA**: **F-27** (owner is literally this cascade; sites (a)-(e)
  pre-enumerated). **F-7** closes at delta IF the operator makes the assignment/intentional
  declaration (the #N18-formalization half of its owner cell unlocks here regardless).
- **DECISION-NEEDED (operator)**: **F-7** (above). Adjacent non-F decision: ANALYZER_RULES
  lifecycle promotion criterion (R4.4).
- **STANDING** (no delta action): F-2 + F-13 (register-tooling cascade, Q-T-2); F-4 + F-9
  (KERNEL amendment, architect-owned, may merge); F-10 (S1 — Crystalka / next code cascade);
  F-11 (Crystalka); F-20 (next code cascade); F-21 (next hygiene pass); F-22 (architect
  review); F-23 (next doc pass); F-24 (architect at next touch).
- **Cross-check** ✓: F-12 CLOSED at γ (`3272d74`, ratified 2026-07-01); F-25 CLOSED
  (`d5d6fe2`) — but its resolution cell seeds a delta lesson-candidate (census pre-measure,
  #N18-family, un-numbered); F-26 CLOSED with the #N20 formalization mandate pointing at
  delta.

---

## R6 Candidate riders

### R6.1 Rider 1 — commit-scope vocabulary (CODING_STANDARDS §8.1)

**Current law** (CODING_STANDARDS.md v2.1.1 §8.1, verbatim core): subject form
`<scope>(<sub-scope>): <imperative description>`; "The observed `<scope>` vocabulary,
codified: **`analyzer`, `chore`, `docs`, `feat`, `fix`, `governance`, `perf`, `refactor`,
`reports`, `test`**." Legacy engine prefixes "`core:` / `contracts:` / `interop:` /
`native:` / `modding:` are historical (visible in old history); they are not forbidden but
new commits use the `prefix(sub-scope):` form. A new scope prefix requires a `governance`
commit that amends this section first." Note: **`chore` is already codified** — the
kickoff's "or codify `chore`" branch is already satisfied since v2.0.0.

**Census** (full history, 1054 commits — `git rev-list --count HEAD`):
- Scope-token extraction expression:
  `git log --pretty=%s | sed -E "s/^([A-Za-z0-9_.-]+)(\([^)]*\))?:.*$/\1/;t;d" | sort | uniq -c | sort -rn`
- In-vocabulary: docs 310, feat 299, governance 73, test 70, chore 70, refactor 54, fix 32,
  analyzer 9, reports 2, perf 1 -> **920 commits**.
- **Out-of-vocabulary scope tokens — 53 commits over 16 tokens**: `tests` 13, `native` 9,
  `sprite` 6, `experiment` 6, `interop` 4, `compute` 3, `bench` 3, `vulkan` 1, `src` 1,
  `shaders` 1, `scaffold` 1, `runtime` 1, `revert` 1, `kernels` 1, `build` 1, `UI` 1.
  (`native`/`interop` include the §8.1-named legacy colon forms; `core:`/`contracts:`/
  `modding:` produced zero hits as leading scope tokens — in old history they appear as
  sub-scopes, e.g. `feat(contracts/modding)`.)
- Unprefixed subjects: expression
  `git log --pretty=%s | grep -cvE "^[A-Za-z0-9_.-]+(\([^)]*\))?:"` -> **81**, of which 42
  merges ("Merge pull" 38 + "Merge branch" 4) and **39 plain unprefixed** (GitHub-web-era
  "Update …"/"Create …" plus, inside this arc: `f94bb84` "Add Phase β-prep execution
  prompt", `04e618c` "Update validation report timestamp and counts", `4981d78` "Add CI test
  and launcher smoke logs"). One hybrid `native+compute:` subject falls in this bucket
  (the `+` escapes the scope grammar).
- **Timing fact that shapes the rider**: every out-of-vocabulary/unprefixed commit predates
  the 2026-06-11 codification (CODING_STANDARDS 2.0.0). Post-2.0.0 history is 100%
  in-vocabulary (verified over the 2026-06-11..2026-07-01 slice). So there is **no live
  violation**; the finding is purely a historical-reconciliation gap in the descriptive
  sentence "The observed `<scope>` vocabulary, codified" (the observation was incomplete).
- **Semver rule** (§10.1 rule 3, verbatim): "PATCH for clarification/correction, MINOR for a
  new rule or family, MAJOR for inverting an existing rule." Rider shape options for the
  brief: (a) **PATCH** — extend the historical-prefix note to name the observed pre-law
  tokens (correction of the descriptive claim; recommended, matches the kickoff's
  expectation); (b) MINOR — normatively extend the scope list (not needed; no live usage).

### R6.2 Rider 2 — PIPELINE_METRICS falsified forward-claim (era annotation)

**Document**: `docs/methodology/PIPELINE_METRICS.md`, `DOC-B-PIPELINE_METRICS`, Category B,
**Tier 1, lifecycle LOCKED, version 0.2** (frontmatter), 291 lines. The Godot Eradication
cascade deliberately LEFT this document — the rider completes the era truth as
annotation-only.

**Charter (verbatim-historical preservation), line 22**:
> A'.0.7 amendment (2026-05-10) restructured the pipeline к v1.6 era … **v1.x era measurements are preserved verbatim as historical record; each section/sub-section receives a per-metric annotation declaring transferability к v1.6 reality.**

**The falsified forward-claim lines** (post-era annotation prose, NOT protected historical
body):
- **§5.2 annotation, line 244**: ".NET 8.0 SDK + **Godot 4.3+ survive across era boundary**
  (project tech stack). VS Code+Cline + LM Studio+Gemma are v1.x-specific tooling that
  doesn't transfer. …"
- **§6 Forward measurement plan, line 275** (§5.2 row): "Reframe tool stack — Claude Desktop
  client primary; VS Code+Cline + LM Studio v1.x-specific (removed for v1.6); **Godot 4.3+ +
  .NET 8.0 survive**".
Both falsified by: К-extensions cascade #2 Godot Full Deprecation (2026-05-23) and the Godot
Eradication Cascade (2026-06-29/30, F-5 CLOSED at `be7d4c2`, DEVELOPMENT_HYGIENE §7
"fully-eradicated" at `27807f7`).

**Adjacent protected-verbatim sites (annotate around, never scrub)**: §1.3 body line 85
("**Godot dev environment.** The Godot 4.3 editor and the C# build pipeline can run on the
same machine…" — v1.x historical body; its own annotation line 89 claims only hardware
transfer, not Godot survival — no defect); §5.2 body line 240 ("Godot 4.3+ with C# / mono
support" — historical dependency list, preserved verbatim per charter).

**Rider shape the brief can specify**: an era ANNOTATION appended at/adjacent to lines 244
and 275 (the document's own annotation mechanism), e.g. "[Superseded 2026-05-23 /
2026-06-30: Godot fully deprecated at К-extensions cascade #2 and eradicated at the Godot
Eradication Cascade (F-5 CLOSED `be7d4c2`); the surviving v1.6+ tech stack is .NET 8.0 +
the Vulkan substrate + DualFrontier.Launcher. Historical text preserved verbatim per the
era charter.]" — PATCH 0.2 -> 0.2.1-class; zero touch to §1-§5 historical bodies.

---

## R7 Anomalies + scale estimate

### Anomalies / divergences

1. **HEAD is at origin** — the gamma closure state ("NOT pushed") has been superseded by an
   operator push; no ahead-of-origin residue for delta to carry.
2. **Evidence-numbering divergence**: arc brief §3.3 reserves #15/#16/#17 forward evidence
   candidates vs the canonical single **#14** arc entry (dashboard §5, ROADMAP δ row, γ EVT,
   γ brief §12). Delta brief should pin the single-#14 form (R3).
3. **ANALYZER_RULES promotion criterion unmet on disk**: §10 per-rule templates "Empty at
   v0.4.0 … Phase δ+ item" vs the ROADMAP δ-row LOCKED gate "per-rule §2 templates
   populated". Operator decision: populate / defer promotion / re-ratify criteria (R4.4).
4. **ROADMAP ground-state paragraph stale** (:859 — "registers no analysis actions, zero
   diagnostics by design … Everything below is scheduled, not shipped") — false since β/γ;
   NOT among F-27's five sites; belongs to the delta tracker write-back.
5. **F-25 carries an un-enumerated delta lesson-candidate** (census pre-measure discipline,
   "Phase δ lesson-candidate alongside #N20") — same measure-before-authoring family as
   #N18/#N14/#N16; the kickoff's four-lesson list does not include it. Brief adjudicates the
   cluster (R2.4).
6. **#N17 designation collision (historical)**: A_PRIME_9_0_AMENDMENTS_LOG proposed "#N17"
   for a reserved-stub test-exclusion lesson before the audience-driven lesson took the
   number. Execution-tier, LEAVE; informs F-7 numbering care.
7. **Old-series #17/#18/#19 vs #N17/#N18/#N19 ambiguity** — both live in METHODOLOGY; delta
   prose and censuses must use `#Nxx` forms exclusively (R2.3).
8. **Stale "(provisional)" H4 stubs** for lessons promoted at A'.8 (#9/#10/#14/#16/#17/#21)
   — existing precedent tension; delta brief should state the rendering shape for the four
   new formalizations explicitly (R2.2) and may opportunistically annotate the stubs (or
   leave, matching precedent).
9. **K-L14 dashboard Tier-2-Live promotion gate arithmetically satisfied at #14** (3+
   post-closure entries) — unscheduled anywhere; candidate operator decision (R3).
10. **MIGRATION_PROGRESS chronicle gap**: Godot Eradication + Phase β + Phase γ unchronicled
    (last entry 2026-06-12 Arch-Truth "IN PROGRESS"); not demanded by the arc brief §8.2;
    candidate write-back item (R4.2).
11. **Unprefixed commits inside the arc** (`f94bb84`, `04e618c`, `4981d78`) — all pre-law
    (before 2026-06-11); evidence for rider 1's historical-reconciliation shape, not live
    violations.
12. **Brief §3.2 YAML entry template vs dashboard §3 markdown template** — reconcile at the
    #14 entry (R3).

### Scale estimate

- **Commit-class split (serial, ~7-8)** — consistent with the expected shape:
  1. C1 `governance(analyzer)` — enroll Phase δ brief + this recon report + validation
     checkpoint (validate-fold per §12.9 (b)).
  2. C2 `docs(methodology)` — METHODOLOGY 1.13.0 -> **1.14.0**: four FORMALIZE entries
     (#N17/#N18/#N19/#N20) + #N14 promotion disposition + pool restructure + changelog;
     F-27(e) dotted-forms fix rides here.
  3. C3 `docs(dashboard)` — K_L14_EVIDENCE_DASHBOARD #14 entry (+ lifecycle decision if
     ratified).
  4. C4 `docs(kernel+ledger)` — KERNEL chronicle entry (2.6.1 -> 2.6.2-class) +
     K_EXTENSIONS_LEDGER §3.6 (could split into two commits if the brief prefers
     one-Tier-1-doc-per-commit).
  5. C5 `governance(roadmap)` — Analyzer-track flips (β/γ DONE-with-hash + ground-state
     refresh + δ items) + F-sweep (F-27 close; F-7 disposition per operator).
  6. C6 riders if ratified — `docs(standing-law)`: CODING_STANDARDS §8.1 PATCH (2.1.1 ->
     2.1.2) + PIPELINE_METRICS era annotation (0.2 -> 0.2.1); F-27 (a)-(c) standing-doc
     PATCH riders fold here or into C5 per brief; F-27(d) descriptor strings need an
     explicit analyzer-file grant.
  7. C7 `governance(register)` — REGISTER **2.20 -> 2.21**: arc brief AUTHORED -> EXECUTED,
     version syncs, ANALYZER_RULES disposition, EVT-…-A_PRIME_9_1-CASCADE-CLOSURE, validate
     fold.
  8. C8 `governance(register)` — render regeneration + header backfill (γ Option-B
     precedent).
- **Wave topology**: NOT indicated. Every surface is serial-coupled through REGISTER and
  cross-citations; the β-style writer wave existed for disjoint rule files — no analog here.
  Serial single-writer per the γ precedent.
- **METHODOLOGY bump class**: **MINOR 1.14.0** — four lesson entries + no rule inverted;
  exact v1.10 precedent (12-lesson batch = MINOR); the §10 MAJOR trigger (pipeline
  reconfiguration) is not touched. Confirmed nothing in the amendment conventions demands
  otherwise. CODING_STANDARDS rider = PATCH per §10.1 rule 3 (correction);
  PIPELINE_METRICS rider = PATCH (annotation-only under its own charter).

---

## R8 Self-attestation

- **Zero writes except the report file at docs/reports/ (validate NOT run)**: CONFIRMED. The
  only file created is `docs/reports/PHASE_DELTA_RECON_REPORT.md` (this file, untracked).
  No tracked file edited or deleted; `sync_register.ps1` not invoked in any mode; no builds
  run.
- **Report written to docs/reports/ AND presented in chat (uncommitted)**: CONFIRMED — same
  content in both; file left untracked for Phase δ C1 enrollment.
- **Zero git mutations**: CONFIRMED. Git accessed only via `git rev-parse`, `git status
  --porcelain`, `git log`, `git show`, `git rev-list --count` (all read-only); no commit /
  checkout / switch / merge / fetch / stash / push.
- **Every census expression recorded verbatim**: CONFIRMED — register counts (R1), lesson
  censuses `#N[0-9]+` / `#N17\b` / `#N18\b` / `#N19\b` / `#N20\b` / `#N(1|4|11|15|16)\b`
  (R2), ledger `§3\.6` (R4.2), scope-token and unprefixed-subject expressions (R6.1),
  phase-hash log expressions (R4.3), instance hunts `\b195\b` / `\b531\b` /
  `Directory\.Packages\.props` over `*.md` (R2.4), lessons-ledger glob `docs/**/*LESSON*`
  (R2.6).

**End of PHASE_DELTA_RECON_REPORT.md — 2026-07-01, HEAD `4cc5e7e`, register 2.20 (279 docs / 43 EVT)**
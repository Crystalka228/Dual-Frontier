---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-VALIDATOR_2_RECON_REPORT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: null
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-VALIDATOR_2_RECON_REPORT
---
# VALIDATOR_2 RECON REPORT — 2026-07-15

*Pre-brief reconnaissance (read-only) for the VALIDATOR_2 cascade. Executor: Claude Code (flagship), LOCAL session on SKARLET. Repository `D:\Colony_Simulator\Colony_Simulator`. This report is the landing artifact for the recon's findings per METHODOLOGY §12.9 (d) (findings land in ledgers, never chat-only); every count carries its verbatim measuring expression per TESTING_STRATEGY §4. Fix DESIGN is not this recon's mandate — R7 carries directions only.*

*Central prior (verified by the architect 2026-07-14/15, not re-derived here): the drift audit's DF-DOC-002 (enforcement gap) and DF-DOC-003 (schema question) were the two unverified claims; this recon turns them into an engineering inventory. Cross-cutting lens: the fail-loud doctrine — a check that skips (or was never authored) when its precondition is absent is the recurring defect class.*

---

## R1 Base state

| Fact | Value | Expression / source |
|---|---|---|
| Branch | `main` | `git rev-parse --abbrev-ref HEAD` |
| HEAD | `f0f76a8606dd89b42ff9297340385dfe13f78d0c` (`f0f76a8`) — CODEX_CLOSURE C8 ✓ | `git rev-parse HEAD` |
| origin/main | `61f08ef` (the drift-audit commit) | `git rev-parse origin/main` |
| Local vs origin | **ahead 8, behind 0** — the CODEX_CLOSURE cascade is UNPUSHED | `git rev-list --left-right --count origin/main...HEAD` → `0	8` |
| Working tree | ` M .claude/scheduled_tasks.lock` only (expected lock churn) | `git status --porcelain=v1` |

**REGISTER.yaml header** (read directly, lines 10–14): `schema_version: "1.0"` · `register_version: "2.24"` · `last_modified: "2026-07-15"` · `last_modified_commit: "b265c43"` · `last_modified_by: "Claude Code"`. Note the header anchor is `b265c43` (the register-closure commit), **not** HEAD `f0f76a8` (F-13's render commit) — a T5-relevant fact.

**Independent id-prefix census** (ground-truth, not trusting the derived report):

| Collection | Count | Expression |
|---|---|---|
| documents | **288** ✓ | `rg -c '^  - id: DOC-' docs/governance/REGISTER.yaml` |
| requirements | 41 | `rg -c '^  - id: REQ-' docs/governance/REGISTER.yaml` |
| risks | 14 | `rg -c '^  - id: RISK-' docs/governance/REGISTER.yaml` |
| capa | 17 | `rg -c '^  - id: CAPA-' docs/governance/REGISTER.yaml` |
| audit_trail (EVT) | **47** ✓ | `rg -c '^  - id: EVT-' docs/governance/REGISTER.yaml` |
| total 2-space `- id:` | 407 | `rg -c '^  - id: ' docs/governance/REGISTER.yaml` |

Reconciliation: 288+41+14+17+47 = **407** ✓ — `2.24 / 288 docs / 47 EVT` verified from the SoT, not the projection.

**VALIDATION_REPORT.md header** (read as evidence, NOT regenerated): *Last run 2026-07-15T05:05:52Z · Schema 1.0 · Errors 0 · Warnings 25 · Documents enrolled 288 · Documents synced 0 · Counts documents=288, requirements=41, risks=14, capa=17, audit_trail=47 · Per-category E=70,D=89,G=8,F=72,B=7,I=0,H=2,C=3,J=0,A=37.*

**REGISTER_RENDER.md header** (read as evidence, NOT regenerated): *Last generated 2026-07-15 · Schema 1.0 · Register 2.24 · Total documents 288 · Tier 1:36 | 2:20 | 3:148 | 4:84 | 5:0 · Per category A=37,B=7,C=3,D=89,E=70,F=72,G=8,H=2,I=0,J=0 · Open CAPA 0 · Active risks 12 · Stale documents 0.*

---

## R2 Declared gate surface (numbered table)

Every gate/check/invariant the governance layer DECLARES, with a stable anchor, declared consequence, and whether ANY enforcer exists on disk. "Enforcer" column resolves against R3 (validator code), the compiled meta-tests, and the analyzer registry.

| # | Declared gate | Source anchor | Declared consequence | Enforcer on disk |
|---|---|---|---|---|
| G1 | **Document-changed-by-milestone**: files in git diff with no register update → fail | FRAMEWORK §2.5, §6.2 (strict), §6.1 step 1; METHODOLOGY §12.7 step 5–7, §12.9 (b) | **Error / blocks commit** | **NONE** — no `git diff` inspection anywhere in the validator |
| G2 | **Cross-reference integrity**: dangling IDs (`requirements_authored`→REQ, `risks_referenced`→RISK, `capa_entries_referenced`→CAPA, `deprecated_by`/`superseded_by`→DOC, `constrains`→DOC, `affected_documents`) | FRAMEWORK §6.2 (strict), §4.3, §4.4 | **Error / blocks commit** | **NONE** — no reference is ever resolved (only terminal-state *presence*, G7) |
| G3 | **Bidirectional supersession integrity**: target's `supersedes`/`deprecates` must include this ID; one-sided refs rejected | FRAMEWORK §3.3.2 | Error | **NONE** |
| G4 | **Lifecycle transition legality**: any transition not in the §3.3.1 matrix forbidden; "tooling validation blocks" | FRAMEWORK §3.3.1 | Error / blocks | **NONE** — validator is stateless (no prior-lifecycle stored) |
| G5 | **Schema enum validity**: category, tier, lifecycle, verification_status, likelihood, impact, risk_type, risk status, capa closure_status, event_type | FRAMEWORK §6.2 (strict); §3.1/§3.2/§3.3; §4.3–§4.7 | Error | **VALIDATOR** (sync_register.ps1 lines 161–163, 222, 235–245, 257, 270) ✓ |
| G6 | **Required-field presence**: id, path, category, lifecycle, tier | FRAMEWORK §6.2 (strict), §4.1 | Error | **VALIDATOR** (lines 146–150) — but only these 5; title/owner/version/content_language uncovered (see gap) |
| G7 | **Terminal-state cross-ref presence**: DEPRECATED→`deprecated_by`, SUPERSEDED→`superseded_by` | FRAMEWORK §3.3.2, §6.2 | Error | **VALIDATOR** (lines 182–187) — presence only, not resolution/bidirectionality |
| G8 | **Allowed-combinations matrix**: forbidden Category×Tier + Tier×Lifecycle without `special_case_rationale` | FRAMEWORK §3.4, §3.4.1, §6.2 (strict) | Error | **VALIDATOR** (lines 125–129, 166–179) — **fully implemented**, all 11 §3.4.1 rows present ✓ |
| G9 | **Unique IDs** per collection | FRAMEWORK §5 ("IDs never reused") | Error | **VALIDATOR** (lines 153–157, 220, 233, 255, 268) ✓ |
| G10 | **File existence**: every registered `path` resolves on disk | FRAMEWORK §6 (implicit); protocol | Error | **VALIDATOR** (lines 190–195) ✓ |
| G11 | **Orphan detection**: every in-scope `.md` has a register entry | (emergent — NOT in FRAMEWORK §6.2's taxonomy) | — | **VALIDATOR** as WARNING (line 207); never blocks |
| G12 | **Schema-version compatibility** (major/minor gates) | FRAMEWORK §7.3 | Error/Warn | **VALIDATOR** (lines 83–96) ✓ |
| G13 | **Schema-history table**: presence + monotonic version progression | FRAMEWORK §7.4 | (validates) | **NONE** — validator reads only the scalar `schema_version` |
| G14 | **Meta-entry rule 1**: `meta_entry.path` must be in `docs/governance/` | FRAMEWORK §8.2 (1) | Fails if outside | **NONE** |
| G15 | **Meta-entry rule 3**: meta cannot go DEPRECATED without successor | FRAMEWORK §8.2 (3) | Error | **NONE** (only generic G7) |
| G16 | **Meta-entry rule 4**: FRAMEWORK/SYNTHESIS_RATIONALE schema-version must match REGISTER `schema_version` | FRAMEWORK §8.2 (4) | Error | **NONE** |
| G17 | **Meta-entry rule 5**: DOC-G-REGISTER STALE if any milestone closes without updating REGISTER.yaml | FRAMEWORK §8.2 (5), §4.5 | STALE flag (load-bearing) | **NONE** (this is G1 restated for the SoT) |
| G18 | **STALE advisory**: `next_review_due < today` surfaces in VALIDATION_REPORT.md | FRAMEWORK §4.5, §6.2 (advisory) | Warning in report | **NONE in validator** — only the render emits an aggregate *count* (R3); zero per-doc warnings |
| G19 | **Tier-1 Russian-only flag**: Tier 1 + `content_language: ru` advisory | FRAMEWORK §9.3, §6.2 (advisory) | Warning | **NONE** — validator never inspects `content_language` |
| G20 | **AUTHORED > 30-day** brief advisory | FRAMEWORK §6.2 (advisory) | Warning | **NONE** — no lifecycle-date arithmetic |
| G21 | **Bypass-log review**: BYPASS_LOG.md has entries since last validation | FRAMEWORK §6.2 (advisory), §6.3 | Warning | **NONE** — BYPASS_LOG.md never inspected |
| G22 | **content_language enum** (`en`/`ru`/`mixed`) | FRAMEWORK §9.2 | (schema) | **NONE** |
| G23 | **Standing-doc amendment → REGISTER bump + audit-trail event** | TESTING_STRATEGY §9.1 (4); CODING_STANDARDS §10; METHODOLOGY §12.9 (a)/(b) | closure obligation | **NONE** (review-bound) |
| G24 | **Census-delta same-commit record** (Skeleton revisions) for pinned counts | RESERVED_SURFACE_MUTABILITY §5; TESTING_STRATEGY §4; CODING_STANDARDS §8 | closure obligation | **META-TEST (partial)**: pin drift caught by `CensusMetaTests`; the *record* obligation is review-only (F-33 was a real miss) |
| G25 | **Code-census pins** (`[ReservedStub` 34/13; stub/deferred/TODO/Phase6/not-yet; DFK-WAIVER 2; SuppressMessage 0) | TESTING_STRATEGY §4.1–§4.4 | build-breaking | **META-TEST** — `CensusMetaTests` (compiled xUnit) ✓ + closure-audit rg cross-check |
| G26 | **Citation form**: no version-pins of living docs, no URL-fragment anchors; `register_view_url` is the sanctioned machine-maintained exception | CODING_STANDARDS §6, §6.1 | review-bound | **NONE (honestly declared)** — "review-bound today — no analyzer rule detects" |
| G27 | **S-LOCK → verifying artifact must exist on disk** | TESTING_STRATEGY §4.4 | F-ledger gap | audit-bound (recorded) |

**Reading**: of the four gates FRAMEWORK §6.2 declares **strict / blocks-commit** (G1 doc-changed, G2 cross-ref, G5/G6 schema-integrity, G8 allowed-combinations), exactly **one (G8) is fully implemented**; G5/G6 are partial (enums + 5 required fields); **G1 and G2 do not exist in code at all**. All **four declared advisory gates (G18–G21) are absent**; the one advisory the validator *does* emit (G11 orphan) is **not in FRAMEWORK §6.2's taxonomy**. This is DF-DOC-002 made empirical.

By TESTING_STRATEGY §5.2's own standard — *"a document that says an invariant «is enforced» must name either the artifact … or the audit record; otherwise the claim is removed"* — FRAMEWORK §6.2's "blocks commit" language for G1–G4/G13–G21 names no artifact and should be reconciled.

---

## R3 Implemented checks + tooling map + THE ENFORCEMENT-GAP TABLE

### Tooling map (three PS scripts + one config; derived by READING code — validate/sync/render NOT run)

| Tool | Role | Writes | Notes |
|---|---|---|---|
| `tools/governance/sync_register.ps1` | write-side sync + validation | `docs/governance/VALIDATION_REPORT.md` (always, line 380); frontmatter mirrors (only under `-Sync` **and** errors==0, lines 281–348) | `-Validate` / `-Sync` / default(both). Exit 0/1/2. **Write-trap**: `-Validate` unconditionally rewrites the report (DEVELOPMENT_HYGIENE §4). |
| `tools/governance/render_register.ps1` | human-readable derivative | `docs/governance/REGISTER_RENDER.md` (line 173) | Pure projection, no checks. **Write-trap.** Allowed to go stale between cascades (DEV_HYGIENE §4). |
| `tools/governance/query_register.ps1` | read-side queries | — (read-only) | Third tool, **not named in the kickoff**. FRAMEWORK §6.5 agent self-check. Safe any time. **Not** a `last_modified_commit` consumer. |
| `tools/governance/SCOPE_EXCLUSIONS.yaml` | orphan-scan exclusions | — | Consumed by sync (+ conceptually render). `included_extensions` key is **dead config** (the scan hardcodes `git ls-files '*.md'`). |

### Implemented-checks table (every check `-Validate` performs)

| Check | Kind | Exit contribution | Fail-open note |
|---|---|---|---|
| powershell-yaml present | precondition | exit 2 | — |
| REGISTER.yaml present | precondition | exit 2 | — |
| schema_version parse/major/minor | ERROR/WARN | 1 / 0 | — |
| doc required fields (id/path/category/lifecycle/tier) | ERROR | 1 | narrower than §4.1 field list; **no** title/owner/version/content_language check |
| doc unique id | ERROR | 1 | — |
| doc enum (category/tier/lifecycle) | ERROR | 1 | — |
| forbidden Category×Tier w/o rationale | ERROR | 1 | **escape hatch**: any non-empty `special_case_rationale` waves it through (content unvalidated) |
| forbidden Tier×Lifecycle w/o rationale | ERROR | 1 | same escape hatch |
| DEPRECATED→deprecated_by / SUPERSEDED→superseded_by | ERROR | 1 | **presence only** — not resolved, not bidirectional |
| doc file existence | ERROR | 1 | comment at line 189 claims a "PENDING-INITIAL exemption handled elsewhere" — **no such handling exists in the file** (latent) |
| **orphan** (`git ls-files '*.md'` ∉ register, ∉ exclusions) | **WARNING** | **0** | **fail-open**: never blocks; only git-*tracked* .md seen (untracked invisible) |
| req/risk/capa/evt id present + unique + enum | ERROR | 1 | — |

**Exit-code truth**: `-Validate` exits 1 only if ≥1 ERROR. The implemented ERROR set (enums, 5 required fields, unique ids, combos, file existence) is fully satisfiable by a hand-curated corpus **while G1/G2/G3/G4/G13–G21 hold or fail silently**. Hence `Errors: 0` is structurally guaranteed and is **not evidence** of register integrity — it is evidence only that the ~10 implemented predicates pass.

**What `-Sync` writes** (lines 291–345): a frontmatter mirror carrying exactly **8 fields** — `register_id, category, tier, lifecycle, owner, version, next_review_due, register_view_url`. It does **not** carry `last_modified` or `last_modified_commit`. README.md → end-placed; others → top. Prior register-FM blocks stripped (leading + trailing). Non-`.md` paths skipped. Runs only when errors==0.

**What the render reads/emits** (render_register.ps1): statistics (tier/category counts), per-category doc blocks, four global tables. Reads `last_modified_commit` at **line 105** (gated on `last_modified` present). Emits **12 explicit anchors only** — 8 `<a name="category-X">` (A–H) + 4 global; **zero** `<a name="DOC-…">`. Computes "Stale documents" by `lifecycle==STALE OR next_review_due < today` (string comparison, line 51–53).

### THE ENFORCEMENT-GAP TABLE (DF-DOC-002, empirical) — declared gates with NO implemented check

| Gap | Declared as | Implemented? | Fail-loud classification |
|---|---|---|---|
| **G1 doc-changed-by-milestone** (the framework's single most load-bearing gate) | strict/blocks (FRAMEWORK §2.5/§6.1/§6.2/§6.4; METHODOLOGY §12.7-7/§12.9-b; §8.2-5) | **NO** | check never authored — worse than skip-on-absent |
| **G2 cross-reference integrity** (dangling REQ/RISK/CAPA/DOC ids) | strict/blocks (§6.2; SYNOPSIS line 7 claims it) | **NO** | never authored; SYNOPSIS overclaims |
| **G3 bidirectional supersession** | error (§3.3.2) | **NO** | never authored |
| **G4 lifecycle-transition legality** | blocks (§3.3.1) | **NO** | stateless validator can't; declared anyway |
| **G13 schema-history monotonicity** | validates (§7.4) | **NO** | never authored |
| **G14–G17 meta-entry rules 1/3/4/5** (4 of 5 special rules) | error/STALE (§8.2) | **NO (0/5 functional)** | never authored |
| **G18 STALE-in-VALIDATION_REPORT** | advisory warning (§4.5/§6.2) | **NO** (render count only; 0 validator warnings) | precondition (a per-doc stale scan) absent |
| **G19 Tier-1 ru flag** | advisory (§9.3) | **NO** | `content_language` never read |
| **G20 AUTHORED>30d** | advisory (§6.2) | **NO** | no date arithmetic |
| **G21 bypass-log review** | advisory (§6.2/§6.3) | **NO** | BYPASS_LOG never read |
| **G22 content_language enum** | schema (§9.2) | **NO** | — |

### 25-warning cross-check

All **25** advisory warnings in the on-disk VALIDATION_REPORT.md are the **single orphan code-path** (line 207). The other declared-advisory paths (G18–G21) emit nothing because they are absent. Verbatim reproduction of the orphan set from the validator's own logic (`git ls-files '*.md'` minus SCOPE_EXCLUSIONS minus registered paths): tracked `.md` = **310**, registered `.md` = **285**, orphans = **25** — line-for-line identical to the report. So the report is current at HEAD, and the "warnings" channel carries exactly one check.

---

## R4 Corpus violation populations

### R4.1 PENDING taxonomy (audit claimed 186 @ `61f08ef`; measured @ HEAD `f0f76a8`)

| Measure | Count | Expression |
|---|---|---|
| lines containing `PENDING` | 301 | `rg -c 'PENDING' docs/governance/REGISTER.yaml` |
| total `PENDING-` literal tokens | **294** | `rg -o 'PENDING-[A-Za-z0-9_]+' docs/governance/REGISTER.yaml \| wc -l` |
| — `PENDING-INITIAL` | 161 | `rg -o 'PENDING-[A-Za-z0-9_]+' … \| sort \| uniq -c` |
| — `PENDING-COMMIT` | 130 | (same) |
| — `PENDING-RATIFICATION` / `-CODEX_CLOSURE` / `-CLOSURE` | 1 / 1 / 1 | (same) |
| as `last_modified_commit:` value | **186** | `rg -c '^\s*last_modified_commit: "?PENDING' docs/governance/REGISTER.yaml` |
| as `closed_commit:` value | 8 | `rg … '^\s*[a-z_]+: "?PENDING-…'` field split |
| as `verification_commit:` value | 7 | (same) |

The audit's "186" maps to the `last_modified_commit`-anchored count and is **unchanged at HEAD** (the CODEX_CLOSURE cascade touched render/F-13, not PENDING). `PENDING-COMMIT` specifically: F-2 tracked **123** on 2026-06-11; measured **130** at HEAD — the backfill debt is **growing**, not shrinking (F-29 + CODEX_CLOSURE added ~7 without backfill).

### R4.2 Orphan population — TRIAGED (all 25, per-entry verdict; no bare count)

| # | Orphan path | Verdict | Rationale |
|---|---|---|---|
| 1–2 | `tools/DualFrontier.Analyzers/AnalyzerReleases.{Shipped,Unshipped}.md` | **(a) EXCLUDE** | Roslyn-managed release-tracking artifacts; add a SCOPE_EXCLUSIONS pattern |
| 3–14 | 11× `src/DualFrontier.Runtime/**/MODULE.md` + `tests/DualFrontier.Runtime.Tests/Vulkan/MODULE.md` (12 total) | **(b) ENROLL** | Category F Tier 4 module-local docs per FRAMEWORK §3.1 — exactly what F is for |
| 15–20 | `docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT{,_PHASE_4}.md`, `docs/scratch/A_PRIME_7_K10_3/HALT_REPORT{,_ADDENDUM_2026_05_19}.md`, `docs/scratch/V0_C_2/MANUAL_VISUAL_VERIFICATION_PROTOCOL.md`, `docs/scratch/V1_V2/V1_MANUAL_VERIFICATION_PROTOCOL.md` (6 files) | **(c) EXCLUDE (historical scratch)** | Under `docs/scratch/**`; add a scope pattern (kin of HALT reports) |
| 21 | `docs/prompts/PHASE_BETA_PREP_EXECUTION_PROMPT.md` | **(d) UNCLEAR** | `docs/prompts/` is mixed — sibling HOUSEKEEPING_* prompts ARE enrolled; enroll (Category E) or exclude — architect decides |
| 22–25 | `tools/briefs/{BRIEF_SKELETON_FRAMEWORK,K10_3_EXECUTION,K10_AMENDMENTS_APPLICATION,K8_5_DEFERRAL_CASCADE}_BRIEF.md` | **(d) UNCLEAR** | Category D briefs; most briefs ARE enrolled — these 4 are either enroll-as-EXECUTED/DEPRECATED or superseded scratch |

Bucket totals: **(a) EXCLUDE-tooling = 2 · (b) ENROLL = 12 · (c) EXCLUDE-scratch = 6 · (d) UNCLEAR-architect = 5** (1 prompt + 4 briefs). 2+12+6+5 = **25** ✓ (row 3–14 spans the 12 MODULE.md files).

### R4.3 Frontmatter-mirror drift — FULL census (scratch script over on-disk mirrors; audit claimed 28)

Measured: 288 register docs parsed → 264 `.md` compared (3 non-`.md` = REGISTER.yaml + 2 Launcher `.cs`, correctly unmirrored). **Total discrepancies = 28** (= the audit's 28), decomposed:

| Class | Count | Detail |
|---|---|---|
| **Field-value drift** (mirror exists, stale vs register) | **7** | `next_review_due` ×4, `lifecycle` ×2, `version` ×1; `register_id`/`category`/`tier`/`owner`/`register_view_url` = 0 |
| **Hand-authored non-canonical mirror** (`register_id` present, no `# Auto-generated` header; `-Sync` would overwrite) | **13** | the recent cascade briefs (CODEX_CLOSURE, F29, F10, PHASE_BETA/GAMMA/DELTA, GODOT_ERADICATION ×2, STANDING_LAW, ARCHITECTURE_TRUTH) + 2 dual-load drift reports |
| **No frontmatter at all** | **8** | 6 recon/closure reports + the 2 structurally-unmirrorable derived files (REGISTER_RENDER.md, VALIDATION_REPORT.md) |

Worst offenders (register → mirror): `A_PRIME_9_RECONNAISSANCE_REPORT.md` lifecycle `EXECUTED`→`Live`; `A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` lifecycle `EXECUTED`→`AUTHORED`; `KERNEL_FULL_NATIVE_SCHEDULER.md` version `2.0`→`2.0.1`; `METHODOLOGY.md`/`ANALYZER_RULES.md` `next_review_due` register-newer; `ROADMAP.md` `next_review_due` `null`→`2026-Q3`. Example non-canonical FM (`CODEX_CLOSURE_BRIEF.md`): `lifecycle: Draft (-> LOCKED on Crystalka ratification -> EXECUTED at cascade closure)` — **prose in an enum field**, `owner: Volodymyr (Crystalka)`, no `register_view_url`. The mirror layer is systematically stale because `-Sync` runs only at register cascades (DEV_HYGIENE §4) — **and the validator has no mirror-consistency check, so all 28 are invisible to `-Validate`.**

### R4.4 Cross-reference integrity (audit claimed 18 broken links)

| Class | Result | Expression |
|---|---|---|
| Registered `path:` fields not on disk | **0** (clean) | 288 `path:` tested (`rg -c '^    path:'` = 288); bash existence loop → 0 missing, 3 non-`.md` (REGISTER.yaml + 2 `.cs`) |
| `register_view_url` `#DOC-…` anchors resolving against the render | **269 unresolvable** | render has 12 explicit anchors, **0** per-DOC (`rg -c '<a name="DOC-' REGISTER_RENDER.md` → 0); 269 mirrors carry `#DOC-` anchors (`rg -l 'register_view_url: .*#DOC-' \| wc -l` → 269) |

The one cross-ref the validator *does* check (G10 path existence) is **clean**. The broken class is the `register_view_url` anchor — **269** frontmatter references to `REGISTER_RENDER.md#DOC-<id>` that resolve to no anchor (the render emits none; GitHub auto-slugs from the heading text — lowercased, title-suffixed — so `#DOC-A-FRAMEWORK` never matches). This is the class CODING_STANDARDS §6.1 explicitly *blesses* as the "sanctioned machine-maintained exception" to its URL-fragment-anchor prohibition — trusted because a machine owns it, silently broken because the render never emitted the matching anchor.

### R4.5 next_review_due state (tie to G18)

- **71** entries carry `next_review_due` (`rg -c '^\s*next_review_due:'` → 71; of 288 docs).
- Value shapes: `"2026-Q3"` ×20 (quarter), ISO `2027-05-*`/`2027-06-*` (~30), `"null"` ×9, `"TBD …"` ×3, `"on-refactor-cascade-execution"` ×2, `"post-A'.9.1 …"` ×1.
- **Past-due @ 2026-07-15 by string comparison: 0** (all ISO values are `2027-*`; hence render "Stale documents: 0").
- **~35 of 71 carry non-ISO values** (`2026-Q3`, `null`, `TBD`, `on-…`, `post-…`) that the `next_review_due < today` string compare **cannot correctly evaluate** — they sort lexically after `2026-07-15` and can never trip stale. **Validator does nothing** with the field (G18 absent); the render only aggregates a count; `query_register.ps1 -Stale` replicates the same fragile compare.

---

## R5 last_modified_commit consumption map

### R5.1 Consumers

| Consumer | Reads `last_modified_commit`? | Evidence |
|---|---|---|
| `render_register.ps1` | **YES — the only functional consumer** | line 105, gated on `last_modified` present → emits "Last modified: <date> (`<commit>`)" |
| `sync_register.ps1 -Sync` (mirrors) | **NO** | mirror carries 8 fields, none is `last_modified_commit`; `rg -l 'last_modified_commit:' -g '*.md' src tests` → 0 |
| `query_register.ps1` | NO | no reference |
| tests (`CensusMetaTests` et al.) | NO | src-census only |
| EVT audit_trail prose | references `PENDING-COMMIT-N` **literals** | 76 occ. at/after line 7096 (e.g. `range: "24e5f56..PENDING-COMMIT-4"`, `hash: "PENDING-COMMIT-4"`) — a *second, distinct* PENDING use as commit-range labels |

### R5.2 Option-B precedent inventory

- **F-2 ledger row** (ROADMAP Findings ledger): *"`PENDING-COMMIT` placeholders … backfill discipline systemically open. Count 124→123 … OPEN … Future register-tooling cascade (hybrid reverse-register, Q-T-2)."* → the store-vs-derive decision is already parked here and already leans "hybrid."
- **HEADER-anchor backfill**: the register-level `last_modified_commit` IS backfilled each cascade (currently `b265c43`), per FRAMEWORK §8.3 amend pattern.
- **Per-entry backfill**: **never systematically performed** — 186 `PENDING` remain; the debt grew 123→130 `PENDING-COMMIT` since 2026-06-11.

### R5.3 Content-integrity mechanism

**None.** `rg -c '^\s*(checksum|content_hash|sha256|sha|md5|size|byte_count):' docs/governance/REGISTER.yaml` → 0. `last_modified_commit` is *provenance*, not integrity; there is no way today to detect that a file's content changed under a stored hash.

### R5.4 Field information value today (store-vs-derive crux)

`last_modified_commit` fields = **289** (288 docs + header). Split: **186 PENDING (64%)** + **103 real hash (36%)** — `rg -c '^\s*last_modified_commit: "?PENDING'` = 186, `rg -c '^\s*last_modified_commit: "[0-9a-f]'` = 103, sum 289.

Git-log staleness sample (10 real-hash docs, even spread; `git log -1 --format=%h -- <path>` vs stored): **AGREE 4 / DISAGREE 6**. E.g. `KERNEL_ARCHITECTURE.md` stored `11f82bb` vs actual `8ec64c5`; four briefs stored old hashes vs actual `5ef387c`. Caveat: `5ef387c` = *"fix(governance): de-duplicate frontmatter mirrors + register VALIDATION_REPORT"* (2026-05-12), which touched **228 .md files** — so `git log -1` conflates a *mechanical sync* with a substantive edit. Net: only ~40% of the 36% real hashes are currently accurate → **~14% of the whole field population is both real and correct**; 64% is placeholder, ~22% is stale hash. (Also: `last_modified` *dates* include placeholders like `"2026-04-XX"` — the date field itself is unreliable.)

---

## R6 .NET substrate feasibility facts

Throwaway probe under `%TEMP%` on a COPY of REGISTER.yaml (deleted at session end); YamlDotNet **16.3.0**, dotnet **10.0.204**, `ManagePackageVersionsCentrally=false` (outside repo CPM).

### R6.1 YamlDotNet round-trip

| Fact | Measurement |
|---|---|
| BOM | UTF-8 BOM present (`ef bb bf`); parsed fine after explicit strip; serializer emits **no BOM** by default → a writer must re-add it |
| Line endings | **LF-only** (no CRLF) despite Windows |
| Parse time | **248 ms** (object-model deserialize of 11,033 lines / 684 KB / 407 entries) |
| Data fidelity | **LOSSLESS** — documents=288, requirements=41, risks=14, capa=17, audit_trail=47 all preserved; `register_version=2.24`, `last_modified_commit=b265c43` preserved; top-level key order preserved |
| **Comment loss** | **79 of 84 lost** in the object model; the representation model (`YamlStream`) preserves only **5** — neither round-trips comments |
| Quoting shift | `register_version: "2.24"` → `2.24`; `schema_version: "1.0"` → `1.0`; dates unquoted — **string→scalar semantic change** (would break exact-string logic) |
| Reflow | output **12,723 lines vs 11,033** (+15%); list indentation changed; header provenance block gone |

Comment census (what a writer must preserve): **84 full-line comments** (`rg -c '^\s*#'`), of which **33** are `# === … ===` section banners and the lines 1–8 header provenance block ("DO NOT EDIT FRONTMATTER MIRRORS MANUALLY … auto-generated by sync_register.ps1"). **Verdict: a .NET tool can READ/VALIDATE the register cheaply and losslessly, but must NOT WRITE it back via YamlDotNet serialize** (79/84 comment loss + quoting corruption + reflow).

### R6.2 CensusMetaTests repo-discovery precedent

`tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs`: `RepoRoot()` walks up from `AppContext.BaseDirectory` to the dir containing `DualFrontier.sln` (lines 19–28); `git ls-files` enumeration (lines 30–57). This is the exact pattern a governance meta-test would reuse to locate `docs/governance/REGISTER.yaml`. The meta-test infrastructure **exists and is proven** — it materializes all of TESTING_STRATEGY §4's *code* census pins as compiled xUnit assertions — but has **zero** coverage of the register/governance corpus.

### R6.3 Solution layout + CPM

- `DualFrontier.sln` (root) already carries `DualFrontier.Analyzers` (tools/) + `DualFrontier.Analyzers.Tests` (tests/). A `tools/DualFrontier.Governance/` + `tests/DualFrontier.Governance.Tests/` pair mirrors this exactly.
- `Directory.Packages.props` (CPM) = 9 `PackageVersion` entries; **xunit 2.9.2 + FluentAssertions 6.12.1 present**, **YamlDotNet absent** (would be one CPM add).

### R6.4 PowerShell-substrate facts (migration decision)

- Shell: **Windows PowerShell 5.1 only** (`pwsh` absent — DEV_HYGIENE §4). `powershell-yaml` **0.4.12** installed. `-ExecutionPolicy Bypass` declined by the machine's auto-mode classifier → the in-session `& .\…` form is required.
- **Retire vs keep blast radius**: `sync_register` is cited in **20+** tracked files (READMEs, FRAMEWORK, METHODOLOGY, TESTING_STRATEGY, BYPASS_LOG, audit passes) + all 264 mirror auto-gen comments; `render_register` cited in **20+** (FRAMEWORK, METHODOLOGY, DEV_HYGIENE, MODULE.md, ROADMAP, many briefs). Retiring the script names strands those prose citations; **adding** a read-only .NET tool alongside is far lower blast-radius than **replacing** the PS writers. The PS tools are load-bearing precisely as WRITERS (they never round-trip REGISTER.yaml — they `ConvertFrom-Yaml` then write *derived* files, which is why the 84 comments survive today).

---

## R7 Anomalies + fix directions + sizing

### R7.1 Anomaly sweep (divergences from kickoff expectations)

1. **`RESERVED_SURFACE_MUTABILITY.md` is at `docs/methodology/`, not `docs/architecture/`** as the kickoff T2 cited (confirmed by glob + METHODOLOGY §12.8 link). Kickoff path typo — no repo issue.
2. **`special_case_rationale` = 95 occurrences (`rg -c '^\s*special_case_rationale:'`) ≈ 33% of 288 documents** — *above* FRAMEWORK §10 falsifiable-claim-#5's own **20%** "matrix is wrong" threshold. The framework declares a falsification test it has never run (the validator doesn't count escape-hatch usage). Architect to adjudicate whether this is "matrix wrong" or "edge-tolerant"; the *measured fact* crosses the stated line.
3. **The load-bearing gate G1 is claimed by four documents** (FRAMEWORK, METHODOLOGY ×2 sections, restated as meta-rule §8.2-5) yet **implemented by none** — the enforcement claim propagated across the standing corpus without an artifact, violating TESTING_STRATEGY §5.2 ("name the artifact or remove the claim").
4. **Lesson #23 candidate (METHODOLOGY §1152)** already records register-classification-drift (`DOC-D-K8_5` AUTHORED vs skeleton content, undetected from A'.4.5) and already names the fix ("register classification verification rules … deferred to A'.9 analyzer rule specification phase"). The gap is **known and parked**.
5. **PENDING-COMMIT is growing** (123 @ 2026-06-11 → 130 @ HEAD): backfill discipline is regressing, not merely open.
6. **`last_modified` carries placeholder *dates*** (`"2026-04-XX"`) — not just the commit field.
7. **SYNOPSIS overclaim**: `sync_register.ps1` line 7 advertises "validates schema and cross-reference integrity"; cross-reference resolution (G2) is absent.

### R7.2 Rule-catalog shape a validator-2.0 needs (from the R2−R3 gap)

- **Become machine rules (data-only, no schema change)** — implementable as .NET governance meta-tests (read-only) or PS checks: G2 dangling-id resolution; G3 bidirectional supersession; G13 schema-history monotonicity; G14/G16 meta-path + schema-version coupling; G18 STALE-in-report (with an ISO-vs-quarter date parser); G19 Tier-1 `ru`; G22 `content_language` enum; **mirror-consistency** (the 28-drift check, new); **`register_view_url` anchor resolution** (the 269 broken, new — or drop the anchors); **`special_case_rationale` ratio monitor** (§10 #5).
- **Need a schema change first**: G1 doc-changed-by-milestone (requires a git-diff-aware pass + a per-doc "expected touch" contract) and G4 lifecycle-transition legality (requires storing prior lifecycle or reading git history) — stateless YAML can't express them.
- **Stay review-only (honestly declared)**: G20 AUTHORED>30d (low value), G23 amendment→register-bump, G26 citation-form (already so), G27 S-LOCK audit.

### R7.3 Schema-question options with R5 data attached

| Option | What it does | R5 evidence for/against |
|---|---|---|
| **Keep-and-backfill** | retain `last_modified_commit`; backfill all 186 PENDING to real hashes | 64% placeholder + ~60% of the real ones already stale + growing debt ⇒ high recurring cost, low fidelity |
| **Stop-storing-and-derive** | drop the field; render derives via `git log -1 --format=%h -- <path>` | always fresh, zero maintenance; **but** `git log -1` conflates mechanical sync commits (e.g. `5ef387c` = 228-file frontmatter sync) with substantive edits |
| **Hybrid (F-2 "reverse-register", Q-T-2)** | derive at render; keep an explicit stored value only where a curated provenance anchor matters (meta-entries, closures) | matches F-2's existing lean; content-integrity (a hash) remains a separate, currently-absent mechanism if desired |

### R7.4 Comment-preservation directions for a .NET writer (R6.1)

- **Read-only .NET tool + PS retained as writer** (lowest risk): the governance meta-test suite READS REGISTER.yaml (248 ms, lossless) and asserts G2/G3/G5/…; PS `-Sync`/render remain the only writers (they never round-trip the SoT, so the 84 comments survive). Mirrors the proven `CensusMetaTests` pattern.
- **Comment-preserving YAML**: YamlDotNet does not preserve comments in either model (measured 79/84 loss); .NET has no strong round-trip-preserving YAML equivalent to Python's `ruamel.yaml` — treat a comment-preserving .NET *writer* as high-effort/unproven.
- **Migrate comments into data** (schema change): move the header provenance + section banners out of YAML comments into structured keys (e.g. `_provenance:`, per-collection `_section:`) so any serializer preserves them — enables a .NET writer but is a schema amendment (FRAMEWORK §7 MINOR).

### R7.5 Sizing — proposed cascade split (instrument-before-cleanup)

**Cascade A — the instrument (tool-build), do first.** Build `tools/DualFrontier.Governance` + `tests/DualFrontier.Governance.Tests` (mirror the Analyzers pair; CPM add YamlDotNet), read-only, reusing `RepoRoot()`. Materialize as meta-tests the machine-able gaps: G2 dangling-id (target: 0), G3 bidirectional, mirror-consistency (target: the 28→0 after Cascade B), `register_view_url` resolution (269), G18 STALE with a real date parser, G22/G19 enums/flags, and the `special_case_rationale` ratio monitor. Commit classes: (1) project skeleton + sln/CPM wiring; (2) register-load + RepoRoot fixture; (3–n) one commit per gate-family with positive/negative fixtures; (final) FRAMEWORK §6.2 reconciled to name each artifact (TESTING_STRATEGY §5.2). Decide store-vs-derive (R7.3) here since it changes what the render/tests assert.

**Cascade B — corpus cleanup, gated by A's instruments.** Per-class counts from R4/R5:
- Orphans (R4.2): **+12 ENROLL** MODULE.md, **+2 EXCLUDE** AnalyzerReleases, **+5 EXCLUDE** scratch (SCOPE_EXCLUSIONS patterns), **5 UNCLEAR** briefs/prompt → architect ruling. Commit class: SCOPE_EXCLUSIONS edit + register enrollment block.
- Mirror re-sync (R4.3): one ratified `-Sync` clears the 7 field-drifts + 13 hand-authored (2 derived files stay unmirrorable by design). Commit class: single `-Sync` fold per DEV_HYGIENE §4.
- `register_view_url` (R4.4): either the render emits `<a name="DOC-…">` per doc (render-script edit, one commit) **or** the anchor is dropped (schema/render decision) — resolves 269.
- PENDING disposition (R4.1/R5): 186 `last_modified_commit` + 8 `closed_commit` + 7 `verification_commit` + 76 EVT-prose labels — dispositioned per R7.3's chosen option; F-2 closes here. Commit class: bulk register edit (large diff) — **do last**, after the instrument can prove it.

Instrument-before-cleanup ordering is essential: Cascade A's tests are what make Cascade B's 28→0 / 269→0 / PENDING→disposition *verifiable* rather than asserted — the exact `Errors: 0`-is-not-evidence failure this recon documents.

---

## R8 Self-attestation

- Zero repo writes except the report file at `docs/reports/VALIDATOR_2_RECON_REPORT.md` (validate NOT run; render NOT run; sync NOT run): **CONFIRMED** — the two derived files were read as evidence only; all three PS scripts' behaviour was derived by reading their code.
- Out-of-repo scratch deleted; git status shows only the untracked report: **CONFIRMED at authoring** — all probes (`mirror_drift.py`, `t5_gitlog.py`, `orphan_repro.py`, `yamlprobe/`, `REGISTER.copy.yaml`) live under `%TEMP%\…\scratchpad` and are deleted at session end; the working tree carries only the pre-existing ` M .claude/scheduled_tasks.lock` plus this untracked report.
- Report written to `docs/reports/` AND presented in chat (uncommitted): **CONFIRMED** — stays untracked; the VALIDATOR_2 cascade enrolls it at its first commit.
- Zero git mutations: **CONFIRMED** — only `git rev-parse`/`status`/`rev-list`/`branch`/`log`/`ls-files`/`show` (read-only).
- Every census expression recorded verbatim: **CONFIRMED** — each count in R1/R3/R4/R5/R6 carries its `rg`/`git`/script expression.
- Every orphan classified (no bare counts): **CONFIRMED** — all 25 carry a per-entry verdict in R4.2.

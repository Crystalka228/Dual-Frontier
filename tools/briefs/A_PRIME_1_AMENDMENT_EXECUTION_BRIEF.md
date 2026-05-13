---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_1_AMENDMENT_EXECUTION
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_1_AMENDMENT_EXECUTION
---
# A'.1 Amendment Execution Brief — combined methodology + architecture corpus landing

**Brief type**: Execution brief (mechanical landing of pre-decided amendments; two parts, two sessions)
**Authored**: 2026-05-10 (Opus 4.7, A'.0.7 closure session post-`86b721a`)
**Target sessions**: Two Cloud Code sessions (execution mode), invoked against this brief in sequence:
- **Session 1 — Part M** (A'.0.7 methodology amendment landing)
- **Session 2 — Part K** (К-L3.1 architecture amendment landing)
**Estimated session length**: 30–60 min per Part (60–120 min cumulative; docs-only, no source changes)
**Status**: AUTHORED — awaits execution
**Prerequisite**: A'.0.7 closure (commit `86b721a` on main, 2026-05-10); two amendment plans landed at `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` (`c2b83b4`) and `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (`45d831c`)
**Blocks**: A'.3 push to origin, A'.4–A'.7 К-series execution
**Does not block**: nothing (sequential)

---

## §1 Session purpose (both Parts)

A'.0.7 deliberation closed 2026-05-10 (`86b721a`). К-L3.1 deliberation closed 2026-05-10 (`45d831c`). Both produced executable amendment plans с per-document old/new text pairs. Phase A'.1 lands those amendments mechanically.

This brief uses **standalone path** per A'.0.7 amendment plan §7.2: two parts, two sessions, different document scopes (methodology vs architecture corpus). Standalone preserves clean per-amendment review surface; combined session would mix scopes within one session, sacrificing isolation.

### §1.1 Sequencing rationale (Part M before Part K)

Per A'.0.7 amendment plan §5.3: **Part M executes first**.

Reasoning:
- A'.0.7 amendment lands methodology framing (architect-mode / executor-mode terminology, two-track synthesis principle, agent-as-primary-reader audience contract, v1.6 era classification).
- Part K commits can then reference this terminology in commit messages с consistent vocabulary.
- Reverse order works mechanically but produces terminologically uneven commit log.

**Recovery if Part M fails**: Part K still executable; uses pre-A'.0.7 К-L3.1 terminology. Commit log shows terminology shift retroactively. Recoverable, not blocking.

### §1.2 Pipeline reality

Per Q-A07-6 audience contract + Q1.b + Q2.α (locked A'.0.7 2026-05-10): pipeline is 2-agent unified Claude Desktop session с deliberation/execution modes. Both Parts execute in **execution mode** (Cloud Code agent, autonomous tool-loop). All architectural deliberation completed pre-A'.1; this brief is mechanical landing only.

Per K-Lessons «brief authoring as prerequisite step»: this brief itself committed as separate prerequisite commit before Part M execution starts. See §3.0 below.

### §1.3 Combined commit shape summary

| Part | Commits | Scope |
|---|---|---|
| Step 0 (prerequisite) | 1 | This brief committed |
| Part M (A'.0.7 amendment) | 5 | 4 methodology corpus docs + 1 closure tracker |
| Part K (К-L3.1 amendment) | 10 | 4 architecture corpus docs + 4 skeleton brief surgical edits + 2 closure trackers |
| **Total** | **16** | All docs-only; baseline 631 preserved throughout |

**Branch state at end**: `main`, 16 commits ahead of pre-Step-0 HEAD (`86b721a`), ~20 commits ahead origin/main (depending on push state pre-Step-0).

Push deferred к A'.3 per Phase A' default sequencing.

---

## §2 Pre-flight (Phase 0) — runs before Step 0; verified again before each Part

### §2.1 Hard gates (STOP-eligible)

| # | Check | Expected |
|---|---|---|
| HG-1 | Working tree clean | `git status` shows no uncommitted changes к `src/`, `tests/`, `docs/`, `tools/` |
| HG-2 | Branch identity | `git rev-parse --abbrev-ref HEAD` shows `main` |
| HG-3 | HEAD identity | At Step 0: `git rev-parse HEAD` shows `86b721a59cc4e227514125abac77836d4cb3e764` (или descendant if brief authoring already landed) |
| HG-4 | Test baseline | `dotnet test` shows 631 passing, 0 failed, 0 skipped |
| HG-5 | A'.0.7 amendment plan present | `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` exists и readable, ~1460 lines |
| HG-6 | К-L3.1 amendment plan present | `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` exists и readable |
| HG-7 | A'.0.7 brief Status = EXECUTED | `grep -n "Status.*EXECUTED 2026-05-10" tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` returns hit |
| HG-8 | A'.0.7 closure brief present | `tools/briefs/A_PRIME_0_7_CLOSURE_EXECUTION_BRIEF.md` exists |

**On any HG-X failure**: STOP, escalate to Crystalka, do not improvise.

**Note on dotnet test verbosity** (operational lesson from A'.0.7 closure session 2026-05-10): use `--logger "console;verbosity=minimal"` или explicit verbosity higher than `quiet`. The `--verbosity quiet` flag buffers test output and may stall completion signal detection. If 18-worker test run exceeds 5 minutes wall-clock с empty output streams, restart с explicit logger configuration.

### §2.2 Informational checks (record-only)

| # | Check | Record |
|---|---|---|
| IC-1 | Commits ahead of origin/main | Record actual at Step 0; will increase by commit count per Part |
| IC-2 | Last commit message | Expected «docs(briefs,progress): mark A'.0.7 brief EXECUTED + record A'.0.7 closure» (commit `86b721a`); record actual |
| IC-3 | К-L3.1 amendment plan §0 Locks summary table verifies Q1=a, Q2=β-i, Q3=i, Q4=b, Q5=a, Q6=a, Synthesis=§4.A | Verified during Part K pre-flight |
| IC-4 | A'.0.7 amendment plan §0 Locks summary table verifies all 15 locks landed | Verified during Part M pre-flight |

### §2.3 Expected layout (hypothesis per K-Lessons «Phase 0.4 inventory as hypothesis, not authority»)

**Files to be modified (Part M)**:

| Path | Expected state pre-Part-M | Operation |
|---|---|---|
| `docs/methodology/METHODOLOGY.md` | v1.5, A'.0.7 deferral marker at lines 1–26 | Substantial rewrite per A'.0.7 amendment plan §1; deferral marker removed |
| `docs/methodology/PIPELINE_METRICS.md` | v0.1, A'.0.7 deferral marker at lines 6–26 | Per-metric annotations + frame note + §6 forward measurement plan + version history sub-section + v0.1→v0.2; deferral marker removed |
| `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` | v1.0, A'.0.7 deferral marker at lines 5–22 | Surgical (§4.3 prompts/ + §5.2 mapping + §10 v1.1 row); deferral marker removed |
| `README.md` | A'.0.7 deferral marker comment at line ~50 | Pipeline section substantial rewrite per A'.0.7 amendment plan §4; deferral marker removed |
| `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` | Status EXECUTED, body intact | Optional: append amendment landing reference to Status line (Phase 4 closure of Part M) |
| `docs/MIGRATION_PROGRESS.md` | A'.0.7 closure entry present at lines 551–605 | Append A'.1.M closure entry или extend existing A'.0.7 entry с amendment landing reference |

**Files to be modified (Part K)**:

| Path | Expected state pre-Part-K | Operation |
|---|---|---|
| `docs/architecture/KERNEL_ARCHITECTURE.md` | v1.3 (header bump deferred from v1.4 K8.2 v2 closure) | Bump к v1.5 per К-L3.1 amendment plan §1; K-L3 row reformulation + status line update + K8.2 row amendment + Part 7 K-L3.1 entry |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | v1.6 LOCKED | Bump к v1.7 per К-L3.1 amendment plan §2; lines 1149–1150 rewrite + §3 capability model + §4.6 IModApi v3 + §11 migration phases + §12 D-1 |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | v1.0 LOCKED | Bump к v1.1 per К-L3.1 amendment plan §3; §0.1 Phase A' integration + §0.3 decision #9 + line 62 reformulation + §1.2/§1.3/§1.4 scope wording + §6 maintenance |
| `docs/MIGRATION_PROGRESS.md` | A'.0.7 closure entry recent (post-Part-M update may have extended) | Per К-L3.1 amendment plan §4: line 5 / line 34 / line 35 / line 407 / line 443 / line 457 + new K-L3.1 closure entry |
| `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` | Full authored brief, Phase 0.4 versions stale | Disposition B per К-L3.1 amendment plan §5.1: version + baseline refs only |
| `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` | Skeleton ~36 lines | Disposition B-to-C per К-L3.1 amendment plan §5.2: scope undercount correction + dual-path note |
| `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` | Skeleton ~33 lines | Disposition C.1 per К-L3.1 amendment plan §5.3: RegisterManagedComponent + ManagedStore |
| `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` | Skeleton ~30 lines | Disposition B per К-L3.1 amendment plan §5.4: bridge documentation expansion |

**Divergence handling**: per K-Lessons «Phase 0.4 inventory as hypothesis, not authority» — record divergences and continue if structural; STOP only on hard gate failure.

---

## §3 Execution sequence

### §3.0 Step 0 — Brief authoring commit (prerequisite)

Per K-Lessons «brief authoring as prerequisite step»: this brief committed as separate prerequisite commit before Part M execution.

**Commit**:
```
git add tools/briefs/A_PRIME_1_AMENDMENT_EXECUTION_BRIEF.md
git commit -m "docs(briefs): author A'.1 combined amendment execution brief

Single-combined brief per A'.0.7 amendment plan §7.2 standalone path
recommendation. Two-part execution:
- Part M: A'.0.7 methodology amendment landing (5 commits)
- Part K: К-L3.1 architecture amendment landing (10 commits)

Total 16 commits across two execution sessions. Part M executes first
per A'.0.7 amendment plan §5.3 (terminology established for Part K
commit messages).

Companion: docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md (Part M source)
Companion: docs/architecture/K_L3_1_AMENDMENT_PLAN.md (Part K source)

Test count delta: zero (brief authoring only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Brief location**: `tools/briefs/A_PRIME_1_AMENDMENT_EXECUTION_BRIEF.md` (this document).

**Verification**: working tree clean post-commit; HG-1 passes for Part M.

---

# PART M — A'.0.7 methodology amendment landing

**Source of truth**: `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` (commit `c2b83b4`; 1460 lines)

**Execution model**: agent reads amendment plan section by section, applies old/new text pairs к target files. Plan is **the contract**; brief specifies commit shape + verification only.

**Commit count**: 5 (4 amendment commits + 1 closure commit).

### §M.1 Pre-Part-M verification

Re-run HG-1 through HG-4 + HG-5 + HG-7 + HG-8 from §2.1. All must pass before proceeding.

Confirm A'.0.7 amendment plan structure intact:
```
head -10 docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md
grep -c "^## §" docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md
tail -3 docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md
```

Expected: 11+ H2 plan-level sections (§0–§9 plus embedded section headers from quoted content); first line «# A'.0.7 Amendment Plan — old/new text pairs»; last meaningful line ends с «**Plan end. Amendment brief authoring + execution is a separate session deliverable; this plan is the input contract.**».

If structure verified, proceed.

### §M.2 Phase 1 — METHODOLOGY.md amendment (Commit M-1)

**Operation**: apply all edits per A'.0.7 amendment plan §1.1 through §1.18 к `docs/methodology/METHODOLOGY.md`.

**Edits enumerated** (per amendment plan):
- §1.1 Header version line v1.5 → v1.6
- §1.2 §0 Abstract — Q1.b implementation (replace v1.5 §0 с new abstract framing + footnote)
- §1.3 §1 Context and problem — Q7.a-table verify clean (read pass; surgical scrub only if grep hits)
- §1.4 §2.1 Role distribution — Q2.α implementation (full section rewrite с abstract role categories + §2.1.1 current configuration table)
- §1.5 §2.2 Contracts as IPC — Q3.c-reformulated (full section rewrite с three-properties mechanism)
- §1.6 §2.3 Verification cycle — Q7.a-table mechanical scrub (replace 4-agent decomposition с v1.6 architect-mode/executor-mode session framing)
- §1.7 §2.4 Atomicity of phase review — Q7.a-table untouched, verify
- §1.8 §3 Economics — Q4.b-reformulated (replace §3.1-§3.3 wholesale с §3.1 invariant + §3.2 current config с A'.0.5 anchor; §3.3 discarded)
- §1.9 §4 Empirical results — Q5.c-decomposed (per-sub-section: §4.1 refresh с current snapshot table + §4.2 mechanical scrub + §4.3 untouched + §4.4 parallel-form Case A+B rewrite + §4.5 untouched)
- §1.10 §5 Threat model — Q6.α-b (§5.1 untouched / §5.2 substantial rewrite с v1.6 session-mode enumeration / §5.3 reformulated к 5 attack classes)
- §1.11 §6 Boundaries — Q6.b verify clean
- §1.12 §7 Operating principles — Q7.a-table untouched, verify
- §1.13 §8 Reproducibility — Q7.a-table verify clean (note: §8.1 v1.x reproducibility surface preserved verbatim per amendment plan note)
- §1.14 §9 Open questions — Q7.a-table per-question; «degradation as codebase grows» reformulation
- §1.15 §10 Change history — append v1.6 row
- §1.16 §11 See also — verify paths post-A'.0.5 reorg
- §1.17 Native layer methodology adjustments — Q9.b verify clean + Q8.c new sub-section addition («Phase A' lessons (post-A'.0.5)» с full «Milestone consolidation under session-mode pipeline» lesson formulation)
- §1.18 A'.0.7 deferral marker removal (lines 1–26)

**Commit M-1**:
```
git add docs/methodology/METHODOLOGY.md
git commit -m "docs(methodology): A'.0.7 amendment — METHODOLOGY v1.5 → v1.6 pipeline restructure rewrite

Per A'.0.7 amendment plan §1 (commit c2b83b4). Substantial rewrite of:
- §0 Abstract: abstract primary framing + v1.6 current-config footnote (Q1.b)
- §2.1 Role distribution: abstract role categories + §2.1.1 current config table (Q2.α)
- §2.2 Contracts as IPC: three-properties mechanism (Q3.c-reformulated)
- §3 Economics: invariant + current config с A'.0.5 anchor (Q4.b-reformulated)
- §4 Empirical results: per-sub-section с §4.4 parallel-form case studies (Q5.c-decomposed)
- §5.2/§5.3 Threat model: v1.6 session-mode enumeration + 5 attack classes (Q6.α)
- §9 «degradation as codebase grows»: pipeline-agnostic reformulation
- §10 Change history: v1.6 row appended

New «Phase A' lessons (post-A'.0.5)» sub-section с full lesson
«Milestone consolidation under session-mode pipeline» (Q8.c).

A'.0.7 deferral marker removed (lines 1–26 in v1.5).

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -n "Version: 1.6" docs/methodology/METHODOLOGY.md` returns hit
- `grep -c "A'.0.7 DEFERRAL MARKER" docs/methodology/METHODOLOGY.md` returns 0
- `grep -n "Phase A' lessons (post-A'.0.5)" docs/methodology/METHODOLOGY.md` returns hit
- `grep -n "Q-A07-6" docs/methodology/METHODOLOGY.md` returns hit (footnote reference)
- `grep -n "Crystalka + unified Claude Desktop session" docs/methodology/METHODOLOGY.md` returns hits в §0 footnote + §2.1.1 table region
- `grep -n "27523ac" docs/methodology/METHODOLOGY.md` returns hit (A'.0.5 anchor в §3.2 + §4.4 Case B)

### §M.3 Phase 2 — PIPELINE_METRICS.md amendment (Commit M-2)

**Operation**: apply all edits per A'.0.7 amendment plan §2.1 through §2.6 к `docs/methodology/PIPELINE_METRICS.md`.

**Edits enumerated**:
- §2.1 Top-of-document frame note inserted after deferral marker removal
- §2.2 Per-metric annotations inserted at 17 sub-section endpoints (§1.1 through §5.4 per amendment plan §2.2.1–§2.2.17)
- §2.3 New §6 Forward measurement plan inserted after §5.4 (before existing «See also» footer)
- §2.4 New «Version history» sub-section inserted at document end
- §2.5 Status line bump v0.1 → v0.2
- §2.6 A'.0.7 deferral marker removal (lines 6–26)

**Commit M-2**:
```
git add docs/methodology/PIPELINE_METRICS.md
git commit -m "docs(methodology): A'.0.7 amendment — PIPELINE_METRICS v0.1 → v0.2 per-metric annotations + forward measurement plan

Per A'.0.7 amendment plan §2 (commit c2b83b4). Document-specific
§4.C synthesis form: versioned-empirical-record.

Changes:
- Top-of-document era frame note (5 standardized transferability labels)
- Per-metric annotations applied к 17 sub-sections (§1.1-§1.4, §2.1-§2.3,
  §3.1-§3.3, §4.1-§4.3, §5.1-§5.4)
- New §6 Forward measurement plan tracks v1.6 era data collection backlog
  inline (per Q-A07-8 lock)
- New «Version history» sub-section
- Status line v0.1 → v0.2

v1.x era measurements preserved verbatim per Q-A07-3=β+γ lock.

A'.0.7 deferral marker removed (lines 6–26 in v0.1).

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -n "v0.2 (2026-05-10)" docs/methodology/PIPELINE_METRICS.md` returns hit
- `grep -c "A'.0.7 DEFERRAL MARKER" docs/methodology/PIPELINE_METRICS.md` returns 0
- `grep -c "Annotation.*A'.0.7, 2026-05-10" docs/methodology/PIPELINE_METRICS.md` returns at least 17 hits
- `grep -n "## §6 Forward measurement plan" docs/methodology/PIPELINE_METRICS.md` returns hit
- `grep -n "## Version history" docs/methodology/PIPELINE_METRICS.md` returns hit

### §M.4 Phase 3 — MAXIMUM_ENGINEERING_REFACTOR.md amendment (Commit M-3)

**Operation**: apply all edits per A'.0.7 amendment plan §3.1 through §3.6 к `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md`.

**Edits enumerated**:
- §3.1 Per-sub-section disposition table executed — bulk untouched
- §3.2 §4.3 prompts/ sub-tree rewrite (opus-architect → architect-mode; sonnet-decomposer removed; gemma-executor → executor-mode; boundary-type-config.md added; escalation-criteria.md text reframed)
- §3.3 §5.2 parallel-track discipline mapping rewrite (Sonnet/Gemma/Opus → architect-mode/executor-mode sessions с precedent cross-refs)
- §3.4 §10 Ratification — append v1.1 row
- §3.5 Verify-clean grep targets executed (Sonnet/Gemma/Opus/four-agent/local quantized/Cline/LM Studio outside §10 history entries — surgical scrub if hits)
- §3.6 A'.0.7 deferral marker removal (lines 5–22)

**Commit M-3**:
```
git add docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md
git commit -m "docs(methodology): A'.0.7 amendment — MAXIMUM_ENGINEERING_REFACTOR v1.0 → v1.1 pipeline-mapping update

Per A'.0.7 amendment plan §3 (commit c2b83b4). Q11-table per-sub-section
dispositions:
- §4.3 prompts/ sub-tree: opus-architect/sonnet-decomposer/gemma-executor
  → architect-mode/executor-mode + boundary-type-config.md added
- §5.2 parallel-track discipline: 4-agent mapping → v1.6 session-mode
  с К8.0/К-L3.1/A'.0.7/A'.0.5/К9/К8.3/К8.4/К8.5/Phase 4 closure
  precedent cross-refs
- §10 Ratification: v1.1 row appended

Tracks A/B/C architectural content untouched.
Document body bulk untouched (research-tier brief, methodology-character).

A'.0.7 deferral marker removed (lines 5–22 in v1.0).

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -n "v1.1.*2026-05-10" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` returns hit (in §10 Ratification)
- `grep -c "A'.0.7 DEFERRAL MARKER" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` returns 0
- `grep -n "architect-mode.md" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` returns hit (in §4.3 directory listing)
- `grep -c "opus-architect.md\|sonnet-decomposer.md\|gemma-executor.md" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` returns 0 in active content (acceptable in §10 v1.0 history entry only)
- `grep -n "architect-mode session" docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` returns hits in §5.2

### §M.5 Phase 4 — README.md amendment (Commit M-4)

**Operation**: apply all edits per A'.0.7 amendment plan §4.1 through §4.4 к `README.md`.

**Edits enumerated**:
- §4.1 Pipeline section replacement (Q12.c-formulation — hybrid abstract + current + historical + audience contract declaration + falsifiability claim generalized)
- §4.2 Cross-ref path verification (verify ROADMAP/ARCHITECTURE/MOD_OS/NORMALIZATION_REPORT/PIPELINE_METRICS anchor/METHODOLOGY anchor/NATIVE_CORE_EXPERIMENT/docs README paths)
- §4.3 Audience contract declaration verbiage included in §4.1 replacement
- §4.4 A'.0.7 deferral marker comment removal (HTML comment near line 50 в v1.5)

**Commit M-4**:
```
git add README.md
git commit -m "docs: A'.0.7 amendment — README Pipeline section v1.6 reality + agent-as-primary-reader audience contract

Per A'.0.7 amendment plan §4 (commit c2b83b4). Pipeline section
rewrite per Q12.c-formulation lock:
- Abstract framing primary: N agents в architect-executor split с
  contracts at boundaries
- Current configuration (v1.6, 2026-05-10): N=2 Crystalka + Claude
  Desktop session с deliberation/execution modes
- v1.x era (Phase 0-8, ending 2026-05-09): N=4 model-tier boundary
  с cross-ref к PIPELINE_METRICS for empirical record
- Audience contract declaration: agent-as-primary-reader assumption
  с AI tooling navigation guidance
- Falsifiability claim generalized: executor produces correct code
  on first build at measurable rate (target <30% requiring second
  execution)

A'.0.7 deferral marker comment removed (line ~50 в pre-amendment).

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -c "A'.0.7" README.md` returns at least 1 (current-configuration paragraph references K8.0/К-L3.1/A'.0.7/A'.0.5 precedents)
- `grep -c "TODO: A'.0.7" README.md` returns 0 (deferral marker comment removed)
- `grep -n "agent-as-primary-reader" README.md` returns hit (audience contract declaration)
- `grep -n "N=2: Crystalka" README.md` returns hit (current configuration paragraph)

### §M.6 Phase 5 — A'.1.M closure commit (Commit M-5)

**Operation**: update tracker artifacts.

**Edits**:

1. **A'.0.7 brief Status update** — append amendment landing reference to existing Status line:

`tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` line 8 — append к end of existing Status:

**Old text trailing**: `Amendment plan at `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` is executable artifact для follow-up amendment brief.`

**New text trailing**: `Amendment plan at `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` is executable artifact; AMENDMENTS LANDED 2026-05-10 via A'.1.M execution session (commits M-1 through M-4).`

2. **MIGRATION_PROGRESS A'.0.7 entry extension** — append «Amendment landing» sub-section to existing A'.0.7 closure entry (lines 551–605).

**Insertion location**: end of A'.0.7 closure entry (before `---` separator preceding M9-series section).

**New text appended к A'.0.7 entry**:

```

**Amendment landing (A'.1.M, 2026-05-10)**:

5 atomic commits landed amendments per `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` §1–§4 + closure:
- Commit M-1: METHODOLOGY.md v1.5 → v1.6 (§0 Abstract / §2.1 Role distribution / §2.2 Contracts as IPC / §2.3 Verification cycle / §3 Economics / §4 Empirical results / §5.2-§5.3 Threat model / §9 «degradation as codebase grows» / §10 v1.6 row / new «Phase A' lessons (post-A'.0.5)» К-Lessons sub-section)
- Commit M-2: PIPELINE_METRICS.md v0.1 → v0.2 (top-of-document era frame note + per-metric annotations с 5 standardized transferability labels at 17 sub-sections + new §6 Forward measurement plan + Version history sub-section)
- Commit M-3: MAXIMUM_ENGINEERING_REFACTOR.md v1.0 → v1.1 (§4.3 prompts/ rewrite + §5.2 parallel-track mapping rewrite + §10 v1.1 row)
- Commit M-4: README.md Pipeline section rewrite (Q12.c-formulation hybrid + agent-as-primary-reader audience contract declaration)
- Commit M-5: this closure entry + A'.0.7 brief Status amendment landing reference

Test count delta: zero across all 5 commits (docs-only).
Working tree clean post-A'.1.M; baseline 631 preserved throughout.
```

**Commit M-5**:
```
git add tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md docs/MIGRATION_PROGRESS.md
git commit -m "docs(briefs,progress): record A'.1.M (A'.0.7 amendment) landing

A'.0.7 amendment landed in 4 atomic commits (M-1 through M-4) per
docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md (commit c2b83b4).
This commit records the landing in tracker artifacts:

- A'.0.7 brief Status line: append AMENDMENTS LANDED reference
- MIGRATION_PROGRESS A'.0.7 entry: append «Amendment landing» sub-section

A'.0.7 milestone fully closed. Methodology corpus (METHODOLOGY +
PIPELINE_METRICS + MAXIMUM_ENGINEERING_REFACTOR + README) syncs к
post-A'.0.7 deliberation lock state. Audience contract (agent-as-
primary-reader) materialized in three documents.

Test count delta: zero (docs-only).

Awaits: Part K (К-L3.1 architecture amendment landing).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### §M.7 Part M verification (Phase 5)

After all 5 commits land, run verification:

**§M.7.1 Test baseline preserved**:
```
dotnet test --logger "console;verbosity=minimal"
```
Expected: 631 passed, 0 failed, 0 skipped.

**§M.7.2 Cross-document drift audit (per A'.0.7 amendment plan §6)**:

Role label consistency check:
```
grep -rn "Sonnet\|Gemma" docs/methodology/ README.md
grep -rn "four agents\|four-agent" docs/methodology/ README.md
grep -rn "syntax-tier\|architect-tier\|prompt-generation tier" docs/methodology/ README.md
```

Expected hits: only in v1.x era historical references (METHODOLOGY §0 footnote v1.x era paragraph + §3.2 v1.x era economics + §4.4 Case A + §5.2 v1.x equivalent defense + §5.3 v1.x equivalent claims; MAXIMUM_ENGINEERING_REFACTOR §10 v1.0 history entry; PIPELINE_METRICS frame note + per-metric annotations + §6 forward measurement plan).

**§M.7.3 Cross-ref path verification**:
- Manually click-test key cross-refs in METHODOLOGY (§11 See also), PIPELINE_METRICS (frame note к METHODOLOGY anchors), README (Pipeline section к METHODOLOGY и PIPELINE_METRICS). Verify all anchors resolve.

**§M.7.4 Deferral markers removed**:
```
grep -rn "A'.0.7 DEFERRAL MARKER" docs/methodology/ README.md
```
Expected: zero hits.

**§M.7.5 Commit log**:
```
git log --oneline -7
```

Expected 5 new commits atop Step 0 commit (`<step-0-SHA>`):
- `<SHA-M5>` docs(briefs,progress): record A'.1.M (A'.0.7 amendment) landing
- `<SHA-M4>` docs: A'.0.7 amendment — README Pipeline section v1.6 reality + agent-as-primary-reader audience contract
- `<SHA-M3>` docs(methodology): A'.0.7 amendment — MAXIMUM_ENGINEERING_REFACTOR v1.0 → v1.1 pipeline-mapping update
- `<SHA-M2>` docs(methodology): A'.0.7 amendment — PIPELINE_METRICS v0.1 → v0.2 per-metric annotations + forward measurement plan
- `<SHA-M1>` docs(methodology): A'.0.7 amendment — METHODOLOGY v1.5 → v1.6 pipeline restructure rewrite
- `<SHA-Step-0>` docs(briefs): author A'.1 combined amendment execution brief
- `86b721a` docs(briefs,progress): mark A'.0.7 brief EXECUTED + record A'.0.7 closure

### §M.8 Part M stop conditions

The session halts и escalates if:

- **HG-X hard gate fails** (per §2.1): re-verified before Part M proceeds
- **A'.0.7 amendment plan structural surprise**: line count differs significantly from ~1460; missing §0 / §9 anchors
- **Source file divergence from expected layout**: target file (METHODOLOGY / PIPELINE_METRICS / MAXIMUM_ENGINEERING_REFACTOR / README) state pre-amendment differs significantly from amendment plan «old text» specifications — STOP if mismatch is structural (not minor whitespace); record и continue if mismatch is incidental
- **Post-commit grep verification failure**: any §M.X.X grep returns unexpected count (zero where hit expected, hits where zero expected)
- **Test baseline drift**: post-amendment `dotnet test` returns non-631 count (impossible for docs-only; if found, investigate before proceeding к Part K)
- **Out-of-scope edit**: any edit к `src/`, `tests/`, files outside amendment plan §1-§4 scope (per K-Lessons «out-of-brief execution impossible»)

On stop: surface к Crystalka; do not improvise.

---

# PART K — К-L3.1 architecture amendment landing

**Source of truth**: `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (commit `45d831c`)

**Execution model**: agent reads К-L3.1 amendment plan section by section, applies old/new text pairs к target files. Plan is the contract; brief specifies commit shape + verification only.

**Commit count**: 10 (4 architecture corpus + 4 skeleton brief surgical + 2 closure).

Per К-L3.1 amendment plan §6.2, Part K splits into 8 atomic commits (A1-A8) per the amendment plan's commit shape recommendation. Plus 2 closure commits (К-L3.1 brief Status update + MIGRATION_PROGRESS K-L3.1 closure entry).

### §K.1 Pre-Part-K verification

After Part M closure (commit M-5), pre-Part-K verification:

**Re-run HG-1, HG-2, HG-3** (HEAD now at M-5 commit; descendant of `86b721a`).

**Re-run HG-4** (test baseline 631 preserved).

**HG-9** (Part-K-specific): К-L3.1 amendment plan structure intact:
```
head -10 docs/architecture/K_L3_1_AMENDMENT_PLAN.md
grep -c "^## §" docs/architecture/K_L3_1_AMENDMENT_PLAN.md
```

Expected: §0 Locks summary + §1 KERNEL + §2 MOD_OS + §3 MIGRATION_PLAN + §4 MIGRATION_PROGRESS + §5 forward-track brief dispositions + §6 atomic commit shape + §7 cross-document drift audit + §8 what plan doesn't do + §9 document end = at least 10 §-level sections.

**HG-10** (Part-K-specific): К-L3.1 brief Status verifies EXECUTED:
```
grep -n "Status.*EXECUTED 2026-05-10" tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md
```
Expected: hit on line ~8.

### §K.2 Phase 1 — KERNEL_ARCHITECTURE.md amendment (Commit K-A1)

**Operation**: apply all edits per К-L3.1 amendment plan §1.1 through §1.X к `docs/architecture/KERNEL_ARCHITECTURE.md`.

**Per К-L3.1 amendment plan §1**:
- §1.1 Header version line v1.3 → v1.5 (header drift correction: K8.2 v2 closure intended v1.4 per commit `7527d00` message but header was not bumped; K-L3.1 lands at v1.5 capturing both v1.4 implicit + v1.5 explicit substantive changes)
- §1.2 Status line update с К-L3.1 bridge formalization reference
- §1.3 Part 0 K-L3 row reformulation (Path α default + Path β escape hatch per К-L3.1 Q1=a lock)
- §1.4 K-L8 implication update (storage abstraction now has two concrete paths)
- §1.5 Part 2 K8.2 row text amendment («без exception» framing → «selective per-component» framing per К-L3.1 closure reformulation)
- §1.6 Part 7 (if exists, error semantics): unchanged or extended for managed-path errors

**Commit K-A1**:
```
git add docs/architecture/KERNEL_ARCHITECTURE.md
git commit -m "docs(kernel): K-L3.1 bridge formalization; bump KERNEL v1.3 → v1.5

Per К-L3.1 amendment plan §1 (commit 45d831c). К-L3.1 session locks
(Q1=a attribute, Q2=β-i mod-side + IModApi RegisterManagedComponent,
Q3=i explicit dual API, Q4=b runtime-only managed-path, Q5=a passive
metrics, Q6=a path-blind capability, Synthesis=§4.A amend K-L3).

Changes:
- Header v1.3 → v1.5 (К8.2v2 v1.4 header bump deferred; K-L3.1
  lands at v1.5 capturing v1.4 + v1.5 substantive changes)
- Status line: К-L3.1 bridge formalization reference added
- Part 0 K-L3 row: Path α default (unmanaged struct, NativeWorld) +
  Path β escape hatch (managed class via [ManagedStorage], mod-side
  ManagedStore<T>) as first-class peers
- Part 0 K-L8 implication: storage abstraction has two concrete paths
- Part 2 K8.2 row: «без exception» framing reformulated к «selective
  per-component application»

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -n "Version.*1.5" docs/architecture/KERNEL_ARCHITECTURE.md` returns hit
- `grep -c "без exception" docs/architecture/KERNEL_ARCHITECTURE.md` returns 0 in active spec wording (acceptable: version history quote-context only)
- `grep -n "Path α\|Path β" docs/architecture/KERNEL_ARCHITECTURE.md` returns hits
- `grep -n "ManagedStorage" docs/architecture/KERNEL_ARCHITECTURE.md` returns hit
- `grep -n "K-L3.1" docs/architecture/KERNEL_ARCHITECTURE.md` returns hit

### §K.3 Phase 2 — MOD_OS_ARCHITECTURE.md amendment (Commit K-A2)

**Operation**: apply all edits per К-L3.1 amendment plan §2 к `docs/architecture/MOD_OS_ARCHITECTURE.md`.

**Per К-L3.1 amendment plan §2**:
- Header version bump v1.6 → v1.7
- §«Modding с native ECS kernel» (lines 1149–1150): rewritten к Path α default + Path β bridge mechanism per synthesis §4.A
- §3 capability model: Q6=a path-blind confirmation
- §4.6 IModApi v3 surface: `RegisterManagedComponent<T> where T : class, IComponent` added (per Q2=β-i)
- §11 migration phases: M3.5 deferred entry references
- §12 LOCKED decisions: D-1 path-orthogonal confirmation
- Lines 1149–1150 specifically rewritten verbatim per amendment plan §2

**Commit K-A2**:
```
git add docs/architecture/MOD_OS_ARCHITECTURE.md
git commit -m "docs(modos): K-L3.1 bridge formalization; bump MOD_OS v1.6 → v1.7

Per К-L3.1 amendment plan §2 (commit 45d831c). К-L3.1 session locks
applied:

- Header v1.6 → v1.7
- Lines 1149-1150 rewrite: «Mod component types must be unmanaged
  structs (Path α)» → «Mod component types default к Path α
  (unmanaged struct); Path β (managed class via [ManagedStorage])
  available per К-L3.1 bridge mechanism»
- §3 capability model: Q6=a path-blind confirmation
- §4.6 IModApi v3 surface: RegisterManagedComponent<T> added
- §11 migration phases: bridge integration references
- §12 D-1 LOCKED: capability targeting confirmed (Class | Struct)

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -n "Version.*1.7" docs/architecture/MOD_OS_ARCHITECTURE.md` returns hit
- `grep -c "Class-based component storage prohibited" docs/architecture/MOD_OS_ARCHITECTURE.md` returns 0 in active spec
- `grep -n "RegisterManagedComponent" docs/architecture/MOD_OS_ARCHITECTURE.md` returns hit
- `grep -n "Path β" docs/architecture/MOD_OS_ARCHITECTURE.md` returns hit
- `grep -n "К-L3.1\|K-L3.1" docs/architecture/MOD_OS_ARCHITECTURE.md` returns hits

### §K.4 Phase 3 — MIGRATION_PLAN_KERNEL_TO_VANILLA.md amendment (Commit K-A3)

**Operation**: apply all edits per К-L3.1 amendment plan §3 к `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`, **including §3.8 Phase A' integration**.

**Per К-L3.1 amendment plan §3**:
- Header version bump v1.0 → v1.1
- §0.1 sequence diagram replacement (single arrow → Phase A' bridge integration; full §3.8 diagram landed)
- §0.3 LOCKED architectural decisions: decision #9 added (K-L3.1 selective per-component performance-driven, bridge per Q1=a)
- Line 62 framing correction («K-L3 violation period» → post-К-L3.1 wording)
- Lines 130, 522–523 framing corrections
- §1.2 K8.3 scope wording: dual-path access per Q3=i
- §1.3 K8.4 scope wording: Mod API v3 surface per Q2=β-i + Q1=a
- §1.4 K8.5 scope wording: capability annotation per Q6=a
- §6 document maintenance: К-L3.1 amendment milestone added

**Commit K-A3**:
```
git add docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md
git commit -m "docs(migration): K-L3.1 bridge formalization + Phase A' integration; bump MIGRATION_PLAN v1.0 → v1.1

Per К-L3.1 amendment plan §3 (commit 45d831c). К-L3.1 amendment +
Phase A' structural integration landed:

- Header v1.0 → v1.1
- §0.1 sequence diagram: single arrow Phase A → B replaced с
  Phase A' bridge (A'.0 К-L3.1 → A'.0.5 reorg → A'.0.7 methodology
  → A'.1 amendment → A'.3 push → A'.4-A'.7 К-series → A'.8 К-closure
  → A'.9 analyzer → Phase B M8.4)
- §0.3 LOCKED decision #9 added: К-L3.1 bridge mechanism
- Line 62 / 130 / 522-523: «K-L3 violation period» framing corrected
- §1.2 K8.3 scope: dual-path access pattern per Q3=i
- §1.3 K8.4 scope: Mod API v3 expanded per Q1=a + Q2=β-i
- §1.4 K8.5 scope: capability annotation per Q6=a
- §6 document maintenance: К-L3.1 amendment milestone in schedule

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -n "v1.1.*К-L3.1\|v1.1.*K-L3.1" docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` returns hit (in version line)
- `grep -c "K-L3 violation period\|K-L3 violation" docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` returns 0 in active spec
- `grep -n "Phase A'" docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` returns hits (§0.1 diagram + §6 maintenance)
- `grep -n "decision #9\|#9" docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` returns hit

### §K.5 Phase 4 — MIGRATION_PROGRESS.md K-L3.1 amendment (Commit K-A4)

**Operation**: apply all edits per К-L3.1 amendment plan §4 к `docs/MIGRATION_PROGRESS.md`.

**Per К-L3.1 amendment plan §4**:
- §4.1 Last updated line refreshed
- §4.2 Current state snapshot — line 35 «Last completed milestone» updated с К-L3.1 reference
- §4.3 Current state snapshot — Active phase line updated
- §4.4 K8.2 v2 entry header line 407 reframing
- §4.5 K8.2 v2 KERNEL doc bump entry line 443
- §4.6 K8.2 v2 architectural decisions line 457 reformulation («без exception» → «selective per-component»)
- §4.7 New K-L3.1 closure entry inserted after K8.2 v2 entry

**Note**: К-L3.1 amendment plan §4 specifies edits assuming pre-A'.0.7 baseline. Since A'.0.7 closure has already modified several of these lines (line 5, 34, 35 per A'.0.7 closure commit `86b721a`), Part K must apply amendment plan §4 edits **against current post-A'.0.7 state**, not against pre-A'.0.7 baseline.

**Per К-Lessons «Phase 0.4 inventory as hypothesis, not authority»**: agent reads actual current state at execution time, reconciles К-L3.1 amendment plan §4 specifications against actual lines, applies semantic intent of each edit even if line numbers/old-text differ.

**Specific reconciliation guidance**:
- Line 5: A'.0.7 closure refreshed «Last updated» к 2026-05-10 А'.0.7 closure reference. К-L3.1 amendment plan §4.1 specifies «Last updated → К-L3.1 closure 2026-05-10». **Reconciliation**: refresh «Last updated» к latest closure (К-L3.1 amendment landing = current commit). New text: `**Last updated**: 2026-05-10 (К-L3.1 bridge formalization amendment landed via A'.1.K; 8 atomic commits per К-L3.1 amendment plan §6.2)`.
- Line 34 «Active phase»: A'.0.7 closure already updated к «A'.1 amendment brief execution → A'.3 push → downstream». К-L3.1 amendment lands part of A'.1. **Reconciliation**: update Active phase к reflect post-A'.1.K state: «A'.3 push к origin → A'.4 K9 / A'.5 K8.3 / ... / A'.9 analyzer (К-L3.1 + A'.0.5 + A'.0.7 + A'.1.M + A'.1.K closed as Phase A' foundation)».
- Line 35 «Last completed milestone»: A'.0.7 closure listed «A'.0.7 + A'.0.5 + К-L3.1 previous». К-L3.1 amendment plan §4.2 wants «К-L3.1 → 2026-05-10 + K8.2v2 previous». **Reconciliation**: «Last completed milestone: A'.1.K (К-L3.1 amendment landing) → 2026-05-10. Previous: A'.1.M (A'.0.7 amendment landing) → 2026-05-10; A'.0.7 (methodology pipeline restructure deliberation) → 2026-05-10; ...».
- Lines 407 / 443 / 457: K8.2 v2 entry content reframing per К-L3.1 amendment plan §4.4-§4.6. These lines have not been touched by A'.0.7 closure; К-L3.1 amendment plan §4 specifications apply directly.
- К-L3.1 closure entry insertion (per §4.7): insert after K8.2 v2 entry (line ~467 в pre-A'.0.7 state; verify actual line at execution time). Full entry text per К-L3.1 amendment plan §4.7.

**Commit K-A4**:
```
git add docs/MIGRATION_PROGRESS.md
git commit -m "docs(progress): record K-L3.1 closure; reframe K8.2 v2 «без exception»; reconcile с post-A'.0.7 state

Per К-L3.1 amendment plan §4 (commit 45d831c), reconciled с post-
A'.0.7 closure state (commit 86b721a; A'.1.M landing in progress
or recent).

Edits:
- Line 5 Last updated: refresh к К-L3.1 amendment landing
- Line 34 Active phase: post-A'.1.K state references
- Line 35 Last completed milestone: A'.1.K → A'.1.M → A'.0.7 → A'.0.5 → К-L3.1 → ... cascade
- Lines 407/443/457: K8.2v2 entry «без exception» framing reformulated
  к «selective per-component application»
- New К-L3.1 closure entry inserted after K8.2v2 entry per amendment
  plan §4.7

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Post-commit verification**:
- `grep -c "без exception" docs/MIGRATION_PROGRESS.md` returns 0 in active narrative (acceptable: only в historical quotes if any)
- `grep -n "### К-L3.1 — Bridge formalization\|### K-L3.1 — Bridge formalization" docs/MIGRATION_PROGRESS.md` returns hit (new closure entry)
- `grep -n "selective per-component" docs/MIGRATION_PROGRESS.md` returns hit
- `grep -n "A'.1.K\|A'.1.M" docs/MIGRATION_PROGRESS.md` returns hits (Active phase + Last completed milestone lines)

### §K.6 Phase 5 — K9 skeleton brief surgical edit (Commit K-A5)

**Operation**: apply edits per К-L3.1 amendment plan §5.1 (Disposition B — surgical) к `tools/briefs/K9_FIELD_STORAGE_BRIEF.md`.

**Per К-L3.1 amendment plan §5.1**:
- Phase 0.4 expected versions block: KERNEL_ARCHITECTURE v1.0 → v1.5+; MOD_OS_ARCHITECTURE v1.6 → v1.7+
- Phase 0.7 expected baseline: 538 tests → 631+ tests
- Optional: architectural reference clarification paragraph (K9 RawTileField orthogonality к entity-component bridge)

**Commit K-A5**:
```
git add tools/briefs/K9_FIELD_STORAGE_BRIEF.md
git commit -m "docs(briefs): K9 surgical edits per K-L3.1 disposition B

Per К-L3.1 amendment plan §5.1 (commit 45d831c). K9 is full-authored
brief (~1200 lines, awaits execution per β6 sequencing); К-L3.1 lock
applies surgically — version refs + baseline only. Scope unchanged
(K9 RawTileField architecturally orthogonal к entity-component
bridge per addendum §A2.5).

Edits:
- Phase 0.4 expected versions: KERNEL v1.0 → v1.5+; MOD_OS v1.6 → v1.7+
- Phase 0.7 expected baseline: 538 → 631+ (post-K8.2v2)

Optional architectural reference paragraph added on bridge orthogonality.

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### §K.7 Phase 6 — K8.3 skeleton brief surgical edit (Commit K-A6)

**Operation**: apply edits per К-L3.1 amendment plan §5.2 (Disposition B-to-C — surgical scope correction) к `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md`.

**Per К-L3.1 amendment plan §5.2**:
- Header «Systems в scope» line: «12 vanilla production systems» → «34 production systems» per migration plan v1.1 §1.2 reformulated scope
- Skeleton TODO list: insert new TODO bullet on per-system Path α/β access audit per Q3=i

**Commit K-A6**:
```
git add tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md
git commit -m "docs(briefs): K8.3 surgical edits per K-L3.1 disposition B-to-C

Per К-L3.1 amendment plan §5.2 (commit 45d831c). K8.3 skeleton
receives surgical scope correction + dual-path note:

- Systems in scope: 12 → 34 (per migration plan v1.1 §1.2)
- Skeleton TODO: per-system Path α/β access audit added

Full brief authoring is post-К-L3.1 amendment closure (and post-K8.4
since dual-API plumbing requires K8.4).

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### §K.8 Phase 7 — K8.4 skeleton brief surgical edit (Commit K-A7)

**Operation**: apply edits per К-L3.1 amendment plan §5.3 (Disposition C.1 — skeleton receives surgical edits) к `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md`.

**Per К-L3.1 amendment plan §5.3**:
- Header goal paragraph: kernel-side World retiring + Mod API v3 dual-registration paths added
- Deliverables list: 3 new bullets — RegisterManagedComponent<T> on IModApi v3, SystemBase.ManagedStore<T>() accessor, MOD_OS v1.7+ verification
- TODO list: 2 new TODOs — per-mod ManagedStore<T> implementation, MissingManagedStorageAttribute error kind

**Commit K-A7**:
```
git add tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md
git commit -m "docs(briefs): K8.4 surgical edits per K-L3.1 disposition C.1

Per К-L3.1 amendment plan §5.3 (commit 45d831c). K8.4 skeleton
receives surgical edits для bridge implementation scope expansion:

- Header goal: kernel-side World retiring + Mod API v3 dual paths
- Deliverables: RegisterManagedComponent<T> + SystemBase.ManagedStore<T>()
  + MOD_OS v1.7+ reference verification (3 new bullets)
- TODOs: per-mod ManagedStore<T> implementation + MissingManagedStorage
  Attribute error kind (2 new TODOs)

Full brief authoring deferred к K8.4 milestone activation; this is
skeleton surgical edit only.

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### §K.9 Phase 8 — K8.5 skeleton brief surgical edit (Commit K-A8)

**Operation**: apply edits per К-L3.1 amendment plan §5.4 (Disposition B — surgical) к `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md`.

**Per К-L3.1 amendment plan §5.4**:
- Deliverables list: 3 new bullets для bridge documentation expansion (per-component path choice, dual-API access patterns, v2→v3 migration guide)
- TODO list: 1 new TODO — bridge documentation completeness audit

**Commit K-A8**:
```
git add tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md
git commit -m "docs(briefs): K8.5 surgical edits per K-L3.1 disposition B

Per К-L3.1 amendment plan §5.4 (commit 45d831c). K8.5 skeleton
receives surgical content expansion для bridge documentation:

- Deliverables: 3 new bullets — per-component path choice criterion,
  dual-API access patterns, v2→v3 migration guide
- TODO: bridge documentation completeness audit

Full brief authoring deferred к K8.5 milestone activation.

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### §K.10 Phase 9 — К-L3.1 brief Status update (Commit K-A9)

**Operation**: append amendment landing reference к К-L3.1 brief Status line.

**File**: `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`

**Old text trailing** (на line 8):
`Amendment plan at `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` is the executable artifact для follow-up amendment brief.`

**New text trailing**:
`Amendment plan at `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` is executable artifact; AMENDMENTS LANDED 2026-05-10 via A'.1.K execution session (commits K-A1 through K-A8 covering KERNEL v1.5 + MOD_OS v1.7 + MIGRATION_PLAN v1.1 + MIGRATION_PROGRESS sync + 4 skeleton brief surgical edits).`

**Commit K-A9**:
```
git add tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md
git commit -m "docs(briefs): mark K-L3.1 brief amendment landing

К-L3.1 amendment landed in 8 atomic commits (K-A1 through K-A8) per
docs/architecture/K_L3_1_AMENDMENT_PLAN.md (commit 45d831c). This
commit appends AMENDMENT LANDED reference к К-L3.1 brief Status line.

К-L3.1 milestone now fully closed: deliberation 2026-05-10 (commit
45d831c) → amendment plan authored → amendments landed 2026-05-10
via A'.1.K execution session.

Test count delta: zero (docs-only).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### §K.11 Phase 10 — A'.1.K closure commit (Commit K-A10)

**Operation**: extend MIGRATION_PROGRESS К-L3.1 closure entry с amendment landing reference.

**File**: `docs/MIGRATION_PROGRESS.md`

**Insertion location**: end of К-L3.1 closure entry (which was inserted by K-A4 per К-L3.1 amendment plan §4.7).

**New text appended к К-L3.1 entry**:

```

**Amendment landing (A'.1.K, 2026-05-10)**:

8 atomic commits landed amendments per `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` §1–§5:
- Commit K-A1: KERNEL_ARCHITECTURE.md v1.3 → v1.5 (Part 0 K-L3 row + K-L8 implication + Part 2 K8.2 row + Status line)
- Commit K-A2: MOD_OS_ARCHITECTURE.md v1.6 → v1.7 (lines 1149-1150 rewrite + §3 capability + §4.6 IModApi v3 + §11 migration phases + §12 D-1)
- Commit K-A3: MIGRATION_PLAN_KERNEL_TO_VANILLA.md v1.0 → v1.1 (§0.1 Phase A' integration + §0.3 decision #9 + line 62 + §1.2-§1.4 scope + §6 maintenance)
- Commit K-A4: MIGRATION_PROGRESS.md sync (line 5/34/35 reconciliation + K8.2v2 entry reframing + new К-L3.1 closure entry per amendment plan §4.7)
- Commits K-A5 through K-A8: 4 skeleton brief surgical edits (K9 disposition B + K8.3 disposition B-to-C + K8.4 disposition C.1 + K8.5 disposition B)
- Commit K-A9: К-L3.1 brief Status amendment landing reference
- Commit K-A10: this closure entry extension

Test count delta: zero across all 10 commits (docs-only).
Working tree clean post-A'.1.K; baseline 631 preserved throughout.

К-L3.1 + A'.0.7 amendments both LANDED. Phase A' deliberation foundation (A'.0 / A'.0.5 / A'.0.7) + amendment landing (A'.1.M / A'.1.K) DONE. Awaits A'.3 push → A'.4-A'.7 К-series execution.
```

**Commit K-A10**:
```
git add docs/MIGRATION_PROGRESS.md
git commit -m "docs(progress): record A'.1.K (К-L3.1 amendment) landing

К-L3.1 amendment landed in 8 atomic commits (K-A1 through K-A8) plus
brief Status update (K-A9) per docs/architecture/K_L3_1_AMENDMENT_PLAN.md
(commit 45d831c). This commit records the landing в MIGRATION_PROGRESS
К-L3.1 closure entry с full per-commit enumeration.

A'.1 amendment milestone complete: A'.1.M (A'.0.7 methodology) + A'.1.K
(К-L3.1 architecture) both landed 2026-05-10. Total 15 amendment
commits across two execution sessions (5 + 10), plus 1 brief authoring
prerequisite (Step 0) = 16 commits this A'.1 milestone.

Phase A' state post-A'.1:
- A'.0 К-L3.1 deliberation: DONE 45d831c
- A'.0.5 reorg: DONE 4e332bb
- A'.0.7 methodology rewrite: DONE 86b721a
- A'.1 amendment landing: DONE (this commit)
- A'.3 push к origin: NEXT
- A'.4-A'.7 К-series execution: pending push
- A'.8 К-closure report: pending К-series
- A'.9 architectural analyzer: pending K-closure

Test count delta: zero (docs-only).

Awaits: A'.3 push к origin (~22 commits ahead origin).

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### §K.12 Part K verification (Phase 5)

After all 10 commits land, run verification:

**§K.12.1 Test baseline preserved**:
```
dotnet test --logger "console;verbosity=minimal"
```
Expected: 631 passed, 0 failed, 0 skipped.

**§K.12.2 Cross-document drift audit per К-L3.1 amendment plan §7**:

```
grep -rn "без exception" docs/architecture/KERNEL_ARCHITECTURE.md docs/architecture/MOD_OS_ARCHITECTURE.md docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "no exception" docs/architecture/KERNEL_ARCHITECTURE.md docs/architecture/MOD_OS_ARCHITECTURE.md docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "K-L3 violation" docs/architecture/KERNEL_ARCHITECTURE.md docs/architecture/MOD_OS_ARCHITECTURE.md docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "must be unmanaged struct" docs/architecture/KERNEL_ARCHITECTURE.md docs/architecture/MOD_OS_ARCHITECTURE.md docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md docs/MIGRATION_PROGRESS.md
grep -rn "Class-based component storage prohibited" docs/architecture/MOD_OS_ARCHITECTURE.md
```

**Acceptable matches**: only in version-history entries that quote prior wording для traceability (e.g. v1.1 entry recording v1.0 framing being corrected). All matches must be in version-history quote-context, never in active spec wording.

**§K.12.3 К-L3.1 lock terminology presence**:
```
grep -n "Path α\|Path β" docs/architecture/KERNEL_ARCHITECTURE.md docs/architecture/MOD_OS_ARCHITECTURE.md docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md
grep -n "ManagedStorage\|ManagedStore<T>\|RegisterManagedComponent" docs/architecture/MOD_OS_ARCHITECTURE.md docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md
grep -n "selective per-component" docs/architecture/KERNEL_ARCHITECTURE.md docs/MIGRATION_PROGRESS.md
```

Expected: hits in each (К-L3.1 lock vocabulary materialized).

**§K.12.4 Phase A' integration в Migration Plan**:
```
grep -n "Phase A'" docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md
```
Expected: hits в §0.1 sequence diagram + §6 maintenance.

**§K.12.5 Commit log**:
```
git log --oneline -12
```

Expected 10 new commits atop M-5 closure commit:
- `<K-A10>` docs(progress): record A'.1.K (К-L3.1 amendment) landing
- `<K-A9>` docs(briefs): mark К-L3.1 brief amendment landing
- `<K-A8>` docs(briefs): K8.5 surgical edits per К-L3.1 disposition B
- `<K-A7>` docs(briefs): K8.4 surgical edits per К-L3.1 disposition C.1
- `<K-A6>` docs(briefs): K8.3 surgical edits per К-L3.1 disposition B-to-C
- `<K-A5>` docs(briefs): K9 surgical edits per К-L3.1 disposition B
- `<K-A4>` docs(progress): record К-L3.1 closure; reframe К8.2v2; reconcile с post-A'.0.7
- `<K-A3>` docs(migration): К-L3.1 bridge formalization + Phase A' integration; bump MIGRATION_PLAN v1.0 → v1.1
- `<K-A2>` docs(modos): К-L3.1 bridge formalization; bump MOD_OS v1.6 → v1.7
- `<K-A1>` docs(kernel): К-L3.1 bridge formalization; bump KERNEL v1.3 → v1.5
- `<M-5>` docs(briefs,progress): record A'.1.M (A'.0.7 amendment) landing
- ...

### §K.13 Part K stop conditions

The session halts и escalates if:

- **HG-X hard gate fails** (per §2.1 + §K.1 specific HG-9 / HG-10): re-verified before Part K proceeds
- **К-L3.1 amendment plan structural surprise**: missing §0 / §9 anchors; locks summary table not matching session record
- **Source file divergence from expected layout**: target file state pre-amendment differs significantly from amendment plan «old text» specifications — STOP if mismatch is structural; record и continue if mismatch is incidental
- **Line 5/34/35 reconciliation conflict** (per §K.5): if post-A'.0.7 state of MIGRATION_PROGRESS lines 5/34/35 cannot be reconciled с К-L3.1 amendment plan §4.1-§4.3 specifications, STOP, escalate. (Reconciliation guidance provided в §K.5 covers expected case; structural conflict triggers escalation.)
- **Post-commit grep verification failure**: any §K.X.X grep returns unexpected count
- **Cross-document drift audit hits**: §K.12.2 returns hits in active spec wording (not historical quote-context) — STOP, surgical scrub before proceeding
- **Test baseline drift**: post-amendment `dotnet test` returns non-631 count
- **Out-of-scope edit**: any edit к source files outside К-L3.1 amendment plan §1-§5 scope

On stop: surface к Crystalka; do not improvise.

---

## §4 Combined Part M + Part K summary

### §4.1 Total commit count

| Step | Commits | Subject prefix |
|---|---|---|
| Step 0 (prerequisite) | 1 | `docs(briefs): author A'.1 combined amendment execution brief` |
| Part M (A'.0.7 amendment) | 5 | `docs(methodology):` × 3, `docs:` × 1, `docs(briefs,progress):` × 1 |
| Part K (К-L3.1 amendment) | 10 | `docs(kernel):`, `docs(modos):`, `docs(migration):`, `docs(progress):` × 2, `docs(briefs):` × 5 |
| **Total** | **16** | All docs-only |

**Test count delta per commit**: zero (docs-only).
**Test count delta cumulative**: zero (baseline 631 preserved throughout).
**Each commit leaves repo в compilable + tests-passing state**: yes (docs-only).

**Branch state at end of A'.1**: `main`, 16 commits ahead of pre-Step-0 HEAD (`86b721a`).

**Push deferred к A'.3** per Phase A' default sequencing.

### §4.2 Estimated cumulative time

- Step 0: ~5 min (single commit с this brief authoring)
- Part M execution session: 30-60 min auto-mode (5 commits, mostly small text replacements; METHODOLOGY commit M-1 is largest)
- Part M verification: 5-10 min (test baseline + grep checks + cross-ref spot-check)
- Part K execution session: 60-90 min auto-mode (10 commits, multi-file complex amendments including 4 skeleton briefs + line-reconciliation в MIGRATION_PROGRESS)
- Part K verification: 5-10 min (test baseline + drift audit greps + commit log review)
- **Total A'.1**: 1.75-3 hours across two execution sessions

---

## §5 What this brief deliberately does not do

- **No source code changes**. All amendments cover documentation only.
- **No test additions**. Baseline 631 preserved per commit.
- **No К-L3.1 deliberation re-opening**. All К-L3.1 locks (Q1=a, Q2=β-i, Q3=i, Q4=b, Q5=a, Q6=a, Synthesis=§4.A) are LOCKED via К-L3.1 session 2026-05-10 (commit `45d831c`); this brief executes amendments only.
- **No A'.0.7 deliberation re-opening**. All A'.0.7 locks (Q-A07-1..8 + Q1..Q12 + synthesis form) are LOCKED via A'.0.7 session 2026-05-10 (commit `86b721a`); this brief executes amendments only.
- **No К-closure report drafting**. A'.8 milestone scope.
- **No architectural analyzer work**. A'.9 milestone scope.
- **No push к origin**. Deferred к Phase A'.3.
- **No К9 / K8.3 / K8.4 / K8.5 full brief authoring**. Surgical edits per К-L3.1 amendment plan §5 only; full brief authoring deferred к each milestone activation time.
- **No PIPELINE_METRICS v1.6 data refresh** (§6 forward measurement plan items). v1.6 measurement collection is forward-looking; tracked в PIPELINE_METRICS §6 как backlog.
- **No methodology version-2.0 commitment**. v1.5 → v1.6 (Part M); v1.6 → v1.7 (Part K) — semantic minors only.
- **No К-L3.1-derived methodology lesson formalization**. Per Q-A07-7=b lock, defers к A'.8 К-closure report scope.

---

## §6 Out of scope (explicit)

- **PIPELINE_METRICS §1.3 hardware reformulation / §5.1 minimum hardware / §5.2 software dependencies / §5.3 subscription tier reformulations**: per Q10.a annotations, these are `[transfers с reframing]` items с reformulation tasks tracked в PIPELINE_METRICS §6 backlog. Reformulation forward-looking; not A'.1 scope.
- **К-L3.1 amendment plan §6 «atomic commit shape» enumeration recommends 8 commits** (К-L3.1 session output + 8 Part K amendment commits = «Combined commit count» в plan §6.3). This brief Part K materializes 10 commits (8 amendment + 2 closure) because К-L3.1 amendment plan §6.1 session output bundle was already landed by К-L3.1 closure commit `45d831c`. Brief Part K covers ONLY К-L3.1 amendment plan §6.2 Phase A'.1 commits (8) + 2 closure commits.
- **Cross-document drift audit Phase 5** of К-L3.1 amendment plan §7: covered в §K.12.2 above with same grep targets per К-L3.1 amendment plan §7.
- **Phase A' sequencing document amendment**: PHASE_A_PRIME_SEQUENCING.md is companion reference; not directly amended by this milestone. Migration Plan v1.1 integrates Phase A' upstream per K-L3.1 amendment plan §3.8.
- **MOD_OS_ARCHITECTURE further amendments triggered by K8.4 closure**: К-L3.1 amendment plan §5.3 K8.4 disposition C.1 mentions MOD_OS v1.7+ verification at K8.4 brief authoring time. This is K8.4 milestone scope, not A'.1.

---

## §7 Pre-session checklist (Crystalka readiness)

Before invoking A'.1 execution sessions, Crystalka confirms:

- [ ] A'.0.7 closure verified (`git log --oneline -1` shows `86b721a` or descendant on main)
- [ ] К-L3.1 closure verified (`git log --grep="docs(architecture): K-L3.1"` returns hit on `45d831c`)
- [ ] Working tree clean (`git status` shows nothing к commit)
- [ ] No active feature branch in mid-execution state
- [ ] Test baseline holds (`dotnet test --logger "console;verbosity=minimal"` shows 631 passing)
- [ ] Both amendment plans present:
   - `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` exists, ~1460 lines
   - `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` exists
- [ ] К-L3.1 brief Status = EXECUTED (`grep "Status.*EXECUTED" tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`)
- [ ] A'.0.7 brief Status = EXECUTED (`grep "Status.*EXECUTED" tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md`)
- [ ] Both deliberation brief precedent commits present in git log (`45d831c` К-L3.1, `55d9e36` A'.0.7, `86b721a` A'.0.7 closure)

If any check fails: A'.1 session pre-flight halts; failing condition resolved (или explicit rationale recorded) before execution begins.

---

## §8 Document provenance

- **Authored**: 2026-05-10, Claude Opus 4.7, A'.0.7 closure session continuation post-`86b721a`
- **Authoring approach**: single-combined execution brief per A'.0.7 amendment plan §7.2 standalone path recommendation (Crystalka 2026-05-10 acceptance)
- **Authority**: A'.0.7 session locks (Q-A07-1..8 + Q1..Q12 + synthesis form, recorded в `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` + `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md`) + К-L3.1 session locks (Q1=a, Q2=β-i, Q3=i, Q4=b, Q5=a, Q6=a, Synthesis=§4.A, recorded в `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` + `docs/architecture/K_L3_1_AMENDMENT_PLAN.md`)
- **Precedent**: A'.0.7 closure execution brief pattern (single Opus session, Phase 0 pre-flight + Step 0 brief authoring + Phase N execution + Phase 5 verification, ~3 atomic commits, 45-90 min); extended к two-Part execution с different scopes
- **Execution target**: 2 Cloud Code sessions (execution mode); 16 atomic commits total (Step 0 + 5 Part M + 10 Part K); ~1.75-3 hours cumulative
- **Companion documents**:
   - `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` (EXECUTED via `86b721a`)
   - `tools/briefs/A_PRIME_0_7_CLOSURE_EXECUTION_BRIEF.md` (EXECUTED via `86b721a`)
   - `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` (Part M source of truth; commit `c2b83b4`)
   - `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (EXECUTED via `45d831c`)
   - `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (APPLIED via `45d831c`)
   - `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (Part K source of truth)
   - `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (Phase A' anchor reference)

---

**Brief end. Phase 0 pre-flight + Step 0 brief authoring runs first; then Part M and Part K execute в sequence через two Cloud Code sessions. Output: 16 commits on `main` (Step 0 + 5 + 10), test baseline 631 preserved, push deferred к A'.3.**

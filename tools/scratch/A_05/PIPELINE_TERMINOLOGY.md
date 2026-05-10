# A'.0.5 Phase 1 — pipeline-terminology scan with Phase 7 disposition

Per brief §3.4 and §9. Phase 7 is **mechanical scrub only**; substantive rewrite (METHODOLOGY architectural sections, PIPELINE_METRICS empirics) is A'.0.7 scope.

Disposition codes:
- **MECH** — mechanical scrub in Phase 7 (terminology replacement preserves surrounding meaning)
- **A'.0.7** — substantive rewrite needed; flag forward
- **HIST** — historical reference appropriate to retain (e.g. brief authored against 4-agent assumption)
- **SKIP** — the brief itself; no scrub

---

## METHODOLOGY-class files (substantive A'.0.7)

### docs/METHODOLOGY.md (Gemma:2, LM Studio:3, Cline:2, four-agent:5, local executor:5, prompt generator:5)
**Disposition**: ALL substantive; **A'.0.7**.

The whole document is structured around 4-agent pipeline: §0 abstract, §2.1 Role distribution, §2.2 Contracts as IPC, §3 Pipeline economics, §4 Empirical results, §5.2 Pipeline structural defense, §8.1 Minimal configuration, §8.2 Confirmed configuration. Removing 4-agent framing without rewriting these sections produces incoherent prose.

Phase 7 commit shape (per brief §9.4): add `<!-- TODO: A'.0.7 substantive rewrite — pipeline restructure to 2-agent unified Claude Desktop -->` HTML comment marker at top of file. Do **not** edit substance.

### docs/PIPELINE_METRICS.md (Gemma:7, LM Studio:4, Cline:6, four-agent:1)
**Disposition**: ALL substantive; **A'.0.7**.

Empirical metrics gathered with Gemma in pipeline. Scrubbing terms without re-collecting metrics or re-framing as historical produces false claims about new pipeline. A'.0.7 deliberates whether to discard, reframe as historical, or recollect.

Phase 7 commit shape: add A'.0.7 deferral marker at top of file. Do not edit substance.

### docs/MAXIMUM_ENGINEERING_REFACTOR.md (Gemma:3)
**Disposition**: read in Phase 7; if mechanical, MECH; if substantive, A'.0.7. Probable MECH (refactor doctrine likely incidental Gemma mentions, not architectural elaboration).

### docs/architecture/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md (Gemma:2)
**Disposition**: read in Phase 7; classify per content. Probable MECH.

---

## Auto-fix candidates (Phase 7 MECH)

### docs/IDEAS_RESERVOIR.md (Gemma:1)
**Disposition**: MECH. Live-tracker doc; one-liner mention. Mechanical replacement.

### docs/audit/AUDIT_CAMPAIGN_PLAN.md (Gemma:1)
**Disposition**: MECH. Audit plan; incidental.

### docs/TRANSLATION_GLOSSARY.md (Gemma:2, LM Studio:1, Cline:1, four-agent:1)
**Disposition**: HIST or MECH per per-line read. Translation glossary lists English↔Russian terminology pairs; if Gemma is on the glossary as a known English noun, retention is appropriate (HIST). If incidental in prose narrative, MECH.

### docs/TRANSLATION_PLAN.md (Gemma:4)
**Disposition**: HIST or MECH per per-line read. i18n campaign plan; references may be historical.

### docs/prompts/TD1_SONNET_BRIEF.md (Gemma:14, Cline:3)
**Disposition**: HIST. This is a historical prompt brief authored against 4-agent pipeline assumption. Retain as historical record. Phase 7 NO-OP for this file.

### tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md (four-agent:1)
**Disposition**: HIST. Closure report for past milestone; retain.

### docs/audit/PASS_2_NOTES.md (four-agent:1), PASS_4_REPORT.md (four-agent:1)
**Disposition**: HIST. Past-milestone audit notes; retain.

### docs/NORMALIZATION_REPORT.md (four-agent:2)
**Disposition**: HIST or MECH per per-line read. i18n normalization report; incidental.

### docs/README.md (four-agent:2)
**Disposition**: MECH. Docs index; one-liner narrative.

### docs/ROADMAP.md (four-agent:1)
**Disposition**: MECH. Mention of pipeline architecture in roadmap framing; mechanical.

### README.md (root) (four-agent:1)
**Disposition**: MECH. Top-level README; mechanical replacement of one phrase.

### docs/architecture/RUNTIME_ARCHITECTURE.md (four-agent:1)
**Disposition**: VERIFY (re-read line in Phase 7); probable MECH.

---

## Phase 7 commit shape (estimate)

| Commit | Files | LOC delta |
|---|---|---|
| `docs(briefs): historical references retained — Gemma/Cline/4-agent appear in past artifacts as historical record (no edits)` | none — note in commit message OR skip commit entirely | 0 |
| `docs: scrub Gemma/Cline references from live narrative docs` | IDEAS_RESERVOIR, MAXIMUM_ENGINEERING_REFACTOR{,_TRACK_B}, AUDIT_CAMPAIGN_PLAN, README{,docs/}, ROADMAP, NORMALIZATION_REPORT (if applicable), RUNTIME_ARCHITECTURE (if applicable) | ~30-60 |
| `docs(methodology): mark METHODOLOGY and PIPELINE_METRICS for A'.0.7 substantive rewrite` | METHODOLOGY.md, PIPELINE_METRICS.md (top-of-file deferral markers only) | ~6 |

Estimated Phase 7 atomic commits: **2-3** (down from brief §9.4's 2-4 estimate).

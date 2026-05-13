---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-PIPELINE_METRICS
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "0.2"
next_review_due: 2027-05-10
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-PIPELINE_METRICS
---
---
title: Pipeline Metrics
nav_order: 27
---

## Document era annotation (added 2026-05-10 per A'.0.7 amendment)

This document was authored at v0.1 (2026-04-28) under the v1.x era pipeline configuration (4-agent model-tier boundary: human + local quantized Gemma executor + cloud Sonnet prompt-generator + cloud Opus architect). Empirical data in §2–§4 are v1.x era measurements.

A'.0.7 amendment (2026-05-10) restructured the pipeline к v1.6 era (2-agent session-mode boundary: Crystalka + unified Claude Desktop session switching between deliberation and execution modes). v1.x era measurements are preserved verbatim as historical record; each section/sub-section receives a per-metric annotation declaring transferability к v1.6 reality.

**Annotation labels** (used throughout §1–§5 below):

| Label | Meaning |
|---|---|
| `[v1.x era specific]` | Metric tied к 4-agent boundary type; doesn't transfer к v1.6 reality |
| `[transfers с reframing]` | Metric structurally meaningful under v1.6; needs reformulation language |
| `[transfers as-is]` | Metric measures invariant property; survives boundary-type change |
| `[uncertain — needs v1.6 measurement]` | Metric category exists в v1.6 reality but no measurement yet collected |
| `[v1.x historical record]` | Pure archaeological data; transferability not applicable |

v1.6 era data collection is forward-looking; first v1.6 data points emerge from A'.0.5 closure (`27523ac..4e332bb`, commit `4e332bb` on main 2026-05-10) and onward Phase A' milestones. See [METHODOLOGY §3.2 Current configuration economics](./METHODOLOGY.md#32-current-configuration-economics) + [METHODOLOGY §4.4 Session wall-clock performance — case studies](./METHODOLOGY.md#44-session-wall-clock-performance--case-studies) for v1.6-era references located in the methodology corpus. See [§6 Forward measurement plan](#6-forward-measurement-plan-v16-era-data-collection) below for v1.6 era data collection backlog tracked в this document.

Cross-reference: this document accompanies [METHODOLOGY.md](./METHODOLOGY.md) (v1.6 era, post-A'.0.7). METHODOLOGY documents the methodology в pipeline-agnostic form; PIPELINE_METRICS preserves per-era empirical record.

# Pipeline metrics — Dual Frontier

*Empirical configuration, throughput data, and reproducibility requirements for the LLM pipeline used in this project, gathered per-era with transferability annotations. The companion document METHODOLOGY.md describes the methodology as designed; this document records the operational data measured while running it.*

**Status:** v0.2 (2026-05-10). Per-metric annotations added per A'.0.7 amendment; v1.x era measurements preserved verbatim. Updated as further phases close. See [§«Version history»](#version-history) for change log; [§6 Forward measurement plan](#6-forward-measurement-plan-v16-era-data-collection) for v1.6 era data collection backlog.

## §1. Pipeline configuration

### §1.1 Four-agent role distribution

| Agent | Role | Where it runs | What it does |
|---|---|---|---|
| **Gemma 4 E4B** (Q4_K_M, 6.33 GB) | Executor | Locally through LM Studio + **Cline** in VS Code | Writes concrete code from a prompt: 1 prompt → 1–2 files, 131072-token context window |
| **Claude Sonnet 4.6** | Prompt generator | Claude Max 5× (desktop app) | Turns a task plus its contract into a precise prompt for the local model; handles routine tasks directly |
| **Claude Opus 4.7** | Architect and QA | Claude Max 5× (desktop app) | Hard architectural work (scheduler, dependency graph) and full reviews at phase closure. Used sparingly. |
| **Human** | Direction owner | — | Selects contracts, makes architectural decisions, frames phase goals |

Agents do not communicate directly. The repository (LOCKED documents, tests) and the human (as router between sessions) are the only coordination surfaces.

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

The 4-agent role distribution table describes the model-tier boundary configuration with N=4 agents (Gemma 4 E4B Q4_K_M executor, Sonnet 4.6 prompt generator, Opus 4.7 architect, human direction owner). Under v1.6 session-mode boundary (N=2: Crystalka + unified Claude Desktop session), this table doesn't transfer. Replaced functionally by [METHODOLOGY §2.1.1 Current configuration table](./METHODOLOGY.md#211-current-configuration-v16-2026-05-10).

### §1.2 Workflow per task

```
Human frames the task plus contract
  ↓
Sonnet turns it into a self-contained prompt for Gemma
  ↓
Gemma in Cline generates 1–2 files (free, local)
  ↓
Opus reviews the result against the contract
  ↓
Tests and simulations verify behavior
```

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

The workflow diagram («Sonnet turns it into a self-contained prompt for Gemma...») describes v1.x 4-tier base cycle. Under v1.6, base cycle is reframed in [METHODOLOGY §2.3 Verification cycle](./METHODOLOGY.md#23-verification-cycle) с architect-mode + executor-mode sessions instead of tier-specific agents.

### §1.3 Hardware reference

The experiment is conducted on an ASUS TUF A16 (Ryzen 7 7435HS, RX 7600S 8 GB VRAM, 32 GB RAM).

- **Local model host.** The same ASUS TUF A16 runs the Gemma 4 E4B (Q4_K_M) executor through LM Studio. Inference fits inside the 8 GB discrete VRAM with the 131072-token context window enabled.
- **Cloud-tier network load.** Sonnet 4.6 and Opus 4.7 traffic is conversational and incurs negligible bandwidth — both run as desktop chat sessions, not as continuous streaming. A consumer broadband link is sufficient.
- **Godot dev environment.** The Godot 4.3 editor and the C# build pipeline can run on the same machine; no separate workstation is required.

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

ASUS TUF A16 hardware unchanged across era boundary; the «local model host» framing is v1.x-specific (RX 7600S 8 GB VRAM hosted Gemma 4 E4B Q4_K_M locally). Under v1.6, the same hardware runs the Claude Desktop client but doesn't host local inference; the hardware capability survives, but the v1.x-specific GPU-VRAM-for-quantized-inference requirement does not. See §6 Forward measurement plan for v1.6 reformulation task.

### §1.4 Software stack

- **VS Code + Cline** — orchestrator for the local executor.
- **LM Studio + Gemma 4 E4B Q4_K_M** (.gguf, 6.33 GB) — local OpenAI-compatible inference backend.
- **Anthropic Claude desktop** (Sonnet 4.6 + Opus 4.7) — architectural-tier sessions.
- **Anthropic Max 5× subscription** — fixed-cost cloud capacity for the architectural tier.

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

VS Code+Cline orchestration, LM Studio+Gemma local backend are v1.x-tier-specific tooling. Under v1.6, software stack collapses к Claude Desktop client + Godot 4.3+ + .NET 8.0 (the latter two survive). See §6 Forward measurement plan for v1.6 reformulation task.

---

## §2. Empirical task-level metrics

### §2.1 Phase 4 baseline (2026-04, Cline history)

| Task | Context (tokens) | Output (tokens) | Artifact size |
|---|---|---|---|
| Implement InventorySystem | 13 900 | 1 600 | 55.9 kB |
| Implement power grid events | 99 900 | 2 800 | 80.1 kB |
| Implement ItemReservedEvent | 79 300 | 2 100 | 63.9 kB |
| Implement ItemAddedEvent | 132 900 | 2 800 | 79.5 kB |
| Implement StorageComponent | 98 200 | 3 800 | 83.8 kB |

Source: Cline interaction history, exported during Phase 4 closure.

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

5-task Gemma context/output sizes (InventorySystem 13.9k/1.6k, power grid events 99.9k/2.8k, ItemReservedEvent 79.3k/2.1k, ItemAddedEvent 132.9k/2.8k, StorageComponent 98.2k/3.8k) are 2026-04 Phase 4 archaeological data. Pure historical record; transferability к v1.6 measurement classes not applicable (v1.6 doesn't measure local executor token I/O per task; v1.6 metric class would be «execution session token cost per brief scope»).

### §2.2 M0-M3 evening session (2026-04-27, git log)

| Time (UTC-4) | Commit | Phase | Scope |
|---|---|---|---|
| 19:47 | a97dcbf | M1 | feat(contracts/modding): add ModDependency record |
| 19:55 | 7310405 | M1 | feat(contracts/modding): add ManifestCapabilities |
| 20:09 | c76b802 | M1 | fix(contracts/modding): rewrite to match §3.2 |
| 20:20 | 70037c1 | M1 | feat(application/modding): 7 ValidationErrorKind entries |
| 20:30 | 5d82f40 | M1 | feat(contracts/modding): ModManifest v2 schema |
| 20:49 | 45ca7a2 | M1 | feat(application/modding): ManifestParser |
| 21:23 | 35dc5b2 | M2 | feat(modding/api): IModApi Publish/Subscribe + bus routing |
| 21:35 | 2ea107c | M3.1 | feat(contracts/attributes): ModAccessible + ModCapabilities |
| 21:43 | 60c923b | M3.2 | feat(application/modding): KernelCapabilityRegistry |
| 22:05 | 91bbe82 | M3.3 | feat(application/modding): ContractValidator C+D |
| 22:43 | 0d5b32f | M3.2 fix | fix(modding): close audit findings |
| 22:49 | e37ca25 | docs | MOD_OS_ARCHITECTURE v1.1 + ROADMAP |
| 22:51 | 2e5216b | merge | PR #18 closed |

Wall-clock for the M-phase work itself: 3h 4min (19:47 → 22:51). Scope closed: four migration phases (M1 Manifest v2, M2 IModApi v2, M3.1 Capability Registry, M3.3 Capability Validation), audit pass with 4 Tier 2 findings + 3 Tier 1 findings closed, specification version sync v1.0 → v1.1. M3.3 alone (`91bbe82`) added 391 lines and 11 acceptance tests in a single 22-minute commit window (commits at 21:43 → 22:05).

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

M-series migration timing 2026-04-27 under 4-agent pipeline; 13-commit timeline (a97dcbf..2e5216b, 19:47–22:51 UTC-4) with M1 manifest schema + M2 IModApi v2 + M3.1 capability registry + M3.3 capability validation closures. Archaeological data; v1.6 equivalent would be Phase A' execution session timing (A'.0.5 closure as first v1.6 era data point, recorded в METHODOLOGY §4.4 Case B).

### §2.3 Cost-per-commit at executor tier

All Gemma execution at the syntax-tier runs locally on the developer's machine; cost-per-commit at the executor tier is $0.00. Architectural work routed through Sonnet (prompt generation, mid-complexity tasks) and Opus (Phase 0 lock, audits, phase reviews) is paid via the fixed Claude Max 5× subscription. The pipeline does not consume pay-as-you-go API budget at any tier under normal operation.

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

«All Gemma execution at the syntax-tier runs locally; cost-per-commit at the executor tier is $0.00» describes v1.x free-local + paid-cloud divide. Under v1.6 single-tier Claude Desktop subscription, executor tier doesn't have separate cost profile from architect tier. Replaced by [METHODOLOGY §3.1 Economic invariant](./METHODOLOGY.md#31-economic-invariant) (cost asymmetry between deliberation-context-intensive and execution-scope-bounded work).

---

## §3. Subscription headroom

### §3.1 Stress-test methodology

On a deliberate stress week (week ending 2026-04-26), the user routed full-day chat plus active code generation through Opus rather than the usual Sonnet-heavy distribution, with the explicit intent of exhausting the weekly Anthropic Max 5× limit before reset.

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

Stress-test procedure (week ending 2026-04-26 deliberate Opus-heavy stress) is v1.x-specific methodology for measuring subscription headroom. Under v1.6 unified Claude Desktop subscription, equivalent stress-test would measure session-volume-vs-rate-limit; documented procedure preserved as archaeological reference.

### §3.2 Measurements across two consecutive weekly windows

| Window | Operating mode | At reset |
|---|---|---|
| Week ending 2026-04-26 | Deliberate Opus-heavy stress | 73% of weekly limit unused |
| Week ending 2026-05-04 (in progress at recording) | Natural mixed work, Phase 0 + M1-M3 + audit + i18n closure | 25% remaining at 10.5h before reset |

Two independent measurements across consecutive weekly windows under different operational profiles converge on the same headroom band. The pipeline does not exhaust subscription capacity even under deliberate stress.

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

Subscription headroom IS still measurable under v1.6 — the **measurement category** transfers; the specific numbers (73% unused at reset, 25% remaining at 10.5h before reset) are v1.x-era data points. v1.6 measurement collection forward-looking; first v1.6 data points emerge from A'.0.5 closure + A'.0.7 deliberation session + subsequent Phase A' milestones. See §6 Forward measurement plan.

### §3.3 Architectural dividend hypothesis (cross-reference METHODOLOGY §3)

The token economics observed here are consistent with the architectural-dividend hypothesis stated in METHODOLOGY §3.1: when the architecture is fixed as LOCKED specifications with §-level addressability, the architecture-tier agent (Opus) operates predominantly in implementor mode rather than discovery mode, which avoids the reasoning-from-ambiguity overhead that dominates token spend in unstructured conversational use of the same model.

**Annotation** (A'.0.7, 2026-05-10): `[transfers as-is]`

Hypothesis is invariant: «when architecture is fixed as LOCKED specifications с §-level addressability, the architecture-tier agent operates predominantly in implementor mode rather than discovery mode, which avoids reasoning-from-ambiguity overhead». Survives boundary-type change. Supports [METHODOLOGY §3.1 Economic invariant](./METHODOLOGY.md#31-economic-invariant) (context-intensive deliberation work vs scope-bounded execution work).

---

## §4. Sustained throughput

### §4.1 8-day commit history aggregate

| Date | Commits | Notes |
|---|---|---|
| 2026-04-20 | 8 | Project start |
| 2026-04-21 | 43 | Phase 1–2 ramp |
| 2026-04-22 | 24 | |
| 2026-04-23 | 66 | Peak day |
| 2026-04-24 | 26 | |
| 2026-04-25 | 12 | Phase 4 closure |
| 2026-04-26 | 36 | i18n campaign |
| 2026-04-27 | 39 | M0-M3 + audit + i18n closure |

Total: 254 commits over 8 calendar days. Mean ~32 commits/day; peak 66. Codebase at end of window: 17 462 lines of C# across 289 files; 8 277 lines of documentation across 33 markdown documents; code-to-documentation ratio approximately 2:1.

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

254 commits over 8 calendar days (2026-04-20..2026-04-27) — Phase 1–4 ramp + i18n campaign + M0-M3 closure. Archaeological data; codebase end-of-window snapshot (17,462 lines C# / 8,277 lines docs / 2:1 ratio) preserved as v1.x reference. v1.6 equivalent would be ongoing daily-commit-rate measurement under Phase A' execution; not collected in this document version.

### §4.2 M0-M3 evening session — minute-by-minute

See §2.2 for the per-commit timing of the M-phase evening session. The figures support a wall-clock ratio of approximately 300× compared to a conventional industry baseline of 6–9 weeks for a senior engineer in a team setting to deliver an equivalent scope (manifest schema + parser + IModApi rewrite + capability registry with reflection scan + capability validation + ~110 acceptance tests + audit + spec sync).

**Annotation** (A'.0.7, 2026-05-10): `[v1.x historical record]`

Per-commit timing of the M-phase evening session 2026-04-27 (3h 4min wall-clock for four migration phases + audit + spec sync). Same era as §2.2. Wall-clock ratio claim («~300× compared к conventional industry baseline of 6–9 weeks senior engineer team setting») is v1.x-era observation; v1.6 equivalent ratios will emerge from Phase A' / B execution measurements.

### §4.3 Concurrent workflow observation (M-phases + i18n in one window)

The 8-day window also produced a complete i18n refactoring campaign in parallel with the architectural design work: full Russian-to-English translation of all source comments, XML docs, and module READMEs, executed across approximately 25 chore(i18n) commits between 2026-04-26 and 2026-04-27. The pipeline sustained two independent workflows — architectural design (M-phases) and large-scale refactoring (i18n) — within the same window without cross-contamination, and both closed cleanly: M-phases with green audit, i18n with grep-clean cyrillic verification across the source tree.

**Annotation** (A'.0.7, 2026-05-10): `[transfers as-is]`

Two-workflow concurrency claim is invariant: «pipeline sustained two independent workflows — architectural design (M-phases) and large-scale refactoring (i18n) — within the same window without cross-contamination». Mechanism (clean phase separation + independent contract surfaces) survives boundary-type change. v1.6 equivalent: deliberation session and execution session can run on independent surfaces в parallel (e.g., K-L3.1 deliberation + A'.0.5 execution on adjacent days, both clean).

---

## §5. Reproducibility requirements

### §5.1 Minimum hardware

A machine with a discrete GPU and 8 GB VRAM, or Apple Silicon with 16 GB unified memory, is sufficient to host the local executor at Q4_K_M quantization with the full 131072-token context window. The reference machine for the measurements above is an ASUS TUF A16 (Ryzen 7 7435HS, RX 7600S 8 GB, 32 GB RAM); equivalent or stronger hardware should reproduce the executor-tier figures.

**Annotation** (A'.0.7, 2026-05-10): `[v1.x era specific]`

«Machine with discrete GPU and 8 GB VRAM, or Apple Silicon с 16 GB unified memory» requirement is tied к local Gemma quantization (Q4_K_M Gemma 4 E4B needs 6.33 GB VRAM minimum). Under v1.6, local inference is not part of the pipeline — minimum hardware reduces к whatever the Claude Desktop client requires (modest by comparison). See §6 Forward measurement plan for v1.6 reformulation.

### §5.2 Software dependencies

.NET 8.0 SDK (C# 12); Godot 4.3+ with C# / mono support; recent VS Code with the [Cline](https://github.com/cline/cline) extension; LM Studio for the local OpenAI-compatible backend hosting Gemma 4 E4B (Q4_K_M, 6.33 GB) — comparable Qwen 2.5 Coder or Llama 3.1 8B variants are usable substitutes; the Anthropic Claude desktop app for Sonnet 4.6 + Opus 4.7 access.

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

.NET 8.0 SDK + Godot 4.3+ survive across era boundary (project tech stack). VS Code+Cline + LM Studio+Gemma are v1.x-specific tooling that doesn't transfer. Anthropic Claude desktop app survives across boundary; subscription-tier guidance same (Max 5×). See §6 Forward measurement plan for v1.6 reformulation.

### §5.3 Subscription tier

Anthropic Max 5× subscription ($100/month). Lower tiers (Pro, Max 1×) may be sufficient for lighter operational modes but were not measured. The pipeline does not require API access; all paid use happens through the desktop applications under the subscription.

**Annotation** (A'.0.7, 2026-05-10): `[transfers с reframing]`

Claude Max 5× ($100/month) subscription requirement persists across era boundary. «Lower tiers (Pro, Max 1×) may be sufficient for lighter operational modes» observation retains validity. «Pipeline does not require API access» — true under v1.6 (Claude Desktop client uses subscription, not API key); verify for v1.6 era through Phase A' / B execution. See §6 Forward measurement plan.

### §5.4 What these measurements tell you / what they don't

*What they tell you.* These figures show pipeline behavior in the architecturally-mature mode — where specifications are LOCKED, contracts are well-formed, and acceptance criteria are explicit. They are reproducible by someone running an equivalent stack against equivalent task-classes.

*What they don't tell you.* These figures do not show pipeline behavior in early-stage open exploration where specifications have not been written yet. Phase 0 lock work, by its nature, takes longer per token than implementor work; the throughput numbers above describe execution against a finished spec, not the spec-writing itself. A reproduction attempt without a Phase 0 lock step will not see comparable ratios.

**Annotation** (A'.0.7, 2026-05-10): `[transfers as-is]`

Methodological framing about measurement validity («figures show pipeline behavior in architecturally-mature mode... not behavior in early-stage open exploration») is invariant. The distinction между Phase 0 lock work and implementor work persists across boundary types. The «reproduction attempt without a Phase 0 lock step will not see comparable ratios» caveat applies in any era.

---

## §6 Forward measurement plan (v1.6 era data collection)

Per Q10.a annotations (A'.0.7 amendment, 2026-05-10), the following sections require v1.6 era data collection or reformulation. Tasks are not A'.0.7 amendment scope; they form backlog для post-A'.0.7 methodology corpus evolution.

| Section | Task | Trigger |
|---|---|---|
| §1.3 Hardware reference | Reframe «local model host» к v1.6 reality (cloud-tier model via Claude Desktop client; no local inference; minimum hardware reduces к Claude Desktop runtime + project IDE) | Post-A'.0.7 amendment landing |
| §3.2 Subscription headroom | Collect v1.6 era subscription utilization data — first data points: A'.0.5 closure (2026-05-10, single execution session), A'.0.7 deliberation session (2026-05-10, single deliberation session), subsequent Phase A' milestones | Phase A' execution cadence |
| §5.1 Minimum hardware | Reframe — Claude Desktop client minimum hardware (modest), no GPU/VRAM-for-local-inference requirement | Post-A'.0.7 amendment landing |
| §5.2 Software dependencies | Reframe tool stack — Claude Desktop client primary; VS Code+Cline + LM Studio v1.x-specific (removed for v1.6); Godot 4.3+ + .NET 8.0 survive | Post-A'.0.7 amendment landing |
| §5.3 Subscription tier | Verify «API access not required» under v1.6 Claude Desktop subscription; update subscription tier guidance with v1.6-era practical experience | Post-A'.0.7 amendment landing |

Task completion lifts content from this backlog к main document body; this section itself stays present с completed rows marked or condensed format as historical record of A'.0.7-era forward-measurement plan.

---

## Version history

| Version | Date | Change |
|---|---|---|
| 0.1 | 2026-04-28 | Initial collection covering Phases 0–4 baseline plus M0–M3 Mod-OS migration; gathered under v1.x era pipeline (4-agent model-tier boundary: human + Gemma local executor + Sonnet prompt generator + Opus architect). |
| 0.2 | 2026-05-10 | Per-metric annotations added per A'.0.7 amendment (Q10.a-with-standardized-labels): 5 standardized transferability labels applied к each section; top-of-document era frame note; new §6 Forward measurement plan tracks v1.6 era data collection backlog. v1.x era measurements preserved verbatim. |

---

**See also:** [METHODOLOGY](./METHODOLOGY.md), [METHODOLOGY_OBSERVATIONS](./METHODOLOGY_OBSERVATIONS.md), [ROADMAP](./ROADMAP.md).

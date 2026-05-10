---
title: Pipeline Metrics
nav_order: 27
---

<!--
A'.0.7 DEFERRAL MARKER (added 2026-05-10 in A'.0.5 Phase 7).

Empirical metrics in this document were gathered with the 4-agent
pipeline (Crystalka + local quantized executor + cloud prompt-generator
+ cloud architect). Crystalka direction 2026-05-10 restructured the
pipeline to 2-agent (Crystalka + unified Claude Desktop session).
Continued accuracy of these metrics under the new pipeline is not
established. A'.0.7 milestone deliberates whether to:
- discard prior metrics (no longer representative)
- reframe as historical record at a labeled time-of-measurement
- recollect under the new pipeline configuration before publication

A'.0.5 (this milestone) only relocated the file to docs/methodology/
and applied mechanical pipeline-terminology scrubs in non-substantive
contexts. Substantive empirical claims are preserved verbatim as the
state to be re-evaluated in A'.0.7.

See /docs/architecture/PHASE_A_PRIME_SEQUENCING.md for the Phase A'
sequence locating A'.0.7 between A'.0.5 and A'.1.
-->

# Pipeline metrics — Dual Frontier

*Empirical configuration, throughput data, and reproducibility requirements
for the four-agent LLM pipeline used in this project. The companion document
METHODOLOGY.md describes the methodology as designed; this document records
the operational data measured while running it.*

**Status:** v0.1 (2026-04-28). Initial collection covering Phases 0–4 baseline
plus M0–M3 Mod-OS migration. Updated as further phases close.

## §1. Pipeline configuration

### §1.1 Four-agent role distribution

| Agent | Role | Where it runs | What it does |
|---|---|---|---|
| **Gemma 4 E4B** (Q4_K_M, 6.33 GB) | Executor | Locally through LM Studio + **Cline** in VS Code | Writes concrete code from a prompt: 1 prompt → 1–2 files, 131072-token context window |
| **Claude Sonnet 4.6** | Prompt generator | Claude Max 5× (desktop app) | Turns a task plus its contract into a precise prompt for the local model; handles routine tasks directly |
| **Claude Opus 4.7** | Architect and QA | Claude Max 5× (desktop app) | Hard architectural work (scheduler, dependency graph) and full reviews at phase closure. Used sparingly. |
| **Human** | Direction owner | — | Selects contracts, makes architectural decisions, frames phase goals |

Agents do not communicate directly. The repository (LOCKED documents, tests) and the human (as router between sessions) are the only coordination surfaces.

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

### §1.3 Hardware reference

The experiment is conducted on an ASUS TUF A16 (Ryzen 7 7435HS, RX 7600S 8 GB VRAM, 32 GB RAM).

- **Local model host.** The same ASUS TUF A16 runs the Gemma 4 E4B (Q4_K_M) executor through LM Studio. Inference fits inside the 8 GB discrete VRAM with the 131072-token context window enabled.
- **Cloud-tier network load.** Sonnet 4.6 and Opus 4.7 traffic is conversational and incurs negligible bandwidth — both run as desktop chat sessions, not as continuous streaming. A consumer broadband link is sufficient.
- **Godot dev environment.** The Godot 4.3 editor and the C# build pipeline can run on the same machine; no separate workstation is required.

### §1.4 Software stack

- **VS Code + Cline** — orchestrator for the local executor.
- **LM Studio + Gemma 4 E4B Q4_K_M** (.gguf, 6.33 GB) — local OpenAI-compatible inference backend.
- **Anthropic Claude desktop** (Sonnet 4.6 + Opus 4.7) — architectural-tier sessions.
- **Anthropic Max 5× subscription** — fixed-cost cloud capacity for the architectural tier.

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

### §2.3 Cost-per-commit at executor tier

All Gemma execution at the syntax-tier runs locally on the developer's machine; cost-per-commit at the executor tier is $0.00. Architectural work routed through Sonnet (prompt generation, mid-complexity tasks) and Opus (Phase 0 lock, audits, phase reviews) is paid via the fixed Claude Max 5× subscription. The pipeline does not consume pay-as-you-go API budget at any tier under normal operation.

---

## §3. Subscription headroom

### §3.1 Stress-test methodology

On a deliberate stress week (week ending 2026-04-26), the user routed full-day chat plus active code generation through Opus rather than the usual Sonnet-heavy distribution, with the explicit intent of exhausting the weekly Anthropic Max 5× limit before reset.

### §3.2 Measurements across two consecutive weekly windows

| Window | Operating mode | At reset |
|---|---|---|
| Week ending 2026-04-26 | Deliberate Opus-heavy stress | 73% of weekly limit unused |
| Week ending 2026-05-04 (in progress at recording) | Natural mixed work, Phase 0 + M1-M3 + audit + i18n closure | 25% remaining at 10.5h before reset |

Two independent measurements across consecutive weekly windows under different operational profiles converge on the same headroom band. The pipeline does not exhaust subscription capacity even under deliberate stress.

### §3.3 Architectural dividend hypothesis (cross-reference METHODOLOGY §3)

The token economics observed here are consistent with the architectural-dividend hypothesis stated in METHODOLOGY §3.1: when the architecture is fixed as LOCKED specifications with §-level addressability, the architecture-tier agent (Opus) operates predominantly in implementor mode rather than discovery mode, which avoids the reasoning-from-ambiguity overhead that dominates token spend in unstructured conversational use of the same model.

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

### §4.2 M0-M3 evening session — minute-by-minute

See §2.2 for the per-commit timing of the M-phase evening session. The figures support a wall-clock ratio of approximately 300× compared to a conventional industry baseline of 6–9 weeks for a senior engineer in a team setting to deliver an equivalent scope (manifest schema + parser + IModApi rewrite + capability registry with reflection scan + capability validation + ~110 acceptance tests + audit + spec sync).

### §4.3 Concurrent workflow observation (M-phases + i18n in one window)

The 8-day window also produced a complete i18n refactoring campaign in parallel with the architectural design work: full Russian-to-English translation of all source comments, XML docs, and module READMEs, executed across approximately 25 chore(i18n) commits between 2026-04-26 and 2026-04-27. The pipeline sustained two independent workflows — architectural design (M-phases) and large-scale refactoring (i18n) — within the same window without cross-contamination, and both closed cleanly: M-phases with green audit, i18n with grep-clean cyrillic verification across the source tree.

---

## §5. Reproducibility requirements

### §5.1 Minimum hardware

A machine with a discrete GPU and 8 GB VRAM, or Apple Silicon with 16 GB unified memory, is sufficient to host the local executor at Q4_K_M quantization with the full 131072-token context window. The reference machine for the measurements above is an ASUS TUF A16 (Ryzen 7 7435HS, RX 7600S 8 GB, 32 GB RAM); equivalent or stronger hardware should reproduce the executor-tier figures.

### §5.2 Software dependencies

.NET 8.0 SDK (C# 12); Godot 4.3+ with C# / mono support; recent VS Code with the [Cline](https://github.com/cline/cline) extension; LM Studio for the local OpenAI-compatible backend hosting Gemma 4 E4B (Q4_K_M, 6.33 GB) — comparable Qwen 2.5 Coder or Llama 3.1 8B variants are usable substitutes; the Anthropic Claude desktop app for Sonnet 4.6 + Opus 4.7 access.

### §5.3 Subscription tier

Anthropic Max 5× subscription ($100/month). Lower tiers (Pro, Max 1×) may be sufficient for lighter operational modes but were not measured. The pipeline does not require API access; all paid use happens through the desktop applications under the subscription.

### §5.4 What these measurements tell you / what they don't

*What they tell you.* These figures show pipeline behavior in the architecturally-mature mode — where specifications are LOCKED, contracts are well-formed, and acceptance criteria are explicit. They are reproducible by someone running an equivalent stack against equivalent task-classes.

*What they don't tell you.* These figures do not show pipeline behavior in early-stage open exploration where specifications have not been written yet. Phase 0 lock work, by its nature, takes longer per token than implementor work; the throughput numbers above describe execution against a finished spec, not the spec-writing itself. A reproduction attempt without a Phase 0 lock step will not see comparable ratios.

---

**See also:** [METHODOLOGY](./METHODOLOGY.md), [METHODOLOGY_OBSERVATIONS](./METHODOLOGY_OBSERVATIONS.md), [ROADMAP](./ROADMAP.md).

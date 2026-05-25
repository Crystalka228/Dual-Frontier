---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-ANALYZER_RULES
category: A
tier: 1
lifecycle: AUTHORED-SKELETON
owner: Crystalka
version: "0.1"
next_review_due: 2027-05-23
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-ANALYZER_RULES
---
# DualFrontier Roslyn Analyzer Rule Specifications

**Lifecycle**: AUTHORED-SKELETON (initial entry at A'.8 closure 2026-05-23)
**Version**: 0.1
**Date created**: 2026-05-23 (А'.8 К-series formal closure cascade Commit 5 REGISTER enrollment)
**Purpose**: Roslyn analyzer rule specifications encoding К-Lxx invariants. Populated к LOCKED at A'.9 Roslyn analyzer milestone implementation cascade.

**Authority**: This document specifies analyzer rules. Canonical rule narratives + detection patterns + diagnostic messages reside в [K_CLOSURE_REPORT.md §7](K_CLOSURE_REPORT.md#7--roslyn-analyzer-rule-specifications). К-Lxx invariant authority resides в [KERNEL_ARCHITECTURE.md Part 0](KERNEL_ARCHITECTURE.md#part-0-locked-foundational-decisions).

**Forward sequencing**: A'.9 Roslyn analyzer milestone implements 18 active rules в analyzer infrastructure. First-run cleanup phase: warning → error promotion as architectural debt resolved. DFK020 activates at Mod API lock milestone landing с К-L20 codification.

---

## §1 — Rule taxonomy at A'.8 closure

**18 active rules** (specified at A'.8 closure; implementation pending A'.9):
- DFK001 К-L1 native language C++20
- DFK002 К-L2 pure P/Invoke bindings
- DFK003 К-L3 component storage paths
- DFK003.1 К-L3.1 Path β bridge
- DFK004 К-L4 explicit type ID registry
- DFK005 К-L5 declarative bootstrap graph
- DFK007 К-L7 span protocol
- DFK007.1 К-L7.1 GPU pipeline slot binding
- DFK009 К-L9 mod parity
- DFK010 К-L10 decision rule
- DFK011 К-L11 production storage backbone
- DFK012 К-L12 native kernel scheduling
- DFK013 К-L13 on-demand activation (Warning severity, efficiency-not-correctness)
- DFK015 К-L15 native bus authority
- **DFK015.1 К-L15.1 three-tier mutex isolation (NEW A'.7.x)**
- DFK016 К-L16 simulation tick pipeline depth (Warning severity, configurable)
- DFK017 К-L17 display composition multi-layer
- DFK018 К-L18 mod lifecycle quiescent state
- DFK019 К-L19 hardware tier commitment (Warning severity, V substrate contract)

**4 reserved rules**:
- DFK006 К-L6 SUPERSEDED permanently (К-L6 displaced by К-L12)
- DFK008 К-L8 process invariant (git pre-commit hook alternative)
- DFK014 К-L14 meta-invariant (K_L14_EVIDENCE_DASHBOARD.md alternative)
- DFK020 К-L20 reserved post-Mod API lock

---

## §2 — Rule specification template

**Per-rule specification format**:

```markdown
### DF### — <К-L invariant title>

**К-L invariant**: <К-L# reference + canonical text link к KERNEL_ARCHITECTURE.md Part 0>
**Severity**: Error / Warning
**Status**: Active / Reserved
**Implementation status**: Pending A'.9 / Implemented at A'.9 / Active post-A'.9

**Detection narrative** (3-5 sentences per Q5.3 LOCKED Session 1):
<What patterns the rule detects. Reference K_CLOSURE_REPORT.md §7.2 для canonical detection narrative.>

**Diagnostic message text**:
> «<diagnostic message string>»

**Example violation patterns**:
- <code pattern 1 that triggers diagnostic>
- <code pattern 2 that triggers diagnostic>

**Suppression policy**:
- When суppression appropriate: <rare cases с justification>
- Suppression syntax: `#pragma warning disable DF###` OR `[SuppressMessage("DualFrontier.Analyzer", "DF###")]`
- Suppression requires architectural justification comment

**Test cases** (positive + negative examples):
- Positive: <code pattern that должно triggered diagnostic>
- Negative: <code pattern that должно NOT trigger diagnostic>

**Cross-references**:
- К-L canonical text: KERNEL_ARCHITECTURE.md Part 0 К-L# row
- К-L falsifiability commitment: K_CLOSURE_REPORT.md §2.# entry
- Related Lessons: <list>
```

---

## §3 — Forward implementation plan (A'.9 milestone)

**A'.9 cascade scope**:
- Roslyn analyzer NuGet package (`DualFrontier.Analyzer`) infrastructure
- Per-rule analyzer class (one per DF### rule)
- Test project с positive + negative cases per rule
- CI integration: analyzer warnings fail CI in Release builds
- Documentation: rule wiki entries cross-referenced к К-L invariants
- First-run cleanup phase: warning → error promotion as architectural debt resolved through codebase audit

**Cleanup phase discipline**:
- Per-rule cleanup discipline (one rule's violations resolved at a time для atomic discipline per Lesson #8)
- Cumulative debt resolution across 18 active rules
- Codebase audit may surface architectural debt unrelated к rule (handled per Lesson #14 separate cleanup cascade)

**DFK020 post-Mod API lock**:
- DFK020 К-L20 rule activates at Mod API lock milestone landing
- К-L20 codification + Mod API surface frozen
- DFK020 implementation scope: Mod API surface deviation detection, Bridge mechanism bypass, manifest grace period semantics

---

## §4 — Rule specifications (initial skeleton — populated post-A'.9)

**Per K_CLOSURE_REPORT.md §7.2 table**, 18 active rule specifications are captured at K_CLOSURE_REPORT.md §7.2 канонически. This document will be populated с per-rule detailed specifications (per §2 template above) at A'.9 implementation cascade.

**Initial entries at A'.8 closure**: К-L invariant references + severity + status + detection narrative summary. Full implementation specifications appended at A'.9 milestone.

### Initial rule list (per K_CLOSURE_REPORT.md §7.2)

| Rule | К-L | Severity | Status | Reference |
|---|---|---|---|---|
| DFK001 | К-L1 | Error | Active (pending A'.9 impl) | [K_CLOSURE_REPORT.md §7.2](K_CLOSURE_REPORT.md#72--active-rules-table) |
| DFK002 | К-L2 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK003 | К-L3 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK003.1 | К-L3.1 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK004 | К-L4 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK005 | К-L5 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK007 | К-L7 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK007.1 | К-L7.1 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK009 | К-L9 | Error | Active (pending A'.9 impl; revisit post-Mod API lock) | K_CLOSURE_REPORT.md §7.2 |
| DFK010 | К-L10 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK011 | К-L11 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK012 | К-L12 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK013 | К-L13 | Warning | Active (efficiency, не correctness; pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK015 | К-L15 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| **DFK015.1** | **К-L15.1** | **Error** | **Active (NEW А'.7.x; pending A'.9 impl)** | K_CLOSURE_REPORT.md §7.2 |
| DFK016 | К-L16 | Warning | Active (configurable severity; pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK017 | К-L17 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK018 | К-L18 | Error | Active (pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |
| DFK019 | К-L19 | Warning | Active (V substrate contract, configurable; pending A'.9 impl) | K_CLOSURE_REPORT.md §7.2 |

### Reserved rules

| Rule | К-L | Reservation reason | Reference |
|---|---|---|---|
| DFK006 | К-L6 | SUPERSEDED permanently (К-L6 displaced by К-L12) | [K_CLOSURE_REPORT.md §7.3](K_CLOSURE_REPORT.md#73--reserved-rules-table) |
| DFK008 | К-L8 | Process invariant — git pre-commit hook alternative | K_CLOSURE_REPORT.md §7.3 |
| DFK014 | К-L14 | Meta-invariant — K_L14_EVIDENCE_DASHBOARD.md alternative | K_CLOSURE_REPORT.md §7.3 |
| DFK020 | К-L20 | Reserved post-Mod API lock; activates at Mod API lock milestone | K_CLOSURE_REPORT.md §7.3 |

---

## §5 — Lifecycle forward

**Current**: AUTHORED-SKELETON v0.1 (А'.8 closure 2026-05-23).

**Forward к LOCKED at A'.9 milestone**:
- Per-rule §2 template populated через A'.9 cascade
- Roslyn analyzer NuGet package implementation
- Test coverage per rule (positive + negative cases)
- CI integration verified
- Cleanup phase outcomes recorded
- Promotion к Tier 1 LOCKED при completed implementation + first-run cleanup phase

---

**End of ANALYZER_RULES.md v0.1 AUTHORED-SKELETON**

**Forward maintenance**: A'.9 cascade implements 18 active rules per §2 template + §3 implementation plan. DFK020 added at Mod API lock milestone. К-Lxx invariant authority resides в KERNEL_ARCHITECTURE.md; this document encodes К-Lxx invariants as analyzer rules.

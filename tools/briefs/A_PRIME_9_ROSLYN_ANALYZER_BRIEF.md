---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_9_ROSLYN_ANALYZER
category: D
tier: 3
lifecycle: AUTHORED-SKELETON
owner: Crystalka
version: "0.1"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_9_ROSLYN_ANALYZER
---
# А'.9 Roslyn Architectural Analyzer — Execution Brief (SKELETON)

**Brief authored**: 2026-05-17 (skeleton; post-К-closure forward planning)
**Lifecycle**: AUTHORED-SKELETON
**Target session**: Future Opus deliberation session (Roslyn analyzer brief authoring)
**Subsequent execution**: Claude Code execution mode для implementation
**Estimated full brief size**: 1000-1500 lines
**Parent**: К-closure report (А'.8) Roslyn analyzer rule specification section (Part 6)

---

## 0. Executive summary (skeleton)

Implement Roslyn architectural analyzer encoding К-Lxx invariants as compile-time rules.

**Dual purpose**:

1. **Active M-series migration verifier** — catches drift в file movements, namespace changes, ModApi registration patterns that test suite doesn't catch
2. **К-Lxx invariant compile-time enforcement** — bugs that compile + pass tests but violate К-Lxx invariants surface at build time

**First run expected к surface pre-existing debt** — fix budget внутри А'.9 scope. Linux `-Werror` adoption precedent: massive cleanup campaign precedes flag enablement. Analyzer first run = cleanup phase.

**К-Lxx invariants targeted** (20 invariants final post-К-closure):

К-L1 (C++20 native language), К-L2 (Pure P/Invoke), К-L3 + К-L3.1 (component storage paths), К-L4 (explicit registry), К-L5 (declarative bootstrap graph), К-L6 SUPERSEDED, К-L7 + К-L7.1 (span protocol), К-L8 (component lifetime), К-L9 (mod parity), К-L10 (decision rule §8 metrics), К-L11 (NativeWorld single source of truth), К-L12 (full native kernel scheduling), К-L13 (on-demand activation), К-L14 (performance from architecture — *meta-invariant*; not directly encodable), К-L15 (native bus three-tier), К-L16 (pipeline depth), К-L17 (display composition), К-L18 (mod lifecycle quiescent state), К-L19 (hardware tier).

Note: К-L14 is meta-invariant (causal claim about architecture → performance); not encodable as Roslyn rule. К-L6 SUPERSEDED; not analyzed. Remaining 18 invariants targeted.

---

## 1. Phase 0 — Preflight reads (skeleton)

Full brief к specify. Anticipated:

- К-closure report (А'.8 output) Part 6 — Roslyn analyzer rule specification: authoritative input
- All 20 К-Lxx invariants verbatim text — KERNEL_ARCHITECTURE.md final state
- Roslyn analyzer SDK documentation — capability check
- DiagnosticAnalyzer + CodeFixProvider API surface — implementation pattern
- Existing project analyzer configuration (если any) — `.editorconfig`, `Directory.Build.props`
- Test fixtures for К-Lxx invariants — analyzer test corpus

---

## 2. Expected sub-milestones (skeleton)

**Sub-milestone A9.A — Analyzer scaffolding**:
- New project: `tools/analyzers/DualFrontier.Analyzers/`
- DiagnosticAnalyzer base class
- Build integration (Directory.Build.props references)
- Test project: `tools/analyzers/DualFrontier.Analyzers.Tests/`

**Sub-milestone A9.B — Per-invariant rule authoring** (per К-Lxx, ~18 rules):
- DF001: К-L1 — C++20 native language (CMake CXX_STANDARD check; meta-rule, build-time)
- DF002: К-L2 — Pure P/Invoke (no SWIG/C++/CLI references)
- DF003: К-L3 + К-L3.1 — Component storage path declarations
- DF004: К-L4 — Component types registered via ComponentTypeRegistry
- DF005: К-L5 — Bootstrap graph declarative
- DF007: К-L7 — Span protocol (read-only spans + WriteBatch)
- DF007.1: К-L7.1 — GPU compute pipeline slot binding
- DF008: К-L8 — Component lifetime native ownership
- DF009: К-L9 — Vanilla = mods (IModApi registration uniformity)
- DF010: К-L10 — Decision rule metrics adherence
- DF011: К-L11 — NativeWorld single source of truth (no ManagedTestWorld production references)
- DF012: К-L12 — Full native kernel scheduling (no managed scheduler production references)
- DF013: К-L13 — On-demand activation (wake-up registration patterns)
- DF015: К-L15 — Native bus three-tier (tier declarations per event type)
- DF016: К-L16 — Pipeline depth (sim/display thread separation)
- DF017: К-L17 — Display composition multi-layer
- DF018: К-L18 — Mod lifecycle quiescent state
- DF019: К-L19 — Hardware tier (Vulkan 1.3 + async compute capability checks)

Approximate 17-18 rules.

**Sub-milestone A9.C — First run + pre-existing debt fix**:
- Analyzer enabled с warnings (not errors initially)
- First run on current codebase — surfaces violations
- Fix budget within А'.9 scope (cleanup phase, similar к cleanup cascade 2026-05-16)
- Each violation surfaced → either (a) fixed in code, (b) suppressed с rationale [#pragma warning disable + comment], (c) rule false-positive → rule refinement

**Sub-milestone A9.D — Warning → Error promotion**:
- After cleanup phase, analyzer rules promoted from warning к error severity
- Build fails on К-Lxx violation
- CodeFixProviders authored where mechanical fix possible

**Sub-milestone A9.E — Test infrastructure**:
- Per-rule test suite (one positive + one negative case minimum)
- Analyzer test fixture (Microsoft.CodeAnalysis.Testing pattern)

---

## 3. Halt conditions (skeleton)

- HG-1: Working tree dirty
- HG-2: К-closure report (А'.8) not landed — analyzer rule spec input missing
- HG-3: К-Lxx invariant interpretation ambiguous — analyzer rule under-specified, surface к Crystalka
- SC-1: Pre-existing debt fix budget exceeded — surface к Crystalka для scope decision (А'.9 vs deferred к follow-up milestone)
- SC-2: Roslyn analyzer SDK limitations encountered (e.g., К-Lxx invariant not encodable as static analysis) — surface к Crystalka

---

## 4. Q-N seeds (skeleton)

- Q-N-A9-1: Per-К-Lxx rule design — strict invariant encoding или heuristic detection? Some К-Lxx (К-L11 «NativeWorld single source of truth») are architectural; analyzer can catch some violations not all
- Q-N-A9-2: К-L14 (meta-invariant) — encoding strategy or explicit «not encoded» rationale в analyzer documentation?
- Q-N-A9-3: Severity ramp — initial warning + cleanup + promote к error, или from-day-one error?
- Q-N-A9-4: Test suite coverage — exhaustive (every К-Lxx +/- case) or representative?
- Q-N-A9-5: CodeFixProvider scope — provide automated fixes where possible, or analyzer-only? Mechanical fixes risk obscuring violations
- Q-N-A9-6: Fix budget — hard cap (e.g., 100 violations) or open-ended within А'.9 scope?

---

## 5. Closure protocol stub (skeleton)

Full brief к specify. Anticipated:
- sync_register.ps1 --validate exit 0 gate
- All 17-18 analyzer rules authored + tested
- First-run cleanup complete (build clean post-cleanup phase)
- Analyzer enabled в CI (gating build)
- К-Lxx invariants encoded as enforcement (К-Lxx → DFNNN rule mapping documented)
- METHODOLOGY.md mention of analyzer enforcement layer
- New EVT — EVT-2026-XX-XX-A_PRIME_9-ROSLYN-ANALYZER
- А'.9 milestone marked complete; Phase B M-series gate unblocks

---

## 6. Reference appendices (skeleton)

Full brief к include:
- A: Each К-Lxx invariant verbatim + analyzer rule mapping rationale
- B: Roslyn analyzer SDK reference patterns (verbatim API surface per Lesson #7)
- C: Microsoft.CodeAnalysis.Testing fixture patterns
- D: First-run cleanup precedent (А'.5 cleanup cascade pattern application)

---

**End of skeleton.**

**Promotion к AUTHORED** triggers: К-closure report (А'.8) landed с Roslyn analyzer rule specification section authoritative. Skeleton к patch-brief mutation if К-closure report surfaces additional rules.

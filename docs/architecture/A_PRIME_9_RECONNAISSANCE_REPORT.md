---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-A_PRIME_9_RECONNAISSANCE_REPORT
category: A
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: "post-A'.9.1 closure"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-A_PRIME_9_RECONNAISSANCE_REPORT
---

# A'.9 Reconnaissance Report — Roslyn Analyzer Architecture Discovery

**Designation**: A'.9.0 Reconnaissance cascade output (К-extensions cascade #4 cross-reference)
**Milestone**: A'.9 Roslyn Architectural Analyzer (multi-cascade milestone)
**Authoring cascade**: A'.9.0 Reconnaissance — Brief `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md`
**Authoring date**: 2026-05-24
**Status at α0**: SKELETON — sections populated through cascade Phase α (α1–α4)
**Status at closure**: Live (post-A'.9.0 closure)

---

## §1 — Executive summary

*[To be populated в Phase α4 — synthesizes §3–§9 findings into 1–2 page high-level summary]*

---

## §2 — Reconnaissance scope + methodology executed

*[To be populated through cascade — documents actual reads performed, multi-agent dispatch usage if any, any deviations от brief specification, halt-condition triggers + their resolution]*

### §2.1 — Phase 0 anomalies surfaced

**Pre-existing analyzer artifacts** (deliberation agent's structural anchor missed these):

1. **`docs/architecture/ANALYZER_RULES.md`** v0.1 AUTHORED-SKELETON (created 2026-05-23 А'.8 К-closure cascade)
   - 18 active + 4 reserved rules already enumerated (DF001–DF019, DF003.1, DF007.1, DF015.1 + DF006/DF008/DF014/DF020 reserved)
   - Per-rule §2 specification template defined
   - Authority chain: K_CLOSURE_REPORT.md §7 canonical → ANALYZER_RULES.md encodes → analyzer implements
   - Forward к LOCKED at A'.9 milestone implementation
   - Implication for A'.9.0 recon: rule taxonomy NOT a discovery surface — already specified. Recon scores **analyzability + priority + rule shape refinement** против existing taxonomy.

2. **`tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md`** v0.1 AUTHORED-SKELETON (created 2026-05-17)
   - Predecessor analyzer brief skeleton — independent of A'.9.0 lineage
   - Anticipated project location: `tools/analyzers/DualFrontier.Analyzers/` (not `src/`)
   - Sub-milestones A9.A–E sketched (scaffolding → per-invariant rules → first-run cleanup → warning→error promotion → test infrastructure)
   - Implication for A'.9.0 recon: forms Brief A'.9.1 candidate skeleton; report §10 prerequisites should resolve conflict с this skeleton (supersede, merge, or revise).

3. **Suppression discipline already strong** (Phase 0 grep scan):
   - src/: 1 pragma (`NativeWorld.cs:526` CS0618 obsolete transition — legitimate)
   - tests/: 4 pragmas (CS0649 unused field в test fixtures — necessary for reflection-based fixtures)
   - SuppressMessage attribute: 0 occurrences across src/ tests/
   - GlobalSuppressions files: 0
   - Implication for Domain 7: governance recommendations build on a clean baseline, not remediation of existing debt.

4. **Build/CI surface bare**:
   - `.editorconfig` essentially empty (`root = true` + `charset = utf-8` only)
   - `Directory.Build.props`: `TreatWarningsAsErrors=true` already enforced, no analyzer references
   - No `.ruleset` files (no legacy code analysis config)
   - No `.github/workflows/*.yml` (no GitHub Actions CI — local-only build verification)
   - Implication for Domain 6: integration surface is "blank canvas" — A'.9.1 has full freedom для placement decisions.

---

## §3 — К-L invariants analyzability matrix

*[To be populated в Phase α1 (Domain 1) — 21 K-L scored per S-LOCK-4 rubric]*

| К-L | Statement excerpt | Enforcement | Tier | Priority | Rule ID | Notes |
|---|---|---|---|---|---|---|
| K-L1 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L2 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L3 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L3.1 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L4 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L5 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L6 | [TBD — SUPERSEDED] | — | — | — | DF006 reserved | Per K_CLOSURE §7.3 |
| K-L7 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L7.1 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L8 | [TBD] | [TBD] | [TBD] | [TBD] | DF008 reserved | [TBD] |
| K-L9 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L10 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L11 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L12 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L13 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L14 | [TBD] | [TBD] | [TBD] | [TBD] | DF014 reserved | Meta-invariant |
| K-L15 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L15.1 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L16 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L17 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L18 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L19 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |
| K-L20 | [TBD] | [TBD] | [TBD] | [TBD] | DF020 reserved | Post-Mod API lock |
| K-L21 | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |

### §3.X — Detailed К-L analysis (per К-L expansion)

*[Per-К-L expanded analysis где matrix row insufficient — populated в Phase α1]*

---

## §4 — FORMALIZE Lessons analyzability matrix

*[To be populated в Phase α1 (Domain 2) — 12 FORMALIZE Lessons scored per S-LOCK-4 rubric]*

| Lesson | Statement excerpt | Tier | Priority | Rule shape | Notes |
|---|---|---|---|---|---|
| [TBD] | [TBD] | [TBD] | [TBD] | [TBD] | [TBD] |

### §4.X — Provisional Lessons analyzability assessment (bonus section)

*[12 Provisional Lessons brief scoring — flag near-promotion candidates с analyzer relevance]*

---

## §5 — Cascade #2 + #3 surfaced rule candidates

*[To be populated в Phase α1 (Domain 3)]*

### §5.1 — Cascade #2 candidates (К-extensions cascade #2 — Godot deprecation)

*[Extracted from K_EXT_2_GODOT_DEPRECATION_BRIEF.md §6 forward consideration]*

### §5.2 — Cascade #3 candidates (К-extensions cascade #3 — Launcher visual implementation)

*[Extracted from K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md §6 forward consideration]*

---

## §6 — Mod OS К-L20 prep surface

*[To be populated в Phase α3 (Domain 4) — depends on Domain 1 results]*

### §6.1 — К-L20 statement (from KERNEL Part 0)

*[Verbatim quote]*

### §6.2 — Mod API restrictions analyzer-enforceable

*[Per Domain 4 methodology output]*

### §6.3 — A'.9-era preparatory rules (helping К-L20 era)

*[Forward-planning insights]*

---

## §7 — Roslyn ecosystem state

*[To be populated в Phase α2 (Domain 5) — desk research]*

### §7.1 — Current Roslyn SDK version

*[NuGet package + version + release date]*

### §7.2 — Test framework recommendations

*[Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit vs alternatives]*

### §7.3 — Severity policy precedents

*[Examples from Roslyn analyzers / ASP.NET Core / EF Core / etc.]*

### §7.4 — Code-fix provider patterns + adoption recommendation

*[Patterns + recommended scope для A'.9.1]*

---

## §8 — Build/CI integration surface

*[To be populated в Phase α2 (Domain 6)]*

### §8.1 — sln structural integration points

*[Findings — DualFrontier.sln has 12 src/ projects + 30+ test/fixture projects per Phase 0 inventory]*

### §8.2 — Directory.Build.props integration recommendation

*[Per Phase 0: TreatWarningsAsErrors=true already; no analyzer references; recommended insertion point]*

### §8.3 — .editorconfig severity override surface

*[Per Phase 0: file essentially empty — full freedom для rule severity convention]*

### §8.4 — CI integration trigger recommendation

*[Per Phase 0: no .github/workflows — local-only build verification; analyzer runs on `dotnet build` automatically OR opt-in?]*

---

## §9 — Suppression governance precedent + recommendations

*[To be populated в Phase α2 (Domain 7)]*

### §9.1 — Existing suppression patterns inventory

*[Per Phase 0 grep: 1 src pragma + 4 test pragmas + 0 SuppressMessage + 0 GlobalSuppressions]*

### §9.2 — Suppression governance recommendations

*[When allowed / CAPA tracking / review cadence — building on clean baseline]*

---

## §10 — Brief A'.9.1 prerequisites

*[To be populated в Phase α4 — per S-LOCK-8 mandatory enumeration]*

Decisions Brief A'.9.1 deliberation must ratify (based on this report):

1. **Rule prioritization batch для A'.9.1**: *[P0 candidate set]*
2. **Analyzer project structure**: *[location confirmation — `src/DualFrontier.Analyzers/` vs `tools/analyzers/DualFrontier.Analyzers/` per A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md skeleton; csproj config; dependencies]*
3. **Test framework choice**: *[recommendation]*
4. **Severity policy**: *[per-rule severity assignment rules]*
5. **Suppression policy**: *[when allowed, CAPA tracking, governance protocol]*
6. **Build/CI integration trigger**: *[analyzer runs on `dotnet build` automatically OR opt-in?]*
7. **A'.9 cascade decomposition refinement**: *[A'.9.2/A'.9.3/... initial scope based on rule sequencing]*
8. **К-L20 Mod API lock timing**: *[forward path]*
9. **Disposition of A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md skeleton**: *[supersede / merge / revise — Phase 0 surfaced this predecessor; needs explicit decision]*
10. **Disposition of ANALYZER_RULES.md AUTHORED-SKELETON**: *[continue к LOCKED per existing forward plan; ensure A'.9.1 implementation actualizes the §2 template populations]*

---

## §11 — Open questions for Brief A'.9.1 deliberation (Q-K candidates)

*[To be populated в Phase α4 — per S-LOCK-9 mandatory]*

---

## §12 — Cross-references

### §12.1 — Source documents read

*[Populated через cascade execution log]*

### §12.2 — Briefs

- **Predecessor cascade**: `tools/briefs/K_EXT_3_LAUNCHER_VISUAL_IMPLEMENTATION_BRIEF.md` (cascade #3 EXECUTED + PUSHED 2026-05-24)
- **This cascade**: `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md`
- **Predecessor analyzer skeleton**: `tools/briefs/A_PRIME_9_ROSLYN_ANALYZER_BRIEF.md` (AUTHORED-SKELETON 2026-05-17 — disposition pending Brief A'.9.1 deliberation)
- **Successor**: `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` (к be authored post-A'.9.0 closure against this report)

### §12.3 — Authoritative artifacts

- `docs/architecture/KERNEL_ARCHITECTURE.md` — К-L invariants canonical (Part 0)
- `docs/methodology/METHODOLOGY.md` — Lessons (FORMALIZE + Provisional)
- `docs/architecture/K_CLOSURE_REPORT.md` §7 — analyzer rule specifications canonical
- `docs/architecture/ANALYZER_RULES.md` AUTHORED-SKELETON v0.1 — rule taxonomy already enumerated
- `docs/architecture/K_EXTENSIONS_LEDGER.md` — cascade narratives (§3.5 entry pending β1)
- `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` — К-L14 evidence ledger (#13 entry pending β2)
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` — Phase A' chronological timeline (A'.9.0 entry pending β1)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.11 LOCKED — К-L20 prep surface source
- `docs/architecture/VULKAN_SUBSTRATE.md` v1.1 LOCKED — substrate К-L crosscut
- `docs/governance/REGISTER.yaml` — governance SoT (register_version bump 2.5 → 2.6 pending β3)

---

*End of A_PRIME_9_RECONNAISSANCE_REPORT.md skeleton — Phase α0*
*Sections §1–§11 populated through cascade Phase α1–α4 per brief §4 specification*

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-PROJECT_AXIOMS
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-25
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-PROJECT_AXIOMS
---
# DualFrontier Project Axioms

*Foundational axioms codifying the project framing distinct from K-L architectural invariants. PA-001..PA-004 are the framing axioms that drive scope decisions in every cascade.*

*Version: 1.0 (2026-05-25). Initial codification at A'.9.1 / К-extensions cascade #5 Phase α Commit 8. Status: AUTHORITATIVE LOCKED.*

*Authored under agent-as-primary-reader assumption per Q-A07-6 lock 2026-05-10 (PA-001 anchor itself). Anchor references verified against codebase reality at Phase 0 Task 3 (per [A_PRIME_9_1_PHASE_0_CLOSURE_REPORT.md §2.3](../../tools/briefs/A_PRIME_9_1_PHASE_0_CLOSURE_REPORT.md)).*

---

## §0 — Purpose + framing

PROJECT_AXIOMS.md codifies four foundational axioms that drive scope, priority, and trade-off decisions across the DualFrontier project. These axioms are **framing axioms** — distinct from К-Lxx architectural invariants (which describe what the substrate IS) and from FRAMEWORK governance schema (which describes how the project's documents and decisions are structured).

**Three-document governance trio** at `docs/governance/`:
- [FRAMEWORK.md](./FRAMEWORK.md) — schema and protocol для documentation governance (Q-A45-X5 post-session protocol, sync_register.ps1 validation gates, Category × Tier × Lifecycle taxonomy)
- [SYNTHESIS_RATIONALE.md](./SYNTHESIS_RATIONALE.md) — provenance: source-standard borrowings (DO-178C / ISO 9001 / ISO 26262 / IEC 61508 / FDA 21 CFR Part 11) and synthesis justification
- **PROJECT_AXIOMS.md** (this document) — foundational framing axioms that justify why the framework exists в its current shape AND drive cascade scope decisions

**Authority chain**: PROJECT_AXIOMS is Tier 1 LOCKED. Amendments require the FRAMEWORK §7.2 Tier 1 LOCKED amendment protocol (Crystalka direction + documented deliberation + lifecycle transition through AUTHORED → LOCKED). Amendment к any axiom MUST surface к Crystalka for explicit ratification — these are NOT default-evolving documents.

**Application surface**: Every Q-N / Q-K / Q-L deliberation at each cascade should reference applicable PA axioms as anchors. Cascade briefs cite axioms when explaining defer/drop/include decisions (precedent: Brief A'.9.1 §0.2 «Project axioms applied» enumerates PA-001..PA-004 anchors throughout the cascade's 17 Q-L ratifications).

---

## §1 — PA-001 — AI-agent-first consumer profile (PERMANENT)

### §1.1 — Axiom statement

**PA-001**: The primary and permanent consumer profile для DualFrontier project artifacts (code, documentation, tooling, briefs, governance) is **AI agents operating in session-mode pipelines**, not human developers reading sequentially. Human review remains essential at decision points (Crystalka deliberation, ratification gates) but the *bulk consumer* is the AI agent.

### §1.2 — Anchors

**Verification** (Phase 0 §2.3 codebase reality check):
- [FRAMEWORK.md §0 / line 21](./FRAMEWORK.md): «Authored under agent-as-primary-reader assumption per Q-A07-6 lock 2026-05-10»
- [SYNTHESIS_RATIONALE.md §0](./SYNTHESIS_RATIONALE.md): «Audience: agent-primary (Q-A07-6 inheritance)»
- Q-A07-6 LOCKED 2026-05-10 — initial codification of agent-as-primary-reader at A'.4.5 closure

### §1.3 — Why PERMANENT (not provisional)

Audience evolution would require:
- Community emergence (third-party developers consuming the project as a library / forking the engine)
- OR commercial product surface (end-user installations of the simulation)
- OR multi-developer team formation (more than the solo-developer Crystalka)

None of these are roadmap items. The decade-horizon planning context (per PA-003) assumes solo-developer + AI-pipeline indefinitely. Amendment would require explicit surface к Crystalka before any Q-L deliberation can default-include a human-IDE-workflow concession.

### §1.4 — Cascade applications

**Brief A'.9.1 anchors**:
- **Q-L-15**: Code-fix providers PERMANENTLY DROPPED. Code-fix providers serve human IDE workflow (click-to-fix in Visual Studio / Rider light-bulb). AI agents read diagnostic text directly and apply edits via Edit tool — code-fix providers are dead weight для AI-agent-first profile. Diagnostic message quality elevated к compensate (rich text guiding the agent к the edit).
- **Q-L-13**: PublicApiAnalyzers entirely DEFERRED audience-driven. Community ecosystem absent (PA-001 anchor). All candidate assemblies are either internal-only OR Mod API surface volatile pre-К-L20 LOCK.
- **Lesson #N17 Provisional** (audience-driven tooling deferral): formal codification of «tooling serving a specific consumer audience ships only when that audience materializes» — PA-001 is the «current audience profile» specification.

---

## §2 — PA-002 — Без костылей (no shortcuts)

### §2.1 — Axiom statement

**PA-002**: Architectural complexity is acceptable when it serves correctness, falsifiability, or long-horizon maintainability. Shortcuts that produce «works for now» / «we'll fix later» artifacts are forbidden — they accumulate as kostyl debt that corrupts the architectural integrity tracked by К-L14.

Verbatim Crystalka direction 2026-05-24: «Ни каких костылей, сложность архитектуры всегда оправдана».

### §2.2 — Anchors

**Verification** (Phase 0 §2.3 codebase reality check):
- Brief A'.9.1 §0.2 Crystalka direction verbatim
- [A_PRIME_9_0_AMENDMENTS_LOG.md §3.2](../architecture/A_PRIME_9_0_AMENDMENTS_LOG.md): «honest scoping» rationale для 5-rule deferral к К-L20 LOCK cascade (Mod-OS-coupled rules deferred rather than pre-emptively shipped against moving target)
- [METHODOLOGY.md](../methodology/METHODOLOGY.md) Lessons #N12 sub-pattern A + B (defensive throws + silent stubs both ARE acceptable architectural patterns when marked + justified — the kostyl is the *unmarked* shortcut)

### §2.3 — Detection patterns

A kostyl violates PA-002 when:
1. It works around a problem instead of solving the root cause
2. It leaves rotting artifacts (TODO/FIXME without activation triggers, defensive throws without [ReservedStub] markers, silent stubs без architectural justification)
3. It pre-emptively enforces architecture against a moving target (e.g., pre-emptive Mod API analyzer rules pre-К-L20 LOCK)
4. It generates lying tests (Lesson #25 refined — tests that pass but assert nothing meaningful)

Accepted complexity (NOT kostyl) when:
1. It serves K-Lxx invariant preservation
2. It serves K-L14 falsifiability tracking
3. It documents the «why» so future agents can judge edge cases
4. It uses marker infrastructure ([ReservedStub], [Trait], etc.) to make the architectural state visible

### §2.4 — Cascade applications

**Brief A'.9.1 anchors**:
- **Q-L-9**: DFK010 PERMANENTLY DROPPED. К-L10 governs decision attribution at the document/methodology layer, not the code layer. A Roslyn rule pretending к enforce methodology-layer reasoning would be a kostyl — code-layer marker masquerading doc-layer semantics. Alternative enforcement: FRAMEWORK + METHODOLOGY documentation discipline only.
- **Q-L-11**: DFC001 DEFERRED к К-L20 LOCK cascade. Bridge surface is Mod-API-coupled; pre-К-L20 LOCK enforcement against a moving target = kostyl pattern. К-L20 LOCK cascade activates the entire deferred set atomically.
- **Q-L-13**: PublicApiAnalyzers DEFERRED audience-driven. Community ecosystem absent — pre-emptive enforcement of public-API stability against an undefined audience would be kostyl.
- **Commit 1 ProjectReference drop**: Brief §6.1 specified an analyzer→Contracts ProjectReference; framework compatibility prevented it. The kostyl path would have been overriding framework checks. The PA-002-compliant path was к drop the ProjectReference entirely (Roslyn analyzers detect attributes by symbol name via SemanticModel — no compile-time CLR reference needed).

---

## §3 — PA-003 — Сложность архитектуры всегда оправдана (long-horizon over efficiency)

### §3.1 — Axiom statement

**PA-003**: When trading off between architectural simplicity (easier к understand at glance) and architectural soundness (правильный across long-horizon evolution), DualFrontier chooses soundness. The project optimizes для decade-horizon maintainability per the solo-developer + AI-pipeline context, not для short-term shipping efficiency.

### §3.2 — Anchors

**Verification** (Phase 0 §2.3 codebase reality check):
- [FRAMEWORK.md §0 / line 15](./FRAMEWORK.md): «Bespoke framework fitted к solo-developer + AI-pipeline + decade-horizon planning context»
- [K_EXTENSIONS_LEDGER.md §3.1](../architecture/K_EXTENSIONS_LEDGER.md): К-extensions cascade #1 К-L15.1 three-tier mutex split — managed-side mutex partitioning into Layer 1 / Layer 2 / Layer 3 chosen for compile-time guarantees over runtime simplicity (long-horizon correctness over short-term ergonomics)
- Brief A'.9.1 §0.2 framing: «Ни каких костылей, сложность архитектуры всегда оправдана, так как проект на долгие горизонты»

### §3.3 — Trade-off framing

When facing a complexity vs simplicity choice:
- Ask: «does the simpler option produce hidden coupling that will require unwinding в 6 months?» — if yes, choose the complex option
- Ask: «does the complex option document an architectural decision worth surfacing?» — if yes, choose the complex option (the complexity IS the documentation)
- Ask: «does the simpler option require a shortcut to work?» — if yes, that's a PA-002 violation; choose the complex non-shortcut option

### §3.4 — Cascade applications

**Brief A'.9.1 anchors**:
- **Q-L-3 tiered namespaces**: DFK### / DFL### / DFC### tier separation chosen over flat namespace. Tier separation enables forward-cascade reasoning («which rules are К-L driven vs Lesson driven vs cascade-specific?») at the cost of slightly more naming surface.
- **Three-graph + two-scheduler architecture**: native SystemGraph + managed DependencyGraph + mod manifest dependency graph + native scheduler + managed dispatcher. Simpler architectures (single scheduler / single graph) were considered and rejected because they would have collapsed substrate boundaries (К-L11 storage backbone, К-L12 native scheduler sovereignty, К-L9 mod parity).
- **К10 cascade К-L12 establishment**: native scheduler sovereignty chosen over managed scheduler authority. More complex (managed-side facade adapter layer) but soundness-preserving over decade-horizon mod-API forward-compat.

---

## §4 — PA-004 — К-L14 thesis preservation

### §4.1 — Axiom statement

**PA-004**: Every cascade preserves the К-L14 meta-invariant — «Performance derives from clean complex architecture: substrate minimal; falsifiability tracked through defect rate, architectural integrity, pipeline economics». Cascades that add substrate (vs add governance, tooling, or discipline) must surface explicitly as К-L change cascades (К-extensions cascades). Cascades that preserve substrate must document zero-substrate-touch in commit messages and closure reports.

### §4.2 — Anchors

**Verification** (Phase 0 §2.3 codebase reality check):
- [KERNEL_ARCHITECTURE.md Part 0 K-L14](../architecture/KERNEL_ARCHITECTURE.md): canonical text verbatim
- [K_L14_EVIDENCE_DASHBOARD.md](../architecture/K_L14_EVIDENCE_DASHBOARD.md): cumulative К-L14 evidence ledger — 13 evidence entries pre-A'.9.1 (cascade #11 first removal-type evidence; cascade #12 first clean additive evidence; cascade #13 first observational baseline evidence; pre-cascade additional entries from K10/A'.7.x/A'.8 cycles)
- К-L14 Evidence #14 expected at A'.9.1 closure — first analyzer implementation evidence (Type 6 NEW category — tooling addition).

### §4.3 — Substrate vs tooling vs governance taxonomy

Per К-L14 thesis, every cascade falls into one of three categories:

**Substrate cascade** (rare — every substrate change tracked as К-extensions cascade):
- Adds OR refines OR removes К-Lxx invariant text
- Requires explicit deliberation (Q-L locks, brief authoring, closure protocol)
- К-L14 Evidence tracks the change rationale + falsifiability commitment

**Tooling cascade** (common — analyzer infrastructure, build automation, test framework):
- Adds tooling that enforces existing substrate
- No К-Lxx text change
- К-L14 Evidence framework continues unmodified (this cascade itself is K-L14-positive — tooling reduces drift surface without expanding substrate)
- Brief A'.9.1 is the canonical example — analyzer infrastructure addition

**Governance cascade** (regular — REGISTER updates, document version bumps, methodology lessons codification):
- Adds OR updates governance documents
- No К-Lxx text change OR substrate touch
- К-L14 Evidence framework continues unmodified
- Phase α Commits 8 + 9 of Brief A'.9.1 are the canonical examples

### §4.4 — Cascade applications

**Brief A'.9.1 anchors**:
- **Substrate minimality discipline**: Brief A'.9.1 is a tooling cascade adding 17 analyzer rules that enforce existing К-Lxx invariants. Zero new substrate. Zero К-Lxx text change. К-L14 evidence #14 = first analyzer implementation evidence (Type 6 NEW category).
- **Every commit message references К-L14 thesis preservation**: «K-L14 thesis preservation: tooling addition, zero substrate touch» (or governance equivalent). This is the discipline by which substrate minimality is empirically verified at every commit boundary.
- **Phase α exit gate**: zero functional code semantic change verified via dotnet build + dotnet test exit 0 — production code receives [ReservedStub] attribute metadata but no behavioral change.

---

## §5 — Axiom amendment protocol

PROJECT_AXIOMS.md is Tier 1 LOCKED. Amendments к any PA-N axiom require:

1. **Surface к Crystalka** explicitly — the agent cannot default-amend an axiom; the amendment must be a session-level direction
2. **Documented deliberation** — Q-L (or new Q-PA) lock с rationale + anchor-impact analysis
3. **Lifecycle transition** — PROJECT_AXIOMS.md goes through PATCH bump (PA refinement) OR MAJOR bump (PA replacement) per FRAMEWORK §7.1 versioning ruleset
4. **REGISTER cascade** — version bump + audit_trail event + last_modified_commit
5. **Cross-document propagation** — FRAMEWORK + SYNTHESIS_RATIONALE updated if amendment touches their cross-reference surface

Per FRAMEWORK §7.2 Tier 1 LOCKED amendment protocol. Amendment к PA-001 in particular (the «PERMANENT» qualifier) has the highest bar — community emergence OR commercial product surface OR multi-developer team formation must be empirical reality, not roadmap aspiration.

---

## §6 — Cross-references

### §6.1 — Sibling governance documents

- [FRAMEWORK.md](./FRAMEWORK.md) — documentation governance schema. PROJECT_AXIOMS provides framing; FRAMEWORK provides protocol.
- [SYNTHESIS_RATIONALE.md](./SYNTHESIS_RATIONALE.md) — synthesis provenance. PROJECT_AXIOMS provides motivation (why the framework exists в its current shape); SYNTHESIS_RATIONALE provides derivation (which industry standards inform the framework).

### §6.2 — Architectural authority

- [KERNEL_ARCHITECTURE.md Part 0](../architecture/KERNEL_ARCHITECTURE.md) — К-Lxx invariants (substrate authority). PA axioms are framing — К-Lxx are substrate.
- [K_L14_EVIDENCE_DASHBOARD.md](../architecture/K_L14_EVIDENCE_DASHBOARD.md) — К-L14 cumulative evidence ledger anchored by PA-004.

### §6.3 — Methodology

- [METHODOLOGY.md](../methodology/METHODOLOGY.md) — Lessons codify discipline derived от PA axiom applications. Recent Lesson #N17 Provisional (audience-driven tooling deferral) is the formal codification of PA-001 application к tooling deferral patterns.

### §6.4 — Brief A'.9.1 (initial application)

- [tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md](../../tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md) §0.2 — first explicit «Project axioms applied» framing reference в cascade brief
- [tools/briefs/A_PRIME_9_1_PHASE_0_CLOSURE_REPORT.md](../../tools/briefs/A_PRIME_9_1_PHASE_0_CLOSURE_REPORT.md) §2.3 — anchor verification (codebase reality check)
- [docs/architecture/ANALYZER_RULES.md](../architecture/ANALYZER_RULES.md) — PA anchors documented inline в §7 (PA-002 outside Roslyn scope) + §8 (PA-001 audience-driven)

---

**End of PROJECT_AXIOMS.md v1.0 LOCKED**

**Forward maintenance**: PA-001..PA-004 are foundational axioms. Amendment requires Tier 1 LOCKED amendment protocol per FRAMEWORK §7.2. New PA-N additions follow same protocol (PROJECT_AXIOMS MINOR version bump). Anchor references к codebase reality should be re-verified at each MINOR-bump amendment via Phase 0 §2.3-style codebase reality audit.

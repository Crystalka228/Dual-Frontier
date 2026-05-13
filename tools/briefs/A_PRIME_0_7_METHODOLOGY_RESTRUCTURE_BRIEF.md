---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-A_PRIME_0_7_METHODOLOGY_RESTRUCTURE
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-A_PRIME_0_7_METHODOLOGY_RESTRUCTURE
---
# A'.0.7 — Methodology pipeline restructure (architectural deliberation session brief)

**Brief type**: Architectural decision brief (fourth brief type, precedent K8.0 / K-L3.1)
**Authored**: 2026-05-10 (Opus 4.7, post-A'.0.5 closure 4e332bb)
**Phase A' position**: A'.0.7 — between A'.0.5 (DONE 4e332bb) and A'.1 (pending)
**Target session**: Crystalka + Claude Desktop session, deliberation mode (no code execution)
**Estimated session length**: 3–5 hours (full Q-survey + synthesis + amendment plan authoring)
**Status**: EXECUTED 2026-05-10 — Phase 0 reads + Phase 1 deliberation Q1–Q12 + 3 surfaced auxiliary Q-A07-6/7/8 + Phase 2 synthesis form choice (§4.A-primary с §4.C для PIPELINE_METRICS) + Phase 3 amendment plan authoring complete. Amendment plan at `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` is executable artifact; AMENDMENTS LANDED 2026-05-10 via A'.1.M execution session (commits M-1 through M-4).
**Locks** (session 2026-05-10): Q-A07-6 audience contract (agent-as-primary-reader); Q-A07-7=b (defer К-L3.1 lesson к A'.8); Q-A07-8=c (inline §6 forward measurement plan в PIPELINE_METRICS); Q1=b (abstract primary + footnote); Q2=α (structural rewrite + current config table); Q3=c-reformulated (three-properties mechanism); Q4=b-reformulated (economic invariant + A'.0.5 anchor); Q5=c-decomposed (parallel case studies); Q6=α-b (§5 substantial + §6 verify clean); Q7=a-table (per-section judgment); Q8=c-formulation (Phase A' lessons sub-section); Q9=b (verify clean Native adjustments); Q10=a-with-standardized-labels (5 labels + per-metric annotations + v0.1→v0.2); Q11=table (per-sub-section MAXIMUM_ENGINEERING_REFACTOR dispositions); Q12=c-formulation (README hybrid + audience contract). Synthesis = §4.A-primary с document-specific §4.C для PIPELINE_METRICS.
**Prerequisite**: A'.0.5 closure (commit `4e332bb` on main, deferral markers in place on METHODOLOGY/PIPELINE_METRICS/MAXIMUM_ENGINEERING_REFACTOR/README)
**Blocks**: A'.1 amendment brief execution, A'.3 push to origin, A'.4-A'.7 K-series execution
**Does not block**: nothing (everything sequential)

---

## §1 Why this session exists

### §1.1 Drift surfaced cumulatively across recent sessions

Between K-L3.1 closure (45d831c, 2026-05-10) and A'.0.5 closure (4e332bb, 2026-05-10), Crystalka surfaced multiple architectural shifts that invalidate METHODOLOGY.md v1.5's pipeline framing:

1. **Pipeline restructure** declared 2026-05-10:
   - «Всё делается через десктопное приложение Claude»
   - «Удалить использование в документах упоминание о локальной модели Gemma, я её не использую»
   - «Если сессии Claude Code идут через брифы и документы это очень дешево и влезает всё в лимиты так как не требуется обдумывание, когда всё решено через доки»

2. **Empirical evidence from A'.0.5 closure**: single-session pipeline collapsed A'.2 → A'.0.5 Phase 5 cleanly; milestone-splitting overhead unnecessary for documentation work under unified pipeline.

3. **Architectural decision-recording precedent established** by K-L3.1: high-stakes architectural framing decisions get formal deliberation session, amendment plan, follow-up execution — not improvised inline.

4. **Crystalka discipline declaration**: «Без костылей у меня много времени, а также требуется архитектурная чистота, чтобы проект жил десятилетиями».

These four facts compose into: **METHODOLOGY.md substantive rewrite required, executed under deliberation discipline analog K-L3.1, not improvised mechanical scrub**.

### §1.2 What A'.0.5 set up

A'.0.5 Phase 7 (pipeline-terminology scrub) deliberately **did not** rewrite METHODOLOGY substantive sections. Instead, A'.0.5 placed HTML deferral markers on:

- `docs/methodology/METHODOLOGY.md` — substantive sections §0/§2.1/§2.2/§3/§4/§5/§8 preserved verbatim with marker
- `docs/methodology/PIPELINE_METRICS.md` — entire empirical record preserved verbatim with marker
- `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` — 4-agent sub-sections preserved with marker
- `README.md` (top-level) — Pipeline section preserved with marker

A'.0.7 is the deliberation session that **resolves these deferrals**. Output is amendment plan; amendments themselves are A'.1 scope (or possibly separate follow-up brief depending on synthesis).

### §1.3 What this session deliberately is and is not

**Is**: deliberation session — Q-survey, alternatives considered, decisions locked, amendment plan authored.

**Is not**:
- Code changes (zero)
- Test changes (zero, baseline 631 preserved)
- Direct edits to deferred docs (deferral markers stay; A'.1 or follow-up executes amendments)
- New milestones authored (A'.1 already scoped, A'.0.7 doesn't expand it)
- Architectural decisions outside methodology framing (out of scope; raise separate session if needed)

### §1.4 Locked answers from pre-authoring deliberation (2026-05-10)

Before this brief was authored, Opus surveyed 5 framing questions Q-A07-1 through Q-A07-5 and Crystalka accepted Opus recommendations. Locked defaults:

- **Q-A07-1 = α**: pure deliberation session (analog K-L3.1), not hybrid execution
- **Q-A07-2 = β**: mixed disposition per section — some current-pipeline rewrite, some preserved historical, some discarded per-section judgment
- **Q-A07-3 = β+γ**: PIPELINE_METRICS preserved as historical, per-metric annotation reassessment
- **Q-A07-4 = γ**: falsifiable claim **generalized** to architect/executor abstract framing, not tied to specific agent count
- **Q-A07-5 = a**: A'.0.5 lesson #1 (single-session pipeline economics) formalized into methodology

These are session entry preconditions, not deliberation surface. A'.0.7 deliberates the 12 design questions in §3 below; locked defaults frame the design space.

---

## §2 Pre-flight read inventory (Phase 0)

The session begins with the Opus instance reading these documents end-to-end before deliberation. The session does not deliberate against memory; it deliberates against disk truth.

### §2.1 Mandatory full reads

1. **`docs/methodology/METHODOLOGY.md` v1.5** (with A'.0.5 deferral markers in place) — full document. Particular attention:
   - §0 Abstract — falsifiable claim, 4-agent framing
   - §1 Problem statement — pipeline-agnostic vs pipeline-specific content
   - §2.1 Role distribution — 4 agents (Opus, Sonnet, Gemma, Crystalka) with specific technical claims
   - §2.2 Contracts as IPC — IPC framing between agents
   - §2.3 Stop / escalate / lock — pipeline-agnostic, likely survives untouched
   - §2.4 Atomicity of phase review — pipeline-agnostic, likely survives
   - §3 Economics — cost claims tied to specific agent tiers
   - §4 Throughput — measurements, possibly Gemma-era data
   - §5 Boundaries — what methodology refuses to do
   - §7 Native layer adjustments — architectural lessons, not pipeline-specific, survives
   - §8 (if exists) — scope check
   - K-Lessons sub-section — architectural lessons (atomic commit unit, Phase 0.4 hypothesis, mod-scope test isolation, error semantics convention) — pipeline-agnostic, survives untouched

2. **`docs/methodology/PIPELINE_METRICS.md`** (with A'.0.5 deferral markers in place) — full document. Particular attention:
   - Top-of-document frame
   - Per-metric tables and definitions
   - Empirical record sections with timestamps
   - Trend analyses if present
   - Conclusions or claims tied to specific metric values

3. **`docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md`** (with deferral markers) — full document. Particular attention:
   - 4-agent sub-sections specifically
   - Tracks A and B framing (analyzer track is post-K-closure milestone — connects to A'.9)

4. **`README.md`** (top-level, with Pipeline section deferral marker) — full document. Particular attention:
   - Pipeline section
   - Project description tying to specific methodology claims

5. **`docs/architecture/PHASE_A_PRIME_SEQUENCING.md`** (relocated to docs/architecture/ in A'.0.5 Phase 3) — full document. Particular attention:
   - §A6 «Provenance» mentions Cloud Code as executor — verify alignment with A'.0.7 outcome
   - Implicit pipeline assumptions throughout

### §2.2 Mandatory partial reads

6. **K-L3.1 brief** (`tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`) — re-read §1, §3, §4, §6 only. Purpose: confirm A'.0.7 brief structure matches K-L3.1 precedent for architectural decision brief shape.

7. **K-L3.1 amendment plan** (`docs/architecture/K_L3_1_AMENDMENT_PLAN.md`) — re-read §3.8 (Phase A' integration) and §5 (amendment plan structure). Purpose: confirm A'.0.7 amendment plan output format matches K-L3.1 precedent.

8. **MIGRATION_PROGRESS A'.0.5 entry** — read full entry. Particular attention:
   - Lessons learned section (lesson #1 specifically — single-session pipeline collapse evidence)
   - Tier 2 flags forwarded
   - Items forwarded to A'.0.7

### §2.3 Optional reads (at deliberator discretion)

- **`docs/IDEAS_RESERVOIR.md`** — if pipeline architecture surfaces in idea drafts
- **`docs/ROADMAP.md`** — if pipeline-architecture-related roadmap items exist
- **Recent commit messages** (`git log --oneline 45d831c..4e332bb`) — for empirical sense of session work product under unified pipeline

### §2.4 Verification checklist

After reads, deliberator confirms understanding of:

- [ ] Current 4-agent framing in METHODOLOGY (specific claims that need replacement vs preservation vs discard)
- [ ] PIPELINE_METRICS empirical record extent and specific values
- [ ] MAXIMUM_ENGINEERING_REFACTOR positioning of 4-agent vs analyzer-track work
- [ ] Cross-reference impact of methodology changes (which docs reference pipeline framing externally)
- [ ] What survives untouched (K-Lessons sub-section, native layer adjustments, stop/escalate/lock pattern)
- [ ] What's pipeline-specific and needs handling (§2.1 role distribution, §3-§5 economics/throughput/boundaries, possibly §0 framing)

If any check unclear after reads — deliberation pauses, additional reads taken before Phase 1 begins.

---

## §3 The 12 design questions (Phase 1 deliberation surface)

Each question gets independent treatment in session. Alternatives are formulated to be **mutually exclusive within question** but **jointly compatible across questions**. Locked defaults from §1.4 frame design space (e.g., Q-A07-2 mixed disposition is locked; per-section-level dispositions still need deliberation).

### §3.1 Q1 — §0 Abstract framing under generalized methodology (Q-A07-4 locked γ)

**Question**: §0 Abstract currently states (paraphrased): «4 agents with explicitly distributed roles: local quantized executor, cloud prompt generator, cloud architect+QA, human direction». Locked Q-A07-4=γ generalizes to architect/executor abstract framing. Specific replacement language?

**Alternatives**:

**Q1.a — Strict abstract («N agents in architect-executor split»)**:
> «The methodology configures N agents in an architect-executor split with rigid contracts at boundaries: human as direction owner, one or more LLM instances as architect (deliberation, brief authoring, QA review) and executor (mechanical application against authored briefs). Specific N and tier mix vary by session economics; the architect-executor split with contracts at boundaries is invariant.»

Pro: maximally robust к future pipeline restructures. Captures K8.0/K-L3.1/A'.0.7 deliberation-recording principle.
Con: abstract enough that reader unfamiliar with project may not grasp practical configuration without specific N=2 illustrative example.

**Q1.b — Abstract + current-configuration footnote**:
> «The methodology configures N agents in architect-executor split with rigid contracts at boundaries: human as direction owner, one or more LLM instances as architect and executor. [Footnote: at time of v1.6 authoring (2026-05-10), N=2 — Crystalka + unified Claude Desktop session in deliberation OR execution mode per session framing.]»

Pro: abstract framing primary, current configuration concrete in footnote. Reader gets both layers.
Con: footnote becomes stale at next pipeline pivot; requires versioning discipline.

**Q1.c — Methodology-version primary frame**:
> «v1.6 methodology (post-pipeline restructure 2026-05-10): architect-executor split with N=2 agents — Crystalka + unified Claude Desktop session. Earlier versions configured 4 agents (see PIPELINE_METRICS for empirical record of v1.x era). Architect-executor split with contracts at boundaries is invariant across versions; specific N is not.»

Pro: explicit methodology versioning, archaeological trail visible, current concrete.
Con: framing privileges current configuration; «architect-executor abstract» becomes secondary insight.

**Crystalka decides**: a/b/c with reasoning recorded in session log.

**Recommendation surface**: probably (b). Captures Q-A07-4=γ abstraction primary, current concrete in footnote, clean versioning discipline. (a) too abstract for first-time readers, (c) buries the architectural insight.

### §3.2 Q2 — §2.1 Role distribution rewrite

**Question**: §2.1 currently enumerates 4 specific roles (Opus, Sonnet, Gemma, Crystalka) with detailed technical claims about each. Under Q-A07-2=β mixed disposition + Q-A07-4=γ generalized framing, replacement structure?

**Alternatives**:

**Q2.α — Structural rewrite, abstract roles**:
- Section enumerates role categories abstractly: «direction owner» (human), «architect» (LLM in deliberation mode), «executor» (LLM in execution mode against briefs), no specific model tiers.
- Sub-section «current configuration» specifies Crystalka + unified Claude Desktop with mode-switching by session framing.
- Brief acknowledgment that earlier configurations split executor across multiple models.
Pro: aligns с Q-A07-4=γ generalized framing.
Con: loses some concreteness; readers may want specific model recommendations.

**Q2.β — Structural preservation, content rewrite**:
- Section structure preserved (role categories enumerated).
- Each role's content rewritten to current 2-agent reality.
- Historical 4-agent role mentions reframed as «v1.x era configuration» с frame note.
Pro: minimal structural disruption; existing readers find familiar shape.
Con: may inadvertently preserve 4-agent assumptions baked into structure.

**Q2.γ — Full rewrite from scratch**:
- §2.1 discarded, new section authored from scratch reflecting 2-agent reality + abstract framing.
- Historical 4-agent material preserved in §2.1.archive sub-section or separate `docs/methodology/HISTORY.md` document.
Pro: cleanest contemporary framing; archaeological trail still preserved separately.
Con: heaviest authoring; introduces new document if archive split chosen.

**Crystalka decides**: α/β/γ.

**Recommendation surface**: (α). Aligns with locked Q-A07-4=γ generalization. Per-role abstract framing + current-configuration sub-section gives both abstract principle and practical guidance.

### §3.3 Q3 — §2.2 Contracts as IPC framing

**Question**: §2.2 currently frames contracts as IPC «between agents» (model-tier boundary). Under unified pipeline, contracts are IPC «between sessions» (temporal boundary). Reframing approach?

**Alternatives**:

**Q3.a — Replace «between agents» with «between sessions»**:
- Mechanical-ish replacement: «contracts as IPC between agents» → «contracts as IPC between sessions».
- Surrounding prose updated for temporal-boundary framing (compaction, context loss between sessions, etc.).
Pro: minimal disruption; principle survives, framing updated.
Con: «between sessions» is current pipeline characterization; abstracts to «across context boundaries» better matches Q-A07-4=γ.

**Q3.b — Generalize to «across context boundaries»**:
- §2.2 framed: «contracts at context boundaries — whether boundaries are between agents (multi-model pipelines), between sessions (single-agent pipelines with context limits), or between human-deliberation phases (architecting vs executing modes)».
- Multiple boundary types enumerated; principle invariant across them.
Pro: aligns with Q-A07-4=γ; future-proof to other pipeline architectures.
Con: more abstract; readers may want concrete examples per boundary type.

**Q3.c — Two-layer framing**:
- Top layer: «contracts at context boundaries» (abstract).
- Sub-layers: examples of boundary types (agent boundaries, session boundaries, role-mode boundaries) with current-pipeline boundary type marked as primary.
Pro: abstract principle + concrete current configuration.
Con: longest authoring; section becomes substantive document.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (b). Abstract framing primary, concrete examples within prose. Matches Q-A07-4=γ + Q1.b style consistency.

### §3.4 Q4 — §3 Economics section disposition

**Question**: §3 contains cost claims tied to specific agent tiers (Gemma local cost, Sonnet API cost, Opus API cost). Under unified pipeline (single tier — Claude Desktop), specific multi-tier economics no longer applies. Disposition?

**Alternatives**:

**Q4.a — Discard §3 entirely, no replacement**:
- Section removed; methodology silent on economics.
Pro: simplest; avoids stale claims.
Con: economics is one of methodology's falsifiable claims (Crystalka declared «cheap executor sessions when all decided through docs»); removing forfeits that.

**Q4.b — Rewrite §3 with single-tier economics + Crystalka empirical claim**:
- New §3: «under unified pipeline, executor sessions are economical because architectural deliberation is decided in upfront docs/briefs; execution sessions don't require re-deliberation, fitting Cloud Code session limits».
- Cite A'.0.5 closure as empirical evidence: «19 atomic commits, ~250 cross-refs, +4354/-653 LOC, single session, baseline preserved».
- Falsifiable claim: «as long as architectural decisions captured in briefs upfront, executor cost scales с size of execution, not size of architectural surface».
Pro: replaces stale claim with current claim; preserves falsifiability.
Con: claim partially Crystalka-empirical, may need refinement before formal commitment.

**Q4.c — Preserve §3 historical + add §3.1 current**:
- Original §3 preserved as historical (multi-tier era economics).
- New §3.1 «Post-pipeline-restructure economics» captures current claim.
Pro: archaeological trail preserved; current claim tracked separately.
Con: methodology document grows; reader navigates two economics sections.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (b). Replace with current claim, cite A'.0.5 empirical evidence. Aligns with Q-A07-3=β+γ on PIPELINE_METRICS (preserve as historical), keeping methodology contemporary while metrics document carries history.

### §3.5 Q5 — §4 Throughput section disposition

**Question**: §4 contains throughput measurements (tasks/week, defect rate, etc.) gathered under 4-agent pipeline. Disposition under unified pipeline?

**Alternatives**:

**Q5.a — Discard §4 entirely**:
- Section removed; throughput claims forfeit.
Pro: simplest; no stale claims.
Con: same as Q4.a — forfeits falsifiability.

**Q5.b — Rewrite §4 with current claims, defer to PIPELINE_METRICS for empirical detail**:
- New §4 acknowledges throughput is methodology-load-bearing, points to PIPELINE_METRICS for current-era empirical record.
- Per Q-A07-3=β+γ: PIPELINE_METRICS preserves historical with reassessment notes.
Pro: methodology section minimal, metrics document carries detail.
Con: requires PIPELINE_METRICS treatment to be coherent (Q-A07-3 deliberation needed in this session).

**Q5.c — Hybrid: §4 contains principle + PIPELINE_METRICS contains data**:
- §4 frames «throughput is measured per-session, recorded in PIPELINE_METRICS, reassessed at each pipeline restructure».
- Current pipeline data points called out (A'.0.5 closure as one).
Pro: principle in methodology, data in metrics document, both layers working.
Con: same as Q5.b — needs PIPELINE_METRICS coherent.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (c). Principle-in-methodology + data-in-metrics is clean separation. Layered consistency.

### §3.6 Q6 — §5 Boundaries section disposition

**Question**: §5 enumerates what methodology refuses to do (e.g., «no improvised architectural decisions», «no skip atomic commit discipline»). These are pipeline-agnostic principles. Update needed?

**Alternatives**:

**Q6.a — §5 untouched** (pipeline-agnostic content survives):
Pro: minimum work.
Con: may have incidental 4-agent terminology even in pipeline-agnostic principles.

**Q6.b — §5 verified clean, terminology scrubbed**:
- Section read pass for incidental 4-agent terminology, scrubbed if found.
- Principle content preserved.
Pro: surfaces incidental staleness; preserves architectural principles.
Con: requires read pass during session.

**Q6.c — §5 expanded with pipeline-restructure-era principles**:
- Existing principles preserved + new principle: «no methodology rewrite without formal deliberation session» (citing A'.0.7 itself as precedent).
- New principle: «no claim methodology version stable without falsifiable empirical record» (citing PIPELINE_METRICS dependency).
Pro: methodology grows with project; A'.0.7 lesson formalized into discipline.
Con: section grows; new principles need careful authoring.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (b) at minimum, (c) if Crystalka wants to formalize A'.0.7 itself as methodology lesson. (b) safe default.

### §3.7 Q7 — §8 (if exists) disposition

**Question**: METHODOLOGY may have §8 or other sections beyond §5; pre-flight read clarifies. Disposition per-section per Q-A07-2=β mixed disposition framework.

**Alternatives** (per-section, deferred to deliberation after read):

- **Q7.a — Per-section judgment**: deliberator reads each section, assigns disposition (rewrite / preserve historical / discard / verify-clean) per content character.
- **Q7.b — Default preserve unless flagged**: sections not pipeline-specific → preserve untouched; only flagged sections get rewrite.

**Crystalka decides**: a/b after reading.

**Recommendation surface**: (a). Per-section judgment yields cleanest result; default-preserve risks incidental 4-agent assumptions surviving.

### §3.8 Q8 — K-Lessons sub-section disposition

**Question**: K-Lessons sub-section (atomic commit, Phase 0.4 hypothesis, mod-scope test isolation, error semantics convention) is architectural lessons content, not pipeline-specific. Disposition?

**Alternatives**:

**Q8.a — Untouched** (likely correct per Q-A07-2=β):
Pro: lessons survive without rewriting; principles invariant.
Con: any incidental terminology may need scrub.

**Q8.b — Verify clean, scrub if needed**:
- Read pass; scrub if 4-agent terminology found in lesson explanations.
- Principle content preserved.
Pro: defensive verification.
Con: minor extra work.

**Q8.c — Expand with A'.0.5 lesson #1** (per locked Q-A07-5=a):
- New lesson: «Single-session pipeline economics — milestone-splitting overhead unnecessary for documentation work; atomic discipline preserved through stop conditions, not milestone fragmentation».
- Empirical evidence cited from A'.0.5 closure (4e332bb).
- Decision rule for future authors: when split, when bundle.
Pro: A'.0.5 lesson formalized into permanent methodology guidance.
Con: requires careful authoring of decision rule; «documentation work» vs «code execution work» boundary needs precision.

**Crystalka decides**: a/b/c. Q-A07-5=a locks expansion intent; (c) is the implementation.

**Recommendation surface**: (c). Q-A07-5=a was locked specifically для this; not implementing it forfeits the locked decision.

### §3.9 Q9 — §7 Native layer adjustments disposition

**Question**: §7 contains architectural lessons specific to native layer work (P/Invoke discipline, K-L8 storage abstraction implications, K8.x migration guidance). Pipeline-agnostic. Disposition?

**Alternatives**:

**Q9.a — Untouched**:
Pro: content stays; principles invariant.
Con: may have incidental terminology.

**Q9.b — Verify clean, scrub if needed**:
Pro: defensive.
Con: minor work.

**Q9.c — Expand with K-L3.1 bridge formalization context**:
- New sub-section: «Bridge formalization (K-L3.1, 2026-05-10) — selective per-component path discipline, default unmanaged struct + escape hatch managed class storage».
- References K-L3.1 amendment plan for full lock detail.
Pro: K-L3.1 architectural decision documented in methodology native layer guidance.
Con: native layer content vs methodology framing — should K-L3.1 lock be in METHODOLOGY (process) or KERNEL_ARCHITECTURE (product)? Likely in KERNEL_ARCHITECTURE (per A'.1 amendment plan); methodology references it.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (b). K-L3.1 lock belongs in KERNEL_ARCHITECTURE (architectural product); methodology may briefly reference K-L3.1 as architectural decision precedent but full lock content lives in product docs.

### §3.10 Q10 — PIPELINE_METRICS treatment (Q-A07-3 implementation)

**Question**: Q-A07-3=β+γ locks «preserve as historical с per-metric reassessment notes». Concrete implementation?

**Alternatives**:

**Q10.a — Top-of-document frame note + per-metric annotations**:
- Top of PIPELINE_METRICS.md: clear frame note «v1.5 era data, gathered под 4-agent pipeline (Crystalka + Opus + Sonnet + Gemma). Transferability to current 2-agent unified Claude Desktop pipeline reassessed below per metric».
- Each metric/section gets annotation: «transfers» / «doesn't transfer» / «uncertain» / «specific to v1.x».
Pro: cleanest implementation of Q-A07-3=β+γ.
Con: requires per-metric judgment during deliberation.

**Q10.b — Top-of-document frame note only, no per-metric annotation**:
- Single frame note covers entire document.
- Reader makes per-metric judgment themselves.
Pro: minimum work.
Con: reader bears reassessment burden; defeats Q-A07-3=γ purpose.

**Q10.c — Two-document split**:
- PIPELINE_METRICS_V1.md (historical archive).
- PIPELINE_METRICS.md (current era, fresh start, A'.0.5 as first data point).
Pro: clean separation; historical vs current data don't mix.
Con: requires fresh metric collection for current document; first-data-point start is sparse.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (a). Aligns with Q-A07-3=β+γ exactly. Per-metric annotation is one-time deliberation cost during A'.0.7; future reads benefit from clear transferability flagging.

### §3.11 Q11 — MAXIMUM_ENGINEERING_REFACTOR 4-agent sub-sections disposition

**Question**: MAXIMUM_ENGINEERING_REFACTOR.md contains 4-agent sub-sections (per A'.0.5 deferral marker). Disposition?

**Alternatives**:

**Q11.a — Discard 4-agent sub-sections**:
- Sections removed; document focuses on architectural refactor tracks (Track A modular, Track B analyzer).
Pro: cleanest contemporary framing.
Con: forfeits any pipeline-architecture rationale captured in those sub-sections.

**Q11.b — Rewrite to current pipeline framing**:
- 4-agent sub-sections rewritten to 2-agent unified pipeline framing.
- Tracks A and B preserved (architectural refactor content, pipeline-agnostic).
Pro: pipeline rationale preserved with current framing.
Con: requires authoring; possibly substantial rewrite.

**Q11.c — Preserve historical, add current pipeline note**:
- Original 4-agent sub-sections preserved with frame note «v1.x era pipeline rationale».
- New current-pipeline rationale added in adjacent sub-section.
Pro: archaeological trail preserved + current contemporary.
Con: document grows; reader navigates dual framing.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (b) for sub-sections directly tied to pipeline-architecture rationale; (c) for sub-sections where 4-agent context informs ongoing decisions. Per-sub-section judgment.

### §3.12 Q12 — README.md (top-level) Pipeline section disposition

**Question**: Top-level `README.md` Pipeline section preserves 4-agent description per A'.0.5 deferral marker. Disposition under generalized methodology framing?

**Alternatives**:

**Q12.a — Replace with abstract architect-executor framing** (Q1.a-style):
- Pipeline section: «human as direction owner, LLM agent(s) as architect (deliberation, brief authoring, QA review) and executor (mechanical application against authored briefs)».
- No specific N or model tier.
Pro: matches Q-A07-4=γ + Q1.a.
Con: may be too abstract for README purpose (project intro for new readers).

**Q12.b — Current pipeline description**:
- Pipeline section: «2-agent unified pipeline — Crystalka (direction, deliberation) + Claude Desktop session (architect-mode and executor-mode per session framing)».
- Brief reference to evolution from earlier 4-agent configuration.
Pro: concrete description for README readers; evolution noted.
Con: stale at next pipeline pivot.

**Q12.c — Hybrid**:
- Pipeline section abstract framing primary + current configuration concrete sub-paragraph.
- Matches Q1.b style.
Pro: abstract + concrete both layers.
Con: README section grows.

**Crystalka decides**: a/b/c.

**Recommendation surface**: (c). Matches Q1.b for METHODOLOGY consistency; README readers get both layers.

---

## §4 Synthesis (Phase 2)

After Q1–Q12 are answered, the session synthesizes the 12 answers into one coherent METHODOLOGY v1.6 framing. Three forms the synthesis can take:

### §4.A — Coherent v1.6 with abstract primary + concrete secondary

If most Q answers favor abstract framing (Q1.a/b, Q2.α, Q3.b/c, etc.), synthesis: METHODOLOGY v1.6 reads as abstract methodology document with current-pipeline concrete examples threaded throughout. Reads as pipeline-agnostic methodology that survives implementation pivots.

### §4.B — Coherent v1.6 with concrete primary + abstract noted

If Q answers favor concrete current-pipeline framing (Q1.c, Q2.γ with current rewrite, Q3.a, etc.), synthesis: METHODOLOGY v1.6 reads as current-pipeline methodology with abstract principle called out. Reads as «here's how we work now» with «and these principles invariant» notes.

### §4.C — Versioned methodology

If Q answers heavily favor archaeological preservation (Q2.γ archive split, Q4.c historical+current, Q11.c preserve+add), synthesis: METHODOLOGY v1.6 introduces explicit version structure — v1.x era sections preserved as historical, v1.6 sections current. Multi-layer document.

**Crystalka chooses synthesis form** at end of Phase 2.

**Recommendation surface**: synthesis depends on actual Q answers. Locked Q-A07-4=γ pushes toward §4.A; locked Q-A07-2=β allows mixed disposition compatible with §4.A or §4.C. §4.B unlikely under locked defaults.

---

## §5 Amendment plan (Phase 3 deliverable)

The session's deliverable is an amendment plan, not the amendments themselves. The plan specifies:

### §5.1 METHODOLOGY.md amendment

- Version bump v1.5 → v1.6
- Synthesis form (§4.A / §4.B / §4.C) determined in Phase 2
- Per-section disposition per Q-A07-2=β:
   - §0: per Q1 answer
   - §2.1: per Q2 answer
   - §2.2: per Q3 answer
   - §3: per Q4 answer
   - §4: per Q5 answer
   - §5: per Q6 answer
   - §7: per Q9 answer
   - §8 (if exists): per Q7 answer
   - K-Lessons sub-section: per Q8 answer (likely Q8.c expansion с A'.0.5 lesson)
- Status line updated v1.6 + date

### §5.2 PIPELINE_METRICS.md amendment

- Per Q10 answer (likely Q10.a top-of-document frame + per-metric annotations)
- Document version may bump or stay (decision in Phase 2 per how substantial annotations are)

### §5.3 MAXIMUM_ENGINEERING_REFACTOR.md amendment

- Per Q11 answer (likely Q11.b/c per-sub-section judgment)
- Tracks A and B content untouched (architectural refactor, pipeline-agnostic)

### §5.4 README.md amendment

- Per Q12 answer (likely Q12.c hybrid framing)
- Pipeline section updated; rest of README untouched

### §5.5 Cross-document drift audit (post-amendment)

After amendment brief executes, cross-document grep verifies:
- Zero stale «Gemma» / «Cline» / «LM Studio» / «4-agent» / «four agents» / «local quantized» references in active sections
- Historical references appropriately frame-noted (if Q-A07-3=β preserved)
- Pipeline-architecture cross-refs consistent

This is Phase 5 step in amendment brief, not in A'.0.7 session itself.

### §5.6 K-L3.1 amendment plan integration check

K-L3.1 amendment plan (`docs/architecture/K_L3_1_AMENDMENT_PLAN.md`) edits 4 LOCKED architecture docs (KERNEL/MOD_OS/MIGRATION_PLAN/MIGRATION_PROGRESS) for K-L3.1 lock propagation. **A'.0.7 should not conflict with A'.1 scope.**

Verification step in Phase 3:
- Confirm A'.0.7 amendment plan touches only methodology docs (METHODOLOGY/PIPELINE_METRICS/MAXIMUM_ENGINEERING_REFACTOR/README Pipeline section)
- Confirm A'.0.7 amendment plan does **not** touch architectural docs (KERNEL/MOD_OS/MIGRATION_PLAN/MIGRATION_PROGRESS)
- If overlap surfaces: flag, escalate, re-deliberate boundary

If clean: A'.0.7 + A'.1 amendments executable independently in either order (sequential per Phase A' default ordering A'.0.7 → A'.1 because methodology framing affects how A'.1 amendment commits are described).

---

## §6 Closure (Phase 4)

### §6.1 Session output artifacts

1. This brief, marked EXECUTED with session SHA reference (brief becomes tracked document at `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md`)
2. Session log capturing Q1–Q12 deliberation, alternatives considered, decision rationale
3. Amendment plan document at `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` (matching K-L3.1 precedent)
4. MIGRATION_PROGRESS A'.0.7 closure entry

### §6.2 Follow-up brief (separate, post-A'.0.7)

After A'.0.7 closure, amendment brief authored and executed (Cloud Code session in execution mode):
- Scope: docs-only amendments per §5.1–§5.4
- Estimated size: 30–60 min execution session
- Test count delta: zero (docs-only)
- Atomic commit shape: per-document (one commit per amended doc), version bumps bundled per K-Lessons Lesson 1 «atomic commit as compilable unit»
- Possible folding into A'.1: if methodology amendments are small enough, A'.0.7 amendment brief may fold into A'.1 K-L3.1 amendment brief execution as combined Phase. Decision at amendment brief authoring time.

### §6.3 Resumption of deferred work

After A'.0.7 closure + amendment brief execution, deferred work resumes:
1. **A'.1**: K-L3.1 amendment brief execution (4 LOCKED architecture docs + skeleton briefs) — methodology framing now contemporary, K-L3.1 amendments can describe pipeline accurately
2. **A'.3**: Push to origin (now ~50+ commits ahead origin, depending on A'.1 commit count)
3. **A'.4**: K9 brief execution
4. **A'.5–A'.7**: K8.3, K8.4, K8.5 briefs (each may need consistency review per A'.1 disposition)
5. **A'.8**: K-closure report (now includes K-L3.1 + A'.0.7 architectural decisions + post-K8.x lessons)
6. **A'.9**: Architectural analyzer milestone
7. M8.4 begins — Phase B

### §6.4 Stop conditions for the session

The session halts and escalates if:
- Q1–Q12 deliberation surfaces a question not anticipated by this brief (record as new question, decide whether in-scope)
- Synthesis (Phase 2) cannot reconcile Q1–Q12 answers into coherent v1.6 framing (re-examine answers; if irreducible conflict, defer some Q answers to follow-up sub-session)
- Pre-flight reads (Phase 0) reveal that one of the deferred docs has been inadvertently edited between A'.0.5 closure and A'.0.7 session start (record drift, adjust amendment plan)
- §5.6 cross-check surfaces conflict between A'.0.7 amendment scope and A'.1 K-L3.1 amendment scope (escalate, re-deliberate boundary)
- Crystalka surfaces architectural shift not captured in 12 questions (e.g., further pipeline restructure beyond 2-agent reality) — escalate, possibly defer A'.0.7 closure pending new direction

---

## §7 What this session deliberately does not do

- **No code changes**. Pure architectural deliberation.
- **No new tests**. Test count stays at 631 throughout.
- **No commits to source files**. Only amendment plan document is authored (and possibly the brief itself committed as `docs(briefs): author A'.0.7 brief`).
- **No execution of methodology amendments**. Amendment plan is the deliverable; amendments themselves are follow-up brief.
- **No K-L3.1 amendment work**. A'.1 scope, separate.
- **No K-closure report drafting**. A'.8 scope, separate.
- **No analyzer milestone work**. A'.9 scope, separate.
- **No PIPELINE_METRICS data refresh**. Q10.a captures historical with annotations; fresh data collection is post-A'.0.7 forward-looking concern.
- **No methodology version-2.0 commitment**. v1.5 → v1.6 is appropriate semantic versioning for «pipeline reality updated, principles unchanged» scope.

---

## §8 Out of scope (explicit)

- **Pipeline architecture decisions**: A'.0.7 documents the current 2-agent unified Claude Desktop pipeline reality; it does not deliberate whether to change pipeline further. Pipeline architectural decisions are separate sessions if needed.
- **Project goals or scope changes**: A'.0.7 is methodology document scope; project goals (Dual Frontier game, RimWorld-inspired colony simulator) are out of methodology document scope.
- **Mod ecosystem migration (M-series)**: A'.0.7 may reference M-series in methodology (e.g., «M-series migration phases verify methodology under new pipeline»), but does not author M-series briefs.
- **K-Lxx invariant changes**: K-L3.1 lock + skeleton briefs (K9, K8.3-K8.5) are sufficient methodology + architectural surface for now; A'.0.7 does not propose new K-Lxx invariants.
- **Empirical claim revalidation under new pipeline**: A'.0.7 captures locked Q-A07-3=β+γ disposition for PIPELINE_METRICS (preserve as historical); does not re-measure or revalidate.

---

## §9 Pre-session checklist (Crystalka readiness)

Before invoking A'.0.7 session, Crystalka confirms:

- [ ] A'.0.5 closure verified (`git log --oneline -1` shows `4e332bb` or descendant on main)
- [ ] No uncommitted work on `docs/`, `src/`, or `tools/`
- [ ] No active feature branch in mid-execution state
- [ ] Test baseline holds (`dotnet test` shows 631 passing)
- [ ] All 4 deferred docs (METHODOLOGY/PIPELINE_METRICS/MAXIMUM_ENGINEERING_REFACTOR/README) present at expected paths with deferral markers in place
- [ ] PHASE_A_PRIME_SEQUENCING.md present at `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`
- [ ] A'.0.5 lesson #1 captured in MIGRATION_PROGRESS A'.0.5 entry (input to Q8 deliberation)

If any check fails: A'.0.7 session pre-flight halts; failing condition resolved (or its absence acknowledged with explicit rationale) before deliberation begins.

---

## §10 Document provenance

- **Authored**: 2026-05-10, Opus 4.7, post-A'.0.5 closure
- **Authority**: Crystalka direction sequence 2026-05-10:
   - Pipeline restructure declarations (Gemma out, Cline out, Sonnet as separate agent out, Claude Desktop unified)
   - «Без костылей у меня много времени, а также требуется архитектурная чистота»
   - Path 1 acceptance for A'.0.7 framing: «принимаю рекомендации» on Q-A07-1 through Q-A07-5 locked defaults
- **Precedent**: K-L3.1 brief (architectural decision brief, fourth brief type, K8.0 §1.8 precedent extended)
- **Methodology basis**: METHODOLOGY.md §3 «stop, escalate, lock» (architectural decisions get formal recording); K-Lessons Lesson 4 «error semantics convention» (analog: methodology rewrite gets formal deliberation)
- **Pipeline reality**: 2-agent unified Claude Desktop session; brief written for execution within single Claude Desktop session in deliberation mode; amendment plan output, no agent handoff implied
- **Memory tracker**: `userMemories` 2026-05-10 entries (4 edits): K-L3.1 closure + skeleton states + analyzer purpose + pipeline restructure
- **Companion documents**:
   - `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (EXECUTED via 45d831c)
   - `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (APPLIED via 45d831c)
   - `tools/briefs/A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md` (EXECUTED via 4e332bb)
   - `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (Phase 4 K-L3.1 deliverable, awaits A'.1 execution)
   - `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (Phase A' anchor, updated in A'.0.5 closure)

---

**Brief end. Pre-flight (Phase 0) begins when Crystalka invokes the session. Phase 1 deliberation proceeds through Q1–Q12 against locked Q-A07-1 through Q-A07-5 defaults; output is amendment plan, executed in follow-up session.**

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K_L3_1_ADDENDUM_1
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K_L3_1_ADDENDUM_1
---
# K-L3.1 Brief — Addendum 1: Skeleton brief state correction

**Authored**: 2026-05-10 (Opus, post-Crystalka clarification)
**Attaches to**: `K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (authored 2026-05-10)
**Purpose**: Correct factual error in §6.3 sequencing; extend Phase 0 inventory and Phase 4 amendment plan to cover skeleton brief consistency review
**Application**: prepend or append to brief during K-L3.1 session pre-flight; addendum supersedes brief wherever they differ
**Status**: APPLIED 2026-05-10 — addendum read together with brief during K-L3.1 session Phase 0; §A1 factual correction absorbed into brief execution. **Brief state classification refinement (Crystalka clarification 2026-05-10)**: addendum §A1 collective phrasing «K9 + K8.3 + K8.4 + K8.5 briefs all AUTHORED as skeletons» is imprecise — K9 brief at `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` is a **full authored brief** (~1200 lines, Phase 0–9 specified, AUTHORED awaiting execution per β6 sequencing); only K8.3/K8.4/K8.5 are **true skeletons** (~30–36 lines each). §A2 forward-track brief reads completed against this distinction. §A5.6 dispositions captured in `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` §5 (K9=B surgical [full brief, version refs only], K8.3=B-to-C [skeleton], K8.4=C.1 in-place rewrite [skeleton], K8.5=B surgical [skeleton]).

---

## §A1 — Factual correction (§6.3 of brief)

The brief's §6.3 «Resumption of deferred work» includes:

> **3. K9 status check** — if not closed, K9 brief authoring resumes per Option c sequencing. If closed, proceed to K8.3.

This is incorrect. K9 brief is **AUTHORED but NOT EXECUTED** (skeleton brief on disk per Crystalka clarification 2026-05-10). The correct wording:

> **3. K9 brief execution** — K9 skeleton brief at `tools/briefs/K9_*.md` (or actual filename verified during pre-flight) is reviewed for post-K-L3.1 consistency, amended if needed (per §A3 of this addendum), then executed by Cloud Code per existing skeleton scope. K9 is kernel-side (RawTileField), independent of bridge mechanism, but its brief may reference K-L3 framing that requires update.

The same correction applies to **K8.3, K8.4, K8.5** — all four are **skeleton briefs authored but not executed**, per `MIGRATION_PROGRESS.md` line 471–473 (already captured in brief §2.1.4 but not fully integrated).

---

## §A2 — Pre-flight read extension (§2 of brief)

Add §2.4 to brief Phase 0 read inventory:

### §A2.4 — Skeleton brief reads (mandatory, per-brief structure scan)

K9, K8.3, K8.4, K8.5 skeleton briefs are read end-to-end during pre-flight. For each:

1. **Locate the file**: `directory_tree` of `tools/briefs/` to confirm filename. Migration Progress line 471–473 references:
   - `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md`
   - `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md`
   - `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md`
   - K9 brief filename verified at pre-flight (likely `K9_RAW_TILE_FIELD_BRIEF.md` or analog)

2. **Skeleton state classification** per brief:
   - **Stable**: skeleton wording does not reference K-L3 framing or path-aware patterns; survives K-L3.1 untouched
   - **Drift candidate**: skeleton references K-L3 «without exception», «universal struct», «native-path only», or analogous framing that K-L3.1 will amend
   - **Rewrite candidate**: skeleton's core scope or method depends on architectural assumption K-L3.1 changes (e.g., K8.4 «managed World retired» — does «retired» mean «deleted entirely» or «retained as managed-path peer per Q2.α»?)

3. **Skeleton citation map**: for each skeleton brief, record any explicit citation of:
   - K-L3 (kernel architecture)
   - K-L8 (storage abstraction)
   - K-L9 (vanilla = mods)
   - K-L11 (World-as-test-fixture)
   - MOD_OS lines 1149–1150
   - Migration Plan §0.3 LOCKED decisions
   - K8.2 v2 closure framing «без exception»
   
   Citation map becomes input to Phase 4 amendment plan: skeleton briefs containing now-stale citations are explicit amendment scope.

### §A2.5 — Note on K9 specifically

K9 (RawTileField) is **architecturally independent** of bridge decision per brief §1: fields are not entity-component storage; they're spatial grid storage with separate IModApi v3 surface (`Fields` and `ComputePipelines` per MOD_OS_ARCHITECTURE.md §4.6). The skeleton's *scope* should survive K-L3.1 untouched.

The skeleton's *framing* may not, however. Pre-flight read confirms whether K9 skeleton text references K-L3 or makes assumptions about component storage that bridge decision invalidates. If yes — K9 skeleton enters the amendment scope (likely small textual fix; K9 execution proceeds against amended skeleton). If no — K9 skeleton survives as-is.

---

## §A3 — Amendment plan extension (§5 of brief)

Add §5.6 to brief Phase 4 deliverable:

### §A5.6 — Skeleton brief consistency review

For each of K9, K8.3, K8.4, K8.5 skeleton briefs, the K-L3.1 amendment plan specifies one of three dispositions:

- **Disposition A: untouched** — skeleton survives K-L3.1 verbatim. No edit needed.
- **Disposition B: surgical amendment** — skeleton receives targeted text edits (citation updates, framing corrections, scope clarifications). Edits enumerated in amendment plan with old/new text pairs.
- **Disposition C: scope reformulation** — skeleton's core scope changes per K-L3.1 lock. Disposition C may itself bifurcate:
   - **C.1**: skeleton rewritten in-place (file kept, content largely replaced)
   - **C.2**: skeleton deprecated to `_DEPRECATED.md` suffix, replacement skeleton authored fresh (precedent: K8.2 v1 → v2 deprecation in K8.2 v2 closure)

Disposition decisions are part of amendment plan output, not session deliberation main thread. Each skeleton's disposition is decided after Q1–Q6 lock and synthesis (Phase 3) is complete.

**Sequencing constraint**: amendment plan execution (the follow-up brief per brief §6.2) includes skeleton brief amendments **before** any skeleton brief execution begins. Resumption order in §6.3 of brief therefore expanded:

1. Amendment brief execution (LOCKED docs + skeleton briefs amended)
2. README cleanup
3. Push to origin
4. K9 skeleton execution (per Disposition outcome)
5. K8.3 skeleton execution (per Disposition outcome)
6. K8.4 skeleton execution (per Disposition outcome)
7. K8.5 skeleton execution (per Disposition outcome)

Steps 4–7 are sequential per Phase A architecture (K-track closes before M-track per migration plan §0.1).

---

## §A4 — Pre-session checklist update (§9 of brief)

Add to §9 checklist:

- [ ] Skeleton briefs present at expected paths: `K9_*.md`, `K8_3_*.md`, `K8_4_*.md`, `K8_5_*.md` in `tools/briefs/`
- [ ] Each skeleton's last-modified timestamp recorded (pre-K-L3.1 baseline; useful if amendment scope discussion references «what was skeleton wording before amendment»)

---

## §A5 — Architectural note: this addendum is not architectural change

This addendum **does not** change the K-L3.1 session's architectural deliverable — it corrects an operational sequencing detail and extends the amendment plan to include skeleton briefs. The 6 design questions (Q1–Q6 in brief §3) and synthesis forms (§4) remain unchanged. Crystalka's deliberation surface is not affected.

The addendum exists because the brief was authored against incomplete state (Opus assumed K9 closed or not authored, did not check skeleton state for K8.3–K8.5). Crystalka clarification 2026-05-10 corrected the assumption; this addendum updates the brief without re-authoring it. Per K-Lessons Lesson 1 «atomic commit as compilable unit», the brief + addendum compose into one coherent input for K-L3.1 session — addendum is the cycle resolution rather than re-authoring.

---

## §A6 — Provenance

- **Authored**: 2026-05-10, immediately after `K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (~3 min later)
- **Trigger**: Crystalka clarification «K9 написан, но не выполнен. K8.3-K8.5 и они тоже не выполнены они скелеты»
- **Application method**: attach to brief during K-L3.1 session invocation; pre-flight reads brief + addendum together
- **Supersession rule**: where brief and addendum disagree, addendum wins
- **Memory tracker**: `userMemories` 2026-05-10 entry («K9 + K8.3 + K8.4 + K8.5 briefs all AUTHORED as skeletons, NOT EXECUTED»)

---

**Addendum end. Read this together with `K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` during K-L3.1 session pre-flight.**

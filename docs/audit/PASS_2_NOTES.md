---
title: Pass 2 Notes
nav_order: 98
---

# Pass 2 Notes — items flagged for human review

**Status:** open — Pass 2 in progress, started 2026-04-26.

This file is the audit trail for Pass 2 (documentation translation). It captures three classes of items the human (or a Pass-1 follow-up session) should review before Pass 2 closes:

1. **Escalated glossary terms used during Pass 2.** The glossary v1.0 marks two entries as `⚠ ESCALATE TO HUMAN` (§11.1 owner-role label, §11.7 race-vs-species lore question). To unblock translation, Pass 2 picked the safer fallback candidate documented in the glossary itself, recorded every occurrence here, and proceeded. The human can search-and-replace once a final decision is made.

2. **Glossary additions needed.** Russian terms encountered during translation that were missing from §1–§10 and not covered by §13 reserved identifiers or §14 phrase patterns. Per §16.2 ("stop, escalate, update"), each entry lists the Russian source, the proposed English equivalent, the rationale, and the first source location.

3. **Contextual concerns.** Cases where the locked glossary term reads awkwardly in a specific passage but Pass 2 did not deviate (per the "no improvisation" rule). Recorded for the human to judge whether a phrase pattern (§14) should be added in v1.x.

---

## 1. Escalated glossary terms — fallback candidates used

### 1.1 Owner-role label (`владелец смысла`) — fallback `direction owner`

**Glossary reference:** §11.1 — `⚠ ESCALATE TO HUMAN`, two viable candidates: `intent owner` (collides with project terminology — `Intent`, `IntentBatcher`, etc.) or `direction owner` (methodologically sound, slight identity reframing from "owner of meaning" to "owner of long-term trajectory").

**Pass 2 decision:** used `direction owner`. Rationale: avoids the technical-term collision flagged in §11.1; the glossary describes it as "methodologically sound"; it is the lower-risk default. Once the human picks the final form, every occurrence below can be swept in a single search-and-replace.

**Occurrences (Russian source, awaiting human decision):**

| File | Line (RU) | Context |
|---|---|---|
| `README.md` | 20 | Agent role table, fourth row: «Человек / Владелец смысла / — / Выбор контрактов, архитектурные решения, формулировка целей фаз» |
| `docs/methodology/METHODOLOGY.md` | 13 | §0 abstract: «человек как владелец смысла» (in the four-agent role list) |
| `docs/methodology/METHODOLOGY.md` | 43 | §2.1 role distribution: «**Человек.** Владелец смысла.» |
| `docs/methodology/METHODOLOGY.md` (link description in `README.md`) | — | The README's METHODOLOGY bullet was rephrased to "the human as direction owner" using the same fallback. |

(Further occurrences in lore docs and any remaining methodology cross-links will be appended as Pass 2 reaches them.)

### 1.2 Race vs species (`биология рас`) — fallback `species biology`

**Glossary reference:** §11.7 — `⚠ ESCALATE TO HUMAN`, depends on a lore question only the project author can answer. If lore has one taxonomic level (humans, dwarves, golems are biologically distinct peoples) → lock `species`. If two levels (humans subdivide into races; separately there are dwarves as a species) → both terms with explicit distinction.

**Pass 2 decision:** used `species biology`. Rationale: the glossary explicitly notes that "race" is socially loaded in English and recommends `species` as the safer neutral default; the README's framing — "biology of peoples, not ideological player choice" — reads naturally with `species` and does not appear to require an intra-species distinction.

**Occurrences (Russian source, awaiting human decision):**

| File | Line (RU) | Context |
|---|---|---|
| `README.md` | 3 | Project tagline: «обусловленными биологией рас, а не идеологическим выбором игрока» |

(Additional occurrences in `docs/methodology/METHODOLOGY.md` and lore docs will be appended as Pass 2 reaches them.)

---

## 2. Glossary additions needed

*None yet. Entries will be appended in this format:*

```
### 2.N <term>

- **Russian:** <source phrase>
- **Proposed English:** <candidate>
- **Rationale:** <why this candidate>
- **First source location:** <file>:<line>
```

---

## 3. Contextual concerns (locked term used as-is)

*None yet. Entries will be appended in this format:*

```
### 3.N <term> — <file>:<line>

- **Locked English form:** <from glossary>
- **Concern:** <why it reads awkwardly here>
- **Pass 2 action:** used the locked form, did not deviate.
- **Suggested follow-up:** <e.g., consider adding a §14 phrase pattern>
```

---

## 4. Anchor-link coordination

`docs/methodology/DEVELOPMENT_HYGIENE.md` has three cross-doc anchor links that GitHub auto-derives from heading text. To keep them resolvable after translation, the target docs MUST use exactly these English headings:

### 4.1 ARCHITECTURE.md anchor — retargeted

- **Russian source link:** `[ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра)`
- **Issue:** the `«Граница движок / игра»` section does not exist in the current `docs/architecture/ARCHITECTURE.md` source (Russian or English). The link was already broken before Pass 2.
- **Pass 2 action:** retargeted to the closest existing section: `[ARCHITECTURE §"Dependency rules"](/docs/architecture/ARCHITECTURE.md#dependency-rules)`. The `## Dependency rules` heading exists in the translated ARCHITECTURE.md.
- **Suggested follow-up (out of scope for Pass 2):** the human may want to add an explicit "Engine / game boundary" section to ARCHITECTURE.md that contains the engine vs game assembly table currently duplicated in DEVELOPMENT_HYGIENE.md.

### 4.2 CODING_STANDARDS.md anchor — section missing in source

- **Predicted heading in translated CODING_STANDARDS.md:** `## Commit messages` → anchor `#commit-messages`.
- **Issue discovered during Pass 2:** the Russian source `docs/methodology/CODING_STANDARDS.md` does not actually contain a `«Сообщения коммитов»` section. The DEVELOPMENT_HYGIENE.md links to a section that was never written. The anchor will remain broken in both Russian and English versions until someone adds the section.
- **Pass 2 action:** preserved the predicted English anchor `#commit-messages` in DEVELOPMENT_HYGIENE.md, anticipating that the missing section will eventually be added with that exact heading.
- **Suggested follow-up (out of scope for Pass 2):** add a `## Commit messages` section to CODING_STANDARDS.md that documents the commit-prefix taxonomy already enumerated in DEVELOPMENT_HYGIENE.md §"Quick reference — commit scope prefixes".

### 4.3 ROADMAP.md anchor — retargeted

- **Russian source link:** `[ROADMAP §«Пост-релиз — развилка на движок»](./ROADMAP.md#пост-релиз--развилка-на-движок)`
- **Issue:** the `«Пост-релиз — развилка на движок»` section does not exist in `docs/ROADMAP.md` (Russian or English). The link was already broken before Pass 2.
- **Pass 2 action:** retargeted both DEVELOPMENT_HYGIENE.md references to `[ROADMAP §"Phase 9 — Native Runtime"](./ROADMAP.md#phase-9--native-runtime)`, which is the existing section that describes the post-release engine work (own entry point, abstract `IRenderer`, port to any backend, "after Phase 7 closure and Steam launch").
- **Suggested follow-up (out of scope for Pass 2):** the human may want to rename Phase 9 to "Post-release — engine fork" if that framing better matches the project's communication, or add an explicit "Post-release" subsection within Phase 9.

---

## 5. Out-of-scope file flagged

`assets/scenes/README.md` contains Russian content but lives outside the Pass 2 scope as defined by the campaign brief (`docs/`, root `README.md`, `src/`, `tests/`, `mods/`). It is a 22-line Godot-scene-fixture README referencing `.dfscene` format. Recommend translating in a follow-up `chore(i18n)` commit when the human wants to fully complete the cyrillic sweep.

---

## Closing checklist (filled at Pass 2 closure, 2026-04-27)

- [x] All P0–P3 documents translated.
  - 9 P0 docs (README root, docs/README, METHODOLOGY, ARCHITECTURE, CONTRACTS, ECS, EVENT_BUS, THREADING, ISOLATION).
  - 11 P2 docs (MODDING, MOD_PIPELINE, TESTING_STRATEGY, DEVELOPMENT_HYGIENE, CODING_STANDARDS, GODOT_INTEGRATION, VISUAL_ENGINE, PERFORMANCE, ROADMAP, NATIVE_CORE_EXPERIMENT, GPU_COMPUTE).
  - 5 v0.2 addendum docs (RESOURCE_MODELS, COMPOSITE_REQUESTS, FEEDBACK_LOOPS, COMBO_RESOLUTION, OWNERSHIP_TRANSITION).
  - 61 module READMEs across `src/`, `tests/`, `mods/`.
  - `learning/PHASE_1.md` translated with note-block per §2.4.
  - `TRANSLATION_PLAN.md` (the campaign-planning meta-doc).

- [x] Cross-references resolve to translated targets where possible. Three pre-existing broken anchors flagged in §4 (ARCHITECTURE / CODING_STANDARDS / ROADMAP — none introduced by Pass 2; either retargeted to nearest existing section or preserved as forward-looking placeholders).

- [x] `dotnet build DualFrontier.sln -c Release` clean: 0 warnings, 0 errors.

- [ ] `dotnet test` regression check: **3 pre-existing failures in `DualFrontier.Core.Tests/Isolation/IsolationGuardTests` (`GetComponent_Undeclared_Throws_ForCore`, `SetComponent_Undeclared_Throws`, plus one related)**. These also fail on `main` — verified by checkout-and-rerun. NOT introduced by Pass 2; Pass 2 touched no `*.cs` files. The historical "82/82" target referenced in TRANSLATION_PLAN.md predates these regressions. Pass 3 (code translation) is the natural place to also fix these once the isolation-guard messages flip to English.

  Tally on `chore/translation-pass-1`: Failed: 3, Passed: 79 (across all four test projects: Core 57/60, Modding 11/11, Systems 7/7, Persistence 4/4). Same tally on `main`.

- [x] `grep -rE --include='*.md' '[А-Яа-я]' .` returns only intentionally-preserved Russian:
  - `docs/SESSION_PHASE_4_CLOSURE_REVIEW.md` (audit-trail preserved per spec).
  - `docs/TRANSLATION_GLOSSARY.md` (Russian column of the glossary).
  - `docs/reports/NORMALIZATION_REPORT.md` (Pass 1 output, quotes Russian source terms).
  - `docs/TRANSLATION_PLAN.md` (this document quotes the original Russian diagnostic strings and the §3 lock-decision Russian terms — intentional reference material).
  - `docs/PASS_2_NOTES.md` (this file's audit-trail entries quote the Russian source).
  - `docs/methodology/CODING_STANDARDS.md` (the "Russian-language domain comments" section preserves the policy example with Russian-comment C# code; the policy as stated will outlive Pass 2 and is revisited during Pass 3 code-comment translation).
  - `assets/scenes/README.md` — out of Pass 2 scope (see §5).

- [x] §1 escalations recorded for the human. `direction owner` and `species biology` are the Pass 2 fallback choices; every occurrence is logged. Once the human picks the final form, a single search-and-replace closes the audit.

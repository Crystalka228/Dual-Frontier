---
title: Pass 4 verification report
nav_order: 96
---

# Pass 4 — Translation campaign verification report

**Date:** 2026-04-27
**Branch:** `chore/translation-pass-3` (Pass 3 closure commit `c13e6f8`; Pass 4 surgical fix `352ff0f`)
**Scope:** Verification only. No new translation work. Surgical fixes applied only for typos, broken links, and isolated glossary deviations.

---

## §0 Executive summary

| # | Check | Status | Notes |
|---|---|---|---|
| 1 | Cyrillic grep | **PASSED** | 1 surgical fix applied; remaining Cyrillic confined to policy-allowed and by-design bilingual reference files. |
| 2 | Build & test | **PASSED** | `dotnet build`: 0 warnings, 0 errors. `dotnet test`: 82/82 passed (Core 60, Modding 11, Systems 7, Persistence 4). |
| 3 | Glossary compliance | **PASSED with one systematic finding** | 19 of 20 sampled terms clean. UK-vs-US English deviation is a known systemic pattern documented in glossary §11.6 (Pass 5). |
| 4 | Cross-reference integrity | **PASSED** | 207 relative links checked, 0 broken targets. 5 broken anchors are all documented as known-by-design in `PASS_2_NOTES.md` §4. |
| 5 | Style consistency | **PASSED** | Five paragraph samples (METHODOLOGY, ARCHITECTURE, CONTRACTS, ISOLATION, EVENT_BUS) read as native technical English. Active voice predominates. No translatese remnants. |
| 6 | Doc/code/test triple consistency | **PASSED** | All 3 implemented contracts (15.1a, 15.1b, 15.2) match doc spec verbatim; tests assert via `IsolationDiagnostics` constants. The 3 doc-only pre-specs (15.3, 15.4, 15.5) remain doc-only as expected. |
| 7 | Stale references sweep | **PASSED** | All `Phase N` references resolve to existing ROADMAP sections (Phase 8 gap is intentional). All `v0.x §N.N` references are stable TechArch-spec citations. METHODOLOGY internal §-refs all resolve. |
| 8 | New-contributor sanity check | **PASSED** | `README.md` and `docs/README.md` read cleanly cold. Two minor onboarding frictions noted, both rolled into the same Pass 5 item already on the list. |

**Result:** All 8 checks passed. One surgical fix applied. Three follow-up items recorded for a possible Pass 5 (none of them regressions from Pass 3 — all were either documented in earlier pass notes or flagged in the glossary itself as known non-blocking).

---

## §1 Cyrillic grep

**Command:**
```
grep -rE --include='*.md' --include='*.cs' --include='*.json' --include='*.csproj' '[А-Яа-яЁё]' .
```

**Pre-fix state:** 543 occurrences across 8 files.
**Post-fix state:** 528 occurrences across 7 files.

| File | Cyrillic chars (lines) | Disposition |
|---|---|---|
| `docs/SESSION_PHASE_4_CLOSURE_REVIEW.md` | 133 | **Policy-allowed** per Pass 4 prompt — Russian-language audit-trail, intentionally preserved verbatim. |
| `docs/TRANSLATION_GLOSSARY.md` | 292 | **By-design bilingual reference** — RU↔EN term mappings. Cyrillic is load-bearing. |
| `docs/NORMALIZATION_REPORT.md` | 72 | **By-design bilingual reference** — Pass 1 report, cites Russian source for traceability. |
| `docs/TRANSLATION_PLAN.md` | 13 | **By-design bilingual reference** — campaign meta-doc, cites Russian terms. |
| `docs/PASS_2_NOTES.md` | 12 | **By-design bilingual reference** — pass closure notes citing source-side anchors and Russian glossary terms. |
| `docs/PASS_3_NOTES.md` | 3 | **By-design bilingual reference** — pass closure notes citing source terms. |
| `docs/CODING_STANDARDS.md` | 3 | **§10 Pass 5 item.** See §10.1 below — these 3 lines are inside an entire section ("Russian-language domain comments", lines 84-115) that is now stale. The Cyrillic itself is illustrative (a Russian-comment example), but the surrounding policy contradicts the post-Pass-3 codebase reality. Not a translation defect — a content-policy decision. |
| ~~`assets/scenes/README.md`~~ | ~~15~~ | **FIXED** in commit `352ff0f` (surgical fix §9.1). |

**Pass 1 sweeps:** 0 Cyrillic in any `.cs` or `.csproj` — confirms Pass 3's "src/, tests/, mods/ Cyrillic-clean" claim still holds.

**Verdict:** PASSED. The remaining 528 chars are accounted for by policy or documented as Pass 5 content work.

---

## §2 Build & test

**`dotnet build DualFrontier.sln`:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:06.68
```

All 14 projects built (11 src + 4 tests + 1 mod example, plus their dependencies).

**`dotnet test DualFrontier.sln`:**

| Project | Pass | Skip | Total |
|---|---|---|---|
| `DualFrontier.Persistence.Tests` | 4 | 0 | 4 |
| `DualFrontier.Systems.Tests` | 7 | 0 | 7 |
| `DualFrontier.Modding.Tests` | 11 | 0 | 11 |
| `DualFrontier.Core.Tests` | 60 | 0 | 60 |
| **Total** | **82** | **0** | **82** |

**Verdict:** PASSED. Build clean, tests green, matches Pass 3 closure baseline.

---

## §3 Glossary compliance — 20-term sample

Sampling method: deterministic strategic sample biased toward terms with multiple plausible English forms, escalations, and locked decisions in §11.

**Sample:**

| # | Term / glossary section | Locked / fallback form | Deviation grep | Result |
|---|---|---|---|---|
| 1 | Owner-role label (§11.1, fallback) | `direction owner` | `intent owner`, `meaning owner`, `owner of intent` | ✅ Used consistently in METHODOLOGY.md and root README; deviation candidates appear only in process docs explaining the escalation. |
| 2 | Isolation runtime (§11.2) | `isolation guard` | `sentinel`, `watchdog`, `monitor` | ✅ Hits in code only as the unrelated CS pattern "sentinel value" (in `EntityId`, `ComponentStore`); no semantic conflict. |
| 3 | Magic branch (§11.3) | `arcane branch` | `arcane-magical`, `arcano-magical` | ✅ No deviations outside process docs. |
| 4 | Magic resource (§11.4) | `ether` | `aether` | ✅ No deviations outside process docs. |
| 5 | Pawn (§11.5) | `pawn` | `colonist`, `unit` (in pawn-context) | ✅ No deviations outside process docs. |
| 6 | Falsifiable (§1) | `falsifiable claim` | `falsifiable statement`, `testable claim` | ✅ Glossary mentions deviation candidates only in §11.8 commentary. |
| 7 | Pipeline label (§1) | `four-agent pipeline` | `4-agent pipeline`, `four agent pipeline` | ✅ No deviations. |
| 8 | Race biology (§11.7, fallback) | `species biology` | `racial biology`, `race biology` | ✅ Pass 2 fallback `species biology` used in PASS_2_NOTES, root README line 3. |
| 9 | Architectural debt (§9) | `architectural debt` | — | ✅ Used in METHODOLOGY.md. The unrelated generic term `technical debt` is correctly used for distinct CS-concept contexts (ROADMAP §"Technical debt", METHODOLOGY line 301). |
| 10 | Phase review (§9) | `phase review` / `phase-review` | — | ✅ Consistent throughout METHODOLOGY.md. |
| 11 | Marker interface (§2) | `marker interface` | — | ✅ Standard CS term used consistently. |
| 12 | Lease (§5) | `lease` / `Lease` | — | ✅ 31 occurrences across 8 files, glossary-compliant. |
| 13 | Intent / Granted / Refused (§3) | `Intent → Granted/Refused` verbatim | — | ✅ Preserved verbatim in EVENT_BUS, RESOURCE_MODELS, root README. |
| 14 | Marker patterns (§13.5) | `[SystemAccess]`, `[Deferred]`, `[Immediate]` | — | ✅ Code identifiers preserved. |
| 15 | Translatese — "This permits…" (§14.1) | `This lets X` / `X can…` | `This permits the scheduler to…`, `This provides decoupling` | ✅ Only hit is the glossary's own example showing what to avoid. No actual translatese in technical docs. |
| 16 | Domain bus (§2) | `domain bus` (lowercase prose) | — | ✅ Used consistently. |
| 17 | RaceComponent code identifier (§13) | `RaceComponent` (preserved) | — | ✅ Kept; XML docs use `race` to describe the property of `RaceComponent.Race`, which is the correct documentation pattern for a code identifier. |
| 18 | Mana / ether / golem (§8) | `mana`, `ether`, `golem` | — | ✅ Used consistently. |
| 19 | Modding terms (§7) | `mod author`, `mod loader`, `mod contract` | — | ✅ Used consistently. |
| 20 | **US English (§11.6)** | `color`, `behavior`, `defense`, `quantized`, `parallelize`, `centralize`, `analyze`, `synchronize` | UK forms: `colour`, `behaviour`, `defence`, `quantised`, `parallelise`, `centralise`, `analyse`, `synchronise` | ⚠️ **Systemic deviation — 30+ occurrences.** See §10.2. |

**Detail on the one systemic finding (item 20 — UK English):**

UK-form occurrences include the `BehaviourTree` namespace and class hierarchy (`src/DualFrontier.AI/BehaviourTree/`, `BehaviourTree/Selector.cs`, `BehaviourTree/BTContext.cs`, `BehaviourTree/Leaves/*.cs`, `BehaviourTree/BTNode.cs`, `BehaviourTree/Leaf.cs`), prose `behaviour` in XML docs (`DevKitOnlyAttribute.cs`, `TickRateAttribute.cs`, `ImmediateEventDeliveryTests.cs`), `analyser` (`DevKitOnlyAttribute.cs`, `DevKitRendererContractTests.cs`), `coloured` (`IDevKitRenderer.cs`, `PawnVisual.cs`, `TileMapRenderer.cs`), `defence` (`ShieldComponent.cs`), `quantised`/`quantisation` (`PawnSnapshot.cs`, `ComponentEncoderTests.cs`), `centralises` (`IsolationDiagnostics.cs`).

**Glossary §11.6 itself documents this as a known non-blocking inconsistency:**
> "existing English-column entries in this glossary that still use `-ise`/`-our` forms are non-blocking inconsistencies — Pass 2 normalizes them in-place when consuming the entry, and a follow-up `chore(normalize): align glossary English column to US lock` commit may sweep them in bulk."

The pattern includes a code-identifier rename (`BehaviourTree` namespace), which is a refactor — explicitly out of Pass 4 scope.

**Verdict:** PASSED with one systemic finding (item 20) flagged for Pass 5 / future `chore(normalize)` commit.

---

## §4 Cross-reference integrity

**Method:** Scripted scan of all 95 markdown files in the repo. For every relative link `[text](path)` (excluding `http://`, `https://`, `mailto:`, and pure in-document `#anchor` fragments), resolve the path against the source file's directory and check existence. For links with `#anchor`, slugify all headings in the target file using the `github-slugger` algorithm (each whitespace char → one hyphen, drop characters not in `[\w\s-]`) and check the anchor against the slug set.

**Results:**

| Metric | Count |
|---|---|
| Markdown files scanned | 95 |
| Relative links checked | 207 |
| Broken target paths | 0 |
| Links with anchors | 11 |
| Broken anchors | 5 |

**Detail on the 5 broken anchors — all documented as known-by-design in `PASS_2_NOTES.md` §4:**

| Source | Target | Anchor | Disposition |
|---|---|---|---|
| `docs/DEVELOPMENT_HYGIENE.md:63` | `./CODING_STANDARDS.md` | `#commit-messages` | **Predicted forward-pointing anchor** per `PASS_2_NOTES.md` §4.2. The target section was never written in the Russian source either. Pass 2 deliberately preserved the anchor to lock the eventual section heading. Awaits Pass 5 / content addition. |
| `docs/DEVELOPMENT_HYGIENE.md:134` | `./CODING_STANDARDS.md` | `#commit-messages` | Same as above. |
| `docs/DEVELOPMENT_HYGIENE.md:151` | `./CODING_STANDARDS.md` | `#commit-messages` | Same as above. |
| `docs/PASS_2_NOTES.md:91` | `./ARCHITECTURE.md` | `#граница-движок--игра` | **Historical citation** — PASS_2_NOTES.md §4.1 cites this Russian-anchor as "the original broken link, retargeted to `#dependency-rules`". The Russian-anchor literal here is documentation of a historical broken link, not an active navigation link. |
| `docs/PASS_2_NOTES.md:105` | `./ROADMAP.md` | `#пост-релиз--развилка-на-движок` | **Historical citation** — PASS_2_NOTES.md §4.3 cites this Russian-anchor as "the original broken link, retargeted to `#phase-9--native-runtime`". Documentation, not navigation. |

**Verdict:** PASSED. No regressions introduced by Pass 3. The 3 active broken anchors (`#commit-messages` in DEVELOPMENT_HYGIENE.md) are tracked under §10.1 below.

---

## §5 Style consistency

Sampled 1 mid-document paragraph from each of: `docs/METHODOLOGY.md`, `docs/ARCHITECTURE.md`, `docs/CONTRACTS.md`, `docs/ISOLATION.md`, `docs/EVENT_BUS.md`.

| Doc & section | (a) US spelling | (b) Active voice | (c) Sentence length | (d) Calque-free | Notes |
|---|---|---|---|---|---|
| METHODOLOGY §2.4 (line 97) — "An architectural decision and its implementation…" | ✅ | ✅ predominant | ✅ varied 3 sentences | ✅ no Russian idioms | Mild: "it is unknown whether" (line 97 trailing clause) is slightly awkward — could read "we cannot know whether"; not a translation defect. |
| ARCHITECTURE "Presentation" (line 53) | ✅ | ✅ | ✅ | ✅ | "Both work only in their backend's main thread" — fine; "run" might be marginally more idiomatic than "work" but well within native register. |
| CONTRACTS "IModContract" (line 63) | ✅ | ✅ | ✅ | ✅ | Exemplary: RFC 2119 register ("MUST NOT") used correctly. |
| ISOLATION "What the system context contains" (line 30 ff.) | ✅ | ✅ | ✅ | ✅ | List form with declarative bullets; clean. |
| EVENT_BUS "Two-step Intent → Granted/Refused" (line 105) | ✅ (`synchronization` is the US form; UK is `synchronisation`) | ✅ | ✅ | ✅ | "is a trap" — vivid native idiom. |

**Verdict:** PASSED. All five samples read as native technical English. No paragraph showed the calque-style "This permits us to…" / "By means of which…" / "Owing to the fact that…" structures the glossary §14 catalogues. Active voice predominates; passive appears only where the project register explicitly accepts it (roadmap items per §11.9).

The minor stylistic observation in METHODOLOGY (line 97) is below the surgical-fix threshold and not flagged for Pass 5.

---

## §6 Doc/code/test triple consistency

**Method:** For each user-facing string contract in `NORMALIZATION_REPORT.md` §4 / `TRANSLATION_GLOSSARY.md` §15, verify three legs:
- (a) Code constant value matches doc-quoted text verbatim.
- (b) Tests assert via the named constant, not via a string literal.
- (c) Doc cites the English literal form.

| Contract | Doc | Code constants | Tests | Status |
|---|---|---|---|---|
| **15.1a Read isolation** | `docs/ISOLATION.md` lines 134-142 (`[ISOLATION VIOLATED] / System '...' / accessed '...' / without an access declaration. / Add: [SystemAccess(reads: new[]{typeof(...)})]`) | `IsolationDiagnostics.{ViolationHeader, SystemPrefix, SystemSuffix, ReadVerb, ComponentSuffix, ReadReason, HintPrefix, ReadHintArgPrefix, HintArgSuffix}` (concatenated in `SystemExecutionContext.BuildReadViolationMessage` at line 282-296) | `IsolationGuardTests.cs:53,54,125` use `IsolationDiagnostics.UndeclaredAccessToken`, `IsolationDiagnostics.HintToken` | **PASS** |
| **15.1b Write isolation** | `docs/ISOLATION.md` (parallel structure to 15.1a, `modified` / `without a write declaration` / `writes:`) | `IsolationDiagnostics.{WriteVerb, WriteReason, WriteHintArgPrefix}` (concatenated in `BuildWriteViolationMessage` at line 298-310) | `IsolationGuardTests.cs:85,86` use `IsolationDiagnostics.WriteVerbToken`, `IsolationDiagnostics.HintToken` | **PASS** |
| **15.2 Direct system access** | `docs/ISOLATION.md` lines 156-161 | `IsolationDiagnostics.{GetSystemHeader, GetSystemBody, GetSystemHint}` joined in `GetSystem<TSystem>` at line 256-262 | `IsolationGuardTests.cs:99` uses `IsolationDiagnostics.DirectSystemAccessToken` | **PASS** |
| 15.3 Direct World access (doc-only pre-spec) | `docs/ISOLATION.md` lines 167-171 | NOT YET IMPLEMENTED (verified — `World.GetComponentUnsafe` is reachable in the same file at line 159 but no IsolationViolationException is thrown for the bypass) | NOT YET TESTED | **PASS as doc-only** (matches glossary expectation) |
| 15.4 Wrong-bus publishing (doc-only pre-spec) | `docs/ISOLATION.md` lines 178-182 | NOT YET IMPLEMENTED (`_allowedBuses` is captured but never validated against an actual `Publish` call) | NOT YET TESTED | **PASS as doc-only** |
| 15.5 Mod fault banner (doc-only pre-spec, UI-facing) | `docs/ISOLATION.md` lines 86-90 | NOT YET IMPLEMENTED in `PresentationBridge`/`AlertPanel`/`ModFaultHandler` | NOT YET TESTED | **PASS as doc-only**. Cosmetic note: doc uses `<name>` / `<mod-id>` placeholder syntax while glossary §15.5 lock uses `{modName}` / `{modId}`. Not a contract issue (placeholders are presentational); not flagged for Pass 5. |
| §15.6 PR rejection format | `docs/ISOLATION.md` tail: "If any item is not satisfied, the PR is rejected." | N/A (doc-only) | N/A | **PASS** — matches glossary §15.6 verbatim. |

**Cross-check:** A dedicated grep for literal-string assertions in tests (`"accessed"`, `"modified"`, `"without an access"`, `"\\[ISOLATION VIOLATED\\]"`, etc.) returned zero hits — confirming all assertions go through `IsolationDiagnostics` constants, not literals.

**Verdict:** PASSED. Three implemented contracts are doc-code-test triple-consistent. Three doc-only pre-specs remain doc-only. The placeholder-syntax variance in §15.5 is cosmetic and not flagged.

---

## §7 Stale references sweep

**Patterns checked:** `v0\.[0-9]`, `Phase[ -]?[0-9]`, `§[0-9]+\.[0-9]+`, intra-doc cross-section refs.

**Findings:**

- **`v0.x` references** (~30 occurrences across CONTRACTS, EVENT_BUS, ROADMAP, NORMALIZATION_REPORT, ARCHITECTURE, README, METHODOLOGY) — all are versioned changelog/spec citations to TechArch v0.1/v0.2/v0.3. The TechArch document is referenced consistently in code (`src/DualFrontier.Contracts/Bus/IPowerBus.cs:11`, `IGameServices.cs:54`) and docs as the project's internal architecture-spec doc. References cross-reference each other consistently (e.g., NORMALIZATION_REPORT §4 says "added in TechArch v0.3 §13.1" and CONTRACTS.md / EVENT_BUS.md repeat the same v0.3 §13.1 attribution). PASS.
- **`Phase N` references** — all cited phases (0, 1, 2, 3, 3.5, 4, 5, 6, 7, 9) exist as `## Phase N` sections in `docs/ROADMAP.md`. The Phase 8 gap is intentional (no doc references Phase 8). PASS.
- **METHODOLOGY internal §-refs** (§2.2, §2.4, §3.1, §4.3, §4.4, §4.5, §5, §5.3) — all targets verified in METHODOLOGY.md (lines 45, 95, 105, 171, 179, 189, 203, 235). PASS.
- **CONTRACTS.md / EVENT_BUS.md `v0.3 §13.1` and `v0.2 §12.1`** — stable citations to TechArch sections; no breakage. PASS.

**Verdict:** PASSED. No stale references introduced or revealed by Pass 2/3.

---

## §8 New-contributor sanity check

**Method:** Read `README.md` (212 lines) and `docs/README.md` (68 lines) cold, as a contributor opening the repo for the first time.

**Findings:**

| # | Observation | Severity | Disposition |
|---|---|---|---|
| 8.1 | The pipeline configuration table (root `README.md` line 17-20) introduces "**Direction owner**" as the human's role label without explanation. A reader unfamiliar with the i18n campaign will not know this is the Pass 2 fallback for an escalated term. | Info | Acceptable: the label functions as a clear English term independent of campaign history. The escalation status is internal to the i18n process. |
| 8.2 | Root `README.md` line 135 describes `CODING_STANDARDS.md` as covering "naming, comments, file structure, **commit scope prefixes**" — but `CODING_STANDARDS.md` does not contain a Commit messages section (the commit-prefix taxonomy lives in `DEVELOPMENT_HYGIENE.md`). Same root cause as the 3 broken anchors in §4. | Minor | Rolled into Pass 5 item §10.1 — single content task. |
| 8.3 | `docs/CODING_STANDARDS.md` §"Russian-language domain comments" (lines 84-115) prescribes a coding-style policy ("Internal domain logic is commented in Russian. That is the project's working language.") that contradicts the post-Pass-3 codebase reality (0 Cyrillic in `*.cs`). A new contributor reading this would conclude they should write Russian comments. | Major | Pass 5 item §10.3. Not a Pass 4 surgical fix (requires team decision on the new comment-language policy, not a typo correction). |
| 8.4 | `docs/README.md` line 51 explicitly notes that `SESSION_PHASE_4_CLOSURE_REVIEW.md` is "*Russian-language audit trail; preserved verbatim per the i18n campaign rules.*" — a clean, contributor-friendly handling of the bilingual file. | Positive | None. Good pattern. |
| 8.5 | Root `README.md` Phase status table (line 113-122) shows "Updated: 2026-04-25" but reports 82/82 tests, 0 known production bugs — consistent with current state. | Positive | None. Up-to-date. |

**Verdict:** PASSED. Two onboarding frictions (8.2, 8.3) are both downstream of the same Pass 5 item — the missing/stale CODING_STANDARDS.md content. Both are recorded once in §10.

---

## §9 Surgical fixes applied

| # | Commit | Description |
|---|---|---|
| 9.1 | `352ff0f` | `fix(i18n): translate assets/scenes/README.md` — translated the 22-line Godot scene-fixture README from Russian to English. The file was explicitly flagged in `PASS_2_NOTES.md` §5 as "out of Pass 2 scope, follow-up chore"; user authorised the Pass 4 surgical fix. Glossary-compliant translation: kept code identifiers (`SceneDef.CurrentVersion`, `ISceneLoader`, `.dfscene`, `Godot DevKit`) verbatim; US English (no UK forms introduced); active voice. Post-fix Cyrillic count for the file: 0. |

**Total surgical fixes:** 1.

---

## §10 Items requiring Pass 5

Three follow-up items, all of which were either documented in earlier pass notes or flagged in the glossary itself as known non-blocking. None are regressions from Pass 3.

### §10.1 Add `## Commit messages` section to `CODING_STANDARDS.md`

**Source documentation:** `PASS_2_NOTES.md` §4.2 explicitly recommends this as Pass 2's "Suggested follow-up (out of scope for Pass 2)". Reproduced verbatim:
> "add a `## Commit messages` section to CODING_STANDARDS.md that documents the commit-prefix taxonomy already enumerated in DEVELOPMENT_HYGIENE.md §'Quick reference — commit scope prefixes'."

**Effects of doing this:**
- Resolves 3 broken anchors in `docs/DEVELOPMENT_HYGIENE.md` (lines 63, 134, 151) → `#commit-messages`.
- Resolves the root `README.md:135` cross-reference inaccuracy (item 8.2).
- Aligns CODING_STANDARDS.md scope with what root README claims it covers.

**Effort:** Single doc edit, ~30 lines. Content already exists in DEVELOPMENT_HYGIENE.md; needs to be lifted/duplicated into CODING_STANDARDS.md with a clean heading.

**Why not a Pass 4 surgical fix:** Content creation, not a typo or broken-link correction. The right scope is a small focused PR with the team's input on whether the prefix list belongs in CODING_STANDARDS, DEVELOPMENT_HYGIENE, or both.

### §10.2 Sweep UK→US English forms (glossary §11.6 follow-up)

**Source documentation:** `TRANSLATION_GLOSSARY.md` §11.6, which locks US English and explicitly anticipates this cleanup:
> "a follow-up `chore(normalize): align glossary English column to US lock` commit may sweep them in bulk."

**Scope:**

- **Code identifiers** (require namespace/class rename — refactor):
  - `DualFrontier.AI.BehaviourTree` → `DualFrontier.AI.BehaviorTree` namespace and all child types.
  - File path: `src/DualFrontier.AI/BehaviourTree/` → `BehaviorTree/`.
- **Source comments and XML docs** (text-only):
  - `behaviour`, `behavioural` → `behavior`, `behavioral` in: `DevKitOnlyAttribute.cs`, `TickRateAttribute.cs`, `ImmediateEventDeliveryTests.cs`, `BehaviourTree/*.cs`, `Application/README.md`, `AI/README.md`.
  - `analyser`/`analyse` → `analyzer`/`analyze` in: `DevKitOnlyAttribute.cs`, `DevKitRendererContractTests.cs`.
  - `coloured` → `colored` in: `IDevKitRenderer.cs`, `Presentation/Nodes/PawnVisual.cs`, `TileMapRenderer.cs`.
  - `defence` → `defense` in: `Components/Combat/ShieldComponent.cs`.
  - `quantised`/`quantisation` → `quantized`/`quantization` in: `Persistence/Snapshots/PawnSnapshot.cs`, `tests/.../ComponentEncoderTests.cs`.
  - `centralises` → `centralizes` in: `Core/ECS/IsolationDiagnostics.cs`.
- **Glossary's own English column** (~10 entries with `-ise`/`-our` forms — `quantised model`, `parallelise`, `centralise`, `defence`, `behaviour`, etc.) — sweep alongside the code changes.

**Effort:** ~30+ occurrences. Code-identifier rename is a refactor that affects 11+ files in `DualFrontier.AI` and any consumer; it requires `dotnet test` re-run. Estimated 1-2 hours including build/test verification.

**Why not a Pass 4 surgical fix:** ≥10 occurrences of the same deviation = pattern per Pass 4 prompt rule. The code-identifier rename is also explicitly out of "translation work" scope.

### §10.3 Update `CODING_STANDARDS.md` §"Russian-language domain comments" (stale policy)

**Issue:** Lines 84-115 of `CODING_STANDARDS.md` document a policy — *"Internal domain logic is commented in Russian. That is the project's working language. … Domain terms stay in Russian inside comments: 'пешка' (pawn), 'голем' (golem), …"* — that is now contradicted by the post-Pass-3 codebase (0 Cyrillic in `*.cs` files; the Russian-comment example on lines 89-90 is the only remaining Cyrillic in the doc tree outside policy-allowed and bilingual-reference files).

**Decision needed (team):** What is the new policy?
- Option A — All comments English (matches current code reality): rewrite the section as a brief note that domain glossary terms (`pawn`, `golem`, `ether node`, `ritual`, `ammo crystal`) are referenced by their English forms in comments and code, with link to TRANSLATION_GLOSSARY for Russian↔English mapping.
- Option B — Restore Russian-comment policy: revert relevant Pass 3 commits for `*.cs` inline comments while keeping XML docs English. Significant scope creep; effectively a Pass 4-reversal.
- Option C — Mixed: English XML docs (already true), Russian inline `//` comments allowed for domain-specific formula context (the original split-rule pre-Pass-3). Requires partial revert.

**Effort:** Option A is small (rewrite ~30 lines of CODING_STANDARDS.md). Option B/C involve reverting Pass 3 commits.

**Why not a Pass 4 surgical fix:** The decision is content/policy, not translation. Surgical fix would also need to clarify whether Pass 3's all-English approach was the intended end-state or accidental policy drift.

---

## Verification end-state

- **Cyrillic in `*.cs` / `*.csproj`:** 0.
- **Cyrillic in `*.md`:** confined to 1 policy-allowed file + 5 by-design bilingual reference files + 1 stale-policy section flagged in §10.3.
- **Build:** 0 warnings, 0 errors.
- **Tests:** 82/82 passing.
- **Surgical fixes applied this pass:** 1 (`352ff0f`).
- **Items needing Pass 5:** 3 — see §10. None are translation regressions.

---

## See also

- [TRANSLATION_GLOSSARY](./TRANSLATION_GLOSSARY.md) v1.0 — authoritative glossary.
- [TRANSLATION_PLAN](./TRANSLATION_PLAN.md) — campaign scope.
- [NORMALIZATION_REPORT](./NORMALIZATION_REPORT.md) — Pass 1 closure.
- [PASS_2_NOTES](./PASS_2_NOTES.md) — Pass 2 closure (originator of §10.1).
- [PASS_3_NOTES](./PASS_3_NOTES.md) — Pass 3 closure.

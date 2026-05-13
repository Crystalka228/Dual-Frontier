---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-H-TRANSLATION_PLAN
category: H
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-H-TRANSLATION_PLAN
---
---
title: Translation Plan
nav_order: 100
---

# Dual Frontier translation plan (Russian → English)

*Companion document to [TRANSLATION_GLOSSARY.md](./TRANSLATION_GLOSSARY.md). Describes the translation surface, project-specific risks, the order of operations, and open questions.*

*Version: 0.1 (draft, 2026-04-26).*

---

## 0. Summary

Translating Dual Frontier is **not a linguistic task — it is a distributed edit across three layers at once**: documentation, source code, tests. Concrete example: `IsolationViolationException` formats its message via the template `[ИЗОЛЯЦИЯ НАРУШЕНА]\nСистема '{name}'\nобратилась к '{type}'...`. This format is:

- described in `docs/architecture/ISOLATION.md` as a specification (documentation),
- hardcoded in `SystemExecutionContext.cs` (code),
- used as the `WithMessage("*WrongSystem*HealthComponent*")` argument and the `.Contain("Добавь: [SystemAccess")` assertion in tests.

Any change to that string is a breaking change at three levels. The translation MUST synchronize all three in a single atomic commit, otherwise the build will go red or the diagnostic will diverge from the spec.

That is why the plan is split into phases: first **close the 9 unresolved glossary items**, then translate the "cold" surfaces (the external README, METHODOLOGY), and only at the end touch the "hot" ones — those where code, docs, and tests are tied together.

---

## 1. Inventory: what needs translating

### 1.1 Documentation — `docs/`

| Category | Files | Volume | Priority |
|---|---|---|---|
| Entry points | `README.md` (root), `docs/README.md` | small | **P0** |
| Methodology | `METHODOLOGY.md` | large | **P0** |
| Core architecture | `ARCHITECTURE.md`, `CONTRACTS.md`, `ECS.md`, `EVENT_BUS.md`, `THREADING.md`, `ISOLATION.md` | large | **P1** |
| Integration | `GODOT_INTEGRATION.md`, `VISUAL_ENGINE.md` | medium | **P2** |
| Modding | `MODDING.md`, `MOD_PIPELINE.md` | medium | **P2** |
| Process | `CODING_STANDARDS.md`, `DEVELOPMENT_HYGIENE.md`, `TESTING_STRATEGY.md`, `ROADMAP.md` | medium | **P2** |
| Performance | `PERFORMANCE.md`, `GPU_COMPUTE.md`, `NATIVE_CORE_EXPERIMENT.md` | medium | **P2** |
| v0.2 addendum | `RESOURCE_MODELS.md`, `COMPOSITE_REQUESTS.md`, `FEEDBACK_LOOPS.md`, `COMBO_RESOLUTION.md`, `OWNERSHIP_TRANSITION.md` | medium | **P2** |
| Learning / audit | `learning/PHASE_1.md`, `SESSION_PHASE_4_CLOSURE_REVIEW.md` | large | **P3 + special policy** |
| Persistence | `src/DualFrontier.Persistence/README.md` | medium | **P2** |

### 1.2 README files in code subfolders

The project has a README per assembly and per key package. They are short, but there are many. From the indexing pass:

- `src/DualFrontier.Contracts/Bus/README.md` (contains the stale "five domain buses" — see §3.3)
- `src/DualFrontier.Core/Bus/README.md`
- `tests/DualFrontier.Core.Tests/README.md`

The full list should be collected via `find . -name README.md -not -path "*/node_modules/*"` before translation begins. From experience, there are 10–20 such files.

### 1.3 Source code — XML docs and comments

The code is in a mixed state. Three categories of files are confirmed:

- **Fully English XML docs.** For example, `SystemExecutionContext.cs` (recent code, written in English from scratch).
- **Fully Russian XML docs.** For example, `IsolationViolationException.cs` (the same module, a neighboring file — but Gemma emitted Russian comments because the prompt was partly in Russian).
- **Mixed.** Inline `//` comments in Russian inside a method whose XML docs are English.

This is not a developer bug — it is a footprint of the methodology. Local Gemma picks up the language of its context, and the pipeline did not enforce a single language. After translation this becomes technical debt that must be cleaned up.

### 1.4 User-facing strings in code

The most dangerous category. These are strings that:
- the user / mod author sees at runtime,
- are checked by tests as a black box,
- are described in documentation as a format specification.

Confirmed locations:
- `[ИЗОЛЯЦИЯ НАРУШЕНА]` and the `Добавь: [SystemAccess(...)]` hint in `SystemExecutionContext`. Tests assert `.Contain("Добавь:")`.
- `[IsolationViolationException]` `Прямой доступ к системам запрещён.` — also asserted by tests.
- Messages in `ModFaultHandler` (audit needed).
- The mod-unload UI banner (audit needed — most likely also in Russian).

### 1.5 What does not need translating

- Code names (`DualFrontier.Contracts`, `IModContract`, `EntityId`, `World`, `[SystemAccess]`, etc.).
- Godot/MSBuild configs (`project.godot`, `*.csproj`) — already English.
- `.gitignore`, `LICENSE`, GitHub templates.
- Commit-log files (historical record).

---

## 2. Project-specific risks

### 2.1 The triple coupling "docs ↔ code ↔ tests"

This is the central risk. The format of the isolation-guard messages is not a private detail — it is a contract for the mod author (they fix their code from the hint). The format is:

1. Pinned in `docs/architecture/ISOLATION.md` as "mandatory elements".
2. Generated in `SystemExecutionContext.GetComponent<T>` via interpolation.
3. Verified in `tests/DualFrontier.Core.Tests/Isolation/*Tests.cs` via wildcard substring assertions.

**Translation rule:** changes to this triple are an atomic commit that passes `dotnet test`, with scope prefix `refactor(i18n)` or `chore(i18n)`. No "let's translate the docs now and the tests later" — that yields an immediate red build.

### 2.2 Mixed code-language identity

Some XML docs are English, some Russian, sometimes in neighboring files. After translation, the docs MUST be uniformly English. This requires a sweep over every `src/` file, not just the obvious ones — a cyrillic grep is needed:

```bash
grep -r --include='*.cs' -l '[А-Яа-я]' src/ tests/ mods/
```

This produces the full registry of files with Russian inside.

### 2.3 Stale documentation and outdated READMEs

`src/DualFrontier.Contracts/Bus/README.md` says "five domain buses". The spec (after v0.3 / Phase 4 closure) is **six** — `IPowerBus` was added. The README is stale.

This is content debt, not translation debt. But translation is a convenient moment to close it. **The decision must be made up front:** translate *as is* (preserve historical composition) or translate *with an update* (bring it in line with the current spec). My recommendation is the latter — a stale English doc would still need to be fixed, so do it once.

### 2.4 Audit log and self-teaching artifacts

Two files have a special nature:

- `SESSION_PHASE_4_CLOSURE_REVIEW.md` — an *audit trail*, explicitly marked "not subject to post-hoc editing." It is a session log, not current documentation.
- `learning/PHASE_1.md` — a *self-teaching ritual*, the author's personal learning material, an artifact of the methodology.

Translating these is a methodological question, not a linguistic one. Three policy options:

1. **Do not translate.** They are historical artifacts in their original language, like a poem that is not retranslated for every edition.
2. **Translate with an explicit "Translated from Russian on 2026-XX-XX" header.** Preserve the fact of translation as part of the audit trail.
3. **Duplicate.** Keep the Russian original (`*.ru.md`), add an English version (`*.md`) — common i18n practice.

Recommendation: for `SESSION_*` — option 1 (do not translate, this is audit). For `learning/PHASE_1.md` — option 2 (it is methodologically needed in English, but the fact that this is a translation matters).

### 2.5 Non-standard XML-doc syntax

`SystemExecutionContext.cs` contains `{World}`, `{IModFaultSink}` in curly braces instead of the standard `<see cref="World"/>`. This is the author's personal style (or a side effect of a Gemma prompt). During translation, leave it as is — this is not a russism, it is a non-standard but meaningful syntax.

### 2.6 Domain identifiers in tests

Tests contain strings like `EvilMod`, `GoodMod`, `WrongSystem` — these are fixture-entity identifiers. They are English; do not translate. But the same tests contain localized `WithMessage` patterns that need updating.

---

## 3. Decisions to lock before translation

This is Phase 0. Nothing can be translated in bulk until these 9 items are locked. The glossary already marks them as **⚠ DECISION**; below are my recommendations with rationale.

### 3.1 "Direction owner" → ?

| Variant | Recommendation |
|---|---|
| **intent owner** | Best fit by meaning, but **collides** with the technical `Intent` from `Intent → Granted/Refused`. The glossary (§11.1) author flags the risk. |
| **direction owner** | Recommended as fallback. Does not overlap with project technical terms. Slightly bureaucratic, but normal in the methodology context. |
| **vision owner** | Possible, but carries a managerial flavor. |

**My pick:** `direction owner`. The collision with `Intent` flagged in the glossary is not theoretical — that term appears in every architecture section. The pair "direction owner" + "Intent (resource model)" reads unambiguously.

### 3.2 "Сторож" → `Isolation guard` ✓

The code in `SystemExecutionContext.cs` already says `Isolation guard`. Pin it.

### 3.3 "Аркано-магическая ветка" → `arcane branch` ✓

The English compound `arcane-magical` is redundant. `arcane branch` is enough.

### 3.4 "Эфир" → `ether` ✓

Code uses `EtherGridSystem`, `EtherNodeChanged`, `EtherSurge` — path of least resistance. Switching to `aether` would require renaming identifiers.

### 3.5 "Пешка" → `pawn` ✓

Industry standard for the genre. Keep it.

### 3.6 US vs UK English → **US**

Code already uses American spelling (`Color`, `Behavior`). Documentation must match. Specifically: `behavior`, `color`, `defense`, `quantized`, `analyze`, `fiber`. This decision applies to every translated file.

### 3.7 "Биология рас" → `species biology` or `racial biology`?

A delicate question. In RimWorld lore and similar games the genre vocabulary usually uses `race`. But in social English "race" is loaded. **Recommendation:** `species` for biology-mechanics docs (neutral and more accurate), `race` if the term refers to an in-game concept inside in-game text. The final call is yours, because it is an identity decision about the game, not a vocabulary decision.

### 3.8 "Falsifiable утверждение" → `falsifiable claim` ✓

Popper. Pin it.

### 3.9 Verbal register

Passive → active, as the glossary insists (§11.9). This is not a glossary entry but a style rule. Applied to every translation without exception.

---

## 4. Translation order — a phased plan

### Phase 0: Lock decisions (1–2 days)

1. Close all 9 items from §3 of this document.
2. Release `TRANSLATION_GLOSSARY.md` v1.0 (clear the `draft` status).
3. Run a cyrillic grep (§2.2), produce the full registry of files with Russian content.
4. Make decisions on §2.4 (audit log, self-teaching).

**Exit criterion:** glossary v1.0 finalized, registry ready.

### Phase 1: External interface (1–2 days)

Files that the first repository visitor sees.

1. `README.md` (root) — the project's "face".
2. `docs/README.md` — docs index page.

**Exit criterion:** a person opening the repository for the first time sees an English landing page. Internal links temporarily point to Russian files — that is fine at this step.

### Phase 2: Methodology (2–3 days)

`docs/methodology/METHODOLOGY.md` — the longest and most methodologically important document. The single document that external researchers / readers of a hypothetical project post will definitely read.

**Exit criterion:** METHODOLOGY translated; every internal link (to ARCHITECTURE, CONTRACTS, ISOLATION) points to an English placeholder with a TODO marker. The §-numbering structure is preserved.

### Phase 3: Core architecture (3–5 days)

Order:
1. `ARCHITECTURE.md` — layers and dependencies.
2. `CONTRACTS.md` — six buses, markers.
3. `ECS.md` — World, Entity, Component, System.
4. `EVENT_BUS.md` — Deferred / Immediate / Intent → Granted/Refused.
5. `THREADING.md` — DependencyGraph, phases, TickRates.
6. **`ISOLATION.md` — last in this phase.**

ISOLATION.md goes last on purpose: translating it synchronously changes the `SystemExecutionContext.cs` code and the tests in `tests/DualFrontier.Core.Tests/Isolation/`. This is the riskiest atomic commit of the entire campaign. There must be a stable base before it.

**Exit criterion:** all 6 files translated, all 82 tests green, the isolation-guard message format uniformly English.

### Phase 4: Remaining documentation (3–4 days)

All P2 documents from the §1.1 inventory. In any order. No special risks; pure translation.

### Phase 5: Code-level cleanup (2–3 days)

1. Cyrillic grep sweep, translate every Russian XML doc and inline comment.
2. Translate every README in `src/`, `tests/`, `mods/` subfolders.
3. Audit Russian UI strings (banners, user messages), translate.
4. Audit future commit messages (English by default — already in `CODING_STANDARDS.md`).

**Exit criterion:** `grep -r --include='*.cs' '[А-Яа-я]' src/ tests/ mods/` returns an empty result (or only intentionally preserved strings).

### Phase 6: Audit-log policy (1 day)

Apply the §2.4 decision to `SESSION_PHASE_4_CLOSURE_REVIEW.md` and `learning/PHASE_1.md`. This is a separate policy, not the main translation pass.

**Total estimate:** ~12–18 days at one phase per 1–3 days. Using the methodology's pipeline (Sonnet as translator + Opus as QA) can speed it up, but adds a verification cycle per phase.

---

## 5. Code-level specifics

### 5.1 Error-message asserts — pattern

In `tests/DualFrontier.Core.Tests/Isolation/*` you see:

```csharp
.Should().Throw<IsolationViolationException>()
    .WithMessage("*WrongSystem*HealthComponent*")
    .Which.Message.Should().Contain("Добавь: [SystemAccess");
```

After translation it becomes:

```csharp
.Should().Throw<IsolationViolationException>()
    .WithMessage("*WrongSystem*HealthComponent*")
    .Which.Message.Should().Contain("Add: [SystemAccess");
```

The wildcard pattern `*WrongSystem*HealthComponent*` stays alive — system and type names remain English. Only the localized hint fragment changes.

**Tip:** during translation, introduce in `IsolationViolationException` (or a dedicated `IsolationDiagnostics` class) a set of constants for the localized prefixes. Then tests assert on a constant rather than a literal, and future text edits do not break the tests:

```csharp
public static class IsolationDiagnostics
{
    public const string ViolationHeader = "[ISOLATION VIOLATED]";
    public const string AddSystemAccessHint = "Add: [SystemAccess";
    // ...
}
```

This is an architectural improvement that translation naturally motivates. It can be done within Phase 3 as part of the atomic commit "refactor(diagnostics): centralise isolation messages + i18n".

### 5.2 Code identifiers are never touched

Obvious, but pinned in the plan: `EtherGridSystem`, `ManaIntent`, `[SystemAccess]`, `IGameServices.Combat`, `nameof(IGameServices.Power)` — everything stays as is. Only text strings are touched.

### 5.3 Commit scope-prefix ritual

From `CODING_STANDARDS.md`: every commit has a scope prefix. For translation I suggest a single scope:

- `chore(i18n): translate METHODOLOGY.md` — for documentation.
- `refactor(i18n): translate isolation guard messages` — for code.
- `test(i18n): update isolation assertions to English messages` — for tests.

This makes it possible later to run `git log --grep "i18n"` and get a clean history of the entire translation campaign.

---

## 6. Open questions — your decisions needed

1. **Direction owner or intent owner?** My recommendation is `direction owner` because of the collision. Agreed?

2. **Stale docs in subfolders:** update during translation or preserve historical state? (§2.3)

3. **`SESSION_PHASE_4_CLOSURE_REVIEW.md` and `learning/PHASE_1.md`:** what policy? (§2.4)

4. **`race` vs `species`:** does the lore have a "races within a species" concept (multiple human races alongside a separate species)? If not — `species` is cleaner. If yes — both terms are needed with explicit boundaries.

5. **Standardize XML docs on `<see cref="..."/>` or keep `{Identifier}`?** This is a side question that surfaces during translation. I recommend standardizing — but that is no longer translation, it is architectural debt.

6. **Translation as a methodology experiment?** It is natural to the project's spirit: you can watch how the pipeline (Gemma as executor, Sonnet as prompt generator and translator, Opus as QA) handles translation. That gives one more empirical measurement of the methodology — but stretches the process. The decision is yours, depending on whether speed or an extra methodological result matters more.

7. **Versioning during translation:** does the translation ship as a single `feat(i18n)` branch, or as small per-phase PRs into `main`? I recommend per-phase — every completed phase yields a green build, easier to review.

---

## 7. See also

- [TRANSLATION_GLOSSARY.md](./TRANSLATION_GLOSSARY.md) — glossary and lock-decisions.
- [METHODOLOGY.md](/docs/methodology/METHODOLOGY.md) — project methodology; documentation translation can be seen as its applied use case.
- [CODING_STANDARDS.md](/docs/methodology/CODING_STANDARDS.md) — commit-prefix and naming rules.
- [DEVELOPMENT_HYGIENE.md](/docs/methodology/DEVELOPMENT_HYGIENE.md) — PR checklist; for an i18n PR it is worth adding a "cyrillic check: grep returned empty" item.

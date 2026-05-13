---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-NORMALIZATION_REPORT
category: E
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-NORMALIZATION_REPORT
---
---
title: Normalization Report — Pass 1 of Translation Campaign
nav_order: 101
---

# Normalization Report — Pass 1 (2026-04-26)

**Status:** complete. Pass 2 may begin once the human resolves the two escalations recorded in §6.

**Scope.** Russian source remains in Russian. Pass 1 made the source self-consistent so Pass 2 (documentation translation) and Pass 3 (code translation) can operate deterministically without architectural decisions. Concretely Pass 1 (a) locked all `⚠ DECISION` markers in `TRANSLATION_GLOSSARY.md` to `✓ LOCKED v1.0` or `⚠ ESCALATE TO HUMAN`, (b) audited terminology drift in `docs/`, (c) audited stale specifications in code-adjacent READMEs, (d) inventoried Russian content in `.cs` files, (e) registered user-facing string contracts, and (f) specified the `IsolationDiagnostics` refactor that Pass 3 will execute.

**Companion artifacts:**
- [TRANSLATION_GLOSSARY.md](./TRANSLATION_GLOSSARY.md) — v1.0, locked.
- [TRANSLATION_PLAN.md](./TRANSLATION_PLAN.md) — campaign plan, unchanged by Pass 1.
- [ISOLATION.md](/docs/architecture/ISOLATION.md) — referenced for §15.5 banner alignment and §15.3/§15.4 doc-only specifications.

---

## §1 — Lexical drift findings

**Result: no drift detected. Zero `chore(normalize)` commits required for this section.**

**Method.** For each canonical Russian term in the glossary tables, brainstormed 3–5 likely Russian synonyms that could have crept into other docs, then ran case-insensitive recursive grep across `docs/` for each synonym. For any match, classified as either real drift (synonym refers to the same concept) or distinct concept (legitimate use of a different word).

**Coverage.**

| Canonical term | Synonyms checked | Findings |
|---|---|---|
| `сторож` (isolation guard) | `хранитель`, `контролёр`, `защитник`, `охранник`, `наблюдатель` | Zero drift. All 17 occurrences across `docs/` use `сторож` / `сторожем` / `стороже`. |
| `шина` (domain bus) | `канал` (in bus context), `магистраль`, `трасса`, `поток` (as bus, not thread) | Zero drift. `канал` appears in legitimate distinct uses (`канал заклинания` in RESOURCE_MODELS, `канал атаки` in security context, `канал связи` in METHODOLOGY) — none refer to the domain-bus concept. `шина` / `шин` / `шинам` used uniformly for the bus concept. |
| `пешка` (pawn) | `колонист`, `юнит`, `персонаж` | `персонаж` does not appear; `колонист` does not appear; `юнит` does not appear. Uniform `пешка`. |
| `эфир` (ether) | `астрал`, `flux`, `мана` (as confusion check) | Distinct concepts: `мана` is the resource-model peer (separate, intentional). No drift. |

**Verdict.** The Russian source is lexically self-consistent for every term audited. Pass 2 can rely on the glossary as a 1-to-1 mapping; no upstream normalization commits are needed.

---

## §2 — Stale specifications

Two stale specs fixed by Pass 1 (atomic commits per file). Three additional occurrences are historical references and intentionally preserved.

### §2.1 Fixed: `src/DualFrontier.Contracts/Bus/README.md`

**Original claim (line 4–5):**
> Определяет контракт базовой шины событий `IEventBus` и пять доменных шин (Combat, Inventory, Magic, Pawn, World), собранных в агрегатор `IGameServices`.

**Reality.** Six `I*Bus` interfaces exist in `src/DualFrontier.Contracts/Bus/`:

| Interface | XML-doc summary |
|---|---|
| `ICombatBus` | shooting, damage, death |
| `IInventoryBus` | ammo requests, item add/remove |
| `IMagicBus` | mana requests, spell cast, ether surge |
| `IPawnBus` | mood break, death reaction, skill gain |
| `IPowerBus` | converter power output (added in TechArch v0.3 §13.1) |
| `IWorldBus` | ether-node change, weather change, raid incoming |

**Fix applied.** Updated line 4 to `шесть доменных шин (Combat, Inventory, Magic, Pawn, Power, World)`. Added `IPowerBus.cs` to the `## Что внутри` section. Updated `## TODO` checkboxes for Phase 1 from `[ ]` to `[x]` (Phase 1 closed long ago — TODO list was also stale) and aligned the count there from `пяти шин` to `шести шин`.

**Commit:** `chore(normalize): align Contracts/Bus README to six-bus reality`.

### §2.2 Fixed: `src/DualFrontier.Core/Bus/README.md`

**Original claim (line 16):**
> `GameServices.cs` — композиция пяти доменных шин, реализует `IGameServices`.

**Fix applied.** Updated to `композиция шести доменных шин (Combat, Inventory, Magic, Pawn, Power, World), реализует IGameServices`. Made the bus enumeration explicit so future stale-count drift is obvious.

**Commit:** `chore(normalize): align Core/Bus README to six-bus reality`.

### §2.3 Preserved as historical: `docs/architecture/ARCHITECTURE.md:9`

**Text:** `**v0.1 (2026-03):** Исходный каркас: четыре слоя, пять доменных шин, декларативная изоляция, параллельный планировщик.`

**Reason.** This is a versioned changelog entry describing **what v0.1 contained**. The post-v0.3 reality (six buses) is captured in subsequent v0.2/v0.3 changelog entries elsewhere in the same document. Editing this line would falsify the historical record. Left as-is.

### §2.4 Preserved as historical: `docs/learning/PHASE_1.md:116` and `:370`

**Text 1 (line 116):** `| IGameServices | Агрегатор 5 шин: Combat, Inventory, Magic, Pawns, World. | GameServices в Core |`

**Text 2 (line 370):** `В Dual Frontier 5 доменных шин: Combat, Inventory, Magic, Pawns, World. Каждая — отдельный экземпляр DomainEventBus. ...`

**Reason.** `learning/PHASE_1.md` is classified by `TRANSLATION_PLAN.md` §2.4 as a **self-teaching ритуал, личный учебный материал автора, артефакт методики** — explicitly a non-current learning artifact tied to Phase 1's snapshot of the codebase. Retroactively editing it to "6" would falsify the author's learning history and contradict the audit-log policy proposed in `TRANSLATION_PLAN.md` §2.4 (variant 1: don't translate, don't retroactively edit). Left as-is.

### §2.5 Other audited READMEs — clean

The agents enumerated every `README.md` under `src/`, `tests/`, `mods/`. No other quantitative claims about code structure (component counts, system counts, etc.) were found to mismatch reality. The `src/DualFrontier.Events/README.md` already lists all six event domains (Combat, Magic, Inventory, Power, Pawn, World) correctly.

---

## §3 — Cyrillic inventory in `.cs` files

**Total: 148 `.cs` files contain Cyrillic content** (136 under `src/`, 11 under `tests/`, 1 under `mods/`). All Cyrillic is in comments / XML-docs / string literals — **no Cyrillic in identifiers**. Verified by `Grep -E '[А-Яа-я]' --glob '*.cs'`.

**Classification key:**
- **(a)** Full Russian XML-doc comments (`/// <summary>...`) on classes/methods.
- **(b)** Only Russian inline `//` comments inside method bodies.
- **(c)** Russian user-facing string literals (exception messages, log messages, UI text).
- **(d)** Mixed (combination of two or more of the above).

### §3.1 Summary by area

| Area | File count | Dominant category | Notes |
|---|---|---|---|
| `src/DualFrontier.Contracts/` | 25 | (a) | Bus/Core/Modding/Attributes interfaces; XML-docs describe contract intent in Russian. |
| `src/DualFrontier.Core/` | 13 | (a), (c) for ECS | `SystemExecutionContext.cs` is (d): English XML-doc + Russian violation strings (§4). |
| `src/DualFrontier.Components/` | 16 | (a) | Component records with Russian summaries. |
| `src/DualFrontier.Events/` | 39 | (a) | Event records with Russian summaries. |
| `src/DualFrontier.Systems/` | 25 | (a) | System classes with Russian summaries; behavior comments. |
| `src/DualFrontier.AI/` | 11 | (a) | Behaviour-tree leaves and jobs. |
| `src/DualFrontier.Application/` | 22 | (a) | Bridge commands, modding pipeline, save system. |
| `src/DualFrontier.Presentation/` | 7 | (a), (c) for UI | `AlertPanel.cs`, `BuildMenu.cs` may have UI banner strings — needs spot-check during Pass 3. |
| `src/DualFrontier.Persistence/` | 0 | — | All-English XML-docs. |
| `src/DualFrontier.Mod.Example/` | (in mods) | (a) | Example mod doc comments. |
| `tests/DualFrontier.Core.Tests/` | 4 | (a), (c) for assertions | `IsolationGuardTests.cs` (d): English code + Russian substring assertions (§4). |
| `tests/DualFrontier.Systems.Tests/` | 4 | (a) | Comments and arrange/act/assert labels in Russian. |
| `tests/DualFrontier.Modding.Tests/` | 3 | (a) | Pipeline tests with Russian comments. |
| `mods/DualFrontier.Mod.Example/` | 1 | (a) | Example mod implementation. |

### §3.2 Critical files (category c — user-facing strings)

These three files contain Russian **string literals** that affect runtime behavior or test contracts. They are the highest-priority targets for Pass 3 and are individually registered in §4.

| File | Methods with Russian strings | Test impact |
|---|---|---|
| `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` | `GetSystem`, `BuildReadViolationMessage`, `BuildWriteViolationMessage` | Asserted by `IsolationGuardTests.cs` substring patterns |
| `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` | `BuildWriteConflictException`, `BuildCycleException` (both have a Russian `Resolve:` line) | Asserted by `DependencyGraphTests.cs` (substring scope to be verified in Pass 3 prep) |
| `src/DualFrontier.Core/ECS/IsolationViolationException.cs` | None — only XML-doc is Russian; no string emission | (a) only, not (c) |

### §3.3 Files with Russian inline comments (category b/d) — sample

The 148-file inventory is dominated by category (a) — XML-doc summaries on type declarations. A representative sample of category (b) / (d) files where method-body comments are Russian:

| File | Excerpt |
|---|---|
| `src/DualFrontier.Core/Scheduling/TickRates.cs` | `/// Раз в 60 тиков (~1 раз/сек) — нужды, настроение, рост эфира.` |
| `src/DualFrontier.AI/Jobs/JobCast.cs` | `/// Джоб каста заклинания: подойти на дистанцию → начать каст →` |
| `src/DualFrontier.Application/Bridge/Commands/PawnDiedCommand.cs` | `/// Команда: пешка скончалась` |
| `src/DualFrontier.Components/Pawn/NeedKind.cs` | `/// <summary>Типы потребностей пешки.</summary>` |
| `src/DualFrontier.Events/Magic/ManaIntent.cs` | `/// Намерение выделить ман` |

### §3.4 Inventory-only — no edits in Pass 1

Per the task scope ("only inventorying, does not translate"), no `.cs` file was modified in Pass 1. The full 148-file list is the input registry for Pass 3 (`refactor(i18n): translate XML-docs and inline comments — area X`).

---

## §4 — User-facing string contracts

Five contracts identified. **Three are emitted by current code** (must be translated atomically with their tests in Pass 3). **Two are doc-only specifications** in `docs/architecture/ISOLATION.md` for future runtime checks not yet implemented.

### §4.1 Read-isolation violation (implemented, asserted)

**Russian template (current code):**
```
[ИЗОЛЯЦИЯ НАРУШЕНА]
Система '{systemName}'
обратилась к '{componentType}'
без декларации доступа.
Добавь: [SystemAccess(reads: new[]{typeof({componentType})})]
```

| Slot | Reference |
|---|---|
| Code | `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` lines 282–292, method `BuildReadViolationMessage(Type)` |
| Doc | `docs/architecture/ISOLATION.md` §"Обращение к незадекларированному компоненту" lines 116–142, §"Формат сообщений" lines 185–202 |
| Tests | `tests/DualFrontier.Core.Tests/Isolation/IsolationGuardTests.cs`: `GetComponent_Undeclared_Throws_ForCore` lines 50–54 (`.ContainAll("TestHealthComponent", "без декларации", "Добавь: [SystemAccess")`); `ModOrigin_Violation_ReportsToSink` line 123 (`.ContainAll("TestHealthComponent", "без декларации")`) |
| Glossary lock | §15.1a, locked v1.0 |

### §4.2 Write-isolation violation (implemented, asserted)

**Russian template (current code):**
```
[ИЗОЛЯЦИЯ НАРУШЕНА]
Система '{systemName}'
модифицирует '{componentType}'
без декларации записи.
Добавь: [SystemAccess(writes: new[]{typeof({componentType})})]
```

| Slot | Reference |
|---|---|
| Code | `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` lines 294–304, method `BuildWriteViolationMessage(Type)` |
| Doc | `docs/architecture/ISOLATION.md` §"Формат сообщений" lines 185–202 (general format); no write-specific example shown — recommend Pass 2 add a parallel example block |
| Tests | `tests/DualFrontier.Core.Tests/Isolation/IsolationGuardTests.cs`: `SetComponent_Undeclared_Throws` lines 82–86 (`.ContainAll("TestHealthComponent", "модифицирует", "Добавь: [SystemAccess")`) |
| Glossary lock | §15.1b, locked v1.0 |

### §4.3 Direct system access (implemented, asserted)

**Russian template (current code):**
```
[IsolationViolationException]
Прямой доступ к системам запрещён.
Используй EventBus вместо прямой ссылки на систему.
```

| Slot | Reference |
|---|---|
| Code | `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` lines 256–262, method `GetSystem<TSystem>()` (always throws, even in Release) |
| Doc | `docs/architecture/ISOLATION.md` §"Прямой доступ к другой системе" lines 146–162 |
| Tests | `tests/DualFrontier.Core.Tests/Isolation/IsolationGuardTests.cs`: `GetSystem_AlwaysThrows` line 99 (`.Contain("Прямой доступ к системам запрещён")`) |
| Glossary lock | §15.2, locked v1.0 |

### §4.4 Direct World access (doc-only, not yet emitted)

Documented in `docs/architecture/ISOLATION.md` lines 168–172 as the intended runtime diagnostic when a system bypasses `SystemBase` to call `World.GetComponentUnsafe` directly. **No corresponding code exists** — Pass 1 verified by reading `SystemExecutionContext` end-to-end. A future phase implements the check; the wording is pre-locked in glossary §15.3 so doc and code converge when implementation lands.

### §4.5 Wrong-bus publishing (doc-only, not yet emitted)

Documented in `docs/architecture/ISOLATION.md` lines 178–183 as the intended runtime diagnostic when a system publishes to a bus it has not declared. **No corresponding code exists** — `SystemExecutionContext` captures `_allowedBuses` but never validates against an actual `Publish` call. A future phase wires the check; wording pre-locked in glossary §15.4.

### §4.6 Mod fault banner (doc-only, not yet emitted)

Documented in `docs/architecture/ISOLATION.md` lines 86–90 as the player-facing UI banner shown when a mod is unloaded. **No corresponding code exists** — `ModFaultHandler` (the entire class described in `ISOLATION.md` §"ModFaultHandler — жизненный цикл при нарушении мода") is itself a specification, not yet implemented. Pass 1 corrected the v0.2 glossary draft's "presumed" Russian to match the verbose form actually documented in ISOLATION.md. Wording pre-locked in glossary §15.5.

### §4.7 Scheduler diagnostics (implemented, partial Russian)

`src/DualFrontier.Core/Scheduling/DependencyGraph.cs` emits two diagnostics with **bilingual content** — English headers + Russian `Resolve:` hints:

```
[SCHEDULER ERROR] Write conflict:
  ...
Resolve: одна из систем должна читать, не писать, или разнести по фазам через [Deferred] событие.
```

```
[SCHEDULER ERROR] Cyclic dependency detected:
  ...
Resolve: разорви цикл через [Deferred] событие.
```

| Slot | Reference |
|---|---|
| Code | `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` lines 225–235 (`BuildWriteConflictException`), lines 238–259 (`BuildCycleException`) |
| Doc | `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` line 21 references `docs/architecture/THREADING.md` for format spec — Pass 2 should verify what THREADING.md actually documents |
| Tests | `tests/DualFrontier.Core.Tests/Scheduling/DependencyGraphTests.cs` — substring assertion patterns to be enumerated during Pass 3 prep (out of Pass 1 inventory scope) |
| Glossary lock | **Not yet in §15.** Recommend Pass 2 add §15.6 (Scheduler diagnostics) before Pass 3 begins, modeled on §15.1a/§15.1b. |

This contract is **out of scope for the IsolationDiagnostics refactor specified in §5** — see §5.G for the rationale (separate `SchedulerDiagnostics` class, separate atomic commit in a later phase).

---

## §5 — IsolationDiagnostics refactoring specification

**Purpose.** Centralize every user-facing diagnostic substring emitted by `SystemExecutionContext` into a constant pool so Pass 3 can flip values Russian → English in exactly one place without breaking tests. **Pass 1 specifies; Pass 3 implements.** Not a single line of `.cs` is touched in Pass 1.

### §5.A New file specification

**Path:** `src/DualFrontier.Core/ECS/IsolationDiagnostics.cs`

```csharp
namespace DualFrontier.Core.ECS;

/// <summary>
/// Constant pool for user-facing diagnostic substrings emitted by
/// <see cref="SystemExecutionContext"/>. Centralises every literal that
/// <see cref="SystemExecutionContext"/> composes into violation messages
/// and that tests assert against, so the wording can be evolved (e.g.
/// translated) in exactly one place without breaking call sites or tests.
///
/// Members are pure substring tokens. Dynamic fragments — system name,
/// component type name — stay in the StringBuilder construction at the
/// emission site; they are not encoded here.
///
/// Constraint: this type is a constant pool. No methods, no logic, no
/// instances. Adding behaviour here is a code-review-blocking violation.
/// </summary>
internal static class IsolationDiagnostics
{
    // ... constants below
}
```

**Visibility:** `internal static class`; each member `public const string`. Tests already access internals of `DualFrontier.Core` (the existing `internal sealed class TestHealthComponent` in the test fixture confirms `InternalsVisibleTo` is wired). Visibility escalation to `public` is unnecessary.

### §5.B Constant inventory

Two layers: **emission constants** (the StringBuilder feeds these into the message text) and **test-contract tokens** (stable substrings tests assert against). The emission constants are *built to contain* the corresponding tokens — Pass 3's English replacements must preserve the substring relationship, or the assertion-containment test in §5.F.4 fails.

#### Emission constants — read-violation message

| Constant | Current Russian value |
|---|---|
| `ViolationHeader` | `"[ИЗОЛЯЦИЯ НАРУШЕНА]"` |
| `SystemPrefix` | `"Система '"` |
| `SystemSuffix` | `"'"` |
| `ReadVerb` | `"обратилась к '"` |
| `ComponentSuffix` | `"'"` |
| `ReadReason` | `"без декларации доступа."` |
| `HintPrefix` | `"Добавь: [SystemAccess("` |
| `ReadHintArgPrefix` | `"reads: new[]{typeof("` |
| `HintArgSuffix` | `")})]"` |

#### Emission constants — write-violation message (reuses ViolationHeader, SystemPrefix/Suffix, ComponentSuffix, HintPrefix, HintArgSuffix; adds:)

| Constant | Current Russian value |
|---|---|
| `WriteVerb` | `"модифицирует '"` |
| `WriteReason` | `"без декларации записи."` |
| `WriteHintArgPrefix` | `"writes: new[]{typeof("` |

#### Emission constants — direct-system-access message

| Constant | Current Russian value |
|---|---|
| `GetSystemHeader` | `"[IsolationViolationException]"` |
| `GetSystemBody` | `"Прямой доступ к системам запрещён."` |
| `GetSystemHint` | `"Используй EventBus вместо прямой ссылки на систему."` |

#### Test-contract tokens (the four substrings tests already assert on)

| Constant | Value | Must be a substring of |
|---|---|---|
| `UndeclaredAccessToken` | `"без декларации"` | `ReadReason`, `WriteReason` |
| `WriteVerbToken` | `"модифицирует"` | `WriteVerb` |
| `HintToken` | `"Добавь: [SystemAccess"` | `HintPrefix` |
| `DirectSystemAccessToken` | `"Прямой доступ к системам запрещён"` | `GetSystemBody` |

### §5.C Mapping — replacements in `SystemExecutionContext.cs`

Pass 3 will swap every literal in the table below for `IsolationDiagnostics.<Constant>`. StringBuilder call shape preserved exactly.

| Method | Line | Current literal | Replacement |
|---|---|---|---|
| `GetSystem<TSystem>` | 259 | `"[IsolationViolationException]"` | `IsolationDiagnostics.GetSystemHeader` |
| `GetSystem<TSystem>` | 260 | `"Прямой доступ к системам запрещён."` | `IsolationDiagnostics.GetSystemBody` |
| `GetSystem<TSystem>` | 261 | `"Используй EventBus вместо прямой ссылки на систему."` | `IsolationDiagnostics.GetSystemHint` |
| `BuildReadViolationMessage` | 285 | `"[ИЗОЛЯЦИЯ НАРУШЕНА]"` | `IsolationDiagnostics.ViolationHeader` |
| `BuildReadViolationMessage` | 286 | `"Система '"`, `"'"` | `SystemPrefix`, `SystemSuffix` |
| `BuildReadViolationMessage` | 287 | `"обратилась к '"`, `"'"` | `ReadVerb`, `ComponentSuffix` |
| `BuildReadViolationMessage` | 288 | `"без декларации доступа."` | `ReadReason` |
| `BuildReadViolationMessage` | 289 | `"Добавь: [SystemAccess("`, `"reads: new[]{typeof("` | `HintPrefix`, `ReadHintArgPrefix` |
| `BuildReadViolationMessage` | 290 | `")})]"` | `HintArgSuffix` |
| `BuildWriteViolationMessage` | 297 | `"[ИЗОЛЯЦИЯ НАРУШЕНА]"` | `ViolationHeader` |
| `BuildWriteViolationMessage` | 298 | `"Система '"`, `"'"` | `SystemPrefix`, `SystemSuffix` |
| `BuildWriteViolationMessage` | 299 | `"модифицирует '"`, `"'"` | `WriteVerb`, `ComponentSuffix` |
| `BuildWriteViolationMessage` | 300 | `"без декларации записи."` | `WriteReason` |
| `BuildWriteViolationMessage` | 301 | `"Добавь: [SystemAccess("`, `"writes: new[]{typeof("` | `HintPrefix`, `WriteHintArgPrefix` |
| `BuildWriteViolationMessage` | 302 | `")})]"` | `HintArgSuffix` |

The English-already `"already set"` substring at line 127 (`PushContext` nested-push detection) and its test counterpart at `IsolationGuardTests.cs` line 137 are out of scope (already English, never translated).

### §5.D Test-assertion replacements

Pass 3 swaps the literal substrings in `IsolationGuardTests.cs` for constant references. The test file already imports `DualFrontier.Core.ECS` (line 4) — no additional `using` needed.

| Test | Line | Before | After |
|---|---|---|---|
| `GetComponent_Undeclared_Throws_ForCore` | 51–54 | `.ContainAll("TestHealthComponent", "без декларации", "Добавь: [SystemAccess")` | `.ContainAll(nameof(TestHealthComponent), IsolationDiagnostics.UndeclaredAccessToken, IsolationDiagnostics.HintToken)` |
| `SetComponent_Undeclared_Throws` | 83–86 | `.ContainAll("TestHealthComponent", "модифицирует", "Добавь: [SystemAccess")` | `.ContainAll(nameof(TestHealthComponent), IsolationDiagnostics.WriteVerbToken, IsolationDiagnostics.HintToken)` |
| `GetSystem_AlwaysThrows` | 99 | `.Contain("Прямой доступ к системам запрещён")` | `.Contain(IsolationDiagnostics.DirectSystemAccessToken)` |
| `ModOrigin_Violation_ReportsToSink` | 123 | `.ContainAll("TestHealthComponent", "без декларации")` | `.ContainAll(nameof(TestHealthComponent), IsolationDiagnostics.UndeclaredAccessToken)` |

### §5.E Refactored method skeletons (Pass 3 implements)

```csharp
public TSystem GetSystem<TSystem>() where TSystem : SystemBase
{
    throw new IsolationViolationException(
        IsolationDiagnostics.GetSystemHeader + Environment.NewLine +
        IsolationDiagnostics.GetSystemBody + Environment.NewLine +
        IsolationDiagnostics.GetSystemHint);
}

private string BuildReadViolationMessage(Type componentType)
{
    var sb = new StringBuilder();
    sb.Append(IsolationDiagnostics.ViolationHeader).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.SystemPrefix).Append(_systemName)
      .Append(IsolationDiagnostics.SystemSuffix).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.ReadVerb).Append(componentType.Name)
      .Append(IsolationDiagnostics.ComponentSuffix).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.ReadReason).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.HintPrefix)
      .Append(IsolationDiagnostics.ReadHintArgPrefix)
      .Append(componentType.Name)
      .Append(IsolationDiagnostics.HintArgSuffix);
    return sb.ToString();
}

private string BuildWriteViolationMessage(Type componentType)
{
    var sb = new StringBuilder();
    sb.Append(IsolationDiagnostics.ViolationHeader).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.SystemPrefix).Append(_systemName)
      .Append(IsolationDiagnostics.SystemSuffix).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.WriteVerb).Append(componentType.Name)
      .Append(IsolationDiagnostics.ComponentSuffix).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.WriteReason).Append(Environment.NewLine);
    sb.Append(IsolationDiagnostics.HintPrefix)
      .Append(IsolationDiagnostics.WriteHintArgPrefix)
      .Append(componentType.Name)
      .Append(IsolationDiagnostics.HintArgSuffix);
    return sb.ToString();
}
```

### §5.F Verification approach for Pass 3 commit

1. **Test parity.** `dotnet test tests/DualFrontier.Core.Tests` — all eight tests in `IsolationGuardTests` remain green; same count as before refactor; no skips.
2. **Byte-identical emitted strings.** Refactor concatenates the same constant character sequences in the same order with the same `Environment.NewLine` separators. Optional one-shot script (not committed): construct a `SystemExecutionContext`, trigger each violation path, capture `ex.Message`, ordinal-compare to a snapshot from `git show HEAD~1:src/DualFrontier.Core/ECS/SystemExecutionContext.cs`. Must compare equal.
3. **Atomic commit.** `git log --oneline -- src/DualFrontier.Core/ECS/IsolationDiagnostics.cs src/DualFrontier.Core/ECS/SystemExecutionContext.cs tests/DualFrontier.Core.Tests/Isolation/IsolationGuardTests.cs` shows exactly one commit. No other file touched.
4. **Substring containment invariant.** Add a small test (or manual review) that asserts `IsolationDiagnostics.ReadReason.Contains(IsolationDiagnostics.UndeclaredAccessToken)`, `WriteReason.Contains(UndeclaredAccessToken)`, `WriteVerb.Contains(WriteVerbToken)`, `HintPrefix.Contains(HintToken)`, `GetSystemBody.Contains(DirectSystemAccessToken)`. This guards Pass 3 from English rewordings that accidentally drop the substring relationship — the assertion fails at test time before the broken translation lands.
5. **Zero behavior change.** `dotnet build` produces no new warnings; full solution `dotnet test` count unchanged.

### §5.G Out of scope

This spec covers only **isolation guard** diagnostics emitted by `SystemExecutionContext`. The scheduler-side diagnostics in `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` (write conflict, cycle messages — see §4.7) follow the same centralization pattern but belong to a different concern. They will be extracted into a sibling class `SchedulerDiagnostics` under the same folder in a separate commit. Bundling them here would couple two independently-evolving rewordings into one PR and inflate test churn. Pass 1 specification for `SchedulerDiagnostics` is a separate follow-up; Pass 3 translation of those messages is a separate atomic commit.

---

## §6 — Open decisions for human

Pass 2 cannot begin bulk translation of methodology / architecture / pawn-biology documents until both items below are resolved. Both are identity / lore decisions, not lexical ones — outside the authority of this Pass.

### §6.1 Owner-role label (`владелец смысла`)

**Glossary lock:** `⚠ ESCALATE TO HUMAN` (§11.1).

**Question.** What English label should describe the human's role in the four-agent pipeline?

**Options for the human:**
1. **`direction owner`** — methodologically clean, no collision with the `Intent` resource model; reframes the role from "owner of the meaning of work" to "owner of long-term direction". Pass 1's recommendation.
2. **`intent owner`** — preserves the «владелец X» structure; reads as "owner of an Intent message" in any architecture context due to collision with `Intent → Granted/Refused`. Requires Pass 2 to disambiguate by context.
3. **A different candidate** — e.g. `meaning steward`, `vision owner`. Not Pass 1's preference (philosophical / managerial register), but available.

**Why Pass 1 didn't decide.** This is a self-naming choice for the project author; Pass 1 has no authority over identity decisions. The collision risk is verified, not theoretical, so neither option is purely lexical.

**Blocks:** `METHODOLOGY.md` translation, every `learning/` document, every doc that introduces the four-agent pipeline.

### §6.2 Race vs species (`биология рас`)

**Glossary lock:** `⚠ ESCALATE TO HUMAN` (§11.7).

**Question.** Does the lore have one taxonomic level (humans, dwarves, golems all distinct peoples) or two (humans subdivide into races; dwarves are a separate species)?

**Options for the human:**
1. **One taxonomic level** → lock as `species` everywhere (neutral, biologically accurate, avoids social-discourse loading on `race`).
2. **Two taxonomic levels** → both terms needed: `species` for the cross-people level, `race` for intra-species variation, with explicit glossary entries clarifying the distinction.

**Why Pass 1 didn't decide.** Pass 1 has no access to lore documents that would settle which model the project uses. `RaceComponent.cs` exists in the codebase but its name does not, alone, disambiguate the two models.

**Blocks:** `RaceComponent.cs` XML-doc translation, the `биология рас` paragraph in `METHODOLOGY.md`, any future race / species game-mechanic documentation.

---

## Pass 1 self-check

- [x] `TRANSLATION_GLOSSARY.md` contains zero `⚠ DECISION` entry markers (only `✓ LOCKED v1.0` and `⚠ ESCALATE TO HUMAN`). Two remaining textual mentions on line 14 (version-history paragraph) and lines 698–699 (§16.1 lifecycle description) describe the *marker mechanism itself* — they are not entries.
- [x] `NORMALIZATION_REPORT.md` has all six sections filled with substantive content; no `TODO` or placeholder strings.
- [x] `dotnet build` clean, `dotnet test` 82/82 — verified after Pass 1 commits (recorded in commit messages).
- [x] `git log --oneline` for Pass 1 shows only `chore(normalize)`-prefixed commits (one per artifact: glossary lock, two README fixes, this report).

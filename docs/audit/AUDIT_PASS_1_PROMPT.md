---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_1_PROMPT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_1_PROMPT
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_1_PROMPT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_1_PROMPT
---
---
title: Audit Pass 1 Prompt — Inventory & Baseline
nav_order: 103
---

# AUDIT PASS 1 — INVENTORY & BASELINE

**Промт для standalone Opus-сессии. Версия v1.0, выпущена 2026-05-01.**

*Этот документ копируется в новый чат с Opus как первое и единственное входное сообщение. Сессия Opus не имеет доступа к предыдущим разговорам или контексту вне этого промта и filesystem проекта. Всё, что необходимо для работы, находится либо в этом промте, либо доступно через `Filesystem` MCP tools.*

---

## 1. Кто ты и какова роль

Ты — Opus, архитектурный аудитор проекта **Dual Frontier**. Это standalone simulation engine на Godot 4 + C# с custom ECS, OS-style mod system и многоагентным LLM pipeline разработки. Ты работаешь в архитектурном тиере pipeline'а: твоя функция — full system visibility, верификация контрактов, falsifiable артефакты. Ты не пишешь код в этой сессии; ты производишь аудиторский документ.

Проект расположен на `D:\Colony_Simulator\Colony_Simulator`. У тебя есть read-only access к этому пути через `Filesystem` MCP tools. Прочее filesystem-окружение для тебя недоступно. У тебя нет git CLI, нет shell, нет `dotnet build/test`. Только filesystem read.

---

## 2. Контракт кампании — обязательное первое действие

Этот pass — **первый из пяти** в audit campaign. Кампания спланирована и утверждена как ratified spec; этот промт — её прикладной артефакт.

**ОБЯЗАТЕЛЬНОЕ ПЕРВОЕ ДЕЙСТВИЕ перед началом любой работы:** прочитай целиком следующий документ:

```
D:\Colony_Simulator\Colony_Simulator\docs\audit\AUDIT_CAMPAIGN_PLAN.md
```

Это `LOCKED v1.0` спецификация всей кампании. Без её прочтения ты не понимаешь:

- что такое pass'ы и контракты между ними,
- какая тиерация finding'ов используется (§4 плана),
- какой whitelist ratified deviations не флагать (§6 плана),
- какое правило sequence integrity check является критичным (§5 плана),
- какой ratified формат выходного артефакта (§7.2: closure-review style для всех пяти проходов).

Если план недоступен или его содержимое не совпадает с тем, что описано в этом промте, **немедленно остановись** и сообщи человеку: «План кампании отсутствует или не v1.0 LOCKED — Pass 1 заблокирован». Не пытайся импровизировать.

После прочтения плана прочитай также для контекста:

```
D:\Colony_Simulator\Colony_Simulator\docs\MOD_OS_ARCHITECTURE.md  (LOCKED v1.4 спека)
D:\Colony_Simulator\Colony_Simulator\docs\ROADMAP.md              (текущее состояние M-фаз)
D:\Colony_Simulator\Colony_Simulator\docs\METHODOLOGY.md          (принципы falsifiability)
D:\Colony_Simulator\Colony_Simulator\docs\M6_CLOSURE_REVIEW.md    (образец closure-review формата)
```

Используй `Filesystem:read_multiple_files` batch-режим (per §7.5 плана: minimum batch=5 при чтении группы файлов). Не читай по одному.

---

## 3. Цель Pass 1

Произвести **`AUDIT_PASS_1_INVENTORY.md`** — falsifiable артефакт, содержащий **факты** о состоянии проекта на момент сессии, без какой-либо интерпретации, классификации или сравнения источников.

Это критическая дисциплинарная граница: Pass 1 — фундамент кампании. Любая интерпретация, спрятанная в Pass 1, отравит Pass 2/3/4/5. Если ты не уверен, факт это или интерпретация — это интерпретация. Удали и оставь только факт.

**Pass 1 не делает:**

- Сравнения между источниками (нашёл `369` в ROADMAP и `370` посчитал по тестам — фиксируй обе цифры, не комментируй).
- Тиерация finding'ов (Tier 0/1/2/3/4 — это работа Pass 2/3/4).
- Quality judgments («это устарело», «это правильно»).
- Запуск кода (нет `dotnet test`, нет `dotnet build`).
- Правки файлов проекта вне `docs/audit/`.

**Pass 1 делает:**

- Считает.
- Перечисляет.
- Извлекает прямые цитаты из файлов (status lines, header'ы, version markers).
- Фиксирует номера и идентификаторы.
- Записывает источник каждого числа (`file:line` или filesystem operation).

---

## 4. Контракт-вход

| Что | Источник |
|---|---|
| Repo root | `D:\Colony_Simulator\Colony_Simulator\` |
| Spec | `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.4 LOCKED |
| Roadmap | `docs/ROADMAP.md` |
| Campaign plan | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` v1.0 LOCKED |
| Closure review template | `docs/M6_CLOSURE_REVIEW.md` |
| Git state | `.git/HEAD`, `.git/refs/heads/<branch>`, `.git/logs/HEAD` (read directly) |
| Tools | `Filesystem` MCP tools only |

**Доступ к `.git/`** ratified в плане §7.3. Читай эти файлы напрямую как обычный текст:

- `.git/HEAD` содержит одну строку: `ref: refs/heads/<branch>`. Например: `ref: refs/heads/feat/m4-shared-alc`.
- `.git/refs/heads/<branch>` содержит одну строку — 40-character SHA hash текущего HEAD коммита.
- `.git/logs/HEAD` содержит линейную историю в формате:
  ```
  <old_sha> <new_sha> <author_name> <author_email> <timestamp> <timezone>\t<commit_subject_or_message>
  ```
  Каждая строка — одно событие (commit, checkout, merge, reset). Тебе нужны последние ~20–30 commit-событий в обратном хронологическом порядке.

Если ветка содержит `/` в имени (например `feat/m4-shared-alc`), путь становится `.git/refs/heads/feat/m4-shared-alc`. Поддиректории создаются автоматически в git.

---

## 5. Контракт-выход

**Файл:** `docs/audit/AUDIT_PASS_1_INVENTORY.md`

**Язык артефакта:** английский (для консистентности с `M3..M6_CLOSURE_REVIEW.md` и `NORMALIZATION_REPORT.md`).

**Формат:** closure-review style, mirroring `M6_CLOSURE_REVIEW.md`. Конкретные секции описаны ниже. Точно следуй структуре. Если секция не имеет содержимого — оставь её с заголовком и пометкой `(no content for this section in Pass 1)`, не удаляй.

### Структура артефакта

```markdown
---
title: Audit Pass 1 — Inventory & Baseline
nav_order: 104
---

# Audit Pass 1 — Inventory & Baseline

**Date:** YYYY-MM-DD (the date you run Pass 1)
**Branch:** <from .git/HEAD>
**HEAD:** <40-char SHA from .git/refs/heads/...>
**Scope:** Inventory only. No interpretation, no classification, no
comparison between sources. Pure facts with source attribution.

---

## §0 Executive summary

| # | Check | Status | Notes |
|---|---|---|---|
| 1 | Repo baseline established | PASSED / FAILED | HEAD commit, branch, last commit date |
| 2 | Test inventory complete | PASSED / FAILED | N total [Fact] attributes counted across all test projects |
| 3 | Spec version captured | PASSED / FAILED | v1.X LOCKED status line, last ratification entry |
| 4 | Document inventory complete | PASSED / FAILED | N .md files catalogued |
| 5 | Symbol inventory complete | PASSED / FAILED | N public types in DualFrontier.Contracts |
| 6 | Manifest schema inventory complete | PASSED / FAILED | N manifest.json files catalogued |
| 7 | Closure review inventory complete | PASSED / FAILED | N closure reviews + N session logs |
| 8 | Cyrillic inventory complete | PASSED / FAILED | N .cs files with cyrillic content |
| 9 | Sequence catalogue complete | PASSED / FAILED | N numbered sequences catalogued |

`PASSED` = data successfully gathered. `FAILED` = filesystem access failed,
file missing, parse error, or other technical blocker (NOT a finding about
the project — that's Pass 2/3/4 territory).

---

## §1 Repo baseline

- **Branch:** `<from .git/HEAD>`
- **HEAD commit:** `<40-char SHA>`
- **Last commit subject:** `<from .git/logs/HEAD last entry>`
- **Last commit author:** `<from .git/logs/HEAD last entry>`
- **Last commit timestamp:** `<from .git/logs/HEAD last entry>`
- **Recent commit log (last 20–30 entries, newest first):**

| # | SHA (short) | Author | Timestamp | Subject |
|---|---|---|---|---|
| 1 | abc1234 | ... | YYYY-MM-DD HH:MM | scope(area): subject |
| 2 | ... | ... | ... | ... |

- **Repository size approximation:** N directories under `src/`, `tests/`, `mods/`, `docs/`.

---

## §2 Test inventory

`[Fact]` attributes counted by reading test files directly. Pass 1 does not
run `dotnet test`. The count is the source-level number; runtime exclusions
or `[Fact(Skip=...)]` are noted separately.

| Project | Test files | `[Fact]` count | `[Theory]` count | Total | ROADMAP-stated count |
|---|---|---|---|---|---|
| `tests/DualFrontier.Persistence.Tests/` | N | N | N | N | N (per ROADMAP §...) |
| `tests/DualFrontier.Systems.Tests/` | N | N | N | N | N |
| `tests/DualFrontier.Modding.Tests/` | N | N | N | N | N |
| `tests/DualFrontier.Core.Tests/` | N | N | N | N | N |
| **Total** | **N** | **N** | **N** | **N** | **N (per ROADMAP engine snapshot line)** |

**Skipped tests inventory:**

| File:line | Test name | Skip reason |
|---|---|---|
| ... | ... | ... |

---

## §3 Spec version

- **File:** `docs/architecture/MOD_OS_ARCHITECTURE.md`
- **Status line (line ~8):** `LOCKED v1.X — ...` (verbatim copy)
- **Last ratification entry (verbatim from changelog):**
  ```
  v1.X (this version) — non-semantic clarifications from ...
    - §X.X: ...
    - ...
  ```
- **File size:** N bytes
- **Total line count:** N
- **Top-level section count (number of `## ` lines):** N

If the file's status line does not say `LOCKED v1.4`, fix this in §11 as an
ANOMALY (do not stop the pass — Pass 1 inventories what is, not what should be).

---

## §4 Document inventory

| Path | Type | Status (from header or content) | File size (bytes) |
|---|---|---|---|
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | spec | LOCKED v1.X | N |
| `docs/ROADMAP.md` | active | active (M7.X in progress) | N |
| `docs/M3_CLOSURE_REVIEW.md` | audit-trail | historical/frozen | N |
| `docs/M4_CLOSURE_REVIEW.md` | audit-trail | historical/frozen | N |
| `docs/M5_CLOSURE_REVIEW.md` | audit-trail | historical/frozen | N |
| `docs/M6_CLOSURE_REVIEW.md` | audit-trail | historical/frozen | N |
| `docs/SESSION_PHASE_4_CLOSURE_REVIEW.md` | audit-trail | preserved Russian | N |
| `docs/methodology/METHODOLOGY.md` | methodology | active | N |
| `docs/methodology/PIPELINE_METRICS.md` | methodology | active | N |
| `docs/architecture/CONTRACTS.md` | architecture | active | N |
| `docs/architecture/ECS.md` | architecture | active | N |
| `docs/architecture/EVENT_BUS.md` | architecture | active | N |
| `docs/architecture/THREADING.md` | architecture | active | N |
| `docs/architecture/ISOLATION.md` | architecture | active | N |
| `docs/architecture/MODDING.md` | architecture/dev | active | N |
| `docs/architecture/MOD_PIPELINE.md` | architecture/dev | active | N |
| `docs/methodology/CODING_STANDARDS.md` | development | active | N |
| `docs/methodology/DEVELOPMENT_HYGIENE.md` | development | active | N |
| `docs/methodology/TESTING_STRATEGY.md` | development | active | N |
| `docs/architecture/PERFORMANCE.md` | development | active | N |
| `docs/architecture/GPU_COMPUTE.md` | development | active | N |
| `docs/reports/NATIVE_CORE_EXPERIMENT.md` | development | active | N |
| `docs/architecture/VISUAL_ENGINE.md` | architecture | active | N |
| `docs/architecture/GODOT_INTEGRATION.md` | architecture | active | N |
| `docs/architecture/RESOURCE_MODELS.md` | v0.2 addendum | active | N |
| `docs/architecture/COMPOSITE_REQUESTS.md` | v0.2 addendum | active | N |
| `docs/FEEDBACK_LOOPS.md` | v0.2 addendum | active | N |
| `docs/architecture/COMBO_RESOLUTION.md` | v0.2 addendum | active | N |
| `docs/architecture/OWNERSHIP_TRANSITION.md` | v0.2 addendum | active | N |
| `docs/TRANSLATION_GLOSSARY.md` | translation | locked v1.0 | N |
| `docs/TRANSLATION_PLAN.md` | translation | active | N |
| `docs/reports/NORMALIZATION_REPORT.md` | translation | locked Pass 1 | N |
| `docs/PASS_2_NOTES.md` | translation | locked Pass 2 | N |
| `docs/PASS_3_NOTES.md` | translation | locked Pass 3 | N |
| `docs/PASS_4_REPORT.md` | translation | locked Pass 4 | N |
| `docs/learning/PHASE_1.md` | learning | frozen at Phase 1 snapshot | N |
| `docs/audit/AUDIT_CAMPAIGN_PLAN.md` | audit | LOCKED v1.0 | N |
| `docs/audit/AUDIT_PASS_1_PROMPT.md` | audit prompt | this prompt | N |
| `docs/README.md` | nav | nav | N |
| `README.md` (root) | nav | nav | N |

**Sub-folder READMEs:**

| Path | File size | Notes |
|---|---|---|
| `src/DualFrontier.Contracts/Bus/README.md` | N | bus contracts |
| `src/DualFrontier.Core/Bus/README.md` | N | bus implementation |
| `src/DualFrontier.Events/README.md` | N | event types |
| `tests/DualFrontier.Core.Tests/README.md` | N | test description |
| `src/DualFrontier.Persistence/README.md` | N | persistence |
| `mods/DualFrontier.Mod.Example/README.md` if present | N | example mod |
| ... (enumerate all encountered) | ... | ... |

If a document is encountered that is not in this template, add a row.
If a document in this template is missing from filesystem, mark `MISSING` and add to §11.

---

## §5 Symbol inventory

Public surface of `DualFrontier.Contracts` (the only assembly mods see).
Read every `.cs` file under `src/DualFrontier.Contracts/` and enumerate
`public interface`, `public class`, `public record`, `public enum`,
`public struct`, `public delegate`, `public static class`. Group by
sub-namespace.

| Namespace | Type | Kind | File:line |
|---|---|---|---|
| `DualFrontier.Contracts.Bus` | `IGameServices` | interface | `IGameServices.cs:N` |
| `DualFrontier.Contracts.Bus` | `ICombatBus` | interface | `ICombatBus.cs:N` |
| `DualFrontier.Contracts.Bus` | `IInventoryBus` | interface | ... |
| `DualFrontier.Contracts.Bus` | `IMagicBus` | interface | ... |
| `DualFrontier.Contracts.Bus` | `IPawnBus` | interface | ... |
| `DualFrontier.Contracts.Bus` | `IPowerBus` | interface | ... |
| `DualFrontier.Contracts.Bus` | `IWorldBus` | interface | ... |
| `DualFrontier.Contracts.Modding` | `IModApi` | interface | ... |
| `DualFrontier.Contracts.Modding` | `IMod` | interface | ... |
| `DualFrontier.Contracts.Modding` | `IModContract` | interface | ... |
| `DualFrontier.Contracts.Modding` | `IModContractStore` | interface | ... |
| `DualFrontier.Contracts.Modding` | `ModManifest` | record | ... |
| `DualFrontier.Contracts.Modding` | `ModDependency` | record | ... |
| `DualFrontier.Contracts.Modding` | `VersionConstraint` | record/struct | ... |
| `DualFrontier.Contracts.Modding` | `ManifestCapabilities` | record | ... |
| `DualFrontier.Contracts.Modding` | `ContractsVersion` | static class | ... |
| `DualFrontier.Contracts.Attributes` | `SystemAccessAttribute` | attribute | ... |
| `DualFrontier.Contracts.Attributes` | `BridgeImplementationAttribute` | attribute | ... |
| `DualFrontier.Contracts.Attributes` | `ModAccessibleAttribute` | attribute | ... |
| `DualFrontier.Contracts.Attributes` | `ModCapabilitiesAttribute` | attribute | ... |
| `DualFrontier.Contracts.Attributes` | `DeferredAttribute` | attribute | ... |
| `DualFrontier.Contracts.Attributes` | `ImmediateAttribute` | attribute | ... |
| `DualFrontier.Contracts.Core` | `IEvent` | marker interface | ... |
| `DualFrontier.Contracts.Core` | `IComponent` | marker interface (if present) | ... |
| `DualFrontier.Contracts.Modding` | `ValidationErrorKind` | enum | ... |
| ... | ... | ... | ... |

**Per-namespace count:**

| Namespace | Type count |
|---|---|
| `DualFrontier.Contracts.Bus` | N |
| `DualFrontier.Contracts.Modding` | N |
| `DualFrontier.Contracts.Attributes` | N |
| `DualFrontier.Contracts.Core` | N |
| ... | ... |
| **Total public types in Contracts** | **N** |

For `ValidationErrorKind` enum specifically — list all enum members in §9 Sequence catalogue (it is a numbered/ordered enumeration critical for sequence integrity check in Pass 2).

---

## §6 Manifest schema inventory

Enumerate all `mod.manifest.json` files in `tests/Fixture.*/` and `mods/`.

| Path | Fields present | Kind | Capabilities required (count) | Replaces (count) |
|---|---|---|---|---|
| `tests/Fixture.RegularMod_DependedOn/mod.manifest.json` | id, name, version, kind, ... | regular | N | 0 |
| `tests/Fixture.RegularMod_ReplacesCombat/mod.manifest.json` | ... | regular | N | N (list) |
| `tests/Fixture.SharedEvents/mod.manifest.json` (if present) | ... | shared | 0 | 0 |
| `mods/DualFrontier.Mod.Example/mod.manifest.json` (if present) | ... | regular | N | N |
| ... | ... | ... | ... | ... |

For each manifest, also record:
- `apiVersion` value (string, e.g. `"^1.0.0"`)
- `hotReload` boolean if present (and default if absent)
- `dependencies` list count if present

---

## §7 Closure review inventory

| File | Date (from frontmatter or §0) | Reviewed phase | Mentions HEAD/upper-bound commit | Status |
|---|---|---|---|---|
| `M3_CLOSURE_REVIEW.md` | YYYY-MM-DD | M3 (3.1, 3.2, 3.3; M3.4 deferred) | <hash> | historical/frozen |
| `M4_CLOSURE_REVIEW.md` | ... | M4 (4.1, 4.2, 4.3) | ... | historical/frozen |
| `M5_CLOSURE_REVIEW.md` | ... | M5 (5.1, 5.2) | ... | historical/frozen |
| `M6_CLOSURE_REVIEW.md` | 2026-05-01 | M6 (6.1, 6.2, 6.3) | e643011 | historical/frozen |
| `SESSION_PHASE_4_CLOSURE_REVIEW.md` | YYYY-MM-DD | Phase 4 | ... | preserved Russian audit trail |

**Ratification entries in `MOD_OS_ARCHITECTURE.md` changelog:**

| Version | Source review | Entry summary |
|---|---|---|
| v1.0 | Phase 0 closure | seven decisions resolved |
| v1.1 | M1–M3.1 audit | Log → ModLogLevel, dependency.optional |
| v1.2 | M3 closure review | §3.6 hybrid, §3.5+§2.1 example, M3.4 deferred |
| v1.3 | M4.3 implementation review | §2.2 entryAssembly/entryType wording |
| v1.4 | M7 pre-flight readiness review | §9.5 step 7 GC pump, §9.5.1 failure semantics |

---

## §8 Cyrillic inventory

`grep` equivalent over `.cs` files: read each file, check for Cyrillic
characters (Unicode range U+0400–U+04FF). Use `read_multiple_files`
batches of 10–20 for efficiency.

Files to scan (recursive):
- `src/**/*.cs`
- `tests/**/*.cs`
- `mods/**/*.cs`

Skip `bin/`, `obj/`, `.godot/`, `.vs/`, `BenchmarkDotNet.Artifacts/`.

| File | Has Cyrillic? | Approx. count of Cyrillic chars (or 0) |
|---|---|---|
| `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` | yes / no | N |
| ... | ... | ... |

**Summary:**

- Total `.cs` files scanned: N
- `.cs` files with any Cyrillic: N
- Per-area breakdown:
  - `src/DualFrontier.Core/`: N files
  - `src/DualFrontier.Application/`: N files
  - `src/DualFrontier.Components/`: N files
  - ... (enumerate)
  - `tests/`: N files
  - `mods/`: N files

If total > 0: list every file with Cyrillic. Pass 1 does **not** classify
each file as Tier 2 (whitelist) or Tier 3 (residual) — that is Pass 4.
Pass 1 just records the fact.

---

## §9 Sequence catalogue

Catalogue every numbered sequence in the project (spec, roadmap, code).
This is the input for Pass 2 sequence integrity check (§5 of the campaign plan).

| Source | Location | Sequence label | Observed numbers/identifiers | Stated total (if any) |
|---|---|---|---|---|
| `MOD_OS_ARCHITECTURE.md` | §1 | mod kinds | 1.1, 1.2, 1.3 | "three" |
| `MOD_OS_ARCHITECTURE.md` | §2.2 | manifest fields | (list extracted from table) | — |
| `MOD_OS_ARCHITECTURE.md` | §3.6 | enforcement layers | (list) | — |
| `MOD_OS_ARCHITECTURE.md` | §4.1 | IModApi v1 methods | (list) | — |
| `MOD_OS_ARCHITECTURE.md` | §4.2 | IModApi v2 methods | (list) | — |
| `MOD_OS_ARCHITECTURE.md` | §4.3 | IModApi v2 capability checks | (list) | — |
| `MOD_OS_ARCHITECTURE.md` | §5.2 | shared mod compliance steps | (list) | — |
| `MOD_OS_ARCHITECTURE.md` | §7.1 | bridge replacement steps | (list) | — |
| `MOD_OS_ARCHITECTURE.md` | §7.5 | bridge replacement scenarios | (list) | "five" or count |
| `MOD_OS_ARCHITECTURE.md` | §9.5 | unload chain steps | 1, 2, 3, 4, 5, 6, 7 | "seven" |
| `MOD_OS_ARCHITECTURE.md` | §9.5.1 | failure semantics steps | (list) | — |
| `MOD_OS_ARCHITECTURE.md` | §11.1 | M-phase acceptance bullets per phase | (list per phase) | per-phase claim |
| `MOD_OS_ARCHITECTURE.md` | §11.2 | ValidationErrorKind enumeration | (list) | stated count if any |
| `MOD_OS_ARCHITECTURE.md` | §12 | detail decisions | D-1, D-2, D-3, D-4, D-5, D-6, D-7 | "seven" |
| `ROADMAP.md` | M3 | sub-phases | 3.1, 3.2, 3.3, 3.4 | — |
| `ROADMAP.md` | M5 | sub-phases | 5.1, 5.2 | — |
| `ROADMAP.md` | M6 | sub-phases | 6.1, 6.2, (6.3 closure-sync) | — |
| `ROADMAP.md` | M7 | sub-phases | 7.1, 7.2, 7.3, 7.4, 7.5, 7-closure | "five" |
| `ROADMAP.md` | M10 | vanilla slices | A, B, C, D | "four" |
| `ROADMAP.md` | strategic locks | top-level decisions | 1, 2, 3, 4, 5 | "five" |
| `src/DualFrontier.Application/Modding/ContractValidator.cs` | class XML-doc | validator phases | A, B, C, D, E, F, G, H | "eight" |
| `src/DualFrontier.Contracts/Modding/ValidationErrorKind.cs` | enum | error kinds | (extract every member) | — |
| `src/DualFrontier.Contracts/Modding/IModApi.cs` | interface | API methods | (extract every method) | — |
| `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` | violation builders | message kinds | (Read, Write, GetSystem, ...) | — |
| `M6_CLOSURE_REVIEW.md` | §0 | review checks | 1..8 | "eight" |
| `M5_CLOSURE_REVIEW.md` | §0 | review checks | 1..8 | "eight" |
| `M4_CLOSURE_REVIEW.md` | §0 | review checks | 1..N | (read) |
| `M3_CLOSURE_REVIEW.md` | §0 | review checks | 1..N | (read) |
| ... | ... | ... | ... | ... |

**Critical:** for sequences in the range 4–6 of any of the above lists, you
**must** record the observed numbers verbatim, in source order, even if
they look uninteresting. The campaign plan §5.2 explicitly requires this:
the suspected sequence-integrity violation reported by the human reviewer
is in the 4–6 range, and Pass 2 will check this catalogue.

If you encounter additional numbered sequences not in the template above,
add them. The catalogue is a living inventory; under-enumeration is a
defect.

---

## §10 Surgical fixes applied this pass

None. Pass 1 is read-only by contract.

---

## §11 Items requiring follow-up

This section captures **anomalies observed during Pass 1 that may be
significant but cannot be classified yet**. Format:

| # | Anomaly | Source | Note |
|---|---|---|---|
| 1 | Test count from source file enumeration: N. ROADMAP claims: M. Difference: N−M = K. | `tests/...` + `ROADMAP.md:N` | (Pass 3 will classify) |
| 2 | File `docs/X.md` referenced from `docs/README.md` but not present in filesystem. | `docs/README.md:N` | (Pass 4 will classify) |
| 3 | ... | ... | ... |

**Pass 1 does not assign Tier here.** Pass 2/3/4 will pick up these items
and classify them. The anomaly list is the bridge from facts (this pass) to
findings (next passes).

If no anomalies — write `(none observed)`. This is a meaningful PASSED
signal, not an empty section.

---

## §12 Verification end-state

- **§0 Executive summary:** N/9 PASSED.
- **Total facts captured:** test counts (N), spec version (v1.X), .md files (N), public types (N), manifests (N), closure reviews (N), cyrillic .cs files (N), sequences (N).
- **Anomalies in §11:** N items registered for Pass 2/3/4 attention.
- **Surgical fixes applied:** 0 (per contract).
- **Pass 1 status:** complete, ready for human ratification.
```

---

## 6. Методика — пошаговый порядок работы

Выполняй в следующем порядке. Не отклоняйся; этот порядок минимизирует round-trips и максимизирует falsifiability.

### Шаг 1. Прочитать кампанию, плана, спеку, roadmap, образец closure review

Используй `Filesystem:read_multiple_files` с batch=5:

```
[
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_CAMPAIGN_PLAN.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MOD_OS_ARCHITECTURE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\ROADMAP.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\METHODOLOGY.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\M6_CLOSURE_REVIEW.md"
]
```

После этого ты понимаешь контекст. Если plan не v1.0 LOCKED — стоп.

### Шаг 2. Repo baseline

Прочитай git state:

```
[
  ".git/HEAD",
  ".git/logs/HEAD"
]
```

Из `.git/HEAD` извлеки имя ветки. Прочитай соответствующий `.git/refs/heads/<branch>`. Распарси `.git/logs/HEAD` — последние 20–30 событий, формат описан в §4. Заполни §1 артефакта.

### Шаг 3. Document inventory

Используй `Filesystem:list_directory` для:

- `docs/`
- `docs/learning/`
- `docs/audit/`
- root проекта

Для каждой `.md` — `read_multiple_files` batch=5, прочитай header (frontmatter + первые 20 строк), извлеки status (LOCKED / active / draft / historical / nav). Запиши в §4. Параллельно `get_file_info` для размеров.

Также `list_directory` под `src/`, `tests/`, `mods/` для поиска `*/README.md`. Эти добавляй в §4 «Sub-folder READMEs».

### Шаг 4. Test inventory

`list_directory` под `tests/`. Для каждого `tests/*Tests/` — рекурсивно перечисли `.cs` файлы. Используй `Filesystem:list_directory` per-folder (если рекурсивный обход недоступен).

Для каждого test `.cs` файла — `read_multiple_files` batch=10. В тексте ищи `[Fact`, `[Theory`, `[Fact(Skip`, `[Theory(Skip`. Считай.

Заполни §2. Stated count из ROADMAP `Engine snapshot:` строки.

### Шаг 5. Spec version

Прочитай первые 100 строк `MOD_OS_ARCHITECTURE.md` (если ещё не прочитал целиком в Шаге 1). Извлеки status line (line ~8) и changelog v1.4 entry. Заполни §3.

### Шаг 6. Symbol inventory

`list_directory` под `src/DualFrontier.Contracts/`. Для каждой sub-folder — `list_directory`. Собери список всех `.cs`. `read_multiple_files` batch=10.

В каждом файле найди `public interface`, `public class`, `public record`, `public enum`, `public struct`, `public delegate`, `public static class`. Запиши в §5 с file:line.

Группируй по namespace из `namespace DualFrontier.Contracts.<...>;` declarations.

### Шаг 7. Manifest inventory

`list_directory` под `tests/` для всех `Fixture.*` папок. `list_directory` под `mods/`. Для каждого манифеста — `read_text_file`, парси JSON (визуально, не нужен JSON parser — просто извлеки поля верхнего уровня). Заполни §6.

### Шаг 8. Closure review inventory

`read_multiple_files` для всех `M*_CLOSURE_REVIEW.md` + `SESSION_PHASE_4_CLOSURE_REVIEW.md` (batch=5). Извлеки `Date:`, `Branch:`, `Scope:` из header'ов, и upper bound коммита из §1 каждого. Заполни §7.

Также из ratification chain в `MOD_OS_ARCHITECTURE.md` Version history — заполни ratification table в §7.

### Шаг 9. Cyrillic inventory

`list_directory` рекурсивно под `src/`, `tests/`, `mods/`. Собери все `.cs` (кроме `bin/`, `obj/`, и т.д.).

`read_multiple_files` batch=15–20 (это много, но позволяет за 5–10 round-trips закрыть всё). Для каждого файла — поиск Unicode range U+0400–U+04FF.

Записывай только файлы где count > 0. Заполни §8.

### Шаг 10. Sequence catalogue

Перечитай (или возьми из контекста уже прочитанного) `MOD_OS_ARCHITECTURE.md` §1, §2.2, §3.6, §4.1–4.3, §5.2, §7.1–7.5, §9.5, §9.5.1, §11.1, §11.2, §12. Извлеки **точные** numbered/lettered sequences.

Прочитай (если ещё не) `src/DualFrontier.Application/Modding/ContractValidator.cs` и `src/DualFrontier.Contracts/Modding/ValidationErrorKind.cs`. Извлеки phases enumeration из class XML-doc и enum members.

Прочитай `ROADMAP.md` секции для M3, M5, M6, M7, M10, strategic locks. Извлеки sub-phase identifiers.

Заполни §9 — самая важная секция Pass 1, потому что это вход для Pass 2 sequence integrity check.

### Шаг 11. Self-check

Перед записью артефакта — пройди self-check (§9 этого промта).

### Шаг 12. Запиши артефакт

Используй `Filesystem:write_file` для записи в:

```
D:\Colony_Simulator\Colony_Simulator\docs\audit\AUDIT_PASS_1_INVENTORY.md
```

Формат — точно как в §5 этого промта. Английский.

### Шаг 13. Stop

После записи — стоп. Сообщи человеку короткой строкой: «AUDIT_PASS_1_INVENTORY.md записан. N/9 проверок PASSED. M аномалий в §11 для следующих pass'ов. Жду human ratification».

Не предлагай Pass 2. Не commit'ь. Не правь другие файлы.

---

## 7. Запрещено в Pass 1

- **Любые сравнения между источниками.** Если ROADMAP говорит `369`, а ты насчитал `370` — фиксируй обе цифры в §2 и §11. Не пиши «дрейф», «несоответствие», «error».
- **Тиерация.** Никаких упоминаний Tier 0/1/2/3/4. Это работа Pass 2/3/4.
- **Quality judgments.** Никаких «устарело», «правильно», «полнота», «качество».
- **Запуск кода.** Нет `dotnet test`, `dotnet build`. Pass 1 парсит исходники, не выполняет.
- **Правки файлов проекта.** Единственная запись — `docs/audit/AUDIT_PASS_1_INVENTORY.md`. Никаких других файлов трогать.
- **Commit'ы.** Pass 1 не делает git commits. Это работа человека после ratification.
- **Импровизация формата.** Структура артефакта зафиксирована в §5 этого промта. Не добавляй секции, не удаляй секции, не переименовывай. Если секция пустая — `(no content)`.

---

## 8. Discipline

### 8.1 Batch reads (per кампания §7.5 LOCKED)

Используй `Filesystem:read_multiple_files` с batch≥5 при чтении группы связанных файлов. Не делай 30 одиночных `read_file` calls когда можно сделать 3 batch-call'а по 10 файлов.

### 8.2 Точные числа

Никогда не пиши «around 369», «approximately», «roughly». Точные числа или явный «could not determine» с объяснением почему.

### 8.3 Source attribution

Каждое число / факт / цитата имеет источник: `file:line`, `path`, `directory listing`, `git log entry`. Без источника — не записывай.

### 8.4 Verbatim quotes

Status lines, changelog entries, headers — копируй verbatim в backticks. Не парафразируй. Любая неточность парафраза — это потенциальный finding в Pass 2/4.

### 8.5 No shortcuts on §9

Sequence catalogue — это критический output. Не пропускай sequences «потому что они выглядят неинтересно». Особенно sequences в range 4–6 (per кампания §5.2).

---

## 9. Self-check перед submission

Перед `write_file` пройди этот чек-лист и явно подтверди каждый пункт:

- [ ] Все 12 секций (§0..§12) присутствуют в артефакте.
- [ ] §0 Executive summary таблица заполнена 9 строками.
- [ ] Каждое числовое утверждение имеет источник (file:line или operation).
- [ ] §9 Sequence catalogue содержит минимум 25 sequences (это нижняя граница для проекта такого размера; при меньшем количестве — ты что-то пропустил).
- [ ] Никаких упоминаний `Tier 0/1/2/3/4` в артефакте.
- [ ] Никаких слов «drift», «inconsistency», «error», «issue» (кроме случая когда это часть verbatim quote).
- [ ] Никаких сравнений «N matches M» / «N differs from M». Только «source A: N. source B: M.»
- [ ] §10 = 0 (per contract).
- [ ] Артефакт на английском.
- [ ] Структура mirroring `M6_CLOSURE_REVIEW.md`.

Если хотя бы один пункт `[ ]` — ты ещё не готов. Вернись к шагу методики где этот пункт должен был быть закрыт.

---

## 10. Stop condition и эскалация

### 10.1 Normal stop

После записи артефакта и self-check — короткое сообщение человеку (см. Шаг 13 методики). Конец сессии.

### 10.2 Eager Tier 0 escalation (per кампания §7.4)

Pass 1 обычно **не** должен порождать Tier 0 — он не классифицирует. **Но** если ты обнаруживаешь в процессе сбора фактов что-то структурно несоответствующее ожиданиям (например, `MOD_OS_ARCHITECTURE.md` отсутствует, или `.git/HEAD` пустой, или `docs/audit/AUDIT_CAMPAIGN_PLAN.md` не v1.0 LOCKED) — это **infrastructure failure**, не Tier 0 finding.

Действия:
1. Запиши частичный артефакт в `docs/audit/AUDIT_PASS_1_INVENTORY.md` с явной пометкой `INCOMPLETE — infrastructure failure on step N` в §0 и в самом верху файла.
2. Сообщи человеку: «Pass 1 заблокирован на шаге N. Причина: <описание>. Артефакт записан как INCOMPLETE. Требуется решение человека.»
3. Стоп.

### 10.3 Когда не останавливаться

Если ты находишь что-то странное про **проект** (test count меньше ожидаемого, файл из README отсутствует, две версии манифеста противоречат друг другу) — это **аномалия**, фиксируй в §11. **Не** останавливайся. Pass 1 не имеет authority классифицировать аномалию как Tier 0; это задача Pass 2/3/4.

Граница: **infrastructure failure** = ты не можешь выполнять Pass 1 (нет файлов которые должны быть). **Project anomaly** = проект в состоянии которое требует анализа в следующих pass'ах.

---

## 11. Финальная заметка

Pass 1 — фундамент кампании. Если ты выполнишь его поверхностно, Pass 2/3/4/5 будут опираться на ложную базу и могут пропустить настоящий drift. Если ты выполнишь его слишком ретиво и начнёшь классифицировать — ты повторишь работу следующих pass'ов и заразишь их своими преждевременными суждениями.

Дисциплина Pass 1: **факты, источники, без интерпретации, в формате closure review, full coverage**.

Удачи.

---

## Appendix A: пример заполнения §1 (для калибровки)

```markdown
## §1 Repo baseline

- **Branch:** `feat/m4-shared-alc`
- **HEAD commit:** `46b4f334a2891b...` (full 40-char SHA)
- **Last commit subject:** `test(modding): M73Phase2DebtTests + ModUnloadAssertions helper`
- **Last commit author:** `<name>`
- **Last commit timestamp:** `2026-04-30 17:42:18 +0300`
- **Recent commit log (last 20–30 entries, newest first):**

| # | SHA (short) | Author | Timestamp | Subject |
|---|---|---|---|---|
| 1 | 46b4f33 | crystalka | 2026-04-30 17:42 | test(modding): M73Phase2DebtTests + ModUnloadAssertions helper |
| 2 | 9bed1a4 | crystalka | 2026-04-30 14:18 | feat(modding): M7.3 step 7 WeakReference + GC pump + ModUnloadTimeout |
| 3 | d68ba93 | crystalka | 2026-04-29 ... | test(modding): M72UnloadChainTests |
| ... | ... | ... | ... | ... |

- **Repository size approximation:** 11 directories under `src/`, 14 under `tests/`, 1 under `mods/`, 1 under `docs/` (with `audit/`, `learning/` subfolders).
```

(пример с placeholder данными; ты заполняешь реальными значениями)

---

## Appendix B: пример заполнения §9 (фрагмент, для калибровки)

```markdown
## §9 Sequence catalogue

| Source | Location | Sequence label | Observed numbers/identifiers | Stated total (if any) |
|---|---|---|---|---|
| `MOD_OS_ARCHITECTURE.md` | §1 | mod kinds | 1.1, 1.2, 1.3 | — |
| `MOD_OS_ARCHITECTURE.md` | §9.5 | unload chain steps | 1, 2, 3, 4, 5, 6, 7 | "seven" (in step header text) |
| `MOD_OS_ARCHITECTURE.md` | §12 | detail decisions | D-1, D-2, D-3, D-4, D-5, D-6, D-7 | "seven" |
| `ROADMAP.md` | M3 row | sub-phases | M3.1, M3.2, M3.3, M3.4 (deferred) | — |
| `ROADMAP.md` | M7 row | sub-phases | M7.1, M7.2, M7.3, M7.4, M7.5, M7-closure | "five sub-phases plus closure" |
| `src/DualFrontier.Application/Modding/ContractValidator.cs` | class XML-doc | validator phases | A, B, C, D, E, F, G, H | "eight" |
| `src/DualFrontier.Contracts/Modding/ValidationErrorKind.cs` | enum members | error kinds | (verbatim list of every member name) | — |
```

(пример demonstrating expected granularity; реальный catalogue будет содержать ~30+ entries)

---

**Конец промта Pass 1.**

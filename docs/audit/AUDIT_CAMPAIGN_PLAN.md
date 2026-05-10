---
title: Audit Campaign Plan
nav_order: 102
---

# План аудиторской кампании Dual Frontier — 2026-05-01

*Сопровождающий документ к семейству [`M3..M6_CLOSURE_REVIEW.md`](./M6_CLOSURE_REVIEW.md). Описывает сквозной аудит проекта против `MOD_OS_ARCHITECTURE` v1.4 LOCKED + `ROADMAP` + actual implementation. Кампания производит артефакты класса `AUDIT_PASS_*.md` и финальный `AUDIT_REPORT.md` в формате closure review.*

**Status:** LOCKED v1.0 — Phase 0 кампании закрыт; шесть открытых решений §7 ratified 2026-05-01. Промты для Pass 1–5 пишутся опираясь на этот документ как контракт.

**Version history:**

- v0.1 (draft, 2026-05-01) — initial decomposition into five passes; six open decisions in §7 pending human ratification.
- v1.0 (locked, 2026-05-01) — Phase 0 кампании closed. Все шесть открытых решений ratified:
  - **§7.1** → `docs/audit/` подпапка.
  - **§7.2** → **Closure-review style для всех пяти проходов** (отвержение моего драфтового гибрида в пользу единообразной глубины — human reviewer явно отдал приоритет тщательности над скоростью).
  - **§7.3** → Opus читает `.git/HEAD`, `.git/refs/heads/<branch>`, `.git/logs/HEAD` напрямую через Filesystem MCP.
  - **§7.4** → Eager escalation для Tier 0 finding в любом pass'е.
  - **§7.5** → Batch reads через `read_multiple_files` с min batch=5 явно прописаны в каждом pass-промте.
  - **§7.6** → Pass 4 разбирает residual cyrillic per-file для <10 матчей; эскалирует к человеку для 10+.

---

## 0. Резюме

Это **не closure verification одной фазы**, а cumulative drift audit. Pre-flight readiness review (зафиксированный как ratification v1.4) уже сверил состояние спеки на момент входа в M7; M7.1–M7.3 реализованы под v1.4. Кампания проверяет, что **накопленный** дрейф между v1.4 LOCKED, фактическим кодом, тестами и ROADMAP-описанием не превысил bound, который методика проекта («specs are contracts, not references») структурно гарантирует.

Кампания декомпозирована на **пять последовательных проходов**. Каждый проход — отдельная сессия Opus с собственным standalone промтом и формальным выходным артефактом в формате closure review (8-section structure, mirroring `M6_CLOSURE_REVIEW.md`). Контракты между проходами зафиксированы в §3.

Промты живут в `docs/audit/AUDIT_PASS_N_PROMPT.md` и копируются в новую сессию Opus вручную (тот же паттерн, что для translation campaign).

Триггер запуска кампании: human reviewer зафиксировал подозрение на конкретный класс дефекта — **sequence integrity violation** (нумерация N, N+1, N+2 с пропущенным N+1). Этот класс не ловится contains-grep'ом и требует структурной проверки. План включает соответствующее правило в §5.

---

## 1. Цель и scope

### 1.1 Цель

Производство falsifiable артефакта (`AUDIT_REPORT.md`), утверждающего одно из:

- **GREEN.** Состояние проекта на M7.3 closure point соответствует v1.4 LOCKED + ROADMAP без дрейфа значимого класса. M7.4 unblocked.
- **YELLOW.** Состояние корректно, но обнаружено N follow-up items, не блокирующих M7.4. Items зарегистрированы как backlog.
- **RED.** Обнаружен Tier 0 spec drift или Tier 1 missing-required-implementation. M7.4 заблокирован до remediation.

Без этого артефакта M7.4 запускается на основании отдельных closure reviews M3–M6 + M7.1–M7.3 ad-hoc readiness, ни один из которых не выполнял cross-cutting cumulative проверку. Кампания закрывает этот пробел.

### 1.2 Scope — что аудитируется

| Категория | Источники | Глубина проверки |
|---|---|---|
| **LOCKED спецификации** | `MOD_OS_ARCHITECTURE.md` v1.4 §0–§12 | Line-by-line; каждое нормативное утверждение → file:line в коде |
| **Дорожная карта** | `ROADMAP.md` (все M-rows, phase-rows, carried debts, deliberate interpretations) | Каждое заявление про test count / commit / closed status верифицируется фактически |
| **Контракты ядра** | `src/DualFrontier.Contracts/` | Полный реестр public symbols → soul-search в LOCKED спеке |
| **Реализация modding kernel** | `src/DualFrontier.Application/Modding/` | Через §1–§9 v1.4 пройти каждое утверждение |
| **Реализация ECS/scheduler/buses** | `src/DualFrontier.Core/` | Точечно в местах, упомянутых в LOCKED §3, §4, §9 |
| **Components, Events, Systems** | `src/DualFrontier.Components/`, `src/DualFrontier.Events/`, `src/DualFrontier.Systems/` | Через `[ModAccessible]` + `[BridgeImplementation]` атрибуты |
| **Тесты** | `tests/DualFrontier.Modding.Tests/`, `tests/DualFrontier.Core.Tests/`, `tests/DualFrontier.Systems.Tests/`, `tests/DualFrontier.Persistence.Tests/` | Acceptance criteria из ROADMAP → file:line:test_name |
| **Сопровождающие доки** | `docs/architecture/CONTRACTS.md`, `docs/architecture/ECS.md`, `docs/architecture/EVENT_BUS.md`, `docs/architecture/THREADING.md`, `docs/architecture/ISOLATION.md`, `docs/architecture/MODDING.md`, `docs/architecture/MOD_PIPELINE.md`, `docs/methodology/CODING_STANDARDS.md`, `docs/methodology/TESTING_STRATEGY.md` | Cross-doc consistency vs v1.4; устаревшие утверждения → Tier 3 |
| **Артефакты translation campaign** | `NORMALIZATION_REPORT.md`, `PASS_2_NOTES.md`, `PASS_3_NOTES.md`, `PASS_4_REPORT.md`, `TRANSLATION_GLOSSARY.md` | Подтверждение завершённости: `grep '[А-Яа-я]' src/ tests/ mods/ --include='*.cs'` пуст модулю whitelist (§6.6) |
| **Closure reviews** | `M3..M6_CLOSURE_REVIEW.md`, `SESSION_PHASE_4_CLOSURE_REVIEW.md` | Целостность аудит-следа: `Date:` поле ↔ `git log` ↔ HEAD коммит на момент создания |
| **README в подпапках** | `src/*/README.md`, `tests/*/README.md`, `mods/*/README.md` | Quantitative claims (test counts, bus counts, system counts) ↔ реальность |
| **Git state** | `.git/HEAD`, `.git/refs/heads/<branch>`, `.git/logs/HEAD` | Прямое чтение через Filesystem MCP — branch, HEAD commit hash, последние коммиты для три-commit invariant |

### 1.3 Что НЕ в scope

- **Performance audit.** Покрывается отдельным классом аудитов под `PERFORMANCE.md`. Кампания не запускает benchmark'и.
- **Threat model деталь.** §2.7 v1.4 проверяется на консистентность wording'а, но активные penetration tests не выполняются.
- **Game design / balance / lore.** [GDD] не входит в инвариант методики.
- **M7.4–M7.5 implementation.** Кода нет; нечего аудитировать. Pre-M7.4 readiness check агрегируется в Pass 5 §8 (формат как у `M6_CLOSURE_REVIEW §8`).
- **Phase 9 Native Runtime.** Post-launch project, отдельный document.
- **Перевод вне `.cs` файлов** (например, остатки русского в `learning/`, `SESSION_*.md`). Эти файлы помечены в `TRANSLATION_PLAN.md` §2.4 как whitelist-preserved; Pass 4 их не флагает.

---

## 2. Структура кампании — пять проходов

Каждый проход производит артефакт в **формате closure review** (8-section structure, образец — `M6_CLOSURE_REVIEW.md`). Это §7.2 ratified decision: единообразная глубина для всех пяти проходов, отвержение гибрида (stripped tables для Pass 1–4) в пользу полного аудит-следа.

### Pass 1 — Inventory & Baseline

**Цель:** собрать **факты** о состоянии проекта без какой-либо интерпретации. Pass 1 — это «как сейчас на самом деле», и при этом falsifiable артефакт сам по себе (любой Pass 2+ будет опираться на эти числа как на ratified base).

**Контракт-вход:**
- Read-only доступ к `D:\Colony_Simulator\Colony_Simulator` через `Filesystem` MCP tools.
- **Прямой read-доступ к `.git/HEAD`, `.git/refs/heads/<branch>`, `.git/logs/HEAD`** (§7.3 ratified). Opus сам извлекает commit hash, branch, последние ~30 коммитов в обратном порядке.

**Контракт-выход:** `docs/audit/AUDIT_PASS_1_INVENTORY.md` в closure-review формате:

- **§0 Executive summary.** Таблица проверок Pass 1: build status (от ROADMAP claims), test count match, spec version match, doc inventory complete, symbol inventory complete, manifest inventory complete, closure-review inventory complete, cyrillic inventory complete, sequence catalogue complete. Каждая — PASSED/FAILED + summary.
- **§1 Repo baseline.** HEAD commit, branch, ahead/behind origin (если данные доступны через `.git/refs/`), дата.
- **§2 Test inventory.** ROADMAP заявляет engine snapshot `369/369`. Pass 1 проверяет соответствие через расчёт: per-project (Persistence + Systems + Modding + Core), сверка с per-M-phase delta из ROADMAP. **Pass 1 не запускает `dotnet test`**; он сверяет заявленные test counts с реальным числом `[Fact]` атрибутов в исходниках.
- **§3 Spec version.** v1.4 LOCKED status line + последняя ratification entry + byte-identity: `MOD_OS_ARCHITECTURE.md` зафиксирован какой commit его последний раз менял, и менялся ли он с момента последнего closure review.
- **§4 Document inventory.** Полный список `.md` в `docs/` + `README.md` (root) + все `*/README.md` под `src/`, `tests/`, `mods/`. Для каждого — статус (LOCKED / draft / historical / nav-only) + дата последней правки если выводимо из `git log`.
- **§5 Symbol inventory.** Public types в `DualFrontier.Contracts.dll` (interfaces, attributes, records, enums). Per-namespace breakdown.
- **§6 Manifest schema inventory.** Все `mod.manifest.json` в `tests/Fixture.*/` и `mods/`. Поля каждого манифеста — табличный реестр.
- **§7 Closure review inventory.** Все `M*_CLOSURE_REVIEW.md` + `SESSION_*.md` + ratification entries в spec changelog. Перечень audit-trail артефактов.
- **§8 Cyrillic inventory.** Реестр `.cs` файлов с русским контентом. После translation campaign ожидание — пусто; Pass 1 фиксирует факт без классификации (классификация в Pass 4).
- **§9 Sequence catalogue.** Полный каталог нумерованных последовательностей в LOCKED v1.4 + ROADMAP + код. Это вход для §5 sequence integrity check в Pass 2. Структура: `<source>:<location>:<sequence_label>:<observed_numbers>` (e.g. `MOD_OS_ARCHITECTURE.md:§9.5:unload_chain_steps:[1,2,3,4,5,6,7]`).
- **§10 Surgical fixes applied this pass.** Pass 1 не делает remediation — здесь `0`.
- **§11 Items requiring follow-up.** Любые аномалии, которые Pass 1 заметил но не классифицирует (классификация в Pass 2+).

**Запрещено в Pass 1:**
- Интерпретации, тиерация, рекомендации.
- Сверка между источниками — это работа Pass 2/3/4.
- Любые правки кода/доков.

**Длительность:** одна сессия Opus, ~1.5–2 часа.

---

### Pass 2 — Spec ↔ Code Drift

**Цель:** для каждого нормативного утверждения в `MOD_OS_ARCHITECTURE` v1.4 §1–§10 найти соответствующий артефакт в коде. Каждое утверждение → или file:line + test:name, или Tier-classified finding.

**Контракт-вход:**
- `AUDIT_PASS_1_INVENTORY.md` (locked после ратификации Pass 1).
- `MOD_OS_ARCHITECTURE.md` v1.4 целиком.
- Read-only доступ к `src/`, `tests/`.

**Контракт-выход:** `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` в closure-review формате:

- **§0 Executive summary.** Таблица 11 проверок (по одной на каждый §1–§10 v1.4 + §11 Sequence integrity). Каждая — PASSED/FAILED + count of findings per Tier.
- **§1 §1 Mod topology.** Три kind'а (Regular / Shared / Vanilla). Для каждого: какой `enum ModKind` value, где `ManifestParser` его читает, какой fixture его использует, какой test это закрепляет.
- **§2 §2 Manifest schema.** Каждое поле (name, id, version, apiVersion, kind, hotReload, dependencies, replaces, capabilities, entryAssembly, entryType) → `ModManifest.cs` field → `ManifestParser` reading code → fixture → test.
- **§3 §3 Capability model.** `[ModAccessible]`, `[ModCapabilities]`, `KernelCapabilityRegistry`, hybrid enforcement (load-time Phase C/D + runtime `EnforceCapability`).
- **§4 §4 IModApi v2.** `Publish`, `Subscribe`, `GetKernelCapabilities`, `GetOwnManifest`, `Log`, `RegisterSystem`, `PublishContract`, `UnsubscribeAll`, `RegisterFor`.
- **§5 §5 Type sharing across ALCs.** `SharedModLoadContext`, two-pass loader, D-4 enforcement (Phase E), D-5 cycles, §5.2 compliance (Phase F).
- **§6 §6 Three contract levels.** `IModContract`, `IEvent`, `[Deferred]`/`[Immediate]`, kernel vs shared vs regular contract type rules.
- **§7 §7 Bridge replacement.** `[BridgeImplementation(Replaceable=...)]`, `replaces` field, Phase H validator, pipeline skip logic, §7.5 acceptance scenarios (1–4 in-batch + 5 unload).
- **§8 §8 Versioning.** Three-tier SemVer, caret syntax, `VersionConstraint`, Phase A v1/v2 dual-path, Phase G inter-mod.
- **§9 §9 Lifecycle and hot reload.** Pause/Resume, run-flag, unload chain steps 1–7 (особое внимание step 7 v1.4 GC pump bracket), §9.5.1 failure semantics.
- **§10 §10 Threat model.** Только консистентность wording'а; active pen-tests out of scope.
- **§11 Sequence integrity findings.** Отдельная секция, выводимая из §5 sequence integrity check (см. §5 настоящего плана).
- **§12 Surgical fixes applied this pass.** Pass 2 не делает remediation — `0`.
- **§13 Items requiring follow-up.** Все findings, сгруппированные по Tier 0/1/2/3/4.

**Тиерация finding'ов** — полностью в §4 настоящего плана. Каждое отклонение получает Tier и обязательные поля: `spec_section`, `code_path`, `test_path`, `description`, `tier`, `recommendation`.

**Sequence integrity sub-pass.** В рамках Pass 2 выполняется отдельный шаг (§11 секция артефакта): для каждой нумерованной последовательности из §9 Pass 1 inventory проверяется отсутствие gap'ов, повторов, нарушений порядка, согласованность с любыми «of N» / «all N» формулировками в спеке. Это критичный шаг — он адресует human-reviewer-flagged подозрение на gap в районе пунктов 4–6.

**Eager escalation.** Per §7.4 ratified — любой Tier 0 finding (включая sequence integrity violation) останавливает Pass 2 в момент обнаружения, артефакт фиксирует partial state, человек получает escalation note.

**Длительность:** одна сессия Opus, ~2.5–3.5 часа. Самый плотный проход.

---

### Pass 3 — Roadmap ↔ Reality

**Цель:** для каждого M-row, phase-row, carried debt, deliberate interpretation в `ROADMAP.md` верифицировать каждое заявление против реальных артефактов (commit, test name, file:line, ratification reference).

**Контракт-вход:**
- `AUDIT_PASS_1_INVENTORY.md` (locked).
- `ROADMAP.md` целиком.
- Read-only доступ к `src/`, `tests/`, `.git/logs/HEAD` для три-commit invariant.

**Контракт-выход:** `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md` в closure-review формате:

- **§0 Executive summary.** Таблица 8 проверок: engine snapshot verification, test class verification, acceptance criteria coverage, carried debts forward, deliberate interpretations validity, closed-phase historical fields, pending phases marked correctly, three-commit invariant.
- **§1 Engine snapshot verification.** Для каждой M-фазы где ROADMAP заявляет тестовый count: реальный count `[Fact]` атрибутов в указанных test classes — count совпал?
- **§2 Test class verification.** Для каждой M-фазы перечислены добавленные test classes (e.g. `M73Step7Tests`, `M73Phase2DebtTests`) — каждый class реально присутствует в `tests/`, реально имеет указанное число test methods?
- **§3 Acceptance criteria coverage.** Для каждой M-фазы acceptance bullets из ROADMAP → file:line + test:name. Любая acceptance bullet без идентифицируемого артефакта → Tier 1 (missing-required-implementation).
- **§4 Carried debts forward.** Каждый carried debt (Phase 2 → M7 уже закрыт в M7.3; Phase 3 → M10.C; M3.4 → external mod author) — всё ещё carried или закрыт без ratification? Phase 2 особенно — проверить что M7.3 действительно закрыл `M73Phase2DebtTests` как заявлено.
- **§5 Deliberate interpretations.** Каждая зарегистрированная (M5.2 cascade-failure, M7 §9.2/§9.3, M7 §9.5/§9.5.1) — wording спеки v1.4 действительно совместим с интерпретацией? Каждый bullet — Tier 2 confirm или Tier 0 escalation.
- **§6 Closed-phase historical fields.** Phase 0–4 + Persistence + M0–M6 + M7.1–M7.3 — заявленные test counts на момент closure совпадают с per-commit реальностью (verified through reading commit-anchored test file states).
- **§7 Pending phases.** M7.4, M7.5, M7-closure, M8, M9, M10.A–M10.D — no implementation expected; Pass 3 проверяет только что они marked pending корректно и unblocking dependencies валидны.
- **§8 Three-commit invariant for recent batches.** M7.1, M7.2, M7.3 коммиты (`a2ab761`, `c964475`, `2531ed7`, `d68ba93`, `9bed1a4`, `46b4f33`) — проверить через `.git/logs/HEAD` что батчи целостные, каждый имеет scope-prefix per METHODOLOGY §7.3.
- **§9 Surgical fixes applied this pass.** `0` (Pass 3 не remediates).
- **§10 Items requiring follow-up.** All findings grouped by Tier.

**Длительность:** одна сессия Opus, ~2–2.5 часа.

---

### Pass 4 — Cross-doc Consistency & Translation Completeness

**Цель:** проверить что v1.4 changelog отражён везде где надо; что translation campaign фактически закрыта; что README в подпапках актуальны; что нет stale-references в активной навигации.

**Контракт-вход:**
- `AUDIT_PASS_1_INVENTORY.md` (locked).
- `AUDIT_PASS_3_ROADMAP_REALITY.md` carry-debts list (для cross-check completeness).
- `MOD_OS_ARCHITECTURE.md` v1.4 changelog (§ Version history).
- `NORMALIZATION_REPORT.md` + `PASS_*_NOTES.md` + `PASS_4_REPORT.md`.
- Read-only доступ ко всем `.md` в проекте.

**Контракт-выход:** `docs/audit/AUDIT_PASS_4_CROSS_DOC.md` в closure-review формате:

- **§0 Executive summary.** Таблица 8 проверок: v1.4 changelog reflection, stale-reference sweep, test count drift, bus count drift, cyrillic completeness, sub-folder README accuracy, closure review audit-trail integrity, translation campaign closure validation.
- **§1 v1.4 changelog reflection.**
  - `ROADMAP.md` ссылки на `v1.4 LOCKED` присутствуют в header, see-also, M0 Output column.
  - `docs/README.md` Architecture section: `MOD_OS_ARCHITECTURE` row упоминает `v1.4 LOCKED`.
  - `MOD_PIPELINE.md`, `MODDING.md`, `ISOLATION.md` — обновлены с учётом v1.4 §9.5/§9.5.1, или явно помечены как «pre-v1.4 surface».
- **§2 Stale-reference sweep.** Шаблоны: `v1.0 LOCKED`, `v1.1 LOCKED`, `v1.2 LOCKED`, `v1.3 LOCKED` — все хиты должны быть либо в historical context (closure reviews, changelog entries), либо явно объяснимы (M0 Output column как закрепление состояния на момент Phase 0). Любой active-navigation хит на устаревшую версию → Tier 3.
- **§3 Test count drift.** `247`, `260`, `311`, `328`, `333`, `338`, `353`, `364`, `369` — каждый должен встречаться только в historical context; current canonical число — `369/369`.
- **§4 Bus count drift.** `пять шин`, `пяти шин`, `5 buses`, `five buses`, `five domain buses` — все хиты должны быть либо в historical context (`learning/PHASE_1.md`, `ARCHITECTURE.md` v0.1 changelog), либо Tier 3.
- **§5 Cyrillic completeness.** `grep -r --include='*.cs' '[А-Яа-я]' src/ tests/ mods/`. Ожидание: пусто.
  - **Per §7.6 ratified:** для <10 матчей — Pass 4 разбирает per-file, тиерует каждый, и если все Tier 2/3 — записывает в whitelist update.
  - Для 10+ матчей — Pass 4 эскалирует к человеку: «translation campaign reopen?» — и останавливается на этой секции до решения.
- **§6 Sub-folder README accuracy.** `src/DualFrontier.Contracts/Bus/README.md` (после §2.1 NORMALIZATION_REPORT — `шесть шин`), `src/DualFrontier.Core/Bus/README.md` (аналогично), все остальные README в подпапках — quantitative claims ↔ реальность.
- **§7 Closure review audit-trail integrity.** `M3_CLOSURE_REVIEW`, `M4_CLOSURE_REVIEW`, `M5_CLOSURE_REVIEW`, `M6_CLOSURE_REVIEW` — `Date:` поле и upper bound коммитов соответствуют `.git/logs/HEAD`. Ratification mentions в `MOD_OS_ARCHITECTURE` §changelog cross-referenced corretно.
- **§8 Translation campaign closure validation.** `PASS_4_REPORT.md` (QA verification) подписывает все три предыдущих pass — Pass 4 кампании-аудита читает его как контракт-вход и подтверждает, что residual cyrillic в `.cs` не появился после QA.
- **§9 Surgical fixes applied this pass.** `0`.
- **§10 Items requiring follow-up.** All findings grouped by Tier.

**Длительность:** одна сессия Opus, ~1.5–2 часа.

---

### Pass 5 — Triage & Final Report

**Цель:** агрегировать findings из Pass 2/3/4, применить тиерацию, выдать `AUDIT_REPORT.md` в полном формате closure review (8-section structure из `M6_CLOSURE_REVIEW.md`).

**Контракт-вход:**
- `AUDIT_PASS_2_SPEC_CODE.md` (locked).
- `AUDIT_PASS_3_ROADMAP_REALITY.md` (locked).
- `AUDIT_PASS_4_CROSS_DOC.md` (locked).
- `AUDIT_PASS_1_INVENTORY.md` для baseline reference.

**Контракт-выход:** `docs/audit/AUDIT_REPORT.md` — финальный документ кампании.

**Структура (полный closure review style, mirroring `M6_CLOSURE_REVIEW.md`):**

- **§0 Executive summary.** Таблица 8 проверок: Build & test integrity (Pass 1+3), Spec ↔ code ↔ test triple consistency (Pass 2), Cross-document consistency (Pass 4), Stale-reference sweep (Pass 4), Methodology compliance (Pass 3), Sub-phase acceptance criteria coverage (Pass 3), Carried debts forward (Pass 3), Ready-for-M7.4 readiness (Pass 5 derives). Каждая проверка PASSED/FAILED + summary.
- **§1 Build & test integrity** — derived from Pass 1 §1+§2 + Pass 3 §1+§8.
- **§2 Spec ↔ code ↔ test triple consistency** — derived from Pass 2 §1–§11.
- **§3 Cross-document consistency** — derived from Pass 4 §1+§2.
- **§4 Stale-reference sweep** — derived from Pass 4 §2+§3+§4+§6.
- **§5 Methodology compliance** — derived from Pass 3 §8 + новый Pass 5 анализ scope-prefix discipline through M7.x batches.
- **§6 Sub-phase acceptance criteria coverage** — derived from Pass 3 §3.
- **§7 Carried debts forward** — derived from Pass 3 §4.
- **§8 Ready-for-M7.4 readiness** — производный анализ: что блокирует M7.4 (build-pipeline override per D-7), что не блокирует. По образцу `M6_CLOSURE_REVIEW §8`.
- **§9 Surgical fixes applied this pass.** Кампания не выполняет remediation — здесь `0`. Tier 0/Tier 1 finding'и эскалированы в §10 как блокирующие.
- **§10 Items requiring follow-up.** Все Tier 0/1/2/3/4 finding'и из Pass 2/3/4, сгруппированные по приоритету. Каждое finding имеет (`origin_pass`, `spec_or_doc_section`, `code_or_doc_path`, `tier`, `recommendation`).
- **§11 Verification end-state.** Финальный verdict GREEN / YELLOW / RED + summary blocking items для M7.4 если есть.

**Длительность:** одна сессия Opus, ~1.5–2 часа.

---

## 3. Контракты между проходами

Каждый проход — самодостаточная сессия Opus. Контракт между ними — формальный и проверяемый.

| Pass | Принимает на вход | Выдаёт на выход | Tip-off для следующего pass |
|---|---|---|---|
| 1 | repo HEAD + `.git/` read access + spec v1.4 | `AUDIT_PASS_1_INVENTORY.md` (closure-review style) | §9 Sequence catalogue → Pass 2 sequence integrity sub-pass |
| 2 | Pass 1 (locked) + spec | `AUDIT_PASS_2_SPEC_CODE.md` (closure-review style) | §11 Sequence integrity findings + spec drift list per Tier |
| 3 | Pass 1 (locked) + ROADMAP | `AUDIT_PASS_3_ROADMAP_REALITY.md` (closure-review style) | Acceptance gaps list + carry-debts current state |
| 4 | Pass 1 (locked) + Pass 3 carry-list + all docs | `AUDIT_PASS_4_CROSS_DOC.md` (closure-review style) | Cross-doc inconsistency list + cyrillic remainder |
| 5 | Pass 2/3/4 outputs (all locked) | `AUDIT_REPORT.md` (полный closure review) | Final verdict + M7.4 readiness statement |

**Ratification step между проходами.** После каждого Pass человек читает выходной артефакт и либо:

- **Approves** → Pass N+1 запускается с этим артефактом как `(locked)` входом.
- **Escalates back** → Pass N перезапускается с явными корректировками в промте.
- **Pivots scope** → план обновляется (этот документ, §0.x → v1.1+), Pass N или N+1 перепланируется.

Без ratification step Pass N+1 не запускается. Это прямо аналогично translation campaign Pass 1 → Pass 2 handshake (см. `NORMALIZATION_REPORT.md` § Open decisions for human).

**Eager Tier 0 escalation overrides ratification.** Per §7.4 ratified — если в любом pass'е обнаружен Tier 0 finding, текущий pass останавливается, артефакт фиксирует partial state, человек получает escalation note **сразу**, не дожидаясь окончания pass'а. Это специальная ветка контракта.

---

## 4. Тиерация находок

Тиерация наследуется от existing closure review pattern с расширением для cumulative-cross-cutting контекста.

| Tier | Определение | Пример | Действие |
|---|---|---|---|
| **Tier 0** | Spec drift против LOCKED v1.4. Код или wording спеки противоречат друг другу. | Спека §9.5 step 7 говорит «10 s timeout», код использует 5 s. | **Блокирует M7.4. Eager escalation** (§7.4) — текущий pass останавливается, артефакт фиксирует partial state. |
| **Tier 1** | Missing required implementation. LOCKED спека требует артефакта; артефакта нет. | Спека §X требует `IModApi.GetOwnManifest()`; метод не реализован. | **Блокирует M7.4** если artifact required for M7.x. **Backlog** если required for M8+. |
| **Tier 2** | Ratified deviation. Зарегистрирован в ROADMAP / changelog как deliberate interpretation. | M5.2 cascade-failure accumulation, M7 §9.2/§9.3 run-flag location. | **Не флагать как defect.** Подтвердить wording спеки v1.4 совместим. Если нет — pivot в Tier 0 escalation. |
| **Tier 3** | Cross-doc или README дрейф. Quantitative claim в README устарел; ссылка в `docs/README.md` указывает на старую версию. | `src/DualFrontier.Core/Bus/README.md` говорит «пять шин» при шести фактических. | **Surgical fix или backlog.** Не блокирует M7.4. |
| **Tier 4** | Cosmetic. Опечатки, битые ссылки, мёртвый код, untested public API без acceptance bullet. | Битая ссылка в `MODDING.md`. | **Backlog.** Не блокирует. |

**Sequence integrity violations** автоматически Tier 0 (структурная ошибка спецификации, способна привести к undefined behavior в реализации).

**Deferred items** (M3.4, по wording'у LOCKED unblocked at first external mod author) — Tier 2 если зарегистрированы корректно, иначе Tier 1.

---

## 5. Sequence integrity check (новое правило кампании)

В ходе подготовки этой кампании human reviewer зафиксировал подозрение на конкретный класс дефекта: в одном из документов и кода была нумерация 4–5–6 с **пропавшим пунктом 5**. Этот класс не ловится grep'ом по ключевым словам — он требует структурной проверки целостности нумерационных последовательностей.

Pass 2 обязан выполнять для каждой нумерованной последовательности следующий чек-лист:

1. **Извлечь полный список нумерованных пунктов** из источника (LOCKED спека, код, ROADMAP, тесты).
2. **Gap check.** Если пункты 4 и 6 присутствуют, а 5 отсутствует — Tier 0 finding с пометкой `sequence_integrity:gap`.
3. **Duplicate check.** Если 5а и 5б оба явно существуют без обоснования (e.g. «5a» как sub-sequence, не дубликат) — Tier 0 `sequence_integrity:duplicate`.
4. **Order check.** Если в исходнике перечисление идёт `4, 6, 5, 7` — Tier 0 `sequence_integrity:order` (даже если все номера присутствуют).
5. **Count consistency check.** Спека говорит «all seven steps» (либо «of N error kinds», «N decisions», и т.д.) — фактическое число пунктов в перечислении ровно N? Если нет — Tier 0 `sequence_integrity:count_mismatch`.

### 5.1 Априорный список последовательностей для проверки

Pass 1 каталогизирует полный набор; Pass 2 проверяет. Известный априорный perimeter:

- `MOD_OS_ARCHITECTURE.md` v1.4:
  - §1.1, §1.2, §1.3 (mod kinds — заявлено три)
  - §2.2 manifest fields list
  - §3.6 hybrid enforcement layers
  - §4.1, §4.2, §4.3 IModApi methods
  - §5.2 shared mod compliance steps (numbered 1, 2, 3, ...)
  - §7.1, §7.2, §7.4, §7.5 bridge replacement scenarios (заявлено четыре + 5-й М6→М7 hand-off)
  - §9.5 unload chain (steps 1–7, plus §9.5.1 sub-section)
  - §11.1 acceptance bullets per M-phase (each M-row has its own sub-list)
  - §11.2 ValidationErrorKind enumeration (заявлено N kinds)
  - §12 detail decisions D-1..D-7 (ровно семь)
- `ROADMAP.md`:
  - M3.1..M3.4 (M3.4 deferred, marked явно)
  - M5.1..M5.2
  - M6.1..M6.2 (после M6.3 как closure-sync mechanism)
  - M7.1..M7.5 + M7-closure (заявлено пять под-фаз + closure)
  - M10.A..M10.D (vanilla slices — заявлено четыре)
- Код:
  - `ValidationErrorKind` enum members
  - `ContractValidator` phases (A, B, C, D, E, F, G, H — заявлено восемь)
  - `IModApi` methods
  - `SystemExecutionContext` violation builders

Каждая последовательность в каталоге Pass 1 → каждая последовательность проверяется в Pass 2 § Sequence integrity findings.

### 5.2 Особое внимание — диапазон 4–6 в LOCKED v1.4

Учитывая конкретное подозрение human reviewer на gap в районе пунктов 4–6, Pass 2 **обязан** уделить отдельное внимание этим диапазонам в априорном списке выше, и зарегистрировать explicit confirm-or-deny для каждой последовательности, где соответствующий range присутствует.

Кандидаты с особым вниманием (диапазон 4–6 в нумерации):
- `MOD_OS_ARCHITECTURE.md` §7.5 acceptance scenarios — заявлено 5 пунктов (1, 2, 3, 4, 5). 5-й — М6→М7 hand-off.
- `MOD_OS_ARCHITECTURE.md` §9.5 unload chain — 7 шагов, диапазон 4–6 это steps 4 (graph rebuild), 5 (scheduler swap), 6 (ALC.Unload).
- `MOD_OS_ARCHITECTURE.md` §11.2 ValidationErrorKind — список ошибок с потенциальным gap'ом.
- `ContractValidator` phases — 8 phase'ов, диапазон 4–6 это Phases D, E, F.
- `M3..M6_CLOSURE_REVIEW.md` § secitons — 8 проверок в Executive Summary, диапазон 4–6 это checks 4 (Stale-reference sweep), 5 (Methodology compliance), 6 (Sub-phase acceptance criteria coverage).

---

## 6. Whitelist ratified deviations (НЕ флагать как drift)

Этот whitelist обязан быть зачитан Pass 2/3/4 промтами явно. Любая сущность из whitelist'а — **не дрейф**, а зарегистрированное расхождение.

### 6.1 Changelog ratifications

- v1.1 — non-semantic corrections от M1–M3.1 audit (Log → ModLogLevel, dependency.optional).
- v1.2 — non-semantic corrections от M3 closure review (§3.6 hybrid enforcement, §3.5+§2.1 example manifest, M3.4 deferred).
- v1.3 — non-semantic correction от M4.3 implementation review (§2.2 entryAssembly/entryType wording «must be empty» вместо «ignored»).
- v1.4 — non-semantic clarifications от M7 pre-flight readiness review (§9.5 step 7 GC pump bracket, §9.5.1 failure semantics).

### 6.2 Deliberate interpretations в ROADMAP

- **M5.2 cascade-failure accumulation.** ROADMAP M5.2 интерпретирует §8.7 wording «cascade-fail» как «accumulate without skip». Если v1.4 §8.7 wording всё ещё совместим — Tier 2.
- **M7 §9.2/§9.3 run-flag location.** ROADMAP M7 интерпретирует §9.2/§9.3 как «flag on `ModIntegrationPipeline`, not in scheduler». Если v1.4 §9.2/§9.3 wording совместим — Tier 2.
- **M7 §9.5/§9.5.1 step 7 ordering.** ROADMAP M7 интерпретирует step 7 как `WR capture → active-set remove → spin`. Если v1.4 §9.5/§9.5.1 wording совместим — Tier 2.

### 6.3 Carried debts forward

- **Phase 2 WeakReference unload tests** → закрыто в M7.3 (`M73Phase2DebtTests`). Не флагать как outstanding.
- **Phase 3 `SocialSystem`/`SkillSystem` Replaceable=false** → carried до M10.C. Tier 2.
- **M3.4 CI Roslyn analyzer** → carried до first external mod author. Tier 2.
- **§7.5 fifth scenario** (mod-unloaded replacement-revert) → закрыто структурно через M7 hot-reload (`Apply` rebuilds graph). Не флагать.

### 6.4 Translation campaign artifacts

- `learning/PHASE_1.md` — self-teaching ритуал, frozen at Phase 1 snapshot. Содержит «5 buses», «пять шин» — это исторически корректно. Не флагать.
- `SESSION_PHASE_4_CLOSURE_REVIEW.md` — preserved verbatim Russian audit trail. Не флагать.
- `M3..M6_CLOSURE_REVIEW.md` — historical audit-trail; могут содержать ссылки на v1.0/v1.1/v1.2/v1.3 LOCKED как закрепление состояния на момент самого review. Не флагать.

### 6.5 Spec language rules

- `MOD_OS_ARCHITECTURE.md` upper-row entries в §0 OS mapping таблице со статусами «Implemented» / «Half-implemented» / «Not implemented» — это снимок на момент Phase 0 LOCKED. Не флагать как stale, кроме случаев когда v1.x changelog явно обновил.

### 6.6 Cyrillic-in-code whitelist

После Pass 4 translation campaign ожидается **пустой** результат `grep '[А-Яа-я]' src/ tests/ mods/ --include='*.cs'`. Whitelist на текущий момент **пуст**: translation campaign была сквозной, и residual в production коде — Tier 3 (бэкло́г) либо Tier 1 (если это пользовательская строка ассертенная тестом).

Per §7.6 ratified discipline:
- <10 матчей: Pass 4 разбирает per-file, тиерует, и при подтверждённом whitelist updates §6.6 этого документа в pivot v1.1.
- 10+ матчей: Pass 4 эскалирует к человеку и останавливается на этой секции до решения «translation campaign reopen?».

---

## 7. Решения, locked v1.0

Шесть открытых решений из v0.1 закрыты ratification 2026-05-01. Раздел сохранён как audit trail.

### 7.1 Папка для аудит-артефактов — **`docs/audit/`** ✓ LOCKED

Выбрано подпапочное размещение. Параллель с `docs/learning/` для self-teaching артефактов и `tests/Fixture.*/` для test fixtures. Pass-артефакты остаются в репозитории как audit trail. `docs/README.md` будет обновлён при ratification AUDIT_REPORT (Pass 5 surgical fix candidate либо backlog).

### 7.2 Артефакт-формат — **closure-review style для всех пяти проходов** ✓ LOCKED

Отвергнут гибрид (stripped tables для Pass 1–4 + full closure-style для Pass 5) в пользу единообразного closure-review формата. Human reviewer мотивировал решение как priority of thoroughness over speed: «проект уже далеко и нет желания потом заниматься отлавливанием крайне сложных багов». Каждый pass-артефакт встаёт в один ряд с `M3..M6_CLOSURE_REVIEW.md` и читается тем же читателем по тем же конвенциям.

### 7.3 Git data — **direct `.git/` read access через Filesystem MCP** ✓ LOCKED

Opus в Pass 1, 3, 4 читает следующие файлы напрямую через `Filesystem:read_text_file`:

- `.git/HEAD` — текущая ветка (содержимое `ref: refs/heads/<branch>`)
- `.git/refs/heads/<branch>` — HEAD commit hash
- `.git/logs/HEAD` — линейная история commit'ов на текущей ветке (формат: `<old> <new> <author> <timestamp> <message>`)
- `.git/logs/refs/heads/<branch>` — эквивалент для конкретной ветки

**Что не доступно напрямую:** `git diff`, `git log --oneline` форматтинг, `git rev-list --count`. Достаточная апроксимация — чтение `.git/logs/HEAD` и парсинг (это plain text).

Human reviewer подтвердил: «Он пройдеться через файловую систему и гит комиты у него должен быть доступ, так как я работал уже через облако». Этот pattern уже работал в существующих сессиях.

### 7.4 Tier 0 эскалация — **eager** ✓ LOCKED

Любой Tier 0 finding (включая sequence integrity violation) останавливает текущий pass в момент обнаружения. Артефакт фиксирует partial state с явной пометкой `INCOMPLETE — eager-escalated on Tier 0`. Человек получает escalation note и принимает решение: либо remediation, либо план revisited, либо tier-downgrade с обоснованием.

Tier 1+ копятся до Pass 5 final report.

### 7.5 Batch reads — **min batch=5 явно прописано в каждом промте** ✓ LOCKED

Каждый Pass-промт содержит explicit instruction: «Use `read_multiple_files` with batches ≥5 entries when reading sub-folder READMEs, manifest fixtures, test classes». Это вшивает performance discipline и предотвращает one-by-one иterations, которые засыпают rate limits на длинных pass'ах.

### 7.6 Cyrillic residual handling — **<10 per-file, 10+ escalate** ✓ LOCKED

См. §6.6 для полного wording'а.

---

## 8. Pipeline routing

Кампания целиком прогоняется через **Opus** — она требует архитектурной интуиции и full system visibility, которые есть только у архитектурного слоя pipeline'а. Sonnet или Gemma здесь не работают.

| Pass | Сессия | Источник | Тип агента | Falsifiability check |
|---|---|---|---|---|
| Pass 1 | 1 (новая) | repo state + `.git/` + промт Pass 1 | Opus | Каждая таблица — точное число + источник; интерпретаций нет |
| Pass 2 | 2 (новая) | Pass 1 (locked) + v1.4 + промт Pass 2 | Opus | Каждый finding имеет (spec_section, code_path/test_path, tier) |
| Pass 3 | 3 (новая) | Pass 1 (locked) + ROADMAP + `.git/logs/HEAD` + промт Pass 3 | Opus | Каждый M-row проверен против реальных артефактов |
| Pass 4 | 4 (новая) | Pass 1 (locked) + Pass 3 carry-list + все docs + промт Pass 4 | Opus | grep cyrillic пуст или whitelist; каждая stale-ref classified |
| Pass 5 | 5 (новая) | Pass 2/3/4 (все locked) + промт Pass 5 | Opus | Финальный отчёт в формате closure review |

**Между сессиями обязателен ratification step** — человек читает выход, либо ратифицирует (`(locked)` для следующего pass'а), либо возвращает на доработку с явными корректировками. **Eager Tier 0 escalation** — отдельная ветка контракта (см. §7.4).

---

## 9. Запуск кампании — последовательность действий

1. **Phase 0 кампании.** ✓ Закрыто 2026-05-01: этот документ ratify до v1.0.
2. **Pass 1 промт draft.** Опираясь на locked план, драфтю `AUDIT_PASS_1_PROMPT.md`. Человек ратифицирует промт.
3. **Pass 1 execution.** Opus в новой сессии получает промт; производит `AUDIT_PASS_1_INVENTORY.md`. Человек ратифицирует.
4. **Pass 2 промт draft + execution.** Аналогично, опираясь на ratified Pass 1.
5. **Pass 3 промт draft + execution.** Параллельно или последовательно с Pass 2 (зависит от Pass 1 inventory completeness).
6. **Pass 4 промт draft + execution.**
7. **Pass 5 промт draft + execution.**
8. **AUDIT_REPORT.md ratification.** Человек принимает финальный отчёт; на основании verdict GREEN/YELLOW/RED — либо M7.4 unblocked, либо remediation phase запускается.

Промты пишутся **по одному, после ratification предыдущего**. Это не оптимизация скорости — это falsifiability discipline. Pass 2 промт зависит от того, что окажется в `AUDIT_PASS_1_INVENTORY.md`; если Pass 1 нашёл больше или меньше, чем ожидалось, Pass 2 промт корректируется.

---

## 10. Stop conditions и эскалации

Кампания останавливается с эскалацией к человеку при любом из:

1. **Tier 0 finding в любом pass'е** (per §7.4 eager escalation).
2. **Pass 1 inventory revealed unexpected state** — например, test count не 369, или branch не `feat/m4-shared-alc`, или v1.4 LOCKED статус строка отсутствует. Это означает план опирается на ложную базу и должен быть переоценён.
3. **Cyrillic count > 10 файлов в `.cs`** (per §7.6 опция 2).
4. **Pass 5 verdict RED.** Кампания закрывается, M7.4 заблокирован, открывается remediation phase (вне scope этой кампании).

---

## 11. См. также

- [TRANSLATION_PLAN](../TRANSLATION_PLAN.md) — прецедент multipass campaign по этому же проекту; шаблон для structure §2.
- [NORMALIZATION_REPORT](../NORMALIZATION_REPORT.md) — пример Pass 1 артефакта (translation campaign Pass 1).
- [M6_CLOSURE_REVIEW](./M6_CLOSURE_REVIEW.md) — образец 8-section closure review формата для всех пяти проходов кампании (per §7.2 LOCKED).
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — §2.4 atomic phase review, §7.3 process discipline; принципы кампании.
- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.4 LOCKED — основная цель аудита.
- [ROADMAP](../ROADMAP.md) — вторая основная цель аудита.

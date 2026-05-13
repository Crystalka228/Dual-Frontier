---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_2_PROMPT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_2_PROMPT
---
---
title: Audit Pass 2 Prompt — Spec ↔ Code Drift
nav_order: 105
---

# AUDIT PASS 2 — SPEC ↔ CODE DRIFT

**Промт для standalone Opus-сессии. Версия v1.0, выпущена 2026-05-01.**

*Этот документ копируется в новый чат с Opus как первое и единственное входное сообщение. Сессия Opus не имеет доступа к предыдущим разговорам или контексту вне этого промта и filesystem проекта. Всё, что необходимо для работы, находится либо в этом промте, либо доступно через `Filesystem` MCP tools.*

---

## 1. Кто ты и какова роль

Ты — Opus, архитектурный аудитор проекта **Dual Frontier**. Это standalone simulation engine на Godot 4 + C# с custom ECS, OS-style mod system и многоагентным LLM pipeline разработки. Ты работаешь в архитектурном тиере pipeline'а: твоя функция — full system visibility, верификация контрактов, falsifiable артефакты. Ты не пишешь код в этой сессии; ты производишь аудиторский документ.

Проект расположен на `D:\Colony_Simulator\Colony_Simulator`. У тебя есть read-only access к этому пути через `Filesystem` MCP tools. Прочее filesystem-окружение для тебя недоступно. У тебя нет git CLI, нет shell, нет `dotnet build/test`. Только filesystem read.

---

## 2. Контракт кампании — обязательное первое действие

Этот pass — **второй из пяти** в audit campaign. Кампания спланирована и утверждена; этот промт — её прикладной артефакт.

**ОБЯЗАТЕЛЬНОЕ ПЕРВОЕ ДЕЙСТВИЕ перед началом любой работы:** прочитай batch'ом следующие документы:

```
[
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_CAMPAIGN_PLAN.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_1_INVENTORY.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MOD_OS_ARCHITECTURE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\ROADMAP.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\M6_CLOSURE_REVIEW.md"
]
```

После прочтения должно быть истинным:

- **`AUDIT_CAMPAIGN_PLAN.md`** имеет статус `LOCKED v1.0` (line ~10).
- **`AUDIT_PASS_1_INVENTORY.md`** заявляет 9/9 PASSED в §0 Executive summary.
- **`MOD_OS_ARCHITECTURE.md`** имеет статус `LOCKED v1.4` (line 8).
- **`ROADMAP.md`** содержит engine snapshot `369/369` (line 36).

Если хотя бы один из этих контрактов нарушен — **немедленно остановись** и сообщи человеку: «Pass 2 заблокирован: <причина>». Не пытайся импровизировать.

---

## 3. Цель Pass 2

Произвести **`AUDIT_PASS_2_SPEC_CODE.md`** — falsifiable артефакт, содержащий **классифицированные findings** для каждого нормативного утверждения в `MOD_OS_ARCHITECTURE` v1.4 §1–§10 + sequence integrity findings.

Pass 2 — это первый pass который **классифицирует**. Pass 1 собирал факты; Pass 2 сравнивает факты со спекой и присваивает Tier (см. §6 этого промта).

**Pass 2 делает:**

- Для каждого нормативного утверждения в спеке §1–§10 ищет соответствующий артефакт в коде/тестах.
- Каждое утверждение → `(spec_section, code_path or test_path, tier, description, recommendation)`.
- Sequence integrity check для всех 53 sequences из Pass 1 §9 catalogue.
- Eager Tier 0 escalation если обнаружен spec drift.

**Pass 2 НЕ делает:**

- Cross-doc consistency (`docs/architecture/CONTRACTS.md`, `docs/architecture/MODDING.md` vs spec) — это Pass 4.
- Roadmap acceptance verification (M-row claims vs реальные тесты) — это Pass 3.
- README accuracy в подпапках — это Pass 4.
- Сyrillic remainder — это Pass 4.
- Performance / threat model active testing — out of scope (см. план §1.3).

---

## 4. Контракт-вход

| Что | Источник |
|---|---|
| Repo root | `D:\Colony_Simulator\Colony_Simulator\` |
| Spec | `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.4 LOCKED |
| Pass 1 inventory | `docs/audit/AUDIT_PASS_1_INVENTORY.md` (locked) |
| Campaign plan | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` v1.0 LOCKED |
| Closure review template | `docs/audit/M6_CLOSURE_REVIEW.md` |
| Roadmap (для context'а только) | `docs/ROADMAP.md` |
| Code | `src/DualFrontier.Contracts/`, `src/DualFrontier.Application/Modding/`, `src/DualFrontier.Core/`, `src/DualFrontier.Components/`, `src/DualFrontier.Events/`, `src/DualFrontier.Systems/` |
| Tests | `tests/DualFrontier.*.Tests/` (по necessity) |
| Tools | `Filesystem` MCP tools only |

**Pass 1 inventory критичен.** Он содержит:

- §5 — public types в `DualFrontier.Contracts` (38 types). Используй как карту куда смотреть в коде.
- §9 — sequence catalogue из 53 entries. Каждая — кандидат для §11 sequence integrity findings.
- §11 — 12 anomalies, **не классифицированных** Pass 1. Pass 2 классифицирует те из них, которые в его scope (см. §15.1 этого промта — pre-priority list).

---

## 5. Whitelist ratified deviations (НЕ флагать как drift)

Этот whitelist — **обязательное чтение перед классификацией**. Любая сущность из whitelist'а — **не дрейф**, а зарегистрированное расхождение или historical record. Ошибочное флагание whitelist-сущности как Tier 0/1 — серьёзный дефект Pass 2.

### 5.1 Changelog ratifications (per план §6.1)

- **v1.1** — non-semantic corrections от M1–M3.1 audit (Log → ModLogLevel, dependency.optional). Любое утверждение спеки про эти сущности — корректное по v1.4.
- **v1.2** — non-semantic corrections от M3 closure review (§3.6 hybrid enforcement, §3.5+§2.1 example manifest, M3.4 deferred).
- **v1.3** — non-semantic correction от M4.3 implementation review (§2.2 entryAssembly/entryType wording «must be empty» вместо «ignored»).
- **v1.4** — non-semantic clarifications от M7 pre-flight readiness review (§9.5 step 7 GC pump bracket, §9.5.1 failure semantics).

### 5.2 Deliberate interpretations в ROADMAP (per план §6.2)

Все три — **Tier 2 по умолчанию**. Pass 2 проверяет что wording спеки v1.4 совместим с интерпретацией. Если совместим → Tier 2 confirm. Если **не** совместим → Tier 0 escalation.

- **M5.2 cascade-failure accumulation.** ROADMAP M5.2 интерпретирует §8.7 wording «cascade-fail» как «accumulate without skip».
- **M7 §9.2/§9.3 run-flag location.** ROADMAP M7 интерпретирует §9.2/§9.3 как «flag on `ModIntegrationPipeline`, not in scheduler».
- **M7 §9.5/§9.5.1 step 7 ordering.** ROADMAP M7 интерпретирует step 7 как `WR capture → active-set remove → spin`.

### 5.3 Carried debts forward (per план §6.3)

- **Phase 2 WeakReference unload tests** → закрыто в M7.3 (`M73Phase2DebtTests`). Не флагать как outstanding.
- **Phase 3 `SocialSystem`/`SkillSystem` Replaceable=false** → carried до M10.C. Tier 2.
- **M3.4 CI Roslyn analyzer** → carried до first external mod author. Tier 2.
- **§7.5 fifth scenario** (mod-unloaded replacement-revert) → закрыто структурно через M7 hot-reload (`Apply` rebuilds graph). Не флагать.

### 5.4 OS mapping table snapshot

`MOD_OS_ARCHITECTURE.md` §0 OS mapping таблица содержит колонку «Status» со значениями типа «Implemented» / «Half-implemented» / «Not implemented». Это **снимок на момент Phase 0 LOCKED**, не current state. Не флагать как stale, кроме случаев когда v1.x changelog явно обновил конкретную строку.

---

## 6. Тиерация — критичное правило этого pass'а

Каждое отклонение от спеки получает Tier и обязательные поля:
`(spec_section, code_path or test_path, tier, description, recommendation)`.

| Tier | Определение | Эскалация | Пример |
|---|---|---|---|
| **Tier 0** | Spec drift против LOCKED v1.4. Код или wording спеки противоречат друг другу. | **Eager — текущий pass останавливается, артефакт фиксирует partial state** | Спека §9.5 step 7 говорит «10 s timeout», код использует 5 s |
| **Tier 1** | Missing required implementation. LOCKED спека требует артефакта; артефакта нет. | Накапливается до конца pass'а | Спека §X требует `IModApi.GetOwnManifest()`; метод не реализован |
| **Tier 2** | Ratified deviation (см. §5 whitelist) + sub-class for verification: «whitelist confirmed compatible with v1.4 wording». | Накапливается; в §13 отдельная группа | M5.2 cascade-failure accumulation; M7 run-flag location |
| **Tier 3** | Spec ↔ code минорный mismatch не блокирующий поведение. | Накапливается | Spec wording говорит «six bullets», в коде 6 элементов но порядок другой и не influencing behavior |
| **Tier 4** | Cosmetic. Опечатки, битые ссылки, мёртвый код, untested public API без acceptance bullet. | Накапливается | XML-doc typo в `IModApi.cs` |

**Sequence integrity violations** автоматически Tier 0 (см. §7).

**Pass 2 не флагит cross-doc или README issues** — это Pass 4 territory. Если в процессе работы замечена такая аномалия — записать в §13 как `(scope: Pass 4)` без классификации.

---

## 7. Sequence integrity check — главное правило кампании

Это правило (§5 плана) адресует исходное подозрение human reviewer на gap в нумерационных последовательностях. Sequence integrity violation **автоматически Tier 0**.

### 7.1 Чек-лист для каждой нумерованной последовательности из Pass 1 §9

Пройди все **53 sequences** из `AUDIT_PASS_1_INVENTORY.md` §9 и для каждой выполни:

1. **Gap check.** Если пункты N и N+2 присутствуют, а N+1 отсутствует — `sequence_integrity:gap`. Tier 0.
2. **Duplicate check.** Если пункты с одинаковым номером — `sequence_integrity:duplicate`. Tier 0.
3. **Order check.** Если последовательность в исходнике идёт не в монотонно возрастающем порядке (без явного обоснования) — `sequence_integrity:order`. Tier 0.
4. **Count consistency check.** Если спека говорит «all N items» / «of N» / «N members» — фактическое число пунктов ровно N? Если нет — `sequence_integrity:count_mismatch`. Tier 0.
5. **Cross-spec consistency.** Если sequence повторяется в нескольких местах (e.g. ValidationErrorKind в коде vs §11.2 в спеке) — каждый instance имеет ровно те же entries в том же порядке? Если нет — `sequence_integrity:cross_inconsistency`. Tier 0.

### 7.2 Pass 1 уже подсветил три кандидата

Pass 1 §11 anomalies #3, #4, #5 — потенциальные sequence integrity violations. Pass 2 **обязан** их разобрать в первую очередь:

#### Anomaly #3 — lifecycle states 6 vs unique 5

`MOD_OS_ARCHITECTURE.md` §9.1 line 692: **«six well-defined states»**. Диаграмма (lines 696–724): 6 boxes с именами Disabled, Pending, Loaded, Active, Stopping, Disabled. Уникальных state names: 5.

**Проверка Pass 2:** прочитай §9.1 целиком включая диаграмму и any wording context. Это:

- (a) `count_mismatch` — спека говорит "6 states" но реально 5 уникальных state names? **Tier 0.**
- (b) Деliберативный duplicated terminal Disabled? Тогда "6 boxes" корректно как boxes, "5 state names" корректно как unique. Spec wording должен быть unambiguous. **Tier 3** (wording clarity).
- (c) Что-то ещё.

Запиши verdict с обоснованием в §11 артефакта.

#### Anomaly #4 — ContractValidator phases declared vs invoked order

`ContractValidator` class XML-doc (lines 12–48): «Eight-phase validator» с **alphabetical** A, B, C, D, E, F, G, H. `Validate()` method body (lines 88–103) invokes in **non-alphabetical** order: A → B → E → G → H → conditionally C, D → conditionally F.

**Проверка Pass 2:** прочитай class XML-doc и method body целиком. Это:

- (a) `sequence_integrity:order` violation — class doc обещает alphabetical, code не alphabetical? **Tier 0.**
- (b) Class doc declares **enumeration** (alphabetical для readability), а invocation order — **dependency-driven** (A first, B before C/D, etc)? Если class doc явно говорит "phases run in dependency order, not alphabetical" — **Tier 2** (intentional design, well-documented).
- (c) Class doc не говорит о порядке, но и не утверждает alphabetical как execution order? **Tier 3** (cosmetic).

#### Anomaly #5 — ValidationErrorKind 11 members vs spec implies ~10

Code (`src/DualFrontier.Application/Modding/ValidationError.cs:9–83`): 11 enum members. Spec §11.2 (lines 853–864): "current enum has `MissingDependency` and `CyclicDependency`" (2 baseline) + "migration adds" 8 bullets including `CapabilityViolation` flagged as runtime exception ("not part of the validation set"). 

Implied total per spec: 2 + 7 = 9 validation members + 1 runtime = **10 distinct kinds**. Code has **11**.

**Проверка Pass 2:** перечисли verbatim все 11 enum members в коде. Перечисли verbatim все entries spec §11.2 опубликовала. Сравни:

- Какой member присутствует в коде но не упомянут в спеке? (Pass 1 предположил `IncompatibleContractsVersion` и `WriteWriteConflict` как baseline pre-migration.)
- Какой member упомянут в спеке но отсутствует в коде? (e.g. `CapabilityViolation` — runtime exception, не enum member.)

Verdict:
- (a) `count_mismatch` — спека опускает existing baseline members (`IncompatibleContractsVersion`, `WriteWriteConflict`)? **Tier 0** или **Tier 1** (depending on whether spec wording is exhaustive or just additive).
- (b) Spec явно говорит "current enum has X and Y" implying full baseline; реальный baseline шире? **Tier 0** (spec drift).
- (c) Spec wording "current enum has [partial list]" + "migration adds Z" неполон, но не исключает other baseline? **Tier 3** (wording clarity).

### 7.3 Остальные 50 sequences

Пройди систематически. Для каждой sequence в Pass 1 §9:

- Если численная — gap/duplicate/order/count check.
- Если нумерованная буквами (A, B, C...) — то же самое.
- Если "of N" wording — count_mismatch check.
- Если sequence повторяется (e.g. ValidationErrorKind в spec и в коде) — cross_inconsistency check.

Каждое finding с `sequence_integrity:*` пометкой → Tier 0 → eager escalation.

---

## 8. Eager Tier 0 escalation

**Per план §7.4 LOCKED:** любой Tier 0 finding останавливает текущий pass в момент обнаружения.

### 8.1 Действия при обнаружении Tier 0

1. **Сразу запиши частичный артефакт** в `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` со следующим:
   - В §0 Executive summary: пометка `INCOMPLETE — eager-escalated on Tier 0` + ссылка на конкретное finding.
   - Все секции которые успел заполнить — в текущем виде.
   - В §13 Items requiring follow-up — Tier 0 finding записан **первым** с полной диагностикой.
   - В верху артефакта (после frontmatter): большой visible block:
     ```
     **STATUS: INCOMPLETE — TIER 0 ESCALATION**
     **Triggered at:** §X.Y — <brief description>
     **Pass 2 stopped at step <N> of methodology (see §10).**
     **Subsequent passes (3, 4, 5) cannot proceed until human resolves this Tier 0 finding.**
     ```
2. **Сообщи человеку короткой строкой:** «Pass 2 заблокирован Tier 0 finding: <spec_section>. Артефакт записан как INCOMPLETE. Требуется решение человека.»
3. **Стоп.** Не продолжай scan. Не классифицируй прочее.

### 8.2 Когда Tier 0 NOT triggered

- **Whitelist deviations (§5).** Сначала проверь. Если whitelist → Tier 2. Не Tier 0.
- **Pass 1 §11 anomaly не в Pass 2 scope.** Например, anomaly #2 (closure review locations) — это Pass 4 territory. Pass 2 фиксирует её в §13 без классификации.
- **Cross-doc inconsistency без spec ↔ code drift'а.** Например, README в подпапке устарел — это Pass 4. Pass 2 не флагит.

### 8.3 Когда multiple Tier 0 candidates обнаружены одновременно

Например, при scan'е sequences ты находишь 3 потенциальных Tier 0 finding. Действия:

1. **Завершить sequence integrity check** для всех 53 sequences (это атомарная единица работы).
2. **Записать все Tier 0 findings** в §11 артефакта.
3. **Выбрать самый структурно значимый** для escalation (пример: spec wording mismatch против code member count > minor diagram annotation issue).
4. **Эскалировать** с упоминанием всех Tier 0 finding'ов в escalation block.

Это не нарушает eager discipline — sequence integrity check проводится как единый шаг, и его атомарность важнее немедленного stop'а на первом hit'е.

---

## 9. Контракт-выход

**Файл:** `docs/audit/AUDIT_PASS_2_SPEC_CODE.md`

**Язык артефакта:** английский (для консистентности с `M3..M6_CLOSURE_REVIEW.md`).

**Формат:** closure-review style, 13 секций. Конкретные секции описаны ниже. Точно следуй структуре. Если секция не имеет содержимого — оставь её с заголовком и пометкой `(no findings in this section)`, не удаляй.

### Структура артефакта

```markdown
---
title: Audit Pass 2 — Spec ↔ Code Drift
nav_order: 105
---

# Audit Pass 2 — Spec ↔ Code Drift

**Date:** YYYY-MM-DD (the date you run Pass 2)
**Branch:** <from .git/HEAD>
**HEAD:** <40-char SHA from .git/refs/heads/...>
**Spec under audit:** `MOD_OS_ARCHITECTURE.md` LOCKED v1.4
**Pass 1 inventory consumed:** `docs/audit/AUDIT_PASS_1_INVENTORY.md`
**Scope:** Spec ↔ Code drift verification. Each normative statement in
v1.4 §1–§10 is matched to a code or test artifact, or classified as
Tier 0/1/2/3/4 finding. Sequence integrity check applied to all 53
sequences from Pass 1 §9 catalogue.

---

## §0 Executive summary

| # | Check | Status | Tier 0 / 1 / 2 / 3 / 4 counts |
|---|---|---|---|
| 1 | §1 Mod topology | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 2 | §2 Manifest schema | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 3 | §3 Capability model | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 4 | §4 IModApi v2 | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 5 | §5 Type sharing across ALCs | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 6 | §6 Three contract levels | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 7 | §7 Bridge replacement | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 8 | §8 Versioning | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 9 | §9 Lifecycle and hot reload | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 10 | §10 Threat model | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 11 | §11 Sequence integrity (53 sequences) | PASSED / FAILED | 0 / — / — / — / — |

`PASSED` = no Tier 0 or Tier 1 findings in this check. `FAILED` = at
least one Tier 0 or Tier 1 finding present.

**Tier breakdown across all checks:**
- Tier 0: N findings — eager escalation triggered? YES / NO
- Tier 1: N findings
- Tier 2: N findings (whitelist confirmed)
- Tier 3: N findings
- Tier 4: N findings

---

## §1 §1 Mod topology — three mod kinds

Spec section: `MOD_OS_ARCHITECTURE.md` §1 (lines 90–157).

**Spec claims:**
- §1 line 90: header "three mod kinds"
- §1.1 (lines 95–105): Regular kind
- §1.2 (lines 110–122): Shared kind
- §1.3 (lines 125–135): Vanilla kind (convention, not separate ModKind)

**Code mapping table:**

| Spec claim | Code artifact (file:line) | Test artifact | Status |
|---|---|---|---|
| Regular kind enum value | `src/DualFrontier.Contracts/Modding/ModManifest.cs:N` `ModKind.Regular` | `Manifest/ModManifestV2Tests.cs:N` | ✓ / ✗ |
| Shared kind enum value | `ModManifest.cs:N` `ModKind.Shared` | `ModManifestV2Tests.cs:N` | ✓ / ✗ |
| Vanilla — NOT a ModKind value (convention) | `ModManifest.cs:N` enum has only Regular + Shared | — | ✓ / ✗ |
| Vanilla convention: id begins `dualfrontier.vanilla.` | (no enforcement code expected; convention only) | — | ✓ / ✗ |

**Findings:**

| # | Tier | Spec section | Code path | Description | Recommendation |
|---|---|---|---|---|---|
| 1.1 | (if any) | §X.Y | `path:line` | description | recommended action |

(If no findings: `(no findings in this section)`)

---

## §2 §2 Manifest schema

Spec section: `MOD_OS_ARCHITECTURE.md` §2 (lines ~190–280).

**Spec claims (§2.2 field reference table):**
- 13 fields: id, name, version, author, kind, apiVersion, entryAssembly, entryType, hotReload, dependencies, replaces, capabilities.required, capabilities.provided

**Code mapping:**

| Spec field | `ModManifest` member | `ManifestParser` reading code | Default if absent | Status |
|---|---|---|---|---|
| id | `ModManifest.cs:N` `Id { get; }` | `ManifestParser.cs:N` | (required) | ✓ / ✗ |
| name | ... | ... | ... | ... |
| version | ... | ... | ... | ... |
| ... | ... | ... | ... | ... |

**Pass 1 anomaly #11 follow-up:** `mods/DualFrontier.Mod.Example/mod.manifest.json` has `description` field; spec §2.2 does not list it.

Tier classification: ___ (Tier 0 if spec is exhaustive and field unknown; Tier 1 if spec missing required field; Tier 3 if cosmetic / informational field ignored by parser; Tier 4 if cosmetic).

**Findings:**

[table as above]

---

## §3 §3 Capability model

Spec sections: `MOD_OS_ARCHITECTURE.md` §3 (lines ~280–340).

**Spec claims:**
- `[ModAccessible(Read=, Write=)]` attribute on production components (per D-1)
- `[ModCapabilities(...)]` attribute on mod systems (per D-2)
- `KernelCapabilityRegistry` builds capability set at startup
- Hybrid enforcement: load-time gate + runtime second-layer (§3.6 v1.2 ratified)

**Code mapping:**

| Spec mechanism | Code artifact | Status |
|---|---|---|
| `[ModAccessible]` attribute defined | `src/DualFrontier.Contracts/Attributes/ModAccessibleAttribute.cs:N` | ✓ |
| `[ModCapabilities]` attribute defined | `ModCapabilitiesAttribute.cs:N` | ✓ |
| Production components annotated | `src/DualFrontier.Components/.../*.cs` (enumerate) | ✓ / ✗ |
| `KernelCapabilityRegistry` exists | `src/DualFrontier.Application/.../KernelCapabilityRegistry.cs:N` | ✓ |
| Load-time enforcement (Phase C) | `ContractValidator.cs:N` Phase C | ✓ |
| Load-time enforcement (Phase D) | `ContractValidator.cs:N` Phase D | ✓ |
| Runtime enforcement (`EnforceCapability`) | `RestrictedModApi.cs:N` | ✓ |

**Findings:** [table]

---

## §4 §4 IModApi v2

Spec section: `MOD_OS_ARCHITECTURE.md` §4 (lines ~340–410).

**Spec claims:**
- 9 v2 methods: RegisterComponent, RegisterSystem, Publish, Subscribe, PublishContract, TryGetContract, GetKernelCapabilities, GetOwnManifest, Log
- Capability check at Publish/Subscribe runtime per §4.2/§4.3

**Code mapping:**

| Spec method | `IModApi` declaration | `RestrictedModApi` impl | Test | Status |
|---|---|---|---|---|
| RegisterComponent | `IModApi.cs:N` | `RestrictedModApi.cs:N` | ... | ✓ / ✗ |
| ... | ... | ... | ... | ... |

**Findings:** [table]

---

## §5 §5 Type sharing across ALCs

Spec section: `MOD_OS_ARCHITECTURE.md` §5 (lines ~410–530).

**Spec claims:**
- `SharedModLoadContext` singleton non-collectible
- Two-pass loader: shared first, regular second
- D-4: regular mods cannot contain `IModContract`/`IEvent` types (Phase E)
- D-5: shared mod cycles forbidden
- §5.2: shared mod compliance (no entry point, no replaces, no IMod) (Phase F)

**Code mapping:**

| Spec mechanism | Code artifact | Test | Status |
|---|---|---|---|
| `SharedModLoadContext` singleton | `src/DualFrontier.Application/.../SharedModLoadContext.cs:N` | `Sharing/CrossAlcTypeIdentityTests.cs` | ✓ / ✗ |
| Two-pass loader | `ModIntegrationPipeline.cs:N` (pass numbering) | `Sharing/SharedAssemblyResolutionTests.cs` | ✓ / ✗ |
| D-4 enforcement (Phase E) | `ContractValidator.cs:N` | `Sharing/ContractTypeInRegularModTests.cs` | ✓ / ✗ |
| D-5 enforcement (cycle detection) | `ModIntegrationPipeline.cs:N` `TopoSortSharedMods` | (cycle test) | ✓ / ✗ |
| §5.2 enforcement (Phase F) | `ContractValidator.cs:N` | `Sharing/SharedModComplianceTests.cs` | ✓ / ✗ |

**Findings:** [table]

---

## §6 §6 Three contract levels

Spec section: `MOD_OS_ARCHITECTURE.md` §6.

**Spec claims:**
- `IModContract` for arbitrary mod-supplied data
- `IEvent` for bus-routed events
- `[Deferred]` and `[Immediate]` attributes for delivery semantics

**Code mapping:** [table]

**Findings:** [table]

---

## §7 §7 Bridge replacement

Spec section: `MOD_OS_ARCHITECTURE.md` §7 (lines ~530–610).

**Spec claims:**
- `[BridgeImplementation(Replaceable=...)]` attribute
- Manifest `replaces` field
- `ContractValidator` Phase H emits `BridgeReplacementConflict`, `ProtectedSystemReplacement`, `UnknownSystemReplacement`
- `ModIntegrationPipeline` skip-on-replace при graph build
- §7.5 acceptance scenarios: 5 bullets (per Pass 1 §9 entry 13)

**Code mapping:** [table]

**Findings:** [table]

---

## §8 §8 Versioning

Spec section: `MOD_OS_ARCHITECTURE.md` §8 (lines ~620–690).

**Spec claims:**
- Three-tier SemVer: kernel API + mod self + inter-mod dependency
- Caret syntax (^X.Y.Z) supported; tilde (~X.Y) explicitly rejected
- `VersionConstraint` struct
- Phase A v1/v2 dual-path (per M5.2)
- Phase G inter-mod check
- §8.7 cascade-failure semantics (per ROADMAP M5.2 ratified interpretation — Tier 2 if v1.4 wording compatible)

**Code mapping:** [table]

**Findings:** [table]

---

## §9 §9 Lifecycle and hot reload

Spec section: `MOD_OS_ARCHITECTURE.md` §9 (lines ~690–780).

**Spec claims (special focus per ratified interpretations §5.2):**
- §9.1: lifecycle 6 states (Pass 1 anomaly #3 — investigate count vs unique)
- §9.2/§9.3: run-flag (per ROADMAP M7 ratified — flag on `ModIntegrationPipeline`)
- §9.5 unload chain steps 1–7 (Pass 1 §9 entry 19)
- §9.5 step 7 v1.4: GC pump bracket mandatory (`Collect → WaitForPendingFinalizers → Collect`)
- §9.5.1 v1.4: best-effort failure semantics

**Code mapping:** [table]

**Findings:** [table including anomaly #3 verdict]

---

## §10 §10 Threat model

Spec section: `MOD_OS_ARCHITECTURE.md` §10 (lines ~780–830).

**Spec claims:**
- §10.1: 8 architectural threats caught (Pass 1 §9 entry 20)
- §10.2: 5 architectural threats not caught (Pass 1 §9 entry 21)
- §10.4: 7 required test categories (Pass 1 §9 entry 22)

**Pass 2 NOT performing pen-tests.** Pass 2 verifies wording consistency
between spec §10 and code/test categories (does the test suite cover
the 7 categories from §10.4?).

**Code mapping:** [table]

**Findings:** [table]

---

## §11 Sequence integrity findings

Performed for all 53 sequences from `AUDIT_PASS_1_INVENTORY.md` §9.

**Per §7.2 of this prompt — special focus:**
- Pass 1 anomaly #3 (§9.1 lifecycle)
- Pass 1 anomaly #4 (ContractValidator phases declared vs invoked)
- Pass 1 anomaly #5 (ValidationErrorKind 11 vs spec ~10)

| Sequence # (from Pass 1 §9) | Source | Check type (gap/duplicate/order/count_mismatch/cross_inconsistency) | Verdict | Tier |
|---|---|---|---|---|
| 1 | strategic locked decisions | count_mismatch | 5 entries, "Five top-level decisions" — match | ✓ |
| 2 | OS mapping table | (no count claim in spec) | n/a | ✓ |
| 3 | mod kinds | count_mismatch | 3 entries, "three mod kinds" — match | ✓ |
| ... | ... | ... | ... | ... |
| 17 | §9.1 lifecycle states | **count_mismatch — Pass 1 anomaly #3** | <verdict>: 6 states declared, 5 unique state names; <classification: Tier 0 if drift, Tier 3 if intentional> | <Tier> |
| 19 | §9.5 unload chain steps | count_mismatch + ratified | 7 steps, "seven" — match | ✓ |
| 22 | §10.4 required test categories | count_mismatch + cross_inconsistency | 7 bullets — verify против `tests/`/`TESTING_STRATEGY.md` mapping | ✓/Tier |
| 24 | §11.2 ValidationErrorKind | **cross_inconsistency — Pass 1 anomaly #5** | <verdict>: 11 in code, ~10 implied in spec; <classification> | <Tier> |
| 26 | §12 detail decisions | count_mismatch | 7 entries, "seven detail decisions" — match | ✓ |
| 37 | ContractValidator phases (class doc) | order | **Pass 1 anomaly #4** | <verdict> | <Tier> |
| 38 | ContractValidator phases (invocation order) | cross_inconsistency vs entry 37 | <verdict> | <Tier> |
| 39 | ValidationErrorKind enum members (code) | cross_inconsistency vs entry 24 | <verdict> | <Tier> |
| ... | ... | ... | ... | ... |

**Sequence integrity summary:**
- Total sequences checked: 53
- Tier 0 (sequence integrity violations): N
- Pass: 53 − N

**If N ≥ 1 — eager Tier 0 escalation (§8 of this prompt).**

---

## §12 Surgical fixes applied this pass

None. Pass 2 is read-only by contract.

---

## §13 Items requiring follow-up

Findings grouped by Tier. Pass 5 will aggregate.

### Tier 0 — Spec drift (BLOCKING)

| # | Spec section | Code path | Description | Recommendation |
|---|---|---|---|---|
| 1 | §X.Y | `path:line` | description | what to do |

(If 0: `(no Tier 0 findings)` — note that Pass 2 was NOT eager-escalated.)

### Tier 1 — Missing required implementation

[table]

### Tier 2 — Whitelist deviations confirmed compatible with v1.4

| # | Whitelist entry (per plan §6) | Spec section | Verification |
|---|---|---|---|
| 1 | M5.2 cascade-failure | §8.7 | Wording «cascade-fail» in v1.4 §8.7 line N reads: "<verbatim>"; ROADMAP M5.2 interpretation as «accumulate without skip» — compatible/incompatible. Tier 2 confirmed. |
| 2 | M7 §9.2/§9.3 run-flag location | §9.2, §9.3 | (verification) |
| 3 | M7 §9.5/§9.5.1 step 7 ordering | §9.5, §9.5.1 | (verification) |
| 4 | M3.4 deferred | §11.1 M3.4 | wording: «deferred», compatible with v1.4 §11.1 — Tier 2 confirmed |
| 5 | Phase 3 SocialSystem/SkillSystem Replaceable=false | (no spec section; ROADMAP M10.C) | code state verified | Tier 2 confirmed |

### Tier 3 — Spec ↔ code minor mismatch

[table]

### Tier 4 — Cosmetic

[table]

### Out-of-scope items observed (for Pass 3/4)

| # | Anomaly | Source | Routing |
|---|---|---|---|
| 1 | Pass 1 anomaly #1 (test count 359 vs ROADMAP 369) | `tests/`, `ROADMAP.md:36` | Pass 3 |
| 2 | Pass 1 anomaly #2 (closure review locations — pre-Pass-1 working tree state) | `docs/audit/`, git status | Resolved before Pass 2 (per human note); Pass 4 verifies clean state |
| 3 | Pass 1 anomaly #6 (Modding.Tests README stale) | `tests/DualFrontier.Modding.Tests/README.md` | Pass 4 |
| 4 | Pass 1 anomaly #7 (Systems.Tests README stale) | `tests/DualFrontier.Systems.Tests/README.md` | Pass 4 |
| 5 | Pass 1 anomaly #8 (bus count cross-doc) | `src/DualFrontier.Contracts/README.md:17` | Pass 4 |
| 6 | Pass 1 anomaly #12 (closure review branch references) | `M5/M6_CLOSURE_REVIEW.md:9` | Pass 3 |

---

## §14 Verification end-state

- **§0 Executive summary:** N/11 PASSED.
- **Total findings:** Tier 0: N, Tier 1: N, Tier 2: N, Tier 3: N, Tier 4: N.
- **Eager escalation triggered:** YES / NO.
- **Surgical fixes applied:** 0 (per contract).
- **Pass 2 status:** complete / INCOMPLETE (eager-escalated), ready for human ratification.
```

---

## 10. Методика — пошаговый порядок работы

Выполняй в следующем порядке. Не отклоняйся.

### Шаг 1. Прочитать контракт-вход (batch=5)

```
[
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_CAMPAIGN_PLAN.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_1_INVENTORY.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MOD_OS_ARCHITECTURE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\ROADMAP.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\M6_CLOSURE_REVIEW.md"
]
```

Verify все contract assertions из §2.

### Шаг 2. Sequence integrity check (priority — может eager escalate)

Это **первый функциональный шаг**, потому что любой Tier 0 здесь останавливает pass.

Прочитай `AUDIT_PASS_1_INVENTORY.md` §9 sequence catalogue — 53 entries. Открой `MOD_OS_ARCHITECTURE.md` для context'а каждой sequence.

Для каждой sequence:
1. Re-read source location в спеке/коде/roadmap (используй `read_text_file` с `view_range` для targeted reads).
2. Применить чек-лист §7.1 этого промта.
3. Записать verdict в §11 артефакта (постепенно).

**Для anomalies #3, #4, #5 из Pass 1 §11** — особое внимание (§7.2 этого промта).

Если по итогам sequence integrity check обнаружены Tier 0 finding(s) → §8.3 collective escalation discipline → запиши §0, §11, §13 → escalate.

### Шаг 3. §1 Mod topology

Прочитай:
- `src/DualFrontier.Contracts/Modding/ModManifest.cs` (полный)
- `src/DualFrontier.Application/Modding/ManifestParser.cs` (полный)
- ~3–5 fixture manifests из Pass 1 §6 inventory

Заполни §1 артефакта таблицей spec ↔ code.

### Шаг 4. §2 Manifest schema

Используй `read_multiple_files` batch для:
- Spec §2 (re-read через view_range)
- `ModManifest.cs`
- `ManifestParser.cs`
- 5–7 representative fixture manifests
- `ValidationError.cs` (для error kinds emitted)

Заполни §2 артефакта таблицей по 13 полям.

Pass 1 anomaly #11 (`description` field) — классифицируй здесь.

### Шаг 5. §3 Capability model

Прочитай:
- `src/DualFrontier.Contracts/Attributes/ModAccessibleAttribute.cs`
- `src/DualFrontier.Contracts/Attributes/ModCapabilitiesAttribute.cs`
- `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs`
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs` (для EnforceCapability)
- `ContractValidator.cs` (Phase C, D)
- 2-3 representative components с `[ModAccessible]` (e.g. `WeaponComponent.cs`, `HealthComponent.cs`)

Заполни §3.

### Шаг 6. §4 IModApi v2

Прочитай:
- `src/DualFrontier.Contracts/Modding/IModApi.cs`
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs`
- 1-2 test classes из `tests/DualFrontier.Modding.Tests/Api/`

Заполни §4 таблицей по 9 методам.

### Шаг 7. §5 Type sharing

Прочитай:
- `src/DualFrontier.Application/Modding/SharedModLoadContext.cs`
- `src/DualFrontier.Application/Modding/ModLoader.cs` (для two-pass)
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` (TopoSortSharedMods)
- `ContractValidator.cs` (Phase E, F)

Заполни §5.

### Шаг 8. §6 Three contract levels

Прочитай:
- `src/DualFrontier.Contracts/Core/IModContract.cs`
- `src/DualFrontier.Contracts/Core/IEvent.cs`
- `src/DualFrontier.Contracts/Attributes/DeferredAttribute.cs`
- `src/DualFrontier.Contracts/Attributes/ImmediateAttribute.cs`

Заполни §6.

### Шаг 9. §7 Bridge replacement

Прочитай:
- `src/DualFrontier.Contracts/Attributes/BridgeImplementationAttribute.cs`
- `ContractValidator.cs` (Phase H)
- `ModIntegrationPipeline.cs` (CollectReplacedFqns + skip logic)
- 2-3 Phase 5 bridge stubs (e.g. `src/DualFrontier.Systems/Combat/CombatSystem.cs`)
- `src/DualFrontier.Systems/Pawn/SocialSystem.cs` (для Replaceable=false guard)

Заполни §7.

### Шаг 10. §8 Versioning

Прочитай:
- `src/DualFrontier.Contracts/Modding/VersionConstraint.cs`
- `src/DualFrontier.Contracts/Modding/ContractsVersion.cs`
- `ContractValidator.cs` (Phase A v1/v2 dual-path, Phase G)
- 1-2 fixture manifests с caret syntax

Заполни §8.

Verify ratified interpretation M5.2 cascade-failure → Tier 2 confirm.

### Шаг 11. §9 Lifecycle and hot reload

Прочитай:
- Spec §9 целиком (re-read)
- `ModIntegrationPipeline.cs` (Pause/Resume/Apply/UnloadMod)
- `M71PauseResumeTests.cs`, `M72UnloadChainTests.cs`, `M73Step7Tests.cs`, `M73Phase2DebtTests.cs` (по necessity)

Verify ratified interpretations:
- M7 §9.2/§9.3 run-flag → Tier 2 confirm (verify v1.4 wording compat)
- M7 §9.5/§9.5.1 step 7 ordering → Tier 2 confirm (verify v1.4 wording compat)

Verify Pass 1 anomaly #3 (lifecycle 6 vs 5) — финализируй verdict в §11 артефакта.

Заполни §9.

### Шаг 12. §10 Threat model

Прочитай:
- Spec §10 целиком
- `tests/DualFrontier.Core.Tests/Isolation/` (для §10.1 isolation threats)
- `tests/DualFrontier.Modding.Tests/` (для §10.4 test categories mapping)

Заполни §10.

### Шаг 13. §13 Items requiring follow-up

Группируй все findings из §1–§11 по Tier. Запиши Tier 2 verifications для всех 5 whitelist entries (см. plan §5).

### Шаг 14. §0 Executive summary

Финализируй таблицу проверок — каждая строка PASSED / FAILED + tier counts.

### Шаг 15. Self-check (§13 этого промта)

Перед записью артефакта пройди self-check.

### Шаг 16. Записать артефакт

```
docs/audit/AUDIT_PASS_2_SPEC_CODE.md
```

### Шаг 17. Stop

После записи — стоп. Не предлагай Pass 3. Не commit.

---

## 11. Запрещено в Pass 2

- **Cross-doc consistency** (spec ↔ `MODDING.md`, `MOD_PIPELINE.md`, etc.). Это Pass 4.
- **Roadmap acceptance verification** (M-row claims vs тесты). Это Pass 3.
- **README accuracy в подпапках.** Это Pass 4.
- **Cyrillic remainder.** Это Pass 4.
- **Performance / threat model active testing.** Out of scope.
- **Любые правки файлов проекта** вне `docs/audit/AUDIT_PASS_2_SPEC_CODE.md`.
- **Commit'ы.** Pass 2 не делает git commits.
- **Импровизация формата артефакта.** 14-секционная структура зафиксирована в §9 этого промта.
- **Ошибочное флагание whitelist (§5).** Перед классификацией Tier 0/1 — всегда проверь whitelist. Если совпало — Tier 2.
- **Преждевременная остановка для не-Tier-0 finding'ов.** Только Tier 0 триггерит eager escalation.

---

## 12. Discipline

### 12.1 Batch reads (per кампания §7.5 LOCKED)

Используй `Filesystem:read_multiple_files` с batch≥5. Группируй чтения по семантически связанным группам (e.g. все attribute файлы вместе, все validator phase tests вместе).

### 12.2 Targeted reads через view_range

Спека `MOD_OS_ARCHITECTURE.md` — 1000+ строк. Используй `read_text_file` с `view_range` для targeted re-reads секций по необходимости. Не загружай всю спеку каждый раз.

### 12.3 Source attribution

Каждое утверждение в артефакте Pass 2 имеет источник: `spec_section` (e.g. `§9.5 line 760`) + `code_path:line` (e.g. `ModIntegrationPipeline.cs:300`). Без source attribution — Tier 4 cosmetic от self-check.

### 12.4 Verbatim quotes

Spec wording для критичных заявлений (count claims, exhaustive lists) — копируй verbatim в backticks. Парафраз в Pass 2 — потенциальная причина false-positive Tier 0.

### 12.5 Whitelist check first

Перед классификацией любого finding'а как Tier 0 или Tier 1 — обязательная проверка против §5 whitelist. Если matches → Tier 2 + verification entry в §13.

---

## 13. Self-check перед submission

Перед `write_file` пройди этот чек-лист и явно подтверди каждый пункт:

- [ ] Все 14 секций (§0..§14) присутствуют в артефакте.
- [ ] §0 Executive summary таблица заполнена 11 строками.
- [ ] Каждое finding имеет 5 обязательных полей: `(spec_section, code_path or test_path, tier, description, recommendation)`.
- [ ] §11 sequence integrity покрывает все 53 sequences из Pass 1 §9.
- [ ] Anomalies #3, #4, #5 из Pass 1 §11 имеют явный verdict в §11 артефакта.
- [ ] Whitelist (§5 этого промта) — каждая из 5 entries имеет verification row в §13 Tier 2.
- [ ] Если Tier 0 обнаружен — §0 содержит INCOMPLETE block; §11/§13 содержат full diagnostic.
- [ ] §12 = 0 (per contract).
- [ ] Out-of-scope items routed correctly: Pass 1 anomaly #1, #6, #7, #8 → Pass 4 routing; #2, #12 → Pass 3 routing.
- [ ] Артефакт на английском.
- [ ] Структура mirroring `M6_CLOSURE_REVIEW.md`.

Если хотя бы один пункт `[ ]` — ты ещё не готов. Вернись к шагу методики где этот пункт должен был быть закрыт.

---

## 14. Stop condition и эскалация

### 14.1 Normal stop

После записи артефакта и self-check — короткое сообщение человеку:

«AUDIT_PASS_2_SPEC_CODE.md записан. N/11 проверок PASSED. Tier counts: 0=A, 1=B, 2=C, 3=D, 4=E. Eager escalation: NO. M items routed to Pass 3/4. Жду human ratification.»

### 14.2 Eager Tier 0 escalation

Per §8 этого промта.

Сообщение человеку:

«Pass 2 заблокирован Tier 0 finding: <spec_section> — <brief description>. Артефакт записан как INCOMPLETE в `docs/audit/AUDIT_PASS_2_SPEC_CODE.md`. Subsequent passes (3, 4, 5) cannot proceed until human resolves.»

### 14.3 Infrastructure failure

Если контракт-вход нарушен (план не v1.0, Pass 1 inventory не PASSED, спека не v1.4):

«Pass 2 заблокирован: <причина>. Не запускаю classification. Требуется human action.»

---

## 15. Финальная заметка

### 15.1 Pre-priority — anomalies из Pass 1

Pass 1 §11 содержит 12 anomalies. Routing для Pass 2:

| Pass 1 anomaly # | Pass 2 scope? | Pre-classification hypothesis |
|---|---|---|
| 1 — test count 359 vs 369 | NO (Pass 3) | — |
| 2 — closure review locations | NO (resolved before Pass 2; Pass 4 verifies) | — |
| 3 — lifecycle 6 vs 5 | **YES — investigate sequence integrity** | Tier 0 candidate (count_mismatch) или Tier 3 (wording clarity) |
| 4 — validator phases declared vs invoked | **YES — investigate sequence integrity** | Tier 0 candidate (order) или Tier 2 (intentional design) |
| 5 — ValidationErrorKind 11 vs ~10 | **YES — investigate sequence integrity** | Tier 0 candidate (count_mismatch) или Tier 3 (wording incompleteness) |
| 6 — Modding.Tests README stale | NO (Pass 4) | — |
| 7 — Systems.Tests README stale | NO (Pass 4) | — |
| 8 — bus count в Contracts/README.md | NO (Pass 4) | — |
| 9 — мой промт упомянул `docs/M*` | NO (cosmetic, my error) | — |
| 10 — мой промт example branch | NO (cosmetic, my error) | — |
| 11 — manifest.json `description` field | **YES — §2 Manifest schema** | Tier 1 если field required и отсутствует / Tier 3 если parser ignores |
| 12 — closure review branch refs | NO (Pass 3) | — |

### 15.2 Дисциплина

Pass 2 — критичный pass. Любая ошибка в нём (false positive Tier 0, missed sequence integrity violation, неправильное флагание whitelist) отравит Pass 5 final report.

Дисциплина Pass 2: **whitelist first, sequence integrity priority, eager Tier 0 escalation, classification with full attribution, no cross-doc / no roadmap / no README scope creep**.

Удачи.

---

## Appendix A: пример Tier 2 verification entry в §13

```markdown
### Tier 2 — Whitelist deviations confirmed compatible with v1.4

| # | Whitelist entry | Spec section | Verification |
|---|---|---|---|
| 1 | M5.2 cascade-failure accumulation | §8.7 (lines 678–684) | v1.4 §8.7 wording verbatim: «<exact quote from spec>». ROADMAP M5.2 interpretation as «accumulate without skip» (per `ROADMAP.md:226–254`). The wording «cascade-fail» in spec is non-prescriptive about skip-vs-accumulate; ROADMAP interpretation expands compatibly. **Tier 2 confirmed.** |
```

## Appendix B: пример Tier 0 finding в §13

```markdown
### Tier 0 — Spec drift (BLOCKING)

| # | Spec section | Code path | Description | Recommendation |
|---|---|---|---|---|
| 1 | §9.1 line 692 | `MOD_OS_ARCHITECTURE.md:692, 696–724` | Spec text says «six well-defined states» (line 692); diagram (lines 696–724) contains 6 boxes with state names. Unique state names: 5 (Disabled appears at boxes both initial and terminal — same enum value, different positions in flow). | Either (a) ratify v1.5 with wording «six lifecycle positions, five distinct states» and update §9.1 diagram caption; (b) add a sixth distinct state (e.g. «Failed» or «Terminated») if the design intent is six states. Pass 2 cannot resolve unilaterally — escalation required. |
```

## Appendix C: формат INCOMPLETE block при eager escalation

В верху артефакта (после frontmatter, перед `# Audit Pass 2 — Spec ↔ Code Drift`):

```markdown
> **STATUS: INCOMPLETE — TIER 0 ESCALATION**
>
> **Triggered at:** §11 Sequence integrity check, sequence #17 (§9.1 lifecycle states).
>
> **Pass 2 stopped at step 11 of methodology (§9 Lifecycle).**
>
> **Subsequent passes (3, 4, 5) cannot proceed until human resolves this Tier 0 finding.**
>
> **See §13 Tier 0 — Spec drift (BLOCKING) for full diagnostic.**
```

---

**Конец промта Pass 2.**

---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_2_RESUMPTION_PROMPT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_2_RESUMPTION_PROMPT
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_2_RESUMPTION_PROMPT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_2_RESUMPTION_PROMPT
---
---
title: Audit Pass 2 Resumption Prompt — §1–§10 Mapping + Whitelist Verification
nav_order: 106
---

# AUDIT PASS 2 — RESUMPTION

**Промт для standalone Opus-сессии. Версия v1.0, выпущена 2026-05-01.**

*Этот документ копируется в новый чат с Opus как первое и единственное входное сообщение. Сессия Opus не имеет доступа к предыдущим разговорам или контексту вне этого промта и filesystem проекта. Это **resumption** прерванного Pass 2, а не initial pass — sequence integrity sub-pass уже выполнен в предыдущей сессии и зафиксирован в существующем артефакте.*

---

## 1. Кто ты и какова роль

Ты — Opus, архитектурный аудитор проекта **Dual Frontier**. Это standalone simulation engine на Godot 4 + C# с custom ECS, OS-style mod system и многоагентным LLM pipeline разработки.

Проект расположен на `D:\Colony_Simulator\Colony_Simulator`. У тебя есть read-only access к этому пути через `Filesystem` MCP tools. **Один файл — `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` — ты будешь обновлять** (это часть resumption discipline). Других правок проекта не делаешь.

---

## 2. Что произошло до этой сессии (context)

В предыдущей Opus-сессии был запущен Pass 2 audit (initial) по `AUDIT_PASS_2_PROMPT.md`. Pass 2 эскалирован на Tier 0 finding в sequence integrity sub-pass:

- **Tier 0 finding:** `MOD_OS_ARCHITECTURE.md` §11.2 enumeration vs `ValidationError.cs:9–83`. Spec wording «The current enum has `MissingDependency` and `CyclicDependency`» implied 9 validation members; код имеет 11 (delta: `IncompatibleContractsVersion`, `WriteWriteConflict`).
- **Sequence integrity sub-pass завершён** — все 53 sequences проверены. Финдинги (1 Tier 0 + 2 Tier 3 + 1 Tier 4) зафиксированы в `AUDIT_PASS_2_SPEC_CODE.md` §11.
- **§1–§10 spec ↔ code mapping НЕ ВЫПОЛНЕН** — placeholders «NOT EXECUTED» в существующем артефакте.
- **Tier 2 whitelist verifications НЕ ВЫПОЛНЕНЫ** — 5 entries помечены как «NOT VERIFIED» в существующем §13.

Между предыдущей сессией и этой человек ратифицировал Tier 0 finding через **v1.5 amendment** к спеке (option (a) per §13 Tier 0 recommendation). Текущая спека:

- **Status:** `LOCKED v1.5` (line 8).
- **§11.2 line 854 (новый wording):** `«The current enum has IncompatibleContractsVersion, WriteWriteConflict, MissingDependency, and CyclicDependency. The migration adds:»`
- **v1.5 changelog entry** (lines ~30–32) с full diagnostic от Pass 2 audit и кредитованием.

Spec drift, который триггернул escalation, теперь устранён. Tier 0 finding **resolved**. Pass 2 разблокирован для resumption.

---

## 3. Контракт кампании — обязательное первое действие

**ОБЯЗАТЕЛЬНОЕ ПЕРВОЕ ДЕЙСТВИЕ перед началом любой работы:** прочитай batch'ом следующие документы:

```
[
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_CAMPAIGN_PLAN.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_1_INVENTORY.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_2_SPEC_CODE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MOD_OS_ARCHITECTURE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\ROADMAP.md"
]
```

После прочтения должно быть истинным:

- **`AUDIT_CAMPAIGN_PLAN.md`** имеет статус `LOCKED v1.0`.
- **`AUDIT_PASS_1_INVENTORY.md`** заявляет 9/9 PASSED.
- **`AUDIT_PASS_2_SPEC_CODE.md`** имеет STATUS INCOMPLETE banner в верху + §11 содержит 53 sequence verdicts + §13 Tier 0 содержит resolved finding.
- **`MOD_OS_ARCHITECTURE.md`** имеет статус `LOCKED v1.5` (line 8) и v1.5 changelog entry присутствует.
- **§11.2 line 854** содержит wording: `«The current enum has IncompatibleContractsVersion, WriteWriteConflict, MissingDependency, and CyclicDependency. The migration adds:»`.

Если хотя бы один из этих контрактов нарушен — **немедленно остановись** и сообщи человеку: «Pass 2 resumption заблокирован: <причина>». Не пытайся импровизировать.

---

## 4. Цель Pass 2 Resumption

Завершить `AUDIT_PASS_2_SPEC_CODE.md` через:

1. **§1–§10 spec ↔ code mapping.** Каждое нормативное утверждение в `MOD_OS_ARCHITECTURE.md` v1.5 §1–§10 → `(spec_section, code_path or test_path, tier, description, recommendation)`.
2. **Tier 2 whitelist verifications.** 5 entries из `AUDIT_PASS_2_PROMPT.md` §5 — каждая получает verbatim spec quote + verification verdict.
3. **Pass 1 anomaly #11 классификация.** `mods/DualFrontier.Mod.Example/mod.manifest.json` имеет поле `description` не описанное в spec §2.2 — классифицировать в §2 mapping.
4. **Обновление существующих секций** артефакта:
   - §0 Executive summary — финальные tier counts, пометка PASSED/FAILED для §1–§10.
   - §13 Tier 0 — статус «RESOLVED — ratified as v1.5 amendment». Diagnostic сохранить как audit trail.
   - §14 Verification end-state — финальный summary.
   - Удалить INCOMPLETE banner из верха артефакта.

**Pass 2 Resumption НЕ делает:**

- Sequence integrity check (53 sequences) — уже выполнен в существующем §11.
- Cross-doc consistency — Pass 4.
- Roadmap acceptance verification — Pass 3.
- Любые правки кода или спеки.

---

## 5. Контракт-вход

| Что | Источник |
|---|---|
| Repo root | `D:\Colony_Simulator\Colony_Simulator\` |
| Spec | `docs/architecture/MOD_OS_ARCHITECTURE.md` **v1.5 LOCKED** |
| Pass 1 inventory | `docs/audit/AUDIT_PASS_1_INVENTORY.md` (locked) |
| Existing Pass 2 artifact | `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (to be updated in place) |
| Campaign plan | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` v1.0 LOCKED |
| Initial Pass 2 prompt (reference for whitelist + tier defs) | `docs/audit/AUDIT_PASS_2_PROMPT.md` |
| Code | `src/DualFrontier.Contracts/`, `src/DualFrontier.Application/Modding/`, `src/DualFrontier.Core/`, `src/DualFrontier.Components/`, `src/DualFrontier.Events/`, `src/DualFrontier.Systems/` |
| Tests | `tests/DualFrontier.*.Tests/` (по necessity) |
| Tools | `Filesystem` MCP tools only |

---

## 6. Whitelist ratified deviations (НЕ флагать как drift)

**Inline-копия из initial Pass 2 prompt §5.** Эта копия — самодостаточная; Opus не должен переключаться на initial prompt чтобы их найти.

### 6.1 Changelog ratifications

- **v1.1** — corrections от M1–M3.1 audit (Log → ModLogLevel, dependency.optional). Спека уже зафиксировала.
- **v1.2** — corrections от M3 closure review (§3.6 hybrid enforcement, §3.5+§2.1 example manifest, M3.4 deferred).
- **v1.3** — correction от M4.3 implementation review (§2.2 entryAssembly/entryType wording «must be empty»).
- **v1.4** — clarifications от M7 pre-flight readiness review (§9.5 step 7 GC pump bracket, §9.5.1 failure semantics).
- **v1.5** — correction от Audit Campaign Pass 2 (§11.2 baseline enumeration expanded to include `IncompatibleContractsVersion`, `WriteWriteConflict`).

### 6.2 Deliberate interpretations в ROADMAP

Все три — **Tier 2 по умолчанию**. Verify wording v1.5 совместим с интерпретацией:

- **M5.2 cascade-failure accumulation.** ROADMAP M5.2 интерпретирует §8.7 wording «cascade-fail» как «accumulate without skip».
- **M7 §9.2/§9.3 run-flag location.** ROADMAP M7 интерпретирует §9.2/§9.3 как «flag on `ModIntegrationPipeline`, not in scheduler».
- **M7 §9.5/§9.5.1 step 7 ordering.** ROADMAP M7 интерпретирует step 7 как `WR capture → active-set remove → spin`.

### 6.3 Carried debts forward

- **Phase 2 WeakReference unload tests** → закрыто в M7.3 (`M73Phase2DebtTests`). Не флагать как outstanding.
- **Phase 3 `SocialSystem`/`SkillSystem` Replaceable=false** → carried до M10.C. Tier 2.
- **M3.4 CI Roslyn analyzer** → carried до first external mod author. Tier 2.
- **§7.5 fifth scenario** (mod-unloaded replacement-revert) → закрыто структурно через M7 hot-reload. Не флагать.

### 6.4 OS mapping table snapshot

`MOD_OS_ARCHITECTURE.md` §0 OS mapping таблица — снимок на момент Phase 0 LOCKED. Не флагать как stale, кроме случаев когда v1.x changelog явно обновил конкретную строку.

---

## 7. Тиерация — те же правила, что в initial Pass 2

| Tier | Определение | Эскалация |
|---|---|---|
| **Tier 0** | Spec drift против LOCKED **v1.5**. Код или wording спеки противоречат друг другу. | **Eager — текущий pass останавливается, артефакт фиксирует partial state** |
| **Tier 1** | Missing required implementation. Спека требует артефакта; артефакта нет. | Накапливается до конца pass'а |
| **Tier 2** | Ratified deviation (см. §6 whitelist) + verification: «whitelist confirmed compatible with v1.5 wording». | Отдельная группа в §13 |
| **Tier 3** | Spec ↔ code минорный mismatch не блокирующий поведение. | Накапливается |
| **Tier 4** | Cosmetic. | Накапливается |

**Pass 2 Resumption не флагит cross-doc или README issues** — это Pass 4 territory. Если в процессе работы замечена такая аномалия — записать в §13 «Out-of-scope items» без классификации.

---

## 8. Eager Tier 0 escalation в resumption

Eager escalation всё ещё активна. Если в процессе §1–§10 mapping обнаружится новый Tier 0 finding (помимо resolved-через-v1.5) — действия:

1. **Сразу обнови артефакт** в `docs/audit/AUDIT_PASS_2_SPEC_CODE.md`:
   - Верни STATUS INCOMPLETE banner в верху (но с обновлённым «Triggered at» и текущим step).
   - Текущие §1–§<N> заполнены, §<N+1>–§10 — placeholders «NOT EXECUTED — eager-escalated».
   - В §13 Tier 0 — добавь второй row под resolved (v1.5) row с полной диагностикой нового finding'а.
2. **Сообщи человеку:** «Pass 2 resumption заблокирован новым Tier 0 finding: <spec_section> — <brief>. Артефакт обновлён, требуется решение человека.»
3. **Стоп.**

**Whitelist check first.** Перед классификацией Tier 0 — обязательная проверка против §6. Если matches → Tier 2 + verification entry в §13.

---

## 9. Контракт-выход — обновление существующего артефакта

**Файл:** `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (overwrite целиком).

**Язык:** английский.

**Что меняется в каждой секции:**

### 9.1 Frontmatter и header

- Удалить INCOMPLETE banner block (строки между frontmatter и `# Audit Pass 2 — Spec ↔ Code Drift`).
- Header: обновить **Spec under audit:** на `LOCKED v1.5` (вместо `LOCKED v1.4`).
- Scope-параграф: убрать предложение «**Pass eager-escalated on Tier 0 finding during sequence integrity check; §1–§10 spec ↔ code mapping was NOT performed.**» Заменить на: «**Pass 2 resumed after v1.5 ratification of Tier 0 finding; §1–§10 spec ↔ code mapping completed in resumption session.**»

### 9.2 §0 Executive summary

Все 11 строк должны иметь финальный verdict (PASSED / FAILED) и tier counts. Заменить «NOT EXECUTED» на реальные результаты.

Tier breakdown across all checks — финальные числа с агрегацией §11 sub-pass + новые §1–§10 findings.

«Eager escalation triggered: NO» (если resumption завершится нормально).

### 9.3 §1 §1 Mod topology — three mod kinds

Заменить «(NOT EXECUTED — see §1 banner.)» на полную секцию с code mapping table и findings.

Spec sections: §1.1 Regular (lines 95–105), §1.2 Shared (lines 110–122), §1.3 Vanilla (lines 125–135), §1.4 Load graph invariants (lines 152–157).

Code mapping table — см. примеры в `AUDIT_PASS_2_PROMPT.md` §9 (initial prompt). Структура та же.

### 9.4 §2 §2 Manifest schema

Заменить «(NOT EXECUTED ...)» на полную секцию.

**Pass 1 anomaly #11 классификация here.** Spec §2.2 (lines 209–224) field reference table перечисляет 13 полей; `description` не входит. Manifest `mods/DualFrontier.Mod.Example/mod.manifest.json` содержит `description`.

Возможные классификации:
- (a) **Tier 1** — spec exhaustively defines schema; unknown fields rejected by parser? Verify в `ManifestParser.cs`. Если parser действительно отвергает — Tier 1 (manifest missing required schema compliance).
- (b) **Tier 3** — parser silently ignores `description`; field is informational? Verify. Если so — Tier 3 (schema definition incompleteness, cosmetic field that doesn't break anything).
- (c) **Tier 4** — `description` is informally accepted convention worth documenting? Tier 4 (cosmetic, recommend adding to spec §2.2 in future v1.x ratification).

Verify в коде, выбери verdict с обоснованием.

Также: example manifest omits `kind` field. По spec §2.2 — `kind` имеет default `"regular"`, omission is permitted. Verify в `ModManifest.cs:82` (default value). Если confirmed — clean (✓), не finding.

### 9.5 §3 §3 Capability model

Заменить «(NOT EXECUTED ...)». Spec sections: §3.1–§3.8 (lines ~280–340).

Code artifacts ожидаемые:
- `[ModAccessible]` — `src/DualFrontier.Contracts/Attributes/ModAccessibleAttribute.cs`
- `[ModCapabilities]` — `src/DualFrontier.Contracts/Attributes/ModCapabilitiesAttribute.cs`
- `KernelCapabilityRegistry` — `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs`
- Phase C (manifest ↔ kernel/dependency) — `src/DualFrontier.Application/Modding/ContractValidator.cs`
- Phase D (`[SystemAccess]` cross-check) — same file
- Runtime `EnforceCapability` — `RestrictedModApi.cs`
- Production component annotations — `src/DualFrontier.Components/.../*.cs`

### 9.6 §4 §4 IModApi v2

Заменить «(NOT EXECUTED ...)». Spec sections: §4.1–§4.5 (lines ~340–410).

Spec §4.1 surface: 9 methods (already verified by Pass 1 §5 + sequence integrity sub-pass §40).

Code artifacts:
- Interface declaration — `src/DualFrontier.Contracts/Modding/IModApi.cs`
- Implementation — `src/DualFrontier.Application/Modding/RestrictedModApi.cs`
- Capability checks at Publish/Subscribe — same file
- Cast-prevention rule (D-3 structural) — verify `RestrictedModApi` is `internal sealed`

### 9.7 §5 §5 Type sharing across ALCs

Заменить «(NOT EXECUTED ...)». Spec sections: §5.1–§5.5 (lines ~410–530).

Code artifacts:
- `SharedModLoadContext` — `src/DualFrontier.Application/Modding/SharedModLoadContext.cs`
- Two-pass loader — `ModIntegrationPipeline.cs` (TopoSortSharedMods)
- Phase E (D-4) — `ContractValidator.cs`
- Phase F (§5.2 compliance) — `ContractValidator.cs`

### 9.8 §6 §6 Three contract levels

Заменить «(NOT EXECUTED ...)». Spec sections: §6.1–§6.5 (lines ~530–600).

Code artifacts:
- `IModContract` — `src/DualFrontier.Contracts/Core/IModContract.cs`
- `IEvent` — `src/DualFrontier.Contracts/Core/IEvent.cs`
- `[Deferred]` — `src/DualFrontier.Contracts/Attributes/DeferredAttribute.cs`
- `[Immediate]` — `src/DualFrontier.Contracts/Attributes/ImmediateAttribute.cs`

### 9.9 §7 §7 Bridge replacement

Заменить «(NOT EXECUTED ...)». Spec sections: §7.1–§7.5 (lines ~530–610).

Code artifacts:
- `[BridgeImplementation(Replaceable=...)]` — `src/DualFrontier.Contracts/Attributes/BridgeImplementationAttribute.cs`
- Phase H validator — `ContractValidator.cs`
- Pipeline skip logic — `ModIntegrationPipeline.cs` (CollectReplacedFqns)
- Bridge stubs — `src/DualFrontier.Systems/Combat/CombatSystem.cs` etc.
- Replaceable=false guards — `SocialSystem.cs`, `SkillSystem.cs` (per Tier 2 carried debt)

§7.5 acceptance scenarios — 5 bullets — verify каждый имеет соответствующий test.

### 9.10 §8 §8 Versioning

Заменить «(NOT EXECUTED ...)». Spec sections: §8.1–§8.7 (lines ~620–690).

Verify ratified interpretation **M5.2 cascade-failure accumulation** (§8.7 line 678+) → Tier 2 confirm:
- Прочитай §8.7 wording verbatim.
- Прочитай ROADMAP M5.2 (lines 226–254) — interpretation документирована.
- Verify wording «cascade-fail» в спеке non-prescriptive about skip-vs-accumulate; ROADMAP interpretation expands compatibly.
- Если совместимо — Tier 2 confirmed, entry в §13 Tier 2.
- Если **не** совместимо — Tier 0 escalation per §8 этого промта.

Code artifacts:
- `VersionConstraint` — `src/DualFrontier.Contracts/Modding/VersionConstraint.cs`
- `ContractsVersion` — `src/DualFrontier.Contracts/Modding/ContractsVersion.cs`
- Phase A (v1/v2 dual-path) — `ContractValidator.cs`
- Phase G (inter-mod check) — `ContractValidator.cs`

### 9.11 §9 §9 Lifecycle and hot reload

Заменить «(NOT EXECUTED ...)». Spec sections: §9.1–§9.6 (lines ~690–780).

Verify ratified interpretations:
- **M7 §9.2/§9.3 run-flag location.** Verify v1.5 wording line 730+ совместима с «flag on `ModIntegrationPipeline`».
- **M7 §9.5/§9.5.1 step 7 ordering.** Verify v1.5 wording (with v1.4 GC pump bracket) line 757+ совместима с ROADMAP M7 ordering interpretation.

Pass 1 anomaly #3 (lifecycle 6 vs 5 unique) уже classified Tier 3 в §11 sub-pass — НЕ повторно классифицировать в §9 mapping; ссылка на §11 entry 17 достаточна.

Code artifacts:
- `ModIntegrationPipeline.cs` (Pause/Resume/Apply/UnloadMod)
- `M71PauseResumeTests.cs`, `M72UnloadChainTests.cs`, `M73Step7Tests.cs`, `M73Phase2DebtTests.cs`

### 9.12 §10 §10 Threat model

Заменить «(NOT EXECUTED ...)». Spec sections: §10.1–§10.4 (lines ~780–830).

Verify §10.4 required test categories (7 bullets) — каждая категория имеет соответствующий test class в `tests/DualFrontier.Modding.Tests/`. Например:
- Isolation tests → `IsolationGuardTests` (in Core.Tests; verify cross-reference)
- Capability violation → `CapabilityValidationTests`
- Bridge replacement → Phase H tests + M62 integration
- Type-sharing → `Sharing/` test folder
- WeakReference unload → `M73Step7Tests.cs`, `M73Phase2DebtTests.cs`
- Cross-mod cycle → cycle test (verify exists)
- Version constraint → `Manifest/VersionConstraintTests.cs`

Если категория **не имеет** соответствующего теста — Tier 1 finding.

### 9.13 §11 Sequence integrity findings

**НЕ ИЗМЕНЯТЬ.** Sub-pass завершён в предыдущей сессии. Все 53 verdicts корректны и зафиксированы.

Опционально: добавить в footer §11 примечание «Sequence integrity sub-pass completed in initial Pass 2 session; no changes in resumption.» для clarity audit trail.

### 9.14 §12 Surgical fixes applied this pass

Остаётся «None. Pass 2 is read-only by contract.»

### 9.15 §13 Items requiring follow-up

**Существенное обновление:**

#### Tier 0 — обновить статус

Существующий entry на ValidationErrorKind §11.2 — обновить:
- В колонку «Description»: добавить в конец `**STATUS: RESOLVED — ratified as v1.5 amendment to MOD_OS_ARCHITECTURE.md (commit pending; see Version history line ~30).**`
- В колонку «Recommendation»: добавить в конец `**Resolution applied:** option (a) per recommendation. v1.5 spec amendment expanded §11.2 baseline enumeration. Pass 2 resumed.`
- Diagnostic body сохранить verbatim для audit trail.

#### Tier 1 — заполнить (если есть finding'и)

Из §1–§10 mapping. Если ничего не найдено — `(no Tier 1 findings)`.

#### Tier 2 — заполнить 5 whitelist verifications

Каждая entry формата:
```markdown
| 1 | M5.2 cascade-failure accumulation | §8.7 (lines 678–684) | v1.5 §8.7 wording verbatim: «<exact quote>». ROADMAP M5.2 (lines 226–254) interpretation: «accumulate without skip». Compatibility analysis: <one-paragraph reasoning>. **Tier 2 confirmed.** |
```

Plus новые Tier 2 finding'и из §1–§10 mapping (если возникнут — например, если какой-то finding в §3 матчит whitelist).

#### Tier 3 — добавить из §1–§10 mapping

Существующие 2 entries из §11 sub-pass (sequence #14 syntaxes, sequence #17 lifecycle states) — сохранить.

#### Tier 4 — добавить из §1–§10 mapping

Существующая 1 entry из §11 sub-pass (sequence #37/#38 validator phases prose order) — сохранить.

#### Out-of-scope items

Существующая таблица (9 items) — сохранить. Add Pass 1 anomaly #11 если она классифицируется в §2 как **актуальное finding** (не out-of-scope) — переместить из «Out-of-scope items» row 8 в соответствующий Tier section.

### 9.16 §14 Verification end-state

Полностью переписать:

```markdown
## §14 Verification end-state

- **§0 Executive summary:** N/11 PASSED (where N is final count).
- **Total findings:** Tier 0: 1 (RESOLVED via v1.5 amendment), Tier 1: N, Tier 2: N (5 whitelist confirmed + any new from §1–§10), Tier 3: N (2 from §11 sub-pass + any new), Tier 4: N (1 from §11 sub-pass + any new).
- **Eager escalation triggered:** NO (resumption completed normally).
- **Surgical fixes applied:** 0 (per contract).
- **Pass 2 status:** complete (initial sequence integrity sub-pass + resumption §1–§10 mapping + whitelist verifications), ready for human ratification.
- **Tier 0 resolution audit trail:** v1.5 ratification of `MOD_OS_ARCHITECTURE.md` §11.2 baseline enumeration. See §13 Tier 0 row 1 «STATUS: RESOLVED».
- **Unblocks:** Pass 3 (Roadmap ↔ Reality), Pass 4 (Cross-doc + README + Cyrillic), Pass 5 (Triage + Final report).
```

---

## 10. Методика — пошаговый порядок работы (resumption)

### Шаг 1. Прочитать контракт-вход (batch=5)

См. §3 этого промта. Verify все assertion'ы.

### Шаг 2. Re-read existing artifact §11 + §13

Это контекст для §1–§10 mapping. §11 содержит sequence verdicts, на которые могут ссылаться §1–§10 finding'и (e.g. §9 lifecycle ссылается на §11 entry 17). §13 содержит resolved Tier 0 + 2 Tier 3 + 1 Tier 4 findings.

### Шаг 3. §1 Mod topology

Прочитай:
- `src/DualFrontier.Contracts/Modding/ModManifest.cs`
- `src/DualFrontier.Application/Modding/ManifestParser.cs`
- 3-5 fixture manifests (выбери regular + shared mix)

Заполни §1 артефакта. Findings в §13 if any.

### Шаг 4. §2 Manifest schema

Прочитай (batch):
```
[
  "src/DualFrontier.Contracts/Modding/ModManifest.cs",
  "src/DualFrontier.Application/Modding/ManifestParser.cs",
  "mods/DualFrontier.Mod.Example/mod.manifest.json",
  "src/DualFrontier.Application/Modding/ValidationError.cs",
  "tests/Fixture.RegularMod_DependedOn/mod.manifest.json",
  "tests/Fixture.RegularMod_ReplacesCombat/mod.manifest.json"
]
```

Pass 1 anomaly #11 классифицируй here per §9.4 этого промта. Verify default `kind` поведение.

Заполни §2 артефакта.

### Шаг 5. §3 Capability model

Прочитай (batch):
```
[
  "src/DualFrontier.Contracts/Attributes/ModAccessibleAttribute.cs",
  "src/DualFrontier.Contracts/Attributes/ModCapabilitiesAttribute.cs",
  "src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs",
  "src/DualFrontier.Application/Modding/RestrictedModApi.cs",
  "src/DualFrontier.Application/Modding/ContractValidator.cs"
]
```

Заполни §3.

### Шаг 6. §4 IModApi v2

Прочитай:
- `src/DualFrontier.Contracts/Modding/IModApi.cs`
- `src/DualFrontier.Application/Modding/RestrictedModApi.cs` (повторно по необходимости)

Заполни §4.

### Шаг 7. §5 Type sharing

Прочитай:
- `src/DualFrontier.Application/Modding/SharedModLoadContext.cs`
- `src/DualFrontier.Application/Modding/ModLoader.cs`
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` (TopoSort sections)
- `src/DualFrontier.Application/Modding/ContractValidator.cs` (Phase E, F)

Заполни §5.

### Шаг 8. §6 Three contract levels

Прочитай (batch):
```
[
  "src/DualFrontier.Contracts/Core/IModContract.cs",
  "src/DualFrontier.Contracts/Core/IEvent.cs",
  "src/DualFrontier.Contracts/Attributes/DeferredAttribute.cs",
  "src/DualFrontier.Contracts/Attributes/ImmediateAttribute.cs",
  "src/DualFrontier.Contracts/Bus/EventBusAttribute.cs"
]
```

Заполни §6.

### Шаг 9. §7 Bridge replacement

Прочитай:
- `src/DualFrontier.Contracts/Attributes/BridgeImplementationAttribute.cs`
- `src/DualFrontier.Application/Modding/ContractValidator.cs` (Phase H)
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` (CollectReplacedFqns)
- 2 Phase 5 bridge stubs (e.g. `CombatSystem.cs`)
- `SocialSystem.cs` для Replaceable=false guard

Заполни §7. Verify §7.5 5 acceptance scenarios → tests mapping.

### Шаг 10. §8 Versioning

Прочитай:
- `src/DualFrontier.Contracts/Modding/VersionConstraint.cs`
- `src/DualFrontier.Contracts/Modding/ContractsVersion.cs`
- `src/DualFrontier.Application/Modding/ContractValidator.cs` (Phase A v1/v2, Phase G)
- §8.7 спеки (re-read с view_range для verbatim quote)

Verify M5.2 cascade-failure ratification → Tier 2 confirm в §13.

Заполни §8.

### Шаг 11. §9 Lifecycle

Прочитай:
- §9 спеки (целиком re-read)
- `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` (Pause/Resume/Apply/UnloadMod methods)

Verify M7 §9.2/§9.3 run-flag → Tier 2 confirm.
Verify M7 §9.5/§9.5.1 step 7 ordering → Tier 2 confirm.

Заполни §9.

### Шаг 12. §10 Threat model

Прочитай:
- §10 спеки (целиком re-read)
- `tests/DualFrontier.Modding.Tests/` папки (list_directory)
- `tests/DualFrontier.Core.Tests/Isolation/` для §10.1 tests

Verify §10.4 7 test categories coverage. Если категория без теста — Tier 1.

Заполни §10.

### Шаг 13. §13 Tier 2 whitelist verifications

Заполни 5 entries per §6 этого промта. Каждая — verbatim spec quote + compatibility analysis.

Plus consolidated Tier 0/1/2/3/4 entries из §1–§10 mapping.

### Шаг 14. §0 Executive summary update

Финализируй tier counts. Финализируй PASSED/FAILED for каждой строки.

### Шаг 15. §14 Verification end-state — переписать

Per §9.16 этого промта.

### Шаг 16. Header + frontmatter cleanup

Per §9.1 этого промта. Убрать INCOMPLETE banner, обновить spec reference на v1.5, переписать scope.

### Шаг 17. Self-check (§12 этого промта)

### Шаг 18. Записать артефакт

Overwrite `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` целиком.

### Шаг 19. Stop

После записи — короткое сообщение человеку. Не предлагай Pass 3.

---

## 11. Запрещено в Pass 2 Resumption

- **Изменения §11** sequence integrity findings — sub-pass завершён.
- **Cross-doc consistency** (spec ↔ `MODDING.md`, etc.) — Pass 4.
- **Roadmap acceptance verification** — Pass 3.
- **README accuracy в подпапках** — Pass 4.
- **Cyrillic remainder** — Pass 4.
- **Любые правки файлов проекта** вне `docs/audit/AUDIT_PASS_2_SPEC_CODE.md`.
- **Создание нового файла Pass 2 артефакта.** Update existing in place.
- **Импровизация формата.** 14-секционная структура зафиксирована.
- **Ошибочное флагание whitelist (§6).** Перед Tier 0/1 — обязательная проверка.

---

## 12. Self-check перед записью

- [ ] §1–§10 каждая секция содержит mapping table + findings (не «NOT EXECUTED»).
- [ ] §0 Executive summary все 11 строк имеют PASSED/FAILED + tier counts.
- [ ] INCOMPLETE banner в верху файла удалён.
- [ ] Header **Spec under audit:** обновлён на `LOCKED v1.5`.
- [ ] §11 sequence integrity findings не изменены (53 verdicts сохранены).
- [ ] §13 Tier 0 row 1 (ValidationErrorKind §11.2) обновлён со статусом «RESOLVED — ratified as v1.5 amendment».
- [ ] §13 Tier 2 содержит 5 whitelist verifications с verbatim spec quotes.
- [ ] Pass 1 anomaly #11 классифицирована (либо в §2 finding, либо в Out-of-scope).
- [ ] Eager Tier 0 escalation NOT triggered (если resumption завершилась нормально).
- [ ] §14 переписана per §9.16 этого промта.
- [ ] §12 = 0 (per contract).
- [ ] Артефакт на английском.

Если хотя бы один пункт `[ ]` — вернись к шагу методики где он должен был быть закрыт.

---

## 13. Stop condition и эскалация

### 13.1 Normal stop

После записи артефакта и self-check:

«AUDIT_PASS_2_SPEC_CODE.md обновлён в resumption. N/11 проверок PASSED. Tier counts: 0=1 (RESOLVED), 1=A, 2=B (5 whitelist + new), 3=C, 4=D. Eager escalation: NO. Pass 2 complete. Жду human ratification — после этого Pass 3 разблокирован.»

### 13.2 Eager Tier 0 escalation в resumption

Per §8 этого промта.

### 13.3 Infrastructure failure

Если контракт-вход нарушен (план не v1.0, Pass 1 не PASSED, спека не v1.5, существующий артефакт не INCOMPLETE):

«Pass 2 resumption заблокирован: <причина>. Не запускаю update. Требуется human action.»

---

## 14. Финальная заметка

Resumption — деликатный жанр. Sub-pass §11 уже стоял на ratification; ты его не трогаешь. §1–§10 — твоя территория. §13 Tier 0 — обновляешь статус, но diagnostic сохраняешь. Это не перезапись Pass 2, а его честное завершение.

Дисциплина resumption: **whitelist first, eager Tier 0 still active for §1–§10, sequence integrity untouched, audit trail preserved**.

Удачи.

---

**Конец промта Pass 2 Resumption.**

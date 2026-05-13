---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-AUDIT_PASS_4_PROMPT
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-AUDIT_PASS_4_PROMPT
---
---
title: Audit Pass 4 Prompt — Cross-doc & Translation Completeness
nav_order: 108
---

# AUDIT PASS 4 — CROSS-DOC & TRANSLATION COMPLETENESS

**Промт для standalone Opus-сессии. Версия v1.0, выпущена 2026-05-02.**

*Этот документ копируется в новый чат с Opus как первое и единственное входное сообщение. Сессия Opus не имеет доступа к предыдущим разговорам или контексту вне этого промта и filesystem проекта.*

---

## 1. Кто ты и какова роль

Ты — Opus, архитектурный аудитор проекта **Dual Frontier**. Это standalone simulation engine на Godot 4 + C# с custom ECS, OS-style mod system и многоагентным LLM pipeline разработки.

Проект расположен на `D:\Colony_Simulator\Colony_Simulator`. У тебя есть read-only access к этому пути через `Filesystem` MCP tools.

---

## 2. Контракт кампании — обязательное первое действие

Этот pass — **четвёртый из пяти** в audit campaign.

**ОБЯЗАТЕЛЬНОЕ ПЕРВОЕ ДЕЙСТВИЕ:** прочитай batch'ом следующие документы:

```
[
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_CAMPAIGN_PLAN.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_1_INVENTORY.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_2_SPEC_CODE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_3_ROADMAP_REALITY.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MOD_OS_ARCHITECTURE.md"
]
```

После прочтения должно быть истинным:

- **`AUDIT_CAMPAIGN_PLAN.md`** имеет статус `LOCKED v1.0`.
- **`AUDIT_PASS_1_INVENTORY.md`** заявляет 9/9 PASSED.
- **`AUDIT_PASS_2_SPEC_CODE.md`** заявляет 10/11 PASSED + 1/11 FAILED with Tier 0 RESOLVED via v1.5; status `complete`.
- **`AUDIT_PASS_3_ROADMAP_REALITY.md`** заявляет 12/12 PASSED; status `complete`; Pass 1 anomaly #1 reconciled, anomaly #12 Tier 2 historical.
- **`MOD_OS_ARCHITECTURE.md`** имеет статус `LOCKED v1.5` (line 8).

Если хотя бы один контракт нарушен — **немедленно остановись** и сообщи человеку: «Pass 4 заблокирован: <причина>». Не импровизируй.

---

## 3. Цель Pass 4

Произвести **`AUDIT_PASS_4_CROSSDOC_TRANSLATION.md`** — falsifiable артефакт, верифицирующий cross-document consistency, sub-folder README accuracy, stale-reference cleanliness, и translation completeness в active sections проекта.

**Pass 4 делает:**

1. **Cross-doc consistency** — supporting docs (`MODDING.md`, `MOD_PIPELINE.md`, `CONTRACTS.md`, `ECS.md`, `EVENT_BUS.md`, `THREADING.md`, `ISOLATION.md`, `CODING_STANDARDS.md`, `TESTING_STRATEGY.md`, и т. д.) verified против spec v1.5 + reality.
2. **Sub-folder README accuracy** — каждый README.md в `src/` / `tests/` / `mods/` подпапках verified против actual contents folder.
3. **Stale-reference sweep** — каждое упоминание `v1.0`, `v1.1`, `v1.2`, `v1.3`, `v1.4` в active navigation (не в spec changelog или historical artifacts) — должно быть `v1.5` после ratification.
4. **Cyrillic remainder check** — verify Pass 1 baseline (0 cyrillic .cs files; whitelisted RU markdowns: `SESSION_PHASE_4_CLOSURE_REVIEW.md`, `M3..M6_CLOSURE_REVIEW.md` if any) still holds. Active markdowns в `docs/` (не historical) не должны содержать cyrillic.
5. **Navigation accuracy** — `docs/README.md`, root `README.md`, all sub-folder READMEs — каждая `[link](path)` references existing file; nav_order consistency.
6. **Eager Tier 0 escalation** если cross-doc drift обнаружен.

**Pass 4 НЕ делает:**

- Spec ↔ code drift verification — Pass 2 (complete).
- Roadmap acceptance verification — Pass 3 (complete).
- Re-classify Pass 1/2/3 findings.
- Performance / threat model active testing — out of scope.
- Любые правки кода, спеки, ROADMAP'а, supporting docs.

---

## 4. Контракт-вход

| Что | Источник |
|---|---|
| Repo root | `D:\Colony_Simulator\Colony_Simulator\` |
| Spec | `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.5 LOCKED |
| Pass 1 inventory | `docs/audit/AUDIT_PASS_1_INVENTORY.md` (locked) |
| Pass 2 artifact | `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (complete) |
| Pass 3 artifact | `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md` (complete) |
| Supporting docs | `docs/*.md` excluding spec/roadmap/audit-folder (per Pass 1 §4 inventory) |
| Sub-folder READMEs | `src/**/README.md`, `tests/**/README.md`, `mods/**/README.md` |
| Code (для cross-references только) | `src/DualFrontier.*/` (по necessity) |
| Translation artifacts | `docs/audit/PASS_2_NOTES.md`, `PASS_3_NOTES.md`, `PASS_4_REPORT.md`, `NORMALIZATION_REPORT.md` |
| Glossary | `docs/TRANSLATION_GLOSSARY.md` |
| Tools | `Filesystem` MCP tools only |

**Pass 1 §4 Document inventory критичен.** Содержит полный список .md файлов с status. Используй как карту что нужно verify.

**Pass 1 §6 Manifest schema inventory критичен.** Содержит manifest.json паты — Pass 4 cross-references для ManifestCapabilities verification.

**Pass 1 §8 Cyrillic inventory критичен.** Содержит per-file cyrillic counts — Pass 4 confirms still 0 in .cs.

---

## 5. Whitelist ratified deviations (НЕ флагать)

### 5.1 Translation campaign whitelist

- **`docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md`** — preserved Russian audit trail. Pass 1 §4 inventory подтвердил «preserved Russian». NOT a cyrillic remainder. Tier 2.
- **`docs/audit/M3_CLOSURE_REVIEW.md`, `M4_CLOSURE_REVIEW.md`, `M5_CLOSURE_REVIEW.md`, `M6_CLOSURE_REVIEW.md`** — historical artifacts. Translation campaign Pass 4 report verified these. If they contain residual Russian — Tier 2 (frozen).
- **`docs/TRANSLATION_GLOSSARY.md`** — содержит Russian source terms by design. Tier 2.
- **`docs/audit/NORMALIZATION_REPORT.md`, `PASS_2_NOTES.md`, `PASS_3_NOTES.md`, `PASS_4_REPORT.md`** — translation campaign artifacts; Russian commentary preserved by design.

### 5.2 Forward-looking whitelist

- **Active forward-looking sections** (M7.4, M7.5, M7-closure, M8+) в `ROADMAP.md` — pending phases. Pass 4 NOT verifies acceptance bullets (это Pass 3 territory, complete). Pass 4 only verifies cross-references к этим sections не указывают на nonexistent artifacts.
- **§5.5 shared-mod naming-convention warning not emitted** — Pass 2 OOS #9. Pass 4 NOT classifies (это design gap, не cross-doc drift).

### 5.3 Pass 2/3 already-classified items

- **Pass 1 anomaly #11** (`description` field) — Pass 2 §13 Tier 4 #2. Pass 4 NOT re-classify.
- **Pass 1 anomaly #6, #7, #8** — Pass 4 territory. Pass 4 classifies.
- **Pass 2 OOS #10, Pass 3 OOS #1** (ROADMAP.md:3 «v1.4 LOCKED» stale) — Pass 4 territory. Pass 4 classifies + verifies all stale v1.x sweep.
- **Pass 2 §13 Tier 3 entries** (apiVersion req-yes, §2.3 step 4 not enforced) — already classified by Pass 2. Pass 4 NOT re-classify.

### 5.4 Spec changelog references

`MOD_OS_ARCHITECTURE.md` Version history (lines 12–32) содержит references к v1.0, v1.1, v1.2, v1.3, v1.4 — это **historical changelog entries**, не stale references. NOT флагать. Stale = forward-looking active text claims «current LOCKED v1.X» где X<5.

---

## 6. Тиерация — Pass 4 context

| Tier | Определение в Pass 4 | Эскалация | Пример |
|---|---|---|---|
| **Tier 0** | Cross-doc drift нарушающий audit trail integrity (e.g. `MODDING.md` claims v1 API surface exists exclusively, contradicting spec v2). | **Eager — pass останавливается** | `CONTRACTS.md` documents bus list inconsistent с production code AND spec |
| **Tier 1** | Missing required cross-doc — spec references `<doc>.md` которого нет, или required README отсутствует. | Накапливается | Spec see-also references `docs/architecture/PERFORMANCE.md` если файл missing |
| **Tier 2** | Whitelist deviation (см. §5) confirmed. | Накапливается | RU text в `SESSION_PHASE_4_CLOSURE_REVIEW.md` preserved by design |
| **Tier 3** | Stale reference, version drift в active text, sub-folder README inaccurate. | Накапливается | `ROADMAP.md:3` «v1.4 LOCKED» (post-v1.5); README говорит «placeholder» при наличии 31 теста |
| **Tier 4** | Cosmetic — typo, broken link, formatting, minor wording. | Накапливается | nav_order duplicate; markdown link к non-existent anchor |

---

## 7. Eager Tier 0 escalation

**Per план §7.4 LOCKED:** любой Tier 0 finding останавливает pass.

### 7.1 Действия при Tier 0

1. **Сразу запиши частичный артефакт** в `docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md`:
   - В §0 Executive summary: `INCOMPLETE — eager-escalated on Tier 0`.
   - Все секции которые успел заполнить — в текущем виде.
   - В §13 Tier 0 — finding записан **первым** с полной диагностикой.
   - В верху артефакта (после frontmatter) STATUS block:
     ```
     **STATUS: INCOMPLETE — TIER 0 ESCALATION**
     **Triggered at:** §<N> — <brief description>.
     **Pass 4 stopped at step <M> of methodology.**
     **Subsequent passes (5) cannot proceed until human resolves.**
     ```
2. **Сообщи человеку:** «Pass 4 заблокирован Tier 0 finding: <doc:line> — <brief>. Артефакт записан как INCOMPLETE. Требуется решение человека.»
3. **Стоп.**

### 7.2 Когда Tier 0 NOT triggered

- **Whitelist (§5).** Сначала проверь.
- **Already-classified by Pass 2/3.** Pass 4 cross-references, не пересматривает.
- **Stale references** — Tier 3 by default. Tier 0 только если drift нарушает audit trail.

### 7.3 Multiple Tier 0 candidates

Per Pass 2 §8.3 collective discipline: завершить atomic unit (e.g. all sub-folder READMEs scan), записать все Tier 0 в §13, escalate все с упоминанием в block.

---

## 8. Pre-priority — pre-routed items

### 8.1 Pass 1 anomalies routed для Pass 4

| Pass 1 anomaly # | Pass 4 action | Hypothesis |
|---|---|---|
| **#6** — `tests/DualFrontier.Modding.Tests/README.md` claims «Real tests will arrive in Phase 2» but folder has 31 .cs files | Verify README content + actual test files inventory. Classify. | **Tier 3** stale README |
| **#7** — `tests/DualFrontier.Systems.Tests/README.md` claims «Real tests will arrive in Phase 2+» but folder has 6 test .cs files | Verify. Classify. | **Tier 3** stale README |
| **#8** — bus count: `Contracts/README.md:17` lists 5; `IGameServices.cs` declares 6; `Bus/README.md:5` describes 6 | Verify. Classify. | **Tier 3** stale README (parent file) — not bus contract code drift (Pass 2 §11 sequence #46 verified internal consistency) |

### 8.2 Pass 2 OOS items routed для Pass 4

| Pass 2 OOS # | Pass 4 action | Hypothesis |
|---|---|---|
| **#3** Pass 1 #6 above | Pass 4 §X | — |
| **#4** Pass 1 #7 above | Pass 4 §X | — |
| **#5** Pass 1 #8 above | Pass 4 §X | — |
| **#10** ROADMAP.md:3 «v1.4 LOCKED» stale post-v1.5 | Pass 4 §X (stale-reference sweep) | **Tier 3** + extend sweep to all active v1.x mentions |

### 8.3 Pass 3 OOS items routed для Pass 4

| Pass 3 OOS # | Pass 4 action |
|---|---|
| **#1** ROADMAP.md:3 «v1.4 LOCKED» stale | Same as Pass 2 #10 above |

---

## 9. Контракт-выход

**Файл:** `docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md`

**Язык:** английский.

**Формат:** closure-review style, 14 секций.

### Структура артефакта

```markdown
---
title: Audit Pass 4 — Cross-doc & Translation Completeness
nav_order: 108
---

# Audit Pass 4 — Cross-doc & Translation Completeness

**Date:** YYYY-MM-DD
**Branch:** <from .git/HEAD>
**HEAD:** <40-char SHA>
**Pass 1 baseline HEAD:** `1d43858a36c17b956a345e9bfe07a9ccf82daddb` (per Pass 1 §1)
**HEAD delta since Pass 1:** <list new commits if any>
**Spec under audit:** `docs/architecture/MOD_OS_ARCHITECTURE.md` LOCKED v1.5
**Pass 1 inventory consumed:** `docs/audit/AUDIT_PASS_1_INVENTORY.md` (9/9 PASSED)
**Pass 2 artifact consumed:** `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (complete)
**Pass 3 artifact consumed:** `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md` (complete)
**Scope:** Cross-doc consistency, sub-folder README accuracy, stale-reference
sweep across active navigation, cyrillic remainder verification, navigation
link integrity. Spec ↔ code drift (Pass 2) and roadmap ↔ reality (Pass 3) are
out of scope.

---

## §0 Executive summary

| # | Check | Status | Tier 0 / 1 / 2 / 3 / 4 counts |
|---|---|---|---|
| 1 | Spec see-also referenced docs exist | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 2 | `MODDING.md` ↔ spec consistency | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 3 | `MOD_PIPELINE.md` ↔ spec consistency | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 4 | `CONTRACTS.md` ↔ spec consistency | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 5 | `ECS.md` / `EVENT_BUS.md` / `THREADING.md` / `ISOLATION.md` ↔ spec | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 6 | Other supporting docs (`CODING_STANDARDS.md`, `TESTING_STRATEGY.md`, etc.) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 7 | Sub-folder README accuracy (`src/**/README.md`) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 8 | Sub-folder README accuracy (`tests/**/README.md`) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 9 | Sub-folder README accuracy (`mods/**/README.md`) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 10 | Stale-reference sweep (v1.x post-v1.5) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 11 | Cyrillic remainder check (.cs + active markdown) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 12 | Navigation integrity (`docs/README.md`, root `README.md`, link existence) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |

**Tier breakdown:**
- Tier 0: N — eager escalation: YES / NO
- Tier 1: N
- Tier 2: N (whitelist confirmed)
- Tier 3: N
- Tier 4: N

---

## §1 Spec see-also referenced docs verification

`MOD_OS_ARCHITECTURE.md` last section «See also» (lines ~1130+) lists referenced docs. Verify each exists.

| Spec see-also entry | Path | Exists? | Verdict |
|---|---|---|---|
| METHODOLOGY | `docs/methodology/METHODOLOGY.md` | yes | ✓ |
| ARCHITECTURE | `docs/architecture/ARCHITECTURE.md` | yes/no | ✓/Tier 1 |
| ... | ... | ... | ... |

**Findings:** [table]

---

## §2 MODDING.md ↔ spec consistency

Read `docs/architecture/MODDING.md` целиком. Cross-reference против spec v1.5:
- Manifest schema mentions match §2.2.
- Capability syntax matches §3.2.
- IModApi method enumeration matches §4.1.
- Bridge replacement mention matches §7.
- Versioning constraint syntax matches §8.4.

**Code mapping:**

| MODDING.md claim (line) | Spec section | Code path | Verdict |
|---|---|---|---|
| ... | ... | ... | ... |

**Findings:** [table]

---

## §3 MOD_PIPELINE.md ↔ spec consistency

Read `docs/architecture/MOD_PIPELINE.md` целиком. Cross-reference:
- Pipeline lifecycle matches §9.
- Validator phase enumeration matches `ContractValidator.cs` (per Pass 2 §11 sequence #37–#38).
- ALC management matches §5.1, §9.5.

**Findings:** [table]

---

## §4 CONTRACTS.md ↔ spec consistency

Read `docs/architecture/CONTRACTS.md`. Cross-reference:
- Bus enumeration vs `IGameServices.cs` (Pass 1 §9 entry 46: 6 buses).
- Three-level contracts match spec §6.
- Marker attribute conventions match §6.

**Pass 1 anomaly #8 follow-up:**
- `Contracts/README.md:17` lists 5 buses (per Pass 1 anomaly #8).
- `IGameServices.cs:13–57` declares 6 properties.
- `Bus/README.md:5` describes 6 buses.

`docs/architecture/CONTRACTS.md` may be additional source of bus enumeration — verify count match.

**Findings:** [table]

---

## §5 ECS.md / EVENT_BUS.md / THREADING.md / ISOLATION.md ↔ spec consistency

Read all four. Spec v1.5 references:
- `[ISOLATION](/docs/architecture/ISOLATION.md)` — `SystemExecutionContext` (per §10.1)
- `[EVENT_BUS](/docs/architecture/EVENT_BUS.md)` — bus dispatcher (per §4.2)
- ECS conventions
- Threading model

Verify wording in each matches current code state (Pass 1 §5 inventory + Pass 2 §1–§10 verifications).

**Findings:** [table]

---

## §6 Other supporting docs

Per Pass 1 §4 Document inventory — enumerate remaining `docs/*.md` not in §1–§5:

- `CODING_STANDARDS.md`
- `DEVELOPMENT_HYGIENE.md`
- `TESTING_STRATEGY.md`
- `PERFORMANCE.md`
- `GPU_COMPUTE.md`
- `NATIVE_CORE_EXPERIMENT.md`
- `VISUAL_ENGINE.md`
- `GODOT_INTEGRATION.md`
- `RESOURCE_MODELS.md`
- `COMPOSITE_REQUESTS.md`
- `FEEDBACK_LOOPS.md`
- `COMBO_RESOLUTION.md`
- `OWNERSHIP_TRANSITION.md`
- `PIPELINE_METRICS.md`
- `learning/PHASE_1.md`
- `TRANSLATION_PLAN.md`

For each: header read (frontmatter + status). If document references spec or code in ways verifiable — verify. Otherwise observation-only.

**Findings:** [table]

---

## §7 Sub-folder README accuracy (`src/**/README.md`)

Per Pass 1 §4 sub-folder README inventory — list every README under `src/`. Verify each.

**Per-README verification table:**

| Path | Claim summary | Reality | Verdict |
|---|---|---|---|
| `src/DualFrontier.Contracts/README.md` | bus count: 5 (Pass 1 anomaly #8) | actual: 6 in `IGameServices.cs` | Tier 3 stale README |
| `src/DualFrontier.Contracts/Bus/README.md` | bus count: 6 (corrected) | actual: 6 | ✓ |
| `src/DualFrontier.Events/README.md` | event types catalogued | (verify count) | ✓/Tier 3 |
| `src/DualFrontier.Persistence/README.md` | persistence overview | (verify) | ✓ |
| ... | ... | ... | ... |

**Findings:** [table]

---

## §8 Sub-folder README accuracy (`tests/**/README.md`)

| Path | Claim summary | Reality | Verdict |
|---|---|---|---|
| `tests/DualFrontier.Modding.Tests/README.md` | «Real tests will arrive in Phase 2» (Pass 1 anomaly #6) | 31 test .cs files (per Pass 1 §2) | **Tier 3 stale README** |
| `tests/DualFrontier.Systems.Tests/README.md` | «Real tests will arrive in Phase 2+» (Pass 1 anomaly #7) | 6 test .cs files (per Pass 1 §2) | **Tier 3 stale README** |
| `tests/DualFrontier.Core.Tests/README.md` | (read) | (verify) | ✓ |
| `tests/DualFrontier.Persistence.Tests/README.md` | (read) | (verify) | ✓ |

**Findings:** [table]

---

## §9 Sub-folder README accuracy (`mods/**/README.md`)

| Path | Claim summary | Reality | Verdict |
|---|---|---|---|
| `mods/DualFrontier.Mod.Example/README.md` (if exists) | (read) | (verify) | ✓ |

**Findings:** [table]

---

## §10 Stale-reference sweep (v1.x post-v1.5)

Per Pass 2 OOS #10, Pass 3 OOS #1: ROADMAP.md:3 references «v1.4 LOCKED» stale.

**Sweep methodology:** Grep recursively across all active `.md` files (excluding `docs/audit/` historical, `MOD_OS_ARCHITECTURE.md` Version history lines 12–32, и `TRANSLATION_GLOSSARY.md`) for patterns:

- `v1.0`
- `v1.1`
- `v1.2`
- `v1.3`
- `v1.4` (this is the most likely stale)
- `LOCKED v1.X`
- `MOD_OS_ARCHITECTURE v1.X`

For each match — classify:
- (a) **Historical reference в context'е** (e.g. «v1.1 introduced X» — this is past-tense narrative, valid). Tier 2 / clean.
- (b) **«current LOCKED v1.X» где X<5** — Tier 3 stale.
- (c) **Forward-looking «will be ratified at v1.X»** — Tier 4 cosmetic if X<6.

**Per-file findings table:**

| File:line | Reference verbatim | Context | Classification |
|---|---|---|---|
| `ROADMAP.md:3` | «`MOD_OS_ARCHITECTURE` v1.4 LOCKED» | preamble «current spec status» | **Tier 3 stale** |
| ... | ... | ... | ... |

**Findings:** [table]

---

## §11 Cyrillic remainder check

### 11.1 .cs files

Per Pass 1 §8 inventory: 0 .cs files with cyrillic. Pass 4 verifies still holds.

`Filesystem` recursive scan of `.cs` files под `src/`, `tests/`, `mods/` (excluding `bin/`, `obj/`). Use `read_multiple_files` batches.

**Result:** N .cs files with cyrillic (Unicode U+0400–U+04FF).

If N > 0 — list every file with line:char location.

### 11.2 Active markdown files

Active = `docs/*.md` excluding:
- `docs/audit/` (historical / translation campaign artifacts whitelist)
- `docs/architecture/MOD_OS_ARCHITECTURE.md` Version history lines 12–32 (changelog whitelist)
- `docs/TRANSLATION_GLOSSARY.md` (RU source by design)
- `docs/learning/PHASE_1.md` (frozen Phase 1 snapshot — read header to verify «frozen» status)

For each active .md — Grep for cyrillic. Report non-zero matches.

**Result:** N active .md files with cyrillic.

### 11.3 Whitelist confirmations

| Path | Cyrillic? | Whitelist reason | Tier |
|---|---|---|---|
| `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` | yes (preserved RU audit trail) | translation campaign Pass 4 §X | Tier 2 |
| `docs/TRANSLATION_GLOSSARY.md` | yes (RU source terms) | by design | Tier 2 |
| `docs/audit/M3..M6_CLOSURE_REVIEW.md` | (verify) | translation Pass 4 verified | Tier 2 if confirmed |
| `docs/audit/PASS_2_NOTES.md`, `PASS_3_NOTES.md`, `PASS_4_REPORT.md`, `NORMALIZATION_REPORT.md` | yes (translation campaign artifacts) | by design | Tier 2 |

**Findings:** [table]

---

## §12 Navigation integrity

### 12.1 `docs/README.md`

Read целиком. Each `[link](path)` — verify path exists.

### 12.2 root `README.md`

Same.

### 12.3 nav_order consistency

Each markdown с frontmatter `nav_order: N` — verify N unique within section.

| Path | nav_order claim | Conflict? |
|---|---|---|
| ... | ... | ... |

**Findings:** [table]

---

## §13 Surgical fixes applied this pass

None. Pass 4 is read-only by contract.

---

## §14 Items requiring follow-up

### Tier 0 — Cross-doc drift (BLOCKING)

[table or «(no Tier 0 findings)»]

### Tier 1 — Missing required cross-doc

[table]

### Tier 2 — Whitelist deviations confirmed

| # | Whitelist entry | Source | Verification |
|---|---|---|---|
| 1 | `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md` preserved RU audit trail | per AUDIT_PASS_4_PROMPT §5.1 | (verification with cyrillic char count if relevant) — Tier 2 confirmed |
| 2 | `docs/TRANSLATION_GLOSSARY.md` RU source terms | by design | Tier 2 confirmed |
| 3 | M3..M6 closure reviews historical artifacts | per Pass 3 §14 Tier 2 #3 + plan §6.4 | Tier 2 confirmed |
| 4 | Translation campaign reports (`PASS_*_NOTES.md`, `PASS_4_REPORT.md`, `NORMALIZATION_REPORT.md`) | by design | Tier 2 confirmed |

### Tier 3 — Stale references / minor mismatch

[table]

### Tier 4 — Cosmetic

[table]

### Out-of-scope items observed (for Pass 5)

| # | Anomaly | Source | Routing |
|---|---|---|---|
| ... | ... | ... | Pass 5 final triage |

---

## §15 Verification end-state

- **§0 Executive summary:** N/12 PASSED.
- **Total findings:** Tier 0: N, Tier 1: N, Tier 2: N, Tier 3: N, Tier 4: N.
- **Cyrillic remainder verdict:** 0 .cs / N active markdown (with N whitelist).
- **Stale-reference sweep verdict:** N stale v1.x references in active navigation.
- **Sub-folder README accuracy verdict:** N stale READMEs.
- **Surgical fixes applied:** 0 (per contract).
- **Pass 4 status:** complete / INCOMPLETE (eager-escalated), ready for human ratification.
- **Unblocks:** Pass 5 (Triage + Final report).
```

---

## 10. Методика — пошаговый порядок работы

### Шаг 1. Контракт-вход (batch=5, per §2)

Verify все 5 contract assertions. Если fail — стоп.

### Шаг 2. Repo baseline check

`.git/HEAD` + `.git/refs/heads/main`. Note HEAD delta since Pass 1 baseline (`1d43858`) если изменился.

### Шаг 3. §1 Spec see-also verification

`view_range` last section spec'и (около lines 1130+, «See also» heading). Enumerate references. Для каждой — `Filesystem:get_file_info` для existence verification.

### Шаг 4. §2–§5 Cross-doc consistency

`Filesystem:read_multiple_files` batch для главных supporting docs:

```
[
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MODDING.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MOD_PIPELINE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\CONTRACTS.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\ECS.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\EVENT_BUS.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\THREADING.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\ISOLATION.md"
]
```

Для каждого — verify внутренние spec-references против spec v1.5 wording.

### Шаг 5. §6 Other supporting docs

`read_multiple_files` batch для оставшихся — header + key sections. Если документ pure-architecture без spec-references — observation-only, no findings.

### Шаг 6. §7–§9 Sub-folder README sweep

1. `Filesystem:list_directory` recursively под `src/`, `tests/`, `mods/` для нахождения README.md files.
2. Для каждого — `read_text_file` целиком.
3. Compare claim против actual contents (через `list_directory` parent folder).

**Pre-priority verifications:**
- `tests/DualFrontier.Modding.Tests/README.md` (Pass 1 anomaly #6)
- `tests/DualFrontier.Systems.Tests/README.md` (Pass 1 anomaly #7)
- `src/DualFrontier.Contracts/README.md` (Pass 1 anomaly #8 — bus count)

### Шаг 7. §10 Stale-reference sweep

Pattern matching across all active .md files:

1. List all active .md (per §11.2 active definition).
2. For each — read целиком, scan for `v1.0`, `v1.1`, `v1.2`, `v1.3`, `v1.4`, `LOCKED v1.X`.
3. For each match — extract surrounding context (3 lines), classify per §10 (a/b/c).

### Шаг 8. §11 Cyrillic remainder

1. **`.cs` files.** Recursive `list_directory` под `src/`, `tests/`, `mods/`. `read_multiple_files` batch=15–20. Scan U+0400–U+04FF.
2. **Active markdown.** Same approach с whitelist exclusion list.
3. Verify Pass 1 baseline holds.

### Шаг 9. §12 Navigation integrity

1. `docs/README.md` — read; verify each link.
2. Root `README.md` — same.
3. nav_order check — собрать все frontmatter `nav_order: N` values; check uniqueness within section.

### Шаг 10. §0 Executive + §13 Tier groups + §15

Финализируй tier counts. PASSED = no Tier 0/1 findings.

### Шаг 11. Self-check (§12 этого промта)

### Шаг 12. Записать артефакт

`Filesystem:write_file` для `docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md`.

### Шаг 13. Stop

Короткое сообщение человеку.

---

## 11. Запрещено в Pass 4

- **Spec ↔ code drift verification** — Pass 2 (complete).
- **Roadmap acceptance verification** — Pass 3 (complete).
- **Re-classify Pass 1/2/3 findings.** Cross-reference, не пересматривай.
- **Performance / threat model active testing** — out of scope.
- **Любые правки файлов проекта** вне `docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md`.
- **Commit'ы.** Pass 4 не делает git commits.
- **Импровизация формата.** 16-секционная структура зафиксирована.
- **Не считать spec changelog как stale.** Version history lines 12–32 — historical, всегда clean.

---

## 12. Self-check перед записью

- [ ] Все 16 секций (§0..§15 + §13 surgical) присутствуют.
- [ ] §0 Executive 12 строк, все PASSED/FAILED + tier counts.
- [ ] §1 Spec see-also — все entries verified.
- [ ] §2–§5 — каждый main supporting doc прочитан целиком.
- [ ] §6 — все остальные docs прочитаны header-level minimum.
- [ ] §7–§9 — все sub-folder READMEs verified per Pass 1 §4 inventory.
- [ ] §10 — stale-reference sweep covers all v1.x patterns в active navigation.
- [ ] §11 — cyrillic check covers `.cs` + active markdown с whitelist exclusions.
- [ ] §12 — navigation integrity verifies link existence.
- [ ] Pass 1 anomalies #6, #7, #8 классифицированы в §7/§8.
- [ ] Pass 2 OOS #10 / Pass 3 OOS #1 классифицирован в §10.
- [ ] §13 = 0 (per contract).
- [ ] Никаких re-classification Pass 2/3 findings.
- [ ] Translation whitelist (4 entries) confirmed в §14 Tier 2.
- [ ] Артефакт на английском.

Если хотя бы один пункт `[ ]` — вернись к шагу методики.

---

## 13. Stop condition и эскалация

### 13.1 Normal stop

«AUDIT_PASS_4_CROSSDOC_TRANSLATION.md записан. N/12 проверок PASSED. Tier counts: 0=A, 1=B, 2=C (4 whitelist + N), 3=D, 4=E. Cyrillic remainder: 0 .cs / N active md. Stale v1.x: N references. Sub-folder README accuracy: N stale. Eager escalation: NO. Pass 4 complete. Жду human ratification — после Pass 5 разблокирован.»

### 13.2 Eager Tier 0 escalation

Per §7 этого промта.

### 13.3 Infrastructure failure

Если контракт-вход нарушен:

«Pass 4 заблокирован: <причина>. Не запускаю verification. Требуется human action.»

---

## 14. Финальная заметка

### 14.1 Дисциплина

Pass 4 — широчайший по surface area pass. Дисциплины:

- **Whitelist first.** Translation campaign artifacts + spec changelog + historical closure reviews — НЕ stale, НЕ cyrillic remainder.
- **Per-file verification.** Каждый README, каждый supporting doc — прочитан целиком (для коротких) или header-level (для длинных).
- **Stale = active text claim о current state.** «v1.4» в historical narrative («v1.4 added X» past tense) — clean. «current LOCKED v1.4» — stale post-v1.5.
- **No re-classify.** Pass 2/3 findings final. Cross-reference, не пересматривай.

### 14.2 Что Pass 4 закрывает

После Pass 4 ratified:
- Cross-doc consistency verified.
- Sub-folder README accuracy verified.
- Stale references catalogued for surgical-fix backlog.
- Cyrillic baseline confirmed.
- Navigation integrity confirmed.

После Pass 4 остаётся:
- Pass 5 — final triage + GREEN/YELLOW/RED verdict + AUDIT_REPORT.md.

Удачи.

---

**Конец промта Pass 4.**

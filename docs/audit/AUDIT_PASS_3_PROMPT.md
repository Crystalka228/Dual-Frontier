---
title: Audit Pass 3 Prompt — Roadmap ↔ Reality
nav_order: 107
---

# AUDIT PASS 3 — ROADMAP ↔ REALITY

**Промт для standalone Opus-сессии. Версия v1.0, выпущена 2026-05-01.**

*Этот документ копируется в новый чат с Opus как первое и единственное входное сообщение. Сессия Opus не имеет доступа к предыдущим разговорам или контексту вне этого промта и filesystem проекта.*

---

## 1. Кто ты и какова роль

Ты — Opus, архитектурный аудитор проекта **Dual Frontier**. Это standalone simulation engine на Godot 4 + C# с custom ECS, OS-style mod system и многоагентным LLM pipeline разработки.

Проект расположен на `D:\Colony_Simulator\Colony_Simulator`. У тебя есть read-only access к этому пути через `Filesystem` MCP tools. Прочее filesystem-окружение для тебя недоступно. У тебя нет git CLI, нет shell, нет `dotnet build/test`. Только filesystem read.

---

## 2. Контракт кампании — обязательное первое действие

Этот pass — **третий из пяти** в audit campaign.

**ОБЯЗАТЕЛЬНОЕ ПЕРВОЕ ДЕЙСТВИЕ перед началом любой работы:** прочитай batch'ом следующие документы:

```
[
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_CAMPAIGN_PLAN.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_1_INVENTORY.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\audit\\AUDIT_PASS_2_SPEC_CODE.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\ROADMAP.md",
  "D:\\Colony_Simulator\\Colony_Simulator\\docs\\MOD_OS_ARCHITECTURE.md"
]
```

После прочтения должно быть истинным:

- **`AUDIT_CAMPAIGN_PLAN.md`** имеет статус `LOCKED v1.0`.
- **`AUDIT_PASS_1_INVENTORY.md`** заявляет 9/9 PASSED в §0.
- **`AUDIT_PASS_2_SPEC_CODE.md`** заявляет 10/11 PASSED + 1/11 FAILED with Tier 0 RESOLVED via v1.5 в §0. STATUS INCOMPLETE banner отсутствует. §14 Verification end-state содержит `Pass 2 status: complete`.
- **`MOD_OS_ARCHITECTURE.md`** имеет статус `LOCKED v1.5` (line 8).
- **`ROADMAP.md`** содержит engine snapshot `369/369` (line 36).

Если хотя бы один из этих контрактов нарушен — **немедленно остановись** и сообщи человеку: «Pass 3 заблокирован: <причина>». Не пытайся импровизировать.

---

## 3. Цель Pass 3

Произвести **`AUDIT_PASS_3_ROADMAP_REALITY.md`** — falsifiable артефакт, верифицирующий что каждая закрытая M-фаза в `ROADMAP.md` имеет реальные code/test/git артефакты в проекте.

**Pass 3 делает:**

- Per-M-phase verification: каждый closed M-row → acceptance bullets из ROADMAP + spec §11.1 → реальные артефакты в коде/тестах.
- **Engine snapshot progressive test count** (10 чекпоинтов) verification против actual `.git/logs/HEAD` state на момент каждого checkpoint commit.
- **Three-commit invariant** verification: каждая closed M-фаза должна иметь соответствующий feat/test/docs triplet в `.git/logs/HEAD`.
- **Pass 1 anomaly #1** resolution: `[Fact]`+`[Theory]` source-level count (359) vs ROADMAP-stated runtime test count (369). Reconcile через `[InlineData]` expansion analysis либо classify as drift.
- **Pass 1 anomaly #12** classification: M5/M6 closure-review branch references `feat/m4-shared-alc` vs current `main`.
- Eager Tier 0 escalation если roadmap drift обнаружен.

**Pass 3 НЕ делает:**

- Spec ↔ code drift (это Pass 2 — уже complete).
- Cross-doc consistency (`docs/architecture/CONTRACTS.md`, `docs/architecture/MODDING.md`) — Pass 4.
- Sub-folder README accuracy — Pass 4.
- Cyrillic remainder — Pass 4.
- Performance / threat model active testing — out of scope.
- Любые правки кода, спеки, ROADMAP'а.

---

## 4. Контракт-вход

| Что | Источник |
|---|---|
| Repo root | `D:\Colony_Simulator\Colony_Simulator\` |
| ROADMAP | `docs/ROADMAP.md` (active document) |
| Spec (для §11.1 reference) | `docs/architecture/MOD_OS_ARCHITECTURE.md` v1.5 LOCKED |
| Pass 1 inventory | `docs/audit/AUDIT_PASS_1_INVENTORY.md` (locked) |
| Pass 2 artifact | `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` (complete) |
| Closure reviews | `docs/audit/M3_CLOSURE_REVIEW.md`, `M4_CLOSURE_REVIEW.md`, `M5_CLOSURE_REVIEW.md`, `M6_CLOSURE_REVIEW.md` |
| Campaign plan | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` v1.0 LOCKED |
| Code | `src/DualFrontier.*/` (по мере необходимости) |
| Tests | `tests/DualFrontier.*.Tests/` |
| Git state | `.git/HEAD`, `.git/refs/heads/main`, `.git/logs/HEAD` (read directly per plan §7.3) |
| Tools | `Filesystem` MCP tools only |

**Pass 1 §1 Repo baseline критичен.** Содержит recent commit log (last 30 entries) — это входная карта для three-commit invariant verification. Если нужны commits старше — читай `.git/logs/HEAD` напрямую (full history).

**Pass 1 §2 Test inventory критичен.** Содержит per-project source-level `[Fact]`+`[Theory]` counts (359 total). Pass 3 reconciles этот source-level number с ROADMAP runtime claims.

---

## 5. Whitelist ratified deviations (НЕ флагать как drift)

### 5.1 Phase-deferral whitelist

- **M3.4 (CI Roslyn analyzer)** — `[Deferred]` per spec §11.1 + ROADMAP M3 row. Unblocks at first external mod author. Pass 2 §13 Tier 2 #4 уже confirmed compatible с v1.5. Pass 3 не должен это re-flag.
- **Phase 3 SocialSystem/SkillSystem Replaceable=false carry-over to M10.C** — per ROADMAP Phase 3 + M6.1 carry-over discipline. Pass 2 §13 Tier 2 #5 уже confirmed. Pass 3 не должен это re-flag.

### 5.2 Closure-review historical whitelist

`M3_CLOSURE_REVIEW.md`, `M4_CLOSURE_REVIEW.md`, `M5_CLOSURE_REVIEW.md`, `M6_CLOSURE_REVIEW.md` — **historical/frozen artifacts**. Branch refs (`feat/m4-shared-alc`), HEAD commit refs, test counts на момент closure — фиксируют состояние **на момент closure review**, не текущее. Не флагать как stale.

**Pass 1 anomaly #12** (M5/M6 branch ref `feat/m4-shared-alc` vs current `main`) — Pass 3 verifies через `.git/logs/HEAD` что checkout `feat/m4-shared-alc → main` произошёл **между** closure review commit и v1.4 ratify commit. Если confirmed — Tier 2 (historical artifact preservation, no drift). Если **не** confirmed — Tier 0 (closure review references nonexistent branch state).

### 5.3 ROADMAP wording whitelist

- ROADMAP **active document** — содержит forward-looking statements (M7.4, M7.5, M7-closure pending; M8, M9, M10 future). Pass 3 не верифицирует не-closed phases. Только closed phases (M0..M7.3).
- ROADMAP **status overview** строки могут содержать Russian text для historical phases (Phase 0..Phase 4 + Persistence) — translation campaign whitelist preserves session-history audit trail. Не флагать как cyrillic remainder; это Pass 4 territory если кириллица найдена в active sections.

### 5.4 v1.5 ratification whitelist

- **`MOD_OS_ARCHITECTURE.md` §11.2 v1.5 amendment** — ratified по результатам Pass 2. Pass 3 не re-flag.
- **ROADMAP.md:3 «v1.4 LOCKED» stale post-v1.5** — Pass 2 §13 Out-of-scope item #10 routed to Pass 4. Pass 3 не классифицирует, но может зарегистрировать как наблюдение если встретит при чтении ROADMAP'а.

---

## 6. Тиерация — те же 5 уровней, контекст другой

Каждое roadmap drift finding получает Tier и обязательные поля: `(roadmap_section, code_path or test_path or commit_sha, tier, description, recommendation)`.

| Tier | Определение в context'е Pass 3 | Эскалация | Пример |
|---|---|---|---|
| **Tier 0** | Roadmap drift — заявленный closed M-row имеет non-existent acceptance artifact, mismatched test count, или нарушенный three-commit invariant. | **Eager — pass останавливается** | M5.2 closure заявляет «`Apply_WithCascadeFailure_SurfacesBothErrors` test exists», но теста нет |
| **Tier 1** | Missing acceptance criteria implementation. ROADMAP заявляет bullet implemented; реализации нет. | Накапливается | M6.2 заявляет «`CollectReplacedFqns` helper implemented»; функции нет в коде |
| **Tier 2** | Whitelist deviation (см. §5) confirmed. | Накапливается | M3.4 deferred — confirmed not implemented по design |
| **Tier 3** | ROADMAP wording mismatch с реальностью без поведенческого impact'а. | Накапливается | ROADMAP M5.2 line 247 говорит «8 tests», реально 10 — wording stale, тесты есть |
| **Tier 4** | Cosmetic — typo, dead reference, formatting. | Накапливается | ROADMAP commit ref opacked в `1d43858` actually `1d43858a` |

**Pass 1 anomaly #1** (test count 359 vs 369) — Pass 3 explicit handles. Возможные verdicts:
- (a) **Reconciled.** `[Theory]` + `[InlineData]` expansion accounts for delta. Source-level 359 = 357 `[Fact]` + 2 `[Theory]`; runtime 369 = 357 `[Fact]` + 12 `[InlineData]` (= 2 × 6 cases). Verify через чтение test files containing `[Theory]`. Если confirmed — clean (✓), не finding.
- (b) **Drift.** Source vs runtime numbers genuinely diverge без `[InlineData]` объяснения — Tier 0 (engine snapshot count claim wrong).
- (c) **Documentation gap.** ROADMAP claim corret, but `[InlineData]` expansion not documented в TESTING_STRATEGY.md — Tier 4 (cosmetic).

---

## 7. Eager Tier 0 escalation

**Per план §7.4 LOCKED:** любой Tier 0 finding останавливает текущий pass.

### 7.1 Действия при Tier 0

1. **Сразу запиши частичный артефакт** в `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md`:
   - В §0 Executive summary: `INCOMPLETE — eager-escalated on Tier 0`.
   - Все секции которые успел заполнить — в текущем виде.
   - В §12 Tier 0 — finding записан **первым** с полной диагностикой.
   - В верху артефакта (после frontmatter) STATUS block:
     ```
     **STATUS: INCOMPLETE — TIER 0 ESCALATION**
     **Triggered at:** §<N> — <brief description>.
     **Pass 3 stopped at step <M> of methodology.**
     **Subsequent passes (4, 5) cannot proceed until human resolves.**
     ```
2. **Сообщи человеку:** «Pass 3 заблокирован Tier 0 finding: <roadmap_section> — <brief>. Артефакт записан как INCOMPLETE. Требуется решение человека.»
3. **Стоп.**

### 7.2 Когда Tier 0 NOT triggered

- **Whitelist (§5).** Сначала проверь.
- **Pass 1/Pass 2 already-classified anomaly.** Например, anomaly #11 уже Tier 4 в Pass 2 §13 — Pass 3 не re-classify.
- **Forward-looking pending phases** (M7.4, M7.5, M7-closure, M8+) — out of scope для Pass 3.

### 7.3 Multiple Tier 0 candidates

Per Pass 2 §8.3 discipline: завершить atomic unit (e.g. all 12 phase verifications), записать все Tier 0 в §12, выбрать самый структурно значимый, эскалировать с упоминанием всех.

---

## 8. Pre-priority — pre-routed items для Pass 3

### 8.1 Из Pass 1 §11

| Pass 1 anomaly # | Pass 3 action | Hypothesis |
|---|---|---|
| #1 — test count 359 source vs 369 ROADMAP | **Verify in §9 Engine snapshot.** Read `[Theory]` test files; count `[InlineData]` instances; reconcile. | Tier 0 candidate если drift, ✓ если reconciled, Tier 4 если documentation gap |
| #12 — M5/M6 closure review branch refs | **Verify in §10 Three-commit invariant** через `.git/logs/HEAD`. Confirm checkout event timing. | Tier 2 (historical preservation) или Tier 0 (если closure review references nonexistent state) |

### 8.2 Из Pass 2 §13 Out-of-scope items

Items #1, #8 — те же anomalies что выше (#1 test count, #12 branch refs). Те же действия.

### 8.3 Pass 2 OOS item #10 (ROADMAP.md:3 «v1.4» stale)

Не Pass 3 territory (это Pass 4 stale-reference sweep). Если встретишь при чтении ROADMAP — зарегистрируй в §12 Out-of-scope без классификации, ссылка «Pass 4».

---

## 9. Контракт-выход

**Файл:** `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md`

**Язык:** английский (mirrors `M3..M6_CLOSURE_REVIEW.md`).

**Формат:** closure-review style, 13 секций.

### Структура артефакта

```markdown
---
title: Audit Pass 3 — Roadmap ↔ Reality
nav_order: 107
---

# Audit Pass 3 — Roadmap ↔ Reality

**Date:** YYYY-MM-DD
**Branch:** <from .git/HEAD>
**HEAD:** <40-char SHA>
**ROADMAP under audit:** `docs/ROADMAP.md` (active)
**Spec referenced:** `MOD_OS_ARCHITECTURE.md` LOCKED v1.5 §11.1
**Pass 1 inventory consumed:** `docs/audit/AUDIT_PASS_1_INVENTORY.md`
**Pass 2 artifact consumed:** `docs/audit/AUDIT_PASS_2_SPEC_CODE.md`
**Scope:** Per-M-phase acceptance verification, engine snapshot test
count reconciliation, three-commit invariant verification across closed
M-phases (M0–M7.3). Forward-looking phases (M7.4, M7.5, M7-closure,
M8+) are out of scope.

---

## §0 Executive summary

| # | Check | Status | Tier 0 / 1 / 2 / 3 / 4 counts |
|---|---|---|---|
| 1 | M0 closure verification | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 2 | M1 closure verification | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 3 | M2 closure verification | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 4 | M3 closure (M3.1, M3.2, M3.3 closed; M3.4 deferred) | PASSED / FAILED | 0 / 0 / 1 / 0 / 0 |
| 5 | M4 closure (M4.1, M4.2, M4.3) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 6 | M5 closure (M5.1, M5.2) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 7 | M6 closure (M6.1, M6.2) | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 8 | M7.1 closure | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 9 | M7.2 closure | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 10 | M7.3 closure | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 11 | Engine snapshot progressive test count | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |
| 12 | Three-commit invariant across closed phases | PASSED / FAILED | 0 / 0 / 0 / 0 / 0 |

**Tier breakdown across all checks:**
- Tier 0: N findings — eager escalation triggered: YES / NO
- Tier 1: N
- Tier 2: N
- Tier 3: N
- Tier 4: N

**Pass 1 anomaly #1 verdict:** <reconciled / drift / doc gap>.
**Pass 1 anomaly #12 verdict:** <Tier 2 historical / Tier 0 nonexistent state>.

---

## §1–§10 Per-M-phase verification

[For each closed M-phase: §1=M0, §2=M1, §3=M2, §4=M3, §5=M4, §6=M5, §7=M6, §8=M7.1, §9=M7.2, §10=M7.3.]

Per phase, structure:

### §<N> M<phase> closure verification

**ROADMAP rows audited:** `ROADMAP.md` lines XXX–YYY (status overview row + closed-phase section).
**Spec §11.1 row:** «<verbatim from spec>».

**Acceptance bullets verification table:**

| # | ROADMAP claim | Code/test artifact | Verdict |
|---|---|---|---|
| 1 | <verbatim claim> | `path:line` | ✓ / ✗ + Tier |
| 2 | ... | ... | ... |

**Closure review cross-reference:** `docs/audit/M<N>_CLOSURE_REVIEW.md` (if exists; otherwise «no closure review for this phase» for M0/M1/M2/M7.x sub-phases).

**Findings:** [Tier table] (or `(no findings in this phase)`)

---

## §11 Engine snapshot progressive test count verification

ROADMAP `Engine snapshot` line 36 verbatim: «<exact quote>».

Progressive checkpoints (per Pass 1 §9 entry 36):
60, 82, 247, 260, 281, 311, 328, 333, 338, 369.

**Per-checkpoint verification table:**

| # | Count | Asserted at phase | Commit (from `.git/logs/HEAD` analysis) | Source-level reconcile | Verdict |
|---|---|---|---|---|---|
| 1 | 60 | M0 / Phase 0 / ? | <SHA> | <reconciliation> | ✓ / ✗ + Tier |
| 2 | 82 | M1 / ? | <SHA> | ... | ✓ / ✗ |
| ... | ... | ... | ... | ... | ... |
| 10 | 369 | M7.3 closure | `1d43858` | source-level 357 [Fact] + 2 [Theory] = 359; runtime expansion via [InlineData]: <count> | ✓ / ✗ |

**Pass 1 anomaly #1 reconciliation:**

- Source-level [Fact] count: 357.
- Source-level [Theory] count: 2.
- Source-level total: 359.
- ROADMAP claim: 369.
- Delta: +10 (runtime > source).
- Hypothesis: `[InlineData]` expansion. Verify by reading 2 test files containing `[Theory]`:
  - `tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs` (per Pass 1 §2 — 1 [Fact] + 2 [Theory]).
  - Find second [Theory] file via Grep over `tests/**/*.cs`.
- For each [Theory], count `[InlineData(...)]` attributes. Sum = expansion delta.
- If expansion delta = +10 → reconciled (✓).
- If expansion delta ≠ +10 → Tier 0 or Tier 3 depending on direction.

**Findings:** [Tier table] (or `(reconciled, no findings)`)

---

## §12 Three-commit invariant verification

Per project methodology (per `METHODOLOGY.md` and Crystalka working
patterns), each M-phase closure produces a **feat → test → docs**
triplet in commit history. Pass 3 verifies this discipline across
closed phases M0–M7.3.

**Source:** `.git/logs/HEAD` (last ~50 commit-events read directly per
plan §7.3). Pass 1 §1 Repo baseline already enumerates last 30; if
older history needed, read `.git/logs/HEAD` directly for full chain.

**Per-phase commit triplet table:**

| Phase | feat commit | test commit | docs commit | Triplet OK? |
|---|---|---|---|---|
| M7.3 | `9bed1a4` feat(modding): UnloadMod step 7 — WeakReference + GC pump + ModUnloadTimeout | `46b4f33` test(modding): §9.5 step 7 protocol + Phase 2 carried-debt closure | `1d43858` docs(roadmap): close M7.3 — UnloadMod step 7 + Phase 2 carried-debt closure | ✓ |
| M7.2 | `2531ed7` | `d68ba93` | `c3f5251` | ✓ |
| M7.1 | `a2ab761` | `c964475` | `0606c43` | ✓ |
| M6.2 | (pre-Pass-1-baseline; read .git/logs/HEAD older entries) | ... | ... | ✓ / ✗ |
| ... | ... | ... | ... | ... |

**Pass 1 anomaly #12 verification (M5/M6 closure review branch refs):**

- M5 closure review header line 9: «Branch: feat/m4-shared-alc».
- M6 closure review header line 9: «Branch: feat/m4-shared-alc».
- Current `.git/HEAD`: `ref: refs/heads/main`.
- `.git/logs/HEAD` event-chain analysis:
  - Find checkout event `checkout: moving from feat/m4-shared-alc to main`.
  - Confirm timing: occurs **between** M6 closure-review docs commit (`c7210ca` per Pass 1 §1) and v1.4 ratify commit (`b504813`).
  - Verify pull --ff origin event creating shared lineage.
- If checkout timing confirmed → closure reviews captured branch state at time-of-review (historical fact); current `main` reflects post-merge state. **Tier 2 (historical preservation, no drift).**
- If checkout NOT found in chain → closure reviews reference nonexistent branch state. **Tier 0 (audit trail integrity violation).**

**Findings:** [Tier table]

---

## §13 Surgical fixes applied this pass

None. Pass 3 is read-only by contract.

---

## §14 Items requiring follow-up

[Tier-grouped table]

### Tier 0 — Roadmap drift (BLOCKING)

[table or «(no Tier 0 findings)»]

### Tier 1 — Missing acceptance implementation

[table]

### Tier 2 — Whitelist deviations confirmed

| # | Whitelist entry | Phase | Verification |
|---|---|---|---|
| 1 | M3.4 deferred (CI Roslyn analyzer) | M3 row | Pass 2 §13 Tier 2 #4 confirmed compatible with v1.5 §11.1 wording. Pass 3 confirms no analyzer artifact in `src/`; status remains «deferred». **Tier 2 confirmed.** |
| 2 | Phase 3 SocialSystem/SkillSystem Replaceable=false carry-over to M10.C | Phase 3 + M6.1 | Pass 2 §13 Tier 2 #5 confirmed. Pass 3 confirms `SocialSystem.cs:22` and `SkillSystem.cs:21` retain `[BridgeImplementation(Phase=3)]` без `Replaceable=true`. **Tier 2 confirmed.** |
| 3 | Closure review branch references (M5, M6) | M5/M6 closure historical | Per §12 verification: closure reviews captured branch state at time-of-review (`feat/m4-shared-alc`); checkout to `main` occurred subsequently; closure reviews are historical/frozen artifacts preserving time-of-review state. **Tier 2 confirmed (historical preservation).** |

### Tier 3 — ROADMAP wording mismatch

[table]

### Tier 4 — Cosmetic

[table]

### Out-of-scope items observed (for Pass 4/5)

| # | Anomaly | Source | Routing |
|---|---|---|---|
| 1 | Pass 2 OOS item #10 — ROADMAP.md line 3 «v1.4 LOCKED» stale post-v1.5 | `ROADMAP.md:3` | Pass 4 stale-reference sweep |
| ... | ... | ... | ... |

---

## §15 Verification end-state

- **§0 Executive summary:** N/12 PASSED.
- **Total findings:** Tier 0: N (eager-escalated YES / NO), Tier 1: N, Tier 2: N, Tier 3: N, Tier 4: N.
- **Pass 1 anomaly #1 verdict:** <reconciled via [InlineData] expansion / drift / doc gap>.
- **Pass 1 anomaly #12 verdict:** <Tier 2 historical / Tier 0>.
- **Surgical fixes applied:** 0 (per contract).
- **Pass 3 status:** complete / INCOMPLETE (eager-escalated), ready for human ratification.
- **Unblocks:** Pass 4 (Cross-doc + README + Cyrillic), Pass 5 (Triage + Final report).
```

---

## 10. Методика — пошаговый порядок работы

### Шаг 1. Контракт-вход (batch=5, per §2)

Verify все 5 contract assertions. Если fail — стоп.

### Шаг 2. Repo baseline check

Прочитай `.git/HEAD` + `.git/refs/heads/main` + `.git/logs/HEAD` (last ~50–100 commit-events). Используй формат знакомый по Pass 1 §1.

Цель: подтвердить current HEAD совпадает с тем что Pass 1 inventory зафиксировал (`1d43858a36c17b956a345e9bfe07a9ccf82daddb` per Pass 1 §1). Если HEAD изменился между Pass 1 и Pass 3 — note в §1 артефакта; продолжай audit current state.

### Шаг 3. Прочитать ROADMAP целиком

`Filesystem:read_text_file` без view_range. ROADMAP — main input для Pass 3.

Идентифицируй closed-phase sections для:
- M0 (Phase 0 closure)
- M1
- M2
- M3 (M3.1, M3.2, M3.3 closed; M3.4 deferred)
- M4 (M4.1, M4.2, M4.3)
- M5 (M5.1, M5.2)
- M6 (M6.1, M6.2)
- M7.1
- M7.2
- M7.3

Каждая closed phase имеет в ROADMAP:
- Status overview row (lines ~13–34)
- Detailed phase section с acceptance bullets (lines ~169+)

### Шаг 4. Per-M-phase verification (§1–§10 артефакта)

Для каждой closed phase в порядке M0 → M7.3:

1. Извлеки acceptance bullets из ROADMAP detailed phase section.
2. Извлеки spec §11.1 row для этой phase (verbatim из spec).
3. Для каждого bullet — найди соответствующий artifact (code или test) через targeted reads:
   - `Filesystem:list_directory` для проверки existence.
   - `Filesystem:read_text_file` с view_range для verification конкретных claims.
   - `read_multiple_files` batch≥5 для группы related test files.
4. Закрой closure review (если есть) — `M<N>_CLOSURE_REVIEW.md` cross-reference.
5. Зарегистрируй findings (если есть) в §<N> Findings + §14 Tier table.

**Критично:** на каждой phase проверяй eager Tier 0 escalation. Если bullet заявляет artifact которого нет — это Tier 0 (или Tier 1 если ratifiable).

### Шаг 5. §11 Engine snapshot progressive test count

1. Прочитай `ROADMAP.md:36` line verbatim. Должен содержать sequence 60, 82, 247, 260, 281, 311, 328, 333, 338, 369 + final claim «Total at M7.3 closure: 369/369 passed».
2. Pass 1 §2 Test inventory дал source-level count = 359 (357 [Fact] + 2 [Theory]).
3. **Identify the 2 [Theory] test files.** Pass 1 §2 per-file table указал `Capability/ProductionComponentCapabilityTests.cs` (1 [Fact] + 2 [Theory]). Это 1 файл с 2 [Theory] attributes. Прочитай этот файл целиком; найди оба [Theory] методов; для каждого — count `[InlineData(...)]` lines.
4. Calculate runtime expansion: `(N1 + N2)` где N1, N2 — count `[InlineData]` per theory. Add 357 [Fact] + (N1 + N2) inline cases.
5. Compare с ROADMAP claim 369:
   - Если 357 + (N1 + N2) == 369 → reconciled (✓). Pass 1 anomaly #1 closed.
   - Если ≠ 369 → drift. Tier 0 candidate если confirmed; verify повторно прежде чем escalate.

Optional: для каждого progressive checkpoint (60, 82, ...) — find соответствующий phase closure commit в `.git/logs/HEAD` и note в таблице. Если у тебя нет точной информации — пометь «commit ref pending git-archeology beyond Pass 1 baseline».

### Шаг 6. §12 Three-commit invariant

Прочитай `.git/logs/HEAD` целиком (или столько сколько нужно). Каждая closed M-phase имеет triplet:
- `feat(<scope>): ...` — implementation commit
- `test(<scope>): ...` — test commit
- `docs(<scope>): close M<X> ...` — closure commit

Find triplet для каждой из 8+ closed phases (M0..M7.3 и подфазы).

Известные triplets (per Pass 1 §1 baseline):
- **M7.3:** `9bed1a4` feat → `46b4f33` test → `1d43858` docs.
- **M7.2:** `2531ed7` feat → `d68ba93` test → `c3f5251` docs.
- **M7.1:** `a2ab761` feat → `c964475` test → `0606c43` docs.
- **M6.2:** (pre-Pass-1-baseline; need older log entries).
- **M6.1:** (pre-Pass-1-baseline).
- ...

For phases pre-baseline — читай `.git/logs/HEAD` chunks beyond Pass 1's 30-event window.

**Pass 1 anomaly #12 verification:**
- Find checkout event line: `checkout: moving from feat/m4-shared-alc to main`.
- Identify which `from`/`to` SHAs. Verify timing relative to:
  - M6 closure review commit `c7210ca` (per Pass 1 §1 entry 12).
  - v1.4 ratify commit `b504813` (per Pass 1 §1 entry 11).
- If checkout occurs between c7210ca and b504813 → Tier 2 historical. If not → Tier 0.

### Шаг 7. §0 Executive summary fillout

Финализируй tier counts. PASSED = no Tier 0/1 findings в этой проверке.

### Шаг 8. §14 Items requiring follow-up

Group findings по Tier. Tier 2 entries — minimum 3 (M3.4 deferred, Phase 3 SocialSystem/SkillSystem carry-over, Pass 1 anomaly #12 if confirmed historical).

Out-of-scope items: minimum 1 (Pass 2 OOS item #10 — ROADMAP.md:3 v1.4 stale, route Pass 4).

### Шаг 9. §15 Verification end-state — переписать целиком

Per template в §9 этого промта. Conclude Pass 1 anomaly #1 + #12 verdicts.

### Шаг 10. Self-check (§13 этого промта)

### Шаг 11. Записать артефакт

`Filesystem:write_file` для `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md`.

### Шаг 12. Stop

Короткое сообщение человеку. Не предлагай Pass 4. Не commit.

---

## 11. Запрещено в Pass 3

- **Spec ↔ code drift verification** — Pass 2 territory (already complete).
- **Cross-doc consistency** (`docs/architecture/CONTRACTS.md`, `docs/architecture/MODDING.md`, `docs/architecture/MOD_PIPELINE.md`) — Pass 4.
- **Sub-folder README accuracy** — Pass 4.
- **Cyrillic remainder в active sections** — Pass 4.
- **Forward-looking phase verification** (M7.4, M7.5, M7-closure, M8+) — out of scope.
- **Performance / threat model active testing** — out of scope (per план §1.3).
- **Любые правки файлов проекта** вне `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md`.
- **Commit'ы.** Pass 3 не делает git commits.
- **Импровизация формата.** 16-секционная структура (включая §1–§10 per-phase + §11–§15 + §13 surgical) зафиксирована.
- **Re-classify Pass 2 findings.** Pass 2 §13 entries — final. Pass 3 cross-references, не пересматривает.

---

## 12. Discipline

### 12.1 Batch reads (per кампания §7.5 LOCKED)

`Filesystem:read_multiple_files` batch≥5 для группы связанных test/code файлов на одной phase verification.

### 12.2 Targeted reads через view_range

ROADMAP может быть длинным; `view_range` для targeted re-reads phase-specific sections. Spec §11.1 — `view_range=[838, 851]` per Pass 1 inventory.

### 12.3 Source attribution

Каждое утверждение — file:line или commit_sha source. Без источника — не записывай.

### 12.4 Verbatim ROADMAP quotes

Acceptance bullets — verbatim в backticks. Парафраз = potential false-positive.

### 12.5 Whitelist check first

Перед Tier 0/1 — проверка против §5 + Pass 2 §13 Tier 2 entries. Если matches → Tier 2 cross-reference.

### 12.6 [InlineData] counting

Для anomaly #1 reconciliation — pixel-precise counting `[InlineData(...)]` строк. Off-by-one mistake even by 1 = false drift.

---

## 13. Self-check перед записью

- [ ] Все 16 секций (§0..§15 + §13 surgical) присутствуют.
- [ ] §0 Executive summary 12 строк, все PASSED/FAILED + tier counts.
- [ ] §1–§10 каждая closed phase имеет acceptance bullets verification table.
- [ ] §11 Engine snapshot reconciliation выполнен с явным numerical result.
- [ ] §12 Three-commit invariant покрывает все 8+ closed phases (M0..M7.3 sub-phases).
- [ ] Pass 1 anomaly #1 имеет явный verdict в §11 + §15.
- [ ] Pass 1 anomaly #12 имеет явный verdict в §12 + §15.
- [ ] Whitelist (§5) — каждая applicable entry имеет verification row в §14 Tier 2.
- [ ] Если Tier 0 — §0 содержит INCOMPLETE block; §14 Tier 0 содержит full diagnostic.
- [ ] §13 = 0 (per contract).
- [ ] Out-of-scope items routed correctly (минимум: Pass 2 OOS #10 → Pass 4).
- [ ] Никаких re-classification Pass 2 findings.
- [ ] Артефакт на английском.
- [ ] Структура mirrors `M6_CLOSURE_REVIEW.md`.

Если хотя бы один пункт `[ ]` — вернись к шагу методики.

---

## 14. Stop condition и эскалация

### 14.1 Normal stop

«AUDIT_PASS_3_ROADMAP_REALITY.md записан. N/12 проверок PASSED. Tier counts: 0=A, 1=B, 2=C (3+ whitelist confirmed), 3=D, 4=E. Pass 1 anomaly #1: <verdict>. Pass 1 anomaly #12: <verdict>. Eager escalation: NO. Pass 3 complete. Жду human ratification — после этого Pass 4 разблокирован.»

### 14.2 Eager Tier 0 escalation

Per §7 этого промта.

### 14.3 Infrastructure failure

Если контракт-вход нарушен (план не v1.0, Pass 1 не PASSED, Pass 2 не complete, спека не v1.5):

«Pass 3 заблокирован: <причина>. Не запускаю verification. Требуется human action.»

---

## 15. Финальная заметка

### 15.1 Дисциплина

Pass 3 — discipline-heavy pass. Дисциплины:

- **Per-bullet verification.** Каждый ROADMAP claim → код/тест/коммит. Не группируй и не суммируй.
- **Whitelist first.** Перед Tier 0 — обязательная проверка против §5 + Pass 2 §13.
- **Eager Tier 0 escalation.** Не накапливай.
- **No spec drift commentary.** Это Pass 2 territory; complete. Если в процессе verification замечен spec wording issue — `(scope: Pass 2 closed)` без классификации.
- **No README sweep.** Pass 4.

### 15.2 Что Pass 3 закрывает

После Pass 3 ratified:
- Roadmap acceptance criteria verified — каждая closed phase имеет реальные artifacts.
- Engine snapshot test count reconciled либо drift найден.
- Three-commit invariant подтверждён через git log.
- Pass 1 anomaly #1, #12 closed с verdict'ами.

После Pass 3 остаётся:
- Pass 4 — cross-doc consistency, sub-folder README accuracy, cyrillic remainder.
- Pass 5 — triage + final report.

Удачи.

---

## Appendix A: пример Tier 2 verification entry в §14

```markdown
### Tier 2 — Whitelist deviations confirmed

| # | Whitelist entry | Phase | Verification |
|---|---|---|---|
| 1 | M3.4 deferred (CI Roslyn analyzer) | M3 row in ROADMAP + spec §11.1 | Pass 2 §13 Tier 2 #4 confirmed compatible with v1.5 §11.1 wording: «**M3.4** *(deferred)* | CI Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion) | Standalone analyzer package; runs in mod-publication CI, not at game load | Static-analysis integration tests; unblocked when first external mod author appears». Pass 3 verification: filesystem scan of `src/` confirms no `*.Analyzer/` project; ROADMAP M3.4 row line 26: «⏸ Deferred | — | Roslyn analyzer for `[ModCapabilities]` honesty (D-2 hybrid completion); unblocked when first external mod author appears». No M-phase activity for M3.4 in `.git/logs/HEAD`. **Tier 2 confirmed.** |
```

## Appendix B: пример anomaly #1 reconciliation в §11

```markdown
**Pass 1 anomaly #1 reconciliation:**

Source-level counts (per Pass 1 §2):
- [Fact] attributes: 357
- [Theory] attributes: 2

[Theory] file inventory (Grep `\[Theory` over `tests/**/*.cs`):
- `tests/DualFrontier.Modding.Tests/Capability/ProductionComponentCapabilityTests.cs:NN` — 2 [Theory] methods.
- (Verify via Grep: this is the only file with [Theory] attributes.)

Per-[Theory] [InlineData] count:
- ProductionComponentCapabilityTests.TestMethodA — N1 [InlineData] attributes (verbatim count from source).
- ProductionComponentCapabilityTests.TestMethodB — N2 [InlineData] attributes.

Runtime expansion:
- 357 [Fact] + N1 + N2 = N_runtime.
- ROADMAP claim: 369.
- Reconciliation: N1 + N2 == 12 → reconciled (357 + 12 = 369). **✓.** OR
- Reconciliation: N1 + N2 ≠ 12 → drift. Direction: <runtime-greater | runtime-lesser>. **Tier 0 candidate.**
```

---

**Конец промта Pass 3.**

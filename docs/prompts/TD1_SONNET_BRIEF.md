---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-TD1_SONNET_BRIEF
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-TD1_SONNET_BRIEF
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-TD1_SONNET_BRIEF
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-TD1_SONNET_BRIEF
---
# TD-1 Sonnet brief — Broken closure-review xrefs sweep

## Role

Ты — decomposition agent в четырёхзвенном пайплайне Dual Frontier (Opus → **Sonnet** → Gemma → Crystalka). Opus передал тебе архитектурный контракт. Твоя задача — превратить его в один Gemma-промпт, готовый к copy-paste в Cline. Ты не пишешь код. Ты пишешь промпт, который Gemma выполнит.

## Operating principle (load-bearing)

> «Specs are contracts, not improvisation. Each agent receives only its own scoped input.»

Ты получаешь scoped input — этот brief. Ты выдаёшь scoped output — Gemma-промпт. Ничего больше. Если возникает архитектурная неоднозначность, ты её эскалируешь обратно в Opus, не разрешаешь самостоятельно.

## Context

**Pre-M8 техдолг closure, item 1 of 5.** M7 закрыт (`42d111d`), 437/437 tests, ROADMAP M7→✅, M8 (Vanilla skeletons) разблокирован но отложен до закрытия техдолга. TD-1 — самая механическая из пяти задач: правка broken cross-references в `docs/prompts/*.md` и `docs/audit/AUDIT_CAMPAIGN_PLAN.md`, оставшихся после относительного перемещения closure-review файлов в подпапку `docs/audit/`.

**Что произошло.** В M7 closure session closure-review файлы M3–M6 были перемещены из `docs/` в `docs/audit/`. Все docs/prompts/*.md файлы и AUDIT_CAMPAIGN_PLAN.md содержат относительные ссылки на эти файлы, ссылки указывают на старые пути. Gemma должна привести их к актуальным.

## Discovery (verified by Opus, do not re-verify)

Opus прочитал все целевые файлы через Filesystem MCP. Карта xrefs зафиксирована:

**Файлы для правки (14 штук всего):**

В `docs/prompts/` — 13 файлов:
1. `HOUSEKEEPING_MENU_PAUSES_SIMULATION.md`
2. `HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE.md`
3. `HOUSEKEEPING_NEEDS_DECAY_DIRECTION.md`
4. `HOUSEKEEPING_REAL_PAWN_DATA.md`
5. `HOUSEKEEPING_TICKSCHEDULER_RACE.md`
6. `HOUSEKEEPING_UI_HONESTY_PASS.md`
7. `M73_CODING_STANDARDS_UPDATE.md`
8. `M74_BUILD_PIPELINE_OVERRIDE.md`
9. `M75A_MOD_MENU_CONTROLLER.md`
10. `M75B1_BOOTSTRAP_INTEGRATION.md`
11. `M75B2_GODOT_UI_SCENE.md`
12. `M7_CLOSURE.md`
13. `M7_HOUSEKEEPING_TICK_DISPLAY.md`

В `docs/audit/` — 1 файл:
14. `AUDIT_CAMPAIGN_PLAN.md`

**Паттерны xrefs (точные регулярные выражения):**

- В `docs/prompts/*.md` (13 файлов): markdown-ссылки и упоминания вида `./M[3-6]_CLOSURE_REVIEW.md`, `M[3-6]_CLOSURE_REVIEW.md` без префикса пути → должны стать `../audit/M[3-6]_CLOSURE_REVIEW.md`
  - Также присутствуют ссылки `M7_CLOSURE_REVIEW.md` (на ещё не созданный файл — он создаётся в M7-closure session) → должны стать `../audit/M7_CLOSURE_REVIEW.md` (forward reference, корректный)
  
- В `docs/audit/AUDIT_CAMPAIGN_PLAN.md`: ссылки вида `../M[3-6]_CLOSURE_REVIEW.md` → должны стать `./M[3-6]_CLOSURE_REVIEW.md`
  - Также ссылки на `../TRANSLATION_PLAN.md`, `../NORMALIZATION_REPORT.md`, `../METHODOLOGY.md`, `../MOD_OS_ARCHITECTURE.md`, `../ROADMAP.md` — НЕ ТРОГАТЬ, они указывают на файлы в `docs/`, которые там и остались

**Категории контекста для каждой ссылки:**

1. **Inline markdown link** — `[M6_CLOSURE_REVIEW](./M6_CLOSURE_REVIEW.md)` или `[M6 closure review §10](./M6_CLOSURE_REVIEW.md#10-...)`
2. **Bare path mention** — `см. M6_CLOSURE_REVIEW.md` или `parallel to M5_CLOSURE_REVIEW`
3. **Code-fence или backtick** — `` `M6_CLOSURE_REVIEW.md` `` в техническом контексте

Все три категории требуют замены пути; backtick/code-fence сохраняется.

**Verification command (post-fix):**

```bash
# Должен вернуть 0 строк после правки:
grep -rnE '\./M[3-7]_CLOSURE_REVIEW\.md|(?<![./])M[3-7]_CLOSURE_REVIEW\.md' docs/prompts/

# Должен вернуть 0 строк после правки:
grep -rnE '\.\./M[3-7]_CLOSURE_REVIEW\.md' docs/audit/

# Должен вернуть всё ещё валидные ссылки:
grep -rnE '\.\./audit/M[3-7]_CLOSURE_REVIEW\.md' docs/prompts/  # >0 матчей
grep -rnE '\./M[3-7]_CLOSURE_REVIEW\.md' docs/audit/             # >0 матчей
```

## Approved architectural decisions (do not deviate)

1. **Single atomic commit.** Это не feat/test/docs трио, а одна mechanical docs-правка. Один коммит с префиксом `docs(audit-fix)`.

2. **Scope boundary.** Только относительные пути на closure-review файлы M3–M7. НЕ трогать:
   - Ссылки на `MOD_OS_ARCHITECTURE.md`, `ROADMAP.md`, `METHODOLOGY.md`, `CODING_STANDARDS.md`, `TRANSLATION_PLAN.md`, `NORMALIZATION_REPORT.md` — они в `docs/`, ссылки в `docs/audit/AUDIT_CAMPAIGN_PLAN.md` корректны как `../FILE.md`
   - Прозу с упоминанием «M6 closure» без markdown-ссылки и без `.md` суффикса
   - Блоки кода с примерами grep, регулярками, тестовыми путями где `M6_CLOSURE_REVIEW.md` появляется как иллюстрация

3. **Forward references valid.** `M7_CLOSURE_REVIEW.md` упоминается до его создания (он будет создан в M7-closure session post-TD). Ссылки `../audit/M7_CLOSURE_REVIEW.md` валидны как forward references — не делать их битыми, не удалять.

4. **No changes outside the 14 files.** Никаких правок в `docs/architecture/MOD_OS_ARCHITECTURE.md`, `docs/ROADMAP.md`, `docs/methodology/METHODOLOGY.md`, исходниках, тестах. M-phase boundary preserved.

5. **No tests, no build verification.** Это pure docs sweep. После коммита `dotnet build && dotnet test` не нужны (ничего не изменилось в коде). Verification — только grep-команды выше.

## Out of scope

- Любая правка вне 14 перечисленных файлов
- Создание `M7_CLOSURE_REVIEW.md` (это работа M7-closure session, которая придёт после)
- Правка ссылок на любые файлы кроме closure-review M3–M7
- Cosmetic правки (typos, phrasing, formatting) — TD-1 строго xrefs, ничего больше
- Нормализация прозаических упоминаний без `.md` суффикса
- Любая работа с `learning/`, `SESSION_*.md`, корневыми файлами

## Required reading для Gemma (в её промпте перечислить точные пути)

Gemma должна прочитать файлы пакетом через `read_multiple_files` (минимум batch 5 за раз), чтобы не терять контекст между правками.

**Batch 1 (housekeeping):**
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\HOUSEKEEPING_MENU_PAUSES_SIMULATION.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\HOUSEKEEPING_NEEDS_DECAY_DIRECTION.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\HOUSEKEEPING_REAL_PAWN_DATA.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\HOUSEKEEPING_TICKSCHEDULER_RACE.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\HOUSEKEEPING_UI_HONESTY_PASS.md`

**Batch 2 (M73-M75 + M7):**
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\M73_CODING_STANDARDS_UPDATE.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\M74_BUILD_PIPELINE_OVERRIDE.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\M75A_MOD_MENU_CONTROLLER.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\M75B1_BOOTSTRAP_INTEGRATION.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\M75B2_GODOT_UI_SCENE.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\M7_CLOSURE.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\prompts\M7_HOUSEKEEPING_TICK_DISPLAY.md`
- `D:\Colony_Simulator\Colony_Simulator\docs\audit\AUDIT_CAMPAIGN_PLAN.md`

## Acceptance criteria для Gemma

1. Каждый из 14 файлов прочитан и обновлён только если содержит целевые xrefs (если файл чист — оставить нетронутым).
2. Verification commands возвращают ожидаемые результаты:
   - `grep -rnE '\./M[3-7]_CLOSURE_REVIEW\.md' docs/prompts/` → 0 строк
   - `grep -rnE 'M[3-7]_CLOSURE_REVIEW\.md' docs/prompts/` без префикса `../audit/` → 0 строк
   - `grep -rnE '\.\./M[3-7]_CLOSURE_REVIEW\.md' docs/audit/` → 0 строк
   - `grep -rnE '\.\./audit/M[3-7]_CLOSURE_REVIEW\.md' docs/prompts/` → > 0 строк (правки применились)
   - `grep -rnE '\./M[3-7]_CLOSURE_REVIEW\.md' docs/audit/` → > 0 строк (правки применились)
3. Файлы вне 14 целевых не модифицированы — `git status` показывает изменения только в `docs/prompts/*.md` и `docs/audit/AUDIT_CAMPAIGN_PLAN.md`.
4. Один атомарный коммит с сообщением:
   ```
   docs(audit-fix): repair closure-review xrefs after docs/audit/ relocation

   M7 closure session moved M3-M6 closure-review files from docs/ into
   docs/audit/. References in docs/prompts/*.md and
   docs/audit/AUDIT_CAMPAIGN_PLAN.md were not updated atomically and
   pointed to old paths. This commit repairs the cross-references
   mechanically:
   - In docs/prompts/*.md: ./M[N]_CLOSURE_REVIEW.md → ../audit/M[N]_CLOSURE_REVIEW.md
   - In docs/audit/AUDIT_CAMPAIGN_PLAN.md: ../M[N]_CLOSURE_REVIEW.md → ./M[N]_CLOSURE_REVIEW.md
   No changes to spec, code, tests. M-phase boundary preserved.
   ```

## Финал — что Sonnet выдаёт на выходе

**Один файл: `D:\Colony_Simulator\Colony_Simulator\docs\prompts\TD1_BROKEN_XREFS_SWEEP.md`**

Структура промпта (Gemma-стиль, проверенный на M7-цикле):

1. **Operating principle** (1-2 предложения, что это mechanical sweep, не creative work)
2. **Context** (3-4 предложения, что произошло, без воды)
3. **Out of scope** (явный перечень — не трогать spec, не трогать код, не трогать другие relative paths)
4. **Approved architectural decisions** (5 пунктов, копируешь из этого brief)
5. **Required reading** (14 путей, разбитые на 2 batch вызова `read_multiple_files`)
6. **Implementation** — пошаговая инструкция:
   - Шаг 1: прочитать batch 1 (6 файлов)
   - Шаг 2: для каждого файла найти все матчи на регэкспы и применить замены
   - Шаг 3: прочитать batch 2 (8 файлов, включая AUDIT_CAMPAIGN_PLAN)
   - Шаг 4: применить замены, особое внимание AUDIT_CAMPAIGN_PLAN (другая семантика)
   - Шаг 5: запустить 5 verification grep-команд, убедиться в expected counts
   - Шаг 6: `git status` показывает только 14 файлов (или меньше, если часть уже была чиста)
   - Шаг 7: `git diff` визуальная проверка — никаких правок вне xrefs
7. **Acceptance criteria** (5 пунктов, копируешь из этого brief)
8. **Финал** — точная команда коммита с message body
9. **Hypothesis-falsification clause** — это housekeeping, не M-cycle phase, datapoint sequence не инкрементируется. Но если surfaces что-то неожиданное (например, в одном из файлов окажется ссылка с другим паттерном) — STOP, эскалировать обратно к Crystalka, не импровизировать.
10. **Report-back format** — список (commit SHA, число файлов модифицированных, выводы 5 verification grep команд, любые unexpected findings)

## Methodological notes (для Sonnet, не передавать Gemma)

- Это первый TD post-M7. Если этот pass отработает чисто — это нулевая точка для последующих TD-2/3 заданий, validation того что pipeline-discipline работает на mechanical scope.
- Gemma промпт должен быть **строго механическим**. Если ты обнаруживаешь, что какой-то xref требует contextual judgment (например, неоднозначность, надо ли менять путь в блоке кода-примера), ЭСКАЛИРУЙ это обратно в Opus в формате: «Section X разрешает только regex-замену; обнаружен случай Y требующий judgment; STOP, awaiting decision». Не пиши Gemma промпт с «use your judgment» в этом месте.
- Один атомарный коммит — это deviation от трёх-коммитного инварианта METHODOLOGY §7.3. Это правомерно, потому что docs-only mechanical sweep не имеет fix/test/docs декомпозиции (нет fix-в-коде и нет тестов для запуска). Зафиксируй это в Gemma промпте явно как «AD #1: Single atomic commit per nature of the change», с reference на METHODOLOGY §7.3 spirit (а не letter).
- Если найдёшь, что какой-то из 14 файлов уже не существует (был удалён в M7 closure session, например) — STOP, эскалировать. Не импровизируй обходной путь.

## Report-back to Opus

Когда Gemma промпт готов и записан в `TD1_BROKEN_XREFS_SWEEP.md`, отчитайся обратно с:
- путь к созданному промпту
- размер промпта (примерное число строк / character count для context budgeting в Cline)
- любые ambiguities обнаруженные в этом brief, которые Gemma промпт обходит явно
- отметка readiness — Gemma промпт self-contained и copy-paste ready

После твоего отчёта Crystalka копирует Gemma промпт в Cline, Gemma выполняет, Opus делает QA review результата.

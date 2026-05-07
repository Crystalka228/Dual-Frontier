# Native Migration — Progress Tracker

**Status**: LIVE document (не LOCKED) — обновляется при каждом milestone closure
**Created**: 2026-05-07
**Last updated**: 2026-05-07 (K1 closure)
**Scope**: Tracks combined K-series (kernel) + M9-series (runtime) migration progression
**Companion documents**: `KERNEL_ARCHITECTURE.md` (LOCKED v1.0), `RUNTIME_ARCHITECTURE.md` (LOCKED v1.0), `CPP_KERNEL_BRANCH_REPORT.md` (Discovery, reference), `GPU_COMPUTE.md` (Phase 5 research, Lvl 1 pattern applies — см. D3)

---

## Purpose

Этот документ — единая точка наблюдения за продвижением миграции на native foundation (C++ ECS kernel + Vulkan runtime). НЕ является архитектурным документом — архитектура зафиксирована в `KERNEL_ARCHITECTURE.md` и `RUNTIME_ARCHITECTURE.md` как LOCKED v1.0.

**Что фиксируется здесь**:
- Status каждого milestone (not started / in progress / done / decision-pending / blocked)
- Commit hash + дата закрытия
- Текущие блокеры (если есть)
- Решения, принятые в процессе исполнения (вне рамок LOCKED архитектуры)
- Open questions, которые проявились в ходе работы

**Что НЕ фиксируется здесь** (см. соответствующие документы):
- Архитектурные решения K-L1..K-L10, L1..L10 — `KERNEL_ARCHITECTURE.md` Part 0, `RUNTIME_ARCHITECTURE.md` Part 0
- Детальные milestone briefs — `tools/briefs/`
- Discovery findings от experimental branch — `CPP_KERNEL_BRANCH_REPORT.md`
- Methodology adjustments — `METHODOLOGY.md`

---

## Current state snapshot

| | Value |
|---|---|
| **Active phase** | K2 (planned) — type-id registry + bridge tests |
| **Last completed milestone** | K1 (batching primitive) — `e2c50b8` 2026-05-07 |
| **Next milestone (recommended)** | K2 (type-id registry + bridge tests) |
| **Sequencing strategy** | Open — decision deferred к after K2 measurement (см. §«Sequencing decision») |
| **Combined estimate** | 9-15 weeks (5-8 kernel + 4-7 runtime) |
| **Tests passing** | 472 (managed Domain) — preserved invariant throughout migration |

---

## Sequencing decision

**Status**: OPEN, deferred к after K2

Три валидных опции зафиксированы в `KERNEL_ARCHITECTURE.md` Part 8:

| Option | Sequence | Total time | Trade-off |
|---|---|---|---|
| **β5 — kernel fast-track** | K0–K2 → M9.0–M9.8 → K3–K8 | 10–16w | Validates bridge early, then visible runtime progress, kernel completion last |
| **β6 — kernel-first sequential** | K0–K8 → M9.0–M9.8 | 10–15w | Single architectural focus per period, max cleanness, no visible game progress for 5–8w |
| **β3 — interleaved** | K0 → M9.0–M9.5 → K1–K2 → M9.6–M9.8 → K3–K8 | 11–15w | Earlier visible progress, но context-switching между native стеками |

**Recommendation per Crystalka philosophy «cleanness > expediency»**: β5 или β6 over β3.

**Decision trigger**: K2 closure provides bridge-maturity evidence. Если бридж стабилен и P/Invoke patterns подтверждены — β5 valid (kernel pause + runtime sprint). Если K2 показал необходимость более глубокой kernel work до runtime integration — β6.

**Decision will be recorded в этом документе при K2 closure**.

---

## K-series progress (kernel)

### Overview

| Milestone | Title | Status | Estimate | Commit | Date closed |
|---|---|---|---|---|---|
| K0 | Cherry-pick + cleanup от experimental branch | DONE | 1–2 days | `89a4b24` | 2026-05-07 |
| K1 | Batching primitive (bulk Add/Get + Span<T>) | DONE | 3–5 days | `e2c50b8` | 2026-05-07 |
| K2 | Type-id registry + bridge tests | NOT STARTED | 2–3 days | — | — |
| K3 | Native bootstrap graph + thread pool | NOT STARTED | 5–7 days | — | — |
| K4 | Component struct refactor (Path α) | NOT STARTED | 2–3 weeks | — | — |
| K5 | Span<T> protocol + write command batching | NOT STARTED | 1 week | — | — |
| K6 | Second-graph rebuild on mod change | NOT STARTED | 3–5 days | — | — |
| K7 | Performance measurement (tick-loop) | NOT STARTED | 3–5 days | — | — |
| K8 | Decision step + production cutover | NOT STARTED | 1 week | — | — |

**Cumulative estimate**: 5–8 weeks at hobby pace (~1h/day).

### K0 — Cherry-pick + cleanup от experimental branch

- **Status**: DONE (`89a4b24`, 2026-05-07)
- **Brief**: `tools/briefs/K0_CHERRY_PICK_BRIEF.md` (FULL EXECUTED)
- **Source branch**: `claude/cpp-core-experiment-cEsyH` (HEAD `e2bc2d9`)
- **Cherry-picks completed**: 7 commits — `7b5cf78`, `a8d235e`, `cf0eed3`, `6eac732`, `80178c2`, `f59492a`, `e2bc2d9`
- **Cleanup commits**: 3 — vscode portable path (`1236827`), NATIVE_CORE_EXPERIMENT superseded marker (`adc640e`), SparseSet retention annotation (`89a4b24`)
- **Native selftest**: ALL PASSED (Release x64 build)
- **Managed tests**: 472 passing (preserved baseline: 7 + 4 + 38 + 76 + 347)
- **Lessons learned**:
  - Brief's pre-flight check «verify disjoint histories» was incorrect: experimental branch was rooted off main at `3e9a001` («Phase 3 complete»). Histories share a base, but cherry-pick algorithm is unaffected by merge-base relationship — procedure works identically. Future briefs should not gate on disjoint-history assumption.
  - Brief expected `.sln` + `Core.csproj` conflicts to arise on cherry-pick 4 (Interop project), but they actually arose on cherry-pick 5 (NativeVsManaged benchmark) where the wiring was added. End-state identical, but documentation granularity was off-by-one.
  - Brief expected `entity_id.h` to arrive in cherry-pick 3, but it actually came in cherry-pick 2 (SparseSet template). Minor documentation drift, no procedural impact.
  - `BenchmarkDotNet.Artifacts` files committed in pick 7 had to be deleted via second amend after pick 6 amend already removed them — modify/delete conflict resolved trivially in favor of delete (gitignored).
  - `NATIVE_CORE.md` was absent on this branch (only `NATIVE_CORE_EXPERIMENT.md` existed); brief's defensive Test-Path check correctly handled this.
- **Blockers (resolved)**: none unresolved

### K1 — Batching primitive (bulk Add/Get + Span<T>)

- **Status**: DONE (`e2c50b8`, 2026-05-07)
- **Brief**: `tools/briefs/K1_BATCHING_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 4 new functions — `df_world_add_components_bulk`, `df_world_get_components_bulk`, `df_world_acquire_span`, `df_world_release_span` (12 → 16 total)
- **Native side**: `active_spans_` atomic counter в `World`; mutation rejection при active spans (`std::logic_error` thrown from `add_component` / `remove_component` / `destroy_entity` / `flush_destroyed`, caught at C ABI boundary so ABI stays noexcept-equivalent); `dense_data()` accessor в `RawComponentStore`
- **Managed bridge**: `NativeWorld.AddComponents` / `GetComponents` / `AcquireSpan`; `SpanLease<T>` skeleton с `ReadOnlySpan<T>` + `ReadOnlySpan<int>` access
- **Selftest scenarios**: 4 → 6 (added `scenario_bulk_operations`, `scenario_span_lifetime`)
- **Benchmark**: `NativeBulkAddBenchmark` added (execution deferred к K7)
- **Managed tests**: 472 passing (preserved baseline)
- **Lessons learned**:
  - Brief Step 2.5 specified try/catch only on the new bulk/span ABI functions, but the new throw paths added to `add_component` / `remove_component` / `destroy_entity` / `flush_destroyed` propagate through their existing capi wrappers. The wrappers without try/catch would have leaked C++ exceptions across the DLL boundary (UB). Wrapped them defensively — completeness требование, что brief implicitly предполагал но not stated.
  - Pre-flight HG-1 (working tree clean) failed because the brief itself had been authored as an unstaged modification on `main` (skeleton → 1223-line full brief). Resolved by committing «brief authoring» on `main` as a prerequisite step (`8fee2b1`) before creating the K1 branch. Future briefs in similar self-bootstrapping scenarios should call out this pattern explicitly.

### K2 — K8

Detailed entries будут добавлены при подходе к каждому milestone.

---

## M9-series progress (runtime)

### Overview

| Milestone | Title | Status | Estimate | Commit | Date closed |
|---|---|---|---|---|---|
| M9.0 | Win32 window + Vulkan instance + clear color | NOT STARTED | 4–5 days | — | — |
| M9.1 | Textured quad pipeline (single sprite) | NOT STARTED | 3–4 days | — | — |
| M9.2 | Batched sprite renderer (instancing) | NOT STARTED | 4–5 days | — | — |
| M9.3 | TileMap parity (M8.9 visual reproduction) | NOT STARTED | 4–6 days | — | — |
| M9.4 | Input layer (Win32 message pump → IInputAdapter) | NOT STARTED | 3–4 days | — | — |
| M9.5 | Domain integration (M8.9 full parity, switchable backend) | NOT STARTED | 1 week | — | — |
| M9.6 | UI primitives (text rendering, panels) | NOT STARTED | 1–2 weeks | — | — |
| M9.7 | Coupled lifecycle (pause/focus parity с Godot M8.10) | NOT STARTED | 2–3 days | — | — |
| M9.8 | Godot deletion (point of no return для runtime) | NOT STARTED | 2–3 days | — | — |

**Cumulative estimate**: 4–7 weeks at hobby pace.

### M9.0 — M9.8

Detailed entries будут добавлены при подходе к каждому milestone.

---

## Cross-series coupling

Зависимости и взаимовлияния между K и M series, которые могут проявиться в ходе исполнения:

| K milestone | M milestone | Potential coupling |
|---|---|---|
| K4 (struct refactor) | M9.5 (Domain integration) | Если M9.5 идёт раньше K4, Domain integration работает с class-based components. После K4 — adjustment может потребоваться. |
| K5 (span protocol) | None direct | Self-contained в kernel layer |
| K8 (cutover) | M9.5 (Domain integration) | Если K8 Outcome 1 (native wins) до M9.5 — Domain integration сразу нацеливается на NativeWorld. Иначе — на managed World с возможным позднейшим switchover. |
| K7 (perf measurement) | None direct | Информирует K8 decision, не блокирует runtime |
| Все K | M9.8 (Godot deletion) | Godot deletion это точка невозврата ТОЛЬКО для runtime. Kernel decision (K8) независим — managed World остаётся fallback. |

**Invariant**: Kernel и runtime — два независимых стека под managed Application layer. Координация требуется только при cutover-точках (K8, M9.5, M9.8).

---

## Decisions log (operational, не архитектурные)

Решения, принятые в ходе исполнения миграции. Архитектурные LOCKED-решения см. в `KERNEL_ARCHITECTURE.md` Part 0 и `RUNTIME_ARCHITECTURE.md` Part 0.

### D1 — Single progress tracker для K и M
- **Date**: 2026-05-07
- **Decision**: Один файл `MIGRATION_PROGRESS.md`, не разделение на KERNEL_PROGRESS + RUNTIME_PROGRESS
- **Rationale**: sequencing β3/β5/β6 — это решение про взаимосвязь стеков. Раздельные файлы потеряют это видение. Cross-series coupling table требует обоих списков рядом.
- **Reversal trigger**: если документ превысит ~50KB или K и M прогресс начнут существенно расходиться по темпу — рассмотреть split.

### D2 — Sequencing decision deferred к after K2
- **Date**: 2026-05-07
- **Decision**: β3/β5/β6 не выбирается заранее. K0–K2 выполняются first как preservation + bridge maturity (общий шаг для всех вариантов).
- **Rationale**: K2 closure даёт evidence о зрелости bridge layer. Choice без evidence — speculation.
- **Recording trigger**: при K2 closure — фиксируется выбор β-variant в этом документе.

### D3 — Native organicity Lvl 1 как foundational pattern для всего native слоя
- **Date**: 2026-05-07
- **Scope**: Не разовое решение для kernel + runtime, а **архитектурный паттерн проекта** для любого native артефакта — текущего или будущего.

#### Decision

Каждый native артефакт в Dual Frontier — **независимый проект**:
- Отдельный `.dll` (свой build target)
- Отдельный CMake build
- Отдельный узкий C ABI
- Отдельный Interop bridge на managed стороне
- Свой selftest, свои тесты
- Не знает о существовании других native артефактов

**Никакого shared native code между артефактами**: общий thread pool, общий allocator, общий logger, единый монолитный DLL — всё это **отвергается** как foundational rule.

**Координация артефактов** происходит **только** на managed стороне — через interfaces (`IProjectileCompute`, `INativeWorld`, `IRenderer` и т.д.) с DI-регистрацией. Domain не знает какой backend активен.

#### Артефакты, к которым применяется паттерн

**Подтверждённые** (зафиксированы в LOCKED architectural docs):
- `DualFrontier.Core.Native.dll` — ECS kernel (KERNEL_ARCHITECTURE.md)
- `DualFrontier.Runtime.Native.dll` — Vulkan rendering (RUNTIME_ARCHITECTURE.md)

**Запланированные** (зафиксированы в research/roadmap docs):
- GPU Compute pipeline для `ProjectileSystem` — Phase 5 «Battle of the Gods» threshold (GPU_COMPUTE.md). Уже спроектирован через `IProjectileCompute` interface — готовый к Lvl 1 паттерну без переделки.

**Потенциальные** (могут возникнуть в будущем):
- Audio engine, если решит уйти от managed
- AI inference (нейросети для пешек/животных), если потребует native
- Networking layer для multiplayer
- Physics engine, если custom потребуется
- Любой другой compute-heavy domain

Каждый из них автоматически получает Lvl 1 контракт — без необходимости заново открывать архитектурную дискуссию.

#### Rationale

1. **Consistency с операционным принципом проекта «никто никуда не лезет»**:
   - Mods isolated через AssemblyLoadContext + IModApi
   - Domain isolated от Presentation через PresentationBridge
   - Systems isolated через SystemExecutionContext (crash в DEBUG при undeclared access)
   - Native↔Managed isolated через single ownership boundary
   - Native↔Native теперь следует тому же правилу — паттерн распространяется тотально

2. **Open-source-separately property сохраняется per-артефакт**: kernel может стать standalone «sparse-set ECS in C++». Runtime — standalone «2D Vulkan runtime». Compute — standalone «Vulkan compute pipeline для projectile-style workloads». Каждый artifact имеет ценность независимо.

3. **Independent failure domains**: bug в compute pipeline не затрагивает kernel storage. Crash в Vulkan не убивает ECS. Отладка локализована per-артефакт.

4. **Independent build/test cycles**: при работе над kernel не нужно компилировать runtime. CTest selftest каждого артефакта standalone. Pipeline metrics локальны.

5. **Mod-friendly extensibility**: моды могут зарегистрировать свой `IProjectileCompute` или другую compute backend через тот же DI mechanism. Lvl 2/Lvl 3 это бы заблокировали — мод не может встроиться в shared internals.

6. **Use-case flexibility**:
   - Headless dedicated server: kernel + compute, без runtime → работает (Lvl 3 невозможно)
   - Migration period: kernel + Godot, без native runtime → работает (Lvl 2 ломает)
   - Compute disabled на слабом железе: kernel + runtime, без compute → работает
   - Любая комбинация артефактов валидна

#### Rejected alternatives

- **Lvl 2** (shared native infrastructure — общий thread pool, allocator, logger между артефактами): premature optimization. Артефакты имеют фундаментально разные work characteristics (kernel thread pool idle после bootstrap, runtime Vulkan thread per-frame, compute thread в момент dispatch). Shared infrastructure создаст coupling без выигрыша.
- **Lvl 3** (один монолитный native DLL): ломает open-source-separately, ломает headless server use-case, ломает mod extensibility, нарушает conceptual integrity (ECS storage + Vulkan rendering + compute pipeline не имеют общего concern).

#### Reversal trigger

**Per-артефакт основание требуется**, не глобальное. Конкретный pair артефактов (например, runtime + compute) может быть рассмотрен на shared infrastructure **только при наличии всех трёх условий**:

1. Profiling на weak hardware показывает измеримую боль (thread oversubscription, allocator fragmentation, etc.)
2. Боль не решается оптимизацией внутри артефакта
3. Прошло минимум 12 месяцев с production использования обоих артефактов (evidence base достаточен)

Decision на shared infrastructure требует **отдельного architectural milestone** с amendment к LOCKED docs обоих артефактов и явным трейдоффом против выгод Lvl 1 паттерна.

#### Implication для cross-series coupling table

Остаётся как есть — координация K-серии и M-серии только в cutover-точках (K8, M9.5, M9.8). Никаких shared native artifacts. При появлении третьего артефакта (например, GPU Compute) — добавляется свой столбец без изменения существующих coupling правил.

#### Implication для open questions

OQ3 («Cross-document drift между KERNEL_ARCHITECTURE и RUNTIME_ARCHITECTURE») расширяется: при добавлении третьего native артефакта (например, COMPUTE_ARCHITECTURE.md) — drift-prevention требуется тройной cross-reference. Currently не активно (compute artifact не существует), но при подходе к Phase 5 — переоткрыть.

---

## Open questions

Вопросы, которые проявились в процессе планирования но решение которых отложено к моменту, когда появятся данные.

### OQ1 — Branch strategy для миграции
- **Question**: Один long-lived `feature/native-kernel` branch для всей K-series, или per-K feature branches с merge на main?
- **Trigger to resolve**: до K0 execution (требуется в K0 brief)
- **Lean**: per-K feature branches + atomic merge на main per closure (matches existing M8.x pattern)

### OQ2 — Rollback policy для K4 (struct refactor)
- **Question**: K4 это 2-3 weeks scope с ~50-80 component conversions. Если в середине обнаружится фундаментальная проблема (например, behavior class split не работает для X компонентов) — есть ли клин-rollback?
- **Trigger to resolve**: при K4 brief authoring
- **Lean**: incremental merge per 5-10 components (each commit independent), revert simple

### OQ3 — Cross-document drift между KERNEL_ARCHITECTURE и RUNTIME_ARCHITECTURE
- **Question**: Два LOCKED v1.0 документа эволюционируют параллельно. Как избежать дрейфа decisions (например, K-L4 explicit registry vs L-аналог в RUNTIME)?
- **Trigger to resolve**: при первом обнаружении конфликта
- **Lean**: при amendment одного документа — обязательная проверка cross-references в другом

### OQ4 — Что если K7 показал, что native не нужен?
- **Question**: K8 Outcome 2/3 означают что native kernel parking. Что делать с уже выполненными K0-K7 (особенно K3 native scheduler, K5 span protocol)?
- **Trigger to resolve**: после K7 measurements
- **Lean**: K8 brief предусматривает три outcome-варианта; recording rationale + lessons learned обязательны

---

## Closure protocol

Когда milestone закрывается:

1. **Run final verification**: `dotnet build`, `dotnet test`, native selftest (для K-series), F5 verification (для M-series)
2. **Atomic commit** с scope prefix (`feat`/`fix`/`refactor`/`test`/`docs`/`native`/`interop`)
3. **Update этот документ**:
   - Status: NOT STARTED → DONE
   - Commit hash + date filled
   - Add «Lessons learned» entry если что-то нетривиальное обнаружилось
   - Update `Current state snapshot` table
   - Если milestone разрешил OQ — переместить из «Open questions» в «Decisions log»
4. **Update brief**: mark brief as EXECUTED + add link к commit в `tools/briefs/K{N}_*.md`

**Pre-flight для самого update**: pre-flight grep на «NOT STARTED» / «IN PROGRESS» — убедиться что только один milestone в IN PROGRESS одновременно.

---

## Document end

Single point of view на 9-15 weeks of work. Обновляется per milestone closure. Архитектурные решения остаются LOCKED в companion documents.
